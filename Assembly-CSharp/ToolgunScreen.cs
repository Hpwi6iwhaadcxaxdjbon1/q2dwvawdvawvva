using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020001D3 RID: 467
public class ToolgunScreen : MonoBehaviour
{
	// Token: 0x04001211 RID: 4625
	public Text blockInfoText;

	// Token: 0x04001212 RID: 4626
	public Text noBlockText;

	// Token: 0x06001926 RID: 6438 RVA: 0x000B9114 File Offset: 0x000B7314
	public void SetScreenText(string newText)
	{
		bool flag = string.IsNullOrEmpty(newText);
		this.blockInfoText.gameObject.SetActive(!flag);
		this.noBlockText.gameObject.SetActive(flag);
		this.blockInfoText.text = newText;
	}
}
