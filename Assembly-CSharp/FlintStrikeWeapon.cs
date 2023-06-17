using System;

// Token: 0x020003AE RID: 942
public class FlintStrikeWeapon : BaseProjectile
{
	// Token: 0x040019E9 RID: 6633
	public float successFraction = 0.5f;

	// Token: 0x040019EA RID: 6634
	public RecoilProperties strikeRecoil;

	// Token: 0x0600210C RID: 8460 RVA: 0x000D92C7 File Offset: 0x000D74C7
	public override RecoilProperties GetRecoil()
	{
		return this.strikeRecoil;
	}
}
