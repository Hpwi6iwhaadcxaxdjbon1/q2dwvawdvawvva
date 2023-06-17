using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020006A4 RID: 1700
public class TerrainPath : TerrainExtension
{
	// Token: 0x040027AE RID: 10158
	internal List<PathList> Roads = new List<PathList>();

	// Token: 0x040027AF RID: 10159
	internal List<PathList> Rails = new List<PathList>();

	// Token: 0x040027B0 RID: 10160
	internal List<PathList> Rivers = new List<PathList>();

	// Token: 0x040027B1 RID: 10161
	internal List<PathList> Powerlines = new List<PathList>();

	// Token: 0x040027B2 RID: 10162
	internal List<LandmarkInfo> Landmarks = new List<LandmarkInfo>();

	// Token: 0x040027B3 RID: 10163
	internal List<MonumentInfo> Monuments = new List<MonumentInfo>();

	// Token: 0x040027B4 RID: 10164
	internal List<RiverInfo> RiverObjs = new List<RiverInfo>();

	// Token: 0x040027B5 RID: 10165
	internal List<LakeInfo> LakeObjs = new List<LakeInfo>();

	// Token: 0x040027B6 RID: 10166
	internal GameObject DungeonGridRoot;

	// Token: 0x040027B7 RID: 10167
	internal List<DungeonGridInfo> DungeonGridEntrances = new List<DungeonGridInfo>();

	// Token: 0x040027B8 RID: 10168
	internal List<DungeonGridCell> DungeonGridCells = new List<DungeonGridCell>();

	// Token: 0x040027B9 RID: 10169
	internal GameObject DungeonBaseRoot;

	// Token: 0x040027BA RID: 10170
	internal List<DungeonBaseInfo> DungeonBaseEntrances = new List<DungeonBaseInfo>();

	// Token: 0x040027BB RID: 10171
	internal List<Vector3> OceanPatrolClose = new List<Vector3>();

	// Token: 0x040027BC RID: 10172
	internal List<Vector3> OceanPatrolFar = new List<Vector3>();

	// Token: 0x040027BD RID: 10173
	private Dictionary<string, List<PowerlineNode>> wires = new Dictionary<string, List<PowerlineNode>>();

	// Token: 0x06003120 RID: 12576 RVA: 0x00125BA0 File Offset: 0x00123DA0
	public override void PostSetup()
	{
		foreach (PathList pathList in this.Roads)
		{
			pathList.ProcgenStartNode = null;
			pathList.ProcgenEndNode = null;
		}
		foreach (PathList pathList2 in this.Rails)
		{
			pathList2.ProcgenStartNode = null;
			pathList2.ProcgenEndNode = null;
		}
		foreach (PathList pathList3 in this.Rivers)
		{
			pathList3.ProcgenStartNode = null;
			pathList3.ProcgenEndNode = null;
		}
		foreach (PathList pathList4 in this.Powerlines)
		{
			pathList4.ProcgenStartNode = null;
			pathList4.ProcgenEndNode = null;
		}
	}

	// Token: 0x06003121 RID: 12577 RVA: 0x00125CD0 File Offset: 0x00123ED0
	public void Clear()
	{
		this.Roads.Clear();
		this.Rails.Clear();
		this.Rivers.Clear();
		this.Powerlines.Clear();
	}

	// Token: 0x06003122 RID: 12578 RVA: 0x00125D00 File Offset: 0x00123F00
	public T FindClosest<T>(List<T> list, Vector3 pos) where T : MonoBehaviour
	{
		T result = default(T);
		float num = float.MaxValue;
		foreach (T t in list)
		{
			float num2 = Vector3Ex.Distance2D(t.transform.position, pos);
			if (num2 < num)
			{
				result = t;
				num = num2;
			}
		}
		return result;
	}

