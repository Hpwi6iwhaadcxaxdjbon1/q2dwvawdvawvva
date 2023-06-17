using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200089D RID: 2205
public class UIPaintableImage : MonoBehaviour
{
	// Token: 0x04003165 RID: 12645
	public RawImage image;

	// Token: 0x04003166 RID: 12646
	public int texSize = 64;

	// Token: 0x04003167 RID: 12647
	public Color clearColor = Color.clear;

	// Token: 0x04003168 RID: 12648
	public FilterMode filterMode = FilterMode.Bilinear;

	// Token: 0x04003169 RID: 12649
	public bool mipmaps;

	// Token: 0x1700045D RID: 1117
	// (get) Token: 0x060036EA RID: 14058 RVA: 0x000B8990 File Offset: 0x000B6B90
	public RectTransform rectTransform
	{
		get
		{
			return base.transform as RectTransform;
		}
	}

	// Token: 0x02000E9F RID: 3743
	public enum DrawMode
	{
		// Token: 0x04004C52 RID: 19538
		AlphaBlended,
		// Token: 0x04004C53 RID: 19539
		Additive,
		// Token: 0x04004C54 RID: 19540
		Lighten,
		// Token: 0x04004C55 RID: 19541
		Erase
	}
}
