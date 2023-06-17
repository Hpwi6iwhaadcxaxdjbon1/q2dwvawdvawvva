using System;
using UnityEngine;

// Token: 0x02000929 RID: 2345
public class BlurTexture : ProcessedTexture
{
	// Token: 0x0600385C RID: 14428 RVA: 0x001502AD File Offset: 0x0014E4AD
	public BlurTexture(int width, int height, bool linear = true)
	{
		this.material = base.CreateMaterial("Hidden/Rust/SeparableBlur");
		this.result = base.CreateRenderTexture("Blur Texture", width, height, linear);
	}

	// Token: 0x0600385D RID: 14429 RVA: 0x001502DA File Offset: 0x0014E4DA
	public void Blur(float radius)
	{
		this.Blur(this.result, radius);
	}

	// Token: 0x0600385E RID: 14430 RVA: 0x001502EC File Offset: 0x0014E4EC
	public void Blur(Texture source, float radius)
	{
		RenderTexture renderTexture = base.CreateTemporary();
		this.material.SetVector("offsets", new Vector4(radius / (float)Screen.width, 0f, 0f, 0f));
		Graphics.Blit(source, renderTexture, this.material, 0);
		this.material.SetVector("offsets", new Vector4(0f, radius / (float)Screen.height, 0f, 0f));
		Graphics.Blit(renderTexture, this.result, this.material, 0);
		base.ReleaseTemporary(renderTexture);
	}
}
