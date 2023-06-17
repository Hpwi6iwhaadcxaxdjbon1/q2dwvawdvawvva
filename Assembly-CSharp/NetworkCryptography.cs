using System;
using Network;

// Token: 0x0200061F RID: 1567
public abstract class NetworkCryptography : INetworkCryptography
{
	// Token: 0x040025C2 RID: 9666
	private byte[] buffer = new byte[8388608];

	// Token: 0x06002E25 RID: 11813 RVA: 0x00114DD0 File Offset: 0x00112FD0
	public unsafe ArraySegment<byte> EncryptCopy(Connection connection, ArraySegment<byte> data)
	{
		ArraySegment<byte> src = new ArraySegment<byte>(data.Array, data.Offset, data.Count);
		ArraySegment<byte> result = new ArraySegment<byte>(this.buffer, data.Offset, this.buffer.Length - data.Offset);
		if (data.Offset > 0)
		{
			byte[] array;
			byte* destination;
			if ((array = result.Array) == null || array.Length == 0)
			{
				destination = null;
			}
			else
			{
				destination = &array[0];
			}
			byte[] array2;
			byte* source;
			if ((array2 = data.Array) == null || array2.Length == 0)
			{
				source = null;
			}
			else
			{
				source = &array2[0];
			}
			Buffer.MemoryCopy((void*)source, (void*)destination, (long)result.Array.Length, (long)data.Offset);
			array2 = null;
			array = null;
		}
		this.EncryptionHandler(connection, src, ref result);
		return result;
	}

	// Token: 0x06002E26 RID: 11814 RVA: 0x00114E94 File Offset: 0x00113094
	public unsafe ArraySegment<byte> DecryptCopy(Connection connection, ArraySegment<byte> data)
	{
		ArraySegment<byte> src = new ArraySegment<byte>(data.Array, data.Offset, data.Count);
		ArraySegment<byte> result = new ArraySegment<byte>(this.buffer, data.Offset, this.buffer.Length - data.Offset);
		if (data.Offset > 0)
		{
			byte[] array;
			byte* destination;
			if ((array = result.Array) == null || array.Length == 0)
			{
				destination = null;
			}
			else
			{
				destination = &array[0];
			}
			byte[] array2;
			byte* source;
			if ((array2 = data.Array) == null || array2.Length == 0)
			{
				source = null;
			}
			else
			{
				source = &array2[0];
			}
			Buffer.MemoryCopy((void*)source, (void*)destination, (long)result.Array.Length, (long)data.Offset);
			array2 = null;
			array = null;
		}
		this.DecryptionHandler(connection, src, ref result);
		return result;
	}

	// Token: 0x06002E27 RID: 11815 RVA: 0x00114F58 File Offset: 0x00113158
	public void Encrypt(Connection connection, ref ArraySegment<byte> data)
	{
		ArraySegment<byte> src = new ArraySegment<byte>(data.Array, data.Offset, data.Count);
		ArraySegment<byte> arraySegment = new ArraySegment<byte>(data.Array, data.Offset, data.Array.Length - data.Offset);
		this.EncryptionHandler(connection, src, ref arraySegment);
		data = arraySegment;
	}

	// Token: 0x06002E28 RID: 11816 RVA: 0x00114FB4 File Offset: 0x001131B4
	public void Decrypt(Connection connection, ref ArraySegment<byte> data)
	{
		ArraySegment<byte> src = new ArraySegment<byte>(data.Array, data.Offset, data.Count);
		ArraySegment<byte> arraySegment = new ArraySegment<byte>(data.Array, data.Offset, data.Array.Length - data.Offset);
		this.DecryptionHandler(connection, src, ref arraySegment);
		data = arraySegment;
	}

	// Token: 0x06002E29 RID: 11817
	protected abstract void EncryptionHandler(Connection connection, ArraySegment<byte> src, ref ArraySegment<byte> dst);

	// Token: 0x06002E2A RID: 11818
	protected abstract void DecryptionHandler(Connection connection, ArraySegment<byte> src, ref ArraySegment<byte> dst);
}
