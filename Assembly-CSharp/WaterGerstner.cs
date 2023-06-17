using System;
using UnityEngine;

// Token: 0x020006FE RID: 1790
public class WaterGerstner
{
	// Token: 0x0400290A RID: 10506
	public const int WaveCount = 6;

	// Token: 0x0600327C RID: 12924 RVA: 0x00136C68 File Offset: 0x00134E68
	public static void UpdatePrecomputedWaves(WaterGerstner.WaveParams[] waves, ref WaterGerstner.PrecomputedWave[] precomputed)
	{
		if (precomputed == null || precomputed.Length != 6)
		{
			precomputed = new WaterGerstner.PrecomputedWave[6];
		}
		Debug.Assert(precomputed.Length == waves.Length);
		for (int i = 0; i < 6; i++)
		{
			float num = waves[i].Angle * 0.017453292f;
			precomputed[i].Angle = num;
			precomputed[i].Direction = new Vector2(Mathf.Cos(num), Mathf.Sin(num));
			precomputed[i].Steepness = waves[i].Steepness;
			precomputed[i].K = 6.2831855f / waves[i].Length;
			precomputed[i].C = Mathf.Sqrt(9.8f / precomputed[i].K) * waves[i].Speed * WaterSystem.WaveTime;
			precomputed[i].A = waves[i].Steepness / precomputed[i].K;
		}
	}

	// Token: 0x0600327D RID: 12925 RVA: 0x00136D6C File Offset: 0x00134F6C
	public static void UpdatePrecomputedShoreWaves(WaterGerstner.ShoreWaveParams shoreWaves, ref WaterGerstner.PrecomputedShoreWaves precomputed)
	{
		if (precomputed.Directions == null || precomputed.Directions.Length != 6)
		{
			precomputed.Directions = new Vector2[6];
		}
		Debug.Assert(precomputed.Directions.Length == shoreWaves.DirectionAngles.Length);
		for (int i = 0; i < 6; i++)
		{
			float f = shoreWaves.DirectionAngles[i] * 0.017453292f;
			precomputed.Directions[i] = new Vector2(Mathf.Cos(f), Mathf.Sin(f));
		}
		precomputed.Steepness = shoreWaves.Steepness;
		precomputed.Amplitude = shoreWaves.Amplitude;
		precomputed.K = 6.2831855f / shoreWaves.Length;
		precomputed.C = Mathf.Sqrt(9.8f / precomputed.K) * shoreWaves.Speed * WaterSystem.WaveTime;
		precomputed.A = shoreWaves.Steepness / precomputed.K;
		precomputed.DirectionVarFreq = shoreWaves.DirectionVarFreq;
		precomputed.DirectionVarAmp = shoreWaves.DirectionVarAmp;
	}

	// Token: 0x0600327E RID: 12926 RVA: 0x00136E60 File Offset: 0x00135060
	public static void UpdateWaveArray(WaterGerstner.PrecomputedWave[] precomputed, ref Vector4[] array)
	{
		if (array == null || array.Length != 6)
		{
			array = new Vector4[6];
		}
		Debug.Assert(array.Length == precomputed.Length);
		for (int i = 0; i < 6; i++)
		{
			array[i] = new Vector4(precomputed[i].Angle, precomputed[i].Steepness, precomputed[i].K, precomputed[i].C);
		}
	}

	// Token: 0x0600327F RID: 12927 RVA: 0x00136ED8 File Offset: 0x001350D8
	public static void UpdateShoreWaveArray(WaterGerstner.PrecomputedShoreWaves precomputed, ref Vector4[] array)
	{
		Debug.Assert(precomputed.Directions.Length == 6);
		if (array == null || array.Length != 3)
		{
			array = new Vector4[3];
		}
		Debug.Assert(array.Length == 3);
		Vector2[] directions = precomputed.Directions;
		array[0] = new Vector4(directions[0].x, directions[0].y, directions[1].x, directions[1].y);
		array[1] = new Vector4(directions[2].x, directions[2].y, directions[3].x, directions[3].y);
		array[2] = new Vector4(directions[4].x, directions[4].y, directions[5].x, directions[5].y);
	}

	// Token: 0x06003280 RID: 12928 RVA: 0x00136FD4 File Offset: 0x001351D4
	private static void GerstnerWave(WaterGerstner.PrecomputedWave wave, Vector2 pos, Vector2 shoreVec, ref float outH)
	{
		Vector2 direction = wave.Direction;
		float num = Mathf.Sin(wave.K * (direction.x * pos.x + direction.y * pos.y - wave.C));
		outH += wave.A * num;
	}

