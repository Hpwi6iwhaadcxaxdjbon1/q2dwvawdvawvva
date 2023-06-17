using System;
using Rust.UI;
using UnityEngine;

// Token: 0x02000890 RID: 2192
public class ServerBrowserTag : MonoBehaviour
{
	// Token: 0x04003135 RID: 12597
	public string serverTag;

	// Token: 0x04003136 RID: 12598
	public RustButton button;

	// Token: 0x1700045C RID: 1116
	// (get) Token: 0x060036D0 RID: 14032 RVA: 0x0014AC7F File Offset: 0x00148E7F
	public bool IsActive
	{
		get
		{
			return this.button != null && this.button.IsPressed;
		}
	}
}
