using System;
using UnityEngine;

// Token: 0x020004A8 RID: 1192
public class SubmarineAudio : MonoBehaviour
{
	// Token: 0x04001F44 RID: 8004
	[Header("Engine")]
	[SerializeField]
	private SoundDefinition engineStartSound;

	// Token: 0x04001F45 RID: 8005
	[SerializeField]
	private SoundDefinition engineStopSound;

	// Token: 0x04001F46 RID: 8006
	[SerializeField]
	private SoundDefinition engineStartFailSound;

	// Token: 0x04001F47 RID: 8007
	[SerializeField]
	private SoundDefinition engineLoopSound;

	// Token: 0x04001F48 RID: 8008
	[SerializeField]
	private AnimationCurve engineLoopPitchCurve;

	// Token: 0x04001F49 RID: 8009
	[Header("Water")]
	[SerializeField]
	private SoundDefinition underwaterLoopDef;

	// Token: 0x04001F4A RID: 8010
	[SerializeField]
	private SoundDefinition underwaterMovementLoopDef;

	// Token: 0x04001F4B RID: 8011
	[SerializeField]
	private BlendedSoundLoops surfaceWaterLoops;

	// Token: 0x04001F4C RID: 8012
	[SerializeField]
	private float surfaceWaterSoundsMaxSpeed = 5f;

	// Token: 0x04001F4D RID: 8013
	[SerializeField]
	private SoundDefinition waterEmergeSoundDef;

	// Token: 0x04001F4E RID: 8014
	[SerializeField]
	private SoundDefinition waterSubmergeSoundDef;

	// Token: 0x04001F4F RID: 8015
	[Header("Interior")]
	[SerializeField]
	private SoundDefinition activeLoopDef;

	// Token: 0x04001F50 RID: 8016
	[SerializeField]
	private SoundDefinition footPedalSoundDef;

	// Token: 0x04001F51 RID: 8017
	[SerializeField]
	private Transform footPedalSoundPos;

	// Token: 0x04001F52 RID: 8018
	[SerializeField]
	private SoundDefinition steeringWheelSoundDef;

	// Token: 0x04001F53 RID: 8019
	[SerializeField]
	private Transform steeringWheelSoundPos;

	// Token: 0x04001F54 RID: 8020
	[SerializeField]
	private SoundDefinition heavyDamageSparksDef;

	// Token: 0x04001F55 RID: 8021
	[SerializeField]
	private Transform heavyDamageSparksPos;

	// Token: 0x04001F56 RID: 8022
	[SerializeField]
	private SoundDefinition flagRaise;

	// Token: 0x04001F57 RID: 8023
	[SerializeField]
	private SoundDefinition flagLower;

	// Token: 0x04001F58 RID: 8024
	[Header("Other")]
	[SerializeField]
	private SoundDefinition climbOrDiveLoopSound;

	// Token: 0x04001F59 RID: 8025
	[SerializeField]
	private SoundDefinition sonarBlipSound;

	// Token: 0x04001F5A RID: 8026
	[SerializeField]
	private SoundDefinition torpedoFailedSound;
}
