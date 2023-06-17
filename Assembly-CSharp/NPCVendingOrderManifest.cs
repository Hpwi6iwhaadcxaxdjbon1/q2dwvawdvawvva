using System;
using UnityEngine;

// Token: 0x02000128 RID: 296
[CreateAssetMenu(menuName = "Rust/NPCVendingOrderManifest")]
public class NPCVendingOrderManifest : ScriptableObject
{
	// Token: 0x04000ECA RID: 3786
	public NPCVendingOrder[] orderList;

	// Token: 0x060016A0 RID: 5792 RVA: 0x000AE8D8 File Offset: 0x000ACAD8
	public int GetIndex(NPCVendingOrder sample)
	{
		if (sample == null)
		{
			return -1;
		}
		for (int i = 0; i < this.orderList.Length; i++)
		{
			NPCVendingOrder y = this.orderList[i];
			if (sample == y)
			{
				return i;
			}
		}
		return -1;
	}

	// Token: 0x060016A1 RID: 5793 RVA: 0x000AE918 File Offset: 0x000ACB18
	public NPCVendingOrder GetFromIndex(int index)
	{
		if (this.orderList == null)
		{
			return null;
		}
		if (index < 0)
		{
			return null;
		}
		if (index >= this.orderList.Length)
		{
			return null;
		}
		return this.orderList[index];
	}
}
