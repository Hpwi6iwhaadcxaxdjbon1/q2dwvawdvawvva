using System;
using System.Collections.Generic;

namespace UnityEngine.UI.Extensions
{
	// Token: 0x02000A2F RID: 2607
	[AddComponentMenu("UI/Extensions/Primitives/UI Circle Simple")]
	public class UICircleSimple : UIPrimitiveBase
	{
		// Token: 0x0400379A RID: 14234
		[Tooltip("The Arc Steps property defines the number of segments that the Arc will be divided into.")]
		[Range(0f, 1000f)]
		public int ArcSteps = 100;

		// Token: 0x0400379B RID: 14235
		public bool Fill = true;

		// Token: 0x0400379C RID: 14236
		public float Thickness = 5f;

		// Token: 0x0400379D RID: 14237
		public bool ThicknessIsOutside;

		// Token: 0x0400379E RID: 14238
		private List<int> indices = new List<int>();

		// Token: 0x0400379F RID: 14239
		private List<UIVertex> vertices = new List<UIVertex>();

		// Token: 0x040037A0 RID: 14240
		private Vector2 uvCenter = new Vector2(0.5f, 0.5f);

		// Token: 0x06003E2F RID: 15919 RVA: 0x0016C7FC File Offset: 0x0016A9FC
		protected override void OnPopulateMesh(VertexHelper vh)
		{
			float num = (base.rectTransform.rect.width < base.rectTransform.rect.height) ? base.rectTransform.rect.width : base.rectTransform.rect.height;
			float num2 = this.ThicknessIsOutside ? (-base.rectTransform.pivot.x * num - this.Thickness) : (-base.rectTransform.pivot.x * num);
			float num3 = this.ThicknessIsOutside ? (-base.rectTransform.pivot.x * num) : (-base.rectTransform.pivot.x * num + this.Thickness);
			vh.Clear();
			this.indices.Clear();
			this.vertices.Clear();
			int item = 0;
			int num4 = 1;
			float num5 = 360f / (float)this.ArcSteps;
			float num6 = Mathf.Cos(0f);
			float num7 = Mathf.Sin(0f);
			UIVertex simpleVert = UIVertex.simpleVert;
			simpleVert.color = this.color;
			simpleVert.position = new Vector2(num2 * num6, num2 * num7);
			simpleVert.uv0 = new Vector2(simpleVert.position.x / num + 0.5f, simpleVert.position.y / num + 0.5f);
			this.vertices.Add(simpleVert);
			Vector2 zero = new Vector2(num3 * num6, num3 * num7);
			if (this.Fill)
			{
				zero = Vector2.zero;
			}
			simpleVert.position = zero;
			simpleVert.uv0 = (this.Fill ? this.uvCenter : new Vector2(simpleVert.position.x / num + 0.5f, simpleVert.position.y / num + 0.5f));
			this.vertices.Add(simpleVert);
			for (int i = 1; i <= this.ArcSteps; i++)
			{
				float f = 0.017453292f * ((float)i * num5);
				num6 = Mathf.Cos(f);
				num7 = Mathf.Sin(f);
				simpleVert.color = this.color;
				simpleVert.position = new Vector2(num2 * num6, num2 * num7);
				simpleVert.uv0 = new Vector2(simpleVert.position.x / num + 0.5f, simpleVert.position.y / num + 0.5f);
				this.vertices.Add(simpleVert);
				if (!this.Fill)
				{
					simpleVert.position = new Vector2(num3 * num6, num3 * num7);
					simpleVert.uv0 = new Vector2(simpleVert.position.x / num + 0.5f, simpleVert.position.y / num + 0.5f);
					this.vertices.Add(simpleVert);
					int item2 = num4;
					this.indices.Add(item);
					this.indices.Add(num4 + 1);
					this.indices.Add(num4);
					num4++;
					item = num4;
					num4++;
					this.indices.Add(item);
					this.indices.Add(num4);
					this.indices.Add(item2);
				}
				else
				{
					this.indices.Add(item);
					this.indices.Add(num4 + 1);
					this.indices.Add(1);
					num4++;
					item = num4;
				}
			}
			if (this.Fill)
			{
				simpleVert.position = zero;
				simpleVert.color = this.color;
				simpleVert.uv0 = this.uvCenter;
				this.vertices.Add(simpleVert);
			}
			vh.AddUIVertexStream(this.vertices, this.indices);
		}

		// Token: 0x06003E30 RID: 15920 RVA: 0x0016CBF7 File Offset: 0x0016ADF7
		public void SetArcSteps(int steps)
		{
			this.ArcSteps = steps;
			this.SetVerticesDirty();
		}

		// Token: 0x06003E31 RID: 15921 RVA: 0x0016CC06 File Offset: 0x0016AE06
		public void SetFill(bool fill)
		{
			this.Fill = fill;
			this.SetVerticesDirty();
		}

		// Token: 0x06003E32 RID: 15922 RVA: 0x0016C6F5 File Offset: 0x0016A8F5
		public void SetBaseColor(Color color)
		{
			this.color = color;
			this.SetVerticesDirty();
		}

		// Token: 0x06003E33 RID: 15923 RVA: 0x0016CC18 File Offset: 0x0016AE18
		public void UpdateBaseAlpha(float value)
		{
			Color color = this.color;
			color.a = value;
			this.color = color;
			this.SetVerticesDirty();
		}

		// Token: 0x06003E34 RID: 15924 RVA: 0x0016CC41 File Offset: 0x0016AE41
		public void SetThickness(int thickness)
		{
			this.Thickness = (float)thickness;
			this.SetVerticesDirty();
		}
	}
}
