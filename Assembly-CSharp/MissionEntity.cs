using System;
using UnityEngine;

// Token: 0x0200060D RID: 1549
public class MissionEntity : BaseMonoBehaviour, IOnParentDestroying
{
	// Token: 0x0400257A RID: 9594
	public bool cleanupOnMissionSuccess = true;

	// Token: 0x0400257B RID: 9595
	public bool cleanupOnMissionFailed = true;

	// Token: 0x06002DDB RID: 11739 RVA: 0x00113AF4 File Offset: 0x00111CF4
	public void OnParentDestroying()
	{
		UnityEngine.Object.Destroy(this);
	}

	// Token: 0x06002DDC RID: 11740 RVA: 0x00113AFC File Offset: 0x00111CFC
	public virtual void Setup(BasePlayer assignee, BaseMission.MissionInstance instance, bool wantsSuccessCleanup, bool wantsFailedCleanup)
	{
		this.cleanupOnMissionFailed = wantsFailedCleanup;
		this.cleanupOnMissionSuccess = wantsSuccessCleanup;
		BaseEntity entity = this.GetEntity();
		if (entity)
		{
			entity.SendMessage("MissionSetupPlayer", assignee, SendMessageOptions.DontRequireReceiver);
		}
	}

	// Token: 0x06002DDD RID: 11741 RVA: 0x00113B34 File Offset: 0x00111D34
	public virtual void MissionStarted(BasePlayer assignee, BaseMission.MissionInstance instance)
	{
		IMissionEntityListener[] componentsInChildren = base.GetComponentsInChildren<IMissionEntityListener>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].MissionStarted(assignee, instance);
		}
	}

	// Token: 0x06002DDE RID: 11742 RVA: 0x00113B60 File Offset: 0x00111D60
	public virtual void MissionEnded(BasePlayer assignee, BaseMission.MissionInstance instance)
	{
		IMissionEntityListener[] componentsInChildren = base.GetComponentsInChildren<IMissionEntityListener>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].MissionEnded(assignee, instance);
		}
		if (instance.createdEntities.Contains(this))
		{
			instance.createdEntities.Remove(this);
		}
		if ((this.cleanupOnMissionSuccess && (instance.status == BaseMission.MissionStatus.Completed || instance.status == BaseMission.MissionStatus.Accomplished)) || (this.cleanupOnMissionFailed && instance.status == BaseMission.MissionStatus.Failed))
		{
			BaseEntity entity = this.GetEntity();
			if (entity)
			{
				entity.Kill(BaseNetworkable.DestroyMode.None);
			}
		}
	}

	// Token: 0x06002DDF RID: 11743 RVA: 0x00113BF0 File Offset: 0x00111DF0
	public BaseEntity GetEntity()
	{
		return base.GetComponent<BaseEntity>();
	}
}
