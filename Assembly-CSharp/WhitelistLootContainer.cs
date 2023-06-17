using System;
using System.Collections.Generic;
using Facepunch;
using ProtoBuf;
using UnityEngine;

// Token: 0x0200042C RID: 1068
public class WhitelistLootContainer : LootContainer
{
	// Token: 0x04001C1C RID: 7196
	public static readonly Translate.Phrase CantLootToast = new Translate.Phrase("whitelistcontainer.noloot", "You are not authorized to access this box");

	// Token: 0x04001C1D RID: 7197
	public List<ulong> whitelist = new List<ulong>();

	// Token: 0x06002403 RID: 9219 RVA: 0x000E5F9C File Offset: 0x000E419C
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		if (info.forDisk)
		{
			info.msg.whitelist = Pool.Get<Whitelist>();
			info.msg.whitelist.users = Pool.GetList<ulong>();
			foreach (ulong num in this.whitelist)
			{
				info.msg.whitelist.users.Add(num);
				Debug.Log("Whitelistcontainer saving user " + num);
			}
		}
	}

	// Token: 0x06002404 RID: 9220 RVA: 0x000E6048 File Offset: 0x000E4248
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		if (info.fromDisk && info.msg.whitelist != null)
		{
			foreach (ulong item in info.msg.whitelist.users)
			{
				this.whitelist.Add(item);
			}
		}
		base.Load(info);
	}

	// Token: 0x06002405 RID: 9221 RVA: 0x000E60C8 File Offset: 0x000E42C8
	public void MissionSetupPlayer(global::BasePlayer player)
	{
		this.AddToWhitelist(player.userID);
	}

	// Token: 0x06002406 RID: 9222 RVA: 0x000E60D6 File Offset: 0x000E42D6
	public void AddToWhitelist(ulong userid)
	{
		if (!this.whitelist.Contains(userid))
		{
			this.whitelist.Add(userid);
		}
	}

	// Token: 0x06002407 RID: 9223 RVA: 0x000E60F2 File Offset: 0x000E42F2
	public void RemoveFromWhitelist(ulong userid)
	{
		if (this.whitelist.Contains(userid))
		{
			this.whitelist.Remove(userid);
		}
	}

	// Token: 0x06002408 RID: 9224 RVA: 0x000E6110 File Offset: 0x000E4310
	public override bool PlayerOpenLoot(global::BasePlayer player, string panelToOpen = "", bool doPositionChecks = true)
	{
		ulong userID = player.userID;
		if (!this.whitelist.Contains(userID))
		{
			player.ShowToast(GameTip.Styles.Red_Normal, WhitelistLootContainer.CantLootToast, Array.Empty<string>());
			return false;
		}
		return base.PlayerOpenLoot(player, panelToOpen, doPositionChecks);
	}
}
