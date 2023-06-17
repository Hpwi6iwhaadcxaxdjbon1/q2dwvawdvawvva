using System;
using UnityEngine.UI;

// Token: 0x020008E2 RID: 2274
public class NonDrawingGraphic : Graphic
{
	// Token: 0x06003799 RID: 14233 RVA: 0x000063A5 File Offset: 0x000045A5
	public override void SetMaterialDirty()
	{
	}

	// Token: 0x0600379A RID: 14234 RVA: 0x000063A5 File Offset: 0x000045A5
	public override void SetVerticesDirty()
	{
	}

	// Token: 0x0600379B RID: 14235 RVA: 0x0014DB2A File Offset: 0x0014BD2A
	protected override void OnPopulateMesh(VertexHelper vh)
	{
		vh.Clear();
	}
}
