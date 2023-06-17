using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200085C RID: 2140
public class VirtualItemIcon : MonoBehaviour
{
	// Token: 0x0400300C RID: 12300
	public ItemDefinition itemDef;

	// Token: 0x0400300D RID: 12301
	public int itemAmount;

	// Token: 0x0400300E RID: 12302
	public bool asBlueprint;

	// Token: 0x0400300F RID: 12303
	public Image iconImage;

	// Token: 0x04003010 RID: 12304
	public Image bpUnderlay;

	// Token: 0x04003011 RID: 12305
	public Text amountText;

	// Token: 0x04003012 RID: 12306
	public Text hoverText;

	// Token: 0x04003013 RID: 12307
	public CanvasGroup iconContents;

	// Token: 0x04003014 RID: 12308
	public Tooltip ToolTip;

	// Token: 0x04003015 RID: 12309
	public CanvasGroup conditionObject;

	// Token: 0x04003016 RID: 12310
	public Image conditionFill;

	// Token: 0x04003017 RID: 12311
	public Image maxConditionFill;

	// Token: 0x04003018 RID: 12312
	public Image cornerIcon;
}
