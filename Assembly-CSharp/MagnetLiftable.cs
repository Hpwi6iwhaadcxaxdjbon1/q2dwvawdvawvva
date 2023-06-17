using System;
using UnityEngine;

// Token: 0x02000490 RID: 1168
public class MagnetLiftable : EntityComponent<BaseEntity>
{
	// Token: 0x04001EB7 RID: 7863
	public ItemAmount[] shredResources;

	// Token: 0x04001EB8 RID: 7864
	public Vector3 shredDirection = Vector3.forward;

	// Token: 0x17000322 RID: 802
	// (get) Token: 0x06002651 RID: 9809 RVA: 0x000F13CB File Offset: 0x000EF5CB
	// (set) Token: 0x06002652 RID: 9810 RVA: 0x000F13D3 File Offset: 0x000EF5D3
	public BasePlayer associatedPlayer { get; private set; }

	// Token: 0x06002653 RID: 9811 RVA: 0x000F13DC File Offset: 0x000EF5DC
	public virtual void SetMagnetized(bool wantsOn, BaseMagnet magnetSource, BasePlayer player)
	{
		this.associatedPlayer = player;
	}
}
