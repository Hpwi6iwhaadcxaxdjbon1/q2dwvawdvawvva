using System;
using UnityEngine;

// Token: 0x020007C4 RID: 1988
public class ChangeSignText : UIDialog
{
	// Token: 0x04002C63 RID: 11363
	public Action<int, Texture2D> onUpdateTexture;

	// Token: 0x04002C64 RID: 11364
	public GameObject objectContainer;

	// Token: 0x04002C65 RID: 11365
	public GameObject currentFrameSection;

	// Token: 0x04002C66 RID: 11366
	public GameObject[] frameOptions;

	// Token: 0x04002C67 RID: 11367
	public Camera cameraPreview;

	// Token: 0x04002C68 RID: 11368
	public Camera camera3D;
}
