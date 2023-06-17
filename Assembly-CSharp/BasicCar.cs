using System;
using UnityEngine;

// Token: 0x0200046E RID: 1134
public class BasicCar : BaseVehicle
{
	// Token: 0x04001D8D RID: 7565
	public BasicCar.VehicleWheel[] wheels;

	// Token: 0x04001D8E RID: 7566
	public float brakePedal;

	// Token: 0x04001D8F RID: 7567
	public float gasPedal;

	// Token: 0x04001D90 RID: 7568
	public float steering;

	// Token: 0x04001D91 RID: 7569
	public Transform centerOfMass;

	// Token: 0x04001D92 RID: 7570
	public Transform steeringWheel;

	// Token: 0x04001D93 RID: 7571
	public float motorForceConstant = 150f;

	// Token: 0x04001D94 RID: 7572
	public float brakeForceConstant = 500f;

	// Token: 0x04001D95 RID: 7573
	public float GasLerpTime = 20f;

	// Token: 0x04001D96 RID: 7574
	public float SteeringLerpTime = 20f;

	// Token: 0x04001D97 RID: 7575
	public Transform driverEye;

	// Token: 0x04001D98 RID: 7576
	public GameObjectRef chairRef;

	// Token: 0x04001D99 RID: 7577
	public Transform chairAnchorTest;

	// Token: 0x04001D9A RID: 7578
	public SoundPlayer idleLoopPlayer;

	// Token: 0x04001D9B RID: 7579
	public Transform engineOffset;

	// Token: 0x04001D9C RID: 7580
	public SoundDefinition engineSoundDef;

	// Token: 0x04001D9D RID: 7581
	private static bool chairtest;

	// Token: 0x04001D9E RID: 7582
	private float throttle;

	// Token: 0x04001D9F RID: 7583
	private float brake;

	// Token: 0x04001DA0 RID: 7584
	private bool lightsOn = true;

	// Token: 0x06002558 RID: 9560 RVA: 0x000EB88D File Offset: 0x000E9A8D
	public override float MaxVelocity()
	{
		return 50f;
	}

	// Token: 0x06002559 RID: 9561 RVA: 0x000EB894 File Offset: 0x000E9A94
	public override Vector3 EyePositionForPlayer(BasePlayer player, Quaternion viewRot)
	{
		if (this.PlayerIsMounted(player))
		{
			return this.driverEye.transform.position;
		}
		return Vector3.zero;
	}

	// Token: 0x0600255A RID: 9562 RVA: 0x000EB8B8 File Offset: 0x000E9AB8
	public override void ServerInit()
	{
		if (base.isClient)
		{
			return;
		}
		base.ServerInit();
		this.rigidBody = base.GetComponent<Rigidbody>();
		this.rigidBody.centerOfMass = this.centerOfMass.localPosition;
		this.rigidBody.isKinematic = false;
		if (BasicCar.chairtest)
		{
			this.SpawnChairTest();
		}
	}

	// Token: 0x0600255B RID: 9563 RVA: 0x000EB910 File Offset: 0x000E9B10
	public void SpawnChairTest()
	{
		BaseEntity baseEntity = GameManager.server.CreateEntity(this.chairRef.resourcePath, this.chairAnchorTest.transform.localPosition, default(Quaternion), true);
		baseEntity.Spawn();
		DestroyOnGroundMissing component = baseEntity.GetComponent<DestroyOnGroundMissing>();
		if (component != null)
		{
			component.enabled = false;
		}
		MeshCollider component2 = baseEntity.GetComponent<MeshCollider>();
		if (component2)
		{
			component2.convex = true;
		}
		baseEntity.SetParent(this, false, false);
	}

	// Token: 0x0600255C RID: 9564 RVA: 0x000EB988 File Offset: 0x000E9B88
	public override void VehicleFixedUpdate()
	{
		base.VehicleFixedUpdate();
		if (!base.HasDriver())
		{
			this.NoDriverInput();
		}
		this.ConvertInputToThrottle();
		this.DoSteering();
		this.ApplyForceAtWheels();
		base.SetFlag(BaseEntity.Flags.Reserved1, base.HasDriver(), false, true);
		base.SetFlag(BaseEntity.Flags.Reserved2, base.HasDriver() && this.lightsOn, false, true);
	}

