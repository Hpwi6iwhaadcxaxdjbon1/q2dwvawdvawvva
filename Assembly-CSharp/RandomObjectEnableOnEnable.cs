using System;
using UnityEngine;

// Token: 0x020008AE RID: 2222
public class RandomObjectEnableOnEnable : MonoBehaviour
{
	// Token: 0x040031E2 RID: 12770
	public GameObject[] objects;

	// Token: 0x06003718 RID: 14104 RVA: 0x0014C85E File Offset: 0x0014AA5E
	public void OnEnable()
	{
		this.objects[UnityEngine.Random.Range(0, this.objects.Length)].SetActive(true);
	}
}
