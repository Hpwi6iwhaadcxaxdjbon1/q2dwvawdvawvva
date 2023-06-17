using System;
using System.Collections.Generic;
using Facepunch;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x020004F4 RID: 1268
public abstract class DeployVolume : PrefabAttribute
{
	// Token: 0x040020FE RID: 8446
	public LayerMask layers = 537001984;

	// Token: 0x040020FF RID: 8447
	[global::InspectorFlags]
	public ColliderInfo.Flags ignore;

	// Token: 0x04002100 RID: 8448
	public DeployVolume.EntityMode entityMode;

	// Token: 0x04002101 RID: 8449
	[FormerlySerializedAs("entities")]
	public BaseEntity[] entityList;

	// Token: 0x04002102 RID: 8450
	[SerializeField]
	public EntityListScriptableObject[] entityGroups;

	// Token: 0x1700037D RID: 893
	// (get) Token: 0x060028F2 RID: 10482 RVA: 0x000FC307 File Offset: 0x000FA507
	// (set) Token: 0x060028F3 RID: 10483 RVA: 0x000FC30F File Offset: 0x000FA50F
	public bool IsBuildingBlock { get; set; }

	// Token: 0x060028F4 RID: 10484 RVA: 0x000FC318 File Offset: 0x000FA518
	protected override Type GetIndexedType()
	{
		return typeof(DeployVolume);
	}

	// Token: 0x060028F5 RID: 10485 RVA: 0x000FC324 File Offset: 0x000FA524
	public override void PreProcess(IPrefabProcessor preProcess, GameObject rootObj, string name, bool serverside, bool clientside, bool bundling)
	{
		base.PreProcess(preProcess, rootObj, name, serverside, clientside, bundling);
		this.IsBuildingBlock = (rootObj.GetComponent<BuildingBlock>() != null);
	}

	// Token: 0x060028F6 RID: 10486
	protected abstract bool Check(Vector3 position, Quaternion rotation, int mask = -1);

	// Token: 0x060028F7 RID: 10487
	protected abstract bool Check(Vector3 position, Quaternion rotation, OBB test, int mask = -1);

