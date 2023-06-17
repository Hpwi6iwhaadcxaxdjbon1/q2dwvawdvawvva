using System;
using UnityEngine;

// Token: 0x020004A3 RID: 1187
public class VehicleLight : MonoBehaviour, IClientComponent
{
	// Token: 0x04001F1B RID: 7963
	public bool IsBrake;

	// Token: 0x04001F1C RID: 7964
	public GameObject toggleObject;

	// Token: 0x04001F1D RID: 7965
	public VehicleLight.LightRenderer[] renderers;

	// Token: 0x04001F1E RID: 7966
	[ColorUsage(true, true)]
	public Color lightOnColour;

	// Token: 0x04001F1F RID: 7967
	[ColorUsage(true, true)]
	public Color brakesOnColour;

	// Token: 0x02000D08 RID: 3336
	[Serializable]
	public class LightRenderer
	{
		// Token: 0x040045F4 RID: 17908
		public Renderer renderer;

		// Token: 0x040045F5 RID: 17909
		public int matIndex;
	}
}
