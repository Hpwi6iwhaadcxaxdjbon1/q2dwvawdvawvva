using System;
using UnityEngine;

// Token: 0x0200041A RID: 1050
public class AIHelicopterAnimation : MonoBehaviour
{
	// Token: 0x04001B95 RID: 7061
	public PatrolHelicopterAI _ai;

	// Token: 0x04001B96 RID: 7062
	public float swayAmount = 1f;

	// Token: 0x04001B97 RID: 7063
	public float lastStrafeScalar;

	// Token: 0x04001B98 RID: 7064
	public float lastForwardBackScalar;

	// Token: 0x04001B99 RID: 7065
	public float degreeMax = 90f;

	// Token: 0x04001B9A RID: 7066
	public Vector3 lastPosition = Vector3.zero;

	// Token: 0x04001B9B RID: 7067
	public float oldMoveSpeed;

	// Token: 0x04001B9C RID: 7068
	public float smoothRateOfChange;

	// Token: 0x04001B9D RID: 7069
	public float flareAmount;

	// Token: 0x06002365 RID: 9061 RVA: 0x000E24DC File Offset: 0x000E06DC
	public void Awake()
	{
		this.lastPosition = base.transform.position;
	}

	// Token: 0x06002366 RID: 9062 RVA: 0x000E24EF File Offset: 0x000E06EF
	public Vector3 GetMoveDirection()
	{
		return this._ai.GetMoveDirection();
	}

	// Token: 0x06002367 RID: 9063 RVA: 0x000E24FC File Offset: 0x000E06FC
	public float GetMoveSpeed()
	{
		return this._ai.GetMoveSpeed();
	}

	// Token: 0x06002368 RID: 9064 RVA: 0x000E250C File Offset: 0x000E070C
	public void Update()
	{
		this.lastPosition = base.transform.position;
		Vector3 moveDirection = this.GetMoveDirection();
		float moveSpeed = this.GetMoveSpeed();
		float num = 0.25f + Mathf.Clamp01(moveSpeed / this._ai.maxSpeed) * 0.75f;
		this.smoothRateOfChange = Mathf.Lerp(this.smoothRateOfChange, moveSpeed - this.oldMoveSpeed, Time.deltaTime * 5f);
		this.oldMoveSpeed = moveSpeed;
		float num2 = Vector3.Angle(moveDirection, base.transform.forward);
		float num3 = Vector3.Angle(moveDirection, -base.transform.forward);
		float num4 = 1f - Mathf.Clamp01(num2 / this.degreeMax);
		float num5 = 1f - Mathf.Clamp01(num3 / this.degreeMax);
		float b = (num4 - num5) * num;
		float num6 = Mathf.Lerp(this.lastForwardBackScalar, b, Time.deltaTime * 2f);
		this.lastForwardBackScalar = num6;
		float num7 = Vector3.Angle(moveDirection, base.transform.right);
		float num8 = Vector3.Angle(moveDirection, -base.transform.right);
		float num9 = 1f - Mathf.Clamp01(num7 / this.degreeMax);
		float num10 = 1f - Mathf.Clamp01(num8 / this.degreeMax);
		float b2 = (num9 - num10) * num;
		float num11 = Mathf.Lerp(this.lastStrafeScalar, b2, Time.deltaTime * 2f);
		this.lastStrafeScalar = num11;
		Vector3 zero = Vector3.zero;
		zero.x += num6 * this.swayAmount;
		zero.z -= num11 * this.swayAmount;
		Quaternion localRotation = Quaternion.identity;
		localRotation = Quaternion.Euler(zero.x, zero.y, zero.z);
		this._ai.helicopterBase.rotorPivot.transform.localRotation = localRotation;
	}
}
