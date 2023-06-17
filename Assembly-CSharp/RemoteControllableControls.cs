using System;

// Token: 0x02000100 RID: 256
[Flags]
public enum RemoteControllableControls
{
	// Token: 0x04000DD9 RID: 3545
	None = 0,
	// Token: 0x04000DDA RID: 3546
	Movement = 1,
	// Token: 0x04000DDB RID: 3547
	Mouse = 2,
	// Token: 0x04000DDC RID: 3548
	SprintAndDuck = 4,
	// Token: 0x04000DDD RID: 3549
	Fire = 8,
	// Token: 0x04000DDE RID: 3550
	Reload = 16,
	// Token: 0x04000DDF RID: 3551
	Crosshair = 32
}
