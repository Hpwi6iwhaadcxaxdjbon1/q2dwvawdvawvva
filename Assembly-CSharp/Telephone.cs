using System;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x020000DD RID: 221
public class Telephone : ContainerIOEntity, ICassettePlayer
{
	// Token: 0x04000C1F RID: 3103
	public const int MaxPhoneNameLength = 20;

	// Token: 0x04000C20 RID: 3104
	public const int MaxSavedNumbers = 10;

	// Token: 0x04000C21 RID: 3105
	public Transform PhoneHotspot;

	// Token: 0x04000C22 RID: 3106
	public Transform AnsweringMachineHotspot;

	// Token: 0x04000C23 RID: 3107
	public Transform[] HandsetRoots;

	// Token: 0x04000C24 RID: 3108
	public ItemDefinition[] ValidCassettes;

	// Token: 0x04000C25 RID: 3109
	public Transform ParentedHandsetTransform;

	// Token: 0x04000C26 RID: 3110
	public LineRenderer CableLineRenderer;

	// Token: 0x04000C27 RID: 3111
	public Transform CableStartPoint;

	// Token: 0x04000C28 RID: 3112
	public Transform CableEndPoint;

	// Token: 0x04000C29 RID: 3113
	public float LineDroopAmount = 0.25f;

	// Token: 0x04000C2B RID: 3115
	public PhoneController Controller;

