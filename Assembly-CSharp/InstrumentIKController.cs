using System;
using UnityEngine;

// Token: 0x020003FA RID: 1018
public class InstrumentIKController : MonoBehaviour
{
	// Token: 0x04001AAB RID: 6827
	public Vector3 HitRotationVector = Vector3.forward;

	// Token: 0x04001AAC RID: 6828
	public Transform[] LeftHandIkTargets = new Transform[0];

	// Token: 0x04001AAD RID: 6829
	public Transform[] LeftHandIKTargetHitRotations = new Transform[0];

	// Token: 0x04001AAE RID: 6830
	public Transform[] RightHandIkTargets = new Transform[0];

	// Token: 0x04001AAF RID: 6831
	public Transform[] RightHandIKTargetHitRotations = new Transform[0];

	// Token: 0x04001AB0 RID: 6832
	public Transform[] RightFootIkTargets = new Transform[0];

	// Token: 0x04001AB1 RID: 6833
	public AnimationCurve HandHeightCurve = AnimationCurve.Constant(0f, 1f, 0f);

	// Token: 0x04001AB2 RID: 6834
	public float HandHeightMultiplier = 1f;

	// Token: 0x04001AB3 RID: 6835
	public float HandMoveLerpSpeed = 50f;

	// Token: 0x04001AB4 RID: 6836
	public bool DebugHitRotation;

	// Token: 0x04001AB5 RID: 6837
	public AnimationCurve HandHitCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	// Token: 0x04001AB6 RID: 6838
	public float NoteHitTime = 0.5f;

	// Token: 0x04001AB7 RID: 6839
	[Header("Look IK")]
	public float BodyLookWeight;

	// Token: 0x04001AB8 RID: 6840
	public float HeadLookWeight;

	// Token: 0x04001AB9 RID: 6841
	public float LookWeightLimit;

	// Token: 0x04001ABA RID: 6842
	public bool HoldHandsAtPlay;
}
