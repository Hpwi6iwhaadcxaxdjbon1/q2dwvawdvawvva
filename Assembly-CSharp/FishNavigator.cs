using System;
using UnityEngine;
using UnityEngine.AI;

// Token: 0x02000204 RID: 516
public class FishNavigator : BaseNavigator
{
	// Token: 0x1700024C RID: 588
	// (get) Token: 0x06001B1B RID: 6939 RVA: 0x000C09C7 File Offset: 0x000BEBC7
	// (set) Token: 0x06001B1C RID: 6940 RVA: 0x000C09CF File Offset: 0x000BEBCF
	public BaseNpc NPC { get; private set; }

	// Token: 0x06001B1D RID: 6941 RVA: 0x000C09D8 File Offset: 0x000BEBD8
	public override void Init(BaseCombatEntity entity, NavMeshAgent agent)
	{
		base.Init(entity, agent);
		this.NPC = (entity as BaseNpc);
	}

	// Token: 0x06001B1E RID: 6942 RVA: 0x000C09EE File Offset: 0x000BEBEE
	protected override bool SetCustomDestination(Vector3 pos, float speedFraction = 1f, float updateInterval = 0f)
	{
		if (!base.SetCustomDestination(pos, speedFraction, updateInterval))
		{
			return false;
		}
		base.Destination = pos;
		return true;
	}

	// Token: 0x06001B1F RID: 6943 RVA: 0x000C0A08 File Offset: 0x000BEC08
	protected override void UpdatePositionAndRotation(Vector3 moveToPosition, float delta)
	{
		base.transform.position = Vector3.MoveTowards(base.transform.position, moveToPosition, this.GetTargetSpeed() * delta);
		base.BaseEntity.ServerPosition = base.transform.localPosition;
		if (base.ReachedPosition(moveToPosition))
		{
			base.Stop();
			return;
		}
		this.UpdateRotation(moveToPosition, delta);
	}

	// Token: 0x06001B20 RID: 6944 RVA: 0x000C0A67 File Offset: 0x000BEC67
	private void UpdateRotation(Vector3 moveToPosition, float delta)
	{
		base.BaseEntity.ServerRotation = Quaternion.LookRotation(Vector3Ex.Direction(moveToPosition, base.transform.position));
	}
}
