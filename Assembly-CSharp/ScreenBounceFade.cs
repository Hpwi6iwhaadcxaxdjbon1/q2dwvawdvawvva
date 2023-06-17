using System;
using UnityEngine;

// Token: 0x02000355 RID: 853
public class ScreenBounceFade : BaseScreenShake
{
	// Token: 0x040018A7 RID: 6311
	public AnimationCurve bounceScale;

	// Token: 0x040018A8 RID: 6312
	public AnimationCurve bounceSpeed;

	// Token: 0x040018A9 RID: 6313
	public AnimationCurve bounceViewmodel;

	// Token: 0x040018AA RID: 6314
	public AnimationCurve distanceFalloff;

	// Token: 0x040018AB RID: 6315
	public AnimationCurve timeFalloff;

	// Token: 0x040018AC RID: 6316
	private float bounceTime;

	// Token: 0x040018AD RID: 6317
	private Vector3 bounceVelocity = Vector3.zero;

	// Token: 0x040018AE RID: 6318
	public float maxDistance = 10f;

	// Token: 0x040018AF RID: 6319
	public float scale = 1f;

	// Token: 0x06001F3C RID: 7996 RVA: 0x000D3881 File Offset: 0x000D1A81
	public override void Setup()
	{
		this.bounceTime = UnityEngine.Random.Range(0f, 1000f);
	}

	// Token: 0x06001F3D RID: 7997 RVA: 0x000D3898 File Offset: 0x000D1A98
	public override void Run(float delta, ref CachedTransform<Camera> cam, ref CachedTransform<BaseViewModel> vm)
	{
		float value = Vector3.Distance(cam.position, base.transform.position);
		float num = 1f - Mathf.InverseLerp(0f, this.maxDistance, value);
		this.bounceTime += Time.deltaTime * this.bounceSpeed.Evaluate(delta);
		float num2 = this.distanceFalloff.Evaluate(num);
		float num3 = this.bounceScale.Evaluate(delta) * 0.1f * num2 * this.scale * this.timeFalloff.Evaluate(delta);
		this.bounceVelocity.x = Mathf.Sin(this.bounceTime * 20f) * num3;
		this.bounceVelocity.y = Mathf.Cos(this.bounceTime * 25f) * num3;
		this.bounceVelocity.z = 0f;
		Vector3 vector = Vector3.zero;
		vector += this.bounceVelocity.x * cam.right;
		vector += this.bounceVelocity.y * cam.up;
		vector *= num;
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
