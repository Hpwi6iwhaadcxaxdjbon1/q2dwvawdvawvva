using System;
using UnityEngine;

// Token: 0x02000357 RID: 855
public class ScreenRotate : BaseScreenShake
{
	// Token: 0x040018B1 RID: 6321
	public AnimationCurve Pitch;

	// Token: 0x040018B2 RID: 6322
	public AnimationCurve Yaw;

	// Token: 0x040018B3 RID: 6323
	public AnimationCurve Roll;

	// Token: 0x040018B4 RID: 6324
	public AnimationCurve ViewmodelEffect;

	// Token: 0x040018B5 RID: 6325
	public float scale = 1f;

	// Token: 0x040018B6 RID: 6326
	public bool useViewModelEffect = true;

	// Token: 0x06001F42 RID: 8002 RVA: 0x000063A5 File Offset: 0x000045A5
	public override void Setup()
	{
	}

	// Token: 0x06001F43 RID: 8003 RVA: 0x000D3A88 File Offset: 0x000D1C88
	public override void Run(float delta, ref CachedTransform<Camera> cam, ref CachedTransform<BaseViewModel> vm)
	{
		Vector3 zero = Vector3.zero;
		zero.x = this.Pitch.Evaluate(delta);
		zero.y = this.Yaw.Evaluate(delta);
		zero.z = this.Roll.Evaluate(delta);
		if (cam)
		{
			cam.rotation *= Quaternion.Euler(zero * this.scale);
		}
		if (vm && this.useViewModelEffect)
		{
			vm.rotation *= Quaternion.Euler(zero * this.scale * -1f * (1f - this.ViewmodelEffect.Evaluate(delta)));
		}
	}
}