	// Token: 0x06003281 RID: 12929 RVA: 0x00137024 File Offset: 0x00135224
	private static void GerstnerWave(WaterGerstner.PrecomputedWave wave, Vector2 pos, Vector2 shoreVec, ref Vector3 outP)
	{
		Vector2 direction = wave.Direction;
		float f = wave.K * (direction.x * pos.x + direction.y * pos.y - wave.C);
		float num = Mathf.Cos(f);
		float num2 = Mathf.Sin(f);
		outP.x += direction.x * wave.A * num;
		outP.y += wave.A * num2;
		outP.z += direction.y * wave.A * num;
	}

	// Token: 0x06003282 RID: 12930 RVA: 0x001370B4 File Offset: 0x001352B4
	private static void GerstnerShoreWave(WaterGerstner.PrecomputedShoreWaves wave, Vector2 waveDir, Vector2 pos, Vector2 shoreVec, float variation_t, ref float outH)
	{
		float num = Mathf.Clamp01(waveDir.x * shoreVec.x + waveDir.y * shoreVec.y);
		num *= num;
		float f = wave.K * (waveDir.x * pos.x + waveDir.y * pos.y - wave.C + variation_t);
		Mathf.Cos(f);
		float num2 = Mathf.Sin(f);
		outH += wave.A * wave.Amplitude * num2 * num;
	}

	// Token: 0x06003283 RID: 12931 RVA: 0x00137138 File Offset: 0x00135338
	private static void GerstnerShoreWave(WaterGerstner.PrecomputedShoreWaves wave, Vector2 waveDir, Vector2 pos, Vector2 shoreVec, float variation_t, ref Vector3 outP)
	{
		float num = Mathf.Clamp01(waveDir.x * shoreVec.x + waveDir.y * shoreVec.y);
		num *= num;
		float f = wave.K * (waveDir.x * pos.x + waveDir.y * pos.y - wave.C + variation_t);
		float num2 = Mathf.Cos(f);
		float num3 = Mathf.Sin(f);
		outP.x += waveDir.x * wave.A * num2 * num;
		outP.y += wave.A * wave.Amplitude * num3 * num;
		outP.z += waveDir.y * wave.A * num2 * num;
	}

	// Token: 0x06003284 RID: 12932 RVA: 0x001371F8 File Offset: 0x001353F8
	public static Vector3 SampleDisplacement(WaterSystem instance, Vector3 location, Vector3 shore)
	{
		WaterGerstner.PrecomputedWave[] precomputedWaves = instance.PrecomputedWaves;
		WaterGerstner.PrecomputedShoreWaves precomputedShoreWaves = instance.PrecomputedShoreWaves;
		Vector2 vector = new Vector2(location.x, location.z);
		Vector2 shoreVec = new Vector2(shore.x, shore.y);
		float t = 1f - Mathf.Clamp01(shore.z * instance.ShoreWavesRcpFadeDistance);
		float d = Mathf.Clamp01(shore.z * instance.TerrainRcpFadeDistance);
		float num = Mathf.Cos(vector.x * precomputedShoreWaves.DirectionVarFreq) * precomputedShoreWaves.DirectionVarAmp;
		float num2 = Mathf.Cos(vector.y * precomputedShoreWaves.DirectionVarFreq) * precomputedShoreWaves.DirectionVarAmp;
		float variation_t = num + num2;
		Vector3 zero = Vector3.zero;
		Vector3 zero2 = Vector3.zero;
		for (int i = 0; i < 6; i++)
		{
			WaterGerstner.GerstnerWave(precomputedWaves[i], vector, shoreVec, ref zero);
			WaterGerstner.GerstnerShoreWave(precomputedShoreWaves, precomputedShoreWaves.Directions[i], vector, shoreVec, variation_t, ref zero2);
		}
		return Vector3.Lerp(zero, zero2, t) * d;
	}

