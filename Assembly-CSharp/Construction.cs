using System;
using System.Collections.Generic;
using System.Linq;
using Facepunch;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x02000252 RID: 594
public class Construction : PrefabAttribute
{
	// Token: 0x040014E5 RID: 5349
	public static string lastPlacementError;

	// Token: 0x040014E6 RID: 5350
	public BaseEntity.Menu.Option info;

	// Token: 0x040014E7 RID: 5351
	public bool canBypassBuildingPermission;

	// Token: 0x040014E8 RID: 5352
	[FormerlySerializedAs("canRotate")]
	public bool canRotateBeforePlacement;

	// Token: 0x040014E9 RID: 5353
	[FormerlySerializedAs("canRotate")]
	public bool canRotateAfterPlacement;

	// Token: 0x040014EA RID: 5354
	public bool checkVolumeOnRotate;

	// Token: 0x040014EB RID: 5355
	public bool checkVolumeOnUpgrade;

	// Token: 0x040014EC RID: 5356
	public bool canPlaceAtMaxDistance;

	// Token: 0x040014ED RID: 5357
	public bool placeOnWater;

	// Token: 0x040014EE RID: 5358
	public Vector3 rotationAmount = new Vector3(0f, 90f, 0f);

	// Token: 0x040014EF RID: 5359
	public Vector3 applyStartingRotation = Vector3.zero;

	// Token: 0x040014F0 RID: 5360
	public Transform deployOffset;

	// Token: 0x040014F1 RID: 5361
	[Range(0f, 10f)]
	public float healthMultiplier = 1f;

	// Token: 0x040014F2 RID: 5362
	[Range(0f, 10f)]
	public float costMultiplier = 1f;

	// Token: 0x040014F3 RID: 5363
	[Range(1f, 50f)]
	public float maxplaceDistance = 4f;

	// Token: 0x040014F4 RID: 5364
	public Mesh guideMesh;

	// Token: 0x040014F5 RID: 5365
	[NonSerialized]
	public Socket_Base[] allSockets;

	// Token: 0x040014F6 RID: 5366
	[NonSerialized]
	public BuildingProximity[] allProximities;

	// Token: 0x040014F7 RID: 5367
	[NonSerialized]
	public ConstructionGrade defaultGrade;

	// Token: 0x040014F8 RID: 5368
	[NonSerialized]
	public SocketHandle socketHandle;

	// Token: 0x040014F9 RID: 5369
	[NonSerialized]
	public Bounds bounds;

	// Token: 0x040014FA RID: 5370
	[NonSerialized]
	public bool isBuildingPrivilege;

	// Token: 0x040014FB RID: 5371
	[NonSerialized]
	public bool isSleepingBag;

	// Token: 0x040014FC RID: 5372
	[NonSerialized]
	public ConstructionGrade[] grades;

	// Token: 0x040014FD RID: 5373
	[NonSerialized]
	public Deployable deployable;

	// Token: 0x040014FE RID: 5374
	[NonSerialized]
	public ConstructionPlaceholder placeholder;

