using System;
using System.Runtime.CompilerServices;
using UnityEngine;

// Token: 0x02000476 RID: 1142
public class CarPhysics<TCar> where TCar : BaseVehicle, CarPhysics<TCar>.ICar
{
	// Token: 0x04001DE9 RID: 7657
	private readonly CarPhysics<TCar>.ServerWheelData[] wheelData;

	// Token: 0x04001DEA RID: 7658
	private readonly TCar car;

	// Token: 0x04001DEB RID: 7659
	private readonly Transform transform;

	// Token: 0x04001DEC RID: 7660
	private readonly Rigidbody rBody;

	// Token: 0x04001DED RID: 7661
	private readonly CarSettings vehicleSettings;

	// Token: 0x04001DEE RID: 7662
	private float speedAngle;

	// Token: 0x04001DEF RID: 7663
	private bool wasSleeping = true;

	// Token: 0x04001DF0 RID: 7664
	private bool hasDriver;

	// Token: 0x04001DF1 RID: 7665
	private bool hadDriver;

	// Token: 0x04001DF2 RID: 7666
	private float lastMovingTime = float.MinValue;

	// Token: 0x04001DF3 RID: 7667
	private WheelFrictionCurve zeroFriction = new WheelFrictionCurve
	{
		stiffness = 0f
	};

	// Token: 0x04001DF4 RID: 7668
	private Vector3 prevLocalCOM;

	// Token: 0x04001DF5 RID: 7669
	private readonly float midWheelPos;

	// Token: 0x04001DF6 RID: 7670
	private const bool WHEEL_HIT_CORRECTION = true;

	// Token: 0x04001DF7 RID: 7671
	private const float SLEEP_SPEED = 0.25f;

	// Token: 0x04001DF8 RID: 7672
	private const float SLEEP_DELAY = 10f;

	// Token: 0x04001DF9 RID: 7673
	private const float AIR_DRAG = 0.25f;

	// Token: 0x04001DFA RID: 7674
	private const float DEFAULT_GROUND_GRIP = 0.75f;

	// Token: 0x04001DFB RID: 7675
	private const float ROAD_GROUND_GRIP = 1f;

	// Token: 0x04001DFC RID: 7676
	private const float ICE_GROUND_GRIP = 0.25f;

	// Token: 0x04001DFD RID: 7677
	private bool slowSpeedExitFlag;

	// Token: 0x04001DFE RID: 7678
	private const float SLOW_SPEED_EXIT_SPEED = 4f;

	// Token: 0x04001DFF RID: 7679
	private TimeSince timeSinceWaterCheck;

	// Token: 0x17000315 RID: 789
	// (get) Token: 0x06002591 RID: 9617 RVA: 0x000ED22C File Offset: 0x000EB42C
	// (set) Token: 0x06002592 RID: 9618 RVA: 0x000ED234 File Offset: 0x000EB434
	public float DriveWheelVelocity { get; private set; }

	// Token: 0x17000316 RID: 790
	// (get) Token: 0x06002593 RID: 9619 RVA: 0x000ED23D File Offset: 0x000EB43D
	// (set) Token: 0x06002594 RID: 9620 RVA: 0x000ED245 File Offset: 0x000EB445
	public float DriveWheelSlip { get; private set; }

	// Token: 0x17000317 RID: 791
	// (get) Token: 0x06002595 RID: 9621 RVA: 0x000ED24E File Offset: 0x000EB44E
	// (set) Token: 0x06002596 RID: 9622 RVA: 0x000ED256 File Offset: 0x000EB456
	public float SteerAngle { get; private set; }

	// Token: 0x17000318 RID: 792
	// (get) Token: 0x06002597 RID: 9623 RVA: 0x000ED25F File Offset: 0x000EB45F
	// (set) Token: 0x06002598 RID: 9624 RVA: 0x000ED267 File Offset: 0x000EB467
	public float TankThrottleLeft { get; private set; }

	// Token: 0x17000319 RID: 793
	// (get) Token: 0x06002599 RID: 9625 RVA: 0x000ED270 File Offset: 0x000EB470
	// (set) Token: 0x0600259A RID: 9626 RVA: 0x000ED278 File Offset: 0x000EB478
	public float TankThrottleRight { get; private set; }

