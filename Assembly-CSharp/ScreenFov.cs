using System;
using UnityEngine;

// Token: 0x02000356 RID: 854
public class ScreenFov : BaseScreenShake
{
	// Token: 0x040018B0 RID: 6320
	public AnimationCurve FovAdjustment;

	// Token: 0x06001F3F RID: 7999 RVA: 0x000063A5 File Offset: 0x000045A5
	public override void Setup()
	{
	}

	// Token: 0x06001F40 RID: 8000 RVA: 0x000D3A52 File Offset: 0x000D1C52
	public override void Run(float delta, ref CachedTransform<Camera> cam, ref CachedTransform<BaseViewModel> vm)
	{
		if (cam)
		{
			cam.component.fieldOfView += this.FovAdjustment.Evaluate(delta);
		}
	}
}
