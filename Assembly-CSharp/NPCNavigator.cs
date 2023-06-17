using System;
using ConVar;
using UnityEngine;
using UnityEngine.AI;

// Token: 0x02000205 RID: 517
public class NPCNavigator : BaseNavigator
{
	// Token: 0x1700024D RID: 589
	// (get) Token: 0x06001B22 RID: 6946 RVA: 0x000C0A92 File Offset: 0x000BEC92
	// (set) Token: 0x06001B23 RID: 6947 RVA: 0x000C0A9A File Offset: 0x000BEC9A
	public BaseNpc NPC { get; private set; }

	// Token: 0x06001B24 RID: 6948 RVA: 0x000C0AA3 File Offset: 0x000BECA3
	public override void Init(BaseCombatEntity entity, NavMeshAgent agent)
	{
		base.Init(entity, agent);
		this.NPC = (entity as BaseNpc);
	}

	// Token: 0x06001B25 RID: 6949 RVA: 0x000C0AB9 File Offset: 0x000BECB9
	protected override bool CanEnableNavMeshNavigation()
	{
		return base.CanEnableNavMeshNavigation();
	}

	// Token: 0x06001B26 RID: 6950 RVA: 0x000C0AC8 File Offset: 0x000BECC8
	protected override bool CanUpdateMovement()
	{
		if (!base.CanUpdateMovement())
		{
			return false;
		}
		if (this.NPC != null && (this.NPC.IsDormant || !this.NPC.syncPosition) && base.Agent.enabled)
		{
			base.SetDestination(this.NPC.ServerPosition, 1f, 0f, 0f);
			return false;
		}
		return true;
	}

	// Token: 0x06001B27 RID: 6951 RVA: 0x000C0B38 File Offset: 0x000BED38
	protected override void UpdatePositionAndRotation(Vector3 moveToPosition, float delta)
	{
		base.UpdatePositionAndRotation(moveToPosition, delta);
		this.UpdateRotation(moveToPosition, delta);
	}

	// Token: 0x06001B28 RID: 6952 RVA: 0x000C0B4C File Offset: 0x000BED4C
	private void UpdateRotation(Vector3 moveToPosition, float delta)
	{
		if (this.overrideFacingDirectionMode != BaseNavigator.OverrideFacingDirectionMode.None)
		{
			return;
		}
		if (this.traversingNavMeshLink)
		{
			Vector3 vector = base.Agent.destination - base.BaseEntity.ServerPosition;
			if (vector.sqrMagnitude > 1f)
			{
				vector = this.currentNavMeshLinkEndPos - base.BaseEntity.ServerPosition;
			}
			float sqrMagnitude = vector.sqrMagnitude;
			return;
		}
		if ((base.Agent.destination - base.BaseEntity.ServerPosition).sqrMagnitude > 1f)
		{
			Vector3 normalized = base.Agent.desiredVelocity.normalized;
			if (normalized.sqrMagnitude > 0.001f)
			{
				base.BaseEntity.ServerRotation = Quaternion.LookRotation(normalized);
				return;
			}
		}
	}

	// Token: 0x06001B29 RID: 6953 RVA: 0x000C0C18 File Offset: 0x000BEE18
	public override void ApplyFacingDirectionOverride()
	{
		base.ApplyFacingDirectionOverride();
		base.BaseEntity.ServerRotation = Quaternion.LookRotation(base.FacingDirectionOverride);
	}

	// Token: 0x06001B2A RID: 6954 RVA: 0x000C0C36 File Offset: 0x000BEE36
	public override bool IsSwimming()
	{
		return AI.npcswimming && this.NPC != null && this.NPC.swimming;
	}
}
