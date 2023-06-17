using System;
using System.IO;

// Token: 0x02000926 RID: 2342
public static class StreamEx
{
	// Token: 0x04003337 RID: 13111
	private static readonly byte[] StaticBuffer = new byte[16384];

	// Token: 0x06003855 RID: 14421 RVA: 0x00150198 File Offset: 0x0014E398
	public static void WriteToOtherStream(this Stream self, Stream target)
	{
		int count;
		while ((count = self.Read(StreamEx.StaticBuffer, 0, StreamEx.StaticBuffer.Length)) > 0)
		{
			target.Write(StreamEx.StaticBuffer, 0, count);
		}
	}
}
