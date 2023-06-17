using System;
using UnityEngine;

// Token: 0x02000551 RID: 1361
public class Prefab<T> : Prefab, IComparable<Prefab<T>> where T : Component
{
	// Token: 0x0400223E RID: 8766
	public T Component;

	// Token: 0x060029FE RID: 10750 RVA: 0x00100230 File Offset: 0x000FE430
	public Prefab(string name, GameObject prefab, T component, GameManager manager, PrefabAttribute.Library attribute) : base(name, prefab, manager, attribute)
	{
		this.Component = component;
	}

	// Token: 0x060029FF RID: 10751 RVA: 0x00100245 File Offset: 0x000FE445
	public int CompareTo(Prefab<T> that)
	{
		return base.CompareTo(that);
	}
}
