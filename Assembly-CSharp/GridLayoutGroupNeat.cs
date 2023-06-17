using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020008E0 RID: 2272
public class GridLayoutGroupNeat : GridLayoutGroup
{
	// Token: 0x06003790 RID: 14224 RVA: 0x0014DA38 File Offset: 0x0014BC38
	private float IdealCellWidth(float cellSize)
	{
		float num = base.rectTransform.rect.x + (float)(base.padding.left + base.padding.right) * 0.5f;
		float num2 = Mathf.Floor(num / cellSize);
		return num / num2 - this.m_Spacing.x;
	}

	// Token: 0x06003791 RID: 14225 RVA: 0x0014DA90 File Offset: 0x0014BC90
	public override void SetLayoutHorizontal()
	{
		Vector2 cellSize = this.m_CellSize;
		this.m_CellSize.x = this.IdealCellWidth(cellSize.x);
		base.SetLayoutHorizontal();
		this.m_CellSize = cellSize;
	}

	// Token: 0x06003792 RID: 14226 RVA: 0x0014DAC8 File Offset: 0x0014BCC8
	public override void SetLayoutVertical()
	{
		Vector2 cellSize = this.m_CellSize;
		this.m_CellSize.x = this.IdealCellWidth(cellSize.x);
		base.SetLayoutVertical();
		this.m_CellSize = cellSize;
	}
}
