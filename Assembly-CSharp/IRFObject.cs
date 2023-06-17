using System;
using UnityEngine;

// Token: 0x020004DE RID: 1246
public interface IRFObject
{
	// Token: 0x06002855 RID: 10325
	Vector3 GetPosition();

	// Token: 0x06002856 RID: 10326
	float GetMaxRange();

	// Token: 0x06002857 RID: 10327
	void RFSignalUpdate(bool on);

	// Token: 0x06002858 RID: 10328
	int GetFrequency();
}
