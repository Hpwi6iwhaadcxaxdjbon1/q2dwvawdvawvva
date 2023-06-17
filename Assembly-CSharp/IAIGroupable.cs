using System;
using UnityEngine;

// Token: 0x02000391 RID: 913
public interface IAIGroupable
{
	// Token: 0x0600203F RID: 8255
	bool AddMember(IAIGroupable member);

	// Token: 0x06002040 RID: 8256
	void RemoveMember(IAIGroupable member);

	// Token: 0x06002041 RID: 8257
	void JoinGroup(IAIGroupable leader, BaseEntity leaderEntity);

	// Token: 0x06002042 RID: 8258
	void SetGroupRoamRootPosition(Vector3 rootPos);

	// Token: 0x06002043 RID: 8259
	bool InGroup();

	// Token: 0x06002044 RID: 8260
	void LeaveGroup();

	// Token: 0x06002045 RID: 8261
	void SetUngrouped();
}
