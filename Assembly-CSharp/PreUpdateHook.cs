using System;
using UnityEngine;

// Token: 0x02000911 RID: 2321
public class PreUpdateHook : MonoBehaviour
{
	// Token: 0x0400331B RID: 13083
	public static Action OnUpdate;

	// Token: 0x0400331C RID: 13084
	public static Action OnLateUpdate;

	// Token: 0x0400331D RID: 13085
	public static Action OnFixedUpdate;

	// Token: 0x06003813 RID: 14355 RVA: 0x0014EF27 File Offset: 0x0014D127
	private void Update()
	{
		Action onUpdate = PreUpdateHook.OnUpdate;
		if (onUpdate == null)
		{
			return;
		}
		onUpdate();
	}

	// Token: 0x06003814 RID: 14356 RVA: 0x0014EF38 File Offset: 0x0014D138
	private void LateUpdate()
	{
		Action onLateUpdate = PreUpdateHook.OnLateUpdate;
		if (onLateUpdate == null)
		{
			return;
		}
		onLateUpdate();
	}

	// Token: 0x06003815 RID: 14357 RVA: 0x0014EF49 File Offset: 0x0014D149
	private void FixedUpdate()
	{
		Action onFixedUpdate = PreUpdateHook.OnFixedUpdate;
		if (onFixedUpdate == null)
		{
			return;
		}
		onFixedUpdate();
	}
}
