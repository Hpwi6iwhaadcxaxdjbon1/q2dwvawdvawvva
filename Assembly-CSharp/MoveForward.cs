using System;
using UnityEngine;

// Token: 0x02000906 RID: 2310
public class MoveForward : MonoBehaviour
{
	// Token: 0x040032F3 RID: 13043
	public float Speed = 2f;

	// Token: 0x060037FB RID: 14331 RVA: 0x0014ECA5 File Offset: 0x0014CEA5
	protected void Update()
	{
		base.GetComponent<Rigidbody>().velocity = this.Speed * base.transform.forward;
	}
}
