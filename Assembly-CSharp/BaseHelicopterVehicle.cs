using System;
using System.Collections.Generic;
using Rust;
using UnityEngine;

// Token: 0x0200047B RID: 1147
public class BaseHelicopterVehicle : BaseVehicle
{
	// Token: 0x04001E20 RID: 7712
	[Header("Helicopter")]
	public float engineThrustMax;

	// Token: 0x04001E21 RID: 7713
	public Vector3 torqueScale;

	// Token: 0x04001E22 RID: 7714
	public Transform com;

	// Token: 0x04001E23 RID: 7715
	public GameObject[] killTriggers;

	// Token: 0x04001E24 RID: 7716
	[Header("Effects")]
	public Transform[] GroundPoints;

	// Token: 0x04001E25 RID: 7717
	public Transform[] GroundEffects;

	// Token: 0x04001E26 RID: 7718
	public GameObjectRef serverGibs;

	// Token: 0x04001E27 RID: 7719
	public GameObjectRef explosionEffect;

	// Token: 0x04001E28 RID: 7720
	public GameObjectRef fireBall;

	// Token: 0x04001E29 RID: 7721
	public GameObjectRef impactEffectSmall;

	// Token: 0x04001E2A RID: 7722
	public GameObjectRef impactEffectLarge;

	// Token: 0x04001E2B RID: 7723
	[Header("Sounds")]
	public SoundDefinition flightEngineSoundDef;

	// Token: 0x04001E2C RID: 7724
	public SoundDefinition flightThwopsSoundDef;

	// Token: 0x04001E2D RID: 7725
	public float rotorGainModSmoothing = 0.25f;

	// Token: 0x04001E2E RID: 7726
	public float engineGainMin = 0.5f;

	// Token: 0x04001E2F RID: 7727
	public float engineGainMax = 1f;

	// Token: 0x04001E30 RID: 7728
	public float thwopGainMin = 0.5f;

	// Token: 0x04001E31 RID: 7729
	public float thwopGainMax = 1f;

	// Token: 0x04001E32 RID: 7730
	public float currentThrottle;

	// Token: 0x04001E33 RID: 7731
	public float avgThrust;

	// Token: 0x04001E34 RID: 7732
	public float liftDotMax = 0.75f;

	// Token: 0x04001E35 RID: 7733
	public float altForceDotMin = 0.85f;

	// Token: 0x04001E36 RID: 7734
	public float liftFraction = 0.25f;

	// Token: 0x04001E37 RID: 7735
	public float thrustLerpSpeed = 1f;

	// Token: 0x04001E38 RID: 7736
	private float avgTerrainHeight;

	// Token: 0x04001E39 RID: 7737
	public const BaseEntity.Flags Flag_InternalLights = BaseEntity.Flags.Reserved6;

	// Token: 0x04001E3A RID: 7738
	protected BaseHelicopterVehicle.HelicopterInputState currentInputState = new BaseHelicopterVehicle.HelicopterInputState();

	// Token: 0x04001E3B RID: 7739
	protected float lastPlayerInputTime;

	// Token: 0x04001E3C RID: 7740
	protected float hoverForceScale = 0.99f;

	// Token: 0x04001E3D RID: 7741
	protected Vector3 damageTorque;

	// Token: 0x04001E3E RID: 7742
	private float nextDamageTime;

	// Token: 0x04001E3F RID: 7743
	private float nextEffectTime;

	// Token: 0x04001E40 RID: 7744
	private float pendingImpactDamage;

	// Token: 0x060025D2 RID: 9682 RVA: 0x000EEDCE File Offset: 0x000ECFCE
	public virtual float GetServiceCeiling()
	{
		return 1000f;
	}

	// Token: 0x060025D3 RID: 9683 RVA: 0x000EB88D File Offset: 0x000E9A8D
	public override float MaxVelocity()
	{
		return 50f;
	}

	// Token: 0x060025D4 RID: 9684 RVA: 0x000EEDD5 File Offset: 0x000ECFD5
	public override void ServerInit()
	{
		base.ServerInit();
		this.rigidBody.centerOfMass = this.com.localPosition;
	}

	// Token: 0x060025D5 RID: 9685 RVA: 0x000EEDF3 File Offset: 0x000ECFF3
	public float MouseToBinary(float amount)
	{
		return Mathf.Clamp(amount, -1f, 1f);
	}

