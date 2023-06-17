using System;
using UnityEngine;

// Token: 0x02000917 RID: 2327
public class StartOfFrameHook : MonoBehaviour
{
	// Token: 0x04003333 RID: 13107
	public static Action OnStartOfFrame;

	// Token: 0x0600382C RID: 14380 RVA: 0x0014F238 File Offset: 0x0014D438
	private void OnEnable()
	{
		Action onStartOfFrame = StartOfFrameHook.OnStartOfFrame;
		if (onStartOfFrame == null)
		{
			return;
		}
		onStartOfFrame();
	}

	// Token: 0x0600382D RID: 14381 RVA: 0x0014F249 File Offset: 0x0014D449
	private void Update()
	{
		base.gameObject.SetActive(false);
		base.gameObject.SetActive(true);
	}
}
