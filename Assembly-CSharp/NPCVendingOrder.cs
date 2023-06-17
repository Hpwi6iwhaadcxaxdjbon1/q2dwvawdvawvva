using System;
using UnityEngine;

// Token: 0x02000127 RID: 295
[CreateAssetMenu(menuName = "Rust/NPC Vending Order")]
public class NPCVendingOrder : ScriptableObject
{
	// Token: 0x04000EC9 RID: 3785
	public NPCVendingOrder.Entry[] orders;

	// Token: 0x02000C22 RID: 3106
	[Serializable]
	public class Entry
	{
		// Token: 0x04004230 RID: 16944
		public ItemDefinition sellItem;

		// Token: 0x04004231 RID: 16945
		public int sellItemAmount;

		// Token: 0x04004232 RID: 16946
		public bool sellItemAsBP;

		// Token: 0x04004233 RID: 16947
		public ItemDefinition currencyItem;

		// Token: 0x04004234 RID: 16948
		public int currencyAmount;

		// Token: 0x04004235 RID: 16949
		public bool currencyAsBP;

		// Token: 0x04004236 RID: 16950
		[Tooltip("The higher this number, the more likely this will be chosen")]
		public int weight;

		// Token: 0x04004237 RID: 16951
		public int refillAmount = 1;

		// Token: 0x04004238 RID: 16952
		public float refillDelay = 10f;
	}
}
