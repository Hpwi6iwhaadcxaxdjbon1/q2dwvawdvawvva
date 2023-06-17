using System;
using System.Collections.Generic;
using Network;
using UnityEngine;

// Token: 0x020000EC RID: 236
public class WaterInflatable : BaseMountable, IPoolVehicle, INotifyTrigger
{
	// Token: 0x04000D0C RID: 3340
	public Rigidbody rigidBody;

	// Token: 0x04000D0D RID: 3341
	public Transform centerOfMass;

	// Token: 0x04000D0E RID: 3342
	public float forwardPushForce = 5f;

	// Token: 0x04000D0F RID: 3343
	public float rearPushForce = 5f;

	// Token: 0x04000D10 RID: 3344
	public float rotationForce = 5f;

	// Token: 0x04000D11 RID: 3345
	public float maxSpeed = 3f;

	// Token: 0x04000D12 RID: 3346
	public float maxPaddleFrequency = 0.5f;

	// Token: 0x04000D13 RID: 3347
	public SoundDefinition paddleSfx;

	// Token: 0x04000D14 RID: 3348
	public SoundDefinition smallPlayerMovementSound;

	// Token: 0x04000D15 RID: 3349
	public SoundDefinition largePlayerMovementSound;

	// Token: 0x04000D16 RID: 3350
	public BlendedSoundLoops waterLoops;

	// Token: 0x04000D17 RID: 3351
	public float waterSoundSpeedDivisor = 1f;

	// Token: 0x04000D18 RID: 3352
	public float additiveDownhillVelocity;

	// Token: 0x04000D19 RID: 3353
	public GameObjectRef handSplashForwardEffect;

	// Token: 0x04000D1A RID: 3354
	public GameObjectRef handSplashBackEffect;

	// Token: 0x04000D1B RID: 3355
	public GameObjectRef footSplashEffect;

	// Token: 0x04000D1C RID: 3356
	public float animationLerpSpeed = 1f;

	// Token: 0x04000D1D RID: 3357
	public Transform smoothedEyePosition;

	// Token: 0x04000D1E RID: 3358
	public float smoothedEyeSpeed = 1f;

	// Token: 0x04000D1F RID: 3359
	public Buoyancy buoyancy;

	// Token: 0x04000D20 RID: 3360
	public bool driftTowardsIsland;

	// Token: 0x04000D21 RID: 3361
	public GameObjectRef mountEffect;

	// Token: 0x04000D22 RID: 3362
	[Range(0f, 1f)]
	public float handSplashOffset = 1f;

	// Token: 0x04000D23 RID: 3363
	public float velocitySplashMultiplier = 4f;

	// Token: 0x04000D24 RID: 3364
	public Vector3 modifyEyeOffset = Vector3.zero;

	// Token: 0x04000D25 RID: 3365
	[Range(0f, 1f)]
	public float inheritVelocityMultiplier;

	// Token: 0x04000D26 RID: 3366
	private TimeSince lastPaddle;

	// Token: 0x04000D27 RID: 3367
	public ParticleSystem[] movingParticleSystems;

	// Token: 0x04000D28 RID: 3368
	public float movingParticlesThreshold = 0.0005f;

	// Token: 0x04000D29 RID: 3369
	public Transform headSpaceCheckPosition;

	// Token: 0x04000D2A RID: 3370
	public float headSpaceCheckRadius = 0.4f;

	// Token: 0x04000D2B RID: 3371
	private TimeSince landFacingCheck;

	// Token: 0x04000D2C RID: 3372
	private bool isFacingLand;

	// Token: 0x04000D2D RID: 3373
	private float landPushAcceleration;

	// Token: 0x04000D2E RID: 3374
	private TimeSince inPoolCheck;

	// Token: 0x04000D2F RID: 3375
	private bool isInPool;

	// Token: 0x04000D30 RID: 3376
	private Vector3 lastPos = Vector3.zero;

	// Token: 0x04000D31 RID: 3377
	private Vector3 lastClipCheckPosition;

	// Token: 0x04000D32 RID: 3378
	private bool forceClippingCheck;

	// Token: 0x04000D33 RID: 3379
	private bool prevSleeping;

