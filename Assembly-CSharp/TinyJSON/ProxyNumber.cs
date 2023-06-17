using System;
using System.Globalization;

namespace TinyJSON
{
	// Token: 0x020009D5 RID: 2517
	public sealed class ProxyNumber : Variant
	{
		// Token: 0x04003677 RID: 13943
		private static readonly char[] floatingPointCharacters = new char[]
		{
			'.',
			'e'
		};

		// Token: 0x04003678 RID: 13944
		private readonly IConvertible value;

		// Token: 0x06003C03 RID: 15363 RVA: 0x00162C3C File Offset: 0x00160E3C
		public ProxyNumber(IConvertible value)
		{
			string text = value as string;
			this.value = ((text != null) ? ProxyNumber.Parse(text) : value);
		}

		// Token: 0x06003C04 RID: 15364 RVA: 0x00162C68 File Offset: 0x00160E68
		private static IConvertible Parse(string value)
		{
			if (value.IndexOfAny(ProxyNumber.floatingPointCharacters) == -1)
			{
				ulong num2;
				if (value[0] == '-')
				{
					long num;
					if (long.TryParse(value, NumberStyles.Float, NumberFormatInfo.InvariantInfo, out num))
					{
						return num;
					}
				}
				else if (ulong.TryParse(value, NumberStyles.Float, NumberFormatInfo.InvariantInfo, out num2))
				{
					return num2;
				}
			}
			decimal num3;
			if (decimal.TryParse(value, NumberStyles.Float, NumberFormatInfo.InvariantInfo, out num3))
			{
				double num4;
				if (num3 == 0m && double.TryParse(value, NumberStyles.Float, NumberFormatInfo.InvariantInfo, out num4) && Math.Abs(num4) > 5E-324)
				{
					return num4;
				}
				return num3;
			}
			else
			{
				double num5;
				if (double.TryParse(value, NumberStyles.Float, NumberFormatInfo.InvariantInfo, out num5))
				{
					return num5;
				}
				return 0;
			}
		}

		// Token: 0x06003C05 RID: 15365 RVA: 0x00162D3B File Offset: 0x00160F3B
		public override bool ToBoolean(IFormatProvider provider)
		{
			return this.value.ToBoolean(provider);
		}

		// Token: 0x06003C06 RID: 15366 RVA: 0x00162D49 File Offset: 0x00160F49
		public override byte ToByte(IFormatProvider provider)
		{
			return this.value.ToByte(provider);
		}

		// Token: 0x06003C07 RID: 15367 RVA: 0x00162D57 File Offset: 0x00160F57
		public override char ToChar(IFormatProvider provider)
		{
			return this.value.ToChar(provider);
		}

		// Token: 0x06003C08 RID: 15368 RVA: 0x00162D65 File Offset: 0x00160F65
		public override decimal ToDecimal(IFormatProvider provider)
		{
			return this.value.ToDecimal(provider);
		}

		// Token: 0x06003C09 RID: 15369 RVA: 0x00162D73 File Offset: 0x00160F73
		public override double ToDouble(IFormatProvider provider)
		{
			return this.value.ToDouble(provider);
		}

		// Token: 0x06003C0A RID: 15370 RVA: 0x00162D81 File Offset: 0x00160F81
		public override short ToInt16(IFormatProvider provider)
		{
			return this.value.ToInt16(provider);
		}

		// Token: 0x06003C0B RID: 15371 RVA: 0x00162D8F File Offset: 0x00160F8F
		public override int ToInt32(IFormatProvider provider)
		{
			return this.value.ToInt32(provider);
		}

		// Token: 0x06003C0C RID: 15372 RVA: 0x00162D9D File Offset: 0x00160F9D
		public override long ToInt64(IFormatProvider provider)
		{
			return this.value.ToInt64(provider);
		}

		// Token: 0x06003C0D RID: 15373 RVA: 0x00162DAB File Offset: 0x00160FAB
		public override sbyte ToSByte(IFormatProvider provider)
		{
			return this.value.ToSByte(provider);
		}

		// Token: 0x06003C0E RID: 15374 RVA: 0x00162DB9 File Offset: 0x00160FB9
		public override float ToSingle(IFormatProvider provider)
		{
			return this.value.ToSingle(provider);
		}

		// Token: 0x06003C0F RID: 15375 RVA: 0x00162DC7 File Offset: 0x00160FC7
		public override string ToString(IFormatProvider provider)
		{
			return this.value.ToString(provider);
		}

		// Token: 0x06003C10 RID: 15376 RVA: 0x00162DD5 File Offset: 0x00160FD5
		public override ushort ToUInt16(IFormatProvider provider)
		{
			return this.value.ToUInt16(provider);
		}

		// Token: 0x06003C11 RID: 15377 RVA: 0x00162DE3 File Offset: 0x00160FE3
		public override uint ToUInt32(IFormatProvider provider)
		{
			return this.value.ToUInt32(provider);
		}

		// Token: 0x06003C12 RID: 15378 RVA: 0x00162DF1 File Offset: 0x00160FF1
		public override ulong ToUInt64(IFormatProvider provider)
		{
			return this.value.ToUInt64(provider);
		}
	}
}
