using System;
using System.Collections.Generic;
using ConVar;
using Facepunch;
using UnityEngine;

// Token: 0x020002F7 RID: 759
public static class GamePhysics
{
	// Token: 0x04001778 RID: 6008
	public const int BufferLength = 8192;

	// Token: 0x04001779 RID: 6009
	private static RaycastHit[] hitBuffer = new RaycastHit[8192];

	// Token: 0x0400177A RID: 6010
	private static RaycastHit[] hitBufferB = new RaycastHit[8192];

	// Token: 0x0400177B RID: 6011
	private static Collider[] colBuffer = new Collider[8192];

	// Token: 0x06001E1D RID: 7709 RVA: 0x000CD62A File Offset: 0x000CB82A
	public static bool CheckSphere(Vector3 position, float radius, int layerMask = -5, QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.UseGlobal)
	{
		layerMask = GamePhysics.HandleTerrainCollision(position, layerMask);
		return UnityEngine.Physics.CheckSphere(position, radius, layerMask, triggerInteraction);
	}

	// Token: 0x06001E1E RID: 7710 RVA: 0x000CD63E File Offset: 0x000CB83E
	public static bool CheckCapsule(Vector3 start, Vector3 end, float radius, int layerMask = -5, QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.UseGlobal)
	{
		layerMask = GamePhysics.HandleTerrainCollision((start + end) * 0.5f, layerMask);
		return UnityEngine.Physics.CheckCapsule(start, end, radius, layerMask, triggerInteraction);
	}

	// Token: 0x06001E1F RID: 7711 RVA: 0x000CD664 File Offset: 0x000CB864
	public static bool CheckOBB(OBB obb, int layerMask = -5, QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.UseGlobal)
	{
		layerMask = GamePhysics.HandleTerrainCollision(obb.position, layerMask);
		return UnityEngine.Physics.CheckBox(obb.position, obb.extents, obb.rotation, layerMask, triggerInteraction);
	}

