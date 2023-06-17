using System;
using System.Collections.Generic;
using Facepunch;
using ProtoBuf;
using UnityEngine;

// Token: 0x0200017D RID: 381
public class PaddlingPool : LiquidContainer, ISplashable
{
	// Token: 0x0400107A RID: 4218
	public const global::BaseEntity.Flags FilledUp = global::BaseEntity.Flags.Reserved4;

	// Token: 0x0400107B RID: 4219
	public Transform poolWaterVolume;

	// Token: 0x0400107C RID: 4220
	public GameObject poolWaterVisual;

	// Token: 0x0400107D RID: 4221
	public float minimumWaterHeight;

	// Token: 0x0400107E RID: 4222
	public float maximumWaterHeight = 1f;

	// Token: 0x0400107F RID: 4223
	public WaterVolume waterVolume;

	// Token: 0x04001080 RID: 4224
	public bool alignWaterUp = true;

	// Token: 0x04001081 RID: 4225
	public GameObjectRef destroyedWithWaterEffect;

	// Token: 0x04001082 RID: 4226
	public Transform destroyedWithWaterEffectPos;

	// Token: 0x04001083 RID: 4227
	public Collider requireLookAt;

	// Token: 0x04001084 RID: 4228
	private float lastFillAmount = -1f;

	// Token: 0x060017A1 RID: 6049 RVA: 0x000B2D54 File Offset: 0x000B0F54
	public override void OnItemAddedOrRemoved(global::Item item, bool added)
	{
		base.OnItemAddedOrRemoved(item, added);
		float normalisedFillLevel = this.GetNormalisedFillLevel();
		base.SetFlag(global::BaseEntity.Flags.Reserved4, normalisedFillLevel >= 1f, false, true);
		this.UpdatePoolFillAmount(normalisedFillLevel);
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x060017A2 RID: 6050 RVA: 0x000B2D98 File Offset: 0x000B0F98
	protected override void OnInventoryDirty()
	{
		base.OnInventoryDirty();
		float normalisedFillLevel = this.GetNormalisedFillLevel();
		this.UpdatePoolFillAmount(normalisedFillLevel);
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x060017A3 RID: 6051 RVA: 0x000B2DC0 File Offset: 0x000B0FC0
	public bool WantsSplash(ItemDefinition splashType, int amount)
	{
		if (base.IsDestroyed)
		{
			return false;
		}
		if (!base.HasFlag(global::BaseEntity.Flags.Reserved4) && splashType != null)
		{
			for (int i = 0; i < this.ValidItems.Length; i++)
			{
				if (this.ValidItems[i] != null && this.ValidItems[i].itemid == splashType.itemid)
				{
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x060017A4 RID: 6052 RVA: 0x000B2E28 File Offset: 0x000B1028
	public int DoSplash(ItemDefinition splashType, int amount)
	{
		base.inventory.AddItem(splashType, amount, 0UL, global::ItemContainer.LimitStack.Existing);
		return amount;
	}

	// Token: 0x060017A5 RID: 6053 RVA: 0x000B2E3B File Offset: 0x000B103B
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.WaterPool = Pool.Get<WaterPool>();
		info.msg.WaterPool.fillAmount = this.GetNormalisedFillLevel();
	}

	// Token: 0x060017A6 RID: 6054 RVA: 0x000B2E6C File Offset: 0x000B106C
	private float GetNormalisedFillLevel()
	{
		if (base.inventory.itemList.Count <= 0 || base.inventory.itemList[0] == null)
		{
			return 0f;
		}
		return (float)base.inventory.itemList[0].amount / (float)this.maxStackSize;
	}

	// Token: 0x060017A7 RID: 6055 RVA: 0x000B2EC4 File Offset: 0x000B10C4
	private void UpdatePoolFillAmount(float normalisedAmount)
	{
		this.poolWaterVisual.gameObject.SetActive(normalisedAmount > 0f);
		this.waterVolume.waterEnabled = (normalisedAmount > 0f);
		float y = Mathf.Lerp(this.minimumWaterHeight, this.maximumWaterHeight, normalisedAmount);
		Vector3 localPosition = this.poolWaterVolume.localPosition;
		localPosition.y = y;
		this.poolWaterVolume.localPosition = localPosition;
		if (this.alignWaterUp)
		{
			this.poolWaterVolume.up = Vector3.up;
		}
		if (normalisedAmount > 0f && this.lastFillAmount < normalisedAmount && this.waterVolume.entityContents != null)
		{
			using (HashSet<global::BaseEntity>.Enumerator enumerator = this.waterVolume.entityContents.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					IPoolVehicle poolVehicle;
					if ((poolVehicle = (enumerator.Current as IPoolVehicle)) != null)
					{
						poolVehicle.WakeUp();
					}
				}
			}
		}
		this.lastFillAmount = normalisedAmount;
	}

	// Token: 0x060017A8 RID: 6056 RVA: 0x00007A3C File Offset: 0x00005C3C
	public override int ConsumptionAmount()
	{
		return 0;
	}

	// Token: 0x060017A9 RID: 6057 RVA: 0x000B2FC0 File Offset: 0x000B11C0
	public override void DestroyShared()
	{
		base.DestroyShared();
		if (base.isServer)
		{
			List<IPoolVehicle> list = Pool.GetList<IPoolVehicle>();
			if (this.waterVolume.entityContents != null)
			{
				using (HashSet<global::BaseEntity>.Enumerator enumerator = this.waterVolume.entityContents.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						IPoolVehicle item;
						if ((item = (enumerator.Current as IPoolVehicle)) != null)
						{
							list.Add(item);
						}
					}
				}
			}
			foreach (IPoolVehicle poolVehicle in list)
			{
				poolVehicle.OnPoolDestroyed();
			}
			Pool.FreeList<IPoolVehicle>(ref list);
		}
	}
}
