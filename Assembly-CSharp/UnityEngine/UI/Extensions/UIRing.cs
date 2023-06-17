using System;
using System.Collections.Generic;

namespace UnityEngine.UI.Extensions
{
	// Token: 0x02000A3A RID: 2618
	[AddComponentMenu("UI/Extensions/Primitives/UI Ring")]
	public class UIRing : UIPrimitiveBase
	{
		// Token: 0x040037F9 RID: 14329
		public float innerRadius = 16f;

		// Token: 0x040037FA RID: 14330
		public float outerRadius = 32f;

		// Token: 0x040037FB RID: 14331
		[Tooltip("The Arc Steps property defines the number of segments that the Arc will be divided into.")]
		[Range(0f, 1000f)]
		public int ArcSteps = 100;

		// Token: 0x040037FC RID: 14332
		private List<int> indices = new List<int>();

		// Token: 0x040037FD RID: 14333
		private List<UIVertex> vertices = new List<UIVertex>();

		// Token: 0x06003EBF RID: 16063 RVA: 0x001706C0 File Offset: 0x0016E8C0
		protected override void OnPopulateMesh(VertexHelper vh)
		{
			float num = this.innerRadius * 2f;
			float num2 = this.outerRadius * 2f;
			vh.Clear();
			this.indices.Clear();
			this.vertices.Clear();
			int item = 0;
			int num3 = 1;
			float num4 = 360f / (float)this.ArcSteps;
			float num5 = Mathf.Cos(0f);
			float num6 = Mathf.Sin(0f);
			UIVertex simpleVert = UIVertex.simpleVert;
			simpleVert.color = this.color;
			simpleVert.position = new Vector2(num2 * num5, num2 * num6);
			this.vertices.Add(simpleVert);
			Vector2 v = new Vector2(num * num5, num * num6);
			simpleVert.position = v;
			this.vertices.Add(simpleVert);
			for (int i = 1; i <= this.ArcSteps; i++)
			{
				float f = 0.017453292f * ((float)i * num4);
				num5 = Mathf.Cos(f);
				num6 = Mathf.Sin(f);
				simpleVert.color = this.color;
				simpleVert.position = new Vector2(num2 * num5, num2 * num6);
				this.vertices.Add(simpleVert);
				simpleVert.position = new Vector2(num * num5, num * num6);
				this.vertices.Add(simpleVert);
				int item2 = num3;
				this.indices.Add(item);
				this.indices.Add(num3 + 1);
				this.indices.Add(num3);
				num3++;
				item = num3;
				num3++;
				this.indices.Add(item);
				this.indices.Add(num3);
				this.indices.Add(item2);
			}
			vh.AddUIVertexStream(this.vertices, this.indices);
		}

		// Token: 0x06003EC0 RID: 16064 RVA: 0x00170895 File Offset: 0x0016EA95
		public void SetArcSteps(int steps)
		{
			this.ArcSteps = steps;
			this.SetVerticesDirty();
		}

		// Token: 0x06003EC1 RID: 16065 RVA: 0x0016C6F5 File Offset: 0x0016A8F5
		public void SetBaseColor(Color color)
		{
			this.color = color;
			this.SetVerticesDirty();
		}

		// Token: 0x06003EC2 RID: 16066 RVA: 0x001708A4 File Offset: 0x0016EAA4
		public void UpdateBaseAlpha(float value)
		{
			Color color = this.color;
			color.a = value;
			this.color = color;
			this.SetVerticesDirty();
		}
	}
}
