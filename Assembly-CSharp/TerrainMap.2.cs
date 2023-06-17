using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

// Token: 0x0200069C RID: 1692
public abstract class TerrainMap<T> : TerrainMap where T : struct
{
	// Token: 0x04002785 RID: 10117
	internal T[] src;

	// Token: 0x04002786 RID: 10118
	internal T[] dst;

	// Token: 0x0600306A RID: 12394 RVA: 0x00122E05 File Offset: 0x00121005
	public void Push()
	{
		if (this.src != this.dst)
		{
			return;
		}
		this.dst = (T[])this.src.Clone();
	}

	// Token: 0x0600306B RID: 12395 RVA: 0x00122E2C File Offset: 0x0012102C
	public void Pop()
	{
		if (this.src == this.dst)
		{
			return;
		}
		Array.Copy(this.dst, this.src, this.src.Length);
		this.dst = this.src;
	}

	// Token: 0x0600306C RID: 12396 RVA: 0x00122E62 File Offset: 0x00121062
	public IEnumerable<T> ToEnumerable()
	{
		return this.src.Cast<T>();
	}

	// Token: 0x0600306D RID: 12397 RVA: 0x00122E6F File Offset: 0x0012106F
	public int BytesPerElement()
	{
		return Marshal.SizeOf(typeof(T));
	}

	// Token: 0x0600306E RID: 12398 RVA: 0x00122E80 File Offset: 0x00121080
	public long GetMemoryUsage()
	{
		return (long)this.BytesPerElement() * (long)this.src.Length;
	}

	// Token: 0x0600306F RID: 12399 RVA: 0x00122E94 File Offset: 0x00121094
	public byte[] ToByteArray()
	{
		byte[] array = new byte[this.BytesPerElement() * this.src.Length];
		Buffer.BlockCopy(this.src, 0, array, 0, array.Length);
		return array;
	}

	// Token: 0x06003070 RID: 12400 RVA: 0x00122EC8 File Offset: 0x001210C8
	public void FromByteArray(byte[] dat)
	{
		Buffer.BlockCopy(dat, 0, this.dst, 0, dat.Length);
	}
}
