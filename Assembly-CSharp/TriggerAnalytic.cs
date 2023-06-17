using System;
using System.Collections.Generic;
using Facepunch.Rust;
using UnityEngine;

// Token: 0x0200057D RID: 1405
public class TriggerAnalytic : TriggerBase, IServerComponent
{
	// Token: 0x040022FB RID: 8955
	public string AnalyticMessage;

	// Token: 0x040022FC RID: 8956
	public float Timeout = 120f;

	// Token: 0x040022FD RID: 8957
	private List<TriggerAnalytic.RecentPlayerEntrance> recentEntrances = new List<TriggerAnalytic.RecentPlayerEntrance>();

	// Token: 0x06002B16 RID: 11030 RVA: 0x00105EF4 File Offset: 0x001040F4
	internal override GameObject InterestedInObject(GameObject obj)
	{
		if (!Analytics.Server.Enabled)
		{
			return null;
		}
		BasePlayer basePlayer;
		if ((basePlayer = (obj.ToBaseEntity() as BasePlayer)) != null && !basePlayer.IsNpc && basePlayer.isServer)
		{
			return basePlayer.gameObject;
		}
		return null;
	}

	// Token: 0x06002B17 RID: 11031 RVA: 0x00105F34 File Offset: 0x00104134
	internal override void OnEntityEnter(BaseEntity ent)
	{
		if (!Analytics.Server.Enabled)
		{
			return;
		}
		base.OnEntityEnter(ent);
		BasePlayer basePlayer = ent.ToPlayer();
		if (basePlayer != null && !basePlayer.IsNpc)
		{
			this.CheckTimeouts();
			if (this.IsPlayerValid(basePlayer))
			{
				Analytics.Server.Trigger(this.AnalyticMessage);
				this.recentEntrances.Add(new TriggerAnalytic.RecentPlayerEntrance
				{
					Player = basePlayer,
					Time = 0f
				});
			}
		}
	}

	// Token: 0x06002B18 RID: 11032 RVA: 0x00105FB0 File Offset: 0x001041B0
	private void CheckTimeouts()
	{
		for (int i = this.recentEntrances.Count - 1; i >= 0; i--)
		{
			if (this.recentEntrances[i].Time > this.Timeout)
			{
				this.recentEntrances.RemoveAt(i);
			}
		}
	}

	// Token: 0x06002B19 RID: 11033 RVA: 0x00106000 File Offset: 0x00104200
	private bool IsPlayerValid(BasePlayer p)
	{
		for (int i = 0; i < this.recentEntrances.Count; i++)
		{
			if (this.recentEntrances[i].Player == p)
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x02000D5B RID: 3419
	private struct RecentPlayerEntrance
	{
		// Token: 0x0400471B RID: 18203
		public BasePlayer Player;

		// Token: 0x0400471C RID: 18204
		public TimeSince Time;
	}
}
