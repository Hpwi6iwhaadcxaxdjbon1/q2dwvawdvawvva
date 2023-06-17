using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000950 RID: 2384
public class TickInterpolator
{
	// Token: 0x0400337C RID: 13180
	private List<TickInterpolator.Segment> points = new List<TickInterpolator.Segment>();

	// Token: 0x0400337D RID: 13181
	private int index;

	// Token: 0x0400337E RID: 13182
	public float Length;

	// Token: 0x0400337F RID: 13183
	public Vector3 CurrentPoint;

	// Token: 0x04003380 RID: 13184
	public Vector3 StartPoint;

	// Token: 0x04003381 RID: 13185
	public Vector3 EndPoint;

	// Token: 0x17000493 RID: 1171
	// (get) Token: 0x0600394B RID: 14667 RVA: 0x00154D2E File Offset: 0x00152F2E
	public int Count
	{
		get
		{
			return this.points.Count;
		}
	}

	// Token: 0x0600394C RID: 14668 RVA: 0x00154D3B File Offset: 0x00152F3B
	public void Reset()
	{
		this.index = 0;
		this.CurrentPoint = this.StartPoint;
	}

	// Token: 0x0600394D RID: 14669 RVA: 0x00154D50 File Offset: 0x00152F50
	public void Reset(Vector3 point)
	{
		this.points.Clear();
		this.index = 0;
		this.Length = 0f;
		this.EndPoint = point;
		this.StartPoint = point;
		this.CurrentPoint = point;
	}

	// Token: 0x0600394E RID: 14670 RVA: 0x00154D94 File Offset: 0x00152F94
	public void AddPoint(Vector3 point)
	{
		TickInterpolator.Segment segment = new TickInterpolator.Segment(this.EndPoint, point);
		this.points.Add(segment);
		this.Length += segment.length;
		this.EndPoint = segment.point;
	}

	// Token: 0x0600394F RID: 14671 RVA: 0x00154DDC File Offset: 0x00152FDC
	public bool MoveNext(float distance)
	{
		float num = 0f;
		while (num < distance && this.index < this.points.Count)
		{
			TickInterpolator.Segment segment = this.points[this.index];
			this.CurrentPoint = segment.point;
			num += segment.length;
			this.index++;
		}
		return num > 0f;
	}

	// Token: 0x06003950 RID: 14672 RVA: 0x00154E45 File Offset: 0x00153045
	public bool HasNext()
	{
		return this.index < this.points.Count;
	}

	// Token: 0x06003951 RID: 14673 RVA: 0x00154E5C File Offset: 0x0015305C
	public void TransformEntries(Matrix4x4 matrix)
	{
		for (int i = 0; i < this.points.Count; i++)
		{
			TickInterpolator.Segment segment = this.points[i];
			segment.point = matrix.MultiplyPoint3x4(segment.point);
			this.points[i] = segment;
		}
		this.CurrentPoint = matrix.MultiplyPoint3x4(this.CurrentPoint);
		this.StartPoint = matrix.MultiplyPoint3x4(this.StartPoint);
		this.EndPoint = matrix.MultiplyPoint3x4(this.EndPoint);
	}

	// Token: 0x02000EC1 RID: 3777
	private struct Segment
	{
		// Token: 0x04004CD8 RID: 19672
		public Vector3 point;

		// Token: 0x04004CD9 RID: 19673
		public float length;

		// Token: 0x06005343 RID: 21315 RVA: 0x001B1F92 File Offset: 0x001B0192
		public Segment(Vector3 a, Vector3 b)
		{
			this.point = b;
			this.length = Vector3.Distance(a, b);
		}
	}
}
