using System;
using UnityEngine;

namespace Rust.Interpolation
{
	// Token: 0x02000B36 RID: 2870
	public struct FloatSnapshot : ISnapshot<FloatSnapshot>
	{
		// Token: 0x04003E06 RID: 15878
		public float value;

		// Token: 0x17000654 RID: 1620
		// (get) Token: 0x0600458F RID: 17807 RVA: 0x00196922 File Offset: 0x00194B22
		// (set) Token: 0x06004590 RID: 17808 RVA: 0x0019692A File Offset: 0x00194B2A
		public float Time { get; set; }

		// Token: 0x06004591 RID: 17809 RVA: 0x00196933 File Offset: 0x00194B33
		public FloatSnapshot(float time, float value)
		{
			this.Time = time;
			this.value = value;
		}

		// Token: 0x06004592 RID: 17810 RVA: 0x00196943 File Offset: 0x00194B43
		public void MatchValuesTo(FloatSnapshot entry)
		{
			this.value = entry.value;
		}

		// Token: 0x06004593 RID: 17811 RVA: 0x00196951 File Offset: 0x00194B51
		public void Lerp(FloatSnapshot prev, FloatSnapshot next, float delta)
		{
			this.value = Mathf.Lerp(prev.value, next.value, delta);
		}

		// Token: 0x06004594 RID: 17812 RVA: 0x0019696C File Offset: 0x00194B6C
		public FloatSnapshot GetNew()
		{
			return default(FloatSnapshot);
		}
	}
}
