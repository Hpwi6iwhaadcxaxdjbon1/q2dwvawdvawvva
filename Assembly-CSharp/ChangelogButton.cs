using System;
using Rust.UI;
using UnityEngine;

// Token: 0x02000819 RID: 2073
public class ChangelogButton : MonoBehaviour
{
	// Token: 0x04002E86 RID: 11910
	public RustButton Button;

	// Token: 0x04002E87 RID: 11911
	public CanvasGroup CanvasGroup;

	// Token: 0x060035D7 RID: 13783 RVA: 0x00148340 File Offset: 0x00146540
	private void Update()
	{
		BaseGameMode activeGameMode = BaseGameMode.GetActiveGameMode(false);
		if (activeGameMode != null)
		{
			if (this.CanvasGroup.alpha != 1f)
			{
				this.CanvasGroup.alpha = 1f;
				this.CanvasGroup.blocksRaycasts = true;
				this.Button.Text.SetPhrase(new Translate.Phrase(activeGameMode.shortname, activeGameMode.shortname));
				return;
			}
		}
		else if (this.CanvasGroup.alpha != 0f)
		{
			this.CanvasGroup.alpha = 0f;
			this.CanvasGroup.blocksRaycasts = false;
		}
	}
}
