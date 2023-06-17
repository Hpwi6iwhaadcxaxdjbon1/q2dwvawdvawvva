using System;
using Rust;
using UnityEngine;

// Token: 0x0200047F RID: 1151
public class CH47HelicopterAIController : CH47Helicopter
{
	// Token: 0x04001E45 RID: 7749
	public GameObjectRef scientistPrefab;

	// Token: 0x04001E46 RID: 7750
	public GameObjectRef dismountablePrefab;

	// Token: 0x04001E47 RID: 7751
	public GameObjectRef weakDismountablePrefab;

	// Token: 0x04001E48 RID: 7752
	public float maxTiltAngle = 0.3f;

	// Token: 0x04001E49 RID: 7753
	public float AiAltitudeForce = 10000f;

	// Token: 0x04001E4A RID: 7754
	public GameObjectRef lockedCratePrefab;

	// Token: 0x04001E4B RID: 7755
	public const BaseEntity.Flags Flag_Damaged = BaseEntity.Flags.Reserved7;

	// Token: 0x04001E4C RID: 7756
	public const BaseEntity.Flags Flag_NearDeath = BaseEntity.Flags.OnFire;

	// Token: 0x04001E4D RID: 7757
	public const BaseEntity.Flags Flag_DropDoorOpen = BaseEntity.Flags.Reserved8;

	// Token: 0x04001E4E RID: 7758
	public GameObject triggerHurt;

	// Token: 0x04001E4F RID: 7759
	public Vector3 landingTarget;

	// Token: 0x04001E50 RID: 7760
	private int numCrates = 1;

	// Token: 0x04001E51 RID: 7761
	private bool shouldLand;

	// Token: 0x04001E52 RID: 7762
	private bool aimDirOverride;

	// Token: 0x04001E53 RID: 7763
	private Vector3 _aimDirection = Vector3.forward;

	// Token: 0x04001E54 RID: 7764
	private Vector3 _moveTarget = Vector3.zero;

	// Token: 0x04001E55 RID: 7765
	private int lastAltitudeCheckFrame;

	// Token: 0x04001E56 RID: 7766
	private float altOverride;

	// Token: 0x04001E57 RID: 7767
	private float currentDesiredAltitude;

	// Token: 0x04001E58 RID: 7768
	private bool altitudeProtection = true;

	// Token: 0x04001E59 RID: 7769
	private float hoverHeight = 30f;

	// Token: 0x060025F9 RID: 9721 RVA: 0x000EFA74 File Offset: 0x000EDC74
	public void DropCrate()
	{
		if (this.numCrates <= 0)
		{
			return;
		}
		Vector3 pos = base.transform.position + Vector3.down * 5f;
		Quaternion rot = Quaternion.Euler(0f, UnityEngine.Random.Range(0f, 360f), 0f);
		BaseEntity baseEntity = GameManager.server.CreateEntity(this.lockedCratePrefab.resourcePath, pos, rot, true);
		if (baseEntity)
		{
			baseEntity.SendMessage("SetWasDropped");
			baseEntity.Spawn();
		}
		this.numCrates--;
	}

	// Token: 0x060025FA RID: 9722 RVA: 0x000EFB0A File Offset: 0x000EDD0A
	public bool OutOfCrates()
	{
		return this.numCrates <= 0;
	}

	// Token: 0x060025FB RID: 9723 RVA: 0x000EFB18 File Offset: 0x000EDD18
	public bool CanDropCrate()
	{
		return this.numCrates > 0;
	}

	// Token: 0x060025FC RID: 9724 RVA: 0x00003278 File Offset: 0x00001478
	public bool IsDropDoorOpen()
	{
		return base.HasFlag(BaseEntity.Flags.Reserved8);
	}

	// Token: 0x060025FD RID: 9725 RVA: 0x00070797 File Offset: 0x0006E997
	public void SetDropDoorOpen(bool open)
	{
		base.SetFlag(BaseEntity.Flags.Reserved8, open, false, true);
	}

	// Token: 0x060025FE RID: 9726 RVA: 0x000EFB23 File Offset: 0x000EDD23
	public bool ShouldLand()
	{
		return this.shouldLand;
	}

