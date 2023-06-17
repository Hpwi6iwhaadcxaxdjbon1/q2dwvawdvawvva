using System;
using System.Collections.Generic;
using CompanionServer;
using ConVar;
using Facepunch;
using Facepunch.Rust;
using Network;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x020000BB RID: 187
public class RelationshipManager : global::BaseEntity
{
	// Token: 0x04000AA3 RID: 2723
	[ReplicatedVar]
	public static bool contacts = true;

	// Token: 0x04000AA4 RID: 2724
	private const int MugshotResolution = 256;

	// Token: 0x04000AA5 RID: 2725
	private const int MugshotMaxFileSize = 65536;

	// Token: 0x04000AA6 RID: 2726
	private const float MugshotMaxDistance = 50f;

	// Token: 0x04000AA7 RID: 2727
	public Dictionary<ulong, global::RelationshipManager.PlayerRelationships> relationships = new Dictionary<ulong, global::RelationshipManager.PlayerRelationships>();

	// Token: 0x04000AA8 RID: 2728
	private int lastReputationUpdateIndex;

	// Token: 0x04000AA9 RID: 2729
	private const int seenReputationSeconds = 60;

	// Token: 0x04000AAA RID: 2730
	private int startingReputation;

	// Token: 0x04000AAB RID: 2731
	[ServerVar]
	public static int forgetafterminutes = 960;

	// Token: 0x04000AAC RID: 2732
	[ServerVar]
	public static int maxplayerrelationships = 128;

	// Token: 0x04000AAD RID: 2733
	[ServerVar]
	public static float seendistance = 10f;

	// Token: 0x04000AAE RID: 2734
	[ServerVar]
	public static float mugshotUpdateInterval = 300f;

	// Token: 0x04000AAF RID: 2735
	private static List<global::BasePlayer> _dirtyRelationshipPlayers = new List<global::BasePlayer>();

	// Token: 0x04000AB0 RID: 2736
	public static int maxTeamSize_Internal = 8;

	// Token: 0x04000AB2 RID: 2738
	public Dictionary<ulong, global::BasePlayer> cachedPlayers = new Dictionary<ulong, global::BasePlayer>();

	// Token: 0x04000AB3 RID: 2739
	public Dictionary<ulong, global::RelationshipManager.PlayerTeam> playerToTeam = new Dictionary<ulong, global::RelationshipManager.PlayerTeam>();

	// Token: 0x04000AB4 RID: 2740
	public Dictionary<ulong, global::RelationshipManager.PlayerTeam> teams = new Dictionary<ulong, global::RelationshipManager.PlayerTeam>();

	// Token: 0x04000AB5 RID: 2741
	private ulong lastTeamIndex = 1UL;