	// Token: 0x0600136B RID: 4971 RVA: 0x0009B098 File Offset: 0x00099298
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("Telephone.OnRpcMessage", 0))
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
						if (!global::BaseEntity.RPC_Server.MaxDistance.Test(1529322558U, "AnswerPhone", this, player, 3f))
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
						if (!global::BaseEntity.RPC_Server.MaxDistance.Test(2754362156U, "ClearCurrentUser", this, player, 9f))
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
						if (!global::BaseEntity.RPC_Server.MaxDistance.Test(1095090232U, "InitiateCall", this, player, 3f))
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
						if (!global::BaseEntity.RPC_Server.MaxDistance.Test(2606442785U, "Server_AddSavedNumber", this, player, 3f))
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
						if (!global::BaseEntity.RPC_Server.MaxDistance.Test(1402406333U, "Server_RemoveSavedNumber", this, player, 3f))
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
						if (!global::BaseEntity.RPC_Server.MaxDistance.Test(942544266U, "Server_RequestPhoneDirectory", this, player, 3f))
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
							this.Server_RequestPhoneDirectory(msg7);
						}
					}
					catch (Exception exception6)
					{
						Debug.LogException(exception6);
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
						if (!global::BaseEntity.RPC_Server.IsVisible.Test(1240133378U, "ServerDeleteVoicemail", this, player, 3f))
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
							this.ServerDeleteVoicemail(msg8);
						}
					}
					catch (Exception exception7)
					{
						Debug.LogException(exception7);
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
							this.ServerHangUp(msg9);
						}
					}
					catch (Exception exception8)
					{
						Debug.LogException(exception8);
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
						if (!global::BaseEntity.RPC_Server.IsVisible.Test(239260010U, "ServerPlayVoicemail", this, player, 3f))
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
							this.ServerPlayVoicemail(msg10);
						}
					}
					catch (Exception exception9)
					{
						Debug.LogException(exception9);
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
							global::BaseEntity.RPCMessage msg11 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.ServerSendVoicemail(msg11);
						}
					}
					catch (Exception exception10)
					{
						Debug.LogException(exception10);
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
						if (!global::BaseEntity.RPC_Server.IsVisible.Test(2760189029U, "ServerStopVoicemail", this, player, 3f))
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
							this.ServerStopVoicemail(msg12);
						}
					}
					catch (Exception exception11)
					{
						Debug.LogException(exception11);
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
						if (!global::BaseEntity.RPC_Server.MaxDistance.Test(3900772076U, "SetCurrentUser", this, player, 3f))
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
					catch (Exception exception12)
					{
						Debug.LogException(exception12);
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
						if (!global::BaseEntity.RPC_Server.MaxDistance.Test(2760249627U, "UpdatePhoneName", this, player, 3f))
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
							this.UpdatePhoneName(msg13);
						}
					}
					catch (Exception exception13)
					{
						Debug.LogException(exception13);
						player.Kick("RPC Error in UpdatePhoneName");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x170001C1 RID: 449
	// (get) Token: 0x0600136C RID: 4972 RVA: 0x0009C2F4 File Offset: 0x0009A4F4
	public uint AnsweringMessageId
	{
		get
		{
			if (!(this.cachedCassette != null))
			{
				return 0U;
			}
			return this.cachedCassette.AudioId;
		}
	}

	// Token: 0x0600136D RID: 4973 RVA: 0x0009C314 File Offset: 0x0009A514
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
		if (this.Controller.savedVoicemail != null)
		{
			info.msg.telephone.voicemail = Facepunch.Pool.GetList<ProtoBuf.VoicemailEntry>();
			foreach (ProtoBuf.VoicemailEntry item in this.Controller.savedVoicemail)
			{
				info.msg.telephone.voicemail.Add(item);
			}
		}
		if (!info.forDisk)
		{
			info.msg.telephone.usingPlayer = this.Controller.currentPlayerRef.uid;
		}
	}

	// Token: 0x0600136E RID: 4974 RVA: 0x0009C458 File Offset: 0x0009A658
	public override void ServerInit()
	{
		base.ServerInit();
		this.Controller.ServerInit();
		global::ItemContainer inventory = base.inventory;
		inventory.canAcceptItem = (Func<global::Item, int, bool>)Delegate.Combine(inventory.canAcceptItem, new Func<global::Item, int, bool>(this.CanAcceptItem));
	}

	// Token: 0x0600136F RID: 4975 RVA: 0x0009C492 File Offset: 0x0009A692
	public override void PostServerLoad()
	{
		base.PostServerLoad();
		this.Controller.PostServerLoad();
	}

	// Token: 0x06001370 RID: 4976 RVA: 0x0009C4A5 File Offset: 0x0009A6A5
	internal override void DoServerDestroy()
	{
		base.DoServerDestroy();
		this.Controller.DoServerDestroy();
	}

	// Token: 0x06001371 RID: 4977 RVA: 0x0009C4B8 File Offset: 0x0009A6B8
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.MaxDistance(9f)]
	public void ClearCurrentUser(global::BaseEntity.RPCMessage msg)
	{
		this.Controller.ClearCurrentUser(msg);
	}

	// Token: 0x06001372 RID: 4978 RVA: 0x0009C4C6 File Offset: 0x0009A6C6
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.MaxDistance(3f)]
	public void SetCurrentUser(global::BaseEntity.RPCMessage msg)
	{
		this.Controller.SetCurrentUser(msg);
	}

	// Token: 0x06001373 RID: 4979 RVA: 0x0009C4D4 File Offset: 0x0009A6D4
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.MaxDistance(3f)]
	public void InitiateCall(global::BaseEntity.RPCMessage msg)
	{
		this.Controller.InitiateCall(msg);
	}

	// Token: 0x06001374 RID: 4980 RVA: 0x0009C4E2 File Offset: 0x0009A6E2
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.MaxDistance(3f)]
	public void AnswerPhone(global::BaseEntity.RPCMessage msg)
	{
		this.Controller.AnswerPhone(msg);
	}

	// Token: 0x06001375 RID: 4981 RVA: 0x0009C4F0 File Offset: 0x0009A6F0
	[global::BaseEntity.RPC_Server]
	private void ServerHangUp(global::BaseEntity.RPCMessage msg)
	{
		this.Controller.ServerHangUp(msg);
	}

	// Token: 0x06001376 RID: 4982 RVA: 0x0009C4FE File Offset: 0x0009A6FE
	public void OnCassetteInserted(global::Cassette c)
	{
		this.cachedCassette = c;
		base.ClientRPC<NetworkableId>(null, "ClientOnCassetteChanged", c.net.ID);
	}

	// Token: 0x06001377 RID: 4983 RVA: 0x0009C520 File Offset: 0x0009A720
	public void OnCassetteRemoved(global::Cassette c)
	{
		this.cachedCassette = null;
		this.Controller.DeleteAllVoicemail();
		base.ClientRPC<NetworkableId>(null, "ClientOnCassetteChanged", default(NetworkableId));
	}

	// Token: 0x06001378 RID: 4984 RVA: 0x0009C554 File Offset: 0x0009A754
	private bool CanAcceptItem(global::Item item, int targetSlot)
	{
		ItemDefinition[] validCassettes = this.ValidCassettes;
		for (int i = 0; i < validCassettes.Length; i++)
		{
			if (validCassettes[i] == item.info)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06001379 RID: 4985 RVA: 0x0009C589 File Offset: 0x0009A789
	public override void DestroyShared()
	{
		base.DestroyShared();
		this.Controller.DestroyShared();
	}

	// Token: 0x0600137A RID: 4986 RVA: 0x0009C59C File Offset: 0x0009A79C
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.MaxDistance(3f)]
	[global::BaseEntity.RPC_Server.CallsPerSecond(5UL)]
	public void UpdatePhoneName(global::BaseEntity.RPCMessage msg)
	{
		this.Controller.UpdatePhoneName(msg);
	}

	// Token: 0x0600137B RID: 4987 RVA: 0x0009C5AA File Offset: 0x0009A7AA
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.MaxDistance(3f)]
	[global::BaseEntity.RPC_Server.CallsPerSecond(5UL)]
	public void Server_RequestPhoneDirectory(global::BaseEntity.RPCMessage msg)
	{
		this.Controller.Server_RequestPhoneDirectory(msg);
	}

	// Token: 0x0600137C RID: 4988 RVA: 0x0009C5B8 File Offset: 0x0009A7B8
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.MaxDistance(3f)]
	[global::BaseEntity.RPC_Server.CallsPerSecond(5UL)]
	public void Server_AddSavedNumber(global::BaseEntity.RPCMessage msg)
	{
		this.Controller.Server_AddSavedNumber(msg);
	}

	// Token: 0x0600137D RID: 4989 RVA: 0x0009C5C6 File Offset: 0x0009A7C6
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.MaxDistance(3f)]
	[global::BaseEntity.RPC_Server.CallsPerSecond(5UL)]
	public void Server_RemoveSavedNumber(global::BaseEntity.RPCMessage msg)
	{
		this.Controller.Server_RemoveSavedNumber(msg);
	}

	// Token: 0x0600137E RID: 4990 RVA: 0x0009C5D4 File Offset: 0x0009A7D4
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.CallsPerSecond(5UL)]
	public void ServerSendVoicemail(global::BaseEntity.RPCMessage msg)
	{
		this.Controller.ServerSendVoicemail(msg);
	}

	// Token: 0x0600137F RID: 4991 RVA: 0x0009C5E2 File Offset: 0x0009A7E2
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsVisible(3f)]
	[global::BaseEntity.RPC_Server.CallsPerSecond(5UL)]
	public void ServerPlayVoicemail(global::BaseEntity.RPCMessage msg)
	{
		this.Controller.ServerPlayVoicemail(msg);
	}

	// Token: 0x06001380 RID: 4992 RVA: 0x0009C5F0 File Offset: 0x0009A7F0
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsVisible(3f)]
	[global::BaseEntity.RPC_Server.CallsPerSecond(5UL)]
	public void ServerStopVoicemail(global::BaseEntity.RPCMessage msg)
	{
		this.Controller.ServerStopVoicemail(msg);
	}

	// Token: 0x06001381 RID: 4993 RVA: 0x0009C5FE File Offset: 0x0009A7FE
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsVisible(3f)]
	[global::BaseEntity.RPC_Server.CallsPerSecond(5UL)]
	public void ServerDeleteVoicemail(global::BaseEntity.RPCMessage msg)
	{
		this.Controller.ServerDeleteVoicemail(msg);
	}

	// Token: 0x06001382 RID: 4994 RVA: 0x0009C60C File Offset: 0x0009A80C
	public override int GetPassthroughAmount(int outputSlot = 0)
	{
		if (this.Controller.serverState == global::Telephone.CallState.Ringing || this.Controller.serverState == global::Telephone.CallState.InProcess)
		{
			return base.GetPassthroughAmount(outputSlot);
		}
		return 0;
	}

	// Token: 0x170001C2 RID: 450
	// (get) Token: 0x06001383 RID: 4995 RVA: 0x0009C633 File Offset: 0x0009A833
	// (set) Token: 0x06001384 RID: 4996 RVA: 0x0009C63B File Offset: 0x0009A83B
	public global::Cassette cachedCassette { get; private set; }

	// Token: 0x06001385 RID: 4997 RVA: 0x0009C644 File Offset: 0x0009A844
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		ProtoBuf.Entity msg = info.msg;
		if (((msg != null) ? msg.telephone : null) != null)
		{
			this.Controller.PhoneNumber = info.msg.telephone.phoneNumber;
			this.Controller.PhoneName = info.msg.telephone.phoneName;
			this.Controller.lastDialedNumber = info.msg.telephone.lastNumber;
			this.Controller.savedVoicemail = Facepunch.Pool.GetList<ProtoBuf.VoicemailEntry>();
			foreach (ProtoBuf.VoicemailEntry voicemailEntry in info.msg.telephone.voicemail)
			{
				this.Controller.savedVoicemail.Add(voicemailEntry);
				voicemailEntry.ShouldPool = false;
			}
			if (!info.fromDisk)
			{
				this.Controller.currentPlayerRef.uid = info.msg.telephone.usingPlayer;
			}
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
			if (info.fromDisk)
			{
				base.SetFlag(global::BaseEntity.Flags.Busy, false, false, true);
			}
		}
	}

	// Token: 0x06001386 RID: 4998 RVA: 0x0009C7C0 File Offset: 0x0009A9C0
	public override bool CanPickup(global::BasePlayer player)
	{
		return base.CanPickup(player) && this.Controller.currentPlayer == null;
	}

	// Token: 0x170001C3 RID: 451
	// (get) Token: 0x06001387 RID: 4999 RVA: 0x000037E7 File Offset: 0x000019E7
	public global::BaseEntity ToBaseEntity
	{
		get
		{
			return this;
		}
	}

	// Token: 0x06001388 RID: 5000 RVA: 0x0009C7E0 File Offset: 0x0009A9E0
	public override void OnFlagsChanged(global::BaseEntity.Flags old, global::BaseEntity.Flags next)
	{
		base.OnFlagsChanged(old, next);
		if (base.isServer)
		{
			if (this.Controller.RequirePower && next.HasFlag(global::BaseEntity.Flags.Busy) && !next.HasFlag(global::BaseEntity.Flags.Reserved8))
			{
				this.Controller.ServerHangUp();
			}
			if (old.HasFlag(global::BaseEntity.Flags.Busy) != next.HasFlag(global::BaseEntity.Flags.Busy))
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
		}
		this.Controller.OnFlagsChanged(old, next);
	}

	// Token: 0x02000C0B RID: 3083
	public enum CallState
	{
		// Token: 0x040041BC RID: 16828
		Idle,
		// Token: 0x040041BD RID: 16829
		Dialing,
		// Token: 0x040041BE RID: 16830
		Ringing,
		// Token: 0x040041BF RID: 16831
		InProcess
	}

	// Token: 0x02000C0C RID: 3084
	public enum DialFailReason
	{
		// Token: 0x040041C1 RID: 16833
		TimedOut,
		// Token: 0x040041C2 RID: 16834
		Engaged,
		// Token: 0x040041C3 RID: 16835
		WrongNumber,
		// Token: 0x040041C4 RID: 16836
		CallSelf,
		// Token: 0x040041C5 RID: 16837
		RemoteHangUp,
		// Token: 0x040041C6 RID: 16838
		NetworkBusy,
		// Token: 0x040041C7 RID: 16839
		TimeOutDuringCall,
		// Token: 0x040041C8 RID: 16840
		SelfHangUp
	}
}
