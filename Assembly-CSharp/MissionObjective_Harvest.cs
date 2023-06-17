using System;
using UnityEngine;

// Token: 0x02000617 RID: 1559
[CreateAssetMenu(menuName = "Rust/Missions/OBJECTIVES/Harvest")]
public class MissionObjective_Harvest : MissionObjective
{
	// Token: 0x0400259E RID: 9630
	public string[] itemShortnames;

	// Token: 0x0400259F RID: 9631
	public int targetItemAmount;

	// Token: 0x06002E0E RID: 11790 RVA: 0x001141FD File Offset: 0x001123FD
	public override void ObjectiveStarted(BasePlayer playerFor, int index, BaseMission.MissionInstance instance)
	{
		base.ObjectiveStarted(playerFor, index, instance);
	}

	// Token: 0x06002E0F RID: 11791 RVA: 0x00114314 File Offset: 0x00112514
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
		if (type == BaseMission.MissionEventType.HARVEST)
		{
			string[] array = this.itemShortnames;
			int i = 0;
			while (i < array.Length)
			{
				if (array[i] == identifier)
				{
					instance.objectiveStatuses[index].genericInt1 += (int)amount;
					if (instance.objectiveStatuses[index].genericInt1 >= this.targetItemAmount)
					{
						this.CompleteObjective(index, instance, playerFor);
						playerFor.MissionDirty(true);
						return;
					}
					break;
				}
				else
				{
					i++;
				}
			}
		}
	}

	// Token: 0x06002E10 RID: 11792 RVA: 0x0011428A File Offset: 0x0011248A
	public override void Think(int index, BaseMission.MissionInstance instance, BasePlayer assignee, float delta)
	{
		base.Think(index, instance, assignee, delta);
	}
}
