using System;
using System.Collections.Generic;
using Facepunch;
using UnityEngine;

// Token: 0x0200021A RID: 538
public class UnderwaterPathFinder : BasePathFinder
{
	// Token: 0x04001397 RID: 5015
	private BaseEntity npc;

	// Token: 0x06001BAC RID: 7084 RVA: 0x000C2BF4 File Offset: 0x000C0DF4
	public void Init(BaseEntity npc)
	{
		this.npc = npc;
	}

	// Token: 0x06001BAD RID: 7085 RVA: 0x000C2C00 File Offset: 0x000C0E00
	public override Vector3 GetBestRoamPosition(BaseNavigator navigator, Vector3 fallbackPos, float minRange, float maxRange)
	{
		List<Vector3> list = Pool.GetList<Vector3>();
		float height = TerrainMeta.WaterMap.GetHeight(navigator.transform.position);
		float height2 = TerrainMeta.HeightMap.GetHeight(navigator.transform.position);
		for (int i = 0; i < 8; i++)
		{
			Vector3 pointOnCircle = BasePathFinder.GetPointOnCircle(fallbackPos, UnityEngine.Random.Range(1f, navigator.MaxRoamDistanceFromHome), UnityEngine.Random.Range(0f, 359f));
			pointOnCircle.y += UnityEngine.Random.Range(-2f, 2f);
			pointOnCircle.y = Mathf.Clamp(pointOnCircle.y, height2, height);
			list.Add(pointOnCircle);
		}
		float num = -1f;
		int num2 = -1;
		for (int j = 0; j < list.Count; j++)
		{
			Vector3 vector = list[j];
			if (this.npc.IsVisible(vector, float.PositiveInfinity))
			{
				float num3 = 0f;
				Vector3 rhs = Vector3Ex.Direction2D(vector, navigator.transform.position);
				float value = Vector3.Dot(navigator.transform.forward, rhs);
				num3 += Mathf.InverseLerp(0.25f, 0.8f, value) * 5f;
				float value2 = Mathf.Abs(vector.y - navigator.transform.position.y);
				num3 += 1f - Mathf.InverseLerp(1f, 3f, value2) * 5f;
				if (num3 > num || num2 == -1)
				{
					num = num3;
					num2 = j;
				}
			}
		}
		Vector3 result = list[num2];
		Pool.FreeList<Vector3>(ref list);
		return result;
	}

	// Token: 0x06001BAE RID: 7086 RVA: 0x000C2DA0 File Offset: 0x000C0FA0
	public override bool GetBestFleePosition(BaseNavigator navigator, AIBrainSenses senses, BaseEntity fleeFrom, Vector3 fallbackPos, float minRange, float maxRange, out Vector3 result)
	{
		if (fleeFrom == null)
		{
			result = navigator.transform.position;
			return false;
		}
		Vector3 a = Vector3Ex.Direction2D(navigator.transform.position, fleeFrom.transform.position);
		result = navigator.transform.position + a * UnityEngine.Random.Range(minRange, maxRange);
		return true;
	}
}
