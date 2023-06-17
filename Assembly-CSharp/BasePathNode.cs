using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020001A3 RID: 419
public class BasePathNode : MonoBehaviour, IAIPathNode
{
	// Token: 0x04001144 RID: 4420
	public BasePath Path;

	// Token: 0x04001145 RID: 4421
	public List<BasePathNode> linked;

	// Token: 0x04001146 RID: 4422
	public float maxVelocityOnApproach = -1f;

	// Token: 0x04001147 RID: 4423
	public bool straightaway;

	// Token: 0x17000209 RID: 521
	// (get) Token: 0x06001887 RID: 6279 RVA: 0x0002C673 File Offset: 0x0002A873
	public Vector3 Position
	{
		get
		{
			return base.transform.position;
		}
	}

	// Token: 0x1700020A RID: 522
	// (get) Token: 0x06001888 RID: 6280 RVA: 0x000B73FB File Offset: 0x000B55FB
	public bool Straightaway
	{
		get
		{
			return this.straightaway;
		}
	}

	// Token: 0x1700020B RID: 523
	// (get) Token: 0x06001889 RID: 6281 RVA: 0x000B7403 File Offset: 0x000B5603
	public IEnumerable<IAIPathNode> Linked
	{
		get
		{
			return this.linked;
		}
	}

	// Token: 0x0600188A RID: 6282 RVA: 0x000B740B File Offset: 0x000B560B
	public bool IsValid()
	{
		return base.transform != null;
	}

	// Token: 0x0600188B RID: 6283 RVA: 0x0002A2F3 File Offset: 0x000284F3
	public void AddLink(IAIPathNode link)
	{
		throw new NotImplementedException();
	}

	// Token: 0x0600188C RID: 6284 RVA: 0x000063A5 File Offset: 0x000045A5
	public void OnDrawGizmosSelected()
	{
	}
}
