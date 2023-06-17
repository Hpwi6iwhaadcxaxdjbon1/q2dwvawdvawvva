using System;
using System.Collections.Generic;
using System.Linq;
using Facepunch;

namespace UnityEngine
{
	// Token: 0x02000A29 RID: 2601
	public static class TransformEx
	{
		// Token: 0x06003DAB RID: 15787 RVA: 0x00169D88 File Offset: 0x00167F88
		public static string GetRecursiveName(this Transform transform, string strEndName = "")
		{
			string text = transform.name;
			if (!string.IsNullOrEmpty(strEndName))
			{
				text = text + "/" + strEndName;
			}
			if (transform.parent != null)
			{
				text = transform.parent.GetRecursiveName(text);
			}
			return text;
		}

		// Token: 0x06003DAC RID: 15788 RVA: 0x00169DD0 File Offset: 0x00167FD0
		public static void RemoveComponent<T>(this Transform transform) where T : Component
		{
			T component = transform.GetComponent<T>();
			if (component == null)
			{
				return;
			}
			GameManager.Destroy(component, 0f);
		}

		// Token: 0x06003DAD RID: 15789 RVA: 0x00169E04 File Offset: 0x00168004
		public static void RetireAllChildren(this Transform transform, GameManager gameManager)
		{
			List<GameObject> list = Pool.GetList<GameObject>();
			foreach (object obj in transform)
			{
				Transform transform2 = (Transform)obj;
				if (!transform2.CompareTag("persist"))
				{
					list.Add(transform2.gameObject);
				}
			}
			foreach (GameObject instance in list)
			{
				gameManager.Retire(instance);
			}
			Pool.FreeList<GameObject>(ref list);
		}

		// Token: 0x06003DAE RID: 15790 RVA: 0x00169EB8 File Offset: 0x001680B8
		public static List<Transform> GetChildren(this Transform transform)
		{
			return transform.Cast<Transform>().ToList<Transform>();
		}

		// Token: 0x06003DAF RID: 15791 RVA: 0x00169EC8 File Offset: 0x001680C8
		public static void OrderChildren(this Transform tx, Func<Transform, object> selector)
		{
			foreach (Transform transform in tx.Cast<Transform>().OrderBy(selector))
			{
				transform.SetAsLastSibling();
			}
		}

		// Token: 0x06003DB0 RID: 15792 RVA: 0x00169F18 File Offset: 0x00168118
		public static List<Transform> GetAllChildren(this Transform transform)
		{
			List<Transform> list = new List<Transform>();
			if (transform != null)
			{
				transform.AddAllChildren(list);
			}
			return list;
		}

		// Token: 0x06003DB1 RID: 15793 RVA: 0x00169F3C File Offset: 0x0016813C
		public static void AddAllChildren(this Transform transform, List<Transform> list)
		{
			list.Add(transform);
			for (int i = 0; i < transform.childCount; i++)
			{
				Transform child = transform.GetChild(i);
				if (!(child == null))
				{
					child.AddAllChildren(list);
				}
			}
		}

		// Token: 0x06003DB2 RID: 15794 RVA: 0x00169F7C File Offset: 0x0016817C
		public static Transform[] GetChildrenWithTag(this Transform transform, string strTag)
		{
			return (from x in transform.GetAllChildren()
			where x.CompareTag(strTag)
			select x).ToArray<Transform>();
		}

		// Token: 0x06003DB3 RID: 15795 RVA: 0x00169FB2 File Offset: 0x001681B2
		public static void Identity(this GameObject go)
		{
			go.transform.localPosition = Vector3.zero;
			go.transform.localRotation = Quaternion.identity;
			go.transform.localScale = Vector3.one;
		}

		// Token: 0x06003DB4 RID: 15796 RVA: 0x00169FE4 File Offset: 0x001681E4
		public static GameObject CreateChild(this GameObject go)
		{
			GameObject gameObject = new GameObject();
			gameObject.transform.parent = go.transform;
			gameObject.Identity();
			return gameObject;
		}

