using System;
using System.Collections;
using ConVar;
using UnityEngine;
using UnityEngine.AI;

// Token: 0x020001F9 RID: 505
public class NPCPlayer : BasePlayer
{
	// Token: 0x040012BC RID: 4796
	public AIInformationZone VirtualInfoZone;

	// Token: 0x040012BD RID: 4797
	public Vector3 finalDestination;

	// Token: 0x040012BE RID: 4798
	[NonSerialized]
	private float randomOffset;

	// Token: 0x040012BF RID: 4799
	[NonSerialized]
	private Vector3 spawnPos;

	// Token: 0x040012C0 RID: 4800
	public PlayerInventoryProperties[] loadouts;

	// Token: 0x040012C1 RID: 4801
	public LayerMask movementMask = 429990145;

	// Token: 0x040012C2 RID: 4802
	public bool LegacyNavigation = true;

	// Token: 0x040012C3 RID: 4803
	public NavMeshAgent NavAgent;

	// Token: 0x040012C4 RID: 4804
	public float damageScale = 1f;

	// Token: 0x040012C5 RID: 4805
	public float shortRange = 10f;

	// Token: 0x040012C6 RID: 4806
	public float attackLengthMaxShortRangeScale = 1f;

	// Token: 0x040012C7 RID: 4807
	private bool _isDormant;

	// Token: 0x040012C8 RID: 4808
	protected float lastGunShotTime;

	// Token: 0x040012C9 RID: 4809
	private float triggerEndTime;

	// Token: 0x040012CA RID: 4810
	protected float nextTriggerTime;

	// Token: 0x040012CB RID: 4811
	private float lastThinkTime;

	// Token: 0x040012CC RID: 4812
	private float lastPositionUpdateTime;

	// Token: 0x040012CD RID: 4813
	private float lastMovementTickTime;

	// Token: 0x040012CE RID: 4814
	private Vector3 lastPos;

	// Token: 0x040012CF RID: 4815
	private float lastThrowTime;

	// Token: 0x17000233 RID: 563
	// (get) Token: 0x06001A5C RID: 6748 RVA: 0x0000441C File Offset: 0x0000261C
	public override bool IsNpc
	{
		get
		{
			return true;
		}
	}

	// Token: 0x17000234 RID: 564
	// (get) Token: 0x06001A5D RID: 6749 RVA: 0x000BE3BE File Offset: 0x000BC5BE
	// (set) Token: 0x06001A5E RID: 6750 RVA: 0x000BE3C6 File Offset: 0x000BC5C6
	public virtual bool IsDormant
	{
		get
		{
			return this._isDormant;
		}
		set
		{
			this._isDormant = value;
			bool isDormant = this._isDormant;
		}
	}

	// Token: 0x17000235 RID: 565
	// (get) Token: 0x06001A5F RID: 6751 RVA: 0x0002BF84 File Offset: 0x0002A184
	protected override float PositionTickRate
	{
		get
		{
			return 0.1f;
		}
	}

	// Token: 0x06001A60 RID: 6752 RVA: 0x00007A3C File Offset: 0x00005C3C
	public virtual bool IsLoadBalanced()
	{
		return false;
	}

	// Token: 0x06001A61 RID: 6753 RVA: 0x000BE3D8 File Offset: 0x000BC5D8
	public override void ServerInit()
	{
		if (base.isClient)
		{
			return;
		}
		this.spawnPos = this.GetPosition();
		this.randomOffset = UnityEngine.Random.Range(0f, 1f);
		base.ServerInit();
		this.UpdateNetworkGroup();
		this.EquipLoadout(this.loadouts);
		if (!this.IsLoadBalanced())
		{
			base.InvokeRepeating(new Action(this.ServerThink_Internal), 0f, 0.1f);
			this.lastThinkTime = UnityEngine.Time.time;
		}
		base.Invoke(new Action(this.EquipTest), 0.25f);
		this.finalDestination = base.transform.position;
		if (this.NavAgent == null)
		{
			this.NavAgent = base.GetComponent<NavMeshAgent>();
		}
		if (this.NavAgent)
		{
			this.NavAgent.updateRotation = false;
			this.NavAgent.updatePosition = false;
			if (!this.LegacyNavigation)
			{
				base.transform.gameObject.GetComponent<BaseNavigator>().Init(this, this.NavAgent);
			}
		}
		base.InvokeRandomized(new Action(this.TickMovement), 1f, this.PositionTickRate, this.PositionTickRate * 0.1f);
	}

