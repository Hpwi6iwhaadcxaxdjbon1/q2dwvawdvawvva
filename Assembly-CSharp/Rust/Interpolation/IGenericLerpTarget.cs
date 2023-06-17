using System;
using System.Collections.Generic;

namespace Rust.Interpolation
{
	// Token: 0x02000B31 RID: 2865
	public interface IGenericLerpTarget<T> : ILerpInfo where T : ISnapshot<T>, new()
	{
		// Token: 0x06004571 RID: 17777
		void SetFrom(T snapshot);

		// Token: 0x06004572 RID: 17778
		T GetCurrentState();

		// Token: 0x06004573 RID: 17779
		void DebugInterpolationState(Interpolator<T>.Segment segment, List<T> entries);
	}
}
