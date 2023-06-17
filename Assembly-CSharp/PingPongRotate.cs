using System;
using UnityEngine;

// Token: 0x0200030D RID: 781
public class PingPongRotate : MonoBehaviour
{
	// Token: 0x040017B0 RID: 6064
	public Vector3 rotationSpeed = Vector3.zero;

	// Token: 0x040017B1 RID: 6065
	public Vector3 offset = Vector3.zero;

	// Token: 0x040017B2 RID: 6066
	public Vector3 rotationAmount = Vector3.zero;

	// Token: 0x06001EA4 RID: 7844 RVA: 0x000D0C94 File Offset: 0x000CEE94
	private void Update()
	{
		Quaternion quaternion = Quaternion.identity;
		for (int i = 0; i < 3; i++)
		{
			quaternion *= this.GetRotation(i);
		}
		base.transform.rotation = quaternion;
	}

	// Token: 0x06001EA5 RID: 7845 RVA: 0x000D0CD0 File Offset: 0x000CEED0
	public Quaternion GetRotation(int index)
	{
		Vector3 axis = Vector3.zero;
		if (index == 0)
		{
			axis = Vector3.right;
		}
		else if (index == 1)
		{
			axis = Vector3.up;
		}
		else if (index == 2)
		{
			axis = Vector3.forward;
		}
		return Quaternion.AngleAxis(Mathf.Sin((this.offset[index] + Time.time) * this.rotationSpeed[index]) * this.rotationAmount[index], axis);
	}
}
