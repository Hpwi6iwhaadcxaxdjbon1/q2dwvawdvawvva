using System;
using System.Collections.Generic;
using Rust;
using UnityEngine;

// Token: 0x02000479 RID: 1145
public abstract class GroundVehicle : BaseVehicle, IEngineControllerUser, IEntity, TriggerHurtNotChild.IHurtTriggerUser
{
	// Token: 0x04001E06 RID: 7686
	[Header("GroundVehicle")]
	[SerializeField]
	protected GroundVehicleAudio gvAudio;

	// Token: 0x04001E07 RID: 7687
	[SerializeField]
	private GameObjectRef fuelStoragePrefab;

	// Token: 0x04001E08 RID: 7688
	[SerializeField]
	private Transform waterloggedPoint;

	// Token: 0x04001E09 RID: 7689
	[SerializeField]
	private GameObjectRef collisionEffect;

	// Token: 0x04001E0A RID: 7690
	[SerializeField]
	private float engineStartupTime = 0.5f;

	// Token: 0x04001E0B RID: 7691
	[SerializeField]
	private float minCollisionDamageForce = 20000f;

	// Token: 0x04001E0C RID: 7692
	[SerializeField]
	private float maxCollisionDamageForce = 2500000f;

	// Token: 0x04001E0D RID: 7693
	[SerializeField]
	private float collisionDamageMultiplier = 1f;

	// Token: 0x04001E0F RID: 7695
	protected VehicleEngineController<GroundVehicle> engineController;

	// Token: 0x04001E10 RID: 7696
	private Dictionary<BaseEntity, float> damageSinceLastTick = new Dictionary<BaseEntity, float>();

	// Token: 0x04001E11 RID: 7697
	private float nextCollisionDamageTime;

	// Token: 0x04001E12 RID: 7698
	private float dragMod;

	// Token: 0x04001E13 RID: 7699
	private float dragModDuration;

	// Token: 0x04001E14 RID: 7700
	private TimeSince timeSinceDragModSet;

	// Token: 0x1700031B RID: 795
	// (get) Token: 0x060025B2 RID: 9650 RVA: 0x000EE92F File Offset: 0x000ECB2F
	// (set) Token: 0x060025B3 RID: 9651 RVA: 0x000EE937 File Offset: 0x000ECB37
	public Vector3 Velocity { get; private set; }

	// Token: 0x1700031C RID: 796
	// (get) Token: 0x060025B4 RID: 9652
	public abstract float DriveWheelVelocity { get; }

	// Token: 0x1700031D RID: 797
	// (get) Token: 0x060025B5 RID: 9653 RVA: 0x00003FA8 File Offset: 0x000021A8
	public bool LightsAreOn
	{
		get
		{
			return base.HasFlag(BaseEntity.Flags.Reserved5);
		}
	}

	// Token: 0x1700031E RID: 798
	// (get) Token: 0x060025B6 RID: 9654 RVA: 0x000EE940 File Offset: 0x000ECB40
	public VehicleEngineController<GroundVehicle>.EngineState CurEngineState
	{
		get
		{
			return this.engineController.CurEngineState;
		}
	}

	// Token: 0x060025B7 RID: 9655 RVA: 0x000EE94D File Offset: 0x000ECB4D
	public override void InitShared()
	{
		base.InitShared();
		this.engineController = new VehicleEngineController<GroundVehicle>(this, base.isServer, this.engineStartupTime, this.fuelStoragePrefab, this.waterloggedPoint, BaseEntity.Flags.Reserved1);
	}

	// Token: 0x060025B8 RID: 9656 RVA: 0x000EE97E File Offset: 0x000ECB7E
	public override void OnFlagsChanged(BaseEntity.Flags old, BaseEntity.Flags next)
	{
		base.OnFlagsChanged(old, next);
		if (old == next)
		{
			return;
		}
		if (base.isServer)
		{
			this.ServerFlagsChanged(old, next);
		}
	}

