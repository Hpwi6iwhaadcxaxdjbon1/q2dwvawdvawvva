using System;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace CompanionServer.Cameras
{
	// Token: 0x02000A07 RID: 2567
	public struct RaycastSamplePositionsJob : IJob
	{
		// Token: 0x04003702 RID: 14082
		public int2 res;

		// Token: 0x04003703 RID: 14083
		public Unity.Mathematics.Random random;

		// Token: 0x04003704 RID: 14084
		public NativeArray<int2> positions;

		// Token: 0x06003D3F RID: 15679 RVA: 0x00167C0C File Offset: 0x00165E0C
		public void Execute()
		{
			int i = 0;
			for (int j = 0; j < this.res.y; j++)
			{
				for (int k = 0; k < this.res.x; k++)
				{
					this.positions[i++] = new int2(k, j);
				}
			}
			for (i = this.res.x * this.res.y - 1; i >= 1; i--)
			{
				int num = this.random.NextInt(i + 1);
				int index = i;
				ref NativeArray<int2> ptr = ref this.positions;
				int index2 = num;
				int2 value = this.positions[num];
				int2 value2 = this.positions[i];
				this.positions[index] = value;
				ptr[index2] = value2;
			}
		}
	}
}
