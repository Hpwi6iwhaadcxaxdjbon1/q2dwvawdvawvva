using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200086B RID: 2155
public class ConnectionScreen : SingletonComponent<ConnectionScreen>
{
	// Token: 0x0400308F RID: 12431
	public Text statusText;

	// Token: 0x04003090 RID: 12432
	public GameObject disconnectButton;

	// Token: 0x04003091 RID: 12433
	public GameObject retryButton;

	// Token: 0x04003092 RID: 12434
	public ServerBrowserInfo browserInfo;
}
