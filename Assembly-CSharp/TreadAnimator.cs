using System;
using UnityEngine;

// Token: 0x020001B0 RID: 432
public class TreadAnimator : MonoBehaviour, IClientComponent
{
	// Token: 0x0400116D RID: 4461
	public Animator mainBodyAnimator;

	// Token: 0x0400116E RID: 4462
	public Transform[] wheelBones;

	// Token: 0x0400116F RID: 4463
	public Vector3[] vecShocksOffsetPosition;

	// Token: 0x04001170 RID: 4464
	public Vector3[] wheelBoneOrigin;

	// Token: 0x04001171 RID: 4465
	public float wheelBoneDistMax = 0.26f;

	// Token: 0x04001172 RID: 4466
	public Material leftTread;

	// Token: 0x04001173 RID: 4467
	public Material rightTread;

	// Token: 0x04001174 RID: 4468
	public TreadEffects treadEffects;

	// Token: 0x04001175 RID: 4469
	public float traceThickness = 0.25f;

	// Token: 0x04001176 RID: 4470
	public float heightFudge = 0.13f;

	// Token: 0x04001177 RID: 4471
	public bool useWheelYOrigin;

	// Token: 0x04001178 RID: 4472
	public Vector2 treadTextureDirection = new Vector2(1f, 0f);

	// Token: 0x04001179 RID: 4473
	public bool isMetallic;
}