	// Token: 0x060025D6 RID: 9686 RVA: 0x000EEE08 File Offset: 0x000ED008
	public virtual void PilotInput(InputState inputState, BasePlayer player)
	{
		this.currentInputState.Reset();
		this.currentInputState.throttle = (inputState.IsDown(BUTTON.FORWARD) ? 1f : 0f);
		this.currentInputState.throttle -= ((inputState.IsDown(BUTTON.BACKWARD) || inputState.IsDown(BUTTON.DUCK)) ? 1f : 0f);
		this.currentInputState.pitch = inputState.current.mouseDelta.y;
		this.currentInputState.roll = -inputState.current.mouseDelta.x;
		this.currentInputState.yaw = (inputState.IsDown(BUTTON.RIGHT) ? 1f : 0f);
		this.currentInputState.yaw -= (inputState.IsDown(BUTTON.LEFT) ? 1f : 0f);
		this.currentInputState.pitch = this.MouseToBinary(this.currentInputState.pitch);
		this.currentInputState.roll = this.MouseToBinary(this.currentInputState.roll);
		this.lastPlayerInputTime = Time.time;
	}

	// Token: 0x060025D7 RID: 9687 RVA: 0x000EEF33 File Offset: 0x000ED133
	public override void PlayerServerInput(InputState inputState, BasePlayer player)
	{
		if (base.IsDriver(player))
		{
			this.PilotInput(inputState, player);
		}
	}

	// Token: 0x060025D8 RID: 9688 RVA: 0x000EEF48 File Offset: 0x000ED148
	public virtual void SetDefaultInputState()
	{
		this.currentInputState.Reset();
		if (base.HasDriver())
		{
			float num = Vector3.Dot(Vector3.up, base.transform.right);
			float num2 = Vector3.Dot(Vector3.up, base.transform.forward);
			this.currentInputState.roll = ((num < 0f) ? 1f : 0f);
			this.currentInputState.roll -= ((num > 0f) ? 1f : 0f);
			if (num2 < --0f)
			{
				this.currentInputState.pitch = -1f;
				return;
			}
			if (num2 > 0f)
			{
				this.currentInputState.pitch = 1f;
				return;
			}
		}
		else
		{
			this.currentInputState.throttle = -1f;
		}
	}

	// Token: 0x060025D9 RID: 9689 RVA: 0x0000441C File Offset: 0x0000261C
	public virtual bool IsEnginePowered()
	{
		return true;
	}

	// Token: 0x060025DA RID: 9690 RVA: 0x000EF020 File Offset: 0x000ED220
	public override void VehicleFixedUpdate()
	{
		base.VehicleFixedUpdate();
		if (Time.time > this.lastPlayerInputTime + 0.5f)
		{
			this.SetDefaultInputState();
		}
		base.EnableGlobalBroadcast(this.IsEngineOn());
		this.MovementUpdate();
		base.SetFlag(BaseEntity.Flags.Reserved6, TOD_Sky.Instance.IsNight, false, true);
		foreach (GameObject gameObject in this.killTriggers)
		{
			bool active = this.rigidBody.velocity.y < 0f;
			gameObject.SetActive(active);
		}
	}

	// Token: 0x060025DB RID: 9691 RVA: 0x000EF0AB File Offset: 0x000ED2AB
	public override void LightToggle(BasePlayer player)
	{
		if (base.IsDriver(player))
		{
			base.SetFlag(BaseEntity.Flags.Reserved5, !base.HasFlag(BaseEntity.Flags.Reserved5), false, true);
		}
	}

	// Token: 0x060025DC RID: 9692 RVA: 0x0000441C File Offset: 0x0000261C
	public virtual bool ShouldApplyHoverForce()
	{
		return true;
	}

	// Token: 0x060025DD RID: 9693 RVA: 0x0000441C File Offset: 0x0000261C
	public virtual bool IsEngineOn()
	{
		return true;
	}

	// Token: 0x060025DE RID: 9694 RVA: 0x000EF0D1 File Offset: 0x000ED2D1
	public void ClearDamageTorque()
	{
		this.SetDamageTorque(Vector3.zero);
	}

	// Token: 0x060025DF RID: 9695 RVA: 0x000EF0DE File Offset: 0x000ED2DE
	public void SetDamageTorque(Vector3 newTorque)
	{
		this.damageTorque = newTorque;
	}

