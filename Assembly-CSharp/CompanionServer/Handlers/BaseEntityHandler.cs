using System;

namespace CompanionServer.Handlers
{
	// Token: 0x020009F3 RID: 2547
	public abstract class BaseEntityHandler<T> : BaseHandler<T> where T : class
	{
		// Token: 0x170004FE RID: 1278
		// (get) Token: 0x06003CF1 RID: 15601 RVA: 0x0016647D File Offset: 0x0016467D
		// (set) Token: 0x06003CF2 RID: 15602 RVA: 0x00166485 File Offset: 0x00164685
		private protected AppIOEntity Entity { protected get; private set; }

		// Token: 0x06003CF3 RID: 15603 RVA: 0x0016648E File Offset: 0x0016468E
		public override void EnterPool()
		{
			base.EnterPool();
			this.Entity = null;
		}

		// Token: 0x06003CF4 RID: 15604 RVA: 0x001664A0 File Offset: 0x001646A0
		public override ValidationResult Validate()
		{
			ValidationResult validationResult = base.Validate();
			if (validationResult != ValidationResult.Success)
			{
				return validationResult;
			}
			AppIOEntity appIOEntity = BaseNetworkable.serverEntities.Find(base.Request.entityId) as AppIOEntity;
			if (appIOEntity == null)
			{
				return ValidationResult.NotFound;
			}
			BuildingPrivlidge buildingPrivilege = appIOEntity.GetBuildingPrivilege();
			if (buildingPrivilege != null && !buildingPrivilege.IsAuthed(base.UserId))
			{
				return ValidationResult.NotFound;
			}
			this.Entity = appIOEntity;
			base.Client.Subscribe(new EntityTarget(base.Request.entityId));
			return ValidationResult.Success;
		}
	}
}
