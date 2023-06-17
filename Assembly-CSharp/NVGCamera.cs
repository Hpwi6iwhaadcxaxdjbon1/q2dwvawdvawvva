using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

// Token: 0x020000FC RID: 252
public class NVGCamera : FacepunchBehaviour, IClothingChanged
{
	// Token: 0x04000DCB RID: 3531
	public static NVGCamera instance;

	// Token: 0x04000DCC RID: 3532
	public PostProcessVolume postProcessVolume;

	// Token: 0x04000DCD RID: 3533
	public GameObject lights;
}
