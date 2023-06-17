using System;

// Token: 0x02000536 RID: 1334
public class LODGrid : SingletonComponent<LODGrid>, IClientComponent
{
	// Token: 0x040021F7 RID: 8695
	public static bool Paused = false;

	// Token: 0x040021F8 RID: 8696
	public float CellSize = 50f;

	// Token: 0x040021F9 RID: 8697
	public float MaxMilliseconds = 0.1f;

	// Token: 0x040021FA RID: 8698
	public const float MaxRefreshDistance = 500f;

	// Token: 0x040021FB RID: 8699
	public static float TreeMeshDistance = 500f;

	// Token: 0x040021FC RID: 8700
	public const float MinTimeBetweenRefreshes = 1f;
}
