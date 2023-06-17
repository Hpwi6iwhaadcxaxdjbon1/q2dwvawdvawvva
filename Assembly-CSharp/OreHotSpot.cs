using System;

// Token: 0x02000176 RID: 374
public class OreHotSpot : BaseCombatEntity, ILOD
{
	// Token: 0x04001063 RID: 4195
	public float visualDistance = 20f;

	// Token: 0x04001064 RID: 4196
	public GameObjectRef visualEffect;

	// Token: 0x04001065 RID: 4197
	public GameObjectRef finishEffect;

	// Token: 0x04001066 RID: 4198
	public GameObjectRef damageEffect;

	// Token: 0x04001067 RID: 4199
	public OreResourceEntity owner;

	// Token: 0x06001782 RID: 6018 RVA: 0x000B2901 File Offset: 0x000B0B01
	public void OreOwner(OreResourceEntity newOwner)
	{
		this.owner = newOwner;
	}

	// Token: 0x06001783 RID: 6019 RVA: 0x00047EB3 File Offset: 0x000460B3
	public override void ServerInit()
	{
		base.ServerInit();
	}

	// Token: 0x06001784 RID: 6020 RVA: 0x000B290A File Offset: 0x000B0B0A
	public override void OnAttacked(HitInfo info)
	{
		base.OnAttacked(info);
		if (base.isClient)
		{
			return;
		}
		if (this.owner)
		{
			this.owner.OnAttacked(info);
		}
	}

	// Token: 0x06001785 RID: 6021 RVA: 0x000B2935 File Offset: 0x000B0B35
	public override void OnKilled(HitInfo info)
	{
		this.FireFinishEffect();
		base.OnKilled(info);
	}

	// Token: 0x06001786 RID: 6022 RVA: 0x000B2944 File Offset: 0x000B0B44
	public void FireFinishEffect()
	{
		if (this.finishEffect.isValid)
		{
			Effect.server.Run(this.finishEffect.resourcePath, base.transform.position, base.transform.forward, null, false);
		}
	}
}
