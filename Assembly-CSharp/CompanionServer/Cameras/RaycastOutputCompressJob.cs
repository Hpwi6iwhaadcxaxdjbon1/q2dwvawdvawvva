using System;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Jobs;

namespace CompanionServer.Cameras
{
	// Token: 0x02000A0B RID: 2571
	public struct RaycastOutputCompressJob : IJob
	{
		// Token: 0x0400371D RID: 14109
		[Unity.Collections.ReadOnly]
		public NativeArray<int> rayOutputs;

		// Token: 0x0400371E RID: 14110
		[WriteOnly]
		public NativeArray<int> dataLength;

		// Token: 0x0400371F RID: 14111
		[WriteOnly]
		public NativeArray<byte> data;

		// Token: 0x06003D46 RID: 15686 RVA: 0x00168084 File Offset: 0x00166284
		public void Execute()
		{
			int num = this.rayOutputs.Length * 4;
			if (this.data.Length < num)
			{
				throw new InvalidOperationException("Not enough data buffer available to compress rays");
			}
			NativeArray<int> nativeArray = new NativeArray<int>(64, Allocator.Temp, NativeArrayOptions.ClearMemory);
			int value = 0;
			for (int i = 0; i < this.rayOutputs.Length; i++)
			{
				int num2 = this.rayOutputs[i];
				ushort num3 = RaycastOutputCompressJob.RayDistance(num2);
				byte b = RaycastOutputCompressJob.RayAlignment(num2);
				byte b2 = RaycastOutputCompressJob.RayMaterial(num2);
				int num4 = (int)(num3 / 128 * 3 + (ushort)(b / 16 * 5) + (ushort)(b2 * 7) & 63);
				int num5 = nativeArray[num4];
				if (num5 == num2)
				{
					this.data[value++] = (byte)(0 | num4);
				}
				else
				{
					int num6 = (int)(num3 - RaycastOutputCompressJob.RayDistance(num5));
					int num7 = (int)(b - RaycastOutputCompressJob.RayAlignment(num5));
					if (b2 == RaycastOutputCompressJob.RayMaterial(num5) && num6 >= -15 && num6 <= 16 && num7 >= -3 && num7 <= 4)
					{
						this.data[value++] = (byte)(64 | num4);
						this.data[value++] = (byte)(num6 + 15 << 3 | num7 + 3);
					}
					else if (b2 == RaycastOutputCompressJob.RayMaterial(num5) && num7 == 0 && num6 >= -127 && num6 <= 128)
					{
						this.data[value++] = (byte)(128 | num4);
						this.data[value++] = (byte)(num6 + 127);
					}
					else if (b2 < 63)
					{
						nativeArray[num4] = num2;
						this.data[value++] = (192 | b2);
						this.data[value++] = (byte)(num3 >> 2);
						this.data[value++] = (byte)((int)(num3 & 3) << 6 | (int)b);
					}
					else
					{
						nativeArray[num4] = num2;
						this.data[value++] = byte.MaxValue;
						this.data[value++] = (byte)(num3 >> 2);
						this.data[value++] = (byte)((int)(num3 & 3) << 6 | (int)b);
						this.data[value++] = b2;
					}
				}
			}
			nativeArray.Dispose();
			this.dataLength[0] = value;
		}

		// Token: 0x06003D47 RID: 15687 RVA: 0x001682E5 File Offset: 0x001664E5
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static ushort RayDistance(int ray)
		{
			return (ushort)(ray >> 16);
		}

		// Token: 0x06003D48 RID: 15688 RVA: 0x001682EC File Offset: 0x001664EC
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static byte RayAlignment(int ray)
		{
			return (byte)(ray >> 8);
		}

		// Token: 0x06003D49 RID: 15689 RVA: 0x001682F2 File Offset: 0x001664F2
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static byte RayMaterial(int ray)
		{
			return (byte)ray;
		}
	}
}
