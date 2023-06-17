using System;
using UnityEngine;

// Token: 0x02000272 RID: 626
public class SocketMod_AreaCheck : SocketMod
{
	// Token: 0x04001560 RID: 5472
	public Bounds bounds = new Bounds(Vector3.zero, Vector3.one * 0.1f);

	// Token: 0x04001561 RID: 5473
	public LayerMask layerMask;

	// Token: 0x04001562 RID: 5474
	public bool wantsInside = true;

	// Token: 0x06001CAD RID: 7341 RVA: 0x000C6E20 File Offset: 0x000C5020
	private void OnDrawGizmosSelected()
	{
		Gizmos.matrix = base.transform.localToWorldMatrix;
		bool flag = true;
		if (!this.wantsInside)
		{
			flag = !flag;
		}
		Gizmos.color = (flag ? Color.green.WithAlpha(0.5f) : Color.red.WithAlpha(0.5f));
		Gizmos.DrawCube(this.bounds.center, this.bounds.size);
	}

	// Token: 0x06001CAE RID: 7342 RVA: 0x000C6E8F File Offset: 0x000C508F
	public static bool IsInArea(Vector3 position, Quaternion rotation, Bounds bounds, LayerMask layerMask, BaseEntity entity = null)
	{
		return GamePhysics.CheckOBBAndEntity(new OBB(position, rotation, bounds), layerMask.value, QueryTriggerInteraction.UseGlobal, entity);
	}

	// Token: 0x06001CAF RID: 7343 RVA: 0x000C6EA8 File Offset: 0x000C50A8
	public bool DoCheck(Vector3 position, Quaternion rotation, BaseEntity entity = null)
	{
		Vector3 position2 = position + rotation * this.worldPosition;
		Quaternion rotation2 = rotation * this.worldRotation;
		return SocketMod_AreaCheck.IsInArea(position2, rotation2, this.bounds, this.layerMask, entity) == this.wantsInside;
	}

	// Token: 0x06001CB0 RID: 7344 RVA: 0x000C6EEF File Offset: 0x000C50EF
	public override bool DoCheck(Construction.Placement place)
	{
		if (this.DoCheck(place.position, place.rotation, null))
		{
			return true;
		}
		Construction.lastPlacementError = "Failed Check: IsInArea (" + this.hierachyName + ")";
		return false;
	}
}
