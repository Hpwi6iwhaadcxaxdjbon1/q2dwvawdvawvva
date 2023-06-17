using System;
using System.Collections.Generic;
using Facepunch;
using Facepunch.Rust;
using Network;
using Rust;
using UnityEngine;

// Token: 0x02000076 RID: 118
public class FlameTurret : StorageContainer
{
	// Token: 0x04000752 RID: 1874
	public static FlameTurret.UpdateFlameTurretWorkQueue updateFlameTurretQueueServer = new FlameTurret.UpdateFlameTurretWorkQueue();

	// Token: 0x04000753 RID: 1875
	public Transform upper;

	// Token: 0x04000754 RID: 1876
	public Vector3 aimDir;

	// Token: 0x04000755 RID: 1877
	public float arc = 45f;

	// Token: 0x04000756 RID: 1878
	public float triggeredDuration = 5f;

	// Token: 0x04000757 RID: 1879
	public float flameRange = 7f;

	// Token: 0x04000758 RID: 1880
	public float flameRadius = 4f;

	// Token: 0x04000759 RID: 1881
	public float fuelPerSec = 1f;

	// Token: 0x0400075A RID: 1882
	public Transform eyeTransform;

	// Token: 0x0400075B RID: 1883
	public List<DamageTypeEntry> damagePerSec;

	// Token: 0x0400075C RID: 1884
	public GameObjectRef triggeredEffect;

	// Token: 0x0400075D RID: 1885
	public GameObjectRef fireballPrefab;

	// Token: 0x0400075E RID: 1886
	public GameObjectRef explosionEffect;

	// Token: 0x0400075F RID: 1887
	public TargetTrigger trigger;

	// Token: 0x04000760 RID: 1888
	private float nextFireballTime;

	// Token: 0x04000761 RID: 1889
	private int turnDir = 1;

	// Token: 0x04000762 RID: 1890
	private float lastMovementUpdate;

	// Token: 0x04000763 RID: 1891
	private float triggeredTime;

	// Token: 0x04000764 RID: 1892
	private float lastServerThink;

	// Token: 0x04000765 RID: 1893
	private float triggerCheckRate = 2f;

	// Token: 0x04000766 RID: 1894
	private float nextTriggerCheckTime;

	// Token: 0x04000767 RID: 1895
	private float pendingFuel;

