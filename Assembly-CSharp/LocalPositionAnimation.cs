using System;
using UnityEngine;

// Token: 0x020002C7 RID: 711
public class LocalPositionAnimation : MonoBehaviour, IClientComponent
{
	// Token: 0x0400167A RID: 5754
	public Vector3 centerPosition;

	// Token: 0x0400167B RID: 5755
	public bool worldSpace;

	// Token: 0x0400167C RID: 5756
	public float scaleX = 1f;

	// Token: 0x0400167D RID: 5757
	public float timeScaleX = 1f;

	// Token: 0x0400167E RID: 5758
	public AnimationCurve movementX = new AnimationCurve();

	// Token: 0x0400167F RID: 5759
	public float scaleY = 1f;

	// Token: 0x04001680 RID: 5760
	public float timeScaleY = 1f;

	// Token: 0x04001681 RID: 5761
	public AnimationCurve movementY = new AnimationCurve();

	// Token: 0x04001682 RID: 5762
	public float scaleZ = 1f;

	// Token: 0x04001683 RID: 5763
	public float timeScaleZ = 1f;

	// Token: 0x04001684 RID: 5764
	public AnimationCurve movementZ = new AnimationCurve();
}
