using System;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x020008A6 RID: 2214
[ExecuteInEditMode]
public class CameraEx : MonoBehaviour
{
	// Token: 0x040031B8 RID: 12728
	public bool overrideAmbientLight;

	// Token: 0x040031B9 RID: 12729
	public AmbientMode ambientMode;

	// Token: 0x040031BA RID: 12730
	public Color ambientGroundColor;

	// Token: 0x040031BB RID: 12731
	public Color ambientEquatorColor;

	// Token: 0x040031BC RID: 12732
	public Color ambientLight;

	// Token: 0x040031BD RID: 12733
	public float ambientIntensity;

	// Token: 0x040031BE RID: 12734
	public ReflectionProbe reflectionProbe;

	// Token: 0x040031BF RID: 12735
	internal Color old_ambientLight;

	// Token: 0x040031C0 RID: 12736
	internal Color old_ambientGroundColor;

	// Token: 0x040031C1 RID: 12737
	internal Color old_ambientEquatorColor;

	// Token: 0x040031C2 RID: 12738
	internal float old_ambientIntensity;

	// Token: 0x040031C3 RID: 12739
	internal AmbientMode old_ambientMode;

	// Token: 0x040031C4 RID: 12740
	public float aspect;
}
