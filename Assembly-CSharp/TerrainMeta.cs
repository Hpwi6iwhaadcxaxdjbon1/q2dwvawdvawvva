using System;
using UnityEngine;

// Token: 0x020006A2 RID: 1698
[ExecuteInEditMode]
public class TerrainMeta : MonoBehaviour
{
	// Token: 0x0400278F RID: 10127
	public Terrain terrain;

	// Token: 0x04002790 RID: 10128
	public TerrainConfig config;

	// Token: 0x04002791 RID: 10129
	public TerrainMeta.PaintMode paint;

	// Token: 0x04002792 RID: 10130
	[HideInInspector]
	public TerrainMeta.PaintMode currentPaintMode;

	// Token: 0x170003E9 RID: 1001
	// (get) Token: 0x060030D6 RID: 12502 RVA: 0x00124DED File Offset: 0x00122FED
	// (set) Token: 0x060030D7 RID: 12503 RVA: 0x00124DF4 File Offset: 0x00122FF4
	public static TerrainConfig Config { get; private set; }

	// Token: 0x170003EA RID: 1002
	// (get) Token: 0x060030D8 RID: 12504 RVA: 0x00124DFC File Offset: 0x00122FFC
	// (set) Token: 0x060030D9 RID: 12505 RVA: 0x00124E03 File Offset: 0x00123003
	public static Terrain Terrain { get; private set; }

	// Token: 0x170003EB RID: 1003
	// (get) Token: 0x060030DA RID: 12506 RVA: 0x00124E0B File Offset: 0x0012300B
	// (set) Token: 0x060030DB RID: 12507 RVA: 0x00124E12 File Offset: 0x00123012
	public static Transform Transform { get; private set; }

	// Token: 0x170003EC RID: 1004
	// (get) Token: 0x060030DC RID: 12508 RVA: 0x00124E1A File Offset: 0x0012301A
	// (set) Token: 0x060030DD RID: 12509 RVA: 0x00124E21 File Offset: 0x00123021
	public static Vector3 Position { get; private set; }

	// Token: 0x170003ED RID: 1005
	// (get) Token: 0x060030DE RID: 12510 RVA: 0x00124E29 File Offset: 0x00123029
	// (set) Token: 0x060030DF RID: 12511 RVA: 0x00124E30 File Offset: 0x00123030
	public static Vector3 Size { get; private set; }

	// Token: 0x170003EE RID: 1006
	// (get) Token: 0x060030E0 RID: 12512 RVA: 0x00124E38 File Offset: 0x00123038
	public static Vector3 Center
	{
		get
		{
			return TerrainMeta.Position + TerrainMeta.Size * 0.5f;
		}
	}

	// Token: 0x170003EF RID: 1007
	// (get) Token: 0x060030E1 RID: 12513 RVA: 0x00124E53 File Offset: 0x00123053
	// (set) Token: 0x060030E2 RID: 12514 RVA: 0x00124E5A File Offset: 0x0012305A
	public static Vector3 OneOverSize { get; private set; }

	// Token: 0x170003F0 RID: 1008
	// (get) Token: 0x060030E3 RID: 12515 RVA: 0x00124E62 File Offset: 0x00123062
	// (set) Token: 0x060030E4 RID: 12516 RVA: 0x00124E69 File Offset: 0x00123069
	public static Vector3 HighestPoint { get; set; }

	// Token: 0x170003F1 RID: 1009
	// (get) Token: 0x060030E5 RID: 12517 RVA: 0x00124E71 File Offset: 0x00123071
	// (set) Token: 0x060030E6 RID: 12518 RVA: 0x00124E78 File Offset: 0x00123078
	public static Vector3 LowestPoint { get; set; }

	// Token: 0x170003F2 RID: 1010
	// (get) Token: 0x060030E7 RID: 12519 RVA: 0x00124E80 File Offset: 0x00123080
	// (set) Token: 0x060030E8 RID: 12520 RVA: 0x00124E87 File Offset: 0x00123087
	public static float LootAxisAngle { get; private set; }

	// Token: 0x170003F3 RID: 1011
	// (get) Token: 0x060030E9 RID: 12521 RVA: 0x00124E8F File Offset: 0x0012308F
	// (set) Token: 0x060030EA RID: 12522 RVA: 0x00124E96 File Offset: 0x00123096
	public static float BiomeAxisAngle { get; private set; }

