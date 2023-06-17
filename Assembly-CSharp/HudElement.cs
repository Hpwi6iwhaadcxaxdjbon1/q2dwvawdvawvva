using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020007E8 RID: 2024
public class HudElement : MonoBehaviour
{
	// Token: 0x04002D4A RID: 11594
	public Text[] ValueText;

	// Token: 0x04002D4B RID: 11595
	public Image[] FilledImage;

	// Token: 0x04002D4C RID: 11596
	private float lastValue;

	// Token: 0x04002D4D RID: 11597
	private float lastMax;

	// Token: 0x06003579 RID: 13689 RVA: 0x00147088 File Offset: 0x00145288
	public void SetValue(float value, float max = 1f)
	{
		using (TimeWarning.New("HudElement.SetValue", 0))
		{
			value = (float)Mathf.CeilToInt(value);
			if (value != this.lastValue || max != this.lastMax)
			{
				this.lastValue = value;
				this.lastMax = max;
				float image = value / max;
				this.SetText(value.ToString("0"));
				this.SetImage(image);
			}
		}
	}

	// Token: 0x0600357A RID: 13690 RVA: 0x00147108 File Offset: 0x00145308
	private void SetText(string v)
	{
		for (int i = 0; i < this.ValueText.Length; i++)
		{
			this.ValueText[i].text = v;
		}
	}

	// Token: 0x0600357B RID: 13691 RVA: 0x00147138 File Offset: 0x00145338
	private void SetImage(float f)
	{
		for (int i = 0; i < this.FilledImage.Length; i++)
		{
			this.FilledImage[i].fillAmount = f;
		}
	}
}
