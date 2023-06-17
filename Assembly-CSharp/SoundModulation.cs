using System;
using UnityEngine;

// Token: 0x02000245 RID: 581
public class SoundModulation : MonoBehaviour, IClientComponent
{
	// Token: 0x040014C3 RID: 5315
	private const int parameterCount = 4;

	// Token: 0x02000C7E RID: 3198
	public enum Parameter
	{
		// Token: 0x04004382 RID: 17282
		Gain,
		// Token: 0x04004383 RID: 17283
		Pitch,
		// Token: 0x04004384 RID: 17284
		Spread,
		// Token: 0x04004385 RID: 17285
		MaxDistance
	}

	// Token: 0x02000C7F RID: 3199
	[Serializable]
	public class Modulator
	{
		// Token: 0x04004386 RID: 17286
		public SoundModulation.Parameter param;

		// Token: 0x04004387 RID: 17287
		public float value = 1f;
	}
}
