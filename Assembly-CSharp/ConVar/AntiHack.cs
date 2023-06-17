using System;

namespace ConVar
{
	// Token: 0x02000AA4 RID: 2724
	[ConsoleSystem.Factory("antihack")]
	public class AntiHack : ConsoleSystem
	{
		// Token: 0x04003A94 RID: 14996
		[ReplicatedVar]
		[Help("collider margin when checking for noclipping on dismount")]
		public static float noclip_margin_dismount = 0.22f;

		// Token: 0x04003A95 RID: 14997
		[ReplicatedVar]
		[Help("collider backtracking when checking for noclipping")]
		public static float noclip_backtracking = 0.01f;

		// Token: 0x04003A96 RID: 14998
		[ServerVar]
		[Help("report violations to the anti cheat backend")]
		public static bool reporting = false;

		// Token: 0x04003A97 RID: 14999
		[ServerVar]
		[Help("are admins allowed to use their admin cheat")]
		public static bool admincheat = true;

		// Token: 0x04003A98 RID: 15000
		[ServerVar]
		[Help("use antihack to verify object placement by players")]
		public static bool objectplacement = true;

		// Token: 0x04003A99 RID: 15001
		[ServerVar]
		[Help("use antihack to verify model state sent by players")]
		public static bool modelstate = true;

		// Token: 0x04003A9A RID: 15002
		[ServerVar]
		[Help("whether or not to force the position on the client")]
		public static bool forceposition = true;

		// Token: 0x04003A9B RID: 15003
		[ServerVar]
		[Help("0 == users, 1 == admins, 2 == developers")]
		public static int userlevel = 2;

		// Token: 0x04003A9C RID: 15004
		[ServerVar]
		[Help("0 == no enforcement, 1 == kick, 2 == ban (DISABLED)")]
		public static int enforcementlevel = 1;

		// Token: 0x04003A9D RID: 15005
		[ServerVar]
		[Help("max allowed client desync, lower value = more false positives")]
		public static float maxdesync = 1f;

		// Token: 0x04003A9E RID: 15006
		[ServerVar]
		[Help("max allowed client tick interval delta time, lower value = more false positives")]
		public static float maxdeltatime = 1f;

		// Token: 0x04003A9F RID: 15007
		[ServerVar]
		[Help("for how many seconds to keep a tick history to use for distance checks")]
		public static float tickhistorytime = 0.5f;

		// Token: 0x04003AA0 RID: 15008
		[ServerVar]
		[Help("how much forgiveness to add when checking the distance from the player tick history")]
		public static float tickhistoryforgiveness = 0.1f;

		// Token: 0x04003AA1 RID: 15009
		[ServerVar]
		[Help("the rate at which violation values go back down")]
		public static float relaxationrate = 0.1f;

		// Token: 0x04003AA2 RID: 15010
		[ServerVar]
		[Help("the time before violation values go back down")]
		public static float relaxationpause = 10f;

		// Token: 0x04003AA3 RID: 15011
		[ServerVar]
		[Help("violation value above this results in enforcement")]
		public static float maxviolation = 100f;

		// Token: 0x04003AA4 RID: 15012
		[ServerVar]
		[Help("0 == disabled, 1 == enabled")]
		public static int terrain_protection = 1;

		// Token: 0x04003AA5 RID: 15013
		[ServerVar]
		[Help("how many slices to subdivide players into for the terrain check")]
		public static int terrain_timeslice = 64;

		// Token: 0x04003AA6 RID: 15014
		[ServerVar]
		[Help("how far to penetrate the terrain before violating")]
		public static float terrain_padding = 0.3f;

		// Token: 0x04003AA7 RID: 15015
		[ServerVar]
		[Help("violation penalty to hand out when terrain is detected")]
		public static float terrain_penalty = 100f;

		// Token: 0x04003AA8 RID: 15016
		[ServerVar]
		[Help("whether or not to kill the player when terrain is detected")]
		public static bool terrain_kill = true;

		// Token: 0x04003AA9 RID: 15017
		[ServerVar]
		[Help("whether or not to check for player inside geometry like rocks as well as base terrain")]
		public static bool terrain_check_geometry = false;

