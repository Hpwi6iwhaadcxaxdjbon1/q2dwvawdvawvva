using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000813 RID: 2067
public class BlueprintHeader : MonoBehaviour
{
	// Token: 0x04002E5F RID: 11871
	public Text categoryName;

	// Token: 0x04002E60 RID: 11872
	public Text unlockCount;

	// Token: 0x060035CF RID: 13775 RVA: 0x001482C4 File Offset: 0x001464C4
	public void Setup(ItemCategory name, int unlocked, int total)
	{
		this.categoryName.text = name.ToString().ToUpper();
		this.unlockCount.text = string.Format("UNLOCKED {0}/{1}", unlocked, total);
	}
}
