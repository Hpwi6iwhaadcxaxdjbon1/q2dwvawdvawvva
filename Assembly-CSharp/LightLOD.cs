using System;
using UnityEngine;

// Token: 0x020008FE RID: 2302
public class LightLOD : MonoBehaviour, ILOD, IClientComponent
{
	// Token: 0x040032D4 RID: 13012
	public float DistanceBias;

	// Token: 0x040032D5 RID: 13013
	public bool ToggleLight;

	// Token: 0x040032D6 RID: 13014
	public bool ToggleShadows = true;

	// Token: 0x060037DF RID: 14303 RVA: 0x000CAC2E File Offset: 0x000C8E2E
	protected void OnValidate()
	{
		LightEx.CheckConflict(base.gameObject);
	}
}
