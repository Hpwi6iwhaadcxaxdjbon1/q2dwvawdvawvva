using System;
using Rust.UI;
using UnityEngine;

// Token: 0x0200017F RID: 383
public class UITwitchTrophy : UIDialog
{
	// Token: 0x04001086 RID: 4230
	public HttpImage EventImage;

	// Token: 0x04001087 RID: 4231
	public RustText EventName;

	// Token: 0x04001088 RID: 4232
	public RustText WinningTeamName;

	// Token: 0x04001089 RID: 4233
	public RectTransform TeamMembersRoot;

	// Token: 0x0400108A RID: 4234
	public GameObject TeamMemberNamePrefab;

	// Token: 0x0400108B RID: 4235
	public GameObject MissingDataOverlay;
}
