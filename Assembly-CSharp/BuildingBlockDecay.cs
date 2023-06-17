using System;
using ConVar;
using UnityEngine;

// Token: 0x020003E1 RID: 993
public class BuildingBlockDecay : global::Decay
{
	// Token: 0x04001A5F RID: 6751
	private bool isFoundation;

	// Token: 0x06002219 RID: 8729 RVA: 0x000DCD5C File Offset: 0x000DAF5C
	public override float GetDecayDelay(BaseEntity entity)
	{
		BuildingBlock buildingBlock = entity as BuildingBlock;
		BuildingGrade.Enum grade = buildingBlock ? buildingBlock.grade : BuildingGrade.Enum.Twigs;
		return base.GetDecayDelay(grade);
	}

	// Token: 0x0600221A RID: 8730 RVA: 0x000DCD8C File Offset: 0x000DAF8C
	public override float GetDecayDuration(BaseEntity entity)
	{
		BuildingBlock buildingBlock = entity as BuildingBlock;
		BuildingGrade.Enum grade = buildingBlock ? buildingBlock.grade : BuildingGrade.Enum.Twigs;
		return base.GetDecayDuration(grade);
	}

	// Token: 0x0600221B RID: 8731 RVA: 0x000DCDBC File Offset: 0x000DAFBC
	public override bool ShouldDecay(BaseEntity entity)
	{
		if (ConVar.Decay.upkeep)
		{
			return true;
		}
		if (this.isFoundation)
		{
			return true;
		}
		BuildingBlock buildingBlock = entity as BuildingBlock;
		return (buildingBlock ? buildingBlock.grade : BuildingGrade.Enum.Twigs) == BuildingGrade.Enum.Twigs;
	}

	// Token: 0x0600221C RID: 8732 RVA: 0x000DCDF7 File Offset: 0x000DAFF7
	protected override void AttributeSetup(GameObject rootObj, string name, bool serverside, bool clientside, bool bundling)
	{
		this.isFoundation = name.Contains("foundation");
	}
}
