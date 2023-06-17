using System;

// Token: 0x02000513 RID: 1299
public class GameModeHardcore : GameModeVanilla
{
	// Token: 0x0600296C RID: 10604 RVA: 0x000FDFED File Offset: 0x000FC1ED
	protected override void OnCreated()
	{
		base.OnCreated();
	}

	// Token: 0x0600296D RID: 10605 RVA: 0x000FDFF8 File Offset: 0x000FC1F8
	public override BaseGameMode.ResearchCostResult GetScrapCostForResearch(ItemDefinition item, ResearchTable.ResearchType researchType)
	{
		ItemBlueprint blueprint = item.Blueprint;
		int? num = (blueprint != null) ? new int?(blueprint.workbenchLevelRequired) : null;
		if (num != null)
		{
			switch (num.GetValueOrDefault())
			{
			case 1:
				return new BaseGameMode.ResearchCostResult
				{
					Scale = new float?(1.2f)
				};
			case 2:
				return new BaseGameMode.ResearchCostResult
				{
					Scale = new float?(1.4f)
				};
			case 3:
				return new BaseGameMode.ResearchCostResult
				{
					Scale = new float?(1.6f)
				};
			}
		}
		return default(BaseGameMode.ResearchCostResult);
	}
}
