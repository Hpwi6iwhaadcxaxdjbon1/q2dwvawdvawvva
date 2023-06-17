using System;

// Token: 0x020001FE RID: 510
public class Stag : BaseAnimalNPC
{
	// Token: 0x040012D7 RID: 4823
	[ServerVar(Help = "Population active on the server, per square km", ShowInAdminUI = true)]
	public static float Population = 3f;

	// Token: 0x1700023A RID: 570
	// (get) Token: 0x06001ABA RID: 6842 RVA: 0x000BF359 File Offset: 0x000BD559
	public override float RealisticMass
	{
		get
		{
			return 200f;
		}
	}

	// Token: 0x1700023B RID: 571
	// (get) Token: 0x06001ABB RID: 6843 RVA: 0x000BB920 File Offset: 0x000B9B20
	public override BaseEntity.TraitFlag Traits
	{
		get
		{
			return BaseEntity.TraitFlag.Alive | BaseEntity.TraitFlag.Animal | BaseEntity.TraitFlag.Food | BaseEntity.TraitFlag.Meat;
		}
	}

	// Token: 0x06001ABC RID: 6844 RVA: 0x000BF360 File Offset: 0x000BD560
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

	// Token: 0x06001ABD RID: 6845 RVA: 0x000BF3C2 File Offset: 0x000BD5C2
	public override string Categorize()
	{
		return "Stag";
	}
}
