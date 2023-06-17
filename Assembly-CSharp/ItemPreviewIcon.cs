using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000829 RID: 2089
public class ItemPreviewIcon : BaseMonoBehaviour, IInventoryChanged, IItemAmountChanged, IItemIconChanged
{
	// Token: 0x04002EDF RID: 11999
	public ItemContainerSource containerSource;

	// Token: 0x04002EE0 RID: 12000
	[Range(0f, 64f)]
	public int slot;

	// Token: 0x04002EE1 RID: 12001
	public bool setSlotFromSiblingIndex = true;

	// Token: 0x04002EE2 RID: 12002
	public CanvasGroup iconContents;

	// Token: 0x04002EE3 RID: 12003
	public Image iconImage;

	// Token: 0x04002EE4 RID: 12004
	public Text amountText;

	// Token: 0x04002EE5 RID: 12005
	[NonSerialized]
	public Item item;
}
