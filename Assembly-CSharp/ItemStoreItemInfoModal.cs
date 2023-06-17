using System;
using Rust.UI;
using TMPro;
using UnityEngine;

// Token: 0x02000875 RID: 2165
public class ItemStoreItemInfoModal : MonoBehaviour
{
	// Token: 0x040030AB RID: 12459
	public HttpImage Icon;

	// Token: 0x040030AC RID: 12460
	public TextMeshProUGUI Name;

	// Token: 0x040030AD RID: 12461
	public TextMeshProUGUI Price;

	// Token: 0x040030AE RID: 12462
	public TextMeshProUGUI Description;

	// Token: 0x040030AF RID: 12463
	private IPlayerItemDefinition item;

	// Token: 0x0600365F RID: 13919 RVA: 0x00149614 File Offset: 0x00147814
	public void Show(IPlayerItemDefinition item)
	{
		this.item = item;
		this.Icon.Load(item.IconUrl);
		this.Name.text = item.Name;
		this.Description.text = item.Description.BBCodeToUnity();
		this.Price.text = item.LocalPriceFormatted;
		base.gameObject.SetActive(true);
		base.GetComponent<CanvasGroup>().alpha = 0f;
		LeanTween.alphaCanvas(base.GetComponent<CanvasGroup>(), 1f, 0.1f);
	}

	// Token: 0x06003660 RID: 13920 RVA: 0x001496A4 File Offset: 0x001478A4
	public void Hide()
	{
		LeanTween.alphaCanvas(base.GetComponent<CanvasGroup>(), 0f, 0.2f).setOnComplete(delegate()
		{
			base.gameObject.SetActive(false);
		});
	}
}
