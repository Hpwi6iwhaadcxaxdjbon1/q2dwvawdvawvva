using System;
using System.Linq;
using Facepunch;
using Facepunch.Models;

// Token: 0x02000321 RID: 801
public static class DeveloperList
{
	// Token: 0x06001EDC RID: 7900 RVA: 0x000D2334 File Offset: 0x000D0534
	public static bool Contains(string steamid)
	{
		return Application.Manifest != null && Application.Manifest.Administrators != null && Application.Manifest.Administrators.Any((Facepunch.Models.Manifest.Administrator x) => x.UserId == steamid);
	}

	// Token: 0x06001EDD RID: 7901 RVA: 0x000D2380 File Offset: 0x000D0580
	public static bool Contains(ulong steamid)
	{
		return DeveloperList.Contains(steamid.ToString());
	}

	// Token: 0x06001EDE RID: 7902 RVA: 0x000D238E File Offset: 0x000D058E
	public static bool IsDeveloper(BasePlayer ply)
	{
		return ply != null && DeveloperList.Contains(ply.UserIDString);
	}
}