	// Token: 0x060025B9 RID: 9657 RVA: 0x000EE99D File Offset: 0x000ECB9D
	public float GetSpeed()
	{
		if (base.IsStationary())
		{
			return 0f;
		}
		return Vector3.Dot(this.Velocity, base.transform.forward);
	}

	// Token: 0x060025BA RID: 9658
	public abstract float GetMaxForwardSpeed();

	// Token: 0x060025BB RID: 9659
	public abstract float GetThrottleInput();

	// Token: 0x060025BC RID: 9660
	public abstract float GetBrakeInput();

	// Token: 0x060025BD RID: 9661 RVA: 0x000EE9C3 File Offset: 0x000ECBC3
	protected override bool CanPushNow(BasePlayer pusher)
	{
		return base.CanPushNow(pusher) && !pusher.isMounted && !pusher.IsSwimming() && !pusher.IsStandingOnEntity(this, 8192);
	}

	// Token: 0x060025BE RID: 9662 RVA: 0x000EE9F1 File Offset: 0x000ECBF1
	public override void ServerInit()
	{
		base.ServerInit();
		this.timeSinceDragModSet = default(TimeSince);
		this.timeSinceDragModSet = float.MaxValue;
	}

	// Token: 0x060025BF RID: 9663
	public abstract void OnEngineStartFailed();

	// Token: 0x060025C0 RID: 9664
	public abstract bool MeetsEngineRequirements();

	// Token: 0x060025C1 RID: 9665 RVA: 0x000063A5 File Offset: 0x000045A5
	protected virtual void ServerFlagsChanged(BaseEntity.Flags old, BaseEntity.Flags next)
	{
	}

	// Token: 0x060025C2 RID: 9666 RVA: 0x000EEA15 File Offset: 0x000ECC15
	protected void OnCollisionEnter(Collision collision)
	{
		if (base.isServer)
		{
			this.ProcessCollision(collision);
		}
	}

	// Token: 0x060025C3 RID: 9667 RVA: 0x000EEA28 File Offset: 0x000ECC28
	public override void VehicleFixedUpdate()
	{
		base.VehicleFixedUpdate();
		if (base.IsMovingOrOn)
		{
			this.Velocity = base.GetLocalVelocity();
		}
		else
		{
			this.Velocity = Vector3.zero;
		}
		if (this.LightsAreOn && !this.AnyMounted())
		{
			base.SetFlag(BaseEntity.Flags.Reserved5, false, false, true);
		}
		if (Time.time >= this.nextCollisionDamageTime)
		{
			this.nextCollisionDamageTime = Time.time + 0.33f;
			foreach (KeyValuePair<BaseEntity, float> keyValuePair in this.damageSinceLastTick)
			{
				this.DoCollisionDamage(keyValuePair.Key, keyValuePair.Value);
			}
			this.damageSinceLastTick.Clear();
		}
	}

	// Token: 0x060025C4 RID: 9668 RVA: 0x000EEAF8 File Offset: 0x000ECCF8
	public override void LightToggle(BasePlayer player)
	{
		if (!base.IsDriver(player))
		{
			return;
		}
		base.SetFlag(BaseEntity.Flags.Reserved5, !this.LightsAreOn, false, true);
	}

	// Token: 0x060025C5 RID: 9669 RVA: 0x000EEB1A File Offset: 0x000ECD1A
	public float GetDamageMultiplier(BaseEntity ent)
	{
		return Mathf.Abs(this.GetSpeed()) * 1f;
	}

	// Token: 0x060025C6 RID: 9670 RVA: 0x000EEB30 File Offset: 0x000ECD30
	public void OnHurtTriggerOccupant(BaseEntity hurtEntity, DamageType damageType, float damageTotal)
	{
		if (base.isClient)
		{
			return;
		}
		if (hurtEntity.IsDestroyed)
		{
			return;
		}
		Vector3 a = hurtEntity.GetLocalVelocity() - this.Velocity;
		Vector3 position = base.ClosestPoint(hurtEntity.transform.position);
		Vector3 a2 = hurtEntity.RealisticMass * a;
		this.rigidBody.AddForceAtPosition(a2 * 1.25f, position, ForceMode.Impulse);
		this.QueueCollisionDamage(this, a2.magnitude * 0.75f / Time.deltaTime);
		this.SetTempDrag(2.25f, 1f);
	}

