using System;
using System.Collections.Generic;
using Facepunch;
using ProtoBuf;
using UnityEngine;

// Token: 0x02000112 RID: 274
public class ElectricBattery : global::IOEntity, IInstanceDataReceiver
{
	// Token: 0x04000E76 RID: 3702
	public int maxOutput;

	// Token: 0x04000E77 RID: 3703
	public float maxCapactiySeconds;

	// Token: 0x04000E78 RID: 3704
	public float rustWattSeconds;

	// Token: 0x04000E79 RID: 3705
	private int activeDrain;

	// Token: 0x04000E7A RID: 3706
	public bool rechargable;

	// Token: 0x04000E7B RID: 3707
	[Tooltip("How much energy we can request from power sources for charging is this value * our maxOutput")]
	public float maximumInboundEnergyRatio = 4f;

	// Token: 0x04000E7C RID: 3708
	public float chargeRatio = 0.25f;

	// Token: 0x04000E7D RID: 3709
	private const float tickRateSeconds = 1f;

	// Token: 0x04000E7E RID: 3710
	public const global::BaseEntity.Flags Flag_HalfFull = global::BaseEntity.Flags.Reserved5;

	// Token: 0x04000E7F RID: 3711
	public const global::BaseEntity.Flags Flag_VeryFull = global::BaseEntity.Flags.Reserved6;

	// Token: 0x04000E80 RID: 3712
	private bool wasLoaded;

	// Token: 0x04000E81 RID: 3713
	private HashSet<global::IOEntity> connectedList = new HashSet<global::IOEntity>();

	// Token: 0x0600161D RID: 5661 RVA: 0x0000441C File Offset: 0x0000261C
	public override bool IsRootEntity()
	{
		return true;
	}

	// Token: 0x0600161E RID: 5662 RVA: 0x00007A3C File Offset: 0x00005C3C
	public override int ConsumptionAmount()
	{
		return 0;
	}

	// Token: 0x0600161F RID: 5663 RVA: 0x000ACD8C File Offset: 0x000AAF8C
	public override int MaximalPowerOutput()
	{
		return this.maxOutput;
	}

	// Token: 0x06001620 RID: 5664 RVA: 0x000ACD94 File Offset: 0x000AAF94
	public int GetActiveDrain()
	{
		if (!base.IsOn())
		{
			return 0;
		}
		return this.activeDrain;
	}

	// Token: 0x06001621 RID: 5665 RVA: 0x000ACDA6 File Offset: 0x000AAFA6
	public void ReceiveInstanceData(ProtoBuf.Item.InstanceData data)
	{
		this.rustWattSeconds = (float)data.dataInt;
	}

	// Token: 0x06001622 RID: 5666 RVA: 0x000ACDB5 File Offset: 0x000AAFB5
	public override void PostServerLoad()
	{
		base.PostServerLoad();
		this.wasLoaded = true;
	}

	// Token: 0x06001623 RID: 5667 RVA: 0x000ACDC4 File Offset: 0x000AAFC4
	public override void OnPickedUp(global::Item createdItem, global::BasePlayer player)
	{
		base.OnPickedUp(createdItem, player);
		if (createdItem.instanceData == null)
		{
			createdItem.instanceData = new ProtoBuf.Item.InstanceData();
		}
		createdItem.instanceData.ShouldPool = false;
		createdItem.instanceData.dataInt = Mathf.FloorToInt(this.rustWattSeconds);
	}

	// Token: 0x06001624 RID: 5668 RVA: 0x000ACE03 File Offset: 0x000AB003
	public override int GetCurrentEnergy()
	{
		return this.currentEnergy;
	}

	// Token: 0x06001625 RID: 5669 RVA: 0x000ACE0B File Offset: 0x000AB00B
	public override int DesiredPower()
	{
		if (this.rustWattSeconds >= this.maxCapactiySeconds)
		{
			return 0;
		}
		return Mathf.FloorToInt((float)this.maxOutput * this.maximumInboundEnergyRatio);
	}

