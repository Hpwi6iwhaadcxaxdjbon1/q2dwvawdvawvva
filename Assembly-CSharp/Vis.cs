using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200059E RID: 1438
public static class Vis
{
	// Token: 0x0400235E RID: 9054
	private static int colCount = 0;

	// Token: 0x0400235F RID: 9055
	private static Collider[] colBuffer = new Collider[8192];

	// Token: 0x06002BBC RID: 11196 RVA: 0x0010893C File Offset: 0x00106B3C
	private static void Buffer(Vector3 position, float radius, int layerMask = -1, QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.Collide)
	{
		layerMask = GamePhysics.HandleTerrainCollision(position, layerMask);
		int num = Vis.colCount;
		Vis.colCount = Physics.OverlapSphereNonAlloc(position, radius, Vis.colBuffer, layerMask, triggerInteraction);
		for (int i = Vis.colCount; i < num; i++)
		{
			Vis.colBuffer[i] = null;
		}
		if (Vis.colCount >= Vis.colBuffer.Length)
		{
			Debug.LogWarning("Vis query is exceeding collider buffer length.");
			Vis.colCount = Vis.colBuffer.Length;
		}
	}

	// Token: 0x06002BBD RID: 11197 RVA: 0x001089A7 File Offset: 0x00106BA7
	public static bool AnyColliders(Vector3 position, float radius, int layerMask = -1, QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.Ignore)
	{
		Vis.Buffer(position, radius, layerMask, triggerInteraction);
		return Vis.colCount > 0;
	}

	// Token: 0x06002BBE RID: 11198 RVA: 0x001089BC File Offset: 0x00106BBC
	public static void Colliders<T>(Vector3 position, float radius, List<T> list, int layerMask = -1, QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.Collide) where T : Collider
	{
		Vis.Buffer(position, radius, layerMask, triggerInteraction);
		for (int i = 0; i < Vis.colCount; i++)
		{
			T t = Vis.colBuffer[i] as T;
			if (!(t == null) && t.enabled)
			{
				list.Add(t);
			}
		}
	}

	// Token: 0x06002BBF RID: 11199 RVA: 0x00108A18 File Offset: 0x00106C18
	public static void Components<T>(Vector3 position, float radius, List<T> list, int layerMask = -1, QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.Collide) where T : Component
	{
		Vis.Buffer(position, radius, layerMask, triggerInteraction);
		for (int i = 0; i < Vis.colCount; i++)
		{
			Collider collider = Vis.colBuffer[i];
			if (!(collider == null) && collider.enabled)
			{
				T component = collider.GetComponent<T>();
				if (!(component == null))
				{
					list.Add(component);
				}
			}
		}
	}

	// Token: 0x06002BC0 RID: 11200 RVA: 0x00108A74 File Offset: 0x00106C74
	public static void Entities<T>(Vector3 position, float radius, List<T> list, int layerMask = -1, QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.Collide) where T : class
	{
		Vis.Buffer(position, radius, layerMask, triggerInteraction);
		for (int i = 0; i < Vis.colCount; i++)
		{
			Collider collider = Vis.colBuffer[i];
			if (!(collider == null) && collider.enabled)
			{
				T t = collider.ToBaseEntity() as T;
				if (t != null)
				{
					list.Add(t);
				}
			}
		}
	}

	// Token: 0x06002BC1 RID: 11201 RVA: 0x00108AD4 File Offset: 0x00106CD4
	public static void EntityComponents<T>(Vector3 position, float radius, List<T> list, int layerMask = -1, QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.Collide) where T : EntityComponentBase
	{
		Vis.Buffer(position, radius, layerMask, triggerInteraction);
		for (int i = 0; i < Vis.colCount; i++)
		{
			Collider collider = Vis.colBuffer[i];
			if (!(collider == null) && collider.enabled)
			{
				BaseEntity baseEntity = collider.ToBaseEntity();
				if (!(baseEntity == null))
				{
					T component = baseEntity.GetComponent<T>();
					if (!(component == null))
					{
						list.Add(component);
					}
				}
			}
		}
	}

	// Token: 0x06002BC2 RID: 11202 RVA: 0x00108B40 File Offset: 0x00106D40
	private static void Buffer(OBB bounds, int layerMask = -1, QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.Collide)
	{
		layerMask = GamePhysics.HandleTerrainCollision(bounds.position, layerMask);
		int num = Vis.colCount;
		Vis.colCount = Physics.OverlapBoxNonAlloc(bounds.position, bounds.extents, Vis.colBuffer, bounds.rotation, layerMask, triggerInteraction);
		for (int i = Vis.colCount; i < num; i++)
		{
			Vis.colBuffer[i] = null;
		}
		if (Vis.colCount >= Vis.colBuffer.Length)
		{
			Debug.LogWarning("Vis query is exceeding collider buffer length.");
			Vis.colCount = Vis.colBuffer.Length;
		}
	}