	// Token: 0x060014DA RID: 5338 RVA: 0x000A4D54 File Offset: 0x000A2F54
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("WaterInflatable.OnRpcMessage", 0))
		{
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x060014DB RID: 5339 RVA: 0x000A4D94 File Offset: 0x000A2F94
	public override void ServerInit()
	{
		base.ServerInit();
		this.rigidBody.centerOfMass = this.centerOfMass.localPosition;
		this.prevSleeping = false;
	}

	// Token: 0x060014DC RID: 5340 RVA: 0x000A4DBC File Offset: 0x000A2FBC
	public override void OnDeployed(BaseEntity parent, BasePlayer deployedBy, Item fromItem)
	{
		base.OnDeployed(parent, deployedBy, fromItem);
		if (deployedBy != null)
		{
			Vector3 estimatedVelocity = deployedBy.estimatedVelocity;
			float value = Vector3.Dot(base.transform.forward, estimatedVelocity.normalized);
			Vector3 vector = Vector3.Lerp(Vector3.zero, estimatedVelocity, Mathf.Clamp(value, 0f, 1f));
			vector *= this.inheritVelocityMultiplier;
			this.rigidBody.AddForce(vector, ForceMode.VelocityChange);
		}
	}

	// Token: 0x060014DD RID: 5341 RVA: 0x000A4E30 File Offset: 0x000A3030
	public override void VehicleFixedUpdate()
	{
		base.VehicleFixedUpdate();
		bool flag = this.rigidBody.IsSleeping();
		if (this.prevSleeping && !flag && this.buoyancy != null)
		{
			this.buoyancy.Wake();
		}
		this.prevSleeping = flag;
		if (this.rigidBody.velocity.magnitude > this.maxSpeed)
		{
			this.rigidBody.velocity = Vector3.ClampMagnitude(this.rigidBody.velocity, this.maxSpeed);
		}
		if (this.AnyMounted() && this.headSpaceCheckPosition != null)
		{
			Vector3 position = base.transform.position;
			if (this.forceClippingCheck || Vector3.Distance(position, this.lastClipCheckPosition) > this.headSpaceCheckRadius * 0.5f)
			{
				this.forceClippingCheck = false;
				this.lastClipCheckPosition = position;
				if (GamePhysics.CheckSphere(this.headSpaceCheckPosition.position, this.headSpaceCheckRadius, 1218511105, QueryTriggerInteraction.UseGlobal))
				{
					this.DismountAllPlayers();
				}
			}
		}
	}

	// Token: 0x060014DE RID: 5342 RVA: 0x000A4F2C File Offset: 0x000A312C
	public override void OnPlayerMounted()
	{
		base.OnPlayerMounted();
		this.lastPos = base.transform.position;
		this.forceClippingCheck = true;
	}

	// Token: 0x060014DF RID: 5343 RVA: 0x000A4F4C File Offset: 0x000A314C
	public override void PlayerServerInput(InputState inputState, BasePlayer player)
	{
		base.PlayerServerInput(inputState, player);
		if (Vector3.Dot(base.transform.up, Vector3.up) < 0.1f)
		{
			this.DismountAllPlayers();
			return;
		}
		if (this.lastPaddle < this.maxPaddleFrequency)
		{
			return;
		}
		if (this.buoyancy != null && this.IsOutOfWaterServer())
		{
			return;
		}
		if (player.GetHeldEntity() == null)
		{
			if (inputState.IsDown(BUTTON.FORWARD))
			{
				if (this.rigidBody.velocity.magnitude < this.maxSpeed)
				{
					this.rigidBody.AddForce(base.transform.forward * this.forwardPushForce, ForceMode.Impulse);
				}
				this.rigidBody.angularVelocity = Vector3.Lerp(this.rigidBody.angularVelocity, base.transform.forward, 0.5f);
				this.lastPaddle = 0f;
				base.ClientRPC<int>(null, "OnPaddled", 0);
			}
			if (inputState.IsDown(BUTTON.BACKWARD))
			{
				this.rigidBody.AddForce(-base.transform.forward * this.rearPushForce, ForceMode.Impulse);
				this.rigidBody.angularVelocity = Vector3.Lerp(this.rigidBody.angularVelocity, -base.transform.forward, 0.5f);
				this.lastPaddle = 0f;
				base.ClientRPC<int>(null, "OnPaddled", 3);
			}
			if (inputState.IsDown(BUTTON.LEFT))
			{
				this.PaddleTurn(WaterInflatable.PaddleDirection.Left);
			}
			if (inputState.IsDown(BUTTON.RIGHT))
			{
				this.PaddleTurn(WaterInflatable.PaddleDirection.Right);
			}
		}
		if (this.inPoolCheck > 2f)
		{
			this.isInPool = base.IsInWaterVolume(base.transform.position);
			this.inPoolCheck = 0f;
		}
		if (this.additiveDownhillVelocity > 0f && !this.isInPool)
		{
			Vector3 vector = base.transform.TransformPoint(Vector3.forward);
			Vector3 position = base.transform.position;
			if (vector.y < position.y)
			{
				float num = this.additiveDownhillVelocity * (position.y - vector.y);
				this.rigidBody.AddForce(num * Time.fixedDeltaTime * base.transform.forward, ForceMode.Acceleration);
			}
			Vector3 velocity = this.rigidBody.velocity;
			this.rigidBody.velocity = Vector3.Lerp(velocity, base.transform.forward * velocity.magnitude, 0.4f);
		}
		if (this.driftTowardsIsland && this.landFacingCheck > 2f && !this.isInPool)
		{
			this.isFacingLand = false;
			this.landFacingCheck = 0f;
			Vector3 position2 = base.transform.position;
			if (!WaterResource.IsFreshWater(position2))
			{
				int num2 = 5;
				Vector3 forward = base.transform.forward;
				forward.y = 0f;
				for (int i = 1; i <= num2; i++)
				{
					int mask = 128;
					if (!TerrainMeta.TopologyMap.GetTopology(position2 + (float)i * 15f * forward, mask))
					{
						this.isFacingLand = true;
						break;
					}
				}
			}
		}
		if (this.driftTowardsIsland && this.isFacingLand && !this.isInPool)
		{
			this.landPushAcceleration = Mathf.Clamp(this.landPushAcceleration + Time.deltaTime, 0f, 3f);
			this.rigidBody.AddForce(base.transform.forward * (Time.deltaTime * this.landPushAcceleration), ForceMode.VelocityChange);
		}
		else
		{
			this.landPushAcceleration = 0f;
		}
		this.lastPos = base.transform.position;
	}

	// Token: 0x060014E0 RID: 5344 RVA: 0x000A531C File Offset: 0x000A351C
	private void PaddleTurn(WaterInflatable.PaddleDirection direction)
	{
		if (direction == WaterInflatable.PaddleDirection.Forward || direction == WaterInflatable.PaddleDirection.Back)
		{
			return;
		}
		this.rigidBody.AddRelativeTorque(this.rotationForce * ((direction == WaterInflatable.PaddleDirection.Left) ? (-Vector3.up) : Vector3.up), ForceMode.Impulse);
		this.lastPaddle = 0f;
		base.ClientRPC<int>(null, "OnPaddled", (int)direction);
	}

	// Token: 0x060014E1 RID: 5345 RVA: 0x00029CA8 File Offset: 0x00027EA8
	public override float WaterFactorForPlayer(BasePlayer player)
	{
		return 0f;
	}

	// Token: 0x060014E2 RID: 5346 RVA: 0x000A537C File Offset: 0x000A357C
	public override void OnCollision(Collision collision, BaseEntity hitEntity)
	{
		BaseVehicle baseVehicle;
		if ((baseVehicle = (hitEntity as BaseVehicle)) != null && (baseVehicle.HasDriver() || baseVehicle.IsMoving() || baseVehicle.HasFlag(BaseEntity.Flags.On)))
		{
			base.Kill(BaseNetworkable.DestroyMode.Gib);
		}
	}

	// Token: 0x060014E3 RID: 5347 RVA: 0x000A53B3 File Offset: 0x000A35B3
	private bool IsOutOfWaterServer()
	{
		return this.buoyancy.timeOutOfWater > 0.2f;
	}

	// Token: 0x060014E4 RID: 5348 RVA: 0x00029A3C File Offset: 0x00027C3C
	public void OnPoolDestroyed()
	{
		base.Kill(BaseNetworkable.DestroyMode.Gib);
	}

	// Token: 0x060014E5 RID: 5349 RVA: 0x000A53C8 File Offset: 0x000A35C8
	public void WakeUp()
	{
		if (this.rigidBody != null)
		{
			this.rigidBody.WakeUp();
			this.rigidBody.AddForce(Vector3.up * 0.1f, ForceMode.Impulse);
		}
		if (this.buoyancy != null)
		{
			this.buoyancy.Wake();
		}
	}

	// Token: 0x170001DE RID: 478
	// (get) Token: 0x060014E6 RID: 5350 RVA: 0x0000441C File Offset: 0x0000261C
	public override bool IsSummerDlcVehicle
	{
		get
		{
			return true;
		}
	}

	// Token: 0x060014E7 RID: 5351 RVA: 0x000A5424 File Offset: 0x000A3624
	public void OnObjects(TriggerNotify trigger)
	{
		if (base.isClient)
		{
			return;
		}
		using (HashSet<BaseEntity>.Enumerator enumerator = trigger.entityContents.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				BaseVehicle baseVehicle;
				if ((baseVehicle = (enumerator.Current as BaseVehicle)) != null && (baseVehicle.HasDriver() || baseVehicle.IsMoving() || baseVehicle.HasFlag(BaseEntity.Flags.On)))
				{
					base.Kill(BaseNetworkable.DestroyMode.Gib);
					break;
				}
			}
		}
	}

	// Token: 0x060014E8 RID: 5352 RVA: 0x000063A5 File Offset: 0x000045A5
	public void OnEmpty()
	{
	}

	// Token: 0x02000C18 RID: 3096
	private enum PaddleDirection
	{
		// Token: 0x04004200 RID: 16896
		Forward,
		// Token: 0x04004201 RID: 16897
		Left,
		// Token: 0x04004202 RID: 16898
		Right,
		// Token: 0x04004203 RID: 16899
		Back
	}
}
