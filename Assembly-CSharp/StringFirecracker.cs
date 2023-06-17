using System;
using UnityEngine;

// Token: 0x02000142 RID: 322
public class StringFirecracker : TimedExplosive
{
	// Token: 0x04000F54 RID: 3924
	public Rigidbody serverRigidBody;

	// Token: 0x04000F55 RID: 3925
	public Rigidbody clientMiddleBody;

	// Token: 0x04000F56 RID: 3926
	public Rigidbody[] clientParts;

	// Token: 0x04000F57 RID: 3927
	public SpringJoint serverClientJoint;

	// Token: 0x04000F58 RID: 3928
	public Transform clientFirecrackerTransform;

	// Token: 0x060016EF RID: 5871 RVA: 0x000AF8CC File Offset: 0x000ADACC
	public override void InitShared()
	{
		base.InitShared();
		if (base.isServer)
		{
			foreach (Rigidbody rigidbody in this.clientParts)
			{
				if (rigidbody != null)
				{
					rigidbody.isKinematic = true;
				}
			}
		}
	}

	// Token: 0x060016F0 RID: 5872 RVA: 0x000AF910 File Offset: 0x000ADB10
	public void CreatePinJoint()
	{
		if (this.serverClientJoint != null)
		{
			return;
		}
		this.serverClientJoint = base.gameObject.AddComponent<SpringJoint>();
		this.serverClientJoint.connectedBody = this.clientMiddleBody;
		this.serverClientJoint.autoConfigureConnectedAnchor = false;
		this.serverClientJoint.anchor = Vector3.zero;
		this.serverClientJoint.connectedAnchor = Vector3.zero;
		this.serverClientJoint.minDistance = 0f;
		this.serverClientJoint.maxDistance = 1f;
		this.serverClientJoint.damper = 1000f;
		this.serverClientJoint.spring = 5000f;
		this.serverClientJoint.enableCollision = false;
		this.serverClientJoint.enablePreprocessing = false;
	}
}
