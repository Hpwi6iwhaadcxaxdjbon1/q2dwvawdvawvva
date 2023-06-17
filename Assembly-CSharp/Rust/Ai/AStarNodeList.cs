using System;
using System.Collections.Generic;

namespace Rust.AI
{
	// Token: 0x02000B49 RID: 2889
	public class AStarNodeList : List<AStarNode>
	{
		// Token: 0x04003E7A RID: 15994
		private readonly AStarNodeList.AStarNodeComparer comparer = new AStarNodeList.AStarNodeComparer();

		// Token: 0x06004606 RID: 17926 RVA: 0x00198C48 File Offset: 0x00196E48
		public bool Contains(IAIPathNode n)
		{
			for (int i = 0; i < base.Count; i++)
			{
				AStarNode astarNode = base[i];
				if (astarNode != null && astarNode.Node.Equals(n))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06004607 RID: 17927 RVA: 0x00198C84 File Offset: 0x00196E84
		public AStarNode GetAStarNodeOf(IAIPathNode n)
		{
			for (int i = 0; i < base.Count; i++)
			{
				AStarNode astarNode = base[i];
				if (astarNode != null && astarNode.Node.Equals(n))
				{
					return astarNode;
				}
			}
			return null;
		}

		// Token: 0x06004608 RID: 17928 RVA: 0x00198CBE File Offset: 0x00196EBE
		public void AStarNodeSort()
		{
			base.Sort(this.comparer);
		}

		// Token: 0x02000FA3 RID: 4003
		private class AStarNodeComparer : IComparer<AStarNode>
		{
			// Token: 0x0600556C RID: 21868 RVA: 0x001BA1AB File Offset: 0x001B83AB
			int IComparer<AStarNode>.Compare(AStarNode lhs, AStarNode rhs)
			{
				if (lhs < rhs)
				{
					return -1;
				}
				if (lhs > rhs)
				{
					return 1;
				}
				return 0;
			}
		}
	}
}
