using System;

// Token: 0x02000505 RID: 1285
[Flags]
public enum EnvironmentType
{
	// Token: 0x04002124 RID: 8484
	Underground = 1,
	// Token: 0x04002125 RID: 8485
	Building = 2,
	// Token: 0x04002126 RID: 8486
	Outdoor = 4,
	// Token: 0x04002127 RID: 8487
	Elevator = 8,
	// Token: 0x04002128 RID: 8488
	PlayerConstruction = 16,
	// Token: 0x04002129 RID: 8489
	TrainTunnels = 32,
	// Token: 0x0400212A RID: 8490
	UnderwaterLab = 64,
	// Token: 0x0400212B RID: 8491
	Submarine = 128,
	// Token: 0x0400212C RID: 8492
	BuildingDark = 256,
	// Token: 0x0400212D RID: 8493
	BuildingVeryDark = 512,
	// Token: 0x0400212E RID: 8494
	NoSunlight = 1024
}
