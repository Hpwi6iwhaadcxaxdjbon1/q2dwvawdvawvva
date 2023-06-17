using System;
using System.Collections.Generic;
using Facepunch;
using Network;
using ProtoBuf;
using Rust;
using UnityEngine;

// Token: 0x02000109 RID: 265
public class Drone : RemoteControlEntity, IRemoteControllableClientCallbacks, IRemoteControllable
{
	// Token: 0x04000E18 RID: 3608
	[ReplicatedVar(Help = "How far drones can be flown away from the controlling computer station", ShowInAdminUI = true, Default = "250")]
	public static float maxControlRange = 500f;

	// Token: 0x04000E19 RID: 3609
	[ServerVar(Help = "If greater than zero, overrides the drone's planar movement speed")]
	public static float movementSpeedOverride = 0f;

	// Token: 0x04000E1A RID: 3610
	[ServerVar(Help = "If greater than zero, overrides the drone's vertical movement speed")]
	public static float altitudeSpeedOverride = 0f;

	// Token: 0x04000E1B RID: 3611
	[ClientVar(ClientAdmin = true)]
	public static float windTimeDivisor = 10f;

	// Token: 0x04000E1C RID: 3612
	[ClientVar(ClientAdmin = true)]
	public static float windPositionDivisor = 100f;

	// Token: 0x04000E1D RID: 3613
	[ClientVar(ClientAdmin = true)]
	public static float windPositionScale = 1f;

	// Token: 0x04000E1E RID: 3614
	[ClientVar(ClientAdmin = true)]
	public static float windRotationMultiplier = 45f;

	// Token: 0x04000E1F RID: 3615
	[ClientVar(ClientAdmin = true)]
	public static float windLerpSpeed = 0.1f;

	// Token: 0x04000E20 RID: 3616
	private const global::BaseEntity.Flags Flag_ThrottleUp = global::BaseEntity.Flags.Reserved1;

	// Token: 0x04000E21 RID: 3617
	private const global::BaseEntity.Flags Flag_Flying = global::BaseEntity.Flags.Reserved2;

	// Token: 0x04000E22 RID: 3618
	[Header("Drone")]
	public Rigidbody body;

	// Token: 0x04000E23 RID: 3619
	public Transform modelRoot;

	// Token: 0x04000E24 RID: 3620
	public bool killInWater = true;

	// Token: 0x04000E25 RID: 3621
	public bool enableGrounding = true;

	// Token: 0x04000E26 RID: 3622
	public bool keepAboveTerrain = true;

	// Token: 0x04000E27 RID: 3623
	public float groundTraceDist = 0.1f;

	// Token: 0x04000E28 RID: 3624
	public float groundCheckInterval = 0.05f;

	// Token: 0x04000E29 RID: 3625
	public float altitudeAcceleration = 10f;

	// Token: 0x04000E2A RID: 3626
	public float movementAcceleration = 10f;

	// Token: 0x04000E2B RID: 3627
	public float yawSpeed = 2f;

	// Token: 0x04000E2C RID: 3628
	public float uprightSpeed = 2f;

	// Token: 0x04000E2D RID: 3629
	public float uprightPrediction = 0.15f;

	// Token: 0x04000E2E RID: 3630
	public float uprightDot = 0.5f;

	// Token: 0x04000E2F RID: 3631
	public float leanWeight = 0.1f;

	// Token: 0x04000E30 RID: 3632
	public float leanMaxVelocity = 5f;

	// Token: 0x04000E31 RID: 3633
	public float hurtVelocityThreshold = 3f;

	// Token: 0x04000E32 RID: 3634
	public float hurtDamagePower = 3f;

	// Token: 0x04000E33 RID: 3635
	public float collisionDisableTime = 0.25f;

	// Token: 0x04000E34 RID: 3636
	public float pitchMin = -60f;

	// Token: 0x04000E35 RID: 3637
	public float pitchMax = 60f;

	// Token: 0x04000E36 RID: 3638
	public float pitchSensitivity = -5f;

	// Token: 0x04000E37 RID: 3639
	public bool disableWhenHurt;

	// Token: 0x04000E38 RID: 3640
	[Range(0f, 1f)]
	public float disableWhenHurtChance = 0.25f;

	// Token: 0x04000E39 RID: 3641
	public float playerCheckInterval = 0.1f;

