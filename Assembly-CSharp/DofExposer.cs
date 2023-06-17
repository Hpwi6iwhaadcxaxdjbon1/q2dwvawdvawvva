using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

// Token: 0x02000304 RID: 772
[ExecuteInEditMode]
public class DofExposer : SingletonComponent<DofExposer>
{
	// Token: 0x04001795 RID: 6037
	public PostProcessVolume PostVolume;

	// Token: 0x04001796 RID: 6038
	public bool DofEnabled;

	// Token: 0x04001797 RID: 6039
	public float FocalLength = 15.24f;

	// Token: 0x04001798 RID: 6040
	public float Blur = 2f;

	// Token: 0x04001799 RID: 6041
	public float FocalAperture = 13.16f;

	// Token: 0x0400179A RID: 6042
	public bool debug;
}
