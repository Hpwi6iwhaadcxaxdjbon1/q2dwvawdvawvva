using System;
using System.Collections.Generic;
using Facepunch;
using UnityEngine;

// Token: 0x02000126 RID: 294
public class InvisibleVendingMachine : NPCVendingMachine
{
	// Token: 0x04000EC7 RID: 3783
	public GameObjectRef buyEffect;

	// Token: 0x04000EC8 RID: 3784
	public NPCVendingOrderManifest vmoManifest;

	// Token: 0x06001695 RID: 5781 RVA: 0x000AE614 File Offset: 0x000AC814
	public NPCShopKeeper GetNPCShopKeeper()
	{
		List<NPCShopKeeper> list = Pool.GetList<NPCShopKeeper>();
		Vis.Entities<NPCShopKeeper>(base.transform.position, 2f, list, 131072, QueryTriggerInteraction.Collide);
		NPCShopKeeper result = null;
		if (list.Count > 0)
		{
			result = list[0];
		}
		Pool.FreeList<NPCShopKeeper>(ref list);
		return result;
	}

	// Token: 0x06001696 RID: 5782 RVA: 0x000AE660 File Offset: 0x000AC860
	public void KeeperLookAt(Vector3 pos)
	{
		NPCShopKeeper npcshopKeeper = this.GetNPCShopKeeper();
		if (npcshopKeeper == null)
		{
			return;
		}
		npcshopKeeper.SetAimDirection(Vector3Ex.Direction2D(pos, npcshopKeeper.transform.position));
	}

	// Token: 0x06001697 RID: 5783 RVA: 0x00007A3C File Offset: 0x00005C3C
	public override bool HasVendingSounds()
	{
		return false;
	}

	// Token: 0x06001698 RID: 5784 RVA: 0x00028E70 File Offset: 0x00027070
	public override float GetBuyDuration()
	{
		return 0.5f;
	}

	// Token: 0x06001699 RID: 5785 RVA: 0x000AE698 File Offset: 0x000AC898
	public override void CompletePendingOrder()
	{
		Effect.server.Run(this.buyEffect.resourcePath, base.transform.position, Vector3.up, null, false);
		NPCShopKeeper npcshopKeeper = this.GetNPCShopKeeper();
		if (npcshopKeeper)
		{
			npcshopKeeper.SignalBroadcast(BaseEntity.Signal.Gesture, "victory", null);
			if (this.vend_Player != null)
			{
				npcshopKeeper.SetAimDirection(Vector3Ex.Direction2D(this.vend_Player.transform.position, npcshopKeeper.transform.position));
			}
		}
		base.CompletePendingOrder();
	}

	// Token: 0x0600169A RID: 5786 RVA: 0x000AE71E File Offset: 0x000AC91E
	public override bool PlayerOpenLoot(BasePlayer player, string panelToOpen = "", bool doPositionChecks = true)
	{
		this.KeeperLookAt(player.transform.position);
		return base.PlayerOpenLoot(player, panelToOpen, true);
	}

	// Token: 0x0600169B RID: 5787 RVA: 0x000AE73C File Offset: 0x000AC93C
	public override void Save(BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		if (this.vmoManifest != null && info.msg.vendingMachine != null)
		{
			info.msg.vendingMachine.vmoIndex = this.vmoManifest.GetIndex(this.vendingOrders);
		}
	}

	// Token: 0x0600169C RID: 5788 RVA: 0x000AE78C File Offset: 0x000AC98C
	public override void ServerInit()
	{
		base.ServerInit();
		if (this.vmoManifest.GetIndex(this.vendingOrders) == -1)
		{
			Debug.LogError("VENDING ORDERS NOT FOUND! Did you forget to add these orders to the VMOManifest?");
		}
	}

	// Token: 0x0600169D RID: 5789 RVA: 0x000AE7B4 File Offset: 0x000AC9B4
	public override void Load(BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.fromDisk && this.vmoManifest != null && info.msg.vendingMachine != null)
		{
			if (info.msg.vendingMachine.vmoIndex == -1 && TerrainMeta.Path.Monuments != null)
			{
				foreach (MonumentInfo monumentInfo in TerrainMeta.Path.Monuments)
				{
					if (monumentInfo.displayPhrase.token.Contains("fish") && Vector3.Distance(monumentInfo.transform.position, base.transform.position) < 100f)
					{
						info.msg.vendingMachine.vmoIndex = 17;
					}
				}
			}
			NPCVendingOrder fromIndex = this.vmoManifest.GetFromIndex(info.msg.vendingMachine.vmoIndex);
			this.vendingOrders = fromIndex;
		}
	}
}
