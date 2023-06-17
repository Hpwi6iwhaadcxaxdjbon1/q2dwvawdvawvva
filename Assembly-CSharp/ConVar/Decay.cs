using System;

namespace ConVar
{
	// Token: 0x02000AB2 RID: 2738
	[ConsoleSystem.Factory("decay")]
	public class Decay : ConsoleSystem
	{
		// Token: 0x04003B18 RID: 15128
		[ServerVar(Help = "Maximum distance to test to see if a structure is outside, higher values are slower but accurate for huge buildings")]
		public static float outside_test_range = 50f;

		// Token: 0x04003B19 RID: 15129
		[ServerVar]
		public static float tick = 600f;

		// Token: 0x04003B1A RID: 15130
		[ServerVar]
		public static float scale = 1f;

		// Token: 0x04003B1B RID: 15131
		[ServerVar]
		public static bool debug = false;

		// Token: 0x04003B1C RID: 15132
		[ServerVar(Help = "Is upkeep enabled")]
		public static bool upkeep = true;

		// Token: 0x04003B1D RID: 15133
		[ServerVar(Help = "How many minutes does the upkeep cost last? default : 1440 (24 hours)")]
		public static float upkeep_period_minutes = 1440f;

		// Token: 0x04003B1E RID: 15134
		[ServerVar(Help = "How many minutes can the upkeep cost last after the cupboard was destroyed? default : 1440 (24 hours)")]
		public static float upkeep_grief_protection = 1440f;

		// Token: 0x04003B1F RID: 15135
		[ServerVar(Help = "Scale at which objects heal when upkeep conditions are met, default of 1 is same rate at which they decay")]
		public static float upkeep_heal_scale = 1f;

		// Token: 0x04003B20 RID: 15136
		[ServerVar(Help = "Scale at which objects decay when they are inside, default of 0.1")]
		public static float upkeep_inside_decay_scale = 0.1f;

		// Token: 0x04003B21 RID: 15137
		[ServerVar(Help = "When set to a value above 0 everything will decay with this delay")]
		public static float delay_override = 0f;

		// Token: 0x04003B22 RID: 15138
		[ServerVar(Help = "How long should this building grade decay be delayed when not protected by upkeep, in hours")]
		public static float delay_twig = 0f;

		// Token: 0x04003B23 RID: 15139
		[ServerVar(Help = "How long should this building grade decay be delayed when not protected by upkeep, in hours")]
		public static float delay_wood = 0f;

		// Token: 0x04003B24 RID: 15140
		[ServerVar(Help = "How long should this building grade decay be delayed when not protected by upkeep, in hours")]
		public static float delay_stone = 0f;

		// Token: 0x04003B25 RID: 15141
		[ServerVar(Help = "How long should this building grade decay be delayed when not protected by upkeep, in hours")]
		public static float delay_metal = 0f;

		// Token: 0x04003B26 RID: 15142
		[ServerVar(Help = "How long should this building grade decay be delayed when not protected by upkeep, in hours")]
		public static float delay_toptier = 0f;

		// Token: 0x04003B27 RID: 15143
		[ServerVar(Help = "When set to a value above 0 everything will decay with this duration")]
		public static float duration_override = 0f;

		// Token: 0x04003B28 RID: 15144
		[ServerVar(Help = "How long should this building grade take to decay when not protected by upkeep, in hours")]
		public static float duration_twig = 1f;

		// Token: 0x04003B29 RID: 15145
		[ServerVar(Help = "How long should this building grade take to decay when not protected by upkeep, in hours")]
		public static float duration_wood = 3f;

		// Token: 0x04003B2A RID: 15146
		[ServerVar(Help = "How long should this building grade take to decay when not protected by upkeep, in hours")]
		public static float duration_stone = 5f;

		// Token: 0x04003B2B RID: 15147
		[ServerVar(Help = "How long should this building grade take to decay when not protected by upkeep, in hours")]
		public static float duration_metal = 8f;

		// Token: 0x04003B2C RID: 15148
		[ServerVar(Help = "How long should this building grade take to decay when not protected by upkeep, in hours")]
		public static float duration_toptier = 12f;

		// Token: 0x04003B2D RID: 15149
		[ServerVar(Help = "Between 0 and this value are considered bracket 0 and will cost bracket_0_costfraction per upkeep period to maintain")]
		public static int bracket_0_blockcount = 15;

		// Token: 0x04003B2E RID: 15150
		[ServerVar(Help = "blocks within bracket 0 will cost this fraction per upkeep period to maintain")]
		public static float bracket_0_costfraction = 0.1f;

		// Token: 0x04003B2F RID: 15151
		[ServerVar(Help = "Between bracket_0_blockcount and this value are considered bracket 1 and will cost bracket_1_costfraction per upkeep period to maintain")]
		public static int bracket_1_blockcount = 50;

		// Token: 0x04003B30 RID: 15152
		[ServerVar(Help = "blocks within bracket 1 will cost this fraction per upkeep period to maintain")]
		public static float bracket_1_costfraction = 0.15f;

		// Token: 0x04003B31 RID: 15153
		[ServerVar(Help = "Between bracket_1_blockcount and this value are considered bracket 2 and will cost bracket_2_costfraction per upkeep period to maintain")]
		public static int bracket_2_blockcount = 125;

		// Token: 0x04003B32 RID: 15154
		[ServerVar(Help = "blocks within bracket 2 will cost this fraction per upkeep period to maintain")]
		public static float bracket_2_costfraction = 0.2f;

		// Token: 0x04003B33 RID: 15155
		[ServerVar(Help = "Between bracket_2_blockcount and this value (and beyond) are considered bracket 3 and will cost bracket_3_costfraction per upkeep period to maintain")]
		public static int bracket_3_blockcount = 200;

		// Token: 0x04003B34 RID: 15156
		[ServerVar(Help = "blocks within bracket 3 will cost this fraction per upkeep period to maintain")]
		public static float bracket_3_costfraction = 0.333f;
	}
}
