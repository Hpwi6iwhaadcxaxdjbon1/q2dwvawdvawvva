using System;
using UnityEngine;

// Token: 0x0200092D RID: 2349
public class ProcessedTexture
{
	// Token: 0x0400333A RID: 13114
	protected RenderTexture result;

	// Token: 0x0400333B RID: 13115
	protected Material material;

	// Token: 0x0600386E RID: 14446 RVA: 0x00150FC8 File Offset: 0x0014F1C8
	public void Dispose()
	{
		this.DestroyRenderTexture(ref this.result);
		this.DestroyMaterial(ref this.material);
	}

	// Token: 0x0600386F RID: 14447 RVA: 0x00150FE2 File Offset: 0x0014F1E2
	protected RenderTexture CreateRenderTexture(string name, int width, int height, bool linear)
	{
		RenderTexture renderTexture = new RenderTexture(width, height, 0, RenderTextureFormat.ARGB32, linear ? RenderTextureReadWrite.Linear : RenderTextureReadWrite.sRGB);
		renderTexture.hideFlags = HideFlags.DontSave;
		renderTexture.name = name;
		renderTexture.filterMode = FilterMode.Bilinear;
		renderTexture.anisoLevel = 0;
		renderTexture.Create();
		return renderTexture;
	}

	// Token: 0x06003870 RID: 14448 RVA: 0x00151019 File Offset: 0x0014F219
	protected void DestroyRenderTexture(ref RenderTexture rt)
	{
		if (rt == null)
		{
			return;
		}
		UnityEngine.Object.Destroy(rt);
		rt = null;
	}

	// Token: 0x06003871 RID: 14449 RVA: 0x00151030 File Offset: 0x0014F230
	protected RenderTexture CreateTemporary()
	{
		return RenderTexture.GetTemporary(this.result.width, this.result.height, this.result.depth, this.result.format, this.result.sRGB ? RenderTextureReadWrite.sRGB : RenderTextureReadWrite.Linear);
	}

	// Token: 0x06003872 RID: 14450 RVA: 0x0015107F File Offset: 0x0014F27F
	protected void ReleaseTemporary(RenderTexture rt)
	{
		RenderTexture.ReleaseTemporary(rt);
	}

	// Token: 0x06003873 RID: 14451 RVA: 0x00151087 File Offset: 0x0014F287
	protected Material CreateMaterial(string shader)
	{
		return this.CreateMaterial(Shader.Find(shader));
	}

	// Token: 0x06003874 RID: 14452 RVA: 0x00151095 File Offset: 0x0014F295
	protected Material CreateMaterial(Shader shader)
	{
		return new Material(shader)
		{
			hideFlags = HideFlags.DontSave
		};
	}

	// Token: 0x06003875 RID: 14453 RVA: 0x00151019 File Offset: 0x0014F219
	protected void DestroyMaterial(ref Material mat)
	{
		if (mat == null)
		{
			return;
		}
		UnityEngine.Object.Destroy(mat);
		mat = null;
	}

	// Token: 0x06003876 RID: 14454 RVA: 0x001510A5 File Offset: 0x0014F2A5
	public static implicit operator Texture(ProcessedTexture t)
	{
		return t.result;
	}
}