	// Token: 0x06001A62 RID: 6754 RVA: 0x000BE50A File Offset: 0x000BC70A
	public void EquipLoadout(PlayerInventoryProperties[] loads)
	{
		if (loads == null)
		{
			return;
		}
		if (loads.Length == 0)
		{
			return;
		}
		loads[UnityEngine.Random.Range(0, loads.Length)].GiveToPlayer(this);
	}

	// Token: 0x06001A63 RID: 6755 RVA: 0x00035504 File Offset: 0x00033704
	public override void ApplyInheritedVelocity(Vector3 velocity)
	{
		this.ServerPosition = BaseNpc.GetNewNavPosWithVelocity(this, velocity);
	}

	// Token: 0x06001A64 RID: 6756 RVA: 0x000BE528 File Offset: 0x000BC728
	public void RandomMove()
	{
		float d = 8f;
		Vector2 vector = UnityEngine.Random.insideUnitCircle * d;
		this.SetDestination(this.spawnPos + new Vector3(vector.x, 0f, vector.y));
	}

	// Token: 0x06001A65 RID: 6757 RVA: 0x000BE56E File Offset: 0x000BC76E
	public virtual void SetDestination(Vector3 newDestination)
	{
		this.finalDestination = newDestination;
	}

	// Token: 0x06001A66 RID: 6758 RVA: 0x000BE577 File Offset: 0x000BC777
	public AttackEntity GetAttackEntity()
	{
		return base.GetHeldEntity() as AttackEntity;
	}

	// Token: 0x06001A67 RID: 6759 RVA: 0x000BE584 File Offset: 0x000BC784
	public BaseProjectile GetGun()
	{
		AttackEntity attackEntity = base.GetHeldEntity() as AttackEntity;
		if (attackEntity == null)
		{
			return null;
		}
		BaseProjectile baseProjectile = attackEntity as BaseProjectile;
		if (baseProjectile)
		{
			return baseProjectile;
		}
		return null;
	}

	// Token: 0x06001A68 RID: 6760 RVA: 0x000BE5BC File Offset: 0x000BC7BC
	public virtual float AmmoFractionRemaining()
	{
		AttackEntity attackEntity = this.GetAttackEntity();
		if (attackEntity)
		{
			return attackEntity.AmmoFraction();
		}
		return 0f;
	}

	// Token: 0x06001A69 RID: 6761 RVA: 0x000BE5E4 File Offset: 0x000BC7E4
	public virtual bool IsReloading()
	{
		AttackEntity attackEntity = this.GetAttackEntity();
		return attackEntity && attackEntity.ServerIsReloading();
	}

	// Token: 0x06001A6A RID: 6762 RVA: 0x000BE608 File Offset: 0x000BC808
	public virtual void AttemptReload()
	{
		AttackEntity attackEntity = this.GetAttackEntity();
		if (attackEntity == null)
		{
			return;
		}
		if (attackEntity.CanReload())
		{
			attackEntity.ServerReload();
		}
	}

	// Token: 0x06001A6B RID: 6763 RVA: 0x000BE634 File Offset: 0x000BC834
	public virtual bool ShotTest(float targetDist)
	{
		AttackEntity attackEntity = base.GetHeldEntity() as AttackEntity;
		if (attackEntity == null)
		{
			return false;
		}
		BaseProjectile baseProjectile = attackEntity as BaseProjectile;
		if (baseProjectile)
		{
			if (baseProjectile.primaryMagazine.contents <= 0)
			{
				baseProjectile.ServerReload();
				return false;
			}
			if (baseProjectile.NextAttackTime > UnityEngine.Time.time)
			{
				return false;
			}
		}
		if (Mathf.Approximately(attackEntity.attackLengthMin, -1f))
		{
			attackEntity.ServerUse(this.damageScale, null);
			this.lastGunShotTime = UnityEngine.Time.time;
			return true;
		}
		if (base.IsInvoking(new Action(this.TriggerDown)))
		{
			return true;
		}
		if (UnityEngine.Time.time < this.nextTriggerTime)
		{
			return true;
		}
		base.InvokeRepeating(new Action(this.TriggerDown), 0f, 0.01f);
		if (targetDist <= this.shortRange)
		{
			this.triggerEndTime = UnityEngine.Time.time + UnityEngine.Random.Range(attackEntity.attackLengthMin, attackEntity.attackLengthMax * this.attackLengthMaxShortRangeScale);
		}
		else
		{
			this.triggerEndTime = UnityEngine.Time.time + UnityEngine.Random.Range(attackEntity.attackLengthMin, attackEntity.attackLengthMax);
		}
		this.TriggerDown();
		return true;
	}

