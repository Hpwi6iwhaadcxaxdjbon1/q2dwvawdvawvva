using System;

namespace Rust.AI
{
	// Token: 0x02000B48 RID: 2888
	public class AStarNode
	{
		// Token: 0x04003E76 RID: 15990
		public AStarNode Parent;

		// Token: 0x04003E77 RID: 15991
		public float G;

		// Token: 0x04003E78 RID: 15992
		public float H;

		// Token: 0x04003E79 RID: 15993
		public IAIPathNode Node;

		// Token: 0x17000665 RID: 1637
		// (get) Token: 0x06004600 RID: 17920 RVA: 0x00198BC8 File Offset: 0x00196DC8
		public float F
		{
			get
			{
				return this.G + this.H;
			}
		}

		// Token: 0x06004601 RID: 17921 RVA: 0x00198BD7 File Offset: 0x00196DD7
		public AStarNode(float g, float h, AStarNode parent, IAIPathNode node)
		{
			this.G = g;
			this.H = h;
			this.Parent = parent;
			this.Node = node;
		}

		// Token: 0x06004602 RID: 17922 RVA: 0x00198BFC File Offset: 0x00196DFC
		public void Update(float g, float h, AStarNode parent, IAIPathNode node)
		{
			this.G = g;
			this.H = h;
			this.Parent = parent;
			this.Node = node;
		}

		// Token: 0x06004603 RID: 17923 RVA: 0x00198C1B File Offset: 0x00196E1B
		public bool Satisfies(IAIPathNode node)
		{
			return this.Node == node;
		}

		// Token: 0x06004604 RID: 17924 RVA: 0x00198C26 File Offset: 0x00196E26
		public static bool operator <(AStarNode lhs, AStarNode rhs)
		{
			return lhs.F < rhs.F;
		}

		// Token: 0x06004605 RID: 17925 RVA: 0x00198C36 File Offset: 0x00196E36
		public static bool operator >(AStarNode lhs, AStarNode rhs)
		{
			return lhs.F > rhs.F;
		}
	}
}
