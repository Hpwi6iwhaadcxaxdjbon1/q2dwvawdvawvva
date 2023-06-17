using System;
using UnityEngine;

// Token: 0x0200072A RID: 1834
public class LightCloneShadow : MonoBehaviour
{
	// Token: 0x040029AE RID: 10670
	public bool cloneShadowMap;

	// Token: 0x040029AF RID: 10671
	public string shaderPropNameMap = "_MainLightShadowMap";

	// Token: 0x040029B0 RID: 10672
	[Range(0f, 2f)]
	public int cloneShadowMapDownscale = 1;

	// Token: 0x040029B1 RID: 10673
	public RenderTexture map;

	// Token: 0x040029B2 RID: 10674
	public bool cloneShadowMask = true;

	// Token: 0x040029B3 RID: 10675
	public string shaderPropNameMask = "_MainLightShadowMask";

	// Token: 0x040029B4 RID: 10676
	[Range(0f, 2f)]
	public int cloneShadowMaskDownscale = 1;

	// Token: 0x040029B5 RID: 10677
	public RenderTexture mask;
}
