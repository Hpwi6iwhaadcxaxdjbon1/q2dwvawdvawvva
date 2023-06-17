using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

// Token: 0x02000707 RID: 1799
[ExecuteInEditMode]
public class WaterSystem : MonoBehaviour
{
	// Token: 0x04002927 RID: 10535
	public WaterQuality Quality = WaterQuality.High;

	// Token: 0x04002928 RID: 10536
	public bool ShowDebug;

	// Token: 0x04002929 RID: 10537
	public bool ShowGizmos;

	// Token: 0x0400292A RID: 10538
	public bool ProgressTime = true;

	// Token: 0x0400292B RID: 10539
	public GameObject FallbackPlane;

	// Token: 0x0400292C RID: 10540
	public WaterSystem.SimulationSettings Simulation = new WaterSystem.SimulationSettings();

	// Token: 0x0400292D RID: 10541
	public WaterSystem.RenderingSettings Rendering = new WaterSystem.RenderingSettings();

	// Token: 0x0400292E RID: 10542
	private WaterGerstner.PrecomputedWave[] precomputedWaves = new WaterGerstner.PrecomputedWave[]
	{
		WaterGerstner.PrecomputedWave.Default,
		WaterGerstner.PrecomputedWave.Default,
		WaterGerstner.PrecomputedWave.Default,
		WaterGerstner.PrecomputedWave.Default,
		WaterGerstner.PrecomputedWave.Default,
		WaterGerstner.PrecomputedWave.Default
	};

	// Token: 0x0400292F RID: 10543
	private WaterGerstner.PrecomputedShoreWaves precomputedShoreWaves = WaterGerstner.PrecomputedShoreWaves.Default;

	// Token: 0x04002930 RID: 10544
	private Vector4[] waveArray = new Vector4[0];

	// Token: 0x04002931 RID: 10545
	private Vector4[] shoreWaveArray = new Vector4[0];

	// Token: 0x04002932 RID: 10546
	private Vector4 global0;

	// Token: 0x04002933 RID: 10547
	private Vector4 global1;

	// Token: 0x0400293B RID: 10555
	private static float oceanLevel = 0f;

	// Token: 0x0400293D RID: 10557
	private static WaterSystem instance;

	// Token: 0x0400293E RID: 10558
	private static Vector3[] emptyShoreMap = new Vector3[]
	{
		Vector3.one
	};

	// Token: 0x0400293F RID: 10559
	private static short[] emptyWaterMap = new short[1];

	// Token: 0x04002940 RID: 10560
	private static short[] emptyHeightMap = new short[1];

	// Token: 0x04002941 RID: 10561
	private static WaterSystem.NativePathState nativePathState = WaterSystem.NativePathState.Initializing;

	// Token: 0x04002942 RID: 10562
	private static Vector3[] currentShoreMap;

	// Token: 0x04002943 RID: 10563
	private static GCHandle currentShoreMapHandle;

	// Token: 0x04002944 RID: 10564
	private static short[] currentWaterMap;

	// Token: 0x04002945 RID: 10565
	private static GCHandle currentWaterMapHandle;

	// Token: 0x04002946 RID: 10566
	private static short[] currentHeightMap;

	// Token: 0x04002947 RID: 10567
	private static GCHandle currentHeightMapHandle;

	// Token: 0x04002948 RID: 10568
	private static Vector4[] currentOpenWaves;

	// Token: 0x04002949 RID: 10569
	private static GCHandle currentOpenWavesHandle;

	// Token: 0x0400294A RID: 10570
	private static Vector4[] currentShoreWaves;

	// Token: 0x0400294B RID: 10571
	private static GCHandle currentShoreWavesHandle;

	// Token: 0x1700041F RID: 1055
	// (get) Token: 0x060032B1 RID: 12977 RVA: 0x00138C99 File Offset: 0x00136E99
	public WaterGerstner.PrecomputedWave[] PrecomputedWaves
	{
		get
		{
			return this.precomputedWaves;
		}
	}