	// Token: 0x04000E3A RID: 3642
	public float playerCheckRadius;

	// Token: 0x04000E3B RID: 3643
	public float deployYOffset = 0.1f;

	// Token: 0x04000E3C RID: 3644
	[Header("Sound")]
	public SoundDefinition movementLoopSoundDef;

	// Token: 0x04000E3D RID: 3645
	public SoundDefinition movementStartSoundDef;

	// Token: 0x04000E3E RID: 3646
	public SoundDefinition movementStopSoundDef;

	// Token: 0x04000E3F RID: 3647
	public AnimationCurve movementLoopPitchCurve;

	// Token: 0x04000E40 RID: 3648
	public float movementSpeedReference = 50f;

	// Token: 0x04000E41 RID: 3649
	[Header("Animation")]
	public float propellerMaxSpeed = 1000f;

	// Token: 0x04000E42 RID: 3650
	public float propellerAcceleration = 3f;

	// Token: 0x04000E43 RID: 3651
	public Transform propellerA;

	// Token: 0x04000E44 RID: 3652
	public Transform propellerB;

	// Token: 0x04000E45 RID: 3653
	public Transform propellerC;

	// Token: 0x04000E46 RID: 3654
	public Transform propellerD;

	// Token: 0x04000E47 RID: 3655
	private float pitch;

	// Token: 0x04000E48 RID: 3656
	protected Vector3? targetPosition;

	// Token: 0x04000E49 RID: 3657
	private global::Drone.DroneInputState currentInput;

	// Token: 0x04000E4A RID: 3658
	private float lastInputTime;

	// Token: 0x04000E4B RID: 3659
	private double lastCollision = -1000.0;

	// Token: 0x04000E4C RID: 3660
	private TimeSince lastGroundCheck;

	// Token: 0x04000E4D RID: 3661
	private bool isGrounded;

	// Token: 0x04000E4E RID: 3662
	private RealTimeSinceEx lastPlayerCheck;

	// Token: 0x170001EA RID: 490
	// (get) Token: 0x060015B6 RID: 5558 RVA: 0x0000441C File Offset: 0x0000261C
	public override bool RequiresMouse
	{
		get
		{
			return true;
		}
	}

	// Token: 0x170001EB RID: 491
	// (get) Token: 0x060015B7 RID: 5559 RVA: 0x000AAEB4 File Offset: 0x000A90B4
	public override float MaxRange
	{
		get
		{
			return global::Drone.maxControlRange;
		}
	}

	// Token: 0x060015B8 RID: 5560 RVA: 0x000AAEBB File Offset: 0x000A90BB
	public override void Spawn()
	{
		base.Spawn();
		this.isGrounded = true;
	}

	// Token: 0x170001EC RID: 492
	// (get) Token: 0x060015B9 RID: 5561 RVA: 0x0000441C File Offset: 0x0000261C
	public override bool CanAcceptInput
	{
		get
		{
			return true;
		}
	}

	// Token: 0x060015BA RID: 5562 RVA: 0x000AAECC File Offset: 0x000A90CC
	public override void StopControl(CameraViewerId viewerID)
	{
		if (viewerID == base.ControllingViewerId)
		{
			base.SetFlag(global::BaseEntity.Flags.Reserved1, false, false, false);
			base.SetFlag(global::BaseEntity.Flags.Reserved2, false, false, false);
			this.pitch = 0f;
			base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
		}
		base.StopControl(viewerID);
	}

