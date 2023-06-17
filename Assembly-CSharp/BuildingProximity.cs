using System;
using System.Collections.Generic;
using Facepunch;
using UnityEngine;

// Token: 0x02000250 RID: 592
public class BuildingProximity : PrefabAttribute
{
	// Token: 0x040014DD RID: 5341
	private const float check_radius = 2f;

	// Token: 0x040014DE RID: 5342
	private const float check_forgiveness = 0.01f;

	// Token: 0x040014DF RID: 5343
	private const float foundation_width = 3f;

	// Token: 0x040014E0 RID: 5344
	private const float foundation_extents = 1.5f;

	// Token: 0x06001C27 RID: 7207 RVA: 0x000C432C File Offset: 0x000C252C
	public static bool Check(BasePlayer player, Construction construction, Vector3 position, Quaternion rotation)
	{
		OBB obb = new OBB(position, rotation, construction.bounds);
		float radius = obb.extents.magnitude + 2f;
		List<BuildingBlock> list = Pool.GetList<BuildingBlock>();
		Vis.Entities<BuildingBlock>(obb.position, radius, list, 2097152, QueryTriggerInteraction.Collide);
		uint num = 0U;
		for (int i = 0; i < list.Count; i++)
		{
			BuildingBlock buildingBlock = list[i];
			Construction blockDefinition = buildingBlock.blockDefinition;
			Vector3 position2 = buildingBlock.transform.position;
			Quaternion rotation2 = buildingBlock.transform.rotation;
			BuildingProximity.ProximityInfo proximity = BuildingProximity.GetProximity(construction, position, rotation, blockDefinition, position2, rotation2);
			BuildingProximity.ProximityInfo proximity2 = BuildingProximity.GetProximity(blockDefinition, position2, rotation2, construction, position, rotation);
			BuildingProximity.ProximityInfo proximityInfo = default(BuildingProximity.ProximityInfo);
			proximityInfo.hit = (proximity.hit || proximity2.hit);
			proximityInfo.connection = (proximity.connection || proximity2.connection);
			if (proximity.sqrDist <= proximity2.sqrDist)
			{
				proximityInfo.line = proximity.line;
				proximityInfo.sqrDist = proximity.sqrDist;
			}
			else
			{
				proximityInfo.line = proximity2.line;
				proximityInfo.sqrDist = proximity2.sqrDist;
			}
			if (proximityInfo.connection)
			{
				BuildingManager.Building building = buildingBlock.GetBuilding();
				if (building != null)
				{
					BuildingPrivlidge dominatingBuildingPrivilege = building.GetDominatingBuildingPrivilege();
					if (dominatingBuildingPrivilege != null)
					{
						if (!construction.canBypassBuildingPermission && !dominatingBuildingPrivilege.IsAuthed(player))
						{
							Construction.lastPlacementError = "Cannot attach to unauthorized building";
							Pool.FreeList<BuildingBlock>(ref list);
							return true;
						}
						if (num == 0U)
						{
							num = building.ID;
						}
						else if (num != building.ID)
						{
							if (!dominatingBuildingPrivilege.IsAuthed(player))
							{
								Construction.lastPlacementError = "Cannot attach to unauthorized building";
							}
							else
							{
								Construction.lastPlacementError = "Cannot connect two buildings with cupboards";
							}
							Pool.FreeList<BuildingBlock>(ref list);
							return true;
						}
					}
				}
			}
			if (proximityInfo.hit)
			{
				Vector3 vector = proximityInfo.line.point1 - proximityInfo.line.point0;
				if (Mathf.Abs(vector.y) <= 1.49f && vector.Magnitude2D() <= 1.49f)
				{
					Construction.lastPlacementError = "Too close to another building";
					Pool.FreeList<BuildingBlock>(ref list);
					return true;
				}
			}
		}
		Pool.FreeList<BuildingBlock>(ref list);
		return false;
	}

	// Token: 0x06001C28 RID: 7208 RVA: 0x000C456C File Offset: 0x000C276C
	private static BuildingProximity.ProximityInfo GetProximity(Construction construction1, Vector3 position1, Quaternion rotation1, Construction construction2, Vector3 position2, Quaternion rotation2)
	{
		BuildingProximity.ProximityInfo proximityInfo = default(BuildingProximity.ProximityInfo);
		proximityInfo.hit = false;
		proximityInfo.connection = false;
		proximityInfo.line = default(Line);
		proximityInfo.sqrDist = float.MaxValue;
		for (int i = 0; i < construction1.allSockets.Length; i++)
		{
			ConstructionSocket constructionSocket = construction1.allSockets[i] as ConstructionSocket;
			if (!(constructionSocket == null))
			{
				for (int j = 0; j < construction2.allSockets.Length; j++)
				{
					Socket_Base socket = construction2.allSockets[j];
					if (constructionSocket.CanConnect(position1, rotation1, socket, position2, rotation2))
					{
						proximityInfo.connection = true;
						return proximityInfo;
					}
				}
			}
		}
		if (construction1.isServer)
		{
			for (int k = 0; k < construction1.allSockets.Length; k++)
			{
				NeighbourSocket neighbourSocket = construction1.allSockets[k] as NeighbourSocket;
				if (!(neighbourSocket == null))
				{
					for (int l = 0; l < construction2.allSockets.Length; l++)
					{
						Socket_Base socket2 = construction2.allSockets[l];
						if (neighbourSocket.CanConnect(position1, rotation1, socket2, position2, rotation2))
						{
							proximityInfo.connection = true;
							return proximityInfo;
						}
					}
				}
			}
		}
		if (!proximityInfo.connection && construction1.allProximities.Length != 0)
		{
			for (int m = 0; m < construction1.allSockets.Length; m++)
			{
				ConstructionSocket constructionSocket2 = construction1.allSockets[m] as ConstructionSocket;
				if (!(constructionSocket2 == null) && constructionSocket2.socketType == ConstructionSocket.Type.Wall)
				{
					Vector3 selectPivot = constructionSocket2.GetSelectPivot(position1, rotation1);
					for (int n = 0; n < construction2.allProximities.Length; n++)
					{
						Vector3 selectPivot2 = construction2.allProximities[n].GetSelectPivot(position2, rotation2);
						Line line = new Line(selectPivot, selectPivot2);
						float sqrMagnitude = (line.point1 - line.point0).sqrMagnitude;
						if (sqrMagnitude < proximityInfo.sqrDist)
						{
							proximityInfo.hit = true;
							proximityInfo.line = line;
							proximityInfo.sqrDist = sqrMagnitude;
						}
					}
				}
			}
		}
		return proximityInfo;
	}

	// Token: 0x06001C29 RID: 7209 RVA: 0x000C4763 File Offset: 0x000C2963
	public Vector3 GetSelectPivot(Vector3 position, Quaternion rotation)
	{
		return position + rotation * this.worldPosition;
	}

	// Token: 0x06001C2A RID: 7210 RVA: 0x000C4777 File Offset: 0x000C2977
	protected override Type GetIndexedType()
	{
		return typeof(BuildingProximity);
	}

	// Token: 0x02000C81 RID: 3201
	private struct ProximityInfo
	{
		// Token: 0x0400438A RID: 17290
		public bool hit;

		// Token: 0x0400438B RID: 17291
		public bool connection;

		// Token: 0x0400438C RID: 17292
		public Line line;

		// Token: 0x0400438D RID: 17293
		public float sqrDist;
	}
}
