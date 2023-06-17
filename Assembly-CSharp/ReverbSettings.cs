using System;
using UnityEngine;

// Token: 0x02000238 RID: 568
[CreateAssetMenu(menuName = "Rust/Reverb Settings")]
public class ReverbSettings : ScriptableObject
{
	// Token: 0x04001462 RID: 5218
	[Range(-10000f, 0f)]
	public int room;

	// Token: 0x04001463 RID: 5219
	[Range(-10000f, 0f)]
	public int roomHF;

	// Token: 0x04001464 RID: 5220
	[Range(-10000f, 0f)]
	public int roomLF;

	// Token: 0x04001465 RID: 5221
	[Range(0.1f, 20f)]
	public float decayTime;

	// Token: 0x04001466 RID: 5222
	[Range(0.1f, 2f)]
	public float decayHFRatio;

	// Token: 0x04001467 RID: 5223
	[Range(-10000f, 1000f)]
	public int reflections;

	// Token: 0x04001468 RID: 5224
	[Range(0f, 0.3f)]
	public float reflectionsDelay;

	// Token: 0x04001469 RID: 5225
	[Range(-10000f, 2000f)]
	public int reverb;

	// Token: 0x0400146A RID: 5226
	[Range(0f, 0.1f)]
	public float reverbDelay;

	// Token: 0x0400146B RID: 5227
	[Range(1000f, 20000f)]
	public float HFReference;

	// Token: 0x0400146C RID: 5228
	[Range(20f, 1000f)]
	public float LFReference;

	// Token: 0x0400146D RID: 5229
	[Range(0f, 100f)]
	public float diffusion;

	// Token: 0x0400146E RID: 5230
	[Range(0f, 100f)]
	public float density;
}
