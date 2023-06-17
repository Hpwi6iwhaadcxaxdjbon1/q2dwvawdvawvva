using System;
using UnityEngine;

// Token: 0x0200095E RID: 2398
public static class PoolableEx
{
	// Token: 0x060039A1 RID: 14753 RVA: 0x001567A8 File Offset: 0x001549A8
	public static bool SupportsPoolingInParent(this GameObject gameObject)
	{
		Poolable componentInParent = gameObject.GetComponentInParent<Poolable>();
		return componentInParent != null && componentInParent.prefabID > 0U;
	}

	// Token: 0x060039A2 RID: 14754 RVA: 0x001567D0 File Offset: 0x001549D0
	public static bool SupportsPooling(this GameObject gameObject)
	{
		Poolable component = gameObject.GetComponent<Poolable>();
		return component != null && component.prefabID > 0U;
	}

	// Token: 0x060039A3 RID: 14755 RVA: 0x001567F8 File Offset: 0x001549F8
	public static void AwakeFromInstantiate(this GameObject gameObject)
	{
		if (gameObject.activeSelf)
		{
			gameObject.GetComponent<Poolable>().SetBehaviourEnabled(true);
			return;
		}
		gameObject.SetActive(true);
	}
}