	// Token: 0x06001A6C RID: 6764 RVA: 0x00006CA5 File Offset: 0x00004EA5
	public virtual float GetAimConeScale()
	{
		return 1f;
	}

	// Token: 0x06001A6D RID: 6765 RVA: 0x000BE751 File Offset: 0x000BC951
	public void CancelBurst(float delay = 0.2f)
	{
		if (this.triggerEndTime > UnityEngine.Time.time + delay)
		{
			this.triggerEndTime = UnityEngine.Time.time + delay;
		}
	}

	// Token: 0x06001A6E RID: 6766 RVA: 0x000BE770 File Offset: 0x000BC970
	public bool MeleeAttack()
	{
		AttackEntity attackEntity = base.GetHeldEntity() as AttackEntity;
		if (attackEntity == null)
		{
			return false;
		}
		BaseMelee baseMelee = attackEntity as BaseMelee;
		if (baseMelee == null)
		{
			return false;
		}
		baseMelee.ServerUse(this.damageScale, null);
		return true;
	}

	// Token: 0x06001A6F RID: 6767 RVA: 0x000BE7B4 File Offset: 0x000BC9B4
	public virtual void TriggerDown()
	{
		AttackEntity attackEntity = base.GetHeldEntity() as AttackEntity;
		if (attackEntity != null)
		{
			attackEntity.ServerUse(this.damageScale, null);
		}
		this.lastGunShotTime = UnityEngine.Time.time;
		if (UnityEngine.Time.time > this.triggerEndTime)
		{
			base.CancelInvoke(new Action(this.TriggerDown));
			this.nextTriggerTime = UnityEngine.Time.time + ((attackEntity != null) ? attackEntity.attackSpacing : 1f);
		}
	}

	// Token: 0x06001A70 RID: 6768 RVA: 0x000BE830 File Offset: 0x000BCA30
	public virtual void EquipWeapon(bool skipDeployDelay = false)
	{
		if (this.inventory == null || this.inventory.containerBelt == null)
		{
			return;
		}
		Item slot = this.inventory.containerBelt.GetSlot(0);
		if (slot != null)
		{
			base.UpdateActiveItem(this.inventory.containerBelt.GetSlot(0).uid);
			BaseEntity heldEntity = slot.GetHeldEntity();
			if (heldEntity != null)
			{
				AttackEntity component = heldEntity.GetComponent<AttackEntity>();
				if (component != null)
				{
					if (skipDeployDelay)
					{
						component.ResetAttackCooldown();
					}
					component.TopUpAmmo();
				}
			}
		}
	}

	// Token: 0x06001A71 RID: 6769 RVA: 0x000BE8BB File Offset: 0x000BCABB
	public void EquipTest()
	{
		this.EquipWeapon(true);
	}

	// Token: 0x06001A72 RID: 6770 RVA: 0x000BE8C4 File Offset: 0x000BCAC4
	internal void ServerThink_Internal()
	{
		float delta = UnityEngine.Time.time - this.lastThinkTime;
		this.ServerThink(delta);
		this.lastThinkTime = UnityEngine.Time.time;
	}

	// Token: 0x06001A73 RID: 6771 RVA: 0x000BE8F0 File Offset: 0x000BCAF0
	public virtual void ServerThink(float delta)
	{
		this.TickAi(delta);
	}

	// Token: 0x06001A74 RID: 6772 RVA: 0x000063A5 File Offset: 0x000045A5
	public virtual void Resume()
	{
	}

	// Token: 0x06001A75 RID: 6773 RVA: 0x00007A3C File Offset: 0x00005C3C
	public virtual bool IsNavRunning()
	{
		return false;
	}

	// Token: 0x17000236 RID: 566
	// (get) Token: 0x06001A76 RID: 6774 RVA: 0x000BE8F9 File Offset: 0x000BCAF9
	public virtual bool IsOnNavMeshLink
	{
		get
		{
			return this.IsNavRunning() && this.NavAgent.isOnOffMeshLink;
		}
	}

