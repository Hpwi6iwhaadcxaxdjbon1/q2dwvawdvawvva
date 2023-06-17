using System;
using UnityEngine;

// Token: 0x020003C1 RID: 961
public class EntityComponent<T> : EntityComponentBase where T : BaseEntity
{
	// Token: 0x04001A0A RID: 6666
	[NonSerialized]
	private T _baseEntity;

	// Token: 0x170002CB RID: 715
	// (get) Token: 0x0600217E RID: 8574 RVA: 0x000DAA6B File Offset: 0x000D8C6B
	protected T baseEntity
	{
		get
		{
			if (this._baseEntity == null)
			{
				this.UpdateBaseEntity();
			}
			return this._baseEntity;
		}
	}

	// Token: 0x0600217F RID: 8575 RVA: 0x000DAA8C File Offset: 0x000D8C8C
	protected void UpdateBaseEntity()
	{
		if (!this)
		{
			return;
		}
		if (!base.gameObject)
		{
			return;
		}
		this._baseEntity = (base.gameObject.ToBaseEntity() as T);
	}

	// Token: 0x06002180 RID: 8576 RVA: 0x000DAAC0 File Offset: 0x000D8CC0
	protected override BaseEntity GetBaseEntity()
	{
		return this.baseEntity;
	}
}
