using System;
using System.Collections.Generic;
using Facepunch;
using Network;
using Rust;
using UnityEngine;

// Token: 0x0200007D RID: 125
public class GunTrap : StorageContainer
{
	// Token: 0x040007A8 RID: 1960
	public GameObjectRef gun_fire_effect;

	// Token: 0x040007A9 RID: 1961
	public GameObjectRef bulletEffect;

	// Token: 0x040007AA RID: 1962
	public GameObjectRef triggeredEffect;

	// Token: 0x040007AB RID: 1963
	public Transform muzzlePos;

	// Token: 0x040007AC RID: 1964
	public Transform eyeTransform;

	// Token: 0x040007AD RID: 1965
	public int numPellets = 15;

	// Token: 0x040007AE RID: 1966
	public int aimCone = 30;

	// Token: 0x040007AF RID: 1967
	public float sensorRadius = 1.25f;

	// Token: 0x040007B0 RID: 1968
	public ItemDefinition ammoType;

	// Token: 0x040007B1 RID: 1969
	public TargetTrigger trigger;

	// Token: 0x06000BE5 RID: 3045 RVA: 0x00068B54 File Offset: 0x00066D54
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("GunTrap.OnRpcMessage", 0))
		{
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000BE6 RID: 3046 RVA: 0x00068B94 File Offset: 0x00066D94
	public override string Categorize()
	{
		return "GunTrap";
	}

	// Token: 0x06000BE7 RID: 3047 RVA: 0x00068B9C File Offset: 0x00066D9C
	public bool UseAmmo()
	{
		foreach (Item item in base.inventory.itemList)
		{
			if (item.info == this.ammoType && item.amount > 0)
			{
				item.UseItem(1);
				return true;
			}
		}
		return false;
	}

	// Token: 0x06000BE8 RID: 3048 RVA: 0x00068C18 File Offset: 0x00066E18
	public void FireWeapon()
	{
		if (!this.UseAmmo())
		{
			return;
		}
		Effect.server.Run(this.gun_fire_effect.resourcePath, this, StringPool.Get(this.muzzlePos.gameObject.name), Vector3.zero, Vector3.zero, null, false);
		for (int i = 0; i < this.numPellets; i++)
		{
			this.FireBullet();
		}
	}

	// Token: 0x06000BE9 RID: 3049 RVA: 0x00068C78 File Offset: 0x00066E78
	public void FireBullet()
	{
		float damageAmount = 10f;
		Vector3 vector = this.muzzlePos.transform.position - this.muzzlePos.forward * 0.25f;
		Vector3 forward = this.muzzlePos.transform.forward;
		Vector3 modifiedAimConeDirection = AimConeUtil.GetModifiedAimConeDirection((float)this.aimCone, forward, true);
		Vector3 arg = vector + modifiedAimConeDirection * 300f;
		base.ClientRPC<Vector3>(null, "CLIENT_FireGun", arg);
		List<RaycastHit> list = Pool.GetList<RaycastHit>();
		int layerMask = 1219701505;
		GamePhysics.TraceAll(new Ray(vector, modifiedAimConeDirection), 0.1f, list, 300f, layerMask, QueryTriggerInteraction.UseGlobal, null);
		for (int i = 0; i < list.Count; i++)
		{
			RaycastHit hit = list[i];
			BaseEntity entity = hit.GetEntity();
			if (!(entity != null) || (!(entity == this) && !entity.EqualNetID(this)))
			{
				if (entity as BaseCombatEntity != null)
				{
					HitInfo info = new HitInfo(this, entity, DamageType.Bullet, damageAmount, hit.point);
					entity.OnAttacked(info);
					if (entity is BasePlayer || entity is BaseNpc)
					{
						Effect.server.ImpactEffect(new HitInfo
						{
							HitPositionWorld = hit.point,
							HitNormalWorld = -hit.normal,
							HitMaterial = StringPool.Get("Flesh")
						});
					}
				}
				if (!(entity != null) || entity.ShouldBlockProjectiles())
				{
					arg = hit.point;
					return;
				}
			}
		}
	}

	// Token: 0x06000BEA RID: 3050 RVA: 0x00068E04 File Offset: 0x00067004
	public override void ServerInit()
	{
		base.ServerInit();
		base.InvokeRandomized(new Action(this.TriggerCheck), UnityEngine.Random.Range(0f, 1f), 0.5f, 0.1f);
	}

	// Token: 0x06000BEB RID: 3051 RVA: 0x00068E37 File Offset: 0x00067037
	public void TriggerCheck()
	{
		if (this.CheckTrigger())
		{
			this.FireWeapon();
		}
	}

	// Token: 0x06000BEC RID: 3052 RVA: 0x00068E48 File Offset: 0x00067048
	public bool CheckTrigger()
	{
		List<RaycastHit> list = Pool.GetList<RaycastHit>();
		HashSet<BaseEntity> entityContents = this.trigger.entityContents;
		bool flag = false;
		if (entityContents != null)
		{
			foreach (BaseEntity baseEntity in entityContents)
			{
				BasePlayer component = baseEntity.GetComponent<BasePlayer>();
				if (!component.IsSleeping() && component.IsAlive() && !component.IsBuildingAuthed())
				{
					list.Clear();
					GamePhysics.TraceAll(new Ray(component.eyes.position, (this.GetEyePosition() - component.eyes.position).normalized), 0f, list, 9f, 1218519297, QueryTriggerInteraction.UseGlobal, null);
					for (int i = 0; i < list.Count; i++)
					{
						BaseEntity entity = list[i].GetEntity();
						if (entity != null && (entity == this || entity.EqualNetID(this)))
						{
							flag = true;
							break;
						}
						if (!(entity != null) || entity.ShouldBlockProjectiles())
						{
							break;
						}
					}
					if (flag)
					{
						break;
					}
				}
			}
		}
		Pool.FreeList<RaycastHit>(ref list);
		return flag;
	}

	// Token: 0x06000BED RID: 3053 RVA: 0x000231B4 File Offset: 0x000213B4
	public bool IsTriggered()
	{
		return base.HasFlag(BaseEntity.Flags.Reserved1);
	}

	// Token: 0x06000BEE RID: 3054 RVA: 0x00068F90 File Offset: 0x00067190
	public Vector3 GetEyePosition()
	{
		return this.eyeTransform.position;
	}

	// Token: 0x02000BCE RID: 3022
	public static class GunTrapFlags
	{
		// Token: 0x040040E5 RID: 16613
		public const BaseEntity.Flags Triggered = BaseEntity.Flags.Reserved1;
	}
}