	// Token: 0x06003123 RID: 12579 RVA: 0x00125D78 File Offset: 0x00123F78
	public static int[,] CreatePowerlineCostmap(ref uint seed)
	{
		float radius = 5f;
		int num = (int)(World.Size / 7.5f);
		TerrainPlacementMap placementMap = TerrainMeta.PlacementMap;
		TerrainHeightMap heightMap = TerrainMeta.HeightMap;
		TerrainTopologyMap topologyMap = TerrainMeta.TopologyMap;
		int[,] array = new int[num, num];
		for (int i = 0; i < num; i++)
		{
			float normZ = ((float)i + 0.5f) / (float)num;
			for (int j = 0; j < num; j++)
			{
				float normX = ((float)j + 0.5f) / (float)num;
				float slope = heightMap.GetSlope(normX, normZ);
				int topology = topologyMap.GetTopology(normX, normZ, radius);
				int num2 = 2295174;
				int num3 = 1628160;
				int num4 = 512;
				if ((topology & num2) != 0)
				{
					array[j, i] = int.MaxValue;
				}
				else if ((topology & num3) != 0 || placementMap.GetBlocked(normX, normZ, radius))
				{
					array[j, i] = 2500;
				}
				else if ((topology & num4) != 0)
				{
					array[j, i] = 1000;
				}
				else
				{
					array[j, i] = 1 + (int)(slope * slope * 10f);
				}
			}
		}
		return array;
	}

	// Token: 0x06003124 RID: 12580 RVA: 0x00125EA0 File Offset: 0x001240A0
	public static int[,] CreateRoadCostmap(ref uint seed)
	{
		float radius = 5f;
		int num = (int)(World.Size / 7.5f);
		TerrainPlacementMap placementMap = TerrainMeta.PlacementMap;
		TerrainHeightMap heightMap = TerrainMeta.HeightMap;
		TerrainTopologyMap topologyMap = TerrainMeta.TopologyMap;
		int[,] array = new int[num, num];
		for (int i = 0; i < num; i++)
		{
			float normZ = ((float)i + 0.5f) / (float)num;
			for (int j = 0; j < num; j++)
			{
				float normX = ((float)j + 0.5f) / (float)num;
				int num2 = SeedRandom.Range(ref seed, 100, 200);
				float slope = heightMap.GetSlope(normX, normZ);
				int topology = topologyMap.GetTopology(normX, normZ, radius);
				int num3 = 2295686;
				int num4 = 49152;
				if (slope > 20f || (topology & num3) != 0)
				{
					array[j, i] = int.MaxValue;
				}
				else if ((topology & num4) != 0 || placementMap.GetBlocked(normX, normZ, radius))
				{
					array[j, i] = 5000;
				}
				else
				{
					array[j, i] = 1 + (int)(slope * slope * 10f) + num2;
				}
			}
		}
		return array;
	}

	// Token: 0x06003125 RID: 12581 RVA: 0x00125FC4 File Offset: 0x001241C4
	public static int[,] CreateRailCostmap(ref uint seed)
	{
		float radius = 5f;
		int num = (int)(World.Size / 7.5f);
		TerrainPlacementMap placementMap = TerrainMeta.PlacementMap;
		TerrainHeightMap heightMap = TerrainMeta.HeightMap;
		TerrainTopologyMap topologyMap = TerrainMeta.TopologyMap;
		int[,] array = new int[num, num];
		for (int i = 0; i < num; i++)
		{
			float normZ = ((float)i + 0.5f) / (float)num;
			for (int j = 0; j < num; j++)
			{
				float normX = ((float)j + 0.5f) / (float)num;
				float slope = heightMap.GetSlope(normX, normZ);
				int topology = topologyMap.GetTopology(normX, normZ, radius);
				int num2 = 2295686;
				int num3 = 49152;
				if (slope > 20f || (topology & num2) != 0)
				{
					array[j, i] = int.MaxValue;
				}
				else if ((topology & num3) != 0 || placementMap.GetBlocked(normX, normZ, radius))
				{
					array[j, i] = 5000;
				}
				else if (slope > 10f)
				{
					array[j, i] = 1500;
				}
				else
				{
					array[j, i] = 1000;
				}
			}
		}
		return array;
	}