	// Token: 0x06001626 RID: 5670 RVA: 0x000ACE30 File Offset: 0x000AB030
	public override void SendAdditionalData(global::BasePlayer player, int slot, bool input)
	{
		int passthroughAmountForAnySlot = base.GetPassthroughAmountForAnySlot(slot, input);
		base.ClientRPCPlayer<int, int, float, float>(null, player, "Client_ReceiveAdditionalData", this.currentEnergy, passthroughAmountForAnySlot, this.rustWattSeconds, (float)this.activeDrain);
	}

	// Token: 0x06001627 RID: 5671 RVA: 0x000ACE67 File Offset: 0x000AB067
	public override void ServerInit()
	{
		base.ServerInit();
		base.InvokeRandomized(new Action(this.CheckDischarge), UnityEngine.Random.Range(0f, 1f), 1f, 0.1f);
	}

	// Token: 0x06001628 RID: 5672 RVA: 0x00007A3C File Offset: 0x00005C3C
	public int GetDrainFor(global::IOEntity ent)
	{
		return 0;
	}

	// Token: 0x06001629 RID: 5673 RVA: 0x000ACE9C File Offset: 0x000AB09C
	public void AddConnectedRecursive(global::IOEntity root, ref HashSet<global::IOEntity> listToUse)
	{
		listToUse.Add(root);
		if (root.WantsPassthroughPower())
		{
			for (int i = 0; i < root.outputs.Length; i++)
			{
				if (root.AllowDrainFrom(i))
				{
					global::IOEntity.IOSlot ioslot = root.outputs[i];
					if (ioslot.type == global::IOEntity.IOType.Electric)
					{
						global::IOEntity ioentity = ioslot.connectedTo.Get(true);
						if (ioentity != null)
						{
							bool flag = ioentity.WantsPower();
							if (!listToUse.Contains(ioentity))
							{
								if (flag)
								{
									this.AddConnectedRecursive(ioentity, ref listToUse);
								}
								else
								{
									listToUse.Add(ioentity);
								}
							}
						}
					}
				}
			}
		}
	}

	// Token: 0x0600162A RID: 5674 RVA: 0x000ACF24 File Offset: 0x000AB124
	public int GetDrain()
	{
		this.connectedList.Clear();
		global::IOEntity ioentity = this.outputs[0].connectedTo.Get(true);
		if (ioentity)
		{
			this.AddConnectedRecursive(ioentity, ref this.connectedList);
		}
		int num = 0;
		foreach (global::IOEntity ioentity2 in this.connectedList)
		{
			if (ioentity2.ShouldDrainBattery(this))
			{
				num += ioentity2.DesiredPower();
				if (num >= this.maxOutput)
				{
					num = this.maxOutput;
					break;
				}
			}
		}
		return num;
	}

	// Token: 0x0600162B RID: 5675 RVA: 0x000ACFCC File Offset: 0x000AB1CC
	public override void OnCircuitChanged(bool forceUpdate)
	{
		base.OnCircuitChanged(forceUpdate);
		int drain = this.GetDrain();
		this.activeDrain = drain;
	}

	// Token: 0x0600162C RID: 5676 RVA: 0x000ACFF0 File Offset: 0x000AB1F0
	public void CheckDischarge()
	{
		if (this.rustWattSeconds < 5f)
		{
			this.SetDischarging(false);
			return;
		}
		global::IOEntity ioentity = this.outputs[0].connectedTo.Get(true);
		int drain = this.GetDrain();
		this.activeDrain = drain;
		if (ioentity)
		{
			this.SetDischarging(ioentity.WantsPower());
			return;
		}
		this.SetDischarging(false);
	}

	// Token: 0x0600162D RID: 5677 RVA: 0x000AD050 File Offset: 0x000AB250
	public void SetDischarging(bool wantsOn)
	{
		this.SetPassthroughOn(wantsOn);
	}

	// Token: 0x0600162E RID: 5678 RVA: 0x000AD059 File Offset: 0x000AB259
	public override int GetPassthroughAmount(int outputSlot = 0)
	{
		if (base.IsOn())
		{
			return Mathf.FloorToInt((float)this.maxOutput * ((this.rustWattSeconds >= 1f) ? 1f : 0f));
		}
		return 0;
	}

	// Token: 0x0600162F RID: 5679 RVA: 0x000AD08B File Offset: 0x000AB28B
	public override bool WantsPower()
	{
		return this.rustWattSeconds < this.maxCapactiySeconds;
	}

