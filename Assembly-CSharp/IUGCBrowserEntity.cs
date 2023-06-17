using System;
using System.Collections.Generic;

// Token: 0x020003DB RID: 987
public interface IUGCBrowserEntity
{
	// Token: 0x170002DA RID: 730
	// (get) Token: 0x060021F2 RID: 8690
	uint[] GetContentCRCs { get; }

	// Token: 0x060021F3 RID: 8691
	void ClearContent();

	// Token: 0x170002DB RID: 731
	// (get) Token: 0x060021F4 RID: 8692
	UGCType ContentType { get; }

	// Token: 0x170002DC RID: 732
	// (get) Token: 0x060021F5 RID: 8693
	List<ulong> EditingHistory { get; }

	// Token: 0x170002DD RID: 733
	// (get) Token: 0x060021F6 RID: 8694
	BaseNetworkable UgcEntity { get; }
}
