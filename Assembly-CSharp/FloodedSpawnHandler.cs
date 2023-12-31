﻿using System;
using UnityEngine;

// Token: 0x0200056A RID: 1386
public static class FloodedSpawnHandler
{
	// Token: 0x040022A1 RID: 8865
	private static readonly int[] SpreadSteps = new int[]
	{
		0,
		1,
		-1,
		2,
		-2,
		3,
		-3
	};

	// Token: 0x06002A86 RID: 10886 RVA: 0x001034EC File Offset: 0x001016EC
	public static bool GetSpawnPoint(BasePlayer.SpawnPoint spawnPoint, float searchHeight)
	{
		SpawnHandler instance = SingletonComponent<SpawnHandler>.Instance;
		if (TerrainMeta.HeightMap == null || instance == null)
		{
			return false;
		}
		LayerMask placementMask = instance.PlacementMask;
		LayerMask placementCheckMask = instance.PlacementCheckMask;
		float placementCheckHeight = instance.PlacementCheckHeight;
		LayerMask radiusCheckMask = instance.RadiusCheckMask;
		float radiusCheckDistance = instance.RadiusCheckDistance;
		int i = 0;
		while (i < 10)
		{
			Vector3 vector = FloodedSpawnHandler.FindSpawnPoint(searchHeight);
			RaycastHit raycastHit;
			if (placementCheckMask == 0 || !Physics.Raycast(vector + Vector3.up * placementCheckHeight, Vector3.down, out raycastHit, placementCheckHeight, placementCheckMask))
			{
				goto IL_B4;
			}
			if ((1 << raycastHit.transform.gameObject.layer & placementMask) != 0)
			{
				vector.y = raycastHit.point.y;
				goto IL_B4;
			}
			IL_FD:
			i++;
			continue;
			IL_B4:
			if (radiusCheckMask == 0 || !Physics.CheckSphere(vector, radiusCheckDistance, radiusCheckMask))
			{
				spawnPoint.pos = vector;
				spawnPoint.rot = Quaternion.Euler(0f, UnityEngine.Random.Range(0f, 360f), 0f);
				return true;
			}
			goto IL_FD;
		}
		return false;
	}

	// Token: 0x06002A87 RID: 10887 RVA: 0x00103608 File Offset: 0x00101808
	private static Vector3 FindSpawnPoint(float searchHeight)
	{
		Vector3 b = (TerrainMeta.Size / 2f).WithY(0f);
		float magnitude = b.magnitude;
		float distance = magnitude / 50f;
		float num = FloodedSpawnHandler.RandomAngle();
		float num2 = num + 3.1415927f;
		Vector3 vector = TerrainMeta.Position + b + FloodedSpawnHandler.Step(num, magnitude);
		for (int i = 0; i < 50; i++)
		{
			float num3 = float.MinValue;
			Vector3 v = Vector3.zero;
			float num4 = 0f;
			foreach (int num5 in FloodedSpawnHandler.SpreadSteps)
			{
				float num6 = num2 + (float)num5 * 0.17453292f;
				Vector3 vector2 = vector + FloodedSpawnHandler.Step(num6, distance);
				float height = TerrainMeta.HeightMap.GetHeight(vector2);
				if (height > num3)
				{
					num3 = height;
					v = vector2;
					num4 = num6;
				}
			}
			vector = v.WithY(num3);
			num2 = (num2 + num4) / 2f;
			if (num3 >= searchHeight)
			{
				break;
			}
		}
		return vector;
	}

	// Token: 0x06002A88 RID: 10888 RVA: 0x00103712 File Offset: 0x00101912
	private static Vector3 Step(float angle, float distance)
	{
		return new Vector3(distance * Mathf.Cos(angle), 0f, distance * -Mathf.Sin(angle));
	}

	// Token: 0x06002A89 RID: 10889 RVA: 0x0010372F File Offset: 0x0010192F
	private static float RandomAngle()
	{
		return UnityEngine.Random.value * 6.2831855f;
	}
}
