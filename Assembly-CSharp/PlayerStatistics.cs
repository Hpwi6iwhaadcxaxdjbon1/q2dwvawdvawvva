using System;
using ConVar;

// Token: 0x02000454 RID: 1108
public class PlayerStatistics
{
	// Token: 0x04001D2F RID: 7471
	public SteamStatistics steam;

	// Token: 0x04001D30 RID: 7472
	public ServerStatistics server;

	// Token: 0x04001D31 RID: 7473
	public CombatLog combat;

	// Token: 0x04001D32 RID: 7474
	private BasePlayer forPlayer;

	// Token: 0x04001D33 RID: 7475
	private TimeSince lastSteamSave;

	// Token: 0x060024C4 RID: 9412 RVA: 0x000E9371 File Offset: 0x000E7571
	public PlayerStatistics(BasePlayer player)
	{
		this.steam = new SteamStatistics(player);
		this.server = new ServerStatistics(player);
		this.combat = new CombatLog(player);
		this.forPlayer = player;
	}

	// Token: 0x060024C5 RID: 9413 RVA: 0x000E93A4 File Offset: 0x000E75A4
	public void Init()
	{
		this.steam.Init();
		this.server.Init();
		this.combat.Init();
	}

	// Token: 0x060024C6 RID: 9414 RVA: 0x000E93C8 File Offset: 0x000E75C8
	public void Save(bool forceSteamSave = false)
	{
		if (Server.official && (forceSteamSave || this.lastSteamSave > 60f))
		{
			this.lastSteamSave = 0f;
			this.steam.Save();
		}
		this.server.Save();
		this.combat.Save();
	}

	// Token: 0x060024C7 RID: 9415 RVA: 0x000E9422 File Offset: 0x000E7622
	public void Add(string name, int val, Stats stats = Stats.Steam)
	{
		if ((stats & Stats.Steam) != (Stats)0)
		{
			this.steam.Add(name, val);
		}
		if ((stats & Stats.Server) != (Stats)0)
		{
			this.server.Add(name, val);
		}
		if ((stats & Stats.Life) != (Stats)0)
		{
			this.forPlayer.LifeStoryGenericStat(name, val);
		}
	}
}
