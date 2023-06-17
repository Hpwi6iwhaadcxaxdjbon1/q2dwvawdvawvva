using System;
using UnityEngine;

// Token: 0x0200018D RID: 397
public class Sled : BaseVehicle, INotifyTrigger
{
	// Token: 0x040010AC RID: 4268
	private const BaseEntity.Flags BrakeOn = BaseEntity.Flags.Reserved1;

	// Token: 0x040010AD RID: 4269
	private const BaseEntity.Flags OnSnow = BaseEntity.Flags.Reserved2;

	// Token: 0x040010AE RID: 4270
	private const BaseEntity.Flags IsGrounded = BaseEntity.Flags.Reserved3;

	// Token: 0x040010AF RID: 4271
	private const BaseEntity.Flags OnSand = BaseEntity.Flags.Reserved4;

	// Token: 0x040010B0 RID: 4272
	public PhysicMaterial BrakeMaterial;

	// Token: 0x040010B1 RID: 4273
	public PhysicMaterial SnowMaterial;

	// Token: 0x040010B2 RID: 4274
	public PhysicMaterial NonSnowMaterial;

	// Token: 0x040010B3 RID: 4275
	public Transform CentreOfMassTransform;

	// Token: 0x040010B4 RID: 4276
	public Collider[] PhysicsMaterialTargets;

	// Token: 0x040010B5 RID: 4277
	public float InitialForceCutoff = 3f;

	// Token: 0x040010B6 RID: 4278
	public float InitialForceIncreaseRate = 0.05f;

	// Token: 0x040010B7 RID: 4279
	public float TurnForce = 1f;

	// Token: 0x040010B8 RID: 4280
	public float DirectionMatchForce = 1f;

	// Token: 0x040010B9 RID: 4281
	public float VerticalAdjustmentForce = 1f;

	// Token: 0x040010BA RID: 4282
	public float VerticalAdjustmentAngleThreshold = 15f;

	// Token: 0x040010BB RID: 4283
	public float NudgeCooldown = 3f;

	// Token: 0x040010BC RID: 4284
	public float NudgeForce = 2f;

	// Token: 0x040010BD RID: 4285
	public float MaxNudgeVelocity = 2f;

	// Token: 0x040010BE RID: 4286
	public const float DecayFrequency = 60f;

	// Token: 0x040010BF RID: 4287
	public float DecayAmount = 10f;

	// Token: 0x040010C0 RID: 4288
	public ParticleSystemContainer TrailEffects;

	// Token: 0x040010C1 RID: 4289
	public SoundDefinition enterSnowSoundDef;

	// Token: 0x040010C2 RID: 4290
	public SoundDefinition snowSlideLoopSoundDef;

	// Token: 0x040010C3 RID: 4291
	public SoundDefinition dirtSlideLoopSoundDef;

	// Token: 0x040010C4 RID: 4292
	public AnimationCurve movementLoopGainCurve;

	// Token: 0x040010C5 RID: 4293
	public AnimationCurve movementLoopPitchCurve;

	// Token: 0x040010C6 RID: 4294
	private VehicleTerrainHandler terrainHandler;

	// Token: 0x040010C7 RID: 4295
	private PhysicMaterial cachedMaterial;

	// Token: 0x040010C8 RID: 4296
	private float initialForceScale;

	// Token: 0x040010C9 RID: 4297
	private TimeSince leftIce;

	// Token: 0x040010CA RID: 4298
	private TimeSince lastNudge;

	// Token: 0x060017D4 RID: 6100 RVA: 0x000B39D4 File Offset: 0x000B1BD4
	public override void ServerInit()
	{
		base.ServerInit();
		this.terrainHandler = new VehicleTerrainHandler(this);
		this.terrainHandler.RayLength = 0.6f;
		this.rigidBody.centerOfMass = this.CentreOfMassTransform.localPosition;
		base.InvokeRandomized(new Action(this.DecayOverTime), UnityEngine.Random.Range(30f, 60f), 60f, 6f);
	}

	// Token: 0x060017D5 RID: 6101 RVA: 0x000B3A44 File Offset: 0x000B1C44
	public override void OnDeployed(BaseEntity parent, BasePlayer deployedBy, Item fromItem)
	{
		base.OnDeployed(parent, deployedBy, fromItem);
		base.SetFlag(BaseEntity.Flags.Reserved1, true, false, true);
		this.UpdateGroundedFlag();
		this.UpdatePhysicsMaterial();
	}

