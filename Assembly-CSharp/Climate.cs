using System;
using System.Collections.Generic;
using System.Linq;
using Rust;
using UnityEngine;

// Token: 0x020004EB RID: 1259
public class Climate : SingletonComponent<Climate>
{
	// Token: 0x040020BA RID: 8378
	private const float fadeAngle = 20f;

	// Token: 0x040020BB RID: 8379
	private const float defaultTemp = 15f;

	// Token: 0x040020BC RID: 8380
	private const int weatherDurationHours = 18;

	// Token: 0x040020BD RID: 8381
	private const int weatherFadeHours = 6;

	// Token: 0x040020BE RID: 8382
	[Range(0f, 1f)]
	public float BlendingSpeed = 1f;

	// Token: 0x040020BF RID: 8383
	[Range(1f, 9f)]
	public float FogMultiplier = 5f;

	// Token: 0x040020C0 RID: 8384
	public float FogDarknessDistance = 200f;

	// Token: 0x040020C1 RID: 8385
	public bool DebugLUTBlending;

	// Token: 0x040020C2 RID: 8386
	public Climate.WeatherParameters Weather;

	// Token: 0x040020C3 RID: 8387
	public WeatherPreset[] WeatherPresets;

	// Token: 0x040020C4 RID: 8388
	public Climate.ClimateParameters Arid;

	// Token: 0x040020C5 RID: 8389
	public Climate.ClimateParameters Temperate;

	// Token: 0x040020C6 RID: 8390
	public Climate.ClimateParameters Tundra;

	// Token: 0x040020C7 RID: 8391
	public Climate.ClimateParameters Arctic;

	// Token: 0x040020D4 RID: 8404
	private Dictionary<WeatherPresetType, WeatherPreset[]> presetLookup;

	// Token: 0x040020D5 RID: 8405
	private Climate.ClimateParameters[] climateLookup;

	// Token: 0x17000371 RID: 881
	// (get) Token: 0x060028AC RID: 10412 RVA: 0x000FAED5 File Offset: 0x000F90D5
	// (set) Token: 0x060028AD RID: 10413 RVA: 0x000FAEDD File Offset: 0x000F90DD
	public float WeatherStateBlend { get; private set; }

	// Token: 0x17000372 RID: 882
	// (get) Token: 0x060028AE RID: 10414 RVA: 0x000FAEE6 File Offset: 0x000F90E6
	// (set) Token: 0x060028AF RID: 10415 RVA: 0x000FAEEE File Offset: 0x000F90EE
	public uint WeatherSeedPrevious { get; private set; }

	// Token: 0x17000373 RID: 883
	// (get) Token: 0x060028B0 RID: 10416 RVA: 0x000FAEF7 File Offset: 0x000F90F7
	// (set) Token: 0x060028B1 RID: 10417 RVA: 0x000FAEFF File Offset: 0x000F90FF
	public uint WeatherSeedTarget { get; private set; }

	// Token: 0x17000374 RID: 884
	// (get) Token: 0x060028B2 RID: 10418 RVA: 0x000FAF08 File Offset: 0x000F9108
	// (set) Token: 0x060028B3 RID: 10419 RVA: 0x000FAF10 File Offset: 0x000F9110
	public uint WeatherSeedNext { get; private set; }

	// Token: 0x17000375 RID: 885
	// (get) Token: 0x060028B4 RID: 10420 RVA: 0x000FAF19 File Offset: 0x000F9119
	// (set) Token: 0x060028B5 RID: 10421 RVA: 0x000FAF21 File Offset: 0x000F9121
	public WeatherPreset WeatherStatePrevious { get; private set; }

	// Token: 0x17000376 RID: 886
	// (get) Token: 0x060028B6 RID: 10422 RVA: 0x000FAF2A File Offset: 0x000F912A
	// (set) Token: 0x060028B7 RID: 10423 RVA: 0x000FAF32 File Offset: 0x000F9132
	public WeatherPreset WeatherStateTarget { get; private set; }

