using System;

namespace Rust.Interpolation
{
	// Token: 0x02000B33 RID: 2867
	public interface ISnapshot<T>
	{
		// Token: 0x17000652 RID: 1618
		// (get) Token: 0x0600457F RID: 17791
		// (set) Token: 0x06004580 RID: 17792
		float Time { get; set; }

		// Token: 0x06004581 RID: 17793
		void MatchValuesTo(T entry);

		// Token: 0x06004582 RID: 17794
		void Lerp(T prev, T next, float delta);

		// Token: 0x06004583 RID: 17795
		T GetNew();
	}
}
