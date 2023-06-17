using System;
using System.Collections.Generic;
using Facepunch;
using UnityEngine;

// Token: 0x02000507 RID: 1287
public static class EnvironmentVolumeEx
{
	// Token: 0x06002938 RID: 10552 RVA: 0x000FCF08 File Offset: 0x000FB108
	public static bool CheckEnvironmentVolumes(this Transform transform, Vector3 pos, Quaternion rot, Vector3 scale, EnvironmentType type)
	{
		List<EnvironmentVolume> list = Pool.GetList<EnvironmentVolume>();
		transform.GetComponentsInChildren<EnvironmentVolume>(true, list);
		for (int i = 0; i < list.Count; i++)
		{
			EnvironmentVolume environmentVolume = list[i];
			OBB obb = new OBB(environmentVolume.transform, new Bounds(environmentVolume.Center, environmentVolume.Size));
			obb.Transform(pos, scale, rot);
			if (EnvironmentManager.Check(obb, type))
			{
				Pool.FreeList<EnvironmentVolume>(ref list);
				return true;
			}
		}
		Pool.FreeList<EnvironmentVolume>(ref list);
		return false;
	}

	// Token: 0x06002939 RID: 10553 RVA: 0x000FCF7F File Offset: 0x000FB17F
	public static bool CheckEnvironmentVolumes(this Transform transform, EnvironmentType type)
	{
		return transform.CheckEnvironmentVolumes(transform.position, transform.rotation, transform.lossyScale, type);
	}

	// Token: 0x0600293A RID: 10554 RVA: 0x000FCF9C File Offset: 0x000FB19C
	public static bool CheckEnvironmentVolumesInsideTerrain(this Transform transform, Vector3 pos, Quaternion rot, Vector3 scale, EnvironmentType type, float padding = 0f)
	{
		if (TerrainMeta.HeightMap == null)
		{
			return true;
		}
		List<EnvironmentVolume> list = Pool.GetList<EnvironmentVolume>();
		transform.GetComponentsInChildren<EnvironmentVolume>(true, list);
		if (list.Count == 0)
		{
			Pool.FreeList<EnvironmentVolume>(ref list);
			return true;
		}
		for (int i = 0; i < list.Count; i++)
		{
			EnvironmentVolume environmentVolume = list[i];
			if ((environmentVolume.Type & type) != (EnvironmentType)0)
			{
				OBB obb = new OBB(environmentVolume.transform, new Bounds(environmentVolume.Center, environmentVolume.Size));
				obb.Transform(pos, scale, rot);
				Vector3 point = obb.GetPoint(-1f, 0f, -1f);
				Vector3 point2 = obb.GetPoint(1f, 0f, -1f);
				Vector3 point3 = obb.GetPoint(-1f, 0f, 1f);
				Vector3 point4 = obb.GetPoint(1f, 0f, 1f);
				float max = obb.ToBounds().max.y + padding;
				bool fail = false;
				TerrainMeta.HeightMap.ForEachParallel(point, point2, point3, point4, delegate(int x, int z)
				{
					if (TerrainMeta.HeightMap.GetHeight(x, z) <= max)
					{
						fail = true;
					}
				});
				if (fail)
				{
					Pool.FreeList<EnvironmentVolume>(ref list);
					return false;
				}
			}
		}
		Pool.FreeList<EnvironmentVolume>(ref list);
		return true;
	}

	// Token: 0x0600293B RID: 10555 RVA: 0x000FD0EE File Offset: 0x000FB2EE
	public static bool CheckEnvironmentVolumesInsideTerrain(this Transform transform, EnvironmentType type)
	{
		return transform.CheckEnvironmentVolumesInsideTerrain(transform.position, transform.rotation, transform.lossyScale, type, 0f);
	}

	// Token: 0x0600293C RID: 10556 RVA: 0x000FD110 File Offset: 0x000FB310
	public static bool CheckEnvironmentVolumesOutsideTerrain(this Transform transform, Vector3 pos, Quaternion rot, Vector3 scale, EnvironmentType type, float padding = 0f)
	{
		if (TerrainMeta.HeightMap == null)
		{
			return true;
		}
		List<EnvironmentVolume> list = Pool.GetList<EnvironmentVolume>();
		transform.GetComponentsInChildren<EnvironmentVolume>(true, list);
		if (list.Count == 0)
		{
			Pool.FreeList<EnvironmentVolume>(ref list);
			return true;
		}
		for (int i = 0; i < list.Count; i++)
		{
			EnvironmentVolume environmentVolume = list[i];
			if ((environmentVolume.Type & type) != (EnvironmentType)0)
			{
				OBB obb = new OBB(environmentVolume.transform, new Bounds(environmentVolume.Center, environmentVolume.Size));
				obb.Transform(pos, scale, rot);
				Vector3 point = obb.GetPoint(-1f, 0f, -1f);
				Vector3 point2 = obb.GetPoint(1f, 0f, -1f);
				Vector3 point3 = obb.GetPoint(-1f, 0f, 1f);
				Vector3 point4 = obb.GetPoint(1f, 0f, 1f);
				float min = obb.ToBounds().min.y - padding;
				bool fail = false;
				TerrainMeta.HeightMap.ForEachParallel(point, point2, point3, point4, delegate(int x, int z)
				{
					if (TerrainMeta.HeightMap.GetHeight(x, z) >= min)
					{
						fail = true;
					}
				});
				if (fail)
				{
					Pool.FreeList<EnvironmentVolume>(ref list);
					return false;
				}
			}
		}
		Pool.FreeList<EnvironmentVolume>(ref list);
		return true;
	}

	// Token: 0x0600293D RID: 10557 RVA: 0x000FD262 File Offset: 0x000FB462
	public static bool CheckEnvironmentVolumesOutsideTerrain(this Transform transform, EnvironmentType type)
	{
		return transform.CheckEnvironmentVolumesOutsideTerrain(transform.position, transform.rotation, transform.lossyScale, type, 0f);
	}
}
