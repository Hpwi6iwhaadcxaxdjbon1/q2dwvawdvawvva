﻿using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000481 RID: 1153
public class CH47PathFinder : BasePathFinder
{
	// Token: 0x04001E5D RID: 7773
	public List<Vector3> visitedPatrolPoints = new List<Vector3>();

	// Token: 0x06002627 RID: 9767 RVA: 0x000F0A18 File Offset: 0x000EEC18
	public override Vector3 GetRandomPatrolPoint()
	{
		Vector3 vector = Vector3.zero;
		MonumentInfo monumentInfo = null;
		if (TerrainMeta.Path != null && TerrainMeta.Path.Monuments != null && TerrainMeta.Path.Monuments.Count > 0)
		{
			int count = TerrainMeta.Path.Monuments.Count;
			int num = UnityEngine.Random.Range(0, count);
			for (int i = 0; i < count; i++)
			{
				int num2 = i + num;
				if (num2 >= count)
				{
					num2 -= count;
				}
				MonumentInfo monumentInfo2 = TerrainMeta.Path.Monuments[num2];
				if (monumentInfo2.Type != MonumentType.Cave && monumentInfo2.Type != MonumentType.WaterWell && monumentInfo2.Tier != MonumentTier.Tier0 && !monumentInfo2.IsSafeZone && (monumentInfo2.Tier & MonumentTier.Tier0) <= (MonumentTier)0)
				{
					bool flag = false;
					foreach (Vector3 b in this.visitedPatrolPoints)
					{
						if (Vector3Ex.Distance2D(monumentInfo2.transform.position, b) < 100f)
						{
							flag = true;
							break;
						}
					}
					if (!flag)
					{
						monumentInfo = monumentInfo2;
						break;
					}
				}
			}
			if (monumentInfo == null)
			{
				this.visitedPatrolPoints.Clear();
				monumentInfo = this.GetRandomValidMonumentInfo();
			}
		}
		if (monumentInfo != null)
		{
			this.visitedPatrolPoints.Add(monumentInfo.transform.position);
			vector = monumentInfo.transform.position;
		}
		else
		{
			float x = TerrainMeta.Size.x;
			float y = 30f;
			vector = Vector3Ex.Range(-1f, 1f);
			vector.y = 0f;
			vector.Normalize();
			vector *= x * UnityEngine.Random.Range(0f, 0.75f);
			vector.y = y;
		}
		float num3 = Mathf.Max(TerrainMeta.WaterMap.GetHeight(vector), TerrainMeta.HeightMap.GetHeight(vector));
		float num4 = num3;
		RaycastHit raycastHit;
		if (Physics.SphereCast(vector + new Vector3(0f, 200f, 0f), 20f, Vector3.down, out raycastHit, 300f, 1218511105))
		{
			num4 = Mathf.Max(raycastHit.point.y, num3);
		}
		vector.y = num4 + 30f;
		return vector;
	}

	// Token: 0x06002628 RID: 9768 RVA: 0x000F0C78 File Offset: 0x000EEE78
	private MonumentInfo GetRandomValidMonumentInfo()
	{
		int count = TerrainMeta.Path.Monuments.Count;
		int num = UnityEngine.Random.Range(0, count);
		for (int i = 0; i < count; i++)
		{
			int num2 = i + num;
			if (num2 >= count)
			{
				num2 -= count;
			}
			MonumentInfo monumentInfo = TerrainMeta.Path.Monuments[num2];
			if (monumentInfo.Type != MonumentType.Cave && monumentInfo.Type != MonumentType.WaterWell && monumentInfo.Tier != MonumentTier.Tier0 && !monumentInfo.IsSafeZone)
			{
				return monumentInfo;
			}
		}
		return null;
	}
}
