using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000566 RID: 1382
public class SpawnDistribution
{
	// Token: 0x0400227B RID: 8827
	internal SpawnHandler Handler;

	// Token: 0x0400227C RID: 8828
	internal float Density;

	// Token: 0x0400227D RID: 8829
	internal int Count;

	// Token: 0x0400227E RID: 8830
	private WorldSpaceGrid<int> grid;

	// Token: 0x0400227F RID: 8831
	private Dictionary<uint, int> dict = new Dictionary<uint, int>();

	// Token: 0x04002280 RID: 8832
	private ByteQuadtree quadtree = new ByteQuadtree();

	// Token: 0x04002281 RID: 8833
	private Vector3 origin;

	// Token: 0x04002282 RID: 8834
	private Vector3 area;

	// Token: 0x06002A67 RID: 10855 RVA: 0x00102B44 File Offset: 0x00100D44
	public SpawnDistribution(SpawnHandler handler, byte[] baseValues, Vector3 origin, Vector3 area)
	{
		this.Handler = handler;
		this.quadtree.UpdateValues(baseValues);
		this.origin = origin;
		float num = 0f;
		for (int i = 0; i < baseValues.Length; i++)
		{
			num += (float)baseValues[i];
		}
		this.Density = num / (float)(255 * baseValues.Length);
		this.Count = 0;
		this.area = new Vector3(area.x / (float)this.quadtree.Size, area.y, area.z / (float)this.quadtree.Size);
		this.grid = new WorldSpaceGrid<int>(area.x, 20f);
	}

	// Token: 0x06002A68 RID: 10856 RVA: 0x00102C0C File Offset: 0x00100E0C
	public bool Sample(out Vector3 spawnPos, out Quaternion spawnRot, bool alignToNormal = false, float dithering = 0f)
	{
		return this.Sample(out spawnPos, out spawnRot, this.SampleNode(), alignToNormal, dithering);
	}

	// Token: 0x06002A69 RID: 10857 RVA: 0x00102C20 File Offset: 0x00100E20
	public bool Sample(out Vector3 spawnPos, out Quaternion spawnRot, ByteQuadtree.Element node, bool alignToNormal = false, float dithering = 0f)
	{
		if (this.Handler == null || TerrainMeta.HeightMap == null)
		{
			spawnPos = Vector3.zero;
			spawnRot = Quaternion.identity;
			return false;
		}
		LayerMask placementMask = this.Handler.PlacementMask;
		LayerMask placementCheckMask = this.Handler.PlacementCheckMask;
		float placementCheckHeight = this.Handler.PlacementCheckHeight;
		LayerMask radiusCheckMask = this.Handler.RadiusCheckMask;
		float radiusCheckDistance = this.Handler.RadiusCheckDistance;
		for (int i = 0; i < 15; i++)
		{
			spawnPos = this.origin;
			spawnPos.x += node.Coords.x * this.area.x;
			spawnPos.z += node.Coords.y * this.area.z;
			spawnPos.x += UnityEngine.Random.value * this.area.x;
			spawnPos.z += UnityEngine.Random.value * this.area.z;
			spawnPos.x += UnityEngine.Random.Range(-dithering, dithering);
			spawnPos.z += UnityEngine.Random.Range(-dithering, dithering);
			Vector3 vector = new Vector3(spawnPos.x, TerrainMeta.HeightMap.GetHeight(spawnPos), spawnPos.z);
			if (vector.y > spawnPos.y)
			{
				RaycastHit raycastHit;
				if (placementCheckMask != 0 && Physics.Raycast(vector + Vector3.up * placementCheckHeight, Vector3.down, out raycastHit, placementCheckHeight, placementCheckMask))
				{
					if ((1 << raycastHit.transform.gameObject.layer & placementMask) == 0)
					{
						goto IL_243;
					}
					vector.y = raycastHit.point.y;
				}
				if (radiusCheckMask == 0 || !Physics.CheckSphere(vector, radiusCheckDistance, radiusCheckMask))
				{
					spawnPos.y = vector.y;
					spawnRot = Quaternion.Euler(new Vector3(0f, UnityEngine.Random.Range(0f, 360f), 0f));
					if (alignToNormal)
					{
						Vector3 normal = TerrainMeta.HeightMap.GetNormal(spawnPos);
						spawnRot = QuaternionEx.LookRotationForcedUp(spawnRot * Vector3.forward, normal);
					}
					return true;
				}
			}
			IL_243:;
		}
		spawnPos = Vector3.zero;
		spawnRot = Quaternion.identity;
		return false;
	}

	// Token: 0x06002A6A RID: 10858 RVA: 0x00102E98 File Offset: 0x00101098
	public ByteQuadtree.Element SampleNode()
	{
		ByteQuadtree.Element result = this.quadtree.Root;
		while (!result.IsLeaf)
		{
			result = result.RandChild;
		}
		return result;
	}

	// Token: 0x06002A6B RID: 10859 RVA: 0x00102EC5 File Offset: 0x001010C5
	public void AddInstance(Spawnable spawnable)
	{
		this.UpdateCount(spawnable, 1);
	}

	// Token: 0x06002A6C RID: 10860 RVA: 0x00102ECF File Offset: 0x001010CF
	public void RemoveInstance(Spawnable spawnable)
	{
		this.UpdateCount(spawnable, -1);
	}

	// Token: 0x06002A6D RID: 10861 RVA: 0x00102EDC File Offset: 0x001010DC
	private void UpdateCount(Spawnable spawnable, int delta)
	{
		this.Count += delta;
		WorldSpaceGrid<int> worldSpaceGrid = this.grid;
		Vector3 spawnPosition = spawnable.SpawnPosition;
		worldSpaceGrid[spawnPosition] += delta;
		BaseEntity component = spawnable.GetComponent<BaseEntity>();
		if (component)
		{
			int num;
			if (this.dict.TryGetValue(component.prefabID, out num))
			{
				this.dict[component.prefabID] = num + delta;
				return;
			}
			num = delta;
			this.dict.Add(component.prefabID, num);
		}
	}

	// Token: 0x06002A6E RID: 10862 RVA: 0x00102F64 File Offset: 0x00101164
	public int GetCount(uint prefabID)
	{
		int result;
		this.dict.TryGetValue(prefabID, out result);
		return result;
	}

	// Token: 0x06002A6F RID: 10863 RVA: 0x00102F81 File Offset: 0x00101181
	public int GetCount(Vector3 position)
	{
		return this.grid[position];
	}

	// Token: 0x06002A70 RID: 10864 RVA: 0x00102F8F File Offset: 0x0010118F
	public float GetGridCellArea()
	{
		return this.grid.CellArea;
	}
}
