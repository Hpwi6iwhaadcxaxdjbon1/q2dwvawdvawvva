using System;

// Token: 0x02000360 RID: 864
public enum AIEventType
{
	// Token: 0x040018EA RID: 6378
	Timer,
	// Token: 0x040018EB RID: 6379
	PlayerDetected,
	// Token: 0x040018EC RID: 6380
	StateError,
	// Token: 0x040018ED RID: 6381
	Attacked,
	// Token: 0x040018EE RID: 6382
	StateFinished,
	// Token: 0x040018EF RID: 6383
	InAttackRange,
	// Token: 0x040018F0 RID: 6384
	HealthBelow,
	// Token: 0x040018F1 RID: 6385
	InRange,
	// Token: 0x040018F2 RID: 6386
	PerformedAttack,
	// Token: 0x040018F3 RID: 6387
	TirednessAbove,
	// Token: 0x040018F4 RID: 6388
	HungerAbove,
	// Token: 0x040018F5 RID: 6389
	ThreatDetected,
	// Token: 0x040018F6 RID: 6390
	TargetDetected,
	// Token: 0x040018F7 RID: 6391
	AmmoBelow,
	// Token: 0x040018F8 RID: 6392
	BestTargetDetected,
	// Token: 0x040018F9 RID: 6393
	IsVisible,
	// Token: 0x040018FA RID: 6394
	AttackTick,
	// Token: 0x040018FB RID: 6395
	IsMounted,
	// Token: 0x040018FC RID: 6396
	And,
	// Token: 0x040018FD RID: 6397
	Chance,
	// Token: 0x040018FE RID: 6398
	TargetLost,
	// Token: 0x040018FF RID: 6399
	TimeSinceThreat,
	// Token: 0x04001900 RID: 6400
	OnPositionMemorySet,
	// Token: 0x04001901 RID: 6401
	AggressionTimer,
	// Token: 0x04001902 RID: 6402
	Reloading,
	// Token: 0x04001903 RID: 6403
	InRangeOfHome,
	// Token: 0x04001904 RID: 6404
	IsBlinded
}
