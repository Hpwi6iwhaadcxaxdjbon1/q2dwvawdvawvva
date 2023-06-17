using System;
using UnityEngine;

namespace ConVar
{
	// Token: 0x02000AE2 RID: 2786
	[ConsoleSystem.Factory("time")]
	public class Time : ConsoleSystem
	{
		// Token: 0x04003C31 RID: 15409
		[ServerVar]
		[Help("Pause time while loading")]
		public static bool pausewhileloading = true;

		// Token: 0x170005DD RID: 1501
		// (get) Token: 0x0600431B RID: 17179 RVA: 0x0018D36C File Offset: 0x0018B56C
		// (set) Token: 0x0600431C RID: 17180 RVA: 0x0018D373 File Offset: 0x0018B573
		[ServerVar]
		[Help("Fixed delta time in seconds")]
		public static float fixeddelta
		{
			get
			{
				return Time.fixedDeltaTime;
			}
			set
			{
				Time.fixedDeltaTime = value;
			}
		}

		// Token: 0x170005DE RID: 1502
		// (get) Token: 0x0600431D RID: 17181 RVA: 0x0018D37B File Offset: 0x0018B57B
		// (set) Token: 0x0600431E RID: 17182 RVA: 0x0018D382 File Offset: 0x0018B582
		[ServerVar]
		[Help("The minimum amount of times to tick per frame")]
		public static float maxdelta
		{
			get
			{
				return Time.maximumDeltaTime;
			}
			set
			{
				Time.maximumDeltaTime = value;
			}
		}

		// Token: 0x170005DF RID: 1503
		// (get) Token: 0x0600431F RID: 17183 RVA: 0x0018D38A File Offset: 0x0018B58A
		// (set) Token: 0x06004320 RID: 17184 RVA: 0x0018D391 File Offset: 0x0018B591
		[ServerVar]
		[Help("The time scale")]
		public static float timescale
		{
			get
			{
				return Time.timeScale;
			}
			set
			{
				Time.timeScale = value;
			}
		}
	}
}
