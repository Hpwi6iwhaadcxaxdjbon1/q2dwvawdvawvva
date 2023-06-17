using System;

// Token: 0x020001E4 RID: 484
public class Bear : BaseAnimalNPC
{
	// Token: 0x04001258 RID: 4696
	[ServerVar(Help = "Population active on the server, per square km", ShowInAdminUI = true)]
	public static float Population = 2f;

	// Token: 0x17000225 RID: 549
	// (get) Token: 0x0600199F RID: 6559 RVA: 0x000BB919 File Offset: 0x000B9B19
	public override float RealisticMass
	{
		get
		{
			return 150f;
		}
	}

	// Token: 0x17000226 RID: 550
	// (get) Token: 0x060019A0 RID: 6560 RVA: 0x000BB920 File Offset: 0x000B9B20
	public override BaseEntity.TraitFlag Traits
	{
		get
		{
			return BaseEntity.TraitFlag.Alive | BaseEntity.TraitFlag.Animal | BaseEntity.TraitFlag.Food | BaseEntity.TraitFlag.Meat;
		}
	}

	// Token: 0x060019A1 RID: 6561 RVA: 0x000BB924 File Offset: 0x000B9B24
	public override bool WantsToEat(BaseEntity best)
	{
		return !best.HasTrait(BaseEntity.TraitFlag.Alive) && base.WantsToEat(best);
	}

	// Token: 0x060019A2 RID: 6562 RVA: 0x000BB938 File Offset: 0x000B9B38
	public override string Categorize()
	{
		return "Bear";
	}
}
