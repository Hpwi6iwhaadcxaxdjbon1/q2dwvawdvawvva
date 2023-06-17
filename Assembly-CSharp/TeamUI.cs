using System;
using TMPro;
using UnityEngine;

// Token: 0x020008BB RID: 2235
public class TeamUI : MonoBehaviour
{
	// Token: 0x04003233 RID: 12851
	public static Translate.Phrase invitePhrase = new Translate.Phrase("team.invited", "{0} has invited you to join a team");

	// Token: 0x04003234 RID: 12852
	public RectTransform MemberPanel;

	// Token: 0x04003235 RID: 12853
	public GameObject memberEntryPrefab;

	// Token: 0x04003236 RID: 12854
	public TeamMemberElement[] elements;

	// Token: 0x04003237 RID: 12855
	public GameObject NoTeamPanel;

	// Token: 0x04003238 RID: 12856
	public GameObject TeamPanel;

	// Token: 0x04003239 RID: 12857
	public GameObject LeaveTeamButton;

	// Token: 0x0400323A RID: 12858
	public GameObject InviteAcceptPanel;

	// Token: 0x0400323B RID: 12859
	public TextMeshProUGUI inviteText;

	// Token: 0x0400323C RID: 12860
	public static bool dirty = true;

	// Token: 0x0400323D RID: 12861
	[NonSerialized]
	public static ulong pendingTeamID;

	// Token: 0x0400323E RID: 12862
	[NonSerialized]
	public static string pendingTeamLeaderName;
}