	// Token: 0x060017D6 RID: 6102 RVA: 0x000B3A6C File Offset: 0x000B1C6C
	public override void VehicleFixedUpdate()
	{
		base.VehicleFixedUpdate();
		if (this.AnyMounted())
		{
			this.terrainHandler.FixedUpdate();
			if (!this.terrainHandler.IsGrounded)
			{
				Quaternion b = Quaternion.FromToRotation(base.transform.up, Vector3.up) * this.rigidBody.rotation;
				if (Quaternion.Angle(this.rigidBody.rotation, b) > this.VerticalAdjustmentAngleThreshold)
				{
					this.rigidBody.MoveRotation(Quaternion.Slerp(this.rigidBody.rotation, b, Time.fixedDeltaTime * this.VerticalAdjustmentForce));
				}
			}
		}
	}

	// Token: 0x060017D7 RID: 6103 RVA: 0x000B3B08 File Offset: 0x000B1D08
	private void UpdatePhysicsMaterial()
	{
		this.cachedMaterial = this.GetPhysicMaterial();
		Collider[] physicsMaterialTargets = this.PhysicsMaterialTargets;
		for (int i = 0; i < physicsMaterialTargets.Length; i++)
		{
			physicsMaterialTargets[i].sharedMaterial = this.cachedMaterial;
		}
		if (!this.AnyMounted() && this.rigidBody.IsSleeping())
		{
			base.CancelInvoke(new Action(this.UpdatePhysicsMaterial));
		}
		base.SetFlag(BaseEntity.Flags.Reserved2, this.terrainHandler.IsOnSnowOrIce, false, true);
		base.SetFlag(BaseEntity.Flags.Reserved4, this.terrainHandler.OnSurface == VehicleTerrainHandler.Surface.Sand, false, true);
	}

	// Token: 0x060017D8 RID: 6104 RVA: 0x000B3BA0 File Offset: 0x000B1DA0
	private void UpdateGroundedFlag()
	{
		if (!this.AnyMounted() && this.rigidBody.IsSleeping())
		{
			base.CancelInvoke(new Action(this.UpdateGroundedFlag));
		}
		base.SetFlag(BaseEntity.Flags.Reserved3, this.terrainHandler.IsGrounded, false, true);
	}

	// Token: 0x060017D9 RID: 6105 RVA: 0x000B3BEC File Offset: 0x000B1DEC
	private PhysicMaterial GetPhysicMaterial()
	{
		if (base.HasFlag(BaseEntity.Flags.Reserved1) || !this.AnyMounted())
		{
			return this.BrakeMaterial;
		}
		bool flag = this.terrainHandler.IsOnSnowOrIce || this.terrainHandler.OnSurface == VehicleTerrainHandler.Surface.Sand;
		if (flag)
		{
			this.leftIce = 0f;
		}
		else if (this.leftIce < 2f)
		{
			flag = true;
		}
		if (!flag)
		{
			return this.NonSnowMaterial;
		}
		return this.SnowMaterial;
	}

	// Token: 0x060017DA RID: 6106 RVA: 0x000B3C70 File Offset: 0x000B1E70
	public override void PlayerMounted(BasePlayer player, BaseMountable seat)
	{
		base.PlayerMounted(player, seat);
		if (base.HasFlag(BaseEntity.Flags.Reserved1))
		{
			this.initialForceScale = 0f;
			base.InvokeRepeating(new Action(this.ApplyInitialForce), 0f, 0.1f);
			base.SetFlag(BaseEntity.Flags.Reserved1, false, false, true);
		}
		if (!base.IsInvoking(new Action(this.UpdatePhysicsMaterial)))
		{
			base.InvokeRepeating(new Action(this.UpdatePhysicsMaterial), 0f, 0.5f);
		}
		if (!base.IsInvoking(new Action(this.UpdateGroundedFlag)))
		{
			base.InvokeRepeating(new Action(this.UpdateGroundedFlag), 0f, 0.1f);
		}
		if (this.rigidBody.IsSleeping())
		{
			this.rigidBody.WakeUp();
		}
	}

