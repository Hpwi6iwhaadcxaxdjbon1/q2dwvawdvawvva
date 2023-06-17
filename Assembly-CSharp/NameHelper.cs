using System;

// Token: 0x02000755 RID: 1877
public static class NameHelper
{
	// Token: 0x06003478 RID: 13432 RVA: 0x00036DC0 File Offset: 0x00034FC0
	public static string Get(ulong userId, string name, bool isClient = true)
	{
		return name;
	}

	// Token: 0x06003479 RID: 13433 RVA: 0x0014512D File Offset: 0x0014332D
	public static string Get(IPlayerInfo playerInfo, bool isClient = true)
	{
		return NameHelper.Get(playerInfo.UserId, playerInfo.UserName, isClient);
	}
}
