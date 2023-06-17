using System;

// Token: 0x020001E6 RID: 486
public class Boar : BaseAnimalNPC
{
	// Token: 0x0400125A RID: 4698
	[ServerVar(Help = "Population active on the server, per square km", ShowInAdminUI = true)]
	public static float Population = 5f;

	// Token: 0x17000229 RID: 553
	// (get) Token: 0x060019AB RID: 6571 RVA: 0x000BB966 File Offset: 0x000B9B66
	public override float RealisticMass
	{
		get
		{
			return 85f;
		}
	}

	// Token: 0x1700022A RID: 554
	// (get) Token: 0x060019AC RID: 6572 RVA: 0x000BB920 File Offset: 0x000B9B20
	public override BaseEntity.TraitFlag Traits
	{
		get
		{
			return BaseEntity.TraitFlag.Alive | BaseEntity.TraitFlag.Animal | BaseEntity.TraitFlag.Food | BaseEntity.TraitFlag.Meat;
		}
	}

	// Token: 0x060019AD RID: 6573 RVA: 0x000BB970 File Offset: 0x000B9B70
	public override bool WantsToEat(BaseEntity best)
	{
		if (best.HasTrait(BaseEntity.TraitFlag.Alive))
		{
			return false;
		}
		if (best.HasTrait(BaseEntity.TraitFlag.Meat))
		{
			return false;
		}
		CollectibleEntity collectibleEntity = best as CollectibleEntity;
		if (collectibleEntity != null)
		{
			ItemAmount[] itemList = collectibleEntity.itemList;
			for (int i = 0; i < itemList.Length; i++)
			{
				if (itemList[i].itemDef.category == ItemCategory.Food)
				{
					return true;
				}
			}
		}
		return base.WantsToEat(best);
	}

	// Token: 0x060019AE RID: 6574 RVA: 0x000BB9D2 File Offset: 0x000B9BD2
	public override string Categorize()
	{
		return "Boar";
	}
}
