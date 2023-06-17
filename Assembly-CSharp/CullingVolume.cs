using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020004F2 RID: 1266
public class CullingVolume : MonoBehaviour, IClientComponent
{
	// Token: 0x040020F9 RID: 8441
	[Tooltip("Override occludee root from children of this object (default) to children of any other object.")]
	public GameObject OccludeeRoot;

	// Token: 0x040020FA RID: 8442
	[Tooltip("Invert visibility. False will show occludes. True will hide them.")]
	public bool Invert;

	// Token: 0x040020FB RID: 8443
	[Tooltip("A portal in the culling volume chain does not toggle objects visible, it merely signals the non-portal volumes to hide their occludees.")]
	public bool Portal;

	// Token: 0x040020FC RID: 8444
	[Tooltip("Secondary culling volumes, connected to this one, that will get signaled when this trigger is activated.")]
	public List<CullingVolume> Connections = new List<CullingVolume>();
}
