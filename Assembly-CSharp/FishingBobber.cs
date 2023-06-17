using System;
using UnityEngine;

// Token: 0x020001B4 RID: 436
public class FishingBobber : BaseCombatEntity
{
	// Token: 0x04001184 RID: 4484
	public Transform centerOfMass;

	// Token: 0x04001185 RID: 4485
	public Rigidbody myRigidBody;

	// Token: 0x04001186 RID: 4486
	public Transform lineAttachPoint;

	// Token: 0x04001187 RID: 4487
	public Transform bobberRoot;

	// Token: 0x04001188 RID: 4488
	public const BaseEntity.Flags CaughtFish = BaseEntity.Flags.Reserved1;

	// Token: 0x04001189 RID: 4489
	public float HorizontalMoveSpeed = 1f;

	// Token: 0x0400118A RID: 4490
	public float PullAwayMoveSpeed = 1f;

	// Token: 0x0400118B RID: 4491
	public float SidewaysInputForce = 1f;

	// Token: 0x0400118C RID: 4492
	public float ReelInMoveSpeed = 1f;

	// Token: 0x0400118D RID: 4493
	private float bobberForcePingPong;

	// Token: 0x0400118E RID: 4494
	private Vector3 initialDirection;

	// Token: 0x04001190 RID: 4496
	private Vector3 initialTargetPosition;

	// Token: 0x04001191 RID: 4497
	private Vector3 spawnPosition;

	// Token: 0x04001192 RID: 4498
	private TimeSince initialCastTime;

	// Token: 0x04001193 RID: 4499
	private float initialDistance;

	// Token: 0x1700021D RID: 541
	// (get) Token: 0x060018E0 RID: 6368 RVA: 0x000B7F6F File Offset: 0x000B616F
	// (set) Token: 0x060018E1 RID: 6369 RVA: 0x000B7F77 File Offset: 0x000B6177
	public float TireAmount { get; private set; }

	// Token: 0x060018E2 RID: 6370 RVA: 0x000B7F80 File Offset: 0x000B6180
	public override void ServerInit()
	{
		this.myRigidBody.centerOfMass = this.centerOfMass.localPosition;
		base.ServerInit();
	}

	// Token: 0x060018E3 RID: 6371 RVA: 0x000B7FA0 File Offset: 0x000B61A0
	public void InitialiseBobber(BasePlayer forPlayer, WaterBody forBody, Vector3 targetPos)
	{
		this.initialDirection = forPlayer.eyes.HeadForward().WithY(0f);
		this.spawnPosition = base.transform.position;
		this.initialTargetPosition = targetPos;
		this.initialCastTime = 0f;
		this.initialDistance = Vector3.Distance(targetPos, forPlayer.transform.position.WithY(targetPos.y));
		base.InvokeRepeating(new Action(this.ProcessInitialCast), 0f, 0f);
	}

	// Token: 0x060018E4 RID: 6372 RVA: 0x000B8030 File Offset: 0x000B6230
	private void ProcessInitialCast()
	{
		float num = 0.8f;
		if (this.initialCastTime > num)
		{
			base.transform.position = this.initialTargetPosition;
			base.CancelInvoke(new Action(this.ProcessInitialCast));
			return;
		}
		float t = this.initialCastTime / num;
		Vector3 vector = Vector3.Lerp(this.spawnPosition, this.initialTargetPosition, 0.5f);
		vector.y += 1.5f;
		Vector3 position = Vector3.Lerp(Vector3.Lerp(this.spawnPosition, vector, t), Vector3.Lerp(vector, this.initialTargetPosition, t), t);
		base.transform.position = position;
	}

	// Token: 0x060018E5 RID: 6373 RVA: 0x000B80D8 File Offset: 0x000B62D8
	public void ServerMovementUpdate(bool inputLeft, bool inputRight, bool inputBack, ref BaseFishingRod.FishState state, Vector3 playerPos, ItemModFishable fishableModifier)
	{
		Vector3 normalized = (playerPos - base.transform.position).normalized;
		Vector3 vector = Vector3.zero;
		this.bobberForcePingPong = Mathf.Clamp(Mathf.PingPong(Time.time, 2f), 0.2f, 2f);
		if (state.Contains(BaseFishingRod.FishState.PullingLeft))
		{
			vector = base.transform.right * (Time.deltaTime * this.HorizontalMoveSpeed * this.bobberForcePingPong * fishableModifier.MoveMultiplier * (inputRight ? 0.5f : 1f));
		}
		if (state.Contains(BaseFishingRod.FishState.PullingRight))
		{
			vector = -base.transform.right * (Time.deltaTime * this.HorizontalMoveSpeed * this.bobberForcePingPong * fishableModifier.MoveMultiplier * (inputLeft ? 0.5f : 1f));
		}
		if (state.Contains(BaseFishingRod.FishState.PullingBack))
		{
			vector += -base.transform.forward * (Time.deltaTime * this.PullAwayMoveSpeed * this.bobberForcePingPong * fishableModifier.MoveMultiplier * (inputBack ? 0.5f : 1f));
		}
		if (inputLeft || inputRight)
		{
			float num = 0.8f;
			if ((inputLeft && state == BaseFishingRod.FishState.PullingRight) || (inputRight && state == BaseFishingRod.FishState.PullingLeft))
			{
				num = 1.25f;
			}
			this.TireAmount += Time.deltaTime * num;
		}
		else
		{
			this.TireAmount -= Time.deltaTime * 0.1f;
		}
		if (inputLeft && !state.Contains(BaseFishingRod.FishState.PullingLeft))
		{
			vector += base.transform.right * (Time.deltaTime * this.SidewaysInputForce);
		}
		else if (inputRight && !state.Contains(BaseFishingRod.FishState.PullingRight))
		{
			vector += -base.transform.right * (Time.deltaTime * this.SidewaysInputForce);
		}
		if (inputBack)
		{
			float num2 = Mathx.RemapValClamped(this.TireAmount, 0f, 5f, 1f, 3f);
			vector += normalized * (this.ReelInMoveSpeed * fishableModifier.ReelInSpeedMultiplier * num2 * Time.deltaTime);
		}
		base.transform.LookAt(playerPos.WithY(base.transform.position.y));
		Vector3 vector2 = base.transform.position + vector;
		if (!this.IsDirectionValid(vector2, vector.magnitude, playerPos))
		{
			state = state.FlipHorizontal();
			return;
		}
		base.transform.position = vector2;
	}

	// Token: 0x060018E6 RID: 6374 RVA: 0x000B8374 File Offset: 0x000B6574
	private bool IsDirectionValid(Vector3 pos, float checkLength, Vector3 playerPos)
	{
		if (Vector3.Angle((pos - playerPos).normalized.WithY(0f), this.initialDirection) > 60f)
		{
			return false;
		}
		Vector3 position = base.transform.position;
		RaycastHit raycastHit;
		return !GamePhysics.Trace(new Ray(position, (pos - position).normalized), 0.1f, out raycastHit, checkLength, 1218511105, QueryTriggerInteraction.UseGlobal, null);
	}
}
