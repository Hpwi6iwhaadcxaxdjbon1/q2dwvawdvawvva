using System;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x0200009F RID: 159
public class MobilePhone : global::HeldEntity
{
	// Token: 0x0400097A RID: 2426
	public PhoneController Controller;

	// Token: 0x06000E6A RID: 3690 RVA: 0x00079B1C File Offset: 0x00077D1C
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("MobilePhone.OnRpcMessage", 0))
		{
			if (rpc == 1529322558U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - AnswerPhone ");
				}
				using (TimeWarning.New("AnswerPhone", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.FromOwner.Test(1529322558U, "AnswerPhone", this, player))
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
							this.AnswerPhone(msg2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in AnswerPhone");
					}
				}
				return true;
			}
			if (rpc == 2754362156U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - ClearCurrentUser ");
				}
				using (TimeWarning.New("ClearCurrentUser", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.FromOwner.Test(2754362156U, "ClearCurrentUser", this, player))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage msg3 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.ClearCurrentUser(msg3);
						}
					}
					catch (Exception exception2)
					{
						Debug.LogException(exception2);
						player.Kick("RPC Error in ClearCurrentUser");
					}
				}
				return true;
			}
			if (rpc == 1095090232U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - InitiateCall ");
				}
				using (TimeWarning.New("InitiateCall", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.FromOwner.Test(1095090232U, "InitiateCall", this, player))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage msg4 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.InitiateCall(msg4);
						}
					}
					catch (Exception exception3)
					{
						Debug.LogException(exception3);
						player.Kick("RPC Error in InitiateCall");
					}
				}
				return true;
			}
			if (rpc == 2606442785U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - Server_AddSavedNumber ");
				}
				using (TimeWarning.New("Server_AddSavedNumber", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.CallsPerSecond.Test(2606442785U, "Server_AddSavedNumber", this, player, 5UL))
						{
							return true;
						}
						if (!global::BaseEntity.RPC_Server.FromOwner.Test(2606442785U, "Server_AddSavedNumber", this, player))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage msg5 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.Server_AddSavedNumber(msg5);
						}
					}
					catch (Exception exception4)
					{
						Debug.LogException(exception4);
						player.Kick("RPC Error in Server_AddSavedNumber");
					}
				}
				return true;
			}
			if (rpc == 1402406333U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - Server_RemoveSavedNumber ");
				}
				using (TimeWarning.New("Server_RemoveSavedNumber", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.CallsPerSecond.Test(1402406333U, "Server_RemoveSavedNumber", this, player, 5UL))
						{
							return true;
						}
						if (!global::BaseEntity.RPC_Server.FromOwner.Test(1402406333U, "Server_RemoveSavedNumber", this, player))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage msg6 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.Server_RemoveSavedNumber(msg6);
						}
					}
					catch (Exception exception5)
					{
						Debug.LogException(exception5);
						player.Kick("RPC Error in Server_RemoveSavedNumber");
					}
				}
				return true;
			}
			if (rpc == 2704491961U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - Server_RequestCurrentState ");
				}
				using (TimeWarning.New("Server_RequestCurrentState", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.FromOwner.Test(2704491961U, "Server_RequestCurrentState", this, player))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage msg7 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.Server_RequestCurrentState(msg7);
						}
					}
					catch (Exception exception6)
					{
						Debug.LogException(exception6);
						player.Kick("RPC Error in Server_RequestCurrentState");
					}
				}
				return true;
			}
			if (rpc == 942544266U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - Server_RequestPhoneDirectory ");
				}
				using (TimeWarning.New("Server_RequestPhoneDirectory", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.CallsPerSecond.Test(942544266U, "Server_RequestPhoneDirectory", this, player, 5UL))
						{
							return true;
						}
						if (!global::BaseEntity.RPC_Server.FromOwner.Test(942544266U, "Server_RequestPhoneDirectory", this, player))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage msg8 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.Server_RequestPhoneDirectory(msg8);
						}
					}
					catch (Exception exception7)
					{
						Debug.LogException(exception7);
						player.Kick("RPC Error in Server_RequestPhoneDirectory");
					}
				}
				return true;
			}
			if (rpc == 1240133378U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - ServerDeleteVoicemail ");
				}
				using (TimeWarning.New("ServerDeleteVoicemail", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.CallsPerSecond.Test(1240133378U, "ServerDeleteVoicemail", this, player, 5UL))
						{
							return true;
						}
						if (!global::BaseEntity.RPC_Server.FromOwner.Test(1240133378U, "ServerDeleteVoicemail", this, player))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage msg9 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.ServerDeleteVoicemail(msg9);
						}
					}
					catch (Exception exception8)
					{
						Debug.LogException(exception8);
						player.Kick("RPC Error in ServerDeleteVoicemail");
					}
				}
				return true;
			}
			if (rpc == 1221129498U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - ServerHangUp ");
				}
				using (TimeWarning.New("ServerHangUp", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.FromOwner.Test(1221129498U, "ServerHangUp", this, player))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage msg10 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.ServerHangUp(msg10);
						}
					}
					catch (Exception exception9)
					{
						Debug.LogException(exception9);
						player.Kick("RPC Error in ServerHangUp");
					}
				}
				return true;
			}
			if (rpc == 239260010U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - ServerPlayVoicemail ");
				}
				using (TimeWarning.New("ServerPlayVoicemail", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.CallsPerSecond.Test(239260010U, "ServerPlayVoicemail", this, player, 5UL))
						{
							return true;
						}
						if (!global::BaseEntity.RPC_Server.FromOwner.Test(239260010U, "ServerPlayVoicemail", this, player))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage msg11 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.ServerPlayVoicemail(msg11);
						}
					}
					catch (Exception exception10)
					{
						Debug.LogException(exception10);
						player.Kick("RPC Error in ServerPlayVoicemail");
					}
				}
				return true;
			}
			if (rpc == 189198880U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - ServerSendVoicemail ");
				}
				using (TimeWarning.New("ServerSendVoicemail", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.CallsPerSecond.Test(189198880U, "ServerSendVoicemail", this, player, 5UL))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage msg12 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.ServerSendVoicemail(msg12);
						}
					}
					catch (Exception exception11)
					{
						Debug.LogException(exception11);
						player.Kick("RPC Error in ServerSendVoicemail");
					}
				}
				return true;
			}
			if (rpc == 2760189029U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - ServerStopVoicemail ");
				}
				using (TimeWarning.New("ServerStopVoicemail", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.CallsPerSecond.Test(2760189029U, "ServerStopVoicemail", this, player, 5UL))
						{
							return true;
						}
						if (!global::BaseEntity.RPC_Server.FromOwner.Test(2760189029U, "ServerStopVoicemail", this, player))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage msg13 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.ServerStopVoicemail(msg13);
						}
					}
					catch (Exception exception12)
					{
						Debug.LogException(exception12);
						player.Kick("RPC Error in ServerStopVoicemail");
					}
				}
				return true;
			}
			if (rpc == 3900772076U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - SetCurrentUser ");
				}
				using (TimeWarning.New("SetCurrentUser", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.FromOwner.Test(3900772076U, "SetCurrentUser", this, player))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage currentUser = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.SetCurrentUser(currentUser);
						}
					}
					catch (Exception exception13)
					{
						Debug.LogException(exception13);
						player.Kick("RPC Error in SetCurrentUser");
					}
				}
				return true;
			}
			if (rpc == 2760249627U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - UpdatePhoneName ");
				}
				using (TimeWarning.New("UpdatePhoneName", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.CallsPerSecond.Test(2760249627U, "UpdatePhoneName", this, player, 5UL))
						{
							return true;
						}
						if (!global::BaseEntity.RPC_Server.FromOwner.Test(2760249627U, "UpdatePhoneName", this, player))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage msg14 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.UpdatePhoneName(msg14);
						}
					}
					catch (Exception exception14)
					{
						Debug.LogException(exception14);
						player.Kick("RPC Error in UpdatePhoneName");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000E6B RID: 3691 RVA: 0x0007AEE0 File Offset: 0x000790E0
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		if (info.msg.telephone == null)
		{
			info.msg.telephone = Facepunch.Pool.Get<ProtoBuf.Telephone>();
		}
		info.msg.telephone.phoneNumber = this.Controller.PhoneNumber;
		info.msg.telephone.phoneName = this.Controller.PhoneName;
		info.msg.telephone.lastNumber = this.Controller.lastDialedNumber;
		info.msg.telephone.savedNumbers = this.Controller.savedNumbers;
		if (!info.forDisk)
		{
			info.msg.telephone.usingPlayer = this.Controller.currentPlayerRef.uid;
		}
	}

	// Token: 0x06000E6C RID: 3692 RVA: 0x0007AFA5 File Offset: 0x000791A5
	public override void ServerInit()
	{
		base.ServerInit();
		this.Controller.ServerInit();
	}

	// Token: 0x06000E6D RID: 3693 RVA: 0x0007AFB8 File Offset: 0x000791B8
	public override void PostServerLoad()
	{
		base.PostServerLoad();
		this.Controller.PostServerLoad();
	}

	// Token: 0x06000E6E RID: 3694 RVA: 0x0007AFCB File Offset: 0x000791CB
	internal override void DoServerDestroy()
	{
		base.DoServerDestroy();
		this.Controller.DoServerDestroy();
	}

	// Token: 0x06000E6F RID: 3695 RVA: 0x0007AFDE File Offset: 0x000791DE
	public override void OnParentChanging(global::BaseEntity oldParent, global::BaseEntity newParent)
	{
		base.OnParentChanging(oldParent, newParent);
		this.Controller.OnParentChanged(newParent);
	}

	// Token: 0x06000E70 RID: 3696 RVA: 0x0007AFF4 File Offset: 0x000791F4
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.FromOwner]
	public void ClearCurrentUser(global::BaseEntity.RPCMessage msg)
	{
		this.Controller.ClearCurrentUser(msg);
	}

	// Token: 0x06000E71 RID: 3697 RVA: 0x0007B002 File Offset: 0x00079202
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.FromOwner]
	public void SetCurrentUser(global::BaseEntity.RPCMessage msg)
	{
		this.Controller.SetCurrentUser(msg);
	}

	// Token: 0x06000E72 RID: 3698 RVA: 0x0007B010 File Offset: 0x00079210
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.FromOwner]
	public void InitiateCall(global::BaseEntity.RPCMessage msg)
	{
		this.Controller.InitiateCall(msg);
	}

	// Token: 0x06000E73 RID: 3699 RVA: 0x0007B01E File Offset: 0x0007921E
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.FromOwner]
	public void AnswerPhone(global::BaseEntity.RPCMessage msg)
	{
		this.Controller.AnswerPhone(msg);
	}

	// Token: 0x06000E74 RID: 3700 RVA: 0x0007B02C File Offset: 0x0007922C
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.FromOwner]
	private void ServerHangUp(global::BaseEntity.RPCMessage msg)
	{
		this.Controller.ServerHangUp(msg);
	}

	// Token: 0x06000E75 RID: 3701 RVA: 0x0007B03A File Offset: 0x0007923A
	public override void DestroyShared()
	{
		base.DestroyShared();
		this.Controller.DestroyShared();
	}

	// Token: 0x06000E76 RID: 3702 RVA: 0x0007B04D File Offset: 0x0007924D
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.FromOwner]
	[global::BaseEntity.RPC_Server.CallsPerSecond(5UL)]
	public void UpdatePhoneName(global::BaseEntity.RPCMessage msg)
	{
		this.Controller.UpdatePhoneName(msg);
	}

	// Token: 0x06000E77 RID: 3703 RVA: 0x0007B05B File Offset: 0x0007925B
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.FromOwner]
	[global::BaseEntity.RPC_Server.CallsPerSecond(5UL)]
	public void Server_RequestPhoneDirectory(global::BaseEntity.RPCMessage msg)
	{
		this.Controller.Server_RequestPhoneDirectory(msg);
	}

	// Token: 0x06000E78 RID: 3704 RVA: 0x0007B069 File Offset: 0x00079269
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.FromOwner]
	[global::BaseEntity.RPC_Server.CallsPerSecond(5UL)]
	public void Server_AddSavedNumber(global::BaseEntity.RPCMessage msg)
	{
		this.Controller.Server_AddSavedNumber(msg);
	}

	// Token: 0x06000E79 RID: 3705 RVA: 0x0007B077 File Offset: 0x00079277
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.FromOwner]
	[global::BaseEntity.RPC_Server.CallsPerSecond(5UL)]
	public void Server_RemoveSavedNumber(global::BaseEntity.RPCMessage msg)
	{
		this.Controller.Server_RemoveSavedNumber(msg);
	}

	// Token: 0x06000E7A RID: 3706 RVA: 0x0007B085 File Offset: 0x00079285
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.FromOwner]
	public void Server_RequestCurrentState(global::BaseEntity.RPCMessage msg)
	{
		this.Controller.SetPhoneStateWithPlayer(this.Controller.serverState);
	}

	// Token: 0x06000E7B RID: 3707 RVA: 0x0007B09D File Offset: 0x0007929D
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.CallsPerSecond(5UL)]
	public void ServerSendVoicemail(global::BaseEntity.RPCMessage msg)
	{
		this.Controller.ServerSendVoicemail(msg);
	}

	// Token: 0x06000E7C RID: 3708 RVA: 0x0007B0AB File Offset: 0x000792AB
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.CallsPerSecond(5UL)]
	[global::BaseEntity.RPC_Server.FromOwner]
	public void ServerPlayVoicemail(global::BaseEntity.RPCMessage msg)
	{
		this.Controller.ServerPlayVoicemail(msg);
	}

	// Token: 0x06000E7D RID: 3709 RVA: 0x0007B0B9 File Offset: 0x000792B9
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.CallsPerSecond(5UL)]
	[global::BaseEntity.RPC_Server.FromOwner]
	public void ServerStopVoicemail(global::BaseEntity.RPCMessage msg)
	{
		this.Controller.ServerStopVoicemail(msg);
	}

	// Token: 0x06000E7E RID: 3710 RVA: 0x0007B0C7 File Offset: 0x000792C7
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.CallsPerSecond(5UL)]
	[global::BaseEntity.RPC_Server.FromOwner]
	public void ServerDeleteVoicemail(global::BaseEntity.RPCMessage msg)
	{
		this.Controller.ServerDeleteVoicemail(msg);
	}

	// Token: 0x06000E7F RID: 3711 RVA: 0x0007B0D8 File Offset: 0x000792D8
	public void ToggleRinging(bool state)
	{
		MobileInventoryEntity associatedEntity = ItemModAssociatedEntity<MobileInventoryEntity>.GetAssociatedEntity(this.GetItem(), true);
		if (associatedEntity != null)
		{
			associatedEntity.ToggleRinging(state);
		}
	}

	// Token: 0x06000E80 RID: 3712 RVA: 0x0007B104 File Offset: 0x00079304
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		ProtoBuf.Entity msg = info.msg;
		if (((msg != null) ? msg.telephone : null) != null)
		{
			this.Controller.PhoneNumber = info.msg.telephone.phoneNumber;
			this.Controller.PhoneName = info.msg.telephone.phoneName;
			this.Controller.lastDialedNumber = info.msg.telephone.lastNumber;
			this.Controller.currentPlayerRef.uid = info.msg.telephone.usingPlayer;
			PhoneDirectory savedNumbers = this.Controller.savedNumbers;
			if (savedNumbers != null)
			{
				savedNumbers.ResetToPool();
			}
			this.Controller.savedNumbers = info.msg.telephone.savedNumbers;
			if (this.Controller.savedNumbers != null)
			{
				this.Controller.savedNumbers.ShouldPool = false;
			}
		}
	}

	// Token: 0x06000E81 RID: 3713 RVA: 0x0007B1F0 File Offset: 0x000793F0
	public override void OnFlagsChanged(global::BaseEntity.Flags old, global::BaseEntity.Flags next)
	{
		base.OnFlagsChanged(old, next);
		if (base.isServer && old.HasFlag(global::BaseEntity.Flags.Busy) != next.HasFlag(global::BaseEntity.Flags.Busy))
		{
			if (next.HasFlag(global::BaseEntity.Flags.Busy))
			{
				if (!base.IsInvoking(new Action(this.Controller.WatchForDisconnects)))
				{
					base.InvokeRepeating(new Action(this.Controller.WatchForDisconnects), 0f, 0.1f);
				}
			}
			else
			{
				base.CancelInvoke(new Action(this.Controller.WatchForDisconnects));
			}
		}
		this.Controller.OnFlagsChanged(old, next);
	}
}
