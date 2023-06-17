using System;
using ConVar;
using Facepunch.Rust;
using Network;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x0200005C RID: 92
public class CollectibleEntity : BaseEntity, IPrefabPreProcess
{
	// Token: 0x04000682 RID: 1666
	public Translate.Phrase itemName;

	// Token: 0x04000683 RID: 1667
	public ItemAmount[] itemList;

	// Token: 0x04000684 RID: 1668
	public GameObjectRef pickupEffect;

	// Token: 0x04000685 RID: 1669
	public float xpScale = 1f;

	// Token: 0x060009C3 RID: 2499 RVA: 0x0005BE60 File Offset: 0x0005A060
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("CollectibleEntity.OnRpcMessage", 0))
		{
			if (rpc == 2778075470U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - Pickup ");
				}
				using (TimeWarning.New("Pickup", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.MaxDistance.Test(2778075470U, "Pickup", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							BaseEntity.RPCMessage msg2 = new BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.Pickup(msg2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in Pickup");
					}
				}
				return true;
			}
			if (rpc == 3528769075U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - PickupEat ");
				}
				using (TimeWarning.New("PickupEat", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.MaxDistance.Test(3528769075U, "PickupEat", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							BaseEntity.RPCMessage msg3 = new BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.PickupEat(msg3);
						}
					}
					catch (Exception exception2)
					{
						Debug.LogException(exception2);
						player.Kick("RPC Error in PickupEat");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x060009C4 RID: 2500 RVA: 0x0005C160 File Offset: 0x0005A360
	public bool IsFood(bool checkConsumeMod = false)
	{
		for (int i = 0; i < this.itemList.Length; i++)
		{
			if (this.itemList[i].itemDef.category == ItemCategory.Food && (!checkConsumeMod || this.itemList[i].itemDef.GetComponent<ItemModConsume>() != null))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x060009C5 RID: 2501 RVA: 0x0005C1B8 File Offset: 0x0005A3B8
	public void DoPickup(BasePlayer reciever, bool eat = false)
	{
		if (this.itemList == null)
		{
			return;
		}
		foreach (ItemAmount itemAmount in this.itemList)
		{
			Item item = ItemManager.Create(itemAmount.itemDef, (int)itemAmount.amount, 0UL);
			if (item != null)
			{
				if (eat && item.info.category == ItemCategory.Food && reciever != null)
				{
					ItemModConsume component = item.info.GetComponent<ItemModConsume>();
					if (component != null)
					{
						component.DoAction(item, reciever);
						goto IL_DB;
					}
				}
				if (reciever)
				{
					Analytics.Azure.OnGatherItem(item.info.shortname, item.amount, this, reciever, null);
					reciever.GiveItem(item, BaseEntity.GiveItemReason.ResourceHarvested);
				}
				else
				{
					item.Drop(base.transform.position + Vector3.up * 0.5f, Vector3.up, default(Quaternion));
				}
			}
			IL_DB:;
		}
		this.itemList = null;
		if (this.pickupEffect.isValid)
		{
			Effect.server.Run(this.pickupEffect.resourcePath, base.transform.position, base.transform.up, null, false);
		}
		RandomItemDispenser randomItemDispenser = PrefabAttribute.server.Find<RandomItemDispenser>(this.prefabID);
		if (randomItemDispenser != null)
		{
			randomItemDispenser.DistributeItems(reciever, base.transform.position);
		}
		base.Kill(BaseNetworkable.DestroyMode.None);
	}

	// Token: 0x060009C6 RID: 2502 RVA: 0x0005C31C File Offset: 0x0005A51C
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.MaxDistance(3f)]
	public void Pickup(BaseEntity.RPCMessage msg)
	{
		if (!msg.player.CanInteract())
		{
			return;
		}
		this.DoPickup(msg.player, false);
	}

	// Token: 0x060009C7 RID: 2503 RVA: 0x0005C339 File Offset: 0x0005A539
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.MaxDistance(3f)]
	public void PickupEat(BaseEntity.RPCMessage msg)
	{
		if (!msg.player.CanInteract())
		{
			return;
		}
		this.DoPickup(msg.player, true);
	}

	// Token: 0x060009C8 RID: 2504 RVA: 0x0005C356 File Offset: 0x0005A556
	public override void PreProcess(IPrefabProcessor preProcess, GameObject rootObj, string name, bool serverside, bool clientside, bool bundling)
	{
		base.PreProcess(preProcess, rootObj, name, serverside, clientside, bundling);
		if (serverside)
		{
			preProcess.RemoveComponent(base.GetComponent<Collider>());
		}
	}
}
