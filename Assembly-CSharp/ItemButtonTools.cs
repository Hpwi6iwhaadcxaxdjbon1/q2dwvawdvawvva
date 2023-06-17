using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000308 RID: 776
public class ItemButtonTools : MonoBehaviour
{
	// Token: 0x040017A0 RID: 6048
	public Image image;

	// Token: 0x040017A1 RID: 6049
	public ItemDefinition itemDef;

	// Token: 0x06001E91 RID: 7825 RVA: 0x000D0718 File Offset: 0x000CE918
	public void GiveSelf(int amount)
	{
		ConsoleSystem.Run(ConsoleSystem.Option.Client, "inventory.giveid", new object[]
		{
			this.itemDef.itemid,
			amount
		});
	}

	// Token: 0x06001E92 RID: 7826 RVA: 0x000D074C File Offset: 0x000CE94C
	public void GiveArmed()
	{
		ConsoleSystem.Run(ConsoleSystem.Option.Client, "inventory.givearm", new object[]
		{
			this.itemDef.itemid
		});
	}

	// Token: 0x06001E93 RID: 7827 RVA: 0x000063A5 File Offset: 0x000045A5
	public void GiveBlueprint()
	{
	}
}
