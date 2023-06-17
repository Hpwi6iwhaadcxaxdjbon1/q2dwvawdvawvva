using System;
using UnityEngine;

// Token: 0x020004FA RID: 1274
[CreateAssetMenu(fileName = "NewEntityList", menuName = "Rust/EntityList")]
public class EntityListScriptableObject : ScriptableObject
{
	// Token: 0x0400210E RID: 8462
	[SerializeField]
	public BaseEntity[] entities;

	// Token: 0x0400210F RID: 8463
	[SerializeField]
	public bool whitelist;

	// Token: 0x06002915 RID: 10517 RVA: 0x000FCA9C File Offset: 0x000FAC9C
	public bool IsInList(uint prefabId)
	{
		if (this.entities == null)
		{
			return false;
		}
		BaseEntity[] array = this.entities;
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i].prefabID == prefabId)
			{
				return true;
			}
		}
		return false;
	}
}
