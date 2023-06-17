using System;
using System.Threading.Tasks;
using UnityEngine;

namespace Rust.UI
{
	// Token: 0x02000B15 RID: 2837
	public class SteamInventoryNewItem : MonoBehaviour
	{
		// Token: 0x0600450F RID: 17679 RVA: 0x00194F50 File Offset: 0x00193150
		public async Task Open(IPlayerItem item)
		{
			base.gameObject.SetActive(true);
			base.GetComponentInChildren<SteamInventoryItem>().Setup(item);
			while (this && base.gameObject.activeSelf)
			{
				await Task.Delay(100);
			}
		}
	}
}