	// Token: 0x17000237 RID: 567
	// (get) Token: 0x06001A77 RID: 6775 RVA: 0x000BE910 File Offset: 0x000BCB10
	public virtual bool HasPath
	{
		get
		{
			return this.IsNavRunning() && this.NavAgent.hasPath;
		}
	}

	// Token: 0x06001A78 RID: 6776 RVA: 0x000063A5 File Offset: 0x000045A5
	public virtual void TickAi(float delta)
	{
	}

	// Token: 0x06001A79 RID: 6777 RVA: 0x000BE928 File Offset: 0x000BCB28
	public void TickMovement()
	{
		float delta = UnityEngine.Time.realtimeSinceStartup - this.lastMovementTickTime;
		this.lastMovementTickTime = UnityEngine.Time.realtimeSinceStartup;
		this.MovementUpdate(delta);
	}

	// Token: 0x06001A7A RID: 6778 RVA: 0x000BE954 File Offset: 0x000BCB54
	public override float GetNetworkTime()
	{
		if (UnityEngine.Time.realtimeSinceStartup - this.lastPositionUpdateTime > this.PositionTickRate * 2f)
		{
			return UnityEngine.Time.time;
		}
		return this.lastPositionUpdateTime;
	}

	// Token: 0x06001A7B RID: 6779 RVA: 0x000BE97C File Offset: 0x000BCB7C
	public virtual void MovementUpdate(float delta)
	{
		if (!this.LegacyNavigation)
		{
			return;
		}
		if (base.isClient)
		{
			return;
		}
		if (!this.IsAlive() || base.IsWounded() || (!base.isMounted && !this.IsNavRunning()))
		{
			return;
		}
		if (this.IsDormant || !this.syncPosition)
		{
			if (this.IsNavRunning())
			{
				this.NavAgent.destination = this.ServerPosition;
			}
			return;
		}
		Vector3 moveToPosition = base.transform.position;
		if (this.HasPath)
		{
			moveToPosition = this.NavAgent.nextPosition;
		}
		if (!this.ValidateNextPosition(ref moveToPosition))
		{
			return;
		}
		this.UpdateSpeed(delta);
		this.UpdatePositionAndRotation(moveToPosition);
	}

	// Token: 0x06001A7C RID: 6780 RVA: 0x000BEA20 File Offset: 0x000BCC20
	private bool ValidateNextPosition(ref Vector3 moveToPosition)
	{
		if (!ValidBounds.Test(moveToPosition) && base.transform != null && !base.IsDestroyed)
		{
			Debug.Log(string.Concat(new object[]
			{
				"Invalid NavAgent Position: ",
				this,
				" ",
				moveToPosition.ToString(),
				" (destroying)"
			}));
			base.Kill(BaseNetworkable.DestroyMode.None);
			return false;
		}
		return true;
	}

	// Token: 0x06001A7D RID: 6781 RVA: 0x000BEA98 File Offset: 0x000BCC98
	private void UpdateSpeed(float delta)
	{
		float b = this.DesiredMoveSpeed();
		this.NavAgent.speed = Mathf.Lerp(this.NavAgent.speed, b, delta * 8f);
	}

	// Token: 0x06001A7E RID: 6782 RVA: 0x000BEACF File Offset: 0x000BCCCF
	protected virtual void UpdatePositionAndRotation(Vector3 moveToPosition)
	{
		this.lastPositionUpdateTime = UnityEngine.Time.time;
		this.ServerPosition = moveToPosition;
		this.SetAimDirection(this.GetAimDirection());
	}

	// Token: 0x06001A7F RID: 6783 RVA: 0x0002C673 File Offset: 0x0002A873
	public Vector3 GetPosition()
	{
		return base.transform.position;
	}

	// Token: 0x06001A80 RID: 6784 RVA: 0x000BEAF0 File Offset: 0x000BCCF0
	public virtual float DesiredMoveSpeed()
	{
		float running = Mathf.Sin(UnityEngine.Time.time + this.randomOffset);
		return base.GetSpeed(running, 0f, 0f);
	}

	// Token: 0x06001A81 RID: 6785 RVA: 0x00007A3C File Offset: 0x00005C3C
	public override bool EligibleForWounding(HitInfo info)
	{
		return false;
	}

