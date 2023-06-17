using System;
using UnityEngine;

// Token: 0x0200064A RID: 1610
[Serializable]
public class ByteMap
{
	// Token: 0x04002647 RID: 9799
	[SerializeField]
	private int size;

	// Token: 0x04002648 RID: 9800
	[SerializeField]
	private int bytes;

	// Token: 0x04002649 RID: 9801
	[SerializeField]
	private byte[] values;

	// Token: 0x06002E87 RID: 11911 RVA: 0x00117B18 File Offset: 0x00115D18
	public ByteMap(int size, int bytes = 1)
	{
		this.size = size;
		this.bytes = bytes;
		this.values = new byte[bytes * size * size];
	}

	// Token: 0x06002E88 RID: 11912 RVA: 0x00117B3E File Offset: 0x00115D3E
	public ByteMap(int size, byte[] values, int bytes = 1)
	{
		this.size = size;
		this.bytes = bytes;
		this.values = values;
	}

	// Token: 0x170003C8 RID: 968
	// (get) Token: 0x06002E89 RID: 11913 RVA: 0x00117B5B File Offset: 0x00115D5B
	public int Size
	{
		get
		{
			return this.size;
		}
	}

	// Token: 0x170003C9 RID: 969
	public uint this[int x, int y]
	{
		get
		{
			int num = y * this.bytes * this.size + x * this.bytes;
			switch (this.bytes)
			{
			case 1:
				return (uint)this.values[num];
			case 2:
			{
				uint num2 = (uint)this.values[num];
				uint num3 = (uint)this.values[num + 1];
				return num2 << 8 | num3;
			}
			case 3:
			{
				uint num4 = (uint)this.values[num];
				uint num3 = (uint)this.values[num + 1];
				uint num5 = (uint)this.values[num + 2];
				return num4 << 16 | num3 << 8 | num5;
			}
			default:
			{
				uint num6 = (uint)this.values[num];
				uint num3 = (uint)this.values[num + 1];
				uint num5 = (uint)this.values[num + 2];
				uint num7 = (uint)this.values[num + 3];
				return num6 << 24 | num3 << 16 | num5 << 8 | num7;
			}
			}
		}
		set
		{
			int num = y * this.bytes * this.size + x * this.bytes;
			switch (this.bytes)
			{
			case 1:
				this.values[num] = (byte)(value & 255U);
				return;
			case 2:
				this.values[num] = (byte)(value >> 8 & 255U);
				this.values[num + 1] = (byte)(value & 255U);
				return;
			case 3:
				this.values[num] = (byte)(value >> 16 & 255U);
				this.values[num + 1] = (byte)(value >> 8 & 255U);
				this.values[num + 2] = (byte)(value & 255U);
				return;
			default:
				this.values[num] = (byte)(value >> 24 & 255U);
				this.values[num + 1] = (byte)(value >> 16 & 255U);
				this.values[num + 2] = (byte)(value >> 8 & 255U);
				this.values[num + 3] = (byte)(value & 255U);
				return;
			}
		}
	}
}
