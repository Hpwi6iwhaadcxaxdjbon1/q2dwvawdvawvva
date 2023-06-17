using System;
using UnityEngine;

// Token: 0x0200016B RID: 363
[CreateAssetMenu(menuName = "Rust/Delivery Drone Config")]
public class DeliveryDroneConfig : BaseScriptableObject
{
	// Token: 0x04001028 RID: 4136
	public Vector3 vendingMachineOffset = new Vector3(0f, 1f, 1f);

	// Token: 0x04001029 RID: 4137
	public float maxDistanceFromVendingMachine = 1f;

	// Token: 0x0400102A RID: 4138
	public Vector3 halfExtents = new Vector3(0.5f, 0.5f, 0.5f);

	// Token: 0x0400102B RID: 4139
	public float testHeight = 200f;

	// Token: 0x0400102C RID: 4140
	public LayerMask layerMask = 27328768;

	// Token: 0x0600176B RID: 5995 RVA: 0x000B22EC File Offset: 0x000B04EC
	public void FindDescentPoints(VendingMachine vendingMachine, float currentY, out Vector3 waitPosition, out Vector3 descendPosition)
	{
		float num = this.maxDistanceFromVendingMachine / 4f;
		for (int i = 0; i <= 4; i++)
		{
			Vector3 b = Vector3.forward * (num * (float)i);
			Vector3 vector = vendingMachine.transform.TransformPoint(this.vendingMachineOffset + b);
			Vector3 vector2 = vector + Vector3.up * this.testHeight;
			RaycastHit raycastHit;
			if (!Physics.BoxCast(vector2, this.halfExtents, Vector3.down, out raycastHit, vendingMachine.transform.rotation, this.testHeight, this.layerMask))
			{
				waitPosition = vector;
				descendPosition = vector2.WithY(currentY);
				return;
			}
			if (i == 4)
			{
				waitPosition = vector2 + Vector3.down * (raycastHit.distance - this.halfExtents.y * 2f);
				descendPosition = vector2.WithY(currentY);
				return;
			}
		}
		throw new Exception("Bug: FindDescentPoint didn't return a fallback value");
	}

	// Token: 0x0600176C RID: 5996 RVA: 0x000B23F0 File Offset: 0x000B05F0
	public bool IsVendingMachineAccessible(VendingMachine vendingMachine, Vector3 offset, out RaycastHit hitInfo)
	{
		Vector3 vector = vendingMachine.transform.TransformPoint(offset);
		return !Physics.BoxCast(vector + Vector3.up * this.testHeight, this.halfExtents, Vector3.down, out hitInfo, vendingMachine.transform.rotation, this.testHeight, this.layerMask) && vendingMachine.IsVisibleAndCanSee(vector, 2f);
	}
}