	// Token: 0x06001C31 RID: 7217 RVA: 0x000C4864 File Offset: 0x000C2A64
	public bool UpdatePlacement(Transform transform, Construction common, ref Construction.Target target)
	{
		if (!target.valid)
		{
			return false;
		}
		if (!common.canBypassBuildingPermission && !target.player.CanBuild())
		{
			Construction.lastPlacementError = "You don't have permission to build here";
			return false;
		}
		List<Socket_Base> list = Pool.GetList<Socket_Base>();
		common.FindMaleSockets(target, list);
		foreach (Socket_Base socket_Base in list)
		{
			Construction.Placement placement = null;
			if (!(target.entity != null) || !(target.socket != null) || !target.entity.IsOccupied(target.socket))
			{
				if (placement == null)
				{
					placement = socket_Base.DoPlacement(target);
				}
				if (placement != null)
				{
					if (!socket_Base.CheckSocketMods(placement))
					{
						transform.position = placement.position;
						transform.rotation = placement.rotation;
					}
					else if (!this.TestPlacingThroughRock(ref placement, target))
					{
						transform.position = placement.position;
						transform.rotation = placement.rotation;
						Construction.lastPlacementError = "Placing through rock";
					}
					else if (!Construction.TestPlacingThroughWall(ref placement, transform, common, target))
					{
						transform.position = placement.position;
						transform.rotation = placement.rotation;
						Construction.lastPlacementError = "Placing through wall";
					}
					else if (!this.TestPlacingCloseToRoad(ref placement, target))
					{
						transform.position = placement.position;
						transform.rotation = placement.rotation;
						Construction.lastPlacementError = "Placing too close to road";
					}
					else if (Vector3.Distance(placement.position, target.player.eyes.position) > common.maxplaceDistance + 1f)
					{
						transform.position = placement.position;
						transform.rotation = placement.rotation;
						Construction.lastPlacementError = "Too far away";
					}
					else
					{
						DeployVolume[] volumes = PrefabAttribute.server.FindAll<DeployVolume>(this.prefabID);
						if (DeployVolume.Check(placement.position, placement.rotation, volumes, -1))
						{
							transform.position = placement.position;
							transform.rotation = placement.rotation;
							Construction.lastPlacementError = "Not enough space";
						}
						else if (BuildingProximity.Check(target.player, this, placement.position, placement.rotation))
						{
							transform.position = placement.position;
							transform.rotation = placement.rotation;
						}
						else if (common.isBuildingPrivilege && !target.player.CanPlaceBuildingPrivilege(placement.position, placement.rotation, common.bounds))
						{
							transform.position = placement.position;
							transform.rotation = placement.rotation;
							Construction.lastPlacementError = "Cannot stack building privileges";
						}
						else
						{
							bool flag = target.player.IsBuildingBlocked(placement.position, placement.rotation, common.bounds);
							if (common.canBypassBuildingPermission || !flag)
							{
								target.inBuildingPrivilege = flag;
								transform.SetPositionAndRotation(placement.position, placement.rotation);
								Pool.FreeList<Socket_Base>(ref list);
								return true;
							}
							transform.position = placement.position;
							transform.rotation = placement.rotation;
							Construction.lastPlacementError = "You don't have permission to build here";
						}
					}
				}
			}
		}
		Pool.FreeList<Socket_Base>(ref list);
		return false;
	}

	// Token: 0x06001C32 RID: 7218 RVA: 0x000C4BB4 File Offset: 0x000C2DB4
	private bool TestPlacingThroughRock(ref Construction.Placement placement, Construction.Target target)
	{
		OBB obb = new OBB(placement.position, Vector3.one, placement.rotation, this.bounds);
		Vector3 center = target.player.GetCenter(true);
		Vector3 origin = target.ray.origin;
		if (Physics.Linecast(center, origin, 65536, QueryTriggerInteraction.Ignore))
		{
			return false;
		}
		RaycastHit raycastHit;
		Vector3 end = obb.Trace(target.ray, out raycastHit, float.PositiveInfinity) ? raycastHit.point : obb.ClosestPoint(origin);
		return !Physics.Linecast(origin, end, 65536, QueryTriggerInteraction.Ignore);
	}

	// Token: 0x06001C33 RID: 7219 RVA: 0x000C4C44 File Offset: 0x000C2E44
	private static bool TestPlacingThroughWall(ref Construction.Placement placement, Transform transform, Construction common, Construction.Target target)
	{
		Vector3 a = placement.position;
		if (common.deployOffset != null)
		{
			a += placement.rotation * common.deployOffset.localPosition;
		}
		Vector3 vector = a - target.ray.origin;
		RaycastHit hit;
		if (!Physics.Raycast(target.ray.origin, vector.normalized, out hit, vector.magnitude, 2097152))
		{
			return true;
		}
		StabilityEntity stabilityEntity = hit.GetEntity() as StabilityEntity;
		if (stabilityEntity != null && target.entity == stabilityEntity)
		{
			return true;
		}
		if (vector.magnitude - hit.distance < 0.2f)
		{
			return true;
		}
		Construction.lastPlacementError = "object in placement path";
		transform.SetPositionAndRotation(hit.point, placement.rotation);
		return false;
	}

