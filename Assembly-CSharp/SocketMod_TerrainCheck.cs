using System;
using UnityEngine;

// Token: 0x0200027C RID: 636
public class SocketMod_TerrainCheck : SocketMod
{
	// Token: 0x04001585 RID: 5509
	public bool wantsInTerrain = true;

	// Token: 0x06001CCF RID: 7375 RVA: 0x000C7C10 File Offset: 0x000C5E10
	private void OnDrawGizmos()
	{
		Gizmos.matrix = base.transform.localToWorldMatrix;
		bool flag = SocketMod_TerrainCheck.IsInTerrain(base.transform.position);
		if (!this.wantsInTerrain)
		{
			flag = !flag;
		}
		Gizmos.color = (flag ? Color.green : Color.red);
		Gizmos.DrawSphere(Vector3.zero, 0.1f);
	}

	// Token: 0x06001CD0 RID: 7376 RVA: 0x000C7C70 File Offset: 0x000C5E70
	public static bool IsInTerrain(Vector3 vPoint)
	{
		if (TerrainMeta.OutOfBounds(vPoint))
		{
			return false;
		}
		if (!TerrainMeta.Collision || !TerrainMeta.Collision.GetIgnore(vPoint, 0.01f))
		{
			foreach (Terrain terrain in Terrain.activeTerrains)
			{
				if (terrain.SampleHeight(vPoint) + terrain.transform.position.y > vPoint.y)
				{
					return true;
				}
			}
		}
		return Physics.Raycast(new Ray(vPoint + Vector3.up * 3f, Vector3.down), 3f, 65536);
	}

	// Token: 0x06001CD1 RID: 7377 RVA: 0x000C7D14 File Offset: 0x000C5F14
	public override bool DoCheck(Construction.Placement place)
	{
		if (SocketMod_TerrainCheck.IsInTerrain(place.position + place.rotation * this.worldPosition) == this.wantsInTerrain)
		{
			return true;
		}
		Construction.lastPlacementError = this.fullName + ": not in terrain";
		return false;
	}
}
