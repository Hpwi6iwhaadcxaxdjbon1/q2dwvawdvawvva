using System;
using System.Collections.Generic;

namespace UnityEngine.UI.Extensions
{
	// Token: 0x02000A2E RID: 2606
	[AddComponentMenu("UI/Extensions/Primitives/UI Circle")]
	public class UICircle : UIPrimitiveBase
	{
		// Token: 0x0400378D RID: 14221
		[Tooltip("The Arc Invert property will invert the construction of the Arc.")]
		public bool ArcInvert = true;

		// Token: 0x0400378E RID: 14222
		[Tooltip("The Arc property is a percentage of the entire circumference of the circle.")]
		[Range(0f, 1f)]
		public float Arc = 1f;

		// Token: 0x0400378F RID: 14223
		[Tooltip("The Arc Steps property defines the number of segments that the Arc will be divided into.")]
		[Range(0f, 1000f)]
		public int ArcSteps = 100;

		// Token: 0x04003790 RID: 14224
		[Tooltip("The Arc Rotation property permits adjusting the geometry orientation around the Z axis.")]
		[Range(0f, 360f)]
		public int ArcRotation;

		// Token: 0x04003791 RID: 14225
		[Tooltip("The Progress property allows the primitive to be used as a progression indicator.")]
		[Range(0f, 1f)]
		public float Progress;

		// Token: 0x04003792 RID: 14226
		private float _progress;

		// Token: 0x04003793 RID: 14227
		public Color ProgressColor = new Color(255f, 255f, 255f, 255f);

		// Token: 0x04003794 RID: 14228
		public bool Fill = true;

		// Token: 0x04003795 RID: 14229
		public float Thickness = 5f;

		// Token: 0x04003796 RID: 14230
		public int Padding;

		// Token: 0x04003797 RID: 14231
		private List<int> indices = new List<int>();

		// Token: 0x04003798 RID: 14232
		private List<UIVertex> vertices = new List<UIVertex>();

		// Token: 0x04003799 RID: 14233
		private Vector2 uvCenter = new Vector2(0.5f, 0.5f);

