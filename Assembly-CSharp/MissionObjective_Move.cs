using System;
using UnityEngine;

// Token: 0x02000619 RID: 1561
[CreateAssetMenu(menuName = "Rust/Missions/OBJECTIVES/Move")]
public class MissionObjective_Move : MissionObjective
{
	// Token: 0x040025A4 RID: 9636
	public string positionName = "default";

	// Token: 0x040025A5 RID: 9637
	public float distForCompletion = 3f;

	// Token: 0x040025A6 RID: 9638
	public bool use2D;

	// Token: 0x06002E16 RID: 11798 RVA: 0x0011457B File Offset: 0x0011277B
	public override void ObjectiveStarted(BasePlayer playerFor, int index, BaseMission.MissionInstance instance)
	{
		base.ObjectiveStarted(playerFor, index, instance);
		instance.missionLocation = instance.GetMissionPoint(this.positionName, playerFor);
		playerFor.MissionDirty(true);
	}

	// Token: 0x06002E17 RID: 11799 RVA: 0x001145A0 File Offset: 0x001127A0
	public override void Think(int index, BaseMission.MissionInstance instance, BasePlayer assignee, float delta)
	{
		base.Think(index, instance, assignee, delta);
		if (!this.ShouldThink(index, instance))
		{
			return;
		}
		Vector3 missionPoint = instance.GetMissionPoint(this.positionName, assignee);
		if ((this.use2D ? Vector3Ex.Distance2D(missionPoint, assignee.transform.position) : Vector3.Distance(missionPoint, assignee.transform.position)) <= this.distForCompletion)
		{
			this.CompleteObjective(index, instance, assignee);
			assignee.MissionDirty(true);
		}
	}
}
