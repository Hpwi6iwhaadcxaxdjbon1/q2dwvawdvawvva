using System;
using ConVar;
using UnityEngine.AI;

// Token: 0x020004E9 RID: 1257
public abstract class BuildingManager
{
	// Token: 0x040020B2 RID: 8370
	public static ServerBuildingManager server = new ServerBuildingManager();

	// Token: 0x040020B3 RID: 8371
	protected ListHashSet<DecayEntity> decayEntities = new ListHashSet<DecayEntity>(8);

	// Token: 0x040020B4 RID: 8372
	protected ListDictionary<uint, BuildingManager.Building> buildingDictionary = new ListDictionary<uint, BuildingManager.Building>();

	// Token: 0x06002898 RID: 10392 RVA: 0x000FA50C File Offset: 0x000F870C
	public BuildingManager.Building GetBuilding(uint buildingID)
	{
		BuildingManager.Building result = null;
		this.buildingDictionary.TryGetValue(buildingID, out result);
		return result;
	}

	// Token: 0x06002899 RID: 10393 RVA: 0x000FA52C File Offset: 0x000F872C
	public void Add(DecayEntity ent)
	{
		if (ent.buildingID == 0U)
		{
			if (!this.decayEntities.Contains(ent))
			{
				this.decayEntities.Add(ent);
			}
			return;
		}
		BuildingManager.Building building = this.GetBuilding(ent.buildingID);
		if (building == null)
		{
			building = this.CreateBuilding(ent.buildingID);
			this.buildingDictionary.Add(ent.buildingID, building);
		}
		building.Add(ent);
		building.Dirty();
	}

	// Token: 0x0600289A RID: 10394 RVA: 0x000FA598 File Offset: 0x000F8798
	public void Remove(DecayEntity ent)
	{
		if (ent.buildingID == 0U)
		{
			this.decayEntities.Remove(ent);
			return;
		}
		BuildingManager.Building building = this.GetBuilding(ent.buildingID);
		if (building == null)
		{
			return;
		}
		building.Remove(ent);
		if (building.IsEmpty())
		{
			this.buildingDictionary.Remove(ent.buildingID);
			this.DisposeBuilding(ref building);
			return;
		}
		building.Dirty();
	}

	// Token: 0x0600289B RID: 10395 RVA: 0x000FA5FC File Offset: 0x000F87FC
	public void Clear()
	{
		this.buildingDictionary.Clear();
	}

	// Token: 0x0600289C RID: 10396
	protected abstract BuildingManager.Building CreateBuilding(uint id);

	// Token: 0x0600289D RID: 10397
	protected abstract void DisposeBuilding(ref BuildingManager.Building building);

	// Token: 0x02000D22 RID: 3362
	public class Building
	{
		// Token: 0x0400464F RID: 17999
		public uint ID;

		// Token: 0x04004650 RID: 18000
		public ListHashSet<BuildingPrivlidge> buildingPrivileges = new ListHashSet<BuildingPrivlidge>(8);

		// Token: 0x04004651 RID: 18001
		public ListHashSet<BuildingBlock> buildingBlocks = new ListHashSet<BuildingBlock>(8);

		// Token: 0x04004652 RID: 18002
		public ListHashSet<DecayEntity> decayEntities = new ListHashSet<DecayEntity>(8);

		// Token: 0x04004653 RID: 18003
		public NavMeshObstacle buildingNavMeshObstacle;

		// Token: 0x04004654 RID: 18004
		public ListHashSet<NavMeshObstacle> navmeshCarvers;

		// Token: 0x04004655 RID: 18005
		public bool isNavMeshCarvingDirty;

		// Token: 0x04004656 RID: 18006
		public bool isNavMeshCarveOptimized;

		// Token: 0x06005049 RID: 20553 RVA: 0x001A8EE4 File Offset: 0x001A70E4
		public bool IsEmpty()
		{
			return !this.HasBuildingPrivileges() && !this.HasBuildingBlocks() && !this.HasDecayEntities();
		}

		// Token: 0x0600504A RID: 20554 RVA: 0x001A8F08 File Offset: 0x001A7108
		public BuildingPrivlidge GetDominatingBuildingPrivilege()
		{
			BuildingPrivlidge buildingPrivlidge = null;
			if (this.HasBuildingPrivileges())
			{
				for (int i = 0; i < this.buildingPrivileges.Count; i++)
				{
					BuildingPrivlidge buildingPrivlidge2 = this.buildingPrivileges[i];
					if (!(buildingPrivlidge2 == null) && buildingPrivlidge2.IsOlderThan(buildingPrivlidge))
					{
						buildingPrivlidge = buildingPrivlidge2;
					}
				}
			}
			return buildingPrivlidge;
		}

