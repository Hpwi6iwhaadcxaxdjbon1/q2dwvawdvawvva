using System;
using UnityEngine;

// Token: 0x0200019F RID: 415
public class CH47FlightTest : MonoBehaviour
{
	// Token: 0x04001124 RID: 4388
	public Rigidbody rigidBody;

	// Token: 0x04001125 RID: 4389
	public float engineThrustMax;

	// Token: 0x04001126 RID: 4390
	public Vector3 torqueScale;

	// Token: 0x04001127 RID: 4391
	public Transform com;

	// Token: 0x04001128 RID: 4392
	public Transform[] GroundPoints;

	// Token: 0x04001129 RID: 4393
	public Transform[] GroundEffects;

	// Token: 0x0400112A RID: 4394
	public float currentThrottle;

	// Token: 0x0400112B RID: 4395
	public float avgThrust;

	// Token: 0x0400112C RID: 4396
	public float liftDotMax = 0.75f;

	// Token: 0x0400112D RID: 4397
	public Transform AIMoveTarget;

	// Token: 0x0400112E RID: 4398
	private static float altitudeTolerance = 1f;

	// Token: 0x0600186A RID: 6250 RVA: 0x000B66A8 File Offset: 0x000B48A8
	public void Awake()
	{
		this.rigidBody.centerOfMass = this.com.localPosition;
	}

	// Token: 0x0600186B RID: 6251 RVA: 0x000B66C0 File Offset: 0x000B48C0
	public CH47FlightTest.HelicopterInputState_t GetHelicopterInputState()
	{
		CH47FlightTest.HelicopterInputState_t helicopterInputState_t = default(CH47FlightTest.HelicopterInputState_t);
		helicopterInputState_t.throttle = (Input.GetKey(KeyCode.W) ? 1f : 0f);
		helicopterInputState_t.throttle -= (Input.GetKey(KeyCode.S) ? 1f : 0f);
		helicopterInputState_t.pitch = Input.GetAxis("Mouse Y");
		helicopterInputState_t.roll = -Input.GetAxis("Mouse X");
		helicopterInputState_t.yaw = (Input.GetKey(KeyCode.D) ? 1f : 0f);
		helicopterInputState_t.yaw -= (Input.GetKey(KeyCode.A) ? 1f : 0f);
		helicopterInputState_t.pitch = (float)Mathf.RoundToInt(helicopterInputState_t.pitch);
		helicopterInputState_t.roll = (float)Mathf.RoundToInt(helicopterInputState_t.roll);
		return helicopterInputState_t;
	}

	// Token: 0x0600186C RID: 6252 RVA: 0x000B6798 File Offset: 0x000B4998
	public CH47FlightTest.HelicopterInputState_t GetAIInputState()
	{
		CH47FlightTest.HelicopterInputState_t result = default(CH47FlightTest.HelicopterInputState_t);
		Vector3 vector = Vector3.Cross(Vector3.up, base.transform.right);
		float num = Vector3.Dot(Vector3.Cross(Vector3.up, vector), Vector3Ex.Direction2D(this.AIMoveTarget.position, base.transform.position));
		result.yaw = ((num < 0f) ? 1f : 0f);
		result.yaw -= ((num > 0f) ? 1f : 0f);
		float num2 = Vector3.Dot(Vector3.up, base.transform.right);
		result.roll = ((num2 < 0f) ? 1f : 0f);
		result.roll -= ((num2 > 0f) ? 1f : 0f);
		float num3 = Vector3Ex.Distance2D(base.transform.position, this.AIMoveTarget.position);
		float num4 = Vector3.Dot(vector, Vector3Ex.Direction2D(this.AIMoveTarget.position, base.transform.position));
		float num5 = Vector3.Dot(Vector3.up, base.transform.forward);
		if (num3 > 10f)
		{
			result.pitch = ((num4 > 0.8f) ? -0.25f : 0f);
			result.pitch -= ((num4 < -0.8f) ? -0.25f : 0f);
			if (num5 < -0.35f)
			{
				result.pitch = -1f;
			}
			else if (num5 > 0.35f)
			{
				result.pitch = 1f;
			}
		}
		else if (num5 < --0f)
		{
			result.pitch = -1f;
		}
		else if (num5 > 0f)
		{
			result.pitch = 1f;
		}
		float idealAltitude = this.GetIdealAltitude();
		float y = base.transform.position.y;
		float num6;
		if (y > idealAltitude + CH47FlightTest.altitudeTolerance)
		{
			num6 = -1f;
		}
		else if (y < idealAltitude - CH47FlightTest.altitudeTolerance)
		{
			num6 = 1f;
		}
		else if (num3 > 20f)
		{
			num6 = Mathf.Lerp(0f, 1f, num3 / 20f);
		}
		else
		{
			num6 = 0f;
		}
		Debug.Log("desiredThrottle : " + num6);
		result.throttle = num6 * 1f;
		return result;
	}

