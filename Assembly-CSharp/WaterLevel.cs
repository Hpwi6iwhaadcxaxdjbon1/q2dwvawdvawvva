using System;
using UnityEngine;

// Token: 0x0200059F RID: 1439
public static class WaterLevel
{
	// Token: 0x06002BCA RID: 11210 RVA: 0x00108E38 File Offset: 0x00107038
	public static float Factor(Vector3 start, Vector3 end, float radius, BaseEntity forEntity = null)
	{
		float result;
		using (TimeWarning.New("WaterLevel.Factor", 0))
		{
			WaterLevel.WaterInfo waterInfo = WaterLevel.GetWaterInfo(start, end, radius, forEntity, true);
			result = (waterInfo.isValid ? Mathf.InverseLerp(Mathf.Min(start.y, end.y) - radius, Mathf.Max(start.y, end.y) + radius, waterInfo.surfaceLevel) : 0f);
		}
		return result;
	}

	// Token: 0x06002BCB RID: 11211 RVA: 0x00108EBC File Offset: 0x001070BC
	public static float Factor(Bounds bounds, BaseEntity forEntity = null)
	{
		float result;
		using (TimeWarning.New("WaterLevel.Factor", 0))
		{
			if (bounds.size == Vector3.zero)
			{
				bounds.size = new Vector3(0.1f, 0.1f, 0.1f);
			}
			WaterLevel.WaterInfo waterInfo = WaterLevel.GetWaterInfo(bounds, forEntity, true);
			result = (waterInfo.isValid ? Mathf.InverseLerp(bounds.min.y, bounds.max.y, waterInfo.surfaceLevel) : 0f);
		}
		return result;
	}

	// Token: 0x06002BCC RID: 11212 RVA: 0x00108F5C File Offset: 0x0010715C
	public static bool Test(Vector3 pos, bool waves = true, BaseEntity forEntity = null)
	{
		bool isValid;
		using (TimeWarning.New("WaterLevel.Test", 0))
		{
			isValid = WaterLevel.GetWaterInfo(pos, waves, forEntity, false).isValid;
		}
		return isValid;
	}

	// Token: 0x06002BCD RID: 11213 RVA: 0x00108FA4 File Offset: 0x001071A4
	public static float GetWaterDepth(Vector3 pos, bool waves = true, BaseEntity forEntity = null)
	{
		float currentDepth;
		using (TimeWarning.New("WaterLevel.GetWaterDepth", 0))
		{
			currentDepth = WaterLevel.GetWaterInfo(pos, waves, forEntity, false).currentDepth;
		}
		return currentDepth;
	}

	// Token: 0x06002BCE RID: 11214 RVA: 0x00108FEC File Offset: 0x001071EC
	public static float GetOverallWaterDepth(Vector3 pos, bool waves = true, BaseEntity forEntity = null, bool noEarlyExit = false)
	{
		float overallDepth;
		using (TimeWarning.New("WaterLevel.GetOverallWaterDepth", 0))
		{
			overallDepth = WaterLevel.GetWaterInfo(pos, waves, forEntity, noEarlyExit).overallDepth;
		}
		return overallDepth;
	}

	// Token: 0x06002BCF RID: 11215 RVA: 0x00109034 File Offset: 0x00107234
	public static WaterLevel.WaterInfo GetBuoyancyWaterInfo(Vector3 pos, Vector2 posUV, float terrainHeight, float waterHeight, bool doDeepwaterChecks, BaseEntity forEntity = null)
	{
		WaterLevel.WaterInfo result;
		using (TimeWarning.New("WaterLevel.GetWaterInfo", 0))
		{
			WaterLevel.WaterInfo waterInfo = default(WaterLevel.WaterInfo);
			if (pos.y > waterHeight)
			{
				result = WaterLevel.GetWaterInfoFromVolumes(pos, forEntity);
			}
			else
			{
				bool flag = pos.y < terrainHeight - 1f;
				if (flag)
				{
					waterHeight = 0f;
					if (pos.y > waterHeight)
					{
						return waterInfo;
					}
				}
				bool flag2 = doDeepwaterChecks && pos.y < waterHeight - 10f;
				int num = TerrainMeta.TopologyMap ? TerrainMeta.TopologyMap.GetTopologyFast(posUV) : 0;
				if ((flag || flag2 || (num & 246144) == 0) && WaterSystem.Collision && WaterSystem.Collision.GetIgnore(pos, 0.01f))
				{
					result = waterInfo;
				}
				else
				{
					RaycastHit raycastHit;
					if (flag2 && Physics.Raycast(pos, Vector3.up, out raycastHit, 5f, 16, QueryTriggerInteraction.Collide))
					{
						waterHeight = Mathf.Min(waterHeight, raycastHit.point.y);
					}
					waterInfo.isValid = true;
					waterInfo.currentDepth = Mathf.Max(0f, waterHeight - pos.y);
					waterInfo.overallDepth = Mathf.Max(0f, waterHeight - terrainHeight);
					waterInfo.surfaceLevel = waterHeight;
					result = waterInfo;
				}
			}
		}
		return result;
	}