	// Token: 0x06001E20 RID: 7712 RVA: 0x000CD690 File Offset: 0x000CB890
	public static bool CheckOBBAndEntity(OBB obb, int layerMask = -5, QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.UseGlobal, BaseEntity ignoreEntity = null)
	{
		layerMask = GamePhysics.HandleTerrainCollision(obb.position, layerMask);
		int num = UnityEngine.Physics.OverlapBoxNonAlloc(obb.position, obb.extents, GamePhysics.colBuffer, obb.rotation, layerMask, triggerInteraction);
		for (int i = 0; i < num; i++)
		{
			BaseEntity baseEntity = GamePhysics.colBuffer[i].ToBaseEntity();
			if (!(baseEntity != null) || !(ignoreEntity != null) || (baseEntity.isServer == ignoreEntity.isServer && !(baseEntity == ignoreEntity)))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06001E21 RID: 7713 RVA: 0x000CD70F File Offset: 0x000CB90F
	public static bool CheckBounds(Bounds bounds, int layerMask = -5, QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.UseGlobal)
	{
		layerMask = GamePhysics.HandleTerrainCollision(bounds.center, layerMask);
		return UnityEngine.Physics.CheckBox(bounds.center, bounds.extents, Quaternion.identity, layerMask, triggerInteraction);
	}

	// Token: 0x06001E22 RID: 7714 RVA: 0x000CD73C File Offset: 0x000CB93C
	public static bool CheckInsideNonConvexMesh(Vector3 point, int layerMask = -5)
	{
		bool queriesHitBackfaces = UnityEngine.Physics.queriesHitBackfaces;
		UnityEngine.Physics.queriesHitBackfaces = true;
		int num = UnityEngine.Physics.RaycastNonAlloc(point, Vector3.up, GamePhysics.hitBuffer, 100f, layerMask);
		int num2 = UnityEngine.Physics.RaycastNonAlloc(point, -Vector3.up, GamePhysics.hitBufferB, 100f, layerMask);
		if (num >= GamePhysics.hitBuffer.Length)
		{
			Debug.LogWarning("CheckInsideNonConvexMesh query is exceeding hitBuffer length.");
			return false;
		}
		if (num2 > GamePhysics.hitBufferB.Length)
		{
			Debug.LogWarning("CheckInsideNonConvexMesh query is exceeding hitBufferB length.");
			return false;
		}
		for (int i = 0; i < num; i++)
		{
			for (int j = 0; j < num2; j++)
			{
				if (GamePhysics.hitBuffer[i].collider == GamePhysics.hitBufferB[j].collider)
				{
					UnityEngine.Physics.queriesHitBackfaces = queriesHitBackfaces;
					return true;
				}
			}
		}
		UnityEngine.Physics.queriesHitBackfaces = queriesHitBackfaces;
		return false;
	}

	// Token: 0x06001E23 RID: 7715 RVA: 0x000CD807 File Offset: 0x000CBA07
	public static bool CheckInsideAnyCollider(Vector3 point, int layerMask = -5)
	{
		return UnityEngine.Physics.CheckSphere(point, 0f, layerMask) || GamePhysics.CheckInsideNonConvexMesh(point, layerMask) || (TerrainMeta.HeightMap != null && TerrainMeta.HeightMap.GetHeight(point) > point.y);
	}

	// Token: 0x06001E24 RID: 7716 RVA: 0x000CD847 File Offset: 0x000CBA47
	public static void OverlapSphere(Vector3 position, float radius, List<Collider> list, int layerMask = -5, QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.Ignore)
	{
		layerMask = GamePhysics.HandleTerrainCollision(position, layerMask);
		GamePhysics.BufferToList(UnityEngine.Physics.OverlapSphereNonAlloc(position, radius, GamePhysics.colBuffer, layerMask, triggerInteraction), list);
	}

	// Token: 0x06001E25 RID: 7717 RVA: 0x000CD867 File Offset: 0x000CBA67
	public static void CapsuleSweep(Vector3 position0, Vector3 position1, float radius, Vector3 direction, float distance, List<RaycastHit> list, int layerMask = -5, QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.Ignore)
	{
		layerMask = GamePhysics.HandleTerrainCollision(position1, layerMask);
		layerMask = GamePhysics.HandleTerrainCollision(position1, layerMask);
		GamePhysics.HitBufferToList(UnityEngine.Physics.CapsuleCastNonAlloc(position0, position1, radius, direction, GamePhysics.hitBuffer, distance, layerMask, triggerInteraction), list);
	}

	// Token: 0x06001E26 RID: 7718 RVA: 0x000CD898 File Offset: 0x000CBA98
	public static void OverlapCapsule(Vector3 point0, Vector3 point1, float radius, List<Collider> list, int layerMask = -5, QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.Ignore)
	{
		layerMask = GamePhysics.HandleTerrainCollision(point0, layerMask);
		layerMask = GamePhysics.HandleTerrainCollision(point1, layerMask);
		GamePhysics.BufferToList(UnityEngine.Physics.OverlapCapsuleNonAlloc(point0, point1, radius, GamePhysics.colBuffer, layerMask, triggerInteraction), list);
	}

	// Token: 0x06001E27 RID: 7719 RVA: 0x000CD8C5 File Offset: 0x000CBAC5
	public static void OverlapOBB(OBB obb, List<Collider> list, int layerMask = -5, QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.Ignore)
	{
		layerMask = GamePhysics.HandleTerrainCollision(obb.position, layerMask);
		GamePhysics.BufferToList(UnityEngine.Physics.OverlapBoxNonAlloc(obb.position, obb.extents, GamePhysics.colBuffer, obb.rotation, layerMask, triggerInteraction), list);
	}

	// Token: 0x06001E28 RID: 7720 RVA: 0x000CD8F9 File Offset: 0x000CBAF9
	public static void OverlapBounds(Bounds bounds, List<Collider> list, int layerMask = -5, QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.Ignore)
	{
		layerMask = GamePhysics.HandleTerrainCollision(bounds.center, layerMask);
		GamePhysics.BufferToList(UnityEngine.Physics.OverlapBoxNonAlloc(bounds.center, bounds.extents, GamePhysics.colBuffer, Quaternion.identity, layerMask, triggerInteraction), list);
	}

	// Token: 0x06001E29 RID: 7721 RVA: 0x000CD930 File Offset: 0x000CBB30
	private static void BufferToList(int count, List<Collider> list)
	{
		if (count >= GamePhysics.colBuffer.Length)
		{
			Debug.LogWarning("Physics query is exceeding collider buffer length.");
		}
		for (int i = 0; i < count; i++)
		{
			list.Add(GamePhysics.colBuffer[i]);
			GamePhysics.colBuffer[i] = null;
		}
	}

	// Token: 0x06001E2A RID: 7722 RVA: 0x000CD974 File Offset: 0x000CBB74
	public static bool CheckSphere<T>(Vector3 pos, float radius, int layerMask = -5, QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.Ignore) where T : Component
	{
		List<Collider> list = Facepunch.Pool.GetList<Collider>();
		GamePhysics.OverlapSphere(pos, radius, list, layerMask, triggerInteraction);
		bool result = GamePhysics.CheckComponent<T>(list);
		Facepunch.Pool.FreeList<Collider>(ref list);
		return result;
	}

	// Token: 0x06001E2B RID: 7723 RVA: 0x000CD9A0 File Offset: 0x000CBBA0
	public static bool CheckCapsule<T>(Vector3 start, Vector3 end, float radius, int layerMask = -5, QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.Ignore) where T : Component
	{
		List<Collider> list = Facepunch.Pool.GetList<Collider>();
		GamePhysics.OverlapCapsule(start, end, radius, list, layerMask, triggerInteraction);
		bool result = GamePhysics.CheckComponent<T>(list);
		Facepunch.Pool.FreeList<Collider>(ref list);
		return result;
	}

	// Token: 0x06001E2C RID: 7724 RVA: 0x000CD9CC File Offset: 0x000CBBCC
	public static bool CheckOBB<T>(OBB obb, int layerMask = -5, QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.Ignore) where T : Component
	{
		List<Collider> list = Facepunch.Pool.GetList<Collider>();
		GamePhysics.OverlapOBB(obb, list, layerMask, triggerInteraction);
		bool result = GamePhysics.CheckComponent<T>(list);
		Facepunch.Pool.FreeList<Collider>(ref list);
		return result;
	}

	// Token: 0x06001E2D RID: 7725 RVA: 0x000CD9F8 File Offset: 0x000CBBF8
	public static bool CheckBounds<T>(Bounds bounds, int layerMask = -5, QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.Ignore) where T : Component
	{
		List<Collider> list = Facepunch.Pool.GetList<Collider>();
		GamePhysics.OverlapBounds(bounds, list, layerMask, triggerInteraction);
		bool result = GamePhysics.CheckComponent<T>(list);
		Facepunch.Pool.FreeList<Collider>(ref list);
		return result;
	}

	// Token: 0x06001E2E RID: 7726 RVA: 0x000CDA24 File Offset: 0x000CBC24
	private static bool CheckComponent<T>(List<Collider> list)
	{
		for (int i = 0; i < list.Count; i++)
		{
			if (list[i].gameObject.GetComponent<T>() != null)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06001E2F RID: 7727 RVA: 0x000CDA5D File Offset: 0x000CBC5D
	public static void OverlapSphere<T>(Vector3 position, float radius, List<T> list, int layerMask = -5, QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.Ignore) where T : Component
	{
		layerMask = GamePhysics.HandleTerrainCollision(position, layerMask);
		GamePhysics.BufferToList<T>(UnityEngine.Physics.OverlapSphereNonAlloc(position, radius, GamePhysics.colBuffer, layerMask, triggerInteraction), list);
	}

	// Token: 0x06001E30 RID: 7728 RVA: 0x000CDA7D File Offset: 0x000CBC7D
	public static void OverlapCapsule<T>(Vector3 point0, Vector3 point1, float radius, List<T> list, int layerMask = -5, QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.Ignore) where T : Component
	{
		layerMask = GamePhysics.HandleTerrainCollision(point0, layerMask);
		layerMask = GamePhysics.HandleTerrainCollision(point1, layerMask);
		GamePhysics.BufferToList<T>(UnityEngine.Physics.OverlapCapsuleNonAlloc(point0, point1, radius, GamePhysics.colBuffer, layerMask, triggerInteraction), list);
	}

	// Token: 0x06001E31 RID: 7729 RVA: 0x000CDAAA File Offset: 0x000CBCAA
	public static void OverlapOBB<T>(OBB obb, List<T> list, int layerMask = -5, QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.Ignore) where T : Component
	{
		layerMask = GamePhysics.HandleTerrainCollision(obb.position, layerMask);
		GamePhysics.BufferToList<T>(UnityEngine.Physics.OverlapBoxNonAlloc(obb.position, obb.extents, GamePhysics.colBuffer, obb.rotation, layerMask, triggerInteraction), list);
	}

	// Token: 0x06001E32 RID: 7730 RVA: 0x000CDADE File Offset: 0x000CBCDE
	public static void OverlapBounds<T>(Bounds bounds, List<T> list, int layerMask = -5, QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.Ignore) where T : Component
	{
		layerMask = GamePhysics.HandleTerrainCollision(bounds.center, layerMask);
		GamePhysics.BufferToList<T>(UnityEngine.Physics.OverlapBoxNonAlloc(bounds.center, bounds.extents, GamePhysics.colBuffer, Quaternion.identity, layerMask, triggerInteraction), list);
	}

	// Token: 0x06001E33 RID: 7731 RVA: 0x000CDB14 File Offset: 0x000CBD14
	private static void BufferToList<T>(int count, List<T> list) where T : Component
	{
		if (count >= GamePhysics.colBuffer.Length)
		{
			Debug.LogWarning("Physics query is exceeding collider buffer length.");
		}
		for (int i = 0; i < count; i++)
		{
			T component = GamePhysics.colBuffer[i].gameObject.GetComponent<T>();
			if (component)
			{
				list.Add(component);
			}
			GamePhysics.colBuffer[i] = null;
		}
	}

	// Token: 0x06001E34 RID: 7732 RVA: 0x000CDB70 File Offset: 0x000CBD70
	private static void HitBufferToList(int count, List<RaycastHit> list)
	{
		if (count >= GamePhysics.hitBuffer.Length)
		{
			Debug.LogWarning("Physics query is exceeding collider buffer length.");
		}
		for (int i = 0; i < count; i++)
		{
			list.Add(GamePhysics.hitBuffer[i]);
		}
	}

	// Token: 0x06001E35 RID: 7733 RVA: 0x000CDBB0 File Offset: 0x000CBDB0
	public static bool Trace(Ray ray, float radius, out RaycastHit hitInfo, float maxDistance = float.PositiveInfinity, int layerMask = -5, QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.UseGlobal, BaseEntity ignoreEntity = null)
	{
		List<RaycastHit> list = Facepunch.Pool.GetList<RaycastHit>();
		GamePhysics.TraceAllUnordered(ray, radius, list, maxDistance, layerMask, triggerInteraction, ignoreEntity);
		if (list.Count == 0)
		{
			hitInfo = default(RaycastHit);
			Facepunch.Pool.FreeList<RaycastHit>(ref list);
			return false;
		}
		GamePhysics.Sort(list);
		hitInfo = list[0];
		Facepunch.Pool.FreeList<RaycastHit>(ref list);
		return true;
	}

	// Token: 0x06001E36 RID: 7734 RVA: 0x000CDC05 File Offset: 0x000CBE05
	public static void TraceAll(Ray ray, float radius, List<RaycastHit> hits, float maxDistance = float.PositiveInfinity, int layerMask = -5, QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.UseGlobal, BaseEntity ignoreEntity = null)
	{
		GamePhysics.TraceAllUnordered(ray, radius, hits, maxDistance, layerMask, triggerInteraction, ignoreEntity);
		GamePhysics.Sort(hits);
	}

	// Token: 0x06001E37 RID: 7735 RVA: 0x000CDC1C File Offset: 0x000CBE1C
	public static void TraceAllUnordered(Ray ray, float radius, List<RaycastHit> hits, float maxDistance = float.PositiveInfinity, int layerMask = -5, QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.UseGlobal, BaseEntity ignoreEntity = null)
	{
		int num;
		if (radius == 0f)
		{
			num = UnityEngine.Physics.RaycastNonAlloc(ray, GamePhysics.hitBuffer, maxDistance, layerMask, triggerInteraction);
		}
		else
		{
			num = UnityEngine.Physics.SphereCastNonAlloc(ray, radius, GamePhysics.hitBuffer, maxDistance, layerMask, triggerInteraction);
		}
		if (num == 0)
		{
			return;
		}
		if (num >= GamePhysics.hitBuffer.Length)
		{
			Debug.LogWarning("Physics query is exceeding hit buffer length.");
		}
		for (int i = 0; i < num; i++)
		{
			RaycastHit raycastHit = GamePhysics.hitBuffer[i];
			if (GamePhysics.Verify(raycastHit, ignoreEntity))
			{
				hits.Add(raycastHit);
			}
		}
	}

	// Token: 0x06001E38 RID: 7736 RVA: 0x000CDC99 File Offset: 0x000CBE99
	public static bool LineOfSightRadius(Vector3 p0, Vector3 p1, int layerMask, float radius, float padding0, float padding1, BaseEntity ignoreEntity = null)
	{
		return GamePhysics.LineOfSightInternal(p0, p1, layerMask, radius, padding0, padding1, ignoreEntity);
	}

	// Token: 0x06001E39 RID: 7737 RVA: 0x000CDCAA File Offset: 0x000CBEAA
	public static bool LineOfSightRadius(Vector3 p0, Vector3 p1, int layerMask, float radius, float padding, BaseEntity ignoreEntity = null)
	{
		return GamePhysics.LineOfSightInternal(p0, p1, layerMask, radius, padding, padding, ignoreEntity);
	}

	// Token: 0x06001E3A RID: 7738 RVA: 0x000CDCBB File Offset: 0x000CBEBB
	public static bool LineOfSightRadius(Vector3 p0, Vector3 p1, int layerMask, float radius, BaseEntity ignoreEntity = null)
	{
		return GamePhysics.LineOfSightInternal(p0, p1, layerMask, radius, 0f, 0f, ignoreEntity);
	}

	// Token: 0x06001E3B RID: 7739 RVA: 0x000CDCD2 File Offset: 0x000CBED2
	public static bool LineOfSight(Vector3 p0, Vector3 p1, int layerMask, float padding0, float padding1, BaseEntity ignoreEntity = null)
	{
		return GamePhysics.LineOfSightRadius(p0, p1, layerMask, 0f, padding0, padding1, ignoreEntity);
	}

	// Token: 0x06001E3C RID: 7740 RVA: 0x000CDCE6 File Offset: 0x000CBEE6
	public static bool LineOfSight(Vector3 p0, Vector3 p1, int layerMask, float padding, BaseEntity ignoreEntity = null)
	{
		return GamePhysics.LineOfSightRadius(p0, p1, layerMask, 0f, padding, padding, ignoreEntity);
	}

	// Token: 0x06001E3D RID: 7741 RVA: 0x000CDCF9 File Offset: 0x000CBEF9
	public static bool LineOfSight(Vector3 p0, Vector3 p1, int layerMask, BaseEntity ignoreEntity = null)
	{
		return GamePhysics.LineOfSightRadius(p0, p1, layerMask, 0f, 0f, 0f, ignoreEntity);
	}

	// Token: 0x06001E3E RID: 7742 RVA: 0x000CDD14 File Offset: 0x000CBF14
	private static bool LineOfSightInternal(Vector3 p0, Vector3 p1, int layerMask, float radius, float padding0, float padding1, BaseEntity ignoreEntity = null)
	{
		if (!ValidBounds.Test(p0))
		{
			return false;
		}
		if (!ValidBounds.Test(p1))
		{
			return false;
		}
		Vector3 a = p1 - p0;
		float magnitude = a.magnitude;
		if (magnitude <= padding0 + padding1)
		{
			return true;
		}
		Vector3 vector = a / magnitude;
		Ray ray = new Ray(p0 + vector * padding0, vector);
		float maxDistance = magnitude - padding0 - padding1;
		RaycastHit raycastHit;
		bool flag;
		if (!ignoreEntity.IsRealNull() || (layerMask & 8388608) != 0)
		{
			flag = GamePhysics.Trace(ray, 0f, out raycastHit, maxDistance, layerMask, QueryTriggerInteraction.Ignore, ignoreEntity);
			if (radius > 0f && !flag)
			{
				flag = GamePhysics.Trace(ray, radius, out raycastHit, maxDistance, layerMask, QueryTriggerInteraction.Ignore, ignoreEntity);
			}
		}
		else
		{
			flag = UnityEngine.Physics.Raycast(ray, out raycastHit, maxDistance, layerMask, QueryTriggerInteraction.Ignore);
			if (radius > 0f && !flag)
			{
				flag = UnityEngine.Physics.SphereCast(ray, radius, out raycastHit, maxDistance, layerMask, QueryTriggerInteraction.Ignore);
			}
		}
		if (!flag)
		{
			if (ConVar.Vis.lineofsight)
			{
				ConsoleNetwork.BroadcastToAllClients("ddraw.line", new object[]
				{
					60f,
					Color.green,
					p0,
					p1
				});
			}
			return true;
		}
		if (ConVar.Vis.lineofsight)
		{
			ConsoleNetwork.BroadcastToAllClients("ddraw.line", new object[]
			{
				60f,
				Color.red,
				p0,
				p1
			});
			ConsoleNetwork.BroadcastToAllClients("ddraw.text", new object[]
			{
				60f,
				Color.white,
				raycastHit.point,
				raycastHit.collider.name
			});
		}
		return false;
	}

	// Token: 0x06001E3F RID: 7743 RVA: 0x000CDEC3 File Offset: 0x000CC0C3
	public static bool Verify(RaycastHit hitInfo, BaseEntity ignoreEntity = null)
	{
		return GamePhysics.Verify(hitInfo.collider, hitInfo.point, ignoreEntity);
	}

	// Token: 0x06001E40 RID: 7744 RVA: 0x000CDED9 File Offset: 0x000CC0D9
	public static bool Verify(Collider collider, Vector3 point, BaseEntity ignoreEntity = null)
	{
		return (!(collider is TerrainCollider) || !TerrainMeta.Collision || !TerrainMeta.Collision.GetIgnore(point, 0.01f)) && !GamePhysics.CompareEntity(collider.ToBaseEntity(), ignoreEntity) && collider.enabled;
	}

	// Token: 0x06001E41 RID: 7745 RVA: 0x000CDF19 File Offset: 0x000CC119
	private static bool CompareEntity(BaseEntity a, BaseEntity b)
	{
		return !a.IsRealNull() && !b.IsRealNull() && a == b;
	}

	// Token: 0x06001E42 RID: 7746 RVA: 0x000CDF3C File Offset: 0x000CC13C
	public static int HandleTerrainCollision(Vector3 position, int layerMask)
	{
		int num = 8388608;
		if ((layerMask & num) != 0 && TerrainMeta.Collision && TerrainMeta.Collision.GetIgnore(position, 0.01f))
		{
			layerMask &= ~num;
		}
		return layerMask;
	}

	// Token: 0x06001E43 RID: 7747 RVA: 0x000CDF79 File Offset: 0x000CC179
	public static void Sort(List<RaycastHit> hits)
	{
		hits.Sort((RaycastHit a, RaycastHit b) => a.distance.CompareTo(b.distance));
	}

	// Token: 0x06001E44 RID: 7748 RVA: 0x000CDFA0 File Offset: 0x000CC1A0
	public static void Sort(RaycastHit[] hits)
	{
		Array.Sort<RaycastHit>(hits, (RaycastHit a, RaycastHit b) => a.distance.CompareTo(b.distance));
	}
}
