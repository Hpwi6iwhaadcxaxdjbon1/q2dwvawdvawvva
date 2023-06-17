using System;
using UnityEngine;

// Token: 0x020004B5 RID: 1205
public class TrainEngineAudio : TrainCarAudio
{
	// Token: 0x04001FD6 RID: 8150
	[SerializeField]
	private TrainEngine trainEngine;

	// Token: 0x04001FD7 RID: 8151
	[SerializeField]
	private Transform cockpitSoundPosition;

	// Token: 0x04001FD8 RID: 8152
	[SerializeField]
	private Transform hornSoundPosition;

	// Token: 0x04001FD9 RID: 8153
	[Header("Engine")]
	[SerializeField]
	private SoundDefinition engineStartSound;

	// Token: 0x04001FDA RID: 8154
	[SerializeField]
	private SoundDefinition engineStopSound;

	// Token: 0x04001FDB RID: 8155
	[SerializeField]
	private SoundDefinition engineActiveLoopDef;

	// Token: 0x04001FDC RID: 8156
	[SerializeField]
	private AnimationCurve engineActiveLoopPitchCurve;

	// Token: 0x04001FDD RID: 8157
	[SerializeField]
	private float engineActiveLoopChangeSpeed = 0.2f;

	// Token: 0x04001FDE RID: 8158
	private Sound engineActiveLoop;

	// Token: 0x04001FDF RID: 8159
	private SoundModulation.Modulator engineActiveLoopPitch;

	// Token: 0x04001FE0 RID: 8160
	[SerializeField]
	private BlendedLoopEngineSound engineLoops;

	// Token: 0x04001FE1 RID: 8161
	[SerializeField]
	private TrainEngineAudio.EngineReflection[] engineReflections;

	// Token: 0x04001FE2 RID: 8162
	[SerializeField]
	private LayerMask reflectionLayerMask;

	// Token: 0x04001FE3 RID: 8163
	[SerializeField]
	private float reflectionMaxDistance = 20f;

	// Token: 0x04001FE4 RID: 8164
	[SerializeField]
	private float reflectionGainChangeSpeed = 10f;

	// Token: 0x04001FE5 RID: 8165
	[SerializeField]
	private float reflectionPositionChangeSpeed = 10f;

	// Token: 0x04001FE6 RID: 8166
	[SerializeField]
	private float reflectionRayOffset = 0.5f;

	// Token: 0x04001FE7 RID: 8167
	[Header("Horn")]
	[SerializeField]
	private SoundDefinition hornLoop;

	// Token: 0x04001FE8 RID: 8168
	[SerializeField]
	private SoundDefinition hornStart;

	// Token: 0x04001FE9 RID: 8169
	[Header("Other")]
	[SerializeField]
	private SoundDefinition lightsToggleSound;

	// Token: 0x04001FEA RID: 8170
	[SerializeField]
	private SoundDefinition proximityAlertDef;

	// Token: 0x04001FEB RID: 8171
	private Sound proximityAlertSound;

	// Token: 0x04001FEC RID: 8172
	[SerializeField]
	private SoundDefinition damagedLoopDef;

	// Token: 0x04001FED RID: 8173
	private Sound damagedLoop;

	// Token: 0x04001FEE RID: 8174
	[SerializeField]
	private SoundDefinition changeThrottleDef;

	// Token: 0x04001FEF RID: 8175
	[SerializeField]
	private SoundDefinition changeCouplingDef;

	// Token: 0x04001FF0 RID: 8176
	[SerializeField]
	private SoundDefinition unloadableStartDef;

	// Token: 0x04001FF1 RID: 8177
	[SerializeField]
	private SoundDefinition unloadableEndDef;

	// Token: 0x04001FF2 RID: 8178
	[SerializeField]
	private GameObject bellObject;

	// Token: 0x04001FF3 RID: 8179
	[SerializeField]
	private SoundDefinition bellRingDef;

	// Token: 0x04001FF4 RID: 8180
	[SerializeField]
	private SoundPlayer brakeSound;

	// Token: 0x02000D0E RID: 3342
	[Serializable]
	public class EngineReflection
	{
		// Token: 0x04004603 RID: 17923
		public Vector3 direction;

		// Token: 0x04004604 RID: 17924
		public Vector3 offset;

		// Token: 0x04004605 RID: 17925
		public SoundDefinition soundDef;

		// Token: 0x04004606 RID: 17926
		public Sound sound;

		// Token: 0x04004607 RID: 17927
		public SoundModulation.Modulator pitchMod;

		// Token: 0x04004608 RID: 17928
		public SoundModulation.Modulator gainMod;

		// Token: 0x04004609 RID: 17929
		public float distance = 20f;
	}
}
