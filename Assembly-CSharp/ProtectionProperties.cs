using System;
using System.Collections.Generic;
using Rust;
using UnityEngine;

// Token: 0x02000757 RID: 1879
[CreateAssetMenu(menuName = "Rust/Protection Properties")]
public class ProtectionProperties : ScriptableObject
{
	// Token: 0x04002A97 RID: 10903
	[TextArea]
	public string comments;

	// Token: 0x04002A98 RID: 10904
	[Range(0f, 100f)]
	public float density = 1f;

	// Token: 0x04002A99 RID: 10905
	[ArrayIndexIsEnumRanged(enumType = typeof(DamageType), min = -4f, max = 3f)]
	public float[] amounts = new float[25];

	// Token: 0x0600347F RID: 13439 RVA: 0x001453AC File Offset: 0x001435AC
	public void OnValidate()
	{
		if (this.amounts.Length < 25)
		{
			float[] array = new float[25];
			for (int i = 0; i < array.Length; i++)
			{
				if (i >= this.amounts.Length)
				{
					if (i == 21)
					{
						array[i] = this.amounts[9];
					}
				}
				else
				{
					array[i] = this.amounts[i];
				}
			}
			this.amounts = array;
		}
	}

	// Token: 0x06003480 RID: 13440 RVA: 0x0014540C File Offset: 0x0014360C
	public void Clear()
	{
		for (int i = 0; i < this.amounts.Length; i++)
		{
			this.amounts[i] = 0f;
		}
	}

	// Token: 0x06003481 RID: 13441 RVA: 0x0014543C File Offset: 0x0014363C
	public void Add(float amount)
	{
		for (int i = 0; i < this.amounts.Length; i++)
		{
			this.amounts[i] += amount;
		}
	}

	// Token: 0x06003482 RID: 13442 RVA: 0x0014546D File Offset: 0x0014366D
	public void Add(DamageType index, float amount)
	{
		this.amounts[(int)index] += amount;
	}

	// Token: 0x06003483 RID: 13443 RVA: 0x00145480 File Offset: 0x00143680
	public void Add(ProtectionProperties other, float scale)
	{
		for (int i = 0; i < Mathf.Min(other.amounts.Length, this.amounts.Length); i++)
		{
			this.amounts[i] += other.amounts[i] * scale;
		}
	}

	// Token: 0x06003484 RID: 13444 RVA: 0x001454C8 File Offset: 0x001436C8
	public void Add(List<Item> items, HitArea area = (HitArea)(-1))
	{
		for (int i = 0; i < items.Count; i++)
		{
			Item item = items[i];
			ItemModWearable component = item.info.GetComponent<ItemModWearable>();
			if (!(component == null) && component.ProtectsArea(area))
			{
				component.CollectProtection(item, this);
			}
		}
	}

	// Token: 0x06003485 RID: 13445 RVA: 0x00145514 File Offset: 0x00143714
	public void Multiply(float multiplier)
	{
		for (int i = 0; i < this.amounts.Length; i++)
		{
			this.amounts[i] *= multiplier;
		}
	}

	// Token: 0x06003486 RID: 13446 RVA: 0x00145545 File Offset: 0x00143745
	public void Multiply(DamageType index, float multiplier)
	{
		this.amounts[(int)index] *= multiplier;
	}

	// Token: 0x06003487 RID: 13447 RVA: 0x00145558 File Offset: 0x00143758
	public void Scale(DamageTypeList damageList, float ProtectionAmount = 1f)
	{
		for (int i = 0; i < this.amounts.Length; i++)
		{
			if (this.amounts[i] != 0f)
			{
				damageList.Scale((DamageType)i, 1f - Mathf.Clamp(this.amounts[i] * ProtectionAmount, -1f, 1f));
			}
		}
	}

	// Token: 0x06003488 RID: 13448 RVA: 0x001455AD File Offset: 0x001437AD
	public float Get(DamageType damageType)
	{
		return this.amounts[(int)damageType];
	}
}