	// Token: 0x170003F4 RID: 1012
	// (get) Token: 0x060030EB RID: 12523 RVA: 0x00124E9E File Offset: 0x0012309E
	// (set) Token: 0x060030EC RID: 12524 RVA: 0x00124EA5 File Offset: 0x001230A5
	public static TerrainData Data { get; private set; }

	// Token: 0x170003F5 RID: 1013
	// (get) Token: 0x060030ED RID: 12525 RVA: 0x00124EAD File Offset: 0x001230AD
	// (set) Token: 0x060030EE RID: 12526 RVA: 0x00124EB4 File Offset: 0x001230B4
	public static TerrainCollider Collider { get; private set; }

	// Token: 0x170003F6 RID: 1014
	// (get) Token: 0x060030EF RID: 12527 RVA: 0x00124EBC File Offset: 0x001230BC
	// (set) Token: 0x060030F0 RID: 12528 RVA: 0x00124EC3 File Offset: 0x001230C3
	public static TerrainCollision Collision { get; private set; }

	// Token: 0x170003F7 RID: 1015
	// (get) Token: 0x060030F1 RID: 12529 RVA: 0x00124ECB File Offset: 0x001230CB
	// (set) Token: 0x060030F2 RID: 12530 RVA: 0x00124ED2 File Offset: 0x001230D2
	public static TerrainPhysics Physics { get; private set; }

	// Token: 0x170003F8 RID: 1016
	// (get) Token: 0x060030F3 RID: 12531 RVA: 0x00124EDA File Offset: 0x001230DA
	// (set) Token: 0x060030F4 RID: 12532 RVA: 0x00124EE1 File Offset: 0x001230E1
	public static TerrainColors Colors { get; private set; }

	// Token: 0x170003F9 RID: 1017
	// (get) Token: 0x060030F5 RID: 12533 RVA: 0x00124EE9 File Offset: 0x001230E9
	// (set) Token: 0x060030F6 RID: 12534 RVA: 0x00124EF0 File Offset: 0x001230F0
	public static TerrainQuality Quality { get; private set; }

	// Token: 0x170003FA RID: 1018
	// (get) Token: 0x060030F7 RID: 12535 RVA: 0x00124EF8 File Offset: 0x001230F8
	// (set) Token: 0x060030F8 RID: 12536 RVA: 0x00124EFF File Offset: 0x001230FF
	public static TerrainPath Path { get; private set; }

	// Token: 0x170003FB RID: 1019
	// (get) Token: 0x060030F9 RID: 12537 RVA: 0x00124F07 File Offset: 0x00123107
	// (set) Token: 0x060030FA RID: 12538 RVA: 0x00124F0E File Offset: 0x0012310E
	public static TerrainBiomeMap BiomeMap { get; private set; }

	// Token: 0x170003FC RID: 1020
	// (get) Token: 0x060030FB RID: 12539 RVA: 0x00124F16 File Offset: 0x00123116
	// (set) Token: 0x060030FC RID: 12540 RVA: 0x00124F1D File Offset: 0x0012311D
	public static TerrainAlphaMap AlphaMap { get; private set; }

	// Token: 0x170003FD RID: 1021
	// (get) Token: 0x060030FD RID: 12541 RVA: 0x00124F25 File Offset: 0x00123125
	// (set) Token: 0x060030FE RID: 12542 RVA: 0x00124F2C File Offset: 0x0012312C
	public static TerrainBlendMap BlendMap { get; private set; }

	// Token: 0x170003FE RID: 1022
	// (get) Token: 0x060030FF RID: 12543 RVA: 0x00124F34 File Offset: 0x00123134
	// (set) Token: 0x06003100 RID: 12544 RVA: 0x00124F3B File Offset: 0x0012313B
	public static TerrainHeightMap HeightMap { get; private set; }

	// Token: 0x170003FF RID: 1023
	// (get) Token: 0x06003101 RID: 12545 RVA: 0x00124F43 File Offset: 0x00123143
	// (set) Token: 0x06003102 RID: 12546 RVA: 0x00124F4A File Offset: 0x0012314A
	public static TerrainSplatMap SplatMap { get; private set; }

	// Token: 0x17000400 RID: 1024
	// (get) Token: 0x06003103 RID: 12547 RVA: 0x00124F52 File Offset: 0x00123152
	// (set) Token: 0x06003104 RID: 12548 RVA: 0x00124F59 File Offset: 0x00123159
	public static TerrainTopologyMap TopologyMap { get; private set; }

