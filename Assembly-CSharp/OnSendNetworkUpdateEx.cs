using System;
using System.Collections.Generic;
using Facepunch;
using UnityEngine;

// Token: 0x02000527 RID: 1319
public static class OnSendNetworkUpdateEx
{
	// Token: 0x060029BA RID: 10682 RVA: 0x000FF918 File Offset: 0x000FDB18
	public static void BroadcastOnSendNetworkUpdate(this GameObject go, BaseEntity entity)
	{
		List<IOnSendNetworkUpdate> list = Pool.GetList<IOnSendNetworkUpdate>();
		go.GetComponentsInChildren<IOnSendNetworkUpdate>(list);
		for (int i = 0; i < list.Count; i++)
		{
			list[i].OnSendNetworkUpdate(entity);
		}
		Pool.FreeList<IOnSendNetworkUpdate>(ref list);
	}

	// Token: 0x060029BB RID: 10683 RVA: 0x000FF958 File Offset: 0x000FDB58
	public static void SendOnSendNetworkUpdate(this GameObject go, BaseEntity entity)
	{
		List<IOnSendNetworkUpdate> list = Pool.GetList<IOnSendNetworkUpdate>();
		go.GetComponents<IOnSendNetworkUpdate>(list);
		for (int i = 0; i < list.Count; i++)
		{
			list[i].OnSendNetworkUpdate(entity);
		}
		Pool.FreeList<IOnSendNetworkUpdate>(ref list);
	}
}
