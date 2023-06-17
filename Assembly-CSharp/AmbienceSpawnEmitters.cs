using System;
using UnityEngine;

// Token: 0x02000223 RID: 547
public class AmbienceSpawnEmitters : MonoBehaviour, IClientComponent
{
	// Token: 0x040013C8 RID: 5064
	public int baseEmitterCount = 5;

	// Token: 0x040013C9 RID: 5065
	public int baseEmitterDistance = 10;

	// Token: 0x040013CA RID: 5066
	public GameObjectRef emitterPrefab;
}
