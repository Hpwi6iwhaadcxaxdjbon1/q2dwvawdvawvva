using System;
using System.Collections.Generic;

// Token: 0x02000203 RID: 515
public class EnvironmentFishManager : BaseMonoBehaviour, IClientComponent
{
	// Token: 0x04001323 RID: 4899
	public EnvironmentFishManager.FishTypeInstance[] fishTypes;

	// Token: 0x02000C62 RID: 3170
	[Serializable]
	public class FishTypeInstance
	{
		// Token: 0x040042E3 RID: 17123
		public GameObjectRef prefab;

		// Token: 0x040042E4 RID: 17124
		public bool shouldSchool;

		// Token: 0x040042E5 RID: 17125
		public float populationScale;

		// Token: 0x040042E6 RID: 17126
		public bool freshwater;

		// Token: 0x040042E7 RID: 17127
		public bool seawater = true;

		// Token: 0x040042E8 RID: 17128
		public float minDepth = 3f;

		// Token: 0x040042E9 RID: 17129
		public float maxDepth = 100f;

		// Token: 0x040042EA RID: 17130
		public List<EnvironmentFish> activeFish;

		// Token: 0x040042EB RID: 17131
		public List<EnvironmentFish> sleeping;
	}
}