		// Token: 0x06003DB5 RID: 15797 RVA: 0x0016A002 File Offset: 0x00168202
		public static GameObject InstantiateChild(this GameObject go, GameObject prefab)
		{
			GameObject gameObject = Instantiate.GameObject(prefab, null);
			gameObject.transform.SetParent(go.transform, false);
			gameObject.Identity();
			return gameObject;
		}

		// Token: 0x06003DB6 RID: 15798 RVA: 0x0016A024 File Offset: 0x00168224
		public static void SetLayerRecursive(this GameObject go, int Layer)
		{
			if (go.layer != Layer)
			{
				go.layer = Layer;
			}
			for (int i = 0; i < go.transform.childCount; i++)
			{
				go.transform.GetChild(i).gameObject.SetLayerRecursive(Layer);
			}
		}

		// Token: 0x06003DB7 RID: 15799 RVA: 0x0016A070 File Offset: 0x00168270
		public static bool DropToGround(this Transform transform, bool alignToNormal = false, float fRange = 100f)
		{
			Vector3 position;
			Vector3 upwards;
			if (transform.GetGroundInfo(out position, out upwards, fRange))
			{
				transform.position = position;
				if (alignToNormal)
				{
					transform.rotation = Quaternion.LookRotation(transform.forward, upwards);
				}
				return true;
			}
			return false;
		}

		// Token: 0x06003DB8 RID: 15800 RVA: 0x0016A0A9 File Offset: 0x001682A9
		public static bool GetGroundInfo(this Transform transform, out Vector3 pos, out Vector3 normal, float range = 100f)
		{
			return TransformUtil.GetGroundInfo(transform.position, out pos, out normal, range, transform);
		}

		// Token: 0x06003DB9 RID: 15801 RVA: 0x0016A0BA File Offset: 0x001682BA
		public static bool GetGroundInfoTerrainOnly(this Transform transform, out Vector3 pos, out Vector3 normal, float range = 100f)
		{
			return TransformUtil.GetGroundInfoTerrainOnly(transform.position, out pos, out normal, range);
		}

		// Token: 0x06003DBA RID: 15802 RVA: 0x0016A0CC File Offset: 0x001682CC
		public static Bounds WorkoutRenderBounds(this Transform tx)
		{
			Bounds bounds = new Bounds(Vector3.zero, Vector3.zero);
			foreach (Renderer renderer in tx.GetComponentsInChildren<Renderer>())
			{
				if (!(renderer is ParticleSystemRenderer))
				{
					if (bounds.center == Vector3.zero)
					{
						bounds = renderer.bounds;
					}
					else
					{
						bounds.Encapsulate(renderer.bounds);
					}
				}
			}
			return bounds;
		}

		// Token: 0x06003DBB RID: 15803 RVA: 0x0016A138 File Offset: 0x00168338
		public static List<T> GetSiblings<T>(this Transform transform, bool includeSelf = false)
		{
			List<T> list = new List<T>();
			if (transform.parent == null)
			{
				return list;
			}
			for (int i = 0; i < transform.parent.childCount; i++)
			{
				Transform child = transform.parent.GetChild(i);
				if (includeSelf || !(child == transform))
				{
					T component = child.GetComponent<T>();
					if (component != null)
					{
						list.Add(component);
					}
				}
			}
			return list;
		}

		// Token: 0x06003DBC RID: 15804 RVA: 0x0016A1A4 File Offset: 0x001683A4
		public static void DestroyChildren(this Transform transform)
		{
			for (int i = 0; i < transform.childCount; i++)
			{
				GameManager.Destroy(transform.GetChild(i).gameObject, 0f);
			}
		}

		// Token: 0x06003DBD RID: 15805 RVA: 0x0016A1D8 File Offset: 0x001683D8
		public static void SetChildrenActive(this Transform transform, bool b)
		{
			for (int i = 0; i < transform.childCount; i++)
			{
				transform.GetChild(i).gameObject.SetActive(b);
			}
		}

