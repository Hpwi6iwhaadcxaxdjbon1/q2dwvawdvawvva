using System;
using UnityEngine;

// Token: 0x020005BD RID: 1469
[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
[AddComponentMenu("Image Effects/Natural Bloom and Dirty Lens")]
public class NaturalBloomAndDirtyLens : MonoBehaviour
{
	// Token: 0x040023D8 RID: 9176
	public Shader shader;

	// Token: 0x040023D9 RID: 9177
	public Texture2D lensDirtTexture;

	// Token: 0x040023DA RID: 9178
	public float range = 10000f;

	// Token: 0x040023DB RID: 9179
	public float cutoff = 1f;

	// Token: 0x040023DC RID: 9180
	[Range(0f, 1f)]
	public float bloomIntensity = 0.05f;

	// Token: 0x040023DD RID: 9181
	[Range(0f, 1f)]
	public float lensDirtIntensity = 0.05f;

	// Token: 0x040023DE RID: 9182
	[Range(0f, 4f)]
	public float spread = 1f;

	// Token: 0x040023DF RID: 9183
	[Range(0f, 4f)]
	public int iterations = 1;

	// Token: 0x040023E0 RID: 9184
	[Range(1f, 10f)]
	public int mips = 6;

	// Token: 0x040023E1 RID: 9185
	public float[] mipWeights = new float[]
	{
		0.5f,
		0.6f,
		0.6f,
		0.45f,
		0.35f,
		0.23f
	};

	// Token: 0x040023E2 RID: 9186
	public bool highPrecision;

	// Token: 0x040023E3 RID: 9187
	public bool downscaleSource;

	// Token: 0x040023E4 RID: 9188
	public bool debug;

	// Token: 0x040023E5 RID: 9189
	public bool temporalFilter;

	// Token: 0x040023E6 RID: 9190
	[Range(0.01f, 1f)]
	public float temporalFilterWeight = 0.75f;
}