	// Token: 0x06002BD0 RID: 11216 RVA: 0x00109198 File Offset: 0x00107398
	public static WaterLevel.WaterInfo GetWaterInfo(Vector3 pos, bool waves = true, BaseEntity forEntity = null, bool noEarlyExit = false)
	{
		WaterLevel.WaterInfo result;
		using (TimeWarning.New("WaterLevel.GetWaterInfo", 0))
		{
			WaterLevel.WaterInfo waterInfo = default(WaterLevel.WaterInfo);
			float num = 0f;
			if (waves)
			{
				num = WaterSystem.GetHeight(pos);
			}
			else if (TerrainMeta.WaterMap)
			{
				num = TerrainMeta.WaterMap.GetHeight(pos);
			}
			if (pos.y > num)
			{
				if (!noEarlyExit)
				{
					return WaterLevel.GetWaterInfoFromVolumes(pos, forEntity);
				}
				waterInfo = WaterLevel.GetWaterInfoFromVolumes(pos, forEntity);
			}
			float num2 = TerrainMeta.HeightMap ? TerrainMeta.HeightMap.GetHeight(pos) : 0f;
			if (pos.y < num2 - 1f)
			{
				num = 0f;
				if (pos.y > num && !noEarlyExit)
				{
					return waterInfo;
				}
			}
			if (WaterSystem.Collision && WaterSystem.Collision.GetIgnore(pos, 0.01f))
			{
				result = waterInfo;
			}
			else
			{
				waterInfo.isValid = true;
				waterInfo.currentDepth = Mathf.Max(0f, num - pos.y);
				waterInfo.overallDepth = Mathf.Max(0f, num - num2);
				waterInfo.surfaceLevel = num;
				result = waterInfo;
			}
		}
		return result;
	}

	// Token: 0x06002BD1 RID: 11217 RVA: 0x001092DC File Offset: 0x001074DC
	public static WaterLevel.WaterInfo GetWaterInfo(Bounds bounds, BaseEntity forEntity = null, bool waves = true)
	{
		WaterLevel.WaterInfo result;
		using (TimeWarning.New("WaterLevel.GetWaterInfo", 0))
		{
			WaterLevel.WaterInfo waterInfo = default(WaterLevel.WaterInfo);
			float num = 0f;
			if (waves)
			{
				num = WaterSystem.GetHeight(bounds.center);
			}
			else if (TerrainMeta.WaterMap)
			{
				num = TerrainMeta.WaterMap.GetHeight(bounds.center);
			}
			if (bounds.min.y > num)
			{
				result = WaterLevel.GetWaterInfoFromVolumes(bounds, forEntity);
			}
			else
			{
				float num2 = TerrainMeta.HeightMap ? TerrainMeta.HeightMap.GetHeight(bounds.center) : 0f;
				if (bounds.max.y < num2 - 1f)
				{
					num = 0f;
					if (bounds.min.y > num)
					{
						return waterInfo;
					}
				}
				if (WaterSystem.Collision && WaterSystem.Collision.GetIgnore(bounds))
				{
					result = waterInfo;
				}
				else
				{
					waterInfo.isValid = true;
					waterInfo.currentDepth = Mathf.Max(0f, num - bounds.min.y);
					waterInfo.overallDepth = Mathf.Max(0f, num - num2);
					waterInfo.surfaceLevel = num;
					result = waterInfo;
				}
			}
		}
		return result;
	}

