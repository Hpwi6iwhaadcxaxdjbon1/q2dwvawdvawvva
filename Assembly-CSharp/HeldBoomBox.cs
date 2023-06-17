using System;
using ConVar;
using Network;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x02000080 RID: 128
public class HeldBoomBox : HeldEntity, ICassettePlayer
{
	// Token: 0x040007C6 RID: 1990
	public BoomBox BoxController;

	// Token: 0x040007C7 RID: 1991
	public SwapKeycard cassetteSwapper;

	// Token: 0x06000C0B RID: 3083 RVA: 0x00069A68 File Offset: 0x00067C68
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("HeldBoomBox.OnRpcMessage", 0))
		{
			if (rpc == 1918716764U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
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
						if (!BaseEntity.RPC_Server.IsActiveItem.Test(1918716764U, "Server_UpdateRadioIP", this, player))
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
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - ServerTogglePlay ");
				}
				using (TimeWarning.New("ServerTogglePlay", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.IsActiveItem.Test(1785864031U, "ServerTogglePlay", this, player))
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

	// Token: 0x17000120 RID: 288
	// (get) Token: 0x06000C0C RID: 3084 RVA: 0x000037E7 File Offset: 0x000019E7
	public BaseEntity ToBaseEntity
	{
		get
		{
			return this;
		}
	}

	// Token: 0x06000C0D RID: 3085 RVA: 0x00069D7C File Offset: 0x00067F7C
	public override void ServerInit()
	{
		base.ServerInit();
		this.BoxController.HurtCallback = new Action<float>(this.HurtCallback);
	}

	// Token: 0x06000C0E RID: 3086 RVA: 0x00069D9B File Offset: 0x00067F9B
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.IsActiveItem]
	public void ServerTogglePlay(BaseEntity.RPCMessage msg)
	{
		this.BoxController.ServerTogglePlay(msg);
	}

	// Token: 0x06000C0F RID: 3087 RVA: 0x00069DA9 File Offset: 0x00067FA9
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.CallsPerSecond(2UL)]
	[BaseEntity.RPC_Server.IsActiveItem]
	private void Server_UpdateRadioIP(BaseEntity.RPCMessage msg)
	{
		this.BoxController.Server_UpdateRadioIP(msg);
	}

	// Token: 0x06000C10 RID: 3088 RVA: 0x00069DB7 File Offset: 0x00067FB7
	public override void Save(BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		this.BoxController.Save(info);
	}

	// Token: 0x06000C11 RID: 3089 RVA: 0x00069DCC File Offset: 0x00067FCC
	public void OnCassetteInserted(Cassette c)
	{
		this.BoxController.OnCassetteInserted(c);
	}

	// Token: 0x06000C12 RID: 3090 RVA: 0x00069DDA File Offset: 0x00067FDA
	public void OnCassetteRemoved(Cassette c)
	{
		this.BoxController.OnCassetteRemoved(c);
	}

	// Token: 0x06000C13 RID: 3091 RVA: 0x00069DE8 File Offset: 0x00067FE8
	public bool ClearRadioByUserId(ulong id)
	{
		return this.BoxController.ClearRadioByUserId(id);
	}

	// Token: 0x06000C14 RID: 3092 RVA: 0x00069DF6 File Offset: 0x00067FF6
	public void HurtCallback(float amount)
	{
		if (base.GetOwnerPlayer() != null && base.GetOwnerPlayer().IsSleeping())
		{
			this.BoxController.ServerTogglePlay(false);
			return;
		}
		Item item = this.GetItem();
		if (item == null)
		{
			return;
		}
		item.LoseCondition(amount);
	}

	// Token: 0x06000C15 RID: 3093 RVA: 0x00069E31 File Offset: 0x00068031
	public override void OnHeldChanged()
	{
		base.OnHeldChanged();
		if (base.IsDisabled())
		{
			this.BoxController.ServerTogglePlay(false);
		}
	}

	// Token: 0x06000C16 RID: 3094 RVA: 0x00069E4D File Offset: 0x0006804D
	public override void Load(BaseNetworkable.LoadInfo info)
	{
		this.BoxController.Load(info);
		base.Load(info);
	}
}
