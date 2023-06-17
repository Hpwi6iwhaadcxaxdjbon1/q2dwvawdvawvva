using System;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace CompanionServer.Cameras
{
	// Token: 0x02000A0C RID: 2572
	public struct RaycastColliderProcessingJob : IJob
	{
		// Token: 0x04003720 RID: 14112
		public NativeArray<int> foundCollidersLength;

		// Token: 0x04003721 RID: 14113
		public NativeArray<int> foundColliders;

		// Token: 0x06003D4A RID: 15690 RVA: 0x001682F8 File Offset: 0x001664F8
		public void Execute()
		{
			int num = math.min(this.foundCollidersLength[0], this.foundColliders.Length);
			if (num <= 1)
			{
				return;
			}
			RaycastColliderProcessingJob.SortAscending(ref this.foundColliders, 0, num - 1);
			NativeArray<int> nativeArray = new NativeArray<int>(num, Allocator.Temp, NativeArrayOptions.UninitializedMemory);
			int num2 = 0;
			int i = 0;
			while (i < num)
			{
				int num3 = this.foundColliders[i];
				int num4 = 1;
				while (i < num && this.foundColliders[i] == num3)
				{
					num4++;
					i++;
				}
				this.foundColliders[num2] = num3;
				nativeArray[num2] = num4;
				num2++;
			}
			RaycastColliderProcessingJob.SortByDescending(ref this.foundColliders, ref nativeArray, 0, num2 - 1);
			nativeArray.Dispose();
			int value = math.min(num2, 512);
			this.foundCollidersLength[0] = value;
		}

		// Token: 0x06003D4B RID: 15691 RVA: 0x001683D0 File Offset: 0x001665D0
		private static void SortByDescending(ref NativeArray<int> colliders, ref NativeArray<int> counts, int leftIndex, int rightIndex)
		{
			int i = leftIndex;
			int num = rightIndex;
			int num2 = counts[leftIndex];
			while (i <= num)
			{
				while (counts[i] > num2)
				{
					i++;
				}
				while (counts[num] < num2)
				{
					num--;
				}
				if (i <= num)
				{
					int num3 = i;
					ref NativeArray<int> ptr = ref colliders;
					int num4 = num;
					int num5 = colliders[num];
					int num6 = colliders[i];
					colliders[num3] = num5;
					ptr[num4] = num6;
					num6 = i;
					ptr = ref counts;
					num5 = num;
					num4 = counts[num];
					num3 = counts[i];
					counts[num6] = num4;
					ptr[num5] = num3;
					i++;
					num--;
				}
			}
			if (leftIndex < num)
			{
				RaycastColliderProcessingJob.SortByDescending(ref colliders, ref counts, leftIndex, num);
			}
			if (i < rightIndex)
			{
				RaycastColliderProcessingJob.SortByDescending(ref colliders, ref counts, i, rightIndex);
			}
		}

		// Token: 0x06003D4C RID: 15692 RVA: 0x001684A4 File Offset: 0x001666A4
		private static void SortAscending(ref NativeArray<int> array, int leftIndex, int rightIndex)
		{
			int i = leftIndex;
			int num = rightIndex;
			int num2 = array[leftIndex];
			while (i <= num)
			{
				while (array[i] < num2)
				{
					i++;
				}
				while (array[num] > num2)
				{
					num--;
				}
				if (i <= num)
				{
					int index = i;
					ref NativeArray<int> ptr = ref array;
					int index2 = num;
					int value = array[num];
					int value2 = array[i];
					array[index] = value;
					ptr[index2] = value2;
					i++;
					num--;
				}
			}
			if (leftIndex < num)
			{
				RaycastColliderProcessingJob.SortAscending(ref array, leftIndex, num);
			}
			if (i < rightIndex)
			{
				RaycastColliderProcessingJob.SortAscending(ref array, i, rightIndex);
			}
		}
	}
}
