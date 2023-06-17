using System;
using System.Collections.Generic;
using Facepunch;
using UnityEngine;

// Token: 0x0200088F RID: 2191
public class ServerBrowserList : BaseMonoBehaviour, VirtualScroll.IDataSource
{
	// Token: 0x04003127 RID: 12583
	public ServerBrowserList.QueryType queryType;

	// Token: 0x04003128 RID: 12584
	public static string VersionTag = "v" + 2392;

	// Token: 0x04003129 RID: 12585
	public ServerBrowserList.ServerKeyvalues[] keyValues = new ServerBrowserList.ServerKeyvalues[0];

	// Token: 0x0400312A RID: 12586
	public ServerBrowserCategory categoryButton;

	// Token: 0x0400312B RID: 12587
	public bool startActive;

	// Token: 0x0400312C RID: 12588
	public Transform listTransform;

	// Token: 0x0400312D RID: 12589
	public int refreshOrder;

	// Token: 0x0400312E RID: 12590
	public bool UseOfficialServers;

	// Token: 0x0400312F RID: 12591
	public VirtualScroll VirtualScroll;

	// Token: 0x04003130 RID: 12592
	public ServerBrowserList.Rules[] rules;

	// Token: 0x04003131 RID: 12593
	public bool hideOfficialServers;

	// Token: 0x04003132 RID: 12594
	public bool excludeEmptyServersUsingQuery;

	// Token: 0x04003133 RID: 12595
	public bool alwaysIncludeEmptyServers;

	// Token: 0x04003134 RID: 12596
	public bool clampPlayerCountsToTrustedValues;

	// Token: 0x060036CC RID: 14028 RVA: 0x00007A3C File Offset: 0x00005C3C
	public int GetItemCount()
	{
		return 0;
	}

	// Token: 0x060036CD RID: 14029 RVA: 0x000063A5 File Offset: 0x000045A5
	public void SetItemData(int i, GameObject obj)
	{
	}

	// Token: 0x02000E9B RID: 3739
	public enum QueryType
	{
		// Token: 0x04004C46 RID: 19526
		RegularInternet,
		// Token: 0x04004C47 RID: 19527
		Friends,
		// Token: 0x04004C48 RID: 19528
		History,
		// Token: 0x04004C49 RID: 19529
		LAN,
		// Token: 0x04004C4A RID: 19530
		Favourites,
		// Token: 0x04004C4B RID: 19531
		None
	}

	// Token: 0x02000E9C RID: 3740
	[Serializable]
	public struct ServerKeyvalues
	{
		// Token: 0x04004C4C RID: 19532
		public string key;

		// Token: 0x04004C4D RID: 19533
		public string value;
	}

	// Token: 0x02000E9D RID: 3741
	[Serializable]
	public struct Rules
	{
		// Token: 0x04004C4E RID: 19534
		public string tag;

		// Token: 0x04004C4F RID: 19535
		public ServerBrowserList serverList;
	}

	// Token: 0x02000E9E RID: 3742
	private class HashSetEqualityComparer<T> : IEqualityComparer<HashSet<T>> where T : IComparable<T>
	{
		// Token: 0x170006FB RID: 1787
		// (get) Token: 0x060052F2 RID: 21234 RVA: 0x001B147F File Offset: 0x001AF67F
		public static ServerBrowserList.HashSetEqualityComparer<T> Instance { get; } = new ServerBrowserList.HashSetEqualityComparer<T>();

		// Token: 0x060052F3 RID: 21235 RVA: 0x001B1488 File Offset: 0x001AF688
		public bool Equals(HashSet<T> x, HashSet<T> y)
		{
			if (x == y)
			{
				return true;
			}
			if (x == null)
			{
				return false;
			}
			if (y == null)
			{
				return false;
			}
			if (x.GetType() != y.GetType())
			{
				return false;
			}
			if (x.Count != y.Count)
			{
				return false;
			}
			foreach (T item in x)
			{
				if (!y.Contains(item))
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x060052F4 RID: 21236 RVA: 0x001B1514 File Offset: 0x001AF714
		public int GetHashCode(HashSet<T> set)
		{
			int num = 0;
			if (set != null)
			{
				foreach (T t in set)
				{
					num ^= (((t != null) ? t.GetHashCode() : 0) & int.MaxValue);
				}
			}
			return num;
		}
	}
}
