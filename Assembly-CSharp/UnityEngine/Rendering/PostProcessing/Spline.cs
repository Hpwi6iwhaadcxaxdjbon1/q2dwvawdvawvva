using System;
using UnityEngine.Assertions;

namespace UnityEngine.Rendering.PostProcessing
{
	// Token: 0x02000A9E RID: 2718
	[Serializable]
	public sealed class Spline
	{
		// Token: 0x04003A3D RID: 14909
		public const int k_Precision = 128;

		// Token: 0x04003A3E RID: 14910
		public const float k_Step = 0.0078125f;

		// Token: 0x04003A3F RID: 14911
		public AnimationCurve curve;

		// Token: 0x04003A40 RID: 14912
		[SerializeField]
		private bool m_Loop;

		// Token: 0x04003A41 RID: 14913
		[SerializeField]
		private float m_ZeroValue;

		// Token: 0x04003A42 RID: 14914
		[SerializeField]
		private float m_Range;

		// Token: 0x04003A43 RID: 14915
		private AnimationCurve m_InternalLoopingCurve;

		// Token: 0x04003A44 RID: 14916
		private int frameCount = -1;

		// Token: 0x04003A45 RID: 14917
		public float[] cachedData;

		// Token: 0x060040ED RID: 16621 RVA: 0x0017ED50 File Offset: 0x0017CF50
		public Spline(AnimationCurve curve, float zeroValue, bool loop, Vector2 bounds)
		{
			Assert.IsNotNull<AnimationCurve>(curve);
			this.curve = curve;
			this.m_ZeroValue = zeroValue;
			this.m_Loop = loop;
			this.m_Range = bounds.magnitude;
			this.cachedData = new float[128];
		}

		// Token: 0x060040EE RID: 16622 RVA: 0x0017EDA4 File Offset: 0x0017CFA4
		public void Cache(int frame)
		{
			if (frame == this.frameCount)
			{
				return;
			}
			int length = this.curve.length;
			if (this.m_Loop && length > 1)
			{
				if (this.m_InternalLoopingCurve == null)
				{
					this.m_InternalLoopingCurve = new AnimationCurve();
				}
				Keyframe key = this.curve[length - 1];
				key.time -= this.m_Range;
				Keyframe key2 = this.curve[0];
				key2.time += this.m_Range;
				this.m_InternalLoopingCurve.keys = this.curve.keys;
				this.m_InternalLoopingCurve.AddKey(key);
				this.m_InternalLoopingCurve.AddKey(key2);
			}
			for (int i = 0; i < 128; i++)
			{
				this.cachedData[i] = this.Evaluate((float)i * 0.0078125f, length);
			}
			this.frameCount = Time.renderedFrameCount;
		}

		// Token: 0x060040EF RID: 16623 RVA: 0x0017EE92 File Offset: 0x0017D092
		public float Evaluate(float t, int length)
		{
			if (length == 0)
			{
				return this.m_ZeroValue;
			}
			if (!this.m_Loop || length == 1)
			{
				return this.curve.Evaluate(t);
			}
			return this.m_InternalLoopingCurve.Evaluate(t);
		}

		// Token: 0x060040F0 RID: 16624 RVA: 0x0017EEC3 File Offset: 0x0017D0C3
		public float Evaluate(float t)
		{
			return this.Evaluate(t, this.curve.length);
		}

		// Token: 0x060040F1 RID: 16625 RVA: 0x0017EED7 File Offset: 0x0017D0D7
		public override int GetHashCode()
		{
			return 17 * 23 + this.curve.GetHashCode();
		}
	}
}
