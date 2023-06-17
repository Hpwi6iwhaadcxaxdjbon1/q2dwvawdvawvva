using System;
using ConVar;
using Network;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x020000D0 RID: 208
public class SmartSwitch : AppIOEntity
{
	// Token: 0x04000B91 RID: 2961
	[Header("Smart Switch")]
	public Animator ReceiverAnimator;

	// Token: 0x06001287 RID: 4743 RVA: 0x00095D3C File Offset: 0x00093F3C
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("SmartSwitch.OnRpcMessage", 0))
		{
			if (rpc == 2810053005U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - ToggleSwitch ");
				}
				using (TimeWarning.New("ToggleSwitch", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.CallsPerSecond.Test(2810053005U, "ToggleSwitch", this, player, 3UL))
						{
							return true;
						}
						if (!global::BaseEntity.RPC_Server.IsVisible.Test(2810053005U, "ToggleSwitch", this, player, 3f))
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
							this.ToggleSwitch(msg2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in ToggleSwitch");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06001288 RID: 4744 RVA: 0x00007641 File Offset: 0x00005841
	public override bool WantsPower()
	{
		return base.IsOn();
	}

	// Token: 0x170001AB RID: 427
	// (get) Token: 0x06001289 RID: 4745 RVA: 0x0000441C File Offset: 0x0000261C
	public override AppEntityType Type
	{
		get
		{
			return AppEntityType.Switch;
		}
	}

	// Token: 0x0600128A RID: 4746 RVA: 0x000627AF File Offset: 0x000609AF
	public override void ServerInit()
	{
		base.ServerInit();
		base.SetFlag(global::BaseEntity.Flags.Busy, false, false, true);
	}

	// Token: 0x0600128B RID: 4747 RVA: 0x0006275C File Offset: 0x0006095C
	public override int ConsumptionAmount()
	{
		if (!base.IsOn())
		{
			return 0;
		}
		return 1;
	}

	// Token: 0x0600128C RID: 4748 RVA: 0x00062769 File Offset: 0x00060969
	public override void ResetIOState()
	{
		base.SetFlag(global::BaseEntity.Flags.On, false, false, true);
	}

	// Token: 0x0600128D RID: 4749 RVA: 0x00062775 File Offset: 0x00060975
	public override int GetPassthroughAmount(int outputSlot = 0)
	{
		if (!base.IsOn())
		{
			return 0;
		}
		return this.GetCurrentEnergy();
	}

	// Token: 0x0600128E RID: 4750 RVA: 0x00095EFC File Offset: 0x000940FC
	public override void IOStateChanged(int inputAmount, int inputSlot)
	{
		if (inputSlot == 1 && inputAmount > 0)
		{
			this.SetSwitch(true);
		}
		if (inputSlot == 2 && inputAmount > 0)
		{
			this.SetSwitch(false);
		}
		base.IOStateChanged(inputAmount, inputSlot);
	}

	// Token: 0x0600128F RID: 4751 RVA: 0x00095F24 File Offset: 0x00094124
	public void SetSwitch(bool wantsOn)
	{
		if (wantsOn == base.IsOn())
		{
			return;
		}
		base.SetFlag(global::BaseEntity.Flags.On, wantsOn, false, true);
		base.SetFlag(global::BaseEntity.Flags.Busy, true, false, true);
		base.Invoke(new Action(this.Unbusy), 0.5f);
		base.SendNetworkUpdateImmediate(false);
		this.MarkDirty();
		base.BroadcastValueChange();
	}

	// Token: 0x06001290 RID: 4752 RVA: 0x00095F7D File Offset: 0x0009417D
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsVisible(3f)]
	[global::BaseEntity.RPC_Server.CallsPerSecond(3UL)]
	public void ToggleSwitch(global::BaseEntity.RPCMessage msg)
	{
		if (!SmartSwitch.PlayerCanToggle(msg.player))
		{
			return;
		}
		this.SetSwitch(!base.IsOn());
	}

	// Token: 0x170001AC RID: 428
	// (get) Token: 0x06001291 RID: 4753 RVA: 0x00007641 File Offset: 0x00005841
	// (set) Token: 0x06001292 RID: 4754 RVA: 0x00095F9C File Offset: 0x0009419C
	public override bool Value
	{
		get
		{
			return base.IsOn();
		}
		set
		{
			this.SetSwitch(value);
		}
	}

	// Token: 0x06001293 RID: 4755 RVA: 0x0006282C File Offset: 0x00060A2C
	public void Unbusy()
	{
		base.SetFlag(global::BaseEntity.Flags.Busy, false, false, true);
	}

	// Token: 0x06001294 RID: 4756 RVA: 0x00095FA5 File Offset: 0x000941A5
	private static bool PlayerCanToggle(global::BasePlayer player)
	{
		return player != null && player.CanBuild();
	}
}
