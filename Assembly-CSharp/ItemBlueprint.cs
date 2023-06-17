using System;
using System.Collections.Generic;
using Rust;
using UnityEngine;

// Token: 0x020005C9 RID: 1481
public class ItemBlueprint : MonoBehaviour
{
	// Token: 0x0400243E RID: 9278
	public List<ItemAmount> ingredients = new List<ItemAmount>();

	// Token: 0x0400243F RID: 9279
	public List<ItemDefinition> additionalUnlocks = new List<ItemDefinition>();

	// Token: 0x04002440 RID: 9280
	public bool defaultBlueprint;

	// Token: 0x04002441 RID: 9281
	public bool userCraftable = true;

	// Token: 0x04002442 RID: 9282
	public bool isResearchable = true;

	// Token: 0x04002443 RID: 9283
	public bool forceShowInConveyorFilter;

	// Token: 0x04002444 RID: 9284
	public Rarity rarity;

	// Token: 0x04002445 RID: 9285
	[Header("Workbench")]
	public int workbenchLevelRequired;

	// Token: 0x04002446 RID: 9286
	[Header("Scrap")]
	public int scrapRequired;

	// Token: 0x04002447 RID: 9287
	public int scrapFromRecycle;

	// Token: 0x04002448 RID: 9288
	[Header("Unlocking")]
	[Tooltip("This item won't show anywhere unless you have the corresponding SteamItem in your inventory - which is defined on the ItemDefinition")]
	public bool NeedsSteamItem;

	// Token: 0x04002449 RID: 9289
	public int blueprintStackSize = -1;

	// Token: 0x0400244A RID: 9290
	public float time = 1f;

	// Token: 0x0400244B RID: 9291
	public int amountToCreate = 1;

	// Token: 0x0400244C RID: 9292
	public string UnlockAchievment;

	// Token: 0x0400244D RID: 9293
	public string RecycleStat;

	// Token: 0x170003AE RID: 942
	// (get) Token: 0x06002CCF RID: 11471 RVA: 0x0010F890 File Offset: 0x0010DA90
	public ItemDefinition targetItem
	{
		get
		{
			return base.GetComponent<ItemDefinition>();
		}
	}

	// Token: 0x170003AF RID: 943
	// (get) Token: 0x06002CD0 RID: 11472 RVA: 0x0010F898 File Offset: 0x0010DA98
	public bool NeedsSteamDLC
	{
		get
		{
			return this.targetItem.steamDlc != null;
		}
	}
}
