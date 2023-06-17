using System;
using UnityEngine;

// Token: 0x020001AC RID: 428
public class RuntimeInterestNode : IAIPathInterestNode
{
	// Token: 0x17000215 RID: 533
	// (get) Token: 0x060018B9 RID: 6329 RVA: 0x000B7A86 File Offset: 0x000B5C86
	// (set) Token: 0x060018BA RID: 6330 RVA: 0x000B7A8E File Offset: 0x000B5C8E
	public Vector3 Position { get; set; }

	// Token: 0x17000216 RID: 534
	// (get) Token: 0x060018BB RID: 6331 RVA: 0x000B7A97 File Offset: 0x000B5C97
	// (set) Token: 0x060018BC RID: 6332 RVA: 0x000B7A9F File Offset: 0x000B5C9F
	public float NextVisitTime { get; set; }

	// Token: 0x060018BD RID: 6333 RVA: 0x000B7AA8 File Offset: 0x000B5CA8
	public RuntimeInterestNode(Vector3 position)
	{
		this.Position = position;
	}
}
