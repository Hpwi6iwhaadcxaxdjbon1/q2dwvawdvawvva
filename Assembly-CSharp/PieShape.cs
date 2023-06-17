﻿using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020008A3 RID: 2211
[ExecuteInEditMode]
public class PieShape : Graphic
{
	// Token: 0x040031A1 RID: 12705
	[Range(0f, 1f)]
	public float outerSize = 1f;

	// Token: 0x040031A2 RID: 12706
	[Range(0f, 1f)]
	public float innerSize = 0.5f;

	// Token: 0x040031A3 RID: 12707
	public float startRadius = -45f;

	// Token: 0x040031A4 RID: 12708
	public float endRadius = 45f;

	// Token: 0x040031A5 RID: 12709
	public float border;

	// Token: 0x040031A6 RID: 12710
	public bool debugDrawing;

	// Token: 0x0600370B RID: 14091 RVA: 0x0014C51C File Offset: 0x0014A71C
	protected override void OnPopulateMesh(VertexHelper vbo)
	{
		vbo.Clear();
		UIVertex simpleVert = UIVertex.simpleVert;
		float num = this.startRadius;
		float num2 = this.endRadius;
		if (this.startRadius > this.endRadius)
		{
			num2 = this.endRadius + 360f;
		}
		float num3 = Mathf.Floor((num2 - num) / 6f);
		if (num3 <= 1f)
		{
			return;
		}
		float num4 = (num2 - num) / num3;
		float num5 = num + (num2 - num) * 0.5f;
		Color color = this.color;
		float num6 = base.rectTransform.rect.height * 0.5f;
		Vector2 b = new Vector2(Mathf.Sin(num5 * 0.017453292f), Mathf.Cos(num5 * 0.017453292f)) * this.border;
		int num7 = 0;
		for (float num8 = num; num8 < num2; num8 += num4)
		{
			if (this.debugDrawing)
			{
				if (color == Color.red)
				{
					color = Color.white;
				}
				else
				{
					color = Color.red;
				}
			}
			simpleVert.color = color;
			float num9 = Mathf.Sin(num8 * 0.017453292f);
			float num10 = Mathf.Cos(num8 * 0.017453292f);
			float num11 = num8 + num4;
			if (num11 > num2)
			{
				num11 = num2;
			}
			float num12 = Mathf.Sin(num11 * 0.017453292f);
			float num13 = Mathf.Cos(num11 * 0.017453292f);
			simpleVert.position = new Vector2(num9 * this.outerSize * num6, num10 * this.outerSize * num6) + b;
			vbo.AddVert(simpleVert);
			simpleVert.position = new Vector2(num12 * this.outerSize * num6, num13 * this.outerSize * num6) + b;
			vbo.AddVert(simpleVert);
			simpleVert.position = new Vector2(num12 * this.innerSize * num6, num13 * this.innerSize * num6) + b;
			vbo.AddVert(simpleVert);
			simpleVert.position = new Vector2(num9 * this.innerSize * num6, num10 * this.innerSize * num6) + b;
			vbo.AddVert(simpleVert);
			vbo.AddTriangle(num7, num7 + 1, num7 + 2);
			vbo.AddTriangle(num7 + 2, num7 + 3, num7);
			num7 += 4;
		}
	}
}
