using System;
using UnityEngine;

namespace Rust.Interpolation
{
	// Token: 0x02000B32 RID: 2866
	public class GenericLerp<T> : IDisposable where T : ISnapshot<T>, new()
	{
		// Token: 0x04003DF6 RID: 15862
		private Interpolator<T> interpolator;

		// Token: 0x04003DF7 RID: 15863
		private IGenericLerpTarget<T> target;

		// Token: 0x04003DF8 RID: 15864
		private static T snapshotPrototype = Activator.CreateInstance<T>();

		// Token: 0x04003DF9 RID: 15865
		private static float TimeOffset = 0f;

		// Token: 0x04003DFA RID: 15866
		private float timeOffset0 = float.MaxValue;

		// Token: 0x04003DFB RID: 15867
		private float timeOffset1 = float.MaxValue;

		// Token: 0x04003DFC RID: 15868
		private float timeOffset2 = float.MaxValue;

		// Token: 0x04003DFD RID: 15869
		private float timeOffset3 = float.MaxValue;

		// Token: 0x04003DFE RID: 15870
		private int timeOffsetCount;

		// Token: 0x04003DFF RID: 15871
		private float extrapolatedTime;

		// Token: 0x17000650 RID: 1616
		// (get) Token: 0x06004574 RID: 17780 RVA: 0x001960C1 File Offset: 0x001942C1
		private int TimeOffsetInterval
		{
			get
			{
				return PositionLerp.TimeOffsetInterval;
			}
		}

		// Token: 0x17000651 RID: 1617
		// (get) Token: 0x06004575 RID: 17781 RVA: 0x001960C8 File Offset: 0x001942C8
		private float LerpTime
		{
			get
			{
				return PositionLerp.LerpTime;
			}
		}

		// Token: 0x06004576 RID: 17782 RVA: 0x001960D0 File Offset: 0x001942D0
		public GenericLerp(IGenericLerpTarget<T> target, int listCount)
		{
			this.target = target;
			this.interpolator = new Interpolator<T>(listCount);
		}

		// Token: 0x06004577 RID: 17783 RVA: 0x00196124 File Offset: 0x00194324
		public void Tick()
		{
			if (this.target == null)
			{
				return;
			}
			float extrapolationTime = this.target.GetExtrapolationTime();
			float interpolationDelay = this.target.GetInterpolationDelay();
			float interpolationSmoothing = this.target.GetInterpolationSmoothing();
			Interpolator<T>.Segment segment = this.interpolator.Query(this.LerpTime, interpolationDelay, extrapolationTime, interpolationSmoothing, ref GenericLerp<T>.snapshotPrototype);
			if (segment.next.Time >= this.interpolator.last.Time)
			{
				this.extrapolatedTime = Mathf.Min(this.extrapolatedTime + Time.deltaTime, extrapolationTime);
			}
			else
			{
				this.extrapolatedTime = Mathf.Max(this.extrapolatedTime - Time.deltaTime, 0f);
			}
			if (this.extrapolatedTime > 0f && extrapolationTime > 0f && interpolationSmoothing > 0f)
			{
				float delta = Time.deltaTime / (this.extrapolatedTime / extrapolationTime * interpolationSmoothing);
				segment.tick.Lerp(this.target.GetCurrentState(), segment.tick, delta);
			}
			this.target.SetFrom(segment.tick);
		}

		// Token: 0x06004578 RID: 17784 RVA: 0x00196240 File Offset: 0x00194440
		public void Snapshot(T snapshot)
		{
			float interpolationDelay = this.target.GetInterpolationDelay();
			float interpolationSmoothing = this.target.GetInterpolationSmoothing();
			float num = interpolationDelay + interpolationSmoothing + 1f;
			float num2 = this.LerpTime;
			this.timeOffset0 = Mathf.Min(this.timeOffset0, num2 - snapshot.Time);
			this.timeOffsetCount++;
			if (this.timeOffsetCount >= this.TimeOffsetInterval / 4)
			{
				this.timeOffset3 = this.timeOffset2;
				this.timeOffset2 = this.timeOffset1;
				this.timeOffset1 = this.timeOffset0;
				this.timeOffset0 = float.MaxValue;
				this.timeOffsetCount = 0;
			}
			GenericLerp<T>.TimeOffset = Mathx.Min(this.timeOffset0, this.timeOffset1, this.timeOffset2, this.timeOffset3);
			num2 = snapshot.Time + GenericLerp<T>.TimeOffset;
			snapshot.Time = num2;
			this.interpolator.Add(snapshot);
			this.interpolator.Cull(num2 - num);
		}

		// Token: 0x06004579 RID: 17785 RVA: 0x00196346 File Offset: 0x00194546
		public void SnapTo(T snapshot)
		{
			this.interpolator.Clear();
			this.Snapshot(snapshot);
			this.target.SetFrom(snapshot);
		}

		// Token: 0x0600457A RID: 17786 RVA: 0x00196366 File Offset: 0x00194566
		public void SnapToNow(T snapshot)
		{
			snapshot.Time = this.LerpTime;
			this.interpolator.last = snapshot;
			this.Wipe();
		}

		// Token: 0x0600457B RID: 17787 RVA: 0x00196390 File Offset: 0x00194590
		public void SnapToEnd()
		{
			float interpolationDelay = this.target.GetInterpolationDelay();
			Interpolator<T>.Segment segment = this.interpolator.Query(this.LerpTime, interpolationDelay, 0f, 0f, ref GenericLerp<T>.snapshotPrototype);
			this.target.SetFrom(segment.tick);
			this.Wipe();
		}

		// Token: 0x0600457C RID: 17788 RVA: 0x001963E4 File Offset: 0x001945E4
		public void Dispose()
		{
			this.target = null;
			this.interpolator.Clear();
			this.timeOffset0 = float.MaxValue;
			this.timeOffset1 = float.MaxValue;
			this.timeOffset2 = float.MaxValue;
			this.timeOffset3 = float.MaxValue;
			this.extrapolatedTime = 0f;
			this.timeOffsetCount = 0;
		}

		// Token: 0x0600457D RID: 17789 RVA: 0x00196441 File Offset: 0x00194641
		private void Wipe()
		{
			this.interpolator.Clear();
			this.timeOffsetCount = 0;
			this.timeOffset0 = float.MaxValue;
			this.timeOffset1 = float.MaxValue;
			this.timeOffset2 = float.MaxValue;
			this.timeOffset3 = float.MaxValue;
		}
	}
}
