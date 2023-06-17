using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200095F RID: 2399
public class PrefabPool
{
	// Token: 0x040033C1 RID: 13249
	public Stack<Poolable> stack = new Stack<Poolable>();

	// Token: 0x17000496 RID: 1174
	// (get) Token: 0x060039A4 RID: 14756 RVA: 0x00156816 File Offset: 0x00154A16
	public int Count
	{
		get
		{
			return this.stack.Count;
		}
	}

	// Token: 0x060039A5 RID: 14757 RVA: 0x00156823 File Offset: 0x00154A23
	public void Push(Poolable info)
	{
		this.stack.Push(info);
		info.EnterPool();
	}

	// Token: 0x060039A6 RID: 14758 RVA: 0x00156838 File Offset: 0x00154A38
	public void Push(GameObject instance)
	{
		Poolable component = instance.GetComponent<Poolable>();
		this.Push(component);
	}

	// Token: 0x060039A7 RID: 14759 RVA: 0x00156854 File Offset: 0x00154A54
	public GameObject Pop(Vector3 pos = default(Vector3), Quaternion rot = default(Quaternion))
	{
		while (this.stack.Count > 0)
		{
			Poolable poolable = this.stack.Pop();
			if (poolable)
			{
				poolable.transform.position = pos;
				poolable.transform.rotation = rot;
				poolable.LeavePool();
				return poolable.gameObject;
			}
		}
		return null;
	}

	// Token: 0x060039A8 RID: 14760 RVA: 0x001568AC File Offset: 0x00154AAC
	public void Clear()
	{
		foreach (Poolable poolable in this.stack)
		{
			if (poolable)
			{
				UnityEngine.Object.Destroy(poolable.gameObject);
			}
		}
		this.stack.Clear();
	}
}
