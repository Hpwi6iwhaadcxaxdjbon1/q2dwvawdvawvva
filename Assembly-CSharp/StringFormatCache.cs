using System;
using System.Collections.Generic;

// Token: 0x02000762 RID: 1890
public static class StringFormatCache
{
	// Token: 0x04002AE1 RID: 10977
	private static Dictionary<StringFormatCache.Key1, string> dict1 = new Dictionary<StringFormatCache.Key1, string>();

	// Token: 0x04002AE2 RID: 10978
	private static Dictionary<StringFormatCache.Key2, string> dict2 = new Dictionary<StringFormatCache.Key2, string>();

	// Token: 0x04002AE3 RID: 10979
	private static Dictionary<StringFormatCache.Key3, string> dict3 = new Dictionary<StringFormatCache.Key3, string>();

	// Token: 0x04002AE4 RID: 10980
	private static Dictionary<StringFormatCache.Key4, string> dict4 = new Dictionary<StringFormatCache.Key4, string>();

	// Token: 0x060034A4 RID: 13476 RVA: 0x00145BD0 File Offset: 0x00143DD0
	public static string Get(string format, string value1)
	{
		StringFormatCache.Key1 key = new StringFormatCache.Key1(format, value1);
		string text;
		if (!StringFormatCache.dict1.TryGetValue(key, out text))
		{
			text = string.Format(format, value1);
			StringFormatCache.dict1.Add(key, text);
		}
		return text;
	}

	// Token: 0x060034A5 RID: 13477 RVA: 0x00145C0C File Offset: 0x00143E0C
	public static string Get(string format, string value1, string value2)
	{
		StringFormatCache.Key2 key = new StringFormatCache.Key2(format, value1, value2);
		string text;
		if (!StringFormatCache.dict2.TryGetValue(key, out text))
		{
			text = string.Format(format, value1, value2);
			StringFormatCache.dict2.Add(key, text);
		}
		return text;
	}

	// Token: 0x060034A6 RID: 13478 RVA: 0x00145C48 File Offset: 0x00143E48
	public static string Get(string format, string value1, string value2, string value3)
	{
		StringFormatCache.Key3 key = new StringFormatCache.Key3(format, value1, value2, value3);
		string text;
		if (!StringFormatCache.dict3.TryGetValue(key, out text))
		{
			text = string.Format(format, value1, value2, value3);
			StringFormatCache.dict3.Add(key, text);
		}
		return text;
	}

	// Token: 0x060034A7 RID: 13479 RVA: 0x00145C88 File Offset: 0x00143E88
	public static string Get(string format, string value1, string value2, string value3, string value4)
	{
		StringFormatCache.Key4 key = new StringFormatCache.Key4(format, value1, value2, value3, value4);
		string text;
		if (!StringFormatCache.dict4.TryGetValue(key, out text))
		{
			text = string.Format(format, new object[]
			{
				value1,
				value2,
				value3,
				value4
			});
			StringFormatCache.dict4.Add(key, text);
		}
		return text;
	}

	// Token: 0x02000E71 RID: 3697
	private struct Key1 : IEquatable<StringFormatCache.Key1>
	{
		// Token: 0x04004BB7 RID: 19383
		public string format;

		// Token: 0x04004BB8 RID: 19384
		public string value1;

		// Token: 0x060052A2 RID: 21154 RVA: 0x001B0C03 File Offset: 0x001AEE03
		public Key1(string format, string value1)
		{
			this.format = format;
			this.value1 = value1;
		}

		// Token: 0x060052A3 RID: 21155 RVA: 0x001B0C13 File Offset: 0x001AEE13
		public override int GetHashCode()
		{
			return this.format.GetHashCode() ^ this.value1.GetHashCode();
		}

		// Token: 0x060052A4 RID: 21156 RVA: 0x001B0C2C File Offset: 0x001AEE2C
		public override bool Equals(object other)
		{
			return other is StringFormatCache.Key1 && this.Equals((StringFormatCache.Key1)other);
		}

		// Token: 0x060052A5 RID: 21157 RVA: 0x001B0C44 File Offset: 0x001AEE44
		public bool Equals(StringFormatCache.Key1 other)
		{
			return this.format == other.format && this.value1 == other.value1;
		}
	}

	// Token: 0x02000E72 RID: 3698
	private struct Key2 : IEquatable<StringFormatCache.Key2>
	{
		// Token: 0x04004BB9 RID: 19385
		public string format;

		// Token: 0x04004BBA RID: 19386
		public string value1;

