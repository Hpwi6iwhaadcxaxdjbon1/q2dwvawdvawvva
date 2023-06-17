using System;
using UnityEngine;

// Token: 0x0200022A RID: 554
public class FlybySound : MonoBehaviour, IClientComponent
{
	// Token: 0x040013FB RID: 5115
	public SoundDefinition flybySound;

	// Token: 0x040013FC RID: 5116
	public float flybySoundDistance = 7f;

	// Token: 0x040013FD RID: 5117
	public SoundDefinition closeFlybySound;

	// Token: 0x040013FE RID: 5118
	public float closeFlybyDistance = 3f;
}
