using System;
using Rust.UI;
using UnityEngine;

// Token: 0x02000896 RID: 2198
public class UnreadMessages : SingletonComponent<UnreadMessages>
{
	// Token: 0x04003146 RID: 12614
	public StyleAsset AllRead;

	// Token: 0x04003147 RID: 12615
	public StyleAsset Unread;

	// Token: 0x04003148 RID: 12616
	public RustButton Button;

	// Token: 0x04003149 RID: 12617
	public GameObject UnreadTextObject;

	// Token: 0x0400314A RID: 12618
	public RustText UnreadText;

	// Token: 0x0400314B RID: 12619
	public GameObject MessageList;

	// Token: 0x0400314C RID: 12620
	public GameObject MessageListContainer;

	// Token: 0x0400314D RID: 12621
	public GameObject MessageListEmpty;
}
