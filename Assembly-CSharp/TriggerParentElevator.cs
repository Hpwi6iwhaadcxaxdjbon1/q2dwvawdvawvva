using System;

// Token: 0x0200058F RID: 1423
public class TriggerParentElevator : TriggerParentEnclosed
{
	// Token: 0x04002338 RID: 9016
	public bool AllowHorsesToBypassClippingChecks = true;

	// Token: 0x06002B75 RID: 11125 RVA: 0x001079BE File Offset: 0x00105BBE
	protected override bool IsClipping(BaseEntity ent)
	{
		return (!this.AllowHorsesToBypassClippingChecks || !(ent is BaseRidableAnimal)) && base.IsClipping(ent);
	}
}
