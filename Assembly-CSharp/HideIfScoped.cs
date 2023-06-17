using System;
using UnityEngine;

// Token: 0x020001CC RID: 460
public class HideIfScoped : MonoBehaviour
{
	// Token: 0x040011E4 RID: 4580
	public Renderer[] renderers;

	// Token: 0x06001910 RID: 6416 RVA: 0x000B8AA0 File Offset: 0x000B6CA0
	public void SetVisible(bool vis)
	{
		Renderer[] array = this.renderers;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].enabled = vis;
		}
	}
}
