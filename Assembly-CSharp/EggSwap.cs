using System;
using UnityEngine;

// Token: 0x02000158 RID: 344
public class EggSwap : MonoBehaviour
{
	// Token: 0x04000FD5 RID: 4053
	public Renderer[] eggRenderers;

	// Token: 0x0600171E RID: 5918 RVA: 0x000B02EC File Offset: 0x000AE4EC
	public void Show(int index)
	{
		this.HideAll();
		this.eggRenderers[index].enabled = true;
	}

	// Token: 0x0600171F RID: 5919 RVA: 0x000B0304 File Offset: 0x000AE504
	public void HideAll()
	{
		Renderer[] array = this.eggRenderers;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].enabled = false;
		}
	}
}
