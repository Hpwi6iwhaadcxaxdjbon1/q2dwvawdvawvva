using System;
using UnityEngine;

// Token: 0x0200060F RID: 1551
public interface IMissionProvider
{
	// Token: 0x06002DE3 RID: 11747
	NetworkableId ProviderID();

	// Token: 0x06002DE4 RID: 11748
	Vector3 ProviderPosition();

	// Token: 0x06002DE5 RID: 11749
	BaseEntity Entity();
}
