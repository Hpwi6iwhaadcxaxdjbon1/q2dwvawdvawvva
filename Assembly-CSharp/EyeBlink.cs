using System;
using UnityEngine;

// Token: 0x0200033E RID: 830
public class EyeBlink : MonoBehaviour
{
	// Token: 0x0400183B RID: 6203
	public Transform LeftEye;

	// Token: 0x0400183C RID: 6204
	public Transform LeftEyelid;

	// Token: 0x0400183D RID: 6205
	public Vector3 LeftEyeOffset = new Vector3(0.01f, -0.002f, 0f);

	// Token: 0x0400183E RID: 6206
	public Transform RightEye;

	// Token: 0x0400183F RID: 6207
	public Transform RightEyelid;

	// Token: 0x04001840 RID: 6208
	public Vector3 RightEyeOffset = new Vector3(0.01f, -0.002f, 0f);

	// Token: 0x04001841 RID: 6209
	public Vector3 ClosedEyelidPosition;

	// Token: 0x04001842 RID: 6210
	public Vector3 ClosedEyelidRotation;

	// Token: 0x04001843 RID: 6211
	public Vector2 TimeWithoutBlinking = new Vector2(1f, 10f);

	// Token: 0x04001844 RID: 6212
	public float BlinkSpeed = 0.2f;

	// Token: 0x04001845 RID: 6213
	public Vector3 LeftEyeInitial;

	// Token: 0x04001846 RID: 6214
	public Vector3 RightEyeInitial;
}
