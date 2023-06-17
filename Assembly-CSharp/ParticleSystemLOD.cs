using System;
using UnityEngine;

// Token: 0x02000543 RID: 1347
public class ParticleSystemLOD : LODComponentParticleSystem
{
	// Token: 0x04002213 RID: 8723
	[Horizontal(1, 0)]
	public ParticleSystemLOD.State[] States;

	// Token: 0x02000D41 RID: 3393
	[Serializable]
	public class State
	{
		// Token: 0x040046BC RID: 18108
		public float distance;

		// Token: 0x040046BD RID: 18109
		[Range(0f, 1f)]
		public float emission;
	}
}
