using System;
using UnityEngine;

// Token: 0x020003DD RID: 989
public class WaterCatcher : LiquidContainer
{
	// Token: 0x04001A4C RID: 6732
	[Header("Water Catcher")]
	public ItemDefinition itemToCreate;

	// Token: 0x04001A4D RID: 6733
	public float maxItemToCreate = 10f;

	// Token: 0x04001A4E RID: 6734
	[Header("Outside Test")]
	public Vector3 rainTestPosition = new Vector3(0f, 1f, 0f);

	// Token: 0x04001A4F RID: 6735
	public float rainTestSize = 1f;

	// Token: 0x04001A50 RID: 6736
	private const float collectInterval = 60f;

	// Token: 0x060021F7 RID: 8695 RVA: 0x000DC44E File Offset: 0x000DA64E
	public override void ServerInit()
	{
		base.ServerInit();
		this.AddResource(1);
		base.InvokeRandomized(new Action(this.CollectWater), 60f, 60f, 6f);
	}

	// Token: 0x060021F8 RID: 8696 RVA: 0x000DC480 File Offset: 0x000DA680
	private void CollectWater()
	{
		if (this.IsFull())
		{
			return;
		}
		float num = 0.25f;
		num += Climate.GetFog(base.transform.position) * 2f;
		if (this.TestIsOutside())
		{
			num += Climate.GetRain(base.transform.position);
			num += Climate.GetSnow(base.transform.position) * 0.5f;
		}
		this.AddResource(Mathf.CeilToInt(this.maxItemToCreate * num));
	}

	// Token: 0x060021F9 RID: 8697 RVA: 0x000DC4FC File Offset: 0x000DA6FC
	private bool IsFull()
	{
		return base.inventory.itemList.Count != 0 && base.inventory.itemList[0].amount >= base.inventory.maxStackSize;
	}

	// Token: 0x060021FA RID: 8698 RVA: 0x000DC538 File Offset: 0x000DA738
	private bool TestIsOutside()
	{
		return !Physics.SphereCast(new Ray(base.transform.localToWorldMatrix.MultiplyPoint3x4(this.rainTestPosition), Vector3.up), this.rainTestSize, 256f, 161546513);
	}

	// Token: 0x060021FB RID: 8699 RVA: 0x000DC580 File Offset: 0x000DA780
	private void AddResource(int iAmount)
	{
		if (this.outputs.Length != 0)
		{
			IOEntity ioentity = this.CheckPushLiquid(this.outputs[0].connectedTo.Get(true), iAmount, this, IOEntity.backtracking * 2);
			LiquidContainer liquidContainer;
			if (ioentity != null && (liquidContainer = (ioentity as LiquidContainer)) != null)
			{
				liquidContainer.inventory.AddItem(this.itemToCreate, iAmount, 0UL, ItemContainer.LimitStack.Existing);
				return;
			}
		}
		base.inventory.AddItem(this.itemToCreate, iAmount, 0UL, ItemContainer.LimitStack.Existing);
		base.UpdateOnFlag();
	}

	// Token: 0x060021FC RID: 8700 RVA: 0x000DC600 File Offset: 0x000DA800
	private IOEntity CheckPushLiquid(IOEntity connected, int amount, IOEntity fromSource, int depth)
	{
		if (depth <= 0 || this.itemToCreate == null)
		{
			return null;
		}
		if (connected == null)
		{
			return null;
		}
		Vector3 zero = Vector3.zero;
		IOEntity ioentity = connected.FindGravitySource(ref zero, IOEntity.backtracking, true);
		if (ioentity != null && !connected.AllowLiquidPassthrough(ioentity, zero, false))
		{
			return null;
		}
		if (connected == this || this.ConsiderConnectedTo(connected))
		{
			return null;
		}
		if (connected.prefabID == 2150367216U)
		{
			return null;
		}
		foreach (IOEntity.IOSlot ioslot in connected.outputs)
		{
			IOEntity ioentity2 = ioslot.connectedTo.Get(true);
			Vector3 sourceWorldPosition = connected.transform.TransformPoint(ioslot.handlePosition);
			if (ioentity2 != null && ioentity2 != fromSource && ioentity2.AllowLiquidPassthrough(connected, sourceWorldPosition, false))
			{
				IOEntity ioentity3 = this.CheckPushLiquid(ioentity2, amount, fromSource, depth - 1);
				if (ioentity3 != null)
				{
					return ioentity3;
				}
			}
		}
		LiquidContainer liquidContainer;
		if ((liquidContainer = (connected as LiquidContainer)) != null && liquidContainer.inventory.GetAmount(this.itemToCreate.itemid, false) + amount < liquidContainer.maxStackSize)
		{
			return connected;
		}
		return null;
	}
}
