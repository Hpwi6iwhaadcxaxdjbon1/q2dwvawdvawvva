using System;
using System.Globalization;

namespace TinyJSON
{
	// Token: 0x020009D8 RID: 2520
	public abstract class Variant : IConvertible
	{
		// Token: 0x0400367C RID: 13948
		protected static readonly IFormatProvider FormatProvider = new NumberFormatInfo();

		// Token: 0x06003C21 RID: 15393 RVA: 0x00162EE0 File Offset: 0x001610E0
		public void Make<T>(out T item)
		{
			JSON.MakeInto<T>(this, out item);
		}

		// Token: 0x06003C22 RID: 15394 RVA: 0x00162EEC File Offset: 0x001610EC
		public T Make<T>()
		{
			T result;
			JSON.MakeInto<T>(this, out result);
			return result;
		}

		// Token: 0x06003C23 RID: 15395 RVA: 0x00162F02 File Offset: 0x00161102
		public string ToJSON()
		{
			return JSON.Dump(this);
		}

		// Token: 0x06003C24 RID: 15396 RVA: 0x0000441C File Offset: 0x0000261C
		public virtual TypeCode GetTypeCode()
		{
			return TypeCode.Object;
		}

		// Token: 0x06003C25 RID: 15397 RVA: 0x00162F0A File Offset: 0x0016110A
		public virtual object ToType(Type conversionType, IFormatProvider provider)
		{
			throw new InvalidCastException(string.Concat(new object[]
			{
				"Cannot convert ",
				base.GetType(),
				" to ",
				conversionType.Name
			}));
		}

		// Token: 0x06003C26 RID: 15398 RVA: 0x00162F3E File Offset: 0x0016113E
		public virtual DateTime ToDateTime(IFormatProvider provider)
		{
			throw new InvalidCastException("Cannot convert " + base.GetType() + " to DateTime");
		}

		// Token: 0x06003C27 RID: 15399 RVA: 0x00162F5A File Offset: 0x0016115A
		public virtual bool ToBoolean(IFormatProvider provider)
		{
			throw new InvalidCastException("Cannot convert " + base.GetType() + " to Boolean");
		}

		// Token: 0x06003C28 RID: 15400 RVA: 0x00162F76 File Offset: 0x00161176
		public virtual byte ToByte(IFormatProvider provider)
		{
			throw new InvalidCastException("Cannot convert " + base.GetType() + " to Byte");
		}

		// Token: 0x06003C29 RID: 15401 RVA: 0x00162F92 File Offset: 0x00161192
		public virtual char ToChar(IFormatProvider provider)
		{
			throw new InvalidCastException("Cannot convert " + base.GetType() + " to Char");
		}

		// Token: 0x06003C2A RID: 15402 RVA: 0x00162FAE File Offset: 0x001611AE
		public virtual decimal ToDecimal(IFormatProvider provider)
		{
			throw new InvalidCastException("Cannot convert " + base.GetType() + " to Decimal");
		}

		// Token: 0x06003C2B RID: 15403 RVA: 0x00162FCA File Offset: 0x001611CA
		public virtual double ToDouble(IFormatProvider provider)
		{
			throw new InvalidCastException("Cannot convert " + base.GetType() + " to Double");
		}

		// Token: 0x06003C2C RID: 15404 RVA: 0x00162FE6 File Offset: 0x001611E6
		public virtual short ToInt16(IFormatProvider provider)
		{
			throw new InvalidCastException("Cannot convert " + base.GetType() + " to Int16");
		}

		// Token: 0x06003C2D RID: 15405 RVA: 0x00163002 File Offset: 0x00161202
		public virtual int ToInt32(IFormatProvider provider)
		{
			throw new InvalidCastException("Cannot convert " + base.GetType() + " to Int32");
		}

		// Token: 0x06003C2E RID: 15406 RVA: 0x0016301E File Offset: 0x0016121E
		public virtual long ToInt64(IFormatProvider provider)
		{
			throw new InvalidCastException("Cannot convert " + base.GetType() + " to Int64");
		}

		// Token: 0x06003C2F RID: 15407 RVA: 0x0016303A File Offset: 0x0016123A
		public virtual sbyte ToSByte(IFormatProvider provider)
		{
			throw new InvalidCastException("Cannot convert " + base.GetType() + " to SByte");
		}

		// Token: 0x06003C30 RID: 15408 RVA: 0x00163056 File Offset: 0x00161256
		public virtual float ToSingle(IFormatProvider provider)
		{
			throw new InvalidCastException("Cannot convert " + base.GetType() + " to Single");
		}