	// Token: 0x17000401 RID: 1025
	// (get) Token: 0x06003105 RID: 12549 RVA: 0x00124F61 File Offset: 0x00123161
	// (set) Token: 0x06003106 RID: 12550 RVA: 0x00124F68 File Offset: 0x00123168
	public static TerrainWaterMap WaterMap { get; private set; }

	// Token: 0x17000402 RID: 1026
	// (get) Token: 0x06003107 RID: 12551 RVA: 0x00124F70 File Offset: 0x00123170
	// (set) Token: 0x06003108 RID: 12552 RVA: 0x00124F77 File Offset: 0x00123177
	public static TerrainDistanceMap DistanceMap { get; private set; }

	// Token: 0x17000403 RID: 1027
	// (get) Token: 0x06003109 RID: 12553 RVA: 0x00124F7F File Offset: 0x0012317F
	// (set) Token: 0x0600310A RID: 12554 RVA: 0x00124F86 File Offset: 0x00123186
	public static TerrainPlacementMap PlacementMap { get; private set; }

	// Token: 0x17000404 RID: 1028
	// (get) Token: 0x0600310B RID: 12555 RVA: 0x00124F8E File Offset: 0x0012318E
	// (set) Token: 0x0600310C RID: 12556 RVA: 0x00124F95 File Offset: 0x00123195
	public static TerrainTexturing Texturing { get; private set; }

	// Token: 0x0600310D RID: 12557 RVA: 0x00124FA0 File Offset: 0x001231A0
	public static bool OutOfBounds(Vector3 worldPos)
	{
		return worldPos.x < TerrainMeta.Position.x || worldPos.z < TerrainMeta.Position.z || worldPos.x > TerrainMeta.Position.x + TerrainMeta.Size.x || worldPos.z > TerrainMeta.Position.z + TerrainMeta.Size.z;
	}

	// Token: 0x0600310E RID: 12558 RVA: 0x00125014 File Offset: 0x00123214
	public static bool OutOfMargin(Vector3 worldPos)
	{
		return worldPos.x < TerrainMeta.Position.x - TerrainMeta.Size.x || worldPos.z < TerrainMeta.Position.z - TerrainMeta.Size.z || worldPos.x > TerrainMeta.Position.x + TerrainMeta.Size.x + TerrainMeta.Size.x || worldPos.z > TerrainMeta.Position.z + TerrainMeta.Size.z + TerrainMeta.Size.z;
	}

	// Token: 0x0600310F RID: 12559 RVA: 0x001250B4 File Offset: 0x001232B4
	public static Vector3 RandomPointOffshore()
	{
		float num = UnityEngine.Random.Range(-1f, 1f);
		float num2 = UnityEngine.Random.Range(0f, 100f);
		Vector3 vector = new Vector3(Mathf.Min(TerrainMeta.Size.x, 4000f) - 100f, 0f, Mathf.Min(TerrainMeta.Size.z, 4000f) - 100f);
		if (num2 < 25f)
		{
			return TerrainMeta.Center + new Vector3(-vector.x, 0f, num * vector.z);
		}
		if (num2 < 50f)
		{
			return TerrainMeta.Center + new Vector3(vector.x, 0f, num * vector.z);
		}
		if (num2 < 75f)
		{
			return TerrainMeta.Center + new Vector3(num * vector.x, 0f, -vector.z);
		}
		return TerrainMeta.Center + new Vector3(num * vector.x, 0f, vector.z);
	}

	// Token: 0x06003110 RID: 12560 RVA: 0x001251C8 File Offset: 0x001233C8
	public static Vector3 Normalize(Vector3 worldPos)
	{
		float x = (worldPos.x - TerrainMeta.Position.x) * TerrainMeta.OneOverSize.x;
		float y = (worldPos.y - TerrainMeta.Position.y) * TerrainMeta.OneOverSize.y;
		float z = (worldPos.z - TerrainMeta.Position.z) * TerrainMeta.OneOverSize.z;
		return new Vector3(x, y, z);
	}

	// Token: 0x06003111 RID: 12561 RVA: 0x00125232 File Offset: 0x00123432
	public static float NormalizeX(float x)
	{
		return (x - TerrainMeta.Position.x) * TerrainMeta.OneOverSize.x;
	}

	// Token: 0x06003112 RID: 12562 RVA: 0x0012524B File Offset: 0x0012344B
	public static float NormalizeY(float y)
	{
		return (y - TerrainMeta.Position.y) * TerrainMeta.OneOverSize.y;
	}

