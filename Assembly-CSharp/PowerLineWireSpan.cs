using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000686 RID: 1670
public class PowerLineWireSpan : MonoBehaviour
{
	// Token: 0x0400273D RID: 10045
	public GameObjectRef wirePrefab;

	// Token: 0x0400273E RID: 10046
	public Transform start;

	// Token: 0x0400273F RID: 10047
	public Transform end;

	// Token: 0x04002740 RID: 10048
	public float WireLength;

	// Token: 0x04002741 RID: 10049
	public List<PowerLineWireConnection> connections = new List<PowerLineWireConnection>();

	// Token: 0x06002FC2 RID: 12226 RVA: 0x0011F0F8 File Offset: 0x0011D2F8
	public void Init(PowerLineWire wire)
	{
		if (this.start && this.end)
		{
			this.WireLength = Vector3.Distance(this.start.position, this.end.position);
			for (int i = 0; i < this.connections.Count; i++)
			{
				Vector3 a = this.start.TransformPoint(this.connections[i].outOffset);
				Vector3 vector = this.end.TransformPoint(this.connections[i].inOffset);
				this.WireLength = (a - vector).magnitude;
				GameObject gameObject = this.wirePrefab.Instantiate(base.transform);
				gameObject.name = "WIRE";
				gameObject.transform.position = Vector3.Lerp(a, vector, 0.5f);
				gameObject.transform.LookAt(vector);
				gameObject.transform.localScale = new Vector3(1f, 1f, Vector3.Distance(a, vector));
				gameObject.SetActive(true);
			}
		}
	}
}