	// Token: 0x17000377 RID: 887
	// (get) Token: 0x060028B8 RID: 10424 RVA: 0x000FAF3B File Offset: 0x000F913B
	// (set) Token: 0x060028B9 RID: 10425 RVA: 0x000FAF43 File Offset: 0x000F9143
	public WeatherPreset WeatherStateNext { get; private set; }

	// Token: 0x17000378 RID: 888
	// (get) Token: 0x060028BA RID: 10426 RVA: 0x000FAF4C File Offset: 0x000F914C
	// (set) Token: 0x060028BB RID: 10427 RVA: 0x000FAF54 File Offset: 0x000F9154
	public WeatherPreset WeatherState { get; private set; }

	// Token: 0x17000379 RID: 889
	// (get) Token: 0x060028BC RID: 10428 RVA: 0x000FAF5D File Offset: 0x000F915D
	// (set) Token: 0x060028BD RID: 10429 RVA: 0x000FAF65 File Offset: 0x000F9165
	public WeatherPreset WeatherClampsMin { get; private set; }

	// Token: 0x1700037A RID: 890
	// (get) Token: 0x060028BE RID: 10430 RVA: 0x000FAF6E File Offset: 0x000F916E
	// (set) Token: 0x060028BF RID: 10431 RVA: 0x000FAF76 File Offset: 0x000F9176
	public WeatherPreset WeatherClampsMax { get; private set; }

	// Token: 0x1700037B RID: 891
	// (get) Token: 0x060028C0 RID: 10432 RVA: 0x000FAF7F File Offset: 0x000F917F
	// (set) Token: 0x060028C1 RID: 10433 RVA: 0x000FAF87 File Offset: 0x000F9187
	public WeatherPreset WeatherOverrides { get; private set; }

	// Token: 0x1700037C RID: 892
	// (get) Token: 0x060028C2 RID: 10434 RVA: 0x000FAF90 File Offset: 0x000F9190
	// (set) Token: 0x060028C3 RID: 10435 RVA: 0x000FAF98 File Offset: 0x000F9198
	public LegacyWeatherState Overrides { get; private set; }

	// Token: 0x060028C4 RID: 10436 RVA: 0x000FAFA4 File Offset: 0x000F91A4
	protected override void Awake()
	{
		base.Awake();
		this.WeatherState = (ScriptableObject.CreateInstance(typeof(WeatherPreset)) as WeatherPreset);
		this.WeatherClampsMin = (ScriptableObject.CreateInstance(typeof(WeatherPreset)) as WeatherPreset);
		this.WeatherClampsMax = (ScriptableObject.CreateInstance(typeof(WeatherPreset)) as WeatherPreset);
		this.WeatherOverrides = (ScriptableObject.CreateInstance(typeof(WeatherPreset)) as WeatherPreset);
		this.WeatherState.Reset();
		this.WeatherClampsMin.Reset();
		this.WeatherClampsMax.Reset();
		this.WeatherOverrides.Reset();
		this.Overrides = new LegacyWeatherState(this.WeatherOverrides);
	}

	// Token: 0x060028C5 RID: 10437 RVA: 0x000FB05C File Offset: 0x000F925C
	protected override void OnDestroy()
	{
		if (Rust.Application.isQuitting)
		{
			return;
		}
		base.OnDestroy();
		if (this.WeatherState != null)
		{
			UnityEngine.Object.Destroy(this.WeatherState);
		}
		if (this.WeatherClampsMin != null)
		{
			UnityEngine.Object.Destroy(this.WeatherClampsMin);
		}
		if (this.WeatherClampsMax != null)
		{
			UnityEngine.Object.Destroy(this.WeatherClampsMax);
		}
		if (this.WeatherOverrides != null)
		{
			UnityEngine.Object.Destroy(this.WeatherOverrides);
		}
	}

