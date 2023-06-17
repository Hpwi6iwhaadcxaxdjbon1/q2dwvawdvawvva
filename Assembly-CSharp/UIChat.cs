using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000797 RID: 1943
public class UIChat : PriorityListComponent<UIChat>
{
	// Token: 0x04002B2D RID: 11053
	public GameObject inputArea;

	// Token: 0x04002B2E RID: 11054
	public GameObject chatArea;

	// Token: 0x04002B2F RID: 11055
	public TMP_InputField inputField;

	// Token: 0x04002B30 RID: 11056
	public TextMeshProUGUI channelLabel;

	// Token: 0x04002B31 RID: 11057
	public ScrollRect scrollRect;

	// Token: 0x04002B32 RID: 11058
	public CanvasGroup canvasGroup;

	// Token: 0x04002B33 RID: 11059
	public GameObjectRef chatItemPlayer;

	// Token: 0x04002B34 RID: 11060
	public GameObject userPopup;

	// Token: 0x04002B35 RID: 11061
	public static bool isOpen;
}
