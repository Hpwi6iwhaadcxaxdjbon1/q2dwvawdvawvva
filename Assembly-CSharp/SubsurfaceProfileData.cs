using System;
using UnityEngine;

// Token: 0x0200071B RID: 1819
[Serializable]
public struct SubsurfaceProfileData
{
	// Token: 0x04002980 RID: 10624
	[Range(0.1f, 50f)]
	public float ScatterRadius;

	// Token: 0x04002981 RID: 10625
	[ColorUsage(false, true, 1f, 1f, 1f, 1f)]
	public Color SubsurfaceColor;

	// Token: 0x04002982 RID: 10626
	[ColorUsage(false, true, 1f, 1f, 1f, 1f)]
	public Color FalloffColor;

	// Token: 0x17000431 RID: 1073
	// (get) Token: 0x0600330E RID: 13070 RVA: 0x0013A3B8 File Offset: 0x001385B8
	public static SubsurfaceProfileData Default
	{
		get
		{
			return new SubsurfaceProfileData
			{
				ScatterRadius = 1.2f,
				SubsurfaceColor = new Color(0.48f, 0.41f, 0.28f),
				FalloffColor = new Color(1f, 0.37f, 0.3f)
			};
		}
	}

	// Token: 0x17000432 RID: 1074
	// (get) Token: 0x0600330F RID: 13071 RVA: 0x0013A410 File Offset: 0x00138610
	public static SubsurfaceProfileData Invalid
	{
		get
		{
			return new SubsurfaceProfileData
			{
				ScatterRadius = 0f,
				SubsurfaceColor = Color.clear,
				FalloffColor = Color.clear
			};
		}
	}
}
