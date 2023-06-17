using System;
using Rust.UI;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200084F RID: 2127
public class SelectedItem : SingletonComponent<SelectedItem>, IInventoryChanged
{
	// Token: 0x04002FB5 RID: 12213
	public Image icon;

	// Token: 0x04002FB6 RID: 12214
	public Image iconSplitter;

	// Token: 0x04002FB7 RID: 12215
	public RustText title;

	// Token: 0x04002FB8 RID: 12216
	public RustText description;

	// Token: 0x04002FB9 RID: 12217
	public GameObject splitPanel;

	// Token: 0x04002FBA RID: 12218
	public GameObject itemProtection;

	// Token: 0x04002FBB RID: 12219
	public GameObject menuOption;

	// Token: 0x04002FBC RID: 12220
	public GameObject optionsParent;

	// Token: 0x04002FBD RID: 12221
	public GameObject innerPanelContainer;
}
