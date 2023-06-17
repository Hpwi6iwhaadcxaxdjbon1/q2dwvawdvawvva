using System;
using UnityEngine;

// Token: 0x02000825 RID: 2085
public class ItemInformationPanel : MonoBehaviour
{
	// Token: 0x060035E7 RID: 13799 RVA: 0x00148449 File Offset: 0x00146649
	public virtual bool EligableForDisplay(ItemDefinition info)
	{
		Debug.LogWarning("ItemInformationPanel.EligableForDisplay");
		return false;
	}

	// Token: 0x060035E8 RID: 13800 RVA: 0x00148456 File Offset: 0x00146656
	public virtual void SetupForItem(ItemDefinition info, Item item = null)
	{
		Debug.LogWarning("ItemInformationPanel.SetupForItem");
	}
}
