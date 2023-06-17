using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200021E RID: 542
[CreateAssetMenu(menuName = "Rust/Ambience Definition")]
public class AmbienceDefinition : ScriptableObject
{
	// Token: 0x0400139F RID: 5023
	[Header("Sound")]
	public List<SoundDefinition> sounds;

	// Token: 0x040013A0 RID: 5024
	[Horizontal(2, -1)]
	public AmbienceDefinition.ValueRange stingFrequency = new AmbienceDefinition.ValueRange(15f, 30f);

	// Token: 0x040013A1 RID: 5025
	[Header("Environment")]
	[InspectorFlags]
	public TerrainBiome.Enum biomes = (TerrainBiome.Enum)(-1);

	// Token: 0x040013A2 RID: 5026
	[InspectorFlags]
	public TerrainTopology.Enum topologies = (TerrainTopology.Enum)(-1);

	// Token: 0x040013A3 RID: 5027
	public EnvironmentType environmentType = EnvironmentType.Underground;

	// Token: 0x040013A4 RID: 5028
	public bool useEnvironmentType;

	// Token: 0x040013A5 RID: 5029
	public AnimationCurve time = AnimationCurve.Linear(0f, 0f, 24f, 0f);

	// Token: 0x040013A6 RID: 5030
	[Horizontal(2, -1)]
	public AmbienceDefinition.ValueRange rain = new AmbienceDefinition.ValueRange(0f, 1f);

	// Token: 0x040013A7 RID: 5031
	[Horizontal(2, -1)]
	public AmbienceDefinition.ValueRange wind = new AmbienceDefinition.ValueRange(0f, 1f);

	// Token: 0x040013A8 RID: 5032
	[Horizontal(2, -1)]
	public AmbienceDefinition.ValueRange snow = new AmbienceDefinition.ValueRange(0f, 1f);

	// Token: 0x02000C6A RID: 3178
	[Serializable]
	public class ValueRange
	{
		// Token: 0x04004316 RID: 17174
		public float min;

		// Token: 0x04004317 RID: 17175
		public float max;

		// Token: 0x06004EE6 RID: 20198 RVA: 0x001A50AE File Offset: 0x001A32AE
		public ValueRange(float min, float max)
		{
			this.min = min;
			this.max = max;
		}
	}
}
