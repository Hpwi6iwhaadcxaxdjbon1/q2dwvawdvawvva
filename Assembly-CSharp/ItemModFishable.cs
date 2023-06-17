using System;
using UnityEngine;

// Token: 0x020005EC RID: 1516
public class ItemModFishable : ItemMod
{
	// Token: 0x040024DE RID: 9438
	public bool CanBeFished = true;

	// Token: 0x040024DF RID: 9439
	[Header("Catching Behaviour")]
	public float StrainModifier = 1f;

	// Token: 0x040024E0 RID: 9440
	public float MoveMultiplier = 1f;

	// Token: 0x040024E1 RID: 9441
	public float ReelInSpeedMultiplier = 1f;

	// Token: 0x040024E2 RID: 9442
	public float CatchWaitTimeMultiplier = 1f;

	// Token: 0x040024E3 RID: 9443
	[Header("Catch Criteria")]
	public float MinimumBaitLevel;

	// Token: 0x040024E4 RID: 9444
	public float MaximumBaitLevel;

	// Token: 0x040024E5 RID: 9445
	public float MinimumWaterDepth;

	// Token: 0x040024E6 RID: 9446
	public float MaximumWaterDepth;

	// Token: 0x040024E7 RID: 9447
	[InspectorFlags]
	public WaterBody.FishingTag RequiredTag;

	// Token: 0x040024E8 RID: 9448
	[Range(0f, 1f)]
	public float Chance;

	// Token: 0x040024E9 RID: 9449
	public string SteamStatName;
}