	// Token: 0x17000420 RID: 1056
	// (get) Token: 0x060032B2 RID: 12978 RVA: 0x00138CA1 File Offset: 0x00136EA1
	public WaterGerstner.PrecomputedShoreWaves PrecomputedShoreWaves
	{
		get
		{
			return this.precomputedShoreWaves;
		}
	}

	// Token: 0x17000421 RID: 1057
	// (get) Token: 0x060032B3 RID: 12979 RVA: 0x00138CA9 File Offset: 0x00136EA9
	public Vector4 Global0
	{
		get
		{
			return this.global0;
		}
	}

	// Token: 0x17000422 RID: 1058
	// (get) Token: 0x060032B4 RID: 12980 RVA: 0x00138CB1 File Offset: 0x00136EB1
	public Vector4 Global1
	{
		get
		{
			return this.global1;
		}
	}

	// Token: 0x17000423 RID: 1059
	// (get) Token: 0x060032B5 RID: 12981 RVA: 0x00138CB9 File Offset: 0x00136EB9
	// (set) Token: 0x060032B6 RID: 12982 RVA: 0x00138CC1 File Offset: 0x00136EC1
	public float ShoreWavesRcpFadeDistance { get; private set; } = 0.04f;

	// Token: 0x17000424 RID: 1060
	// (get) Token: 0x060032B7 RID: 12983 RVA: 0x00138CCA File Offset: 0x00136ECA
	// (set) Token: 0x060032B8 RID: 12984 RVA: 0x00138CD2 File Offset: 0x00136ED2
	public float TerrainRcpFadeDistance { get; private set; } = 0.1f;

	// Token: 0x17000425 RID: 1061
	// (get) Token: 0x060032BA RID: 12986 RVA: 0x00138CE4 File Offset: 0x00136EE4
	// (set) Token: 0x060032B9 RID: 12985 RVA: 0x00138CDB File Offset: 0x00136EDB
	public bool IsInitialized { get; private set; }

	// Token: 0x17000426 RID: 1062
	// (get) Token: 0x060032BB RID: 12987 RVA: 0x00138CEC File Offset: 0x00136EEC
	// (set) Token: 0x060032BC RID: 12988 RVA: 0x00138CF3 File Offset: 0x00136EF3
	public static WaterCollision Collision { get; private set; }

	// Token: 0x17000427 RID: 1063
	// (get) Token: 0x060032BE RID: 12990 RVA: 0x00138D03 File Offset: 0x00136F03
	// (set) Token: 0x060032BD RID: 12989 RVA: 0x00138CFB File Offset: 0x00136EFB
	public static WaterDynamics Dynamics { get; private set; }

	// Token: 0x17000428 RID: 1064
	// (get) Token: 0x060032C0 RID: 12992 RVA: 0x00138D12 File Offset: 0x00136F12
	// (set) Token: 0x060032BF RID: 12991 RVA: 0x00138D0A File Offset: 0x00136F0A
	public static WaterBody Ocean { get; private set; } = null;

	// Token: 0x17000429 RID: 1065
	// (get) Token: 0x060032C2 RID: 12994 RVA: 0x00138D21 File Offset: 0x00136F21
	// (set) Token: 0x060032C1 RID: 12993 RVA: 0x00138D19 File Offset: 0x00136F19
	public static HashSet<WaterBody> WaterBodies { get; private set; } = new HashSet<WaterBody>();

	// Token: 0x1700042A RID: 1066
	// (get) Token: 0x060032C3 RID: 12995 RVA: 0x00138D28 File Offset: 0x00136F28
	// (set) Token: 0x060032C4 RID: 12996 RVA: 0x00138D2F File Offset: 0x00136F2F
	public static float OceanLevel
	{
		get
		{
			return WaterSystem.oceanLevel;
		}
		set
		{
			value = Mathf.Max(value, 0f);
			if (!Mathf.Approximately(WaterSystem.oceanLevel, value))
			{
				WaterSystem.oceanLevel = value;
				WaterSystem.UpdateOceanLevel();
			}
		}
	}

