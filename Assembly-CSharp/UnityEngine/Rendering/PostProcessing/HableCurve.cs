using System;

namespace UnityEngine.Rendering.PostProcessing
{
	// Token: 0x02000A96 RID: 2710
	public class HableCurve
	{
		// Token: 0x040039A0 RID: 14752
		private readonly HableCurve.Segment[] m_Segments = new HableCurve.Segment[3];

		// Token: 0x040039A1 RID: 14753
		public readonly HableCurve.Uniforms uniforms;

		// Token: 0x17000582 RID: 1410
		// (get) Token: 0x0600408F RID: 16527 RVA: 0x0017CCF2 File Offset: 0x0017AEF2
		// (set) Token: 0x06004090 RID: 16528 RVA: 0x0017CCFA File Offset: 0x0017AEFA
		public float whitePoint { get; private set; }

		// Token: 0x17000583 RID: 1411
		// (get) Token: 0x06004091 RID: 16529 RVA: 0x0017CD03 File Offset: 0x0017AF03
		// (set) Token: 0x06004092 RID: 16530 RVA: 0x0017CD0B File Offset: 0x0017AF0B
		public float inverseWhitePoint { get; private set; }

		// Token: 0x17000584 RID: 1412
		// (get) Token: 0x06004093 RID: 16531 RVA: 0x0017CD14 File Offset: 0x0017AF14
		// (set) Token: 0x06004094 RID: 16532 RVA: 0x0017CD1C File Offset: 0x0017AF1C
		internal float x0 { get; private set; }

		// Token: 0x17000585 RID: 1413
		// (get) Token: 0x06004095 RID: 16533 RVA: 0x0017CD25 File Offset: 0x0017AF25
		// (set) Token: 0x06004096 RID: 16534 RVA: 0x0017CD2D File Offset: 0x0017AF2D
		internal float x1 { get; private set; }

		// Token: 0x06004097 RID: 16535 RVA: 0x0017CD38 File Offset: 0x0017AF38
		public HableCurve()
		{
			for (int i = 0; i < 3; i++)
			{
				this.m_Segments[i] = new HableCurve.Segment();
			}
			this.uniforms = new HableCurve.Uniforms(this);
		}

		// Token: 0x06004098 RID: 16536 RVA: 0x0017CD7C File Offset: 0x0017AF7C
		public float Eval(float x)
		{
			float num = x * this.inverseWhitePoint;
			int num2 = (num < this.x0) ? 0 : ((num < this.x1) ? 1 : 2);
			return this.m_Segments[num2].Eval(num);
		}

		// Token: 0x06004099 RID: 16537 RVA: 0x0017CDBC File Offset: 0x0017AFBC
		public void Init(float toeStrength, float toeLength, float shoulderStrength, float shoulderLength, float shoulderAngle, float gamma)
		{
			HableCurve.DirectParams directParams = default(HableCurve.DirectParams);
			toeLength = Mathf.Pow(Mathf.Clamp01(toeLength), 2.2f);
			toeStrength = Mathf.Clamp01(toeStrength);
			shoulderAngle = Mathf.Clamp01(shoulderAngle);
			shoulderStrength = Mathf.Clamp(shoulderStrength, 1E-05f, 0.99999f);
			shoulderLength = Mathf.Max(0f, shoulderLength);
			gamma = Mathf.Max(1E-05f, gamma);
			float num = toeLength * 0.5f;
			float num2 = (1f - toeStrength) * num;
			float num3 = 1f - num2;
			float num4 = num + num3;
			float num5 = (1f - shoulderStrength) * num3;
			float x = num + num5;
			float y = num2 + num5;
			float num6 = RuntimeUtilities.Exp2(shoulderLength) - 1f;
			float w = num4 + num6;
			directParams.x0 = num;
			directParams.y0 = num2;
			directParams.x1 = x;
			directParams.y1 = y;
			directParams.W = w;
			directParams.gamma = gamma;
			directParams.overshootX = directParams.W * 2f * shoulderAngle * shoulderLength;
			directParams.overshootY = 0.5f * shoulderAngle * shoulderLength;
			this.InitSegments(directParams);
		}

