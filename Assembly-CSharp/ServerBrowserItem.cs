using System;
using Rust.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200088E RID: 2190
public class ServerBrowserItem : MonoBehaviour
{
	// Token: 0x04003120 RID: 12576
	public TextMeshProUGUI serverName;

	// Token: 0x04003121 RID: 12577
	public RustFlexText mapName;

	// Token: 0x04003122 RID: 12578
	public TextMeshProUGUI playerCount;

	// Token: 0x04003123 RID: 12579
	public TextMeshProUGUI ping;

	// Token: 0x04003124 RID: 12580
	public Toggle favourited;

	// Token: 0x04003125 RID: 12581
	public ServerBrowserTagList serverTagList;

	// Token: 0x04003126 RID: 12582
	public TextMeshProUGUI changeset;
}