	// Token: 0x060025FF RID: 9727 RVA: 0x000EFB2B File Offset: 0x000EDD2B
	public void SetLandingTarget(Vector3 target)
	{
		this.shouldLand = true;
		this.landingTarget = target;
		this.numCrates = 0;
	}

	// Token: 0x06002600 RID: 9728 RVA: 0x000EFB42 File Offset: 0x000EDD42
	public void ClearLandingTarget()
	{
		this.shouldLand = false;
	}

	// Token: 0x06002601 RID: 9729 RVA: 0x000EFB4C File Offset: 0x000EDD4C
	public void TriggeredEventSpawn()
	{
		float x = TerrainMeta.Size.x;
		float y = 30f;
		Vector3 vector = Vector3Ex.Range(-1f, 1f);
		vector.y = 0f;
		vector.Normalize();
		vector *= x * 1f;
		vector.y = y;
		base.transform.position = vector;
	}

	// Token: 0x06002602 RID: 9730 RVA: 0x000EFBAF File Offset: 0x000EDDAF
	public override void AttemptMount(BasePlayer player, bool doMountChecks = true)
	{
		if (!player.IsNpc && !player.IsAdmin)
		{
			return;
		}
		base.AttemptMount(player, doMountChecks);
	}

	// Token: 0x06002603 RID: 9731 RVA: 0x000EFBCA File Offset: 0x000EDDCA
	public override void ServerInit()
	{
		base.ServerInit();
		base.Invoke(new Action(this.SpawnScientists), 0.25f);
		this.SetMoveTarget(base.transform.position);
	}

	// Token: 0x06002604 RID: 9732 RVA: 0x000EFBFC File Offset: 0x000EDDFC
	public void SpawnPassenger(Vector3 spawnPos, string prefabPath)
	{
		Quaternion identity = Quaternion.identity;
		HumanNPC component = GameManager.server.CreateEntity(prefabPath, spawnPos, identity, true).GetComponent<HumanNPC>();
		component.Spawn();
		this.AttemptMount(component, true);
	}

	// Token: 0x06002605 RID: 9733 RVA: 0x000EFC34 File Offset: 0x000EDE34
	public void SpawnPassenger(Vector3 spawnPos)
	{
		Quaternion identity = Quaternion.identity;
		HumanNPC component = GameManager.server.CreateEntity(this.dismountablePrefab.resourcePath, spawnPos, identity, true).GetComponent<HumanNPC>();
		component.Spawn();
		this.AttemptMount(component, true);
	}

	// Token: 0x06002606 RID: 9734 RVA: 0x000EFC74 File Offset: 0x000EDE74
	public void SpawnScientist(Vector3 spawnPos)
	{
		Quaternion identity = Quaternion.identity;
		HumanNPC component = GameManager.server.CreateEntity(this.scientistPrefab.resourcePath, spawnPos, identity, true).GetComponent<HumanNPC>();
		component.Spawn();
		this.AttemptMount(component, true);
		component.Brain.SetEnabled(false);
	}

	// Token: 0x06002607 RID: 9735 RVA: 0x000EFCC0 File Offset: 0x000EDEC0
	public void SpawnScientists()
	{
		if (this.shouldLand)
		{
			float dropoffScale = CH47LandingZone.GetClosest(this.landingTarget).dropoffScale;
			int num = Mathf.FloorToInt((float)(this.mountPoints.Count - 2) * dropoffScale);
			for (int i = 0; i < num; i++)
			{
				Vector3 spawnPos = base.transform.position + base.transform.forward * 10f;
				this.SpawnPassenger(spawnPos, this.dismountablePrefab.resourcePath);
			}
			for (int j = 0; j < 1; j++)
			{
				Vector3 spawnPos2 = base.transform.position - base.transform.forward * 15f;
				this.SpawnPassenger(spawnPos2);
			}
			return;
		}
		for (int k = 0; k < 4; k++)
		{
			Vector3 spawnPos3 = base.transform.position + base.transform.forward * 10f;
			this.SpawnScientist(spawnPos3);
		}
		for (int l = 0; l < 1; l++)
		{
			Vector3 spawnPos4 = base.transform.position - base.transform.forward * 15f;
			this.SpawnScientist(spawnPos4);
		}
	}

	// Token: 0x06002608 RID: 9736 RVA: 0x000EFE01 File Offset: 0x000EE001
	public void EnableFacingOverride(bool enabled)
	{
		this.aimDirOverride = enabled;
	}

