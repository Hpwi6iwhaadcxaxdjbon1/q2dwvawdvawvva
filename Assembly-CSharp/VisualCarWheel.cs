using System;
using UnityEngine;

// Token: 0x020004B9 RID: 1209
[Serializable]
public class VisualCarWheel : CarWheel
{
	// Token: 0x0400200C RID: 8204
	public Transform visualWheel;

	// Token: 0x0400200D RID: 8205
	public Transform visualWheelSteering;

	// Token: 0x0400200E RID: 8206
	public bool visualPowerWheel = true;

	// Token: 0x0400200F RID: 8207
	public ParticleSystem snowFX;

	// Token: 0x04002010 RID: 8208
	public ParticleSystem sandFX;

	// Token: 0x04002011 RID: 8209
	public ParticleSystem dirtFX;

	// Token: 0x04002012 RID: 8210
	public ParticleSystem asphaltFX;

	// Token: 0x04002013 RID: 8211
	public ParticleSystem waterFX;

	// Token: 0x04002014 RID: 8212
	public ParticleSystem snowSpinFX;

	// Token: 0x04002015 RID: 8213
	public ParticleSystem sandSpinFX;

	// Token: 0x04002016 RID: 8214
	public ParticleSystem dirtSpinFX;

	// Token: 0x04002017 RID: 8215
	public ParticleSystem asphaltSpinFX;
}
