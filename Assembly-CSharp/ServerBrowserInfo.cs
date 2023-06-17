using System;
using Rust.UI;
using UnityEngine.UI;

// Token: 0x0200088D RID: 2189
public class ServerBrowserInfo : SingletonComponent<ServerBrowserInfo>
{
	// Token: 0x04003117 RID: 12567
	public bool isMain;

	// Token: 0x04003118 RID: 12568
	public Text serverName;

	// Token: 0x04003119 RID: 12569
	public Text serverMeta;

	// Token: 0x0400311A RID: 12570
	public Text serverText;

	// Token: 0x0400311B RID: 12571
	public Button viewWebpage;

	// Token: 0x0400311C RID: 12572
	public Button refresh;

	// Token: 0x0400311D RID: 12573
	public ServerInfo? currentServer;

	// Token: 0x0400311E RID: 12574
	public HttpImage headerImage;

	// Token: 0x0400311F RID: 12575
	public HttpImage logoImage;
}
