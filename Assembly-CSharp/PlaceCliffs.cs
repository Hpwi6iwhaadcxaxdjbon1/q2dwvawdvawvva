using System;
using System.Linq;
using UnityEngine;

// Token: 0x020006D4 RID: 1748
public class PlaceCliffs : ProceduralComponent
{
	// Token: 0x04002866 RID: 10342
	public SpawnFilter Filter;

	// Token: 0x04002867 RID: 10343
	public string ResourceFolder = string.Empty;

	// Token: 0x04002868 RID: 10344
	public int RetryMultiplier = 1;

	// Token: 0x04002869 RID: 10345
	public int CutoffSlope = 10;

	// Token: 0x0400286A RID: 10346
	public float MinScale = 1f;

	// Token: 0x0400286B RID: 10347
	public float MaxScale = 2f;

	// Token: 0x0400286C RID: 10348
	private static int target_count = 4;

	// Token: 0x0400286D RID: 10349
	private static int target_length = 0;

	// Token: 0x0400286E RID: 10350
	private static float min_scale_delta = 0.1f;

	// Token: 0x0400286F RID: 10351
	private static int max_scale_attempts = 10;

	// Token: 0x04002870 RID: 10352
	private static int min_rotation = PlaceCliffs.rotation_delta;

	// Token: 0x04002871 RID: 10353
	private static int max_rotation = 60;

	// Token: 0x04002872 RID: 10354
	private static int rotation_delta = 10;

	// Token: 0x04002873 RID: 10355
	private static float offset_c = 0f;

	// Token: 0x04002874 RID: 10356
	private static float offset_l = -0.75f;

	// Token: 0x04002875 RID: 10357
	private static float offset_r = 0.75f;

	// Token: 0x04002876 RID: 10358
	private static Vector3[] offsets = new Vector3[]
	{
		new Vector3(PlaceCliffs.offset_c, PlaceCliffs.offset_c, PlaceCliffs.offset_c),
		new Vector3(PlaceCliffs.offset_l, PlaceCliffs.offset_c, PlaceCliffs.offset_c),
		new Vector3(PlaceCliffs.offset_r, PlaceCliffs.offset_c, PlaceCliffs.offset_c),
		new Vector3(PlaceCliffs.offset_c, PlaceCliffs.offset_c, PlaceCliffs.offset_l),
		new Vector3(PlaceCliffs.offset_c, PlaceCliffs.offset_c, PlaceCliffs.offset_r)
	};

