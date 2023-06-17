using System;
using UnityEngine;

namespace Rust.Interpolation
{
	// Token: 0x02000B35 RID: 2869
	public struct TransformSnapshot : ISnapshot<TransformSnapshot>
	{
		// Token: 0x04003E03 RID: 15875
		public Vector3 pos;

		// Token: 0x04003E04 RID: 15876
		public Quaternion rot;

		// Token: 0x17000653 RID: 1619
		// (get) Token: 0x06004589 RID: 17801 RVA: 0x00196898 File Offset: 0x00194A98
		// (set) Token: 0x0600458A RID: 17802 RVA: 0x001968A0 File Offset: 0x00194AA0
		public float Time { get; set; }

		// Token: 0x0600458B RID: 17803 RVA: 0x001968A9 File Offset: 0x00194AA9
		public TransformSnapshot(float time, Vector3 pos, Quaternion rot)
		{
			this.Time = time;
			this.pos = pos;
			this.rot = rot;
		}

		// Token: 0x0600458C RID: 17804 RVA: 0x001968C0 File Offset: 0x00194AC0
		public void MatchValuesTo(TransformSnapshot entry)
		{
			this.pos = entry.pos;
			this.rot = entry.rot;
		}

		// Token: 0x0600458D RID: 17805 RVA: 0x001968DA File Offset: 0x00194ADA
		public void Lerp(TransformSnapshot prev, TransformSnapshot next, float delta)
		{
			this.pos = Vector3.LerpUnclamped(prev.pos, next.pos, delta);
			this.rot = Quaternion.SlerpUnclamped(prev.rot, next.rot, delta);
		}

		// Token: 0x0600458E RID: 17806 RVA: 0x0019690C File Offset: 0x00194B0C
		public TransformSnapshot GetNew()
		{
			return default(TransformSnapshot);
		}
	}
}
