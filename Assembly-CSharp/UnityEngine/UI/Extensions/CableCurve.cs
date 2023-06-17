using System;
using System.Collections.Generic;

namespace UnityEngine.UI.Extensions
{
	// Token: 0x02000A3C RID: 2620
	[Serializable]
	public class CableCurve
	{
		// Token: 0x04003803 RID: 14339
		[SerializeField]
		private Vector2 m_start;

		// Token: 0x04003804 RID: 14340
		[SerializeField]
		private Vector2 m_end;

		// Token: 0x04003805 RID: 14341
		[SerializeField]
		private float m_slack;

		// Token: 0x04003806 RID: 14342
		[SerializeField]
		private int m_steps;

		// Token: 0x04003807 RID: 14343
		[SerializeField]
		private bool m_regen;

		// Token: 0x04003808 RID: 14344
		private static Vector2[] emptyCurve = new Vector2[]
		{
			new Vector2(0f, 0f),
			new Vector2(0f, 0f)
		};

		// Token: 0x04003809 RID: 14345
		[SerializeField]
		private Vector2[] points;

		// Token: 0x17000556 RID: 1366
		// (get) Token: 0x06003ED1 RID: 16081 RVA: 0x00170F50 File Offset: 0x0016F150
		// (set) Token: 0x06003ED2 RID: 16082 RVA: 0x00170F58 File Offset: 0x0016F158
		public bool regenPoints
		{
			get
			{
				return this.m_regen;
			}
			set
			{
				this.m_regen = value;
			}
		}

		// Token: 0x17000557 RID: 1367
		// (get) Token: 0x06003ED3 RID: 16083 RVA: 0x00170F61 File Offset: 0x0016F161
		// (set) Token: 0x06003ED4 RID: 16084 RVA: 0x00170F69 File Offset: 0x0016F169
		public Vector2 start
		{
			get
			{
				return this.m_start;
			}
			set
			{
				if (value != this.m_start)
				{
					this.m_regen = true;
				}
				this.m_start = value;
			}
		}

		// Token: 0x17000558 RID: 1368
		// (get) Token: 0x06003ED5 RID: 16085 RVA: 0x00170F87 File Offset: 0x0016F187
		// (set) Token: 0x06003ED6 RID: 16086 RVA: 0x00170F8F File Offset: 0x0016F18F
		public Vector2 end
		{
			get
			{
				return this.m_end;
			}
			set
			{
				if (value != this.m_end)
				{
					this.m_regen = true;
				}
				this.m_end = value;
			}
		}

		// Token: 0x17000559 RID: 1369
		// (get) Token: 0x06003ED7 RID: 16087 RVA: 0x00170FAD File Offset: 0x0016F1AD
		// (set) Token: 0x06003ED8 RID: 16088 RVA: 0x00170FB5 File Offset: 0x0016F1B5
		public float slack
		{
			get
			{
				return this.m_slack;
			}
			set
			{
				if (value != this.m_slack)
				{
					this.m_regen = true;
				}
				this.m_slack = Mathf.Max(0f, value);
			}
		}

		// Token: 0x1700055A RID: 1370
		// (get) Token: 0x06003ED9 RID: 16089 RVA: 0x00170FD8 File Offset: 0x0016F1D8
		// (set) Token: 0x06003EDA RID: 16090 RVA: 0x00170FE0 File Offset: 0x0016F1E0
		public int steps
		{
			get
			{
				return this.m_steps;
			}
			set
			{
				if (value != this.m_steps)
				{
					this.m_regen = true;
				}
				this.m_steps = Mathf.Max(2, value);
			}
		}

		// Token: 0x1700055B RID: 1371
		// (get) Token: 0x06003EDB RID: 16091 RVA: 0x00171000 File Offset: 0x0016F200
		public Vector2 midPoint
		{
			get
			{
				Vector2 result = Vector2.zero;
				if (this.m_steps == 2)
				{
					return (this.points[0] + this.points[1]) * 0.5f;
				}
				if (this.m_steps > 2)
				{
					int num = this.m_steps / 2;
					if (this.m_steps % 2 == 0)
					{
						result = (this.points[num] + this.points[num + 1]) * 0.5f;
					}
					else
					{
						result = this.points[num];
					}
				}
				return result;
			}
		}

