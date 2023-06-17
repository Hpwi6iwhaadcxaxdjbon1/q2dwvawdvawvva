using System;
using UnityEngine;

// Token: 0x02000242 RID: 578
public class SoundFollowCollider : MonoBehaviour, IClientComponent
{
	// Token: 0x040014BD RID: 5309
	public SoundDefinition soundDefinition;

	// Token: 0x040014BE RID: 5310
	public Sound sound;

	// Token: 0x040014BF RID: 5311
	public Bounds soundFollowBounds;

	// Token: 0x040014C0 RID: 5312
	public bool startImmediately;
}
