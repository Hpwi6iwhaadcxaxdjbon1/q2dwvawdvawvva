using System;
using UnityEngine;

// Token: 0x020006D8 RID: 1752
public class PlaceMonument : ProceduralComponent
{
	// Token: 0x04002882 RID: 10370
	public SpawnFilter Filter;

	// Token: 0x04002883 RID: 10371
	public GameObjectRef Monument;

	// Token: 0x04002884 RID: 10372
	private const int Attempts = 10000;

	// Token: 0x060031D8 RID: 12760 RVA: 0x00131260 File Offset: 0x0012F460
	public override void Process(uint seed)
	{
		if (!this.Monument.isValid)
		{
			return;
		}
		TerrainHeightMap heightMap = TerrainMeta.HeightMap;
		Vector3 position = TerrainMeta.Position;
		Vector3 size = TerrainMeta.Size;
		float x = position.x;
		float z = position.z;
		float max = position.x + size.x;
		float max2 = position.z + size.z;
		PlaceMonument.SpawnInfo spawnInfo = default(PlaceMonument.SpawnInfo);
		int num = int.MinValue;
		Prefab<MonumentInfo> prefab = Prefab.Load<MonumentInfo>(this.Monument.resourceID, null, null);
		for (int i = 0; i < 10000; i++)
		{
			float x2 = SeedRandom.Range(ref seed, x, max);
			float z2 = SeedRandom.Range(ref seed, z, max2);
			float normX = TerrainMeta.NormalizeX(x2);
			float normZ = TerrainMeta.NormalizeZ(z2);
			float num2 = SeedRandom.Value(ref seed);
			float factor = this.Filter.GetFactor(normX, normZ, true);
			if (factor * factor >= num2)
			{
				float height = heightMap.GetHeight(normX, normZ);
				Vector3 vector = new Vector3(x2, height, z2);
				Quaternion localRotation = prefab.Object.transform.localRotation;
				Vector3 localScale = prefab.Object.transform.localScale;
				prefab.ApplyDecorComponents(ref vector, ref localRotation, ref localScale);
				if ((!prefab.Component || prefab.Component.CheckPlacement(vector, localRotation, localScale)) && prefab.ApplyTerrainAnchors(ref vector, localRotation, localScale, this.Filter) && prefab.ApplyTerrainChecks(vector, localRotation, localScale, this.Filter) && prefab.ApplyTerrainFilters(vector, localRotation, localScale, null) && prefab.ApplyWaterChecks(vector, localRotation, localScale) && !prefab.CheckEnvironmentVolumes(vector, localRotation, localScale, EnvironmentType.Underground | EnvironmentType.TrainTunnels))
				{
					PlaceMonument.SpawnInfo spawnInfo2 = default(PlaceMonument.SpawnInfo);
					spawnInfo2.prefab = prefab;
					spawnInfo2.position = vector;
					spawnInfo2.rotation = localRotation;
					spawnInfo2.scale = localScale;
					int num3 = -Mathf.RoundToInt(vector.Magnitude2D());
					if (num3 > num)
					{
						num = num3;
						spawnInfo = spawnInfo2;
					}
				}
			}
		}
		if (num != -2147483648)
		{
			World.AddPrefab("Monument", spawnInfo.prefab, spawnInfo.position, spawnInfo.rotation, spawnInfo.scale);
		}
	}

	// Token: 0x02000E0D RID: 3597
	private struct SpawnInfo
	{
		// Token: 0x040049C5 RID: 18885
		public Prefab prefab;

		// Token: 0x040049C6 RID: 18886
		public Vector3 position;

		// Token: 0x040049C7 RID: 18887
		public Quaternion rotation;

		// Token: 0x040049C8 RID: 18888
		public Vector3 scale;
	}
}
