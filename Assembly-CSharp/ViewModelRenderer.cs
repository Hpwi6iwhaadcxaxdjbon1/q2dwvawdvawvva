using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000977 RID: 2423
public class ViewModelRenderer : MonoBehaviour
{
	// Token: 0x0400340F RID: 13327
	public List<Texture2D> cachedTextureRefs = new List<Texture2D>();

	// Token: 0x04003410 RID: 13328
	public List<ViewModelDrawEvent> opaqueEvents = new List<ViewModelDrawEvent>();

	// Token: 0x04003411 RID: 13329
	public List<ViewModelDrawEvent> transparentEvents = new List<ViewModelDrawEvent>();

	// Token: 0x04003412 RID: 13330
	public Matrix4x4 prevModelMatrix;

	// Token: 0x04003413 RID: 13331
	private Renderer viewModelRenderer;
}