	// Token: 0x06003285 RID: 12933 RVA: 0x001372FC File Offset: 0x001354FC
	private static float SampleHeightREF(WaterSystem instance, Vector3 location, Vector3 shore)
	{
		WaterGerstner.PrecomputedWave[] precomputedWaves = instance.PrecomputedWaves;
		WaterGerstner.PrecomputedShoreWaves precomputedShoreWaves = instance.PrecomputedShoreWaves;
		Vector2 vector = new Vector2(location.x, location.z);
		Vector2 shoreVec = new Vector2(shore.x, shore.y);
		float t = 1f - Mathf.Clamp01(shore.z * instance.ShoreWavesRcpFadeDistance);
		float num = Mathf.Clamp01(shore.z * instance.TerrainRcpFadeDistance);
		float num2 = Mathf.Cos(vector.x * precomputedShoreWaves.DirectionVarFreq) * precomputedShoreWaves.DirectionVarAmp;
		float num3 = Mathf.Cos(vector.y * precomputedShoreWaves.DirectionVarFreq) * precomputedShoreWaves.DirectionVarAmp;
		float variation_t = num2 + num3;
		float a = 0f;
		float b = 0f;
		for (int i = 0; i < 6; i++)
		{
			WaterGerstner.GerstnerWave(precomputedWaves[i], vector, shoreVec, ref a);
			WaterGerstner.GerstnerShoreWave(precomputedShoreWaves, precomputedShoreWaves.Directions[i], vector, shoreVec, variation_t, ref b);
		}
		return Mathf.Lerp(a, b, t) * num;
	}

	// Token: 0x06003286 RID: 12934 RVA: 0x001373FC File Offset: 0x001355FC
	private static void SampleHeightArrayREF(WaterSystem instance, Vector2[] location, Vector3[] shore, float[] height)
	{
		Debug.Assert(location.Length == height.Length);
		for (int i = 0; i < location.Length; i++)
		{
			Vector3 location2 = new Vector3(location[i].x, 0f, location[i].y);
			height[i] = WaterGerstner.SampleHeight(instance, location2, shore[i]);
		}
	}

	// Token: 0x06003287 RID: 12935 RVA: 0x0013745C File Offset: 0x0013565C
	public static float SampleHeight(WaterSystem instance, Vector3 location, Vector3 shore)
	{
		WaterGerstner.PrecomputedWave[] precomputedWaves = instance.PrecomputedWaves;
		Vector2[] directions = instance.PrecomputedShoreWaves.Directions;
		Vector4 global = instance.Global0;
		Vector4 global2 = instance.Global1;
		float x = global2.x;
		float y = global2.y;
		float z = global2.z;
		float w = global2.w;
		float num = x / z;
		Vector2 vector = new Vector2(location.x, location.z);
		Vector2 vector2 = new Vector2(shore.x, shore.y);
		float t = 1f - Mathf.Clamp01(shore.z * global.x);
		float num2 = Mathf.Clamp01(shore.z * global.y);
		float num3 = Mathf.Cos(vector.x * global.z) * global.w;
		float num4 = Mathf.Cos(vector.y * global.z) * global.w;
		float num5 = num3 + num4;
		float num6 = 0f;
		float num7 = 0f;
		for (int i = 0; i < 6; i++)
		{
			Vector2 direction = precomputedWaves[i].Direction;
			float c = precomputedWaves[i].C;
			float k = precomputedWaves[i].K;
			float a = precomputedWaves[i].A;
			float num8 = Mathf.Sin(k * (direction.x * vector.x + direction.y * vector.y - c));
			num6 += a * num8;
			Vector2 vector3 = directions[i];
			float num9 = Mathf.Clamp01(vector3.x * vector2.x + vector3.y * vector2.y);
			num9 *= num9;
			float num10 = Mathf.Sin(z * (vector3.x * vector.x + vector3.y * vector.y - w + num5));
			num7 += num * y * num10 * num9;
		}
		return Mathf.Lerp(num6, num7, t) * num2;
	}

	// Token: 0x06003288 RID: 12936 RVA: 0x00137658 File Offset: 0x00135858
	public static void SampleHeightArray(WaterSystem instance, Vector2[] location, Vector3[] shore, float[] height)
	{
		Debug.Assert(location.Length == height.Length);
		WaterGerstner.PrecomputedWave[] precomputedWaves = instance.PrecomputedWaves;
		Vector2[] directions = instance.PrecomputedShoreWaves.Directions;
		Vector4 global = instance.Global0;
		Vector4 global2 = instance.Global1;
		float x = global2.x;
		float y = global2.y;
		float z = global2.z;
		float w = global2.w;
		float num = x / z;
		for (int i = 0; i < location.Length; i++)
		{
			Vector2 vector = new Vector2(location[i].x, location[i].y);
			Vector2 vector2 = new Vector2(shore[i].x, shore[i].y);
			float t = 1f - Mathf.Clamp01(shore[i].z * global.x);
			float num2 = Mathf.Clamp01(shore[i].z * global.y);
			float num3 = Mathf.Cos(vector.x * global.z) * global.w;
			float num4 = Mathf.Cos(vector.y * global.z) * global.w;
			float num5 = num3 + num4;
			float num6 = 0f;
			float num7 = 0f;
			for (int j = 0; j < 6; j++)
			{
				Vector2 direction = precomputedWaves[j].Direction;
				float c = precomputedWaves[j].C;
				float k = precomputedWaves[j].K;
				float a = precomputedWaves[j].A;
				float num8 = Mathf.Sin(k * (direction.x * vector.x + direction.y * vector.y - c));
				num6 += a * num8;
				Vector2 vector3 = directions[j];
				float num9 = Mathf.Clamp01(vector3.x * vector2.x + vector3.y * vector2.y);
				num9 *= num9;
				float num10 = Mathf.Sin(z * (vector3.x * vector.x + vector3.y * vector.y - w + num5));
				num7 += num * y * num10 * num9;
			}
			height[i] = Mathf.Lerp(num6, num7, t) * num2;
		}
	}

