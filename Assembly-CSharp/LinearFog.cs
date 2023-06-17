using System;
using UnityEngine;

// Token: 0x020005BB RID: 1467
[ExecuteInEditMode]
public class LinearFog : MonoBehaviour
{
	// Token: 0x040023D1 RID: 9169
	public Material fogMaterial;

	// Token: 0x040023D2 RID: 9170
	public Color fogColor = Color.white;

	// Token: 0x040023D3 RID: 9171
	public float fogStart;

	// Token: 0x040023D4 RID: 9172
	public float fogRange = 1f;

	// Token: 0x040023D5 RID: 9173
	public float fogDensity = 1f;

	// Token: 0x040023D6 RID: 9174
	public bool fogSky;

	// Token: 0x06002C1B RID: 11291 RVA: 0x0010B2D8 File Offset: 0x001094D8
	private void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		if (!this.fogMaterial)
		{
			Graphics.Blit(source, destination);
			return;
		}
		this.fogMaterial.SetColor("_FogColor", this.fogColor);
		this.fogMaterial.SetFloat("_Start", this.fogStart);
		this.fogMaterial.SetFloat("_Range", this.fogRange);
		this.fogMaterial.SetFloat("_Density", this.fogDensity);
		if (this.fogSky)
		{
			this.fogMaterial.SetFloat("_CutOff", 2f);
		}
		else
		{
			this.fogMaterial.SetFloat("_CutOff", 1f);
		}
		for (int i = 0; i < this.fogMaterial.passCount; i++)
		{
			Graphics.Blit(source, destination, this.fogMaterial, i);
		}
	}
}
