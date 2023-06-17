using System;
using System.Collections;
using System.Collections.Generic;

namespace TinyJSON
{
	// Token: 0x020009D3 RID: 2515
	public sealed class ProxyArray : Variant, IEnumerable<Variant>, IEnumerable
	{
		// Token: 0x04003675 RID: 13941
		private readonly List<Variant> list;

		// Token: 0x06003BF7 RID: 15351 RVA: 0x00162B1F File Offset: 0x00160D1F
		public ProxyArray()
		{
			this.list = new List<Variant>();
		}

		// Token: 0x06003BF8 RID: 15352 RVA: 0x00162B32 File Offset: 0x00160D32
		IEnumerator<Variant> IEnumerable<Variant>.GetEnumerator()
		{
			return this.list.GetEnumerator();
		}

		// Token: 0x06003BF9 RID: 15353 RVA: 0x00162B32 File Offset: 0x00160D32
		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.list.GetEnumerator();
		}

		// Token: 0x06003BFA RID: 15354 RVA: 0x00162B44 File Offset: 0x00160D44
		public void Add(Variant item)
		{
			this.list.Add(item);
		}

		// Token: 0x170004DD RID: 1245
		public override Variant this[int index]
		{
			get
			{
				return this.list[index];
			}
			set
			{
				this.list[index] = value;
			}
		}

		// Token: 0x170004DE RID: 1246
		// (get) Token: 0x06003BFD RID: 15357 RVA: 0x00162B6F File Offset: 0x00160D6F
		public int Count
		{
			get
			{
				return this.list.Count;
			}
		}

		// Token: 0x06003BFE RID: 15358 RVA: 0x00162B7C File Offset: 0x00160D7C
		internal bool CanBeMultiRankArray(int[] rankLengths)
		{
			return this.CanBeMultiRankArray(0, rankLengths);
		}

		// Token: 0x06003BFF RID: 15359 RVA: 0x00162B88 File Offset: 0x00160D88
		private bool CanBeMultiRankArray(int rank, int[] rankLengths)
		{
			int count = this.list.Count;
			rankLengths[rank] = count;
			if (rank == rankLengths.Length - 1)
			{
				return true;
			}
			ProxyArray proxyArray = this.list[0] as ProxyArray;
			if (proxyArray == null)
			{
				return false;
			}
			int count2 = proxyArray.Count;
			for (int i = 1; i < count; i++)
			{
				ProxyArray proxyArray2 = this.list[i] as ProxyArray;
				if (proxyArray2 == null)
				{
					return false;
				}
				if (proxyArray2.Count != count2)
				{
					return false;
				}
				if (!proxyArray2.CanBeMultiRankArray(rank + 1, rankLengths))
				{
					return false;
				}
			}
			return true;
		}
	}
}
