using System;
using UnityEngine;

// Token: 0x02000247 RID: 583
public class SoundPlayer : BaseMonoBehaviour, IClientComponent
{
	// Token: 0x040014C5 RID: 5317
	public SoundDefinition soundDefinition;

	// Token: 0x040014C6 RID: 5318
	public bool playImmediately = true;

	// Token: 0x040014C7 RID: 5319
	public float minStartDelay;

	// Token: 0x040014C8 RID: 5320
	public float maxStartDelay;

	// Token: 0x040014C9 RID: 5321
	public bool debugRepeat;

	// Token: 0x040014CA RID: 5322
	public bool pending;

	// Token: 0x040014CB RID: 5323
	public Vector3 soundOffset = Vector3.zero;
}
