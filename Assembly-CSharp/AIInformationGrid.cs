using System;
using UnityEngine;

// Token: 0x020001DB RID: 475
public class AIInformationGrid : MonoBehaviour
{
	// Token: 0x04001220 RID: 4640
	public int CellSize = 10;

	// Token: 0x04001221 RID: 4641
	public Bounds BoundingBox;

	// Token: 0x04001222 RID: 4642
	public AIInformationCell[] Cells;

	// Token: 0x04001223 RID: 4643
	private Vector3 origin;

	// Token: 0x04001224 RID: 4644
	private int xCellCount;

	// Token: 0x04001225 RID: 4645
	private int zCellCount;

	// Token: 0x04001226 RID: 4646
	private const int maxPointResults = 2048;

	// Token: 0x04001227 RID: 4647
	private AIMovePoint[] movePointResults = new AIMovePoint[2048];

	// Token: 0x04001228 RID: 4648
	private AICoverPoint[] coverPointResults = new AICoverPoint[2048];

	// Token: 0x04001229 RID: 4649
	private const int maxCellResults = 512;

	// Token: 0x0400122A RID: 4650
	private AIInformationCell[] resultCells = new AIInformationCell[512];

	// Token: 0x0600193E RID: 6462 RVA: 0x000B97EC File Offset: 0x000B79EC
	[ContextMenu("Init")]
	public void Init()
	{
		AIInformationZone component = base.GetComponent<AIInformationZone>();
		if (component == null)
		{
			Debug.LogWarning("Unable to Init AIInformationGrid, no AIInformationZone found!");
			return;
		}
		this.BoundingBox = component.bounds;
		this.BoundingBox.center = base.transform.position + component.bounds.center + new Vector3(0f, this.BoundingBox.extents.y, 0f);
		float num = this.BoundingBox.extents.x * 2f;
		float num2 = this.BoundingBox.extents.z * 2f;
		this.xCellCount = (int)Mathf.Ceil(num / (float)this.CellSize);
		this.zCellCount = (int)Mathf.Ceil(num2 / (float)this.CellSize);
		this.Cells = new AIInformationCell[this.xCellCount * this.zCellCount];
		Vector3 min = this.BoundingBox.min;
		this.origin = min;
		min.x = this.BoundingBox.min.x + (float)this.CellSize / 2f;
		min.z = this.BoundingBox.min.z + (float)this.CellSize / 2f;
		for (int i = 0; i < this.zCellCount; i++)
		{
			for (int j = 0; j < this.xCellCount; j++)
			{
				Vector3 center = min;
				Bounds bounds = new Bounds(center, new Vector3((float)this.CellSize, this.BoundingBox.extents.y * 2f, (float)this.CellSize));
				this.Cells[this.GetIndex(j, i)] = new AIInformationCell(bounds, base.gameObject, j, i);
				min.x += (float)this.CellSize;
			}
			min.x = this.BoundingBox.min.x + (float)this.CellSize / 2f;
			min.z += (float)this.CellSize;
		}
	}

	// Token: 0x0600193F RID: 6463 RVA: 0x000B9A07 File Offset: 0x000B7C07
	private int GetIndex(int x, int z)
	{
		return z * this.xCellCount + x;
	}

	// Token: 0x06001940 RID: 6464 RVA: 0x000B9A13 File Offset: 0x000B7C13
	public AIInformationCell CellAt(int x, int z)
	{
		return this.Cells[this.GetIndex(x, z)];
	}

	// Token: 0x06001941 RID: 6465 RVA: 0x000B9A24 File Offset: 0x000B7C24
	public AIMovePoint[] GetMovePointsInRange(Vector3 position, float maxRange, out int pointCount)
	{
		pointCount = 0;
		int num;
		AIInformationCell[] cellsInRange = this.GetCellsInRange(position, maxRange, out num);
		if (num > 0)
		{
			for (int i = 0; i < num; i++)
			{
				if (cellsInRange[i] != null)
				{
					foreach (AIMovePoint aimovePoint in cellsInRange[i].MovePoints.Items)
					{
						this.movePointResults[pointCount] = aimovePoint;
						pointCount++;
					}
				}
			}
		}
		return this.movePointResults;
	}

	// Token: 0x06001942 RID: 6466 RVA: 0x000B9AB4 File Offset: 0x000B7CB4
	public AICoverPoint[] GetCoverPointsInRange(Vector3 position, float maxRange, out int pointCount)
	{
		pointCount = 0;
		int num;
		AIInformationCell[] cellsInRange = this.GetCellsInRange(position, maxRange, out num);
		if (num > 0)
		{
			for (int i = 0; i < num; i++)
			{
				if (cellsInRange[i] != null)
				{
					foreach (AICoverPoint aicoverPoint in cellsInRange[i].CoverPoints.Items)
					{
						this.coverPointResults[pointCount] = aicoverPoint;
						pointCount++;
					}
				}
			}
		}
		return this.coverPointResults;
	}

	// Token: 0x06001943 RID: 6467 RVA: 0x000B9B44 File Offset: 0x000B7D44
	public AIInformationCell[] GetCellsInRange(Vector3 position, float maxRange, out int cellCount)
	{
		cellCount = 0;
		int num = (int)(maxRange / (float)this.CellSize);
		AIInformationCell cell = this.GetCell(position);
		if (cell == null)
		{
			return this.resultCells;
		}
		int num2 = Mathf.Max(cell.X - num, 0);
		int num3 = Mathf.Min(cell.X + num, this.xCellCount - 1);
		int num4 = Mathf.Max(cell.Z - num, 0);
		int num5 = Mathf.Min(cell.Z + num, this.zCellCount - 1);
		for (int i = num4; i <= num5; i++)
		{
			for (int j = num2; j <= num3; j++)
			{
				this.resultCells[cellCount] = this.CellAt(j, i);
				cellCount++;
				if (cellCount >= 512)
				{
					return this.resultCells;
				}
			}
		}
		return this.resultCells;
	}

	// Token: 0x06001944 RID: 6468 RVA: 0x000B9C10 File Offset: 0x000B7E10
	public AIInformationCell GetCell(Vector3 position)
	{
		if (this.Cells == null)
		{
			return null;
		}
		Vector3 vector = position - this.origin;
		if (vector.x < 0f || vector.z < 0f)
		{
			return null;
		}
		int num = (int)(vector.x / (float)this.CellSize);
		int num2 = (int)(vector.z / (float)this.CellSize);
		if (num < 0 || num >= this.xCellCount)
		{
			return null;
		}
		if (num2 < 0 || num2 >= this.zCellCount)
		{
			return null;
		}
		return this.CellAt(num, num2);
	}

	// Token: 0x06001945 RID: 6469 RVA: 0x000B9C96 File Offset: 0x000B7E96
	public void OnDrawGizmos()
	{
		this.DebugDraw();
	}

	// Token: 0x06001946 RID: 6470 RVA: 0x000B9CA0 File Offset: 0x000B7EA0
	public void DebugDraw()
	{
		if (this.Cells == null)
		{
			return;
		}
		foreach (AIInformationCell aiinformationCell in this.Cells)
		{
			if (aiinformationCell != null)
			{
				aiinformationCell.DebugDraw(Color.white, false, 1f);
			}
		}
	}
}
