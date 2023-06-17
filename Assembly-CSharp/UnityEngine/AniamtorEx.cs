using System;

namespace UnityEngine
{
	// Token: 0x02000A1C RID: 2588
	public static class AniamtorEx
	{
		// Token: 0x06003D81 RID: 15745 RVA: 0x00169694 File Offset: 0x00167894
		public static void SetFloatFixed(this Animator animator, int id, float value, float dampTime, float deltaTime)
		{
			if (value == 0f)
			{
				float @float = animator.GetFloat(id);
				if (@float == 0f)
				{
					return;
				}
				if (@float < 1E-45f)
				{
					animator.SetFloat(id, 0f);
					return;
				}
			}
			animator.SetFloat(id, value, dampTime, deltaTime);
		}

		// Token: 0x06003D82 RID: 15746 RVA: 0x001696DA File Offset: 0x001678DA
		public static void SetBoolChecked(this Animator animator, int id, bool value)
		{
			if (animator.GetBool(id) != value)
			{
				animator.SetBool(id, value);
			}
		}
	}
}
