﻿using System;
using System.IO;

// Token: 0x02000944 RID: 2372
public static class MurmurHash
{
	// Token: 0x0400335F RID: 13151
	private const uint seed = 1337U;

	// Token: 0x060038EB RID: 14571 RVA: 0x00152C94 File Offset: 0x00150E94
	public static int Signed(Stream stream)
	{
		return (int)MurmurHash.Unsigned(stream);
	}

	// Token: 0x060038EC RID: 14572 RVA: 0x00152C9C File Offset: 0x00150E9C
	public static uint Unsigned(Stream stream)
	{
		uint num = 1337U;
		uint num2 = 0U;
		using (BinaryReader binaryReader = new BinaryReader(stream))
		{
			byte[] array = binaryReader.ReadBytes(4);
			while (array.Length != 0)
			{
				num2 += (uint)array.Length;
				switch (array.Length)
				{
				case 1:
				{
					uint num3 = (uint)array[0];
					num3 *= 3432918353U;
					num3 = MurmurHash.rot(num3, 15);
					num3 *= 461845907U;
					num ^= num3;
					break;
				}
				case 2:
				{
					uint num3 = (uint)((int)array[0] | (int)array[1] << 8);
					num3 *= 3432918353U;
					num3 = MurmurHash.rot(num3, 15);
					num3 *= 461845907U;
					num ^= num3;
					break;
				}
				case 3:
				{
					uint num3 = (uint)((int)array[0] | (int)array[1] << 8 | (int)array[2] << 16);
					num3 *= 3432918353U;
					num3 = MurmurHash.rot(num3, 15);
					num3 *= 461845907U;
					num ^= num3;
					break;
				}
				case 4:
				{
					uint num3 = (uint)((int)array[0] | (int)array[1] << 8 | (int)array[2] << 16 | (int)array[3] << 24);
					num3 *= 3432918353U;
					num3 = MurmurHash.rot(num3, 15);
					num3 *= 461845907U;
					num ^= num3;
					num = MurmurHash.rot(num, 13);
					num = num * 5U + 3864292196U;
					break;
				}
				}
				array = binaryReader.ReadBytes(4);
			}
		}
		num ^= num2;
		num = MurmurHash.mix(num);
		return num;
	}

	// Token: 0x060038ED RID: 14573 RVA: 0x00152E0C File Offset: 0x0015100C
	private static uint rot(uint x, byte r)
	{
		return x << (int)r | x >> (int)(32 - r);
	}

	// Token: 0x060038EE RID: 14574 RVA: 0x00152E1E File Offset: 0x0015101E
	private static uint mix(uint h)
	{
		h ^= h >> 16;
		h *= 2246822507U;
		h ^= h >> 13;
		h *= 3266489909U;
		h ^= h >> 16;
		return h;
	}
}