	// Token: 0x06002BD2 RID: 11218 RVA: 0x00109434 File Offset: 0x00107634
	public static WaterLevel.WaterInfo GetWaterInfo(Vector3 start, Vector3 end, float radius, BaseEntity forEntity = null, bool waves = true)
	{
		WaterLevel.WaterInfo result;
		using (TimeWarning.New("WaterLevel.GetWaterInfo", 0))
		{
			WaterLevel.WaterInfo waterInfo = default(WaterLevel.WaterInfo);
			float num = 0f;
			Vector3 vector = (start + end) * 0.5f;
			float num2 = Mathf.Min(start.y, end.y) - radius;
			float num3 = Mathf.Max(start.y, end.y) + radius;
			if (waves)
			{
				num = WaterSystem.GetHeight(vector);
			}
			else if (TerrainMeta.WaterMap)
			{
				num = TerrainMeta.WaterMap.GetHeight(vector);
			}
			if (num2 > num)
			{
				result = WaterLevel.GetWaterInfoFromVolumes(start, end, radius, forEntity);
			}
			else
			{
				float num4 = TerrainMeta.HeightMap ? TerrainMeta.HeightMap.GetHeight(vector) : 0f;
				if (num3 < num4 - 1f)
				{
					num = 0f;
					if (num2 > num)
					{
						return waterInfo;
					}
				}
				if (WaterSystem.Collision && WaterSystem.Collision.GetIgnore(start, end, radius))
				{
					Vector3 vector2 = vector.WithY(Mathf.Lerp(num2, num3, 0.75f));
					if (WaterSystem.Collision.GetIgnore(vector2, 0.01f))
					{
						return waterInfo;
					}
					num = Mathf.Min(num, vector2.y);
				}
				waterInfo.isValid = true;
				waterInfo.currentDepth = Mathf.Max(0f, num - num2);
				waterInfo.overallDepth = Mathf.Max(0f, num - num4);
				waterInfo.surfaceLevel = num;
				result = waterInfo;
			}
		}
		return result;
	}

	// Token: 0x06002BD3 RID: 11219 RVA: 0x001095D0 File Offset: 0x001077D0
	private static WaterLevel.WaterInfo GetWaterInfoFromVolumes(Bounds bounds, BaseEntity forEntity)
	{
		WaterLevel.WaterInfo result = default(WaterLevel.WaterInfo);
		if (forEntity == null)
		{
			return result;
		}
		forEntity.WaterTestFromVolumes(bounds, out result);
		return result;
	}

	// Token: 0x06002BD4 RID: 11220 RVA: 0x001095FC File Offset: 0x001077FC
	private static WaterLevel.WaterInfo GetWaterInfoFromVolumes(Vector3 pos, BaseEntity forEntity)
	{
		WaterLevel.WaterInfo result = default(WaterLevel.WaterInfo);
		if (forEntity == null)
		{
			return result;
		}
		forEntity.WaterTestFromVolumes(pos, out result);
		return result;
	}

	// Token: 0x06002BD5 RID: 11221 RVA: 0x00109628 File Offset: 0x00107828
	private static WaterLevel.WaterInfo GetWaterInfoFromVolumes(Vector3 start, Vector3 end, float radius, BaseEntity forEntity)
	{
		WaterLevel.WaterInfo result = default(WaterLevel.WaterInfo);
		if (forEntity == null)
		{
			return result;
		}
		forEntity.WaterTestFromVolumes(start, end, radius, out result);
		return result;
	}

	// Token: 0x02000D60 RID: 3424
	public struct WaterInfo
	{
		// Token: 0x04004727 RID: 18215
		public bool isValid;

		// Token: 0x04004728 RID: 18216
		public float currentDepth;

		// Token: 0x04004729 RID: 18217
		public float overallDepth;

		// Token: 0x0400472A RID: 18218
		public float surfaceLevel;
	}
}
