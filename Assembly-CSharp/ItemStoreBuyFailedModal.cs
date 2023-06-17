using System;
using UnityEngine;

// Token: 0x02000871 RID: 2161
public class ItemStoreBuyFailedModal : MonoBehaviour
{
	// Token: 0x06003653 RID: 13907 RVA: 0x0014945F File Offset: 0x0014765F
	public void Show(ulong orderid)
	{
		base.gameObject.SetActive(true);
		base.GetComponent<CanvasGroup>().alpha = 0f;
		LeanTween.alphaCanvas(base.GetComponent<CanvasGroup>(), 1f, 0.1f);
	}

	// Token: 0x06003654 RID: 13908 RVA: 0x00149493 File Offset: 0x00147693
	public void Hide()
	{
		LeanTween.alphaCanvas(base.GetComponent<CanvasGroup>(), 0f, 0.2f).setOnComplete(delegate()
		{
			base.gameObject.SetActive(false);
		});
	}
}
