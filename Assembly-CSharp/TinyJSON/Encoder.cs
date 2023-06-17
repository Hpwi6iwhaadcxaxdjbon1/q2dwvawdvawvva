using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Text;

namespace TinyJSON
{
	// Token: 0x020009C7 RID: 2503
	public sealed class Encoder
	{
		// Token: 0x04003663 RID: 13923
		private static readonly Type includeAttrType = typeof(Include);

		// Token: 0x04003664 RID: 13924
		private static readonly Type excludeAttrType = typeof(Exclude);

		// Token: 0x04003665 RID: 13925
		private static readonly Type typeHintAttrType = typeof(TypeHint);

		// Token: 0x04003666 RID: 13926
		private readonly StringBuilder builder;

		// Token: 0x04003667 RID: 13927
		private readonly EncodeOptions options;

		// Token: 0x04003668 RID: 13928
		private int indent;

		// Token: 0x06003BC2 RID: 15298 RVA: 0x001614AA File Offset: 0x0015F6AA
		private Encoder(EncodeOptions options)
		{
			this.options = options;
			this.builder = new StringBuilder();
			this.indent = 0;
		}

		// Token: 0x06003BC3 RID: 15299 RVA: 0x001614CB File Offset: 0x0015F6CB
		public static string Encode(object obj)
		{
			return Encoder.Encode(obj, EncodeOptions.None);
		}

		// Token: 0x06003BC4 RID: 15300 RVA: 0x001614D4 File Offset: 0x0015F6D4
		public static string Encode(object obj, EncodeOptions options)
		{
			Encoder encoder = new Encoder(options);
			encoder.EncodeValue(obj, false);
			return encoder.builder.ToString();
		}

		// Token: 0x170004D8 RID: 1240
		// (get) Token: 0x06003BC5 RID: 15301 RVA: 0x001614EE File Offset: 0x0015F6EE
		private bool PrettyPrintEnabled
		{
			get
			{
				return (this.options & EncodeOptions.PrettyPrint) == EncodeOptions.PrettyPrint;
			}
		}

		// Token: 0x170004D9 RID: 1241
		// (get) Token: 0x06003BC6 RID: 15302 RVA: 0x001614FB File Offset: 0x0015F6FB
		private bool TypeHintsEnabled
		{
			get
			{
				return (this.options & EncodeOptions.NoTypeHints) != EncodeOptions.NoTypeHints;
			}
		}

		// Token: 0x170004DA RID: 1242
		// (get) Token: 0x06003BC7 RID: 15303 RVA: 0x0016150B File Offset: 0x0015F70B
		private bool IncludePublicPropertiesEnabled
		{
			get
			{
				return (this.options & EncodeOptions.IncludePublicProperties) == EncodeOptions.IncludePublicProperties;
			}
		}

		// Token: 0x170004DB RID: 1243
		// (get) Token: 0x06003BC8 RID: 15304 RVA: 0x00161518 File Offset: 0x0015F718
		private bool EnforceHierarchyOrderEnabled
		{
			get
			{
				return (this.options & EncodeOptions.EnforceHierarchyOrder) == EncodeOptions.EnforceHierarchyOrder;
			}
		}

