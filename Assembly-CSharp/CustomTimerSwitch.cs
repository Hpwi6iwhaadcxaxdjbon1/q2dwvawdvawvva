using System;
using ConVar;
using Network;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x02000063 RID: 99
public class CustomTimerSwitch : TimerSwitch
{
	// Token: 0x040006B9 RID: 1721
	public GameObjectRef timerPanelPrefab;

	// Token: 0x06000A26 RID: 2598 RVA: 0x0005E20C File Offset: 0x0005C40C
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("CustomTimerSwitch.OnRpcMessage", 0))
		{
			if (rpc == 1019813162U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - SERVER_SetTime ");
				}
				using (TimeWarning.New("SERVER_SetTime", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.IsVisible.Test(1019813162U, "SERVER_SetTime", this, player, 3f))
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
							this.SERVER_SetTime(msg2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in SERVER_SetTime");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000A27 RID: 2599 RVA: 0x0005E374 File Offset: 0x0005C574
	public override void UpdateFromInput(int inputAmount, int inputSlot)
	{
		base.UpdateFromInput(inputAmount, inputSlot);
		if (inputAmount > 0 && inputSlot == 1)
		{
			base.SwitchPressed();
		}
	}

	// Token: 0x06000A28 RID: 2600 RVA: 0x0005E38C File Offset: 0x0005C58C
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.IsVisible(3f)]
	public void SERVER_SetTime(BaseEntity.RPCMessage msg)
	{
		if (!this.CanPlayerAdmin(msg.player))
		{
			return;
		}
		float timerLength = msg.read.Float();
		this.timerLength = timerLength;
		base.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x06000A29 RID: 2601 RVA: 0x0005E130 File Offset: 0x0005C330
	public bool CanPlayerAdmin(BasePlayer player)
	{
		return player != null && player.CanBuild() && !base.IsOn();
	}
}