		// Token: 0x06003EDC RID: 16092 RVA: 0x0017109C File Offset: 0x0016F29C
		public CableCurve()
		{
			this.points = CableCurve.emptyCurve;
			this.m_start = Vector2.up;
			this.m_end = Vector2.up + Vector2.right;
			this.m_slack = 0.5f;
			this.m_steps = 20;
			this.m_regen = true;
		}

		// Token: 0x06003EDD RID: 16093 RVA: 0x001710F4 File Offset: 0x0016F2F4
		public CableCurve(Vector2[] inputPoints)
		{
			this.points = inputPoints;
			this.m_start = inputPoints[0];
			this.m_end = inputPoints[1];
			this.m_slack = 0.5f;
			this.m_steps = 20;
			this.m_regen = true;
		}

		// Token: 0x06003EDE RID: 16094 RVA: 0x00171144 File Offset: 0x0016F344
		public CableCurve(List<Vector2> inputPoints)
		{
			this.points = inputPoints.ToArray();
			this.m_start = inputPoints[0];
			this.m_end = inputPoints[1];
			this.m_slack = 0.5f;
			this.m_steps = 20;
			this.m_regen = true;
		}

		// Token: 0x06003EDF RID: 16095 RVA: 0x00171198 File Offset: 0x0016F398
		public CableCurve(CableCurve v)
		{
			this.points = v.Points();
			this.m_start = v.start;
			this.m_end = v.end;
			this.m_slack = v.slack;
			this.m_steps = v.steps;
			this.m_regen = v.regenPoints;
		}

		// Token: 0x06003EE0 RID: 16096 RVA: 0x001711F4 File Offset: 0x0016F3F4
		public Vector2[] Points()
		{
			if (!this.m_regen)
			{
				return this.points;
			}
			if (this.m_steps < 2)
			{
				return CableCurve.emptyCurve;
			}
			float num = Vector2.Distance(this.m_end, this.m_start);
			float num2 = Vector2.Distance(new Vector2(this.m_end.x, this.m_start.y), this.m_start);
			float num3 = num + Mathf.Max(0.0001f, this.m_slack);
			float num4 = 0f;
			float y = this.m_start.y;
			float num5 = num2;
			float y2 = this.end.y;
			if (num5 - num4 == 0f)
			{
				return CableCurve.emptyCurve;
			}
			float num6 = Mathf.Sqrt(Mathf.Pow(num3, 2f) - Mathf.Pow(y2 - y, 2f)) / (num5 - num4);
			int num7 = 30;
			int num8 = 0;
			int num9 = num7 * 10;
			bool flag = false;
			float num10 = 0f;
			float num11 = 100f;
			for (int i = 0; i < num7; i++)
			{
				for (int j = 0; j < 10; j++)
				{
					num8++;
					float num12 = num10 + num11;
					float num13 = (float)Math.Sinh((double)num12) / num12;
					if (!float.IsInfinity(num13))
					{
						if (num13 == num6)
						{
							flag = true;
							num10 = num12;
							break;
						}
						if (num13 > num6)
						{
							break;
						}
						num10 = num12;
						if (num8 > num9)
						{
							flag = true;
							break;
						}
					}
				}
				if (flag)
				{
					break;
				}
				num11 *= 0.1f;
			}
			float num14 = (num5 - num4) / 2f / num10;
			float num15 = (num4 + num5 - num14 * Mathf.Log((num3 + y2 - y) / (num3 - y2 + y))) / 2f;
			float num16 = (y2 + y - num3 * (float)Math.Cosh((double)num10) / (float)Math.Sinh((double)num10)) / 2f;
			this.points = new Vector2[this.m_steps];
			float num17 = (float)(this.m_steps - 1);
			for (int k = 0; k < this.m_steps; k++)
			{
				float num18 = (float)k / num17;
				Vector2 zero = Vector2.zero;
				zero.x = Mathf.Lerp(this.start.x, this.end.x, num18);
				zero.y = num14 * (float)Math.Cosh((double)((num18 * num2 - num15) / num14)) + num16;
				this.points[k] = zero;
			}
			this.m_regen = false;
			return this.points;
		}
	}
}
