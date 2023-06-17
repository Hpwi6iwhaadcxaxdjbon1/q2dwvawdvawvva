using System;
using UnityEngine;

// Token: 0x020002C8 RID: 712
[ExecuteInEditMode]
public class LookAt : MonoBehaviour, IClientComponent
{
	// Token: 0x04001685 RID: 5765
	public Transform target;

	// Token: 0x06001D7A RID: 7546 RVA: 0x000CADEE File Offset: 0x000C8FEE
	private void Update()
	{
		if (this.target == null)
		{
			return;
		}
		base.transform.LookAt(this.target);
	}
}
