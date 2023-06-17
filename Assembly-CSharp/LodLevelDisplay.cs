using System;
using UnityEngine;

// Token: 0x02000327 RID: 807
public class LodLevelDisplay : MonoBehaviour, IEditorComponent
{
	// Token: 0x040017F3 RID: 6131
	public Color TextColor = Color.green;

	// Token: 0x040017F4 RID: 6132
	[Range(1f, 6f)]
	public float TextScaleMultiplier = 1f;
}
