using System;
using UnityEngine;

// Token: 0x0200058C RID: 1420
public class TriggerNotifyEntity : TriggerBase, IPrefabPreProcess
{
	// Token: 0x0400232C RID: 9004
	public GameObject notifyTarget;

	// Token: 0x0400232D RID: 9005
	private INotifyEntityTrigger toNotify;

	// Token: 0x0400232E RID: 9006
	public bool runClientside = true;

	// Token: 0x0400232F RID: 9007
	public bool runServerside = true;

	// Token: 0x17000398 RID: 920
	// (get) Token: 0x06002B64 RID: 11108 RVA: 0x00107416 File Offset: 0x00105616
	public bool HasContents
	{
		get
		{
			return this.contents != null && this.contents.Count > 0;
		}
	}

	// Token: 0x06002B65 RID: 11109 RVA: 0x001074E6 File Offset: 0x001056E6
	internal override void OnEntityEnter(BaseEntity ent)
	{
		base.OnEntityEnter(ent);
		if (this.toNotify != null || (this.notifyTarget != null && this.notifyTarget.TryGetComponent<INotifyEntityTrigger>(out this.toNotify)))
		{
			this.toNotify.OnEntityEnter(ent);
		}
	}

	// Token: 0x06002B66 RID: 11110 RVA: 0x00107524 File Offset: 0x00105724
	internal override void OnEntityLeave(BaseEntity ent)
	{
		base.OnEntityLeave(ent);
		if (this.toNotify != null || (this.notifyTarget != null && this.notifyTarget.TryGetComponent<INotifyEntityTrigger>(out this.toNotify)))
		{
			this.toNotify.OnEntityLeave(ent);
		}
	}

	// Token: 0x06002B67 RID: 11111 RVA: 0x00107562 File Offset: 0x00105762
	public void PreProcess(IPrefabProcessor preProcess, GameObject rootObj, string name, bool serverside, bool clientside, bool bundling)
	{
		if ((!clientside || !this.runClientside) && (!serverside || !this.runServerside))
		{
			preProcess.RemoveComponent(this);
		}
	}
}
