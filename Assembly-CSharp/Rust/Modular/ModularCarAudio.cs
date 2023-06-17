using System;
using UnityEngine;

namespace Rust.Modular
{
	// Token: 0x02000B28 RID: 2856
	public class ModularCarAudio : GroundVehicleAudio
	{
		// Token: 0x04003DC4 RID: 15812
		public bool showDebug;

		// Token: 0x04003DC5 RID: 15813
		[Header("Skid")]
		[SerializeField]
		private SoundDefinition skidSoundLoop;

		// Token: 0x04003DC6 RID: 15814
		[SerializeField]
		private SoundDefinition skidSoundDirtLoop;

		// Token: 0x04003DC7 RID: 15815
		[SerializeField]
		private SoundDefinition skidSoundSnowLoop;

		// Token: 0x04003DC8 RID: 15816
		[SerializeField]
		private float skidMinSlip = 10f;

		// Token: 0x04003DC9 RID: 15817
		[SerializeField]
		private float skidMaxSlip = 25f;

		// Token: 0x04003DCA RID: 15818
		[Header("Movement & Suspension")]
		[SerializeField]
		private SoundDefinition movementStartOneshot;

		// Token: 0x04003DCB RID: 15819
		[SerializeField]
		private SoundDefinition movementStopOneshot;

		// Token: 0x04003DCC RID: 15820
		[SerializeField]
		private float movementStartStopMinTimeBetweenSounds = 0.25f;

		// Token: 0x04003DCD RID: 15821
		[SerializeField]
		private SoundDefinition movementRattleLoop;

		// Token: 0x04003DCE RID: 15822
		[SerializeField]
		private float movementRattleMaxSpeed = 10f;

		// Token: 0x04003DCF RID: 15823
		[SerializeField]
		private float movementRattleMaxAngSpeed = 10f;

		// Token: 0x04003DD0 RID: 15824
		[SerializeField]
		private float movementRattleIdleGain = 0.3f;

		// Token: 0x04003DD1 RID: 15825
		[SerializeField]
		private SoundDefinition suspensionLurchSound;

		// Token: 0x04003DD2 RID: 15826
		[SerializeField]
		private float suspensionLurchMinExtensionDelta = 0.4f;

		// Token: 0x04003DD3 RID: 15827
		[SerializeField]
		private float suspensionLurchMinTimeBetweenSounds = 0.25f;

		// Token: 0x04003DD4 RID: 15828
		[Header("Wheels")]
		[SerializeField]
		private SoundDefinition tyreRollingSoundDef;

		// Token: 0x04003DD5 RID: 15829
		[SerializeField]
		private SoundDefinition tyreRollingWaterSoundDef;

		// Token: 0x04003DD6 RID: 15830
		[SerializeField]
		private SoundDefinition tyreRollingGrassSoundDef;

		// Token: 0x04003DD7 RID: 15831
		[SerializeField]
		private SoundDefinition tyreRollingSnowSoundDef;

		// Token: 0x04003DD8 RID: 15832
		[SerializeField]
		private AnimationCurve tyreRollGainCurve;
	}
}
