using System;
using UnityEngine;

// Token: 0x02000968 RID: 2408
public class AlternateAttack : StateMachineBehaviour
{
	// Token: 0x040033C8 RID: 13256
	public bool random;

	// Token: 0x040033C9 RID: 13257
	public bool dontIncrement;

	// Token: 0x040033CA RID: 13258
	public string[] targetTransitions;

	// Token: 0x060039C6 RID: 14790 RVA: 0x00156D04 File Offset: 0x00154F04
	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if (this.random)
		{
			string stateName = this.targetTransitions[UnityEngine.Random.Range(0, this.targetTransitions.Length)];
			animator.Play(stateName, layerIndex, 0f);
			return;
		}
		int integer = animator.GetInteger("lastAttack");
		string stateName2 = this.targetTransitions[integer % this.targetTransitions.Length];
		animator.Play(stateName2, layerIndex, 0f);
		if (!this.dontIncrement)
		{
			animator.SetInteger("lastAttack", integer + 1);
		}
	}
}