		// Token: 0x06003DBE RID: 15806 RVA: 0x0016A208 File Offset: 0x00168408
		public static Transform ActiveChild(this Transform transform, string name, bool bDisableOthers)
		{
			Transform result = null;
			for (int i = 0; i < transform.childCount; i++)
			{
				Transform child = transform.GetChild(i);
				if (child.name.Equals(name, StringComparison.InvariantCultureIgnoreCase))
				{
					result = child;
					child.gameObject.SetActive(true);
				}
				else if (bDisableOthers)
				{
					child.gameObject.SetActive(false);
				}
			}
			return result;
		}

		// Token: 0x06003DBF RID: 15807 RVA: 0x0016A260 File Offset: 0x00168460
		public static T GetComponentInChildrenIncludeDisabled<T>(this Transform transform) where T : Component
		{
			List<T> list = Pool.GetList<T>();
			transform.GetComponentsInChildren<T>(true, list);
			T result = (list.Count > 0) ? list[0] : default(T);
			Pool.FreeList<T>(ref list);
			return result;
		}

		// Token: 0x06003DC0 RID: 15808 RVA: 0x0016A2A0 File Offset: 0x001684A0
		public static bool HasComponentInChildrenIncludeDisabled<T>(this Transform transform) where T : Component
		{
			List<T> list = Pool.GetList<T>();
			transform.GetComponentsInChildren<T>(true, list);
			bool result = list.Count > 0;
			Pool.FreeList<T>(ref list);
			return result;
		}

		// Token: 0x06003DC1 RID: 15809 RVA: 0x0016A2CB File Offset: 0x001684CB
		public static void SetHierarchyGroup(this Transform transform, string strRoot, bool groupActive = true, bool persistant = false)
		{
			transform.SetParent(HierarchyUtil.GetRoot(strRoot, groupActive, persistant).transform, true);
		}

		// Token: 0x06003DC2 RID: 15810 RVA: 0x0016A2E4 File Offset: 0x001684E4
		public static Bounds GetBounds(this Transform transform, bool includeRenderers = true, bool includeColliders = true, bool includeInactive = true)
		{
			Bounds result = new Bounds(Vector3.zero, Vector3.zero);
			if (includeRenderers)
			{
				foreach (MeshFilter meshFilter in transform.GetComponentsInChildren<MeshFilter>(includeInactive))
				{
					if (meshFilter.sharedMesh)
					{
						Matrix4x4 matrix = transform.worldToLocalMatrix * meshFilter.transform.localToWorldMatrix;
						Bounds bounds = meshFilter.sharedMesh.bounds;
						result.Encapsulate(bounds.Transform(matrix));
					}
				}
				foreach (SkinnedMeshRenderer skinnedMeshRenderer in transform.GetComponentsInChildren<SkinnedMeshRenderer>(includeInactive))
				{
					if (skinnedMeshRenderer.sharedMesh)
					{
						Matrix4x4 matrix2 = transform.worldToLocalMatrix * skinnedMeshRenderer.transform.localToWorldMatrix;
						Bounds bounds2 = skinnedMeshRenderer.sharedMesh.bounds;
						result.Encapsulate(bounds2.Transform(matrix2));
					}
				}
			}
			if (includeColliders)
			{
				foreach (MeshCollider meshCollider in transform.GetComponentsInChildren<MeshCollider>(includeInactive))
				{
					if (meshCollider.sharedMesh && !meshCollider.isTrigger)
					{
						Matrix4x4 matrix3 = transform.worldToLocalMatrix * meshCollider.transform.localToWorldMatrix;
						Bounds bounds3 = meshCollider.sharedMesh.bounds;
						result.Encapsulate(bounds3.Transform(matrix3));
					}
				}
			}
			return result;
		}
	}
}
