using System;
using UnityEngine;

// Token: 0x02000240 RID: 576
public class SoundFade : MonoBehaviour, IClientComponent
{
	// Token: 0x040014B5 RID: 5301
	public SoundFadeHQAudioFilter hqFadeFilter;

	// Token: 0x040014B6 RID: 5302
	public float currentGain = 1f;

	// Token: 0x040014B7 RID: 5303
	public float startingGain;

	// Token: 0x040014B8 RID: 5304
	public float finalGain = 1f;

	// Token: 0x040014B9 RID: 5305
	public int sampleRate = 44100;

	// Token: 0x040014BA RID: 5306
	public bool highQualityFadeCompleted;

	// Token: 0x040014BB RID: 5307
	public float length;

	// Token: 0x040014BC RID: 5308
	public SoundFade.Direction currentDirection;

	// Token: 0x02000C7D RID: 3197
	public enum Direction
	{
		// Token: 0x0400437F RID: 17279
		In,
		// Token: 0x04004380 RID: 17280
		Out
	}
}
