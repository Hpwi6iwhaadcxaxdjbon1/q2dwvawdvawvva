using System;
using UnityEngine;

// Token: 0x0200096F RID: 2415
public class RandomParameterNumber : StateMachineBehaviour
{
	// Token: 0x040033F4 RID: 13300
	public string parameterName;

	// Token: 0x040033F5 RID: 13301
	public int min;

	// Token: 0x040033F6 RID: 13302
	public int max;

	// Token: 0x040033F7 RID: 13303
	public bool preventRepetition;

	// Token: 0x040033F8 RID: 13304
	public bool isFloat;

	// Token: 0x040033F9 RID: 13305
	private int last;

	// Token: 0x060039D7 RID: 14807 RVA: 0x0015727C File Offset: 0x0015547C
	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		int num = UnityEngine.Random.Range(this.min, this.max);
		int num2 = 0;
		while (this.last == num && this.preventRepetition && num2 < 100)
		{
			num = UnityEngine.Random.Range(this.min, this.max);
			num2++;
		}
		if (this.isFloat)
		{
			animator.SetFloat(this.parameterName, (float)num);
		}
		else
		{
			animator.SetInteger(this.parameterName, num);
		}
		this.last = num;
	}
}
