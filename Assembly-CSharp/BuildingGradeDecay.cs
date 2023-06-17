using System;
using ConVar;

// Token: 0x020003E2 RID: 994
public class BuildingGradeDecay : global::Decay
{
	// Token: 0x04001A60 RID: 6752
	public BuildingGrade.Enum decayGrade;

	// Token: 0x0600221E RID: 8734 RVA: 0x000DCE12 File Offset: 0x000DB012
	public override float GetDecayDelay(BaseEntity entity)
	{
		return base.GetDecayDelay(this.decayGrade);
	}

	// Token: 0x0600221F RID: 8735 RVA: 0x000DCE20 File Offset: 0x000DB020
	public override float GetDecayDuration(BaseEntity entity)
	{
		return base.GetDecayDuration(this.decayGrade);
	}

	// Token: 0x06002220 RID: 8736 RVA: 0x000DCE2E File Offset: 0x000DB02E
	public override bool ShouldDecay(BaseEntity entity)
	{
		return ConVar.Decay.upkeep || entity.IsOutside();
	}
}
