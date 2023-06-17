using System;
using UnityEngine;

// Token: 0x020007E6 RID: 2022
public abstract class Graph : MonoBehaviour
{
	// Token: 0x04002D35 RID: 11573
	public Material Material;

	// Token: 0x04002D36 RID: 11574
	public int Resolution = 128;

	// Token: 0x04002D37 RID: 11575
	public Vector2 ScreenFill = new Vector2(0f, 0f);

	// Token: 0x04002D38 RID: 11576
	public Vector2 ScreenOrigin = new Vector2(0f, 0f);

	// Token: 0x04002D39 RID: 11577
	public Vector2 Pivot = new Vector2(0f, 0f);

	// Token: 0x04002D3A RID: 11578
	public Rect Area = new Rect(0f, 0f, 128f, 32f);

	// Token: 0x04002D3B RID: 11579
	internal float CurrentValue;

	// Token: 0x04002D3C RID: 11580
	private int index;

	// Token: 0x04002D3D RID: 11581
	private float[] values;

	// Token: 0x04002D3E RID: 11582
	private float max;

	// Token: 0x06003571 RID: 13681
	protected abstract float GetValue();

	// Token: 0x06003572 RID: 13682
	protected abstract Color GetColor(float value);

	// Token: 0x06003573 RID: 13683 RVA: 0x00146D88 File Offset: 0x00144F88
	protected Vector3 GetVertex(float x, float y)
	{
		return new Vector3(x, y, 0f);
	}

	// Token: 0x06003574 RID: 13684 RVA: 0x00146D98 File Offset: 0x00144F98
	protected void Update()
	{
		if (this.values == null || this.values.Length != this.Resolution)
		{
			this.values = new float[this.Resolution];
		}
		this.max = 0f;
		for (int i = 0; i < this.values.Length - 1; i++)
		{
			this.max = Mathf.Max(this.max, this.values[i] = this.values[i + 1]);
		}
		this.max = Mathf.Max(this.max, this.CurrentValue = (this.values[this.values.Length - 1] = this.GetValue()));
	}

	// Token: 0x06003575 RID: 13685 RVA: 0x00146E48 File Offset: 0x00145048
	protected void OnGUI()
	{
		if (Event.current.type != EventType.Repaint)
		{
			return;
		}
		if (this.values == null || this.values.Length == 0)
		{
			return;
		}
		float num = Mathf.Max(this.Area.width, this.ScreenFill.x * (float)Screen.width);
		float num2 = Mathf.Max(this.Area.height, this.ScreenFill.y * (float)Screen.height);
		float num3 = this.Area.x - this.Pivot.x * num + this.ScreenOrigin.x * (float)Screen.width;
		float num4 = this.Area.y - this.Pivot.y * num2 + this.ScreenOrigin.y * (float)Screen.height;
		GL.PushMatrix();
		this.Material.SetPass(0);
		GL.LoadPixelMatrix();
		GL.Begin(7);
		for (int i = 0; i < this.values.Length; i++)
		{
			float num5 = this.values[i];
			float num6 = num / (float)this.values.Length;
			float num7 = num2 * num5 / this.max;
			float num8 = num3 + (float)i * num6;
			float num9 = num4;
			GL.Color(this.GetColor(num5));
			GL.Vertex(this.GetVertex(num8 + 0f, num9 + num7));
			GL.Vertex(this.GetVertex(num8 + num6, num9 + num7));
			GL.Vertex(this.GetVertex(num8 + num6, num9 + 0f));
			GL.Vertex(this.GetVertex(num8 + 0f, num9 + 0f));
		}
		GL.End();
		GL.PopMatrix();
	}
}
