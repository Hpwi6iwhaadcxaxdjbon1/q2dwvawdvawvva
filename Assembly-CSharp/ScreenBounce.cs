using System;
using UnityEngine;

// Token: 0x02000354 RID: 852
public class ScreenBounce : BaseScreenShake
{
	// Token: 0x040018A2 RID: 6306
	public AnimationCurve bounceScale;

	// Token: 0x040018A3 RID: 6307
	public AnimationCurve bounceSpeed;

	// Token: 0x040018A4 RID: 6308
	public AnimationCurve bounceViewmodel;

	// Token: 0x040018A5 RID: 6309
	private float bounceTime;

	// Token: 0x040018A6 RID: 6310
	private Vector3 bounceVelocity = Vector3.zero;

	// Token: 0x06001F39 RID: 7993 RVA: 0x000D3726 File Offset: 0x000D1926
	public override void Setup()
	{
		this.bounceTime = UnityEngine.Random.Range(0f, 1000f);
	}

	// Token: 0x06001F3A RID: 7994 RVA: 0x000D3740 File Offset: 0x000D1940
	public override void Run(float delta, ref CachedTransform<Camera> cam, ref CachedTransform<BaseViewModel> vm)
	{
		this.bounceTime += Time.deltaTime * this.bounceSpeed.Evaluate(delta);
		float num = this.bounceScale.Evaluate(delta) * 0.1f;
		this.bounceVelocity.x = Mathf.Sin(this.bounceTime * 20f) * num;
		this.bounceVelocity.y = Mathf.Cos(this.bounceTime * 25f) * num;
		this.bounceVelocity.z = 0f;
		Vector3 vector = Vector3.zero;
		vector += this.bounceVelocity.x * cam.right;
		vector += this.bounceVelocity.y * cam.up;
		if (cam)
		{
			cam.position += vector;
		}
		if (vm)
		{
			vm.position += vector * -1f * this.bounceViewmodel.Evaluate(delta);
		}
	}
}
