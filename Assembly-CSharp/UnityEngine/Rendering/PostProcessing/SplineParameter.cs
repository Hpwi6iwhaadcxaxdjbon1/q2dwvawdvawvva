using System;

namespace UnityEngine.Rendering.PostProcessing
{
	// Token: 0x02000A84 RID: 2692
	[Serializable]
	public sealed class SplineParameter : ParameterOverride<Spline>
	{
		// Token: 0x0600401E RID: 16414 RVA: 0x0017ABF3 File Offset: 0x00178DF3
		protected internal override void OnEnable()
		{
			if (this.value != null)
			{
				this.value.Cache(int.MinValue);
			}
		}

		// Token: 0x0600401F RID: 16415 RVA: 0x0017AC0D File Offset: 0x00178E0D
		internal override void SetValue(ParameterOverride parameter)
		{
			base.SetValue(parameter);
			if (this.value != null)
			{
				this.value.Cache(Time.renderedFrameCount);
			}
		}

		// Token: 0x06004020 RID: 16416 RVA: 0x0017AC30 File Offset: 0x00178E30
		public override void Interp(Spline from, Spline to, float t)
		{
			if (from == null || to == null)
			{
				base.Interp(from, to, t);
				return;
			}
			int renderedFrameCount = Time.renderedFrameCount;
			from.Cache(renderedFrameCount);
			to.Cache(renderedFrameCount);
			for (int i = 0; i < 128; i++)
			{
				float num = from.cachedData[i];
				float num2 = to.cachedData[i];
				this.value.cachedData[i] = num + (num2 - num) * t;
			}
		}
	}
}
