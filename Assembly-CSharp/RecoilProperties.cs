using System;
using UnityEngine;

// Token: 0x0200075B RID: 1883
[CreateAssetMenu(menuName = "Rust/Recoil Properties")]
public class RecoilProperties : ScriptableObject
{
	// Token: 0x04002AA6 RID: 10918
	public float recoilYawMin;

	// Token: 0x04002AA7 RID: 10919
	public float recoilYawMax;

	// Token: 0x04002AA8 RID: 10920
	public float recoilPitchMin;

	// Token: 0x04002AA9 RID: 10921
	public float recoilPitchMax;

	// Token: 0x04002AAA RID: 10922
	public float timeToTakeMin;

	// Token: 0x04002AAB RID: 10923
	public float timeToTakeMax = 0.1f;

	// Token: 0x04002AAC RID: 10924
	public float ADSScale = 0.5f;

	// Token: 0x04002AAD RID: 10925
	public float movementPenalty;

	// Token: 0x04002AAE RID: 10926
	public float clampPitch = float.NegativeInfinity;

	// Token: 0x04002AAF RID: 10927
	public AnimationCurve pitchCurve = new AnimationCurve(new Keyframe[]
	{
		new Keyframe(0f, 1f),
		new Keyframe(1f, 1f)
	});

	// Token: 0x04002AB0 RID: 10928
	public AnimationCurve yawCurve = new AnimationCurve(new Keyframe[]
	{
		new Keyframe(0f, 1f),
		new Keyframe(1f, 1f)
	});

	// Token: 0x04002AB1 RID: 10929
	public bool useCurves;

	// Token: 0x04002AB2 RID: 10930
	public bool curvesAsScalar;

	// Token: 0x04002AB3 RID: 10931
	public int shotsUntilMax = 30;

	// Token: 0x04002AB4 RID: 10932
	public float maxRecoilRadius = 5f;

	// Token: 0x04002AB5 RID: 10933
	[Header("AimCone")]
	public bool overrideAimconeWithCurve;

	// Token: 0x04002AB6 RID: 10934
	public float aimconeCurveScale = 1f;

	// Token: 0x04002AB7 RID: 10935
	[Tooltip("How much to scale aimcone by based on how far into the shot sequence we are (shots v shotsUntilMax)")]
	public AnimationCurve aimconeCurve = new AnimationCurve(new Keyframe[]
	{
		new Keyframe(0f, 1f),
		new Keyframe(1f, 1f)
	});

	// Token: 0x04002AB8 RID: 10936
	[Tooltip("Randomly select how much to scale final aimcone by per shot, you can use this to weigh a fraction of shots closer to the center")]
	public AnimationCurve aimconeProbabilityCurve = new AnimationCurve(new Keyframe[]
	{
		new Keyframe(0f, 1f),
		new Keyframe(0.5f, 0f),
		new Keyframe(1f, 1f)
	});

	// Token: 0x04002AB9 RID: 10937
	public RecoilProperties newRecoilOverride;

	// Token: 0x06003495 RID: 13461 RVA: 0x00145977 File Offset: 0x00143B77
	public RecoilProperties GetRecoil()
	{
		if (!(this.newRecoilOverride != null))
		{
			return this;
		}
		return this.newRecoilOverride;
	}
}