	// Token: 0x060017DB RID: 6107 RVA: 0x000B3D40 File Offset: 0x000B1F40
	private void ApplyInitialForce()
	{
		Vector3 forward = base.transform.forward;
		Vector3 a = (Vector3.Dot(forward, -Vector3.up) > Vector3.Dot(-forward, -Vector3.up)) ? forward : (-forward);
		this.rigidBody.AddForce(a * this.initialForceScale * (this.terrainHandler.IsOnSnowOrIce ? 1f : 0.25f), ForceMode.Acceleration);
		this.initialForceScale += this.InitialForceIncreaseRate;
		if (this.initialForceScale >= this.InitialForceCutoff && (this.rigidBody.velocity.magnitude > 1f || !this.terrainHandler.IsOnSnowOrIce))
		{
			base.CancelInvoke(new Action(this.ApplyInitialForce));
		}
	}

	// Token: 0x060017DC RID: 6108 RVA: 0x000B3E1C File Offset: 0x000B201C
	public override void PlayerServerInput(InputState inputState, BasePlayer player)
	{
		base.PlayerServerInput(inputState, player);
		if (Vector3.Dot(base.transform.up, Vector3.up) < 0.1f || this.WaterFactor() > 0.25f)
		{
			this.DismountAllPlayers();
			return;
		}
		float num = inputState.IsDown(BUTTON.LEFT) ? -1f : 0f;
		num += (inputState.IsDown(BUTTON.RIGHT) ? 1f : 0f);
		if (inputState.IsDown(BUTTON.FORWARD) && this.lastNudge > this.NudgeCooldown && this.rigidBody.velocity.magnitude < this.MaxNudgeVelocity)
		{
			this.rigidBody.WakeUp();
			this.rigidBody.AddForce(base.transform.forward * this.NudgeForce, ForceMode.Impulse);
			this.rigidBody.AddForce(base.transform.up * this.NudgeForce * 0.5f, ForceMode.Impulse);
			this.lastNudge = 0f;
		}
		num *= this.TurnForce;
		Vector3 velocity = this.rigidBody.velocity;
		if (num != 0f)
		{
			base.transform.Rotate(Vector3.up * num * Time.deltaTime * velocity.magnitude, Space.Self);
		}
		if (this.terrainHandler.IsGrounded && Vector3.Dot(this.rigidBody.velocity.normalized, base.transform.forward) >= 0.5f)
		{
			this.rigidBody.velocity = Vector3.Lerp(this.rigidBody.velocity, base.transform.forward * velocity.magnitude, Time.deltaTime * this.DirectionMatchForce);
		}
	}

	// Token: 0x060017DD RID: 6109 RVA: 0x000B3FF5 File Offset: 0x000B21F5
	private void DecayOverTime()
	{
		if (this.AnyMounted())
		{
			return;
		}
		base.Hurt(this.DecayAmount);
	}

	// Token: 0x060017DE RID: 6110 RVA: 0x000B400C File Offset: 0x000B220C
	public override bool CanPickup(BasePlayer player)
	{
		return base.CanPickup(player) && !player.isMounted;
	}

	// Token: 0x060017DF RID: 6111 RVA: 0x000B4024 File Offset: 0x000B2224
	public void OnObjects(TriggerNotify trigger)
	{
		foreach (BaseEntity baseEntity in trigger.entityContents)
		{
			if (!(baseEntity is Sled))
			{
				BaseVehicleModule baseVehicleModule;
				if ((baseVehicleModule = (baseEntity as BaseVehicleModule)) != null && baseVehicleModule.Vehicle != null && (baseVehicleModule.Vehicle.IsOn() || !baseVehicleModule.Vehicle.IsStationary()))
				{
					base.Kill(BaseNetworkable.DestroyMode.Gib);
					break;
				}
				BaseVehicle baseVehicle;
				if ((baseVehicle = (baseEntity as BaseVehicle)) != null && baseVehicle.HasDriver() && (baseVehicle.IsMoving() || baseVehicle.HasFlag(BaseEntity.Flags.On)))
				{
					base.Kill(BaseNetworkable.DestroyMode.Gib);
					break;
				}
			}
		}
	}

	// Token: 0x060017E0 RID: 6112 RVA: 0x000063A5 File Offset: 0x000045A5
	public void OnEmpty()
	{
	}

	// Token: 0x17000203 RID: 515
	// (get) Token: 0x060017E1 RID: 6113 RVA: 0x00007A3C File Offset: 0x00005C3C
	public override bool BlocksDoors
	{
		get
		{
			return false;
		}
	}
}
