using System;
using UnityEngine;

// Token: 0x0200075F RID: 1887
[CreateAssetMenu(menuName = "Rust/Steam DLC Item")]
public class SteamDLCItem : ScriptableObject
{
	// Token: 0x04002ACA RID: 10954
	public int id;

	// Token: 0x04002ACB RID: 10955
	public Translate.Phrase dlcName;

	// Token: 0x04002ACC RID: 10956
	public int dlcAppID;

	// Token: 0x04002ACD RID: 10957
	public bool bypassLicenseCheck;

	// Token: 0x0600349D RID: 13469 RVA: 0x00145B2C File Offset: 0x00143D2C
	public bool HasLicense(ulong steamid)
	{
		return this.bypassLicenseCheck || (PlatformService.Instance.IsValid && PlatformService.Instance.PlayerOwnsDownloadableContent(steamid, this.dlcAppID));
	}

	// Token: 0x0600349E RID: 13470 RVA: 0x00145B57 File Offset: 0x00143D57
	public bool CanUse(BasePlayer player)
	{
		return player.isServer && (this.HasLicense(player.userID) || player.userID < 10000000UL);
	}
}