	// Token: 0x060015BB RID: 5563 RVA: 0x000AAF34 File Offset: 0x000A9134
	public override void UserInput(InputState inputState, CameraViewerId viewerID)
	{
		if (viewerID != base.ControllingViewerId)
		{
			return;
		}
		this.currentInput.Reset();
		int num = (inputState.IsDown(BUTTON.FORWARD) ? 1 : 0) + (inputState.IsDown(BUTTON.BACKWARD) ? -1 : 0);
		int num2 = (inputState.IsDown(BUTTON.RIGHT) ? 1 : 0) + (inputState.IsDown(BUTTON.LEFT) ? -1 : 0);
		this.currentInput.movement = new Vector3((float)num2, 0f, (float)num).normalized;
		this.currentInput.throttle = (float)((inputState.IsDown(BUTTON.SPRINT) ? 1 : 0) + (inputState.IsDown(BUTTON.DUCK) ? -1 : 0));
		this.currentInput.yaw = inputState.current.mouseDelta.x;
		this.currentInput.pitch = inputState.current.mouseDelta.y;
		this.lastInputTime = Time.time;
		bool flag = false;
		bool flag2 = false;
		bool flag3 = this.currentInput.throttle > 0f;
		if (flag3 != base.HasFlag(global::BaseEntity.Flags.Reserved1))
		{
			base.SetFlag(global::BaseEntity.Flags.Reserved1, flag3, false, false);
			flag = true;
		}
		float b = this.pitch;
		this.pitch += this.currentInput.pitch * this.pitchSensitivity;
		this.pitch = Mathf.Clamp(this.pitch, this.pitchMin, this.pitchMax);
		if (!Mathf.Approximately(this.pitch, b))
		{
			flag2 = true;
		}
		if (flag2)
		{
			base.SendNetworkUpdateImmediate(false);
			return;
		}
		if (flag)
		{
			base.SendNetworkUpdate_Flags();
		}
	}

	// Token: 0x060015BC RID: 5564 RVA: 0x000AB0E0 File Offset: 0x000A92E0
	protected virtual void Update_Server()
	{
		if (!base.isServer || this.IsDead())
		{
			return;
		}
		if (base.IsBeingControlled || this.targetPosition == null)
		{
			return;
		}
		Vector3 position = base.transform.position;
		float height = TerrainMeta.HeightMap.GetHeight(position);
		Vector3 vector = this.targetPosition.Value - this.body.velocity * 0.5f;
		if (this.keepAboveTerrain)
		{
			vector.y = Mathf.Max(vector.y, height + 1f);
		}
		Vector2 a = vector.XZ2D();
		Vector2 b = position.XZ2D();
		Vector3 vector2;
		float num;
		(a - b).XZ3D().ToDirectionAndMagnitude(out vector2, out num);
		this.currentInput.Reset();
		this.lastInputTime = Time.time;
		if (position.y - height > 1f)
		{
			float d = Mathf.Clamp01(num);
			this.currentInput.movement = base.transform.InverseTransformVector(vector2) * d;
			if (num > 0.5f)
			{
				float y = base.transform.rotation.eulerAngles.y;
				float y2 = Quaternion.FromToRotation(Vector3.forward, vector2).eulerAngles.y;
				this.currentInput.yaw = Mathf.Clamp(Mathf.LerpAngle(y, y2, Time.deltaTime) - y, -2f, 2f);
			}
		}
		this.currentInput.throttle = Mathf.Clamp(vector.y - position.y, -1f, 1f);
	}

