using System;

// Token: 0x020001E7 RID: 487
public class Chicken : BaseAnimalNPC
{
	// Token: 0x0400125B RID: 4699
	[ServerVar(Help = "Population active on the server, per square km", ShowInAdminUI = true)]
	public static float Population = 3f;

	// Token: 0x1700022B RID: 555
	// (get) Token: 0x060019B1 RID: 6577 RVA: 0x000ABEAB File Offset: 0x000AA0AB
	public override float RealisticMass
	{
		get
		{
			return 3f;
		}
	}

	// Token: 0x1700022C RID: 556
	// (get) Token: 0x060019B2 RID: 6578 RVA: 0x000BB920 File Offset: 0x000B9B20
	public override BaseEntity.TraitFlag Traits
	{
		get
		{
			return BaseEntity.TraitFlag.Alive | BaseEntity.TraitFlag.Animal | BaseEntity.TraitFlag.Food | BaseEntity.TraitFlag.Meat;
		}
	}

	// Token: 0x060019B3 RID: 6579 RVA: 0x000BB9E8 File Offset: 0x000B9BE8
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

	// Token: 0x060019B4 RID: 6580 RVA: 0x000BBA4A File Offset: 0x000B9C4A
	public override string Categorize()
	{
		return "Chicken";
	}
}
