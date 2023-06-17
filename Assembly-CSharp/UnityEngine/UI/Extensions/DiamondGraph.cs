using System;

namespace UnityEngine.UI.Extensions
{
	// Token: 0x02000A2D RID: 2605
	[AddComponentMenu("UI/Extensions/Primitives/Diamond Graph")]
	public class DiamondGraph : UIPrimitiveBase
	{
		// Token: 0x04003789 RID: 14217
		[SerializeField]
		private float m_a = 1f;

		// Token: 0x0400378A RID: 14218
		[SerializeField]
		private float m_b = 1f;

		// Token: 0x0400378B RID: 14219
		[SerializeField]
		private float m_c = 1f;

		// Token: 0x0400378C RID: 14220
		[SerializeField]
		private float m_d = 1f;

		// Token: 0x17000525 RID: 1317
		// (get) Token: 0x06003E18 RID: 15896 RVA: 0x0016C078 File Offset: 0x0016A278
		// (set) Token: 0x06003E19 RID: 15897 RVA: 0x0016C080 File Offset: 0x0016A280
		public float A
		{
			get
			{
				return this.m_a;
			}
			set
			{
				this.m_a = value;
			}
		}

		// Token: 0x17000526 RID: 1318
		// (get) Token: 0x06003E1A RID: 15898 RVA: 0x0016C089 File Offset: 0x0016A289
		// (set) Token: 0x06003E1B RID: 15899 RVA: 0x0016C091 File Offset: 0x0016A291
		public float B
		{
			get
			{
				return this.m_b;
			}
			set
			{
				this.m_b = value;
			}
		}

		// Token: 0x17000527 RID: 1319
		// (get) Token: 0x06003E1C RID: 15900 RVA: 0x0016C09A File Offset: 0x0016A29A
		// (set) Token: 0x06003E1D RID: 15901 RVA: 0x0016C0A2 File Offset: 0x0016A2A2
		public float C
		{
			get
			{
				return this.m_c;
			}
			set
			{
				this.m_c = value;
			}
		}

		// Token: 0x17000528 RID: 1320
		// (get) Token: 0x06003E1E RID: 15902 RVA: 0x0016C0AB File Offset: 0x0016A2AB
		// (set) Token: 0x06003E1F RID: 15903 RVA: 0x0016C0B3 File Offset: 0x0016A2B3
		public float D
		{
			get
			{
				return this.m_d;
			}
			set
			{
				this.m_d = value;
			}
		}

		// Token: 0x06003E20 RID: 15904 RVA: 0x0016C0BC File Offset: 0x0016A2BC
		protected override void OnPopulateMesh(VertexHelper vh)
		{
			vh.Clear();
			float num = base.rectTransform.rect.width / 2f;
			this.m_a = Math.Min(1f, Math.Max(0f, this.m_a));
			this.m_b = Math.Min(1f, Math.Max(0f, this.m_b));
			this.m_c = Math.Min(1f, Math.Max(0f, this.m_c));
			this.m_d = Math.Min(1f, Math.Max(0f, this.m_d));
			Color32 color = this.color;
			vh.AddVert(new Vector3(-num * this.m_a, 0f), color, new Vector2(0f, 0f));
			vh.AddVert(new Vector3(0f, num * this.m_b), color, new Vector2(0f, 1f));
			vh.AddVert(new Vector3(num * this.m_c, 0f), color, new Vector2(1f, 1f));
			vh.AddVert(new Vector3(0f, -num * this.m_d), color, new Vector2(1f, 0f));
			vh.AddTriangle(0, 1, 2);
			vh.AddTriangle(2, 3, 0);
		}
	}
}
