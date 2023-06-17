using System;
using UnityEngine;

// Token: 0x020001D0 RID: 464
public class FlameJet : MonoBehaviour
{
	// Token: 0x04001200 RID: 4608
	public LineRenderer line;

	// Token: 0x04001201 RID: 4609
	public float tesselation = 0.025f;

	// Token: 0x04001202 RID: 4610
	private float length;

	// Token: 0x04001203 RID: 4611
	public float maxLength = 2f;

	// Token: 0x04001204 RID: 4612
	public float drag;

	// Token: 0x04001205 RID: 4613
	private int numSegments;

	// Token: 0x04001206 RID: 4614
	private float spacing;

	// Token: 0x04001207 RID: 4615
	public bool on;

	// Token: 0x04001208 RID: 4616
	private Vector3[] lastWorldSegments;

	// Token: 0x04001209 RID: 4617
	private Vector3[] currentSegments = new Vector3[0];

	// Token: 0x0400120A RID: 4618
	public Color startColor;

	// Token: 0x0400120B RID: 4619
	public Color endColor;

	// Token: 0x0400120C RID: 4620
	public Color currentColor;

	// Token: 0x0600191B RID: 6427 RVA: 0x000B8DE4 File Offset: 0x000B6FE4
	private void Initialize()
	{
		this.currentColor = this.startColor;
		this.tesselation = 0.1f;
		this.numSegments = Mathf.CeilToInt(this.maxLength / this.tesselation);
		this.spacing = this.maxLength / (float)this.numSegments;
		if (this.currentSegments.Length != this.numSegments)
		{
			this.currentSegments = new Vector3[this.numSegments];
		}
	}

	// Token: 0x0600191C RID: 6428 RVA: 0x000B8E55 File Offset: 0x000B7055
	private void Awake()
	{
		this.Initialize();
	}

	// Token: 0x0600191D RID: 6429 RVA: 0x000B8E5D File Offset: 0x000B705D
	public void LateUpdate()
	{
		if (this.on || this.currentColor.a > 0f)
		{
			this.UpdateLine();
		}
	}

	// Token: 0x0600191E RID: 6430 RVA: 0x000B8E7F File Offset: 0x000B707F
	public void SetOn(bool isOn)
	{
		this.on = isOn;
	}

	// Token: 0x0600191F RID: 6431 RVA: 0x000B8E88 File Offset: 0x000B7088
	private float curve(float x)
	{
		return x * x;
	}

	// Token: 0x06001920 RID: 6432 RVA: 0x000B8E90 File Offset: 0x000B7090
	private void UpdateLine()
	{
		this.currentColor.a = Mathf.Lerp(this.currentColor.a, this.on ? 1f : 0f, Time.deltaTime * 40f);
		this.line.SetColors(this.currentColor, this.endColor);
		if (this.lastWorldSegments == null)
		{
			this.lastWorldSegments = new Vector3[this.numSegments];
		}
		int num = this.currentSegments.Length;
		for (int i = 0; i < num; i++)
		{
			float x = 0f;
			float y = 0f;
			if (this.lastWorldSegments != null && this.lastWorldSegments[i] != Vector3.zero && i > 0)
			{
				Vector3 a = base.transform.InverseTransformPoint(this.lastWorldSegments[i]);
				float f = (float)i / (float)this.currentSegments.Length;
				Vector3 vector = Vector3.Lerp(a, Vector3.zero, Time.deltaTime * this.drag);
				vector = Vector3.Lerp(Vector3.zero, vector, Mathf.Sqrt(f));
				x = vector.x;
				y = vector.y;
			}
			if (i == 0)
			{
				y = (x = 0f);
			}
			Vector3 vector2 = new Vector3(x, y, (float)i * this.spacing);
			this.currentSegments[i] = vector2;
			this.lastWorldSegments[i] = base.transform.TransformPoint(vector2);
		}
		this.line.positionCount = this.numSegments;
		this.line.SetPositions(this.currentSegments);
	}
}
