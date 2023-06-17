using System;

namespace ConVar
{
	// Token: 0x02000AB9 RID: 2745
	[ConsoleSystem.Factory("file")]
	public class FileConVar : ConsoleSystem
	{
		// Token: 0x170005A8 RID: 1448
		// (get) Token: 0x060041CE RID: 16846 RVA: 0x001868A3 File Offset: 0x00184AA3
		// (set) Token: 0x060041CF RID: 16847 RVA: 0x001868AA File Offset: 0x00184AAA
		[ClientVar]
		public static bool debug
		{
			get
			{
				return FileSystem.LogDebug;
			}
			set
			{
				FileSystem.LogDebug = value;
			}
		}

		// Token: 0x170005A9 RID: 1449
		// (get) Token: 0x060041D0 RID: 16848 RVA: 0x001868B2 File Offset: 0x00184AB2
		// (set) Token: 0x060041D1 RID: 16849 RVA: 0x001868B9 File Offset: 0x00184AB9
		[ClientVar]
		public static bool time
		{
			get
			{
				return FileSystem.LogTime;
			}
			set
			{
				FileSystem.LogTime = value;
			}
		}
	}
}
