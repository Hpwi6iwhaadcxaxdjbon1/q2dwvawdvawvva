using System;
using Rust.UI;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020004D6 RID: 1238
public class IndustrialFilterDialog : UIDialog
{
	// Token: 0x04002050 RID: 8272
	public GameObjectRef ItemPrefab;

	// Token: 0x04002051 RID: 8273
	public Transform ItemParent;

	// Token: 0x04002052 RID: 8274
	public GameObject ItemSearchParent;

	// Token: 0x04002053 RID: 8275
	public ItemSearchEntry ItemSearchEntryPrefab;

	// Token: 0x04002054 RID: 8276
	public VirtualItemIcon TargetItemIcon;

	// Token: 0x04002055 RID: 8277
	public GameObject TargetCategoryRoot;

	// Token: 0x04002056 RID: 8278
	public RustText TargetCategoryText;

	// Token: 0x04002057 RID: 8279
	public Image TargetCategoryImage;

	// Token: 0x04002058 RID: 8280
	public GameObject NoItemsPrompt;

	// Token: 0x04002059 RID: 8281
	public Rust.UI.Dropdown FilterModeDropdown;

	// Token: 0x0400205A RID: 8282
	public GameObject[] FilterModeExplanations;

	// Token: 0x0400205B RID: 8283
	public GameObject FilterModeBlocker;

	// Token: 0x0400205C RID: 8284
	public RustText FilterCountText;

	// Token: 0x0400205D RID: 8285
	public GameObject BufferRoot;

	// Token: 0x0400205E RID: 8286
	public GameObjectRef BufferItemPrefab;

	// Token: 0x0400205F RID: 8287
	public Transform BufferTransform;

	// Token: 0x04002060 RID: 8288
	public RustButton PasteButton;

	// Token: 0x04002061 RID: 8289
	public GameObject[] RegularCopyPasteButtons;

	// Token: 0x04002062 RID: 8290
	public GameObject[] JsonCopyPasteButtons;
}
