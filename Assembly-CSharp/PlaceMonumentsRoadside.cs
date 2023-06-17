using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x020006DC RID: 1756
public class PlaceMonumentsRoadside : ProceduralComponent
{
	// Token: 0x040028A6 RID: 10406
	public SpawnFilter Filter;

	// Token: 0x040028A7 RID: 10407
	public string ResourceFolder = string.Empty;

	// Token: 0x040028A8 RID: 10408
	public int TargetCount;

	// Token: 0x040028A9 RID: 10409
	[FormerlySerializedAs("MinDistance")]
	public int MinDistanceSameType = 500;

	// Token: 0x040028AA RID: 10410
	public int MinDistanceDifferentType;

	// Token: 0x040028AB RID: 10411
	[FormerlySerializedAs("MinSize")]
	public int MinWorldSize;

	// Token: 0x040028AC RID: 10412
	[Tooltip("Distance to monuments of the same type")]
	public PlaceMonumentsRoadside.DistanceMode DistanceSameType = PlaceMonumentsRoadside.DistanceMode.Max;

	// Token: 0x040028AD RID: 10413
	[Tooltip("Distance to monuments of a different type")]
	public PlaceMonumentsRoadside.DistanceMode DistanceDifferentType;

	// Token: 0x040028AE RID: 10414
	public PlaceMonumentsRoadside.RoadMode RoadType;

	// Token: 0x040028AF RID: 10415
	private const int GroupCandidates = 8;

	// Token: 0x040028B0 RID: 10416
	private const int IndividualCandidates = 8;

	// Token: 0x040028B1 RID: 10417
	private static Quaternion rot90 = Quaternion.Euler(0f, 90f, 0f);