	// Token: 0x060015BD RID: 5565 RVA: 0x000AB278 File Offset: 0x000A9478
	public void FixedUpdate()
	{
		if (!base.isServer || this.IsDead())
		{
			return;
		}
		float num = this.WaterFactor();
		if (this.killInWater && num > 0f)
		{
			if (num > 0.99f)
			{
				base.Kill(global::BaseNetworkable.DestroyMode.None);
			}
			return;
		}
		if ((!base.IsBeingControlled && this.targetPosition == null) || (this.isGrounded && this.currentInput.throttle <= 0f))
		{
			if (base.HasFlag(global::BaseEntity.Flags.Reserved2))
			{
				base.SetFlag(global::BaseEntity.Flags.Reserved2, false, false, false);
				base.SendNetworkUpdate_Flags();
			}
			return;
		}
		if (this.playerCheckRadius > 0f && this.lastPlayerCheck > (double)this.playerCheckInterval)
		{
			this.lastPlayerCheck = 0.0;
			List<global::BasePlayer> list = Pool.GetList<global::BasePlayer>();
			Vis.Entities<global::BasePlayer>(base.transform.position, this.playerCheckRadius, list, 131072, QueryTriggerInteraction.Collide);
			if (list.Count > 0)
			{
				this.lastCollision = TimeEx.currentTimestamp;
			}
			Pool.FreeList<global::BasePlayer>(ref list);
		}
		double currentTimestamp = TimeEx.currentTimestamp;
		object obj = this.lastCollision > 0.0 && currentTimestamp - this.lastCollision < (double)this.collisionDisableTime;
		if (this.enableGrounding)
		{
			if (this.lastGroundCheck >= this.groundCheckInterval)
			{
				this.lastGroundCheck = 0f;
				RaycastHit raycastHit;
				bool flag = this.body.SweepTest(Vector3.down, out raycastHit, this.groundTraceDist, QueryTriggerInteraction.Ignore);
				if (!flag && this.isGrounded)
				{
					this.lastPlayerCheck = (double)this.playerCheckInterval;
				}
				this.isGrounded = flag;
			}
		}
		else
		{
			this.isGrounded = false;
		}
		Vector3 vector = base.transform.TransformDirection(this.currentInput.movement);
		Vector3 a;
		float num2;
		this.body.velocity.WithY(0f).ToDirectionAndMagnitude(out a, out num2);
		float num3 = Mathf.Clamp01(num2 / this.leanMaxVelocity);
		Vector3 a2 = Mathf.Approximately(vector.sqrMagnitude, 0f) ? (-num3 * a) : vector;
		Vector3 normalized = (Vector3.up + a2 * this.leanWeight * num3).normalized;
		Vector3 up = base.transform.up;
		float num4 = Mathf.Max(Vector3.Dot(normalized, up), 0f);
		object obj2 = obj;
		if (obj2 == null || this.isGrounded)
		{
			Vector3 a3 = (this.isGrounded && this.currentInput.throttle <= 0f) ? Vector3.zero : (-1f * base.transform.up * Physics.gravity.y);
			Vector3 b = this.isGrounded ? Vector3.zero : (vector * ((global::Drone.movementSpeedOverride > 0f) ? global::Drone.movementSpeedOverride : this.movementAcceleration));
			Vector3 b2 = base.transform.up * this.currentInput.throttle * ((global::Drone.altitudeSpeedOverride > 0f) ? global::Drone.altitudeSpeedOverride : this.altitudeAcceleration);
			Vector3 a4 = a3 + b + b2;
			this.body.AddForce(a4 * num4, ForceMode.Acceleration);
		}
		if (obj2 == null && !this.isGrounded)
		{
			Vector3 a5 = base.transform.TransformVector(0f, this.currentInput.yaw * this.yawSpeed, 0f);
			Vector3 a6 = Vector3.Cross(Quaternion.Euler(this.body.angularVelocity * this.uprightPrediction) * up, normalized) * this.uprightSpeed;
			float d = (num4 < this.uprightDot) ? 0f : num4;
			Vector3 a7 = a5 * num4 + a6 * d;
			this.body.AddTorque(a7 * num4, ForceMode.Acceleration);
		}
		bool flag2 = obj2 == 0;
		if (flag2 != base.HasFlag(global::BaseEntity.Flags.Reserved2))
		{
			base.SetFlag(global::BaseEntity.Flags.Reserved2, flag2, false, false);
			base.SendNetworkUpdate_Flags();
		}
	}

	// Token: 0x060015BE RID: 5566 RVA: 0x000AB690 File Offset: 0x000A9890
	public void OnCollisionEnter(Collision collision)
	{
		if (base.isServer)
		{
			this.lastCollision = TimeEx.currentTimestamp;
			float magnitude = collision.relativeVelocity.magnitude;
			if (magnitude > this.hurtVelocityThreshold)
			{
				base.Hurt(Mathf.Pow(magnitude, this.hurtDamagePower), DamageType.Fall, null, false);
			}
		}
	}

	// Token: 0x060015BF RID: 5567 RVA: 0x000AB6DE File Offset: 0x000A98DE
	public void OnCollisionStay()
	{
		if (base.isServer)
		{
			this.lastCollision = TimeEx.currentTimestamp;
		}
	}

	// Token: 0x060015C0 RID: 5568 RVA: 0x000AB6F3 File Offset: 0x000A98F3
	public override void Hurt(HitInfo info)
	{
		base.Hurt(info);
		if (base.isServer && this.disableWhenHurt && info.damageTypes.GetMajorityDamageType() != DamageType.Fall && UnityEngine.Random.value < this.disableWhenHurtChance)
		{
			this.lastCollision = TimeEx.currentTimestamp;
		}
	}

	// Token: 0x060015C1 RID: 5569 RVA: 0x0000627E File Offset: 0x0000447E
	public override float GetNetworkTime()
	{
		return Time.fixedTime;
	}

	// Token: 0x170001ED RID: 493
	// (get) Token: 0x060015C2 RID: 5570 RVA: 0x0000441C File Offset: 0x0000261C
	protected override bool PositionTickFixedTime
	{
		get
		{
			return true;
		}
	}

