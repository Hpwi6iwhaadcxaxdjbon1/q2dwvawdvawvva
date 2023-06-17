using System;
using Rust.Workshop;
using UnityEngine;

// Token: 0x02000751 RID: 1873
[CreateAssetMenu(menuName = "Rust/ItemSkin")]
public class ItemSkin : SteamInventoryItem
{
	// Token: 0x04002A77 RID: 10871
	public Skinnable Skinnable;

	// Token: 0x04002A78 RID: 10872
	public Material[] Materials;

	// Token: 0x04002A79 RID: 10873
	[Tooltip("If set, whenever we make an item with this skin, we'll spawn this item without a skin instead")]
	public ItemDefinition Redirect;

	// Token: 0x04002A7A RID: 10874
	public SteamInventoryItem UnlockedViaSteamItem;

	// Token: 0x06003469 RID: 13417 RVA: 0x00144977 File Offset: 0x00142B77
	public void ApplySkin(GameObject obj)
	{
		if (this.Skinnable == null)
		{
			return;
		}
		Skin.Apply(obj, this.Skinnable, this.Materials);
	}

	// Token: 0x0600346A RID: 13418 RVA: 0x0014499C File Offset: 0x00142B9C
	public override bool HasUnlocked(ulong playerId)
	{
		if (this.Redirect != null && this.Redirect.isRedirectOf != null && this.Redirect.isRedirectOf.steamItem != null)
		{
			BasePlayer basePlayer = BasePlayer.FindByID(playerId);
			if (basePlayer != null && basePlayer.blueprints.CheckSkinOwnership(this.Redirect.isRedirectOf.steamItem.id, basePlayer.userID))
			{
				return true;
			}
		}
		if (this.UnlockedViaSteamItem != null)
		{
			BasePlayer basePlayer2 = BasePlayer.FindByID(playerId);
			if (basePlayer2 != null && basePlayer2.blueprints.CheckSkinOwnership(this.UnlockedViaSteamItem.id, basePlayer2.userID))
			{
				return true;
			}
		}
		return base.HasUnlocked(playerId);
	}
}
