using System;

// Token: 0x0200060E RID: 1550
public interface IMissionEntityListener
{
	// Token: 0x06002DE1 RID: 11745
	void MissionStarted(BasePlayer assignee, BaseMission.MissionInstance instance);

	// Token: 0x06002DE2 RID: 11746
	void MissionEnded(BasePlayer assignee, BaseMission.MissionInstance instance);
}