	// Token: 0x1700042B RID: 1067
	// (get) Token: 0x060032C5 RID: 12997 RVA: 0x00138D59 File Offset: 0x00136F59
	// (set) Token: 0x060032C6 RID: 12998 RVA: 0x00138D60 File Offset: 0x00136F60
	public static float WaveTime { get; private set; } = 0f;

	// Token: 0x1700042C RID: 1068
	// (get) Token: 0x060032C7 RID: 12999 RVA: 0x00138D68 File Offset: 0x00136F68
	public static WaterSystem Instance
	{
		get
		{
			return WaterSystem.instance;
		}
	}

	// Token: 0x060032C8 RID: 13000 RVA: 0x00138D70 File Offset: 0x00136F70
	private void CheckInstance()
	{
		WaterSystem.instance = ((WaterSystem.instance != null) ? WaterSystem.instance : this);
		WaterSystem.Collision = ((WaterSystem.Collision != null) ? WaterSystem.Collision : base.GetComponent<WaterCollision>());
		WaterSystem.Dynamics = ((WaterSystem.Dynamics != null) ? WaterSystem.Dynamics : base.GetComponent<WaterDynamics>());
	}

	// Token: 0x060032C9 RID: 13001 RVA: 0x00138DD5 File Offset: 0x00136FD5
	public void Awake()
	{
		this.CheckInstance();
	}

	// Token: 0x060032CA RID: 13002
	[DllImport("RustNative", EntryPoint = "Water_SetBaseConstants")]
	private static extern void SetBaseConstants_Native(int shoreMapSize, ref Vector3 shoreMap, int waterHeightMapSize, ref short waterHeightMap, Vector4 packedParams);

	// Token: 0x060032CB RID: 13003
	[DllImport("RustNative", EntryPoint = "Water_SetTerrainConstants")]
	private static extern void SetTerrainConstants_Native(int terrainHeightMapSize, ref short terrainHeightMap, Vector3 terrainPosition, Vector3 terrainSize);

	// Token: 0x060032CC RID: 13004
	[DllImport("RustNative", EntryPoint = "Water_SetGerstnerConstants")]
	private static extern void SetGerstnerConstants_Native(Vector4 globalParams0, Vector4 globalParams1, ref Vector4 openWaves, ref Vector4 shoreWaves);

	// Token: 0x060032CD RID: 13005
	[DllImport("RustNative", EntryPoint = "Water_UpdateOceanLevel")]
	private static extern void UpdateOceanLevel_Native(float oceanWaterLevel);

	// Token: 0x060032CE RID: 13006
	[DllImport("RustNative", EntryPoint = "Water_GetHeightArray")]
	private static extern float GetHeightArray_Native(int sampleCount, ref Vector2 pos, ref Vector2 posUV, ref Vector3 shore, ref float terrainHeight, ref float waterHeight);

	// Token: 0x060032CF RID: 13007
	[DllImport("RustNative", EntryPoint = "Water_GetHeight")]
	private static extern float GetHeight_Native(Vector3 pos);

	// Token: 0x060032D0 RID: 13008
	[DllImport("RustNative")]
	private static extern bool CPU_SupportsSSE41();

