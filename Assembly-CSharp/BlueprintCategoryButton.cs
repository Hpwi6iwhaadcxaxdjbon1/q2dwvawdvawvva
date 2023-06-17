using System;
using TMPro;
using UnityEngine;

// Token: 0x02000811 RID: 2065
public class BlueprintCategoryButton : MonoBehaviour, IInventoryChanged
{
	// Token: 0x04002E52 RID: 11858
	public TextMeshProUGUI amountLabel;

	// Token: 0x04002E53 RID: 11859
	public ItemCategory Category;

	// Token: 0x04002E54 RID: 11860
	public bool AlwaysShow;

	// Token: 0x04002E55 RID: 11861
	public bool ShowItemCount = true;

	// Token: 0x04002E56 RID: 11862
	public GameObject BackgroundHighlight;

	// Token: 0x04002E57 RID: 11863
	public SoundDefinition clickSound;

	// Token: 0x04002E58 RID: 11864
	public SoundDefinition hoverSound;
}
