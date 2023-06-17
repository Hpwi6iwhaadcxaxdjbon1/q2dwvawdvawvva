using System;

// Token: 0x020001FF RID: 511
public class Wolf : BaseAnimalNPC
{
	// Token: 0x040012D8 RID: 4824
	[ServerVar(Help = "Population active on the server, per square km", ShowInAdminUI = true)]
	public static float Population = 2f;

	// Token: 0x1700023C RID: 572
	// (get) Token: 0x06001AC0 RID: 6848 RVA: 0x000BF3D5 File Offset: 0x000BD5D5
	public override float RealisticMass
	{
		get
		{
			return 45f;
		}
	}

	// Token: 0x1700023D RID: 573
	// (get) Token: 0x06001AC1 RID: 6849 RVA: 0x000BB920 File Offset: 0x000B9B20
	public override BaseEntity.TraitFlag Traits
	{
		get
		{
			return BaseEntity.TraitFlag.Alive | BaseEntity.TraitFlag.Animal | BaseEntity.TraitFlag.Food | BaseEntity.TraitFlag.Meat;
		}
	}

	// Token: 0x06001AC2 RID: 6850 RVA: 0x000BF3DC File Offset: 0x000BD5DC
	public override bool WantsToEat(BaseEntity best)
	{
		return !best.HasTrait(BaseEntity.TraitFlag.Alive) && (best.HasTrait(BaseEntity.TraitFlag.Meat) || base.WantsToEat(best));
	}

	// Token: 0x06001AC3 RID: 6851 RVA: 0x000BF3FC File Offset: 0x000BD5FC
	public override string Categorize()
	{
		return "Wolf";
	}
}
