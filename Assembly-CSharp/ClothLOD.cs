using System;
using UnityEngine;

// Token: 0x0200052E RID: 1326
public class ClothLOD : FacepunchBehaviour
{
	// Token: 0x040021E3 RID: 8675
	[ServerVar(Help = "distance cloth will simulate until")]
	public static float clothLODDist = 20f;

	// Token: 0x040021E4 RID: 8676
	public Cloth cloth;
}
