using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020001DA RID: 474
public class AIInformationCellContents<T> where T : AIPoint
{
	// Token: 0x0400121F RID: 4639
	public HashSet<T> Items = new HashSet<T>();

	// Token: 0x17000221 RID: 545
	// (get) Token: 0x06001937 RID: 6455 RVA: 0x000B9738 File Offset: 0x000B7938
	public int Count
	{
		get
		{
			return this.Items.Count;
		}
	}

	// Token: 0x17000222 RID: 546
	// (get) Token: 0x06001938 RID: 6456 RVA: 0x000B9745 File Offset: 0x000B7945
	public bool Empty
	{
		get
		{
			return this.Items.Count == 0;
		}
	}

	// Token: 0x06001939 RID: 6457 RVA: 0x000B9758 File Offset: 0x000B7958
	public void Init(Bounds cellBounds, GameObject root)
	{
		this.Clear();
		foreach (T t in root.GetComponentsInChildren<T>(true))
		{
			if (cellBounds.Contains(t.gameObject.transform.position))
			{
				this.Add(t);
			}
		}
	}

	// Token: 0x0600193A RID: 6458 RVA: 0x000B97AE File Offset: 0x000B79AE
	public void Clear()
	{
		this.Items.Clear();
	}

	// Token: 0x0600193B RID: 6459 RVA: 0x000B97BB File Offset: 0x000B79BB
	public void Add(T item)
	{
		this.Items.Add(item);
	}

	// Token: 0x0600193C RID: 6460 RVA: 0x000B97CA File Offset: 0x000B79CA
	public void Remove(T item)
	{
		this.Items.Remove(item);
	}
}
