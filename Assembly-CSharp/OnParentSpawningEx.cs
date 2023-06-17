using System;
using System.Collections.Generic;
using Facepunch;
using UnityEngine;

// Token: 0x02000523 RID: 1315
public static class OnParentSpawningEx
{
	// Token: 0x060029B4 RID: 10676 RVA: 0x000FF818 File Offset: 0x000FDA18
	public static void BroadcastOnParentSpawning(this GameObject go)
	{
		List<IOnParentSpawning> list = Pool.GetList<IOnParentSpawning>();
		go.GetComponentsInChildren<IOnParentSpawning>(list);
		for (int i = 0; i < list.Count; i++)
		{
			list[i].OnParentSpawning();
		}
		Pool.FreeList<IOnParentSpawning>(ref list);
	}

	// Token: 0x060029B5 RID: 10677 RVA: 0x000FF858 File Offset: 0x000FDA58
	public static void SendOnParentSpawning(this GameObject go)
	{
		List<IOnParentSpawning> list = Pool.GetList<IOnParentSpawning>();
		go.GetComponents<IOnParentSpawning>(list);
		for (int i = 0; i < list.Count; i++)
		{
			list[i].OnParentSpawning();
		}
		Pool.FreeList<IOnParentSpawning>(ref list);
	}
}
