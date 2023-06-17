using System;
using Rust;
using UnityEngine;

// Token: 0x020005FA RID: 1530
public class ItemModSound : ItemMod
{
	// Token: 0x0400251F RID: 9503
	public GameObjectRef effect = new GameObjectRef();

	// Token: 0x04002520 RID: 9504
	public ItemModSound.Type actionType;

	// Token: 0x06002D7E RID: 11646 RVA: 0x00111BB4 File Offset: 0x0010FDB4
	public override void OnParentChanged(Item item)
	{
		if (Rust.Application.isLoadingSave)
		{
			return;
		}
		if (this.actionType == ItemModSound.Type.OnAttachToWeapon)
		{
			if (item.parentItem == null)
			{
				return;
			}
			if (item.parentItem.info.category != ItemCategory.Weapon)
			{
				return;
			}
			BasePlayer ownerPlayer = item.parentItem.GetOwnerPlayer();
			if (ownerPlayer == null)
			{
				return;
			}
			if (ownerPlayer.IsNpc)
			{
				return;
			}
			Effect.server.Run(this.effect.resourcePath, ownerPlayer, 0U, Vector3.zero, Vector3.zero, null, false);
		}
	}

	// Token: 0x02000D81 RID: 3457
	public enum Type
	{
		// Token: 0x040047BE RID: 18366
		OnAttachToWeapon
	}
}
