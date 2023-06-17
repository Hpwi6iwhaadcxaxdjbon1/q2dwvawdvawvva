using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000808 RID: 2056
public class UIFishing : SingletonComponent<UIFishing>
{
	// Token: 0x04002E1C RID: 11804
	public Slider TensionLine;

	// Token: 0x04002E1D RID: 11805
	public Image FillImage;

	// Token: 0x04002E1E RID: 11806
	public Gradient FillGradient;

	// Token: 0x060035C1 RID: 13761 RVA: 0x001462B2 File Offset: 0x001444B2
	private void Start()
	{
		base.gameObject.SetActive(false);
	}
}
