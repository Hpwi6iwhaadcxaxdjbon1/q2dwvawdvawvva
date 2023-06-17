using System;
using System.Collections.Generic;
using ConVar;
using Facepunch;
using UnityEngine;

// Token: 0x02000518 RID: 1304
public class GroundWatch : BaseMonoBehaviour, IServerComponent
{
	// Token: 0x04002196 RID: 8598
	public Vector3 groundPosition = Vector3.zero;

	// Token: 0x04002197 RID: 8599
	public LayerMask layers = 27328512;

	// Token: 0x04002198 RID: 8600
	public float radius = 0.1f;

	// Token: 0x04002199 RID: 8601
	[Header("Whitelist")]
	public BaseEntity[] whitelist;

	// Token: 0x0400219A RID: 8602
	private int fails;

	// Token: 0x06002993 RID: 10643 RVA: 0x000FEC2D File Offset: 0x000FCE2D
	private void OnDrawGizmosSelected()
	{
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.color = Color.green;
		Gizmos.DrawSphere(this.groundPosition, this.radius);
	}

	// Token: 0x06002994 RID: 10644 RVA: 0x000FEC5C File Offset: 0x000FCE5C
	public static void PhysicsChanged(GameObject obj)
	{
		if (obj == null)
		{
			return;
		}
		Collider component = obj.GetComponent<Collider>();
		if (!component)
		{
			return;
		}
		Bounds bounds = component.bounds;
		List<BaseEntity> list = Facepunch.Pool.GetList<BaseEntity>();
		global::Vis.Entities<BaseEntity>(bounds.center, bounds.extents.magnitude + 1f, list, 2263296, QueryTriggerInteraction.Collide);
		foreach (BaseEntity baseEntity in list)
		{
			if (!baseEntity.IsDestroyed && !baseEntity.isClient && !(baseEntity is BuildingBlock))
			{
				baseEntity.BroadcastMessage("OnPhysicsNeighbourChanged", SendMessageOptions.DontRequireReceiver);
			}
		}
		Facepunch.Pool.FreeList<BaseEntity>(ref list);
	}

	// Token: 0x06002995 RID: 10645 RVA: 0x000FED24 File Offset: 0x000FCF24
	public static void PhysicsChanged(Vector3 origin, float radius, int layerMask)
	{
		List<BaseEntity> list = Facepunch.Pool.GetList<BaseEntity>();
		global::Vis.Entities<BaseEntity>(origin, radius, list, layerMask, QueryTriggerInteraction.Collide);
		foreach (BaseEntity baseEntity in list)
		{
			if (!baseEntity.IsDestroyed && !baseEntity.isClient && !(baseEntity is BuildingBlock))
			{
				baseEntity.BroadcastMessage("OnPhysicsNeighbourChanged", SendMessageOptions.DontRequireReceiver);
			}
		}
		Facepunch.Pool.FreeList<BaseEntity>(ref list);
	}

	// Token: 0x06002996 RID: 10646 RVA: 0x000FEDA8 File Offset: 0x000FCFA8
	private void OnPhysicsNeighbourChanged()
	{
		if (!this.OnGround())
		{
			this.fails++;
			if (this.fails < ConVar.Physics.groundwatchfails)
			{
				if (ConVar.Physics.groundwatchdebug)
				{
					Debug.Log("GroundWatch retry: " + this.fails);
				}
				base.Invoke(new Action(this.OnPhysicsNeighbourChanged), ConVar.Physics.groundwatchdelay);
				return;
			}
			BaseEntity baseEntity = base.gameObject.ToBaseEntity();
			if (baseEntity)
			{
				baseEntity.transform.BroadcastMessage("OnGroundMissing", SendMessageOptions.DontRequireReceiver);
				return;
			}
		}
		else
		{
			this.fails = 0;
		}
	}

	// Token: 0x06002997 RID: 10647 RVA: 0x000FEE40 File Offset: 0x000FD040
	private bool OnGround()
	{
		BaseEntity component = base.GetComponent<BaseEntity>();
		if (component)
		{
			Construction construction = PrefabAttribute.server.Find<Construction>(component.prefabID);
			if (construction)
			{
				Socket_Base[] allSockets = construction.allSockets;
				for (int i = 0; i < allSockets.Length; i++)
				{
					SocketMod[] socketMods = allSockets[i].socketMods;
					for (int j = 0; j < socketMods.Length; j++)
					{
						SocketMod_AreaCheck socketMod_AreaCheck = socketMods[j] as SocketMod_AreaCheck;
						if (socketMod_AreaCheck && socketMod_AreaCheck.wantsInside && !socketMod_AreaCheck.DoCheck(component.transform.position, component.transform.rotation, component))
						{
							if (ConVar.Physics.groundwatchdebug)
							{
								Debug.Log("GroundWatch failed: " + socketMod_AreaCheck.hierachyName);
							}
							return false;
						}
					}
				}
			}
		}
		List<Collider> list = Facepunch.Pool.GetList<Collider>();
		global::Vis.Colliders<Collider>(base.transform.TransformPoint(this.groundPosition), this.radius, list, this.layers, QueryTriggerInteraction.Collide);
		foreach (Collider collider in list)
		{
			BaseEntity baseEntity = collider.gameObject.ToBaseEntity();
			if (!baseEntity || (!(baseEntity == component) && !baseEntity.IsDestroyed && !baseEntity.isClient))
			{
				if (this.whitelist != null && this.whitelist.Length != 0)
				{
					bool flag = false;
					foreach (BaseEntity baseEntity2 in this.whitelist)
					{
						if (baseEntity.prefabID == baseEntity2.prefabID)
						{
							flag = true;
							break;
						}
					}
					if (!flag)
					{
						continue;
					}
				}
				DecayEntity decayEntity = component as DecayEntity;
				DecayEntity decayEntity2 = baseEntity as DecayEntity;
				if (!decayEntity || decayEntity.buildingID == 0U || !decayEntity2 || decayEntity2.buildingID == 0U || decayEntity.buildingID == decayEntity2.buildingID)
				{
					Facepunch.Pool.FreeList<Collider>(ref list);
					return true;
				}
			}
		}
		if (ConVar.Physics.groundwatchdebug)
		{
			Debug.Log("GroundWatch failed: Legacy radius check");
		}
		Facepunch.Pool.FreeList<Collider>(ref list);
		return false;
	}
}
