using System;
using Facepunch;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020007F8 RID: 2040
public class MonumentMarker : MonoBehaviour
{
	// Token: 0x04002DBF RID: 11711
	public Text text;

	// Token: 0x04002DC0 RID: 11712
	public Image imageBackground;

	// Token: 0x04002DC1 RID: 11713
	public Image image;

	// Token: 0x04002DC2 RID: 11714
	public Color dayColor;

	// Token: 0x04002DC3 RID: 11715
	public Color nightColor;

	// Token: 0x060035A1 RID: 13729 RVA: 0x00147BB0 File Offset: 0x00145DB0
	public void Setup(LandmarkInfo info)
	{
		this.text.text = (info.displayPhrase.IsValid() ? info.displayPhrase.translated : info.transform.root.name);
		if (info.mapIcon != null)
		{
			this.image.sprite = info.mapIcon;
			this.text.SetActive(false);
			this.imageBackground.SetActive(true);
		}
		else
		{
			this.text.SetActive(true);
			this.imageBackground.SetActive(false);
		}
		this.SetNightMode(false);
	}

	// Token: 0x060035A2 RID: 13730 RVA: 0x00147C4C File Offset: 0x00145E4C
	public void SetNightMode(bool nightMode)
	{
		Color color = nightMode ? this.nightColor : this.dayColor;
		Color color2 = nightMode ? this.dayColor : this.nightColor;
		if (this.text != null)
		{
			this.text.color = color;
		}
		if (this.image != null)
		{
			this.image.color = color;
		}
		if (this.imageBackground != null)
		{
			this.imageBackground.color = color2;
		}
	}
}