	// Token: 0x060028F8 RID: 10488 RVA: 0x000FC348 File Offset: 0x000FA548
	public static bool Check(Vector3 position, Quaternion rotation, DeployVolume[] volumes, int mask = -1)
	{
		for (int i = 0; i < volumes.Length; i++)
		{
			if (volumes[i].Check(position, rotation, mask))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x060028F9 RID: 10489 RVA: 0x000FC374 File Offset: 0x000FA574
	public static bool Check(Vector3 position, Quaternion rotation, DeployVolume[] volumes, OBB test, int mask = -1)
	{
		for (int i = 0; i < volumes.Length; i++)
		{
			if (volumes[i].Check(position, rotation, test, mask))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x060028FA RID: 10490 RVA: 0x000FC3A4 File Offset: 0x000FA5A4
	public static bool CheckSphere(Vector3 pos, float radius, int layerMask, DeployVolume volume)
	{
		List<Collider> list = Pool.GetList<Collider>();
		GamePhysics.OverlapSphere(pos, radius, list, layerMask, QueryTriggerInteraction.Collide);
		bool result = DeployVolume.CheckFlags(list, volume);
		Pool.FreeList<Collider>(ref list);
		return result;
	}

	// Token: 0x060028FB RID: 10491 RVA: 0x000FC3D0 File Offset: 0x000FA5D0
	public static bool CheckCapsule(Vector3 start, Vector3 end, float radius, int layerMask, DeployVolume volume)
	{
		List<Collider> list = Pool.GetList<Collider>();
		GamePhysics.OverlapCapsule(start, end, radius, list, layerMask, QueryTriggerInteraction.Collide);
		bool result = DeployVolume.CheckFlags(list, volume);
		Pool.FreeList<Collider>(ref list);
		return result;
	}

	// Token: 0x060028FC RID: 10492 RVA: 0x000FC400 File Offset: 0x000FA600
	public static bool CheckOBB(OBB obb, int layerMask, DeployVolume volume)
	{
		List<Collider> list = Pool.GetList<Collider>();
		GamePhysics.OverlapOBB(obb, list, layerMask, QueryTriggerInteraction.Collide);
		bool result = DeployVolume.CheckFlags(list, volume);
		Pool.FreeList<Collider>(ref list);
		return result;
	}

	// Token: 0x060028FD RID: 10493 RVA: 0x000FC42C File Offset: 0x000FA62C
	public static bool CheckBounds(Bounds bounds, int layerMask, DeployVolume volume)
	{
		List<Collider> list = Pool.GetList<Collider>();
		GamePhysics.OverlapBounds(bounds, list, layerMask, QueryTriggerInteraction.Collide);
		bool result = DeployVolume.CheckFlags(list, volume);
		Pool.FreeList<Collider>(ref list);
		return result;
	}

	// Token: 0x1700037E RID: 894
	// (get) Token: 0x060028FE RID: 10494 RVA: 0x000FC456 File Offset: 0x000FA656
	// (set) Token: 0x060028FF RID: 10495 RVA: 0x000FC45D File Offset: 0x000FA65D
	public static Collider LastDeployHit { get; private set; }

	// Token: 0x06002900 RID: 10496 RVA: 0x000FC468 File Offset: 0x000FA668
	private static bool CheckFlags(List<Collider> list, DeployVolume volume)
	{
		DeployVolume.LastDeployHit = null;
		for (int i = 0; i < list.Count; i++)
		{
			DeployVolume.LastDeployHit = list[i];
			GameObject gameObject = list[i].gameObject;
			if (!gameObject.CompareTag("DeployVolumeIgnore"))
			{
				ColliderInfo component = gameObject.GetComponent<ColliderInfo>();
				if ((!(component != null) || !component.HasFlag(ColliderInfo.Flags.OnlyBlockBuildingBlock) || volume.IsBuildingBlock) && (component == null || volume.ignore == (ColliderInfo.Flags)0 || !component.HasFlag(volume.ignore)))
				{
					if (volume.entityList.Length == 0 && volume.entityGroups.Length == 0)
					{
						return true;
					}
					BaseEntity entity = list[i].ToBaseEntity();
					if (volume.entityGroups != null)
					{
						foreach (EntityListScriptableObject entityListScriptableObject in volume.entityGroups)
						{
							if (entityListScriptableObject.entities == null || entityListScriptableObject.entities.Length == 0)
							{
								Debug.LogWarning("Skipping entity group '" + entityListScriptableObject.name + "' when checking volume: there are no entities");
							}
							else if (!DeployVolume.CheckEntityList(entity, entityListScriptableObject.entities, entityListScriptableObject.whitelist))
							{
								return false;
							}
						}
					}
					return DeployVolume.CheckEntityList(entity, volume.entityList, volume.entityMode == DeployVolume.EntityMode.IncludeList);
				}
			}
		}
		return false;
	}

	// Token: 0x06002901 RID: 10497 RVA: 0x000FC5C0 File Offset: 0x000FA7C0
	private static bool CheckEntityList(BaseEntity entity, BaseEntity[] entities, bool whitelist)
	{
		if (entities == null || entities.Length == 0)
		{
			return true;
		}
		bool flag = false;
		if (entity != null)
		{
			foreach (BaseEntity baseEntity in entities)
			{
				if (entity.prefabID == baseEntity.prefabID)
				{
					flag = true;
					break;
				}
			}
		}
		if (whitelist)
		{
			return flag;
		}
		return !flag;
	}

	// Token: 0x02000D30 RID: 3376
	public enum EntityMode
	{
		// Token: 0x04004678 RID: 18040
		ExcludeList,
		// Token: 0x04004679 RID: 18041
		IncludeList
	}
}
