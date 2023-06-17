using System;
using UnityEngine;

// Token: 0x0200064B RID: 1611
[Serializable]
public sealed class ByteQuadtree
{
	// Token: 0x0400264A RID: 9802
	[SerializeField]
	private int size;

	// Token: 0x0400264B RID: 9803
	[SerializeField]
	private int levels;

	// Token: 0x0400264C RID: 9804
	[SerializeField]
	private ByteMap[] values;

	// Token: 0x06002E8C RID: 11916 RVA: 0x00117D30 File Offset: 0x00115F30
	public void UpdateValues(byte[] baseValues)
	{
		this.size = Mathf.RoundToInt(Mathf.Sqrt((float)baseValues.Length));
		this.levels = Mathf.RoundToInt(Mathf.Log((float)this.size, 2f)) + 1;
		this.values = new ByteMap[this.levels];
		this.values[0] = new ByteMap(this.size, baseValues, 1);
		for (int i = 1; i < this.levels; i++)
		{
			ByteMap byteMap = this.values[i - 1];
			ByteMap byteMap2 = this.values[i] = this.CreateLevel(i);
			for (int j = 0; j < byteMap2.Size; j++)
			{
				for (int k = 0; k < byteMap2.Size; k++)
				{
					byteMap2[k, j] = byteMap[2 * k, 2 * j] + byteMap[2 * k + 1, 2 * j] + byteMap[2 * k, 2 * j + 1] + byteMap[2 * k + 1, 2 * j + 1];
				}
			}
		}
	}

	// Token: 0x170003CA RID: 970
	// (get) Token: 0x06002E8D RID: 11917 RVA: 0x00117E41 File Offset: 0x00116041
	public int Size
	{
		get
		{
			return this.size;
		}
	}

	// Token: 0x170003CB RID: 971
	// (get) Token: 0x06002E8E RID: 11918 RVA: 0x00117E49 File Offset: 0x00116049
	public ByteQuadtree.Element Root
	{
		get
		{
			return new ByteQuadtree.Element(this, 0, 0, this.levels - 1);
		}
	}

	// Token: 0x06002E8F RID: 11919 RVA: 0x00117E5C File Offset: 0x0011605C
	private ByteMap CreateLevel(int level)
	{
		int num = 1 << this.levels - level - 1;
		int bytes = 1 + (level + 3) / 4;
		return new ByteMap(num, bytes);
	}

	// Token: 0x02000D98 RID: 3480
	public struct Element
	{
		// Token: 0x0400481F RID: 18463
		private ByteQuadtree source;

		// Token: 0x04004820 RID: 18464
		private int x;

		// Token: 0x04004821 RID: 18465
		private int y;

		// Token: 0x04004822 RID: 18466
		private int level;

		// Token: 0x06005111 RID: 20753 RVA: 0x001AAEB1 File Offset: 0x001A90B1
		public Element(ByteQuadtree source, int x, int y, int level)
		{
			this.source = source;
			this.x = x;
			this.y = y;
			this.level = level;
		}

		// Token: 0x170006C8 RID: 1736
		// (get) Token: 0x06005112 RID: 20754 RVA: 0x001AAED0 File Offset: 0x001A90D0
		public bool IsLeaf
		{
			get
			{
				return this.level == 0;
			}
		}

		// Token: 0x170006C9 RID: 1737
		// (get) Token: 0x06005113 RID: 20755 RVA: 0x001AAEDB File Offset: 0x001A90DB
		public bool IsRoot
		{
			get
			{
				return this.level == this.source.levels - 1;
			}
		}

		// Token: 0x170006CA RID: 1738
		// (get) Token: 0x06005114 RID: 20756 RVA: 0x001AAEF2 File Offset: 0x001A90F2
		public int ByteMap
		{
			get
			{
				return this.level;
			}
		}

		// Token: 0x170006CB RID: 1739
		// (get) Token: 0x06005115 RID: 20757 RVA: 0x001AAEFA File Offset: 0x001A90FA
		public uint Value
		{
			get
			{
				return this.source.values[this.level][this.x, this.y];
			}
		}

		// Token: 0x170006CC RID: 1740
		// (get) Token: 0x06005116 RID: 20758 RVA: 0x001AAF1F File Offset: 0x001A911F
		public Vector2 Coords
		{
			get
			{
				return new Vector2((float)this.x, (float)this.y);
			}
		}

