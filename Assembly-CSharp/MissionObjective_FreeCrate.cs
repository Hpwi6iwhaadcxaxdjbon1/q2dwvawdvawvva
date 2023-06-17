using System;
using UnityEngine;

// Token: 0x02000616 RID: 1558
[CreateAssetMenu(menuName = "Rust/Missions/OBJECTIVES/FreeCrate")]
public class MissionObjective_FreeCrate : MissionObjective
{
	// Token: 0x0400259D RID: 9629
	public int targetAmount;

	// Token: 0x06002E0A RID: 11786 RVA: 0x001141FD File Offset: 0x001123FD
	public override void ObjectiveStarted(BasePlayer playerFor, int index, BaseMission.MissionInstance instance)
	{
		base.ObjectiveStarted(playerFor, index, instance);
	}

	// Token: 0x06002E0B RID: 11787 RVA: 0x001142A0 File Offset: 0x001124A0
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
		if (type == BaseMission.MissionEventType.FREE_CRATE)
		{
			instance.objectiveStatuses[index].genericInt1 += (int)amount;
			if (instance.objectiveStatuses[index].genericInt1 >= this.targetAmount)
			{
				this.CompleteObjective(index, instance, playerFor);
				playerFor.MissionDirty(true);
			}
		}
	}

	// Token: 0x06002E0C RID: 11788 RVA: 0x0011428A File Offset: 0x0011248A
	public override void Think(int index, BaseMission.MissionInstance instance, BasePlayer assignee, float delta)
	{
		base.Think(index, instance, assignee, delta);
	}
}
