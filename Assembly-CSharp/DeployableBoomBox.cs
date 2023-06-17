using System;
using ConVar;
using Network;
using Rust;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x02000064 RID: 100
public class DeployableBoomBox : ContainerIOEntity, ICassettePlayer, IAudioConnectionSource
{
	// Token: 0x040006BA RID: 1722
	public BoomBox BoxController;

	// Token: 0x040006BB RID: 1723
	public int PowerUsageWhilePlaying = 10;

	// Token: 0x040006BC RID: 1724
	public const int MaxBacktrackHopsClient = 30;

	// Token: 0x040006BD RID: 1725
	public bool IsStatic;

	// Token: 0x06000A2B RID: 2603 RVA: 0x0005E3CC File Offset: 0x0005C5CC
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("DeployableBoomBox.OnRpcMessage", 0))
		{
			if (rpc == 1918716764U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - Server_UpdateRadioIP ");
				}
				using (TimeWarning.New("Server_UpdateRadioIP", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.CallsPerSecond.Test(1918716764U, "Server_UpdateRadioIP", this, player, 2UL))
						{
							return true;
						}
						if (!BaseEntity.RPC_Server.IsVisible.Test(1918716764U, "Server_UpdateRadioIP", this, player, 3f))
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
							this.Server_UpdateRadioIP(msg2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in Server_UpdateRadioIP");
					}
				}
				return true;
			}
			if (rpc == 1785864031U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - ServerTogglePlay ");
				}
				using (TimeWarning.New("ServerTogglePlay", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.IsVisible.Test(1785864031U, "ServerTogglePlay", this, player, 3f))
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
							this.ServerTogglePlay(msg3);
						}
					}
					catch (Exception exception2)
					{
						Debug.LogException(exception2);
						player.Kick("RPC Error in ServerTogglePlay");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x17000106 RID: 262
	// (get) Token: 0x06000A2C RID: 2604 RVA: 0x000037E7 File Offset: 0x000019E7
	public BaseEntity ToBaseEntity
	{
		get
		{
			return this;
		}
	}

	// Token: 0x06000A2D RID: 2605 RVA: 0x000037E7 File Offset: 0x000019E7
	public IOEntity ToEntity()
	{
		return this;
	}

	// Token: 0x06000A2E RID: 2606 RVA: 0x0005E6E8 File Offset: 0x0005C8E8
	public override int ConsumptionAmount()
	{
		if (!base.IsOn())
		{
			return 0;
		}
		return this.PowerUsageWhilePlaying;
	}

	// Token: 0x06000A2F RID: 2607 RVA: 0x0005E6E8 File Offset: 0x0005C8E8
	public override int DesiredPower()
	{
		if (!base.IsOn())
		{
			return 0;
		}
		return this.PowerUsageWhilePlaying;
	}

	// Token: 0x06000A30 RID: 2608 RVA: 0x0005E6FC File Offset: 0x0005C8FC
	public override void UpdateHasPower(int inputAmount, int inputSlot)
	{
		if (inputSlot == 0)
		{
			base.UpdateHasPower(inputAmount, inputSlot);
			if (!this.IsPowered() && base.IsOn())
			{
				this.BoxController.ServerTogglePlay(false);
			}
			return;
		}
		if (this.IsPowered() && !base.IsConnectedToAnySlot(this, inputSlot, IOEntity.backtracking, false))
		{
			this.BoxController.ServerTogglePlay(inputAmount > 0);
		}
	}

	// Token: 0x06000A31 RID: 2609 RVA: 0x0005E758 File Offset: 0x0005C958
	public override void ServerInit()
	{
		base.ServerInit();
		base.inventory.canAcceptItem = new Func<Item, int, bool>(this.ItemFilter);
		this.BoxController.HurtCallback = new Action<float>(this.HurtCallback);
		if (this.IsStatic)
		{
			base.SetFlag(BaseEntity.Flags.Reserved8, true, false, true);
		}
	}

	// Token: 0x06000A32 RID: 2610 RVA: 0x0005E7B0 File Offset: 0x0005C9B0
	private bool ItemFilter(Item item, int count)
	{
		ItemDefinition[] validCassettes = this.BoxController.ValidCassettes;
		for (int i = 0; i < validCassettes.Length; i++)
		{
			if (validCassettes[i] == item.info)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06000A33 RID: 2611 RVA: 0x00057F12 File Offset: 0x00056112
	public override int GetPassthroughAmount(int outputSlot = 0)
	{
		if (!base.IsOn())
		{
			return 0;
		}
		return base.GetPassthroughAmount(outputSlot);
	}

	// Token: 0x06000A34 RID: 2612 RVA: 0x0005E7EA File Offset: 0x0005C9EA
	public override int CalculateCurrentEnergy(int inputAmount, int inputSlot)
	{
		if (inputSlot != 0)
		{
			return this.currentEnergy;
		}
		return base.CalculateCurrentEnergy(inputAmount, inputSlot);
	}

	// Token: 0x06000A35 RID: 2613 RVA: 0x0005E7FE File Offset: 0x0005C9FE
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.IsVisible(3f)]
	public void ServerTogglePlay(BaseEntity.RPCMessage msg)
	{
		this.BoxController.ServerTogglePlay(msg);
	}

	// Token: 0x06000A36 RID: 2614 RVA: 0x0005E80C File Offset: 0x0005CA0C
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.CallsPerSecond(2UL)]
	[BaseEntity.RPC_Server.IsVisible(3f)]
	private void Server_UpdateRadioIP(BaseEntity.RPCMessage msg)
	{
		this.BoxController.Server_UpdateRadioIP(msg);
	}

	// Token: 0x06000A37 RID: 2615 RVA: 0x0005E81A File Offset: 0x0005CA1A
	public override void Save(BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		this.BoxController.Save(info);
	}

	// Token: 0x06000A38 RID: 2616 RVA: 0x0005E82F File Offset: 0x0005CA2F
	public bool ClearRadioByUserId(ulong id)
	{
		return this.BoxController.ClearRadioByUserId(id);
	}

	// Token: 0x06000A39 RID: 2617 RVA: 0x0005E83D File Offset: 0x0005CA3D
	public void OnCassetteInserted(Cassette c)
	{
		this.BoxController.OnCassetteInserted(c);
	}

	// Token: 0x06000A3A RID: 2618 RVA: 0x0005E84B File Offset: 0x0005CA4B
	public void OnCassetteRemoved(Cassette c)
	{
		this.BoxController.OnCassetteRemoved(c);
	}

	// Token: 0x06000A3B RID: 2619 RVA: 0x0005E859 File Offset: 0x0005CA59
	public void HurtCallback(float amount)
	{
		base.Hurt(amount, DamageType.Decay, null, true);
	}

	// Token: 0x06000A3C RID: 2620 RVA: 0x0005E866 File Offset: 0x0005CA66
	public override void Load(BaseNetworkable.LoadInfo info)
	{
		this.BoxController.Load(info);
		base.Load(info);
		if (base.isServer && this.IsStatic)
		{
			base.SetFlag(BaseEntity.Flags.Reserved8, true, false, true);
		}
	}
}
