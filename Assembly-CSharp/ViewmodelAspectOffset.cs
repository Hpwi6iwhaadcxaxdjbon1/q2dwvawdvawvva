using System;
using UnityEngine;

// Token: 0x02000978 RID: 2424
public class ViewmodelAspectOffset : MonoBehaviour
{
	// Token: 0x04003414 RID: 13332
	public Vector3 OffsetAmount = Vector3.zero;

	// Token: 0x04003415 RID: 13333
	[Tooltip("What aspect ratio should we start moving the viewmodel? 16:9 = 1.7, 21:9 = 2.3")]
	public float aspectCutoff = 2f;
}
