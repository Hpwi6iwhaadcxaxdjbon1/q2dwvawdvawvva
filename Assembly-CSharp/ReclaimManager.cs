using System;
using System.Collections.Generic;
using Facepunch;
using ProtoBuf;
using UnityEngine;

// Token: 0x02000515 RID: 1301
public class ReclaimManager : global::BaseEntity
{
	// Token: 0x04002175 RID: 8565
	private const int defaultReclaims = 128;

	// Token: 0x04002176 RID: 8566
	private const int reclaimSlotCount = 40;

	// Token: 0x04002177 RID: 8567
	private int lastReclaimID;

	// Token: 0x04002178 RID: 8568
	[ServerVar]
	public static float reclaim_expire_minutes = 120f;

	// Token: 0x04002179 RID: 8569
	private static global::ReclaimManager _instance;

	// Token: 0x0400217A RID: 8570
	public List<global::ReclaimManager.PlayerReclaimEntry> entries = new List<global::ReclaimManager.PlayerReclaimEntry>();

	// Token: 0x0400217B RID: 8571
	private float lastTickTime;

	// Token: 0x17000381 RID: 897
	// (get) Token: 0x0600297D RID: 10621 RVA: 0x000FE5F2 File Offset: 0x000FC7F2
	public static global::ReclaimManager instance
	{
		get
		{
			return global::ReclaimManager._instance;
		}
	}

	// Token: 0x0600297E RID: 10622 RVA: 0x000FE5FC File Offset: 0x000FC7FC
	public int AddPlayerReclaim(ulong victimID, List<global::Item> itemList, ulong killerID = 0UL, string killerString = "", int reclaimIDToUse = -1)
	{
		global::ReclaimManager.PlayerReclaimEntry playerReclaimEntry = this.NewEntry();
		for (int i = itemList.Count - 1; i >= 0; i--)
		{
			itemList[i].MoveToContainer(playerReclaimEntry.inventory, -1, true, false, null, true);
		}
		if (reclaimIDToUse == -1)
		{
			this.lastReclaimID++;
			reclaimIDToUse = this.lastReclaimID;
		}
		playerReclaimEntry.victimID = victimID;
		playerReclaimEntry.killerID = killerID;
		playerReclaimEntry.killerString = killerString;
		playerReclaimEntry.id = reclaimIDToUse;
		this.entries.Add(playerReclaimEntry);
		return reclaimIDToUse;
	}

	// Token: 0x0600297F RID: 10623 RVA: 0x000FE684 File Offset: 0x000FC884
	public void DoCleanup()
	{
		for (int i = this.entries.Count - 1; i >= 0; i--)
		{
			global::ReclaimManager.PlayerReclaimEntry playerReclaimEntry = this.entries[i];
			if (playerReclaimEntry.inventory.itemList.Count == 0 || playerReclaimEntry.timeAlive / 60f > global::ReclaimManager.reclaim_expire_minutes)
			{
				this.RemoveEntry(playerReclaimEntry);
			}
		}
	}

	// Token: 0x06002980 RID: 10624 RVA: 0x000FE6E4 File Offset: 0x000FC8E4
	public void TickEntries()
	{
		float num = Time.realtimeSinceStartup - this.lastTickTime;
		foreach (global::ReclaimManager.PlayerReclaimEntry playerReclaimEntry in this.entries)
		{
			playerReclaimEntry.timeAlive += num;
		}
		this.lastTickTime = Time.realtimeSinceStartup;
		this.DoCleanup();
	}

