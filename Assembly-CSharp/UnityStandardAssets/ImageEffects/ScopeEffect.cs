using System;
using UnityEngine;

namespace UnityStandardAssets.ImageEffects
{
	// Token: 0x02000A16 RID: 2582
	[ExecuteInEditMode]
	[AddComponentMenu("Image Effects/Other/Scope Overlay")]
	public class ScopeEffect : PostEffectsBase, IImageEffect
	{
		// Token: 0x0400374F RID: 14159
		public Material overlayMaterial;

		// Token: 0x06003D66 RID: 15718 RVA: 0x0000441C File Offset: 0x0000261C
		public override bool CheckResources()
		{
			return true;
		}

		// Token: 0x06003D67 RID: 15719 RVA: 0x00169222 File Offset: 0x00167422
		public bool IsActive()
		{
			return base.enabled && this.CheckResources();
		}

		// Token: 0x06003D68 RID: 15720 RVA: 0x00169234 File Offset: 0x00167434
		public void OnRenderImage(RenderTexture source, RenderTexture destination)
		{
			this.overlayMaterial.SetVector("_Screen", new Vector2((float)Screen.width, (float)Screen.height));
			Graphics.Blit(source, destination, this.overlayMaterial);
		}
	}
}
