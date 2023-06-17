using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000948 RID: 2376
public class PathInterpolator
{
	// Token: 0x0400336A RID: 13162
	public Vector3[] Points;

	// Token: 0x0400336B RID: 13163
	public Vector3[] Tangents;

	// Token: 0x04003371 RID: 13169
	protected bool initialized;

	// Token: 0x17000484 RID: 1156
	// (get) Token: 0x06003907 RID: 14599 RVA: 0x00153A4F File Offset: 0x00151C4F
	// (set) Token: 0x06003908 RID: 14600 RVA: 0x00153A57 File Offset: 0x00151C57
	public int MinIndex { get; set; }

	// Token: 0x17000485 RID: 1157
	// (get) Token: 0x06003909 RID: 14601 RVA: 0x00153A60 File Offset: 0x00151C60
	// (set) Token: 0x0600390A RID: 14602 RVA: 0x00153A68 File Offset: 0x00151C68
	public int MaxIndex { get; set; }

	// Token: 0x17000486 RID: 1158
	// (get) Token: 0x0600390B RID: 14603 RVA: 0x00153A71 File Offset: 0x00151C71
	// (set) Token: 0x0600390C RID: 14604 RVA: 0x00153A79 File Offset: 0x00151C79
	public virtual float Length { get; private set; }

	// Token: 0x17000487 RID: 1159
	// (get) Token: 0x0600390D RID: 14605 RVA: 0x00153A82 File Offset: 0x00151C82
	// (set) Token: 0x0600390E RID: 14606 RVA: 0x00153A8A File Offset: 0x00151C8A
	public virtual float StepSize { get; private set; }

	// Token: 0x17000488 RID: 1160
	// (get) Token: 0x0600390F RID: 14607 RVA: 0x00153A93 File Offset: 0x00151C93
	// (set) Token: 0x06003910 RID: 14608 RVA: 0x00153A9B File Offset: 0x00151C9B
	public bool Circular { get; private set; }

	// Token: 0x17000489 RID: 1161
	// (get) Token: 0x06003911 RID: 14609 RVA: 0x00007A3C File Offset: 0x00005C3C
	public int DefaultMinIndex
	{
		get
		{
			return 0;
		}
	}

	// Token: 0x1700048A RID: 1162
	// (get) Token: 0x06003912 RID: 14610 RVA: 0x00153AA4 File Offset: 0x00151CA4
	public int DefaultMaxIndex
	{
		get
		{
			return this.Points.Length - 1;
		}
	}

	// Token: 0x1700048B RID: 1163
	// (get) Token: 0x06003913 RID: 14611 RVA: 0x00153AB0 File Offset: 0x00151CB0
	public float StartOffset
	{
		get
		{
			return this.Length * (float)(this.MinIndex - this.DefaultMinIndex) / (float)(this.DefaultMaxIndex - this.DefaultMinIndex);
		}
	}

	// Token: 0x1700048C RID: 1164
	// (get) Token: 0x06003914 RID: 14612 RVA: 0x00153AD6 File Offset: 0x00151CD6
	public float EndOffset
	{
		get
		{
			return this.Length * (float)(this.DefaultMaxIndex - this.MaxIndex) / (float)(this.DefaultMaxIndex - this.DefaultMinIndex);
		}
	}

	// Token: 0x06003915 RID: 14613 RVA: 0x00153AFC File Offset: 0x00151CFC
	public PathInterpolator(Vector3[] points)
	{
		if (points.Length < 2)
		{
			throw new ArgumentException("Point list too short.");
		}
		this.Points = points;
		this.MinIndex = this.DefaultMinIndex;
		this.MaxIndex = this.DefaultMaxIndex;
		this.Circular = (Vector3.Distance(points[0], points[points.Length - 1]) < 0.1f);
	}

	// Token: 0x06003916 RID: 14614 RVA: 0x00153B64 File Offset: 0x00151D64
	public PathInterpolator(Vector3[] points, Vector3[] tangents) : this(points)
	{
		if (tangents.Length != points.Length)
		{
			throw new ArgumentException(string.Concat(new object[]
			{
				"Points and tangents lengths must match. Points: ",
				points.Length,
				" Tangents: ",
				tangents.Length
			}));
		}
		this.Tangents = tangents;
		this.RecalculateLength();
		this.initialized = true;
	}

