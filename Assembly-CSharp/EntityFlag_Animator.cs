using System;
using UnityEngine;

// Token: 0x020003C4 RID: 964
public class EntityFlag_Animator : EntityFlag_Toggle
{
	// Token: 0x04001A0C RID: 6668
	public Animator TargetAnimator;

	// Token: 0x04001A0D RID: 6669
	public string ParamName = string.Empty;

	// Token: 0x04001A0E RID: 6670
	public EntityFlag_Animator.AnimatorMode AnimationMode;

	// Token: 0x04001A0F RID: 6671
	public float FloatOnState;

	// Token: 0x04001A10 RID: 6672
	public float FloatOffState;

	// Token: 0x04001A11 RID: 6673
	public int IntegerOnState;

	// Token: 0x04001A12 RID: 6674
	public int IntegerOffState;

	// Token: 0x02000CBC RID: 3260
	public enum AnimatorMode
	{
		// Token: 0x040044A0 RID: 17568
		Bool,
		// Token: 0x040044A1 RID: 17569
		Float,
		// Token: 0x040044A2 RID: 17570
		Trigger,
		// Token: 0x040044A3 RID: 17571
		Integer
	}
}
