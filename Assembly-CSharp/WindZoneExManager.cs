using System;
using UnityEngine;

// Token: 0x02000735 RID: 1845
[ExecuteInEditMode]
[RequireComponent(typeof(WindZone))]
public class WindZoneExManager : MonoBehaviour
{
	// Token: 0x040029D7 RID: 10711
	public float maxAccumMain = 4f;

	// Token: 0x040029D8 RID: 10712
	public float maxAccumTurbulence = 4f;

	// Token: 0x040029D9 RID: 10713
	public float globalMainScale = 1f;

	// Token: 0x040029DA RID: 10714
	public float globalTurbulenceScale = 1f;

	// Token: 0x040029DB RID: 10715
	public Transform testPosition;

	// Token: 0x02000E42 RID: 3650
	private enum TestMode
	{
		// Token: 0x04004ADA RID: 19162
		Disabled,
		// Token: 0x04004ADB RID: 19163
		Low
	}
}
