using System;
using UnityEngine;

// Token: 0x02000396 RID: 918
public class AudioVisualisationEntityLight : AudioVisualisationEntity
{
	// Token: 0x0400195D RID: 6493
	public Light TargetLight;

	// Token: 0x0400195E RID: 6494
	public Light SecondaryLight;

	// Token: 0x0400195F RID: 6495
	public MeshRenderer[] TargetRenderer;

	// Token: 0x04001960 RID: 6496
	public AudioVisualisationEntityLight.LightColourSet RedColour;

	// Token: 0x04001961 RID: 6497
	public AudioVisualisationEntityLight.LightColourSet GreenColour;

	// Token: 0x04001962 RID: 6498
	public AudioVisualisationEntityLight.LightColourSet BlueColour;

	// Token: 0x04001963 RID: 6499
	public AudioVisualisationEntityLight.LightColourSet YellowColour;

	// Token: 0x04001964 RID: 6500
	public AudioVisualisationEntityLight.LightColourSet PinkColour;

	// Token: 0x04001965 RID: 6501
	public float lightMinIntensity = 0.05f;

	// Token: 0x04001966 RID: 6502
	public float lightMaxIntensity = 1f;

	// Token: 0x02000CB3 RID: 3251
	[Serializable]
	public struct LightColourSet
	{
		// Token: 0x0400448A RID: 17546
		[ColorUsage(true, true)]
		public Color LightColor;

		// Token: 0x0400448B RID: 17547
		[ColorUsage(true, true)]
		public Color SecondaryLightColour;

		// Token: 0x0400448C RID: 17548
		[ColorUsage(true, true)]
		public Color EmissionColour;
	}
}
