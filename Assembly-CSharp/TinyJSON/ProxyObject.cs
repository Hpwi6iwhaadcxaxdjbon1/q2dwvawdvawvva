using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;

namespace TinyJSON
{
	// Token: 0x020009D6 RID: 2518
	public sealed class ProxyObject : Variant, IEnumerable<KeyValuePair<string, Variant>>, IEnumerable
	{
		// Token: 0x04003679 RID: 13945
		public const string TypeHintKey = "@type";

		// Token: 0x0400367A RID: 13946
		private readonly Dictionary<string, Variant> dict;

		// Token: 0x06003C14 RID: 15380 RVA: 0x00162E16 File Offset: 0x00161016
		public ProxyObject()
		{
			this.dict = new Dictionary<string, Variant>();
		}

		// Token: 0x06003C15 RID: 15381 RVA: 0x00162E29 File Offset: 0x00161029
		IEnumerator<KeyValuePair<string, Variant>> IEnumerable<KeyValuePair<string, Variant>>.GetEnumerator()
		{
			return this.dict.GetEnumerator();
		}

		// Token: 0x06003C16 RID: 15382 RVA: 0x00162E29 File Offset: 0x00161029
		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.dict.GetEnumerator();
		}

		// Token: 0x06003C17 RID: 15383 RVA: 0x00162E3B File Offset: 0x0016103B
		public void Add(string key, Variant item)
		{
			this.dict.Add(key, item);
		}

		// Token: 0x06003C18 RID: 15384 RVA: 0x00162E4A File Offset: 0x0016104A
		public bool TryGetValue(string key, out Variant item)
		{
			return this.dict.TryGetValue(key, out item);
		}

		// Token: 0x170004DF RID: 1247
		// (get) Token: 0x06003C19 RID: 15385 RVA: 0x00162E5C File Offset: 0x0016105C
		public string TypeHint
		{
			get
			{
				Variant variant;
				if (this.TryGetValue("@type", out variant))
				{
					return variant.ToString(CultureInfo.InvariantCulture);
				}
				return null;
			}
		}

		// Token: 0x170004E0 RID: 1248
		public override Variant this[string key]
		{
			get
			{
				return this.dict[key];
			}
			set
			{
				this.dict[key] = value;
			}
		}

		// Token: 0x170004E1 RID: 1249
		// (get) Token: 0x06003C1C RID: 15388 RVA: 0x00162EA2 File Offset: 0x001610A2
		public int Count
		{
			get
			{
				return this.dict.Count;
			}
		}

		// Token: 0x170004E2 RID: 1250
		// (get) Token: 0x06003C1D RID: 15389 RVA: 0x00162EAF File Offset: 0x001610AF
		public Dictionary<string, Variant>.KeyCollection Keys
		{
			get
			{
				return this.dict.Keys;
			}
		}

		// Token: 0x170004E3 RID: 1251
		// (get) Token: 0x06003C1E RID: 15390 RVA: 0x00162EBC File Offset: 0x001610BC
		public Dictionary<string, Variant>.ValueCollection Values
		{
			get
			{
				return this.dict.Values;
			}
		}
	}
}
