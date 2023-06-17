using System;
using Rust.UI;
using UnityEngine;

// Token: 0x020007B2 RID: 1970
public class DemoShotListWidget : SingletonComponent<DemoShotListWidget>
{
	// Token: 0x04002B7C RID: 11132
	public GameObjectRef ShotListEntry;

	// Token: 0x04002B7D RID: 11133
	public GameObjectRef FolderEntry;

	// Token: 0x04002B7E RID: 11134
	public Transform ShotListParent;

	// Token: 0x04002B7F RID: 11135
	public RustInput FolderNameInput;

	// Token: 0x04002B80 RID: 11136
	public GameObject ShotsRoot;

	// Token: 0x04002B81 RID: 11137
	public GameObject NoShotsRoot;

	// Token: 0x04002B82 RID: 11138
	public GameObject TopUpArrow;

	// Token: 0x04002B83 RID: 11139
	public GameObject TopDownArrow;

	// Token: 0x04002B84 RID: 11140
	public Canvas DragCanvas;
}
