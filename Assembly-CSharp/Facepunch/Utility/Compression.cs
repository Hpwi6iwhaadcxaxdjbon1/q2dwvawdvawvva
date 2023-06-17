using System;
using Ionic.Zlib;

namespace Facepunch.Utility
{
	// Token: 0x02000AFB RID: 2811
	public class Compression
	{
		// Token: 0x060044A2 RID: 17570 RVA: 0x00193150 File Offset: 0x00191350
		public static byte[] Compress(byte[] data)
		{
			byte[] result;
			try
			{
				result = GZipStream.CompressBuffer(data);
			}
			catch (Exception)
			{
				result = null;
			}
			return result;
		}

		// Token: 0x060044A3 RID: 17571 RVA: 0x0019317C File Offset: 0x0019137C
		public static byte[] Uncompress(byte[] data)
		{
			return GZipStream.UncompressBuffer(data);
		}
	}
}
