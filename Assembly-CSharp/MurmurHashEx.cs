using System;
using System.IO;
using System.Text;

// Token: 0x02000945 RID: 2373
public static class MurmurHashEx
{
	// Token: 0x060038EF RID: 14575 RVA: 0x00152E4B File Offset: 0x0015104B
	public static int MurmurHashSigned(this string str)
	{
		return MurmurHash.Signed(MurmurHashEx.StringToStream(str));
	}

	// Token: 0x060038F0 RID: 14576 RVA: 0x00152E58 File Offset: 0x00151058
	public static uint MurmurHashUnsigned(this string str)
	{
		return MurmurHash.Unsigned(MurmurHashEx.StringToStream(str));
	}

	// Token: 0x060038F1 RID: 14577 RVA: 0x00152E65 File Offset: 0x00151065
	private static MemoryStream StringToStream(string str)
	{
		return new MemoryStream(Encoding.UTF8.GetBytes(str ?? string.Empty));
	}
}
