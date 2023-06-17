using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000225 RID: 549
public class RainSurfaceAmbience : SingletonComponent<RainSurfaceAmbience>, IClientComponent
{
	// Token: 0x040013D0 RID: 5072
	public List<RainSurfaceAmbience.SurfaceSound> surfaces = new List<RainSurfaceAmbience.SurfaceSound>();

	// Token: 0x040013D1 RID: 5073
	public GameObjectRef emitterPrefab;

	// Token: 0x040013D2 RID: 5074
	public Dictionary<ParticlePatch, AmbienceEmitter> spawnedEmitters = new Dictionary<ParticlePatch, AmbienceEmitter>();

	// Token: 0x02000C6C RID: 3180
	[Serializable]
	public class SurfaceSound
	{
		// Token: 0x0400431B RID: 17179
		public AmbienceDefinitionList baseAmbience;

		// Token: 0x0400431C RID: 17180
		public List<PhysicMaterial> materials = new List<PhysicMaterial>();
	}
}
