using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000236 RID: 566
public class MusicZone : MonoBehaviour, IClientComponent
{
	// Token: 0x0400145E RID: 5214
	public List<MusicTheme> themes;

	// Token: 0x0400145F RID: 5215
	public float priority;

	// Token: 0x04001460 RID: 5216
	public bool suppressAutomaticMusic;
}