		// Token: 0x06003C31 RID: 15409 RVA: 0x00163072 File Offset: 0x00161272
		public virtual string ToString(IFormatProvider provider)
		{
			throw new InvalidCastException("Cannot convert " + base.GetType() + " to String");
		}

		// Token: 0x06003C32 RID: 15410 RVA: 0x0016308E File Offset: 0x0016128E
		public virtual ushort ToUInt16(IFormatProvider provider)
		{
			throw new InvalidCastException("Cannot convert " + base.GetType() + " to UInt16");
		}

		// Token: 0x06003C33 RID: 15411 RVA: 0x001630AA File Offset: 0x001612AA
		public virtual uint ToUInt32(IFormatProvider provider)
		{
			throw new InvalidCastException("Cannot convert " + base.GetType() + " to UInt32");
		}

		// Token: 0x06003C34 RID: 15412 RVA: 0x001630C6 File Offset: 0x001612C6
		public virtual ulong ToUInt64(IFormatProvider provider)
		{
			throw new InvalidCastException("Cannot convert " + base.GetType() + " to UInt64");
		}

		// Token: 0x06003C35 RID: 15413 RVA: 0x001630E2 File Offset: 0x001612E2
		public override string ToString()
		{
			return this.ToString(Variant.FormatProvider);
		}

		// Token: 0x170004E4 RID: 1252
		public virtual Variant this[string key]
		{
			get
			{
				throw new NotSupportedException();
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		// Token: 0x170004E5 RID: 1253
		public virtual Variant this[int index]
		{
			get
			{
				throw new NotSupportedException();
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		// Token: 0x06003C3A RID: 15418 RVA: 0x001630F6 File Offset: 0x001612F6
		public static implicit operator bool(Variant variant)
		{
			return variant.ToBoolean(Variant.FormatProvider);
		}

		// Token: 0x06003C3B RID: 15419 RVA: 0x00163103 File Offset: 0x00161303
		public static implicit operator float(Variant variant)
		{
			return variant.ToSingle(Variant.FormatProvider);
		}

		// Token: 0x06003C3C RID: 15420 RVA: 0x00163110 File Offset: 0x00161310
		public static implicit operator double(Variant variant)
		{
			return variant.ToDouble(Variant.FormatProvider);
		}

		// Token: 0x06003C3D RID: 15421 RVA: 0x0016311D File Offset: 0x0016131D
		public static implicit operator ushort(Variant variant)
		{
			return variant.ToUInt16(Variant.FormatProvider);
		}

		// Token: 0x06003C3E RID: 15422 RVA: 0x0016312A File Offset: 0x0016132A
		public static implicit operator short(Variant variant)
		{
			return variant.ToInt16(Variant.FormatProvider);
		}

		// Token: 0x06003C3F RID: 15423 RVA: 0x00163137 File Offset: 0x00161337
		public static implicit operator uint(Variant variant)
		{
			return variant.ToUInt32(Variant.FormatProvider);
		}

		// Token: 0x06003C40 RID: 15424 RVA: 0x00163144 File Offset: 0x00161344
		public static implicit operator int(Variant variant)
		{
			return variant.ToInt32(Variant.FormatProvider);
		}

		// Token: 0x06003C41 RID: 15425 RVA: 0x00163151 File Offset: 0x00161351
		public static implicit operator ulong(Variant variant)
		{
			return variant.ToUInt64(Variant.FormatProvider);
		}

		// Token: 0x06003C42 RID: 15426 RVA: 0x0016315E File Offset: 0x0016135E
		public static implicit operator long(Variant variant)
		{
			return variant.ToInt64(Variant.FormatProvider);
		}

		// Token: 0x06003C43 RID: 15427 RVA: 0x0016316B File Offset: 0x0016136B
		public static implicit operator decimal(Variant variant)
		{
			return variant.ToDecimal(Variant.FormatProvider);
		}

		// Token: 0x06003C44 RID: 15428 RVA: 0x001630E2 File Offset: 0x001612E2
		public static implicit operator string(Variant variant)
		{
			return variant.ToString(Variant.FormatProvider);
		}

		// Token: 0x06003C45 RID: 15429 RVA: 0x00163178 File Offset: 0x00161378
		public static implicit operator Guid(Variant variant)
		{
			return new Guid(variant.ToString(Variant.FormatProvider));
		}
	}
}
