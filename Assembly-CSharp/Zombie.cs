using System;

// Token: 0x02000200 RID: 512
public class Zombie : BaseAnimalNPC
{
	// Token: 0x040012D9 RID: 4825
	[ServerVar(Help = "Population active on the server, per square km", ShowInAdminUI = true)]
	public static float Population;

	// Token: 0x1700023E RID: 574
	// (get) Token: 0x06001AC6 RID: 6854 RVA: 0x000BB920 File Offset: 0x000B9B20
	public override BaseEntity.TraitFlag Traits
	{
		get
		{
			return BaseEntity.TraitFlag.Alive | BaseEntity.TraitFlag.Animal | BaseEntity.TraitFlag.Food | BaseEntity.TraitFlag.Meat;
		}
	}

	// Token: 0x06001AC7 RID: 6855 RVA: 0x000BB924 File Offset: 0x000B9B24
	public override bool WantsToEat(BaseEntity best)
	{
		return !best.HasTrait(BaseEntity.TraitFlag.Alive) && base.WantsToEat(best);
	}

	// Token: 0x06001AC8 RID: 6856 RVA: 0x000BF40F File Offset: 0x000BD60F
	protected override void TickSleep()
	{
		this.Sleep = 100f;
	}

	// Token: 0x06001AC9 RID: 6857 RVA: 0x000BF41C File Offset: 0x000BD61C
	public override string Categorize()
	{
		return "Zombie";
	}
}