	// Token: 0x1700031A RID: 794
	// (get) Token: 0x0600259B RID: 9627 RVA: 0x000ED281 File Offset: 0x000EB481
	private bool InSlowSpeedExitMode
	{
		get
		{
			return !this.hasDriver && this.slowSpeedExitFlag;
		}
	}

	// Token: 0x0600259C RID: 9628 RVA: 0x000ED294 File Offset: 0x000EB494
	public CarPhysics(TCar car, Transform transform, Rigidbody rBody, CarSettings vehicleSettings)
	{
		CarPhysics<TCar>.<>c__DisplayClass47_0 CS$<>8__locals1;
		CS$<>8__locals1.transform = transform;
		base..ctor();
		CS$<>8__locals1.<>4__this = this;
		this.car = car;
		this.transform = CS$<>8__locals1.transform;
		this.rBody = rBody;
		this.vehicleSettings = vehicleSettings;
		this.timeSinceWaterCheck = default(TimeSince);
		this.timeSinceWaterCheck = float.MaxValue;
		this.prevLocalCOM = rBody.centerOfMass;
		CarWheel[] wheels = car.GetWheels();
		this.wheelData = new CarPhysics<TCar>.ServerWheelData[wheels.Length];
		for (int i = 0; i < this.wheelData.Length; i++)
		{
			this.wheelData[i] = this.<.ctor>g__AddWheel|47_0(wheels[i], ref CS$<>8__locals1);
		}
		this.midWheelPos = car.GetWheelsMidPos();
		this.wheelData[0].wheel.wheelCollider.ConfigureVehicleSubsteps(1000f, 1, 1);
		this.lastMovingTime = Time.realtimeSinceStartup;
	}

