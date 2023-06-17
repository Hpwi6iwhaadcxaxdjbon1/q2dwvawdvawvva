using System;
using UnityEngine;

// Token: 0x0200061D RID: 1565
public class MovementSounds : MonoBehaviour
{
	// Token: 0x040025B7 RID: 9655
	public SoundDefinition waterMovementDef;

	// Token: 0x040025B8 RID: 9656
	public float waterMovementFadeInSpeed = 1f;

	// Token: 0x040025B9 RID: 9657
	public float waterMovementFadeOutSpeed = 1f;

	// Token: 0x040025BA RID: 9658
	public SoundDefinition enterWaterSmall;

	// Token: 0x040025BB RID: 9659
	public SoundDefinition enterWaterMedium;

	// Token: 0x040025BC RID: 9660
	public SoundDefinition enterWaterLarge;

	// Token: 0x040025BD RID: 9661
	private Sound waterMovement;

	// Token: 0x040025BE RID: 9662
	private SoundModulation.Modulator waterGainMod;

	// Token: 0x040025BF RID: 9663
	public bool inWater;

	// Token: 0x040025C0 RID: 9664
	public float waterLevel;

	// Token: 0x040025C1 RID: 9665
	public bool mute;
}
