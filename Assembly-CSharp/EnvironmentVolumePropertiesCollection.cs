using System;
using UnityEngine;

// Token: 0x02000721 RID: 1825
[CreateAssetMenu(menuName = "Rust/Environment Volume Properties Collection")]
public class EnvironmentVolumePropertiesCollection : ScriptableObject
{
	// Token: 0x0400298A RID: 10634
	public float TransitionSpeed = 1f;

	// Token: 0x0400298B RID: 10635
	[Horizontal(1, 0)]
	public EnvironmentVolumePropertiesCollection.EnvironmentMultiplier[] ReflectionMultipliers;

	// Token: 0x0400298C RID: 10636
	public float DefaultReflectionMultiplier = 1f;

	// Token: 0x0400298D RID: 10637
	[Horizontal(1, 0)]
	public EnvironmentVolumePropertiesCollection.EnvironmentMultiplier[] AmbientMultipliers;

	// Token: 0x0400298E RID: 10638
	public float DefaultAmbientMultiplier = 1f;

	// Token: 0x0400298F RID: 10639
	public EnvironmentVolumePropertiesCollection.OceanParameters OceanOverrides;

	// Token: 0x02000E3B RID: 3643
	[Serializable]
	public class EnvironmentMultiplier
	{
		// Token: 0x04004AB7 RID: 19127
		public EnvironmentType Type;

		// Token: 0x04004AB8 RID: 19128
		public float Multiplier;
	}

	// Token: 0x02000E3C RID: 3644
	[Serializable]
	public class OceanParameters
	{
		// Token: 0x04004AB9 RID: 19129
		public AnimationCurve TransitionCurve = AnimationCurve.Linear(0f, 0f, 40f, 1f);

		// Token: 0x04004ABA RID: 19130
		[Range(0f, 1f)]
		public float DirectionalLightMultiplier = 0.25f;

		// Token: 0x04004ABB RID: 19131
		[Range(0f, 1f)]
		public float AmbientLightMultiplier;

		// Token: 0x04004ABC RID: 19132
		[Range(0f, 1f)]
		public float ReflectionMultiplier = 1f;

		// Token: 0x04004ABD RID: 19133
		[Range(0f, 1f)]
		public float SunMeshBrightnessMultiplier = 1f;

		// Token: 0x04004ABE RID: 19134
		[Range(0f, 1f)]
		public float MoonMeshBrightnessMultiplier = 1f;

		// Token: 0x04004ABF RID: 19135
		[Range(0f, 1f)]
		public float AtmosphereBrightnessMultiplier = 1f;

		// Token: 0x04004AC0 RID: 19136
		[Range(0f, 1f)]
		public float LightColorMultiplier = 1f;

		// Token: 0x04004AC1 RID: 19137
		public Color LightColor = Color.black;

		// Token: 0x04004AC2 RID: 19138
		[Range(0f, 1f)]
		public float SunRayColorMultiplier = 1f;

		// Token: 0x04004AC3 RID: 19139
		public Color SunRayColor = Color.black;

		// Token: 0x04004AC4 RID: 19140
		[Range(0f, 1f)]
		public float MoonRayColorMultiplier = 1f;

		// Token: 0x04004AC5 RID: 19141
		public Color MoonRayColor = Color.black;
	}
}