	// Token: 0x06000B2B RID: 2859 RVA: 0x000647DC File Offset: 0x000629DC
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("FlameTurret.OnRpcMessage", 0))
		{
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000B2C RID: 2860 RVA: 0x000238E0 File Offset: 0x00021AE0
	public bool IsTriggered()
	{
		return base.HasFlag(BaseEntity.Flags.Reserved4);
	}

	// Token: 0x06000B2D RID: 2861 RVA: 0x0006481C File Offset: 0x00062A1C
	public Vector3 GetEyePosition()
	{
		return this.eyeTransform.position;
	}

	// Token: 0x06000B2E RID: 2862 RVA: 0x00064829 File Offset: 0x00062A29
	public override bool CanPickup(BasePlayer player)
	{
		return base.CanPickup(player) && !this.IsTriggered();
	}

	// Token: 0x06000B2F RID: 2863 RVA: 0x0006483F File Offset: 0x00062A3F
	public void SetTriggered(bool triggered)
	{
		if (triggered && this.HasFuel())
		{
			this.triggeredTime = Time.realtimeSinceStartup;
		}
		base.SetFlag(BaseEntity.Flags.Reserved4, triggered && this.HasFuel(), false, true);
	}

	// Token: 0x06000B30 RID: 2864 RVA: 0x00064870 File Offset: 0x00062A70
	public override void ServerInit()
	{
		base.ServerInit();
		base.InvokeRepeating(new Action(this.SendAimDir), 0f, 0.1f);
	}

	// Token: 0x06000B31 RID: 2865 RVA: 0x00064894 File Offset: 0x00062A94
	public void SendAimDir()
	{
		float delta = Time.realtimeSinceStartup - this.lastMovementUpdate;
		this.lastMovementUpdate = Time.realtimeSinceStartup;
		this.MovementUpdate(delta);
		base.ClientRPC<Vector3>(null, "CLIENT_ReceiveAimDir", this.aimDir);
		FlameTurret.updateFlameTurretQueueServer.Add(this);
	}

	// Token: 0x06000B32 RID: 2866 RVA: 0x000648DD File Offset: 0x00062ADD
	public float GetSpinSpeed()
	{
		return (float)(this.IsTriggered() ? 180 : 45);
	}

	// Token: 0x06000B33 RID: 2867 RVA: 0x000648F1 File Offset: 0x00062AF1
	public override void OnAttacked(HitInfo info)
	{
		if (base.isClient)
		{
			return;
		}
		if (info.damageTypes.IsMeleeType())
		{
			this.SetTriggered(true);
		}
		base.OnAttacked(info);
	}

	// Token: 0x06000B34 RID: 2868 RVA: 0x00064918 File Offset: 0x00062B18
	public void MovementUpdate(float delta)
	{
		this.aimDir += new Vector3(0f, delta * this.GetSpinSpeed(), 0f) * (float)this.turnDir;
		if (this.aimDir.y >= this.arc || this.aimDir.y <= -this.arc)
		{
			this.turnDir *= -1;
			this.aimDir.y = Mathf.Clamp(this.aimDir.y, -this.arc, this.arc);
		}
	}

	// Token: 0x06000B35 RID: 2869 RVA: 0x000649B8 File Offset: 0x00062BB8
	public void ServerThink()
	{
		bool flag = this.IsTriggered();
		float delta = Time.realtimeSinceStartup - this.lastServerThink;
		this.lastServerThink = Time.realtimeSinceStartup;
		if (this.IsTriggered() && (Time.realtimeSinceStartup - this.triggeredTime > this.triggeredDuration || !this.HasFuel()))
		{
			this.SetTriggered(false);
		}
		if (!this.IsTriggered() && this.HasFuel() && this.CheckTrigger())
		{
			this.SetTriggered(true);
			Effect.server.Run(this.triggeredEffect.resourcePath, base.transform.position, Vector3.up, null, false);
		}
		if (flag != this.IsTriggered())
		{
			base.SendNetworkUpdateImmediate(false);
		}
		if (this.IsTriggered())
		{
			this.DoFlame(delta);
		}
	}

	// Token: 0x06000B36 RID: 2870 RVA: 0x00064A70 File Offset: 0x00062C70
	public bool CheckTrigger()
	{
		if (Time.realtimeSinceStartup < this.nextTriggerCheckTime)
		{
			return false;
		}
		this.nextTriggerCheckTime = Time.realtimeSinceStartup + 1f / this.triggerCheckRate;
		List<RaycastHit> list = Pool.GetList<RaycastHit>();
		HashSet<BaseEntity> entityContents = this.trigger.entityContents;
		bool flag = false;
		if (entityContents != null)
		{
			foreach (BaseEntity baseEntity in entityContents)
			{
				BasePlayer component = baseEntity.GetComponent<BasePlayer>();
				if (!component.IsSleeping() && component.IsAlive() && component.transform.position.y <= this.GetEyePosition().y + 0.5f && !component.IsBuildingAuthed())
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

	// Token: 0x06000B37 RID: 2871 RVA: 0x00064C10 File Offset: 0x00062E10
	public override void OnKilled(HitInfo info)
	{
		float num = (float)this.GetFuelAmount() / 500f;
		DamageUtil.RadiusDamage(this, base.LookupPrefab(), this.GetEyePosition(), 2f, 6f, this.damagePerSec, 133120, true);
		Effect.server.Run(this.explosionEffect.resourcePath, base.transform.position, Vector3.up, null, false);
		int num2 = Mathf.CeilToInt(Mathf.Clamp(num * 8f, 1f, 8f));
		for (int i = 0; i < num2; i++)
		{
			BaseEntity baseEntity = GameManager.server.CreateEntity(this.fireballPrefab.resourcePath, base.transform.position, base.transform.rotation, true);
			if (baseEntity)
			{
				Vector3 onUnitSphere = UnityEngine.Random.onUnitSphere;
				baseEntity.transform.position = base.transform.position + new Vector3(0f, 1.5f, 0f) + onUnitSphere * UnityEngine.Random.Range(-1f, 1f);
				baseEntity.Spawn();
				baseEntity.SetVelocity(onUnitSphere * (float)UnityEngine.Random.Range(3, 10));
			}
		}
		base.OnKilled(info);
	}

	// Token: 0x06000B38 RID: 2872 RVA: 0x00064D48 File Offset: 0x00062F48
	public int GetFuelAmount()
	{
		Item slot = base.inventory.GetSlot(0);
		if (slot == null || slot.amount < 1)
		{
			return 0;
		}
		return slot.amount;
	}

	// Token: 0x06000B39 RID: 2873 RVA: 0x00064D76 File Offset: 0x00062F76
	public bool HasFuel()
	{
		return this.GetFuelAmount() > 0;
	}

	// Token: 0x06000B3A RID: 2874 RVA: 0x00064D84 File Offset: 0x00062F84
	public bool UseFuel(float seconds)
	{
		Item slot = base.inventory.GetSlot(0);
		if (slot == null || slot.amount < 1)
		{
			return false;
		}
		this.pendingFuel += seconds * this.fuelPerSec;
		if (this.pendingFuel >= 1f)
		{
			int num = Mathf.FloorToInt(this.pendingFuel);
			slot.UseItem(num);
			Analytics.Azure.AddPendingItems(this, slot.info.shortname, num, "flame_turret", true, false);
			this.pendingFuel -= (float)num;
		}
		return true;
	}

	// Token: 0x06000B3B RID: 2875 RVA: 0x00064E0C File Offset: 0x0006300C
	public void DoFlame(float delta)
	{
		if (!this.UseFuel(delta))
		{
			return;
		}
		Ray ray = new Ray(this.GetEyePosition(), base.transform.TransformDirection(Quaternion.Euler(this.aimDir) * Vector3.forward));
		Vector3 origin = ray.origin;
		RaycastHit raycastHit;
		bool flag = Physics.SphereCast(ray, 0.4f, out raycastHit, this.flameRange, 1218652417);
		if (!flag)
		{
			raycastHit.point = origin + ray.direction * this.flameRange;
		}
		float amount = this.damagePerSec[0].amount;
		this.damagePerSec[0].amount = amount * delta;
		DamageUtil.RadiusDamage(this, base.LookupPrefab(), raycastHit.point - ray.direction * 0.1f, this.flameRadius * 0.5f, this.flameRadius, this.damagePerSec, 2230272, true);
		DamageUtil.RadiusDamage(this, base.LookupPrefab(), base.transform.position + new Vector3(0f, 1.25f, 0f), 0.25f, 0.25f, this.damagePerSec, 133120, false);
		this.damagePerSec[0].amount = amount;
		if (Time.realtimeSinceStartup >= this.nextFireballTime)
		{
			this.nextFireballTime = Time.realtimeSinceStartup + UnityEngine.Random.Range(1f, 2f);
			Vector3 a = (UnityEngine.Random.Range(0, 10) <= 7 && flag) ? raycastHit.point : (ray.origin + ray.direction * (flag ? raycastHit.distance : this.flameRange) * UnityEngine.Random.Range(0.4f, 1f));
			BaseEntity baseEntity = GameManager.server.CreateEntity(this.fireballPrefab.resourcePath, a - ray.direction * 0.25f, default(Quaternion), true);
			if (baseEntity)
			{
				baseEntity.creatorEntity = this;
				baseEntity.Spawn();
			}
		}
	}

	// Token: 0x02000BC9 RID: 3017
	public class UpdateFlameTurretWorkQueue : ObjectWorkQueue<FlameTurret>
	{
		// Token: 0x06004D8D RID: 19853 RVA: 0x001A0F63 File Offset: 0x0019F163
		protected override void RunJob(FlameTurret entity)
		{
			if (!this.ShouldAdd(entity))
			{
				return;
			}
			entity.ServerThink();
		}

		// Token: 0x06004D8E RID: 19854 RVA: 0x001A0F75 File Offset: 0x0019F175
		protected override bool ShouldAdd(FlameTurret entity)
		{
			return base.ShouldAdd(entity) && entity.IsValid();
		}
	}
}