		// Token: 0x04003AAA RID: 15018
		[ServerVar]
		[Help("0 == disabled, 1 == ray, 2 == sphere, 3 == curve")]
		public static int noclip_protection = 3;

		// Token: 0x04003AAB RID: 15019
		[ServerVar]
		[Help("whether or not to reject movement when noclip is detected")]
		public static bool noclip_reject = true;

		// Token: 0x04003AAC RID: 15020
		[ServerVar]
		[Help("violation penalty to hand out when noclip is detected")]
		public static float noclip_penalty = 0f;

		// Token: 0x04003AAD RID: 15021
		[ServerVar]
		[Help("collider margin when checking for noclipping")]
		public static float noclip_margin = 0.09f;

		// Token: 0x04003AAE RID: 15022
		[ServerVar]
		[Help("movement curve step size, lower value = less false positives")]
		public static float noclip_stepsize = 0.1f;

		// Token: 0x04003AAF RID: 15023
		[ServerVar]
		[Help("movement curve max steps, lower value = more false positives")]
		public static int noclip_maxsteps = 15;

		// Token: 0x04003AB0 RID: 15024
		[ServerVar]
		[Help("0 == disabled, 1 == simple, 2 == advanced")]
		public static int speedhack_protection = 2;

		// Token: 0x04003AB1 RID: 15025
		[ServerVar]
		[Help("whether or not to reject movement when speedhack is detected")]
		public static bool speedhack_reject = true;

		// Token: 0x04003AB2 RID: 15026
		[ServerVar]
		[Help("violation penalty to hand out when speedhack is detected")]
		public static float speedhack_penalty = 0f;

		// Token: 0x04003AB3 RID: 15027
		[ServerVar]
		[Help("speed threshold to assume speedhacking, lower value = more false positives")]
		public static float speedhack_forgiveness = 2f;

		// Token: 0x04003AB4 RID: 15028
		[ServerVar]
		[Help("speed threshold to assume speedhacking, lower value = more false positives")]
		public static float speedhack_forgiveness_inertia = 10f;

		// Token: 0x04003AB5 RID: 15029
		[ServerVar]
		[Help("speed forgiveness when moving down slopes, lower value = more false positives")]
		public static float speedhack_slopespeed = 10f;

		// Token: 0x04003AB6 RID: 15030
		[ServerVar]
		[Help("0 == disabled, 1 == client, 2 == capsule, 3 == curve")]
		public static int flyhack_protection = 3;

		// Token: 0x04003AB7 RID: 15031
		[ServerVar]
		[Help("whether or not to reject movement when flyhack is detected")]
		public static bool flyhack_reject = false;

		// Token: 0x04003AB8 RID: 15032
		[ServerVar]
		[Help("violation penalty to hand out when flyhack is detected")]
		public static float flyhack_penalty = 100f;

		// Token: 0x04003AB9 RID: 15033
		[ServerVar]
		[Help("distance threshold to assume flyhacking, lower value = more false positives")]
		public static float flyhack_forgiveness_vertical = 1.5f;

		// Token: 0x04003ABA RID: 15034
		[ServerVar]
		[Help("distance threshold to assume flyhacking, lower value = more false positives")]
		public static float flyhack_forgiveness_vertical_inertia = 10f;

		// Token: 0x04003ABB RID: 15035
		[ServerVar]
		[Help("distance threshold to assume flyhacking, lower value = more false positives")]
		public static float flyhack_forgiveness_horizontal = 1.5f;

		// Token: 0x04003ABC RID: 15036
		[ServerVar]
		[Help("distance threshold to assume flyhacking, lower value = more false positives")]
		public static float flyhack_forgiveness_horizontal_inertia = 10f;

		// Token: 0x04003ABD RID: 15037
		[ServerVar]
		[Help("collider downwards extrusion when checking for flyhacking")]
		public static float flyhack_extrusion = 2f;

		// Token: 0x04003ABE RID: 15038
		[ServerVar]
		[Help("collider margin when checking for flyhacking")]
		public static float flyhack_margin = 0.05f;