		// Token: 0x0600409A RID: 16538 RVA: 0x0017CED0 File Offset: 0x0017B0D0
		private void InitSegments(HableCurve.DirectParams srcParams)
		{
			HableCurve.DirectParams directParams = srcParams;
			this.whitePoint = srcParams.W;
			this.inverseWhitePoint = 1f / srcParams.W;
			directParams.W = 1f;
			directParams.x0 /= srcParams.W;
			directParams.x1 /= srcParams.W;
			directParams.overshootX = srcParams.overshootX / srcParams.W;
			float num;
			float num2;
			this.AsSlopeIntercept(out num, out num2, directParams.x0, directParams.x1, directParams.y0, directParams.y1);
			float gamma = srcParams.gamma;
			HableCurve.Segment segment = this.m_Segments[1];
			segment.offsetX = -(num2 / num);
			segment.offsetY = 0f;
			segment.scaleX = 1f;
			segment.scaleY = 1f;
			segment.lnA = gamma * Mathf.Log(num);
			segment.B = gamma;
			float m = this.EvalDerivativeLinearGamma(num, num2, gamma, directParams.x0);
			float m2 = this.EvalDerivativeLinearGamma(num, num2, gamma, directParams.x1);
			directParams.y0 = Mathf.Max(1E-05f, Mathf.Pow(directParams.y0, directParams.gamma));
			directParams.y1 = Mathf.Max(1E-05f, Mathf.Pow(directParams.y1, directParams.gamma));
			directParams.overshootY = Mathf.Pow(1f + directParams.overshootY, directParams.gamma) - 1f;
			this.x0 = directParams.x0;
			this.x1 = directParams.x1;
			HableCurve.Segment segment2 = this.m_Segments[0];
			segment2.offsetX = 0f;
			segment2.offsetY = 0f;
			segment2.scaleX = 1f;
			segment2.scaleY = 1f;
			float lnA;
			float b;
			this.SolveAB(out lnA, out b, directParams.x0, directParams.y0, m);
			segment2.lnA = lnA;
			segment2.B = b;
			HableCurve.Segment segment3 = this.m_Segments[2];
			float x = 1f + directParams.overshootX - directParams.x1;
			float y = 1f + directParams.overshootY - directParams.y1;
			float lnA2;
			float b2;
			this.SolveAB(out lnA2, out b2, x, y, m2);
			segment3.offsetX = 1f + directParams.overshootX;
			segment3.offsetY = 1f + directParams.overshootY;
			segment3.scaleX = -1f;
			segment3.scaleY = -1f;
			segment3.lnA = lnA2;
			segment3.B = b2;
			float num3 = this.m_Segments[2].Eval(1f);
			float num4 = 1f / num3;
			this.m_Segments[0].offsetY *= num4;
			this.m_Segments[0].scaleY *= num4;
			this.m_Segments[1].offsetY *= num4;
			this.m_Segments[1].scaleY *= num4;
			this.m_Segments[2].offsetY *= num4;
			this.m_Segments[2].scaleY *= num4;
		}

		// Token: 0x0600409B RID: 16539 RVA: 0x0017D1E9 File Offset: 0x0017B3E9
		private void SolveAB(out float lnA, out float B, float x0, float y0, float m)
		{
			B = m * x0 / y0;
			lnA = Mathf.Log(y0) - B * Mathf.Log(x0);
		}

		// Token: 0x0600409C RID: 16540 RVA: 0x0017D208 File Offset: 0x0017B408
		private void AsSlopeIntercept(out float m, out float b, float x0, float x1, float y0, float y1)
		{
			float num = y1 - y0;
			float num2 = x1 - x0;
			if (num2 == 0f)
			{
				m = 1f;
			}
			else
			{
				m = num / num2;
			}
			b = y0 - x0 * m;
		}

		// Token: 0x0600409D RID: 16541 RVA: 0x0017D23F File Offset: 0x0017B43F
		private float EvalDerivativeLinearGamma(float m, float b, float g, float x)
		{
			return g * m * Mathf.Pow(m * x + b, g - 1f);
		}

		// Token: 0x02000F34 RID: 3892
		private class Segment
		{
			// Token: 0x04004EEC RID: 20204
			public float offsetX;

			// Token: 0x04004EED RID: 20205
			public float offsetY;

			// Token: 0x04004EEE RID: 20206
			public float scaleX;

			// Token: 0x04004EEF RID: 20207
			public float scaleY;

			// Token: 0x04004EF0 RID: 20208
			public float lnA;