	// Token: 0x0600259D RID: 9629 RVA: 0x000ED3A8 File Offset: 0x000EB5A8
	public void FixedUpdate(float dt, float speed)
	{
		if (this.rBody.centerOfMass != this.prevLocalCOM)
		{
			this.COMChanged();
		}
		float num = Mathf.Abs(speed);
		this.hasDriver = this.car.HasDriver();
		if (!this.hasDriver && this.hadDriver)
		{
			if (num <= 4f)
			{
				this.slowSpeedExitFlag = true;
			}
		}
		else if (this.hasDriver && !this.hadDriver)
		{
			this.slowSpeedExitFlag = false;
		}
		if ((this.hasDriver || !this.vehicleSettings.canSleep) && this.rBody.IsSleeping())
		{
			this.rBody.WakeUp();
		}
		if (!this.rBody.IsSleeping())
		{
			if ((this.wasSleeping && !this.rBody.isKinematic) || num > 0.25f || Mathf.Abs(this.rBody.angularVelocity.magnitude) > 0.25f)
			{
				this.lastMovingTime = Time.time;
			}
			bool flag = this.vehicleSettings.canSleep && !this.hasDriver && Time.time > this.lastMovingTime + 10f;
			if (flag && (this.car.GetParentEntity() as BaseVehicle).IsValid())
			{
				flag = false;
			}
			if (flag)
			{
				for (int i = 0; i < this.wheelData.Length; i++)
				{
					CarPhysics<TCar>.ServerWheelData serverWheelData = this.wheelData[i];
					serverWheelData.wheelCollider.motorTorque = 0f;
					serverWheelData.wheelCollider.brakeTorque = 0f;
					serverWheelData.wheelCollider.steerAngle = 0f;
				}
				this.rBody.Sleep();
			}
			else
			{
				this.speedAngle = Vector3.Angle(this.rBody.velocity, this.transform.forward) * Mathf.Sign(Vector3.Dot(this.rBody.velocity, this.transform.right));
				float num2 = this.car.GetMaxDriveForce();
				float maxForwardSpeed = this.car.GetMaxForwardSpeed();
				float num3 = this.car.IsOn() ? this.car.GetThrottleInput() : 0f;
				float steerInput = this.car.GetSteerInput();
				float brakeInput = this.InSlowSpeedExitMode ? 1f : this.car.GetBrakeInput();
				float num4 = 1f;
				if (num < 3f)
				{
					num4 = 2.75f;
				}
				else if (num < 9f)
				{
					float t = Mathf.InverseLerp(9f, 3f, num);
					num4 = Mathf.Lerp(1f, 2.75f, t);
				}
				num2 *= num4;
				this.ComputeSteerAngle(num3, steerInput, dt, speed);
				if (this.timeSinceWaterCheck > 0.25f)
				{
					float a = this.car.WaterFactor();
					float b = 0f;
					TriggerVehicleDrag triggerVehicleDrag;
					if (this.car.FindTrigger<TriggerVehicleDrag>(out triggerVehicleDrag))
					{
						b = triggerVehicleDrag.vehicleDrag;
					}
					float a2 = (num3 != 0f) ? 0f : 0.25f;
					float num5 = Mathf.Max(a, b);
					num5 = Mathf.Max(num5, this.car.GetModifiedDrag());
					this.rBody.drag = Mathf.Max(a2, num5);
					this.rBody.angularDrag = num5 * 0.5f;
					this.timeSinceWaterCheck = 0f;
				}
				int num6 = 0;
				float num7 = 0f;
				bool flag2 = !this.hasDriver && this.rBody.velocity.magnitude < 2.5f && this.car.timeSinceLastPush > 2f;
				for (int j = 0; j < this.wheelData.Length; j++)
				{
					CarPhysics<TCar>.ServerWheelData serverWheelData2 = this.wheelData[j];
					serverWheelData2.wheelCollider.motorTorque = 1E-05f;
					if (flag2 && this.car.OnSurface != VehicleTerrainHandler.Surface.Frictionless)
					{
						serverWheelData2.wheelCollider.brakeTorque = 10000f;
					}
					else
					{
						serverWheelData2.wheelCollider.brakeTorque = 0f;
					}
					if (serverWheelData2.wheel.steerWheel)
					{
						serverWheelData2.wheel.wheelCollider.steerAngle = (serverWheelData2.isFrontWheel ? this.SteerAngle : (this.vehicleSettings.rearWheelSteer * -this.SteerAngle));
					}
					this.UpdateSuspension(serverWheelData2);
					if (serverWheelData2.isGrounded)
					{
						num6++;
						num7 += this.wheelData[j].downforce;
					}
				}
				this.AdjustHitForces(num6, num7 / (float)num6);
				for (int k = 0; k < this.wheelData.Length; k++)
				{
					CarPhysics<TCar>.ServerWheelData wd = this.wheelData[k];
					this.UpdateLocalFrame(wd, dt);
					this.ComputeTyreForces(wd, speed, num2, maxForwardSpeed, num3, steerInput, brakeInput, num4);
					this.ApplyTyreForces(wd, num3, steerInput, speed);
				}
				this.ComputeOverallForces();
			}
			this.wasSleeping = false;
		}
		else
		{
			this.wasSleeping = true;
		}
		this.hadDriver = this.hasDriver;
	}

