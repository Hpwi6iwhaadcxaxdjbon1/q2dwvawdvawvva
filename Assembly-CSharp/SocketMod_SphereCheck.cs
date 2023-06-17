using System;
using UnityEngine;

// Token: 0x0200027B RID: 635
public class SocketMod_SphereCheck : SocketMod
{
	// Token: 0x04001580 RID: 5504
	public float sphereRadius = 1f;

	// Token: 0x04001581 RID: 5505
	public LayerMask layerMask;

	// Token: 0x04001582 RID: 5506
	public bool wantsCollide;

	// Token: 0x04001583 RID: 5507
	public static Translate.Phrase Error_WantsCollideConstruction = new Translate.Phrase("error_wantsconstruction", "Must be placed on construction");

	// Token: 0x04001584 RID: 5508
	public static Translate.Phrase Error_DoesNotWantCollideConstruction = new Translate.Phrase("error_doesnotwantconstruction", "Cannot be placed on construction");

	// Token: 0x06001CCB RID: 7371 RVA: 0x000C7A38 File Offset: 0x000C5C38
	private void OnDrawGizmosSelected()
	{
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.color = (this.wantsCollide ? new Color(0f, 1f, 0f, 0.7f) : new Color(1f, 0f, 0f, 0.7f));
		Gizmos.DrawSphere(Vector3.zero, this.sphereRadius);
	}

	// Token: 0x06001CCC RID: 7372 RVA: 0x000C7AA8 File Offset: 0x000C5CA8
	public override bool DoCheck(Construction.Placement place)
	{
		Vector3 position = place.position + place.rotation * this.worldPosition;
		if (this.wantsCollide == GamePhysics.CheckSphere(position, this.sphereRadius, this.layerMask.value, QueryTriggerInteraction.UseGlobal))
		{
			return true;
		}
		bool flag = false;
		Construction.lastPlacementError = "Failed Check: Sphere Test (" + this.hierachyName + ")";
		if (this.layerMask == 2097152 && this.wantsCollide)
		{
			Construction.lastPlacementError = SocketMod_SphereCheck.Error_WantsCollideConstruction.translated;
			if (flag)
			{
				Construction.lastPlacementError = Construction.lastPlacementError + " (" + this.hierachyName + ")";
			}
		}
		else if (!this.wantsCollide && (this.layerMask & 2097152) == 2097152)
		{
			Construction.lastPlacementError = SocketMod_SphereCheck.Error_DoesNotWantCollideConstruction.translated;
			if (flag)
			{
				Construction.lastPlacementError = Construction.lastPlacementError + " (" + this.hierachyName + ")";
			}
		}
		else
		{
			Construction.lastPlacementError = "Failed Check: Sphere Test (" + this.hierachyName + ")";
		}
		return false;
	}
}
