using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000105 RID: 261
public class ComputerMenu : UIDialog
{
	// Token: 0x04000DE8 RID: 3560
	public RectTransform bookmarkContainer;

	// Token: 0x04000DE9 RID: 3561
	public GameObject bookmarkPrefab;

	// Token: 0x04000DEA RID: 3562
	public List<RCBookmarkEntry> activeEntries = new List<RCBookmarkEntry>();
}
