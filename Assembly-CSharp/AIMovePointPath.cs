using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020001DF RID: 479
public class AIMovePointPath : MonoBehaviour
{
	// Token: 0x0400124C RID: 4684
	public Color DebugPathColor = Color.green;

	// Token: 0x0400124D RID: 4685
	public AIMovePointPath.Mode LoopMode;

	// Token: 0x0400124E RID: 4686
	public List<AIMovePoint> Points = new List<AIMovePoint>();

	// Token: 0x06001978 RID: 6520 RVA: 0x000BB202 File Offset: 0x000B9402
	public void Clear()
	{
		this.Points.Clear();
	}

	// Token: 0x06001979 RID: 6521 RVA: 0x000BB20F File Offset: 0x000B940F
	public void AddPoint(AIMovePoint point)
	{
		this.Points.Add(point);
	}

	// Token: 0x0600197A RID: 6522 RVA: 0x000BB21D File Offset: 0x000B941D
	public AIMovePoint FindNearestPoint(Vector3 position)
	{
		return this.Points[this.FindNearestPointIndex(position)];
	}

	// Token: 0x0600197B RID: 6523 RVA: 0x000BB234 File Offset: 0x000B9434
	public int FindNearestPointIndex(Vector3 position)
	{
		float num = float.MaxValue;
		int result = 0;
		int num2 = 0;
		foreach (AIMovePoint aimovePoint in this.Points)
		{
			float num3 = Vector3.SqrMagnitude(position - aimovePoint.transform.position);
			if (num3 < num)
			{
				num = num3;
				result = num2;
			}
			num2++;
		}
		return result;
	}

	// Token: 0x0600197C RID: 6524 RVA: 0x000BB2B4 File Offset: 0x000B94B4
	public AIMovePoint GetPointAtIndex(int index)
	{
		if (index < 0 || index >= this.Points.Count)
		{
			return null;
		}
		return this.Points[index];
	}

	// Token: 0x0600197D RID: 6525 RVA: 0x000BB2D8 File Offset: 0x000B94D8
	public int GetNextPointIndex(int currentPointIndex, ref AIMovePointPath.PathDirection pathDirection)
	{
		int num = currentPointIndex + ((pathDirection == AIMovePointPath.PathDirection.Forwards) ? 1 : -1);
		if (num < 0)
		{
			if (this.LoopMode == AIMovePointPath.Mode.Loop)
			{
				num = this.Points.Count - 1;
			}
			else
			{
				num = 1;
				pathDirection = AIMovePointPath.PathDirection.Forwards;
			}
		}
		else if (num >= this.Points.Count)
		{
			if (this.LoopMode == AIMovePointPath.Mode.Loop)
			{
				num = 0;
			}
			else
			{
				num = this.Points.Count - 2;
				pathDirection = AIMovePointPath.PathDirection.Backwards;
			}
		}
		return num;
	}

	// Token: 0x0600197E RID: 6526 RVA: 0x000BB340 File Offset: 0x000B9540
	private void OnDrawGizmos()
	{
		Color color = Gizmos.color;
		Gizmos.color = this.DebugPathColor;
		int num = -1;
		foreach (AIMovePoint aimovePoint in this.Points)
		{
			num++;
			if (!(aimovePoint == null))
			{
				if (num + 1 < this.Points.Count)
				{
					Gizmos.DrawLine(aimovePoint.transform.position, this.Points[num + 1].transform.position);
				}
				else if (this.LoopMode == AIMovePointPath.Mode.Loop)
				{
					Gizmos.DrawLine(aimovePoint.transform.position, this.Points[0].transform.position);
				}
			}
		}
		Gizmos.color = color;
	}

	// Token: 0x0600197F RID: 6527 RVA: 0x000BB420 File Offset: 0x000B9620
	private void OnDrawGizmosSelected()
	{
		if (this.Points == null)
		{
			return;
		}
		foreach (AIMovePoint aimovePoint in this.Points)
		{
			aimovePoint.DrawLookAtPoints();
		}
	}

	// Token: 0x06001980 RID: 6528 RVA: 0x000BB47C File Offset: 0x000B967C
	[ContextMenu("Add Child Points")]
	public void AddChildPoints()
	{
		this.Points = new List<AIMovePoint>();
		this.Points.AddRange(base.GetComponentsInChildren<AIMovePoint>());
	}

	// Token: 0x02000C3C RID: 3132
	public enum Mode
	{
		// Token: 0x0400429A RID: 17050
		Loop,
		// Token: 0x0400429B RID: 17051
		Reverse
	}

	// Token: 0x02000C3D RID: 3133
	public enum PathDirection
	{
		// Token: 0x0400429D RID: 17053
		Forwards,
		// Token: 0x0400429E RID: 17054
		Backwards
	}
}
