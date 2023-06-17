using System;
using UnityEngine;

// Token: 0x02000362 RID: 866
public class AIMemory
{
	// Token: 0x0400190C RID: 6412
	public AIMemoryBank<BaseEntity> Entity = new AIMemoryBank<BaseEntity>(MemoryBankType.Entity, 8);

	// Token: 0x0400190D RID: 6413
	public AIMemoryBank<Vector3> Position = new AIMemoryBank<Vector3>(MemoryBankType.Position, 8);

	// Token: 0x0400190E RID: 6414
	public AIMemoryBank<AIPoint> AIPoint = new AIMemoryBank<AIPoint>(MemoryBankType.AIPoint, 8);

	// Token: 0x06001F8A RID: 8074 RVA: 0x000D4F5C File Offset: 0x000D315C
	public void Clear()
	{
		this.Entity.Clear();
		this.Position.Clear();
		this.AIPoint.Clear();
	}
}
