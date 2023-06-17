using System;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x020000ED RID: 237
public class WaterWell : LiquidContainer
{
	// Token: 0x04000D34 RID: 3380
	public Animator animator;

	// Token: 0x04000D35 RID: 3381
	private const global::BaseEntity.Flags Pumping = global::BaseEntity.Flags.Reserved2;

	// Token: 0x04000D36 RID: 3382
	private const global::BaseEntity.Flags WaterFlow = global::BaseEntity.Flags.Reserved3;

	// Token: 0x04000D37 RID: 3383
	public float caloriesPerPump = 5f;

	// Token: 0x04000D38 RID: 3384
	public float pressurePerPump = 0.2f;

	// Token: 0x04000D39 RID: 3385
	public float pressureForProduction = 1f;

	// Token: 0x04000D3A RID: 3386
	public float currentPressure;

	// Token: 0x04000D3B RID: 3387
	public int waterPerPump = 50;

	// Token: 0x04000D3C RID: 3388
	public GameObject waterLevelObj;

	// Token: 0x04000D3D RID: 3389
	public float waterLevelObjFullOffset;

	// Token: 0x060014EA RID: 5354 RVA: 0x000A5554 File Offset: 0x000A3754
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("WaterWell.OnRpcMessage", 0))
		{
			if (rpc == 2538739344U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_Pump ");
				}
				using (TimeWarning.New("RPC_Pump", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.MaxDistance.Test(2538739344U, "RPC_Pump", this, player, 3f))
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
							this.RPC_Pump(msg2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in RPC_Pump");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x060014EB RID: 5355 RVA: 0x000A56BC File Offset: 0x000A38BC
	public override void ServerInit()
	{
		base.ServerInit();
		base.SetFlag(global::BaseEntity.Flags.Reserved2, false, false, true);
		base.SetFlag(global::BaseEntity.Flags.Reserved3, false, false, true);
	}

	// Token: 0x060014EC RID: 5356 RVA: 0x000A56E0 File Offset: 0x000A38E0
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.MaxDistance(3f)]
	public void RPC_Pump(global::BaseEntity.RPCMessage msg)
	{
		global::BasePlayer player = msg.player;
		if (player == null || player.IsDead() || player.IsSleeping())
		{
			return;
		}
		if (player.metabolism.calories.value < this.caloriesPerPump)
		{
			return;
		}
		if (base.HasFlag(global::BaseEntity.Flags.Reserved2))
		{
			return;
		}
		base.SetFlag(global::BaseEntity.Flags.Reserved2, true, false, true);
		player.metabolism.calories.value -= this.caloriesPerPump;
		player.metabolism.SendChangesToClient();
		this.currentPressure = Mathf.Clamp01(this.currentPressure + this.pressurePerPump);
		base.Invoke(new Action(this.StopPump), 1.8f);
		if (this.currentPressure >= 0f)
		{
			base.CancelInvoke(new Action(this.Produce));
			base.Invoke(new Action(this.Produce), 1f);
		}
		base.SendNetworkUpdateImmediate(false);
	}

	// Token: 0x060014ED RID: 5357 RVA: 0x000A57D7 File Offset: 0x000A39D7
	public void StopPump()
	{
		base.SetFlag(global::BaseEntity.Flags.Reserved2, false, false, true);
		base.SendNetworkUpdateImmediate(false);
	}

	// Token: 0x060014EE RID: 5358 RVA: 0x000A57EE File Offset: 0x000A39EE
	public override void OnItemAddedOrRemoved(global::Item item, bool added)
	{
		base.OnItemAddedOrRemoved(item, added);
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x060014EF RID: 5359 RVA: 0x000A57FF File Offset: 0x000A39FF
	public void Produce()
	{
		base.inventory.AddItem(this.defaultLiquid, this.waterPerPump, 0UL, global::ItemContainer.LimitStack.Existing);
		base.SetFlag(global::BaseEntity.Flags.Reserved3, true, false, true);
		this.ScheduleTapOff();
		base.SendNetworkUpdateImmediate(false);
	}

	// Token: 0x060014F0 RID: 5360 RVA: 0x000A5836 File Offset: 0x000A3A36
	public void ScheduleTapOff()
	{
		base.CancelInvoke(new Action(this.TapOff));
		base.Invoke(new Action(this.TapOff), 1f);
	}

	// Token: 0x060014F1 RID: 5361 RVA: 0x000A5861 File Offset: 0x000A3A61
	private void TapOff()
	{
		base.SetFlag(global::BaseEntity.Flags.Reserved3, false, false, true);
	}

	// Token: 0x060014F2 RID: 5362 RVA: 0x000A5874 File Offset: 0x000A3A74
	public void ReducePressure()
	{
		float num = UnityEngine.Random.Range(0.1f, 0.2f);
		this.currentPressure = Mathf.Clamp01(this.currentPressure - num);
	}

	// Token: 0x060014F3 RID: 5363 RVA: 0x000A58A4 File Offset: 0x000A3AA4
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.waterwell = Facepunch.Pool.Get<ProtoBuf.WaterWell>();
		info.msg.waterwell.pressure = this.currentPressure;
		info.msg.waterwell.waterLevel = this.GetWaterAmount();
	}

	// Token: 0x060014F4 RID: 5364 RVA: 0x000A58F4 File Offset: 0x000A3AF4
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.waterwell != null)
		{
			this.currentPressure = info.msg.waterwell.pressure;
		}
	}

	// Token: 0x060014F5 RID: 5365 RVA: 0x000A5920 File Offset: 0x000A3B20
	public float GetWaterAmount()
	{
		if (!base.isServer)
		{
			return 0f;
		}
		global::Item slot = base.inventory.GetSlot(0);
		if (slot == null)
		{
			return 0f;
		}
		return (float)slot.amount;
	}
}