	// Token: 0x0600186D RID: 6253 RVA: 0x000B6A0B File Offset: 0x000B4C0B
	public float GetIdealAltitude()
	{
		return this.AIMoveTarget.transform.position.y;
	}

	// Token: 0x0600186E RID: 6254 RVA: 0x000B6A24 File Offset: 0x000B4C24
	public void FixedUpdate()
	{
		CH47FlightTest.HelicopterInputState_t aiinputState = this.GetAIInputState();
		this.currentThrottle = Mathf.Lerp(this.currentThrottle, aiinputState.throttle, 2f * Time.fixedDeltaTime);
		this.currentThrottle = Mathf.Clamp(this.currentThrottle, -0.2f, 1f);
		this.rigidBody.AddRelativeTorque(new Vector3(aiinputState.pitch * this.torqueScale.x, aiinputState.yaw * this.torqueScale.y, aiinputState.roll * this.torqueScale.z) * Time.fixedDeltaTime, ForceMode.Force);
		this.avgThrust = Mathf.Lerp(this.avgThrust, this.engineThrustMax * this.currentThrottle, Time.fixedDeltaTime);
		float value = Mathf.Clamp01(Vector3.Dot(base.transform.up, Vector3.up));
		float num = Mathf.InverseLerp(this.liftDotMax, 1f, value);
		Vector3 force = Vector3.up * this.engineThrustMax * 0.5f * this.currentThrottle * num;
		Vector3 force2 = (base.transform.up - Vector3.up).normalized * this.engineThrustMax * this.currentThrottle * (1f - num);
		float d = this.rigidBody.mass * -Physics.gravity.y;
		this.rigidBody.AddForce(base.transform.up * d * num * 0.99f, ForceMode.Force);
		this.rigidBody.AddForce(force, ForceMode.Force);
		this.rigidBody.AddForce(force2, ForceMode.Force);
		for (int i = 0; i < this.GroundEffects.Length; i++)
		{
			Component component = this.GroundPoints[i];
			Transform transform = this.GroundEffects[i];
			RaycastHit raycastHit;
			if (Physics.Raycast(component.transform.position, Vector3.down, out raycastHit, 50f, 8388608))
			{
				transform.gameObject.SetActive(true);
				transform.transform.position = raycastHit.point + new Vector3(0f, 1f, 0f);
			}
			else
			{
				transform.gameObject.SetActive(false);
			}
		}
	}

	// Token: 0x0600186F RID: 6255 RVA: 0x000B6C84 File Offset: 0x000B4E84
	public void OnDrawGizmos()
	{
		Gizmos.color = Color.yellow;
		Gizmos.DrawSphere(this.AIMoveTarget.transform.position, 1f);
		Vector3 vector = Vector3.Cross(base.transform.right, Vector3.up);
		Vector3 a = Vector3.Cross(vector, Vector3.up);
		Gizmos.color = Color.blue;
		Gizmos.DrawLine(base.transform.position, base.transform.position + vector * 10f);
		Gizmos.color = Color.red;
		Gizmos.DrawLine(base.transform.position, base.transform.position + a * 10f);
	}

	// Token: 0x02000C37 RID: 3127
	public struct HelicopterInputState_t
	{
		// Token: 0x04004289 RID: 17033
		public float throttle;

		// Token: 0x0400428A RID: 17034
		public float roll;

		// Token: 0x0400428B RID: 17035
		public float yaw;

		// Token: 0x0400428C RID: 17036
		public float pitch;
	}
}
