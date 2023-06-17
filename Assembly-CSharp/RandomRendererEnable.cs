using System;
using UnityEngine;

// Token: 0x0200015D RID: 349
public class RandomRendererEnable : MonoBehaviour
{
	// Token: 0x04000FF1 RID: 4081
	public Renderer[] randoms;

	// Token: 0x170001F8 RID: 504
	// (get) Token: 0x0600173C RID: 5948 RVA: 0x000B1057 File Offset: 0x000AF257
	// (set) Token: 0x0600173D RID: 5949 RVA: 0x000B105F File Offset: 0x000AF25F
	public int EnabledIndex { get; private set; }

	// Token: 0x0600173E RID: 5950 RVA: 0x000B1068 File Offset: 0x000AF268
	public void OnEnable()
	{
		int num = UnityEngine.Random.Range(0, this.randoms.Length);
		this.EnabledIndex = num;
		this.randoms[num].enabled = true;
	}
}
