using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020001A7 RID: 423
public interface IAIPathNode
{
	// Token: 0x1700020E RID: 526
	// (get) Token: 0x060018A6 RID: 6310
	Vector3 Position { get; }

	// Token: 0x1700020F RID: 527
	// (get) Token: 0x060018A7 RID: 6311
	bool Straightaway { get; }

	// Token: 0x17000210 RID: 528
	// (get) Token: 0x060018A8 RID: 6312
	IEnumerable<IAIPathNode> Linked { get; }

	// Token: 0x060018A9 RID: 6313
	bool IsValid();

	// Token: 0x060018AA RID: 6314
	void AddLink(IAIPathNode link);
}
