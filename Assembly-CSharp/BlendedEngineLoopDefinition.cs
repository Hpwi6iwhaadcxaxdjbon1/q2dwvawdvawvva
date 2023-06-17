using System;
using UnityEngine;

// Token: 0x02000226 RID: 550
[CreateAssetMenu(menuName = "Rust/Blended Engine Loop Definition")]
public class BlendedEngineLoopDefinition : ScriptableObject
{
	// Token: 0x040013D3 RID: 5075
	public BlendedEngineLoopDefinition.EngineLoopDefinition[] engineLoops;

	// Token: 0x040013D4 RID: 5076
	public float minRPM;

	// Token: 0x040013D5 RID: 5077
	public float maxRPM;

	// Token: 0x040013D6 RID: 5078
	public float RPMChangeRateUp = 0.5f;

	// Token: 0x040013D7 RID: 5079
	public float RPMChangeRateDown = 0.2f;

	// Token: 0x02000C6D RID: 3181
	[Serializable]
	public class EngineLoopDefinition
	{
		// Token: 0x0400431D RID: 17181
		public SoundDefinition soundDefinition;

		// Token: 0x0400431E RID: 17182
		public float RPM;

		// Token: 0x0400431F RID: 17183
		public float startRPM;

		// Token: 0x04004320 RID: 17184
		public float startFullRPM;

		// Token: 0x04004321 RID: 17185
		public float stopFullRPM;

		// Token: 0x04004322 RID: 17186
		public float stopRPM;

		// Token: 0x06004EE9 RID: 20201 RVA: 0x001A50E6 File Offset: 0x001A32E6
		public float GetPitchForRPM(float targetRPM)
		{
			return targetRPM / this.RPM;
		}
	}
}
