using System;
using UnityEngine;

// Token: 0x020001A4 RID: 420
public class BradleyMoveTest : MonoBehaviour
{
	// Token: 0x04001148 RID: 4424
	public WheelCollider[] leftWheels;

	// Token: 0x04001149 RID: 4425
	public WheelCollider[] rightWheels;

	// Token: 0x0400114A RID: 4426
	public float moveForceMax = 2000f;

	// Token: 0x0400114B RID: 4427
	public float brakeForce = 100f;

	// Token: 0x0400114C RID: 4428
	public float throttle = 1f;

	// Token: 0x0400114D RID: 4429
	public float turnForce = 2000f;

	// Token: 0x0400114E RID: 4430
	public float sideStiffnessMax = 1f;

	// Token: 0x0400114F RID: 4431
	public float sideStiffnessMin = 0.5f;

	// Token: 0x04001150 RID: 4432
	public Transform centerOfMass;

	// Token: 0x04001151 RID: 4433
	public float turning;

	// Token: 0x04001152 RID: 4434
	public bool brake;

	// Token: 0x04001153 RID: 4435
	public Rigidbody myRigidBody;

	// Token: 0x04001154 RID: 4436
	public Vector3 destination;

	// Token: 0x04001155 RID: 4437
	public float stoppingDist = 5f;

	// Token: 0x04001156 RID: 4438
	public GameObject followTest;

	// Token: 0x0600188E RID: 6286 RVA: 0x000B742C File Offset: 0x000B562C
	public void Awake()
	{
		this.Initialize();
	}

	// Token: 0x0600188F RID: 6287 RVA: 0x000B7434 File Offset: 0x000B5634
	public void Initialize()
	{
		this.myRigidBody.centerOfMass = this.centerOfMass.localPosition;
		this.destination = base.transform.position;
	}

	// Token: 0x06001890 RID: 6288 RVA: 0x000B745D File Offset: 0x000B565D
	public void SetDestination(Vector3 dest)
	{
		this.destination = dest;
	}

	// Token: 0x06001891 RID: 6289 RVA: 0x000B7468 File Offset: 0x000B5668
	public void FixedUpdate()
	{
		Vector3 velocity = this.myRigidBody.velocity;
		this.SetDestination(this.followTest.transform.position);
		float num = Vector3.Distance(base.transform.position, this.destination);
		if (num > this.stoppingDist)
		{
			Vector3 zero = Vector3.zero;
			float num2 = Vector3.Dot(zero, base.transform.right);
			float num3 = Vector3.Dot(zero, -base.transform.right);
			float num4 = Vector3.Dot(zero, base.transform.right);
			if (Vector3.Dot(zero, -base.transform.forward) > num4)
			{
				if (num2 >= num3)
				{
					this.turning = 1f;
				}
				else
				{
					this.turning = -1f;
				}
			}
			else
			{
				this.turning = num4;
			}
			this.throttle = Mathf.InverseLerp(this.stoppingDist, 30f, num);
		}
		this.throttle = Mathf.Clamp(this.throttle, -1f, 1f);
		float num5 = this.throttle;
		float num6 = this.throttle;
		if (this.turning > 0f)
		{
			num6 = -this.turning;
			num5 = this.turning;
		}
		else if (this.turning < 0f)
		{
			num5 = this.turning;
			num6 = this.turning * -1f;
		}
		this.ApplyBrakes(this.brake ? 1f : 0f);
		float num7 = this.throttle;
		num5 = Mathf.Clamp(num5 + num7, -1f, 1f);
		num6 = Mathf.Clamp(num6 + num7, -1f, 1f);
		this.AdjustFriction();
		float t = Mathf.InverseLerp(3f, 1f, velocity.magnitude * Mathf.Abs(Vector3.Dot(velocity.normalized, base.transform.forward)));
		float torqueAmount = Mathf.Lerp(this.moveForceMax, this.turnForce, t);
		this.SetMotorTorque(num5, false, torqueAmount);
		this.SetMotorTorque(num6, true, torqueAmount);
	}

	// Token: 0x06001892 RID: 6290 RVA: 0x000B766B File Offset: 0x000B586B
	public void ApplyBrakes(float amount)
	{
		this.ApplyBrakeTorque(amount, true);
		this.ApplyBrakeTorque(amount, false);
	}

	// Token: 0x06001893 RID: 6291 RVA: 0x000B7680 File Offset: 0x000B5880
	public float GetMotorTorque(bool rightSide)
	{
		float num = 0f;
		foreach (WheelCollider wheelCollider in rightSide ? this.rightWheels : this.leftWheels)
		{
			num += wheelCollider.motorTorque;
		}
		return num / (float)this.rightWheels.Length;
	}

	// Token: 0x06001894 RID: 6292 RVA: 0x000B76D0 File Offset: 0x000B58D0
	public void SetMotorTorque(float newThrottle, bool rightSide, float torqueAmount)
	{
		newThrottle = Mathf.Clamp(newThrottle, -1f, 1f);
		float motorTorque = torqueAmount * newThrottle;
		WheelCollider[] array = rightSide ? this.rightWheels : this.leftWheels;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].motorTorque = motorTorque;
		}
	}

	// Token: 0x06001895 RID: 6293 RVA: 0x000B771C File Offset: 0x000B591C
	public void ApplyBrakeTorque(float amount, bool rightSide)
	{
		WheelCollider[] array = rightSide ? this.rightWheels : this.leftWheels;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].brakeTorque = this.brakeForce * amount;
		}
	}

	// Token: 0x06001896 RID: 6294 RVA: 0x000063A5 File Offset: 0x000045A5
	public void AdjustFriction()
	{
	}
}
