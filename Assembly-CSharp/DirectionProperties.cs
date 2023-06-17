using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200025C RID: 604
public class DirectionProperties : PrefabAttribute
{
	// Token: 0x04001516 RID: 5398
	private const float radius = 200f;

	// Token: 0x04001517 RID: 5399
	public Bounds bounds = new Bounds(Vector3.zero, Vector3.zero);

	// Token: 0x04001518 RID: 5400
	public ProtectionProperties extraProtection;

	// Token: 0x06001C5E RID: 7262 RVA: 0x000C59F2 File Offset: 0x000C3BF2
	protected override Type GetIndexedType()
	{
		return typeof(DirectionProperties);
	}

	// Token: 0x06001C5F RID: 7263 RVA: 0x000C5A00 File Offset: 0x000C3C00
	public bool IsWeakspot(Transform tx, HitInfo info)
	{
		if (this.bounds.size == Vector3.zero)
		{
			return false;
		}
		BasePlayer initiatorPlayer = info.InitiatorPlayer;
		if (initiatorPlayer == null)
		{
			return false;
		}
		BaseEntity hitEntity = info.HitEntity;
		if (hitEntity == null)
		{
			return false;
		}
		Matrix4x4 worldToLocalMatrix = tx.worldToLocalMatrix;
		Vector3 b = worldToLocalMatrix.MultiplyPoint3x4(info.PointStart) - this.worldPosition;
		float num = this.worldForward.DotDegrees(b);
		Vector3 target = worldToLocalMatrix.MultiplyPoint3x4(info.HitPositionWorld);
		OBB obb = new OBB(this.worldPosition, this.worldRotation, this.bounds);
		Vector3 position = initiatorPlayer.eyes.position;
		WeakpointProperties[] array = PrefabAttribute.server.FindAll<WeakpointProperties>(hitEntity.prefabID);
		if (array != null && array.Length != 0)
		{
			bool flag = false;
			foreach (WeakpointProperties weakpointProperties in array)
			{
				if ((!weakpointProperties.BlockWhenRoofAttached || this.CheckWeakpointRoof(hitEntity)) && this.IsWeakspotVisible(hitEntity, position, tx.TransformPoint(weakpointProperties.worldPosition)))
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				return false;
			}
		}
		else if (!this.IsWeakspotVisible(hitEntity, position, tx.TransformPoint(obb.position)))
		{
			return false;
		}
		return num > 100f && obb.Contains(target);
	}

	// Token: 0x06001C60 RID: 7264 RVA: 0x000C5B4C File Offset: 0x000C3D4C
	private bool CheckWeakpointRoof(BaseEntity hitEntity)
	{
		foreach (EntityLink entityLink in hitEntity.GetEntityLinks(true))
		{
			if (entityLink.socket is NeighbourSocket)
			{
				using (List<EntityLink>.Enumerator enumerator2 = entityLink.connections.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						BuildingBlock buildingBlock;
						if ((buildingBlock = (enumerator2.Current.owner as BuildingBlock)) != null && (buildingBlock.ShortPrefabName == "roof" || buildingBlock.ShortPrefabName == "roof.triangle"))
						{
							return false;
						}
					}
				}
			}
		}
		return true;
	}

	// Token: 0x06001C61 RID: 7265 RVA: 0x000C5C20 File Offset: 0x000C3E20
	private bool IsWeakspotVisible(BaseEntity hitEntity, Vector3 playerEyes, Vector3 weakspotPos)
	{
		return hitEntity.IsVisible(playerEyes, weakspotPos, float.PositiveInfinity);
	}
}