		// Token: 0x06003BC9 RID: 15305 RVA: 0x00161528 File Offset: 0x0015F728
		private void EncodeValue(object value, bool forceTypeHint)
		{
			if (value == null)
			{
				this.builder.Append("null");
				return;
			}
			if (value is string)
			{
				this.EncodeString((string)value);
				return;
			}
			if (value is ProxyString)
			{
				this.EncodeString(((ProxyString)value).ToString(CultureInfo.InvariantCulture));
				return;
			}
			if (value is char)
			{
				this.EncodeString(value.ToString());
				return;
			}
			if (value is bool)
			{
				this.builder.Append(((bool)value) ? "true" : "false");
				return;
			}
			if (value is Enum)
			{
				this.EncodeString(value.ToString());
				return;
			}
			if (value is Array)
			{
				this.EncodeArray((Array)value, forceTypeHint);
				return;
			}
			if (value is IList)
			{
				this.EncodeList((IList)value, forceTypeHint);
				return;
			}
			if (value is IDictionary)
			{
				this.EncodeDictionary((IDictionary)value, forceTypeHint);
				return;
			}
			if (value is Guid)
			{
				this.EncodeString(value.ToString());
				return;
			}
			if (value is ProxyArray)
			{
				this.EncodeProxyArray((ProxyArray)value);
				return;
			}
			if (value is ProxyObject)
			{
				this.EncodeProxyObject((ProxyObject)value);
				return;
			}
			if (value is float || value is double || value is int || value is uint || value is long || value is sbyte || value is byte || value is short || value is ushort || value is ulong || value is decimal || value is ProxyBoolean || value is ProxyNumber)
			{
				this.builder.Append(Convert.ToString(value, CultureInfo.InvariantCulture));
				return;
			}
			this.EncodeObject(value, forceTypeHint);
		}

		// Token: 0x06003BCA RID: 15306 RVA: 0x001616DC File Offset: 0x0015F8DC
		private IEnumerable<FieldInfo> GetFieldsForType(Type type)
		{
			if (this.EnforceHierarchyOrderEnabled)
			{
				Stack<Type> stack = new Stack<Type>();
				while (type != null)
				{
					stack.Push(type);
					type = type.BaseType;
				}
				List<FieldInfo> list = new List<FieldInfo>();
				while (stack.Count > 0)
				{
					list.AddRange(stack.Pop().GetFields(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic));
				}
				return list;
			}
			return type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
		}

		// Token: 0x06003BCB RID: 15307 RVA: 0x00161740 File Offset: 0x0015F940
		private IEnumerable<PropertyInfo> GetPropertiesForType(Type type)
		{
			if (this.EnforceHierarchyOrderEnabled)
			{
				Stack<Type> stack = new Stack<Type>();
				while (type != null)
				{
					stack.Push(type);
					type = type.BaseType;
				}
				List<PropertyInfo> list = new List<PropertyInfo>();
				while (stack.Count > 0)
				{
					list.AddRange(stack.Pop().GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic));
				}
				return list;
			}
			return type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
		}

		// Token: 0x06003BCC RID: 15308 RVA: 0x001617A4 File Offset: 0x0015F9A4
		private void EncodeObject(object value, bool forceTypeHint)
		{
			Type type = value.GetType();
			this.AppendOpenBrace();
			forceTypeHint = (forceTypeHint || this.TypeHintsEnabled);
			bool includePublicPropertiesEnabled = this.IncludePublicPropertiesEnabled;
			bool firstItem = !forceTypeHint;
			if (forceTypeHint)
			{
				if (this.PrettyPrintEnabled)
				{
					this.AppendIndent();
				}
				this.EncodeString("@type");
				this.AppendColon();
				this.EncodeString(type.FullName);
				firstItem = false;
			}
			foreach (FieldInfo fieldInfo in this.GetFieldsForType(type))
			{
				bool forceTypeHint2 = false;
				bool flag = fieldInfo.IsPublic;
				foreach (object o in fieldInfo.GetCustomAttributes(true))
				{
					if (Encoder.excludeAttrType.IsInstanceOfType(o))
					{
						flag = false;
					}
					if (Encoder.includeAttrType.IsInstanceOfType(o))
					{
						flag = true;
					}
					if (Encoder.typeHintAttrType.IsInstanceOfType(o))
					{
						forceTypeHint2 = true;
					}
				}
				if (flag)
				{
					this.AppendComma(firstItem);
					this.EncodeString(fieldInfo.Name);
					this.AppendColon();
					this.EncodeValue(fieldInfo.GetValue(value), forceTypeHint2);
					firstItem = false;
				}
			}
			foreach (PropertyInfo propertyInfo in this.GetPropertiesForType(type))
			{
				if (propertyInfo.CanRead)
				{
					bool forceTypeHint3 = false;
					bool flag2 = includePublicPropertiesEnabled;
					foreach (object o2 in propertyInfo.GetCustomAttributes(true))
					{
						if (Encoder.excludeAttrType.IsInstanceOfType(o2))
						{
							flag2 = false;
						}
						if (Encoder.includeAttrType.IsInstanceOfType(o2))
						{
							flag2 = true;
						}
						if (Encoder.typeHintAttrType.IsInstanceOfType(o2))
						{
							forceTypeHint3 = true;
						}
					}
					if (flag2)
					{
						this.AppendComma(firstItem);
						this.EncodeString(propertyInfo.Name);
						this.AppendColon();
						this.EncodeValue(propertyInfo.GetValue(value, null), forceTypeHint3);
						firstItem = false;
					}
				}
			}
			this.AppendCloseBrace();
		}

