using System;

// Token: 0x020003CC RID: 972
public class HumanBodyResourceDispenser : ResourceDispenser
{
	// Token: 0x0600219E RID: 8606 RVA: 0x000DAE7C File Offset: 0x000D907C
	public override bool OverrideOwnership(Item item, AttackEntity weapon)
	{
		if (item.info.shortname == "skull.human")
		{
			PlayerCorpse component = base.GetComponent<PlayerCorpse>();
			if (component)
			{
				item.name = HumanBodyResourceDispenser.CreateSkullName(component.playerName);
				item.streamerName = HumanBodyResourceDispenser.CreateSkullName(component.streamerName);
				return true;
			}
		}
		return false;
	}

	// Token: 0x0600219F RID: 8607 RVA: 0x000DAED4 File Offset: 0x000D90D4
	public static string CreateSkullName(string playerName)
	{
		return "Skull of \"" + playerName + "\"";
	}
}
