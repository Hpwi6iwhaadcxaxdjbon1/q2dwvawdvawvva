using System;
using UnityEngine;

// Token: 0x0200055E RID: 1374
public class RealmedCollider : BasePrefab
{
	// Token: 0x04002267 RID: 8807
	public Collider ServerCollider;

	// Token: 0x04002268 RID: 8808
	public Collider ClientCollider;

	// Token: 0x06002A44 RID: 10820 RVA: 0x00101894 File Offset: 0x000FFA94
	public override void PreProcess(IPrefabProcessor process, GameObject rootObj, string name, bool serverside, bool clientside, bool bundling)
	{
		base.PreProcess(process, rootObj, name, serverside, clientside, bundling);
		if (this.ServerCollider != this.ClientCollider)
		{
			if (clientside)
			{
				if (this.ServerCollider)
				{
					process.RemoveComponent(this.ServerCollider);
					this.ServerCollider = null;
				}
			}
			else if (this.ClientCollider)
			{
				process.RemoveComponent(this.ClientCollider);
				this.ClientCollider = null;
			}
		}
		process.RemoveComponent(this);
	}
}
