using System;
using UnityEngine;

// Token: 0x020004A5 RID: 1189
public class SnowmobileAudio : GroundVehicleAudio
{
	// Token: 0x04001F22 RID: 7970
	[Header("Engine")]
	[SerializeField]
	private EngineAudioSet engineAudioSet;

	// Token: 0x04001F23 RID: 7971
	[Header("Skis")]
	[SerializeField]
	private AnimationCurve skiGainCurve;

	// Token: 0x04001F24 RID: 7972
	[SerializeField]
	private SoundDefinition skiSlideSoundDef;

	// Token: 0x04001F25 RID: 7973
	[SerializeField]
	private SoundDefinition skiSlideSnowSoundDef;

	// Token: 0x04001F26 RID: 7974
	[SerializeField]
	private SoundDefinition skiSlideSandSoundDef;

	// Token: 0x04001F27 RID: 7975
	[SerializeField]
	private SoundDefinition skiSlideGrassSoundDef;

	// Token: 0x04001F28 RID: 7976
	[SerializeField]
	private SoundDefinition skiSlideWaterSoundDef;

	// Token: 0x04001F29 RID: 7977
	[Header("Movement")]
	[SerializeField]
	private AnimationCurve movementGainCurve;

	// Token: 0x04001F2A RID: 7978
	[SerializeField]
	private SoundDefinition movementLoopDef;

	// Token: 0x04001F2B RID: 7979
	[SerializeField]
	private SoundDefinition suspensionLurchSoundDef;

	// Token: 0x04001F2C RID: 7980
	[SerializeField]
	private float suspensionLurchMinExtensionDelta = 0.4f;

	// Token: 0x04001F2D RID: 7981
	[SerializeField]
	private float suspensionLurchMinTimeBetweenSounds = 0.25f;
}
