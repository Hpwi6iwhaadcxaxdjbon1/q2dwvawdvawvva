using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200087F RID: 2175
public class KeyBindUI : MonoBehaviour
{
	// Token: 0x040030EF RID: 12527
	public GameObject blockingCanvas;

	// Token: 0x040030F0 RID: 12528
	public Button btnA;

	// Token: 0x040030F1 RID: 12529
	public Button btnB;

	// Token: 0x040030F2 RID: 12530
	public string bindString;

	// Token: 0x1700045B RID: 1115
	// (get) Token: 0x0600368A RID: 13962 RVA: 0x0014A280 File Offset: 0x00148480
	// (set) Token: 0x0600368B RID: 13963 RVA: 0x0014A287 File Offset: 0x00148487
	public static bool IsBinding { get; private set; }
}
