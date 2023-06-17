using System;

namespace ConVar
{
	// Token: 0x02000ADD RID: 2781
	public class Steam
	{
		// Token: 0x170005DB RID: 1499
		// (get) Token: 0x0600430B RID: 17163 RVA: 0x0018CD5E File Offset: 0x0018AF5E
		// (set) Token: 0x0600430C RID: 17164 RVA: 0x0018CD65 File Offset: 0x0018AF65
		[ReplicatedVar(Saved = true, ShowInAdminUI = true)]
		public static bool server_allow_steam_nicknames { get; set; } = true;
	}
}