	// Token: 0x060010BB RID: 4283 RVA: 0x00089FF0 File Offset: 0x000881F0
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("RelationshipManager.OnRpcMessage", 0))
		{
			if (rpc == 532372582U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - BagQuotaRequest_SERVER ");
				}
				using (TimeWarning.New("BagQuotaRequest_SERVER", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.CallsPerSecond.Test(532372582U, "BagQuotaRequest_SERVER", this, player, 2UL))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							this.BagQuotaRequest_SERVER();
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in BagQuotaRequest_SERVER");
					}
				}
				return true;
			}
			if (rpc == 1684577101U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - SERVER_ChangeRelationship ");
				}
				using (TimeWarning.New("SERVER_ChangeRelationship", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.CallsPerSecond.Test(1684577101U, "SERVER_ChangeRelationship", this, player, 2UL))
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
							this.SERVER_ChangeRelationship(msg2);
						}
					}
					catch (Exception exception2)
					{
						Debug.LogException(exception2);
						player.Kick("RPC Error in SERVER_ChangeRelationship");
					}
				}
				return true;
			}
			if (rpc == 1239936737U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - SERVER_ReceiveMugshot ");
				}
				using (TimeWarning.New("SERVER_ReceiveMugshot", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.CallsPerSecond.Test(1239936737U, "SERVER_ReceiveMugshot", this, player, 10UL))
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
							this.SERVER_ReceiveMugshot(msg3);
						}
					}
					catch (Exception exception3)
					{
						Debug.LogException(exception3);
						player.Kick("RPC Error in SERVER_ReceiveMugshot");
					}
				}
				return true;
			}
			if (rpc == 2178173141U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - SERVER_SendFreshContacts ");
				}
				using (TimeWarning.New("SERVER_SendFreshContacts", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.CallsPerSecond.Test(2178173141U, "SERVER_SendFreshContacts", this, player, 1UL))
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
							this.SERVER_SendFreshContacts(msg4);
						}
					}
					catch (Exception exception4)
					{
						Debug.LogException(exception4);
						player.Kick("RPC Error in SERVER_SendFreshContacts");
					}
				}
				return true;
			}
			if (rpc == 290196604U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - SERVER_UpdatePlayerNote ");
				}
				using (TimeWarning.New("SERVER_UpdatePlayerNote", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.CallsPerSecond.Test(290196604U, "SERVER_UpdatePlayerNote", this, player, 10UL))
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
							this.SERVER_UpdatePlayerNote(msg5);
						}
					}
					catch (Exception exception5)
					{
						Debug.LogException(exception5);
						player.Kick("RPC Error in SERVER_UpdatePlayerNote");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x060010BC RID: 4284 RVA: 0x000063A5 File Offset: 0x000045A5
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.CallsPerSecond(2UL)]
	public void BagQuotaRequest_SERVER()
	{
	}

	// Token: 0x060010BD RID: 4285 RVA: 0x0008A6C4 File Offset: 0x000888C4
	public override void ServerInit()
	{
		base.ServerInit();
		if (global::RelationshipManager.contacts)
		{
			base.InvokeRepeating(new Action(this.UpdateContactsTick), 0f, 1f);
			base.InvokeRepeating(new Action(this.UpdateReputations), 0f, 0.05f);
			base.InvokeRepeating(new Action(this.SendRelationships), 0f, 5f);
		}
	}

	// Token: 0x060010BE RID: 4286 RVA: 0x0008A734 File Offset: 0x00088934
	public void UpdateReputations()
	{
		if (!global::RelationshipManager.contacts)
		{
			return;
		}
		if (global::BasePlayer.activePlayerList.Count == 0)
		{
			return;
		}
		if (this.lastReputationUpdateIndex >= global::BasePlayer.activePlayerList.Count)
		{
			this.lastReputationUpdateIndex = 0;
		}
		global::BasePlayer basePlayer = global::BasePlayer.activePlayerList[this.lastReputationUpdateIndex];
		int reputation = basePlayer.reputation;
		int reputationFor = this.GetReputationFor(basePlayer.userID);
		basePlayer.reputation = reputationFor;
		if (reputation != reputationFor)
		{
			basePlayer.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
		}
		this.lastReputationUpdateIndex++;
	}

	// Token: 0x060010BF RID: 4287 RVA: 0x0008A7B4 File Offset: 0x000889B4
	public void UpdateContactsTick()
	{
		if (!global::RelationshipManager.contacts)
		{
			return;
		}
		foreach (global::BasePlayer player in global::BasePlayer.activePlayerList)
		{
			this.UpdateAcquaintancesFor(player, 1f);
		}
	}

	// Token: 0x060010C0 RID: 4288 RVA: 0x0008A814 File Offset: 0x00088A14
	public int GetReputationFor(ulong playerID)
	{
		int num = this.startingReputation;
		foreach (global::RelationshipManager.PlayerRelationships playerRelationships in this.relationships.Values)
		{
			foreach (KeyValuePair<ulong, global::RelationshipManager.PlayerRelationshipInfo> keyValuePair in playerRelationships.relations)
			{
				if (keyValuePair.Key == playerID)
				{
					if (keyValuePair.Value.type == global::RelationshipManager.RelationshipType.Friend)
					{
						num++;
					}
					else if (keyValuePair.Value.type == global::RelationshipManager.RelationshipType.Acquaintance)
					{
						if (keyValuePair.Value.weight > 60)
						{
							num++;
						}
					}
					else if (keyValuePair.Value.type == global::RelationshipManager.RelationshipType.Enemy)
					{
						num--;
					}
				}
			}
		}
		return num;
	}

	// Token: 0x060010C1 RID: 4289 RVA: 0x0008A904 File Offset: 0x00088B04
	[ServerVar]
	public static void wipecontacts(ConsoleSystem.Arg arg)
	{
		global::BasePlayer basePlayer = arg.Player();
		if (basePlayer == null)
		{
			return;
		}
		if (global::RelationshipManager.ServerInstance == null)
		{
			return;
		}
		ulong userID = basePlayer.userID;
		if (global::RelationshipManager.ServerInstance.relationships.ContainsKey(userID))
		{
			Debug.Log("Wiped contacts for :" + userID);
			global::RelationshipManager.ServerInstance.relationships.Remove(userID);
			global::RelationshipManager.ServerInstance.MarkRelationshipsDirtyFor(userID);
			return;
		}
		Debug.Log("No contacts for :" + userID);
	}

	// Token: 0x060010C2 RID: 4290 RVA: 0x0008A990 File Offset: 0x00088B90
	[ServerVar]
	public static void wipe_all_contacts(ConsoleSystem.Arg arg)
	{
		global::BasePlayer basePlayer = arg.Player();
		if (basePlayer == null)
		{
			return;
		}
		if (global::RelationshipManager.ServerInstance == null)
		{
			return;
		}
		if (!arg.HasArgs(1) || arg.Args[0] != "confirm")
		{
			Debug.Log("Please append the word 'confirm' at the end of the console command to execute");
			return;
		}
		ulong userID = basePlayer.userID;
		global::RelationshipManager.ServerInstance.relationships.Clear();
		foreach (global::BasePlayer player in global::BasePlayer.activePlayerList)
		{
			global::RelationshipManager.ServerInstance.MarkRelationshipsDirtyFor(player);
		}
		Debug.Log("Wiped all contacts.");
	}

	// Token: 0x060010C3 RID: 4291 RVA: 0x0008AA50 File Offset: 0x00088C50
	public float GetAcquaintanceMaxDist()
	{
		return global::RelationshipManager.seendistance;
	}

	// Token: 0x060010C4 RID: 4292 RVA: 0x0008AA58 File Offset: 0x00088C58
	public void UpdateAcquaintancesFor(global::BasePlayer player, float deltaSeconds)
	{
		global::RelationshipManager.PlayerRelationships playerRelationships = this.GetRelationships(player.userID);
		List<global::BasePlayer> list = Facepunch.Pool.GetList<global::BasePlayer>();
		global::BaseNetworkable.GetCloseConnections(player.transform.position, this.GetAcquaintanceMaxDist(), list);
		foreach (global::BasePlayer basePlayer in list)
		{
			if (!(basePlayer == player) && !basePlayer.isClient && basePlayer.IsAlive() && !basePlayer.IsSleeping())
			{
				global::RelationshipManager.PlayerRelationshipInfo relations = playerRelationships.GetRelations(basePlayer.userID);
				if (Vector3.Distance(player.transform.position, basePlayer.transform.position) <= this.GetAcquaintanceMaxDist())
				{
					relations.lastSeenTime = UnityEngine.Time.realtimeSinceStartup;
					if ((relations.type == global::RelationshipManager.RelationshipType.NONE || relations.type == global::RelationshipManager.RelationshipType.Acquaintance) && player.IsPlayerVisibleToUs(basePlayer, 1218519041))
					{
						int num = Mathf.CeilToInt(deltaSeconds);
						if (player.InSafeZone() || basePlayer.InSafeZone())
						{
							num = 0;
						}
						if (relations.type != global::RelationshipManager.RelationshipType.Acquaintance || (relations.weight < 60 && num > 0))
						{
							this.SetRelationship(player, basePlayer, global::RelationshipManager.RelationshipType.Acquaintance, num, false);
						}
					}
				}
			}
		}
		Facepunch.Pool.FreeList<global::BasePlayer>(ref list);
	}

	// Token: 0x060010C5 RID: 4293 RVA: 0x0008ABA4 File Offset: 0x00088DA4
	public void SetSeen(global::BasePlayer player, global::BasePlayer otherPlayer)
	{
		ulong userID = player.userID;
		ulong userID2 = otherPlayer.userID;
		global::RelationshipManager.PlayerRelationshipInfo relations = this.GetRelationships(userID).GetRelations(userID2);
		if (relations.type != global::RelationshipManager.RelationshipType.NONE)
		{
			relations.lastSeenTime = UnityEngine.Time.realtimeSinceStartup;
		}
	}

	// Token: 0x060010C6 RID: 4294 RVA: 0x0008ABE0 File Offset: 0x00088DE0
	public bool CleanupOldContacts(global::RelationshipManager.PlayerRelationships ownerRelationships, ulong playerID, global::RelationshipManager.RelationshipType relationshipType = global::RelationshipManager.RelationshipType.Acquaintance)
	{
		int numberRelationships = this.GetNumberRelationships(playerID);
		if (numberRelationships < global::RelationshipManager.maxplayerrelationships)
		{
			return true;
		}
		List<ulong> list = Facepunch.Pool.GetList<ulong>();
		foreach (KeyValuePair<ulong, global::RelationshipManager.PlayerRelationshipInfo> keyValuePair in ownerRelationships.relations)
		{
			if (keyValuePair.Value.type == relationshipType && UnityEngine.Time.realtimeSinceStartup - keyValuePair.Value.lastSeenTime > (float)global::RelationshipManager.forgetafterminutes * 60f)
			{
				list.Add(keyValuePair.Key);
			}
		}
		int count = list.Count;
		foreach (ulong player in list)
		{
			ownerRelationships.Forget(player);
		}
		Facepunch.Pool.FreeList<ulong>(ref list);
		return numberRelationships - count < global::RelationshipManager.maxplayerrelationships;
	}

	// Token: 0x060010C7 RID: 4295 RVA: 0x0008ACDC File Offset: 0x00088EDC
	public void ForceRelationshipByID(global::BasePlayer player, ulong otherPlayerID, global::RelationshipManager.RelationshipType newType, int weight, bool sendImmediate = false)
	{
		if (!global::RelationshipManager.contacts)
		{
			return;
		}
		if (player == null)
		{
			return;
		}
		if (player.userID == otherPlayerID)
		{
			return;
		}
		if (player.IsNpc)
		{
			return;
		}
		ulong userID = player.userID;
		if (!this.HasRelations(userID, otherPlayerID))
		{
			return;
		}
		global::RelationshipManager.PlayerRelationshipInfo relations = this.GetRelationships(userID).GetRelations(otherPlayerID);
		if (relations.type != newType)
		{
			relations.weight = 0;
		}
		relations.type = newType;
		relations.weight += weight;
		if (sendImmediate)
		{
			this.SendRelationshipsFor(player);
			return;
		}
		this.MarkRelationshipsDirtyFor(player);
	}

	// Token: 0x060010C8 RID: 4296 RVA: 0x0008AD68 File Offset: 0x00088F68
	public void SetRelationship(global::BasePlayer player, global::BasePlayer otherPlayer, global::RelationshipManager.RelationshipType type, int weight = 1, bool sendImmediate = false)
	{
		if (!global::RelationshipManager.contacts)
		{
			return;
		}
		ulong userID = player.userID;
		ulong userID2 = otherPlayer.userID;
		if (player == null)
		{
			return;
		}
		if (player == otherPlayer)
		{
			return;
		}
		if (player.IsNpc)
		{
			return;
		}
		if (otherPlayer != null && otherPlayer.IsNpc)
		{
			return;
		}
		global::RelationshipManager.PlayerRelationships playerRelationships = this.GetRelationships(userID);
		if (!this.CleanupOldContacts(playerRelationships, userID, global::RelationshipManager.RelationshipType.Acquaintance))
		{
			this.CleanupOldContacts(playerRelationships, userID, global::RelationshipManager.RelationshipType.Enemy);
		}
		global::RelationshipManager.PlayerRelationshipInfo relations = playerRelationships.GetRelations(userID2);
		bool flag = false;
		if (relations.type != type)
		{
			flag = true;
			relations.weight = 0;
		}
		relations.type = type;
		relations.weight += weight;
		float num = UnityEngine.Time.realtimeSinceStartup - relations.lastMugshotTime;
		if (flag || relations.mugshotCrc == 0U || num >= global::RelationshipManager.mugshotUpdateInterval)
		{
			bool flag2 = otherPlayer.IsAlive();
			bool flag3 = player.SecondsSinceAttacked > 10f && !player.IsAiming;
			float num2 = 100f;
			if (flag3)
			{
				Vector3 normalized = (otherPlayer.eyes.position - player.eyes.position).normalized;
				bool flag4 = Vector3.Dot(player.eyes.HeadForward(), normalized) >= 0.6f;
				float num3 = Vector3Ex.Distance2D(player.transform.position, otherPlayer.transform.position);
				if (flag2 && num3 < num2 && flag4)
				{
					base.ClientRPCPlayer<ulong>(null, player, "CLIENT_DoMugshot", userID2);
					relations.lastMugshotTime = UnityEngine.Time.realtimeSinceStartup;
				}
			}
		}
		if (sendImmediate)
		{
			this.SendRelationshipsFor(player);
			return;
		}
		this.MarkRelationshipsDirtyFor(player);
	}

	// Token: 0x060010C9 RID: 4297 RVA: 0x0008AF08 File Offset: 0x00089108
	public ProtoBuf.RelationshipManager.PlayerRelationships GetRelationshipSaveByID(ulong playerID)
	{
		ProtoBuf.RelationshipManager.PlayerRelationships playerRelationships = Facepunch.Pool.Get<ProtoBuf.RelationshipManager.PlayerRelationships>();
		global::RelationshipManager.PlayerRelationships playerRelationships2 = this.GetRelationships(playerID);
		if (playerRelationships2 != null)
		{
			playerRelationships.playerID = playerID;
			playerRelationships.relations = Facepunch.Pool.GetList<ProtoBuf.RelationshipManager.PlayerRelationshipInfo>();
			foreach (KeyValuePair<ulong, global::RelationshipManager.PlayerRelationshipInfo> keyValuePair in playerRelationships2.relations)
			{
				ProtoBuf.RelationshipManager.PlayerRelationshipInfo playerRelationshipInfo = Facepunch.Pool.Get<ProtoBuf.RelationshipManager.PlayerRelationshipInfo>();
				playerRelationshipInfo.playerID = keyValuePair.Value.player;
				playerRelationshipInfo.type = (int)keyValuePair.Value.type;
				playerRelationshipInfo.weight = keyValuePair.Value.weight;
				playerRelationshipInfo.mugshotCrc = keyValuePair.Value.mugshotCrc;
				playerRelationshipInfo.displayName = keyValuePair.Value.displayName;
				playerRelationshipInfo.notes = keyValuePair.Value.notes;
				playerRelationshipInfo.timeSinceSeen = UnityEngine.Time.realtimeSinceStartup - keyValuePair.Value.lastSeenTime;
				playerRelationships.relations.Add(playerRelationshipInfo);
			}
			return playerRelationships;
		}
		return null;
	}

	// Token: 0x060010CA RID: 4298 RVA: 0x0008B024 File Offset: 0x00089224
	public void MarkRelationshipsDirtyFor(ulong playerID)
	{
		global::BasePlayer basePlayer = global::RelationshipManager.FindByID(playerID);
		if (basePlayer)
		{
			this.MarkRelationshipsDirtyFor(basePlayer);
		}
	}

	// Token: 0x060010CB RID: 4299 RVA: 0x0008B047 File Offset: 0x00089247
	public static void ForceSendRelationships(global::BasePlayer player)
	{
		if (global::RelationshipManager.ServerInstance)
		{
			global::RelationshipManager.ServerInstance.MarkRelationshipsDirtyFor(player);
		}
	}

	// Token: 0x060010CC RID: 4300 RVA: 0x0008B060 File Offset: 0x00089260
	public void MarkRelationshipsDirtyFor(global::BasePlayer player)
	{
		if (player == null)
		{
			return;
		}
		if (!global::RelationshipManager._dirtyRelationshipPlayers.Contains(player))
		{
			global::RelationshipManager._dirtyRelationshipPlayers.Add(player);
		}
		ulong userID = player.userID;
	}

	// Token: 0x060010CD RID: 4301 RVA: 0x0008B08C File Offset: 0x0008928C
	public void SendRelationshipsFor(global::BasePlayer player)
	{
		if (!global::RelationshipManager.contacts)
		{
			return;
		}
		ulong userID = player.userID;
		ProtoBuf.RelationshipManager.PlayerRelationships relationshipSaveByID = this.GetRelationshipSaveByID(userID);
		base.ClientRPCPlayer<ProtoBuf.RelationshipManager.PlayerRelationships>(null, player, "CLIENT_RecieveLocalRelationships", relationshipSaveByID);
	}

	// Token: 0x060010CE RID: 4302 RVA: 0x0008B0C0 File Offset: 0x000892C0
	public void SendRelationships()
	{
		if (!global::RelationshipManager.contacts)
		{
			return;
		}
		foreach (global::BasePlayer basePlayer in global::RelationshipManager._dirtyRelationshipPlayers)
		{
			if (!(basePlayer == null) && basePlayer.IsConnected && !basePlayer.IsSleeping())
			{
				this.SendRelationshipsFor(basePlayer);
			}
		}
		global::RelationshipManager._dirtyRelationshipPlayers.Clear();
	}

	// Token: 0x060010CF RID: 4303 RVA: 0x0008B140 File Offset: 0x00089340
	public int GetNumberRelationships(ulong player)
	{
		global::RelationshipManager.PlayerRelationships playerRelationships;
		if (this.relationships.TryGetValue(player, out playerRelationships))
		{
			return playerRelationships.relations.Count;
		}
		return 0;
	}

	// Token: 0x060010D0 RID: 4304 RVA: 0x0008B16C File Offset: 0x0008936C
	public bool HasRelations(ulong player, ulong otherPlayer)
	{
		global::RelationshipManager.PlayerRelationships playerRelationships;
		return this.relationships.TryGetValue(player, out playerRelationships) && playerRelationships.relations.ContainsKey(otherPlayer);
	}

	// Token: 0x060010D1 RID: 4305 RVA: 0x0008B19C File Offset: 0x0008939C
	public global::RelationshipManager.PlayerRelationships GetRelationships(ulong player)
	{
		global::RelationshipManager.PlayerRelationships result;
		if (this.relationships.TryGetValue(player, out result))
		{
			return result;
		}
		global::RelationshipManager.PlayerRelationships playerRelationships = Facepunch.Pool.Get<global::RelationshipManager.PlayerRelationships>();
		playerRelationships.ownerPlayer = player;
		this.relationships.Add(player, playerRelationships);
		return playerRelationships;
	}

	// Token: 0x060010D2 RID: 4306 RVA: 0x0008B1D8 File Offset: 0x000893D8
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.CallsPerSecond(1UL)]
	public void SERVER_SendFreshContacts(global::BaseEntity.RPCMessage msg)
	{
		global::BasePlayer player = msg.player;
		if (player)
		{
			this.SendRelationshipsFor(player);
		}
	}

	// Token: 0x060010D3 RID: 4307 RVA: 0x0008B1FC File Offset: 0x000893FC
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.CallsPerSecond(2UL)]
	public void SERVER_ChangeRelationship(global::BaseEntity.RPCMessage msg)
	{
		ulong userID = msg.player.userID;
		ulong num = msg.read.UInt64();
		int num2 = Mathf.Clamp(msg.read.Int32(), 0, 3);
		global::RelationshipManager.PlayerRelationships playerRelationships = this.GetRelationships(userID);
		playerRelationships.GetRelations(num);
		global::BasePlayer player = msg.player;
		global::RelationshipManager.RelationshipType relationshipType = (global::RelationshipManager.RelationshipType)num2;
		if (num2 == 0)
		{
			if (playerRelationships.Forget(num))
			{
				this.SendRelationshipsFor(player);
			}
			return;
		}
		global::BasePlayer basePlayer = global::RelationshipManager.FindByID(num);
		if (basePlayer == null)
		{
			this.ForceRelationshipByID(player, num, relationshipType, 0, true);
			return;
		}
		this.SetRelationship(player, basePlayer, relationshipType, 1, true);
	}

	// Token: 0x060010D4 RID: 4308 RVA: 0x0008B28C File Offset: 0x0008948C
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.CallsPerSecond(10UL)]
	public void SERVER_UpdatePlayerNote(global::BaseEntity.RPCMessage msg)
	{
		ulong userID = msg.player.userID;
		ulong player = msg.read.UInt64();
		string notes = msg.read.String(256);
		this.GetRelationships(userID).GetRelations(player).notes = notes;
		this.MarkRelationshipsDirtyFor(userID);
	}

	// Token: 0x060010D5 RID: 4309 RVA: 0x0008B2DC File Offset: 0x000894DC
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.CallsPerSecond(10UL)]
	public void SERVER_ReceiveMugshot(global::BaseEntity.RPCMessage msg)
	{
		ulong userID = msg.player.userID;
		ulong num = msg.read.UInt64();
		uint num2 = msg.read.UInt32();
		byte[] array = msg.read.BytesWithSize(65536U);
		if (array == null || !ImageProcessing.IsValidJPG(array, 256, 512))
		{
			return;
		}
		global::RelationshipManager.PlayerRelationships playerRelationships;
		global::RelationshipManager.PlayerRelationshipInfo playerRelationshipInfo;
		if (!this.relationships.TryGetValue(userID, out playerRelationships) || !playerRelationships.relations.TryGetValue(num, out playerRelationshipInfo))
		{
			return;
		}
		uint steamIdHash = global::RelationshipManager.GetSteamIdHash(userID, num);
		uint num3 = FileStorage.server.Store(array, FileStorage.Type.jpg, this.net.ID, steamIdHash);
		if (num3 != num2)
		{
			Debug.LogWarning("Client/Server FileStorage CRC differs");
		}
		if (num3 != playerRelationshipInfo.mugshotCrc)
		{
			FileStorage.server.RemoveExact(playerRelationshipInfo.mugshotCrc, FileStorage.Type.jpg, this.net.ID, steamIdHash);
		}
		playerRelationshipInfo.mugshotCrc = num3;
		this.MarkRelationshipsDirtyFor(userID);
	}

	// Token: 0x060010D6 RID: 4310 RVA: 0x0008B3C4 File Offset: 0x000895C4
	private void DeleteMugshot(ulong steamId, ulong targetSteamId, uint crc)
	{
		if (crc == 0U)
		{
			return;
		}
		uint steamIdHash = global::RelationshipManager.GetSteamIdHash(steamId, targetSteamId);
		FileStorage.server.RemoveExact(crc, FileStorage.Type.jpg, this.net.ID, steamIdHash);
	}

	// Token: 0x060010D7 RID: 4311 RVA: 0x0008B3F5 File Offset: 0x000895F5
	private static uint GetSteamIdHash(ulong requesterSteamId, ulong targetSteamId)
	{
		return (uint)((requesterSteamId & 65535UL) << 16 | (targetSteamId & 65535UL));
	}

	// Token: 0x1700018F RID: 399
	// (get) Token: 0x060010D9 RID: 4313 RVA: 0x0008B42B File Offset: 0x0008962B
	// (set) Token: 0x060010D8 RID: 4312 RVA: 0x0008B40C File Offset: 0x0008960C
	[ServerVar]
	public static int maxTeamSize
	{
		get
		{
			return global::RelationshipManager.maxTeamSize_Internal;
		}
		set
		{
			global::RelationshipManager.maxTeamSize_Internal = value;
			if (global::RelationshipManager.ServerInstance)
			{
				global::RelationshipManager.ServerInstance.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
			}
		}
	}

	// Token: 0x060010DA RID: 4314 RVA: 0x0008B434 File Offset: 0x00089634
	public int GetMaxTeamSize()
	{
		BaseGameMode activeGameMode = BaseGameMode.GetActiveGameMode(true);
		if (activeGameMode)
		{
			return activeGameMode.GetMaxRelationshipTeamSize();
		}
		return global::RelationshipManager.maxTeamSize;
	}

	// Token: 0x17000190 RID: 400
	// (get) Token: 0x060010DB RID: 4315 RVA: 0x0008B45C File Offset: 0x0008965C
	// (set) Token: 0x060010DC RID: 4316 RVA: 0x0008B463 File Offset: 0x00089663
	public static global::RelationshipManager ServerInstance { get; private set; }

	// Token: 0x060010DD RID: 4317 RVA: 0x0008B46B File Offset: 0x0008966B
	public void OnEnable()
	{
		if (base.isServer)
		{
			if (global::RelationshipManager.ServerInstance != null)
			{
				Debug.LogError("Major fuckup! RelationshipManager spawned twice, Contact Developers!");
				UnityEngine.Object.Destroy(base.gameObject);
				return;
			}
			global::RelationshipManager.ServerInstance = this;
		}
	}

	// Token: 0x060010DE RID: 4318 RVA: 0x0008B49E File Offset: 0x0008969E
	public void OnDestroy()
	{
		if (base.isServer)
		{
			global::RelationshipManager.ServerInstance = null;
		}
	}

	// Token: 0x060010DF RID: 4319 RVA: 0x0008B4B0 File Offset: 0x000896B0
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.relationshipManager = Facepunch.Pool.Get<ProtoBuf.RelationshipManager>();
		info.msg.relationshipManager.maxTeamSize = global::RelationshipManager.maxTeamSize;
		if (info.forDisk)
		{
			info.msg.relationshipManager.lastTeamIndex = this.lastTeamIndex;
			info.msg.relationshipManager.teamList = Facepunch.Pool.GetList<ProtoBuf.PlayerTeam>();
			foreach (KeyValuePair<ulong, global::RelationshipManager.PlayerTeam> keyValuePair in this.teams)
			{
				global::RelationshipManager.PlayerTeam value = keyValuePair.Value;
				if (value != null)
				{
					ProtoBuf.PlayerTeam playerTeam = Facepunch.Pool.Get<ProtoBuf.PlayerTeam>();
					playerTeam.teamLeader = value.teamLeader;
					playerTeam.teamID = value.teamID;
					playerTeam.teamName = value.teamName;
					playerTeam.members = Facepunch.Pool.GetList<ProtoBuf.PlayerTeam.TeamMember>();
					foreach (ulong num in value.members)
					{
						ProtoBuf.PlayerTeam.TeamMember teamMember = Facepunch.Pool.Get<ProtoBuf.PlayerTeam.TeamMember>();
						global::BasePlayer basePlayer = global::RelationshipManager.FindByID(num);
						teamMember.displayName = ((basePlayer != null) ? basePlayer.displayName : (SingletonComponent<ServerMgr>.Instance.persistance.GetPlayerName(num) ?? "DEAD"));
						teamMember.userID = num;
						playerTeam.members.Add(teamMember);
					}
					info.msg.relationshipManager.teamList.Add(playerTeam);
				}
			}
			info.msg.relationshipManager.relationships = Facepunch.Pool.GetList<ProtoBuf.RelationshipManager.PlayerRelationships>();
			foreach (ulong num2 in this.relationships.Keys)
			{
				global::RelationshipManager.PlayerRelationships playerRelationships = this.relationships[num2];
				ProtoBuf.RelationshipManager.PlayerRelationships relationshipSaveByID = this.GetRelationshipSaveByID(num2);
				info.msg.relationshipManager.relationships.Add(relationshipSaveByID);
			}
		}
	}

	// Token: 0x060010E0 RID: 4320 RVA: 0x0008B6DC File Offset: 0x000898DC
	public void DisbandTeam(global::RelationshipManager.PlayerTeam teamToDisband)
	{
		this.teams.Remove(teamToDisband.teamID);
		Facepunch.Pool.Free<global::RelationshipManager.PlayerTeam>(ref teamToDisband);
	}

	// Token: 0x060010E1 RID: 4321 RVA: 0x0008B6F8 File Offset: 0x000898F8
	public static global::BasePlayer FindByID(ulong userID)
	{
		global::BasePlayer basePlayer = null;
		if (global::RelationshipManager.ServerInstance.cachedPlayers.TryGetValue(userID, out basePlayer))
		{
			if (basePlayer != null)
			{
				return basePlayer;
			}
			global::RelationshipManager.ServerInstance.cachedPlayers.Remove(userID);
		}
		global::BasePlayer basePlayer2 = global::BasePlayer.FindByID(userID);
		if (!basePlayer2)
		{
			basePlayer2 = global::BasePlayer.FindSleeping(userID);
		}
		if (basePlayer2 != null)
		{
			global::RelationshipManager.ServerInstance.cachedPlayers.Add(userID, basePlayer2);
		}
		return basePlayer2;
	}

	// Token: 0x060010E2 RID: 4322 RVA: 0x0008B768 File Offset: 0x00089968
	public global::RelationshipManager.PlayerTeam FindTeam(ulong TeamID)
	{
		if (this.teams.ContainsKey(TeamID))
		{
			return this.teams[TeamID];
		}
		return null;
	}

	// Token: 0x060010E3 RID: 4323 RVA: 0x0008B788 File Offset: 0x00089988
	public global::RelationshipManager.PlayerTeam FindPlayersTeam(ulong userID)
	{
		global::RelationshipManager.PlayerTeam result;
		if (this.playerToTeam.TryGetValue(userID, out result))
		{
			return result;
		}
		return null;
	}

	// Token: 0x060010E4 RID: 4324 RVA: 0x0008B7A8 File Offset: 0x000899A8
	public global::RelationshipManager.PlayerTeam CreateTeam()
	{
		global::RelationshipManager.PlayerTeam playerTeam = Facepunch.Pool.Get<global::RelationshipManager.PlayerTeam>();
		playerTeam.teamID = this.lastTeamIndex;
		playerTeam.teamStartTime = UnityEngine.Time.realtimeSinceStartup;
		this.teams.Add(this.lastTeamIndex, playerTeam);
		this.lastTeamIndex += 1UL;
		return playerTeam;
	}

	// Token: 0x060010E5 RID: 4325 RVA: 0x0008B7F4 File Offset: 0x000899F4
	[ServerUserVar]
	public static void trycreateteam(ConsoleSystem.Arg arg)
	{
		if (global::RelationshipManager.maxTeamSize == 0)
		{
			arg.ReplyWith("Teams are disabled on this server");
			return;
		}
		global::BasePlayer basePlayer = arg.Player();
		if (basePlayer.currentTeam != 0UL)
		{
			return;
		}
		global::RelationshipManager.PlayerTeam playerTeam = global::RelationshipManager.ServerInstance.CreateTeam();
		playerTeam.teamLeader = basePlayer.userID;
		playerTeam.AddPlayer(basePlayer);
		Analytics.Azure.OnTeamChanged("created", playerTeam.teamID, basePlayer.userID, basePlayer.userID, playerTeam.members);
	}

	// Token: 0x060010E6 RID: 4326 RVA: 0x0008B868 File Offset: 0x00089A68
	[ServerUserVar]
	public static void promote(ConsoleSystem.Arg arg)
	{
		global::BasePlayer basePlayer = arg.Player();
		if (basePlayer.currentTeam == 0UL)
		{
			return;
		}
		global::BasePlayer lookingAtPlayer = global::RelationshipManager.GetLookingAtPlayer(basePlayer);
		if (lookingAtPlayer == null)
		{
			return;
		}
		if (lookingAtPlayer.IsDead())
		{
			return;
		}
		if (lookingAtPlayer == basePlayer)
		{
			return;
		}
		if (lookingAtPlayer.currentTeam == basePlayer.currentTeam)
		{
			global::RelationshipManager.PlayerTeam playerTeam = global::RelationshipManager.ServerInstance.teams[basePlayer.currentTeam];
			if (playerTeam != null && playerTeam.teamLeader == basePlayer.userID)
			{
				playerTeam.SetTeamLeader(lookingAtPlayer.userID);
			}
		}
	}

	// Token: 0x060010E7 RID: 4327 RVA: 0x0008B8EC File Offset: 0x00089AEC
	[ServerUserVar]
	public static void leaveteam(ConsoleSystem.Arg arg)
	{
		global::BasePlayer basePlayer = arg.Player();
		if (basePlayer == null)
		{
			return;
		}
		if (basePlayer.currentTeam == 0UL)
		{
			return;
		}
		global::RelationshipManager.PlayerTeam playerTeam = global::RelationshipManager.ServerInstance.FindTeam(basePlayer.currentTeam);
		if (playerTeam != null)
		{
			playerTeam.RemovePlayer(basePlayer.userID);
			basePlayer.ClearTeam();
		}
	}

	// Token: 0x060010E8 RID: 4328 RVA: 0x0008B93C File Offset: 0x00089B3C
	[ServerUserVar]
	public static void acceptinvite(ConsoleSystem.Arg arg)
	{
		global::BasePlayer basePlayer = arg.Player();
		if (basePlayer == null)
		{
			return;
		}
		if (basePlayer.currentTeam != 0UL)
		{
			return;
		}
		ulong @ulong = arg.GetULong(0, 0UL);
		global::RelationshipManager.PlayerTeam playerTeam = global::RelationshipManager.ServerInstance.FindTeam(@ulong);
		if (playerTeam == null)
		{
			basePlayer.ClearPendingInvite();
			return;
		}
		playerTeam.AcceptInvite(basePlayer);
	}

	// Token: 0x060010E9 RID: 4329 RVA: 0x0008B98C File Offset: 0x00089B8C
	[ServerUserVar]
	public static void rejectinvite(ConsoleSystem.Arg arg)
	{
		global::BasePlayer basePlayer = arg.Player();
		if (basePlayer == null)
		{
			return;
		}
		if (basePlayer.currentTeam != 0UL)
		{
			return;
		}
		ulong @ulong = arg.GetULong(0, 0UL);
		global::RelationshipManager.PlayerTeam playerTeam = global::RelationshipManager.ServerInstance.FindTeam(@ulong);
		if (playerTeam == null)
		{
			basePlayer.ClearPendingInvite();
			return;
		}
		playerTeam.RejectInvite(basePlayer);
	}

	// Token: 0x060010EA RID: 4330 RVA: 0x0008B9DC File Offset: 0x00089BDC
	public static global::BasePlayer GetLookingAtPlayer(global::BasePlayer source)
	{
		RaycastHit hit;
		if (UnityEngine.Physics.Raycast(source.eyes.position, source.eyes.HeadForward(), out hit, 5f, 1218652417, QueryTriggerInteraction.Ignore))
		{
			global::BaseEntity entity = hit.GetEntity();
			if (entity)
			{
				return entity.GetComponent<global::BasePlayer>();
			}
		}
		return null;
	}

	// Token: 0x060010EB RID: 4331 RVA: 0x0008BA2C File Offset: 0x00089C2C
	[ServerVar]
	public static void sleeptoggle(ConsoleSystem.Arg arg)
	{
		global::BasePlayer basePlayer = arg.Player();
		if (basePlayer == null)
		{
			return;
		}
		RaycastHit hit;
		if (UnityEngine.Physics.Raycast(basePlayer.eyes.position, basePlayer.eyes.HeadForward(), out hit, 5f, 1218652417, QueryTriggerInteraction.Ignore))
		{
			global::BaseEntity entity = hit.GetEntity();
			if (entity)
			{
				global::BasePlayer component = entity.GetComponent<global::BasePlayer>();
				if (component && component != basePlayer && !component.IsNpc)
				{
					if (component.IsSleeping())
					{
						component.EndSleeping();
						return;
					}
					component.StartSleeping();
				}
			}
		}
	}

	// Token: 0x060010EC RID: 4332 RVA: 0x0008BAB8 File Offset: 0x00089CB8
	[ServerUserVar]
	public static void kickmember(ConsoleSystem.Arg arg)
	{
		global::BasePlayer basePlayer = arg.Player();
		if (basePlayer == null)
		{
			return;
		}
		global::RelationshipManager.PlayerTeam playerTeam = global::RelationshipManager.ServerInstance.FindTeam(basePlayer.currentTeam);
		if (playerTeam == null)
		{
			return;
		}
		if (playerTeam.GetLeader() != basePlayer)
		{
			return;
		}
		ulong @ulong = arg.GetULong(0, 0UL);
		if (basePlayer.userID == @ulong)
		{
			return;
		}
		playerTeam.RemovePlayer(@ulong);
	}

	// Token: 0x060010ED RID: 4333 RVA: 0x0008BB18 File Offset: 0x00089D18
	[ServerUserVar]
	public static void sendinvite(ConsoleSystem.Arg arg)
	{
		global::BasePlayer basePlayer = arg.Player();
		global::RelationshipManager.PlayerTeam playerTeam = global::RelationshipManager.ServerInstance.FindTeam(basePlayer.currentTeam);
		if (playerTeam == null)
		{
			return;
		}
		if (playerTeam.GetLeader() == null)
		{
			return;
		}
		if (playerTeam.GetLeader() != basePlayer)
		{
			return;
		}
		RaycastHit hit;
		if (UnityEngine.Physics.Raycast(basePlayer.eyes.position, basePlayer.eyes.HeadForward(), out hit, 5f, 1218652417, QueryTriggerInteraction.Ignore))
		{
			global::BaseEntity entity = hit.GetEntity();
			if (entity)
			{
				global::BasePlayer component = entity.GetComponent<global::BasePlayer>();
				if (component && component != basePlayer && !component.IsNpc && component.currentTeam == 0UL)
				{
					playerTeam.SendInvite(component);
				}
			}
		}
	}

	// Token: 0x060010EE RID: 4334 RVA: 0x0008BBD0 File Offset: 0x00089DD0
	[ServerVar]
	public static void fakeinvite(ConsoleSystem.Arg arg)
	{
		global::BasePlayer basePlayer = arg.Player();
		ulong @ulong = arg.GetULong(0, 0UL);
		global::RelationshipManager.PlayerTeam playerTeam = global::RelationshipManager.ServerInstance.FindTeam(@ulong);
		if (playerTeam == null)
		{
			return;
		}
		if (basePlayer.currentTeam != 0UL)
		{
			Debug.Log("already in team");
		}
		playerTeam.SendInvite(basePlayer);
		Debug.Log("sent bot invite");
	}

	// Token: 0x060010EF RID: 4335 RVA: 0x0008BC24 File Offset: 0x00089E24
	[ServerVar]
	public static void addtoteam(ConsoleSystem.Arg arg)
	{
		global::BasePlayer basePlayer = arg.Player();
		global::RelationshipManager.PlayerTeam playerTeam = global::RelationshipManager.ServerInstance.FindTeam(basePlayer.currentTeam);
		if (playerTeam == null)
		{
			return;
		}
		if (playerTeam.GetLeader() == null)
		{
			return;
		}
		if (playerTeam.GetLeader() != basePlayer)
		{
			return;
		}
		RaycastHit hit;
		if (UnityEngine.Physics.Raycast(basePlayer.eyes.position, basePlayer.eyes.HeadForward(), out hit, 5f, 1218652417, QueryTriggerInteraction.Ignore))
		{
			global::BaseEntity entity = hit.GetEntity();
			if (entity)
			{
				global::BasePlayer component = entity.GetComponent<global::BasePlayer>();
				if (component && component != basePlayer && !component.IsNpc)
				{
					playerTeam.AddPlayer(component);
				}
			}
		}
	}

	// Token: 0x060010F0 RID: 4336 RVA: 0x0008BCD1 File Offset: 0x00089ED1
	public static bool TeamsEnabled()
	{
		return global::RelationshipManager.maxTeamSize > 0;
	}

	// Token: 0x060010F1 RID: 4337 RVA: 0x0008BCDC File Offset: 0x00089EDC
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.fromDisk && info.msg.relationshipManager != null)
		{
			this.lastTeamIndex = info.msg.relationshipManager.lastTeamIndex;
			foreach (ProtoBuf.PlayerTeam playerTeam in info.msg.relationshipManager.teamList)
			{
				global::RelationshipManager.PlayerTeam playerTeam2 = Facepunch.Pool.Get<global::RelationshipManager.PlayerTeam>();
				playerTeam2.teamLeader = playerTeam.teamLeader;
				playerTeam2.teamID = playerTeam.teamID;
				playerTeam2.teamName = playerTeam.teamName;
				playerTeam2.members = new List<ulong>();
				foreach (ProtoBuf.PlayerTeam.TeamMember teamMember in playerTeam.members)
				{
					playerTeam2.members.Add(teamMember.userID);
				}
				this.teams[playerTeam2.teamID] = playerTeam2;
			}
			foreach (global::RelationshipManager.PlayerTeam playerTeam3 in this.teams.Values)
			{
				foreach (ulong num in playerTeam3.members)
				{
					this.playerToTeam[num] = playerTeam3;
					global::BasePlayer basePlayer = global::RelationshipManager.FindByID(num);
					if (basePlayer != null && basePlayer.currentTeam != playerTeam3.teamID)
					{
						Debug.LogWarning(string.Format("Player {0} has the wrong teamID: got {1}, expected {2}. Fixing automatically.", num, basePlayer.currentTeam, playerTeam3.teamID));
						basePlayer.currentTeam = playerTeam3.teamID;
					}
				}
			}
			foreach (ProtoBuf.RelationshipManager.PlayerRelationships playerRelationships in info.msg.relationshipManager.relationships)
			{
				ulong playerID = playerRelationships.playerID;
				global::RelationshipManager.PlayerRelationships playerRelationships2 = this.GetRelationships(playerID);
				playerRelationships2.relations.Clear();
				foreach (ProtoBuf.RelationshipManager.PlayerRelationshipInfo playerRelationshipInfo in playerRelationships.relations)
				{
					global::RelationshipManager.PlayerRelationshipInfo playerRelationshipInfo2 = new global::RelationshipManager.PlayerRelationshipInfo();
					playerRelationshipInfo2.type = (global::RelationshipManager.RelationshipType)playerRelationshipInfo.type;
					playerRelationshipInfo2.weight = playerRelationshipInfo.weight;
					playerRelationshipInfo2.displayName = playerRelationshipInfo.displayName;
					playerRelationshipInfo2.mugshotCrc = playerRelationshipInfo.mugshotCrc;
					playerRelationshipInfo2.notes = playerRelationshipInfo.notes;
					playerRelationshipInfo2.player = playerRelationshipInfo.playerID;
					playerRelationshipInfo2.lastSeenTime = UnityEngine.Time.realtimeSinceStartup - playerRelationshipInfo.timeSinceSeen;
					playerRelationships2.relations.Add(playerRelationshipInfo.playerID, playerRelationshipInfo2);
				}
			}
		}
	}

	// Token: 0x02000BF0 RID: 3056
	public enum RelationshipType
	{
		// Token: 0x04004159 RID: 16729
		NONE,
		// Token: 0x0400415A RID: 16730
		Acquaintance,
		// Token: 0x0400415B RID: 16731
		Friend,
		// Token: 0x0400415C RID: 16732
		Enemy
	}

	// Token: 0x02000BF1 RID: 3057
	public class PlayerRelationshipInfo : Facepunch.Pool.IPooled, IServerFileReceiver
	{
		// Token: 0x0400415D RID: 16733
		public string displayName;

		// Token: 0x0400415E RID: 16734
		public ulong player;

		// Token: 0x0400415F RID: 16735
		public global::RelationshipManager.RelationshipType type;

		// Token: 0x04004160 RID: 16736
		public int weight;

		// Token: 0x04004161 RID: 16737
		public uint mugshotCrc;

		// Token: 0x04004162 RID: 16738
		public string notes;

		// Token: 0x04004163 RID: 16739
		public float lastSeenTime;

		// Token: 0x04004164 RID: 16740
		public float lastMugshotTime;

		// Token: 0x06004DC8 RID: 19912 RVA: 0x001A15D8 File Offset: 0x0019F7D8
		public void EnterPool()
		{
			this.Reset();
		}

		// Token: 0x06004DC9 RID: 19913 RVA: 0x001A15D8 File Offset: 0x0019F7D8
		public void LeavePool()
		{
			this.Reset();
		}

		// Token: 0x06004DCA RID: 19914 RVA: 0x001A15E0 File Offset: 0x0019F7E0
		private void Reset()
		{
			this.displayName = null;
			this.player = 0UL;
			this.type = global::RelationshipManager.RelationshipType.NONE;
			this.weight = 0;
			this.mugshotCrc = 0U;
			this.notes = "";
			this.lastMugshotTime = 0f;
		}
	}

	// Token: 0x02000BF2 RID: 3058
	public class PlayerRelationships : Facepunch.Pool.IPooled
	{
		// Token: 0x04004165 RID: 16741
		public bool dirty;

		// Token: 0x04004166 RID: 16742
		public ulong ownerPlayer;

		// Token: 0x04004167 RID: 16743
		public Dictionary<ulong, global::RelationshipManager.PlayerRelationshipInfo> relations;

		// Token: 0x06004DCC RID: 19916 RVA: 0x001A161C File Offset: 0x0019F81C
		public bool Forget(ulong player)
		{
			global::RelationshipManager.PlayerRelationshipInfo playerRelationshipInfo;
			if (this.relations.TryGetValue(player, out playerRelationshipInfo))
			{
				this.relations.Remove(player);
				if (playerRelationshipInfo.mugshotCrc != 0U)
				{
					global::RelationshipManager.ServerInstance.DeleteMugshot(this.ownerPlayer, player, playerRelationshipInfo.mugshotCrc);
				}
				return true;
			}
			return false;
		}

		// Token: 0x06004DCD RID: 19917 RVA: 0x001A1668 File Offset: 0x0019F868
		public global::RelationshipManager.PlayerRelationshipInfo GetRelations(ulong player)
		{
			global::BasePlayer basePlayer = global::RelationshipManager.FindByID(player);
			global::RelationshipManager.PlayerRelationshipInfo playerRelationshipInfo;
			if (this.relations.TryGetValue(player, out playerRelationshipInfo))
			{
				if (basePlayer != null)
				{
					playerRelationshipInfo.displayName = basePlayer.displayName;
				}
				return playerRelationshipInfo;
			}
			global::RelationshipManager.PlayerRelationshipInfo playerRelationshipInfo2 = Facepunch.Pool.Get<global::RelationshipManager.PlayerRelationshipInfo>();
			if (basePlayer != null)
			{
				playerRelationshipInfo2.displayName = basePlayer.displayName;
			}
			playerRelationshipInfo2.player = player;
			this.relations.Add(player, playerRelationshipInfo2);
			return playerRelationshipInfo2;
		}

		// Token: 0x06004DCE RID: 19918 RVA: 0x001A16D3 File Offset: 0x0019F8D3
		public PlayerRelationships()
		{
			this.LeavePool();
		}

		// Token: 0x06004DCF RID: 19919 RVA: 0x001A16E1 File Offset: 0x0019F8E1
		public void EnterPool()
		{
			this.ownerPlayer = 0UL;
			if (this.relations != null)
			{
				this.relations.Clear();
				Facepunch.Pool.Free<Dictionary<ulong, global::RelationshipManager.PlayerRelationshipInfo>>(ref this.relations);
			}
		}

		// Token: 0x06004DD0 RID: 19920 RVA: 0x001A1709 File Offset: 0x0019F909
		public void LeavePool()
		{
			this.ownerPlayer = 0UL;
			this.relations = Facepunch.Pool.Get<Dictionary<ulong, global::RelationshipManager.PlayerRelationshipInfo>>();
			this.relations.Clear();
		}
	}

	// Token: 0x02000BF3 RID: 3059
	public class PlayerTeam
	{
		// Token: 0x04004168 RID: 16744
		public ulong teamID;

		// Token: 0x04004169 RID: 16745
		public string teamName;

		// Token: 0x0400416A RID: 16746
		public ulong teamLeader;

		// Token: 0x0400416B RID: 16747
		public List<ulong> members = new List<ulong>();

		// Token: 0x0400416C RID: 16748
		public List<ulong> invites = new List<ulong>();

		// Token: 0x0400416D RID: 16749
		public float teamStartTime;

		// Token: 0x0400416E RID: 16750
		private List<Network.Connection> onlineMemberConnections = new List<Network.Connection>();

		// Token: 0x17000677 RID: 1655
		// (get) Token: 0x06004DD1 RID: 19921 RVA: 0x001A1729 File Offset: 0x0019F929
		public float teamLifetime
		{
			get
			{
				return UnityEngine.Time.realtimeSinceStartup - this.teamStartTime;
			}
		}

		// Token: 0x06004DD2 RID: 19922 RVA: 0x001A1737 File Offset: 0x0019F937
		public global::BasePlayer GetLeader()
		{
			return global::RelationshipManager.FindByID(this.teamLeader);
		}

		// Token: 0x06004DD3 RID: 19923 RVA: 0x001A1744 File Offset: 0x0019F944
		public void SendInvite(global::BasePlayer player)
		{
			if (this.invites.Count > 8)
			{
				this.invites.RemoveRange(0, 1);
			}
			global::BasePlayer basePlayer = global::RelationshipManager.FindByID(this.teamLeader);
			if (basePlayer == null)
			{
				return;
			}
			this.invites.Add(player.userID);
			player.ClientRPCPlayer<string, ulong, ulong>(null, player, "CLIENT_PendingInvite", basePlayer.displayName, this.teamLeader, this.teamID);
		}

		// Token: 0x06004DD4 RID: 19924 RVA: 0x001A17B2 File Offset: 0x0019F9B2
		public void AcceptInvite(global::BasePlayer player)
		{
			if (!this.invites.Contains(player.userID))
			{
				return;
			}
			this.invites.Remove(player.userID);
			this.AddPlayer(player);
			player.ClearPendingInvite();
		}

		// Token: 0x06004DD5 RID: 19925 RVA: 0x001A17E8 File Offset: 0x0019F9E8
		public void RejectInvite(global::BasePlayer player)
		{
			player.ClearPendingInvite();
			this.invites.Remove(player.userID);
		}

		// Token: 0x06004DD6 RID: 19926 RVA: 0x001A1804 File Offset: 0x0019FA04
		public bool AddPlayer(global::BasePlayer player)
		{
			ulong userID = player.userID;
			if (this.members.Contains(userID))
			{
				return false;
			}
			if (player.currentTeam != 0UL)
			{
				return false;
			}
			if (this.members.Count >= global::RelationshipManager.maxTeamSize)
			{
				return false;
			}
			player.currentTeam = this.teamID;
			bool flag = this.members.Count == 0;
			this.members.Add(userID);
			global::RelationshipManager.ServerInstance.playerToTeam.Add(userID, this);
			this.MarkDirty();
			player.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
			if (!flag)
			{
				Analytics.Azure.OnTeamChanged("added", this.teamID, this.teamLeader, userID, this.members);
			}
			return true;
		}

		// Token: 0x06004DD7 RID: 19927 RVA: 0x001A18AC File Offset: 0x0019FAAC
		public bool RemovePlayer(ulong playerID)
		{
			if (this.members.Contains(playerID))
			{
				this.members.Remove(playerID);
				global::RelationshipManager.ServerInstance.playerToTeam.Remove(playerID);
				global::BasePlayer basePlayer = global::RelationshipManager.FindByID(playerID);
				if (basePlayer != null)
				{
					basePlayer.ClearTeam();
					basePlayer.BroadcastAppTeamRemoval();
				}
				if (this.teamLeader == playerID)
				{
					if (this.members.Count > 0)
					{
						this.SetTeamLeader(this.members[0]);
						Analytics.Azure.OnTeamChanged("removed", this.teamID, this.teamLeader, playerID, this.members);
					}
					else
					{
						Analytics.Azure.OnTeamChanged("disband", this.teamID, this.teamLeader, playerID, this.members);
						this.Disband();
					}
				}
				this.MarkDirty();
				return true;
			}
			return false;
		}

		// Token: 0x06004DD8 RID: 19928 RVA: 0x001A1977 File Offset: 0x0019FB77
		public void SetTeamLeader(ulong newTeamLeader)
		{
			Analytics.Azure.OnTeamChanged("promoted", this.teamID, this.teamLeader, newTeamLeader, this.members);
			this.teamLeader = newTeamLeader;
			this.MarkDirty();
		}

		// Token: 0x06004DD9 RID: 19929 RVA: 0x001A19A3 File Offset: 0x0019FBA3
		public void Disband()
		{
			global::RelationshipManager.ServerInstance.DisbandTeam(this);
			CompanionServer.Server.TeamChat.Remove(this.teamID);
		}

		// Token: 0x06004DDA RID: 19930 RVA: 0x001A19C0 File Offset: 0x0019FBC0
		public void MarkDirty()
		{
			foreach (ulong userID in this.members)
			{
				global::BasePlayer basePlayer = global::RelationshipManager.FindByID(userID);
				if (basePlayer != null)
				{
					basePlayer.UpdateTeam(this.teamID);
				}
			}
			this.BroadcastAppTeamUpdate();
		}

		// Token: 0x06004DDB RID: 19931 RVA: 0x001A1A2C File Offset: 0x0019FC2C
		public List<Network.Connection> GetOnlineMemberConnections()
		{
			if (this.members.Count == 0)
			{
				return null;
			}
			this.onlineMemberConnections.Clear();
			foreach (ulong userID in this.members)
			{
				global::BasePlayer basePlayer = global::RelationshipManager.FindByID(userID);
				if (!(basePlayer == null) && basePlayer.Connection != null)
				{
					this.onlineMemberConnections.Add(basePlayer.Connection);
				}
			}
			return this.onlineMemberConnections;
		}
	}
}
