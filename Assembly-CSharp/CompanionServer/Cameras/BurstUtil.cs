using System;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

namespace CompanionServer.Cameras
{
	// Token: 0x02000A0D RID: 2573
	internal static class BurstUtil
	{
		// Token: 0x06003D4D RID: 15693 RVA: 0x0016853C File Offset: 0x0016673C
		public unsafe static ref readonly T GetReadonly<[IsUnmanaged] T>(this NativeArray<T> array, int index) where T : struct, ValueType
		{
			T* unsafeReadOnlyPtr = (T*)array.GetUnsafeReadOnlyPtr<T>();
			return unsafeReadOnlyPtr + (IntPtr)index * (IntPtr)sizeof(T) / (IntPtr)sizeof(T);
		}

		// Token: 0x06003D4E RID: 15694 RVA: 0x00168560 File Offset: 0x00166760
		public unsafe static ref T Get<[IsUnmanaged] T>(this NativeArray<T> array, int index) where T : struct, ValueType
		{
			T* unsafePtr = (T*)array.GetUnsafePtr<T>();
			return ref unsafePtr[(IntPtr)index * (IntPtr)sizeof(T) / (IntPtr)sizeof(T)];
		}

		// Token: 0x06003D4F RID: 15695 RVA: 0x00168584 File Offset: 0x00166784
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static int GetColliderId(this RaycastHit hit)
		{
			return ((BurstUtil.RaycastHitPublic*)(&hit))->m_Collider;
		}

		// Token: 0x06003D50 RID: 15696 RVA: 0x00168590 File Offset: 0x00166790
		public unsafe static Collider GetCollider(int colliderInstanceId)
		{
			BurstUtil.RaycastHitPublic raycastHitPublic = new BurstUtil.RaycastHitPublic
			{
				m_Collider = colliderInstanceId
			};
			return ((RaycastHit*)(&raycastHitPublic))->collider;
		}

		// Token: 0x02000EF7 RID: 3831
		private struct RaycastHitPublic
		{
			// Token: 0x04004DDB RID: 19931
			public Vector3 m_Point;

			// Token: 0x04004DDC RID: 19932
			public Vector3 m_Normal;

			// Token: 0x04004DDD RID: 19933
			public uint m_FaceID;

			// Token: 0x04004DDE RID: 19934
			public float m_Distance;

			// Token: 0x04004DDF RID: 19935
			public Vector2 m_UV;

			// Token: 0x04004DE0 RID: 19936
			public int m_Collider;
		}
	}
}
