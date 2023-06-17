using System;
using UnityEngine;

// Token: 0x0200096D RID: 2413
public class IronsightAimPoint : MonoBehaviour
{
	// Token: 0x040033EF RID: 13295
	public Transform targetPoint;

	// Token: 0x060039CC RID: 14796 RVA: 0x00156E40 File Offset: 0x00155040
	private void OnDrawGizmos()
	{
		Gizmos.color = Color.cyan;
		Vector3 normalized = (this.targetPoint.position - base.transform.position).normalized;
		Gizmos.color = Color.red;
		this.DrawArrow(base.transform.position, base.transform.position + normalized * 0.1f, 0.1f);
		Gizmos.color = Color.cyan;
		this.DrawArrow(base.transform.position, this.targetPoint.position, 0.02f);
		Gizmos.color = Color.yellow;
		this.DrawArrow(this.targetPoint.position, this.targetPoint.position + normalized * 3f, 0.02f);
	}

	// Token: 0x060039CD RID: 14797 RVA: 0x00156F1C File Offset: 0x0015511C
	private void DrawArrow(Vector3 start, Vector3 end, float arrowLength)
	{
		Vector3 normalized = (end - start).normalized;
		Vector3 up = Camera.current.transform.up;
		Gizmos.DrawLine(start, end);
		Gizmos.DrawLine(end, end + up * arrowLength - normalized * arrowLength);
		Gizmos.DrawLine(end, end - up * arrowLength - normalized * arrowLength);
		Gizmos.DrawLine(end + up * arrowLength - normalized * arrowLength, end - up * arrowLength - normalized * arrowLength);
	}
}
