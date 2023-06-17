using System;
using Facepunch;
using UnityEngine;

// Token: 0x02000964 RID: 2404
[Serializable]
public class GameObjectRef : ResourceRef<GameObject>
{
	// Token: 0x060039BC RID: 14780 RVA: 0x00156C6E File Offset: 0x00154E6E
	public GameObject Instantiate(Transform parent = null)
	{
		return Facepunch.Instantiate.GameObject(base.Get(), parent);
	}

	// Token: 0x060039BD RID: 14781 RVA: 0x00156C7C File Offset: 0x00154E7C
	public BaseEntity GetEntity()
	{
		return base.Get().GetComponent<BaseEntity>();
	}
}
