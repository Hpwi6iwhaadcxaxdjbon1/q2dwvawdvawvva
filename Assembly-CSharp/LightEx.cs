using System;
using UnityEngine;

// Token: 0x020002C5 RID: 709
public class LightEx : UpdateBehaviour, IClientComponent
{
	// Token: 0x04001665 RID: 5733
	public bool alterColor;

	// Token: 0x04001666 RID: 5734
	public float colorTimeScale = 1f;

	// Token: 0x04001667 RID: 5735
	public Color colorA = Color.red;

	// Token: 0x04001668 RID: 5736
	public Color colorB = Color.yellow;

	// Token: 0x04001669 RID: 5737
	public AnimationCurve blendCurve = new AnimationCurve();

	// Token: 0x0400166A RID: 5738
	public bool loopColor = true;

	// Token: 0x0400166B RID: 5739
	public bool alterIntensity;

	// Token: 0x0400166C RID: 5740
	public float intensityTimeScale = 1f;

	// Token: 0x0400166D RID: 5741
	public AnimationCurve intenseCurve = new AnimationCurve();

	// Token: 0x0400166E RID: 5742
	public float intensityCurveScale = 3f;

	// Token: 0x0400166F RID: 5743
	public bool loopIntensity = true;

	// Token: 0x04001670 RID: 5744
	public bool randomOffset;

	// Token: 0x04001671 RID: 5745
	public float randomIntensityStartScale = -1f;

	// Token: 0x06001D75 RID: 7541 RVA: 0x000CAC2E File Offset: 0x000C8E2E
	protected void OnValidate()
	{
		LightEx.CheckConflict(base.gameObject);
	}

	// Token: 0x06001D76 RID: 7542 RVA: 0x00007A3C File Offset: 0x00005C3C
	public static bool CheckConflict(GameObject go)
	{
		return false;
	}
}