		// Token: 0x0600504B RID: 20555 RVA: 0x001A8F57 File Offset: 0x001A7157
		public bool HasBuildingPrivileges()
		{
			return this.buildingPrivileges != null && this.buildingPrivileges.Count > 0;
		}

		// Token: 0x0600504C RID: 20556 RVA: 0x001A8F71 File Offset: 0x001A7171
		public bool HasBuildingBlocks()
		{
			return this.buildingBlocks != null && this.buildingBlocks.Count > 0;
		}

		// Token: 0x0600504D RID: 20557 RVA: 0x001A8F8B File Offset: 0x001A718B
		public bool HasDecayEntities()
		{
			return this.decayEntities != null && this.decayEntities.Count > 0;
		}

		// Token: 0x0600504E RID: 20558 RVA: 0x001A8FA5 File Offset: 0x001A71A5
		public void AddBuildingPrivilege(BuildingPrivlidge ent)
		{
			if (ent == null)
			{
				return;
			}
			if (!this.buildingPrivileges.Contains(ent))
			{
				this.buildingPrivileges.Add(ent);
			}
		}

		// Token: 0x0600504F RID: 20559 RVA: 0x001A8FCB File Offset: 0x001A71CB
		public void RemoveBuildingPrivilege(BuildingPrivlidge ent)
		{
			if (ent == null)
			{
				return;
			}
			this.buildingPrivileges.Remove(ent);
		}

		// Token: 0x06005050 RID: 20560 RVA: 0x001A8FE4 File Offset: 0x001A71E4
		public void AddBuildingBlock(BuildingBlock ent)
		{
			if (ent == null)
			{
				return;
			}
			if (!this.buildingBlocks.Contains(ent))
			{
				this.buildingBlocks.Add(ent);
				if (AI.nav_carve_use_building_optimization)
				{
					NavMeshObstacle component = ent.GetComponent<NavMeshObstacle>();
					if (component != null)
					{
						this.isNavMeshCarvingDirty = true;
						if (this.navmeshCarvers == null)
						{
							this.navmeshCarvers = new ListHashSet<NavMeshObstacle>(8);
						}
						this.navmeshCarvers.Add(component);
					}
				}
			}
		}

		// Token: 0x06005051 RID: 20561 RVA: 0x001A9054 File Offset: 0x001A7254
		public void RemoveBuildingBlock(BuildingBlock ent)
		{
			if (ent == null)
			{
				return;
			}
			this.buildingBlocks.Remove(ent);
			if (AI.nav_carve_use_building_optimization && this.navmeshCarvers != null)
			{
				NavMeshObstacle component = ent.GetComponent<NavMeshObstacle>();
				if (component != null)
				{
					this.navmeshCarvers.Remove(component);
					if (this.navmeshCarvers.Count == 0)
					{
						this.navmeshCarvers = null;
					}
					this.isNavMeshCarvingDirty = true;
					if (this.navmeshCarvers == null)
					{
						BuildingManager.Building building = ent.GetBuilding();
						if (building != null)
						{
							int num = 2;
							BuildingManager.server.UpdateNavMeshCarver(building, ref num, 0);
						}
					}
				}
			}
		}

		// Token: 0x06005052 RID: 20562 RVA: 0x001A90E1 File Offset: 0x001A72E1
		public void AddDecayEntity(DecayEntity ent)
		{
			if (ent == null)
			{
				return;
			}
			if (!this.decayEntities.Contains(ent))
			{
				this.decayEntities.Add(ent);
			}
		}

		// Token: 0x06005053 RID: 20563 RVA: 0x001A9107 File Offset: 0x001A7307
		public void RemoveDecayEntity(DecayEntity ent)
		{
			if (ent == null)
			{
				return;
			}
			this.decayEntities.Remove(ent);
		}

		// Token: 0x06005054 RID: 20564 RVA: 0x001A9120 File Offset: 0x001A7320
		public void Add(DecayEntity ent)
		{
			this.AddDecayEntity(ent);
			this.AddBuildingBlock(ent as BuildingBlock);
			this.AddBuildingPrivilege(ent as BuildingPrivlidge);
		}

		// Token: 0x06005055 RID: 20565 RVA: 0x001A9141 File Offset: 0x001A7341
		public void Remove(DecayEntity ent)
		{
			this.RemoveDecayEntity(ent);
			this.RemoveBuildingBlock(ent as BuildingBlock);
			this.RemoveBuildingPrivilege(ent as BuildingPrivlidge);
		}

		// Token: 0x06005056 RID: 20566 RVA: 0x001A9164 File Offset: 0x001A7364
		public void Dirty()
		{
			BuildingPrivlidge dominatingBuildingPrivilege = this.GetDominatingBuildingPrivilege();
			if (dominatingBuildingPrivilege != null)
			{
				dominatingBuildingPrivilege.BuildingDirty();
			}
		}
	}
}
