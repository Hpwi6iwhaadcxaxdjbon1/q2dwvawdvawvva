using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000850 RID: 2128
public class SquareBorder : MonoBehaviour
{
	// Token: 0x04002FBE RID: 12222
	public float Size;

	// Token: 0x04002FBF RID: 12223
	public Color Color;

	// Token: 0x04002FC0 RID: 12224
	public RectTransform Top;

	// Token: 0x04002FC1 RID: 12225
	public RectTransform Bottom;

	// Token: 0x04002FC2 RID: 12226
	public RectTransform Left;

	// Token: 0x04002FC3 RID: 12227
	public RectTransform Right;

	// Token: 0x04002FC4 RID: 12228
	public Image TopImage;

	// Token: 0x04002FC5 RID: 12229
	public Image BottomImage;

	// Token: 0x04002FC6 RID: 12230
	public Image LeftImage;

	// Token: 0x04002FC7 RID: 12231
	public Image RightImage;

	// Token: 0x04002FC8 RID: 12232
	private float _lastSize;

	// Token: 0x04002FC9 RID: 12233
	private Color _lastColor;

	// Token: 0x0600361E RID: 13854 RVA: 0x00148DE0 File Offset: 0x00146FE0
	private void Update()
	{
		if (this._lastSize != this.Size)
		{
			this.Top.offsetMin = new Vector2(0f, -this.Size);
			this.Bottom.offsetMax = new Vector2(0f, this.Size);
			this.Left.offsetMin = new Vector2(0f, this.Size);
			this.Left.offsetMax = new Vector2(this.Size, -this.Size);
			this.Right.offsetMin = new Vector2(-this.Size, this.Size);
			this.Right.offsetMax = new Vector2(0f, -this.Size);
			this._lastSize = this.Size;
		}
		if (this._lastColor != this.Color)
		{
			this.TopImage.color = this.Color;
			this.BottomImage.color = this.Color;
			this.LeftImage.color = this.Color;
			this.RightImage.color = this.Color;
			this._lastColor = this.Color;
		}
	}
}
