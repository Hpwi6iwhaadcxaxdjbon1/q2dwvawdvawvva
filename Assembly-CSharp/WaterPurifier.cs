using System;
using Rust;
using UnityEngine;

// Token: 0x020003DF RID: 991
public class WaterPurifier : LiquidContainer
{
	// Token: 0x04001A55 RID: 6741
	public GameObjectRef storagePrefab;

	// Token: 0x04001A56 RID: 6742
	public Transform storagePrefabAnchor;

	// Token: 0x04001A57 RID: 6743
	public ItemDefinition freshWater;

	// Token: 0x04001A58 RID: 6744
	public int waterToProcessPerMinute = 120;

	// Token: 0x04001A59 RID: 6745
	public int freshWaterRatio = 4;

	// Token: 0x04001A5A RID: 6746
	public bool stopWhenOutputFull;

	// Token: 0x04001A5B RID: 6747
	protected LiquidContainer waterStorage;

	// Token: 0x04001A5C RID: 6748
	private float dirtyWaterProcssed;

	// Token: 0x04001A5D RID: 6749
	private float pendingFreshWater;

	// Token: 0x06002205 RID: 8709 RVA: 0x000231B4 File Offset: 0x000213B4
	public bool IsBoiling()
	{
		return base.HasFlag(BaseEntity.Flags.Reserved1);
	}

	// Token: 0x06002206 RID: 8710 RVA: 0x000DC890 File Offset: 0x000DAA90
	public override void ServerInit()
	{
		base.ServerInit();
		if (!Rust.Application.isLoadingSave)
		{
			this.SpawnStorageEnt(false);
		}
	}

	// Token: 0x06002207 RID: 8711 RVA: 0x000DC8A6 File Offset: 0x000DAAA6
	public override void PostServerLoad()
	{
		base.PostServerLoad();
		this.SpawnStorageEnt(true);
	}

	// Token: 0x06002208 RID: 8712 RVA: 0x000DC8B8 File Offset: 0x000DAAB8
	protected virtual void SpawnStorageEnt(bool load)
	{
		if (load)
		{
			BaseEntity parentEntity = base.GetParentEntity();
			if (parentEntity)
			{
				foreach (BaseEntity baseEntity in parentEntity.children)
				{
					LiquidContainer liquidContainer;
					if (baseEntity != this && (liquidContainer = (baseEntity as LiquidContainer)) != null)
					{
						this.waterStorage = liquidContainer;
						break;
					}
				}
			}
		}
		if (this.waterStorage != null)
		{
			this.waterStorage.SetConnectedTo(this);
			return;
		}
		this.waterStorage = (GameManager.server.CreateEntity(this.storagePrefab.resourcePath, this.storagePrefabAnchor.localPosition, this.storagePrefabAnchor.localRotation, true) as LiquidContainer);
		this.waterStorage.SetParent(base.GetParentEntity(), false, false);
		this.waterStorage.Spawn();
		this.waterStorage.SetConnectedTo(this);
	}

	// Token: 0x06002209 RID: 8713 RVA: 0x00029A3C File Offset: 0x00027C3C
	internal override void OnParentRemoved()
	{
		base.Kill(BaseNetworkable.DestroyMode.Gib);
	}

	// Token: 0x0600220A RID: 8714 RVA: 0x000DC9B0 File Offset: 0x000DABB0
	public override void OnKilled(HitInfo info)
	{
		base.OnKilled(info);
		if (!this.waterStorage.IsDestroyed)
		{
			this.waterStorage.Kill(BaseNetworkable.DestroyMode.None);
		}
	}

	// Token: 0x0600220B RID: 8715 RVA: 0x000063A5 File Offset: 0x000045A5
	public void ParentTemperatureUpdate(float temp)
	{
	}

	// Token: 0x0600220C RID: 8716 RVA: 0x000DC9D4 File Offset: 0x000DABD4
	public void CheckCoolDown()
	{
		if (!base.GetParentEntity() || !base.GetParentEntity().IsOn() || !this.HasDirtyWater())
		{
			base.SetFlag(BaseEntity.Flags.Reserved1, false, false, true);
			base.CancelInvoke(new Action(this.CheckCoolDown));
		}
	}

	// Token: 0x0600220D RID: 8717 RVA: 0x000DCA24 File Offset: 0x000DAC24
	public bool HasDirtyWater()
	{
		Item slot = base.inventory.GetSlot(0);
		return slot != null && slot.info.itemType == ItemContainer.ContentsType.Liquid && slot.amount > 0;
	}

	// Token: 0x0600220E RID: 8718 RVA: 0x000DCA5C File Offset: 0x000DAC5C
	public void Cook(float timeCooked)
	{
		if (this.waterStorage == null)
		{
			return;
		}
		bool flag = this.HasDirtyWater();
		if (!this.IsBoiling() && flag)
		{
			base.InvokeRepeating(new Action(this.CheckCoolDown), 2f, 2f);
			base.SetFlag(BaseEntity.Flags.Reserved1, true, false, true);
		}
		if (!this.IsBoiling())
		{
			return;
		}
		if (flag)
		{
			this.ConvertWater(timeCooked);
		}
	}

	// Token: 0x0600220F RID: 8719 RVA: 0x000DCACC File Offset: 0x000DACCC
	protected void ConvertWater(float timeCooked)
	{
		if (this.stopWhenOutputFull)
		{
			Item slot = this.waterStorage.inventory.GetSlot(0);
			if (slot != null && slot.amount >= slot.MaxStackable())
			{
				return;
			}
		}
		float num = timeCooked * ((float)this.waterToProcessPerMinute / 60f);
		this.dirtyWaterProcssed += num;
		if (this.dirtyWaterProcssed >= 1f)
		{
			Item slot2 = base.inventory.GetSlot(0);
			int num2 = Mathf.Min(Mathf.FloorToInt(this.dirtyWaterProcssed), slot2.amount);
			num = (float)num2;
			slot2.UseItem(num2);
			this.dirtyWaterProcssed -= (float)num2;
			base.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
		}
		this.pendingFreshWater += num / (float)this.freshWaterRatio;
		if (this.pendingFreshWater >= 1f)
		{
			int num3 = Mathf.FloorToInt(this.pendingFreshWater);
			this.pendingFreshWater -= (float)num3;
			Item slot3 = this.waterStorage.inventory.GetSlot(0);
			if (slot3 != null && slot3.info != this.freshWater)
			{
				slot3.RemoveFromContainer();
				slot3.Remove(0f);
			}
			if (slot3 == null)
			{
				Item item = ItemManager.Create(this.freshWater, num3, 0UL);
				if (!item.MoveToContainer(this.waterStorage.inventory, -1, true, false, null, true))
				{
					item.Remove(0f);
				}
			}
			else
			{
				slot3.amount += num3;
				slot3.amount = Mathf.Clamp(slot3.amount, 0, this.waterStorage.maxStackSize);
				this.waterStorage.inventory.MarkDirty();
			}
			this.waterStorage.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
		}
	}

	// Token: 0x06002210 RID: 8720 RVA: 0x000DCC7C File Offset: 0x000DAE7C
	public override void Load(BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.fromDisk)
		{
			base.SetFlag(BaseEntity.Flags.On, false, false, true);
		}
	}

	// Token: 0x02000CC3 RID: 3267
	public static class WaterPurifierFlags
	{
		// Token: 0x040044C1 RID: 17601
		public const BaseEntity.Flags Boiling = BaseEntity.Flags.Reserved1;
	}
}
