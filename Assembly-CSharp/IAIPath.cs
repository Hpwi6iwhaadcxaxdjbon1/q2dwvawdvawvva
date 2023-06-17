using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020001A6 RID: 422
public interface IAIPath
{
	// Token: 0x1700020C RID: 524
	// (get) Token: 0x0600189F RID: 6303
	IEnumerable<IAIPathSpeedZone> SpeedZones { get; }

	// Token: 0x1700020D RID: 525
	// (get) Token: 0x060018A0 RID: 6304
	IEnumerable<IAIPathInterestNode> InterestNodes { get; }

	// Token: 0x060018A1 RID: 6305
	void GetNodesNear(Vector3 point, ref List<IAIPathNode> nearNodes, float dist = 10f);

	// Token: 0x060018A2 RID: 6306
	IAIPathInterestNode GetRandomInterestNodeAwayFrom(Vector3 from, float dist = 10f);

	// Token: 0x060018A3 RID: 6307
	IAIPathNode GetClosestToPoint(Vector3 point);

	// Token: 0x060018A4 RID: 6308
	void AddInterestNode(IAIPathInterestNode interestNode);

	// Token: 0x060018A5 RID: 6309
	void AddSpeedZone(IAIPathSpeedZone speedZone);
}
