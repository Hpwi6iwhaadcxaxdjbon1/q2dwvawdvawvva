using System;
using UnityEngine;

// Token: 0x0200039F RID: 927
public class LaserLight : AudioVisualisationEntity
{
	// Token: 0x0400198E RID: 6542
	public Animator LaserAnimator;

	// Token: 0x0400198F RID: 6543
	public LineRenderer[] LineRenderers;

	// Token: 0x04001990 RID: 6544
	public MeshRenderer[] DotRenderers;

	// Token: 0x04001991 RID: 6545
	public MeshRenderer FlareRenderer;

	// Token: 0x04001992 RID: 6546
	public Light[] LightSources;

	// Token: 0x04001993 RID: 6547
	public LaserLight.ColourSetting RedSettings;

	// Token: 0x04001994 RID: 6548
	public LaserLight.ColourSetting GreenSettings;

	// Token: 0x04001995 RID: 6549
	public LaserLight.ColourSetting BlueSettings;

	// Token: 0x04001996 RID: 6550
	public LaserLight.ColourSetting YellowSettings;

	// Token: 0x04001997 RID: 6551
	public LaserLight.ColourSetting PinkSettings;

	// Token: 0x06002082 RID: 8322 RVA: 0x000D700A File Offset: 0x000D520A
	public override void OnFlagsChanged(BaseEntity.Flags old, BaseEntity.Flags next)
	{
		base.OnFlagsChanged(old, next);
	}

	// Token: 0x02000CB4 RID: 3252
	[Serializable]
	public struct ColourSetting
	{
		// Token: 0x0400448D RID: 17549
		public Color PointLightColour;

		// Token: 0x0400448E RID: 17550
		public Material LaserMaterial;

		// Token: 0x0400448F RID: 17551
		public Color DotColour;

		// Token: 0x04004490 RID: 17552
		public Color FlareColour;
	}
}
