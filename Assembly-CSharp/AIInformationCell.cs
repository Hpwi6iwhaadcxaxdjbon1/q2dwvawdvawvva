using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020001D9 RID: 473
public class AIInformationCell
{
	// Token: 0x04001219 RID: 4633
	public Bounds BoundingBox;

	// Token: 0x0400121A RID: 4634
	public List<AIInformationCell> NeighbourCells = new List<AIInformationCell>();

	// Token: 0x0400121B RID: 4635
	public AIInformationCellContents<AIMovePoint> MovePoints = new AIInformationCellContents<AIMovePoint>();

	// Token: 0x0400121C RID: 4636
	public AIInformationCellContents<AICoverPoint> CoverPoints = new AIInformationCellContents<AICoverPoint>();

	// Token: 0x1700021F RID: 543
	// (get) Token: 0x06001933 RID: 6451 RVA: 0x000B95C4 File Offset: 0x000B77C4
	public int X { get; }

	// Token: 0x17000220 RID: 544
	// (get) Token: 0x06001934 RID: 6452 RVA: 0x000B95CC File Offset: 0x000B77CC
	public int Z { get; }

	// Token: 0x06001935 RID: 6453 RVA: 0x000B95D4 File Offset: 0x000B77D4
	public AIInformationCell(Bounds bounds, GameObject root, int x, int z)
	{
		this.BoundingBox = bounds;
		this.X = x;
		this.Z = z;
		this.MovePoints.Init(bounds, root);
		this.CoverPoints.Init(bounds, root);
	}

	// Token: 0x06001936 RID: 6454 RVA: 0x000B9638 File Offset: 0x000B7838
	public void DebugDraw(Color color, bool points, float scale = 1f)
	{
		Color color2 = Gizmos.color;
		Gizmos.color = color;
		Gizmos.DrawWireCube(this.BoundingBox.center, this.BoundingBox.size * scale);
		Gizmos.color = color2;
		if (points)
		{
			foreach (AIMovePoint aimovePoint in this.MovePoints.Items)
			{
				Gizmos.DrawLine(this.BoundingBox.center, aimovePoint.transform.position);
			}
			foreach (AICoverPoint aicoverPoint in this.CoverPoints.Items)
			{
				Gizmos.DrawLine(this.BoundingBox.center, aicoverPoint.transform.position);
			}
		}
	}
}