		// Token: 0x06003BCD RID: 15309 RVA: 0x001619C0 File Offset: 0x0015FBC0
		private void EncodeProxyArray(ProxyArray value)
		{
			if (value.Count == 0)
			{
				this.builder.Append("[]");
				return;
			}
			this.AppendOpenBracket();
			bool firstItem = true;
			foreach (Variant value2 in ((IEnumerable<Variant>)value))
			{
				this.AppendComma(firstItem);
				this.EncodeValue(value2, false);
				firstItem = false;
			}
			this.AppendCloseBracket();
		}

		// Token: 0x06003BCE RID: 15310 RVA: 0x00161A3C File Offset: 0x0015FC3C
		private void EncodeProxyObject(ProxyObject value)
		{
			if (value.Count == 0)
			{
				this.builder.Append("{}");
				return;
			}
			this.AppendOpenBrace();
			bool firstItem = true;
			foreach (string text in value.Keys)
			{
				this.AppendComma(firstItem);
				this.EncodeString(text);
				this.AppendColon();
				this.EncodeValue(value[text], false);
				firstItem = false;
			}
			this.AppendCloseBrace();
		}

		// Token: 0x06003BCF RID: 15311 RVA: 0x00161AD4 File Offset: 0x0015FCD4
		private void EncodeDictionary(IDictionary value, bool forceTypeHint)
		{
			if (value.Count == 0)
			{
				this.builder.Append("{}");
				return;
			}
			this.AppendOpenBrace();
			bool firstItem = true;
			foreach (object obj in value.Keys)
			{
				this.AppendComma(firstItem);
				this.EncodeString(obj.ToString());
				this.AppendColon();
				this.EncodeValue(value[obj], forceTypeHint);
				firstItem = false;
			}
			this.AppendCloseBrace();
		}

		// Token: 0x06003BD0 RID: 15312 RVA: 0x00161B74 File Offset: 0x0015FD74
		private void EncodeList(IList value, bool forceTypeHint)
		{
			if (value.Count == 0)
			{
				this.builder.Append("[]");
				return;
			}
			this.AppendOpenBracket();
			bool firstItem = true;
			foreach (object value2 in value)
			{
				this.AppendComma(firstItem);
				this.EncodeValue(value2, forceTypeHint);
				firstItem = false;
			}
			this.AppendCloseBracket();
		}

		// Token: 0x06003BD1 RID: 15313 RVA: 0x00161BF8 File Offset: 0x0015FDF8
		private void EncodeArray(Array value, bool forceTypeHint)
		{
			if (value.Rank == 1)
			{
				this.EncodeList(value, forceTypeHint);
				return;
			}
			int[] indices = new int[value.Rank];
			this.EncodeArrayRank(value, 0, indices, forceTypeHint);
		}

		// Token: 0x06003BD2 RID: 15314 RVA: 0x00161C30 File Offset: 0x0015FE30
		private void EncodeArrayRank(Array value, int rank, int[] indices, bool forceTypeHint)
		{
			this.AppendOpenBracket();
			int lowerBound = value.GetLowerBound(rank);
			int upperBound = value.GetUpperBound(rank);
			if (rank == value.Rank - 1)
			{
				for (int i = lowerBound; i <= upperBound; i++)
				{
					indices[rank] = i;
					this.AppendComma(i == lowerBound);
					this.EncodeValue(value.GetValue(indices), forceTypeHint);
				}
			}
			else
			{
				for (int j = lowerBound; j <= upperBound; j++)
				{
					indices[rank] = j;
					this.AppendComma(j == lowerBound);
					this.EncodeArrayRank(value, rank + 1, indices, forceTypeHint);
				}
			}
			this.AppendCloseBracket();
		}

