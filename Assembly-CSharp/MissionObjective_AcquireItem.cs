using System;
using UnityEngine;

// Token: 0x02000615 RID: 1557
[CreateAssetMenu(menuName = "Rust/Missions/OBJECTIVES/AcquireItem")]
public class MissionObjective_AcquireItem : MissionObjective
{
	// Token: 0x0400259B RID: 9627
	public string itemShortname;

	// Token: 0x0400259C RID: 9628
	public int targetItemAmount;

	// Token: 0x06002E06 RID: 11782 RVA: 0x001141FD File Offset: 0x001123FD
	public override void ObjectiveStarted(BasePlayer playerFor, int index, BaseMission.MissionInstance instance)
	{
		base.ObjectiveStarted(playerFor, index, instance);
	}

	// Token: 0x06002E07 RID: 11783 RVA: 0x00114208 File Offset: 0x00112408
	public override void ProcessMissionEvent(BasePlayer playerFor, BaseMission.MissionInstance instance, int index, BaseMission.MissionEventType type, string identifier, float amount)
	{
		base.ProcessMissionEvent(playerFor, instance, index, type, identifier, amount);
		if (base.IsCompleted(index, instance))
		{
			return;
		}
		if (!base.CanProgress(index, instance))
		{
			return;
		}
		if (type == BaseMission.MissionEventType.ACQUIRE_ITEM)
		{
			if (this.itemShortname == identifier)
			{
				instance.objectiveStatuses[index].genericInt1 += (int)amount;
			}
			if (instance.objectiveStatuses[index].genericInt1 >= this.targetItemAmount)
			{
				this.CompleteObjective(index, instance, playerFor);
				playerFor.MissionDirty(true);
			}
		}
	}

	// Token: 0x06002E08 RID: 11784 RVA: 0x0011428A File Offset: 0x0011248A
	public override void Think(int index, BaseMission.MissionInstance instance, BasePlayer assignee, float delta)
	{
		base.Think(index, instance, assignee, delta);
	}
}
