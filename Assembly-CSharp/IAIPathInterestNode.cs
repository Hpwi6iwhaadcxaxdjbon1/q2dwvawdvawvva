using System;
using UnityEngine;

// Token: 0x020001A9 RID: 425
public interface IAIPathInterestNode
{
	// Token: 0x17000211 RID: 529
	// (get) Token: 0x060018AD RID: 6317
	Vector3 Position { get; }

	// Token: 0x17000212 RID: 530
	// (get) Token: 0x060018AE RID: 6318
	// (set) Token: 0x060018AF RID: 6319
	float NextVisitTime { get; set; }
}