		// Token: 0x04003ABF RID: 15039
		[ServerVar]
		[Help("movement curve step size, lower value = less false positives")]
		public static float flyhack_stepsize = 0.1f;

		// Token: 0x04003AC0 RID: 15040
		[ServerVar]
		[Help("movement curve max steps, lower value = more false positives")]
		public static int flyhack_maxsteps = 15;

		// Token: 0x04003AC1 RID: 15041
		[ServerVar]
		[Help("0 == disabled, 1 == speed, 2 == speed + entity, 3 == speed + entity + LOS, 4 == speed + entity + LOS + trajectory, 5 == speed + entity + LOS + trajectory + update, 6 == speed + entity + LOS + trajectory + tickhistory")]
		public static int projectile_protection = 6;

		// Token: 0x04003AC2 RID: 15042
		[ServerVar]
		[Help("violation penalty to hand out when projectile hack is detected")]
		public static float projectile_penalty = 0f;

		// Token: 0x04003AC3 RID: 15043
		[ServerVar]
		[Help("projectile speed forgiveness in percent, lower value = more false positives")]
		public static float projectile_forgiveness = 0.5f;

		// Token: 0x04003AC4 RID: 15044
		[ServerVar]
		[Help("projectile server frames to include in delay, lower value = more false positives")]
		public static float projectile_serverframes = 2f;

		// Token: 0x04003AC5 RID: 15045
		[ServerVar]
		[Help("projectile client frames to include in delay, lower value = more false positives")]
		public static float projectile_clientframes = 2f;

		// Token: 0x04003AC6 RID: 15046
		[ServerVar]
		[Help("projectile trajectory forgiveness, lower value = more false positives")]
		public static float projectile_trajectory = 1f;

		// Token: 0x04003AC7 RID: 15047
		[ServerVar]
		[Help("projectile penetration angle change, lower value = more false positives")]
		public static float projectile_anglechange = 60f;

		// Token: 0x04003AC8 RID: 15048
		[ServerVar]
		[Help("projectile penetration velocity change, lower value = more false positives")]
		public static float projectile_velocitychange = 1.1f;

		// Token: 0x04003AC9 RID: 15049
		[ServerVar]
		[Help("projectile desync forgiveness, lower value = more false positives")]
		public static float projectile_desync = 1f;

		// Token: 0x04003ACA RID: 15050
		[ServerVar]
		[Help("projectile backtracking when checking for LOS")]
		public static float projectile_backtracking = 0.01f;

		// Token: 0x04003ACB RID: 15051
		[ServerVar]
		[Help("line of sight directional forgiveness when checking eye or center position")]
		public static float projectile_losforgiveness = 0.2f;

		// Token: 0x04003ACC RID: 15052
		[ServerVar]
		[Help("how often a projectile is allowed to penetrate something before its damage is ignored")]
		public static int projectile_damagedepth = 2;

		// Token: 0x04003ACD RID: 15053
		[ServerVar]
		[Help("how often a projectile is allowed to penetrate something before its impact spawn is ignored")]
		public static int projectile_impactspawndepth = 1;

		// Token: 0x04003ACE RID: 15054
		[ServerVar]
		[Help("whether or not to include terrain in the projectile LOS checks")]
		public static bool projectile_terraincheck = true;

		// Token: 0x04003ACF RID: 15055
		[ServerVar]
		[Help("0 == disabled, 1 == initiator, 2 == initiator + target, 3 == initiator + target + LOS, 4 == initiator + target + LOS + tickhistory")]
		public static int melee_protection = 4;

		// Token: 0x04003AD0 RID: 15056
		[ServerVar]
		[Help("violation penalty to hand out when melee hack is detected")]
		public static float melee_penalty = 0f;

		// Token: 0x04003AD1 RID: 15057
		[ServerVar]
		[Help("melee distance forgiveness in percent, lower value = more false positives")]
		public static float melee_forgiveness = 0.5f;

		// Token: 0x04003AD2 RID: 15058
		[ServerVar]
		[Help("melee server frames to include in delay, lower value = more false positives")]
		public static float melee_serverframes = 2f;