	// Token: 0x06001A82 RID: 6786 RVA: 0x000BEB20 File Offset: 0x000BCD20
	public virtual Vector3 GetAimDirection()
	{
		if (Vector3Ex.Distance2D(this.finalDestination, this.GetPosition()) >= 1f)
		{
			Vector3 normalized = (this.finalDestination - this.GetPosition()).normalized;
			return new Vector3(normalized.x, 0f, normalized.z);
		}
		return this.eyes.BodyForward();
	}

	// Token: 0x06001A83 RID: 6787 RVA: 0x000BEB84 File Offset: 0x000BCD84
	public virtual void SetAimDirection(Vector3 newAim)
	{
		if (newAim == Vector3.zero)
		{
			return;
		}
		AttackEntity attackEntity = this.GetAttackEntity();
		if (attackEntity)
		{
			newAim = attackEntity.ModifyAIAim(newAim, 1f);
		}
		this.eyes.rotation = Quaternion.LookRotation(newAim, Vector3.up);
		this.viewAngles = this.eyes.rotation.eulerAngles;
		this.ServerRotation = this.eyes.rotation;
		this.lastPositionUpdateTime = UnityEngine.Time.time;
	}

	// Token: 0x06001A84 RID: 6788 RVA: 0x000BEC08 File Offset: 0x000BCE08
	public bool TryUseThrownWeapon(BaseEntity target, float attackRate)
	{
		if (this.HasThrownItemCooldown())
		{
			return false;
		}
		Item item = this.FindThrownWeapon();
		if (item == null)
		{
			this.lastThrowTime = UnityEngine.Time.time;
			return false;
		}
		return this.TryUseThrownWeapon(item, target, attackRate);
	}

	// Token: 0x06001A85 RID: 6789 RVA: 0x000BEC40 File Offset: 0x000BCE40
	public bool TryUseThrownWeapon(Item item, BaseEntity target, float attackRate)
	{
		if (this.HasThrownItemCooldown())
		{
			return false;
		}
		float num = Vector3.Distance(target.transform.position, base.transform.position);
		if (num <= 2f || num >= 20f)
		{
			return false;
		}
		Vector3 position = target.transform.position;
		if (!base.IsVisible(base.CenterPoint(), position, float.PositiveInfinity))
		{
			return false;
		}
		if (this.UseThrownWeapon(item, target))
		{
			if (this is ScarecrowNPC)
			{
				ScarecrowNPC.NextBeanCanAllowedTime = UnityEngine.Time.time + Halloween.scarecrow_throw_beancan_global_delay;
			}
			this.lastThrowTime = UnityEngine.Time.time;
			return true;
		}
		return false;
	}

	// Token: 0x06001A86 RID: 6790 RVA: 0x000BECD9 File Offset: 0x000BCED9
	public bool HasThrownItemCooldown()
	{
		return UnityEngine.Time.time - this.lastThrowTime < 10f;
	}

	// Token: 0x06001A87 RID: 6791 RVA: 0x000BECF0 File Offset: 0x000BCEF0
	protected bool UseThrownWeapon(Item item, BaseEntity target)
	{
		base.UpdateActiveItem(item.uid);
		ThrownWeapon thrownWeapon = base.GetActiveItem().GetHeldEntity() as ThrownWeapon;
		if (thrownWeapon == null)
		{
			return false;
		}
		base.StartCoroutine(this.DoThrow(thrownWeapon, target));
		return true;
	}

	// Token: 0x06001A88 RID: 6792 RVA: 0x000BED35 File Offset: 0x000BCF35
	private IEnumerator DoThrow(ThrownWeapon thrownWeapon, BaseEntity target)
	{
		this.modelState.aiming = true;
		yield return new WaitForSeconds(1.5f);
		this.SetAimDirection(Vector3Ex.Direction(target.transform.position, base.transform.position));
		thrownWeapon.ResetAttackCooldown();
		thrownWeapon.ServerThrow(target.transform.position);
		this.modelState.aiming = false;
		base.Invoke(new Action(this.EquipTest), 0.5f);
		yield break;
	}

	// Token: 0x06001A89 RID: 6793 RVA: 0x000BED54 File Offset: 0x000BCF54
	public Item FindThrownWeapon()
	{
		if (this.inventory == null || this.inventory.containerBelt == null)
		{
			return null;
		}
		for (int i = 0; i < this.inventory.containerBelt.capacity; i++)
		{
			Item slot = this.inventory.containerBelt.GetSlot(i);
			if (slot != null && slot.GetHeldEntity() as ThrownWeapon != null)
			{
				return slot;
			}
		}
		return null;
	}
}
