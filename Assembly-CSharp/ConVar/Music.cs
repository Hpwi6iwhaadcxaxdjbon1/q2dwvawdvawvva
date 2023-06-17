using System;
using System.Text;
using UnityEngine;

namespace ConVar
{
	// Token: 0x02000ACD RID: 2765
	[ConsoleSystem.Factory("music")]
	public class Music : ConsoleSystem
	{
		// Token: 0x04003B81 RID: 15233
		[ClientVar]
		public static bool enabled = true;

		// Token: 0x04003B82 RID: 15234
		[ClientVar]
		public static int songGapMin = 240;

		// Token: 0x04003B83 RID: 15235
		[ClientVar]
		public static int songGapMax = 480;

		// Token: 0x06004275 RID: 17013 RVA: 0x00189EE4 File Offset: 0x001880E4
		[ClientVar]
		public static void info(ConsoleSystem.Arg arg)
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (SingletonComponent<MusicManager>.Instance == null)
			{
				stringBuilder.Append("No music manager was found");
			}
			else
			{
				stringBuilder.Append("Current music info: ");
				stringBuilder.AppendLine();
				stringBuilder.Append("  theme: " + SingletonComponent<MusicManager>.Instance.currentTheme);
				stringBuilder.AppendLine();
				stringBuilder.Append("  intensity: " + SingletonComponent<MusicManager>.Instance.intensity);
				stringBuilder.AppendLine();
				stringBuilder.Append("  next music: " + SingletonComponent<MusicManager>.Instance.nextMusic);
				stringBuilder.AppendLine();
				stringBuilder.Append("  current time: " + Time.time);
				stringBuilder.AppendLine();
			}
			arg.ReplyWith(stringBuilder.ToString());
		}
	}
}
