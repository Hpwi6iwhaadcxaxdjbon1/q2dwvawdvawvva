using System;

// Token: 0x0200052C RID: 1324
public class RendererGrid : SingletonComponent<RendererGrid>, IClientComponent
{
	// Token: 0x040021DD RID: 8669
	public static bool Paused;

	// Token: 0x040021DE RID: 8670
	public GameObjectRef BatchPrefab;

	// Token: 0x040021DF RID: 8671
	public float CellSize = 50f;

	// Token: 0x040021E0 RID: 8672
	public float MaxMilliseconds = 0.1f;

	// Token: 0x040021E1 RID: 8673
	public const float MinTimeBetweenRefreshes = 1f;
}
