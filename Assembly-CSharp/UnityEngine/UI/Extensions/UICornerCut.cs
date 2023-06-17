using System;

namespace UnityEngine.UI.Extensions
{
	// Token: 0x02000A30 RID: 2608
	[AddComponentMenu("UI/Extensions/Primitives/Cut Corners")]
	public class UICornerCut : UIPrimitiveBase
	{
		// Token: 0x040037A1 RID: 14241
		public Vector2 cornerSize = new Vector2(16f, 16f);

		// Token: 0x040037A2 RID: 14242
		[Header("Corners to cut")]
		[SerializeField]
		private bool m_cutUL = true;

		// Token: 0x040037A3 RID: 14243
		[SerializeField]
		private bool m_cutUR;

		// Token: 0x040037A4 RID: 14244
		[SerializeField]
		private bool m_cutLL;

		// Token: 0x040037A5 RID: 14245
		[SerializeField]
		private bool m_cutLR;

		// Token: 0x040037A6 RID: 14246
		[Tooltip("Up-Down colors become Left-Right colors")]
		[SerializeField]
		private bool m_makeColumns;

		// Token: 0x040037A7 RID: 14247
		[Header("Color the cut bars differently")]
		[SerializeField]
		private bool m_useColorUp;

		// Token: 0x040037A8 RID: 14248
		[SerializeField]
		private Color32 m_colorUp;

		// Token: 0x040037A9 RID: 14249
		[SerializeField]
		private bool m_useColorDown;

		// Token: 0x040037AA RID: 14250
		[SerializeField]
		private Color32 m_colorDown;

		// Token: 0x17000529 RID: 1321
		// (get) Token: 0x06003E36 RID: 15926 RVA: 0x0016CCAC File Offset: 0x0016AEAC
		// (set) Token: 0x06003E37 RID: 15927 RVA: 0x0016CCB4 File Offset: 0x0016AEB4
		public bool CutUL
		{
			get
			{
				return this.m_cutUL;
			}
			set
			{
				this.m_cutUL = value;
				this.SetAllDirty();
			}
		}

		// Token: 0x1700052A RID: 1322
		// (get) Token: 0x06003E38 RID: 15928 RVA: 0x0016CCC3 File Offset: 0x0016AEC3
		// (set) Token: 0x06003E39 RID: 15929 RVA: 0x0016CCCB File Offset: 0x0016AECB
		public bool CutUR
		{
			get
			{
				return this.m_cutUR;
			}
			set
			{
				this.m_cutUR = value;
				this.SetAllDirty();
			}
		}

		// Token: 0x1700052B RID: 1323
		// (get) Token: 0x06003E3A RID: 15930 RVA: 0x0016CCDA File Offset: 0x0016AEDA
		// (set) Token: 0x06003E3B RID: 15931 RVA: 0x0016CCE2 File Offset: 0x0016AEE2
		public bool CutLL
		{
			get
			{
				return this.m_cutLL;
			}
			set
			{
				this.m_cutLL = value;
				this.SetAllDirty();
			}
		}

		// Token: 0x1700052C RID: 1324
		// (get) Token: 0x06003E3C RID: 15932 RVA: 0x0016CCF1 File Offset: 0x0016AEF1
		// (set) Token: 0x06003E3D RID: 15933 RVA: 0x0016CCF9 File Offset: 0x0016AEF9
		public bool CutLR
		{
			get
			{
				return this.m_cutLR;
			}
			set
			{
				this.m_cutLR = value;
				this.SetAllDirty();
			}
		}

		// Token: 0x1700052D RID: 1325
		// (get) Token: 0x06003E3E RID: 15934 RVA: 0x0016CD08 File Offset: 0x0016AF08
		// (set) Token: 0x06003E3F RID: 15935 RVA: 0x0016CD10 File Offset: 0x0016AF10
		public bool MakeColumns
		{
			get
			{
				return this.m_makeColumns;
			}
			set
			{
				this.m_makeColumns = value;
				this.SetAllDirty();
			}
		}

		// Token: 0x1700052E RID: 1326
		// (get) Token: 0x06003E40 RID: 15936 RVA: 0x0016CD1F File Offset: 0x0016AF1F
		// (set) Token: 0x06003E41 RID: 15937 RVA: 0x0016CD27 File Offset: 0x0016AF27
		public bool UseColorUp
		{
			get
			{
				return this.m_useColorUp;
			}
			set
			{
				this.m_useColorUp = value;
			}
		}

		// Token: 0x1700052F RID: 1327
		// (get) Token: 0x06003E42 RID: 15938 RVA: 0x0016CD30 File Offset: 0x0016AF30
		// (set) Token: 0x06003E43 RID: 15939 RVA: 0x0016CD38 File Offset: 0x0016AF38
		public Color32 ColorUp
		{
			get
			{
				return this.m_colorUp;
			}
			set
			{
				this.m_colorUp = value;
			}
		}

		// Token: 0x17000530 RID: 1328
		// (get) Token: 0x06003E44 RID: 15940 RVA: 0x0016CD41 File Offset: 0x0016AF41
		// (set) Token: 0x06003E45 RID: 15941 RVA: 0x0016CD49 File Offset: 0x0016AF49
		public bool UseColorDown
		{
			get
			{
				return this.m_useColorDown;
			}
			set
			{
				this.m_useColorDown = value;
			}
		}

		// Token: 0x17000531 RID: 1329
		// (get) Token: 0x06003E46 RID: 15942 RVA: 0x0016CD52 File Offset: 0x0016AF52
		// (set) Token: 0x06003E47 RID: 15943 RVA: 0x0016CD5A File Offset: 0x0016AF5A
		public Color32 ColorDown
		{
			get
			{
				return this.m_colorDown;
			}
			set
			{
				this.m_colorDown = value;
			}
		}

