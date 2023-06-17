using System;
using Rust;

namespace UnityEngine
{
	// Token: 0x02000A1E RID: 2590
	public static class ColliderEx
	{
		// Token: 0x06003D8A RID: 15754 RVA: 0x001697D3 File Offset: 0x001679D3
		public static PhysicMaterial GetMaterialAt(this Collider obj, Vector3 pos)
		{
			if (obj is TerrainCollider)
			{
				return TerrainMeta.Physics.GetMaterial(pos);
			}
			return obj.sharedMaterial;
		}

		// Token: 0x06003D8B RID: 15755 RVA: 0x001697EF File Offset: 0x001679EF
		public static bool IsOnLayer(this Collider col, Layer rustLayer)
		{
			return col != null && col.gameObject.IsOnLayer(rustLayer);
		}

		// Token: 0x06003D8C RID: 15756 RVA: 0x00169808 File Offset: 0x00167A08
		public static bool IsOnLayer(this Collider col, int layer)
		{
			return col != null && col.gameObject.IsOnLayer(layer);
		}

		// Token: 0x06003D8D RID: 15757 RVA: 0x00169824 File Offset: 0x00167A24
		public static float GetRadius(this Collider col, Vector3 transformScale)
		{
			float result = 1f;
			SphereCollider sphereCollider;
			BoxCollider boxCollider;
			CapsuleCollider capsuleCollider;
			MeshCollider meshCollider;
			if ((sphereCollider = (col as SphereCollider)) != null)
			{
				result = sphereCollider.radius * transformScale.Max();
			}
			else if ((boxCollider = (col as BoxCollider)) != null)
			{
				result = Vector3.Scale(boxCollider.size, transformScale).Max() * 0.5f;
			}
			else if ((capsuleCollider = (col as CapsuleCollider)) != null)
			{
				int direction = capsuleCollider.direction;
				float num;
				if (direction != 0)
				{
					if (direction != 1)
					{
						num = transformScale.x;
					}
					else
					{
						num = transformScale.x;
					}
				}
				else
				{
					num = transformScale.y;
				}
				result = capsuleCollider.radius * num;
			}
			else if ((meshCollider = (col as MeshCollider)) != null)
			{
				result = Vector3.Scale(meshCollider.bounds.size, transformScale).Max() * 0.5f;
			}
			return result;
		}
	}
}
