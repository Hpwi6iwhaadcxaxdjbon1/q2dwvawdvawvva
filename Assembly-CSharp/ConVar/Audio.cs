using System;
using UnityEngine;

namespace ConVar
{
	// Token: 0x02000AA6 RID: 2726
	[ConsoleSystem.Factory("audio")]
	public class Audio : ConsoleSystem
	{
		// Token: 0x04003AF2 RID: 15090
		[ClientVar(Help = "Volume", Saved = true)]
		public static float master = 1f;

		// Token: 0x04003AF3 RID: 15091
		[ClientVar(Help = "Volume", Saved = true)]
		public static float musicvolume = 1f;

		// Token: 0x04003AF4 RID: 15092
		[ClientVar(Help = "Volume", Saved = true)]
		public static float musicvolumemenu = 1f;

		// Token: 0x04003AF5 RID: 15093
		[ClientVar(Help = "Volume", Saved = true)]
		public static float game = 1f;

		// Token: 0x04003AF6 RID: 15094
		[ClientVar(Help = "Volume", Saved = true)]
		public static float voices = 1f;

		// Token: 0x04003AF7 RID: 15095
		[ClientVar(Help = "Volume", Saved = true)]
		public static float instruments = 1f;

		// Token: 0x04003AF8 RID: 15096
		[ClientVar(Help = "Volume", Saved = true)]
		public static float voiceProps = 1f;

		// Token: 0x04003AF9 RID: 15097
		[ClientVar(Help = "Volume", Saved = true)]
		public static float eventAudio = 1f;

		// Token: 0x04003AFA RID: 15098
		[ClientVar(Help = "Ambience System")]
		public static bool ambience = true;

		// Token: 0x04003AFB RID: 15099
		[ClientVar(Help = "Max ms per frame to spend updating sounds")]
		public static float framebudget = 0.3f;

		// Token: 0x04003AFC RID: 15100
		[ClientVar]
		public static float minupdatefraction = 0.1f;

		// Token: 0x04003AFD RID: 15101
		[ClientVar(Help = "Use more advanced sound occlusion", Saved = true)]
		public static bool advancedocclusion = false;

		// Token: 0x04003AFE RID: 15102
		[ClientVar(Help = "Use higher quality sound fades on some sounds")]
		public static bool hqsoundfade = false;

		// Token: 0x04003AFF RID: 15103
		[ClientVar(Saved = false)]
		public static bool debugVoiceLimiting = false;

		// Token: 0x1700059F RID: 1439
		// (get) Token: 0x06004162 RID: 16738 RVA: 0x00183FEA File Offset: 0x001821EA
		// (set) Token: 0x06004163 RID: 16739 RVA: 0x00183FF4 File Offset: 0x001821F4
		[ClientVar(Help = "Volume", Saved = true)]
		public static int speakers
		{
			get
			{
				return (int)UnityEngine.AudioSettings.speakerMode;
			}
			set
			{
				value = Mathf.Clamp(value, 2, 7);
				if (!Application.isEditor)
				{
					AudioConfiguration configuration = UnityEngine.AudioSettings.GetConfiguration();
					configuration.speakerMode = (AudioSpeakerMode)value;
					using (TimeWarning.New("Audio Settings Reset", 250))
					{
						UnityEngine.AudioSettings.Reset(configuration);
					}
				}
			}
		}

		// Token: 0x06004164 RID: 16740 RVA: 0x000063A5 File Offset: 0x000045A5
		[ClientVar]
		public static void printSounds(ConsoleSystem.Arg arg)
		{
		}

		// Token: 0x06004165 RID: 16741 RVA: 0x000063A5 File Offset: 0x000045A5
		[ClientVar(ClientAdmin = true, Help = "print active engine sound info")]
		public static void printEngineSounds(ConsoleSystem.Arg arg)
		{
		}
	}
}
