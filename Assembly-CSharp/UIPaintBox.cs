using System;
using Painting;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000801 RID: 2049
public class UIPaintBox : MonoBehaviour
{
	// Token: 0x04002DF5 RID: 11765
	public UIPaintBox.OnBrushChanged onBrushChanged = new UIPaintBox.OnBrushChanged();

	// Token: 0x04002DF6 RID: 11766
	public Brush brush;

	// Token: 0x060035AC RID: 13740 RVA: 0x00147CE0 File Offset: 0x00145EE0
	public void UpdateBrushSize(int size)
	{
		this.brush.brushSize = Vector2.one * (float)size;
		this.brush.spacing = Mathf.Clamp((float)size * 0.1f, 1f, 3f);
		this.OnChanged();
	}

	// Token: 0x060035AD RID: 13741 RVA: 0x00147D2C File Offset: 0x00145F2C
	public void UpdateBrushTexture(Texture2D tex)
	{
		this.brush.texture = tex;
		this.OnChanged();
	}

	// Token: 0x060035AE RID: 13742 RVA: 0x00147D40 File Offset: 0x00145F40
	public void UpdateBrushColor(Color col)
	{
		this.brush.color.r = col.r;
		this.brush.color.g = col.g;
		this.brush.color.b = col.b;
		this.OnChanged();
	}

	// Token: 0x060035AF RID: 13743 RVA: 0x00147D95 File Offset: 0x00145F95
	public void UpdateBrushAlpha(float a)
	{
		this.brush.color.a = a;
		this.OnChanged();
	}

	// Token: 0x060035B0 RID: 13744 RVA: 0x00147DAE File Offset: 0x00145FAE
	public void UpdateBrushEraser(bool b)
	{
		this.brush.erase = b;
	}

	// Token: 0x060035B1 RID: 13745 RVA: 0x00147DBC File Offset: 0x00145FBC
	private void OnChanged()
	{
		this.onBrushChanged.Invoke(this.brush);
	}

	// Token: 0x02000E83 RID: 3715
	[Serializable]
	public class OnBrushChanged : UnityEvent<Brush>
	{
	}
}
