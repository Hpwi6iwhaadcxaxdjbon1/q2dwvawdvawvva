using System;
using UnityEngine;

// Token: 0x020001C9 RID: 457
public class LineRendererActivate : MonoBehaviour, IClientComponent
{
	// Token: 0x06001909 RID: 6409 RVA: 0x000B8982 File Offset: 0x000B6B82
	private void OnEnable()
	{
		base.GetComponent<LineRenderer>().enabled = true;
	}
}