	// Token: 0x060032D1 RID: 13009 RVA: 0x00138DE0 File Offset: 0x00136FE0
	private static void SetNativeConstants(TerrainTexturing terrainTexturing, TerrainWaterMap terrainWaterMap, TerrainHeightMap terrainHeightMap, Vector4 globalParams0, Vector4 globalParams1, Vector4[] openWaves, Vector4[] shoreWaves)
	{
		if (WaterSystem.nativePathState == WaterSystem.NativePathState.Initializing)
		{
			try
			{
				WaterSystem.nativePathState = ((!WaterSystem.CPU_SupportsSSE41()) ? WaterSystem.NativePathState.Failed : WaterSystem.nativePathState);
			}
			catch (EntryPointNotFoundException)
			{
				WaterSystem.nativePathState = WaterSystem.NativePathState.Failed;
			}
		}
		if (WaterSystem.nativePathState != WaterSystem.NativePathState.Failed)
		{
			try
			{
				int shoreMapSize = 1;
				Vector3[] shoreMap = WaterSystem.emptyShoreMap;
				if (terrainTexturing != null && terrainTexturing.ShoreMap != null)
				{
					shoreMapSize = terrainTexturing.ShoreMapSize;
					shoreMap = terrainTexturing.ShoreMap;
				}
				int waterHeightMapSize = 1;
				short[] src = WaterSystem.emptyWaterMap;
				if (terrainWaterMap != null && terrainWaterMap.src != null && terrainWaterMap.src.Length != 0)
				{
					waterHeightMapSize = terrainWaterMap.res;
					src = terrainWaterMap.src;
				}
				int terrainHeightMapSize = 1;
				short[] src2 = WaterSystem.emptyHeightMap;
				if (terrainHeightMap != null && terrainHeightMap.src != null && terrainHeightMap.src.Length != 0)
				{
					terrainHeightMapSize = terrainHeightMap.res;
					src2 = terrainHeightMap.src;
				}
				Vector4 packedParams;
				packedParams.x = WaterSystem.OceanLevel;
				packedParams.y = ((WaterSystem.instance != null) ? 1f : 0f);
				packedParams.z = ((TerrainTexturing.Instance != null) ? 1f : 0f);
				packedParams.w = 0f;
				WaterSystem.PinObject<Vector3[]>(shoreMap, ref WaterSystem.currentShoreMap, ref WaterSystem.currentShoreMapHandle);
				WaterSystem.PinObject<short[]>(src, ref WaterSystem.currentWaterMap, ref WaterSystem.currentWaterMapHandle);
				WaterSystem.PinObject<short[]>(src2, ref WaterSystem.currentHeightMap, ref WaterSystem.currentHeightMapHandle);
				WaterSystem.PinObject<Vector4[]>(openWaves, ref WaterSystem.currentOpenWaves, ref WaterSystem.currentOpenWavesHandle);
				WaterSystem.PinObject<Vector4[]>(shoreWaves, ref WaterSystem.currentShoreWaves, ref WaterSystem.currentShoreWavesHandle);
				WaterSystem.SetBaseConstants_Native(shoreMapSize, ref shoreMap[0], waterHeightMapSize, ref src[0], packedParams);
				WaterSystem.SetTerrainConstants_Native(terrainHeightMapSize, ref src2[0], TerrainMeta.Position, TerrainMeta.Size);
				WaterSystem.SetGerstnerConstants_Native(globalParams0, globalParams1, ref openWaves[0], ref shoreWaves[0]);
				WaterSystem.nativePathState = WaterSystem.NativePathState.Ready;
			}
			catch (EntryPointNotFoundException)
			{
				WaterSystem.nativePathState = WaterSystem.NativePathState.Failed;
			}
		}
	}

	// Token: 0x060032D2 RID: 13010 RVA: 0x00138FE4 File Offset: 0x001371E4
	private static void PinObject<T>(T value, ref T currentValue, ref GCHandle currentValueHandle) where T : class
	{
		if (value != null && value != currentValue)
		{
			if (currentValueHandle.IsAllocated)
			{
				currentValueHandle.Free();
			}
			currentValue = value;
			currentValueHandle = GCHandle.Alloc(value, GCHandleType.Pinned);
		}
	}

	// Token: 0x060032D3 RID: 13011 RVA: 0x00139034 File Offset: 0x00137234
	private static float GetHeight_Managed(Vector3 pos)
	{
		Vector2 uv;
		uv.x = (pos.x - TerrainMeta.Position.x) * TerrainMeta.OneOverSize.x;
		uv.y = (pos.z - TerrainMeta.Position.z) * TerrainMeta.OneOverSize.z;
		float num = WaterSystem.OceanLevel;
		float num2 = (TerrainMeta.WaterMap != null) ? TerrainMeta.WaterMap.GetHeightFast(uv) : 0f;
		float num3 = (TerrainMeta.HeightMap != null) ? TerrainMeta.HeightMap.GetHeightFast(uv) : 0f;
		if (WaterSystem.instance != null && (double)num2 <= (double)num + 0.01)
		{
			Vector3 shore = (TerrainTexturing.Instance != null) ? TerrainTexturing.Instance.GetCoarseVectorToShore(uv) : Vector3.zero;
			float num4 = Mathf.Clamp01(Mathf.Abs(num - num3) * 0.1f);
			num2 = WaterGerstner.SampleHeight(WaterSystem.instance, pos, shore) * num4;
		}
		return num2;
	}

