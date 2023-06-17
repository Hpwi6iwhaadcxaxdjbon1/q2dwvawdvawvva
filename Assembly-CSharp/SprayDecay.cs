using System;
using ConVar;
using UnityEngine;

// Token: 0x020003E6 RID: 998
public class SprayDecay : global::Decay
{
	// Token: 0x06002244 RID: 8772 RVA: 0x0000441C File Offset: 0x0000261C
	public override bool ShouldDecay(BaseEntity entity)
	{
		return true;
	}

	// Token: 0x06002245 RID: 8773 RVA: 0x00029CA8 File Offset: 0x00027EA8
	public override float GetDecayDelay(BaseEntity entity)
	{
		return 0f;
	}

	// Token: 0x06002246 RID: 8774 RVA: 0x000DD7F8 File Offset: 0x000DB9F8
	public override float GetDecayDuration(BaseEntity entity)
	{
		return Mathf.Max(Global.SprayDuration, 1f);
	}
}
