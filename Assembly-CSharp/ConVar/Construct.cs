using System;

namespace ConVar
{
	// Token: 0x02000AAC RID: 2732
	[ConsoleSystem.Factory("construct")]
	public class Construct : ConsoleSystem
	{
		// Token: 0x04003B11 RID: 15121
		[ServerVar]
		[Help("How many minutes before a placed frame gets destroyed")]
		public static float frameminutes = 30f;
	}
}
