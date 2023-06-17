using System;
using UnityEngine;

// Token: 0x0200022F RID: 559
public class MovementSoundTrigger : TriggerBase, IClientComponentEx, ILOD
{
	// Token: 0x04001425 RID: 5157
	public SoundDefinition softSound;

	// Token: 0x04001426 RID: 5158
	public SoundDefinition medSound;

	// Token: 0x04001427 RID: 5159
	public SoundDefinition hardSound;

	// Token: 0x04001428 RID: 5160
	public Collider collider;

	// Token: 0x06001BD8 RID: 7128 RVA: 0x000C3649 File Offset: 0x000C1849
	public virtual void PreClientComponentCull(IPrefabProcessor p)
	{
		p.RemoveComponent(this.collider);
		p.RemoveComponent(this);
		p.NominateForDeletion(base.gameObject);
	}
}