	// Token: 0x06003917 RID: 14615 RVA: 0x00153BCC File Offset: 0x00151DCC
	public void RecalculateTangents()
	{
		if (this.Tangents == null || this.Tangents.Length != this.Points.Length)
		{
			this.Tangents = new Vector3[this.Points.Length];
		}
		for (int i = 0; i < this.Points.Length; i++)
		{
			int num = i - 1;
			int num2 = i + 1;
			if (num < 0)
			{
				num = (this.Circular ? (this.Points.Length - 2) : 0);
			}
			if (num2 > this.Points.Length - 1)
			{
				num2 = (this.Circular ? 1 : (this.Points.Length - 1));
			}
			Vector3 b = this.Points[num];
			Vector3 a = this.Points[num2];
			this.Tangents[i] = (a - b).normalized;
		}
		this.RecalculateLength();
		this.initialized = true;
	}

	// Token: 0x06003918 RID: 14616 RVA: 0x00153CAC File Offset: 0x00151EAC
	public void RecalculateLength()
	{
		float num = 0f;
		for (int i = 0; i < this.Points.Length - 1; i++)
		{
			Vector3 b = this.Points[i];
			Vector3 a = this.Points[i + 1];
			num += (a - b).magnitude;
		}
		this.Length = num;
		this.StepSize = num / (float)this.Points.Length;
	}

	// Token: 0x06003919 RID: 14617 RVA: 0x00153D1C File Offset: 0x00151F1C
	public void Resample(float distance)
	{
		float num = 0f;
		for (int i = 0; i < this.Points.Length - 1; i++)
		{
			Vector3 b = this.Points[i];
			Vector3 a = this.Points[i + 1];
			num += (a - b).magnitude;
		}
		int num2 = Mathf.RoundToInt(num / distance);
		if (num2 < 2)
		{
			return;
		}
		distance = num / (float)(num2 - 1);
		List<Vector3> list = new List<Vector3>(num2);
		float num3 = 0f;
		for (int j = 0; j < this.Points.Length - 1; j++)
		{
			int num4 = j;
			int num5 = j + 1;
			Vector3 vector = this.Points[num4];
			Vector3 vector2 = this.Points[num5];
			float num6 = (vector2 - vector).magnitude;
			if (num4 == 0)
			{
				list.Add(vector);
			}
			while (num3 + num6 > distance)
			{
				float num7 = distance - num3;
				float t = num7 / num6;
				Vector3 vector3 = Vector3.Lerp(vector, vector2, t);
				list.Add(vector3);
				vector = vector3;
				num3 = 0f;
				num6 -= num7;
			}
			num3 += num6;
			if (num5 == this.Points.Length - 1 && num3 > distance * 0.5f)
			{
				list.Add(vector2);
			}
		}
		if (list.Count < 2)
		{
			return;
		}
		this.Points = list.ToArray();
		this.MinIndex = this.DefaultMinIndex;
		this.MaxIndex = this.DefaultMaxIndex;
		this.initialized = false;
	}

	// Token: 0x0600391A RID: 14618 RVA: 0x00153E9C File Offset: 0x0015209C
	public void Smoothen(int iterations, Func<int, float> filter = null)
	{
		this.Smoothen(iterations, Vector3.one, filter);
	}

	// Token: 0x0600391B RID: 14619 RVA: 0x00153EAC File Offset: 0x001520AC
	public void Smoothen(int iterations, Vector3 multipliers, Func<int, float> filter = null)
	{
		for (int i = 0; i < iterations; i++)
		{
			for (int j = this.MinIndex + (this.Circular ? 0 : 1); j <= this.MaxIndex - 1; j += 2)
			{
				this.SmoothenIndex(j, multipliers, filter);
			}
			for (int k = this.MinIndex + (this.Circular ? 1 : 2); k <= this.MaxIndex - 1; k += 2)
			{
				this.SmoothenIndex(k, multipliers, filter);
			}
		}
		this.initialized = false;
	}

	// Token: 0x0600391C RID: 14620 RVA: 0x00153F28 File Offset: 0x00152128
	private void SmoothenIndex(int i, Vector3 multipliers, Func<int, float> filter = null)
	{
		int num = i - 1;
		int num2 = i + 1;
		if (i == 0)
		{
			num = this.Points.Length - 2;
		}
		Vector3 a = this.Points[num];
		Vector3 vector = this.Points[i];
		Vector3 b = this.Points[num2];
		Vector3 vector2 = (a + vector + vector + b) * 0.25f;
		if (filter != null)
		{
			multipliers *= filter(i);
		}
		if (multipliers != Vector3.one)
		{
			vector2.x = Mathf.LerpUnclamped(vector.x, vector2.x, multipliers.x);
			vector2.y = Mathf.LerpUnclamped(vector.y, vector2.y, multipliers.y);
			vector2.z = Mathf.LerpUnclamped(vector.z, vector2.z, multipliers.z);
		}
		this.Points[i] = vector2;
		if (i == 0)
		{
			this.Points[this.Points.Length - 1] = this.Points[0];
		}
	}

