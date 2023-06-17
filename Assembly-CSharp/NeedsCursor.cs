using System;
using UnityEngine;

// Token: 0x020007A2 RID: 1954
public class NeedsCursor : MonoBehaviour, IClientComponent
{
	// Token: 0x0600350F RID: 13583 RVA: 0x001465D5 File Offset: 0x001447D5
	private void Update()
	{
		CursorManager.HoldOpen(false);
	}
}
