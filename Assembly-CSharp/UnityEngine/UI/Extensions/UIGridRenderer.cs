using System;

namespace UnityEngine.UI.Extensions
{
	// Token: 0x02000A32 RID: 2610
	[AddComponentMenu("UI/Extensions/Primitives/UIGridRenderer")]
	public class UIGridRenderer : UILineRenderer
	{
		// Token: 0x040037B0 RID: 14256
		[SerializeField]
		private int m_GridColumns = 10;

		// Token: 0x040037B1 RID: 14257
		[SerializeField]
		private int m_GridRows = 10;

		// Token: 0x17000532 RID: 1330
		// (get) Token: 0x06003E52 RID: 15954 RVA: 0x0016D836 File Offset: 0x0016BA36
		// (set) Token: 0x06003E53 RID: 15955 RVA: 0x0016D83E File Offset: 0x0016BA3E
		public int GridColumns
		{
			get
			{
				return this.m_GridColumns;
			}
			set
			{
				if (this.m_GridColumns == value)
				{
					return;
				}
				this.m_GridColumns = value;
				this.SetAllDirty();
			}
		}

		// Token: 0x17000533 RID: 1331
		// (get) Token: 0x06003E54 RID: 15956 RVA: 0x0016D857 File Offset: 0x0016BA57
		// (set) Token: 0x06003E55 RID: 15957 RVA: 0x0016D85F File Offset: 0x0016BA5F
		public int GridRows
		{
			get
			{
				return this.m_GridRows;
			}
			set
			{
				if (this.m_GridRows == value)
				{
					return;
				}
				this.m_GridRows = value;
				this.SetAllDirty();
			}
		}

		// Token: 0x06003E56 RID: 15958 RVA: 0x0016D878 File Offset: 0x0016BA78
		protected override void OnPopulateMesh(VertexHelper vh)
		{
			this.relativeSize = true;
			int num = this.GridRows * 3 + 1;
			if (this.GridRows % 2 == 0)
			{
				num++;
			}
			num += this.GridColumns * 3 + 1;
			this.m_points = new Vector2[num];
			int num2 = 0;
			for (int i = 0; i < this.GridRows; i++)
			{
				float x = 1f;
				float x2 = 0f;
				if (i % 2 == 0)
				{
					x = 0f;
					x2 = 1f;
				}
				float y = (float)i / (float)this.GridRows;
				this.m_points[num2].x = x;
				this.m_points[num2].y = y;
				num2++;
				this.m_points[num2].x = x2;
				this.m_points[num2].y = y;
				num2++;
				this.m_points[num2].x = x2;
				this.m_points[num2].y = (float)(i + 1) / (float)this.GridRows;
				num2++;
			}
			if (this.GridRows % 2 == 0)
			{
				this.m_points[num2].x = 1f;
				this.m_points[num2].y = 1f;
				num2++;
			}
			this.m_points[num2].x = 0f;
			this.m_points[num2].y = 1f;
			num2++;
			for (int j = 0; j < this.GridColumns; j++)
			{
				float y2 = 1f;
				float y3 = 0f;
				if (j % 2 == 0)
				{
					y2 = 0f;
					y3 = 1f;
				}
				float x3 = (float)j / (float)this.GridColumns;
				this.m_points[num2].x = x3;
				this.m_points[num2].y = y2;
				num2++;
				this.m_points[num2].x = x3;
				this.m_points[num2].y = y3;
				num2++;
				this.m_points[num2].x = (float)(j + 1) / (float)this.GridColumns;
				this.m_points[num2].y = y3;
				num2++;
			}
			if (this.GridColumns % 2 == 0)
			{
				this.m_points[num2].x = 1f;
				this.m_points[num2].y = 1f;
			}
			else
			{
				this.m_points[num2].x = 1f;
				this.m_points[num2].y = 0f;
			}
			base.OnPopulateMesh(vh);
		}
	}
}
