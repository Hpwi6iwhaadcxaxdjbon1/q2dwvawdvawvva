using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000478 RID: 1144
public class DamageRenderer : MonoBehaviour, IClientComponent
{
	// Token: 0x04001E02 RID: 7682
	[SerializeField]
	private List<Material> damageShowingMats;

	// Token: 0x04001E03 RID: 7683
	[SerializeField]
	private float maxDamageOpacity = 0.9f;

	// Token: 0x04001E04 RID: 7684
	[SerializeField]
	[HideInInspector]
	private List<DamageRenderer.DamageShowingRenderer> damageShowingRenderers;

	// Token: 0x04001E05 RID: 7685
	[SerializeField]
	[HideInInspector]
	private List<GlassPane> damageShowingGlassRenderers;

	// Token: 0x02000CFA RID: 3322
	[Serializable]
	private struct DamageShowingRenderer
	{
		// Token: 0x040045CE RID: 17870
		public Renderer renderer;

		// Token: 0x040045CF RID: 17871
		public int[] indices;

		// Token: 0x06004FED RID: 20461 RVA: 0x001A7434 File Offset: 0x001A5634
		public DamageShowingRenderer(Renderer renderer, int[] indices)
		{
			this.renderer = renderer;
			this.indices = indices;
		}
	}
}