	// Token: 0x0600255D RID: 9565 RVA: 0x000EB9EC File Offset: 0x000E9BEC
	private void DoSteering()
	{
		foreach (BasicCar.VehicleWheel vehicleWheel in this.wheels)
		{
			if (vehicleWheel.steerWheel)
			{
				vehicleWheel.wheelCollider.steerAngle = this.steering;
			}
		}
		base.SetFlag(BaseEntity.Flags.Reserved4, this.steering < -2f, false, true);
		base.SetFlag(BaseEntity.Flags.Reserved5, this.steering > 2f, false, true);
	}

	// Token: 0x0600255E RID: 9566 RVA: 0x000063A5 File Offset: 0x000045A5
	public void ConvertInputToThrottle()
	{
	}

	// Token: 0x0600255F RID: 9567 RVA: 0x000EBA60 File Offset: 0x000E9C60
	private void ApplyForceAtWheels()
	{
		if (this.rigidBody == null)
		{
			return;
		}
		Vector3 velocity = this.rigidBody.velocity;
		float num = velocity.magnitude * Vector3.Dot(velocity.normalized, base.transform.forward);
		float num2 = this.brakePedal;
		float num3 = this.gasPedal;
		if (num > 0f && num3 < 0f)
		{
			num2 = 100f;
		}
		else if (num < 0f && num3 > 0f)
		{
			num2 = 100f;
		}
		foreach (BasicCar.VehicleWheel vehicleWheel in this.wheels)
		{
			if (vehicleWheel.wheelCollider.isGrounded)
			{
				if (vehicleWheel.powerWheel)
				{
					vehicleWheel.wheelCollider.motorTorque = num3 * this.motorForceConstant;
				}
				if (vehicleWheel.brakeWheel)
				{
					vehicleWheel.wheelCollider.brakeTorque = num2 * this.brakeForceConstant;
				}
			}
		}
		base.SetFlag(BaseEntity.Flags.Reserved3, num2 >= 100f && this.AnyMounted(), false, true);
	}

	// Token: 0x06002560 RID: 9568 RVA: 0x000EBB6C File Offset: 0x000E9D6C
	public void NoDriverInput()
	{
		if (BasicCar.chairtest)
		{
			this.gasPedal = Mathf.Sin(Time.time) * 50f;
			return;
		}
		this.gasPedal = 0f;
		this.brakePedal = Mathf.Lerp(this.brakePedal, 100f, Time.deltaTime * this.GasLerpTime / 5f);
	}

	// Token: 0x06002561 RID: 9569 RVA: 0x000EBBCA File Offset: 0x000E9DCA
	public override void PlayerServerInput(InputState inputState, BasePlayer player)
	{
		if (base.IsDriver(player))
		{
			this.DriverInput(inputState, player);
		}
	}

	// Token: 0x06002562 RID: 9570 RVA: 0x000EBBE0 File Offset: 0x000E9DE0
	public void DriverInput(InputState inputState, BasePlayer player)
	{
		if (inputState.IsDown(BUTTON.FORWARD))
		{
			this.gasPedal = 100f;
			this.brakePedal = 0f;
		}
		else if (inputState.IsDown(BUTTON.BACKWARD))
		{
			this.gasPedal = -30f;
			this.brakePedal = 0f;
		}
		else
		{
			this.gasPedal = 0f;
			this.brakePedal = 30f;
		}
		if (inputState.IsDown(BUTTON.LEFT))
		{
			this.steering = -60f;
			return;
		}
		if (inputState.IsDown(BUTTON.RIGHT))
		{
			this.steering = 60f;
			return;
		}
		this.steering = 0f;
	}

	// Token: 0x06002563 RID: 9571 RVA: 0x000EBC7B File Offset: 0x000E9E7B
	public override void LightToggle(BasePlayer player)
	{
		if (base.IsDriver(player))
		{
			this.lightsOn = !this.lightsOn;
		}
	}

	// Token: 0x02000CF5 RID: 3317
	[Serializable]
	public class VehicleWheel
	{
		// Token: 0x040045AF RID: 17839
		public Transform shock;

		// Token: 0x040045B0 RID: 17840
		public WheelCollider wheelCollider;

		// Token: 0x040045B1 RID: 17841
		public Transform wheel;

		// Token: 0x040045B2 RID: 17842
		public Transform axle;

		// Token: 0x040045B3 RID: 17843
		public bool steerWheel;

		// Token: 0x040045B4 RID: 17844
		public bool brakeWheel = true;

		// Token: 0x040045B5 RID: 17845
		public bool powerWheel = true;
	}
}
