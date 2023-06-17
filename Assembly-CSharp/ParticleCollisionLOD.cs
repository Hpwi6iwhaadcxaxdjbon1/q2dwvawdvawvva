using System;

// Token: 0x02000541 RID: 1345
public class ParticleCollisionLOD : LODComponentParticleSystem
{
	// Token: 0x04002211 RID: 8721
	[Horizontal(1, 0)]
	public ParticleCollisionLOD.State[] States;

	// Token: 0x02000D3F RID: 3391
	public enum QualityLevel
	{
		// Token: 0x040046B6 RID: 18102
		Disabled = -1,
		// Token: 0x040046B7 RID: 18103
		HighQuality,
		// Token: 0x040046B8 RID: 18104
		MediumQuality,
		// Token: 0x040046B9 RID: 18105
		LowQuality
	}

	// Token: 0x02000D40 RID: 3392
	[Serializable]
	public class State
	{
		// Token: 0x040046BA RID: 18106
		public float distance;

		// Token: 0x040046BB RID: 18107
		public ParticleCollisionLOD.QualityLevel quality = ParticleCollisionLOD.QualityLevel.Disabled;
	}
}
