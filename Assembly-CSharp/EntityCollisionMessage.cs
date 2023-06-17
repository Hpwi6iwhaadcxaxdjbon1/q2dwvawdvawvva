using System;
using UnityEngine;

// Token: 0x020003C0 RID: 960
public class EntityCollisionMessage : EntityComponent<BaseEntity>
{
	// Token: 0x0600217C RID: 8572 RVA: 0x000DA9F8 File Offset: 0x000D8BF8
	private void OnCollisionEnter(Collision collision)
	{
		if (base.baseEntity == null)
		{
			return;
		}
		if (base.baseEntity.IsDestroyed)
		{
			return;
		}
		BaseEntity baseEntity = collision.GetEntity();
		if (baseEntity == base.baseEntity)
		{
			return;
		}
		if (baseEntity != null)
		{
			if (baseEntity.IsDestroyed)
			{
				return;
			}
			if (base.baseEntity.isServer)
			{
				baseEntity = baseEntity.ToServer<BaseEntity>();
			}
		}
		base.baseEntity.OnCollision(collision, baseEntity);
	}
}
