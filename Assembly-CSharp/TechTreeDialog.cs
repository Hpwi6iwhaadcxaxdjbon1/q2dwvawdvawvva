using System;
using System.Collections.Generic;
using Rust.UI;
using UnityEngine;

// Token: 0x020007D5 RID: 2005
public class TechTreeDialog : UIDialog, IInventoryChanged
{
	// Token: 0x04002CCB RID: 11467
	public TechTreeData data;

	// Token: 0x04002CCC RID: 11468
	public float graphScale = 1f;

	// Token: 0x04002CCD RID: 11469
	public TechTreeEntry entryPrefab;

	// Token: 0x04002CCE RID: 11470
	public TechTreeGroup groupPrefab;

	// Token: 0x04002CCF RID: 11471
	public TechTreeLine linePrefab;

	// Token: 0x04002CD0 RID: 11472
	public RectTransform contents;

	// Token: 0x04002CD1 RID: 11473
	public RectTransform contentParent;

	// Token: 0x04002CD2 RID: 11474
	public TechTreeSelectedNodeUI selectedNodeUI;

	// Token: 0x04002CD3 RID: 11475
	public float nodeSize = 128f;

	// Token: 0x04002CD4 RID: 11476
	public float gridSize = 64f;

	// Token: 0x04002CD5 RID: 11477
	public GameObjectRef unlockEffect;

	// Token: 0x04002CD6 RID: 11478
	public RustText scrapCount;

	// Token: 0x04002CD7 RID: 11479
	private Vector2 startPos = Vector2.zero;

	// Token: 0x04002CD8 RID: 11480
	public List<int> processed = new List<int>();

	// Token: 0x04002CD9 RID: 11481
	public Dictionary<int, TechTreeWidget> widgets = new Dictionary<int, TechTreeWidget>();

	// Token: 0x04002CDA RID: 11482
	public List<TechTreeLine> lines = new List<TechTreeLine>();

	// Token: 0x04002CDB RID: 11483
	public ScrollRectZoom zoom;
}
