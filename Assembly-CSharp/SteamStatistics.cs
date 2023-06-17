using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ConVar;
using UnityEngine;

// Token: 0x02000456 RID: 1110
public class SteamStatistics
{
	// Token: 0x04001D37 RID: 7479
	private BasePlayer player;

	// Token: 0x04001D38 RID: 7480
	public Dictionary<string, int> intStats = new Dictionary<string, int>();

	// Token: 0x04001D39 RID: 7481
	private Task refresh;

	// Token: 0x060024CE RID: 9422 RVA: 0x000E94D5 File Offset: 0x000E76D5
	public SteamStatistics(BasePlayer p)
	{
		this.player = p;
	}

	// Token: 0x060024CF RID: 9423 RVA: 0x000E94EF File Offset: 0x000E76EF
	public void Init()
	{
		if (!PlatformService.Instance.IsValid)
		{
			return;
		}
		this.refresh = PlatformService.Instance.LoadPlayerStats(this.player.userID);
		this.intStats.Clear();
	}

	// Token: 0x060024D0 RID: 9424 RVA: 0x000E9524 File Offset: 0x000E7724
	public void Save()
	{
		if (!PlatformService.Instance.IsValid)
		{
			return;
		}
		PlatformService.Instance.SavePlayerStats(this.player.userID);
	}

	// Token: 0x060024D1 RID: 9425 RVA: 0x000E954C File Offset: 0x000E774C
	public void Add(string name, int var)
	{
		if (!PlatformService.Instance.IsValid)
		{
			return;
		}
		if (this.refresh == null || !this.refresh.IsCompleted)
		{
			return;
		}
		using (TimeWarning.New("PlayerStats.Add", 0))
		{
			int num = 0;
			if (this.intStats.TryGetValue(name, out num))
			{
				Dictionary<string, int> dictionary = this.intStats;
				dictionary[name] += var;
				PlatformService.Instance.SetPlayerStatInt(this.player.userID, name, (long)this.intStats[name]);
			}
			else
			{
				num = (int)PlatformService.Instance.GetPlayerStatInt(this.player.userID, name, 0L);
				if (!PlatformService.Instance.SetPlayerStatInt(this.player.userID, name, (long)(num + var)))
				{
					if (Global.developer > 0)
					{
						Debug.LogWarning("[STEAMWORKS] Couldn't SetUserStat: " + name);
					}
				}
				else
				{
					this.intStats.Add(name, num + var);
				}
			}
		}
	}

	// Token: 0x060024D2 RID: 9426 RVA: 0x000E9654 File Offset: 0x000E7854
	public int Get(string name)
	{
		if (!PlatformService.Instance.IsValid)
		{
			return 0;
		}
		if (this.refresh == null || !this.refresh.IsCompleted)
		{
			return 0;
		}
		int result;
		using (TimeWarning.New("PlayerStats.Get", 0))
		{
			int num;
			if (this.intStats.TryGetValue(name, out num))
			{
				result = num;
			}
			else
			{
				result = (int)PlatformService.Instance.GetPlayerStatInt(this.player.userID, name, 0L);
			}
		}
		return result;
	}
}
