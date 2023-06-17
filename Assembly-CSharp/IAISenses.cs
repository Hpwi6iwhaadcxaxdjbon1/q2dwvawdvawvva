using System;

// Token: 0x02000386 RID: 902
internal interface IAISenses
{
	// Token: 0x0600201F RID: 8223
	bool IsThreat(BaseEntity entity);

	// Token: 0x06002020 RID: 8224
	bool IsTarget(BaseEntity entity);

	// Token: 0x06002021 RID: 8225
	bool IsFriendly(BaseEntity entity);
}
