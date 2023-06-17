using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200087E RID: 2174
public class ApplyTweakUIChanges : MonoBehaviour
{
	// Token: 0x040030ED RID: 12525
	public Button ApplyButton;

	// Token: 0x040030EE RID: 12526
	public TweakUIBase[] Options;

	// Token: 0x06003685 RID: 13957 RVA: 0x0014A1FA File Offset: 0x001483FA
	private void OnEnable()
	{
		this.SetClean();
	}

	// Token: 0x06003686 RID: 13958 RVA: 0x0014A204 File Offset: 0x00148404
	public void Apply()
	{
		if (this.Options == null)
		{
			return;
		}
		foreach (TweakUIBase tweakUIBase in this.Options)
		{
			if (!(tweakUIBase == null))
			{
				tweakUIBase.OnApplyClicked();
			}
		}
		this.SetClean();
	}

	// Token: 0x06003687 RID: 13959 RVA: 0x0014A248 File Offset: 0x00148448
	public void SetDirty()
	{
		if (this.ApplyButton != null)
		{
			this.ApplyButton.interactable = true;
		}
	}

	// Token: 0x06003688 RID: 13960 RVA: 0x0014A264 File Offset: 0x00148464
	public void SetClean()
	{
		if (this.ApplyButton != null)
		{
			this.ApplyButton.interactable = false;
		}
	}
}
