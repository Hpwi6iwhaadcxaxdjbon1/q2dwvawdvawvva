using System;
using UnityEngine;

// Token: 0x0200014F RID: 335
public class DestroyArcadeEntity : BaseMonoBehaviour
{
	// Token: 0x04000FB8 RID: 4024
	public ArcadeEntity ent;

	// Token: 0x04000FB9 RID: 4025
	public float TimeToDie = 1f;

	// Token: 0x04000FBA RID: 4026
	public float TimeToDieVariance;

	// Token: 0x06001711 RID: 5905 RVA: 0x000B01EA File Offset: 0x000AE3EA
	private void Start()
	{
		base.Invoke(new Action(this.DestroyAction), this.TimeToDie + UnityEngine.Random.Range(this.TimeToDieVariance * -0.5f, this.TimeToDieVariance * 0.5f));
	}

	// Token: 0x06001712 RID: 5906 RVA: 0x000B0222 File Offset: 0x000AE422
	private void DestroyAction()
	{
		if (this.ent != null & this.ent.host)
		{
			UnityEngine.Object.Destroy(this.ent.gameObject);
		}
	}
}
