using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020005B8 RID: 1464
public class OutlineManager : MonoBehaviour, IClientComponent
{
	// Token: 0x040023CA RID: 9162
	public static Material blurMat;

	// Token: 0x040023CB RID: 9163
	public List<OutlineObject> objectsToRender;

	// Token: 0x040023CC RID: 9164
	public float blurAmount = 2f;

	// Token: 0x040023CD RID: 9165
	public Material glowSolidMaterial;

	// Token: 0x040023CE RID: 9166
	public Material blendGlowMaterial;
}
