using System;
using UnityEngine;

// Token: 0x02000907 RID: 2311
public class NetworkSleep : MonoBehaviour
{
	// Token: 0x040032F4 RID: 13044
	public static int totalBehavioursDisabled;

	// Token: 0x040032F5 RID: 13045
	public static int totalCollidersDisabled;

	// Token: 0x040032F6 RID: 13046
	public Behaviour[] behaviours;

	// Token: 0x040032F7 RID: 13047
	public Collider[] colliders;

	// Token: 0x040032F8 RID: 13048
	internal int BehavioursDisabled;

	// Token: 0x040032F9 RID: 13049
	internal int CollidersDisabled;
}
