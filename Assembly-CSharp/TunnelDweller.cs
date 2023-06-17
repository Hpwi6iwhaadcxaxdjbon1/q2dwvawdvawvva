using System;
using Rust;

// Token: 0x020001F7 RID: 503
public class TunnelDweller : HumanNPC
{
	// Token: 0x040012BB RID: 4795
	private const string DWELLER_KILL_STAT = "dweller_kills_while_moving";

	// Token: 0x06001A57 RID: 6743 RVA: 0x000BE348 File Offset: 0x000BC548
	protected override string OverrideCorpseName()
	{
		return "Tunnel Dweller";
	}

	// Token: 0x06001A58 RID: 6744 RVA: 0x000BE350 File Offset: 0x000BC550
	protected override void OnKilledByPlayer(BasePlayer p)
	{
		base.OnKilledByPlayer(p);
		TrainEngine trainEngine;
		if (GameInfo.HasAchievements && p.GetParentEntity() != null && (trainEngine = (p.GetParentEntity() as TrainEngine)) != null && trainEngine.CurThrottleSetting != TrainEngine.EngineSpeeds.Zero && trainEngine.IsMovingOrOn)
		{
			p.stats.Add("dweller_kills_while_moving", 1, Stats.All);
			p.stats.Save(true);
		}
	}
}
