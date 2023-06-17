using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020004AB RID: 1195
public class EngineDamageOverTime
{
	// Token: 0x04001F8A RID: 8074
	private readonly List<EngineDamageOverTime.RecentDamage> recentDamage = new List<EngineDamageOverTime.RecentDamage>();

	// Token: 0x04001F8B RID: 8075
	private readonly float maxSeconds;

	// Token: 0x04001F8C RID: 8076
	private readonly float triggerDamage;

	// Token: 0x04001F8D RID: 8077
	private readonly Action trigger;

	// Token: 0x06002725 RID: 10021 RVA: 0x000F4D54 File Offset: 0x000F2F54
	public EngineDamageOverTime(float triggerDamage, float maxSeconds, Action trigger)
	{
		this.triggerDamage = triggerDamage;
		this.maxSeconds = maxSeconds;
		this.trigger = trigger;
	}

	// Token: 0x06002726 RID: 10022 RVA: 0x000F4D7C File Offset: 0x000F2F7C
	public void TakeDamage(float amount)
	{
		this.recentDamage.Add(new EngineDamageOverTime.RecentDamage(Time.time, amount));
		if (this.GetRecentDamage() > this.triggerDamage)
		{
			this.trigger();
			this.recentDamage.Clear();
		}
	}

	// Token: 0x06002727 RID: 10023 RVA: 0x000F4DB8 File Offset: 0x000F2FB8
	private float GetRecentDamage()
	{
		float num = 0f;
		int i;
		for (i = this.recentDamage.Count - 1; i >= 0; i--)
		{
			EngineDamageOverTime.RecentDamage recentDamage = this.recentDamage[i];
			if (Time.time > recentDamage.time + this.maxSeconds)
			{
				break;
			}
			num += recentDamage.amount;
		}
		if (i > 0)
		{
			this.recentDamage.RemoveRange(0, i + 1);
		}
		return num;
	}

	// Token: 0x02000D0C RID: 3340
	private struct RecentDamage
	{
		// Token: 0x04004600 RID: 17920
		public readonly float time;

		// Token: 0x04004601 RID: 17921
		public readonly float amount;

		// Token: 0x0600501A RID: 20506 RVA: 0x001A7F7E File Offset: 0x001A617E
		public RecentDamage(float time, float amount)
		{
			this.time = time;
			this.amount = amount;
		}
	}
}
