﻿using System;
using UnityEngine;

// Token: 0x020006E5 RID: 1765
public class AddToHeightMap : ProceduralObject
{
	// Token: 0x040028BF RID: 10431
	public bool DestroyGameObject;

	// Token: 0x060031FA RID: 12794 RVA: 0x001341D4 File Offset: 0x001323D4
	public void Apply()
	{
		Collider component = base.GetComponent<Collider>();
		Bounds bounds = component.bounds;
		int num = TerrainMeta.HeightMap.Index(TerrainMeta.NormalizeX(bounds.min.x));
		int num2 = TerrainMeta.HeightMap.Index(TerrainMeta.NormalizeZ(bounds.max.x));
		int num3 = TerrainMeta.HeightMap.Index(TerrainMeta.NormalizeX(bounds.min.z));
		int num4 = TerrainMeta.HeightMap.Index(TerrainMeta.NormalizeZ(bounds.max.z));
		for (int i = num3; i <= num4; i++)
		{
			float normZ = TerrainMeta.HeightMap.Coordinate(i);
			for (int j = num; j <= num2; j++)
			{
				float normX = TerrainMeta.HeightMap.Coordinate(j);
				Vector3 origin = new Vector3(TerrainMeta.DenormalizeX(normX), bounds.max.y, TerrainMeta.DenormalizeZ(normZ));
				Ray ray = new Ray(origin, Vector3.down);
				RaycastHit raycastHit;
				if (component.Raycast(ray, out raycastHit, bounds.size.y))
				{
					float num5 = TerrainMeta.NormalizeY(raycastHit.point.y);
					float height = TerrainMeta.HeightMap.GetHeight01(j, i);
					if (num5 > height)
					{
						TerrainMeta.HeightMap.SetHeight(j, i, num5);
					}
				}
			}
		}
	}

	// Token: 0x060031FB RID: 12795 RVA: 0x00134327 File Offset: 0x00132527
	public override void Process()
	{
		this.Apply();
		if (this.DestroyGameObject)
		{
			GameManager.Destroy(base.gameObject, 0f);
			return;
		}
		GameManager.Destroy(this, 0f);
	}
}
