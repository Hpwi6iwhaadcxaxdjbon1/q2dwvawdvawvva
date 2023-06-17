using System;

namespace TinyJSON
{
	// Token: 0x020009D7 RID: 2519
	public sealed class ProxyString : Variant
	{
		// Token: 0x0400367B RID: 13947
		private readonly string value;

		// Token: 0x06003C1F RID: 15391 RVA: 0x00162EC9 File Offset: 0x001610C9
		public ProxyString(string value)
		{
			this.value = value;
		}

		// Token: 0x06003C20 RID: 15392 RVA: 0x00162ED8 File Offset: 0x001610D8
		public override string ToString(IFormatProvider provider)
		{
			return this.value;
		}
	}
}
