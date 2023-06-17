using System;
using Rust;
using UnityEngine;

// Token: 0x02000488 RID: 1160
public class MLRSRocket : TimedExplosive, SamSite.ISamSiteTarget
{
	// Token: 0x04001E8B RID: 7819
	[SerializeField]
	private GameObjectRef mapMarkerPrefab;

	// Token: 0x04001E8C RID: 7820
	[SerializeField]
	private GameObjectRef launchBlastFXPrefab;

	// Token: 0x04001E8D RID: 7821
	[SerializeField]
	private GameObjectRef explosionGroundFXPrefab;

	// Token: 0x04001E8E RID: 7822
	[SerializeField]
	private ServerProjectile serverProjectile;

	// Token: 0x04001E8F RID: 7823
	private EntityRef mapMarkerInstanceRef;

	// Token: 0x1700031F RID: 799
	// (get) Token: 0x06002633 RID: 9779 RVA: 0x000F0E63 File Offset: 0x000EF063
	public SamSite.SamTargetType SAMTargetType
	{
		get
		{
			return SamSite.targetTypeMissile;
		}
	}

	// Token: 0x06002634 RID: 9780 RVA: 0x000F0E6A File Offset: 0x000EF06A
	public override void ServerInit()
	{
		base.ServerInit();
		this.CreateMapMarker();
		Effect.server.Run(this.launchBlastFXPrefab.resourcePath, base.PivotPoint(), base.transform.up, null, true);
	}

	// Token: 0x06002635 RID: 9781 RVA: 0x000F0E9C File Offset: 0x000EF09C
	public override void ProjectileImpact(RaycastHit info, Vector3 rayOrigin)
	{
		this.Explode(rayOrigin);
		if (Physics.Raycast(info.point + Vector3.up, Vector3.down, 4f, 1218511121, QueryTriggerInteraction.Ignore))
		{
			Effect.server.Run(this.explosionGroundFXPrefab.resourcePath, info.point, Vector3.up, null, true);
		}
	}

	// Token: 0x06002636 RID: 9782 RVA: 0x000F0EF8 File Offset: 0x000EF0F8
	private void CreateMapMarker()
	{
		BaseEntity baseEntity = this.mapMarkerInstanceRef.Get(base.isServer);
		if (baseEntity.IsValid())
		{
			baseEntity.Kill(BaseNetworkable.DestroyMode.None);
		}
		GameManager server = GameManager.server;
		GameObjectRef gameObjectRef = this.mapMarkerPrefab;
		BaseEntity baseEntity2 = server.CreateEntity((gameObjectRef != null) ? gameObjectRef.resourcePath : null, base.transform.position, Quaternion.identity, true);
		baseEntity2.OwnerID = base.OwnerID;
		baseEntity2.Spawn();
		baseEntity2.SetParent(this, true, false);
		this.mapMarkerInstanceRef.Set(baseEntity2);
	}

	// Token: 0x06002637 RID: 9783 RVA: 0x0000441C File Offset: 0x0000261C
	public bool IsValidSAMTarget(bool staticRespawn)
	{
		return true;
	}

	// Token: 0x06002638 RID: 9784 RVA: 0x000F0F7B File Offset: 0x000EF17B
	public override Vector3 GetLocalVelocityServer()
	{
		return this.serverProjectile.CurrentVelocity;
	}

	// Token: 0x06002639 RID: 9785 RVA: 0x000F0F88 File Offset: 0x000EF188
	private void OnTriggerEnter(Collider other)
	{
		if (!other.IsOnLayer(Layer.Trigger))
		{
			return;
		}
		if (other.CompareTag("MLRSRocketTrigger"))
		{
			this.Explode();
			TimedExplosive componentInParent = other.GetComponentInParent<TimedExplosive>();
			if (componentInParent != null)
			{
				componentInParent.Explode();
				return;
			}
		}
		else if (other.GetComponent<TriggerSafeZone>() != null)
		{
			base.Kill(BaseNetworkable.DestroyMode.None);
		}
	}
}
