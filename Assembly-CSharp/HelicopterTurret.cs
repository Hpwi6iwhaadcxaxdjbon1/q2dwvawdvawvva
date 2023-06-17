using System;
using System.Collections.Generic;
using ConVar;
using UnityEngine;

// Token: 0x0200041C RID: 1052
public class HelicopterTurret : MonoBehaviour
{
	// Token: 0x04001BA4 RID: 7076
	public PatrolHelicopterAI _heliAI;

	// Token: 0x04001BA5 RID: 7077
	public float fireRate = 0.125f;

	// Token: 0x04001BA6 RID: 7078
	public float burstLength = 3f;

	// Token: 0x04001BA7 RID: 7079
	public float timeBetweenBursts = 3f;

	// Token: 0x04001BA8 RID: 7080
	public float maxTargetRange = 300f;

	// Token: 0x04001BA9 RID: 7081
	public float loseTargetAfter = 5f;

	// Token: 0x04001BAA RID: 7082
	public Transform gun_yaw;

	// Token: 0x04001BAB RID: 7083
	public Transform gun_pitch;

	// Token: 0x04001BAC RID: 7084
	public Transform muzzleTransform;

	// Token: 0x04001BAD RID: 7085
	public bool left;

	// Token: 0x04001BAE RID: 7086
	public BaseCombatEntity _target;

	// Token: 0x04001BAF RID: 7087
	private float lastBurstTime = float.NegativeInfinity;

	// Token: 0x04001BB0 RID: 7088
	private float lastFireTime = float.NegativeInfinity;

	// Token: 0x04001BB1 RID: 7089
	private float lastSeenTargetTime = float.NegativeInfinity;

	// Token: 0x04001BB2 RID: 7090
	private bool targetVisible;

	// Token: 0x0600236F RID: 9071 RVA: 0x000E291F File Offset: 0x000E0B1F
	public void SetTarget(BaseCombatEntity newTarget)
	{
		this._target = newTarget;
		this.UpdateTargetVisibility();
	}

	// Token: 0x06002370 RID: 9072 RVA: 0x000E292E File Offset: 0x000E0B2E
	public bool NeedsNewTarget()
	{
		return !this.HasTarget() || (!this.targetVisible && this.TimeSinceTargetLastSeen() > this.loseTargetAfter);
	}

	// Token: 0x06002371 RID: 9073 RVA: 0x000E2954 File Offset: 0x000E0B54
	public bool UpdateTargetFromList(List<PatrolHelicopterAI.targetinfo> newTargetList)
	{
		int num = UnityEngine.Random.Range(0, newTargetList.Count);
		int i = newTargetList.Count;
		while (i >= 0)
		{
			i--;
			PatrolHelicopterAI.targetinfo targetinfo = newTargetList[num];
			if (targetinfo != null && targetinfo.ent != null && targetinfo.IsVisible() && this.InFiringArc(targetinfo.ply))
			{
				this.SetTarget(targetinfo.ply);
				return true;
			}
			num++;
			if (num >= newTargetList.Count)
			{
				num = 0;
			}
		}
		return false;
	}

	// Token: 0x06002372 RID: 9074 RVA: 0x000E29CC File Offset: 0x000E0BCC
	public bool TargetVisible()
	{
		this.UpdateTargetVisibility();
		return this.targetVisible;
	}

	// Token: 0x06002373 RID: 9075 RVA: 0x000E29DA File Offset: 0x000E0BDA
	public float TimeSinceTargetLastSeen()
	{
		return UnityEngine.Time.realtimeSinceStartup - this.lastSeenTargetTime;
	}

	// Token: 0x06002374 RID: 9076 RVA: 0x000E29E8 File Offset: 0x000E0BE8
	public bool HasTarget()
	{
		return this._target != null;
	}

	// Token: 0x06002375 RID: 9077 RVA: 0x000E29F6 File Offset: 0x000E0BF6
	public void ClearTarget()
	{
		this._target = null;
		this.targetVisible = false;
	}

	// Token: 0x06002376 RID: 9078 RVA: 0x000E2A08 File Offset: 0x000E0C08
	public void TurretThink()
	{
		if (this.HasTarget() && this.TimeSinceTargetLastSeen() > this.loseTargetAfter * 2f)
		{
			this.ClearTarget();
		}
		if (!this.HasTarget())
		{
			return;
		}
		if (UnityEngine.Time.time - this.lastBurstTime > this.burstLength + this.timeBetweenBursts && this.TargetVisible())
		{
			this.lastBurstTime = UnityEngine.Time.time;
		}
		if (UnityEngine.Time.time < this.lastBurstTime + this.burstLength && UnityEngine.Time.time - this.lastFireTime >= this.fireRate && this.InFiringArc(this._target))
		{
			this.lastFireTime = UnityEngine.Time.time;
			this.FireGun();
		}
	}

	// Token: 0x06002377 RID: 9079 RVA: 0x000E2AB8 File Offset: 0x000E0CB8
	public void FireGun()
	{
		this._heliAI.FireGun(this._target.transform.position + new Vector3(0f, 0.25f, 0f), PatrolHelicopter.bulletAccuracy, this.left);
	}

	// Token: 0x06002378 RID: 9080 RVA: 0x000E2B04 File Offset: 0x000E0D04
	public Vector3 GetPositionForEntity(BaseCombatEntity potentialtarget)
	{
		return potentialtarget.transform.position;
	}

	// Token: 0x06002379 RID: 9081 RVA: 0x000E2B14 File Offset: 0x000E0D14
	public float AngleToTarget(BaseCombatEntity potentialtarget)
	{
		Vector3 positionForEntity = this.GetPositionForEntity(potentialtarget);
		Vector3 position = this.muzzleTransform.position;
		Vector3 normalized = (positionForEntity - position).normalized;
		return Vector3.Angle(this.left ? (-this._heliAI.transform.right) : this._heliAI.transform.right, normalized);
	}

	// Token: 0x0600237A RID: 9082 RVA: 0x000E2B78 File Offset: 0x000E0D78
	public bool InFiringArc(BaseCombatEntity potentialtarget)
	{
		return this.AngleToTarget(potentialtarget) < 80f;
	}

	// Token: 0x0600237B RID: 9083 RVA: 0x000E2B88 File Offset: 0x000E0D88
	public void UpdateTargetVisibility()
	{
		if (!this.HasTarget())
		{
			return;
		}
		Vector3 position = this._target.transform.position;
		BasePlayer basePlayer = this._target as BasePlayer;
		if (basePlayer)
		{
			position = basePlayer.eyes.position;
		}
		bool flag = false;
		float num = Vector3.Distance(position, this.muzzleTransform.position);
		Vector3 normalized = (position - this.muzzleTransform.position).normalized;
		RaycastHit raycastHit;
		if (num < this.maxTargetRange && this.InFiringArc(this._target) && GamePhysics.Trace(new Ray(this.muzzleTransform.position + normalized * 6f, normalized), 0f, out raycastHit, num * 1.1f, 1218652417, QueryTriggerInteraction.UseGlobal, null) && raycastHit.collider.gameObject.ToBaseEntity() == this._target)
		{
			flag = true;
		}
		if (flag)
		{
			this.lastSeenTargetTime = UnityEngine.Time.realtimeSinceStartup;
		}
		this.targetVisible = flag;
	}
}