	// Token: 0x06003113 RID: 12563 RVA: 0x00125264 File Offset: 0x00123464
	public static float NormalizeZ(float z)
	{
		return (z - TerrainMeta.Position.z) * TerrainMeta.OneOverSize.z;
	}

	// Token: 0x06003114 RID: 12564 RVA: 0x00125280 File Offset: 0x00123480
	public static Vector3 Denormalize(Vector3 normPos)
	{
		float x = TerrainMeta.Position.x + normPos.x * TerrainMeta.Size.x;
		float y = TerrainMeta.Position.y + normPos.y * TerrainMeta.Size.y;
		float z = TerrainMeta.Position.z + normPos.z * TerrainMeta.Size.z;
		return new Vector3(x, y, z);
	}

	// Token: 0x06003115 RID: 12565 RVA: 0x001252EA File Offset: 0x001234EA
	public static float DenormalizeX(float normX)
	{
		return TerrainMeta.Position.x + normX * TerrainMeta.Size.x;
	}

	// Token: 0x06003116 RID: 12566 RVA: 0x00125303 File Offset: 0x00123503
	public static float DenormalizeY(float normY)
	{
		return TerrainMeta.Position.y + normY * TerrainMeta.Size.y;
	}

	// Token: 0x06003117 RID: 12567 RVA: 0x0012531C File Offset: 0x0012351C
	public static float DenormalizeZ(float normZ)
	{
		return TerrainMeta.Position.z + normZ * TerrainMeta.Size.z;
	}

	// Token: 0x06003118 RID: 12568 RVA: 0x00125335 File Offset: 0x00123535
	protected void Awake()
	{
		if (!Application.isPlaying)
		{
			return;
		}
		Shader.DisableKeyword("TERRAIN_PAINTING");
	}

	// Token: 0x06003119 RID: 12569 RVA: 0x0012534C File Offset: 0x0012354C
	public void Init(Terrain terrainOverride = null, TerrainConfig configOverride = null)
	{
		if (terrainOverride != null)
		{
			this.terrain = terrainOverride;
		}
		if (configOverride != null)
		{
			this.config = configOverride;
		}
		TerrainMeta.Terrain = this.terrain;
		TerrainMeta.Config = this.config;
		TerrainMeta.Transform = this.terrain.transform;
		TerrainMeta.Data = this.terrain.terrainData;
		TerrainMeta.Size = this.terrain.terrainData.size;
		TerrainMeta.OneOverSize = TerrainMeta.Size.Inverse();
		TerrainMeta.Position = this.terrain.GetPosition();
		TerrainMeta.Collider = this.terrain.GetComponent<TerrainCollider>();
		TerrainMeta.Collision = this.terrain.GetComponent<TerrainCollision>();
		TerrainMeta.Physics = this.terrain.GetComponent<TerrainPhysics>();
		TerrainMeta.Colors = this.terrain.GetComponent<TerrainColors>();
		TerrainMeta.Quality = this.terrain.GetComponent<TerrainQuality>();
		TerrainMeta.Path = this.terrain.GetComponent<TerrainPath>();
		TerrainMeta.BiomeMap = this.terrain.GetComponent<TerrainBiomeMap>();
		TerrainMeta.AlphaMap = this.terrain.GetComponent<TerrainAlphaMap>();
		TerrainMeta.BlendMap = this.terrain.GetComponent<TerrainBlendMap>();
		TerrainMeta.HeightMap = this.terrain.GetComponent<TerrainHeightMap>();
		TerrainMeta.SplatMap = this.terrain.GetComponent<TerrainSplatMap>();
		TerrainMeta.TopologyMap = this.terrain.GetComponent<TerrainTopologyMap>();
		TerrainMeta.WaterMap = this.terrain.GetComponent<TerrainWaterMap>();
		TerrainMeta.DistanceMap = this.terrain.GetComponent<TerrainDistanceMap>();
		TerrainMeta.PlacementMap = this.terrain.GetComponent<TerrainPlacementMap>();
		TerrainMeta.Texturing = this.terrain.GetComponent<TerrainTexturing>();
		this.terrain.drawInstanced = false;
		TerrainMeta.HighestPoint = new Vector3(TerrainMeta.Position.x, TerrainMeta.Position.y + TerrainMeta.Size.y, TerrainMeta.Position.z);
		TerrainMeta.LowestPoint = new Vector3(TerrainMeta.Position.x, TerrainMeta.Position.y, TerrainMeta.Position.z);
		TerrainExtension[] components = base.GetComponents<TerrainExtension>();
		for (int i = 0; i < components.Length; i++)
		{
			components[i].Init(this.terrain, this.config);
		}
		uint seed = World.Seed;
		int num = SeedRandom.Range(ref seed, 0, 4) * 90;
		int num2 = SeedRandom.Range(ref seed, -45, 46);
		int num3 = SeedRandom.Sign(ref seed);
		TerrainMeta.LootAxisAngle = (float)num;
		TerrainMeta.BiomeAxisAngle = (float)(num + num2 + num3 * 90);
	}