	// Token: 0x06001630 RID: 5680 RVA: 0x000AD09C File Offset: 0x000AB29C
	public override void IOStateChanged(int inputAmount, int inputSlot)
	{
		base.IOStateChanged(inputAmount, inputSlot);
		if (inputSlot == 0)
		{
			if (!this.IsPowered())
			{
				if (this.rechargable)
				{
					base.CancelInvoke(new Action(this.AddCharge));
					return;
				}
			}
			else if (this.rechargable && !base.IsInvoking(new Action(this.AddCharge)))
			{
				base.InvokeRandomized(new Action(this.AddCharge), 1f, 1f, 0.1f);
			}
		}
	}

	// Token: 0x06001631 RID: 5681 RVA: 0x000AD114 File Offset: 0x000AB314
	public void TickUsage()
	{
		float oldCharge = this.rustWattSeconds;
		bool flag = this.rustWattSeconds > 0f;
		if (this.rustWattSeconds >= 1f)
		{
			float num = 1f * (float)this.activeDrain;
			this.rustWattSeconds -= num;
		}
		if (this.rustWattSeconds <= 0f)
		{
			this.rustWattSeconds = 0f;
		}
		bool flag2 = this.rustWattSeconds > 0f;
		this.ChargeChanged(oldCharge);
		if (flag != flag2)
		{
			this.MarkDirty();
			base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
		}
	}

	// Token: 0x06001632 RID: 5682 RVA: 0x000AD19C File Offset: 0x000AB39C
	public virtual void ChargeChanged(float oldCharge)
	{
		float num = this.rustWattSeconds;
		bool flag = this.rustWattSeconds > this.maxCapactiySeconds * 0.25f;
		bool flag2 = this.rustWattSeconds > this.maxCapactiySeconds * 0.75f;
		if (base.HasFlag(global::BaseEntity.Flags.Reserved5) != flag || base.HasFlag(global::BaseEntity.Flags.Reserved6) != flag2)
		{
			base.SetFlag(global::BaseEntity.Flags.Reserved5, flag, false, false);
			base.SetFlag(global::BaseEntity.Flags.Reserved6, flag2, false, false);
			base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
		}
	}

	// Token: 0x06001633 RID: 5683 RVA: 0x000AD21C File Offset: 0x000AB41C
	public void AddCharge()
	{
		float oldCharge = this.rustWattSeconds;
		float num = (float)Mathf.Min(this.currentEnergy, this.DesiredPower()) * 1f * this.chargeRatio;
		this.rustWattSeconds += num;
		this.rustWattSeconds = Mathf.Clamp(this.rustWattSeconds, 0f, this.maxCapactiySeconds);
		this.ChargeChanged(oldCharge);
	}

	// Token: 0x06001634 RID: 5684 RVA: 0x000AD284 File Offset: 0x000AB484
	public void SetPassthroughOn(bool wantsOn)
	{
		if (wantsOn == base.IsOn() && !this.wasLoaded)
		{
			return;
		}
		this.wasLoaded = false;
		base.SetFlag(global::BaseEntity.Flags.On, wantsOn, false, true);
		if (base.IsOn())
		{
			if (!base.IsInvoking(new Action(this.TickUsage)))
			{
				base.InvokeRandomized(new Action(this.TickUsage), 1f, 1f, 0.1f);
			}
		}
		else
		{
			base.CancelInvoke(new Action(this.TickUsage));
		}
		this.MarkDirty();
	}

	// Token: 0x06001635 RID: 5685 RVA: 0x0006282C File Offset: 0x00060A2C
	public void Unbusy()
	{
		base.SetFlag(global::BaseEntity.Flags.Busy, false, false, true);
	}

	// Token: 0x06001636 RID: 5686 RVA: 0x000AD30B File Offset: 0x000AB50B
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		if (info.msg.ioEntity == null)
		{
			info.msg.ioEntity = Pool.Get<ProtoBuf.IOEntity>();
		}
		info.msg.ioEntity.genericFloat1 = this.rustWattSeconds;
	}

	// Token: 0x06001637 RID: 5687 RVA: 0x000AD347 File Offset: 0x000AB547
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.ioEntity != null)
		{
			this.rustWattSeconds = info.msg.ioEntity.genericFloat1;
		}
	}
}
