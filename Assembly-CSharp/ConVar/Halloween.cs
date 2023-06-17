using System;

namespace ConVar
{
	// Token: 0x02000AC0 RID: 2752
	[ConsoleSystem.Factory("halloween")]
	public class Halloween : ConsoleSystem
	{
		// Token: 0x04003B70 RID: 15216
		[ServerVar]
		public static bool enabled = false;

		// Token: 0x04003B71 RID: 15217
		[ServerVar(Help = "Population active on the server, per square km")]
		public static float murdererpopulation = 0f;

		// Token: 0x04003B72 RID: 15218
		[ServerVar(Help = "Population active on the server, per square km")]
		public static float scarecrowpopulation = 0f;

		// Token: 0x04003B73 RID: 15219
		[ServerVar(Help = "Scarecrows can throw beancans (Default: true).")]
		public static bool scarecrows_throw_beancans = true;

		// Token: 0x04003B74 RID: 15220
		[ServerVar(Help = "The delay globally on a server between each time a scarecrow throws a beancan (Default: 8 seconds).")]
		public static float scarecrow_throw_beancan_global_delay = 8f;

		// Token: 0x04003B75 RID: 15221
		[ServerVar(Help = "Modified damage from beancan explosion vs players (Default: 0.1).")]
		public static float scarecrow_beancan_vs_player_dmg_modifier = 0.1f;

		// Token: 0x04003B76 RID: 15222
		[ServerVar(Help = "Modifier to how much damage scarecrows take to the body. (Default: 0.25)")]
		public static float scarecrow_body_dmg_modifier = 0.25f;

		// Token: 0x04003B77 RID: 15223
		[ServerVar(Help = "Stopping distance for destinations set while chasing a target (Default: 0.5)")]
		public static float scarecrow_chase_stopping_distance = 0.5f;
	}
}
