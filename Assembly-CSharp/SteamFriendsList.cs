using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020008B7 RID: 2231
public class SteamFriendsList : MonoBehaviour
{
	// Token: 0x04003210 RID: 12816
	public RectTransform targetPanel;

	// Token: 0x04003211 RID: 12817
	public SteamUserButton userButton;

	// Token: 0x04003212 RID: 12818
	public bool IncludeFriendsList = true;

	// Token: 0x04003213 RID: 12819
	public bool IncludeRecentlySeen;

	// Token: 0x04003214 RID: 12820
	public bool IncludeLastAttacker;

	// Token: 0x04003215 RID: 12821
	public bool IncludeRecentlyPlayedWith;

	// Token: 0x04003216 RID: 12822
	public bool ShowTeamFirst;

	// Token: 0x04003217 RID: 12823
	public bool HideSteamIdsInStreamerMode;

	// Token: 0x04003218 RID: 12824
	public bool RefreshOnEnable = true;

	// Token: 0x04003219 RID: 12825
	public SteamFriendsList.onFriendSelectedEvent onFriendSelected;

	// Token: 0x0400321A RID: 12826
	public Func<ulong, bool> shouldShowPlayer;

	// Token: 0x02000EA3 RID: 3747
	[Serializable]
	public class onFriendSelectedEvent : UnityEvent<ulong, string>
	{
	}
}