	// Token: 0x060028C6 RID: 10438 RVA: 0x000FB0DC File Offset: 0x000F92DC
	protected void Update()
	{
		if (Rust.Application.isReceiving)
		{
			return;
		}
		if (Rust.Application.isLoading)
		{
			return;
		}
		if (!TOD_Sky.Instance)
		{
			return;
		}
		TOD_Sky instance = TOD_Sky.Instance;
		long num = (long)((ulong)World.Seed + (ulong)instance.Cycle.Ticks);
		long num2 = 648000000000L;
		long num3 = 216000000000L;
		long num4 = num / num2;
		this.WeatherStateBlend = Mathf.InverseLerp(0f, (float)num3, (float)(num % num2));
		this.WeatherStatePrevious = this.GetWeatherPreset(this.WeatherSeedPrevious = this.GetSeedFromLong(num4));
		this.WeatherStateTarget = this.GetWeatherPreset(this.WeatherSeedTarget = this.GetSeedFromLong(num4 + 1L));
		this.WeatherStateNext = this.GetWeatherPreset(this.WeatherSeedNext = this.GetSeedFromLong(num4 + 2L));
		this.WeatherState.Fade(this.WeatherStatePrevious, this.WeatherStateTarget, this.WeatherStateBlend);
		this.WeatherState.Override(this.WeatherOverrides);
	}

	// Token: 0x060028C7 RID: 10439 RVA: 0x000FB1E0 File Offset: 0x000F93E0
	private static bool Initialized()
	{
		return SingletonComponent<Climate>.Instance && SingletonComponent<Climate>.Instance.WeatherStatePrevious && SingletonComponent<Climate>.Instance.WeatherStateTarget && SingletonComponent<Climate>.Instance.WeatherStateNext && SingletonComponent<Climate>.Instance.WeatherState && SingletonComponent<Climate>.Instance.WeatherClampsMin && SingletonComponent<Climate>.Instance.WeatherOverrides;
	}

	// Token: 0x060028C8 RID: 10440 RVA: 0x000FB26E File Offset: 0x000F946E
	public static float GetClouds(Vector3 position)
	{
		if (!Climate.Initialized())
		{
			return 0f;
		}
		return SingletonComponent<Climate>.Instance.WeatherState.Clouds.Coverage;
	}

	// Token: 0x060028C9 RID: 10441 RVA: 0x000FB291 File Offset: 0x000F9491
	public static float GetFog(Vector3 position)
	{
		if (!Climate.Initialized())
		{
			return 0f;
		}
		return SingletonComponent<Climate>.Instance.WeatherState.Atmosphere.Fogginess;
	}

	// Token: 0x060028CA RID: 10442 RVA: 0x000FB2B4 File Offset: 0x000F94B4
	public static float GetWind(Vector3 position)
	{
		if (!Climate.Initialized())
		{
			return 0f;
		}
		return SingletonComponent<Climate>.Instance.WeatherState.Wind;
	}

	// Token: 0x060028CB RID: 10443 RVA: 0x000FB2D4 File Offset: 0x000F94D4
	public static float GetThunder(Vector3 position)
	{
		if (!Climate.Initialized())
		{
			return 0f;
		}
		float thunder = SingletonComponent<Climate>.Instance.WeatherOverrides.Thunder;
		if (thunder >= 0f)
		{
			return thunder;
		}
		float thunder2 = SingletonComponent<Climate>.Instance.WeatherState.Thunder;
		float thunder3 = SingletonComponent<Climate>.Instance.WeatherStatePrevious.Thunder;
		float thunder4 = SingletonComponent<Climate>.Instance.WeatherStateTarget.Thunder;
		if (thunder3 > 0f && thunder2 > 0.5f * thunder3)
		{
			return thunder2;
		}
		if (thunder4 > 0f && thunder2 > 0.5f * thunder4)
		{
			return thunder2;
		}
		return 0f;
	}

