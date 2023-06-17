using System;
using UnityEngine;

// Token: 0x02000463 RID: 1123
public class BaseTrap : DecayEntity
{
	// Token: 0x0600252B RID: 9515 RVA: 0x000063A5 File Offset: 0x000045A5
	public virtual void ObjectEntered(GameObject obj)
	{
	}

	// Token: 0x0600252C RID: 9516 RVA: 0x00071CC5 File Offset: 0x0006FEC5
	public virtual void Arm()
	{
		base.SetFlag(BaseEntity.Flags.On, true, false, true);
		base.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x0600252D RID: 9517 RVA: 0x000063A5 File Offset: 0x000045A5
	public virtual void OnEmpty()
	{
	}
}