	// Token: 0x06002BC3 RID: 11203 RVA: 0x00108BC0 File Offset: 0x00106DC0
	public static void Colliders<T>(OBB bounds, List<T> list, int layerMask = -1, QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.Collide) where T : Collider
	{
		Vis.Buffer(bounds, layerMask, triggerInteraction);
		for (int i = 0; i < Vis.colCount; i++)
		{
			T t = Vis.colBuffer[i] as T;
			if (!(t == null) && t.enabled)
			{
				list.Add(t);
			}
		}
	}

	// Token: 0x06002BC4 RID: 11204 RVA: 0x00108C1C File Offset: 0x00106E1C
	public static void Components<T>(OBB bounds, List<T> list, int layerMask = -1, QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.Collide) where T : Component
	{
		Vis.Buffer(bounds, layerMask, triggerInteraction);
		for (int i = 0; i < Vis.colCount; i++)
		{
			Collider collider = Vis.colBuffer[i];
			if (!(collider == null) && collider.enabled)
			{
				T component = collider.GetComponent<T>();
				if (!(component == null))
				{
					list.Add(component);
				}
			}
		}
	}

	// Token: 0x06002BC5 RID: 11205 RVA: 0x00108C78 File Offset: 0x00106E78
	public static void Entities<T>(OBB bounds, List<T> list, int layerMask = -1, QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.Collide) where T : BaseEntity
	{
		Vis.Buffer(bounds, layerMask, triggerInteraction);
		for (int i = 0; i < Vis.colCount; i++)
		{
			Collider collider = Vis.colBuffer[i];
			if (!(collider == null) && collider.enabled)
			{
				T t = collider.ToBaseEntity() as T;
				if (!(t == null))
				{
					list.Add(t);
				}
			}
		}
	}

	// Token: 0x06002BC6 RID: 11206 RVA: 0x00108CDC File Offset: 0x00106EDC
	public static void EntityComponents<T>(OBB bounds, List<T> list, int layerMask = -1, QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.Collide) where T : EntityComponentBase
	{
		Vis.Buffer(bounds, layerMask, triggerInteraction);
		for (int i = 0; i < Vis.colCount; i++)
		{
			Collider collider = Vis.colBuffer[i];
			if (!(collider == null) && collider.enabled)
			{
				BaseEntity baseEntity = collider.ToBaseEntity();
				if (!(baseEntity == null))
				{
					T component = baseEntity.GetComponent<T>();
					if (!(component == null))
					{
						list.Add(component);
					}
				}
			}
		}
	}

	// Token: 0x06002BC7 RID: 11207 RVA: 0x00108D48 File Offset: 0x00106F48
	private static void Buffer(Vector3 startPosition, Vector3 endPosition, float radius, int layerMask = -1, QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.Collide)
	{
		layerMask = GamePhysics.HandleTerrainCollision(startPosition, layerMask);
		int num = Vis.colCount;
		Vis.colCount = Physics.OverlapCapsuleNonAlloc(startPosition, endPosition, radius, Vis.colBuffer, layerMask, triggerInteraction);
		for (int i = Vis.colCount; i < num; i++)
		{
			Vis.colBuffer[i] = null;
		}
		if (Vis.colCount >= Vis.colBuffer.Length)
		{
			Debug.LogWarning("Vis query is exceeding collider buffer length.");
			Vis.colCount = Vis.colBuffer.Length;
		}
	}

	// Token: 0x06002BC8 RID: 11208 RVA: 0x00108DB8 File Offset: 0x00106FB8
	public static void Entities<T>(Vector3 startPosition, Vector3 endPosition, float radius, List<T> list, int layerMask = -1, QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.Collide) where T : BaseEntity
	{
		Vis.Buffer(startPosition, endPosition, radius, layerMask, triggerInteraction);
		for (int i = 0; i < Vis.colCount; i++)
		{
			Collider collider = Vis.colBuffer[i];
			if (!(collider == null) && collider.enabled)
			{
				T t = collider.ToBaseEntity() as T;
				if (!(t == null))
				{
					list.Add(t);
				}
			}
		}
	}
}
