using System;

// Token: 0x020001EB RID: 491
public class Horse : BaseAnimalNPC
{
	// Token: 0x04001284 RID: 4740
	[ServerVar(Help = "Population active on the server, per square km", ShowInAdminUI = true)]
	public static float Population;

	// Token: 0x1700022E RID: 558
	// (get) Token: 0x060019DB RID: 6619 RVA: 0x000BC89A File Offset: 0x000BAA9A
	public override float RealisticMass
	{
		get
		{
			return 500f;
		}
	}

	// Token: 0x1700022F RID: 559
	// (get) Token: 0x060019DC RID: 6620 RVA: 0x000BB920 File Offset: 0x000B9B20
	public override BaseEntity.TraitFlag Traits
	{
		get
		{
			return BaseEntity.TraitFlag.Alive | BaseEntity.TraitFlag.Animal | BaseEntity.TraitFlag.Food | BaseEntity.TraitFlag.Meat;
		}
	}

	// Token: 0x060019DD RID: 6621 RVA: 0x000BC8A1 File Offset: 0x000BAAA1
	public override void ServerInit()
	{
		base.ServerInit();
	}

	// Token: 0x060019DE RID: 6622 RVA: 0x000BC8AC File Offset: 0x000BAAAC
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

	// Token: 0x060019DF RID: 6623 RVA: 0x000BC90E File Offset: 0x000BAB0E
	public override string Categorize()
	{
		return "Horse";
	}
}