	// Token: 0x06002609 RID: 9737 RVA: 0x000EFE0A File Offset: 0x000EE00A
	public void SetMoveTarget(Vector3 position)
	{
		this._moveTarget = position;
	}

	// Token: 0x0600260A RID: 9738 RVA: 0x000EFE13 File Offset: 0x000EE013
	public Vector3 GetMoveTarget()
	{
		return this._moveTarget;
	}

	// Token: 0x0600260B RID: 9739 RVA: 0x000EFE1B File Offset: 0x000EE01B
	public void SetAimDirection(Vector3 dir)
	{
		this._aimDirection = dir;
	}

	// Token: 0x0600260C RID: 9740 RVA: 0x000EFE24 File Offset: 0x000EE024
	public Vector3 GetAimDirectionOverride()
	{
		return this._aimDirection;
	}

	// Token: 0x0600260D RID: 9741 RVA: 0x0002C673 File Offset: 0x0002A873
	public Vector3 GetPosition()
	{
		return base.transform.position;
	}

	// Token: 0x0600260E RID: 9742 RVA: 0x000EFE2C File Offset: 0x000EE02C
	public override void MounteeTookDamage(BasePlayer mountee, HitInfo info)
	{
		this.InitiateAnger();
	}

	// Token: 0x0600260F RID: 9743 RVA: 0x000EFE34 File Offset: 0x000EE034
	public void CancelAnger()
	{
		if (base.SecondsSinceAttacked > 120f)
		{
			this.UnHostile();
			base.CancelInvoke(new Action(this.UnHostile));
		}
	}

	// Token: 0x06002610 RID: 9744 RVA: 0x000EFE5C File Offset: 0x000EE05C
	public void InitiateAnger()
	{
		base.CancelInvoke(new Action(this.UnHostile));
		base.Invoke(new Action(this.UnHostile), 120f);
		foreach (BaseVehicle.MountPointInfo mountPointInfo in this.mountPoints)
		{
			if (mountPointInfo.mountable != null)
			{
				BasePlayer mounted = mountPointInfo.mountable.GetMounted();
				if (mounted)
				{
					ScientistNPC scientistNPC = mounted as ScientistNPC;
					if (scientistNPC != null)
					{
						scientistNPC.Brain.SetEnabled(true);
					}
				}
			}
		}
	}

	// Token: 0x06002611 RID: 9745 RVA: 0x000EFF10 File Offset: 0x000EE110
	public void UnHostile()
	{
		foreach (BaseVehicle.MountPointInfo mountPointInfo in this.mountPoints)
		{
			if (mountPointInfo.mountable != null)
			{
				BasePlayer mounted = mountPointInfo.mountable.GetMounted();
				if (mounted)
				{
					ScientistNPC scientistNPC = mounted as ScientistNPC;
					if (scientistNPC != null)
					{
						scientistNPC.Brain.SetEnabled(false);
					}
				}
			}
		}
	}

	// Token: 0x06002612 RID: 9746 RVA: 0x000EFF9C File Offset: 0x000EE19C
	public override void OnKilled(HitInfo info)
	{
		if (!this.OutOfCrates())
		{
			this.DropCrate();
		}
		base.OnKilled(info);
	}

	// Token: 0x06002613 RID: 9747 RVA: 0x000EFFB4 File Offset: 0x000EE1B4
	public override void OnAttacked(HitInfo info)
	{
		base.OnAttacked(info);
		this.InitiateAnger();
		base.SetFlag(BaseEntity.Flags.Reserved7, base.healthFraction <= 0.8f, false, true);
		base.SetFlag(BaseEntity.Flags.OnFire, base.healthFraction <= 0.33f, false, true);
	}

	// Token: 0x06002614 RID: 9748 RVA: 0x000F0004 File Offset: 0x000EE204
	public void DelayedKill()
	{
		foreach (BaseVehicle.MountPointInfo mountPointInfo in this.mountPoints)
		{
			if (mountPointInfo.mountable != null)
			{
				BasePlayer mounted = mountPointInfo.mountable.GetMounted();
				if (mounted && mounted.transform != null && !mounted.IsDestroyed && !mounted.IsDead() && mounted.IsNpc)
				{
					mounted.Kill(BaseNetworkable.DestroyMode.None);
				}
			}
		}
		base.Kill(BaseNetworkable.DestroyMode.None);
	}

