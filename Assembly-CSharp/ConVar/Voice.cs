using System;
using UnityEngine;

namespace ConVar
{
	// Token: 0x02000AE6 RID: 2790
	[ConsoleSystem.Factory("voice")]
	public class Voice : ConsoleSystem
	{
		// Token: 0x04003C41 RID: 15425
		[ClientVar(Saved = true)]
		public static bool loopback = false;

		// Token: 0x04003C42 RID: 15426
		private static float _voiceRangeBoostAmount = 50f;

		// Token: 0x170005E0 RID: 1504
		// (get) Token: 0x06004332 RID: 17202 RVA: 0x0018D6A4 File Offset: 0x0018B8A4
		// (set) Token: 0x06004333 RID: 17203 RVA: 0x0018D6AB File Offset: 0x0018B8AB
		[ReplicatedVar]
		public static float voiceRangeBoostAmount
		{
			get
			{
				return Voice._voiceRangeBoostAmount;
			}
			set
			{
				Voice._voiceRangeBoostAmount = Mathf.Clamp(value, 0f, 200f);
			}
		}

		// Token: 0x06004334 RID: 17204 RVA: 0x0018D6C4 File Offset: 0x0018B8C4
		[ServerVar(Help = "Enabled/disables voice range boost for a player eg. ToggleVoiceRangeBoost sam 1")]
		public static void ToggleVoiceRangeBoost(ConsoleSystem.Arg arg)
		{
			BasePlayer player = arg.GetPlayer(0);
			if (player == null)
			{
				arg.ReplyWith("Invalid player: " + arg.GetString(0, ""));
				return;
			}
			bool @bool = arg.GetBool(1, false);
			player.SetPlayerFlag(BasePlayer.PlayerFlags.VoiceRangeBoost, @bool);
			arg.ReplyWith(string.Format("Set {0} volume boost to {1}", player.displayName, @bool));
		}
	}
}