	// Token: 0x060031CC RID: 12748 RVA: 0x001303D8 File Offset: 0x0012E5D8
	public override void Process(uint seed)
	{
		if (World.Networked)
		{
			World.Spawn("Decor", "assets/bundled/prefabs/autospawn/" + this.ResourceFolder + "/");
			return;
		}
		TerrainHeightMap heightMap = TerrainMeta.HeightMap;
		Prefab[] array = Prefab.Load("assets/bundled/prefabs/autospawn/" + this.ResourceFolder, null, null, true);
		if (array == null || array.Length == 0)
		{
			return;
		}
		Prefab[] array2 = (from prefab in array
		where prefab.Attribute.Find<DecorSocketMale>(prefab.ID) && prefab.Attribute.Find<DecorSocketFemale>(prefab.ID)
		select prefab).ToArray<Prefab>();
		if (array2 == null || array2.Length == 0)
		{
			return;
		}
		Prefab[] array3 = (from prefab in array
		where prefab.Attribute.Find<DecorSocketMale>(prefab.ID)
		select prefab).ToArray<Prefab>();
		if (array3 == null || array3.Length == 0)
		{
			return;
		}
		Prefab[] array4 = (from prefab in array
		where prefab.Attribute.Find<DecorSocketFemale>(prefab.ID)
		select prefab).ToArray<Prefab>();
		if (array4 == null || array4.Length == 0)
		{
			return;
		}
		Vector3 position = TerrainMeta.Position;
		Vector3 size = TerrainMeta.Size;
		float x = position.x;
		float z = position.z;
		float max = position.x + size.x;
		float max2 = position.z + size.z;
		int num = Mathf.RoundToInt(size.x * size.z * 0.001f * (float)this.RetryMultiplier);
		for (int i = 0; i < num; i++)
		{
			float x2 = SeedRandom.Range(ref seed, x, max);
			float z2 = SeedRandom.Range(ref seed, z, max2);
			float normX = TerrainMeta.NormalizeX(x2);
			float normZ = TerrainMeta.NormalizeZ(z2);
			float num2 = SeedRandom.Value(ref seed);
			float factor = this.Filter.GetFactor(normX, normZ, true);
			Prefab random = array2.GetRandom(ref seed);
			if (factor * factor >= num2)
			{
				Vector3 normal = TerrainMeta.HeightMap.GetNormal(normX, normZ);
				if (Vector3.Angle(Vector3.up, normal) >= (float)this.CutoffSlope)
				{
					float height = heightMap.GetHeight(normX, normZ);
					Vector3 vector = new Vector3(x2, height, z2);
					Quaternion lhs = QuaternionEx.LookRotationForcedUp(normal, Vector3.up);
					float num3 = Mathf.Max((this.MaxScale - this.MinScale) / (float)PlaceCliffs.max_scale_attempts, PlaceCliffs.min_scale_delta);
					for (float num4 = this.MaxScale; num4 >= this.MinScale; num4 -= num3)
					{
						Vector3 vector2 = vector;
						Quaternion quaternion = lhs * random.Object.transform.localRotation;
						Vector3 vector3 = num4 * random.Object.transform.localScale;
						if (random.ApplyTerrainAnchors(ref vector2, quaternion, vector3, null) && random.ApplyTerrainChecks(vector2, quaternion, vector3, null) && random.ApplyTerrainFilters(vector2, quaternion, vector3, null) && random.ApplyWaterChecks(vector2, quaternion, vector3))
						{
							PlaceCliffs.CliffPlacement cliffPlacement = this.PlaceMale(array3, ref seed, random, vector2, quaternion, vector3);
							PlaceCliffs.CliffPlacement cliffPlacement2 = this.PlaceFemale(array4, ref seed, random, vector2, quaternion, vector3);
							World.AddPrefab("Decor", random, vector2, quaternion, vector3);
							while (cliffPlacement != null)
							{
								if (cliffPlacement.prefab == null)
								{
									break;
								}
								World.AddPrefab("Decor", cliffPlacement.prefab, cliffPlacement.pos, cliffPlacement.rot, cliffPlacement.scale);
								cliffPlacement = cliffPlacement.next;
								i++;
							}
							while (cliffPlacement2 != null)
							{
								if (cliffPlacement2.prefab == null)
								{
									break;
								}
								World.AddPrefab("Decor", cliffPlacement2.prefab, cliffPlacement2.pos, cliffPlacement2.rot, cliffPlacement2.scale);
								cliffPlacement2 = cliffPlacement2.next;
								i++;
							}
							break;
						}
					}
				}
			}
		}
	}

	// Token: 0x060031CD RID: 12749 RVA: 0x00130774 File Offset: 0x0012E974
	private PlaceCliffs.CliffPlacement PlaceMale(Prefab[] prefabs, ref uint seed, Prefab parentPrefab, Vector3 parentPos, Quaternion parentRot, Vector3 parentScale)
	{
		return this.Place<DecorSocketFemale, DecorSocketMale>(prefabs, ref seed, parentPrefab, parentPos, parentRot, parentScale, 0, 0, 0);
	}

	// Token: 0x060031CE RID: 12750 RVA: 0x00130794 File Offset: 0x0012E994
	private PlaceCliffs.CliffPlacement PlaceFemale(Prefab[] prefabs, ref uint seed, Prefab parentPrefab, Vector3 parentPos, Quaternion parentRot, Vector3 parentScale)
	{
		return this.Place<DecorSocketMale, DecorSocketFemale>(prefabs, ref seed, parentPrefab, parentPos, parentRot, parentScale, 0, 0, 0);
	}

