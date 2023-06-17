using System;
using UnityEngine;

// Token: 0x0200090C RID: 2316
public class ParticleSystemPlayer : MonoBehaviour, IOnParentDestroying
{
	// Token: 0x06003808 RID: 14344 RVA: 0x0014ED60 File Offset: 0x0014CF60
	protected void OnEnable()
	{
		base.GetComponent<ParticleSystem>().enableEmission = true;
	}

	// Token: 0x06003809 RID: 14345 RVA: 0x0014ED6E File Offset: 0x0014CF6E
	public void OnParentDestroying()
	{
		base.GetComponent<ParticleSystem>().enableEmission = false;
	}
}
