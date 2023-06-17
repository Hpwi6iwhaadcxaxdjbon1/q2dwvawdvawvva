using System;
using UnityEngine;

// Token: 0x02000739 RID: 1849
public class SendMessageToEntityOnAnimationFinish : StateMachineBehaviour
{
	// Token: 0x040029F2 RID: 10738
	public string messageToSendToEntity;

	// Token: 0x040029F3 RID: 10739
	public float repeatRate = 0.1f;

	// Token: 0x040029F4 RID: 10740
	private const float lastMessageSent = 0f;

	// Token: 0x06003379 RID: 13177 RVA: 0x0013C24C File Offset: 0x0013A44C
	public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if (0f + this.repeatRate > Time.time)
		{
			return;
		}
		if (animator.IsInTransition(layerIndex))
		{
			return;
		}
		if (stateInfo.normalizedTime < 1f)
		{
			return;
		}
		for (int i = 0; i < animator.layerCount; i++)
		{
			if (i != layerIndex)
			{
				if (animator.IsInTransition(i))
				{
					return;
				}
				AnimatorStateInfo currentAnimatorStateInfo = animator.GetCurrentAnimatorStateInfo(i);
				if (currentAnimatorStateInfo.speed > 0f && currentAnimatorStateInfo.normalizedTime < 1f)
				{
					return;
				}
			}
		}
		BaseEntity baseEntity = animator.gameObject.ToBaseEntity();
		if (baseEntity)
		{
			baseEntity.SendMessage(this.messageToSendToEntity, SendMessageOptions.DontRequireReceiver);
		}
	}
}
