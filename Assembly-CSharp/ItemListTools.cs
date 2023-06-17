using System;
using System.Collections.Generic;
using System.Linq;
using Rust.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000309 RID: 777
public class ItemListTools : MonoBehaviour
{
	// Token: 0x040017A2 RID: 6050
	public GameObject categoryButton;

	// Token: 0x040017A3 RID: 6051
	public GameObject itemButton;

	// Token: 0x040017A4 RID: 6052
	public RustInput searchInputText;

	// Token: 0x040017A5 RID: 6053
	internal Button lastCategory;

	// Token: 0x040017A6 RID: 6054
	private IOrderedEnumerable<ItemDefinition> currentItems;

	// Token: 0x040017A7 RID: 6055
	private IOrderedEnumerable<ItemDefinition> allItems;

	// Token: 0x06001E95 RID: 7829 RVA: 0x000D0777 File Offset: 0x000CE977
	public void OnPanelOpened()
	{
		this.CacheAllItems();
		this.Refresh();
		this.searchInputText.InputField.ActivateInputField();
	}

	// Token: 0x06001E96 RID: 7830 RVA: 0x000D0795 File Offset: 0x000CE995
	private void OnOpenDevTools()
	{
		this.searchInputText.InputField.ActivateInputField();
	}

	// Token: 0x06001E97 RID: 7831 RVA: 0x000D07A7 File Offset: 0x000CE9A7
	private void CacheAllItems()
	{
		if (this.allItems != null)
		{
			return;
		}
		this.allItems = from x in ItemManager.GetItemDefinitions()
		orderby x.displayName.translated
		select x;
	}

	// Token: 0x06001E98 RID: 7832 RVA: 0x000D07E1 File Offset: 0x000CE9E1
	public void Refresh()
	{
		this.RebuildCategories();
	}

	// Token: 0x06001E99 RID: 7833 RVA: 0x000D07EC File Offset: 0x000CE9EC
	private void RebuildCategories()
	{
		for (int i = 0; i < this.categoryButton.transform.parent.childCount; i++)
		{
			Transform child = this.categoryButton.transform.parent.GetChild(i);
			if (!(child == this.categoryButton.transform))
			{
				GameManager.Destroy(child.gameObject, 0f);
			}
		}
		this.categoryButton.SetActive(true);
		foreach (IGrouping<ItemCategory, ItemDefinition> source in from x in ItemManager.GetItemDefinitions()
		group x by x.category into x
		orderby x.First<ItemDefinition>().category
		select x)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.categoryButton);
			gameObject.transform.SetParent(this.categoryButton.transform.parent, false);
			gameObject.GetComponentInChildren<TextMeshProUGUI>().text = source.First<ItemDefinition>().category.ToString();
			Button btn = gameObject.GetComponentInChildren<Button>();
			ItemDefinition[] itemArray = source.ToArray<ItemDefinition>();
			btn.onClick.AddListener(delegate()
			{
				if (this.lastCategory)
				{
					this.lastCategory.interactable = true;
				}
				this.lastCategory = btn;
				this.lastCategory.interactable = false;
				this.SwitchItemCategory(itemArray);
			});
			if (this.lastCategory == null)
			{
				this.lastCategory = btn;
				this.lastCategory.interactable = false;
				this.SwitchItemCategory(itemArray);
			}
		}
		this.categoryButton.SetActive(false);
	}

	// Token: 0x06001E9A RID: 7834 RVA: 0x000D09BC File Offset: 0x000CEBBC
	private void SwitchItemCategory(ItemDefinition[] defs)
	{
		this.currentItems = from x in defs
		orderby x.displayName.translated
		select x;
		this.searchInputText.Text = "";
		this.FilterItems(null);
	}

	// Token: 0x06001E9B RID: 7835 RVA: 0x000D0A0C File Offset: 0x000CEC0C
	public void FilterItems(string searchText)
	{
		if (this.itemButton == null)
		{
			return;
		}
		for (int i = 0; i < this.itemButton.transform.parent.childCount; i++)
		{
			Transform child = this.itemButton.transform.parent.GetChild(i);
			if (!(child == this.itemButton.transform))
			{
				GameManager.Destroy(child.gameObject, 0f);
			}
		}
		this.itemButton.SetActive(true);
		bool flag = !string.IsNullOrEmpty(searchText);
		string value = flag ? searchText.ToLower() : null;
		IEnumerable<ItemDefinition> enumerable = flag ? this.allItems : this.currentItems;
		int num = 0;
		foreach (ItemDefinition itemDefinition in enumerable)
		{
			if (!itemDefinition.hidden && (!flag || itemDefinition.displayName.translated.ToLower().Contains(value)))
			{
				GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.itemButton);
				gameObject.transform.SetParent(this.itemButton.transform.parent, false);
				gameObject.GetComponentInChildren<TextMeshProUGUI>().text = itemDefinition.displayName.translated;
				gameObject.GetComponentInChildren<ItemButtonTools>().itemDef = itemDefinition;
				gameObject.GetComponentInChildren<ItemButtonTools>().image.sprite = itemDefinition.iconSprite;
				num++;
				if (num >= 160)
				{
					break;
				}
			}
		}
		this.itemButton.SetActive(false);
	}
}
