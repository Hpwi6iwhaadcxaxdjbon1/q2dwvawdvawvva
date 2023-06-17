using System;
using ConVar;

// Token: 0x020003E0 RID: 992
public class DebrisEntity : BaseCombatEntity
{
	// Token: 0x04001A5E RID: 6750
	public float DebrisDespawnOverride;

	// Token: 0x06002212 RID: 8722 RVA: 0x000DCCAE File Offset: 0x000DAEAE
	public override void ServerInit()
	{
		this.ResetRemovalTime();
		base.ServerInit();
	}

	// Token: 0x06002213 RID: 8723 RVA: 0x00003384 File Offset: 0x00001584
	public void RemoveCorpse()
	{
		base.Kill(BaseNetworkable.DestroyMode.None);
	}

	// Token: 0x06002214 RID: 8724 RVA: 0x000DCCBC File Offset: 0x000DAEBC
	public void ResetRemovalTime(float dur)
	{
		using (TimeWarning.New("ResetRemovalTime", 0))
		{
			if (base.IsInvoking(new Action(this.RemoveCorpse)))
			{
				base.CancelInvoke(new Action(this.RemoveCorpse));
			}
			base.Invoke(new Action(this.RemoveCorpse), dur);
		}
	}

	// Token: 0x06002215 RID: 8725 RVA: 0x000DCD2C File Offset: 0x000DAF2C
	public float GetRemovalTime()
	{
		if (this.DebrisDespawnOverride <= 0f)
		{
			return Server.debrisdespawn;
		}
		return this.DebrisDespawnOverride;
	}

	// Token: 0x06002216 RID: 8726 RVA: 0x000DCD47 File Offset: 0x000DAF47
	public void ResetRemovalTime()
	{
		this.ResetRemovalTime(this.GetRemovalTime());
	}

	// Token: 0x06002217 RID: 8727 RVA: 0x000DCD55 File Offset: 0x000DAF55
	public override string Categorize()
	{
		return "debris";
	}
}
