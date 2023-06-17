using System;
using UnityEngine;

// Token: 0x020001D7 RID: 471
public class AICoverPoint : AIPoint
{
	// Token: 0x04001218 RID: 4632
	public float coverDot = 0.5f;

	// Token: 0x0600192B RID: 6443 RVA: 0x000B9170 File Offset: 0x000B7370
	public void OnDrawGizmos()
	{
		Vector3 vector = base.transform.position + Vector3.up * 1f;
		Gizmos.color = Color.white;
		Gizmos.DrawLine(vector, vector + base.transform.forward * 0.5f);
		Gizmos.color = Color.yellow;
		Gizmos.DrawCube(base.transform.position + Vector3.up * 0.125f, new Vector3(0.5f, 0.25f, 0.5f));
		Gizmos.DrawLine(base.transform.position, vector);
		Vector3 normalized = (base.transform.forward + base.transform.right * this.coverDot * 1f).normalized;
		Vector3 normalized2 = (base.transform.forward + -base.transform.right * this.coverDot * 1f).normalized;
		Gizmos.DrawLine(vector, vector + normalized * 1f);
		Gizmos.DrawLine(vector, vector + normalized2 * 1f);
	}
}
