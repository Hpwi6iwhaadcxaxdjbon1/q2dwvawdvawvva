using System;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x020000CC RID: 204
public class SleepingBagCamper : global::SleepingBag
{
	// Token: 0x04000B63 RID: 2915
	public EntityRef<BaseVehicleSeat> AssociatedSeat;

	// Token: 0x06001252 RID: 4690 RVA: 0x0009436C File Offset: 0x0009256C
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("SleepingBagCamper.OnRpcMessage", 0))
		{
			if (rpc == 2177887503U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - ServerClearBed ");
				}
				using (TimeWarning.New("ServerClearBed", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.MaxDistance.Test(2177887503U, "ServerClearBed", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage msg2 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.ServerClearBed(msg2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in ServerClearBed");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06001253 RID: 4691 RVA: 0x000944D4 File Offset: 0x000926D4
	public override void ServerInit()
	{
		base.ServerInit();
		base.SetFlag(global::BaseEntity.Flags.Reserved3, true, false, true);
	}

	// Token: 0x06001254 RID: 4692 RVA: 0x000944EC File Offset: 0x000926EC
	protected override void PostPlayerSpawn(global::BasePlayer p)
	{
		base.PostPlayerSpawn(p);
		BaseVehicleSeat baseVehicleSeat = this.AssociatedSeat.Get(base.isServer);
		if (baseVehicleSeat != null)
		{
			p.EndSleeping();
			baseVehicleSeat.MountPlayer(p);
		}
	}

	// Token: 0x06001255 RID: 4693 RVA: 0x00094528 File Offset: 0x00092728
	public void SetSeat(BaseVehicleSeat seat, bool sendNetworkUpdate = false)
	{
		this.AssociatedSeat.Set(seat);
		if (sendNetworkUpdate)
		{
			base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
		}
	}

	// Token: 0x06001256 RID: 4694 RVA: 0x00094540 File Offset: 0x00092740
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		if (!info.forDisk)
		{
			info.msg.sleepingBagCamper = Facepunch.Pool.Get<ProtoBuf.SleepingBagCamper>();
			info.msg.sleepingBagCamper.seatID = this.AssociatedSeat.uid;
		}
	}

	// Token: 0x06001257 RID: 4695 RVA: 0x0009457C File Offset: 0x0009277C
	public override bool IsOccupied()
	{
		return (this.AssociatedSeat.IsValid(base.isServer) && this.AssociatedSeat.Get(base.isServer).AnyMounted()) || WaterLevel.Test(base.transform.position, true, null);
	}

	// Token: 0x06001258 RID: 4696 RVA: 0x000945C8 File Offset: 0x000927C8
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.MaxDistance(3f)]
	public void ServerClearBed(global::BaseEntity.RPCMessage msg)
	{
		global::BasePlayer player = msg.player;
		if (player == null || !this.AssociatedSeat.IsValid(base.isServer) || this.AssociatedSeat.Get(base.isServer).GetMounted() != player)
		{
			return;
		}
		global::SleepingBag.RemoveBagForPlayer(this, this.deployerUserID);
		this.deployerUserID = 0UL;
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}
}
