using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020001AE RID: 430
public class RuntimePathNode : IAIPathNode
{
	// Token: 0x0400116A RID: 4458
	private HashSet<IAIPathNode> linked = new HashSet<IAIPathNode>();

	// Token: 0x1700021A RID: 538
	// (get) Token: 0x060018C8 RID: 6344 RVA: 0x000B7C5E File Offset: 0x000B5E5E
	// (set) Token: 0x060018C9 RID: 6345 RVA: 0x000B7C66 File Offset: 0x000B5E66
	public Vector3 Position { get; set; }

	// Token: 0x1700021B RID: 539
	// (get) Token: 0x060018CA RID: 6346 RVA: 0x000B7C6F File Offset: 0x000B5E6F
	// (set) Token: 0x060018CB RID: 6347 RVA: 0x000B7C77 File Offset: 0x000B5E77
	public bool Straightaway { get; set; }

	// Token: 0x1700021C RID: 540
	// (get) Token: 0x060018CC RID: 6348 RVA: 0x000B7C80 File Offset: 0x000B5E80
	public IEnumerable<IAIPathNode> Linked
	{
		get
		{
			return this.linked;
		}
	}

	// Token: 0x060018CD RID: 6349 RVA: 0x000B7C88 File Offset: 0x000B5E88
	public RuntimePathNode(Vector3 position)
	{
		this.Position = position;
	}

	// Token: 0x060018CE RID: 6350 RVA: 0x0000441C File Offset: 0x0000261C
	public bool IsValid()
	{
		return true;
	}

	// Token: 0x060018CF RID: 6351 RVA: 0x000B7CA2 File Offset: 0x000B5EA2
	public void AddLink(IAIPathNode link)
	{
		this.linked.Add(link);
	}
}
