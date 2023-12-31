﻿using System;
using UnityEngine;

// Token: 0x0200015B RID: 347
public class JiggleBone : BaseMonoBehaviour
{
	// Token: 0x04000FE1 RID: 4065
	public bool debugMode = true;

	// Token: 0x04000FE2 RID: 4066
	private Vector3 targetPos;

	// Token: 0x04000FE3 RID: 4067
	private Vector3 dynamicPos;

	// Token: 0x04000FE4 RID: 4068
	public Vector3 boneAxis = new Vector3(0f, 0f, 1f);

	// Token: 0x04000FE5 RID: 4069
	public float targetDistance = 2f;

	// Token: 0x04000FE6 RID: 4070
	public float bStiffness = 0.1f;

	// Token: 0x04000FE7 RID: 4071
	public float bMass = 0.9f;

	// Token: 0x04000FE8 RID: 4072
	public float bDamping = 0.75f;

	// Token: 0x04000FE9 RID: 4073
	public float bGravity = 0.75f;

	// Token: 0x04000FEA RID: 4074
	private Vector3 force;

	// Token: 0x04000FEB RID: 4075
	private Vector3 acc;

	// Token: 0x04000FEC RID: 4076
	private Vector3 vel;

	// Token: 0x04000FED RID: 4077
	public bool SquashAndStretch = true;

	// Token: 0x04000FEE RID: 4078
	public float sideStretch = 0.15f;

	// Token: 0x04000FEF RID: 4079
	public float frontStretch = 0.2f;

	// Token: 0x04000FF0 RID: 4080
	public float disableDistance = 20f;

	// Token: 0x06001738 RID: 5944 RVA: 0x000B0BA0 File Offset: 0x000AEDA0
	private void Awake()
	{
		Vector3 vector = base.transform.position + base.transform.TransformDirection(new Vector3(this.boneAxis.x * this.targetDistance, this.boneAxis.y * this.targetDistance, this.boneAxis.z * this.targetDistance));
		this.dynamicPos = vector;
	}

	// Token: 0x06001739 RID: 5945 RVA: 0x000B0C0C File Offset: 0x000AEE0C
	private void LateUpdate()
	{
		base.transform.rotation = default(Quaternion);
		Vector3 dir = base.transform.TransformDirection(new Vector3(this.boneAxis.x * this.targetDistance, this.boneAxis.y * this.targetDistance, this.boneAxis.z * this.targetDistance));
		Vector3 vector = base.transform.TransformDirection(new Vector3(0f, 1f, 0f));
		Vector3 vector2 = base.transform.position + base.transform.TransformDirection(new Vector3(this.boneAxis.x * this.targetDistance, this.boneAxis.y * this.targetDistance, this.boneAxis.z * this.targetDistance));
		this.force.x = (vector2.x - this.dynamicPos.x) * this.bStiffness;
		this.acc.x = this.force.x / this.bMass;
		this.vel.x = this.vel.x + this.acc.x * (1f - this.bDamping);
		this.force.y = (vector2.y - this.dynamicPos.y) * this.bStiffness;
		this.force.y = this.force.y - this.bGravity / 10f;
		this.acc.y = this.force.y / this.bMass;
		this.vel.y = this.vel.y + this.acc.y * (1f - this.bDamping);
		this.force.z = (vector2.z - this.dynamicPos.z) * this.bStiffness;
		this.acc.z = this.force.z / this.bMass;
		this.vel.z = this.vel.z + this.acc.z * (1f - this.bDamping);
		this.dynamicPos += this.vel + this.force;
		base.transform.LookAt(this.dynamicPos, vector);
		if (this.SquashAndStretch)
		{
			float magnitude = (this.dynamicPos - vector2).magnitude;
			float x;
			if (this.boneAxis.x == 0f)
			{
				x = 1f + -magnitude * this.sideStretch;
			}
			else
			{
				x = 1f + magnitude * this.frontStretch;
			}
			float y;
			if (this.boneAxis.y == 0f)
			{
				y = 1f + -magnitude * this.sideStretch;
			}
			else
			{
				y = 1f + magnitude * this.frontStretch;
			}
			float z;
			if (this.boneAxis.z == 0f)
			{
				z = 1f + -magnitude * this.sideStretch;
			}
			else
			{
				z = 1f + magnitude * this.frontStretch;
			}
			base.transform.localScale = new Vector3(x, y, z);
		}
		if (this.debugMode)
		{
			Debug.DrawRay(base.transform.position, dir, Color.blue);
			Debug.DrawRay(base.transform.position, vector, Color.green);
			Debug.DrawRay(vector2, Vector3.up * 0.2f, Color.yellow);
			Debug.DrawRay(this.dynamicPos, Vector3.up * 0.2f, Color.red);
		}
	}
}
