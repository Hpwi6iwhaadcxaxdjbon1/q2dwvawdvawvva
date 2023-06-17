using System;
using Facepunch;
using ProtoBuf;

// Token: 0x0200040A RID: 1034
public class CardGamePlayerStorage : StorageContainer
{
	// Token: 0x04001B1A RID: 6938
	private EntityRef cardTableRef;

	// Token: 0x0600231E RID: 8990 RVA: 0x000E0A5C File Offset: 0x000DEC5C
	public BaseCardGameEntity GetCardGameEntity()
	{
		global::BaseEntity baseEntity = this.cardTableRef.Get(base.isServer);
		if (baseEntity != null && baseEntity.IsValid())
		{
			return baseEntity as BaseCardGameEntity;
		}
		return null;
	}

	// Token: 0x0600231F RID: 8991 RVA: 0x000E0A94 File Offset: 0x000DEC94
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.simpleUID != null)
		{
			this.cardTableRef.uid = info.msg.simpleUID.uid;
		}
	}

	// Token: 0x06002320 RID: 8992 RVA: 0x000E0AC8 File Offset: 0x000DECC8
	protected override void OnInventoryDirty()
	{
		base.OnInventoryDirty();
		BaseCardGameEntity cardGameEntity = this.GetCardGameEntity();
		if (cardGameEntity != null)
		{
			cardGameEntity.PlayerStorageChanged();
		}
	}

	// Token: 0x06002321 RID: 8993 RVA: 0x000E0AF1 File Offset: 0x000DECF1
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.simpleUID = Pool.Get<SimpleUID>();
		info.msg.simpleUID.uid = this.cardTableRef.uid;
	}

	// Token: 0x06002322 RID: 8994 RVA: 0x000E0B25 File Offset: 0x000DED25
	public void SetCardTable(BaseCardGameEntity cardGameEntity)
	{
		this.cardTableRef.Set(cardGameEntity);
	}
}
