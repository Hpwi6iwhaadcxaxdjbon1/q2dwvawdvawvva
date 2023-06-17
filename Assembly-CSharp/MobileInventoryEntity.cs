using System;

// Token: 0x020003A1 RID: 929
public class MobileInventoryEntity : BaseEntity
{
	// Token: 0x0400199B RID: 6555
	public SoundDefinition ringingLoop;

	// Token: 0x0400199C RID: 6556
	public SoundDefinition silentLoop;

	// Token: 0x0400199D RID: 6557
	public const BaseEntity.Flags Ringing = BaseEntity.Flags.Reserved1;

	// Token: 0x0400199E RID: 6558
	public static BaseEntity.Flags Flag_Silent = BaseEntity.Flags.Reserved2;

	// Token: 0x0600208B RID: 8331 RVA: 0x000D7081 File Offset: 0x000D5281
	public void ToggleRinging(bool state)
	{
		base.SetFlag(BaseEntity.Flags.Reserved1, state, false, true);
	}

	// Token: 0x0600208C RID: 8332 RVA: 0x000D7091 File Offset: 0x000D5291
	public void SetSilentMode(bool wantsSilent)
	{
		base.SetFlag(MobileInventoryEntity.Flag_Silent, wantsSilent, false, true);
	}
}