	// Token: 0x060031E4 RID: 12772 RVA: 0x00132F44 File Offset: 0x00131144
	public override void Process(uint seed)
	{
		string[] array = (from folder in this.ResourceFolder.Split(new char[]
		{
			','
		})
		select "assets/bundled/prefabs/autospawn/" + folder + "/").ToArray<string>();
		if (World.Networked)
		{
			World.Spawn("Monument", array);
			return;
		}
		if ((ulong)World.Size < (ulong)((long)this.MinWorldSize))
		{
			return;
		}
		TerrainHeightMap heightMap = TerrainMeta.HeightMap;
		List<Prefab<MonumentInfo>> list = new List<Prefab<MonumentInfo>>();
		string[] array2 = array;
		for (int i = 0; i < array2.Length; i++)
		{
			Prefab<MonumentInfo>[] array3 = Prefab.Load<MonumentInfo>(array2[i], null, null, true);
			array3.Shuffle(ref seed);
			list.AddRange(array3);
		}
		Prefab<MonumentInfo>[] array4 = list.ToArray();
		if (array4 == null || array4.Length == 0)
		{
			return;
		}
		array4.BubbleSort<Prefab<MonumentInfo>>();
		PlaceMonumentsRoadside.SpawnInfoGroup[] array5 = new PlaceMonumentsRoadside.SpawnInfoGroup[array4.Length];
		for (int j = 0; j < array4.Length; j++)
		{
			Prefab<MonumentInfo> prefab = array4[j];
			PlaceMonumentsRoadside.SpawnInfoGroup spawnInfoGroup = null;
			for (int k = 0; k < j; k++)
			{
				PlaceMonumentsRoadside.SpawnInfoGroup spawnInfoGroup2 = array5[k];
				Prefab<MonumentInfo> prefab2 = spawnInfoGroup2.prefab;
				if (prefab == prefab2)
				{
					spawnInfoGroup = spawnInfoGroup2;
					break;
				}
			}
			if (spawnInfoGroup == null)
			{
				spawnInfoGroup = new PlaceMonumentsRoadside.SpawnInfoGroup();
				spawnInfoGroup.prefab = array4[j];
				spawnInfoGroup.candidates = new List<PlaceMonumentsRoadside.SpawnInfo>();
			}
			array5[j] = spawnInfoGroup;
		}
		foreach (PlaceMonumentsRoadside.SpawnInfoGroup spawnInfoGroup3 in array5)
		{
			if (!spawnInfoGroup3.processed)
			{
				Prefab<MonumentInfo> prefab3 = spawnInfoGroup3.prefab;
				MonumentInfo component = prefab3.Component;
				if (!(component == null) && (ulong)World.Size >= (ulong)((long)component.MinWorldSize))
				{
					int num = 0;
					Vector3 vector = Vector3.zero;
					foreach (TerrainPathConnect terrainPathConnect in prefab3.Object.GetComponentsInChildren<TerrainPathConnect>(true))
					{
						if (terrainPathConnect.Type == InfrastructureType.Road)
						{
							vector += terrainPathConnect.transform.position;
							num++;
						}
					}
					Vector3 point = -vector.XZ3D().normalized;
					Vector3 forward = PlaceMonumentsRoadside.rot90 * point;
					if (num > 1)
					{
						vector /= (float)num;
					}
					foreach (PathList pathList in TerrainMeta.Path.Roads)
					{
						bool flag = false;
						switch (this.RoadType)
						{
						case PlaceMonumentsRoadside.RoadMode.SideRoadOrRingRoad:
							flag = (pathList.Hierarchy == 0 || pathList.Hierarchy == 1);
							break;
						case PlaceMonumentsRoadside.RoadMode.SideRoad:
							flag = (pathList.Hierarchy == 1);
							break;
						case PlaceMonumentsRoadside.RoadMode.RingRoad:
							flag = (pathList.Hierarchy == 0);
							break;
						case PlaceMonumentsRoadside.RoadMode.SideRoadOrDesireTrail:
							flag = (pathList.Hierarchy == 1 || pathList.Hierarchy == 2);
							break;
						case PlaceMonumentsRoadside.RoadMode.DesireTrail:
							flag = (pathList.Hierarchy == 2);
							break;
						}
						if (flag)
						{
							PathInterpolator path = pathList.Path;
							float num2 = 5f;
							float num3 = 5f;
							float num4 = path.StartOffset + num3;
							float num5 = path.Length - path.EndOffset - num3;
							for (float num6 = num4; num6 <= num5; num6 += num2)
							{
								Vector3 vector2 = pathList.Spline ? path.GetPointCubicHermite(num6) : path.GetPoint(num6);
								Vector3 tangent = path.GetTangent(num6);
								for (int m = -1; m <= 1; m += 2)
								{
									Quaternion quaternion = Quaternion.LookRotation((float)m * tangent.XZ3D());
									Vector3 vector3 = vector2;
									Quaternion quaternion2 = quaternion;
									Vector3 localScale = prefab3.Object.transform.localScale;
									quaternion2 *= Quaternion.LookRotation(forward);
									vector3 -= quaternion2 * vector;
									PlaceMonumentsRoadside.SpawnInfo item = default(PlaceMonumentsRoadside.SpawnInfo);
									item.prefab = prefab3;
									item.position = vector3;
									item.rotation = quaternion2;
									item.scale = localScale;
									spawnInfoGroup3.candidates.Add(item);
								}
							}
						}
					}
					spawnInfoGroup3.processed = true;
				}
			}
		}
		List<PlaceMonumentsRoadside.SpawnInfo> list2 = new List<PlaceMonumentsRoadside.SpawnInfo>();
		int num7 = 0;
		List<PlaceMonumentsRoadside.SpawnInfo> list3 = new List<PlaceMonumentsRoadside.SpawnInfo>();
		for (int n = 0; n < 8; n++)
		{
			int num8 = 0;
			list2.Clear();
			array5.Shuffle(ref seed);
			foreach (PlaceMonumentsRoadside.SpawnInfoGroup spawnInfoGroup4 in array5)
			{
				Prefab<MonumentInfo> prefab4 = spawnInfoGroup4.prefab;
				MonumentInfo component2 = prefab4.Component;
				if (!(component2 == null) && (ulong)World.Size >= (ulong)((long)component2.MinWorldSize))
				{
					DungeonGridInfo dungeonEntrance = component2.DungeonEntrance;
					int num9 = (int)(prefab4.Parameters ? (prefab4.Parameters.Priority + 1) : PrefabPriority.Low);
					int num10 = 100000 * num9 * num9 * num9 * num9;
					int num11 = 0;
					int num12 = 0;
					PlaceMonumentsRoadside.SpawnInfo item2 = default(PlaceMonumentsRoadside.SpawnInfo);
					spawnInfoGroup4.candidates.Shuffle(ref seed);
					for (int num13 = 0; num13 < spawnInfoGroup4.candidates.Count; num13++)
					{
						PlaceMonumentsRoadside.SpawnInfo spawnInfo = spawnInfoGroup4.candidates[num13];
						PlaceMonumentsRoadside.DistanceInfo distanceInfo = this.GetDistanceInfo(list2, prefab4, spawnInfo.position, spawnInfo.rotation, spawnInfo.scale);
						if (distanceInfo.minDistanceSameType >= (float)this.MinDistanceSameType && distanceInfo.minDistanceDifferentType >= (float)this.MinDistanceDifferentType)
						{
							int num14 = num10;
							if (distanceInfo.minDistanceSameType != 3.4028235E+38f)
							{
								if (this.DistanceSameType == PlaceMonumentsRoadside.DistanceMode.Min)
								{
									num14 -= Mathf.RoundToInt(distanceInfo.minDistanceSameType * distanceInfo.minDistanceSameType * 2f);
								}
								else if (this.DistanceSameType == PlaceMonumentsRoadside.DistanceMode.Max)
								{
									num14 += Mathf.RoundToInt(distanceInfo.minDistanceSameType * distanceInfo.minDistanceSameType * 2f);
								}
							}
							if (distanceInfo.minDistanceDifferentType != 3.4028235E+38f)
							{
								if (this.DistanceDifferentType == PlaceMonumentsRoadside.DistanceMode.Min)
								{
									num14 -= Mathf.RoundToInt(distanceInfo.minDistanceDifferentType * distanceInfo.minDistanceDifferentType);
								}
								else if (this.DistanceDifferentType == PlaceMonumentsRoadside.DistanceMode.Max)
								{
									num14 += Mathf.RoundToInt(distanceInfo.minDistanceDifferentType * distanceInfo.minDistanceDifferentType);
								}
							}
							if (num14 > num12 && prefab4.ApplyTerrainAnchors(ref spawnInfo.position, spawnInfo.rotation, spawnInfo.scale, this.Filter) && component2.CheckPlacement(spawnInfo.position, spawnInfo.rotation, spawnInfo.scale))
							{
								if (dungeonEntrance)
								{
									Vector3 vector4 = spawnInfo.position + spawnInfo.rotation * Vector3.Scale(spawnInfo.scale, dungeonEntrance.transform.position);
									Vector3 vector5 = dungeonEntrance.SnapPosition(vector4);
									spawnInfo.position += vector5 - vector4;
									if (!dungeonEntrance.IsValidSpawnPosition(vector5))
									{
										goto IL_747;
									}
								}
								if (prefab4.ApplyTerrainChecks(spawnInfo.position, spawnInfo.rotation, spawnInfo.scale, this.Filter) && prefab4.ApplyTerrainFilters(spawnInfo.position, spawnInfo.rotation, spawnInfo.scale, null) && prefab4.ApplyWaterChecks(spawnInfo.position, spawnInfo.rotation, spawnInfo.scale) && !prefab4.CheckEnvironmentVolumes(spawnInfo.position, spawnInfo.rotation, spawnInfo.scale, EnvironmentType.Underground | EnvironmentType.TrainTunnels))
								{
									num12 = num14;
									item2 = spawnInfo;
									num11++;
									if (num11 >= 8 || this.DistanceDifferentType == PlaceMonumentsRoadside.DistanceMode.Any)
									{
										break;
									}
								}
							}
						}
						IL_747:;
					}
					if (num12 > 0)
					{
						list2.Add(item2);
						num8 += num12;
					}
					if (this.TargetCount > 0 && list2.Count >= this.TargetCount)
					{
						break;
					}
				}
			}
			if (num8 > num7)
			{
				num7 = num8;
				GenericsUtil.Swap<List<PlaceMonumentsRoadside.SpawnInfo>>(ref list2, ref list3);
			}
		}
		foreach (PlaceMonumentsRoadside.SpawnInfo spawnInfo2 in list3)
		{
			World.AddPrefab("Monument", spawnInfo2.prefab, spawnInfo2.position, spawnInfo2.rotation, spawnInfo2.scale);
		}
	}

