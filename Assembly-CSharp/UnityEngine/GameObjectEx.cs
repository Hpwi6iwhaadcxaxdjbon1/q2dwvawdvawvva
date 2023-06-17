using System;
using System.Collections.Generic;
using Facepunch;
using Rust;
using Rust.Registry;

namespace UnityEngine
{
	// Token: 0x02000A22 RID: 2594
	public static class GameObjectEx
	{
		// Token: 0x06003D95 RID: 15765 RVA: 0x00169991 File Offset: 0x00167B91
		public static BaseEntity ToBaseEntity(this GameObject go)
		{
			return go.transform.ToBaseEntity();
		}

		// Token: 0x06003D96 RID: 15766 RVA: 0x0016999E File Offset: 0x00167B9E
		public static BaseEntity ToBaseEntity(this Collider collider)
		{
			return collider.transform.ToBaseEntity();
		}

		// Token: 0x06003D97 RID: 15767 RVA: 0x001699AC File Offset: 0x00167BAC
		public static BaseEntity ToBaseEntity(this Transform transform)
		{
			IEntity entity = GameObjectEx.GetEntityFromRegistry(transform);
			if (entity == null && !transform.gameObject.activeInHierarchy)
			{
				entity = GameObjectEx.GetEntityFromComponent(transform);
			}
			return entity as BaseEntity;
		}

		// Token: 0x06003D98 RID: 15768 RVA: 0x001699DD File Offset: 0x00167BDD
		public static bool IsOnLayer(this GameObject go, Layer rustLayer)
		{
			return go.IsOnLayer((int)rustLayer);
		}

		// Token: 0x06003D99 RID: 15769 RVA: 0x001699E6 File Offset: 0x00167BE6
		public static bool IsOnLayer(this GameObject go, int layer)
		{
			return go != null && go.layer == layer;
		}

		// Token: 0x06003D9A RID: 15770 RVA: 0x001699FC File Offset: 0x00167BFC
		private static IEntity GetEntityFromRegistry(Transform transform)
		{
			Transform transform2 = transform;
			IEntity entity = Entity.Get(transform2);
			while (entity == null && transform2.parent != null)
			{
				transform2 = transform2.parent;
				entity = Entity.Get(transform2);
			}
			if (entity != null && !entity.IsDestroyed)
			{
				return entity;
			}
			return null;
		}

		// Token: 0x06003D9B RID: 15771 RVA: 0x00169A44 File Offset: 0x00167C44
		private static IEntity GetEntityFromComponent(Transform transform)
		{
			Transform transform2 = transform;
			IEntity component = transform2.GetComponent<IEntity>();
			while (component == null && transform2.parent != null)
			{
				transform2 = transform2.parent;
				component = transform2.GetComponent<IEntity>();
			}
			if (component != null && !component.IsDestroyed)
			{
				return component;
			}
			return null;
		}

		// Token: 0x06003D9C RID: 15772 RVA: 0x00169A89 File Offset: 0x00167C89
		public static void SetHierarchyGroup(this GameObject obj, string strRoot, bool groupActive = true, bool persistant = false)
		{
			obj.transform.SetParent(HierarchyUtil.GetRoot(strRoot, groupActive, persistant).transform, true);
		}

		// Token: 0x06003D9D RID: 15773 RVA: 0x00169AA4 File Offset: 0x00167CA4
		public static bool HasComponent<T>(this GameObject obj) where T : Component
		{
			return obj.GetComponent<T>() != null;
		}

		// Token: 0x06003D9E RID: 15774 RVA: 0x00169AB8 File Offset: 0x00167CB8
		public static void SetChildComponentsEnabled<T>(this GameObject gameObject, bool enabled) where T : MonoBehaviour
		{
			List<T> list = Pool.GetList<T>();
			gameObject.GetComponentsInChildren<T>(true, list);
			foreach (T t in list)
			{
				t.enabled = enabled;
			}
			Pool.FreeList<T>(ref list);
		}
	}
}
