using System;

// Token: 0x02000224 RID: 548
public class AmbienceZone : TriggerBase, IClientComponentEx
{
	// Token: 0x040013CB RID: 5067
	public AmbienceDefinitionList baseAmbience;

	// Token: 0x040013CC RID: 5068
	public AmbienceDefinitionList stings;

	// Token: 0x040013CD RID: 5069
	public float priority;

	// Token: 0x040013CE RID: 5070
	public bool overrideCrossfadeTime;

	// Token: 0x040013CF RID: 5071
	public float crossfadeTime = 1f;

	// Token: 0x06001BC0 RID: 7104 RVA: 0x000C30C2 File Offset: 0x000C12C2
	public virtual void PreClientComponentCull(IPrefabProcessor p)
	{
		p.RemoveComponent(this);
		p.NominateForDeletion(base.gameObject);
	}
}
