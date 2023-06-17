using System;
using UnityEngine;

// Token: 0x020008FC RID: 2300
public class IgnoreCollision : MonoBehaviour
{
	// Token: 0x040032D1 RID: 13009
	public Collider collider;

	// Token: 0x060037DB RID: 14299 RVA: 0x0014E6D6 File Offset: 0x0014C8D6
	protected void OnTriggerEnter(Collider other)
	{
		Debug.Log("IgnoreCollision: " + this.collider.gameObject.name + " + " + other.gameObject.name);
		Physics.IgnoreCollision(other, this.collider, true);
	}
}
