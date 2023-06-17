using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020008A2 RID: 2210
public class PieOption : MonoBehaviour
{
	// Token: 0x0400319E RID: 12702
	public PieShape background;

	// Token: 0x0400319F RID: 12703
	public Image imageIcon;

	// Token: 0x040031A0 RID: 12704
	public Image overlayIcon;

	// Token: 0x1700045F RID: 1119
	// (get) Token: 0x06003707 RID: 14087 RVA: 0x0014C34D File Offset: 0x0014A54D
	internal float midRadius
	{
		get
		{
			return (this.background.startRadius + this.background.endRadius) * 0.5f;
		}
	}

	// Token: 0x17000460 RID: 1120
	// (get) Token: 0x06003708 RID: 14088 RVA: 0x0014C36C File Offset: 0x0014A56C
	internal float sliceSize
	{
		get
		{
			return this.background.endRadius - this.background.startRadius;
		}
	}

	// Token: 0x06003709 RID: 14089 RVA: 0x0014C388 File Offset: 0x0014A588
	public void UpdateOption(float startSlice, float sliceSize, float border, string optionTitle, float outerSize, float innerSize, float imageSize, Sprite sprite, bool showOverlay)
	{
		if (this.background == null)
		{
			return;
		}
		float num = this.background.rectTransform.rect.height * 0.5f;
		float num2 = num * (innerSize + (outerSize - innerSize) * 0.5f);
		float num3 = num * (outerSize - innerSize);
		this.background.startRadius = startSlice;
		this.background.endRadius = startSlice + sliceSize;
		this.background.border = border;
		this.background.outerSize = outerSize;
		this.background.innerSize = innerSize;
		this.background.color = new Color(UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), 0f);
		float num4 = startSlice + sliceSize * 0.5f;
		float x = Mathf.Sin(num4 * 0.017453292f) * num2;
		float y = Mathf.Cos(num4 * 0.017453292f) * num2;
		this.imageIcon.rectTransform.localPosition = new Vector3(x, y);
		this.imageIcon.rectTransform.sizeDelta = new Vector2(num3 * imageSize, num3 * imageSize);
		this.imageIcon.sprite = sprite;
		this.overlayIcon.gameObject.SetActive(showOverlay);
		if (showOverlay)
		{
			this.overlayIcon.rectTransform.localPosition = this.imageIcon.rectTransform.localPosition;
			this.overlayIcon.rectTransform.sizeDelta = this.imageIcon.rectTransform.sizeDelta;
		}
	}
}
