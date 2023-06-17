using System;
using UnityEngine;

// Token: 0x0200058A RID: 1418
public class TriggerNotify : TriggerBase, IPrefabPreProcess
{
	// Token: 0x04002328 RID: 9000
	public GameObject notifyTarget;

	// Token: 0x04002329 RID: 9001
	private INotifyTrigger toNotify;

	// Token: 0x0400232A RID: 9002
	public bool runClientside = true;

	// Token: 0x0400232B RID: 9003
	public bool runServerside = true;

	// Token: 0x17000397 RID: 919
	// (get) Token: 0x06002B5D RID: 11101 RVA: 0x00107416 File Offset: 0x00105616
	public bool HasContents
	{
		get
		{
			return this.contents != null && this.contents.Count > 0;
		}
	}

	// Token: 0x06002B5E RID: 11102 RVA: 0x00107430 File Offset: 0x00105630
	internal override void OnObjects()
	{
		base.OnObjects();
		if (this.toNotify != null || (this.notifyTarget != null && this.notifyTarget.TryGetComponent<INotifyTrigger>(out this.toNotify)))
		{
			this.toNotify.OnObjects(this);
		}
	}

	// Token: 0x06002B5F RID: 11103 RVA: 0x0010746D File Offset: 0x0010566D
	internal override void OnEmpty()
	{
		base.OnEmpty();
		if (this.toNotify != null || (this.notifyTarget != null && this.notifyTarget.TryGetComponent<INotifyTrigger>(out this.toNotify)))
		{
			this.toNotify.OnEmpty();
		}
	}

	// Token: 0x06002B60 RID: 11104 RVA: 0x001074A9 File Offset: 0x001056A9
	public void PreProcess(IPrefabProcessor preProcess, GameObject rootObj, string name, bool serverside, bool clientside, bool bundling)
	{
		if ((!clientside || !this.runClientside) && (!serverside || !this.runServerside))
		{
			preProcess.RemoveComponent(this);
		}
	}
}