	// Token: 0x02000E31 RID: 3633
	[Serializable]
	public class WaveParams
	{
		// Token: 0x04004A80 RID: 19072
		[Range(0f, 360f)]
		public float Angle;

		// Token: 0x04004A81 RID: 19073
		[Range(0f, 0.99f)]
		public float Steepness = 0.4f;

		// Token: 0x04004A82 RID: 19074
		[Range(0.01f, 1000f)]
		public float Length = 15f;

		// Token: 0x04004A83 RID: 19075
		[Range(-10f, 10f)]
		public float Speed = 0.4f;
	}

	// Token: 0x02000E32 RID: 3634
	[Serializable]
	public class ShoreWaveParams
	{
		// Token: 0x04004A84 RID: 19076
		[Range(0f, 2f)]
		public float Steepness = 0.99f;

		// Token: 0x04004A85 RID: 19077
		[Range(0f, 1f)]
		public float Amplitude = 0.2f;

		// Token: 0x04004A86 RID: 19078
		[Range(0.01f, 1000f)]
		public float Length = 20f;

		// Token: 0x04004A87 RID: 19079
		[Range(-10f, 10f)]
		public float Speed = 0.6f;

		// Token: 0x04004A88 RID: 19080
		public float[] DirectionAngles = new float[]
		{
			0f,
			57.3f,
			114.5f,
			171.9f,
			229.2f,
			286.5f
		};

		// Token: 0x04004A89 RID: 19081
		public float DirectionVarFreq = 0.1f;

		// Token: 0x04004A8A RID: 19082
		public float DirectionVarAmp = 2.5f;
	}

	// Token: 0x02000E33 RID: 3635
	public struct PrecomputedWave
	{
		// Token: 0x04004A8B RID: 19083
		public float Angle;

		// Token: 0x04004A8C RID: 19084
		public Vector2 Direction;

		// Token: 0x04004A8D RID: 19085
		public float Steepness;

		// Token: 0x04004A8E RID: 19086
		public float K;

		// Token: 0x04004A8F RID: 19087
		public float C;

		// Token: 0x04004A90 RID: 19088
		public float A;

		// Token: 0x04004A91 RID: 19089
		public static WaterGerstner.PrecomputedWave Default = new WaterGerstner.PrecomputedWave
		{
			Angle = 0f,
			Direction = Vector2.right,
			Steepness = 0.4f,
			K = 1f,
			C = 1f,
			A = 1f
		};
	}

	// Token: 0x02000E34 RID: 3636
	public struct PrecomputedShoreWaves
	{
		// Token: 0x04004A92 RID: 19090
		public Vector2[] Directions;

		// Token: 0x04004A93 RID: 19091
		public float Steepness;

		// Token: 0x04004A94 RID: 19092
		public float Amplitude;

		// Token: 0x04004A95 RID: 19093
		public float K;

		// Token: 0x04004A96 RID: 19094
		public float C;

		// Token: 0x04004A97 RID: 19095
		public float A;

		// Token: 0x04004A98 RID: 19096
		public float DirectionVarFreq;

		// Token: 0x04004A99 RID: 19097
		public float DirectionVarAmp;

		// Token: 0x04004A9A RID: 19098
		public static WaterGerstner.PrecomputedShoreWaves Default = new WaterGerstner.PrecomputedShoreWaves
		{
			Directions = new Vector2[]
			{
				Vector2.right,
				Vector2.right,
				Vector2.right,
				Vector2.right,
				Vector2.right,
				Vector2.right
			},
			Steepness = 0.75f,
			Amplitude = 0.2f,
			K = 1f,
			C = 1f,
			A = 1f,
			DirectionVarFreq = 0.1f,
			DirectionVarAmp = 3f
		};
	}
}
