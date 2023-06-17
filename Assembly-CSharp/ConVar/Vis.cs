using System;

namespace ConVar
{
	// Token: 0x02000AE5 RID: 2789
	[ConsoleSystem.Factory("vis")]
	public class Vis : ConsoleSystem
	{
		// Token: 0x04003C38 RID: 15416
		[ClientVar]
		[Help("Turns on debug display of lerp")]
		public static bool lerp;

		// Token: 0x04003C39 RID: 15417
		[ServerVar]
		[Help("Turns on debug display of damages")]
		public static bool damage;

		// Token: 0x04003C3A RID: 15418
		[ServerVar]
		[ClientVar]
		[Help("Turns on debug display of attacks")]
		public static bool attack;

		// Token: 0x04003C3B RID: 15419
		[ServerVar]
		[ClientVar]
		[Help("Turns on debug display of protection")]
		public static bool protection;

		// Token: 0x04003C3C RID: 15420
		[ServerVar]
		[Help("Turns on debug display of weakspots")]
		public static bool weakspots;

		// Token: 0x04003C3D RID: 15421
		[ServerVar]
		[Help("Show trigger entries")]
		public static bool triggers;

		// Token: 0x04003C3E RID: 15422
		[ServerVar]
		[Help("Turns on debug display of hitboxes")]
		public static bool hitboxes;

		// Token: 0x04003C3F RID: 15423
		[ServerVar]
		[Help("Turns on debug display of line of sight checks")]
		public static bool lineofsight;

		// Token: 0x04003C40 RID: 15424
		[ServerVar]
		[Help("Turns on debug display of senses, which are received by Ai")]
		public static bool sense;
	}
}
