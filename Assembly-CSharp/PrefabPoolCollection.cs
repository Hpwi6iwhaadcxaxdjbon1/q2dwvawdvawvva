using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

// Token: 0x02000960 RID: 2400
public class PrefabPoolCollection
{
	// Token: 0x040033C2 RID: 13250
	public Dictionary<uint, PrefabPool> storage = new Dictionary<uint, PrefabPool>();

	// Token: 0x060039AA RID: 14762 RVA: 0x0015692C File Offset: 0x00154B2C
	public void Push(GameObject instance)
	{
		Poolable component = instance.GetComponent<Poolable>();
		PrefabPool prefabPool;
		if (!this.storage.TryGetValue(component.prefabID, out prefabPool))
		{
			prefabPool = new PrefabPool();
			this.storage.Add(component.prefabID, prefabPool);
		}
		prefabPool.Push(component);
	}

	// Token: 0x060039AB RID: 14763 RVA: 0x00156974 File Offset: 0x00154B74
	public GameObject Pop(uint id, Vector3 pos = default(Vector3), Quaternion rot = default(Quaternion))
	{
		PrefabPool prefabPool;
		if (this.storage.TryGetValue(id, out prefabPool))
		{
			return prefabPool.Pop(pos, rot);
		}
		return null;
	}

	// Token: 0x060039AC RID: 14764 RVA: 0x0015699C File Offset: 0x00154B9C
	public void Clear(string filter = null)
	{
		if (string.IsNullOrEmpty(filter))
		{
			using (Dictionary<uint, PrefabPool>.Enumerator enumerator = this.storage.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					KeyValuePair<uint, PrefabPool> keyValuePair = enumerator.Current;
					keyValuePair.Value.Clear();
				}
				return;
			}
		}
		foreach (KeyValuePair<uint, PrefabPool> keyValuePair2 in this.storage)
		{
			if (StringPool.Get(keyValuePair2.Key).Contains(filter, CompareOptions.IgnoreCase))
			{
				keyValuePair2.Value.Clear();
			}
		}
	}
}
