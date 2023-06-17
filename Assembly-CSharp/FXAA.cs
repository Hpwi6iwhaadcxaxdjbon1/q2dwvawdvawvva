using System;
using UnityEngine;

// Token: 0x0200098A RID: 2442
[AddComponentMenu("Image Effects/FXAA")]
public class FXAA : FXAAPostEffectsBase, IImageEffect
{
	// Token: 0x0400346E RID: 13422
	public Shader shader;

	// Token: 0x0400346F RID: 13423
	private Material mat;

	// Token: 0x06003A1E RID: 14878 RVA: 0x00157FC7 File Offset: 0x001561C7
	private void CreateMaterials()
	{
		if (this.mat == null)
		{
			this.mat = base.CheckShaderAndCreateMaterial(this.shader, this.mat);
		}
	}

	// Token: 0x06003A1F RID: 14879 RVA: 0x00157FEF File Offset: 0x001561EF
	private void Start()
	{
		this.CreateMaterials();
		base.CheckSupport(false);
	}

	// Token: 0x06003A20 RID: 14880 RVA: 0x00157FFF File Offset: 0x001561FF
	public bool IsActive()
	{
		return base.enabled;
	}

	// Token: 0x06003A21 RID: 14881 RVA: 0x00158008 File Offset: 0x00156208
	public void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		this.CreateMaterials();
		float num = 1f / (float)Screen.width;
		float num2 = 1f / (float)Screen.height;
		this.mat.SetVector("_rcpFrame", new Vector4(num, num2, 0f, 0f));
		this.mat.SetVector("_rcpFrameOpt", new Vector4(num * 2f, num2 * 2f, num * 0.5f, num2 * 0.5f));
		Graphics.Blit(source, destination, this.mat);
	}
}
