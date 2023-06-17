using System;
using System.IO;

namespace ConVar
{
	// Token: 0x02000AD6 RID: 2774
	[ConsoleSystem.Factory("profile")]
	public class Profile : ConsoleSystem
	{
		// Token: 0x060042B0 RID: 17072 RVA: 0x0018B66A File Offset: 0x0018986A
		private static void NeedProfileFolder()
		{
			if (!Directory.Exists("profile"))
			{
				Directory.CreateDirectory("profile");
			}
		}

		// Token: 0x060042B1 RID: 17073 RVA: 0x000063A5 File Offset: 0x000045A5
		[ClientVar]
		[ServerVar]
		public static void start(ConsoleSystem.Arg arg)
		{
		}

		// Token: 0x060042B2 RID: 17074 RVA: 0x000063A5 File Offset: 0x000045A5
		[ServerVar]
		[ClientVar]
		public static void stop(ConsoleSystem.Arg arg)
		{
		}

		// Token: 0x060042B3 RID: 17075 RVA: 0x000063A5 File Offset: 0x000045A5
		[ServerVar]
		[ClientVar]
		public static void flush_analytics(ConsoleSystem.Arg arg)
		{
		}
	}
}
