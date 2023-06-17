using System;
using UnityEngine;

// Token: 0x0200030C RID: 780
public class ObjectSpam : MonoBehaviour
{
	// Token: 0x040017AD RID: 6061
	public GameObject source;

	// Token: 0x040017AE RID: 6062
	public int amount = 1000;

	// Token: 0x040017AF RID: 6063
	public float radius;

	// Token: 0x06001EA2 RID: 7842 RVA: 0x000D0C24 File Offset: 0x000CEE24
	private void Start()
	{
		for (int i = 0; i < this.amount; i++)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.source);
			gameObject.transform.position = base.transform.position + Vector3Ex.Range(-this.radius, this.radius);
			gameObject.hideFlags = (HideFlags.HideInHierarchy | HideFlags.HideInInspector);
		}
	}
}
