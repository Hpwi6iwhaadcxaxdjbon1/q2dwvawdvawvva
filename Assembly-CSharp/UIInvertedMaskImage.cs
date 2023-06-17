using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020008AB RID: 2219
public class UIInvertedMaskImage : Image
{
	// Token: 0x040031D5 RID: 12757
	private Material cachedMaterial;

	// Token: 0x17000461 RID: 1121
	// (get) Token: 0x06003714 RID: 14100 RVA: 0x0014C7F6 File Offset: 0x0014A9F6
	public override Material materialForRendering
	{
		get
		{
			if (this.cachedMaterial == null)
			{
				this.cachedMaterial = UnityEngine.Object.Instantiate<Material>(base.materialForRendering);
				this.cachedMaterial.SetInt("_StencilComp", 6);
			}
			return this.cachedMaterial;
		}
	}
}