			// Token: 0x04004EF1 RID: 20209
			public float B;

			// Token: 0x0600541B RID: 21531 RVA: 0x001B492C File Offset: 0x001B2B2C
			public float Eval(float x)
			{
				float num = (x - this.offsetX) * this.scaleX;
				float num2 = 0f;
				if (num > 0f)
				{
					num2 = Mathf.Exp(this.lnA + this.B * Mathf.Log(num));
				}
				return num2 * this.scaleY + this.offsetY;
			}
		}

		// Token: 0x02000F35 RID: 3893
		private struct DirectParams
		{
			// Token: 0x04004EF2 RID: 20210
			internal float x0;

			// Token: 0x04004EF3 RID: 20211
			internal float y0;

			// Token: 0x04004EF4 RID: 20212
			internal float x1;

			// Token: 0x04004EF5 RID: 20213
			internal float y1;

			// Token: 0x04004EF6 RID: 20214
			internal float W;

			// Token: 0x04004EF7 RID: 20215
			internal float overshootX;

			// Token: 0x04004EF8 RID: 20216
			internal float overshootY;

			// Token: 0x04004EF9 RID: 20217
			internal float gamma;
		}

		// Token: 0x02000F36 RID: 3894
		public class Uniforms
		{
			// Token: 0x04004EFA RID: 20218
			private HableCurve parent;

			// Token: 0x0600541D RID: 21533 RVA: 0x001B4980 File Offset: 0x001B2B80
			internal Uniforms(HableCurve parent)
			{
				this.parent = parent;
			}

			// Token: 0x17000725 RID: 1829
			// (get) Token: 0x0600541E RID: 21534 RVA: 0x001B498F File Offset: 0x001B2B8F
			public Vector4 curve
			{
				get
				{
					return new Vector4(this.parent.inverseWhitePoint, this.parent.x0, this.parent.x1, 0f);
				}
			}

			// Token: 0x17000726 RID: 1830
			// (get) Token: 0x0600541F RID: 21535 RVA: 0x001B49BC File Offset: 0x001B2BBC
			public Vector4 toeSegmentA
			{
				get
				{
					HableCurve.Segment segment = this.parent.m_Segments[0];
					return new Vector4(segment.offsetX, segment.offsetY, segment.scaleX, segment.scaleY);
				}
			}

			// Token: 0x17000727 RID: 1831
			// (get) Token: 0x06005420 RID: 21536 RVA: 0x001B49F4 File Offset: 0x001B2BF4
			public Vector4 toeSegmentB
			{
				get
				{
					HableCurve.Segment segment = this.parent.m_Segments[0];
					return new Vector4(segment.lnA, segment.B, 0f, 0f);
				}
			}

			// Token: 0x17000728 RID: 1832
			// (get) Token: 0x06005421 RID: 21537 RVA: 0x001B4A2C File Offset: 0x001B2C2C
			public Vector4 midSegmentA
			{
				get
				{
					HableCurve.Segment segment = this.parent.m_Segments[1];
					return new Vector4(segment.offsetX, segment.offsetY, segment.scaleX, segment.scaleY);
				}
			}

			// Token: 0x17000729 RID: 1833
			// (get) Token: 0x06005422 RID: 21538 RVA: 0x001B4A64 File Offset: 0x001B2C64
			public Vector4 midSegmentB
			{
				get
				{
					HableCurve.Segment segment = this.parent.m_Segments[1];
					return new Vector4(segment.lnA, segment.B, 0f, 0f);
				}
			}

			// Token: 0x1700072A RID: 1834
			// (get) Token: 0x06005423 RID: 21539 RVA: 0x001B4A9C File Offset: 0x001B2C9C
			public Vector4 shoSegmentA
			{
				get
				{
					HableCurve.Segment segment = this.parent.m_Segments[2];
					return new Vector4(segment.offsetX, segment.offsetY, segment.scaleX, segment.scaleY);
				}
			}

			// Token: 0x1700072B RID: 1835
			// (get) Token: 0x06005424 RID: 21540 RVA: 0x001B4AD4 File Offset: 0x001B2CD4
			public Vector4 shoSegmentB
			{
				get
				{
					HableCurve.Segment segment = this.parent.m_Segments[2];
					return new Vector4(segment.lnA, segment.B, 0f, 0f);
				}
			}
		}
	}
}
