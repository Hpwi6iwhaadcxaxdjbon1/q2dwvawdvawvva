using System;
using UnityEngine;

// Token: 0x02000279 RID: 633
public class SocketMod_PhysicMaterial : SocketMod
{
	// Token: 0x0400157A RID: 5498
	public PhysicMaterial[] ValidMaterials;

	// Token: 0x0400157B RID: 5499
	private PhysicMaterial foundMaterial;

	// Token: 0x06001CC6 RID: 7366 RVA: 0x000C7834 File Offset: 0x000C5A34
	public override bool DoCheck(Construction.Placement place)
	{
		RaycastHit raycastHit;
		if (Physics.Raycast(place.position + place.rotation.eulerAngles.normalized * 0.5f, -place.rotation.eulerAngles.normalized, out raycastHit, 1f, 27328512, QueryTriggerInteraction.Ignore))
		{
			this.foundMaterial = raycastHit.collider.GetMaterialAt(raycastHit.point);
			PhysicMaterial[] validMaterials = this.ValidMaterials;
			for (int i = 0; i < validMaterials.Length; i++)
			{
				if (validMaterials[i] == this.foundMaterial)
				{
					return true;
				}
			}
		}
		return false;
	}
}
