using System;
using UnityEngine;

// Token: 0x02000614 RID: 1556
public class MissionObjective : ScriptableObject
{
	// Token: 0x06002DFB RID: 11771 RVA: 0x000063A5 File Offset: 0x000045A5
	public virtual void MissionStarted(int index, BaseMission.MissionInstance instance)
	{
	}

	// Token: 0x06002DFC RID: 11772 RVA: 0x001140F9 File Offset: 0x001122F9
	public virtual void ObjectiveStarted(BasePlayer playerFor, int index, BaseMission.MissionInstance instance)
	{
		instance.objectiveStatuses[index].started = true;
		playerFor.MissionDirty(true);
	}

	// Token: 0x06002DFD RID: 11773 RVA: 0x00114110 File Offset: 0x00112310
	public bool IsStarted(int index, BaseMission.MissionInstance instance)
	{
		return instance.objectiveStatuses[index].started;
	}

	// Token: 0x06002DFE RID: 11774 RVA: 0x0011411F File Offset: 0x0011231F
	public bool CanProgress(int index, BaseMission.MissionInstance instance)
	{
		return !instance.GetMission().objectives[index].onlyProgressIfStarted || this.IsStarted(index, instance);
	}

	// Token: 0x06002DFF RID: 11775 RVA: 0x00114140 File Offset: 0x00112340
	public bool ShouldObjectiveStart(int index, BaseMission.MissionInstance instance)
	{
		foreach (int num in instance.GetMission().objectives[index].startAfterCompletedObjectives)
		{
			if (!instance.objectiveStatuses[num].completed && !instance.objectiveStatuses[num].failed)
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x06002E00 RID: 11776 RVA: 0x00114193 File Offset: 0x00112393
	public bool IsCompleted(int index, BaseMission.MissionInstance instance)
	{
		return instance.objectiveStatuses[index].completed || instance.objectiveStatuses[index].failed;
	}

	// Token: 0x06002E01 RID: 11777 RVA: 0x001141B3 File Offset: 0x001123B3
	public virtual bool ShouldThink(int index, BaseMission.MissionInstance instance)
	{
		return !this.IsCompleted(index, instance);
	}

	// Token: 0x06002E02 RID: 11778 RVA: 0x001141C0 File Offset: 0x001123C0
	public virtual void CompleteObjective(int index, BaseMission.MissionInstance instance, BasePlayer playerFor)
	{
		instance.objectiveStatuses[index].completed = true;
		instance.GetMission().OnObjectiveCompleted(index, instance, playerFor);
	}

	// Token: 0x06002E03 RID: 11779 RVA: 0x000063A5 File Offset: 0x000045A5
	public virtual void ProcessMissionEvent(BasePlayer playerFor, BaseMission.MissionInstance instance, int index, BaseMission.MissionEventType type, string identifier, float amount)
	{
	}

	// Token: 0x06002E04 RID: 11780 RVA: 0x001141DE File Offset: 0x001123DE
	public virtual void Think(int index, BaseMission.MissionInstance instance, BasePlayer assignee, float delta)
	{
		if (this.ShouldObjectiveStart(index, instance) && !this.IsStarted(index, instance))
		{
			this.ObjectiveStarted(assignee, index, instance);
		}
	}
}
