using System;
using UnityEngine;

// Token: 0x02000595 RID: 1429
public class TriggerRagdollRelocate : TriggerBase
{
	// Token: 0x04002348 RID: 9032
	public Transform targetLocation;

	// Token: 0x06002B93 RID: 11155 RVA: 0x001080A4 File Offset: 0x001062A4
	internal override void OnObjectAdded(GameObject obj, Collider col)
	{
		base.OnObjectAdded(obj, col);
		BaseEntity baseEntity = obj.transform.ToBaseEntity();
		if (baseEntity != null && baseEntity.isServer)
		{
			this.RepositionTransform(baseEntity.transform);
		}
		Ragdoll componentInParent = obj.GetComponentInParent<Ragdoll>();
		if (componentInParent != null)
		{
			this.RepositionTransform(componentInParent.transform);
			foreach (Rigidbody rigidbody in componentInParent.rigidbodies)
			{
				if (rigidbody.transform.position.y < base.transform.position.y)
				{
					this.RepositionTransform(rigidbody.transform);
				}
			}
		}
	}

	// Token: 0x06002B94 RID: 11156 RVA: 0x0010816C File Offset: 0x0010636C
	private void RepositionTransform(Transform t)
	{
		Vector3 position = this.targetLocation.InverseTransformPoint(t.position);
		position.y = 0f;
		position = this.targetLocation.TransformPoint(position);
		t.position = position;
	}
}
