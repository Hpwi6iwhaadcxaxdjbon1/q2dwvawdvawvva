using System;
using UnityEngine;

// Token: 0x0200028A RID: 650
public class AvatarSwitcher : StateMachineBehaviour
{
	// Token: 0x040015BC RID: 5564
	public Avatar ToApply;

	// Token: 0x06001D02 RID: 7426 RVA: 0x000C8A9D File Offset: 0x000C6C9D
	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		base.OnStateEnter(animator, stateInfo, layerIndex);
		if (this.ToApply != null)
		{
			animator.avatar = this.ToApply;
			animator.Play(stateInfo.shortNameHash, layerIndex);
		}
	}
}