	// Token: 0x060031E5 RID: 12773 RVA: 0x00133798 File Offset: 0x00131998
	private PlaceMonumentsRoadside.DistanceInfo GetDistanceInfo(List<PlaceMonumentsRoadside.SpawnInfo> spawns, Prefab<MonumentInfo> prefab, Vector3 monumentPos, Quaternion monumentRot, Vector3 monumentScale)
	{
		PlaceMonumentsRoadside.DistanceInfo distanceInfo = default(PlaceMonumentsRoadside.DistanceInfo);
		distanceInfo.minDistanceDifferentType = float.MaxValue;
		distanceInfo.maxDistanceDifferentType = float.MinValue;
		distanceInfo.minDistanceSameType = float.MaxValue;
		distanceInfo.maxDistanceSameType = float.MinValue;
		OBB obb = new OBB(monumentPos, monumentScale, monumentRot, prefab.Component.Bounds);
		if (TerrainMeta.Path != null)
		{
			foreach (MonumentInfo monumentInfo in TerrainMeta.Path.Monuments)
			{
				if (!prefab.Component.HasDungeonLink || (!monumentInfo.HasDungeonLink && monumentInfo.WantsDungeonLink))
				{
					float num = monumentInfo.SqrDistance(obb);
					if (num < distanceInfo.minDistanceDifferentType)
					{
						distanceInfo.minDistanceDifferentType = num;
					}
					if (num > distanceInfo.maxDistanceDifferentType)
					{
						distanceInfo.maxDistanceDifferentType = num;
					}
				}
			}
			if (distanceInfo.minDistanceDifferentType != 3.4028235E+38f)
			{
				distanceInfo.minDistanceDifferentType = Mathf.Sqrt(distanceInfo.minDistanceDifferentType);
			}
			if (distanceInfo.maxDistanceDifferentType != -3.4028235E+38f)
			{
				distanceInfo.maxDistanceDifferentType = Mathf.Sqrt(distanceInfo.maxDistanceDifferentType);
			}
		}
		if (spawns != null)
		{
			foreach (PlaceMonumentsRoadside.SpawnInfo spawnInfo in spawns)
			{
				float num2 = new OBB(spawnInfo.position, spawnInfo.scale, spawnInfo.rotation, spawnInfo.prefab.Component.Bounds).SqrDistance(obb);
				if (num2 < distanceInfo.minDistanceSameType)
				{
					distanceInfo.minDistanceSameType = num2;
				}
				if (num2 > distanceInfo.maxDistanceSameType)
				{
					distanceInfo.maxDistanceSameType = num2;
				}
			}
			if (prefab.Component.HasDungeonLink)
			{
				foreach (MonumentInfo monumentInfo2 in TerrainMeta.Path.Monuments)
				{
					if (monumentInfo2.HasDungeonLink || !monumentInfo2.WantsDungeonLink)
					{
						float num3 = monumentInfo2.SqrDistance(obb);
						if (num3 < distanceInfo.minDistanceSameType)
						{
							distanceInfo.minDistanceSameType = num3;
						}
						if (num3 > distanceInfo.maxDistanceSameType)
						{
							distanceInfo.maxDistanceSameType = num3;
						}
					}
				}
				foreach (DungeonGridInfo dungeonGridInfo in TerrainMeta.Path.DungeonGridEntrances)
				{
					float num4 = dungeonGridInfo.SqrDistance(monumentPos);
					if (num4 < distanceInfo.minDistanceSameType)
					{
						distanceInfo.minDistanceSameType = num4;
					}
					if (num4 > distanceInfo.maxDistanceSameType)
					{
						distanceInfo.maxDistanceSameType = num4;
					}
				}
			}
			if (distanceInfo.minDistanceSameType != 3.4028235E+38f)
			{
				distanceInfo.minDistanceSameType = Mathf.Sqrt(distanceInfo.minDistanceSameType);
			}
			if (distanceInfo.maxDistanceSameType != -3.4028235E+38f)
			{
				distanceInfo.maxDistanceSameType = Mathf.Sqrt(distanceInfo.maxDistanceSameType);
			}
		}
		return distanceInfo;
	}

