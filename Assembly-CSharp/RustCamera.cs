using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

// Token: 0x020002E1 RID: 737
public abstract class RustCamera<T> : SingletonComponent<T> where T : RustCamera<T>
{
	// Token: 0x0400172B RID: 5931
	[SerializeField]
	private AmplifyOcclusionEffect ssao;

	// Token: 0x0400172C RID: 5932
	[SerializeField]
	private SEScreenSpaceShadows contactShadows;

	// Token: 0x0400172D RID: 5933
	[SerializeField]
	private VisualizeTexelDensity visualizeTexelDensity;

	// Token: 0x0400172E RID: 5934
	[SerializeField]
	private EnvironmentVolumePropertiesCollection environmentVolumeProperties;

	// Token: 0x0400172F RID: 5935
	[SerializeField]
	private PostProcessLayer post;

	// Token: 0x04001730 RID: 5936
	[SerializeField]
	private PostProcessVolume baseEffectVolume;
}