		// Token: 0x06003BD3 RID: 15315 RVA: 0x00161CB8 File Offset: 0x0015FEB8
		private void EncodeString(string value)
		{
			this.builder.Append('"');
			char[] array = value.ToCharArray();
			int i = 0;
			while (i < array.Length)
			{
				char c = array[i];
				switch (c)
				{
				case '\b':
					this.builder.Append("\\b");
					break;
				case '\t':
					this.builder.Append("\\t");
					break;
				case '\n':
					this.builder.Append("\\n");
					break;
				case '\v':
					goto IL_DD;
				case '\f':
					this.builder.Append("\\f");
					break;
				case '\r':
					this.builder.Append("\\r");
					break;
				default:
					if (c != '"')
					{
						if (c != '\\')
						{
							goto IL_DD;
						}
						this.builder.Append("\\\\");
					}
					else
					{
						this.builder.Append("\\\"");
					}
					break;
				}
				IL_123:
				i++;
				continue;
				IL_DD:
				int num = Convert.ToInt32(c);
				if (num >= 32 && num <= 126)
				{
					this.builder.Append(c);
					goto IL_123;
				}
				this.builder.Append("\\u" + Convert.ToString(num, 16).PadLeft(4, '0'));
				goto IL_123;
			}
			this.builder.Append('"');
		}

		// Token: 0x06003BD4 RID: 15316 RVA: 0x00161E04 File Offset: 0x00160004
		private void AppendIndent()
		{
			for (int i = 0; i < this.indent; i++)
			{
				this.builder.Append('\t');
			}
		}

		// Token: 0x06003BD5 RID: 15317 RVA: 0x00161E30 File Offset: 0x00160030
		private void AppendOpenBrace()
		{
			this.builder.Append('{');
			if (this.PrettyPrintEnabled)
			{
				this.builder.Append('\n');
				this.indent++;
			}
		}

		// Token: 0x06003BD6 RID: 15318 RVA: 0x00161E64 File Offset: 0x00160064
		private void AppendCloseBrace()
		{
			if (this.PrettyPrintEnabled)
			{
				this.builder.Append('\n');
				this.indent--;
				this.AppendIndent();
			}
			this.builder.Append('}');
		}

		// Token: 0x06003BD7 RID: 15319 RVA: 0x00161E9E File Offset: 0x0016009E
		private void AppendOpenBracket()
		{
			this.builder.Append('[');
			if (this.PrettyPrintEnabled)
			{
				this.builder.Append('\n');
				this.indent++;
			}
		}

		// Token: 0x06003BD8 RID: 15320 RVA: 0x00161ED2 File Offset: 0x001600D2
		private void AppendCloseBracket()
		{
			if (this.PrettyPrintEnabled)
			{
				this.builder.Append('\n');
				this.indent--;
				this.AppendIndent();
			}
			this.builder.Append(']');
		}

		// Token: 0x06003BD9 RID: 15321 RVA: 0x00161F0C File Offset: 0x0016010C
		private void AppendComma(bool firstItem)
		{
			if (!firstItem)
			{
				this.builder.Append(',');
				if (this.PrettyPrintEnabled)
				{
					this.builder.Append('\n');
				}
			}
			if (this.PrettyPrintEnabled)
			{
				this.AppendIndent();
			}
		}

		// Token: 0x06003BDA RID: 15322 RVA: 0x00161F43 File Offset: 0x00160143
		private void AppendColon()
		{
			this.builder.Append(':');
			if (this.PrettyPrintEnabled)
			{
				this.builder.Append(' ');
			}
		}
	}
}
