using System;

// Token: 0x020003BA RID: 954
public static class BaseNetworkableEx
{
	// Token: 0x0600216E RID: 8558 RVA: 0x000DA7A2 File Offset: 0x000D89A2
	public static bool IsValid(this BaseNetworkable ent)
	{
		return !(ent == null) && ent.net != null;
	}

	// Token: 0x0600216F RID: 8559 RVA: 0x000DA7BA File Offset: 0x000D89BA
	public static bool IsRealNull(this BaseNetworkable ent)
	{
		return ent == null;
	}
}
