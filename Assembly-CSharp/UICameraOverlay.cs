using System;
using Rust.UI;
using UnityEngine;

// Token: 0x020008C6 RID: 2246
public class UICameraOverlay : SingletonComponent<UICameraOverlay>
{
	// Token: 0x04003250 RID: 12880
	public static readonly Translate.Phrase FocusOffText = new Translate.Phrase("camera.infinite_focus", "Infinite Focus");

	// Token: 0x04003251 RID: 12881
	public static readonly Translate.Phrase FocusAutoText = new Translate.Phrase("camera.auto_focus", "Auto Focus");

	// Token: 0x04003252 RID: 12882
	public static readonly Translate.Phrase FocusManualText = new Translate.Phrase("camera.manual_focus", "Manual Focus");

	// Token: 0x04003253 RID: 12883
	public CanvasGroup CanvasGroup;

	// Token: 0x04003254 RID: 12884
	public RustText FocusModeLabel;

	// Token: 0x06003754 RID: 14164 RVA: 0x0014D191 File Offset: 0x0014B391
	public void Show()
	{
		this.CanvasGroup.alpha = 1f;
	}

	// Token: 0x06003755 RID: 14165 RVA: 0x0014D1A3 File Offset: 0x0014B3A3
	public void Hide()
	{
		this.CanvasGroup.alpha = 0f;
	}

	// Token: 0x06003756 RID: 14166 RVA: 0x0014D1B5 File Offset: 0x0014B3B5
	public void SetFocusMode(CameraFocusMode mode)
	{
		if (mode == CameraFocusMode.Auto)
		{
			this.FocusModeLabel.SetPhrase(UICameraOverlay.FocusAutoText);
			return;
		}
		if (mode != CameraFocusMode.Manual)
		{
			this.FocusModeLabel.SetPhrase(UICameraOverlay.FocusOffText);
			return;
		}
		this.FocusModeLabel.SetPhrase(UICameraOverlay.FocusManualText);
	}
}
