using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020003D8 RID: 984
public class PoweredWaterPurifier : WaterPurifier
{
	// Token: 0x04001A40 RID: 6720
	public float ConvertInterval = 5f;

	// Token: 0x04001A41 RID: 6721
	public int PowerDrain = 5;

	// Token: 0x04001A42 RID: 6722
	public Material PoweredMaterial;

	// Token: 0x04001A43 RID: 6723
	public Material UnpoweredMaterial;

	// Token: 0x04001A44 RID: 6724
	public MeshRenderer TargetRenderer;

	// Token: 0x060021D4 RID: 8660 RVA: 0x00025420 File Offset: 0x00023620
	public override void ResetState()
	{
		base.ResetState();
	}

	// Token: 0x060021D5 RID: 8661 RVA: 0x000DBED0 File Offset: 0x000DA0D0
	public override bool CanPickup(BasePlayer player)
	{
		if (base.isClient)
		{
			return base.CanPickup(player);
		}
		return base.CanPickup(player) && !base.HasDirtyWater() && this.waterStorage != null && (this.waterStorage.inventory == null || this.waterStorage.inventory.itemList.Count == 0);
	}

	// Token: 0x060021D6 RID: 8662 RVA: 0x000DBF38 File Offset: 0x000DA138
	protected override void SpawnStorageEnt(bool load)
	{
		if (load)
		{
			using (List<BaseEntity>.Enumerator enumerator = this.children.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					LiquidContainer waterStorage;
					if ((waterStorage = (enumerator.Current as LiquidContainer)) != null)
					{
						this.waterStorage = waterStorage;
					}
				}
			}
		}
		if (this.waterStorage != null)
		{
			this.waterStorage.SetConnectedTo(this);
			return;
		}
		this.waterStorage = (GameManager.server.CreateEntity(this.storagePrefab.resourcePath, this.storagePrefabAnchor.position, this.storagePrefabAnchor.rotation, true) as LiquidContainer);
		this.waterStorage.SetParent(this, true, false);
		this.waterStorage.Spawn();
		this.waterStorage.SetConnectedTo(this);
	}

	// Token: 0x060021D7 RID: 8663 RVA: 0x000DC010 File Offset: 0x000DA210
	public override void OnItemAddedOrRemoved(Item item, bool added)
	{
		base.OnItemAddedOrRemoved(item, added);
		if (base.HasLiquidItem())
		{
			if (base.HasFlag(BaseEntity.Flags.Reserved8) && !base.IsInvoking(new Action(this.ConvertWater)))
			{
				base.InvokeRandomized(new Action(this.ConvertWater), this.ConvertInterval, this.ConvertInterval, this.ConvertInterval * 0.1f);
				return;
			}
		}
		else if (base.IsInvoking(new Action(this.ConvertWater)))
		{
			base.CancelInvoke(new Action(this.ConvertWater));
		}
	}

	// Token: 0x060021D8 RID: 8664 RVA: 0x000DC09F File Offset: 0x000DA29F
	private void ConvertWater()
	{
		if (!base.HasDirtyWater())
		{
			return;
		}
		base.ConvertWater(this.ConvertInterval);
	}

	// Token: 0x060021D9 RID: 8665 RVA: 0x000DC0B6 File Offset: 0x000DA2B6
	public override int ConsumptionAmount()
	{
		return this.PowerDrain;
	}

	// Token: 0x060021DA RID: 8666 RVA: 0x000DC0C0 File Offset: 0x000DA2C0
	public override void OnFlagsChanged(BaseEntity.Flags old, BaseEntity.Flags next)
	{
		base.OnFlagsChanged(old, next);
		if (base.isServer)
		{
			if (old.HasFlag(BaseEntity.Flags.Reserved8) != next.HasFlag(BaseEntity.Flags.Reserved8))
			{
				if (next.HasFlag(BaseEntity.Flags.Reserved8))
				{
					if (!base.IsInvoking(new Action(this.ConvertWater)))
					{
						base.InvokeRandomized(new Action(this.ConvertWater), this.ConvertInterval, this.ConvertInterval, this.ConvertInterval * 0.1f);
					}
				}
				else if (base.IsInvoking(new Action(this.ConvertWater)))
				{
					base.CancelInvoke(new Action(this.ConvertWater));
				}
			}
			if (this.waterStorage != null)
			{
				this.waterStorage.SetFlag(BaseEntity.Flags.Reserved8, base.HasFlag(BaseEntity.Flags.Reserved8), false, true);
			}
		}
	}
}
