using System;
using UnityEngine;

// Token: 0x0200022B RID: 555
public class FootstepSound : MonoBehaviour, IClientComponent
{
	// Token: 0x040013FF RID: 5119
	public SoundDefinition lightSound;

	// Token: 0x04001400 RID: 5120
	public SoundDefinition medSound;

	// Token: 0x04001401 RID: 5121
	public SoundDefinition hardSound;

	// Token: 0x04001402 RID: 5122
	private const float panAmount = 0.05f;

	// Token: 0x02000C73 RID: 3187
	public enum Hardness
	{
		// Token: 0x04004343 RID: 17219
		Light = 1,
		// Token: 0x04004344 RID: 17220
		Medium,
		// Token: 0x04004345 RID: 17221
		Hard
	}
}
