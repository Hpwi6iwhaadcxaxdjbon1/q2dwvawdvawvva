using System;
using Unity.Collections;
using Unity.Jobs;

namespace CompanionServer.Cameras
{
	// Token: 0x02000A08 RID: 2568
	public struct RaycastBufferSetupJob : IJob
	{
		// Token: 0x04003705 RID: 14085
		public NativeArray<int> colliderIds;

		// Token: 0x04003706 RID: 14086
		public NativeArray<byte> colliderMaterials;

		// Token: 0x04003707 RID: 14087
		[WriteOnly]
		public NativeArray<int> colliderHits;

		// Token: 0x06003D40 RID: 15680 RVA: 0x00167CDC File Offset: 0x00165EDC
		public void Execute()
		{
			if (this.colliderIds.Length > 1)
			{
				RaycastBufferSetupJob.SortByAscending(ref this.colliderIds, ref this.colliderMaterials, 0, this.colliderIds.Length - 1);
			}
			for (int i = 0; i < this.colliderHits.Length; i++)
			{
				this.colliderHits[i] = 0;
			}
		}

		// Token: 0x06003D41 RID: 15681 RVA: 0x00167D3C File Offset: 0x00165F3C
		private static void SortByAscending(ref NativeArray<int> colliderIds, ref NativeArray<byte> colliderMaterials, int leftIndex, int rightIndex)
		{
			int i = leftIndex;
			int num = rightIndex;
			int num2 = colliderIds[leftIndex];
			while (i <= num)
			{
				while (colliderIds[i] < num2)
				{
					i++;
				}
				while (colliderIds[num] > num2)
				{
					num--;
				}
				if (i <= num)
				{
					int index = i;
					ref NativeArray<int> ptr = ref colliderIds;
					int index2 = num;
					int num3 = colliderIds[num];
					int num4 = colliderIds[i];
					colliderIds[index] = num3;
					ptr[index2] = num4;
					num4 = i;
					ref NativeArray<byte> ptr2 = ref colliderMaterials;
					num3 = num;
					byte value = colliderMaterials[num];
					byte value2 = colliderMaterials[i];
					colliderMaterials[num4] = value;
					ptr2[num3] = value2;
					i++;
					num--;
				}
			}
			if (leftIndex < num)
			{
				RaycastBufferSetupJob.SortByAscending(ref colliderIds, ref colliderMaterials, leftIndex, num);
			}
			if (i < rightIndex)
			{
				RaycastBufferSetupJob.SortByAscending(ref colliderIds, ref colliderMaterials, i, rightIndex);
			}
		}
	}
}