	// Token: 0x0600311A RID: 12570 RVA: 0x001255B2 File Offset: 0x001237B2
	public static void InitNoTerrain(bool createPath = false)
	{
		TerrainMeta.Size = new Vector3(4096f, 4096f, 4096f);
		TerrainMeta.OneOverSize = TerrainMeta.Size.Inverse();
		TerrainMeta.Position = -0.5f * TerrainMeta.Size;
	}

	// Token: 0x0600311B RID: 12571 RVA: 0x001255F0 File Offset: 0x001237F0
	public void SetupComponents()
	{
		foreach (TerrainExtension terrainExtension in base.GetComponents<TerrainExtension>())
		{
			terrainExtension.Setup();
			terrainExtension.isInitialized = true;
		}
	}

	// Token: 0x0600311C RID: 12572 RVA: 0x00125624 File Offset: 0x00123824
	public void PostSetupComponents()
	{
		TerrainExtension[] components = base.GetComponents<TerrainExtension>();
		for (int i = 0; i < components.Length; i++)
		{
			components[i].PostSetup();
		}
	}

	// Token: 0x0600311D RID: 12573 RVA: 0x00125650 File Offset: 0x00123850
	public void BindShaderProperties()
	{
		if (this.config)
		{
			Shader.SetGlobalTexture("Terrain_AlbedoArray", this.config.AlbedoArray);
			Shader.SetGlobalTexture("Terrain_NormalArray", this.config.NormalArray);
			Shader.SetGlobalVector("Terrain_TexelSize", new Vector2(1f / this.config.GetMinSplatTiling(), 1f / this.config.GetMinSplatTiling()));
			Shader.SetGlobalVector("Terrain_TexelSize0", new Vector4(1f / this.config.Splats[0].SplatTiling, 1f / this.config.Splats[1].SplatTiling, 1f / this.config.Splats[2].SplatTiling, 1f / this.config.Splats[3].SplatTiling));
			Shader.SetGlobalVector("Terrain_TexelSize1", new Vector4(1f / this.config.Splats[4].SplatTiling, 1f / this.config.Splats[5].SplatTiling, 1f / this.config.Splats[6].SplatTiling, 1f / this.config.Splats[7].SplatTiling));
			Shader.SetGlobalVector("Splat0_UVMIX", new Vector3(this.config.Splats[0].UVMIXMult, this.config.Splats[0].UVMIXStart, 1f / this.config.Splats[0].UVMIXDist));
			Shader.SetGlobalVector("Splat1_UVMIX", new Vector3(this.config.Splats[1].UVMIXMult, this.config.Splats[1].UVMIXStart, 1f / this.config.Splats[1].UVMIXDist));
			Shader.SetGlobalVector("Splat2_UVMIX", new Vector3(this.config.Splats[2].UVMIXMult, this.config.Splats[2].UVMIXStart, 1f / this.config.Splats[2].UVMIXDist));
			Shader.SetGlobalVector("Splat3_UVMIX", new Vector3(this.config.Splats[3].UVMIXMult, this.config.Splats[3].UVMIXStart, 1f / this.config.Splats[3].UVMIXDist));
			Shader.SetGlobalVector("Splat4_UVMIX", new Vector3(this.config.Splats[4].UVMIXMult, this.config.Splats[4].UVMIXStart, 1f / this.config.Splats[4].UVMIXDist));
			Shader.SetGlobalVector("Splat5_UVMIX", new Vector3(this.config.Splats[5].UVMIXMult, this.config.Splats[5].UVMIXStart, 1f / this.config.Splats[5].UVMIXDist));
			Shader.SetGlobalVector("Splat6_UVMIX", new Vector3(this.config.Splats[6].UVMIXMult, this.config.Splats[6].UVMIXStart, 1f / this.config.Splats[6].UVMIXDist));
			Shader.SetGlobalVector("Splat7_UVMIX", new Vector3(this.config.Splats[7].UVMIXMult, this.config.Splats[7].UVMIXStart, 1f / this.config.Splats[7].UVMIXDist));
		}
		if (TerrainMeta.HeightMap)
		{
			Shader.SetGlobalTexture("Terrain_Normal", TerrainMeta.HeightMap.NormalTexture);
		}
		if (TerrainMeta.AlphaMap)
		{
			Shader.SetGlobalTexture("Terrain_Alpha", TerrainMeta.AlphaMap.AlphaTexture);
		}
		if (TerrainMeta.BiomeMap)
		{
			Shader.SetGlobalTexture("Terrain_Biome", TerrainMeta.BiomeMap.BiomeTexture);
		}
		if (TerrainMeta.SplatMap)
		{
			Shader.SetGlobalTexture("Terrain_Control0", TerrainMeta.SplatMap.SplatTexture0);
			Shader.SetGlobalTexture("Terrain_Control1", TerrainMeta.SplatMap.SplatTexture1);
		}
		TerrainMeta.WaterMap;
		if (TerrainMeta.DistanceMap)
		{
			Shader.SetGlobalTexture("Terrain_Distance", TerrainMeta.DistanceMap.DistanceTexture);
		}
		if (this.terrain)
		{
			Shader.SetGlobalVector("Terrain_Position", TerrainMeta.Position);
			Shader.SetGlobalVector("Terrain_Size", TerrainMeta.Size);
			Shader.SetGlobalVector("Terrain_RcpSize", TerrainMeta.OneOverSize);
			if (this.terrain.materialTemplate)
			{
				if (this.terrain.materialTemplate.IsKeywordEnabled("_TERRAIN_BLEND_LINEAR"))
				{
					this.terrain.materialTemplate.DisableKeyword("_TERRAIN_BLEND_LINEAR");
				}
				if (this.terrain.materialTemplate.IsKeywordEnabled("_TERRAIN_VERTEX_NORMALS"))
				{
					this.terrain.materialTemplate.DisableKeyword("_TERRAIN_VERTEX_NORMALS");
				}
			}
		}
	}

