using System;
using UnityEngine;

// Token: 0x02000970 RID: 2416
public class RandomParameterNumberFloat : StateMachineBehaviour
{
	// Token: 0x040033FA RID: 13306
	public string parameterName;

	// Token: 0x040033FB RID: 13307
	public int min;

	// Token: 0x040033FC RID: 13308
	public int max;

	// Token: 0x060039D9 RID: 14809 RVA: 0x001572F7 File Offset: 0x001554F7
	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if (string.IsNullOrEmpty(this.parameterName))
		{
			return;
		}
		animator.SetFloat(this.parameterName, Mathf.Floor(UnityEngine.Random.Range((float)this.min, (float)this.max + 0.5f)));
	}
}
