using System;
using UnityEngine;

// Token: 0x020004EF RID: 1263
public class CreateEffect : MonoBehaviour
{
	// Token: 0x040020DD RID: 8413
	public GameObjectRef EffectToCreate;

	// Token: 0x060028DD RID: 10461 RVA: 0x000FB9C7 File Offset: 0x000F9BC7
	public void OnEnable()
	{
		Effect.client.Run(this.EffectToCreate.resourcePath, base.transform.position, base.transform.up, base.transform.forward, Effect.Type.Generic);
	}
}