	// Token: 0x06003126 RID: 12582 RVA: 0x001260E8 File Offset: 0x001242E8
	public static int[,] CreateBoatCostmap(float depth)
	{
		TerrainHeightMap heightMap = TerrainMeta.HeightMap;
		TerrainWaterMap waterMap = TerrainMeta.WaterMap;
		int res = heightMap.res;
		int[,] array = new int[res, res];
		for (int i = 0; i < res; i++)
		{
			float normZ = ((float)i + 0.5f) / (float)res;
			for (int j = 0; j < res; j++)
			{
				float normX = ((float)j + 0.5f) / (float)res;
				float height = heightMap.GetHeight(normX, normZ);
				if (waterMap.GetHeight(normX, normZ) - height < depth)
				{
					array[j, i] = int.MaxValue;
				}
				else
				{
					array[j, i] = 1;
				}
			}
		}
		return array;
	}

	// Token: 0x06003127 RID: 12583 RVA: 0x00126188 File Offset: 0x00124388
	public void AddWire(PowerlineNode node)
	{
		string name = node.transform.root.name;
		if (!this.wires.ContainsKey(name))
		{
			this.wires.Add(name, new List<PowerlineNode>());
		}
		this.wires[name].Add(node);
	}

	// Token: 0x06003128 RID: 12584 RVA: 0x001261D8 File Offset: 0x001243D8
	public void CreateWires()
	{
		List<GameObject> list = new List<GameObject>();
		int num = 0;
		GameObjectRef gameObjectRef = null;
		foreach (KeyValuePair<string, List<PowerlineNode>> keyValuePair in this.wires)
		{
			foreach (PowerlineNode powerlineNode in keyValuePair.Value)
			{
				PowerLineWireConnectionHelper component = powerlineNode.GetComponent<PowerLineWireConnectionHelper>();
				if (component)
				{
					if (list.Count == 0)
					{
						gameObjectRef = powerlineNode.WirePrefab;
						num = component.connections.Count;
					}
					else
					{
						GameObject gameObject = list[list.Count - 1];
						if (powerlineNode.WirePrefab.guid != ((gameObjectRef != null) ? gameObjectRef.guid : null) || component.connections.Count != num || (gameObject.transform.position - powerlineNode.transform.position).sqrMagnitude > powerlineNode.MaxDistance * powerlineNode.MaxDistance)
						{
							this.CreateWire(keyValuePair.Key, list, gameObjectRef);
							list.Clear();
						}
					}
					list.Add(powerlineNode.gameObject);
				}
			}
			this.CreateWire(keyValuePair.Key, list, gameObjectRef);
			list.Clear();
		}
	}

	// Token: 0x06003129 RID: 12585 RVA: 0x00126374 File Offset: 0x00124574
	private void CreateWire(string name, List<GameObject> objects, GameObjectRef wirePrefab)
	{
		if (objects.Count < 3 || wirePrefab == null || !wirePrefab.isValid)
		{
			return;
		}
		PowerLineWire powerLineWire = PowerLineWire.Create(null, objects, wirePrefab, "Powerline Wires", null, 1f, 0.1f);
		if (powerLineWire)
		{
			powerLineWire.enabled = false;
			powerLineWire.gameObject.SetHierarchyGroup(name, true, false);
		}
	}

	// Token: 0x0600312A RID: 12586 RVA: 0x001263CC File Offset: 0x001245CC
	public MonumentInfo FindMonumentWithBoundsOverlap(Vector3 position)
	{
		if (TerrainMeta.Path.Monuments == null)
		{
			return null;
		}
		foreach (MonumentInfo monumentInfo in TerrainMeta.Path.Monuments)
		{
			if (monumentInfo != null && monumentInfo.IsInBounds(position))
			{
				return monumentInfo;
			}
		}
		return null;
	}
}
