using System;
using UnityEngine;

// Token: 0x02000764 RID: 1892
[CreateAssetMenu(menuName = "Rust/Underwear")]
public class Underwear : ScriptableObject
{
	// Token: 0x04002AE9 RID: 10985
	public string shortname = "";

	// Token: 0x04002AEA RID: 10986
	public Translate.Phrase displayName;

	// Token: 0x04002AEB RID: 10987
	public Sprite icon;

	// Token: 0x04002AEC RID: 10988
	public Sprite iconFemale;

	// Token: 0x04002AED RID: 10989
	public SkinReplacement[] replacementsMale;

	// Token: 0x04002AEE RID: 10990
	public SkinReplacement[] replacementsFemale;

	// Token: 0x04002AEF RID: 10991
	[Tooltip("User can craft this item on any server if they have this steam item")]
	public SteamInventoryItem steamItem;

	// Token: 0x04002AF0 RID: 10992
	[Tooltip("User can craft this item if they have this DLC purchased")]
	public SteamDLCItem steamDLC;

	// Token: 0x04002AF1 RID: 10993
	public bool adminOnly;

	// Token: 0x060034AF RID: 13487 RVA: 0x00145EA2 File Offset: 0x001440A2
	public uint GetID()
	{
		return StringPool.Get(this.shortname);
	}

	// Token: 0x060034B0 RID: 13488 RVA: 0x00145EAF File Offset: 0x001440AF
	public bool HasMaleParts()
	{
		return this.replacementsMale.Length != 0;
	}

	// Token: 0x060034B1 RID: 13489 RVA: 0x00145EBB File Offset: 0x001440BB
	public bool HasFemaleParts()
	{
		return this.replacementsFemale.Length != 0;
	}

	// Token: 0x060034B2 RID: 13490 RVA: 0x00145EC8 File Offset: 0x001440C8
	public bool ValidForPlayer(BasePlayer player)
	{
		if (this.HasMaleParts() && this.HasFemaleParts())
		{
			return true;
		}
		bool flag = Underwear.IsFemale(player);
		return (flag && this.HasFemaleParts()) || (!flag && this.HasMaleParts());
	}

	// Token: 0x060034B3 RID: 13491 RVA: 0x00145F0C File Offset: 0x0014410C
	public static bool IsFemale(BasePlayer player)
	{
		ulong userID = player.userID;
		ulong num = 4332UL;
		UnityEngine.Random.State state = UnityEngine.Random.state;
		UnityEngine.Random.InitState((int)(num + userID));
		float num2 = UnityEngine.Random.Range(0f, 1f);
		UnityEngine.Random.state = state;
		return num2 > 0.5f;
	}

	// Token: 0x060034B4 RID: 13492 RVA: 0x00145F54 File Offset: 0x00144154
	public static bool Validate(Underwear underwear, BasePlayer player)
	{
		if (underwear == null)
		{
			return true;
		}
		if (!underwear.ValidForPlayer(player))
		{
			return false;
		}
		if (underwear.adminOnly && (!player.IsAdmin || !player.IsDeveloper))
		{
			return false;
		}
		bool flag = underwear.steamItem == null || player.blueprints.steamInventory.HasItem(underwear.steamItem.id);
		bool flag2 = false;
		if (player.isServer && (underwear.steamDLC == null || underwear.steamDLC.HasLicense(player.userID)))
		{
			flag2 = true;
		}
		return flag && flag2;
	}
}
