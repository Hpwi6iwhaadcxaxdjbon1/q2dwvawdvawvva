using System;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace CompanionServer.Cameras
{
	// Token: 0x02000A09 RID: 2569
	public struct RaycastRaySetupJob : IJobParallelFor
	{
		// Token: 0x04003708 RID: 14088
		public float2 res;

		// Token: 0x04003709 RID: 14089
		public float2 halfRes;

		// Token: 0x0400370A RID: 14090
		public float aspectRatio;

		// Token: 0x0400370B RID: 14091
		public float worldHeight;

		// Token: 0x0400370C RID: 14092
		public float3 cameraPos;

		// Token: 0x0400370D RID: 14093
		public quaternion cameraRot;

		// Token: 0x0400370E RID: 14094
		public float nearPlane;

		// Token: 0x0400370F RID: 14095
		public float farPlane;

		// Token: 0x04003710 RID: 14096
		public int layerMask;

		// Token: 0x04003711 RID: 14097
		public int sampleOffset;

		// Token: 0x04003712 RID: 14098
		[Unity.Collections.ReadOnly]
		public NativeArray<int2> samplePositions;

		// Token: 0x04003713 RID: 14099
		[WriteOnly]
		[NativeMatchesParallelForLength]
		public NativeArray<RaycastCommand> raycastCommands;

		// Token: 0x06003D42 RID: 15682 RVA: 0x00167E10 File Offset: 0x00166010
		public void Execute(int index)
		{
			int i;
			for (i = this.sampleOffset + index; i >= this.samplePositions.Length; i -= this.samplePositions.Length)
			{
			}
			float2 @float = (this.samplePositions[i] - this.halfRes) / this.res;
			float3 v = new float3(@float.x * this.worldHeight * this.aspectRatio, @float.y * this.worldHeight, 1f);
			float3 float2 = math.mul(this.cameraRot, v);
			float3 v2 = this.cameraPos + float2 * this.nearPlane;
			this.raycastCommands[index] = new RaycastCommand(v2, float2, this.farPlane, this.layerMask, 1);
		}
	}
}
