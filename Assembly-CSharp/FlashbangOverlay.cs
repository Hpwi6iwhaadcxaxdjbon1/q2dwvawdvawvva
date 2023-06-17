using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

// Token: 0x02000298 RID: 664
public class FlashbangOverlay : MonoBehaviour, IClientComponent
{
	// Token: 0x040015F2 RID: 5618
	public static FlashbangOverlay Instance;

	// Token: 0x040015F3 RID: 5619
	public PostProcessVolume postProcessVolume;

	// Token: 0x040015F4 RID: 5620
	public AnimationCurve burnIntensityCurve;

	// Token: 0x040015F5 RID: 5621
	public AnimationCurve whiteoutIntensityCurve;

	// Token: 0x040015F6 RID: 5622
	public SoundDefinition deafLoopDef;
}
