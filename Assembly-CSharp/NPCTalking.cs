using System;
using System.Collections.Generic;
using ConVar;
using Facepunch.Rust;
using Network;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x020000A6 RID: 166
public class NPCTalking : NPCShopKeeper, IConversationProvider
{
	// Token: 0x04000A19 RID: 2585
	public ConversationData[] conversations;

	// Token: 0x04000A1A RID: 2586
	public NPCTalking.NPCConversationResultAction[] conversationResultActions;

	// Token: 0x04000A1B RID: 2587
	[NonSerialized]
	private float maxConversationDistance = 5f;

	// Token: 0x04000A1C RID: 2588
	private List<BasePlayer> conversingPlayers = new List<BasePlayer>();

	// Token: 0x04000A1D RID: 2589
	private BasePlayer lastActionPlayer;

	// Token: 0x06000F43 RID: 3907 RVA: 0x000807A4 File Offset: 0x0007E9A4
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("NPCTalking.OnRpcMessage", 0))
		{
			if (rpc == 4224060672U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - ConversationAction ");
				}
				using (TimeWarning.New("ConversationAction", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.CallsPerSecond.Test(4224060672U, "ConversationAction", this, player, 5UL))
						{
							return true;
						}
						if (!BaseEntity.RPC_Server.MaxDistance.Test(4224060672U, "ConversationAction", this, player, 3f))
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
							this.ConversationAction(msg2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in ConversationAction");
					}
				}
				return true;
			}
			if (rpc == 2112414875U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - Server_BeginTalking ");
				}
				using (TimeWarning.New("Server_BeginTalking", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.CallsPerSecond.Test(2112414875U, "Server_BeginTalking", this, player, 1UL))
						{
							return true;
						}
						if (!BaseEntity.RPC_Server.MaxDistance.Test(2112414875U, "Server_BeginTalking", this, player, 3f))
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
							this.Server_BeginTalking(msg3);
						}
					}
					catch (Exception exception2)
					{
						Debug.LogException(exception2);
						player.Kick("RPC Error in Server_BeginTalking");
					}
				}
				return true;
			}
			if (rpc == 1597539152U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - Server_EndTalking ");
				}
				using (TimeWarning.New("Server_EndTalking", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.CallsPerSecond.Test(1597539152U, "Server_EndTalking", this, player, 1UL))
						{
							return true;
						}
						if (!BaseEntity.RPC_Server.MaxDistance.Test(1597539152U, "Server_EndTalking", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							BaseEntity.RPCMessage msg4 = new BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.Server_EndTalking(msg4);
						}
					}
					catch (Exception exception3)
					{
						Debug.LogException(exception3);
						player.Kick("RPC Error in Server_EndTalking");
					}
				}
				return true;
			}
			if (rpc == 2713250658U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - Server_ResponsePressed ");
				}
				using (TimeWarning.New("Server_ResponsePressed", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.CallsPerSecond.Test(2713250658U, "Server_ResponsePressed", this, player, 5UL))
						{
							return true;
						}
						if (!BaseEntity.RPC_Server.MaxDistance.Test(2713250658U, "Server_ResponsePressed", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							BaseEntity.RPCMessage msg5 = new BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.Server_ResponsePressed(msg5);
						}
					}
					catch (Exception exception4)
					{
						Debug.LogException(exception4);
						player.Kick("RPC Error in Server_ResponsePressed");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000F44 RID: 3908 RVA: 0x00080DCC File Offset: 0x0007EFCC
	public int GetConversationIndex(string conversationName)
	{
		for (int i = 0; i < this.conversations.Length; i++)
		{
			if (this.conversations[i].shortname == conversationName)
			{
				return i;
			}
		}
		return -1;
	}

	// Token: 0x06000F45 RID: 3909 RVA: 0x00080E04 File Offset: 0x0007F004
	public virtual string GetConversationStartSpeech(BasePlayer player)
	{
		return "intro";
	}

	// Token: 0x06000F46 RID: 3910 RVA: 0x00080E0B File Offset: 0x0007F00B
	public ConversationData GetConversation(string conversationName)
	{
		return this.GetConversation(this.GetConversationIndex(conversationName));
	}

	// Token: 0x06000F47 RID: 3911 RVA: 0x00080E1A File Offset: 0x0007F01A
	public ConversationData GetConversation(int index)
	{
		return this.conversations[index];
	}

	// Token: 0x06000F48 RID: 3912 RVA: 0x00080E24 File Offset: 0x0007F024
	public virtual ConversationData GetConversationFor(BasePlayer player)
	{
		return this.conversations[0];
	}

	// Token: 0x06000F49 RID: 3913 RVA: 0x000231B4 File Offset: 0x000213B4
	public bool ProviderBusy()
	{
		return base.HasFlag(BaseEntity.Flags.Reserved1);
	}

	// Token: 0x06000F4A RID: 3914 RVA: 0x00080E2E File Offset: 0x0007F02E
	public void ForceEndConversation(BasePlayer player)
	{
		base.ClientRPCPlayer(null, player, "Client_EndConversation");
	}

	// Token: 0x06000F4B RID: 3915 RVA: 0x00080E3D File Offset: 0x0007F03D
	public void ForceSpeechNode(BasePlayer player, int speechNodeIndex)
	{
		if (player == null)
		{
			return;
		}
		base.ClientRPCPlayer<int>(null, player, "Client_ForceSpeechNode", speechNodeIndex);
	}

	// Token: 0x06000F4C RID: 3916 RVA: 0x00080E57 File Offset: 0x0007F057
	public virtual void OnConversationEnded(BasePlayer player)
	{
		if (this.conversingPlayers.Contains(player))
		{
			this.conversingPlayers.Remove(player);
		}
	}

	// Token: 0x06000F4D RID: 3917 RVA: 0x00080E74 File Offset: 0x0007F074
	public void CleanupConversingPlayers()
	{
		for (int i = this.conversingPlayers.Count - 1; i >= 0; i--)
		{
			BasePlayer basePlayer = this.conversingPlayers[i];
			if (basePlayer == null || !basePlayer.IsAlive() || basePlayer.IsSleeping())
			{
				this.conversingPlayers.RemoveAt(i);
			}
		}
	}

	// Token: 0x06000F4E RID: 3918 RVA: 0x00080ECC File Offset: 0x0007F0CC
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.MaxDistance(3f)]
	[BaseEntity.RPC_Server.CallsPerSecond(1UL)]
	public void Server_BeginTalking(BaseEntity.RPCMessage msg)
	{
		BasePlayer player = msg.player;
		this.CleanupConversingPlayers();
		ConversationData conversationFor = this.GetConversationFor(player);
		if (conversationFor != null)
		{
			if (this.conversingPlayers.Contains(player))
			{
				this.OnConversationEnded(player);
			}
			this.conversingPlayers.Add(player);
			this.UpdateFlags();
			this.OnConversationStarted(player);
			base.ClientRPCPlayer<int, string>(null, player, "Client_StartConversation", this.GetConversationIndex(conversationFor.shortname), this.GetConversationStartSpeech(player));
		}
	}

	// Token: 0x06000F4F RID: 3919 RVA: 0x000063A5 File Offset: 0x000045A5
	public virtual void OnConversationStarted(BasePlayer speakingTo)
	{
	}

	// Token: 0x06000F50 RID: 3920 RVA: 0x000063A5 File Offset: 0x000045A5
	public virtual void UpdateFlags()
	{
	}

	// Token: 0x06000F51 RID: 3921 RVA: 0x00080F45 File Offset: 0x0007F145
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.MaxDistance(3f)]
	[BaseEntity.RPC_Server.CallsPerSecond(1UL)]
	public void Server_EndTalking(BaseEntity.RPCMessage msg)
	{
		this.OnConversationEnded(msg.player);
	}

	// Token: 0x06000F52 RID: 3922 RVA: 0x00080F54 File Offset: 0x0007F154
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.MaxDistance(3f)]
	[BaseEntity.RPC_Server.CallsPerSecond(5UL)]
	public void ConversationAction(BaseEntity.RPCMessage msg)
	{
		BasePlayer player = msg.player;
		string action = msg.read.String(256);
		this.OnConversationAction(player, action);
	}

	// Token: 0x06000F53 RID: 3923 RVA: 0x00080F81 File Offset: 0x0007F181
	public bool ValidConversationPlayer(BasePlayer player)
	{
		return Vector3.Distance(player.transform.position, base.transform.position) <= this.maxConversationDistance && !this.conversingPlayers.Contains(player);
	}

	// Token: 0x06000F54 RID: 3924 RVA: 0x00080FBC File Offset: 0x0007F1BC
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.MaxDistance(3f)]
	[BaseEntity.RPC_Server.CallsPerSecond(5UL)]
	public void Server_ResponsePressed(BaseEntity.RPCMessage msg)
	{
		BasePlayer player = msg.player;
		int num = msg.read.Int32();
		int num2 = msg.read.Int32();
		ConversationData conversationFor = this.GetConversationFor(player);
		if (conversationFor == null)
		{
			return;
		}
		ConversationData.ResponseNode responseNode = conversationFor.speeches[num].responses[num2];
		if (responseNode != null)
		{
			if (responseNode.conditions.Length != 0)
			{
				this.UpdateFlags();
			}
			bool flag = responseNode.PassesConditions(player, this);
			if (flag && !string.IsNullOrEmpty(responseNode.actionString))
			{
				this.OnConversationAction(player, responseNode.actionString);
			}
			int speechNodeIndex = conversationFor.GetSpeechNodeIndex(flag ? responseNode.resultingSpeechNode : responseNode.GetFailedSpeechNode(player, this));
			if (speechNodeIndex == -1)
			{
				this.ForceEndConversation(player);
				return;
			}
			this.ForceSpeechNode(player, speechNodeIndex);
		}
	}

	// Token: 0x06000F55 RID: 3925 RVA: 0x0008107E File Offset: 0x0007F27E
	public BasePlayer GetActionPlayer()
	{
		return this.lastActionPlayer;
	}

	// Token: 0x06000F56 RID: 3926 RVA: 0x00081088 File Offset: 0x0007F288
	public virtual void OnConversationAction(BasePlayer player, string action)
	{
		if (action == "openvending")
		{
			InvisibleVendingMachine vendingMachine = base.GetVendingMachine();
			if (vendingMachine != null && Vector3.Distance(player.transform.position, base.transform.position) < 5f)
			{
				this.ForceEndConversation(player);
				vendingMachine.PlayerOpenLoot(player, "vendingmachine.customer", false);
				return;
			}
		}
		ItemDefinition itemDefinition = ItemManager.FindItemDefinition("scrap");
		NPCTalking.NPCConversationResultAction[] array = this.conversationResultActions;
		int i = 0;
		while (i < array.Length)
		{
			NPCTalking.NPCConversationResultAction npcconversationResultAction = array[i];
			if (npcconversationResultAction.action == action)
			{
				this.CleanupConversingPlayers();
				foreach (BasePlayer basePlayer in this.conversingPlayers)
				{
					if (!(basePlayer == player) && !(basePlayer == null))
					{
						int speechNodeIndex = this.GetConversationFor(player).GetSpeechNodeIndex("startbusy");
						this.ForceSpeechNode(basePlayer, speechNodeIndex);
					}
				}
				int num = npcconversationResultAction.scrapCost;
				List<Item> list = player.inventory.FindItemIDs(itemDefinition.itemid);
				foreach (Item item in list)
				{
					num -= item.amount;
				}
				if (num > 0)
				{
					int speechNodeIndex2 = this.GetConversationFor(player).GetSpeechNodeIndex("toopoor");
					this.ForceSpeechNode(player, speechNodeIndex2);
					return;
				}
				Analytics.Azure.OnNPCVendor(player, this, npcconversationResultAction.scrapCost, npcconversationResultAction.action);
				num = npcconversationResultAction.scrapCost;
				foreach (Item item2 in list)
				{
					int num2 = Mathf.Min(num, item2.amount);
					item2.UseItem(num2);
					num -= num2;
					if (num <= 0)
					{
						break;
					}
				}
				this.lastActionPlayer = player;
				base.BroadcastEntityMessage(npcconversationResultAction.broadcastMessage, npcconversationResultAction.broadcastRange, 1218652417);
				this.lastActionPlayer = null;
				return;
			}
			else
			{
				i++;
			}
		}
	}

	// Token: 0x02000BEB RID: 3051
	[Serializable]
	public class NPCConversationResultAction
	{
		// Token: 0x0400414D RID: 16717
		public string action;

		// Token: 0x0400414E RID: 16718
		public int scrapCost;

		// Token: 0x0400414F RID: 16719
		public string broadcastMessage;

		// Token: 0x04004150 RID: 16720
		public float broadcastRange;
	}
}