	// Token: 0x060032D4 RID: 13012 RVA: 0x00139134 File Offset: 0x00137334
	public static void GetHeightArray_Managed(Vector2[] pos, Vector2[] posUV, Vector3[] shore, float[] terrainHeight, float[] waterHeight)
	{
		if (TerrainTexturing.Instance != null)
		{
			for (int i = 0; i < posUV.Length; i++)
			{
				shore[i] = TerrainTexturing.Instance.GetCoarseVectorToShore(posUV[i]);
			}
		}
		if (WaterSystem.instance != null)
		{
			WaterGerstner.SampleHeightArray(WaterSystem.instance, pos, shore, waterHeight);
		}
		float num = WaterSystem.OceanLevel;
		for (int j = 0; j < posUV.Length; j++)
		{
			Vector2 uv = posUV[j];
			terrainHeight[j] = ((TerrainMeta.HeightMap != null) ? TerrainMeta.HeightMap.GetHeightFast(uv) : 0f);
			float num2 = (TerrainMeta.WaterMap != null) ? TerrainMeta.WaterMap.GetHeightFast(uv) : 0f;
			if (WaterSystem.instance != null && (double)num2 <= (double)num + 0.01)
			{
				float num3 = Mathf.Clamp01(Mathf.Abs(num - terrainHeight[j]) * 0.1f);
				waterHeight[j] = num + waterHeight[j] * num3;
			}
			else
			{
				waterHeight[j] = num2;
			}
		}
	}

	// Token: 0x060032D5 RID: 13013 RVA: 0x00139240 File Offset: 0x00137440
	public static float GetHeight(Vector3 pos)
	{
		float val;
		if (WaterSystem.nativePathState == WaterSystem.NativePathState.Ready)
		{
			val = WaterSystem.GetHeight_Native(pos);
		}
		else
		{
			val = WaterSystem.GetHeight_Managed(pos);
		}
		return Math.Max(val, WaterSystem.OceanLevel);
	}

	// Token: 0x060032D6 RID: 13014 RVA: 0x00139270 File Offset: 0x00137470
	public static void GetHeightArray(Vector2[] pos, Vector2[] posUV, Vector3[] shore, float[] terrainHeight, float[] waterHeight)
	{
		Debug.Assert(pos.Length == posUV.Length);
		Debug.Assert(pos.Length == shore.Length);
		Debug.Assert(pos.Length == terrainHeight.Length);
		Debug.Assert(pos.Length == waterHeight.Length);
		if (WaterSystem.nativePathState == WaterSystem.NativePathState.Ready)
		{
			WaterSystem.GetHeightArray_Native(pos.Length, ref pos[0], ref posUV[0], ref shore[0], ref terrainHeight[0], ref waterHeight[0]);
			return;
		}
		WaterSystem.GetHeightArray_Managed(pos, posUV, shore, terrainHeight, waterHeight);
	}

	// Token: 0x060032D7 RID: 13015 RVA: 0x001392F4 File Offset: 0x001374F4
	public static Vector3 GetNormal(Vector3 pos)
	{
		return ((TerrainMeta.WaterMap != null) ? TerrainMeta.WaterMap.GetNormal(pos) : Vector3.up).normalized;
	}

	// Token: 0x060032D8 RID: 13016 RVA: 0x00139328 File Offset: 0x00137528
	public static void RegisterBody(WaterBody body)
	{
		if (body.Type == WaterBodyType.Ocean)
		{
			if (WaterSystem.Ocean == null)
			{
				WaterSystem.Ocean = body;
				body.Transform.position = body.Transform.position.WithY(WaterSystem.OceanLevel);
			}
			else if (WaterSystem.Ocean != body)
			{
				Debug.LogWarning("[Water] Ocean body is already registered. Ignoring call because only one is allowed.");
				return;
			}
		}
		WaterSystem.WaterBodies.Add(body);
	}

