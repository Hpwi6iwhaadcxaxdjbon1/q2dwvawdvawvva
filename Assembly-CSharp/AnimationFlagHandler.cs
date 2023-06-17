using System;
using UnityEngine;

// Token: 0x0200015A RID: 346
public class AnimationFlagHandler : MonoBehaviour
{
	// Token: 0x04000FE0 RID: 4064
	public Animator animator;

	// Token: 0x06001735 RID: 5941 RVA: 0x000B0B80 File Offset: 0x000AED80
	public void SetBoolTrue(string name)
	{
		this.animator.SetBool(name, true);
	}

	// Token: 0x06001736 RID: 5942 RVA: 0x000B0B8F File Offset: 0x000AED8F
	public void SetBoolFalse(string name)
	{
		this.animator.SetBool(name, false);
	}
}
