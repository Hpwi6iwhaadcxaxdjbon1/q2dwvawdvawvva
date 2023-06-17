using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000815 RID: 2069
public class CraftingQueue : SingletonComponent<CraftingQueue>
{
	// Token: 0x04002E64 RID: 11876
	public GameObject queueContainer;

	// Token: 0x04002E65 RID: 11877
	public GameObject queueItemPrefab;

	// Token: 0x04002E66 RID: 11878
	private ScrollRect scrollRect;
}
