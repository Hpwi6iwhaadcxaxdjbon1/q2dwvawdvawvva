using System;
using System.Collections.Generic;
using UnityEngine.Assertions;

namespace UnityEngine.Rendering.PostProcessing
{
	// Token: 0x02000A99 RID: 2713
	internal static class MeshUtilities
	{
		// Token: 0x040039A6 RID: 14758
		private static Dictionary<PrimitiveType, Mesh> s_Primitives = new Dictionary<PrimitiveType, Mesh>();

		// Token: 0x040039A7 RID: 14759
		private static Dictionary<Type, PrimitiveType> s_ColliderPrimitives = new Dictionary<Type, PrimitiveType>
		{
			{
				typeof(BoxCollider),
				PrimitiveType.Cube
			},
			{
				typeof(SphereCollider),
				PrimitiveType.Sphere
			},
			{
				typeof(CapsuleCollider),
				PrimitiveType.Capsule
			}
		};

		// Token: 0x060040A6 RID: 16550 RVA: 0x0017D47C File Offset: 0x0017B67C
		internal static Mesh GetColliderMesh(Collider collider)
		{
			Type type = collider.GetType();
			if (type == typeof(MeshCollider))
			{
				return ((MeshCollider)collider).sharedMesh;
			}
			Assert.IsTrue(MeshUtilities.s_ColliderPrimitives.ContainsKey(type), "Unknown collider");
			return MeshUtilities.GetPrimitive(MeshUtilities.s_ColliderPrimitives[type]);
		}

		// Token: 0x060040A7 RID: 16551 RVA: 0x0017D4D4 File Offset: 0x0017B6D4
		internal static Mesh GetPrimitive(PrimitiveType primitiveType)
		{
			Mesh builtinMesh;
			if (!MeshUtilities.s_Primitives.TryGetValue(primitiveType, out builtinMesh))
			{
				builtinMesh = MeshUtilities.GetBuiltinMesh(primitiveType);
				MeshUtilities.s_Primitives.Add(primitiveType, builtinMesh);
			}
			return builtinMesh;
		}

		// Token: 0x060040A8 RID: 16552 RVA: 0x0017D504 File Offset: 0x0017B704
		private static Mesh GetBuiltinMesh(PrimitiveType primitiveType)
		{
			GameObject gameObject = GameObject.CreatePrimitive(primitiveType);
			Mesh sharedMesh = gameObject.GetComponent<MeshFilter>().sharedMesh;
			RuntimeUtilities.Destroy(gameObject);
			return sharedMesh;
		}
	}
}