	// Token: 0x060025C7 RID: 9671 RVA: 0x000EEBC4 File Offset: 0x000ECDC4
	private float QueueCollisionDamage(BaseEntity hitEntity, float forceMagnitude)
	{
		float num = Mathf.InverseLerp(this.minCollisionDamageForce, this.maxCollisionDamageForce, forceMagnitude);
		if (num > 0f)
		{
			float num2 = Mathf.Lerp(1f, 200f, num) * this.collisionDamageMultiplier;
			float num3;
			if (this.damageSinceLastTick.TryGetValue(hitEntity, out num3))
			{
				if (num3 < num2)
				{
					this.damageSinceLastTick[hitEntity] = num2;
				}
			}
			else
			{
				this.damageSinceLastTick[hitEntity] = num2;
			}
		}
		return num;
	}

	// Token: 0x060025C8 RID: 9672 RVA: 0x000EEC35 File Offset: 0x000ECE35
	protected virtual void DoCollisionDamage(BaseEntity hitEntity, float damage)
	{
		base.Hurt(damage, DamageType.Collision, this, false);
	}

	// Token: 0x060025C9 RID: 9673 RVA: 0x000EEC44 File Offset: 0x000ECE44
	private void ProcessCollision(Collision collision)
	{
		if (base.isClient || collision == null || collision.gameObject == null || collision.gameObject == null)
		{
			return;
		}
		ContactPoint contact = collision.GetContact(0);
		BaseEntity baseEntity = null;
		if (contact.otherCollider.attachedRigidbody == this.rigidBody)
		{
			baseEntity = contact.otherCollider.ToBaseEntity();
		}
		else if (contact.thisCollider.attachedRigidbody == this.rigidBody)
		{
			baseEntity = contact.thisCollider.ToBaseEntity();
		}
		if (baseEntity != null)
		{
			float forceMagnitude = collision.impulse.magnitude / Time.fixedDeltaTime;
			if (this.QueueCollisionDamage(baseEntity, forceMagnitude) > 0f)
			{
				base.TryShowCollisionFX(collision, this.collisionEffect);
			}
		}
	}

	// Token: 0x060025CA RID: 9674 RVA: 0x000EED0B File Offset: 0x000ECF0B
	public virtual float GetModifiedDrag()
	{
		return (1f - Mathf.InverseLerp(0f, this.dragModDuration, this.timeSinceDragModSet)) * this.dragMod;
	}

	// Token: 0x060025CB RID: 9675 RVA: 0x000EED35 File Offset: 0x000ECF35
	public override EntityFuelSystem GetFuelSystem()
	{
		return this.engineController.FuelSystem;
	}

	// Token: 0x060025CC RID: 9676 RVA: 0x000070B5 File Offset: 0x000052B5
	protected override void OnChildAdded(BaseEntity child)
	{
		base.OnChildAdded(child);
		if (base.isServer && this.isSpawned)
		{
			this.GetFuelSystem().CheckNewChild(child);
		}
	}

	// Token: 0x060025CD RID: 9677 RVA: 0x000EED42 File Offset: 0x000ECF42
	private void SetTempDrag(float drag, float duration)
	{
		this.dragMod = Mathf.Clamp(drag, 0f, 1000f);
		this.timeSinceDragModSet = 0f;
		this.dragModDuration = duration;
	}

	// Token: 0x060025CF RID: 9679 RVA: 0x00007C28 File Offset: 0x00005E28
	void IEngineControllerUser.Invoke(Action action, float time)
	{
		base.Invoke(action, time);
	}

	// Token: 0x060025D0 RID: 9680 RVA: 0x00007C32 File Offset: 0x00005E32
	void IEngineControllerUser.CancelInvoke(Action action)
	{
		base.CancelInvoke(action);
	}
}
