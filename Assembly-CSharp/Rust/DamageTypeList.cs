using System;
using System.Collections.Generic;

namespace Rust
{
	// Token: 0x02000B06 RID: 2822
	public class DamageTypeList
	{
		// Token: 0x04003D1E RID: 15646
		public float[] types = new float[25];

		// Token: 0x060044EA RID: 17642 RVA: 0x00194B80 File Offset: 0x00192D80
		public void Set(DamageType index, float amount)
		{
			this.types[(int)index] = amount;
		}

		// Token: 0x060044EB RID: 17643 RVA: 0x00194B8B File Offset: 0x00192D8B
		public float Get(DamageType index)
		{
			return this.types[(int)index];
		}

		// Token: 0x060044EC RID: 17644 RVA: 0x00194B95 File Offset: 0x00192D95
		public void Add(DamageType index, float amount)
		{
			this.Set(index, this.Get(index) + amount);
		}

		// Token: 0x060044ED RID: 17645 RVA: 0x00194BA7 File Offset: 0x00192DA7
		public void Scale(DamageType index, float amount)
		{
			this.Set(index, this.Get(index) * amount);
		}

		// Token: 0x060044EE RID: 17646 RVA: 0x00194BB9 File Offset: 0x00192DB9
		public bool Has(DamageType index)
		{
			return this.Get(index) > 0f;
		}

		// Token: 0x060044EF RID: 17647 RVA: 0x00194BCC File Offset: 0x00192DCC
		public float Total()
		{
			float num = 0f;
			for (int i = 0; i < this.types.Length; i++)
			{
				float num2 = this.types[i];
				if (!float.IsNaN(num2) && !float.IsInfinity(num2))
				{
					num += num2;
				}
			}
			return num;
		}

		// Token: 0x060044F0 RID: 17648 RVA: 0x00194C10 File Offset: 0x00192E10
		public void Clear()
		{
			for (int i = 0; i < this.types.Length; i++)
			{
				this.types[i] = 0f;
			}
		}

		// Token: 0x060044F1 RID: 17649 RVA: 0x00194C40 File Offset: 0x00192E40
		public void Add(List<DamageTypeEntry> entries)
		{
			foreach (DamageTypeEntry damageTypeEntry in entries)
			{
				this.Add(damageTypeEntry.type, damageTypeEntry.amount);
			}
		}

		// Token: 0x060044F2 RID: 17650 RVA: 0x00194C9C File Offset: 0x00192E9C
		public void ScaleAll(float amount)
		{
			for (int i = 0; i < this.types.Length; i++)
			{
				this.Scale((DamageType)i, amount);
			}
		}

		// Token: 0x060044F3 RID: 17651 RVA: 0x00194CC4 File Offset: 0x00192EC4
		public DamageType GetMajorityDamageType()
		{
			int result = 0;
			float num = 0f;
			for (int i = 0; i < this.types.Length; i++)
			{
				float num2 = this.types[i];
				if (!float.IsNaN(num2) && !float.IsInfinity(num2) && num2 >= num)
				{
					result = i;
					num = num2;
				}
			}
			return (DamageType)result;
		}

		// Token: 0x060044F4 RID: 17652 RVA: 0x00194D0E File Offset: 0x00192F0E
		public bool IsMeleeType()
		{
			return this.GetMajorityDamageType().IsMeleeType();
		}

		// Token: 0x060044F5 RID: 17653 RVA: 0x00194D1B File Offset: 0x00192F1B
		public bool IsBleedCausing()
		{
			return this.GetMajorityDamageType().IsBleedCausing();
		}

		// Token: 0x060044F6 RID: 17654 RVA: 0x00194D28 File Offset: 0x00192F28
		public bool IsConsideredAnAttack()
		{
			return this.GetMajorityDamageType().IsConsideredAnAttack();
		}
	}
}
