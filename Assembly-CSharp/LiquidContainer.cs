using System;
using System.Collections.Generic;
using ConVar;
using Facepunch;
using Network;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x02000090 RID: 144
public class LiquidContainer : ContainerIOEntity
{
	// Token: 0x04000888 RID: 2184
	public ItemDefinition defaultLiquid;

	// Token: 0x04000889 RID: 2185
	public int startingAmount;

	// Token: 0x0400088A RID: 2186
	public bool autofillOutputs;

	// Token: 0x0400088B RID: 2187
	public float autofillTickRate = 2f;

	// Token: 0x0400088C RID: 2188
	public int autofillTickAmount = 2;

	// Token: 0x0400088D RID: 2189
	public int maxOutputFlow = 6;

	// Token: 0x0400088E RID: 2190
	public ItemDefinition[] ValidItems;

	// Token: 0x0400088F RID: 2191
	private int currentDrainAmount;

	// Token: 0x04000890 RID: 2192
	private HashSet<IOEntity> connectedList = new HashSet<IOEntity>();

	// Token: 0x04000891 RID: 2193
	private HashSet<ContainerIOEntity> pushTargets = new HashSet<ContainerIOEntity>();

	// Token: 0x04000892 RID: 2194
	private const int maxPushTargets = 3;

	// Token: 0x04000893 RID: 2195
	private IOEntity considerConnectedTo;

	// Token: 0x04000894 RID: 2196
	private Action updateDrainAmountAction;

	// Token: 0x04000895 RID: 2197
	private Action updatePushLiquidTargetsAction;

	// Token: 0x04000896 RID: 2198
	private Action pushLiquidAction;

	// Token: 0x04000897 RID: 2199
	private Action deductFuelAction;

	// Token: 0x04000898 RID: 2200
	private TimeUntil waterTransferStartTime;

	// Token: 0x04000899 RID: 2201
	private float lastOutputDrainUpdate;

