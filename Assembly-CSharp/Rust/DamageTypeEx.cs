using System;

namespace Rust
{
	// Token: 0x02000B08 RID: 2824
	public static class DamageTypeEx
	{
		// Token: 0x060044F9 RID: 17657 RVA: 0x00194D4A File Offset: 0x00192F4A
		public static bool IsMeleeType(this DamageType damageType)
		{
			return damageType == DamageType.Blunt || damageType == DamageType.Slash || damageType == DamageType.Stab;
		}

		// Token: 0x060044FA RID: 17658 RVA: 0x00194D5D File Offset: 0x00192F5D
		public static bool IsBleedCausing(this DamageType damageType)
		{
			return damageType == DamageType.Bite || damageType == DamageType.Slash || damageType == DamageType.Stab || damageType == DamageType.Bullet || damageType == DamageType.Arrow;
		}

		// Token: 0x060044FB RID: 17659 RVA: 0x00194D7A File Offset: 0x00192F7A
		public static bool IsConsideredAnAttack(this DamageType damageType)
		{
			return damageType != DamageType.Decay && damageType != DamageType.Collision;
		}
	}
}