	// Token: 0x060032D9 RID: 13017 RVA: 0x00139397 File Offset: 0x00137597
	public static void UnregisterBody(WaterBody body)
	{
		WaterSystem.WaterBodies.Remove(body);
	}

	// Token: 0x060032DA RID: 13018 RVA: 0x001393A8 File Offset: 0x001375A8
	private void UpdateWaves()
	{
		WaterSystem.WaveTime = (this.ProgressTime ? Time.realtimeSinceStartup : WaterSystem.WaveTime);
		using (TimeWarning.New("WaterGerstner.UpdatePrecomputedWaves", 0))
		{
			WaterGerstner.UpdatePrecomputedWaves(this.Simulation.OpenSeaWaves, ref this.precomputedWaves);
		}
		using (TimeWarning.New("WaterGerstner.UpdatePrecomputedShoreWaves", 0))
		{
			WaterGerstner.UpdatePrecomputedShoreWaves(this.Simulation.ShoreWaves, ref this.precomputedShoreWaves);
		}
	}

	// Token: 0x060032DB RID: 13019 RVA: 0x00139448 File Offset: 0x00137648
	private static void UpdateOceanLevel()
	{
		if (WaterSystem.Ocean != null)
		{
			WaterSystem.Ocean.Transform.position = WaterSystem.Ocean.Transform.position.WithY(WaterSystem.OceanLevel);
		}
		if (WaterSystem.nativePathState == WaterSystem.NativePathState.Ready)
		{
			WaterSystem.UpdateOceanLevel_Native(WaterSystem.OceanLevel);
		}
		foreach (WaterBody waterBody in WaterSystem.WaterBodies)
		{
			waterBody.OnOceanLevelChanged(WaterSystem.OceanLevel);
		}
	}

	// Token: 0x060032DC RID: 13020 RVA: 0x001394E4 File Offset: 0x001376E4
	public void UpdateWaveData()
	{
		this.ShoreWavesRcpFadeDistance = 1f / this.Simulation.ShoreWavesFadeDistance;
		this.TerrainRcpFadeDistance = 1f / this.Simulation.TerrainFadeDistance;
		this.global0.x = this.ShoreWavesRcpFadeDistance;
		this.global0.y = this.TerrainRcpFadeDistance;
		this.global0.z = this.precomputedShoreWaves.DirectionVarFreq;
		this.global0.w = this.precomputedShoreWaves.DirectionVarAmp;
		this.global1.x = this.precomputedShoreWaves.Steepness;
		this.global1.y = this.precomputedShoreWaves.Amplitude;
		this.global1.z = this.precomputedShoreWaves.K;
		this.global1.w = this.precomputedShoreWaves.C;
		using (TimeWarning.New("WaterGerstner.UpdateWaveArray", 0))
		{
			WaterGerstner.UpdateWaveArray(this.precomputedWaves, ref this.waveArray);
		}
		using (TimeWarning.New("WaterGerstner.UpdateShoreWaveArray", 0))
		{
			WaterGerstner.UpdateShoreWaveArray(this.precomputedShoreWaves, ref this.shoreWaveArray);
		}
		using (TimeWarning.New("WaterSystem.SetNativeConstants", 0))
		{
			WaterSystem.SetNativeConstants(TerrainTexturing.Instance, TerrainMeta.WaterMap, TerrainMeta.HeightMap, this.global0, this.global1, this.waveArray, this.shoreWaveArray);
		}
	}

	// Token: 0x060032DD RID: 13021 RVA: 0x00139684 File Offset: 0x00137884
	private void Update()
	{
		using (TimeWarning.New("UpdateWaves", 0))
		{
			this.UpdateWaves();
		}
		using (TimeWarning.New("UpdateWaveData", 0))
		{
			this.UpdateWaveData();
		}
	}