	// Token: 0x0600259E RID: 9630 RVA: 0x000ED8D8 File Offset: 0x000EBAD8
	public bool IsGrounded()
	{
		int num = 0;
		for (int i = 0; i < this.wheelData.Length; i++)
		{
			if (this.wheelData[i].isGrounded)
			{
				num++;
			}
			if (num >= 2)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x0600259F RID: 9631 RVA: 0x000ED914 File Offset: 0x000EBB14
	private void COMChanged()
	{
		for (int i = 0; i < this.wheelData.Length; i++)
		{
			CarPhysics<TCar>.ServerWheelData serverWheelData = this.wheelData[i];
			serverWheelData.forceDistance = this.GetWheelForceDistance(serverWheelData.wheel.wheelCollider);
		}
		this.prevLocalCOM = this.rBody.centerOfMass;
	}

	// Token: 0x060025A0 RID: 9632 RVA: 0x000ED968 File Offset: 0x000EBB68
	private void ComputeSteerAngle(float throttleInput, float steerInput, float dt, float speed)
	{
		if (this.vehicleSettings.tankSteering)
		{
			this.SteerAngle = 0f;
			this.ComputeTankSteeringThrottle(throttleInput, steerInput, speed);
			return;
		}
		float num = this.vehicleSettings.maxSteerAngle * steerInput;
		float num2 = Mathf.InverseLerp(0f, this.vehicleSettings.minSteerLimitSpeed, speed);
		if (this.vehicleSettings.steeringLimit)
		{
			float num3 = Mathf.Lerp(this.vehicleSettings.maxSteerAngle, this.vehicleSettings.minSteerLimitAngle, num2);
			num = Mathf.Clamp(num, -num3, num3);
		}
		float num4 = 0f;
		if (this.vehicleSettings.steeringAssist)
		{
			float num5 = Mathf.InverseLerp(0.1f, 3f, speed);
			num4 = this.speedAngle * this.vehicleSettings.steeringAssistRatio * num5 * Mathf.InverseLerp(2f, 3f, Mathf.Abs(this.speedAngle));
		}
		float num6 = Mathf.Clamp(num + num4, -this.vehicleSettings.maxSteerAngle, this.vehicleSettings.maxSteerAngle);
		if (this.SteerAngle != num6)
		{
			float num7 = 1f - num2 * 0.7f;
			float num9;
			if ((this.SteerAngle == 0f || Mathf.Sign(num6) == Mathf.Sign(this.SteerAngle)) && Mathf.Abs(num6) > Mathf.Abs(this.SteerAngle))
			{
				float num8 = this.SteerAngle / this.vehicleSettings.maxSteerAngle;
				num9 = Mathf.Lerp(this.vehicleSettings.steerMinLerpSpeed * num7, this.vehicleSettings.steerMaxLerpSpeed * num7, num8 * num8);
			}
			else
			{
				num9 = this.vehicleSettings.steerReturnLerpSpeed * num7;
			}
			if (this.car.GetSteerModInput())
			{
				num9 *= 1.5f;
			}
			this.SteerAngle = Mathf.MoveTowards(this.SteerAngle, num6, dt * num9);
		}
	}

	// Token: 0x060025A1 RID: 9633 RVA: 0x000EDB3C File Offset: 0x000EBD3C
	private float GetWheelForceDistance(WheelCollider col)
	{
		return this.rBody.centerOfMass.y - this.transform.InverseTransformPoint(col.transform.position).y + col.radius + (1f - col.suspensionSpring.targetPosition) * col.suspensionDistance;
	}

	// Token: 0x060025A2 RID: 9634 RVA: 0x000EDB98 File Offset: 0x000EBD98
	private void UpdateSuspension(CarPhysics<TCar>.ServerWheelData wd)
	{
		wd.isGrounded = wd.wheelCollider.GetGroundHit(out wd.hit);
		wd.origin = wd.wheelColliderTransform.TransformPoint(wd.wheelCollider.center);
		RaycastHit raycastHit;
		if (wd.isGrounded && GamePhysics.Trace(new Ray(wd.origin, -wd.wheelColliderTransform.up), 0f, out raycastHit, wd.wheelCollider.suspensionDistance + wd.wheelCollider.radius, 1235321089, QueryTriggerInteraction.Ignore, null))
		{
			wd.hit.point = raycastHit.point;
			wd.hit.normal = raycastHit.normal;
		}
		if (wd.isGrounded)
		{
			if (wd.hit.force < 0f)
			{
				wd.hit.force = 0f;
			}
			wd.downforce = wd.hit.force;
			return;
		}
		wd.downforce = 0f;
	}

	// Token: 0x060025A3 RID: 9635 RVA: 0x000EDC94 File Offset: 0x000EBE94
	private void AdjustHitForces(int groundedWheels, float neutralForcePerWheel)
	{
		float num = neutralForcePerWheel * 0.25f;
		for (int i = 0; i < this.wheelData.Length; i++)
		{
			CarPhysics<TCar>.ServerWheelData serverWheelData = this.wheelData[i];
			if (serverWheelData.isGrounded && serverWheelData.downforce < num)
			{
				if (groundedWheels == 1)
				{
					serverWheelData.downforce = num;
				}
				else
				{
					float a = (num - serverWheelData.downforce) / (float)(groundedWheels - 1);
					serverWheelData.downforce = num;
					for (int j = 0; j < this.wheelData.Length; j++)
					{
						CarPhysics<TCar>.ServerWheelData serverWheelData2 = this.wheelData[j];
						if (serverWheelData2.isGrounded && serverWheelData2.downforce > num)
						{
							float num2 = Mathf.Min(a, serverWheelData2.downforce - num);
							serverWheelData2.downforce -= num2;
						}
					}
				}
			}
		}
	}

	// Token: 0x060025A4 RID: 9636 RVA: 0x000EDD58 File Offset: 0x000EBF58
	private void UpdateLocalFrame(CarPhysics<TCar>.ServerWheelData wd, float dt)
	{
		if (!wd.isGrounded)
		{
			wd.hit.point = wd.origin - wd.wheelColliderTransform.up * (wd.wheelCollider.suspensionDistance + wd.wheelCollider.radius);
			wd.hit.normal = wd.wheelColliderTransform.up;
			wd.hit.collider = null;
		}
		Vector3 pointVelocity = this.rBody.GetPointVelocity(wd.hit.point);
		wd.velocity = pointVelocity - Vector3.Project(pointVelocity, wd.hit.normal);
		wd.localVelocity.y = Vector3.Dot(wd.hit.forwardDir, wd.velocity);
		wd.localVelocity.x = Vector3.Dot(wd.hit.sidewaysDir, wd.velocity);
		if (!wd.isGrounded)
		{
			wd.localRigForce = Vector2.zero;
			return;
		}
		float num = Mathf.InverseLerp(1f, 0.25f, wd.velocity.sqrMagnitude);
		Vector2 vector2;
		if (num > 0f)
		{
			float num2 = Vector3.Dot(Vector3.up, wd.hit.normal);
			Vector3 rhs;
			if (num2 > 1E-06f)
			{
				Vector3 vector = Vector3.up * wd.downforce / num2;
				rhs = vector - Vector3.Project(vector, wd.hit.normal);
			}
			else
			{
				rhs = Vector3.up * 100000f;
			}
			vector2.y = Vector3.Dot(wd.hit.forwardDir, rhs);
			vector2.x = Vector3.Dot(wd.hit.sidewaysDir, rhs);
			vector2 *= num;
		}
		else
		{
			vector2 = Vector2.zero;
		}
		Vector2 a = -(Mathf.Clamp(wd.downforce / -Physics.gravity.y, 0f, wd.wheelCollider.sprungMass) * 0.5f) * wd.localVelocity / dt;
		wd.localRigForce = a + vector2;
	}

	// Token: 0x060025A5 RID: 9637 RVA: 0x000EDF70 File Offset: 0x000EC170
	private void ComputeTyreForces(CarPhysics<TCar>.ServerWheelData wd, float speed, float maxDriveForce, float maxSpeed, float throttleInput, float steerInput, float brakeInput, float driveForceMultiplier)
	{
		float absSpeed = Mathf.Abs(speed);
		if (this.vehicleSettings.tankSteering && brakeInput == 0f)
		{
			if (wd.isLeftWheel)
			{
				throttleInput = this.TankThrottleLeft;
			}
			else
			{
				throttleInput = this.TankThrottleRight;
			}
		}
		float num = wd.wheel.powerWheel ? throttleInput : 0f;
		wd.hasThrottleInput = (num != 0f);
		float num2 = this.vehicleSettings.maxDriveSlip;
		if (Mathf.Sign(num) != Mathf.Sign(wd.localVelocity.y))
		{
			num2 -= wd.localVelocity.y * Mathf.Sign(num);
		}
		float num3 = Mathf.Abs(num);
		float num4 = -this.vehicleSettings.rollingResistance + num3 * (1f + this.vehicleSettings.rollingResistance) - brakeInput * (1f - this.vehicleSettings.rollingResistance);
		if (this.InSlowSpeedExitMode || num4 < 0f || maxDriveForce == 0f)
		{
			num4 *= -1f;
			wd.isBraking = true;
		}
		else
		{
			num4 *= Mathf.Sign(num);
			wd.isBraking = false;
		}
		float num6;
		if (wd.isBraking)
		{
			float num5 = Mathf.Clamp(this.car.GetMaxForwardSpeed() * this.vehicleSettings.brakeForceMultiplier, 10f * this.vehicleSettings.brakeForceMultiplier, 50f * this.vehicleSettings.brakeForceMultiplier);
			num5 += this.rBody.mass * 1.5f;
			num6 = num4 * num5;
		}
		else
		{
			num6 = this.ComputeDriveForce(speed, absSpeed, num4 * maxDriveForce, maxDriveForce, maxSpeed, driveForceMultiplier);
		}
		if (wd.isGrounded)
		{
			wd.tyreSlip.x = wd.localVelocity.x;
			wd.tyreSlip.y = wd.localVelocity.y - wd.angularVelocity * wd.wheelCollider.radius;
			float num7;
			switch (this.car.OnSurface)
			{
			case VehicleTerrainHandler.Surface.Road:
				num7 = 1f;
				goto IL_230;
			case VehicleTerrainHandler.Surface.Ice:
				num7 = 0.25f;
				goto IL_230;
			case VehicleTerrainHandler.Surface.Frictionless:
				num7 = 0f;
				goto IL_230;
			}
			num7 = 0.75f;
			IL_230:
			float num8 = wd.wheel.tyreFriction * wd.downforce * num7;
			float num9 = 0f;
			if (!wd.isBraking)
			{
				num9 = Mathf.Min(Mathf.Abs(num6 * wd.tyreSlip.x) / num8, num2);
				if (num6 != 0f && num9 < 0.1f)
				{
					num9 = 0.1f;
				}
			}
			if (Mathf.Abs(wd.tyreSlip.y) < num9)
			{
				wd.tyreSlip.y = num9 * Mathf.Sign(wd.tyreSlip.y);
			}
			Vector2 vector = -num8 * wd.tyreSlip.normalized;
			vector.x = Mathf.Abs(vector.x) * 1.5f;
			vector.y = Mathf.Abs(vector.y);
			wd.tyreForce.x = Mathf.Clamp(wd.localRigForce.x, -vector.x, vector.x);
			if (wd.isBraking)
			{
				float num10 = Mathf.Min(vector.y, num6);
				wd.tyreForce.y = Mathf.Clamp(wd.localRigForce.y, -num10, num10);
			}
			else
			{
				wd.tyreForce.y = Mathf.Clamp(num6, -vector.y, vector.y);
			}
		}
		else
		{
			wd.tyreSlip = Vector2.zero;
			wd.tyreForce = Vector2.zero;
		}
		if (wd.isGrounded)
		{
			float num11;
			if (wd.isBraking)
			{
				num11 = 0f;
			}
			else
			{
				float driveForceToMaxSlip = this.vehicleSettings.driveForceToMaxSlip;
				num11 = Mathf.Clamp01((Mathf.Abs(num6) - Mathf.Abs(wd.tyreForce.y)) / driveForceToMaxSlip) * num2 * Mathf.Sign(num6);
			}
			wd.angularVelocity = (wd.localVelocity.y + num11) / wd.wheelCollider.radius;
			return;
		}
		float num12 = 50f;
		float num13 = 10f;
		if (num > 0f)
		{
			wd.angularVelocity += num12 * num;
		}
		else
		{
			wd.angularVelocity -= num13;
		}
		wd.angularVelocity -= num12 * brakeInput;
		wd.angularVelocity = Mathf.Clamp(wd.angularVelocity, 0f, maxSpeed / wd.wheelCollider.radius);
	}

	// Token: 0x060025A6 RID: 9638 RVA: 0x000EE3FC File Offset: 0x000EC5FC
	private void ComputeTankSteeringThrottle(float throttleInput, float steerInput, float speed)
	{
		this.TankThrottleLeft = throttleInput;
		this.TankThrottleRight = throttleInput;
		float tankSteerInvert = this.GetTankSteerInvert(throttleInput, speed);
		if (throttleInput == 0f)
		{
			this.TankThrottleLeft = -steerInput;
			this.TankThrottleRight = steerInput;
			return;
		}
		if (steerInput > 0f)
		{
			this.TankThrottleLeft = Mathf.Lerp(throttleInput, -1f * tankSteerInvert, steerInput);
			this.TankThrottleRight = Mathf.Lerp(throttleInput, 1f * tankSteerInvert, steerInput);
			return;
		}
		if (steerInput < 0f)
		{
			this.TankThrottleLeft = Mathf.Lerp(throttleInput, 1f * tankSteerInvert, -steerInput);
			this.TankThrottleRight = Mathf.Lerp(throttleInput, -1f * tankSteerInvert, -steerInput);
		}
	}

	// Token: 0x060025A7 RID: 9639 RVA: 0x000EE49C File Offset: 0x000EC69C
	private float ComputeDriveForce(float speed, float absSpeed, float demandedForce, float maxForce, float maxForwardSpeed, float driveForceMultiplier)
	{
		float num = (speed >= 0f) ? maxForwardSpeed : (maxForwardSpeed * this.vehicleSettings.reversePercentSpeed);
		if (absSpeed < num)
		{
			if ((speed >= 0f || demandedForce <= 0f) && (speed <= 0f || demandedForce >= 0f))
			{
				maxForce = this.car.GetAdjustedDriveForce(absSpeed, maxForwardSpeed) * driveForceMultiplier;
			}
			return Mathf.Clamp(demandedForce, -maxForce, maxForce);
		}
		float num2 = maxForce * Mathf.Max(1f - absSpeed / num, -1f) * Mathf.Sign(speed);
		if ((speed < 0f && demandedForce > 0f) || (speed > 0f && demandedForce < 0f))
		{
			num2 = Mathf.Clamp(num2 + demandedForce, -maxForce, maxForce);
		}
		return num2;
	}

	// Token: 0x060025A8 RID: 9640 RVA: 0x000EE55C File Offset: 0x000EC75C
	private void ComputeOverallForces()
	{
		this.DriveWheelVelocity = 0f;
		this.DriveWheelSlip = 0f;
		int num = 0;
		for (int i = 0; i < this.wheelData.Length; i++)
		{
			CarPhysics<TCar>.ServerWheelData serverWheelData = this.wheelData[i];
			if (serverWheelData.wheel.powerWheel)
			{
				this.DriveWheelVelocity += serverWheelData.angularVelocity;
				if (serverWheelData.isGrounded)
				{
					float num2 = CarPhysics<TCar>.ComputeCombinedSlip(serverWheelData.localVelocity, serverWheelData.tyreSlip);
					this.DriveWheelSlip += num2;
				}
				num++;
			}
		}
		if (num > 0)
		{
			this.DriveWheelVelocity /= (float)num;
			this.DriveWheelSlip /= (float)num;
		}
	}

	// Token: 0x060025A9 RID: 9641 RVA: 0x000EE60C File Offset: 0x000EC80C
	private static float ComputeCombinedSlip(Vector2 localVelocity, Vector2 tyreSlip)
	{
		float magnitude = localVelocity.magnitude;
		if (magnitude > 0.01f)
		{
			float num = tyreSlip.x * localVelocity.x / magnitude;
			float y = tyreSlip.y;
			return Mathf.Sqrt(num * num + y * y);
		}
		return tyreSlip.magnitude;
	}

	// Token: 0x060025AA RID: 9642 RVA: 0x000EE654 File Offset: 0x000EC854
	private void ApplyTyreForces(CarPhysics<TCar>.ServerWheelData wd, float throttleInput, float steerInput, float speed)
	{
		if (wd.isGrounded)
		{
			Vector3 force = wd.hit.forwardDir * wd.tyreForce.y;
			Vector3 force2 = wd.hit.sidewaysDir * wd.tyreForce.x;
			Vector3 sidewaysForceAppPoint = this.GetSidewaysForceAppPoint(wd, wd.hit.point);
			this.rBody.AddForceAtPosition(force, wd.hit.point, ForceMode.Force);
			this.rBody.AddForceAtPosition(force2, sidewaysForceAppPoint, ForceMode.Force);
		}
	}

	// Token: 0x060025AB RID: 9643 RVA: 0x000EE6DC File Offset: 0x000EC8DC
	private Vector3 GetSidewaysForceAppPoint(CarPhysics<TCar>.ServerWheelData wd, Vector3 contactPoint)
	{
		Vector3 vector = contactPoint + wd.wheelColliderTransform.up * this.vehicleSettings.antiRoll * wd.forceDistance;
		float num = wd.wheel.steerWheel ? this.SteerAngle : 0f;
		if (num != 0f && Mathf.Sign(num) != Mathf.Sign(wd.tyreSlip.x))
		{
			vector += wd.wheelColliderTransform.forward * this.midWheelPos * (this.vehicleSettings.handlingBias - 0.5f);
		}
		return vector;
	}

	// Token: 0x060025AC RID: 9644 RVA: 0x000EE788 File Offset: 0x000EC988
	private float GetTankSteerInvert(float throttleInput, float speed)
	{
		float result = 1f;
		if (throttleInput < 0f && speed < 1.75f)
		{
			result = -1f;
		}
		else if (throttleInput == 0f && speed < -1f)
		{
			result = -1f;
		}
		else if (speed < -1f)
		{
			result = -1f;
		}
		return result;
	}

	// Token: 0x060025AD RID: 9645 RVA: 0x000EE7DC File Offset: 0x000EC9DC
	[CompilerGenerated]
	private CarPhysics<TCar>.ServerWheelData <.ctor>g__AddWheel|47_0(CarWheel wheel, ref CarPhysics<TCar>.<>c__DisplayClass47_0 A_2)
	{
		CarPhysics<TCar>.ServerWheelData serverWheelData = new CarPhysics<TCar>.ServerWheelData();
		serverWheelData.wheelCollider = wheel.wheelCollider;
		serverWheelData.wheelColliderTransform = wheel.wheelCollider.transform;
		serverWheelData.forceDistance = this.GetWheelForceDistance(wheel.wheelCollider);
		serverWheelData.wheel = wheel;
		serverWheelData.wheelCollider.sidewaysFriction = this.zeroFriction;
		serverWheelData.wheelCollider.forwardFriction = this.zeroFriction;
		Vector3 vector = A_2.transform.InverseTransformPoint(wheel.wheelCollider.transform.position);
		serverWheelData.isFrontWheel = (vector.z > 0f);
		serverWheelData.isLeftWheel = (vector.x < 0f);
		return serverWheelData;
	}

	// Token: 0x02000CF7 RID: 3319
	public interface ICar
	{
		// Token: 0x170006A5 RID: 1701
		// (get) Token: 0x06004FE1 RID: 20449
		VehicleTerrainHandler.Surface OnSurface { get; }

		// Token: 0x06004FE2 RID: 20450
		float GetThrottleInput();

		// Token: 0x06004FE3 RID: 20451
		float GetBrakeInput();

		// Token: 0x06004FE4 RID: 20452
		float GetSteerInput();

		// Token: 0x06004FE5 RID: 20453
		bool GetSteerModInput();

		// Token: 0x06004FE6 RID: 20454
		float GetMaxForwardSpeed();

		// Token: 0x06004FE7 RID: 20455
		float GetMaxDriveForce();

		// Token: 0x06004FE8 RID: 20456
		float GetAdjustedDriveForce(float absSpeed, float topSpeed);

		// Token: 0x06004FE9 RID: 20457
		float GetModifiedDrag();

		// Token: 0x06004FEA RID: 20458
		CarWheel[] GetWheels();

		// Token: 0x06004FEB RID: 20459
		float GetWheelsMidPos();
	}

	// Token: 0x02000CF8 RID: 3320
	private class ServerWheelData
	{
		// Token: 0x040045BA RID: 17850
		public CarWheel wheel;

		// Token: 0x040045BB RID: 17851
		public Transform wheelColliderTransform;

		// Token: 0x040045BC RID: 17852
		public WheelCollider wheelCollider;

		// Token: 0x040045BD RID: 17853
		public bool isGrounded;

		// Token: 0x040045BE RID: 17854
		public float downforce;

		// Token: 0x040045BF RID: 17855
		public float forceDistance;

		// Token: 0x040045C0 RID: 17856
		public WheelHit hit;

		// Token: 0x040045C1 RID: 17857
		public Vector2 localRigForce;

		// Token: 0x040045C2 RID: 17858
		public Vector2 localVelocity;

		// Token: 0x040045C3 RID: 17859
		public float angularVelocity;

		// Token: 0x040045C4 RID: 17860
		public Vector3 origin;

		// Token: 0x040045C5 RID: 17861
		public Vector2 tyreForce;

		// Token: 0x040045C6 RID: 17862
		public Vector2 tyreSlip;

		// Token: 0x040045C7 RID: 17863
		public Vector3 velocity;

		// Token: 0x040045C8 RID: 17864
		public bool isBraking;

		// Token: 0x040045C9 RID: 17865
		public bool hasThrottleInput;

		// Token: 0x040045CA RID: 17866
		public bool isFrontWheel;

		// Token: 0x040045CB RID: 17867
		public bool isLeftWheel;
	}
}