		// Token: 0x06003E22 RID: 15906 RVA: 0x0016C260 File Offset: 0x0016A460
		protected override void OnPopulateMesh(VertexHelper vh)
		{
			int num = this.ArcInvert ? -1 : 1;
			float num2 = ((base.rectTransform.rect.width < base.rectTransform.rect.height) ? base.rectTransform.rect.width : base.rectTransform.rect.height) - (float)this.Padding;
			float num3 = -base.rectTransform.pivot.x * num2;
			float num4 = -base.rectTransform.pivot.x * num2 + this.Thickness;
			vh.Clear();
			this.indices.Clear();
			this.vertices.Clear();
			int item = 0;
			int num5 = 1;
			float num6 = this.Arc * 360f / (float)this.ArcSteps;
			this._progress = (float)this.ArcSteps * this.Progress;
			float f = (float)num * 0.017453292f * (float)this.ArcRotation;
			float num7 = Mathf.Cos(f);
			float num8 = Mathf.Sin(f);
			UIVertex simpleVert = UIVertex.simpleVert;
			simpleVert.color = ((this._progress > 0f) ? this.ProgressColor : this.color);
			simpleVert.position = new Vector2(num3 * num7, num3 * num8);
			simpleVert.uv0 = new Vector2(simpleVert.position.x / num2 + 0.5f, simpleVert.position.y / num2 + 0.5f);
			this.vertices.Add(simpleVert);
			Vector2 zero = new Vector2(num4 * num7, num4 * num8);
			if (this.Fill)
			{
				zero = Vector2.zero;
			}
			simpleVert.position = zero;
			simpleVert.uv0 = (this.Fill ? this.uvCenter : new Vector2(simpleVert.position.x / num2 + 0.5f, simpleVert.position.y / num2 + 0.5f));
			this.vertices.Add(simpleVert);
			for (int i = 1; i <= this.ArcSteps; i++)
			{
				float f2 = (float)num * 0.017453292f * ((float)i * num6 + (float)this.ArcRotation);
				num7 = Mathf.Cos(f2);
				num8 = Mathf.Sin(f2);
				simpleVert.color = (((float)i > this._progress) ? this.color : this.ProgressColor);
				simpleVert.position = new Vector2(num3 * num7, num3 * num8);
				simpleVert.uv0 = new Vector2(simpleVert.position.x / num2 + 0.5f, simpleVert.position.y / num2 + 0.5f);
				this.vertices.Add(simpleVert);
				if (!this.Fill)
				{
					simpleVert.position = new Vector2(num4 * num7, num4 * num8);
					simpleVert.uv0 = new Vector2(simpleVert.position.x / num2 + 0.5f, simpleVert.position.y / num2 + 0.5f);
					this.vertices.Add(simpleVert);
					int item2 = num5;
					this.indices.Add(item);
					this.indices.Add(num5 + 1);
					this.indices.Add(num5);
					num5++;
					item = num5;
					num5++;
					this.indices.Add(item);
					this.indices.Add(num5);
					this.indices.Add(item2);
				}
				else
				{
					this.indices.Add(item);
					this.indices.Add(num5 + 1);
					if ((float)i > this._progress)
					{
						this.indices.Add(this.ArcSteps + 2);
					}
					else
					{
						this.indices.Add(1);
					}
					num5++;
					item = num5;
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

		// Token: 0x06003E23 RID: 15907 RVA: 0x0016C6AA File Offset: 0x0016A8AA
		public void SetProgress(float progress)
		{
			this.Progress = progress;
			this.SetVerticesDirty();
		}

		// Token: 0x06003E24 RID: 15908 RVA: 0x0016C6B9 File Offset: 0x0016A8B9
		public void SetArcSteps(int steps)
		{
			this.ArcSteps = steps;
			this.SetVerticesDirty();
		}

		// Token: 0x06003E25 RID: 15909 RVA: 0x0016C6C8 File Offset: 0x0016A8C8
		public void SetInvertArc(bool invert)
		{
			this.ArcInvert = invert;
			this.SetVerticesDirty();
		}

		// Token: 0x06003E26 RID: 15910 RVA: 0x0016C6D7 File Offset: 0x0016A8D7
		public void SetArcRotation(int rotation)
		{
			this.ArcRotation = rotation;
			this.SetVerticesDirty();
		}

		// Token: 0x06003E27 RID: 15911 RVA: 0x0016C6E6 File Offset: 0x0016A8E6
		public void SetFill(bool fill)
		{
			this.Fill = fill;
			this.SetVerticesDirty();
		}

		// Token: 0x06003E28 RID: 15912 RVA: 0x0016C6F5 File Offset: 0x0016A8F5
		public void SetBaseColor(Color color)
		{
			this.color = color;
			this.SetVerticesDirty();
		}

		// Token: 0x06003E29 RID: 15913 RVA: 0x0016C704 File Offset: 0x0016A904
		public void UpdateBaseAlpha(float value)
		{
			Color color = this.color;
			color.a = value;
			this.color = color;
			this.SetVerticesDirty();
		}

		// Token: 0x06003E2A RID: 15914 RVA: 0x0016C72D File Offset: 0x0016A92D
		public void SetProgressColor(Color color)
		{
			this.ProgressColor = color;
			this.SetVerticesDirty();
		}

		// Token: 0x06003E2B RID: 15915 RVA: 0x0016C73C File Offset: 0x0016A93C
		public void UpdateProgressAlpha(float value)
		{
			this.ProgressColor.a = value;
			this.SetVerticesDirty();
		}

		// Token: 0x06003E2C RID: 15916 RVA: 0x0016C750 File Offset: 0x0016A950
		public void SetPadding(int padding)
		{
			this.Padding = padding;
			this.SetVerticesDirty();
		}

		// Token: 0x06003E2D RID: 15917 RVA: 0x0016C75F File Offset: 0x0016A95F
		public void SetThickness(int thickness)
		{
			this.Thickness = (float)thickness;
			this.SetVerticesDirty();
		}
	}
}
