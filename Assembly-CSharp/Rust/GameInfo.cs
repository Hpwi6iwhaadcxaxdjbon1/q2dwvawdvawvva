using System;
using ConVar;
using UnityEngine;

namespace Rust
{
	// Token: 0x02000B09 RID: 2825
	internal static class GameInfo
	{
		// Token: 0x1700063A RID: 1594
		// (get) Token: 0x060044FC RID: 17660 RVA: 0x00194D8B File Offset: 0x00192F8B
		internal static bool IsOfficialServer
		{
			get
			{
				return Application.isEditor || Server.official;
			}
		}

		// Token: 0x1700063B RID: 1595
		// (get) Token: 0x060044FD RID: 17661 RVA: 0x00194D9B File Offset: 0x00192F9B
		internal static bool HasAchievements
		{
			get
			{
				return GameInfo.IsOfficialServer;
			}
		}
	}
}
