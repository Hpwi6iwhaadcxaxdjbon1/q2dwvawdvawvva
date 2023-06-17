using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000841 RID: 2113
public class LootGrid : MonoBehaviour
{
	// Token: 0x04002F3E RID: 12094
	public int Container;

	// Token: 0x04002F3F RID: 12095
	public int Offset;

	// Token: 0x04002F40 RID: 12096
	public int Count = 1;

	// Token: 0x04002F41 RID: 12097
	public GameObject ItemIconPrefab;

	// Token: 0x04002F42 RID: 12098
	public Sprite BackgroundImage;

	// Token: 0x04002F43 RID: 12099
	public ItemContainerSource Inventory;

	// Token: 0x04002F44 RID: 12100
	private List<ItemIcon> _icons = new List<ItemIcon>();

	// Token: 0x06003609 RID: 13833 RVA: 0x0014868C File Offset: 0x0014688C
	public void CreateInventory(ItemContainerSource inventory, int? slots = null, int? offset = null)
	{
		foreach (ItemIcon itemIcon in this._icons)
		{
			UnityEngine.Object.Destroy(itemIcon.gameObject);
		}
		this._icons.Clear();
		this.Inventory = inventory;
		this.Count = (slots ?? this.Count);
		this.Offset = (offset ?? this.Offset);
		for (int i = 0; i < this.Count; i++)
		{
			ItemIcon component = UnityEngine.Object.Instantiate<GameObject>(this.ItemIconPrefab, base.transform).GetComponent<ItemIcon>();
			component.slot = this.Offset + i;
			component.emptySlotBackgroundSprite = (this.BackgroundImage ?? component.emptySlotBackgroundSprite);
			component.containerSource = inventory;
			this._icons.Add(component);
		}
	}
}
