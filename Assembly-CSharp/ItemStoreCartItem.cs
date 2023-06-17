using System;
using TMPro;
using UnityEngine;

// Token: 0x02000873 RID: 2163
public class ItemStoreCartItem : MonoBehaviour
{
	// Token: 0x040030A2 RID: 12450
	public int Index;

	// Token: 0x040030A3 RID: 12451
	public TextMeshProUGUI Name;

	// Token: 0x040030A4 RID: 12452
	public TextMeshProUGUI Price;

	// Token: 0x0600365B RID: 13915 RVA: 0x00149525 File Offset: 0x00147725
	public void Init(int index, IPlayerItemDefinition def)
	{
		this.Index = index;
		this.Name.text = def.Name;
		this.Price.text = def.LocalPriceFormatted;
	}
}
