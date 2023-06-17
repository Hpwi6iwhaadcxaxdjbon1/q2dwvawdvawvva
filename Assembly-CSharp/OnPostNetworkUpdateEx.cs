using System;
using System.Collections.Generic;
using Facepunch;
using UnityEngine;

// Token: 0x02000525 RID: 1317
public static class OnPostNetworkUpdateEx
{
	// Token: 0x060029B7 RID: 10679 RVA: 0x000FF898 File Offset: 0x000FDA98
	public static void BroadcastOnPostNetworkUpdate(this GameObject go, BaseEntity entity)
	{
		List<IOnPostNetworkUpdate> list = Pool.GetList<IOnPostNetworkUpdate>();
		go.GetComponentsInChildren<IOnPostNetworkUpdate>(list);
		for (int i = 0; i < list.Count; i++)
		{
			list[i].OnPostNetworkUpdate(entity);
		}
		Pool.FreeList<IOnPostNetworkUpdate>(ref list);
	}

	// Token: 0x060029B8 RID: 10680 RVA: 0x000FF8D8 File Offset: 0x000FDAD8
	public static void SendOnPostNetworkUpdate(this GameObject go, BaseEntity entity)
	{
		List<IOnPostNetworkUpdate> list = Pool.GetList<IOnPostNetworkUpdate>();
		go.GetComponents<IOnPostNetworkUpdate>(list);
		for (int i = 0; i < list.Count; i++)
		{
			list[i].OnPostNetworkUpdate(entity);
		}
		Pool.FreeList<IOnPostNetworkUpdate>(ref list);
	}
}
