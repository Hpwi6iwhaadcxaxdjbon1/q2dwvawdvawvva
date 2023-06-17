using System;
using UnityEngine;

// Token: 0x0200094F RID: 2383
public class TickHistory
{
	// Token: 0x0400337B RID: 13179
	private Deque<Vector3> points = new Deque<Vector3>(8);

	// Token: 0x17000492 RID: 1170
	// (get) Token: 0x06003944 RID: 14660 RVA: 0x00154B9A File Offset: 0x00152D9A
	public int Count
	{
		get
		{
			return this.points.Count;
		}
	}

	// Token: 0x06003945 RID: 14661 RVA: 0x00154BA7 File Offset: 0x00152DA7
	public void Reset()
	{
		this.points.Clear();
	}

	// Token: 0x06003946 RID: 14662 RVA: 0x00154BB4 File Offset: 0x00152DB4
	public void Reset(Vector3 point)
	{
		this.Reset();
		this.AddPoint(point, -1);
	}

	// Token: 0x06003947 RID: 14663 RVA: 0x00154BC4 File Offset: 0x00152DC4
	public float Distance(BasePlayer player, Vector3 point)
	{
		if (this.points.Count == 0)
		{
			return player.Distance(point);
		}
		Vector3 position = player.transform.position;
		Quaternion rotation = player.transform.rotation;
		Bounds bounds = player.bounds;
		Matrix4x4 tickHistoryMatrix = player.tickHistoryMatrix;
		float num = float.MaxValue;
		for (int i = 0; i < this.points.Count; i++)
		{
			Vector3 point2 = tickHistoryMatrix.MultiplyPoint3x4(this.points[i]);
			Vector3 point3 = (i == this.points.Count - 1) ? position : tickHistoryMatrix.MultiplyPoint3x4(this.points[i + 1]);
			Line line = new Line(point2, point3);
			Vector3 position2 = line.ClosestPoint(point);
			OBB obb = new OBB(position2, rotation, bounds);
			num = Mathf.Min(num, obb.Distance(point));
		}
		return num;
	}

	// Token: 0x06003948 RID: 14664 RVA: 0x00154CA5 File Offset: 0x00152EA5
	public void AddPoint(Vector3 point, int limit = -1)
	{
		while (limit > 0 && this.points.Count >= limit)
		{
			this.points.PopFront();
		}
		this.points.PushBack(point);
	}

	// Token: 0x06003949 RID: 14665 RVA: 0x00154CD4 File Offset: 0x00152ED4
	public void TransformEntries(Matrix4x4 matrix)
	{
		for (int i = 0; i < this.points.Count; i++)
		{
			Vector3 vector = this.points[i];
			vector = matrix.MultiplyPoint3x4(vector);
			this.points[i] = vector;
		}
	}
}