	// Token: 0x060031CF RID: 12751 RVA: 0x001307B4 File Offset: 0x0012E9B4
	private PlaceCliffs.CliffPlacement Place<ParentSocketType, ChildSocketType>(Prefab[] prefabs, ref uint seed, Prefab parentPrefab, Vector3 parentPos, Quaternion parentRot, Vector3 parentScale, int parentAngle = 0, int parentCount = 0, int parentScore = 0) where ParentSocketType : PrefabAttribute where ChildSocketType : PrefabAttribute
	{
		PlaceCliffs.CliffPlacement cliffPlacement = null;
		if (parentAngle > 160 || parentAngle < -160)
		{
			return cliffPlacement;
		}
		int num = SeedRandom.Range(ref seed, 0, prefabs.Length);
		ParentSocketType parentSocketType = parentPrefab.Attribute.Find<ParentSocketType>(parentPrefab.ID);
		Vector3 a = parentPos + parentRot * Vector3.Scale(parentSocketType.worldPosition, parentScale);
		float num2 = Mathf.Max((this.MaxScale - this.MinScale) / (float)PlaceCliffs.max_scale_attempts, PlaceCliffs.min_scale_delta);
		for (int i = 0; i < prefabs.Length; i++)
		{
			Prefab prefab = prefabs[(num + i) % prefabs.Length];
			if (prefab != parentPrefab)
			{
				ParentSocketType parentSocketType2 = prefab.Attribute.Find<ParentSocketType>(prefab.ID);
				ChildSocketType childSocketType = prefab.Attribute.Find<ChildSocketType>(prefab.ID);
				bool flag = parentSocketType2 != null;
				if (cliffPlacement == null || cliffPlacement.count <= PlaceCliffs.target_count || cliffPlacement.score <= PlaceCliffs.target_length || !flag)
				{
					for (float num3 = this.MaxScale; num3 >= this.MinScale; num3 -= num2)
					{
						for (int j = PlaceCliffs.min_rotation; j <= PlaceCliffs.max_rotation; j += PlaceCliffs.rotation_delta)
						{
							for (int k = -1; k <= 1; k += 2)
							{
								Vector3[] array = PlaceCliffs.offsets;
								int l = 0;
								while (l < array.Length)
								{
									Vector3 b = array[l];
									Vector3 vector = parentScale * num3;
									Quaternion quaternion = Quaternion.Euler(0f, (float)(k * j), 0f) * parentRot;
									Vector3 vector2 = a - quaternion * (Vector3.Scale(childSocketType.worldPosition, vector) + b);
									if (this.Filter.GetFactor(vector2, true) >= 0.5f && prefab.ApplyTerrainAnchors(ref vector2, quaternion, vector, null) && prefab.ApplyTerrainChecks(vector2, quaternion, vector, null) && prefab.ApplyTerrainFilters(vector2, quaternion, vector, null) && prefab.ApplyWaterChecks(vector2, quaternion, vector))
									{
										int parentAngle2 = parentAngle + j;
										int num4 = parentCount + 1;
										int num5 = parentScore + Mathf.CeilToInt(Vector3Ex.Distance2D(parentPos, vector2));
										PlaceCliffs.CliffPlacement cliffPlacement2 = null;
										if (flag)
										{
											cliffPlacement2 = this.Place<ParentSocketType, ChildSocketType>(prefabs, ref seed, prefab, vector2, quaternion, vector, parentAngle2, num4, num5);
											if (cliffPlacement2 != null)
											{
												num4 = cliffPlacement2.count;
												num5 = cliffPlacement2.score;
											}
										}
										else
										{
											num5 *= 2;
										}
										if (cliffPlacement == null)
										{
											cliffPlacement = new PlaceCliffs.CliffPlacement();
										}
										if (cliffPlacement.score < num5)
										{
											cliffPlacement.next = cliffPlacement2;
											cliffPlacement.count = num4;
											cliffPlacement.score = num5;
											cliffPlacement.prefab = prefab;
											cliffPlacement.pos = vector2;
											cliffPlacement.rot = quaternion;
											cliffPlacement.scale = vector;
											goto IL_2D3;
										}
										goto IL_2D3;
									}
									else
									{
										l++;
									}
								}
							}
						}
					}
				}
			}
			IL_2D3:;
		}
		return cliffPlacement;
	}

	// Token: 0x02000E0B RID: 3595
	private class CliffPlacement
	{
		// Token: 0x040049BA RID: 18874
		public int count;

		// Token: 0x040049BB RID: 18875
		public int score;

		// Token: 0x040049BC RID: 18876
		public Prefab prefab;

		// Token: 0x040049BD RID: 18877
		public Vector3 pos = Vector3.zero;

		// Token: 0x040049BE RID: 18878
		public Quaternion rot = Quaternion.identity;

		// Token: 0x040049BF RID: 18879
		public Vector3 scale = Vector3.one;

		// Token: 0x040049C0 RID: 18880
		public PlaceCliffs.CliffPlacement next;
	}
}
