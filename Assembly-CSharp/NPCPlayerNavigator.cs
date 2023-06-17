using System;
using UnityEngine;
using UnityEngine.AI;

// Token: 0x02000206 RID: 518
public class NPCPlayerNavigator : BaseNavigator
{
	// Token: 0x1700024E RID: 590
	// (get) Token: 0x06001B2C RID: 6956 RVA: 0x000C0C5C File Offset: 0x000BEE5C
	// (set) Token: 0x06001B2D RID: 6957 RVA: 0x000C0C64 File Offset: 0x000BEE64
	public NPCPlayer NPCPlayerEntity { get; private set; }

	// Token: 0x06001B2E RID: 6958 RVA: 0x000C0C6D File Offset: 0x000BEE6D
	public override void Init(BaseCombatEntity entity, NavMeshAgent agent)
	{
		base.Init(entity, agent);
		this.NPCPlayerEntity = (entity as NPCPlayer);
	}

	// Token: 0x06001B2F RID: 6959 RVA: 0x000C0C83 File Offset: 0x000BEE83
	protected override bool CanEnableNavMeshNavigation()
	{
		return base.CanEnableNavMeshNavigation() && (!this.NPCPlayerEntity.isMounted || this.CanNavigateMounted);
	}

	// Token: 0x06001B30 RID: 6960 RVA: 0x000C0CA8 File Offset: 0x000BEEA8
	protected override bool CanUpdateMovement()
	{
		if (!base.CanUpdateMovement())
		{
			return false;
		}
		if (this.NPCPlayerEntity.IsWounded())
		{
			return false;
		}
		if (base.CurrentNavigationType == BaseNavigator.NavigationType.NavMesh && (this.NPCPlayerEntity.IsDormant || !this.NPCPlayerEntity.syncPosition) && base.Agent.enabled)
		{
			base.SetDestination(this.NPCPlayerEntity.ServerPosition, 1f, 0f, 0f);
			return false;
		}
		return true;
	}

	// Token: 0x06001B31 RID: 6961 RVA: 0x000C0D24 File Offset: 0x000BEF24
	protected override void UpdatePositionAndRotation(Vector3 moveToPosition, float delta)
	{
		base.UpdatePositionAndRotation(moveToPosition, delta);
		if (this.overrideFacingDirectionMode == BaseNavigator.OverrideFacingDirectionMode.None)
		{
			if (base.CurrentNavigationType == BaseNavigator.NavigationType.NavMesh)
			{
				this.NPCPlayerEntity.SetAimDirection(base.Agent.desiredVelocity.normalized);
				return;
			}
			if (base.CurrentNavigationType == BaseNavigator.NavigationType.AStar || base.CurrentNavigationType == BaseNavigator.NavigationType.Base)
			{
				this.NPCPlayerEntity.SetAimDirection(Vector3Ex.Direction2D(moveToPosition, base.transform.position));
			}
		}
	}

	// Token: 0x06001B32 RID: 6962 RVA: 0x000C0D98 File Offset: 0x000BEF98
	public override void ApplyFacingDirectionOverride()
	{
		base.ApplyFacingDirectionOverride();
		if (this.overrideFacingDirectionMode == BaseNavigator.OverrideFacingDirectionMode.None)
		{
			return;
		}
		if (this.overrideFacingDirectionMode == BaseNavigator.OverrideFacingDirectionMode.Direction)
		{
			this.NPCPlayerEntity.SetAimDirection(this.facingDirectionOverride);
			return;
		}
		if (this.facingDirectionEntity != null)
		{
			Vector3 aimDirection = NPCPlayerNavigator.GetAimDirection(this.NPCPlayerEntity, this.facingDirectionEntity);
			this.facingDirectionOverride = aimDirection;
			this.NPCPlayerEntity.SetAimDirection(this.facingDirectionOverride);
		}
	}

	// Token: 0x06001B33 RID: 6963 RVA: 0x000C0E08 File Offset: 0x000BF008
	private static Vector3 GetAimDirection(BasePlayer aimingPlayer, BaseEntity target)
	{
		if (target == null)
		{
			return Vector3Ex.Direction2D(aimingPlayer.transform.position + aimingPlayer.eyes.BodyForward() * 1000f, aimingPlayer.transform.position);
		}
		if (Vector3Ex.Distance2D(aimingPlayer.transform.position, target.transform.position) <= 0.75f)
		{
			return Vector3Ex.Direction2D(target.transform.position, aimingPlayer.transform.position);
		}
		return (NPCPlayerNavigator.TargetAimPositionOffset(target) - aimingPlayer.eyes.position).normalized;
	}

	// Token: 0x06001B34 RID: 6964 RVA: 0x000C0EB0 File Offset: 0x000BF0B0
	private static Vector3 TargetAimPositionOffset(BaseEntity target)
	{
		BasePlayer basePlayer = target as BasePlayer;
		if (!(basePlayer != null))
		{
			return target.CenterPoint();
		}
		if (basePlayer.IsSleeping() || basePlayer.IsWounded())
		{
			return basePlayer.transform.position + Vector3.up * 0.1f;
		}
		return basePlayer.eyes.position - Vector3.up * 0.15f;
	}
}
