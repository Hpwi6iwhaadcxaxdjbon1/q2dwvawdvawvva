using System;
using UnityEngine;

// Token: 0x02000983 RID: 2435
[AddComponentMenu("")]
public class AmplifyOcclusionBase : MonoBehaviour
{
	// Token: 0x04003438 RID: 13368
	[Header("Ambient Occlusion")]
	public AmplifyOcclusionBase.ApplicationMethod ApplyMethod;

	// Token: 0x04003439 RID: 13369
	[Tooltip("Number of samples per pass.")]
	public AmplifyOcclusionBase.SampleCountLevel SampleCount = AmplifyOcclusionBase.SampleCountLevel.Medium;

	// Token: 0x0400343A RID: 13370
	public AmplifyOcclusionBase.PerPixelNormalSource PerPixelNormals = AmplifyOcclusionBase.PerPixelNormalSource.Camera;

	// Token: 0x0400343B RID: 13371
	[Tooltip("Final applied intensity of the occlusion effect.")]
	[Range(0f, 1f)]
	public float Intensity = 1f;

	// Token: 0x0400343C RID: 13372
	public Color Tint = Color.black;

	// Token: 0x0400343D RID: 13373
	[Tooltip("Radius spread of the occlusion.")]
	[Range(0f, 32f)]
	public float Radius = 2f;

	// Token: 0x0400343E RID: 13374
	[Tooltip("Max sampling range in pixels.")]
	[Range(32f, 1024f)]
	[NonSerialized]
	public int PixelRadiusLimit = 512;

	// Token: 0x0400343F RID: 13375
	[Tooltip("Occlusion contribution amount on relation to radius.")]
	[Range(0f, 2f)]
	[NonSerialized]
	public float RadiusIntensity = 1f;

	// Token: 0x04003440 RID: 13376
	[Tooltip("Power exponent attenuation of the occlusion.")]
	[Range(0f, 16f)]
	public float PowerExponent = 1.8f;

	// Token: 0x04003441 RID: 13377
	[Tooltip("Controls the initial occlusion contribution offset.")]
	[Range(0f, 0.99f)]
	public float Bias = 0.05f;

	// Token: 0x04003442 RID: 13378
	[Tooltip("Controls the thickness occlusion contribution.")]
	[Range(0f, 1f)]
	public float Thickness = 1f;

	// Token: 0x04003443 RID: 13379
	[Tooltip("Compute the Occlusion and Blur at half of the resolution.")]
	public bool Downsample = true;

	// Token: 0x04003444 RID: 13380
	[Header("Distance Fade")]
	[Tooltip("Control parameters at faraway.")]
	public bool FadeEnabled;

	// Token: 0x04003445 RID: 13381
	[Tooltip("Distance in Unity unities that start to fade.")]
	public float FadeStart = 100f;

	// Token: 0x04003446 RID: 13382
	[Tooltip("Length distance to performe the transition.")]
	public float FadeLength = 50f;

	// Token: 0x04003447 RID: 13383
	[Tooltip("Final Intensity parameter.")]
	[Range(0f, 1f)]
	public float FadeToIntensity;

	// Token: 0x04003448 RID: 13384
	public Color FadeToTint = Color.black;

	// Token: 0x04003449 RID: 13385
	[Tooltip("Final Radius parameter.")]
	[Range(0f, 32f)]
	public float FadeToRadius = 2f;

	// Token: 0x0400344A RID: 13386
	[Tooltip("Final PowerExponent parameter.")]
	[Range(0f, 16f)]
	public float FadeToPowerExponent = 1.8f;

	// Token: 0x0400344B RID: 13387
	[Tooltip("Final Thickness parameter.")]
	[Range(0f, 1f)]
	public float FadeToThickness = 1f;

	// Token: 0x0400344C RID: 13388
	[Header("Bilateral Blur")]
	public bool BlurEnabled = true;

	// Token: 0x0400344D RID: 13389
	[Tooltip("Radius in screen pixels.")]
	[Range(1f, 4f)]
	public int BlurRadius = 3;

	// Token: 0x0400344E RID: 13390
	[Tooltip("Number of times that the Blur will repeat.")]
	[Range(1f, 4f)]
	public int BlurPasses = 1;

	// Token: 0x0400344F RID: 13391
	[Tooltip("0 - Blured, 1 - Sharpened.")]
	[Range(0f, 20f)]
	public float BlurSharpness = 10f;

	// Token: 0x04003450 RID: 13392
	[Header("Temporal Filter")]
	[Tooltip("Accumulates the effect over the time.")]
	public bool FilterEnabled = true;

	// Token: 0x04003451 RID: 13393
	[Tooltip("Controls the accumulation decayment. 0 - Faster update, more flicker. 1 - Slow update (ghosting on moving objects), less flicker.")]
	[Range(0f, 1f)]
	public float FilterBlending = 0.5f;

	// Token: 0x04003452 RID: 13394
	[Tooltip("Controls the discard sensibility based on the motion of the scene and objects. 0 - Discard less, reuse more (more ghost effect). 1 - Discard more, reuse less (less ghost effect).")]
	[Range(0f, 1f)]
	public float FilterResponse = 0.5f;

	// Token: 0x04003453 RID: 13395
	[Tooltip("Enables directional variations.")]
	[NonSerialized]
	public bool TemporalDirections = true;

	// Token: 0x04003454 RID: 13396
	[Tooltip("Enables offset variations.")]
	[NonSerialized]
	public bool TemporalOffsets = true;

	// Token: 0x04003455 RID: 13397
	[Tooltip("Reduces ghosting effect near the objects's edges while moving.")]
	[NonSerialized]
	public bool TemporalDilation;

	// Token: 0x04003456 RID: 13398
	[Tooltip("Uses the object movement information for calc new areas of occlusion.")]
	[NonSerialized]
	public bool UseMotionVectors = true;

	// Token: 0x02000EC9 RID: 3785
	public enum ApplicationMethod
	{
		// Token: 0x04004CF5 RID: 19701
		PostEffect,
		// Token: 0x04004CF6 RID: 19702
		Deferred,
		// Token: 0x04004CF7 RID: 19703
		Debug
	}

	// Token: 0x02000ECA RID: 3786
	public enum PerPixelNormalSource
	{
		// Token: 0x04004CF9 RID: 19705
		None,
		// Token: 0x04004CFA RID: 19706
		Camera,
		// Token: 0x04004CFB RID: 19707
		GBuffer,
		// Token: 0x04004CFC RID: 19708
		GBufferOctaEncoded
	}

	// Token: 0x02000ECB RID: 3787
	public enum SampleCountLevel
	{
		// Token: 0x04004CFE RID: 19710
		Low,
		// Token: 0x04004CFF RID: 19711
		Medium,
		// Token: 0x04004D00 RID: 19712
		High,
		// Token: 0x04004D01 RID: 19713
		VeryHigh
	}
}
