using System;
using ConVar;
using UnityEngine;

// Token: 0x0200099A RID: 2458
[RequireComponent(typeof(Light))]
[ExecuteInEditMode]
public class NGSS_Directional : MonoBehaviour
{
	// Token: 0x040034D4 RID: 13524
	[Tooltip("Overall softness for both PCF and PCSS shadows.\nRecommended value: 0.01.")]
	[Range(0f, 0.02f)]
	public float PCSS_GLOBAL_SOFTNESS = 0.01f;

	// Token: 0x040034D5 RID: 13525
	[Tooltip("PCSS softness when shadows is close to caster.\nRecommended value: 0.05.")]
	[Range(0f, 1f)]
	public float PCSS_FILTER_DIR_MIN = 0.05f;

	// Token: 0x040034D6 RID: 13526
	[Tooltip("PCSS softness when shadows is far from caster.\nRecommended value: 0.25.\nIf too high can lead to visible artifacts when early bailout is enabled.")]
	[Range(0f, 0.5f)]
	public float PCSS_FILTER_DIR_MAX = 0.25f;

	// Token: 0x040034D7 RID: 13527
	[Tooltip("Amount of banding or noise. Example: 0.0 gives 100 % Banding and 10.0 gives 100 % Noise.")]
	[Range(0f, 10f)]
	public float BANDING_NOISE_AMOUNT = 1f;

	// Token: 0x040034D8 RID: 13528
	[Tooltip("Recommended values: Mobile = 16, Consoles = 25, Desktop Low = 32, Desktop High = 64")]
	public NGSS_Directional.SAMPLER_COUNT SAMPLERS_COUNT;

	// Token: 0x06003A74 RID: 14964 RVA: 0x00159BC8 File Offset: 0x00157DC8
	private void Update()
	{
		bool globalSettings = ConVar.Graphics.shadowquality >= 2;
		this.SetGlobalSettings(globalSettings);
	}

	// Token: 0x06003A75 RID: 14965 RVA: 0x00159BE8 File Offset: 0x00157DE8
	private void SetGlobalSettings(bool enabled)
	{
		if (enabled)
		{
			Shader.SetGlobalFloat("NGSS_PCSS_GLOBAL_SOFTNESS", this.PCSS_GLOBAL_SOFTNESS);
			Shader.SetGlobalFloat("NGSS_PCSS_FILTER_DIR_MIN", (this.PCSS_FILTER_DIR_MIN > this.PCSS_FILTER_DIR_MAX) ? this.PCSS_FILTER_DIR_MAX : this.PCSS_FILTER_DIR_MIN);
			Shader.SetGlobalFloat("NGSS_PCSS_FILTER_DIR_MAX", (this.PCSS_FILTER_DIR_MAX < this.PCSS_FILTER_DIR_MIN) ? this.PCSS_FILTER_DIR_MIN : this.PCSS_FILTER_DIR_MAX);
			Shader.SetGlobalFloat("NGSS_POISSON_SAMPLING_NOISE_DIR", this.BANDING_NOISE_AMOUNT);
		}
	}

	// Token: 0x02000ED2 RID: 3794
	public enum SAMPLER_COUNT
	{
		// Token: 0x04004D1E RID: 19742
		SAMPLERS_16,
		// Token: 0x04004D1F RID: 19743
		SAMPLERS_25,
		// Token: 0x04004D20 RID: 19744
		SAMPLERS_32,
		// Token: 0x04004D21 RID: 19745
		SAMPLERS_64
	}
}
