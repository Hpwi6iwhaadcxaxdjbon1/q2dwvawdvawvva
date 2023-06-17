using System;
using System.Collections.Generic;
using Facepunch;
using UnityEngine;

// Token: 0x02000521 RID: 1313
public static class OnParentDestroyingEx
{
	// Token: 0x060029B1 RID: 10673 RVA: 0x000FF798 File Offset: 0x000FD998
	public static void BroadcastOnParentDestroying(this GameObject go)
	{
		List<IOnParentDestroying> list = Pool.GetList<IOnParentDestroying>();
		go.GetComponentsInChildren<IOnParentDestroying>(list);
		for (int i = 0; i < list.Count; i++)
		{
			list[i].OnParentDestroying();
		}
		Pool.FreeList<IOnParentDestroying>(ref list);
	}

	// Token: 0x060029B2 RID: 10674 RVA: 0x000FF7D8 File Offset: 0x000FD9D8
	public static void SendOnParentDestroying(this GameObject go)
	{
		List<IOnParentDestroying> list = Pool.GetList<IOnParentDestroying>();
		go.GetComponents<IOnParentDestroying>(list);
		for (int i = 0; i < list.Count; i++)
		{
			list[i].OnParentDestroying();
		}
		Pool.FreeList<IOnParentDestroying>(ref list);
	}
}