	// Token: 0x060028CC RID: 10444 RVA: 0x000FB368 File Offset: 0x000F9568
	public static float GetRainbow(Vector3 position)
	{
		if (!Climate.Initialized())
		{
			return 0f;
		}
		TOD_Sky instance = TOD_Sky.Instance;
		if (!instance || !instance.IsDay || instance.LerpValue < 1f)
		{
			return 0f;
		}
		if (Climate.GetFog(position) > 0.25f)
		{
			return 0f;
		}
		float num = TerrainMeta.BiomeMap ? TerrainMeta.BiomeMap.GetBiome(position, 3) : 0f;
		if (num <= 0f)
		{
			return 0f;
		}
		float rainbow = SingletonComponent<Climate>.Instance.WeatherOverrides.Rainbow;
		if (rainbow >= 0f)
		{
			return rainbow * num;
		}
		if (SingletonComponent<Climate>.Instance.WeatherState.Rainbow <= 0f)
		{
			return 0f;
		}
		if (SingletonComponent<Climate>.Instance.WeatherStateTarget.Rainbow > 0f)
		{
			return 0f;
		}
		float rainbow2 = SingletonComponent<Climate>.Instance.WeatherStatePrevious.Rainbow;
		float num2 = SeedRandom.Value(SingletonComponent<Climate>.Instance.WeatherSeedPrevious);
		if (rainbow2 < num2)
		{
			return 0f;
		}
		return num;
	}

	// Token: 0x060028CD RID: 10445 RVA: 0x000FB46C File Offset: 0x000F966C
	public static float GetAurora(Vector3 position)
	{
		if (!Climate.Initialized())
		{
			return 0f;
		}
		TOD_Sky instance = TOD_Sky.Instance;
		if (!instance || !instance.IsNight || instance.LerpValue > 0f)
		{
			return 0f;
		}
		if (Climate.GetClouds(position) > 0.1f)
		{
			return 0f;
		}
		if (Climate.GetFog(position) > 0.1f)
		{
			return 0f;
		}
		if (!TerrainMeta.BiomeMap)
		{
			return 0f;
		}
		return TerrainMeta.BiomeMap.GetBiome(position, 8);
	}

	// Token: 0x060028CE RID: 10446 RVA: 0x000FB4F4 File Offset: 0x000F96F4
	public static float GetRain(Vector3 position)
	{
		if (!Climate.Initialized())
		{
			return 0f;
		}
		float t = TerrainMeta.BiomeMap ? TerrainMeta.BiomeMap.GetBiome(position, 1) : 0f;
		float num = TerrainMeta.BiomeMap ? TerrainMeta.BiomeMap.GetBiome(position, 8) : 0f;
		return SingletonComponent<Climate>.Instance.WeatherState.Rain * Mathf.Lerp(1f, 0.5f, t) * (1f - num);
	}

	// Token: 0x060028CF RID: 10447 RVA: 0x000FB578 File Offset: 0x000F9778
	public static float GetSnow(Vector3 position)
	{
		if (!Climate.Initialized())
		{
			return 0f;
		}
		float num = TerrainMeta.BiomeMap ? TerrainMeta.BiomeMap.GetBiome(position, 8) : 0f;
		return SingletonComponent<Climate>.Instance.WeatherState.Rain * num;
	}

	// Token: 0x060028D0 RID: 10448 RVA: 0x000FB5C4 File Offset: 0x000F97C4
	public static float GetTemperature(Vector3 position)
	{
		if (!Climate.Initialized())
		{
			return 15f;
		}
		TOD_Sky instance = TOD_Sky.Instance;
		if (!instance)
		{
			return 15f;
		}
		Climate.ClimateParameters climateParameters;
		Climate.ClimateParameters climateParameters2;
		float t = SingletonComponent<Climate>.Instance.FindBlendParameters(position, out climateParameters, out climateParameters2);
		if (climateParameters == null || climateParameters2 == null)
		{
			return 15f;
		}
		float hour = instance.Cycle.Hour;
		float a = climateParameters.Temperature.Evaluate(hour);
		float b = climateParameters2.Temperature.Evaluate(hour);
		return Mathf.Lerp(a, b, t);
	}