	// Token: 0x06001C34 RID: 7220 RVA: 0x000C4D20 File Offset: 0x000C2F20
	private bool TestPlacingCloseToRoad(ref Construction.Placement placement, Construction.Target target)
	{
		TerrainHeightMap heightMap = TerrainMeta.HeightMap;
		TerrainTopologyMap topologyMap = TerrainMeta.TopologyMap;
		if (heightMap == null)
		{
			return true;
		}
		if (topologyMap == null)
		{
			return true;
		}
		OBB obb = new OBB(placement.position, Vector3.one, placement.rotation, this.bounds);
		float num = Mathf.Abs(heightMap.GetHeight(obb.position) - obb.position.y);
		if (num > 9f)
		{
			return true;
		}
		float radius = Mathf.Lerp(3f, 0f, num / 9f);
		Vector3 position = obb.position;
		Vector3 point = obb.GetPoint(-1f, 0f, -1f);
		Vector3 point2 = obb.GetPoint(-1f, 0f, 1f);
		Vector3 point3 = obb.GetPoint(1f, 0f, -1f);
		Vector3 point4 = obb.GetPoint(1f, 0f, 1f);
		int topology = topologyMap.GetTopology(position, radius);
		int topology2 = topologyMap.GetTopology(point, radius);
		int topology3 = topologyMap.GetTopology(point2, radius);
		int topology4 = topologyMap.GetTopology(point3, radius);
		int topology5 = topologyMap.GetTopology(point4, radius);
		return ((topology | topology2 | topology3 | topology4 | topology5) & 526336) == 0;
	}

	// Token: 0x06001C35 RID: 7221 RVA: 0x000C4E6C File Offset: 0x000C306C
	public virtual bool ShowAsNeutral(Construction.Target target)
	{
		return target.inBuildingPrivilege;
	}

	// Token: 0x06001C36 RID: 7222 RVA: 0x000C4E74 File Offset: 0x000C3074
	public BaseEntity CreateConstruction(Construction.Target target, bool bNeedsValidPlacement = false)
	{
		GameObject gameObject = GameManager.server.CreatePrefab(this.fullName, Vector3.zero, Quaternion.identity, false);
		bool flag = this.UpdatePlacement(gameObject.transform, this, ref target);
		BaseEntity baseEntity = gameObject.ToBaseEntity();
		if (bNeedsValidPlacement && !flag)
		{
			if (baseEntity.IsValid())
			{
				baseEntity.Kill(BaseNetworkable.DestroyMode.None);
			}
			else
			{
				GameManager.Destroy(gameObject, 0f);
			}
			return null;
		}
		DecayEntity decayEntity = baseEntity as DecayEntity;
		if (decayEntity)
		{
			decayEntity.AttachToBuilding(target.entity as DecayEntity);
		}
		return baseEntity;
	}

