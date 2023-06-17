using System;
using ConVar;
using Network;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x0200005B RID: 91
public class CollectableEasterEgg : BaseEntity
{
	// Token: 0x0400067C RID: 1660
	public Transform artwork;

	// Token: 0x0400067D RID: 1661
	public float bounceRange = 0.2f;

	// Token: 0x0400067E RID: 1662
	public float bounceSpeed = 1f;

	// Token: 0x0400067F RID: 1663
	public GameObjectRef pickupEffect;

	// Token: 0x04000680 RID: 1664
	public ItemDefinition itemToGive;

	// Token: 0x04000681 RID: 1665
	private float lastPickupStartTime;

	// Token: 0x060009BE RID: 2494 RVA: 0x0005BA00 File Offset: 0x00059C00
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("CollectableEasterEgg.OnRpcMessage", 0))
		{
			if (rpc == 2436818324U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_PickUp ");
				}
				using (TimeWarning.New("RPC_PickUp", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.IsVisible.Test(2436818324U, "RPC_PickUp", this, player, 3f))
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
							this.RPC_PickUp(msg2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in RPC_PickUp");
					}
				}
				return true;
			}
			if (rpc == 2243088389U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_StartPickUp ");
				}
				using (TimeWarning.New("RPC_StartPickUp", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.IsVisible.Test(2243088389U, "RPC_StartPickUp", this, player, 3f))
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
							this.RPC_StartPickUp(msg3);
						}
					}
					catch (Exception exception2)
					{
						Debug.LogException(exception2);
						player.Kick("RPC Error in RPC_StartPickUp");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x060009BF RID: 2495 RVA: 0x0005BD00 File Offset: 0x00059F00
	public override void ServerInit()
	{
		int num = UnityEngine.Random.Range(0, 3);
		base.SetFlag(BaseEntity.Flags.Reserved1, num == 0, false, false);
		base.SetFlag(BaseEntity.Flags.Reserved2, num == 1, false, false);
		base.SetFlag(BaseEntity.Flags.Reserved3, num == 2, false, false);
		base.ServerInit();
	}

	// Token: 0x060009C0 RID: 2496 RVA: 0x0005BD4E File Offset: 0x00059F4E
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.IsVisible(3f)]
	public void RPC_StartPickUp(BaseEntity.RPCMessage msg)
	{
		if (msg.player == null)
		{
			return;
		}
		this.lastPickupStartTime = UnityEngine.Time.realtimeSinceStartup;
	}

	// Token: 0x060009C1 RID: 2497 RVA: 0x0005BD6C File Offset: 0x00059F6C
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.IsVisible(3f)]
	public void RPC_PickUp(BaseEntity.RPCMessage msg)
	{
		if (msg.player == null)
		{
			return;
		}
		float num = UnityEngine.Time.realtimeSinceStartup - this.lastPickupStartTime;
		if (!(msg.player.GetHeldEntity() as EasterBasket))
		{
			if (num > 2f)
			{
				return;
			}
			if (num < 0.8f)
			{
				return;
			}
		}
		if (EggHuntEvent.serverEvent)
		{
			if (!EggHuntEvent.serverEvent.IsEventActive())
			{
				return;
			}
			EggHuntEvent.serverEvent.EggCollected(msg.player);
			int iAmount = 1;
			msg.player.GiveItem(ItemManager.Create(this.itemToGive, iAmount, 0UL), BaseEntity.GiveItemReason.Generic);
		}
		Effect.server.Run(this.pickupEffect.resourcePath, base.transform.position + Vector3.up * 0.3f, Vector3.up, null, false);
		base.Kill(BaseNetworkable.DestroyMode.None);
	}
}
