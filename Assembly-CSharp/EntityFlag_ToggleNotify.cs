using System;

// Token: 0x020003C7 RID: 967
public class EntityFlag_ToggleNotify : EntityFlag_Toggle
{
	// Token: 0x04001A1E RID: 6686
	public bool UseEntityParent;

	// Token: 0x06002196 RID: 8598 RVA: 0x000DAD8C File Offset: 0x000D8F8C
	protected override void OnStateToggled(bool state)
	{
		base.OnStateToggled(state);
		IFlagNotify flagNotify;
		if (!this.UseEntityParent && base.baseEntity != null && (flagNotify = (base.baseEntity as IFlagNotify)) != null)
		{
			flagNotify.OnFlagToggled(state);
		}
		IFlagNotify flagNotify2;
		if (this.UseEntityParent && base.baseEntity != null && base.baseEntity.GetParentEntity() != null && (flagNotify2 = (base.baseEntity.GetParentEntity() as IFlagNotify)) != null)
		{
			flagNotify2.OnFlagToggled(state);
		}
	}
}
