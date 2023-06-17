using System;
using UnityEngine;

// Token: 0x02000239 RID: 569
public class ReverbZoneTrigger : TriggerBase, IClientComponentEx, ILOD
{
	// Token: 0x0400146F RID: 5231
	public Collider trigger;

	// Token: 0x04001470 RID: 5232
	public AudioReverbZone reverbZone;

	// Token: 0x04001471 RID: 5233
	public float lodDistance = 100f;

	// Token: 0x04001472 RID: 5234
	public bool inRange;

	// Token: 0x04001473 RID: 5235
	public ReverbSettings reverbSettings;

	// Token: 0x06001C03 RID: 7171 RVA: 0x000C3F64 File Offset: 0x000C2164
	public virtual void PreClientComponentCull(IPrefabProcessor p)
	{
		p.RemoveComponent(this.trigger);
		p.RemoveComponent(this.reverbZone);
		p.RemoveComponent(this);
		p.NominateForDeletion(base.gameObject);
	}

	// Token: 0x06001C04 RID: 7172 RVA: 0x00007A3C File Offset: 0x00005C3C
	public bool IsSyncedToParent()
	{
		return false;
	}
}
