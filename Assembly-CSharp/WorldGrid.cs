using System;

// Token: 0x020005B4 RID: 1460
public class WorldGrid : SingletonComponent<WorldGrid>, IClientComponent
{
	// Token: 0x040023AA RID: 9130
	public static bool Paused;

	// Token: 0x040023AB RID: 9131
	public float CellSize = 50f;

	// Token: 0x040023AC RID: 9132
	public float MaxMilliseconds = 0.1f;

	// Token: 0x040023AD RID: 9133
	public const float MaxRefreshDistance = 500f;

	// Token: 0x040023AE RID: 9134
	public const float MinTimeBetweenRefreshes = 1f;
}
