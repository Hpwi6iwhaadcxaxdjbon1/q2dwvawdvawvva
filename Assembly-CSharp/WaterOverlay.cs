using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

// Token: 0x020002A1 RID: 673
public class WaterOverlay : MonoBehaviour, IClientComponent
{
	// Token: 0x04001606 RID: 5638
	public PostProcessVolume postProcessVolume;

	// Token: 0x04001607 RID: 5639
	public WaterOverlay.EffectParams adminParams = WaterOverlay.EffectParams.DefaultAdmin;

	// Token: 0x04001608 RID: 5640
	public WaterOverlay.EffectParams gogglesParams = WaterOverlay.EffectParams.DefaultGoggles;

	// Token: 0x04001609 RID: 5641
	public WaterOverlay.EffectParams submarineParams = WaterOverlay.EffectParams.DefaultSubmarine;

	// Token: 0x0400160A RID: 5642
	public WaterOverlay.EffectParams underwaterLabParams = WaterOverlay.EffectParams.DefaultUnderwaterLab;

	// Token: 0x0400160B RID: 5643
	public Material[] UnderwaterFogMaterials;

	// Token: 0x02000C8E RID: 3214
	[Serializable]
	public struct EffectParams
	{
		// Token: 0x040043D6 RID: 17366
		public float scatterCoefficient;

		// Token: 0x040043D7 RID: 17367
		public bool blur;

		// Token: 0x040043D8 RID: 17368
		public float blurDistance;

		// Token: 0x040043D9 RID: 17369
		public float blurSize;

		// Token: 0x040043DA RID: 17370
		public int blurIterations;

		// Token: 0x040043DB RID: 17371
		public bool wiggle;

		// Token: 0x040043DC RID: 17372
		public float wiggleSpeed;

		// Token: 0x040043DD RID: 17373
		public bool chromaticAberration;

		// Token: 0x040043DE RID: 17374
		public bool godRays;

		// Token: 0x040043DF RID: 17375
		public static WaterOverlay.EffectParams DefaultAdmin = new WaterOverlay.EffectParams
		{
			scatterCoefficient = 0.025f,
			blur = false,
			blurDistance = 10f,
			blurSize = 2f,
			wiggle = false,
			wiggleSpeed = 0f,
			chromaticAberration = true,
			godRays = false
		};

		// Token: 0x040043E0 RID: 17376
		public static WaterOverlay.EffectParams DefaultGoggles = new WaterOverlay.EffectParams
		{
			scatterCoefficient = 0.05f,
			blur = true,
			blurDistance = 10f,
			blurSize = 2f,
			wiggle = true,
			wiggleSpeed = 2f,
			chromaticAberration = true,
			godRays = true
		};

		// Token: 0x040043E1 RID: 17377
		public static WaterOverlay.EffectParams DefaultSubmarine = new WaterOverlay.EffectParams
		{
			scatterCoefficient = 0.025f,
			blur = false,
			blurDistance = 10f,
			blurSize = 2f,
			wiggle = false,
			wiggleSpeed = 0f,
			chromaticAberration = false,
			godRays = false
		};

		// Token: 0x040043E2 RID: 17378
		public static WaterOverlay.EffectParams DefaultUnderwaterLab = new WaterOverlay.EffectParams
		{
			scatterCoefficient = 0.005f,
			blur = false,
			blurDistance = 10f,
			blurSize = 2f,
			wiggle = false,
			wiggleSpeed = 0f,
			chromaticAberration = true,
			godRays = false
		};
	}
}