		// Token: 0x170006CD RID: 1741
		// (get) Token: 0x06005117 RID: 20759 RVA: 0x001AAF34 File Offset: 0x001A9134
		public int Depth
		{
			get
			{
				return this.source.levels - this.level - 1;
			}
		}

		// Token: 0x170006CE RID: 1742
		// (get) Token: 0x06005118 RID: 20760 RVA: 0x001AAF4A File Offset: 0x001A914A
		public ByteQuadtree.Element Parent
		{
			get
			{
				if (this.IsRoot)
				{
					throw new Exception("Element is the root and therefore has no parent.");
				}
				return new ByteQuadtree.Element(this.source, this.x / 2, this.y / 2, this.level + 1);
			}
		}

		// Token: 0x170006CF RID: 1743
		// (get) Token: 0x06005119 RID: 20761 RVA: 0x001AAF82 File Offset: 0x001A9182
		public ByteQuadtree.Element Child1
		{
			get
			{
				if (this.IsLeaf)
				{
					throw new Exception("Element is a leaf and therefore has no children.");
				}
				return new ByteQuadtree.Element(this.source, this.x * 2, this.y * 2, this.level - 1);
			}
		}

		// Token: 0x170006D0 RID: 1744
		// (get) Token: 0x0600511A RID: 20762 RVA: 0x001AAFBA File Offset: 0x001A91BA
		public ByteQuadtree.Element Child2
		{
			get
			{
				if (this.IsLeaf)
				{
					throw new Exception("Element is a leaf and therefore has no children.");
				}
				return new ByteQuadtree.Element(this.source, this.x * 2 + 1, this.y * 2, this.level - 1);
			}
		}

		// Token: 0x170006D1 RID: 1745
		// (get) Token: 0x0600511B RID: 20763 RVA: 0x001AAFF4 File Offset: 0x001A91F4
		public ByteQuadtree.Element Child3
		{
			get
			{
				if (this.IsLeaf)
				{
					throw new Exception("Element is a leaf and therefore has no children.");
				}
				return new ByteQuadtree.Element(this.source, this.x * 2, this.y * 2 + 1, this.level - 1);
			}
		}

		// Token: 0x170006D2 RID: 1746
		// (get) Token: 0x0600511C RID: 20764 RVA: 0x001AB02E File Offset: 0x001A922E
		public ByteQuadtree.Element Child4
		{
			get
			{
				if (this.IsLeaf)
				{
					throw new Exception("Element is a leaf and therefore has no children.");
				}
				return new ByteQuadtree.Element(this.source, this.x * 2 + 1, this.y * 2 + 1, this.level - 1);
			}
		}

		// Token: 0x170006D3 RID: 1747
		// (get) Token: 0x0600511D RID: 20765 RVA: 0x001AB06C File Offset: 0x001A926C
		public ByteQuadtree.Element MaxChild
		{
			get
			{
				ByteQuadtree.Element child = this.Child1;
				ByteQuadtree.Element child2 = this.Child2;
				ByteQuadtree.Element child3 = this.Child3;
				ByteQuadtree.Element child4 = this.Child4;
				uint value = child.Value;
				uint value2 = child2.Value;
				uint value3 = child3.Value;
				uint value4 = child4.Value;
				if (value >= value2 && value >= value3 && value >= value4)
				{
					return child;
				}
				if (value2 >= value3 && value2 >= value4)
				{
					return child2;
				}
				if (value3 >= value4)
				{
					return child3;
				}
				return child4;
			}
		}

		// Token: 0x170006D4 RID: 1748
		// (get) Token: 0x0600511E RID: 20766 RVA: 0x001AB0E4 File Offset: 0x001A92E4
		public ByteQuadtree.Element RandChild
		{
			get
			{
				ByteQuadtree.Element child = this.Child1;
				ByteQuadtree.Element child2 = this.Child2;
				ByteQuadtree.Element child3 = this.Child3;
				ByteQuadtree.Element child4 = this.Child4;
				uint value = child.Value;
				uint value2 = child2.Value;
				uint value3 = child3.Value;
				uint value4 = child4.Value;
				float num = value + value2 + value3 + value4;
				float value5 = UnityEngine.Random.value;
				if (value / num >= value5)
				{
					return child;
				}
				if ((value + value2) / num >= value5)
				{
					return child2;
				}
				if ((value + value2 + value3) / num >= value5)
				{
					return child3;
				}
				return child4;
			}
		}
	}
}
