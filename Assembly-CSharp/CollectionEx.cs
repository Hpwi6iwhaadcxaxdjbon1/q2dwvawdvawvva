using System;
using System.Collections.Generic;

// Token: 0x0200091D RID: 2333
public static class CollectionEx
{
	// Token: 0x0600383E RID: 14398 RVA: 0x0014F8FD File Offset: 0x0014DAFD
	public static bool IsNullOrEmpty<T>(this ICollection<T> collection)
	{
		return collection == null || collection.Count == 0;
	}

	// Token: 0x0600383F RID: 14399 RVA: 0x0014F90D File Offset: 0x0014DB0D
	public static bool IsEmpty<T>(this ICollection<T> collection)
	{
		return collection.Count == 0;
	}
}
