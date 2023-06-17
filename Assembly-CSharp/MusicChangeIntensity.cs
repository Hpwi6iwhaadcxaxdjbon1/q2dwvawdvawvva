using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000230 RID: 560
public class MusicChangeIntensity : MonoBehaviour
{
	// Token: 0x04001429 RID: 5161
	public float raiseTo;

	// Token: 0x0400142A RID: 5162
	public List<MusicChangeIntensity.DistanceIntensity> distanceIntensities = new List<MusicChangeIntensity.DistanceIntensity>();

	// Token: 0x0400142B RID: 5163
	public float tickInterval = 0.2f;

	// Token: 0x02000C75 RID: 3189
	[Serializable]
	public class DistanceIntensity
	{
		// Token: 0x04004353 RID: 17235
		public float distance = 60f;

		// Token: 0x04004354 RID: 17236
		public float raiseTo;

		// Token: 0x04004355 RID: 17237
		public bool forceStartMusicInSuppressedMusicZones;
	}
}
