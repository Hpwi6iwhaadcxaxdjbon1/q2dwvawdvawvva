using System;

namespace UnityEngine.UI.Extensions
{
	// Token: 0x02000A36 RID: 2614
	[AddComponentMenu("UI/Extensions/Primitives/UI Polygon")]
	public class UIPolygon : UIPrimitiveBase
	{
		// Token: 0x040037E7 RID: 14311
		public bool fill = true;

		// Token: 0x040037E8 RID: 14312
		public float thickness = 5f;

		// Token: 0x040037E9 RID: 14313
		[Range(3f, 360f)]
		public int sides = 3;

		// Token: 0x040037EA RID: 14314
		[Range(0f, 360f)]
		public float rotation;

		// Token: 0x040037EB RID: 14315
		[Range(0f, 1f)]
		public float[] VerticesDistances = new float[3];

		// Token: 0x040037EC RID: 14316
		private float size;

		// Token: 0x06003E8E RID: 16014 RVA: 0x0016FB20 File Offset: 0x0016DD20
		public void DrawPolygon(int _sides)
		{
			this.sides = _sides;
			this.VerticesDistances = new float[_sides + 1];
			for (int i = 0; i < _sides; i++)
			{
				this.VerticesDistances[i] = 1f;
			}
			this.rotation = 0f;
			this.SetAllDirty();
		}

		// Token: 0x06003E8F RID: 16015 RVA: 0x0016FB6C File Offset: 0x0016DD6C
		public void DrawPolygon(int _sides, float[] _VerticesDistances)
		{
			this.sides = _sides;
			this.VerticesDistances = _VerticesDistances;
			this.rotation = 0f;
			this.SetAllDirty();
		}

		// Token: 0x06003E90 RID: 16016 RVA: 0x0016FB8D File Offset: 0x0016DD8D
		public void DrawPolygon(int _sides, float[] _VerticesDistances, float _rotation)
		{
			this.sides = _sides;
			this.VerticesDistances = _VerticesDistances;
			this.rotation = _rotation;
			this.SetAllDirty();
		}

		// Token: 0x06003E91 RID: 16017 RVA: 0x0016FBAC File Offset: 0x0016DDAC
		private void Update()
		{
			this.size = base.rectTransform.rect.width;
			if (base.rectTransform.rect.width > base.rectTransform.rect.height)
			{
				this.size = base.rectTransform.rect.height;
			}
			else
			{
				this.size = base.rectTransform.rect.width;
			}
			this.thickness = Mathf.Clamp(this.thickness, 0f, this.size / 2f);
		}

		// Token: 0x06003E92 RID: 16018 RVA: 0x0016FC54 File Offset: 0x0016DE54
		protected override void OnPopulateMesh(VertexHelper vh)
		{
			vh.Clear();
			Vector2 vector = Vector2.zero;
			Vector2 vector2 = Vector2.zero;
			Vector2 vector3 = new Vector2(0f, 0f);
			Vector2 vector4 = new Vector2(0f, 1f);
			Vector2 vector5 = new Vector2(1f, 1f);
			Vector2 vector6 = new Vector2(1f, 0f);
			float num = 360f / (float)this.sides;
			int num2 = this.sides + 1;
			if (this.VerticesDistances.Length != num2)
			{
				this.VerticesDistances = new float[num2];
				for (int i = 0; i < num2 - 1; i++)
				{
					this.VerticesDistances[i] = 1f;
				}
			}
			this.VerticesDistances[num2 - 1] = this.VerticesDistances[0];
			for (int j = 0; j < num2; j++)
			{
				float num3 = -base.rectTransform.pivot.x * this.size * this.VerticesDistances[j];
				float num4 = -base.rectTransform.pivot.x * this.size * this.VerticesDistances[j] + this.thickness;
				float f = 0.017453292f * ((float)j * num + this.rotation);
				float num5 = Mathf.Cos(f);
				float num6 = Mathf.Sin(f);
				vector3 = new Vector2(0f, 1f);
				vector4 = new Vector2(1f, 1f);
				vector5 = new Vector2(1f, 0f);
				vector6 = new Vector2(0f, 0f);
				Vector2 vector7 = vector;
				Vector2 vector8 = new Vector2(num3 * num5, num3 * num6);
				Vector2 zero;
				Vector2 vector9;
				if (this.fill)
				{
					zero = Vector2.zero;
					vector9 = Vector2.zero;
				}
				else
				{
					zero = new Vector2(num4 * num5, num4 * num6);
					vector9 = vector2;
				}
				vector = vector8;
				vector2 = zero;
				vh.AddUIVertexQuad(base.SetVbo(new Vector2[]
				{
					vector7,
					vector8,
					zero,
					vector9
				}, new Vector2[]
				{
					vector3,
					vector4,
					vector5,
					vector6
				}));
			}
		}
	}
}
