using System;
using Rust.UI;
using UnityEngine;

// Token: 0x0200078F RID: 1935
public class UIAIDesignerScreen : SingletonComponent<UIAIDesignerScreen>, IUIScreen
{
	// Token: 0x04002B03 RID: 11011
	public GameObject SaveEntityButton;

	// Token: 0x04002B04 RID: 11012
	public GameObject SaveServerButton;

	// Token: 0x04002B05 RID: 11013
	public GameObject SaveDefaultButton;

	// Token: 0x04002B06 RID: 11014
	public RustInput InputAIDescription;

	// Token: 0x04002B07 RID: 11015
	public RustText TextDefaultStateContainer;

	// Token: 0x04002B08 RID: 11016
	public Transform PrefabAddNewStateButton;

	// Token: 0x04002B09 RID: 11017
	public Transform StateContainer;

	// Token: 0x04002B0A RID: 11018
	public Transform PrefabState;

	// Token: 0x04002B0B RID: 11019
	public EnumListUI PopupList;

	// Token: 0x04002B0C RID: 11020
	public static EnumListUI EnumList;

	// Token: 0x04002B0D RID: 11021
	public NeedsCursor needsCursor;

	// Token: 0x04002B0E RID: 11022
	protected CanvasGroup canvasGroup;

	// Token: 0x04002B0F RID: 11023
	public GameObject RootPanel;
}
