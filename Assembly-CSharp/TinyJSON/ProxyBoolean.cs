using System;

namespace TinyJSON
{
	// Token: 0x020009D4 RID: 2516
	public sealed class ProxyBoolean : Variant
	{
		// Token: 0x04003676 RID: 13942
		private readonly bool value;

		// Token: 0x06003C00 RID: 15360 RVA: 0x00162C0E File Offset: 0x00160E0E
		public ProxyBoolean(bool value)
		{
			this.value = value;
		}

		// Token: 0x06003C01 RID: 15361 RVA: 0x00162C1D File Offset: 0x00160E1D
		public override bool ToBoolean(IFormatProvider provider)
		{
			return this.value;
		}

		// Token: 0x06003C02 RID: 15362 RVA: 0x00162C25 File Offset: 0x00160E25
		public override string ToString(IFormatProvider provider)
		{
			if (!this.value)
			{
				return "false";
			}
			return "true";
		}
	}
}
