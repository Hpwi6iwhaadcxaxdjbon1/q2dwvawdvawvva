﻿using System;
using ConVar;
using UnityEngine;
using UnityEngine.AI;

// Token: 0x020004EA RID: 1258
public class ServerBuildingManager : BuildingManager
{
	// Token: 0x040020B5 RID: 8373
	private int decayTickBuildingIndex;

	// Token: 0x040020B6 RID: 8374
	private int decayTickEntityIndex;

	// Token: 0x040020B7 RID: 8375
	private int decayTickWorldIndex;

	// Token: 0x040020B8 RID: 8376
	private int navmeshCarveTickBuildingIndex;

	// Token: 0x040020B9 RID: 8377
	private uint maxBuildingID;

	// Token: 0x060028A0 RID: 10400 RVA: 0x000FA634 File Offset: 0x000F8834
	public void CheckSplit(DecayEntity ent)
	{
		if (ent.buildingID == 0U)
		{
			return;
		}
		BuildingManager.Building building = ent.GetBuilding();
		if (building == null)
		{
			return;
		}
		if (this.ShouldSplit(building))
		{
			this.Split(building);
		}
	}

	// Token: 0x060028A1 RID: 10401 RVA: 0x000FA668 File Offset: 0x000F8868
	private bool ShouldSplit(BuildingManager.Building building)
	{
		if (building.HasBuildingBlocks())
		{
			building.buildingBlocks[0].EntityLinkBroadcast();
			using (ListHashSet<BuildingBlock>.Enumerator enumerator = building.buildingBlocks.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (!enumerator.Current.ReceivedEntityLinkBroadcast())
					{
						return true;
					}
				}
			}
			return false;
		}
		return false;
	}

	// Token: 0x060028A2 RID: 10402 RVA: 0x000FA6DC File Offset: 0x000F88DC
	private void Split(BuildingManager.Building building)
	{
		while (building.HasBuildingBlocks())
		{
			BaseEntity baseEntity = building.buildingBlocks[0];
			uint newID = BuildingManager.server.NewBuildingID();
			baseEntity.EntityLinkBroadcast<BuildingBlock>(delegate(BuildingBlock b)
			{
				b.AttachToBuilding(newID);
			});
		}
		while (building.HasBuildingPrivileges())
		{
			BuildingPrivlidge buildingPrivlidge = building.buildingPrivileges[0];
			BuildingBlock nearbyBuildingBlock = buildingPrivlidge.GetNearbyBuildingBlock();
			buildingPrivlidge.AttachToBuilding(nearbyBuildingBlock ? nearbyBuildingBlock.buildingID : 0U);
		}
		while (building.HasDecayEntities())
		{
			DecayEntity decayEntity = building.decayEntities[0];
			BuildingBlock nearbyBuildingBlock2 = decayEntity.GetNearbyBuildingBlock();
			decayEntity.AttachToBuilding(nearbyBuildingBlock2 ? nearbyBuildingBlock2.buildingID : 0U);
		}
		if (AI.nav_carve_use_building_optimization)
		{
			building.isNavMeshCarvingDirty = true;
			int num = 2;
			this.UpdateNavMeshCarver(building, ref num, 0);
		}
	}

	// Token: 0x060028A3 RID: 10403 RVA: 0x000FA7A8 File Offset: 0x000F89A8
	public void CheckMerge(DecayEntity ent)
	{
		if (ent.buildingID == 0U)
		{
			return;
		}
		BuildingManager.Building building = ent.GetBuilding();
		if (building == null)
		{
			return;
		}
		ent.EntityLinkMessage<BuildingBlock>(delegate(BuildingBlock b)
		{
			BuildingManager.Building building;
			if (b.buildingID != building.ID)
			{
				building = b.GetBuilding();
				if (building != null)
				{
					this.Merge(building, building);
				}
			}
		});
		if (AI.nav_carve_use_building_optimization)
		{
			building.isNavMeshCarvingDirty = true;
			int num = 2;
			this.UpdateNavMeshCarver(building, ref num, 0);
		}
	}

	// Token: 0x060028A4 RID: 10404 RVA: 0x000FA818 File Offset: 0x000F8A18
	private void Merge(BuildingManager.Building building1, BuildingManager.Building building2)
	{
		while (building2.HasDecayEntities())
		{
			building2.decayEntities[0].AttachToBuilding(building1.ID);
		}
		if (AI.nav_carve_use_building_optimization)
		{
			building1.isNavMeshCarvingDirty = true;
			building2.isNavMeshCarvingDirty = true;
			int num = 3;
			this.UpdateNavMeshCarver(building1, ref num, 0);
			this.UpdateNavMeshCarver(building1, ref num, 0);
		}
	}

	// Token: 0x060028A5 RID: 10405 RVA: 0x000FA874 File Offset: 0x000F8A74
	public void Cycle()
	{
		using (TimeWarning.New("StabilityCheckQueue", 0))
		{
			StabilityEntity.stabilityCheckQueue.RunQueue((double)Stability.stabilityqueue);
		}
		using (TimeWarning.New("UpdateSurroundingsQueue", 0))
		{
			StabilityEntity.updateSurroundingsQueue.RunQueue((double)Stability.surroundingsqueue);
		}
		using (TimeWarning.New("UpdateSkinQueue", 0))
		{
			BuildingBlock.updateSkinQueueServer.RunQueue(1.0);
		}
		using (TimeWarning.New("BuildingDecayTick", 0))
		{
			int num = 5;
			BufferList<BuildingManager.Building> values = this.buildingDictionary.Values;
			int num2 = this.decayTickBuildingIndex;
			while (num2 < values.Count && num > 0)
			{
				BufferList<DecayEntity> values2 = values[num2].decayEntities.Values;
				int num3 = this.decayTickEntityIndex;
				while (num3 < values2.Count && num > 0)
				{
					values2[num3].DecayTick();
					num--;
					if (num <= 0)
					{
						this.decayTickBuildingIndex = num2;
						this.decayTickEntityIndex = num3;
					}
					num3++;
				}
				if (num > 0)
				{
					this.decayTickEntityIndex = 0;
				}
				num2++;
			}
			if (num > 0)
			{
				this.decayTickBuildingIndex = 0;
			}
		}
		using (TimeWarning.New("WorldDecayTick", 0))
		{
			int num4 = 5;
			BufferList<DecayEntity> values3 = this.decayEntities.Values;
			int num5 = this.decayTickWorldIndex;
			while (num5 < values3.Count && num4 > 0)
			{
				values3[num5].DecayTick();
				num4--;
				if (num4 <= 0)
				{
					this.decayTickWorldIndex = num5;
				}
				num5++;
			}
			if (num4 > 0)
			{
				this.decayTickWorldIndex = 0;
			}
		}
		if (AI.nav_carve_use_building_optimization)
		{
			using (TimeWarning.New("NavMeshCarving", 0))
			{
				int num6 = 5;
				BufferList<BuildingManager.Building> values4 = this.buildingDictionary.Values;
				int num7 = this.navmeshCarveTickBuildingIndex;
				while (num7 < values4.Count && num6 > 0)
				{
					BuildingManager.Building building = values4[num7];
					this.UpdateNavMeshCarver(building, ref num6, num7);
					num7++;
				}
				if (num6 > 0)
				{
					this.navmeshCarveTickBuildingIndex = 0;
				}
			}
		}
	}

	// Token: 0x060028A6 RID: 10406 RVA: 0x000FAADC File Offset: 0x000F8CDC
	public void UpdateNavMeshCarver(BuildingManager.Building building, ref int ticks, int i)
	{
		if (!AI.nav_carve_use_building_optimization || (!building.isNavMeshCarveOptimized && building.navmeshCarvers.Count < AI.nav_carve_min_building_blocks_to_apply_optimization))
		{
			return;
		}
		if (building.isNavMeshCarvingDirty)
		{
			building.isNavMeshCarvingDirty = false;
			if (building.navmeshCarvers == null)
			{
				if (building.buildingNavMeshObstacle != null)
				{
					UnityEngine.Object.Destroy(building.buildingNavMeshObstacle.gameObject);
					building.buildingNavMeshObstacle = null;
					building.isNavMeshCarveOptimized = false;
				}
				return;
			}
			Vector3 vector = new Vector3(global::World.Size, global::World.Size, global::World.Size);
			Vector3 vector2 = new Vector3((float)(-(float)((ulong)global::World.Size)), (float)(-(float)((ulong)global::World.Size)), (float)(-(float)((ulong)global::World.Size)));
			int count = building.navmeshCarvers.Count;
			if (count > 0)
			{
				for (int j = 0; j < count; j++)
				{
					NavMeshObstacle navMeshObstacle = building.navmeshCarvers[j];
					if (navMeshObstacle.enabled)
					{
						navMeshObstacle.enabled = false;
					}
					for (int k = 0; k < 3; k++)
					{
						if (navMeshObstacle.transform.position[k] < vector[k])
						{
							vector[k] = navMeshObstacle.transform.position[k];
						}
						if (navMeshObstacle.transform.position[k] > vector2[k])
						{
							vector2[k] = navMeshObstacle.transform.position[k];
						}
					}
				}
				Vector3 vector3 = (vector2 + vector) * 0.5f;
				Vector3 vector4 = Vector3.zero;
				float num = Mathf.Abs(vector3.x - vector.x);
				float num2 = Mathf.Abs(vector3.y - vector.y);
				float num3 = Mathf.Abs(vector3.z - vector.z);
				float num4 = Mathf.Abs(vector2.x - vector3.x);
				float num5 = Mathf.Abs(vector2.y - vector3.y);
				float num6 = Mathf.Abs(vector2.z - vector3.z);
				vector4.x = Mathf.Max((num > num4) ? num : num4, AI.nav_carve_min_base_size);
				vector4.y = Mathf.Max((num2 > num5) ? num2 : num5, AI.nav_carve_min_base_size);
				vector4.z = Mathf.Max((num3 > num6) ? num3 : num6, AI.nav_carve_min_base_size);
				if (count < 10)
				{
					vector4 *= AI.nav_carve_size_multiplier;
				}
				else
				{
					vector4 *= AI.nav_carve_size_multiplier - 1f;
				}
				if (building.navmeshCarvers.Count > 0)
				{
					if (building.buildingNavMeshObstacle == null)
					{
						building.buildingNavMeshObstacle = new GameObject(string.Format("Building ({0}) NavMesh Carver", building.ID)).AddComponent<NavMeshObstacle>();
						building.buildingNavMeshObstacle.enabled = false;
						building.buildingNavMeshObstacle.carving = true;
						building.buildingNavMeshObstacle.shape = NavMeshObstacleShape.Box;
						building.buildingNavMeshObstacle.height = AI.nav_carve_height;
						building.isNavMeshCarveOptimized = true;
					}
					if (building.buildingNavMeshObstacle != null)
					{
						building.buildingNavMeshObstacle.transform.position = vector3;
						building.buildingNavMeshObstacle.size = vector4;
						if (!building.buildingNavMeshObstacle.enabled)
						{
							building.buildingNavMeshObstacle.enabled = true;
						}
					}
				}
			}
			else if (building.buildingNavMeshObstacle != null)
			{
				UnityEngine.Object.Destroy(building.buildingNavMeshObstacle.gameObject);
				building.buildingNavMeshObstacle = null;
				building.isNavMeshCarveOptimized = false;
			}
			ticks--;
			if (ticks <= 0)
			{
				this.navmeshCarveTickBuildingIndex = i;
			}
		}
	}

	// Token: 0x060028A7 RID: 10407 RVA: 0x000FAE88 File Offset: 0x000F9088
	public uint NewBuildingID()
	{
		uint result = this.maxBuildingID + 1U;
		this.maxBuildingID = result;
		return result;
	}

	// Token: 0x060028A8 RID: 10408 RVA: 0x000FAEA6 File Offset: 0x000F90A6
	public void LoadBuildingID(uint id)
	{
		this.maxBuildingID = Mathx.Max(this.maxBuildingID, id);
	}

	// Token: 0x060028A9 RID: 10409 RVA: 0x000FAEBA File Offset: 0x000F90BA
	protected override BuildingManager.Building CreateBuilding(uint id)
	{
		return new BuildingManager.Building
		{
			ID = id
		};
	}

	// Token: 0x060028AA RID: 10410 RVA: 0x000FAEC8 File Offset: 0x000F90C8
	protected override void DisposeBuilding(ref BuildingManager.Building building)
	{
		building = null;
	}
}