		// Token: 0x06003E48 RID: 15944 RVA: 0x0016CD64 File Offset: 0x0016AF64
		protected override void OnPopulateMesh(VertexHelper vh)
		{
			Rect rect = base.rectTransform.rect;
			Rect rect2 = rect;
			Color32 color = this.color;
			bool flag = this.m_cutUL | this.m_cutUR;
			bool flag2 = this.m_cutLL | this.m_cutLR;
			bool flag3 = this.m_cutLL | this.m_cutUL;
			bool flag4 = this.m_cutLR | this.m_cutUR;
			if ((flag || flag2) && this.cornerSize.sqrMagnitude > 0f)
			{
				vh.Clear();
				if (flag3)
				{
					rect2.xMin += this.cornerSize.x;
				}
				if (flag2)
				{
					rect2.yMin += this.cornerSize.y;
				}
				if (flag)
				{
					rect2.yMax -= this.cornerSize.y;
				}
				if (flag4)
				{
					rect2.xMax -= this.cornerSize.x;
				}
				if (this.m_makeColumns)
				{
					Vector2 vector = new Vector2(rect.xMin, this.m_cutUL ? rect2.yMax : rect.yMax);
					Vector2 vector2 = new Vector2(rect.xMax, this.m_cutUR ? rect2.yMax : rect.yMax);
					Vector2 vector3 = new Vector2(rect.xMin, this.m_cutLL ? rect2.yMin : rect.yMin);
					Vector2 vector4 = new Vector2(rect.xMax, this.m_cutLR ? rect2.yMin : rect.yMin);
					if (flag3)
					{
						UICornerCut.AddSquare(vector3, vector, new Vector2(rect2.xMin, rect.yMax), new Vector2(rect2.xMin, rect.yMin), rect, this.m_useColorUp ? this.m_colorUp : color, vh);
					}
					if (flag4)
					{
						UICornerCut.AddSquare(vector2, vector4, new Vector2(rect2.xMax, rect.yMin), new Vector2(rect2.xMax, rect.yMax), rect, this.m_useColorDown ? this.m_colorDown : color, vh);
					}
				}
				else
				{
					Vector2 vector = new Vector2(this.m_cutUL ? rect2.xMin : rect.xMin, rect.yMax);
					Vector2 vector2 = new Vector2(this.m_cutUR ? rect2.xMax : rect.xMax, rect.yMax);
					Vector2 vector3 = new Vector2(this.m_cutLL ? rect2.xMin : rect.xMin, rect.yMin);
					Vector2 vector4 = new Vector2(this.m_cutLR ? rect2.xMax : rect.xMax, rect.yMin);
					if (flag2)
					{
						UICornerCut.AddSquare(vector4, vector3, new Vector2(rect.xMin, rect2.yMin), new Vector2(rect.xMax, rect2.yMin), rect, this.m_useColorDown ? this.m_colorDown : color, vh);
					}
					if (flag)
					{
						UICornerCut.AddSquare(vector, vector2, new Vector2(rect.xMax, rect2.yMax), new Vector2(rect.xMin, rect2.yMax), rect, this.m_useColorUp ? this.m_colorUp : color, vh);
					}
				}
				if (this.m_makeColumns)
				{
					UICornerCut.AddSquare(new Rect(rect2.xMin, rect.yMin, rect2.width, rect.height), rect, color, vh);
					return;
				}
				UICornerCut.AddSquare(new Rect(rect.xMin, rect2.yMin, rect.width, rect2.height), rect, color, vh);
			}
		}

		// Token: 0x06003E49 RID: 15945 RVA: 0x0016D114 File Offset: 0x0016B314
		private static void AddSquare(Rect rect, Rect rectUV, Color32 color32, VertexHelper vh)
		{
			int num = UICornerCut.AddVert(rect.xMin, rect.yMin, rectUV, color32, vh);
			int idx = UICornerCut.AddVert(rect.xMin, rect.yMax, rectUV, color32, vh);
			int num2 = UICornerCut.AddVert(rect.xMax, rect.yMax, rectUV, color32, vh);
			int idx2 = UICornerCut.AddVert(rect.xMax, rect.yMin, rectUV, color32, vh);
			vh.AddTriangle(num, idx, num2);
			vh.AddTriangle(num2, idx2, num);
		}

		// Token: 0x06003E4A RID: 15946 RVA: 0x0016D190 File Offset: 0x0016B390
		private static void AddSquare(Vector2 a, Vector2 b, Vector2 c, Vector2 d, Rect rectUV, Color32 color32, VertexHelper vh)
		{
			int num = UICornerCut.AddVert(a.x, a.y, rectUV, color32, vh);
			int idx = UICornerCut.AddVert(b.x, b.y, rectUV, color32, vh);
			int num2 = UICornerCut.AddVert(c.x, c.y, rectUV, color32, vh);
			int idx2 = UICornerCut.AddVert(d.x, d.y, rectUV, color32, vh);
			vh.AddTriangle(num, idx, num2);
			vh.AddTriangle(num2, idx2, num);
		}

		// Token: 0x06003E4B RID: 15947 RVA: 0x0016D214 File Offset: 0x0016B414
		private static int AddVert(float x, float y, Rect area, Color32 color32, VertexHelper vh)
		{
			Vector2 uv = new Vector2(Mathf.InverseLerp(area.xMin, area.xMax, x), Mathf.InverseLerp(area.yMin, area.yMax, y));
			vh.AddVert(new Vector3(x, y), color32, uv);
			return vh.currentVertCount - 1;
		}
	}
}
