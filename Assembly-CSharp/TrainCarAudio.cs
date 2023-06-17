using System;
using UnityEngine;

// Token: 0x020004B0 RID: 1200
public class TrainCarAudio : MonoBehaviour
{
	// Token: 0x04001FA2 RID: 8098
	[Header("Train Car Audio")]
	[SerializeField]
	private TrainCar trainCar;

	// Token: 0x04001FA3 RID: 8099
	[SerializeField]
	private SoundDefinition movementStartDef;

	// Token: 0x04001FA4 RID: 8100
	[SerializeField]
	private SoundDefinition movementStopDef;

	// Token: 0x04001FA5 RID: 8101
	[SerializeField]
	private SoundDefinition movementLoopDef;

	// Token: 0x04001FA6 RID: 8102
	[SerializeField]
	private AnimationCurve movementLoopGainCurve;

	// Token: 0x04001FA7 RID: 8103
	[SerializeField]
	private float movementChangeOneshotDebounce = 1f;

	// Token: 0x04001FA8 RID: 8104
	private Sound movementLoop;

	// Token: 0x04001FA9 RID: 8105
	private SoundModulation.Modulator movementLoopGain;

	// Token: 0x04001FAA RID: 8106
	[SerializeField]
	private SoundDefinition turnLoopDef;

	// Token: 0x04001FAB RID: 8107
	private Sound turnLoop;

	// Token: 0x04001FAC RID: 8108
	[SerializeField]
	private SoundDefinition trackClatterLoopDef;

	// Token: 0x04001FAD RID: 8109
	[SerializeField]
	private AnimationCurve trackClatterGainCurve;

	// Token: 0x04001FAE RID: 8110
	[SerializeField]
	private AnimationCurve trackClatterPitchCurve;

	// Token: 0x04001FAF RID: 8111
	private Sound trackClatterLoop;

	// Token: 0x04001FB0 RID: 8112
	private SoundModulation.Modulator trackClatterGain;

	// Token: 0x04001FB1 RID: 8113
	private SoundModulation.Modulator trackClatterPitch;
}
