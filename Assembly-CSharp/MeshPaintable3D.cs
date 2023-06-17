using System;
using UnityEngine;

// Token: 0x020002CE RID: 718
public class MeshPaintable3D : BaseMeshPaintable
{
	// Token: 0x040016A2 RID: 5794
	[ClientVar]
	public static float brushScale = 2f;

	// Token: 0x040016A3 RID: 5795
	[ClientVar]
	public static float uvBufferScale = 2f;

	// Token: 0x040016A4 RID: 5796
	public string replacementTextureName = "_MainTex";

	// Token: 0x040016A5 RID: 5797
	public int textureWidth = 256;

	// Token: 0x040016A6 RID: 5798
	public int textureHeight = 256;

	// Token: 0x040016A7 RID: 5799
	public Camera cameraPreview;

	// Token: 0x040016A8 RID: 5800
	public Camera camera3D;
}