	// Token: 0x060028D1 RID: 10449 RVA: 0x000FB640 File Offset: 0x000F9840
	private uint GetSeedFromLong(long val)
	{
		uint result = (uint)((val % (long)((ulong)-1) + (long)((ulong)-1)) % (long)((ulong)-1));
		SeedRandom.Wanghash(ref result);
		SeedRandom.Wanghash(ref result);
		SeedRandom.Wanghash(ref result);
		return result;
	}

	// Token: 0x060028D2 RID: 10450 RVA: 0x000FB674 File Offset: 0x000F9874
	private WeatherPreset GetWeatherPreset(uint seed)
	{
		float max = this.Weather.ClearChance + this.Weather.DustChance + this.Weather.FogChance + this.Weather.OvercastChance + this.Weather.StormChance + this.Weather.RainChance;
		float num = SeedRandom.Range(ref seed, 0f, max);
		if (num < this.Weather.RainChance)
		{
			return this.GetWeatherPreset(seed, WeatherPresetType.Rain);
		}
		if (num < this.Weather.RainChance + this.Weather.StormChance)
		{
			return this.GetWeatherPreset(seed, WeatherPresetType.Storm);
		}
		if (num < this.Weather.RainChance + this.Weather.StormChance + this.Weather.OvercastChance)
		{
			return this.GetWeatherPreset(seed, WeatherPresetType.Overcast);
		}
		if (num < this.Weather.RainChance + this.Weather.StormChance + this.Weather.OvercastChance + this.Weather.FogChance)
		{
			return this.GetWeatherPreset(seed, WeatherPresetType.Fog);
		}
		if (num < this.Weather.RainChance + this.Weather.StormChance + this.Weather.OvercastChance + this.Weather.FogChance + this.Weather.DustChance)
		{
			return this.GetWeatherPreset(seed, WeatherPresetType.Dust);
		}
		return this.GetWeatherPreset(seed, WeatherPresetType.Clear);
	}

	// Token: 0x060028D3 RID: 10451 RVA: 0x000FB7CC File Offset: 0x000F99CC
	private WeatherPreset GetWeatherPreset(uint seed, WeatherPresetType type)
	{
		if (this.presetLookup == null)
		{
			this.presetLookup = new Dictionary<WeatherPresetType, WeatherPreset[]>();
		}
		WeatherPreset[] array;
		if (!this.presetLookup.TryGetValue(type, out array))
		{
			this.presetLookup.Add(type, array = this.CacheWeatherPresets(type));
		}
		return array.GetRandom(ref seed);
	}

	// Token: 0x060028D4 RID: 10452 RVA: 0x000FB81C File Offset: 0x000F9A1C
	private WeatherPreset[] CacheWeatherPresets(WeatherPresetType type)
	{
		return (from x in this.WeatherPresets
		where x.Type == type
		select x).ToArray<WeatherPreset>();
	}

	// Token: 0x060028D5 RID: 10453 RVA: 0x000FB854 File Offset: 0x000F9A54
	private float FindBlendParameters(Vector3 pos, out Climate.ClimateParameters src, out Climate.ClimateParameters dst)
	{
		if (this.climateLookup == null)
		{
			this.climateLookup = new Climate.ClimateParameters[]
			{
				this.Arid,
				this.Temperate,
				this.Tundra,
				this.Arctic
			};
		}
		if (TerrainMeta.BiomeMap == null)
		{
			src = this.Temperate;
			dst = this.Temperate;
			return 0.5f;
		}
		int biomeMaxType = TerrainMeta.BiomeMap.GetBiomeMaxType(pos, -1);
		int biomeMaxType2 = TerrainMeta.BiomeMap.GetBiomeMaxType(pos, ~biomeMaxType);
		src = this.climateLookup[TerrainBiome.TypeToIndex(biomeMaxType)];
		dst = this.climateLookup[TerrainBiome.TypeToIndex(biomeMaxType2)];
		return TerrainMeta.BiomeMap.GetBiome(pos, biomeMaxType2);
	}

