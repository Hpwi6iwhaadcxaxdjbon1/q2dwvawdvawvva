using System;
using Rust.UI;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020008B8 RID: 2232
public class SteamUserButton : MonoBehaviour
{
	// Token: 0x0400321B RID: 12827
	public RustText steamName;

	// Token: 0x0400321C RID: 12828
	public RustText steamInfo;

	// Token: 0x0400321D RID: 12829
	public RawImage avatar;

	// Token: 0x0400321E RID: 12830
	public Color textColorInGame;

	// Token: 0x0400321F RID: 12831
	public Color textColorOnline;

	// Token: 0x04003220 RID: 12832
	public Color textColorNormal;

	// Token: 0x17000464 RID: 1124
	// (get) Token: 0x06003729 RID: 14121 RVA: 0x0014C9D1 File Offset: 0x0014ABD1
	// (set) Token: 0x0600372A RID: 14122 RVA: 0x0014C9D9 File Offset: 0x0014ABD9
	public ulong SteamId { get; private set; }

	// Token: 0x17000465 RID: 1125
	// (get) Token: 0x0600372B RID: 14123 RVA: 0x0014C9E2 File Offset: 0x0014ABE2
	// (set) Token: 0x0600372C RID: 14124 RVA: 0x0014C9EA File Offset: 0x0014ABEA
	public string Username { get; private set; }
}
