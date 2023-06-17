using System;
using TMPro;
using UnityEngine;

// Token: 0x020008D7 RID: 2263
public class UISleepingScreen : SingletonComponent<UISleepingScreen>, IUIScreen
{
	// Token: 0x04003285 RID: 12933
	protected CanvasGroup canvasGroup;

	// Token: 0x04003286 RID: 12934
	private bool visible;

	// Token: 0x06003780 RID: 14208 RVA: 0x0014D982 File Offset: 0x0014BB82
	protected override void Awake()
	{
		base.Awake();
		this.canvasGroup = base.GetComponent<CanvasGroup>();
		this.visible = true;
	}

	// Token: 0x06003781 RID: 14209 RVA: 0x0014D9A0 File Offset: 0x0014BBA0
	public void SetVisible(bool b)
	{
		if (this.visible == b)
		{
			return;
		}
		this.visible = b;
		this.canvasGroup.alpha = (this.visible ? 1f : 0f);
		SingletonComponent<UISleepingScreen>.Instance.gameObject.SetChildComponentsEnabled(this.visible);
	}
}
