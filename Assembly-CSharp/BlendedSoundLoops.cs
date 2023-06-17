using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000228 RID: 552
public class BlendedSoundLoops : MonoBehaviour, IClientComponent
{
	// Token: 0x040013DD RID: 5085
	[Range(0f, 1f)]
	public float blend;

	// Token: 0x040013DE RID: 5086
	public float blendSmoothing = 1f;

	// Token: 0x040013DF RID: 5087
	public float loopFadeOutTime = 0.5f;

	// Token: 0x040013E0 RID: 5088
	public float loopFadeInTime = 0.5f;

	// Token: 0x040013E1 RID: 5089
	public float gainModSmoothing = 1f;

	// Token: 0x040013E2 RID: 5090
	public float pitchModSmoothing = 1f;

	// Token: 0x040013E3 RID: 5091
	public bool shouldPlay = true;

	// Token: 0x040013E4 RID: 5092
	public float gain = 1f;

	// Token: 0x040013E5 RID: 5093
	public List<BlendedSoundLoops.Loop> loops = new List<BlendedSoundLoops.Loop>();

	// Token: 0x040013E6 RID: 5094
	public float maxDistance;

	// Token: 0x02000C6F RID: 3183
	[Serializable]
	public class Loop
	{
		// Token: 0x04004328 RID: 17192
		public SoundDefinition soundDef;

		// Token: 0x04004329 RID: 17193
		public AnimationCurve gainCurve;

		// Token: 0x0400432A RID: 17194
		public AnimationCurve pitchCurve;

		// Token: 0x0400432B RID: 17195
		[HideInInspector]
		public Sound sound;

		// Token: 0x0400432C RID: 17196
		[HideInInspector]
		public SoundModulation.Modulator gainMod;

		// Token: 0x0400432D RID: 17197
		[HideInInspector]
		public SoundModulation.Modulator pitchMod;
	}
}