	// Token: 0x02000E35 RID: 3637
	[Serializable]
	public class SimulationSettings
	{
		// Token: 0x04004A9B RID: 19099
		public Vector3 Wind = new Vector3(3f, 0f, 3f);

		// Token: 0x04004A9C RID: 19100
		public int SolverResolution = 64;

		// Token: 0x04004A9D RID: 19101
		public float SolverSizeInWorld = 18f;

		// Token: 0x04004A9E RID: 19102
		public float Gravity = 9.81f;

		// Token: 0x04004A9F RID: 19103
		public float Amplitude = 0.0001f;

		// Token: 0x04004AA0 RID: 19104
		public Texture2D PerlinNoise;

		// Token: 0x04004AA1 RID: 19105
		public WaterGerstner.WaveParams[] OpenSeaWaves = new WaterGerstner.WaveParams[6];

		// Token: 0x04004AA2 RID: 19106
		public WaterGerstner.ShoreWaveParams ShoreWaves = new WaterGerstner.ShoreWaveParams();

		// Token: 0x04004AA3 RID: 19107
		[Range(0.1f, 250f)]
		public float ShoreWavesFadeDistance = 25f;

		// Token: 0x04004AA4 RID: 19108
		[Range(0.1f, 250f)]
		public float TerrainFadeDistance = 10f;

		// Token: 0x04004AA5 RID: 19109
		[Range(0.001f, 1f)]
		public float OpenSeaCrestFoamThreshold = 0.08f;

		// Token: 0x04004AA6 RID: 19110
		[Range(0.001f, 1f)]
		public float ShoreCrestFoamThreshold = 0.08f;

		// Token: 0x04004AA7 RID: 19111
		[Range(0.001f, 1f)]
		public float ShoreCrestFoamFarThreshold = 0.08f;

		// Token: 0x04004AA8 RID: 19112
		[Range(0.1f, 250f)]
		public float ShoreCrestFoamFadeDistance = 10f;
	}

	// Token: 0x02000E36 RID: 3638
	[Serializable]
	public class RenderingSettings
	{
		// Token: 0x04004AA9 RID: 19113
		public float MaxDisplacementDistance = 50f;

		// Token: 0x04004AAA RID: 19114
		public WaterSystem.RenderingSettings.SkyProbe SkyReflections;

		// Token: 0x04004AAB RID: 19115
		public WaterSystem.RenderingSettings.SSR ScreenSpaceReflections;

		// Token: 0x04004AAC RID: 19116
		public WaterSystem.RenderingSettings.Caustics CausticsAnimation;

		// Token: 0x02000FD1 RID: 4049
		[Serializable]
		public class SkyProbe
		{
			// Token: 0x040050F3 RID: 20723
			public float ProbeUpdateInterval = 1f;

			// Token: 0x040050F4 RID: 20724
			public bool TimeSlicing = true;
		}

		// Token: 0x02000FD2 RID: 4050
		[Serializable]
		public class SSR
		{
			// Token: 0x040050F5 RID: 20725
			public float FresnelCutoff = 0.02f;

			// Token: 0x040050F6 RID: 20726
			public float ThicknessMin = 1f;

			// Token: 0x040050F7 RID: 20727
			public float ThicknessMax = 20f;

			// Token: 0x040050F8 RID: 20728
			public float ThicknessStartDist = 40f;

			// Token: 0x040050F9 RID: 20729
			public float ThicknessEndDist = 100f;
		}

		// Token: 0x02000FD3 RID: 4051
		[Serializable]
		public class Caustics
		{
			// Token: 0x040050FA RID: 20730
			public float FrameRate = 15f;

			// Token: 0x040050FB RID: 20731
			public Texture2D[] FramesShallow = new Texture2D[0];

			// Token: 0x040050FC RID: 20732
			public Texture2D[] FramesDeep = new Texture2D[0];
		}
	}

	// Token: 0x02000E37 RID: 3639
	private enum NativePathState
	{
		// Token: 0x04004AAE RID: 19118
		Initializing,
		// Token: 0x04004AAF RID: 19119
		Failed,
		// Token: 0x04004AB0 RID: 19120
		Ready
	}
}
