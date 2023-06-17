using System;
using UnityEngine;

// Token: 0x020003CB RID: 971
public class HideIfOwnerFirstPerson : EntityComponent<BaseEntity>, IClientComponent, IViewModeChanged
{
	// Token: 0x04001A25 RID: 6693
	public GameObject[] disableGameObjects;

	// Token: 0x04001A26 RID: 6694
	public bool worldModelEffect;
}
