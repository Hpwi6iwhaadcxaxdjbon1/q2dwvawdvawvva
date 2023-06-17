using System;

// Token: 0x020003AD RID: 941
public class CrossbowWeapon : BaseProjectile
{
	// Token: 0x06002109 RID: 8457 RVA: 0x0000441C File Offset: 0x0000261C
	public override bool ForceSendMagazine(BaseNetworkable.SaveInfo saveInfo)
	{
		return true;
	}

	// Token: 0x0600210A RID: 8458 RVA: 0x000D92BE File Offset: 0x000D74BE
	public override void DidAttackServerside()
	{
		base.SendNetworkUpdateImmediate(false);
	}
}