	// Token: 0x06000D58 RID: 3416 RVA: 0x00071FBC File Offset: 0x000701BC
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("LiquidContainer.OnRpcMessage", 0))
		{
			if (rpc == 2002733690U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - SVDrink ");
				}
				using (TimeWarning.New("SVDrink", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.MaxDistance.Test(2002733690U, "SVDrink", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							BaseEntity.RPCMessage rpc2 = new BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.SVDrink(rpc2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in SVDrink");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x1700013B RID: 315
	// (get) Token: 0x06000D59 RID: 3417 RVA: 0x0000441C File Offset: 0x0000261C
	public override bool IsGravitySource
	{
		get
		{
			return true;
		}
	}

	// Token: 0x1700013C RID: 316
	// (get) Token: 0x06000D5A RID: 3418 RVA: 0x00072124 File Offset: 0x00070324
	protected override bool DisregardGravityRestrictionsOnLiquid
	{
		get
		{
			return base.HasFlag(BaseEntity.Flags.Reserved8) || base.DisregardGravityRestrictionsOnLiquid;
		}
	}

	// Token: 0x06000D5B RID: 3419 RVA: 0x0000441C File Offset: 0x0000261C
	public override bool IsRootEntity()
	{
		return true;
	}

	// Token: 0x06000D5C RID: 3420 RVA: 0x0007213C File Offset: 0x0007033C
	private bool CanAcceptItem(Item item, int count)
	{
		if (this.ValidItems == null || this.ValidItems.Length == 0)
		{
			return true;
		}
		ItemDefinition[] validItems = this.ValidItems;
		for (int i = 0; i < validItems.Length; i++)
		{
			if (validItems[i] == item.info)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06000D5D RID: 3421 RVA: 0x00072184 File Offset: 0x00070384
	public override void ServerInit()
	{
		this.updateDrainAmountAction = new Action(this.UpdateDrainAmount);
		this.pushLiquidAction = new Action(this.PushLiquidThroughOutputs);
		this.deductFuelAction = new Action(this.DeductFuel);
		this.updatePushLiquidTargetsAction = new Action(this.UpdatePushLiquidTargets);
		base.ServerInit();
		if (this.startingAmount > 0)
		{
			base.inventory.AddItem(this.defaultLiquid, this.startingAmount, 0UL, ItemContainer.LimitStack.Existing);
		}
		if (this.autofillOutputs && this.HasLiquidItem())
		{
			this.UpdatePushLiquidTargets();
		}
		ItemContainer inventory = base.inventory;
		inventory.canAcceptItem = (Func<Item, int, bool>)Delegate.Combine(inventory.canAcceptItem, new Func<Item, int, bool>(this.CanAcceptItem));
	}

	// Token: 0x06000D5E RID: 3422 RVA: 0x00072240 File Offset: 0x00070440
	public override void OnCircuitChanged(bool forceUpdate)
	{
		base.OnCircuitChanged(forceUpdate);
		this.ClearDrains();
		base.Invoke(this.updateDrainAmountAction, 0.1f);
		if (this.autofillOutputs && this.HasLiquidItem())
		{
			base.Invoke(this.updatePushLiquidTargetsAction, 0.1f);
		}
	}

	// Token: 0x06000D5F RID: 3423 RVA: 0x0007228C File Offset: 0x0007048C
	public override void OnItemAddedOrRemoved(Item item, bool added)
	{
		base.OnItemAddedOrRemoved(item, added);
		this.UpdateOnFlag();
		base.MarkDirtyForceUpdateOutputs();
		base.Invoke(this.updateDrainAmountAction, 0.1f);
		if (this.connectedList.Count > 0)
		{
			List<IOEntity> list = Facepunch.Pool.GetList<IOEntity>();
			foreach (IOEntity ioentity in this.connectedList)
			{
				if (ioentity != null)
				{
					list.Add(ioentity);
				}
			}
			foreach (IOEntity ioentity2 in list)
			{
				ioentity2.SendChangedToRoot(true);
			}
			Facepunch.Pool.FreeList<IOEntity>(ref list);
		}
		if (this.HasLiquidItem() && this.autofillOutputs)
		{
			base.Invoke(this.updatePushLiquidTargetsAction, 0.1f);
		}
		if (added)
		{
			this.waterTransferStartTime = 10f;
		}
	}

	// Token: 0x06000D60 RID: 3424 RVA: 0x00072398 File Offset: 0x00070598
	private void ClearDrains()
	{
		foreach (IOEntity ioentity in this.connectedList)
		{
			if (ioentity != null)
			{
				ioentity.SetFuelType(null, null);
			}
		}
		this.connectedList.Clear();
	}

	// Token: 0x06000D61 RID: 3425 RVA: 0x00072400 File Offset: 0x00070600
	public override int GetCurrentEnergy()
	{
		return Mathf.Clamp(this.GetLiquidCount(), 0, this.maxOutputFlow);
	}

	// Token: 0x06000D62 RID: 3426 RVA: 0x00072414 File Offset: 0x00070614
	public override int CalculateCurrentEnergy(int inputAmount, int inputSlot)
	{
		if (!this.HasLiquidItem())
		{
			return base.CalculateCurrentEnergy(inputAmount, inputSlot);
		}
		return this.GetCurrentEnergy();
	}

	// Token: 0x06000D63 RID: 3427 RVA: 0x00072430 File Offset: 0x00070630
	private void UpdateDrainAmount()
	{
		int value = 0;
		Item liquidItem = this.GetLiquidItem();
		if (liquidItem != null)
		{
			foreach (IOEntity.IOSlot ioslot in this.outputs)
			{
				if (ioslot.connectedTo.Get(true) != null)
				{
					this.CalculateDrain(ioslot.connectedTo.Get(true), base.transform.TransformPoint(ioslot.handlePosition), IOEntity.backtracking * 2, ref value, this, (liquidItem != null) ? liquidItem.info : null);
				}
			}
		}
		this.currentDrainAmount = Mathf.Clamp(value, 0, this.maxOutputFlow);
		if (this.currentDrainAmount <= 0 && base.IsInvoking(this.deductFuelAction))
		{
			base.CancelInvoke(this.deductFuelAction);
			return;
		}
		if (this.currentDrainAmount > 0 && !base.IsInvoking(this.deductFuelAction))
		{
			base.InvokeRepeating(this.deductFuelAction, 0f, 1f);
		}
	}

	// Token: 0x1700013D RID: 317
	// (get) Token: 0x06000D64 RID: 3428 RVA: 0x0000441C File Offset: 0x0000261C
	public override bool BlockFluidDraining
	{
		get
		{
			return true;
		}
	}

	// Token: 0x06000D65 RID: 3429 RVA: 0x00072518 File Offset: 0x00070718
	private void CalculateDrain(IOEntity ent, Vector3 fromSlotWorld, int depth, ref int amount, IOEntity lastEntity, ItemDefinition waterType)
	{
		if (ent == this || depth <= 0 || ent == null || lastEntity == null)
		{
			return;
		}
		if (ent is LiquidContainer)
		{
			return;
		}
		if (!ent.BlockFluidDraining && ent.HasFlag(BaseEntity.Flags.On))
		{
			int num = ent.DesiredPower();
			amount += num;
			ent.SetFuelType(waterType, this);
			this.connectedList.Add(ent);
		}
		if (ent.AllowLiquidPassthrough(lastEntity, fromSlotWorld, false))
		{
			foreach (IOEntity.IOSlot ioslot in ent.outputs)
			{
				if (ioslot.connectedTo.Get(true) != null && ioslot.connectedTo.Get(true) != ent)
				{
					this.CalculateDrain(ioslot.connectedTo.Get(true), ent.transform.TransformPoint(ioslot.handlePosition), depth - 1, ref amount, ent, waterType);
				}
			}
		}
	}

	// Token: 0x06000D66 RID: 3430 RVA: 0x000725FD File Offset: 0x000707FD
	public override void UpdateOutputs()
	{
		base.UpdateOutputs();
		if (UnityEngine.Time.realtimeSinceStartup - this.lastOutputDrainUpdate < 0.2f)
		{
			return;
		}
		this.lastOutputDrainUpdate = UnityEngine.Time.realtimeSinceStartup;
		this.ClearDrains();
		base.Invoke(this.updateDrainAmountAction, 0.1f);
	}

	// Token: 0x06000D67 RID: 3431 RVA: 0x0007263C File Offset: 0x0007083C
	private void DeductFuel()
	{
		if (this.HasLiquidItem())
		{
			Item liquidItem = this.GetLiquidItem();
			liquidItem.amount -= this.currentDrainAmount;
			liquidItem.MarkDirty();
			if (liquidItem.amount <= 0)
			{
				liquidItem.Remove(0f);
			}
		}
	}

	// Token: 0x06000D68 RID: 3432 RVA: 0x00072685 File Offset: 0x00070885
	protected void UpdateOnFlag()
	{
		base.SetFlag(BaseEntity.Flags.On, base.inventory.itemList.Count > 0 && base.inventory.itemList[0].amount > 0, false, true);
	}

	// Token: 0x06000D69 RID: 3433 RVA: 0x000726BF File Offset: 0x000708BF
	public virtual void OpenTap(float duration)
	{
		if (base.HasFlag(BaseEntity.Flags.Reserved5))
		{
			return;
		}
		base.SetFlag(BaseEntity.Flags.Reserved5, true, false, true);
		base.Invoke(new Action(this.ShutTap), duration);
		base.SendNetworkUpdateImmediate(false);
	}

	// Token: 0x06000D6A RID: 3434 RVA: 0x000726F8 File Offset: 0x000708F8
	public virtual void ShutTap()
	{
		base.SetFlag(BaseEntity.Flags.Reserved5, false, false, true);
		base.SendNetworkUpdateImmediate(false);
	}

	// Token: 0x06000D6B RID: 3435 RVA: 0x0007270F File Offset: 0x0007090F
	public bool HasLiquidItem()
	{
		return this.GetLiquidItem() != null;
	}

	// Token: 0x06000D6C RID: 3436 RVA: 0x0007271A File Offset: 0x0007091A
	public Item GetLiquidItem()
	{
		if (base.inventory.itemList.Count == 0)
		{
			return null;
		}
		return base.inventory.itemList[0];
	}

	// Token: 0x06000D6D RID: 3437 RVA: 0x00072741 File Offset: 0x00070941
	public int GetLiquidCount()
	{
		if (!this.HasLiquidItem())
		{
			return 0;
		}
		return this.GetLiquidItem().amount;
	}

	// Token: 0x06000D6E RID: 3438 RVA: 0x00072758 File Offset: 0x00070958
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.MaxDistance(3f)]
	public void SVDrink(BaseEntity.RPCMessage rpc)
	{
		if (!rpc.player.metabolism.CanConsume())
		{
			return;
		}
		foreach (Item item in base.inventory.itemList)
		{
			ItemModConsume component = item.info.GetComponent<ItemModConsume>();
			if (!(component == null) && component.CanDoAction(item, rpc.player))
			{
				component.DoAction(item, rpc.player);
				break;
			}
		}
	}

	// Token: 0x06000D6F RID: 3439 RVA: 0x000727F0 File Offset: 0x000709F0
	private void UpdatePushLiquidTargets()
	{
		this.pushTargets.Clear();
		if (!this.HasLiquidItem())
		{
			return;
		}
		if (base.IsConnectedTo(this, IOEntity.backtracking * 2, false))
		{
			return;
		}
		Item liquidItem = this.GetLiquidItem();
		using (TimeWarning.New("UpdatePushTargets", 0))
		{
			foreach (IOEntity.IOSlot ioslot in this.outputs)
			{
				if (ioslot.type == IOEntity.IOType.Fluidic)
				{
					IOEntity ioentity = ioslot.connectedTo.Get(true);
					if (ioentity != null)
					{
						this.CheckPushLiquid(ioentity, liquidItem, this, IOEntity.backtracking * 4);
					}
				}
			}
		}
		if (this.pushTargets.Count > 0)
		{
			base.InvokeRandomized(this.pushLiquidAction, 0f, this.autofillTickRate, this.autofillTickRate * 0.2f);
		}
	}

	// Token: 0x06000D70 RID: 3440 RVA: 0x000728D4 File Offset: 0x00070AD4
	private void PushLiquidThroughOutputs()
	{
		if (this.waterTransferStartTime > 0f)
		{
			return;
		}
		if (!this.HasLiquidItem())
		{
			base.CancelInvoke(this.pushLiquidAction);
			return;
		}
		Item liquidItem = this.GetLiquidItem();
		if (this.pushTargets.Count > 0)
		{
			int num = Mathf.Clamp(this.autofillTickAmount, 0, liquidItem.amount) / this.pushTargets.Count;
			if (num == 0 && liquidItem.amount > 0)
			{
				num = liquidItem.amount;
			}
			if (ConVar.Server.waterContainersLeaveWaterBehind && num == liquidItem.amount)
			{
				num--;
			}
			if (num == 0)
			{
				return;
			}
			foreach (ContainerIOEntity containerIOEntity in this.pushTargets)
			{
				if (containerIOEntity.inventory.CanAcceptItem(liquidItem, 0) == ItemContainer.CanAcceptResult.CanAccept && (containerIOEntity.inventory.CanAccept(liquidItem) || containerIOEntity.inventory.FindItemByItemID(liquidItem.info.itemid) != null))
				{
					int num2 = Mathf.Clamp(num, 0, containerIOEntity.inventory.GetMaxTransferAmount(liquidItem.info));
					containerIOEntity.inventory.AddItem(liquidItem.info, num2, 0UL, ItemContainer.LimitStack.Existing);
					liquidItem.amount -= num2;
					liquidItem.MarkDirty();
					if (liquidItem.amount <= 0)
					{
						break;
					}
				}
			}
		}
		if (liquidItem.amount <= 0 || this.pushTargets.Count == 0)
		{
			if (liquidItem.amount <= 0)
			{
				liquidItem.Remove(0f);
			}
			base.CancelInvoke(this.pushLiquidAction);
		}
	}

	// Token: 0x06000D71 RID: 3441 RVA: 0x00072A6C File Offset: 0x00070C6C
	private void CheckPushLiquid(IOEntity connected, Item ourFuel, IOEntity fromSource, int depth)
	{
		if (depth <= 0 || ourFuel.amount <= 0)
		{
			return;
		}
		Vector3 zero = Vector3.zero;
		IOEntity ioentity = connected.FindGravitySource(ref zero, IOEntity.backtracking * 2, true);
		if (ioentity != null && !connected.AllowLiquidPassthrough(ioentity, zero, false))
		{
			return;
		}
		if (connected == this || this.ConsiderConnectedTo(connected))
		{
			return;
		}
		ContainerIOEntity containerIOEntity;
		if ((containerIOEntity = (connected as ContainerIOEntity)) != null && !this.pushTargets.Contains(containerIOEntity) && containerIOEntity.inventory.CanAcceptItem(ourFuel, 0) == ItemContainer.CanAcceptResult.CanAccept)
		{
			this.pushTargets.Add(containerIOEntity);
			return;
		}
		foreach (IOEntity.IOSlot ioslot in connected.outputs)
		{
			IOEntity ioentity2 = ioslot.connectedTo.Get(true);
			Vector3 sourceWorldPosition = connected.transform.TransformPoint(ioslot.handlePosition);
			if (ioentity2 != null && ioentity2 != fromSource && ioentity2.AllowLiquidPassthrough(connected, sourceWorldPosition, false))
			{
				this.CheckPushLiquid(ioentity2, ourFuel, fromSource, depth - 1);
				if (this.pushTargets.Count >= 3)
				{
					return;
				}
			}
		}
	}

	// Token: 0x06000D72 RID: 3442 RVA: 0x00072B7C File Offset: 0x00070D7C
	public void SetConnectedTo(IOEntity entity)
	{
		this.considerConnectedTo = entity;
	}

	// Token: 0x06000D73 RID: 3443 RVA: 0x00072B85 File Offset: 0x00070D85
	protected override bool ConsiderConnectedTo(IOEntity entity)
	{
		return entity == this.considerConnectedTo;
	}
}
