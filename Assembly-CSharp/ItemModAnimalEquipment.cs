using System;

// Token: 0x020005D3 RID: 1491
public class ItemModAnimalEquipment : ItemMod
{
	// Token: 0x04002499 RID: 9369
	public BaseEntity.Flags WearableFlag;

	// Token: 0x0400249A RID: 9370
	public bool hideHair;

	// Token: 0x0400249B RID: 9371
	public ProtectionProperties animalProtection;

	// Token: 0x0400249C RID: 9372
	public ProtectionProperties riderProtection;

	// Token: 0x0400249D RID: 9373
	public int additionalInventorySlots;

	// Token: 0x0400249E RID: 9374
	public float speedModifier;

	// Token: 0x0400249F RID: 9375
	public float staminaUseModifier;

	// Token: 0x040024A0 RID: 9376
	public ItemModAnimalEquipment.SlotType slot;

	// Token: 0x02000D7C RID: 3452
	public enum SlotType
	{
		// Token: 0x040047AB RID: 18347
		Basic,
		// Token: 0x040047AC RID: 18348
		Armor,
		// Token: 0x040047AD RID: 18349
		Saddle,
		// Token: 0x040047AE RID: 18350
		Bit,
		// Token: 0x040047AF RID: 18351
		Feet,
		// Token: 0x040047B0 RID: 18352
		SaddleDouble
	}
}
