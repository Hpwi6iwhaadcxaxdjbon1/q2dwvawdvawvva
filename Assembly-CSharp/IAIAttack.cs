using System;

// Token: 0x02000392 RID: 914
public interface IAIAttack
{
	// Token: 0x06002046 RID: 8262
	void AttackTick(float delta, BaseEntity target, bool targetIsLOS);

	// Token: 0x06002047 RID: 8263
	BaseEntity GetBestTarget();

	// Token: 0x06002048 RID: 8264
	bool CanAttack(BaseEntity entity);

	// Token: 0x06002049 RID: 8265
	float EngagementRange();

	// Token: 0x0600204A RID: 8266
	bool IsTargetInRange(BaseEntity entity, out float dist);

	// Token: 0x0600204B RID: 8267
	bool CanSeeTarget(BaseEntity entity);

	// Token: 0x0600204C RID: 8268
	float GetAmmoFraction();

	// Token: 0x0600204D RID: 8269
	bool NeedsToReload();

	// Token: 0x0600204E RID: 8270
	bool Reload();

	// Token: 0x0600204F RID: 8271
	float CooldownDuration();

	// Token: 0x06002050 RID: 8272
	bool IsOnCooldown();

	// Token: 0x06002051 RID: 8273
	bool StartAttacking(BaseEntity entity);

	// Token: 0x06002052 RID: 8274
	void StopAttacking();
}