	// Token: 0x06002615 RID: 9749 RVA: 0x000F00A8 File Offset: 0x000EE2A8
	public override void DismountAllPlayers()
	{
		foreach (BaseVehicle.MountPointInfo mountPointInfo in this.mountPoints)
		{
			if (mountPointInfo.mountable != null)
			{
				BasePlayer mounted = mountPointInfo.mountable.GetMounted();
				if (mounted)
				{
					mounted.Hurt(10000f, DamageType.Explosion, this, false);
				}
			}
		}
	}

	// Token: 0x06002616 RID: 9750 RVA: 0x000F0128 File Offset: 0x000EE328
	public void SetAltitudeProtection(bool on)
	{
		this.altitudeProtection = on;
	}

	// Token: 0x06002617 RID: 9751 RVA: 0x000F0134 File Offset: 0x000EE334
	public void CalculateDesiredAltitude()
	{
		this.CalculateOverrideAltitude();
		if (this.altOverride > this.currentDesiredAltitude)
		{
			this.currentDesiredAltitude = this.altOverride;
			return;
		}
		this.currentDesiredAltitude = Mathf.MoveTowards(this.currentDesiredAltitude, this.altOverride, Time.fixedDeltaTime * 5f);
	}

	// Token: 0x06002618 RID: 9752 RVA: 0x000F0185 File Offset: 0x000EE385
	public void SetMinHoverHeight(float newHeight)
	{
		this.hoverHeight = newHeight;
	}

	// Token: 0x06002619 RID: 9753 RVA: 0x000F0190 File Offset: 0x000EE390
	public float CalculateOverrideAltitude()
	{
		if (Time.frameCount == this.lastAltitudeCheckFrame)
		{
			return this.altOverride;
		}
		this.lastAltitudeCheckFrame = Time.frameCount;
		float y = this.GetMoveTarget().y;
		float num = Mathf.Max(TerrainMeta.WaterMap.GetHeight(this.GetMoveTarget()), TerrainMeta.HeightMap.GetHeight(this.GetMoveTarget()));
		float num2 = Mathf.Max(y, num + this.hoverHeight);
		if (this.altitudeProtection)
		{
			Vector3 rhs = (this.rigidBody.velocity.magnitude < 0.1f) ? base.transform.forward : this.rigidBody.velocity.normalized;
			Vector3 normalized = (Vector3.Cross(Vector3.Cross(base.transform.up, rhs), Vector3.up) + Vector3.down * 0.3f).normalized;
			RaycastHit raycastHit;
			RaycastHit raycastHit2;
			if (Physics.SphereCast(base.transform.position - normalized * 20f, 20f, normalized, out raycastHit, 75f, 1218511105) && Physics.SphereCast(raycastHit.point + Vector3.up * 200f, 20f, Vector3.down, out raycastHit2, 200f, 1218511105))
			{
				num2 = raycastHit2.point.y + this.hoverHeight;
			}
		}
		this.altOverride = num2;
		return this.altOverride;
	}

