using System;
using UnityEngine;

// Token: 0x02000248 RID: 584
public class SoundPlayerCull : MonoBehaviour, IClientComponent, ILOD
{
	// Token: 0x040014CC RID: 5324
	public SoundPlayer soundPlayer;

	// Token: 0x040014CD RID: 5325
	public float cullDistance = 100f;
}