	// Token: 0x06001C37 RID: 7223 RVA: 0x000C4EFC File Offset: 0x000C30FC
	public bool HasMaleSockets(Construction.Target target)
	{
		foreach (Socket_Base socket_Base in this.allSockets)
		{
			if (socket_Base.male && !socket_Base.maleDummy && socket_Base.TestTarget(target))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06001C38 RID: 7224 RVA: 0x000C4F40 File Offset: 0x000C3140
	public void FindMaleSockets(Construction.Target target, List<Socket_Base> sockets)
	{
		foreach (Socket_Base socket_Base in this.allSockets)
		{
			if (socket_Base.male && !socket_Base.maleDummy && socket_Base.TestTarget(target))
			{
				sockets.Add(socket_Base);
			}
		}
	}

	// Token: 0x06001C39 RID: 7225 RVA: 0x000C4F88 File Offset: 0x000C3188
	public ConstructionGrade GetGrade(BuildingGrade.Enum iGrade, ulong iSkin)
	{
		foreach (ConstructionGrade constructionGrade in this.grades)
		{
			if (constructionGrade.gradeBase.type == iGrade && constructionGrade.gradeBase.skin == iSkin)
			{
				return constructionGrade;
			}
		}
		return this.defaultGrade;
	}

	// Token: 0x06001C3A RID: 7226 RVA: 0x000C4FD4 File Offset: 0x000C31D4
	protected override void AttributeSetup(GameObject rootObj, string name, bool serverside, bool clientside, bool bundling)
	{
		base.AttributeSetup(rootObj, name, serverside, clientside, bundling);
		this.isBuildingPrivilege = rootObj.GetComponent<BuildingPrivlidge>();
		this.isSleepingBag = rootObj.GetComponent<SleepingBag>();
		this.bounds = rootObj.GetComponent<BaseEntity>().bounds;
		this.deployable = base.GetComponent<Deployable>();
		this.placeholder = base.GetComponentInChildren<ConstructionPlaceholder>();
		this.allSockets = base.GetComponentsInChildren<Socket_Base>(true);
		this.allProximities = base.GetComponentsInChildren<BuildingProximity>(true);
		this.socketHandle = base.GetComponentsInChildren<SocketHandle>(true).FirstOrDefault<SocketHandle>();
		this.grades = rootObj.GetComponents<ConstructionGrade>();
		foreach (ConstructionGrade constructionGrade in this.grades)
		{
			if (!(constructionGrade == null))
			{
				constructionGrade.construction = this;
				if (!(this.defaultGrade != null))
				{
					this.defaultGrade = constructionGrade;
				}
			}
		}
	}

	// Token: 0x06001C3B RID: 7227 RVA: 0x000C50AF File Offset: 0x000C32AF
	protected override Type GetIndexedType()
	{
		return typeof(Construction);
	}

	// Token: 0x02000C82 RID: 3202
	public struct Target
	{
		// Token: 0x0400438E RID: 17294
		public bool valid;

		// Token: 0x0400438F RID: 17295
		public Ray ray;

		// Token: 0x04004390 RID: 17296
		public BaseEntity entity;

		// Token: 0x04004391 RID: 17297
		public Socket_Base socket;

		// Token: 0x04004392 RID: 17298
		public bool onTerrain;

		// Token: 0x04004393 RID: 17299
		public Vector3 position;

		// Token: 0x04004394 RID: 17300
		public Vector3 normal;

		// Token: 0x04004395 RID: 17301
		public Vector3 rotation;

		// Token: 0x04004396 RID: 17302
		public BasePlayer player;

		// Token: 0x04004397 RID: 17303
		public bool inBuildingPrivilege;

		// Token: 0x06004F0C RID: 20236 RVA: 0x001A5814 File Offset: 0x001A3A14
		public Quaternion GetWorldRotation(bool female)
		{
			Quaternion rhs = this.socket.rotation;
			if (this.socket.male && this.socket.female && female)
			{
				rhs = this.socket.rotation * Quaternion.Euler(180f, 0f, 180f);
			}
			return this.entity.transform.rotation * rhs;
		}

		// Token: 0x06004F0D RID: 20237 RVA: 0x001A5888 File Offset: 0x001A3A88
		public Vector3 GetWorldPosition()
		{
			return this.entity.transform.localToWorldMatrix.MultiplyPoint3x4(this.socket.position);
		}
	}

	// Token: 0x02000C83 RID: 3203
	public class Placement
	{
		// Token: 0x04004398 RID: 17304
		public Vector3 position;

		// Token: 0x04004399 RID: 17305
		public Quaternion rotation;
	}

	// Token: 0x02000C84 RID: 3204
	public class Grade
	{
		// Token: 0x0400439A RID: 17306
		public BuildingGrade grade;

		// Token: 0x0400439B RID: 17307
		public float maxHealth;

		// Token: 0x0400439C RID: 17308
		public List<ItemAmount> costToBuild;

		// Token: 0x17000693 RID: 1683
		// (get) Token: 0x06004F0F RID: 20239 RVA: 0x001A58B8 File Offset: 0x001A3AB8
		public PhysicMaterial physicMaterial
		{
			get
			{
				return this.grade.physicMaterial;
			}
		}

		// Token: 0x17000694 RID: 1684
		// (get) Token: 0x06004F10 RID: 20240 RVA: 0x001A58C5 File Offset: 0x001A3AC5
		public ProtectionProperties damageProtecton
		{
			get
			{
				return this.grade.damageProtecton;
			}
		}
	}
}
