using System;
using UnityEngine.Serialization;

// Token: 0x02000501 RID: 1281
public class EffectRecycle : BaseMonoBehaviour, IClientComponent, IRagdollInhert, IEffectRecycle
{
	// Token: 0x0400211D RID: 8477
	[FormerlySerializedAs("lifeTime")]
	[ReadOnly]
	public float detachTime;

	// Token: 0x0400211E RID: 8478
	[FormerlySerializedAs("lifeTime")]
	[ReadOnly]
	public float recycleTime;

	// Token: 0x0400211F RID: 8479
	public EffectRecycle.PlayMode playMode;

	// Token: 0x04002120 RID: 8480
	public EffectRecycle.ParentDestroyBehaviour onParentDestroyed;

	// Token: 0x02000D31 RID: 3377
	public enum PlayMode
	{
		// Token: 0x0400467B RID: 18043
		Once,
		// Token: 0x0400467C RID: 18044
		Looped
	}

	// Token: 0x02000D32 RID: 3378
	public enum ParentDestroyBehaviour
	{
		// Token: 0x0400467E RID: 18046
		Detach,
		// Token: 0x0400467F RID: 18047
		Destroy,
		// Token: 0x04004680 RID: 18048
		DetachWaitDestroy
	}
}
