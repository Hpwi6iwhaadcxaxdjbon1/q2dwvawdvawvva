using System;
using ConVar;
using Facepunch;
using Facepunch.Rust;
using Network;
using ProtoBuf;
using Rust;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x02000068 RID: 104
public class DieselEngine : StorageContainer
{
	// Token: 0x040006CA RID: 1738
	public GameObjectRef rumbleEffect;

	// Token: 0x040006CB RID: 1739
	public Transform rumbleOrigin;

	// Token: 0x040006CC RID: 1740
	public const global::BaseEntity.Flags Flag_HasFuel = global::BaseEntity.Flags.Reserved3;

	// Token: 0x040006CD RID: 1741
	public float runningTimePerFuelUnit = 120f;

	// Token: 0x040006CE RID: 1742
	private float cachedFuelTime;

	// Token: 0x040006CF RID: 1743
	private const float rumbleMaxDistSq = 100f;

	// Token: 0x040006D0 RID: 1744
	private const string EXCAVATOR_ACTIVATED_STAT = "excavator_activated";

	// Token: 0x06000A65 RID: 2661 RVA: 0x0005F740 File Offset: 0x0005D940
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("DieselEngine.OnRpcMessage", 0))
		{
			if (rpc == 578721460U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - EngineSwitch ");
				}
				using (TimeWarning.New("EngineSwitch", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.IsVisible.Test(578721460U, "EngineSwitch", this, player, 6f))
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
							this.EngineSwitch(msg2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in EngineSwitch");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000A66 RID: 2662 RVA: 0x0005F8A8 File Offset: 0x0005DAA8
	public override bool CanOpenLootPanel(global::BasePlayer player, string panelName)
	{
		return base.CanOpenLootPanel(player, panelName);
	}

	// Token: 0x06000A67 RID: 2663 RVA: 0x0005F8B4 File Offset: 0x0005DAB4
	public void FixedUpdate()
	{
		if (base.isClient)
		{
			return;
		}
		if (base.IsOn())
		{
			if (this.cachedFuelTime <= UnityEngine.Time.fixedDeltaTime && this.ConsumeFuelItem(1))
			{
				this.cachedFuelTime += this.runningTimePerFuelUnit;
			}
			this.cachedFuelTime -= UnityEngine.Time.fixedDeltaTime;
			if (this.cachedFuelTime <= 0f)
			{
				this.cachedFuelTime = 0f;
				this.EngineOff();
			}
		}
	}

	// Token: 0x06000A68 RID: 2664 RVA: 0x0005F92C File Offset: 0x0005DB2C
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsVisible(6f)]
	public void EngineSwitch(global::BaseEntity.RPCMessage msg)
	{
		if (msg.read.Bit())
		{
			if (this.GetFuelAmount() > 0)
			{
				this.EngineOn();
				if (GameInfo.HasAchievements && msg.player != null)
				{
					msg.player.stats.Add("excavator_activated", 1, global::Stats.All);
					msg.player.stats.Save(true);
					return;
				}
			}
		}
		else
		{
			this.EngineOff();
		}
	}

	// Token: 0x06000A69 RID: 2665 RVA: 0x0005F999 File Offset: 0x0005DB99
	public void TimedShutdown()
	{
		this.EngineOff();
	}

	// Token: 0x06000A6A RID: 2666 RVA: 0x0005F9A4 File Offset: 0x0005DBA4
	public bool ConsumeFuelItem(int amount = 1)
	{
		global::Item slot = base.inventory.GetSlot(0);
		if (slot == null || slot.amount < amount)
		{
			return false;
		}
		slot.UseItem(amount);
		Analytics.Azure.OnExcavatorConsumeFuel(slot, amount, this);
		this.UpdateHasFuelFlag();
		return true;
	}

	// Token: 0x06000A6B RID: 2667 RVA: 0x0005F9E4 File Offset: 0x0005DBE4
	public int GetFuelAmount()
	{
		global::Item slot = base.inventory.GetSlot(0);
		if (slot == null || slot.amount < 1)
		{
			return 0;
		}
		return slot.amount;
	}

	// Token: 0x06000A6C RID: 2668 RVA: 0x0005FA12 File Offset: 0x0005DC12
	public void UpdateHasFuelFlag()
	{
		base.SetFlag(global::BaseEntity.Flags.Reserved3, this.GetFuelAmount() > 0, false, true);
	}

	// Token: 0x06000A6D RID: 2669 RVA: 0x0005FA2A File Offset: 0x0005DC2A
	public override void PlayerStoppedLooting(global::BasePlayer player)
	{
		base.PlayerStoppedLooting(player);
		this.UpdateHasFuelFlag();
	}

	// Token: 0x06000A6E RID: 2670 RVA: 0x0005FA39 File Offset: 0x0005DC39
	public void EngineOff()
	{
		base.SetFlag(global::BaseEntity.Flags.On, false, false, true);
		base.BroadcastEntityMessage("DieselEngineOff", 20f, 1218652417);
	}

	// Token: 0x06000A6F RID: 2671 RVA: 0x0005FA5A File Offset: 0x0005DC5A
	public void EngineOn()
	{
		base.SetFlag(global::BaseEntity.Flags.On, true, false, true);
		base.BroadcastEntityMessage("DieselEngineOn", 20f, 1218652417);
	}

	// Token: 0x06000A70 RID: 2672 RVA: 0x0005FA7C File Offset: 0x0005DC7C
	public void RescheduleEngineShutdown()
	{
		float time = 120f;
		base.Invoke(new Action(this.TimedShutdown), time);
	}

	// Token: 0x06000A71 RID: 2673 RVA: 0x0005FAA2 File Offset: 0x0005DCA2
	public override void PostServerLoad()
	{
		base.PostServerLoad();
		if (base.IsOn())
		{
			base.BroadcastEntityMessage("DieselEngineOn", 20f, 1218652417);
			return;
		}
		base.BroadcastEntityMessage("DieselEngineOff", 20f, 1218652417);
	}

	// Token: 0x06000A72 RID: 2674 RVA: 0x0005FADD File Offset: 0x0005DCDD
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.ioEntity = Facepunch.Pool.Get<ProtoBuf.IOEntity>();
		info.msg.ioEntity.genericFloat1 = this.cachedFuelTime;
	}

	// Token: 0x06000A73 RID: 2675 RVA: 0x0005FB0C File Offset: 0x0005DD0C
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.ioEntity != null)
		{
			this.cachedFuelTime = info.msg.ioEntity.genericFloat1;
		}
	}

	// Token: 0x06000A74 RID: 2676 RVA: 0x00030086 File Offset: 0x0002E286
	public bool HasFuel()
	{
		return base.HasFlag(global::BaseEntity.Flags.Reserved3);
	}
}
