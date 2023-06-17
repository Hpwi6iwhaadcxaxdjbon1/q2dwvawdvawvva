using System;

// Token: 0x020004C6 RID: 1222
public class DummySwitch : IOEntity
{
	// Token: 0x04002037 RID: 8247
	public string listenString = "";

	// Token: 0x04002038 RID: 8248
	public string listenStringOff = "";

	// Token: 0x04002039 RID: 8249
	public float duration = -1f;

	// Token: 0x060027D6 RID: 10198 RVA: 0x00007641 File Offset: 0x00005841
	public override bool WantsPower()
	{
		return base.IsOn();
	}

	// Token: 0x060027D7 RID: 10199 RVA: 0x00062769 File Offset: 0x00060969
	public override void ResetIOState()
	{
		base.SetFlag(BaseEntity.Flags.On, false, false, true);
	}

	// Token: 0x060027D8 RID: 10200 RVA: 0x00062775 File Offset: 0x00060975
	public override int GetPassthroughAmount(int outputSlot = 0)
	{
		if (!base.IsOn())
		{
			return 0;
		}
		return this.GetCurrentEnergy();
	}

	// Token: 0x060027D9 RID: 10201 RVA: 0x000F8045 File Offset: 0x000F6245
	public void SetOn(bool wantsOn)
	{
		base.SetFlag(BaseEntity.Flags.On, wantsOn, false, true);
		this.MarkDirty();
		if (base.IsOn() && this.duration != -1f)
		{
			base.Invoke(new Action(this.SetOff), this.duration);
		}
	}

	// Token: 0x060027DA RID: 10202 RVA: 0x000F8084 File Offset: 0x000F6284
	public void SetOff()
	{
		this.SetOn(false);
	}

	// Token: 0x060027DB RID: 10203 RVA: 0x000F8090 File Offset: 0x000F6290
	public override void OnEntityMessage(BaseEntity from, string msg)
	{
		if (msg == this.listenString)
		{
			if (base.IsOn())
			{
				this.SetOn(false);
			}
			this.SetOn(true);
			return;
		}
		if (msg == this.listenStringOff && this.listenStringOff != "" && base.IsOn())
		{
			this.SetOn(false);
		}
	}
}
