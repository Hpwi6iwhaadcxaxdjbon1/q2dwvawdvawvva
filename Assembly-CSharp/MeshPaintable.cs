using System;
using UnityEngine;

// Token: 0x020002CD RID: 717
public class MeshPaintable : BaseMeshPaintable
{
	// Token: 0x0400169C RID: 5788
	public string replacementTextureName = "_MainTex";

	// Token: 0x0400169D RID: 5789
	public int textureWidth = 256;

	// Token: 0x0400169E RID: 5790
	public int textureHeight = 256;

	// Token: 0x0400169F RID: 5791
	public Color clearColor = Color.clear;

	// Token: 0x040016A0 RID: 5792
	public Texture2D targetTexture;

	// Token: 0x040016A1 RID: 5793
	public bool hasChanges;
}