	// Token: 0x06002981 RID: 10625 RVA: 0x000FE75C File Offset: 0x000FC95C
	public bool HasReclaims(ulong playerID)
	{
		foreach (global::ReclaimManager.PlayerReclaimEntry playerReclaimEntry in this.entries)
		{
			if (playerReclaimEntry.victimID == playerID && playerReclaimEntry.inventory.itemList.Count > 0)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06002982 RID: 10626 RVA: 0x000FE7CC File Offset: 0x000FC9CC
	public global::ReclaimManager.PlayerReclaimEntry GetReclaimForPlayer(ulong playerID, int reclaimID)
	{
		foreach (global::ReclaimManager.PlayerReclaimEntry playerReclaimEntry in this.entries)
		{
			if (playerReclaimEntry.victimID == playerID && playerReclaimEntry.id == reclaimID)
			{
				return playerReclaimEntry;
			}
		}
		return null;
	}

	// Token: 0x06002983 RID: 10627 RVA: 0x000FE834 File Offset: 0x000FCA34
	public bool GetReclaimsForPlayer(ulong playerID, ref List<global::ReclaimManager.PlayerReclaimEntry> list)
	{
		foreach (global::ReclaimManager.PlayerReclaimEntry playerReclaimEntry in this.entries)
		{
			if (playerReclaimEntry.victimID == playerID)
			{
				list.Add(playerReclaimEntry);
			}
		}
		return list.Count > 0;
	}

	// Token: 0x06002984 RID: 10628 RVA: 0x000FE89C File Offset: 0x000FCA9C
	public global::ReclaimManager.PlayerReclaimEntry NewEntry()
	{
		global::ReclaimManager.PlayerReclaimEntry playerReclaimEntry = Pool.Get<global::ReclaimManager.PlayerReclaimEntry>();
		playerReclaimEntry.Init();
		return playerReclaimEntry;
	}

	// Token: 0x06002985 RID: 10629 RVA: 0x000FE8A9 File Offset: 0x000FCAA9
	public void RemoveEntry(global::ReclaimManager.PlayerReclaimEntry entry)
	{
		entry.Cleanup();
		this.entries.Remove(entry);
		Pool.Free<global::ReclaimManager.PlayerReclaimEntry>(ref entry);
		entry = null;
	}

	// Token: 0x06002986 RID: 10630 RVA: 0x000FE8C8 File Offset: 0x000FCAC8
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.fromDisk && info.msg.reclaimManager != null)
		{
			this.lastReclaimID = info.msg.reclaimManager.lastReclaimID;
			foreach (ProtoBuf.ReclaimManager.ReclaimInfo reclaimInfo in info.msg.reclaimManager.reclaimEntries)
			{
				global::ReclaimManager.PlayerReclaimEntry playerReclaimEntry = this.NewEntry();
				playerReclaimEntry.killerID = reclaimInfo.killerID;
				playerReclaimEntry.victimID = reclaimInfo.victimID;
				playerReclaimEntry.killerString = reclaimInfo.killerString;
				playerReclaimEntry.inventory.Load(reclaimInfo.inventory);
				this.entries.Add(playerReclaimEntry);
			}
		}
	}

	// Token: 0x06002987 RID: 10631 RVA: 0x000FE9A0 File Offset: 0x000FCBA0
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		if (info.forDisk)
		{
			info.msg.reclaimManager = Pool.Get<ProtoBuf.ReclaimManager>();
			info.msg.reclaimManager.reclaimEntries = Pool.GetList<ProtoBuf.ReclaimManager.ReclaimInfo>();
			info.msg.reclaimManager.lastReclaimID = this.lastReclaimID;
			foreach (global::ReclaimManager.PlayerReclaimEntry playerReclaimEntry in this.entries)
			{
				ProtoBuf.ReclaimManager.ReclaimInfo reclaimInfo = Pool.Get<ProtoBuf.ReclaimManager.ReclaimInfo>();
				reclaimInfo.killerID = playerReclaimEntry.killerID;
				reclaimInfo.victimID = playerReclaimEntry.victimID;
				reclaimInfo.killerString = playerReclaimEntry.killerString;
				reclaimInfo.inventory = playerReclaimEntry.inventory.Save();
				info.msg.reclaimManager.reclaimEntries.Add(reclaimInfo);
			}
		}
	}

	// Token: 0x06002988 RID: 10632 RVA: 0x000FEA8C File Offset: 0x000FCC8C
	public override void ServerInit()
	{
		base.InvokeRepeating(new Action(this.TickEntries), 1f, 60f);
		global::ReclaimManager._instance = this;
		base.ServerInit();
	}

	// Token: 0x06002989 RID: 10633 RVA: 0x000FEAB6 File Offset: 0x000FCCB6
	internal override void DoServerDestroy()
	{
		global::ReclaimManager._instance = null;
		base.DoServerDestroy();
	}

	// Token: 0x02000D35 RID: 3381
	public class PlayerReclaimEntry
	{
		// Token: 0x04004685 RID: 18053
		public ulong killerID;

		// Token: 0x04004686 RID: 18054
		public string killerString;

		// Token: 0x04004687 RID: 18055
		public ulong victimID;

		// Token: 0x04004688 RID: 18056
		public float timeAlive;

		// Token: 0x04004689 RID: 18057
		public int id;

		// Token: 0x0400468A RID: 18058
		public global::ItemContainer inventory;

		// Token: 0x0600506B RID: 20587 RVA: 0x001A938C File Offset: 0x001A758C
		public void Init()
		{
			this.inventory = Pool.Get<global::ItemContainer>();
			this.inventory.entityOwner = global::ReclaimManager.instance;
			this.inventory.allowedContents = global::ItemContainer.ContentsType.Generic;
			this.inventory.SetOnlyAllowedItem(null);
			this.inventory.maxStackSize = 0;
			this.inventory.ServerInitialize(null, 40);
			this.inventory.canAcceptItem = null;
			this.inventory.GiveUID();
		}

		// Token: 0x0600506C RID: 20588 RVA: 0x001A9400 File Offset: 0x001A7600
		public void Cleanup()
		{
			this.timeAlive = 0f;
			this.killerID = 0UL;
			this.killerString = "";
			this.victimID = 0UL;
			this.id = -2;
			if (this.inventory != null)
			{
				this.inventory.Clear();
				Pool.Free<global::ItemContainer>(ref this.inventory);
			}
		}
	}
}
