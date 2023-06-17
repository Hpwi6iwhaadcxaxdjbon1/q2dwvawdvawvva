using System;

// Token: 0x0200090D RID: 2317
public struct PerformanceSamplePoint
{
	// Token: 0x04003301 RID: 13057
	public int UpdateCount;

	// Token: 0x04003302 RID: 13058
	public int FixedUpdateCount;

	// Token: 0x04003303 RID: 13059
	public int RenderCount;

	// Token: 0x04003304 RID: 13060
	public TimeSpan PreCull;

	// Token: 0x04003305 RID: 13061
	public TimeSpan Update;

	// Token: 0x04003306 RID: 13062
	public TimeSpan LateUpdate;

	// Token: 0x04003307 RID: 13063
	public TimeSpan Render;

	// Token: 0x04003308 RID: 13064
	public TimeSpan FixedUpdate;

	// Token: 0x04003309 RID: 13065
	public TimeSpan NetworkMessage;

	// Token: 0x0400330A RID: 13066
	public TimeSpan TotalCPU;

	// Token: 0x0400330B RID: 13067
	public int CpuUpdateCount;
}
