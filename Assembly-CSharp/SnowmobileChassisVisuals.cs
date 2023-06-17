using System;
using UnityEngine;

// Token: 0x020004A6 RID: 1190
public class SnowmobileChassisVisuals : VehicleChassisVisuals<Snowmobile>, IClientComponent
{
	// Token: 0x04001F2E RID: 7982
	[SerializeField]
	private Animator animator;

	// Token: 0x04001F2F RID: 7983
	[SerializeField]
	private SnowmobileAudio audioScript;

	// Token: 0x04001F30 RID: 7984
	[SerializeField]
	private SnowmobileChassisVisuals.TreadRenderer[] treadRenderers;

	// Token: 0x04001F31 RID: 7985
	[SerializeField]
	private float treadSpeedMultiplier = 0.01f;

	// Token: 0x04001F32 RID: 7986
	[SerializeField]
	private bool flipRightSkiExtension;

	// Token: 0x04001F33 RID: 7987
	[SerializeField]
	private Transform leftSki;

	// Token: 0x04001F34 RID: 7988
	[SerializeField]
	private Transform leftSkiPistonIn;

	// Token: 0x04001F35 RID: 7989
	[SerializeField]
	private Transform leftSkiPistonOut;

	// Token: 0x04001F36 RID: 7990
	[SerializeField]
	private Transform rightSki;

	// Token: 0x04001F37 RID: 7991
	[SerializeField]
	private Transform rightSkiPistonIn;

	// Token: 0x04001F38 RID: 7992
	[SerializeField]
	private Transform rightSkiPistonOut;

	// Token: 0x04001F39 RID: 7993
	[SerializeField]
	private float skiVisualAdjust;

	// Token: 0x04001F3A RID: 7994
	[SerializeField]
	private float treadVisualAdjust;

	// Token: 0x04001F3B RID: 7995
	[SerializeField]
	private float skiVisualMaxExtension;

	// Token: 0x04001F3C RID: 7996
	[SerializeField]
	private float treadVisualMaxExtension;

	// Token: 0x04001F3D RID: 7997
	[SerializeField]
	private float wheelSizeVisualMultiplier = 1f;

	// Token: 0x02000D09 RID: 3337
	[Serializable]
	private class TreadRenderer
	{
		// Token: 0x040045F6 RID: 17910
		public Renderer renderer;

		// Token: 0x040045F7 RID: 17911
		public int materialIndex;
	}
}
