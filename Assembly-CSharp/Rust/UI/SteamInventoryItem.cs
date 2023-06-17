using System;
using Facepunch.Extend;
using TMPro;
using UnityEngine;

namespace Rust.UI
{
	// Token: 0x02000B13 RID: 2835
	public class SteamInventoryItem : MonoBehaviour
	{
		// Token: 0x04003D5A RID: 15706
		public IPlayerItem Item;

		// Token: 0x04003D5B RID: 15707
		public HttpImage Image;

		// Token: 0x0600450C RID: 17676 RVA: 0x00194EEC File Offset: 0x001930EC
		public bool Setup(IPlayerItem item)
		{
			this.Item = item;
			if (item.GetDefinition() == null)
			{
				return false;
			}
			base.transform.FindChildRecursive("ItemName").GetComponent<TextMeshProUGUI>().text = item.GetDefinition().Name;
			return this.Image.Load(item.GetDefinition().IconUrl);
		}
	}
}
