using System;
using UnityEngine;

// Token: 0x02000928 RID: 2344
public class BlendTexture : ProcessedTexture
{
	// Token: 0x06003859 RID: 14425 RVA: 0x00150232 File Offset: 0x0014E432
	public BlendTexture(int width, int height, bool linear = true)
	{
		this.material = base.CreateMaterial("Hidden/BlitCopyAlpha");
		this.result = base.CreateRenderTexture("Blend Texture", width, height, linear);
	}

	// Token: 0x0600385A RID: 14426 RVA: 0x0015025F File Offset: 0x0014E45F
	public void Blend(Texture source, Texture target, float alpha)
	{
		this.material.SetTexture("_BlendTex", target);
		this.material.SetFloat("_Alpha", Mathf.Clamp01(alpha));
		Graphics.Blit(source, this.result, this.material);
	}

	// Token: 0x0600385B RID: 14427 RVA: 0x0015029A File Offset: 0x0014E49A
	public void CopyTo(BlendTexture target)
	{
		Graphics.Blit(this.result, target.result);
	}
}
