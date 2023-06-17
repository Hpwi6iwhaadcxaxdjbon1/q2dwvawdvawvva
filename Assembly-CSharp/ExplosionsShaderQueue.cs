using System;
using UnityEngine;

// Token: 0x02000998 RID: 2456
public class ExplosionsShaderQueue : MonoBehaviour
{
	// Token: 0x040034BE RID: 13502
	public int AddQueue = 1;

	// Token: 0x040034BF RID: 13503
	private Renderer rend;

	// Token: 0x06003A65 RID: 14949 RVA: 0x001596F8 File Offset: 0x001578F8
	private void Start()
	{
		this.rend = base.GetComponent<Renderer>();
		if (this.rend != null)
		{
			this.rend.sharedMaterial.renderQueue += this.AddQueue;
			return;
		}
		base.Invoke("SetProjectorQueue", 0.1f);
	}

	// Token: 0x06003A66 RID: 14950 RVA: 0x0015974D File Offset: 0x0015794D
	private void SetProjectorQueue()
	{
		base.GetComponent<Projector>().material.renderQueue += this.AddQueue;
	}

	// Token: 0x06003A67 RID: 14951 RVA: 0x0015976C File Offset: 0x0015796C
	private void OnDisable()
	{
		if (this.rend != null)
		{
			this.rend.sharedMaterial.renderQueue = -1;
		}
	}
}
