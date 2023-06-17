using System;

namespace TinyJSON
{
	// Token: 0x020009CE RID: 2510
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true)]
	public class DecodeAlias : Attribute
	{
		// Token: 0x170004DC RID: 1244
		// (get) Token: 0x06003BE2 RID: 15330 RVA: 0x00162010 File Offset: 0x00160210
		// (set) Token: 0x06003BE3 RID: 15331 RVA: 0x00162018 File Offset: 0x00160218
		public string[] Names { get; private set; }

		// Token: 0x06003BE4 RID: 15332 RVA: 0x00162021 File Offset: 0x00160221
		public DecodeAlias(params string[] names)
		{
			this.Names = names;
		}

		// Token: 0x06003BE5 RID: 15333 RVA: 0x00162030 File Offset: 0x00160230
		public bool Contains(string name)
		{
			return Array.IndexOf<string>(this.Names, name) > -1;
		}
	}
}
