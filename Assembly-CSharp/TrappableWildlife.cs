using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000467 RID: 1127
[CreateAssetMenu(menuName = "Rust/TrappableWildlife")]
[Serializable]
public class TrappableWildlife : ScriptableObject
{
	// Token: 0x04001D7A RID: 7546
	public GameObjectRef worldObject;

	// Token: 0x04001D7B RID: 7547
	public ItemDefinition inventoryObject;

	// Token: 0x04001D7C RID: 7548
	public int minToCatch;

	// Token: 0x04001D7D RID: 7549
	public int maxToCatch;

	// Token: 0x04001D7E RID: 7550
	public List<TrappableWildlife.BaitType> baitTypes;

	// Token: 0x04001D7F RID: 7551
	public int caloriesForInterest = 20;

	// Token: 0x04001D80 RID: 7552
	public float successRate = 1f;

	// Token: 0x04001D81 RID: 7553
	public float xpScale = 1f;

	// Token: 0x02000CF1 RID: 3313
	[Serializable]
	public class BaitType
	{
		// Token: 0x040045A6 RID: 17830
		public float successRate = 1f;

		// Token: 0x040045A7 RID: 17831
		public ItemDefinition bait;

		// Token: 0x040045A8 RID: 17832
		public int minForInterest = 1;

		// Token: 0x040045A9 RID: 17833
		public int maxToConsume = 1;
	}
}
