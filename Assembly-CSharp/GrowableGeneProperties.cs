using System;
using UnityEngine;

// Token: 0x020003F3 RID: 1011
[CreateAssetMenu(menuName = "Rust/Growable Gene Properties")]
public class GrowableGeneProperties : ScriptableObject
{
	// Token: 0x04001A9E RID: 6814
	[ArrayIndexIsEnum(enumType = typeof(GrowableGenetics.GeneType))]
	public GrowableGeneProperties.GeneWeight[] Weights = new GrowableGeneProperties.GeneWeight[5];

	// Token: 0x02000CC5 RID: 3269
	[Serializable]
	public struct GeneWeight
	{
		// Token: 0x040044C4 RID: 17604
		public float BaseWeight;

		// Token: 0x040044C5 RID: 17605
		public float[] SlotWeights;

		// Token: 0x040044C6 RID: 17606
		public float CrossBreedingWeight;
	}
}