	// Token: 0x060025E0 RID: 9696 RVA: 0x000EF0E7 File Offset: 0x000ED2E7
	public void AddDamageTorque(Vector3 torqueToAdd)
	{
		this.damageTorque += torqueToAdd;
	}

	// Token: 0x060025E1 RID: 9697 RVA: 0x000EF0FC File Offset: 0x000ED2FC
	public virtual void MovementUpdate()
	{
		if (!this.IsEngineOn())
		{
			return;
		}
		BaseHelicopterVehicle.HelicopterInputState helicopterInputState = this.currentInputState;
		this.currentThrottle = Mathf.Lerp(this.currentThrottle, helicopterInputState.throttle, 2f * Time.fixedDeltaTime);
		this.currentThrottle = Mathf.Clamp(this.currentThrottle, -0.8f, 1f);
		if (helicopterInputState.pitch != 0f || helicopterInputState.roll != 0f || helicopterInputState.yaw != 0f)
		{
			this.rigidBody.AddRelativeTorque(new Vector3(helicopterInputState.pitch * this.torqueScale.x, helicopterInputState.yaw * this.torqueScale.y, helicopterInputState.roll * this.torqueScale.z), ForceMode.Force);
		}
		if (this.damageTorque != Vector3.zero)
		{
			this.rigidBody.AddRelativeTorque(new Vector3(this.damageTorque.x, this.damageTorque.y, this.damageTorque.z), ForceMode.Force);
		}
		this.avgThrust = Mathf.Lerp(this.avgThrust, this.engineThrustMax * this.currentThrottle, Time.fixedDeltaTime * this.thrustLerpSpeed);
		float value = Mathf.Clamp01(Vector3.Dot(base.transform.up, Vector3.up));
		float num = Mathf.InverseLerp(this.liftDotMax, 1f, value);
		float serviceCeiling = this.GetServiceCeiling();
		this.avgTerrainHeight = Mathf.Lerp(this.avgTerrainHeight, TerrainMeta.HeightMap.GetHeight(base.transform.position), Time.deltaTime);
		float num2 = 1f - Mathf.InverseLerp(this.avgTerrainHeight + serviceCeiling - 20f, this.avgTerrainHeight + serviceCeiling, base.transform.position.y);
		num *= num2;
		float d = 1f - Mathf.InverseLerp(this.altForceDotMin, 1f, value);
		Vector3 force = Vector3.up * this.engineThrustMax * this.liftFraction * this.currentThrottle * num;
		Vector3 force2 = (base.transform.up - Vector3.up).normalized * this.engineThrustMax * this.currentThrottle * d;
		if (this.ShouldApplyHoverForce())
		{
			float d2 = this.rigidBody.mass * -Physics.gravity.y;
			this.rigidBody.AddForce(base.transform.up * d2 * num * this.hoverForceScale, ForceMode.Force);
		}
		this.rigidBody.AddForce(force, ForceMode.Force);
		this.rigidBody.AddForce(force2, ForceMode.Force);
	}

	// Token: 0x060025E2 RID: 9698 RVA: 0x000EF3BC File Offset: 0x000ED5BC
	public void DelayedImpactDamage()
	{
		float explosionForceMultiplier = this.explosionForceMultiplier;
		this.explosionForceMultiplier = 0f;
		base.Hurt(this.pendingImpactDamage * this.MaxHealth(), DamageType.Explosion, this, false);
		this.pendingImpactDamage = 0f;
		this.explosionForceMultiplier = explosionForceMultiplier;
	}

	// Token: 0x060025E3 RID: 9699 RVA: 0x0000441C File Offset: 0x0000261C
	public virtual bool CollisionDamageEnabled()
	{
		return true;
	}

