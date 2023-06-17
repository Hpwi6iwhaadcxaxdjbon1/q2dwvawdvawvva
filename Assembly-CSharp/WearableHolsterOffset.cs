using System;
using UnityEngine;

// Token: 0x020005A5 RID: 1445
public class WearableHolsterOffset : MonoBehaviour
{
	// Token: 0x04002389 RID: 9097
	public WearableHolsterOffset.offsetInfo[] Offsets;

	// Token: 0x02000D66 RID: 3430
	[Serializable]
	public class offsetInfo
	{
		// Token: 0x04004759 RID: 18265
		public HeldEntity.HolsterInfo.HolsterSlot type;

		// Token: 0x0400475A RID: 18266
		public Vector3 offset;

		// Token: 0x0400475B RID: 18267
		public Vector3 rotationOffset;

		// Token: 0x0400475C RID: 18268
		public int priority;
	}
}
