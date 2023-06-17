using System;

// Token: 0x02000186 RID: 390
public class SnowballGun : BaseProjectile
{
	// Token: 0x04001099 RID: 4249
	public ItemDefinition OverrideProjectile;

	// Token: 0x060017C1 RID: 6081 RVA: 0x000B3474 File Offset: 0x000B1674
	protected override void ReloadMagazine(int desiredAmount = -1)
	{
		BasePlayer ownerPlayer = base.GetOwnerPlayer();
		if (!ownerPlayer)
		{
			return;
		}
		desiredAmount = 1;
		this.primaryMagazine.Reload(ownerPlayer, desiredAmount, this.CanRefundAmmo);
		this.primaryMagazine.contents = this.primaryMagazine.capacity;
		this.primaryMagazine.ammoType = this.OverrideProjectile;
		base.SendNetworkUpdateImmediate(false);
		ItemManager.DoRemoves();
		ownerPlayer.inventory.ServerUpdate(0f);
	}

	// Token: 0x17000201 RID: 513
	// (get) Token: 0x060017C2 RID: 6082 RVA: 0x000B34EB File Offset: 0x000B16EB
	protected override ItemDefinition PrimaryMagazineAmmo
	{
		get
		{
			if (!(this.OverrideProjectile != null))
			{
				return base.PrimaryMagazineAmmo;
			}
			return this.OverrideProjectile;
		}
	}

	// Token: 0x17000202 RID: 514
	// (get) Token: 0x060017C3 RID: 6083 RVA: 0x00007A3C File Offset: 0x00005C3C
	protected override bool CanRefundAmmo
	{
		get
		{
			return false;
		}
	}
}
