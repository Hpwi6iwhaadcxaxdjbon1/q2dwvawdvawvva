using System;

// Token: 0x020003C5 RID: 965
public class EntityFlag_TOD : EntityComponent<BaseEntity>
{
	// Token: 0x04001A13 RID: 6675
	public BaseEntity.Flags desiredFlag;

	// Token: 0x04001A14 RID: 6676
	public bool onAtNight = true;

	// Token: 0x0600218A RID: 8586 RVA: 0x000DAB9B File Offset: 0x000D8D9B
	public void Start()
	{
		base.Invoke(new Action(this.Initialize), 1f);
	}

	// Token: 0x0600218B RID: 8587 RVA: 0x000DABB4 File Offset: 0x000D8DB4
	public void Initialize()
	{
		if (base.baseEntity == null || base.baseEntity.isClient)
		{
			return;
		}
		base.InvokeRandomized(new Action(this.DoTimeCheck), 0f, 5f, 1f);
	}

	// Token: 0x0600218C RID: 8588 RVA: 0x000DABF4 File Offset: 0x000D8DF4
	public bool WantsOn()
	{
		if (TOD_Sky.Instance == null)
		{
			return false;
		}
		bool isNight = TOD_Sky.Instance.IsNight;
		return this.onAtNight == isNight;
	}

	// Token: 0x0600218D RID: 8589 RVA: 0x000DAC28 File Offset: 0x000D8E28
	private void DoTimeCheck()
	{
		bool flag = base.baseEntity.HasFlag(this.desiredFlag);
		bool flag2 = this.WantsOn();
		if (flag != flag2)
		{
			base.baseEntity.SetFlag(this.desiredFlag, flag2, false, true);
		}
	}
}
