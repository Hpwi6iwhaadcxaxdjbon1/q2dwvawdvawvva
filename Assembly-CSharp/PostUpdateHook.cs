using System;
using UnityEngine;

// Token: 0x0200090F RID: 2319
public class PostUpdateHook : MonoBehaviour
{
	// Token: 0x0400330F RID: 13071
	public static Action OnUpdate;

	// Token: 0x04003310 RID: 13072
	public static Action OnLateUpdate;

	// Token: 0x04003311 RID: 13073
	public static Action OnFixedUpdate;

	// Token: 0x0600380F RID: 14351 RVA: 0x0014EEF4 File Offset: 0x0014D0F4
	private void Update()
	{
		Action onUpdate = PostUpdateHook.OnUpdate;
		if (onUpdate == null)
		{
			return;
		}
		onUpdate();
	}

	// Token: 0x06003810 RID: 14352 RVA: 0x0014EF05 File Offset: 0x0014D105
	private void LateUpdate()
	{
		Action onLateUpdate = PostUpdateHook.OnLateUpdate;
		if (onLateUpdate == null)
		{
			return;
		}
		onLateUpdate();
	}

	// Token: 0x06003811 RID: 14353 RVA: 0x0014EF16 File Offset: 0x0014D116
	private void FixedUpdate()
	{
		Action onFixedUpdate = PostUpdateHook.OnFixedUpdate;
		if (onFixedUpdate == null)
		{
			return;
		}
		onFixedUpdate();
	}
}
