using System;

// Token: 0x0200038D RID: 909
[Flags]
public enum EntityType
{
	// Token: 0x04001951 RID: 6481
	Player = 1,
	// Token: 0x04001952 RID: 6482
	NPC = 2,
	// Token: 0x04001953 RID: 6483
	WorldItem = 4,
	// Token: 0x04001954 RID: 6484
	Corpse = 8,
	// Token: 0x04001955 RID: 6485
	TimedExplosive = 16,
	// Token: 0x04001956 RID: 6486
	Chair = 32,
	// Token: 0x04001957 RID: 6487
	BasePlayerNPC = 64
}
