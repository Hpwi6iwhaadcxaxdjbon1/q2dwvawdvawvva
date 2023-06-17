using System;
using UnityEngine;

// Token: 0x0200056B RID: 1387
public abstract class BaseSpawnPoint : MonoBehaviour, IServerComponent
{
	// Token: 0x06002A8B RID: 10891
	public abstract void GetLocation(out Vector3 pos, out Quaternion rot);

	// Token: 0x06002A8C RID: 10892
	public abstract void ObjectSpawned(SpawnPointInstance instance);

	// Token: 0x06002A8D RID: 10893
	public abstract void ObjectRetired(SpawnPointInstance instance);

	// Token: 0x06002A8E RID: 10894 RVA: 0x00103754 File Offset: 0x00101954
	public virtual bool IsAvailableTo(GameObjectRef prefabRef)
	{
		return base.gameObject.activeSelf;
	}

	// Token: 0x06002A8F RID: 10895 RVA: 0x00103761 File Offset: 0x00101961
	public virtual bool HasPlayersIntersecting()
	{
		return BaseNetworkable.HasCloseConnections(base.transform.position, 2f);
	}

	// Token: 0x06002A90 RID: 10896 RVA: 0x00103778 File Offset: 0x00101978
	protected void DropToGround(ref Vector3 pos, ref Quaternion rot)
	{
		if (TerrainMeta.HeightMap && TerrainMeta.Collision && !TerrainMeta.Collision.GetIgnore(pos, 0.01f))
		{
			float height = TerrainMeta.HeightMap.GetHeight(pos);
			pos.y = Mathf.Max(pos.y, height);
		}
		RaycastHit raycastHit;
		if (TransformUtil.GetGroundInfo(pos, out raycastHit, 20f, 1235288065, null))
		{
			pos = raycastHit.point;
			rot = Quaternion.LookRotation(rot * Vector3.forward, raycastHit.normal);
		}
	}
}
