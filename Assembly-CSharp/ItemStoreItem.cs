using System;
using Rust.UI;
using TMPro;
using UnityEngine;

// Token: 0x02000874 RID: 2164
public class ItemStoreItem : MonoBehaviour
{
	// Token: 0x040030A5 RID: 12453
	public HttpImage Icon;

	// Token: 0x040030A6 RID: 12454
	public RustText Name;

	// Token: 0x040030A7 RID: 12455
	public TextMeshProUGUI Price;

	// Token: 0x040030A8 RID: 12456
	public RustText ItemName;

	// Token: 0x040030A9 RID: 12457
	public GameObject InCartTag;

	// Token: 0x040030AA RID: 12458
	private IPlayerItemDefinition item;

	// Token: 0x0600365D RID: 13917 RVA: 0x00149550 File Offset: 0x00147750
	internal void Init(IPlayerItemDefinition item, bool inCart)
	{
		this.item = item;
		this.Icon.Load(item.IconUrl);
		this.Name.SetText(item.Name);
		this.Price.text = item.LocalPriceFormatted;
		this.InCartTag.SetActive(inCart);
		if (string.IsNullOrWhiteSpace(item.ItemShortName))
		{
			this.ItemName.SetText("");
			return;
		}
		ItemDefinition itemDefinition = ItemManager.FindItemDefinition(item.ItemShortName);
		if (itemDefinition != null && !string.Equals(itemDefinition.displayName.english, item.Name, StringComparison.InvariantCultureIgnoreCase))
		{
			this.ItemName.SetPhrase(itemDefinition.displayName);
			return;
		}
		this.ItemName.SetText("");
	}
}
