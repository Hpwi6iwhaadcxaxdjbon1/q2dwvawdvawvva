using System;
using UnityEngine;

// Token: 0x0200047A RID: 1146
public abstract class GroundVehicleAudio : MonoBehaviour, IClientComponent
{
	// Token: 0x04001E15 RID: 7701
	[SerializeField]
	protected GroundVehicle groundVehicle;

	// Token: 0x04001E16 RID: 7702
	[Header("Engine")]
	[SerializeField]
	private SoundDefinition engineStartSound;

	// Token: 0x04001E17 RID: 7703
	[SerializeField]
	private SoundDefinition engineStopSound;

	// Token: 0x04001E18 RID: 7704
	[SerializeField]
	private SoundDefinition engineStartFailSound;

	// Token: 0x04001E19 RID: 7705
	[SerializeField]
	private BlendedLoopEngineSound blendedEngineLoops;

	// Token: 0x04001E1A RID: 7706
	[SerializeField]
	private float wheelRatioMultiplier = 600f;

	// Token: 0x04001E1B RID: 7707
	[Header("Water")]
	[SerializeField]
	private SoundDefinition waterSplashSoundDef;

	// Token: 0x04001E1C RID: 7708
	[SerializeField]
	private BlendedSoundLoops waterLoops;

	// Token: 0x04001E1D RID: 7709
	[SerializeField]
	private float waterSoundsMaxSpeed = 10f;

	// Token: 0x04001E1E RID: 7710
	[Header("Brakes")]
	[SerializeField]
	private SoundDefinition brakeSoundDef;

	// Token: 0x04001E1F RID: 7711
	[Header("Lights")]
	[SerializeField]
	private SoundDefinition lightsToggleSound;
}
