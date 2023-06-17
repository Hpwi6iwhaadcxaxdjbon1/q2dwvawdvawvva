using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000855 RID: 2133
public class UIInventory : SingletonComponent<UIInventory>
{
	// Token: 0x04002FD2 RID: 12242
	public TextMeshProUGUI PlayerName;

	// Token: 0x04002FD3 RID: 12243
	public static bool isOpen;

	// Token: 0x04002FD4 RID: 12244
	public static float LastOpened;

	// Token: 0x04002FD5 RID: 12245
	public VerticalLayoutGroup rightContents;

	// Token: 0x04002FD6 RID: 12246
	public GameObject QuickCraft;

	// Token: 0x04002FD7 RID: 12247
	public Transform InventoryIconContainer;

	// Token: 0x04002FD8 RID: 12248
	public ChangelogPanel ChangelogPanel;

	// Token: 0x04002FD9 RID: 12249
	public ContactsPanel contactsPanel;
}
