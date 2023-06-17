using System;
using Rust.UI;
using UnityEngine;

// Token: 0x020008B3 RID: 2227
public class Scoreboard : MonoBehaviour, IClientComponent
{
	// Token: 0x040031EE RID: 12782
	public static Scoreboard instance;

	// Token: 0x040031EF RID: 12783
	public RustText scoreboardTitle;

	// Token: 0x040031F0 RID: 12784
	public RectTransform scoreboardRootContents;

	// Token: 0x040031F1 RID: 12785
	public RustText scoreLimitText;

	// Token: 0x040031F2 RID: 12786
	public GameObject teamPrefab;

	// Token: 0x040031F3 RID: 12787
	public GameObject columnPrefab;

	// Token: 0x040031F4 RID: 12788
	public GameObject dividerPrefab;

	// Token: 0x040031F5 RID: 12789
	public Color localPlayerColor;

	// Token: 0x040031F6 RID: 12790
	public Color otherPlayerColor;

	// Token: 0x040031F7 RID: 12791
	public Scoreboard.TeamColumn[] teamColumns;

	// Token: 0x040031F8 RID: 12792
	public GameObject[] TeamPanels;

	// Token: 0x02000EA2 RID: 3746
	public class TeamColumn
	{
		// Token: 0x04004C68 RID: 19560
		public GameObject nameColumn;

		// Token: 0x04004C69 RID: 19561
		public GameObject[] activeColumns;
	}
}
