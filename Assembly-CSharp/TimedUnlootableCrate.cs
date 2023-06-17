using System;

// Token: 0x02000420 RID: 1056
public class TimedUnlootableCrate : LootContainer
{
	// Token: 0x04001BF5 RID: 7157
	public bool unlootableOnSpawn = true;

	// Token: 0x04001BF6 RID: 7158
	public float unlootableDuration = 300f;

	// Token: 0x060023C9 RID: 9161 RVA: 0x000E4FA1 File Offset: 0x000E31A1
	public override void ServerInit()
	{
		base.ServerInit();
		if (this.unlootableOnSpawn)
		{
			this.SetUnlootableFor(this.unlootableDuration);
		}
	}

	// Token: 0x060023CA RID: 9162 RVA: 0x000E4FBD File Offset: 0x000E31BD
	public void SetUnlootableFor(float duration)
	{
		base.SetFlag(BaseEntity.Flags.OnFire, true, false, true);
		base.SetFlag(BaseEntity.Flags.Locked, true, false, true);
		this.unlootableDuration = duration;
		base.Invoke(new Action(this.MakeLootable), duration);
	}

	// Token: 0x060023CB RID: 9163 RVA: 0x000E4FEE File Offset: 0x000E31EE
	public void MakeLootable()
	{
		base.SetFlag(BaseEntity.Flags.OnFire, false, false, true);
		base.SetFlag(BaseEntity.Flags.Locked, false, false, true);
	}
}
