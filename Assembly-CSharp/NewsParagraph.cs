using System;
using System.Collections.Generic;
using Rust.UI;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

// Token: 0x0200087B RID: 2171
public class NewsParagraph : MonoBehaviour, IPointerClickHandler, IEventSystemHandler
{
	// Token: 0x040030DF RID: 12511
	public RustText Text;

	// Token: 0x040030E0 RID: 12512
	public List<string> Links;

	// Token: 0x06003678 RID: 13944 RVA: 0x001498E0 File Offset: 0x00147AE0
	public void OnPointerClick(PointerEventData eventData)
	{
		if (this.Text == null || this.Links == null || eventData.button != PointerEventData.InputButton.Left)
		{
			return;
		}
		int num = TMP_TextUtilities.FindIntersectingLink(this.Text, eventData.position, eventData.pressEventCamera);
		if (num < 0 || num >= this.Text.textInfo.linkCount)
		{
			return;
		}
		TMP_LinkInfo tmp_LinkInfo = this.Text.textInfo.linkInfo[num];
		int num2;
		if (!int.TryParse(tmp_LinkInfo.GetLinkID(), out num2) || num2 < 0 || num2 >= this.Links.Count)
		{
			return;
		}
		string text = this.Links[num2];
		if (text.StartsWith("http", StringComparison.InvariantCultureIgnoreCase))
		{
			Application.OpenURL(text);
		}
	}
}
