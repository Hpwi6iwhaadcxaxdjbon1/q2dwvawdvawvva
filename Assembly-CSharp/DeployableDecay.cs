using System;
using ConVar;

// Token: 0x020003E5 RID: 997
public class DeployableDecay : global::Decay
{
	// Token: 0x04001A6C RID: 6764
	public float decayDelay = 8f;

	// Token: 0x04001A6D RID: 6765
	public float decayDuration = 8f;

	// Token: 0x06002240 RID: 8768 RVA: 0x000DD7B2 File Offset: 0x000DB9B2
	public override float GetDecayDelay(BaseEntity entity)
	{
		return this.decayDelay * 60f * 60f;
	}

	// Token: 0x06002241 RID: 8769 RVA: 0x000DD7C6 File Offset: 0x000DB9C6
	public override float GetDecayDuration(BaseEntity entity)
	{
		return this.decayDuration * 60f * 60f;
	}

	// Token: 0x06002242 RID: 8770 RVA: 0x000DCE2E File Offset: 0x000DB02E
	public override bool ShouldDecay(BaseEntity entity)
	{
		return ConVar.Decay.upkeep || entity.IsOutside();
	}
}