	// Token: 0x0600391D RID: 14621 RVA: 0x00154041 File Offset: 0x00152241
	public Vector3 GetStartPoint()
	{
		return this.Points[this.MinIndex];
	}

	// Token: 0x0600391E RID: 14622 RVA: 0x00154054 File Offset: 0x00152254
	public Vector3 GetEndPoint()
	{
		return this.Points[this.MaxIndex];
	}

	// Token: 0x0600391F RID: 14623 RVA: 0x00154067 File Offset: 0x00152267
	public Vector3 GetStartTangent()
	{
		if (!this.initialized)
		{
			throw new Exception("Tangents have not been calculated yet or are outdated.");
		}
		return this.Tangents[this.MinIndex];
	}

	// Token: 0x06003920 RID: 14624 RVA: 0x0015408D File Offset: 0x0015228D
	public Vector3 GetEndTangent()
	{
		if (!this.initialized)
		{
			throw new Exception("Tangents have not been calculated yet or are outdated.");
		}
		return this.Tangents[this.MaxIndex];
	}

	// Token: 0x06003921 RID: 14625 RVA: 0x001540B4 File Offset: 0x001522B4
	public Vector3 GetPoint(float distance)
	{
		if (this.Length == 0f)
		{
			return this.GetStartPoint();
		}
		float num = distance / this.Length * (float)(this.Points.Length - 1);
		int num2 = (int)num;
		if (num <= (float)this.MinIndex)
		{
			return this.GetStartPoint();
		}
		if (num >= (float)this.MaxIndex)
		{
			return this.GetEndPoint();
		}
		Vector3 a = this.Points[num2];
		Vector3 b = this.Points[num2 + 1];
		float t = num - (float)num2;
		return Vector3.Lerp(a, b, t);
	}

	// Token: 0x06003922 RID: 14626 RVA: 0x00154138 File Offset: 0x00152338
	public virtual Vector3 GetTangent(float distance)
	{
		if (!this.initialized)
		{
			throw new Exception("Tangents have not been calculated yet or are outdated.");
		}
		if (this.Length == 0f)
		{
			return this.GetStartPoint();
		}
		float num = distance / this.Length * (float)(this.Tangents.Length - 1);
		int num2 = (int)num;
		if (num <= (float)this.MinIndex)
		{
			return this.GetStartTangent();
		}
		if (num >= (float)this.MaxIndex)
		{
			return this.GetEndTangent();
		}
		Vector3 a = this.Tangents[num2];
		Vector3 b = this.Tangents[num2 + 1];
		float t = num - (float)num2;
		return Vector3.Slerp(a, b, t);
	}

	// Token: 0x06003923 RID: 14627 RVA: 0x001541D0 File Offset: 0x001523D0
	public virtual Vector3 GetPointCubicHermite(float distance)
	{
		if (!this.initialized)
		{
			throw new Exception("Tangents have not been calculated yet or are outdated.");
		}
		if (this.Length == 0f)
		{
			return this.GetStartPoint();
		}
		float num = distance / this.Length * (float)(this.Points.Length - 1);
		int num2 = (int)num;
		if (num <= (float)this.MinIndex)
		{
			return this.GetStartPoint();
		}
		if (num >= (float)this.MaxIndex)
		{
			return this.GetEndPoint();
		}
		Vector3 a = this.Points[num2];
		Vector3 a2 = this.Points[num2 + 1];
		Vector3 a3 = this.Tangents[num2] * this.StepSize;
		Vector3 a4 = this.Tangents[num2 + 1] * this.StepSize;
		float num3 = num - (float)num2;
		float num4 = num3 * num3;
		float num5 = num3 * num4;
		return (2f * num5 - 3f * num4 + 1f) * a + (num5 - 2f * num4 + num3) * a3 + (-2f * num5 + 3f * num4) * a2 + (num5 - num4) * a4;
	}
}
