using System;
using Rust;
using UnityEngine;

// Token: 0x02000190 RID: 400
public class Stocking : LootContainer
{
	// Token: 0x040010D1 RID: 4305
	public static ListHashSet<Stocking> stockings;

	// Token: 0x060017E8 RID: 6120 RVA: 0x000B41A3 File Offset: 0x000B23A3
	public override void ServerInit()
	{
		base.ServerInit();
		if (Stocking.stockings == null)
		{
			Stocking.stockings = new ListHashSet<Stocking>(8);
		}
		Stocking.stockings.Add(this);
	}

	// Token: 0x060017E9 RID: 6121 RVA: 0x000B41C8 File Offset: 0x000B23C8
	internal override void DoServerDestroy()
	{
		Stocking.stockings.Remove(this);
		base.DoServerDestroy();
	}

	// Token: 0x060017EA RID: 6122 RVA: 0x000B41DC File Offset: 0x000B23DC
	public bool IsEmpty()
	{
		if (base.inventory == null)
		{
			return false;
		}
		for (int i = base.inventory.itemList.Count - 1; i >= 0; i--)
		{
			if (base.inventory.itemList[i] != null)
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x060017EB RID: 6123 RVA: 0x000B4228 File Offset: 0x000B2428
	public override void SpawnLoot()
	{
		if (base.inventory == null)
		{
			Debug.Log("CONTACT DEVELOPERS! Stocking::PopulateLoot has null inventory!!! " + base.name);
			return;
		}
		if (this.IsEmpty())
		{
			base.SpawnLoot();
			base.SetFlag(BaseEntity.Flags.On, true, false, true);
			base.Hurt(this.MaxHealth() * 0.1f, DamageType.Generic, null, false);
		}
	}

	// Token: 0x060017EC RID: 6124 RVA: 0x000B4280 File Offset: 0x000B2480
	public override void PlayerStoppedLooting(BasePlayer player)
	{
		base.PlayerStoppedLooting(player);
		base.SetFlag(BaseEntity.Flags.On, false, false, true);
		if (this.IsEmpty() && base.healthFraction <= 0.1f)
		{
			base.Hurt(base.health, DamageType.Generic, this, false);
		}
	}
}
