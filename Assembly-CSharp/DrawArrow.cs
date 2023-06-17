using System;
using UnityEngine;

// Token: 0x02000305 RID: 773
public class DrawArrow : MonoBehaviour
{
	// Token: 0x0400179B RID: 6043
	public Color color = new Color(1f, 1f, 1f, 1f);

	// Token: 0x0400179C RID: 6044
	public float length = 0.2f;

	// Token: 0x0400179D RID: 6045
	public float arrowLength = 0.02f;

	// Token: 0x06001E89 RID: 7817 RVA: 0x000D0538 File Offset: 0x000CE738
	private void OnDrawGizmos()
	{
		Vector3 forward = base.transform.forward;
		Vector3 up = Camera.current.transform.up;
		Vector3 position = base.transform.position;
		Vector3 vector = base.transform.position + forward * this.length;
		Gizmos.color = this.color;
		Gizmos.DrawLine(position, vector);
		Gizmos.DrawLine(vector, vector + up * this.arrowLength - forward * this.arrowLength);
		Gizmos.DrawLine(vector, vector - up * this.arrowLength - forward * this.arrowLength);
		Gizmos.DrawLine(vector + up * this.arrowLength - forward * this.arrowLength, vector - up * this.arrowLength - forward * this.arrowLength);
	}
}
