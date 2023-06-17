using System;

// Token: 0x02000979 RID: 2425
public class ViewmodelAttachment : EntityComponent<BaseEntity>, IClientComponent, IViewModeChanged, IViewModelUpdated
{
	// Token: 0x04003416 RID: 13334
	public GameObjectRef modelObject;

	// Token: 0x04003417 RID: 13335
	public string targetBone;

	// Token: 0x04003418 RID: 13336
	public bool hideViewModelIronSights;
}