	// Token: 0x0600261A RID: 9754 RVA: 0x000F030C File Offset: 0x000EE50C
	public override void SetDefaultInputState()
	{
		this.currentInputState.Reset();
		Vector3 moveTarget = this.GetMoveTarget();
		Vector3 vector = Vector3.Cross(base.transform.right, Vector3.up);
		Vector3 vector2 = Vector3.Cross(Vector3.up, vector);
		float num = -Vector3.Dot(Vector3.up, base.transform.right);
		float num2 = Vector3.Dot(Vector3.up, base.transform.forward);
		float num3 = Vector3Ex.Distance2D(base.transform.position, moveTarget);
		float y = base.transform.position.y;
		float num4 = this.currentDesiredAltitude;
		(base.transform.position + base.transform.forward * 10f).y = num4;
		Vector3 lhs = Vector3Ex.Direction2D(moveTarget, base.transform.position);
		float num5 = -Vector3.Dot(lhs, vector2);
		float num6 = Vector3.Dot(lhs, vector);
		float num7 = Mathf.InverseLerp(0f, 25f, num3);
		if (num6 > 0f)
		{
			float num8 = Mathf.InverseLerp(-this.maxTiltAngle, 0f, num2);
			this.currentInputState.pitch = 1f * num6 * num8 * num7;
		}
		else
		{
			float num9 = 1f - Mathf.InverseLerp(0f, this.maxTiltAngle, num2);
			this.currentInputState.pitch = 1f * num6 * num9 * num7;
		}
		if (num5 > 0f)
		{
			float num10 = Mathf.InverseLerp(-this.maxTiltAngle, 0f, num);
			this.currentInputState.roll = 1f * num5 * num10 * num7;
		}
		else
		{
			float num11 = 1f - Mathf.InverseLerp(0f, this.maxTiltAngle, num);
			this.currentInputState.roll = 1f * num5 * num11 * num7;
		}
		float value = Mathf.Abs(num4 - y);
		float num12 = 1f - Mathf.InverseLerp(10f, 30f, value);
		this.currentInputState.pitch *= num12;
		this.currentInputState.roll *= num12;
		float num13 = this.maxTiltAngle;
		float num14 = Mathf.InverseLerp(0f + Mathf.Abs(this.currentInputState.pitch) * num13, num13 + Mathf.Abs(this.currentInputState.pitch) * num13, Mathf.Abs(num2));
		this.currentInputState.pitch += num14 * ((num2 < 0f) ? -1f : 1f);
		float num15 = Mathf.InverseLerp(0f + Mathf.Abs(this.currentInputState.roll) * num13, num13 + Mathf.Abs(this.currentInputState.roll) * num13, Mathf.Abs(num));
		this.currentInputState.roll += num15 * ((num < 0f) ? -1f : 1f);
		if (this.aimDirOverride || num3 > 30f)
		{
			Vector3 rhs = this.aimDirOverride ? this.GetAimDirectionOverride() : Vector3Ex.Direction2D(this.GetMoveTarget(), base.transform.position);
			Vector3 to = this.aimDirOverride ? this.GetAimDirectionOverride() : Vector3Ex.Direction2D(this.GetMoveTarget(), base.transform.position);
			float num16 = Vector3.Dot(vector2, rhs);
			float f = Vector3.Angle(vector, to);
			float num17 = Mathf.InverseLerp(0f, 70f, Mathf.Abs(f));
			this.currentInputState.yaw = ((num16 > 0f) ? 1f : 0f);
			this.currentInputState.yaw -= ((num16 < 0f) ? 1f : 0f);
			this.currentInputState.yaw *= num17;
		}
		float throttle = Mathf.InverseLerp(5f, 30f, num3);
		this.currentInputState.throttle = throttle;
	}

	// Token: 0x0600261B RID: 9755 RVA: 0x000F0710 File Offset: 0x000EE910
	public void MaintainAIAltutide()
	{
		ref Vector3 ptr = base.transform.position + this.rigidBody.velocity;
		float num = this.currentDesiredAltitude;
		float y = ptr.y;
		float value = Mathf.Abs(num - y);
		bool flag = num > y;
		float d = Mathf.InverseLerp(0f, 10f, value) * this.AiAltitudeForce * (flag ? 1f : -1f);
		this.rigidBody.AddForce(Vector3.up * d, ForceMode.Force);
	}

	// Token: 0x0600261C RID: 9756 RVA: 0x000F0794 File Offset: 0x000EE994
	public override void VehicleFixedUpdate()
	{
		this.hoverForceScale = 1f;
		base.VehicleFixedUpdate();
		base.SetFlag(BaseEntity.Flags.Reserved5, TOD_Sky.Instance.IsNight, false, true);
		this.CalculateDesiredAltitude();
		this.MaintainAIAltutide();
	}

	// Token: 0x0600261D RID: 9757 RVA: 0x000F07CC File Offset: 0x000EE9CC
	public override void DestroyShared()
	{
		if (base.isServer)
		{
			foreach (BaseVehicle.MountPointInfo mountPointInfo in this.mountPoints)
			{
				if (mountPointInfo.mountable != null)
				{
					BasePlayer mounted = mountPointInfo.mountable.GetMounted();
					if (mounted && mounted.transform != null && !mounted.IsDestroyed && !mounted.IsDead() && mounted.IsNpc)
					{
						mounted.Kill(BaseNetworkable.DestroyMode.None);
					}
				}
			}
		}
		base.DestroyShared();
	}
}
