using System;

namespace ConVar
{
	// Token: 0x02000AC1 RID: 2753
	[ConsoleSystem.Factory("harmony")]
	public class Harmony : ConsoleSystem
	{
		// Token: 0x0600423E RID: 16958 RVA: 0x00188335 File Offset: 0x00186535
		[ServerVar(Name = "load")]
		public static void Load(ConsoleSystem.Arg args)
		{
			HarmonyLoader.TryLoadMod(args.GetString(0, ""));
		}

		// Token: 0x0600423F RID: 16959 RVA: 0x00188349 File Offset: 0x00186549
		[ServerVar(Name = "unload")]
		public static void Unload(ConsoleSystem.Arg args)
		{
			HarmonyLoader.TryUnloadMod(args.GetString(0, ""));
		}
	}
}
