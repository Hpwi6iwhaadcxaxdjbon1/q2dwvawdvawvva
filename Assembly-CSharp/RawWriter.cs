using System;
using System.Collections.Generic;
using System.IO;

// Token: 0x0200094B RID: 2379
public static class RawWriter
{
	// Token: 0x0600392B RID: 14635 RVA: 0x00154630 File Offset: 0x00152830
	public static void Write(IEnumerable<byte> data, string path)
	{
		using (FileStream fileStream = File.Open(path, FileMode.Create))
		{
			using (BinaryWriter binaryWriter = new BinaryWriter(fileStream))
			{
				foreach (byte value in data)
				{
					binaryWriter.Write(value);
				}
			}
		}
	}

	// Token: 0x0600392C RID: 14636 RVA: 0x001546B4 File Offset: 0x001528B4
	public static void Write(IEnumerable<int> data, string path)
	{
		using (FileStream fileStream = File.Open(path, FileMode.Create))
		{
			using (BinaryWriter binaryWriter = new BinaryWriter(fileStream))
			{
				foreach (int value in data)
				{
					binaryWriter.Write(value);
				}
			}
		}
	}

	// Token: 0x0600392D RID: 14637 RVA: 0x00154738 File Offset: 0x00152938
	public static void Write(IEnumerable<short> data, string path)
	{
		using (FileStream fileStream = File.Open(path, FileMode.Create))
		{
			using (BinaryWriter binaryWriter = new BinaryWriter(fileStream))
			{
				foreach (short value in data)
				{
					binaryWriter.Write(value);
				}
			}
		}
	}

	// Token: 0x0600392E RID: 14638 RVA: 0x001547BC File Offset: 0x001529BC
	public static void Write(IEnumerable<float> data, string path)
	{
		using (FileStream fileStream = File.Open(path, FileMode.Create))
		{
			using (BinaryWriter binaryWriter = new BinaryWriter(fileStream))
			{
				foreach (float value in data)
				{
					binaryWriter.Write(value);
				}
			}
		}
	}
}
