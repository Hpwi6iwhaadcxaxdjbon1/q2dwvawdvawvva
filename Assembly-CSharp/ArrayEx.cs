using System;
using UnityEngine;

// Token: 0x0200091B RID: 2331
public static class ArrayEx
{
	// Token: 0x06003832 RID: 14386 RVA: 0x0014F26B File Offset: 0x0014D46B
	public static T[] New<T>(int length)
	{
		if (length == 0)
		{
			return Array.Empty<T>();
		}
		return new T[length];
	}

	// Token: 0x06003833 RID: 14387 RVA: 0x0014F27C File Offset: 0x0014D47C
	public static T GetRandom<T>(this T[] array)
	{
		if (array == null || array.Length == 0)
		{
			return default(T);
		}
		return array[UnityEngine.Random.Range(0, array.Length)];
	}

	// Token: 0x06003834 RID: 14388 RVA: 0x0014F2AC File Offset: 0x0014D4AC
	public static T GetRandom<T>(this T[] array, uint seed)
	{
		if (array == null || array.Length == 0)
		{
			return default(T);
		}
		return array[SeedRandom.Range(ref seed, 0, array.Length)];
	}

	// Token: 0x06003835 RID: 14389 RVA: 0x0014F2DC File Offset: 0x0014D4DC
	public static T GetRandom<T>(this T[] array, ref uint seed)
	{
		if (array == null || array.Length == 0)
		{
			return default(T);
		}
		return array[SeedRandom.Range(ref seed, 0, array.Length)];
	}

	// Token: 0x06003836 RID: 14390 RVA: 0x0014F30A File Offset: 0x0014D50A
	public static void Shuffle<T>(this T[] array, uint seed)
	{
		array.Shuffle(ref seed);
	}

	// Token: 0x06003837 RID: 14391 RVA: 0x0014F314 File Offset: 0x0014D514
	public static void Shuffle<T>(this T[] array, ref uint seed)
	{
		for (int i = 0; i < array.Length; i++)
		{
			int num = SeedRandom.Range(ref seed, 0, array.Length);
			int num2 = SeedRandom.Range(ref seed, 0, array.Length);
			T t = array[num];
			array[num] = array[num2];
			array[num2] = t;
		}
	}

	// Token: 0x06003838 RID: 14392 RVA: 0x0014F364 File Offset: 0x0014D564
	public static void BubbleSort<T>(this T[] array) where T : IComparable<T>
	{
		for (int i = 1; i < array.Length; i++)
		{
			T t = array[i];
			for (int j = i - 1; j >= 0; j--)
			{
				T t2 = array[j];
				if (t.CompareTo(t2) >= 0)
				{
					break;
				}
				array[j + 1] = t2;
				array[j] = t;
			}
		}
	}
}