	// Token: 0x060015C3 RID: 5571 RVA: 0x000AB733 File Offset: 0x000A9933
	public override Vector3 GetLocalVelocityServer()
	{
		if (this.body == null)
		{
			return Vector3.zero;
		}
		return this.body.velocity;
	}

	// Token: 0x060015C4 RID: 5572 RVA: 0x000AB754 File Offset: 0x000A9954
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		if (!info.forDisk)
		{
			info.msg.drone = Pool.Get<ProtoBuf.Drone>();
			info.msg.drone.pitch = this.pitch;
		}
	}

	// Token: 0x060015C5 RID: 5573 RVA: 0x000AB78B File Offset: 0x000A998B
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.drone != null)
		{
			this.pitch = info.msg.drone.pitch;
		}
	}

	// Token: 0x060015C6 RID: 5574 RVA: 0x000AB7B8 File Offset: 0x000A99B8
	public virtual void Update()
	{
		this.Update_Server();
		if (base.HasFlag(global::BaseEntity.Flags.Reserved2))
		{
			Vector3 eulerAngles = this.viewEyes.localRotation.eulerAngles;
			eulerAngles.x = Mathf.LerpAngle(eulerAngles.x, this.pitch, 0.1f);
			this.viewEyes.localRotation = Quaternion.Euler(eulerAngles);
		}
	}

	// Token: 0x060015C7 RID: 5575 RVA: 0x000AB81A File Offset: 0x000A9A1A
	protected override bool CanChangeID(global::BasePlayer player)
	{
		return player != null && base.OwnerID == player.userID && !base.HasFlag(global::BaseEntity.Flags.Reserved2);
	}

	// Token: 0x060015C8 RID: 5576 RVA: 0x000AB843 File Offset: 0x000A9A43
	public override bool CanPickup(global::BasePlayer player)
	{
		return base.CanPickup(player) && !base.HasFlag(global::BaseEntity.Flags.Reserved2);
	}

	// Token: 0x060015C9 RID: 5577 RVA: 0x000AB85E File Offset: 0x000A9A5E
	public override void OnPickedUpPreItemMove(global::Item createdItem, global::BasePlayer player)
	{
		base.OnPickedUpPreItemMove(createdItem, player);
		if (player != null && player.userID == base.OwnerID)
		{
			createdItem.text = base.GetIdentifier();
		}
	}

	// Token: 0x060015CA RID: 5578 RVA: 0x000AB88C File Offset: 0x000A9A8C
	public override void OnDeployed(global::BaseEntity parent, global::BasePlayer deployedBy, global::Item fromItem)
	{
		base.OnDeployed(parent, deployedBy, fromItem);
		base.transform.position += base.transform.up * this.deployYOffset;
		if (this.body != null)
		{
			this.body.velocity = Vector3.zero;
			this.body.angularVelocity = Vector3.zero;
		}
		if (fromItem != null && !string.IsNullOrEmpty(fromItem.text) && global::ComputerStation.IsValidIdentifier(fromItem.text))
		{
			base.UpdateIdentifier(fromItem.text, false);
		}
	}

	// Token: 0x060015CB RID: 5579 RVA: 0x0000441C File Offset: 0x0000261C
	public override bool ShouldNetworkOwnerInfo()
	{
		return true;
	}

	// Token: 0x060015CC RID: 5580 RVA: 0x00007A3C File Offset: 0x00005C3C
	public override bool ShouldInheritNetworkGroup()
	{
		return false;
	}

	// Token: 0x060015CD RID: 5581 RVA: 0x000AB926 File Offset: 0x000A9B26
	public override float MaxVelocity()
	{
		return 30f;
	}

	// Token: 0x02000C1E RID: 3102
	private struct DroneInputState
	{
		// Token: 0x04004224 RID: 16932
		public Vector3 movement;

		// Token: 0x04004225 RID: 16933
		public float throttle;

		// Token: 0x04004226 RID: 16934
		public float pitch;

		// Token: 0x04004227 RID: 16935
		public float yaw;

		// Token: 0x06004E03 RID: 19971 RVA: 0x001A1DE2 File Offset: 0x0019FFE2
		public void Reset()
		{
			this.movement = Vector3.zero;
			this.pitch = 0f;
			this.yaw = 0f;
		}
	}
}
