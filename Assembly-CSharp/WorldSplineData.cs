using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Token: 0x02000955 RID: 2389
[Serializable]
public class WorldSplineData
{
	// Token: 0x04003392 RID: 13202
	public Vector3[] inputPoints;

	// Token: 0x04003393 RID: 13203
	public Vector3[] inputTangents;

	// Token: 0x04003394 RID: 13204
	public float inputLUTInterval;

	// Token: 0x04003395 RID: 13205
	public List<WorldSplineData.LUTEntry> LUTValues;

	// Token: 0x04003396 RID: 13206
	public float Length;

	// Token: 0x04003397 RID: 13207
	[SerializeField]
	private int maxPointsIndex;

	// Token: 0x06003979 RID: 14713 RVA: 0x00155898 File Offset: 0x00153A98
	public WorldSplineData(WorldSpline worldSpline)
	{
		worldSpline.CheckValidity();
		this.LUTValues = new List<WorldSplineData.LUTEntry>();
		this.inputPoints = new Vector3[worldSpline.points.Length];
		worldSpline.points.CopyTo(this.inputPoints, 0);
		this.inputTangents = new Vector3[worldSpline.tangents.Length];
		worldSpline.tangents.CopyTo(this.inputTangents, 0);
		this.inputLUTInterval = worldSpline.lutInterval;
		this.maxPointsIndex = this.inputPoints.Length - 1;
		this.CreateLookupTable(worldSpline);
	}

	// Token: 0x0600397A RID: 14714 RVA: 0x00155929 File Offset: 0x00153B29
	public bool IsSameAs(WorldSpline worldSpline)
	{
		return this.inputPoints.SequenceEqual(worldSpline.points) && this.inputTangents.SequenceEqual(worldSpline.tangents) && this.inputLUTInterval == worldSpline.lutInterval;
	}

	// Token: 0x0600397B RID: 14715 RVA: 0x00155961 File Offset: 0x00153B61
	public bool IsDifferentTo(WorldSpline worldSpline)
	{
		return !this.IsSameAs(worldSpline);
	}

	// Token: 0x0600397C RID: 14716 RVA: 0x0015596D File Offset: 0x00153B6D
	public Vector3 GetStartPoint()
	{
		return this.inputPoints[0];
	}

	// Token: 0x0600397D RID: 14717 RVA: 0x0015597B File Offset: 0x00153B7B
	public Vector3 GetEndPoint()
	{
		return this.inputPoints[this.maxPointsIndex];
	}

	// Token: 0x0600397E RID: 14718 RVA: 0x0015598E File Offset: 0x00153B8E
	public Vector3 GetStartTangent()
	{
		return this.inputTangents[0];
	}

	// Token: 0x0600397F RID: 14719 RVA: 0x0015599C File Offset: 0x00153B9C
	public Vector3 GetEndTangent()
	{
		return this.inputTangents[this.maxPointsIndex];
	}

	// Token: 0x06003980 RID: 14720 RVA: 0x001559B0 File Offset: 0x00153BB0
	public Vector3 GetPointCubicHermite(float distance)
	{
		Vector3 vector;
		return this.GetPointAndTangentCubicHermite(distance, out vector);
	}

	// Token: 0x06003981 RID: 14721 RVA: 0x001559C8 File Offset: 0x00153BC8
	public Vector3 GetTangentCubicHermite(float distance)
	{
		Vector3 result;
		this.GetPointAndTangentCubicHermite(distance, out result);
		return result;
	}

	// Token: 0x06003982 RID: 14722 RVA: 0x001559E0 File Offset: 0x00153BE0
	public Vector3 GetPointAndTangentCubicHermite(float distance, out Vector3 tangent)
	{
		if (distance <= 0f)
		{
			tangent = this.GetStartTangent();
			return this.GetStartPoint();
		}
		if (distance >= this.Length)
		{
			tangent = this.GetEndTangent();
			return this.GetEndPoint();
		}
		int num = Mathf.FloorToInt(distance);
		if (this.LUTValues.Count > num)
		{
			int num2 = -1;
			while (num2 < 0 && (float)num > 0f)
			{
				WorldSplineData.LUTEntry lutentry = this.LUTValues[num];
				int num3 = 0;
				while (num3 < lutentry.points.Count && lutentry.points[num3].distance <= distance)
				{
					num2 = num3;
					num3++;
				}
				if (num2 < 0)
				{
					num--;
				}
			}
			float a;
			Vector3 vector;
			if (num2 < 0)
			{
				a = 0f;
				vector = this.GetStartPoint();
			}
			else
			{
				WorldSplineData.LUTEntry.LUTPoint lutpoint = this.LUTValues[num].points[num2];
				a = lutpoint.distance;
				vector = lutpoint.pos;
			}
			num2 = -1;
			while (num2 < 0 && num < this.LUTValues.Count)
			{
				WorldSplineData.LUTEntry lutentry2 = this.LUTValues[num];
				for (int i = 0; i < lutentry2.points.Count; i++)
				{
					if (lutentry2.points[i].distance > distance)
					{
						num2 = i;
						break;
					}
				}
				if (num2 < 0)
				{
					num++;
				}
			}
			float b;
			Vector3 vector2;
			if (num2 < 0)
			{
				b = this.Length;
				vector2 = this.GetEndPoint();
			}
			else
			{
				WorldSplineData.LUTEntry.LUTPoint lutpoint2 = this.LUTValues[num].points[num2];
				b = lutpoint2.distance;
				vector2 = lutpoint2.pos;
			}
			float t = Mathf.InverseLerp(a, b, distance);
			tangent = (vector2 - vector).normalized;
			return Vector3.Lerp(vector, vector2, t);
		}
		tangent = this.GetEndTangent();
		return this.GetEndPoint();
	}

