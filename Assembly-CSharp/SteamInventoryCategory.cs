using System;
using UnityEngine;

// Token: 0x02000760 RID: 1888
[CreateAssetMenu(menuName = "Rust/Generic Steam Inventory Category")]
public class SteamInventoryCategory : ScriptableObject
{
	// Token: 0x04002ACE RID: 10958
	[Header("Steam Inventory")]
	public bool canBeSoldToOtherUsers;

	// Token: 0x04002ACF RID: 10959
	public bool canBeTradedWithOtherUsers;

	// Token: 0x04002AD0 RID: 10960
	public bool isCommodity;

	// Token: 0x04002AD1 RID: 10961
	public SteamInventoryCategory.Price price;

	// Token: 0x04002AD2 RID: 10962
	public SteamInventoryCategory.DropChance dropChance;

	// Token: 0x04002AD3 RID: 10963
	public bool CanBeInCrates = true;

	// Token: 0x02000E6D RID: 3693
	public enum Price
	{
		// Token: 0x04004B70 RID: 19312
		CannotBuy,
		// Token: 0x04004B71 RID: 19313
		VLV25,
		// Token: 0x04004B72 RID: 19314
		VLV50,
		// Token: 0x04004B73 RID: 19315
		VLV75,
		// Token: 0x04004B74 RID: 19316
		VLV100,
		// Token: 0x04004B75 RID: 19317
		VLV150,
		// Token: 0x04004B76 RID: 19318
		VLV200,
		// Token: 0x04004B77 RID: 19319
		VLV250,
		// Token: 0x04004B78 RID: 19320
		VLV300,
		// Token: 0x04004B79 RID: 19321
		VLV350,
		// Token: 0x04004B7A RID: 19322
		VLV400,
		// Token: 0x04004B7B RID: 19323
		VLV450,
		// Token: 0x04004B7C RID: 19324
		VLV500,
		// Token: 0x04004B7D RID: 19325
		VLV550,
		// Token: 0x04004B7E RID: 19326
		VLV600,
		// Token: 0x04004B7F RID: 19327
		VLV650,
		// Token: 0x04004B80 RID: 19328
		VLV700,
		// Token: 0x04004B81 RID: 19329
		VLV750,
		// Token: 0x04004B82 RID: 19330
		VLV800,
		// Token: 0x04004B83 RID: 19331
		VLV850,
		// Token: 0x04004B84 RID: 19332
		VLV900,
		// Token: 0x04004B85 RID: 19333
		VLV950,
		// Token: 0x04004B86 RID: 19334
		VLV1000,
		// Token: 0x04004B87 RID: 19335
		VLV1100,
		// Token: 0x04004B88 RID: 19336
		VLV1200,
		// Token: 0x04004B89 RID: 19337
		VLV1300,
		// Token: 0x04004B8A RID: 19338
		VLV1400,
		// Token: 0x04004B8B RID: 19339
		VLV1500,
		// Token: 0x04004B8C RID: 19340
		VLV1600,
		// Token: 0x04004B8D RID: 19341
		VLV1700,
		// Token: 0x04004B8E RID: 19342
		VLV1800,
		// Token: 0x04004B8F RID: 19343
		VLV1900,
		// Token: 0x04004B90 RID: 19344
		VLV2000,
		// Token: 0x04004B91 RID: 19345
		VLV2500,
		// Token: 0x04004B92 RID: 19346
		VLV3000,
		// Token: 0x04004B93 RID: 19347
		VLV3500,
		// Token: 0x04004B94 RID: 19348
		VLV4000,
		// Token: 0x04004B95 RID: 19349
		VLV4500,
		// Token: 0x04004B96 RID: 19350
		VLV5000,
		// Token: 0x04004B97 RID: 19351
		VLV6000,
		// Token: 0x04004B98 RID: 19352
		VLV7000,
		// Token: 0x04004B99 RID: 19353
		VLV8000,
		// Token: 0x04004B9A RID: 19354
		VLV9000,
		// Token: 0x04004B9B RID: 19355
		VLV10000
	}

	// Token: 0x02000E6E RID: 3694
	public enum DropChance
	{
		// Token: 0x04004B9D RID: 19357
		NeverDrop,
		// Token: 0x04004B9E RID: 19358
		VeryRare,
		// Token: 0x04004B9F RID: 19359
		Rare,
		// Token: 0x04004BA0 RID: 19360
		Common,
		// Token: 0x04004BA1 RID: 19361
		VeryCommon,
		// Token: 0x04004BA2 RID: 19362
		ExtremelyRare
	}
}