	// Token: 0x02000DD1 RID: 3537
	public enum PaintMode
	{
		// Token: 0x04004910 RID: 18704
		None,
		// Token: 0x04004911 RID: 18705
		Splats,
		// Token: 0x04004912 RID: 18706
		Biomes,
		// Token: 0x04004913 RID: 18707
		Alpha,
		// Token: 0x04004914 RID: 18708
		Blend,
		// Token: 0x04004915 RID: 18709
		Field,
		// Token: 0x04004916 RID: 18710
		Cliff,
		// Token: 0x04004917 RID: 18711
		Summit,
		// Token: 0x04004918 RID: 18712
		Beachside,
		// Token: 0x04004919 RID: 18713
		Beach,
		// Token: 0x0400491A RID: 18714
		Forest,
		// Token: 0x0400491B RID: 18715
		Forestside,
		// Token: 0x0400491C RID: 18716
		Ocean,
		// Token: 0x0400491D RID: 18717
		Oceanside,
		// Token: 0x0400491E RID: 18718
		Decor,
		// Token: 0x0400491F RID: 18719
		Monument,
		// Token: 0x04004920 RID: 18720
		Road,
		// Token: 0x04004921 RID: 18721
		Roadside,
		// Token: 0x04004922 RID: 18722
		Bridge,
		// Token: 0x04004923 RID: 18723
		River,
		// Token: 0x04004924 RID: 18724
		Riverside,
		// Token: 0x04004925 RID: 18725
		Lake,
		// Token: 0x04004926 RID: 18726
		Lakeside,
		// Token: 0x04004927 RID: 18727
		Offshore,
		// Token: 0x04004928 RID: 18728
		Rail,
		// Token: 0x04004929 RID: 18729
		Railside,
		// Token: 0x0400492A RID: 18730
		Building,
		// Token: 0x0400492B RID: 18731
		Cliffside,
		// Token: 0x0400492C RID: 18732
		Mountain,
		// Token: 0x0400492D RID: 18733
		Clutter,
		// Token: 0x0400492E RID: 18734
		Alt,
		// Token: 0x0400492F RID: 18735
		Tier0,
		// Token: 0x04004930 RID: 18736
		Tier1,
		// Token: 0x04004931 RID: 18737
		Tier2,
		// Token: 0x04004932 RID: 18738
		Mainland,
		// Token: 0x04004933 RID: 18739
		Hilltop
	}
}
