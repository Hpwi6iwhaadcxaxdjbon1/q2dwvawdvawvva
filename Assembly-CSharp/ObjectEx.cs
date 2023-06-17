using System;

// Token: 0x02000922 RID: 2338
public static class ObjectEx
{
	// Token: 0x0600384A RID: 14410 RVA: 0x00150032 File Offset: 0x0014E232
	public static bool IsUnityNull<T>(this T obj) where T : class
	{
		return obj == null || obj.Equals(null);
	}
}
