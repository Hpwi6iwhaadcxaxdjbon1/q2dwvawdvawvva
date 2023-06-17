using System;
using UnityEngine;

// Token: 0x020005BC RID: 1468
[ExecuteInEditMode]
public class MaterialOverlay : MonoBehaviour
{
	// Token: 0x040023D7 RID: 9175
	public Material material;

	// Token: 0x06002C1D RID: 11293 RVA: 0x0010B3D4 File Offset: 0x001095D4
	private void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		if (!this.material)
		{
			Graphics.Blit(source, destination);
			return;
		}
		for (int i = 0; i < this.material.passCount; i++)
		{
			Graphics.Blit(source, destination, this.material, i);
		}
	}
}
