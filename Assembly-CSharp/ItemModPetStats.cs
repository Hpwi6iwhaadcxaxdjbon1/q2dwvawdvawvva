using System;
using UnityEngine;

// Token: 0x020005F2 RID: 1522
public class ItemModPetStats : ItemMod
{
	// Token: 0x040024F9 RID: 9465
	[Tooltip("Speed modifier. Value, not percentage.")]
	public float SpeedModifier;

	// Token: 0x040024FA RID: 9466
	[Tooltip("HP amount to modify max health by. Value, not percentage.")]
	public float MaxHealthModifier;

	// Token: 0x040024FB RID: 9467
	[Tooltip("Damage amount to modify base attack damage by. Value, not percentage.")]
	public float AttackDamageModifier;

	// Token: 0x040024FC RID: 9468
	[Tooltip("Attack rate (seconds) to modify base attack rate by. Value, not percentage.")]
	public float AttackRateModifier;

	// Token: 0x06002D66 RID: 11622 RVA: 0x00111420 File Offset: 0x0010F620
	public void Apply(BasePet pet)
	{
		if (pet == null)
		{
			return;
		}
		pet.SetMaxHealth(pet.MaxHealth() + this.MaxHealthModifier);
		if (pet.Brain != null && pet.Brain.Navigator != null)
		{
			pet.Brain.Navigator.Speed += this.SpeedModifier;
		}
		pet.BaseAttackRate += this.AttackRateModifier;
		pet.BaseAttackDamge += this.AttackDamageModifier;
	}
}