	// Token: 0x02000D25 RID: 3365
	[Serializable]
	public class ClimateParameters
	{
		// Token: 0x0400465A RID: 18010
		public AnimationCurve Temperature;

		// Token: 0x0400465B RID: 18011
		[Horizontal(4, -1)]
		public Climate.Float4 AerialDensity;

		// Token: 0x0400465C RID: 18012
		[Horizontal(4, -1)]
		public Climate.Float4 FogDensity;

		// Token: 0x0400465D RID: 18013
		[Horizontal(4, -1)]
		public Climate.Texture2D4 LUT;
	}

	// Token: 0x02000D26 RID: 3366
	[Serializable]
	public class WeatherParameters
	{
		// Token: 0x0400465E RID: 18014
		[Range(0f, 1f)]
		public float ClearChance = 1f;

		// Token: 0x0400465F RID: 18015
		[Range(0f, 1f)]
		public float DustChance;

		// Token: 0x04004660 RID: 18016
		[Range(0f, 1f)]
		public float FogChance;

		// Token: 0x04004661 RID: 18017
		[Range(0f, 1f)]
		public float OvercastChance;

		// Token: 0x04004662 RID: 18018
		[Range(0f, 1f)]
		public float StormChance;

		// Token: 0x04004663 RID: 18019
		[Range(0f, 1f)]
		public float RainChance;
	}

	// Token: 0x02000D27 RID: 3367
	public class Value4<T>
	{
		// Token: 0x04004664 RID: 18020
		public T Dawn;

		// Token: 0x04004665 RID: 18021
		public T Noon;

		// Token: 0x04004666 RID: 18022
		public T Dusk;

		// Token: 0x04004667 RID: 18023
		public T Night;

		// Token: 0x0600505E RID: 20574 RVA: 0x001A9214 File Offset: 0x001A7414
		public float FindBlendParameters(TOD_Sky sky, out T src, out T dst)
		{
			float num = Mathf.Abs(sky.SunriseTime - sky.Cycle.Hour);
			float num2 = Mathf.Abs(sky.SunsetTime - sky.Cycle.Hour);
			float num3 = (180f - sky.SunZenith) / 180f;
			float num4 = 0.11111111f;
			if (num < num2)
			{
				if (num3 < 0.5f)
				{
					src = this.Night;
					dst = this.Dawn;
					return Mathf.InverseLerp(0.5f - num4, 0.5f, num3);
				}
				src = this.Dawn;
				dst = this.Noon;
				return Mathf.InverseLerp(0.5f, 0.5f + num4, num3);
			}
			else
			{
				if (num3 > 0.5f)
				{
					src = this.Noon;
					dst = this.Dusk;
					return Mathf.InverseLerp(0.5f + num4, 0.5f, num3);
				}
				src = this.Dusk;
				dst = this.Night;
				return Mathf.InverseLerp(0.5f, 0.5f - num4, num3);
			}
		}
	}

	// Token: 0x02000D28 RID: 3368
	[Serializable]
	public class Float4 : Climate.Value4<float>
	{
	}

	// Token: 0x02000D29 RID: 3369
	[Serializable]
	public class Color4 : Climate.Value4<Color>
	{
	}

	// Token: 0x02000D2A RID: 3370
	[Serializable]
	public class Texture2D4 : Climate.Value4<Texture2D>
	{
	}
}
