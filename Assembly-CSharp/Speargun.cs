using System;
using UnityEngine;

// Token: 0x020003B7 RID: 951
public class Speargun : CrossbowWeapon
{
	// Token: 0x040019F2 RID: 6642
	public GameObject worldAmmoModel;

	// Token: 0x06002119 RID: 8473 RVA: 0x0000441C File Offset: 0x0000261C
	public override bool ForceSendMagazine(BaseNetworkable.SaveInfo saveInfo)
	{
		return true;
	}

	// Token: 0x0600211A RID: 8474 RVA: 0x000D9408 File Offset: 0x000D7608
	protected override bool VerifyClientAttack(BasePlayer player)
	{
		return player.WaterFactor() >= 1f && base.VerifyClientAttack(player);
	}

	// Token: 0x0600211B RID: 8475 RVA: 0x0000441C File Offset: 0x0000261C
	public override bool CanBeUsedInWater()
	{
		return true;
	}
}
