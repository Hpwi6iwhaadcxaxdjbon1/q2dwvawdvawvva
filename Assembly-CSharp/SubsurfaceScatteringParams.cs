using System;

// Token: 0x02000716 RID: 1814
[Serializable]
public struct SubsurfaceScatteringParams
{
	// Token: 0x04002972 RID: 10610
	public bool enabled;

	// Token: 0x04002973 RID: 10611
	public SubsurfaceScatteringParams.Quality quality;

	// Token: 0x04002974 RID: 10612
	public bool halfResolution;

	// Token: 0x04002975 RID: 10613
	public float radiusScale;

	// Token: 0x04002976 RID: 10614
	public static SubsurfaceScatteringParams Default = new SubsurfaceScatteringParams
	{
		enabled = true,
		quality = SubsurfaceScatteringParams.Quality.Medium,
		halfResolution = true,
		radiusScale = 1f
	};

	// Token: 0x02000E39 RID: 3641
	public enum Quality
	{
		// Token: 0x04004AB2 RID: 19122
		Low,
		// Token: 0x04004AB3 RID: 19123
		Medium,
		// Token: 0x04004AB4 RID: 19124
		High
	}
}
