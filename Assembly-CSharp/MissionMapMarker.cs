using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020007F7 RID: 2039
public class MissionMapMarker : MonoBehaviour
{
	// Token: 0x04002DBD RID: 11709
	public Image Icon;

	// Token: 0x04002DBE RID: 11710
	public Tooltip TooltipComponent;

	// Token: 0x0600359F RID: 13727 RVA: 0x00147B5C File Offset: 0x00145D5C
	public void Populate(BaseMission.MissionInstance mission)
	{
		BaseMission mission2 = mission.GetMission();
		this.Icon.sprite = mission2.icon;
		this.TooltipComponent.token = mission2.missionName.token;
		this.TooltipComponent.Text = mission2.missionName.english;
	}
}
