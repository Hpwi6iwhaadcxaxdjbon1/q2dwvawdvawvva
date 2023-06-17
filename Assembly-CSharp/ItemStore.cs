using System;
using System.Collections.Generic;
using Facepunch;
using Rust.UI;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x02000870 RID: 2160
public class ItemStore : SingletonComponent<ItemStore>, VirtualScroll.IDataSource
{
	// Token: 0x04003093 RID: 12435
	public static readonly Translate.Phrase CartEmptyPhrase = new Translate.Phrase("store.cart.empty", "Cart");

	// Token: 0x04003094 RID: 12436
	public static readonly Translate.Phrase CartSingularPhrase = new Translate.Phrase("store.cart.singular", "1 item");

	// Token: 0x04003095 RID: 12437
	public static readonly Translate.Phrase CartPluralPhrase = new Translate.Phrase("store.cart.plural", "{amount} items");

	// Token: 0x04003096 RID: 12438
	public GameObject ItemPrefab;

	// Token: 0x04003097 RID: 12439
	[FormerlySerializedAs("ItemParent")]
	public RectTransform LimitedItemParent;

	// Token: 0x04003098 RID: 12440
	public RectTransform GeneralItemParent;

	// Token: 0x04003099 RID: 12441
	public List<IPlayerItemDefinition> Cart = new List<IPlayerItemDefinition>();

	// Token: 0x0400309A RID: 12442
	public ItemStoreItemInfoModal ItemStoreInfoModal;

	// Token: 0x0400309B RID: 12443
	public GameObject BuyingModal;

	// Token: 0x0400309C RID: 12444
	public ItemStoreBuyFailedModal ItemStoreBuyFailedModal;

	// Token: 0x0400309D RID: 12445
	public ItemStoreBuySuccessModal ItemStoreBuySuccessModal;

	// Token: 0x0400309E RID: 12446
	public SoundDefinition AddToCartSound;

	// Token: 0x0400309F RID: 12447
	public RustText CartButtonLabel;

	// Token: 0x040030A0 RID: 12448
	public RustText QuantityValue;

	// Token: 0x040030A1 RID: 12449
	public RustText TotalValue;

	// Token: 0x0600364F RID: 13903 RVA: 0x001493E7 File Offset: 0x001475E7
	public int GetItemCount()
	{
		return this.Cart.Count;
	}

	// Token: 0x06003650 RID: 13904 RVA: 0x001493F4 File Offset: 0x001475F4
	public void SetItemData(int i, GameObject obj)
	{
		obj.GetComponent<ItemStoreCartItem>().Init(i, this.Cart[i]);
	}
}
