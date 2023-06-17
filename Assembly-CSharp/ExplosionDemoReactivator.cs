using System;
using UnityEngine;

// Token: 0x0200098E RID: 2446
public class ExplosionDemoReactivator : MonoBehaviour
{
	// Token: 0x04003486 RID: 13446
	public float TimeDelayToReactivate = 3f;

	// Token: 0x06003A3E RID: 14910 RVA: 0x00158E10 File Offset: 0x00157010
	private void Start()
	{
		base.InvokeRepeating("Reactivate", 0f, this.TimeDelayToReactivate);
	}

	// Token: 0x06003A3F RID: 14911 RVA: 0x00158E28 File Offset: 0x00157028
	private void Reactivate()
	{
		foreach (Transform transform in base.GetComponentsInChildren<Transform>())
		{
			transform.gameObject.SetActive(false);
			transform.gameObject.SetActive(true);
		}
	}
}
