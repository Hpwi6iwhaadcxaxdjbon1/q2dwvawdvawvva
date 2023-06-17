using System;
using UnityEngine;

// Token: 0x0200048F RID: 1167
public class MagnetCraneAudio : MonoBehaviour
{
	// Token: 0x04001EA0 RID: 7840
	public MagnetCrane crane;

	// Token: 0x04001EA1 RID: 7841
	[Header("Sound defs")]
	public SoundDefinition engineStartSoundDef;

	// Token: 0x04001EA2 RID: 7842
	public SoundDefinition engineStopSoundDef;

	// Token: 0x04001EA3 RID: 7843
	public BlendedLoopEngineSound engineLoops;

	// Token: 0x04001EA4 RID: 7844
	public SoundDefinition cabinRotationStartDef;

	// Token: 0x04001EA5 RID: 7845
	public SoundDefinition cabinRotationStopDef;

	// Token: 0x04001EA6 RID: 7846
	public SoundDefinition cabinRotationLoopDef;

	// Token: 0x04001EA7 RID: 7847
	private Sound cabinRotationLoop;

	// Token: 0x04001EA8 RID: 7848
	public SoundDefinition turningLoopDef;

	// Token: 0x04001EA9 RID: 7849
	private Sound turningLoop;

	// Token: 0x04001EAA RID: 7850
	public SoundDefinition trackMovementLoopDef;

	// Token: 0x04001EAB RID: 7851
	private Sound trackMovementLoop;

	// Token: 0x04001EAC RID: 7852
	private SoundModulation.Modulator trackGainMod;

	// Token: 0x04001EAD RID: 7853
	private SoundModulation.Modulator trackPitchMod;

	// Token: 0x04001EAE RID: 7854
	public SoundDefinition armMovementLoopDef;

	// Token: 0x04001EAF RID: 7855
	public SoundDefinition armMovementStartDef;

	// Token: 0x04001EB0 RID: 7856
	public SoundDefinition armMovementStopDef;

	// Token: 0x04001EB1 RID: 7857
	private Sound armMovementLoop01;

	// Token: 0x04001EB2 RID: 7858
	private SoundModulation.Modulator armMovementLoop01PitchMod;

	// Token: 0x04001EB3 RID: 7859
	public GameObject arm01SoundPosition;

	// Token: 0x04001EB4 RID: 7860
	public GameObject arm02SoundPosition;

	// Token: 0x04001EB5 RID: 7861
	private Sound armMovementLoop02;

	// Token: 0x04001EB6 RID: 7862
	private SoundModulation.Modulator armMovementLoop02PitchMod;
}
