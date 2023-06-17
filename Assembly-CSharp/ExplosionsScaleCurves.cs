using System;
using UnityEngine;

// Token: 0x02000995 RID: 2453
public class ExplosionsScaleCurves : MonoBehaviour
{
	// Token: 0x040034A2 RID: 13474
	public AnimationCurve ScaleCurveX = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

	// Token: 0x040034A3 RID: 13475
	public AnimationCurve ScaleCurveY = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

	// Token: 0x040034A4 RID: 13476
	public AnimationCurve ScaleCurveZ = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

	// Token: 0x040034A5 RID: 13477
	public Vector3 GraphTimeMultiplier = Vector3.one;

	// Token: 0x040034A6 RID: 13478
	public Vector3 GraphScaleMultiplier = Vector3.one;

	// Token: 0x040034A7 RID: 13479
	private float startTime;

	// Token: 0x040034A8 RID: 13480
	private Transform t;

	// Token: 0x040034A9 RID: 13481
	private float evalX;

	// Token: 0x040034AA RID: 13482
	private float evalY;

	// Token: 0x040034AB RID: 13483
	private float evalZ;

	// Token: 0x06003A59 RID: 14937 RVA: 0x001592D5 File Offset: 0x001574D5
	private void Awake()
	{
		this.t = base.transform;
	}

	// Token: 0x06003A5A RID: 14938 RVA: 0x001592E3 File Offset: 0x001574E3
	private void OnEnable()
	{
		this.startTime = Time.time;
		this.evalX = 0f;
		this.evalY = 0f;
		this.evalZ = 0f;
	}

	// Token: 0x06003A5B RID: 14939 RVA: 0x00159314 File Offset: 0x00157514
	private void Update()
	{
		float num = Time.time - this.startTime;
		if (num <= this.GraphTimeMultiplier.x)
		{
			this.evalX = this.ScaleCurveX.Evaluate(num / this.GraphTimeMultiplier.x) * this.GraphScaleMultiplier.x;
		}
		if (num <= this.GraphTimeMultiplier.y)
		{
			this.evalY = this.ScaleCurveY.Evaluate(num / this.GraphTimeMultiplier.y) * this.GraphScaleMultiplier.y;
		}
		if (num <= this.GraphTimeMultiplier.z)
		{
			this.evalZ = this.ScaleCurveZ.Evaluate(num / this.GraphTimeMultiplier.z) * this.GraphScaleMultiplier.z;
		}
		this.t.localScale = new Vector3(this.evalX, this.evalY, this.evalZ);
	}
}