	// Token: 0x06003983 RID: 14723 RVA: 0x00155BAC File Offset: 0x00153DAC
	public void SetDefaultTangents(WorldSpline worldSpline)
	{
		PathInterpolator pathInterpolator = new PathInterpolator(worldSpline.points, worldSpline.tangents);
		pathInterpolator.RecalculateTangents();
		worldSpline.tangents = pathInterpolator.Tangents;
	}

	// Token: 0x06003984 RID: 14724 RVA: 0x00155BE0 File Offset: 0x00153DE0
	public bool DetectSplineProblems(WorldSpline worldSpline)
	{
		bool result = false;
		Vector3 to = this.GetTangentCubicHermite(0f);
		for (float num = 0.05f; num <= this.Length; num += 0.05f)
		{
			Vector3 tangentCubicHermite = this.GetTangentCubicHermite(num);
			float num2 = Vector3.Angle(tangentCubicHermite, to);
			if (num2 > 5f)
			{
				if (worldSpline != null)
				{
					Vector3 dir;
					Vector3 pointAndTangentCubicHermiteWorld = worldSpline.GetPointAndTangentCubicHermiteWorld(num, out dir);
					Debug.DrawRay(pointAndTangentCubicHermiteWorld, dir, Color.red, 30f);
					Debug.DrawRay(pointAndTangentCubicHermiteWorld, Vector3.up, Color.red, 30f);
				}
				Debug.Log(string.Format("Spline may have a too-sharp bend at {0:P0}. Angle change: ", num / this.Length) + num2);
				result = true;
			}
			to = tangentCubicHermite;
		}
		return result;
	}

	// Token: 0x06003985 RID: 14725 RVA: 0x00155C94 File Offset: 0x00153E94
	private void CreateLookupTable(WorldSpline worldSpline)
	{
		PathInterpolator pathInterpolator = new PathInterpolator(worldSpline.points, worldSpline.tangents);
		Vector3 b = pathInterpolator.GetPointCubicHermite(0f);
		this.Length = 0f;
		this.AddEntry(0f, this.GetStartPoint());
		Vector3 vector;
		for (float num = worldSpline.lutInterval; num < pathInterpolator.Length; num += worldSpline.lutInterval)
		{
			vector = pathInterpolator.GetPointCubicHermite(num);
			this.Length += Vector3.Distance(vector, b);
			this.AddEntry(this.Length, pathInterpolator.GetPointCubicHermite(num));
			b = vector;
		}
		vector = this.GetEndPoint();
		this.Length += Vector3.Distance(vector, b);
		this.AddEntry(this.Length, vector);
	}

	// Token: 0x06003986 RID: 14726 RVA: 0x00155D50 File Offset: 0x00153F50
	private void AddEntry(float distance, Vector3 pos)
	{
		int num = Mathf.FloorToInt(distance);
		if (this.LUTValues.Count < num + 1)
		{
			for (int i = this.LUTValues.Count; i < num + 1; i++)
			{
				this.LUTValues.Add(new WorldSplineData.LUTEntry());
			}
		}
		this.LUTValues[num].points.Add(new WorldSplineData.LUTEntry.LUTPoint(distance, pos));
	}

	// Token: 0x02000EC3 RID: 3779
	[Serializable]
	public class LUTEntry
	{
		// Token: 0x04004CDC RID: 19676
		public List<WorldSplineData.LUTEntry.LUTPoint> points = new List<WorldSplineData.LUTEntry.LUTPoint>();

		// Token: 0x02000FD7 RID: 4055
		[Serializable]
		public struct LUTPoint
		{
			// Token: 0x04005108 RID: 20744
			public float distance;

			// Token: 0x04005109 RID: 20745
			public Vector3 pos;

			// Token: 0x060055A7 RID: 21927 RVA: 0x001BA993 File Offset: 0x001B8B93
			public LUTPoint(float distance, Vector3 pos)
			{
				this.distance = distance;
				this.pos = pos;
			}
		}
	}
}
