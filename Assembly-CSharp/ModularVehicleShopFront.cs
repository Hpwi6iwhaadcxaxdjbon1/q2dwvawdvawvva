using System;
using UnityEngine;

// Token: 0x0200049E RID: 1182
public class ModularVehicleShopFront : ShopFront
{
	// Token: 0x04001F00 RID: 7936
	[SerializeField]
	private float maxUseDistance = 1.5f;

	// Token: 0x060026C0 RID: 9920 RVA: 0x000F28B6 File Offset: 0x000F0AB6
	public override bool CanBeLooted(BasePlayer player)
	{
		return this.WithinUseDistance(player) && base.CanBeLooted(player);
	}

	// Token: 0x060026C1 RID: 9921 RVA: 0x000F28CA File Offset: 0x000F0ACA
	private bool WithinUseDistance(BasePlayer player)
	{
		return base.Distance(player.eyes.position) <= this.maxUseDistance;
	}
}
