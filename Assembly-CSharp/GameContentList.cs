using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000324 RID: 804
public class GameContentList : MonoBehaviour
{
	// Token: 0x040017F1 RID: 6129
	public GameContentList.ResourceType resourceType;

	// Token: 0x040017F2 RID: 6130
	public List<UnityEngine.Object> foundObjects;

	// Token: 0x02000CA7 RID: 3239
	public enum ResourceType
	{
		// Token: 0x04004451 RID: 17489
		Audio,
		// Token: 0x04004452 RID: 17490
		Textures,
		// Token: 0x04004453 RID: 17491
		Models
	}
}