	// Token: 0x02000E19 RID: 3609
	private struct SpawnInfo
	{
		// Token: 0x040049F2 RID: 18930
		public Prefab<MonumentInfo> prefab;

		// Token: 0x040049F3 RID: 18931
		public Vector3 position;

		// Token: 0x040049F4 RID: 18932
		public Quaternion rotation;

		// Token: 0x040049F5 RID: 18933
		public Vector3 scale;
	}

	// Token: 0x02000E1A RID: 3610
	private class SpawnInfoGroup
	{
		// Token: 0x040049F6 RID: 18934
		public bool processed;

		// Token: 0x040049F7 RID: 18935
		public Prefab<MonumentInfo> prefab;

		// Token: 0x040049F8 RID: 18936
		public List<PlaceMonumentsRoadside.SpawnInfo> candidates;
	}

	// Token: 0x02000E1B RID: 3611
	private struct DistanceInfo
	{
		// Token: 0x040049F9 RID: 18937
		public float minDistanceSameType;

		// Token: 0x040049FA RID: 18938
		public float maxDistanceSameType;

		// Token: 0x040049FB RID: 18939
		public float minDistanceDifferentType;

		// Token: 0x040049FC RID: 18940
		public float maxDistanceDifferentType;
	}

	// Token: 0x02000E1C RID: 3612
	public enum DistanceMode
	{
		// Token: 0x040049FE RID: 18942
		Any,
		// Token: 0x040049FF RID: 18943
		Min,
		// Token: 0x04004A00 RID: 18944
		Max
	}

	// Token: 0x02000E1D RID: 3613
	public enum RoadMode
	{
		// Token: 0x04004A02 RID: 18946
		SideRoadOrRingRoad,
		// Token: 0x04004A03 RID: 18947
		SideRoad,
		// Token: 0x04004A04 RID: 18948
		RingRoad,
		// Token: 0x04004A05 RID: 18949
		SideRoadOrDesireTrail,
		// Token: 0x04004A06 RID: 18950
		DesireTrail
	}
}