	// Token: 0x060025E4 RID: 9700 RVA: 0x000EF404 File Offset: 0x000ED604
	public void ProcessCollision(Collision collision)
	{
		if (base.isClient)
		{
			return;
		}
		if (!this.CollisionDamageEnabled())
		{
			return;
		}
		if (Time.time < this.nextDamageTime)
		{
			return;
		}
		float magnitude = collision.relativeVelocity.magnitude;
		if (collision.gameObject && (1 << collision.collider.gameObject.layer & 1218543873) <= 0)
		{
			return;
		}
		float num = Mathf.InverseLerp(5f, 30f, magnitude);
		if (num > 0f)
		{
			this.pendingImpactDamage += Mathf.Max(num, 0.15f);
			if (Vector3.Dot(base.transform.up, Vector3.up) < 0.5f)
			{
				this.pendingImpactDamage *= 5f;
			}
			if (Time.time > this.nextEffectTime)
			{
				this.nextEffectTime = Time.time + 0.25f;
				if (this.impactEffectSmall.isValid)
				{
					Vector3 vector = collision.GetContact(0).point;
					vector += (base.transform.position - vector) * 0.25f;
					Effect.server.Run(this.impactEffectSmall.resourcePath, vector, base.transform.up, null, false);
				}
			}
			this.rigidBody.AddForceAtPosition(collision.GetContact(0).normal * (1f + 3f * num), collision.GetContact(0).point, ForceMode.VelocityChange);
			this.nextDamageTime = Time.time + 0.333f;
			base.Invoke(new Action(this.DelayedImpactDamage), 0.015f);
		}
	}

	// Token: 0x060025E5 RID: 9701 RVA: 0x000EF5B6 File Offset: 0x000ED7B6
	private void OnCollisionEnter(Collision collision)
	{
		this.ProcessCollision(collision);
	}

	// Token: 0x060025E6 RID: 9702 RVA: 0x000EF5C0 File Offset: 0x000ED7C0
	public override void OnKilled(HitInfo info)
	{
		if (base.isClient)
		{
			base.OnKilled(info);
			return;
		}
		if (this.explosionEffect.isValid)
		{
			Effect.server.Run(this.explosionEffect.resourcePath, base.transform.position, Vector3.up, null, true);
		}
		Vector3 vector = this.rigidBody.velocity * 0.25f;
		List<ServerGib> list = null;
		if (this.serverGibs.isValid)
		{
			GameObject gibSource = this.serverGibs.Get().GetComponent<ServerGib>()._gibSource;
			list = ServerGib.CreateGibs(this.serverGibs.resourcePath, base.gameObject, gibSource, vector, 3f);
		}
		Vector3 vector2 = base.CenterPoint();
		if (this.fireBall.isValid && !base.InSafeZone())
		{
			for (int i = 0; i < 12; i++)
			{
				BaseEntity baseEntity = GameManager.server.CreateEntity(this.fireBall.resourcePath, vector2, base.transform.rotation, true);
				if (baseEntity)
				{
					float min = 3f;
					float max = 10f;
					Vector3 onUnitSphere = UnityEngine.Random.onUnitSphere;
					onUnitSphere.Normalize();
					float num = UnityEngine.Random.Range(0.5f, 4f);
					RaycastHit raycastHit;
					bool flag = Physics.Raycast(vector2, onUnitSphere, out raycastHit, num, 1218652417);
					Vector3 vector3 = raycastHit.point;
					if (!flag)
					{
						vector3 = vector2 + onUnitSphere * num;
					}
					vector3 -= onUnitSphere * 0.5f;
					baseEntity.transform.position = vector3;
					Collider component = baseEntity.GetComponent<Collider>();
					baseEntity.Spawn();
					baseEntity.SetVelocity(vector + onUnitSphere * UnityEngine.Random.Range(min, max));
					if (list != null)
					{
						foreach (ServerGib serverGib in list)
						{
							Physics.IgnoreCollision(component, serverGib.GetCollider(), true);
						}
					}
				}
			}
		}
		base.OnKilled(info);
	}

	// Token: 0x02000CFB RID: 3323
	public class HelicopterInputState
	{
		// Token: 0x040045D0 RID: 17872
		public float throttle;

		// Token: 0x040045D1 RID: 17873
		public float roll;

		// Token: 0x040045D2 RID: 17874
		public float yaw;

		// Token: 0x040045D3 RID: 17875
		public float pitch;

		// Token: 0x040045D4 RID: 17876
		public bool groundControl;

		// Token: 0x06004FEE RID: 20462 RVA: 0x001A7444 File Offset: 0x001A5644
		public void Reset()
		{
			this.throttle = 0f;
			this.roll = 0f;
			this.yaw = 0f;
			this.pitch = 0f;
			this.groundControl = false;
		}
	}
}