		// Token: 0x04003AD3 RID: 15059
		[ServerVar]
		[Help("melee client frames to include in delay, lower value = more false positives")]
		public static float melee_clientframes = 2f;

		// Token: 0x04003AD4 RID: 15060
		[ServerVar]
		[Help("line of sight directional forgiveness when checking eye or center position")]
		public static float melee_losforgiveness = 0.2f;

		// Token: 0x04003AD5 RID: 15061
		[ServerVar]
		[Help("whether or not to include terrain in the melee LOS checks")]
		public static bool melee_terraincheck = true;

		// Token: 0x04003AD6 RID: 15062
		[ServerVar]
		[Help("0 == disabled, 1 == distance, 2 == distance + LOS, 3 = distance + LOS + altitude, 4 = distance + LOS + altitude + noclip, 5 = distance + LOS + altitude + noclip + history")]
		public static int eye_protection = 4;

		// Token: 0x04003AD7 RID: 15063
		[ServerVar]
		[Help("violation penalty to hand out when eye hack is detected")]
		public static float eye_penalty = 0f;

		// Token: 0x04003AD8 RID: 15064
		[ServerVar]
		[Help("eye speed forgiveness in percent, lower value = more false positives")]
		public static float eye_forgiveness = 0.5f;

		// Token: 0x04003AD9 RID: 15065
		[ServerVar]
		[Help("eye server frames to include in delay, lower value = more false positives")]
		public static float eye_serverframes = 2f;

		// Token: 0x04003ADA RID: 15066
		[ServerVar]
		[Help("eye client frames to include in delay, lower value = more false positives")]
		public static float eye_clientframes = 2f;

		// Token: 0x04003ADB RID: 15067
		[ServerVar]
		[Help("whether or not to include terrain in the eye LOS checks")]
		public static bool eye_terraincheck = true;

		// Token: 0x04003ADC RID: 15068
		[ServerVar]
		[Help("distance at which to start testing eye noclipping")]
		public static float eye_noclip_cutoff = 0.06f;

		// Token: 0x04003ADD RID: 15069
		[ServerVar]
		[Help("collider margin when checking for noclipping")]
		public static float eye_noclip_margin = 0.21f;

		// Token: 0x04003ADE RID: 15070
		[ServerVar]
		[Help("collider backtracking when checking for noclipping")]
		public static float eye_noclip_backtracking = 0.01f;

		// Token: 0x04003ADF RID: 15071
		[ServerVar]
		[Help("line of sight sphere cast radius, 0 == raycast")]
		public static float eye_losradius = 0.18f;

		// Token: 0x04003AE0 RID: 15072
		[ServerVar]
		[Help("violation penalty to hand out when eye history mismatch is detected")]
		public static float eye_history_penalty = 100f;

		// Token: 0x04003AE1 RID: 15073
		[ServerVar]
		[Help("how much forgiveness to add when checking the distance between player tick history and player eye history")]
		public static float eye_history_forgiveness = 0.1f;

		// Token: 0x04003AE2 RID: 15074
		[ServerVar]
		[Help("line of sight sphere cast radius, 0 == raycast")]
		public static float build_losradius = 0.01f;

		// Token: 0x04003AE3 RID: 15075
		[ServerVar]
		[Help("line of sight sphere cast radius, 0 == raycast")]
		public static float build_losradius_sleepingbag = 0.3f;

		// Token: 0x04003AE4 RID: 15076
		[ServerVar]
		[Help("whether or not to include terrain in the build LOS checks")]
		public static bool build_terraincheck = true;

		// Token: 0x04003AE5 RID: 15077
		[ServerVar]
		[Help("whether or not to check for building being done on the wrong side of something (e.g. inside rocks). 0 = Disabled, 1 = Info only, 2 = Enabled")]
		public static int build_inside_check = 2;

		// Token: 0x04003AE6 RID: 15078
		[ServerVar]
		[Help("0 == silent, 1 == print max violation, 2 == print nonzero violation, 3 == print any violation except noclip, 4 == print any violation")]
		public static int debuglevel = 1;
	}
}