		// Token: 0x04004BBB RID: 19387
		public string value2;

		// Token: 0x060052A6 RID: 21158 RVA: 0x001B0C6C File Offset: 0x001AEE6C
		public Key2(string format, string value1, string value2)
		{
			this.format = format;
			this.value1 = value1;
			this.value2 = value2;
		}

		// Token: 0x060052A7 RID: 21159 RVA: 0x001B0C83 File Offset: 0x001AEE83
		public override int GetHashCode()
		{
			return this.format.GetHashCode() ^ this.value1.GetHashCode() ^ this.value2.GetHashCode();
		}

		// Token: 0x060052A8 RID: 21160 RVA: 0x001B0CA8 File Offset: 0x001AEEA8
		public override bool Equals(object other)
		{
			return other is StringFormatCache.Key2 && this.Equals((StringFormatCache.Key2)other);
		}

		// Token: 0x060052A9 RID: 21161 RVA: 0x001B0CC0 File Offset: 0x001AEEC0
		public bool Equals(StringFormatCache.Key2 other)
		{
			return this.format == other.format && this.value1 == other.value1 && this.value2 == other.value2;
		}
	}

	// Token: 0x02000E73 RID: 3699
	private struct Key3 : IEquatable<StringFormatCache.Key3>
	{
		// Token: 0x04004BBC RID: 19388
		public string format;

		// Token: 0x04004BBD RID: 19389
		public string value1;

		// Token: 0x04004BBE RID: 19390
		public string value2;

		// Token: 0x04004BBF RID: 19391
		public string value3;

		// Token: 0x060052AA RID: 21162 RVA: 0x001B0CFB File Offset: 0x001AEEFB
		public Key3(string format, string value1, string value2, string value3)
		{
			this.format = format;
			this.value1 = value1;
			this.value2 = value2;
			this.value3 = value3;
		}

		// Token: 0x060052AB RID: 21163 RVA: 0x001B0D1A File Offset: 0x001AEF1A
		public override int GetHashCode()
		{
			return this.format.GetHashCode() ^ this.value1.GetHashCode() ^ this.value2.GetHashCode() ^ this.value3.GetHashCode();
		}

		// Token: 0x060052AC RID: 21164 RVA: 0x001B0D4B File Offset: 0x001AEF4B
		public override bool Equals(object other)
		{
			return other is StringFormatCache.Key3 && this.Equals((StringFormatCache.Key3)other);
		}

		// Token: 0x060052AD RID: 21165 RVA: 0x001B0D64 File Offset: 0x001AEF64
		public bool Equals(StringFormatCache.Key3 other)
		{
			return this.format == other.format && this.value1 == other.value1 && this.value2 == other.value2 && this.value3 == other.value3;
		}
	}

	// Token: 0x02000E74 RID: 3700
	private struct Key4 : IEquatable<StringFormatCache.Key4>
	{
		// Token: 0x04004BC0 RID: 19392
		public string format;

		// Token: 0x04004BC1 RID: 19393
		public string value1;

		// Token: 0x04004BC2 RID: 19394
		public string value2;

		// Token: 0x04004BC3 RID: 19395
		public string value3;

		// Token: 0x04004BC4 RID: 19396
		public string value4;

		// Token: 0x060052AE RID: 21166 RVA: 0x001B0DBD File Offset: 0x001AEFBD
		public Key4(string format, string value1, string value2, string value3, string value4)
		{
			this.format = format;
			this.value1 = value1;
			this.value2 = value2;
			this.value3 = value3;
			this.value4 = value4;
		}

		// Token: 0x060052AF RID: 21167 RVA: 0x001B0DE4 File Offset: 0x001AEFE4
		public override int GetHashCode()
		{
			return this.format.GetHashCode() ^ this.value1.GetHashCode() ^ this.value2.GetHashCode() ^ this.value3.GetHashCode() ^ this.value4.GetHashCode();
		}

		// Token: 0x060052B0 RID: 21168 RVA: 0x001B0E21 File Offset: 0x001AF021
		public override bool Equals(object other)
		{
			return other is StringFormatCache.Key4 && this.Equals((StringFormatCache.Key4)other);
		}

		// Token: 0x060052B1 RID: 21169 RVA: 0x001B0E3C File Offset: 0x001AF03C
		public bool Equals(StringFormatCache.Key4 other)
		{
			return this.format == other.format && this.value1 == other.value1 && this.value2 == other.value2 && this.value3 == other.value3 && this.value4 == other.value4;
		}
	}
}
