using System;

// Token: 0x020001E5 RID: 485
public class Polarbear : BaseAnimalNPC
{
	// Token: 0x04001259 RID: 4697
	[ServerVar(Help = "Population active on the server, per square km", ShowInAdminUI = true)]
	public static float Population = 1f;

	// Token: 0x17000227 RID: 551
	// (get) Token: 0x060019A5 RID: 6565 RVA: 0x000BB919 File Offset: 0x000B9B19
	public override float RealisticMass
	{
		get
		{
			return 150f;
		}
	}

	// Token: 0x17000228 RID: 552
	// (get) Token: 0x060019A6 RID: 6566 RVA: 0x000BB920 File Offset: 0x000B9B20
	public override BaseEntity.TraitFlag Traits
	{
		get
		{
			return BaseEntity.TraitFlag.Alive | BaseEntity.TraitFlag.Animal | BaseEntity.TraitFlag.Food | BaseEntity.TraitFlag.Meat;
		}
	}

	// Token: 0x060019A7 RID: 6567 RVA: 0x000BB924 File Offset: 0x000B9B24
	public override bool WantsToEat(BaseEntity best)
	{
		return !best.HasTrait(BaseEntity.TraitFlag.Alive) && base.WantsToEat(best);
	}

	// Token: 0x060019A8 RID: 6568 RVA: 0x000BB953 File Offset: 0x000B9B53
	public override string Categorize()
	{
		return "Polarbear";
	}
}
