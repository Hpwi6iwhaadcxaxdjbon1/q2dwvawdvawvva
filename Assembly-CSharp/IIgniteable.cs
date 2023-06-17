using System;
using UnityEngine;

// Token: 0x02000413 RID: 1043
public interface IIgniteable
{
	// Token: 0x0600233E RID: 9022
	void Ignite(Vector3 fromPos);

	// Token: 0x0600233F RID: 9023
	bool CanIgnite();
}
