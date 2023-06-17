using System;
using Facepunch;
using ProtoBuf;
using UnityEngine;

// Token: 0x02000136 RID: 310
public class ReclaimBackpack : StorageContainer
{
	// Token: 0x04000F0D RID: 3853
	public int reclaimID;

	// Token: 0x04000F0E RID: 3854
	public ulong playerSteamID;

	// Token: 0x04000F0F RID: 3855
	public bool onlyOwnerLoot = true;

	// Token: 0x04000F10 RID: 3856
	public Collider myCollider;

	// Token: 0x04000F11 RID: 3857
	public GameObject art;

	// Token: 0x04000F12 RID: 3858
	private bool isBeingLooted;

	// Token: 0x060016C8 RID: 5832 RVA: 0x000AEF4F File Offset: 0x000AD14F
	public void InitForPlayer(ulong playerID, int newID)
	{
		this.playerSteamID = playerID;
		this.reclaimID = newID;
	}

	// Token: 0x060016C9 RID: 5833 RVA: 0x000AEF60 File Offset: 0x000AD160
	public override void ServerInit()
	{
		base.ServerInit();
		base.inventory.SetFlag(global::ItemContainer.Flag.NoItemInput, true);
		base.Invoke(new Action(this.RemoveMe), global::ReclaimManager.reclaim_expire_minutes * 60f);
		base.InvokeRandomized(new Action(this.CheckEmpty), 1f, 30f, 3f);
	}

	// Token: 0x060016CA RID: 5834 RVA: 0x00003384 File Offset: 0x00001584
	public void RemoveMe()
	{
		base.Kill(global::BaseNetworkable.DestroyMode.None);
	}

	// Token: 0x060016CB RID: 5835 RVA: 0x000AEFC2 File Offset: 0x000AD1C2
	public void CheckEmpty()
	{
		if (global::ReclaimManager.instance.GetReclaimForPlayer(this.playerSteamID, this.reclaimID) == null && !this.isBeingLooted)
		{
			base.Kill(global::BaseNetworkable.DestroyMode.None);
		}
	}

	// Token: 0x060016CC RID: 5836 RVA: 0x000AEFEC File Offset: 0x000AD1EC
	public override bool OnStartBeingLooted(global::BasePlayer baseEntity)
	{
		if (baseEntity.InSafeZone() && baseEntity.userID != this.playerSteamID)
		{
			return false;
		}
		if (this.onlyOwnerLoot && baseEntity.userID != this.playerSteamID)
		{
			return false;
		}
		global::ReclaimManager.PlayerReclaimEntry reclaimForPlayer = global::ReclaimManager.instance.GetReclaimForPlayer(baseEntity.userID, this.reclaimID);
		if (reclaimForPlayer != null)
		{
			for (int i = reclaimForPlayer.inventory.itemList.Count - 1; i >= 0; i--)
			{
				reclaimForPlayer.inventory.itemList[i].MoveToContainer(base.inventory, -1, true, false, null, true);
			}
			global::ReclaimManager.instance.RemoveEntry(reclaimForPlayer);
		}
		this.isBeingLooted = true;
		return base.OnStartBeingLooted(baseEntity);
	}

	// Token: 0x060016CD RID: 5837 RVA: 0x000AF09C File Offset: 0x000AD29C
	public override void PlayerStoppedLooting(global::BasePlayer player)
	{
		base.PlayerStoppedLooting(player);
		this.isBeingLooted = false;
		if (base.inventory.itemList.Count > 0)
		{
			global::ReclaimManager.instance.AddPlayerReclaim(this.playerSteamID, base.inventory.itemList, 0UL, "", this.reclaimID);
		}
	}

	// Token: 0x060016CE RID: 5838 RVA: 0x000AF0F4 File Offset: 0x000AD2F4
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.lootableCorpse = Pool.Get<ProtoBuf.LootableCorpse>();
		info.msg.lootableCorpse.playerID = this.playerSteamID;
		info.msg.lootableCorpse.underwearSkin = (uint)this.reclaimID;
	}

	// Token: 0x060016CF RID: 5839 RVA: 0x000AF144 File Offset: 0x000AD344
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.lootableCorpse != null)
		{
			this.playerSteamID = info.msg.lootableCorpse.playerID;
			this.reclaimID = (int)info.msg.lootableCorpse.underwearSkin;
		}
	}
}
