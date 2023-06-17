using System;
using ConVar;

// Token: 0x020005FC RID: 1532
public class ItemModSummerSunglassesEquip : ItemMod
{
	// Token: 0x04002522 RID: 9506
	public float SunsetTime;

	// Token: 0x04002523 RID: 9507
	public float SunriseTime;

	// Token: 0x04002524 RID: 9508
	public string AchivementName;

	// Token: 0x06002D82 RID: 11650 RVA: 0x00111E64 File Offset: 0x00110064
	public override void DoAction(Item item, BasePlayer player)
	{
		base.DoAction(item, player);
		if (player != null && !string.IsNullOrEmpty(this.AchivementName) && player.inventory.containerWear.FindItemByUID(item.uid) != null)
		{
			float time = Env.time;
			if (time < this.SunriseTime || time > this.SunsetTime)
			{
				player.GiveAchievement(this.AchivementName);
			}
		}
	}
}
