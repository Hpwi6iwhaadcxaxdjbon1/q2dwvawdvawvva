using System;
using System.Collections.Generic;

// Token: 0x02000222 RID: 546
public class AmbienceManager : SingletonComponent<AmbienceManager>, IClientComponent
{
	// Token: 0x040013BE RID: 5054
	public List<AmbienceManager.EmitterTypeLimit> localEmitterLimits = new List<AmbienceManager.EmitterTypeLimit>();

	// Token: 0x040013BF RID: 5055
	public AmbienceManager.EmitterTypeLimit catchallEmitterLimit = new AmbienceManager.EmitterTypeLimit();

	// Token: 0x040013C0 RID: 5056
	public int maxActiveLocalEmitters = 5;

	// Token: 0x040013C1 RID: 5057
	public int activeLocalEmitters;

	// Token: 0x040013C2 RID: 5058
	public List<AmbienceEmitter> cameraEmitters = new List<AmbienceEmitter>();

	// Token: 0x040013C3 RID: 5059
	public List<AmbienceEmitter> emittersInRange = new List<AmbienceEmitter>();

	// Token: 0x040013C4 RID: 5060
	public List<AmbienceEmitter> activeEmitters = new List<AmbienceEmitter>();

	// Token: 0x040013C5 RID: 5061
	public float localEmitterRange = 30f;

	// Token: 0x040013C6 RID: 5062
	public List<AmbienceZone> currentAmbienceZones = new List<AmbienceZone>();

	// Token: 0x040013C7 RID: 5063
	public bool isUnderwater;

	// Token: 0x02000C6B RID: 3179
	[Serializable]
	public class EmitterTypeLimit
	{
		// Token: 0x04004318 RID: 17176
		public List<AmbienceDefinitionList> ambience;

		// Token: 0x04004319 RID: 17177
		public int limit = 1;

		// Token: 0x0400431A RID: 17178
		public int active;
	}
}
