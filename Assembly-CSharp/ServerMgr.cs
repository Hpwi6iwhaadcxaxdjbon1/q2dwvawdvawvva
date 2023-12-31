﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CompanionServer;
using ConVar;
using Facepunch;
using Facepunch.Math;
using Facepunch.Models;
using Facepunch.Network;
using Facepunch.Rust;
using Ionic.Crc;
using Network;
using Network.Visibility;
using ProtoBuf;
using Rust;
using Steamworks;
using UnityEngine;

// Token: 0x02000743 RID: 1859
public class ServerMgr : SingletonComponent<ServerMgr>, IServerCallback
{
	// Token: 0x04002A1B RID: 10779
	public ConnectionQueue connectionQueue = new ConnectionQueue();

	// Token: 0x04002A1C RID: 10780
	public TimeAverageValueLookup<Message.Type> packetHistory = new TimeAverageValueLookup<Message.Type>();

	// Token: 0x04002A1D RID: 10781
	public TimeAverageValueLookup<uint> rpcHistory = new TimeAverageValueLookup<uint>();

	// Token: 0x04002A1E RID: 10782
	public const string BYPASS_PROCEDURAL_SPAWN_PREF = "bypassProceduralSpawn";

	// Token: 0x04002A1F RID: 10783
	private ConnectionAuth auth;

	// Token: 0x04002A21 RID: 10785
	public UserPersistance persistance;

	// Token: 0x04002A22 RID: 10786
	public PlayerStateManager playerStateManager;

	// Token: 0x04002A23 RID: 10787
	private AIThinkManager.QueueType aiTick;

	// Token: 0x04002A24 RID: 10788
	private List<ulong> bannedPlayerNotices = new List<ulong>();

	// Token: 0x04002A25 RID: 10789
	private string _AssemblyHash;

	// Token: 0x04002A26 RID: 10790
	private IEnumerator restartCoroutine;

	// Token: 0x060033DD RID: 13277 RVA: 0x0013F414 File Offset: 0x0013D614
	private void Log(Exception e)
	{
		if (ConVar.Global.developer > 0)
		{
			Debug.LogException(e);
		}
	}

	// Token: 0x060033DE RID: 13278 RVA: 0x0013F424 File Offset: 0x0013D624
	public void OnNetworkMessage(Message packet)
	{
		if (ConVar.Server.packetlog_enabled)
		{
			this.packetHistory.Increment(packet.type);
		}
		Message.Type type = packet.type;
		if (type != Message.Type.Ready)
		{
			switch (type)
			{
			case Message.Type.RPCMessage:
				if (!packet.connection.connected)
				{
					return;
				}
				if (packet.connection.GetPacketsPerSecond(packet.type) >= (ulong)((long)ConVar.Server.maxpacketspersecond_rpc))
				{
					Network.Net.sv.Kick(packet.connection, "Packet Flooding: RPC Message", false);
					return;
				}
				using (TimeWarning.New("OnRPCMessage", 20))
				{
					try
					{
						this.OnRPCMessage(packet);
					}
					catch (Exception e)
					{
						this.Log(e);
						Network.Net.sv.Kick(packet.connection, "Invalid Packet: RPC Message", false);
					}
				}
				packet.connection.AddPacketsPerSecond(packet.type);
				return;
			case Message.Type.EntityPosition:
			case Message.Type.ConsoleMessage:
			case Message.Type.Effect:
				break;
			case Message.Type.ConsoleCommand:
				if (!packet.connection.connected)
				{
					return;
				}
				if (packet.connection.GetPacketsPerSecond(packet.type) >= (ulong)((long)ConVar.Server.maxpacketspersecond_command))
				{
					Network.Net.sv.Kick(packet.connection, "Packet Flooding: Client Command", packet.connection.connected);
					return;
				}
				using (TimeWarning.New("OnClientCommand", 20))
				{
					try
					{
						ConsoleNetwork.OnClientCommand(packet);
					}
					catch (Exception e2)
					{
						this.Log(e2);
						Network.Net.sv.Kick(packet.connection, "Invalid Packet: Client Command", false);
					}
				}
				packet.connection.AddPacketsPerSecond(packet.type);
				return;
			case Message.Type.DisconnectReason:
				if (!packet.connection.connected)
				{
					return;
				}
				if (packet.connection.GetPacketsPerSecond(packet.type) >= 1UL)
				{
					Network.Net.sv.Kick(packet.connection, "Packet Flooding: Disconnect Reason", packet.connection.connected);
					return;
				}
				using (TimeWarning.New("ReadDisconnectReason", 20))
				{
					try
					{
						this.ReadDisconnectReason(packet);
						Network.Net.sv.Disconnect(packet.connection);
					}
					catch (Exception e3)
					{
						this.Log(e3);
						Network.Net.sv.Kick(packet.connection, "Invalid Packet: Disconnect Reason", false);
					}
				}
				packet.connection.AddPacketsPerSecond(packet.type);
				return;
			case Message.Type.Tick:
				if (!packet.connection.connected)
				{
					return;
				}
				if (packet.connection.GetPacketsPerSecond(packet.type) >= (ulong)((long)ConVar.Server.maxpacketspersecond_tick))
				{
					Network.Net.sv.Kick(packet.connection, "Packet Flooding: Player Tick", packet.connection.connected);
					return;
				}
				using (TimeWarning.New("OnPlayerTick", 20))
				{
					try
					{
						this.OnPlayerTick(packet);
					}
					catch (Exception e4)
					{
						this.Log(e4);
						Network.Net.sv.Kick(packet.connection, "Invalid Packet: Player Tick", false);
					}
				}
				packet.connection.AddPacketsPerSecond(packet.type);
				return;
			default:
				switch (type)
				{
				case Message.Type.GiveUserInformation:
					if (packet.connection.GetPacketsPerSecond(packet.type) >= 1UL)
					{
						Network.Net.sv.Kick(packet.connection, "Packet Flooding: User Information", packet.connection.connected);
						return;
					}
					using (TimeWarning.New("GiveUserInformation", 20))
					{
						try
						{
							this.OnGiveUserInformation(packet);
						}
						catch (Exception e5)
						{
							this.Log(e5);
							Network.Net.sv.Kick(packet.connection, "Invalid Packet: User Information", false);
						}
					}
					packet.connection.AddPacketsPerSecond(packet.type);
					return;
				case Message.Type.VoiceData:
					if (!packet.connection.connected)
					{
						return;
					}
					if (packet.connection.GetPacketsPerSecond(packet.type) >= (ulong)((long)ConVar.Server.maxpacketspersecond_voice))
					{
						Network.Net.sv.Kick(packet.connection, "Packet Flooding: Disconnect Reason", packet.connection.connected);
						return;
					}
					using (TimeWarning.New("OnPlayerVoice", 20))
					{
						try
						{
							this.OnPlayerVoice(packet);
						}
						catch (Exception e6)
						{
							this.Log(e6);
							Network.Net.sv.Kick(packet.connection, "Invalid Packet: Player Voice", false);
						}
					}
					packet.connection.AddPacketsPerSecond(packet.type);
					return;
				case Message.Type.EAC:
					using (TimeWarning.New("OnEACMessage", 20))
					{
						try
						{
							EACServer.OnMessageReceived(packet);
						}
						catch (Exception e7)
						{
							this.Log(e7);
							Network.Net.sv.Kick(packet.connection, "Invalid Packet: EAC", false);
						}
					}
					return;
				case Message.Type.World:
					if (!global::World.Transfer)
					{
						return;
					}
					if (!packet.connection.connected)
					{
						return;
					}
					if (packet.connection.GetPacketsPerSecond(packet.type) >= (ulong)((long)ConVar.Server.maxpacketspersecond_world))
					{
						Network.Net.sv.Kick(packet.connection, "Packet Flooding: World", packet.connection.connected);
						return;
					}
					using (TimeWarning.New("OnWorldMessage", 20))
					{
						try
						{
							WorldNetworking.OnMessageReceived(packet);
						}
						catch (Exception e8)
						{
							this.Log(e8);
							Network.Net.sv.Kick(packet.connection, "Invalid Packet: World", false);
						}
					}
					return;
				}
				break;
			}
			this.ProcessUnhandledPacket(packet);
			return;
		}
		if (!packet.connection.connected)
		{
			return;
		}
		if (packet.connection.GetPacketsPerSecond(packet.type) >= 1UL)
		{
			Network.Net.sv.Kick(packet.connection, "Packet Flooding: Client Ready", packet.connection.connected);
			return;
		}
		using (TimeWarning.New("ClientReady", 20))
		{
			try
			{
				this.ClientReady(packet);
			}
			catch (Exception e9)
			{
				this.Log(e9);
				Network.Net.sv.Kick(packet.connection, "Invalid Packet: Client Ready", false);
			}
		}
		packet.connection.AddPacketsPerSecond(packet.type);
	}

	// Token: 0x060033DF RID: 13279 RVA: 0x0013FA88 File Offset: 0x0013DC88
	public void ProcessUnhandledPacket(Message packet)
	{
		if (ConVar.Global.developer > 0)
		{
			Debug.LogWarning("[SERVER][UNHANDLED] " + packet.type);
		}
		Network.Net.sv.Kick(packet.connection, "Sent Unhandled Message", false);
	}

	// Token: 0x060033E0 RID: 13280 RVA: 0x0013FAC4 File Offset: 0x0013DCC4
	public void ReadDisconnectReason(Message packet)
	{
		string text = packet.read.String(4096);
		string text2 = packet.connection.ToString();
		if (!string.IsNullOrEmpty(text) && !string.IsNullOrEmpty(text2))
		{
			DebugEx.Log(text2 + " disconnecting: " + text, StackTraceLogType.None);
		}
	}

	// Token: 0x060033E1 RID: 13281 RVA: 0x0013FB10 File Offset: 0x0013DD10
	private global::BasePlayer SpawnPlayerSleeping(Network.Connection connection)
	{
		global::BasePlayer basePlayer = global::BasePlayer.FindSleeping(connection.userid);
		if (basePlayer == null)
		{
			return null;
		}
		if (!basePlayer.IsSleeping())
		{
			Debug.LogWarning("Player spawning into sleeper that isn't sleeping!");
			basePlayer.Kill(global::BaseNetworkable.DestroyMode.None);
			return null;
		}
		basePlayer.PlayerInit(connection);
		basePlayer.inventory.SendSnapshot();
		DebugEx.Log(string.Concat(new object[]
		{
			basePlayer.net.connection.ToString(),
			" joined [",
			basePlayer.net.connection.os,
			"/",
			basePlayer.net.connection.ownerid,
			"]"
		}), StackTraceLogType.None);
		return basePlayer;
	}

	// Token: 0x060033E2 RID: 13282 RVA: 0x0013FBCC File Offset: 0x0013DDCC
	private global::BasePlayer SpawnNewPlayer(Network.Connection connection)
	{
		global::BasePlayer.SpawnPoint spawnPoint = ServerMgr.FindSpawnPoint(null);
		global::BasePlayer basePlayer = GameManager.server.CreateEntity("assets/prefabs/player/player.prefab", spawnPoint.pos, spawnPoint.rot, true).ToPlayer();
		basePlayer.health = 0f;
		basePlayer.lifestate = BaseCombatEntity.LifeState.Dead;
		basePlayer.ResetLifeStateOnSpawn = false;
		basePlayer.limitNetworking = true;
		basePlayer.Spawn();
		basePlayer.limitNetworking = false;
		basePlayer.PlayerInit(connection);
		if (BaseGameMode.GetActiveGameMode(true))
		{
			BaseGameMode.GetActiveGameMode(true).OnNewPlayer(basePlayer);
		}
		else if (UnityEngine.Application.isEditor || (global::SleepingBag.FindForPlayer(basePlayer.userID, true).Length == 0 && !basePlayer.hasPreviousLife))
		{
			basePlayer.Respawn();
		}
		DebugEx.Log(string.Format("{0} with steamid {1} joined from ip {2}", basePlayer.displayName, basePlayer.userID, basePlayer.net.connection.ipaddress), StackTraceLogType.None);
		DebugEx.Log(string.Format("\tNetworkId {0} is {1} ({2})", basePlayer.userID, basePlayer.net.ID, basePlayer.displayName), StackTraceLogType.None);
		if (basePlayer.net.connection.ownerid != basePlayer.net.connection.userid)
		{
			DebugEx.Log(string.Format("\t{0} is sharing the account {1}", basePlayer, basePlayer.net.connection.ownerid), StackTraceLogType.None);
		}
		return basePlayer;
	}

	// Token: 0x060033E3 RID: 13283 RVA: 0x0013FD20 File Offset: 0x0013DF20
	private void ClientReady(Message packet)
	{
		if (packet.connection.state != Network.Connection.State.Welcoming)
		{
			Network.Net.sv.Kick(packet.connection, "Invalid connection state", false);
			return;
		}
		using (ClientReady clientReady = ProtoBuf.ClientReady.Deserialize(packet.read))
		{
			foreach (ClientReady.ClientInfo clientInfo in clientReady.clientInfo)
			{
				packet.connection.info.Set(clientInfo.name, clientInfo.value);
			}
			this.connectionQueue.JoinedGame(packet.connection);
			Analytics.Azure.OnPlayerConnected(packet.connection);
			using (TimeWarning.New("ClientReady", 0))
			{
				global::BasePlayer basePlayer;
				using (TimeWarning.New("SpawnPlayerSleeping", 0))
				{
					basePlayer = this.SpawnPlayerSleeping(packet.connection);
				}
				if (basePlayer == null)
				{
					using (TimeWarning.New("SpawnNewPlayer", 0))
					{
						basePlayer = this.SpawnNewPlayer(packet.connection);
					}
				}
				basePlayer.SendRespawnOptions();
				if (basePlayer != null)
				{
					Util.SendSignedInNotification(basePlayer);
				}
			}
		}
		ServerMgr.SendReplicatedVars(packet.connection);
	}

	// Token: 0x060033E4 RID: 13284 RVA: 0x0013FEA8 File Offset: 0x0013E0A8
	private void OnRPCMessage(Message packet)
	{
		NetworkableId uid = packet.read.EntityID();
		uint num = packet.read.UInt32();
		if (ConVar.Server.rpclog_enabled)
		{
			this.rpcHistory.Increment(num);
		}
		global::BaseEntity baseEntity = global::BaseNetworkable.serverEntities.Find(uid) as global::BaseEntity;
		if (baseEntity == null)
		{
			return;
		}
		baseEntity.SV_RPCMessage(num, packet);
	}

	// Token: 0x060033E5 RID: 13285 RVA: 0x0013FF04 File Offset: 0x0013E104
	private void OnPlayerTick(Message packet)
	{
		global::BasePlayer basePlayer = packet.Player();
		if (basePlayer == null)
		{
			return;
		}
		basePlayer.OnReceivedTick(packet.read);
	}

	// Token: 0x060033E6 RID: 13286 RVA: 0x0013FF30 File Offset: 0x0013E130
	private void OnPlayerVoice(Message packet)
	{
		global::BasePlayer basePlayer = packet.Player();
		if (basePlayer == null)
		{
			return;
		}
		basePlayer.OnReceivedVoice(packet.read.BytesWithSize(10485760U));
	}

	// Token: 0x060033E7 RID: 13287 RVA: 0x0013FF64 File Offset: 0x0013E164
	private void OnGiveUserInformation(Message packet)
	{
		if (packet.connection.state != Network.Connection.State.Unconnected)
		{
			Network.Net.sv.Kick(packet.connection, "Invalid connection state", false);
			return;
		}
		packet.connection.state = Network.Connection.State.Connecting;
		if (packet.read.UInt8() != 228)
		{
			Network.Net.sv.Kick(packet.connection, "Invalid Connection Protocol", false);
			return;
		}
		packet.connection.userid = packet.read.UInt64();
		packet.connection.protocol = packet.read.UInt32();
		packet.connection.os = packet.read.String(128);
		packet.connection.username = packet.read.String(256);
		if (string.IsNullOrEmpty(packet.connection.os))
		{
			throw new Exception("Invalid OS");
		}
		if (string.IsNullOrEmpty(packet.connection.username))
		{
			Network.Net.sv.Kick(packet.connection, "Invalid Username", false);
			return;
		}
		packet.connection.username = packet.connection.username.Replace('\n', ' ').Replace('\r', ' ').Replace('\t', ' ').Trim();
		if (string.IsNullOrEmpty(packet.connection.username))
		{
			Network.Net.sv.Kick(packet.connection, "Invalid Username", false);
			return;
		}
		string text = string.Empty;
		string branch = ConVar.Server.branch;
		if (packet.read.Unread >= 4)
		{
			text = packet.read.String(128);
		}
		if (branch != string.Empty && branch != text)
		{
			DebugEx.Log(string.Concat(new object[]
			{
				"Kicking ",
				packet.connection,
				" - their branch is '",
				text,
				"' not '",
				branch,
				"'"
			}), StackTraceLogType.None);
			Network.Net.sv.Kick(packet.connection, "Wrong Steam Beta: Requires '" + branch + "' branch!", false);
			return;
		}
		if (packet.connection.protocol > 2392U)
		{
			DebugEx.Log(string.Concat(new object[]
			{
				"Kicking ",
				packet.connection,
				" - their protocol is ",
				packet.connection.protocol,
				" not ",
				2392
			}), StackTraceLogType.None);
			Network.Net.sv.Kick(packet.connection, "Wrong Connection Protocol: Server update required!", false);
			return;
		}
		if (packet.connection.protocol < 2392U)
		{
			DebugEx.Log(string.Concat(new object[]
			{
				"Kicking ",
				packet.connection,
				" - their protocol is ",
				packet.connection.protocol,
				" not ",
				2392
			}), StackTraceLogType.None);
			Network.Net.sv.Kick(packet.connection, "Wrong Connection Protocol: Client update required!", false);
			return;
		}
		packet.connection.token = packet.read.BytesWithSize(512U);
		if (packet.connection.token == null || packet.connection.token.Length < 1)
		{
			Network.Net.sv.Kick(packet.connection, "Invalid Token", false);
			return;
		}
		this.auth.OnNewConnection(packet.connection);
	}

	// Token: 0x17000448 RID: 1096
	// (get) Token: 0x060033E8 RID: 13288 RVA: 0x001402CC File Offset: 0x0013E4CC
	// (set) Token: 0x060033E9 RID: 13289 RVA: 0x001402D4 File Offset: 0x0013E4D4
	public bool runFrameUpdate { get; private set; }

	// Token: 0x060033EA RID: 13290 RVA: 0x001402E0 File Offset: 0x0013E4E0
	public void Initialize(bool loadSave = true, string saveFile = "", bool allowOutOfDateSaves = false, bool skipInitialSpawn = false)
	{
		this.persistance = new UserPersistance(ConVar.Server.rootFolder);
		this.playerStateManager = new PlayerStateManager(this.persistance);
		this.SpawnMapEntities();
		if (SingletonComponent<SpawnHandler>.Instance)
		{
			using (TimeWarning.New("SpawnHandler.UpdateDistributions", 0))
			{
				SingletonComponent<SpawnHandler>.Instance.UpdateDistributions();
			}
		}
		if (loadSave)
		{
			global::World.LoadedFromSave = true;
			skipInitialSpawn = (global::World.LoadedFromSave = SaveRestore.Load(saveFile, allowOutOfDateSaves));
		}
		else
		{
			SaveRestore.SaveCreatedTime = DateTime.UtcNow;
			global::World.LoadedFromSave = false;
		}
		SaveRestore.InitializeWipeId();
		if (SingletonComponent<SpawnHandler>.Instance)
		{
			if (!skipInitialSpawn)
			{
				using (TimeWarning.New("SpawnHandler.InitialSpawn", 200))
				{
					SingletonComponent<SpawnHandler>.Instance.InitialSpawn();
				}
			}
			using (TimeWarning.New("SpawnHandler.StartSpawnTick", 200))
			{
				SingletonComponent<SpawnHandler>.Instance.StartSpawnTick();
			}
		}
		this.CreateImportantEntities();
		this.auth = base.GetComponent<ConnectionAuth>();
		Analytics.Azure.Initialize();
	}

	// Token: 0x060033EB RID: 13291 RVA: 0x0014040C File Offset: 0x0013E60C
	public void OpenConnection()
	{
		if (ConVar.Server.queryport <= 0 || ConVar.Server.queryport == ConVar.Server.port)
		{
			ConVar.Server.queryport = Math.Max(ConVar.Server.port, RCon.Port) + 1;
		}
		Network.Net.sv.ip = ConVar.Server.ip;
		Network.Net.sv.port = ConVar.Server.port;
		this.StartSteamServer();
		if (!Network.Net.sv.Start())
		{
			Debug.LogWarning("Couldn't Start Server.");
			this.CloseConnection();
			return;
		}
		Network.Net.sv.callbackHandler = this;
		Network.Net.sv.cryptography = new NetworkCryptographyServer();
		EACServer.DoStartup();
		base.InvokeRepeating("DoTick", 1f, 1f / (float)ConVar.Server.tickrate);
		base.InvokeRepeating("DoHeartbeat", 1f, 1f);
		this.runFrameUpdate = true;
		ConsoleSystem.OnReplicatedVarChanged += ServerMgr.OnReplicatedVarChanged;
	}

	// Token: 0x060033EC RID: 13292 RVA: 0x001404EC File Offset: 0x0013E6EC
	private void CloseConnection()
	{
		if (this.persistance != null)
		{
			this.persistance.Dispose();
			this.persistance = null;
		}
		EACServer.DoShutdown();
		Network.Net.sv.callbackHandler = null;
		using (TimeWarning.New("sv.Stop", 0))
		{
			Network.Net.sv.Stop("Shutting Down");
		}
		using (TimeWarning.New("RCon.Shutdown", 0))
		{
			RCon.Shutdown();
		}
		using (TimeWarning.New("PlatformService.Shutdown", 0))
		{
			IPlatformService instance = PlatformService.Instance;
			if (instance != null)
			{
				instance.Shutdown();
			}
		}
		using (TimeWarning.New("CompanionServer.Shutdown", 0))
		{
			CompanionServer.Server.Shutdown();
		}
		ConsoleSystem.OnReplicatedVarChanged -= ServerMgr.OnReplicatedVarChanged;
	}

	// Token: 0x060033ED RID: 13293 RVA: 0x001405F4 File Offset: 0x0013E7F4
	private void OnDisable()
	{
		if (Rust.Application.isQuitting)
		{
			return;
		}
		this.CloseConnection();
	}

	// Token: 0x060033EE RID: 13294 RVA: 0x00140604 File Offset: 0x0013E804
	private void OnApplicationQuit()
	{
		Rust.Application.isQuitting = true;
		this.CloseConnection();
	}

	// Token: 0x060033EF RID: 13295 RVA: 0x00140612 File Offset: 0x0013E812
	private void CreateImportantEntities()
	{
		this.CreateImportantEntity<EnvSync>("assets/bundled/prefabs/system/net_env.prefab");
		this.CreateImportantEntity<CommunityEntity>("assets/bundled/prefabs/system/server/community.prefab");
		this.CreateImportantEntity<ResourceDepositManager>("assets/bundled/prefabs/system/server/resourcedepositmanager.prefab");
		this.CreateImportantEntity<global::RelationshipManager>("assets/bundled/prefabs/system/server/relationship_manager.prefab");
		this.CreateImportantEntity<TreeManager>("assets/bundled/prefabs/system/tree_manager.prefab");
	}

	// Token: 0x060033F0 RID: 13296 RVA: 0x0014064C File Offset: 0x0013E84C
	public void CreateImportantEntity<T>(string prefabName) where T : global::BaseEntity
	{
		if (global::BaseNetworkable.serverEntities.Any((global::BaseNetworkable x) => x is T))
		{
			return;
		}
		Debug.LogWarning("Missing " + typeof(T).Name + " - creating");
		global::BaseEntity baseEntity = GameManager.server.CreateEntity(prefabName, default(Vector3), default(Quaternion), true);
		if (baseEntity == null)
		{
			Debug.LogWarning("Couldn't create");
			return;
		}
		baseEntity.Spawn();
	}

	// Token: 0x060033F1 RID: 13297 RVA: 0x001406E4 File Offset: 0x0013E8E4
	private void StartSteamServer()
	{
		PlatformService.Instance.Initialize(RustPlatformHooks.Instance);
		base.InvokeRepeating("UpdateServerInformation", 2f, 30f);
		base.InvokeRepeating("UpdateItemDefinitions", 10f, 3600f);
		DebugEx.Log("SteamServer Initialized", StackTraceLogType.None);
	}

	// Token: 0x060033F2 RID: 13298 RVA: 0x00140736 File Offset: 0x0013E936
	private void UpdateItemDefinitions()
	{
		Debug.Log("Checking for new Steam Item Definitions..");
		PlatformService.Instance.RefreshItemDefinitions();
	}

	// Token: 0x060033F3 RID: 13299 RVA: 0x0014074C File Offset: 0x0013E94C
	internal void OnValidateAuthTicketResponse(ulong SteamId, ulong OwnerId, global::AuthResponse Status)
	{
		if (Auth_Steam.ValidateConnecting(SteamId, OwnerId, Status))
		{
			return;
		}
		Network.Connection connection = Network.Net.sv.connections.FirstOrDefault((Network.Connection x) => x.userid == SteamId);
		if (connection == null)
		{
			Debug.LogWarning(string.Format("Steam gave us a {0} ticket response for unconnected id {1}", Status, SteamId));
			return;
		}
		if (Status == global::AuthResponse.OK)
		{
			Debug.LogWarning(string.Format("Steam gave us a 'ok' ticket response for already connected id {0}", SteamId));
			return;
		}
		if (Status == global::AuthResponse.TimedOut)
		{
			return;
		}
		if ((Status == global::AuthResponse.PublisherBanned || Status == global::AuthResponse.VACBanned) && !this.bannedPlayerNotices.Contains(SteamId))
		{
			ConsoleNetwork.BroadcastToAllClients("chat.add", new object[]
			{
				2,
				0,
				"<color=#fff>SERVER</color> Kicking " + connection.username.EscapeRichText() + " (banned by anticheat)"
			});
			this.bannedPlayerNotices.Add(SteamId);
		}
		Debug.Log(string.Format("Kicking {0}/{1}/{2} (Steam Status \"{3}\")", new object[]
		{
			connection.ipaddress,
			connection.userid,
			connection.username,
			Status.ToString()
		}));
		connection.authStatus = Status.ToString();
		Network.Net.sv.Kick(connection, "Steam: " + Status.ToString(), false);
	}

	// Token: 0x17000449 RID: 1097
	// (get) Token: 0x060033F4 RID: 13300 RVA: 0x001408C0 File Offset: 0x0013EAC0
	public static int AvailableSlots
	{
		get
		{
			return ConVar.Server.maxplayers - global::BasePlayer.activePlayerList.Count;
		}
	}

	// Token: 0x060033F5 RID: 13301 RVA: 0x001408D4 File Offset: 0x0013EAD4
	private void Update()
	{
		if (!this.runFrameUpdate)
		{
			return;
		}
		Facepunch.Models.Manifest manifest = Facepunch.Application.Manifest;
		if (manifest != null && manifest.Features.ServerAnalytics)
		{
			try
			{
				PerformanceLogging.server.OnFrame();
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
			}
		}
		using (TimeWarning.New("ServerMgr.Update", 500))
		{
			try
			{
				using (TimeWarning.New("EACServer.DoUpdate", 100))
				{
					EACServer.DoUpdate();
				}
			}
			catch (Exception exception2)
			{
				Debug.LogWarning("Server Exception: EACServer.DoUpdate");
				Debug.LogException(exception2, this);
			}
			try
			{
				using (TimeWarning.New("PlatformService.Update", 100))
				{
					PlatformService.Instance.Update();
				}
			}
			catch (Exception exception3)
			{
				Debug.LogWarning("Server Exception: Platform Service Update");
				Debug.LogException(exception3, this);
			}
			try
			{
				using (TimeWarning.New("Net.sv.Cycle", 100))
				{
					Network.Net.sv.Cycle();
				}
			}
			catch (Exception exception4)
			{
				Debug.LogWarning("Server Exception: Network Cycle");
				Debug.LogException(exception4, this);
			}
			try
			{
				using (TimeWarning.New("ServerBuildingManager.Cycle", 0))
				{
					BuildingManager.server.Cycle();
				}
			}
			catch (Exception exception5)
			{
				Debug.LogWarning("Server Exception: Building Manager");
				Debug.LogException(exception5, this);
			}
			try
			{
				using (TimeWarning.New("BasePlayer.ServerCycle", 0))
				{
					bool batchsynctransforms = ConVar.Physics.batchsynctransforms;
					bool autosynctransforms = ConVar.Physics.autosynctransforms;
					if (batchsynctransforms && autosynctransforms)
					{
						UnityEngine.Physics.autoSyncTransforms = false;
					}
					if (!UnityEngine.Physics.autoSyncTransforms)
					{
						UnityEngine.Physics.SyncTransforms();
					}
					try
					{
						using (TimeWarning.New("CameraRendererManager.Tick", 100))
						{
							CameraRendererManager instance = SingletonComponent<CameraRendererManager>.Instance;
							if (instance != null)
							{
								instance.Tick();
							}
						}
					}
					catch (Exception exception6)
					{
						Debug.LogWarning("Server Exception: CameraRendererManager.Tick");
						Debug.LogException(exception6, this);
					}
					global::BasePlayer.ServerCycle(UnityEngine.Time.deltaTime);
					try
					{
						using (TimeWarning.New("FlameTurret.BudgetedUpdate", 0))
						{
							FlameTurret.updateFlameTurretQueueServer.RunQueue(0.25);
						}
					}
					catch (Exception exception7)
					{
						Debug.LogWarning("Server Exception: FlameTurret.BudgetedUpdate");
						Debug.LogException(exception7, this);
					}
					try
					{
						using (TimeWarning.New("AutoTurret.BudgetedUpdate", 0))
						{
							global::AutoTurret.updateAutoTurretScanQueue.RunQueue(0.5);
						}
					}
					catch (Exception exception8)
					{
						Debug.LogWarning("Server Exception: AutoTurret.BudgetedUpdate");
						Debug.LogException(exception8, this);
					}
					try
					{
						using (TimeWarning.New("BaseFishingRod.BudgetedUpdate", 0))
						{
							BaseFishingRod.updateFishingRodQueue.RunQueue(1.0);
						}
					}
					catch (Exception exception9)
					{
						Debug.LogWarning("Server Exception: BaseFishingRod.BudgetedUpdate");
						Debug.LogException(exception9, this);
					}
					if (batchsynctransforms && autosynctransforms)
					{
						UnityEngine.Physics.autoSyncTransforms = true;
					}
				}
			}
			catch (Exception exception10)
			{
				Debug.LogWarning("Server Exception: Player Update");
				Debug.LogException(exception10, this);
			}
			try
			{
				using (TimeWarning.New("connectionQueue.Cycle", 0))
				{
					this.connectionQueue.Cycle(ServerMgr.AvailableSlots);
				}
			}
			catch (Exception exception11)
			{
				Debug.LogWarning("Server Exception: Connection Queue");
				Debug.LogException(exception11, this);
			}
			try
			{
				using (TimeWarning.New("IOEntity.ProcessQueue", 0))
				{
					global::IOEntity.ProcessQueue();
				}
			}
			catch (Exception exception12)
			{
				Debug.LogWarning("Server Exception: IOEntity.ProcessQueue");
				Debug.LogException(exception12, this);
			}
			if (!AI.spliceupdates)
			{
				this.aiTick = AIThinkManager.QueueType.Human;
			}
			else
			{
				this.aiTick = ((this.aiTick == AIThinkManager.QueueType.Human) ? AIThinkManager.QueueType.Animal : AIThinkManager.QueueType.Human);
			}
			if (this.aiTick == AIThinkManager.QueueType.Human)
			{
				try
				{
					using (TimeWarning.New("AIThinkManager.ProcessQueue", 0))
					{
						AIThinkManager.ProcessQueue(AIThinkManager.QueueType.Human);
					}
				}
				catch (Exception exception13)
				{
					Debug.LogWarning("Server Exception: AIThinkManager.ProcessQueue");
					Debug.LogException(exception13, this);
				}
				if (!AI.spliceupdates)
				{
					this.aiTick = AIThinkManager.QueueType.Animal;
				}
			}
			if (this.aiTick == AIThinkManager.QueueType.Animal)
			{
				try
				{
					using (TimeWarning.New("AIThinkManager.ProcessAnimalQueue", 0))
					{
						AIThinkManager.ProcessQueue(AIThinkManager.QueueType.Animal);
					}
				}
				catch (Exception exception14)
				{
					Debug.LogWarning("Server Exception: AIThinkManager.ProcessAnimalQueue");
					Debug.LogException(exception14, this);
				}
			}
			try
			{
				using (TimeWarning.New("AIThinkManager.ProcessPetQueue", 0))
				{
					AIThinkManager.ProcessQueue(AIThinkManager.QueueType.Pets);
				}
			}
			catch (Exception exception15)
			{
				Debug.LogWarning("Server Exception: AIThinkManager.ProcessPetQueue");
				Debug.LogException(exception15, this);
			}
			try
			{
				using (TimeWarning.New("AIThinkManager.ProcessPetMovementQueue", 0))
				{
					BasePet.ProcessMovementQueue();
				}
			}
			catch (Exception exception16)
			{
				Debug.LogWarning("Server Exception: AIThinkManager.ProcessPetMovementQueue");
				Debug.LogException(exception16, this);
			}
			try
			{
				using (TimeWarning.New("BaseRidableAnimal.ProcessQueue", 0))
				{
					BaseRidableAnimal.ProcessQueue();
				}
			}
			catch (Exception exception17)
			{
				Debug.LogWarning("Server Exception: BaseRidableAnimal.ProcessQueue");
				Debug.LogException(exception17, this);
			}
			try
			{
				using (TimeWarning.New("GrowableEntity.BudgetedUpdate", 0))
				{
					global::GrowableEntity.growableEntityUpdateQueue.RunQueue((double)global::GrowableEntity.framebudgetms);
				}
			}
			catch (Exception exception18)
			{
				Debug.LogWarning("Server Exception: GrowableEntity.BudgetedUpdate");
				Debug.LogException(exception18, this);
			}
			try
			{
				using (TimeWarning.New("BasePlayer.BudgetedLifeStoryUpdate", 0))
				{
					global::BasePlayer.lifeStoryQueue.RunQueue((double)global::BasePlayer.lifeStoryFramebudgetms);
				}
			}
			catch (Exception exception19)
			{
				Debug.LogWarning("Server Exception: BasePlayer.BudgetedLifeStoryUpdate");
				Debug.LogException(exception19, this);
			}
			try
			{
				using (TimeWarning.New("JunkPileWater.UpdateNearbyPlayers", 0))
				{
					JunkPileWater.junkpileWaterWorkQueue.RunQueue((double)JunkPileWater.framebudgetms);
				}
			}
			catch (Exception exception20)
			{
				Debug.LogWarning("Server Exception: JunkPileWater.UpdateNearbyPlayers");
				Debug.LogException(exception20, this);
			}
			try
			{
				using (TimeWarning.New("IndustrialEntity.RunQueue", 0))
				{
					IndustrialEntity.Queue.RunQueue((double)ConVar.Server.industrialFrameBudgetMs);
				}
			}
			catch (Exception exception21)
			{
				Debug.LogWarning("Server Exception: IndustrialEntity.RunQueue");
				Debug.LogException(exception21, this);
			}
			try
			{
				using (TimeWarning.New("AntiHack.Cycle", 0))
				{
					global::AntiHack.Cycle();
				}
			}
			catch (Exception exception22)
			{
				Debug.LogWarning("Server Exception: AntiHack.Cycle");
				Debug.LogException(exception22, this);
			}
		}
	}

	// Token: 0x060033F6 RID: 13302 RVA: 0x00141274 File Offset: 0x0013F474
	private void LateUpdate()
	{
		if (!this.runFrameUpdate)
		{
			return;
		}
		using (TimeWarning.New("ServerMgr.LateUpdate", 500))
		{
			if (Facepunch.Network.SteamNetworking.steamnagleflush)
			{
				try
				{
					using (TimeWarning.New("Connection.Flush", 0))
					{
						for (int i = 0; i < Network.Net.sv.connections.Count; i++)
						{
							Network.Net.sv.Flush(Network.Net.sv.connections[i]);
						}
					}
				}
				catch (Exception exception)
				{
					Debug.LogWarning("Server Exception: Connection.Flush");
					Debug.LogException(exception, this);
				}
			}
		}
	}

	// Token: 0x060033F7 RID: 13303 RVA: 0x00141338 File Offset: 0x0013F538
	private void FixedUpdate()
	{
		using (TimeWarning.New("ServerMgr.FixedUpdate", 0))
		{
			try
			{
				using (TimeWarning.New("BaseMountable.FixedUpdateCycle", 0))
				{
					BaseMountable.FixedUpdateCycle();
				}
			}
			catch (Exception exception)
			{
				Debug.LogWarning("Server Exception: Mountable Cycle");
				Debug.LogException(exception, this);
			}
			try
			{
				using (TimeWarning.New("Buoyancy.Cycle", 0))
				{
					Buoyancy.Cycle();
				}
			}
			catch (Exception exception2)
			{
				Debug.LogWarning("Server Exception: Buoyancy Cycle");
				Debug.LogException(exception2, this);
			}
		}
	}

	// Token: 0x060033F8 RID: 13304 RVA: 0x00141400 File Offset: 0x0013F600
	private void DoTick()
	{
		RCon.Update();
		CompanionServer.Server.Update();
		for (int i = 0; i < Network.Net.sv.connections.Count; i++)
		{
			Network.Connection connection = Network.Net.sv.connections[i];
			if (!connection.isAuthenticated && connection.GetSecondsConnected() >= (float)ConVar.Server.authtimeout)
			{
				Network.Net.sv.Kick(connection, "Authentication Timed Out", false);
			}
		}
	}

	// Token: 0x060033F9 RID: 13305 RVA: 0x00141469 File Offset: 0x0013F669
	private void DoHeartbeat()
	{
		ItemManager.Heartbeat();
	}

	// Token: 0x1700044A RID: 1098
	// (get) Token: 0x060033FA RID: 13306 RVA: 0x00141470 File Offset: 0x0013F670
	private string AssemblyHash
	{
		get
		{
			if (this._AssemblyHash == null)
			{
				string location = typeof(ServerMgr).Assembly.Location;
				if (!string.IsNullOrEmpty(location))
				{
					byte[] array = File.ReadAllBytes(location);
					CRC32 crc = new CRC32();
					crc.SlurpBlock(array, 0, array.Length);
					this._AssemblyHash = crc.Crc32Result.ToString("x");
				}
				else
				{
					this._AssemblyHash = "il2cpp";
				}
			}
			return this._AssemblyHash;
		}
	}

	// Token: 0x060033FB RID: 13307 RVA: 0x001414E8 File Offset: 0x0013F6E8
	private static BaseGameMode Gamemode()
	{
		BaseGameMode activeGameMode = BaseGameMode.GetActiveGameMode(true);
		if (!(activeGameMode != null))
		{
			return null;
		}
		return activeGameMode;
	}

	// Token: 0x060033FC RID: 13308 RVA: 0x00141508 File Offset: 0x0013F708
	public static string GamemodeName()
	{
		BaseGameMode baseGameMode = ServerMgr.Gamemode();
		return ((baseGameMode != null) ? baseGameMode.shortname : null) ?? "rust";
	}

	// Token: 0x060033FD RID: 13309 RVA: 0x00141524 File Offset: 0x0013F724
	public static string GamemodeTitle()
	{
		BaseGameMode baseGameMode = ServerMgr.Gamemode();
		return ((baseGameMode != null) ? baseGameMode.gamemodeTitle : null) ?? "Survival";
	}

	// Token: 0x060033FE RID: 13310 RVA: 0x00141540 File Offset: 0x0013F740
	private void UpdateServerInformation()
	{
		if (!SteamServer.IsValid)
		{
			return;
		}
		using (TimeWarning.New("UpdateServerInformation", 0))
		{
			SteamServer.ServerName = ConVar.Server.hostname;
			SteamServer.MaxPlayers = ConVar.Server.maxplayers;
			SteamServer.Passworded = false;
			SteamServer.MapName = global::World.GetServerBrowserMapName();
			string text = "stok";
			if (this.Restarting)
			{
				text = "strst";
			}
			string text2 = string.Format("born{0}", Epoch.FromDateTime(SaveRestore.SaveCreatedTime));
			string text3 = string.Format("gm{0}", ServerMgr.GamemodeName());
			string text4 = ConVar.Server.pve ? ",pve" : string.Empty;
			string tags = ConVar.Server.tags;
			string text5 = ((tags != null) ? tags.Trim(new char[]
			{
				','
			}) : null) ?? "";
			string text6 = (!string.IsNullOrWhiteSpace(text5)) ? ("," + text5) : "";
			BuildInfo buildInfo = BuildInfo.Current;
			string text7;
			if (buildInfo == null)
			{
				text7 = null;
			}
			else
			{
				BuildInfo.ScmInfo scm = buildInfo.Scm;
				text7 = ((scm != null) ? scm.ChangeId : null);
			}
			string text8 = text7 ?? "0";
			SteamServer.GameTags = string.Format("mp{0},cp{1},pt{2},qp{3},v{4}{5}{6},h{7},{8},{9},{10},cs{11}", new object[]
			{
				ConVar.Server.maxplayers,
				global::BasePlayer.activePlayerList.Count,
				Network.Net.sv.ProtocolId,
				SingletonComponent<ServerMgr>.Instance.connectionQueue.Queued,
				2392,
				text4,
				text6,
				this.AssemblyHash,
				text,
				text2,
				text3,
				text8
			});
			if (ConVar.Server.description != null && ConVar.Server.description.Length > 100)
			{
				string[] array = ConVar.Server.description.SplitToChunks(100).ToArray<string>();
				for (int i = 0; i < 16; i++)
				{
					if (i < array.Length)
					{
						SteamServer.SetKey(string.Format("description_{0:00}", i), array[i]);
					}
					else
					{
						SteamServer.SetKey(string.Format("description_{0:00}", i), string.Empty);
					}
				}
			}
			else
			{
				SteamServer.SetKey("description_0", ConVar.Server.description);
				for (int j = 1; j < 16; j++)
				{
					SteamServer.SetKey(string.Format("description_{0:00}", j), string.Empty);
				}
			}
			SteamServer.SetKey("hash", this.AssemblyHash);
			string value = global::World.Seed.ToString();
			BaseGameMode activeGameMode = BaseGameMode.GetActiveGameMode(true);
			if (activeGameMode != null && !activeGameMode.ingameMap)
			{
				value = "0";
			}
			SteamServer.SetKey("world.seed", value);
			SteamServer.SetKey("world.size", global::World.Size.ToString());
			SteamServer.SetKey("pve", ConVar.Server.pve.ToString());
			SteamServer.SetKey("headerimage", ConVar.Server.headerimage);
			SteamServer.SetKey("logoimage", ConVar.Server.logoimage);
			SteamServer.SetKey("url", ConVar.Server.url);
			SteamServer.SetKey("gmn", ServerMgr.GamemodeName());
			SteamServer.SetKey("gmt", ServerMgr.GamemodeTitle());
			SteamServer.SetKey("uptime", ((int)UnityEngine.Time.realtimeSinceStartup).ToString());
			SteamServer.SetKey("gc_mb", global::Performance.report.memoryAllocations.ToString());
			SteamServer.SetKey("gc_cl", global::Performance.report.memoryCollections.ToString());
			SteamServer.SetKey("ram_sys", (global::Performance.report.memoryUsageSystem / 1000000L).ToString());
			SteamServer.SetKey("fps", global::Performance.report.frameRate.ToString());
			SteamServer.SetKey("fps_avg", global::Performance.report.frameRateAverage.ToString("0.00"));
			SteamServer.SetKey("ent_cnt", global::BaseNetworkable.serverEntities.Count.ToString());
			SteamServer.SetKey("build", BuildInfo.Current.Scm.ChangeId);
		}
	}

	// Token: 0x060033FF RID: 13311 RVA: 0x00141954 File Offset: 0x0013FB54
	public void OnDisconnected(string strReason, Network.Connection connection)
	{
		Analytics.Azure.OnPlayerDisconnected(connection, strReason);
		this.connectionQueue.RemoveConnection(connection);
		ConnectionAuth.OnDisconnect(connection);
		PlatformService.Instance.EndPlayerSession(connection.userid);
		EACServer.OnLeaveGame(connection);
		global::BasePlayer basePlayer = connection.player as global::BasePlayer;
		if (basePlayer)
		{
			basePlayer.OnDisconnected();
		}
	}

	// Token: 0x06003400 RID: 13312 RVA: 0x001419AA File Offset: 0x0013FBAA
	public static void OnEnterVisibility(Network.Connection connection, Group group)
	{
		if (!Network.Net.sv.IsConnected())
		{
			return;
		}
		NetWrite netWrite = Network.Net.sv.StartWrite();
		netWrite.PacketID(Message.Type.GroupEnter);
		netWrite.GroupID(group.ID);
		netWrite.Send(new SendInfo(connection));
	}

	// Token: 0x06003401 RID: 13313 RVA: 0x001419E4 File Offset: 0x0013FBE4
	public static void OnLeaveVisibility(Network.Connection connection, Group group)
	{
		if (!Network.Net.sv.IsConnected())
		{
			return;
		}
		NetWrite netWrite = Network.Net.sv.StartWrite();
		netWrite.PacketID(Message.Type.GroupLeave);
		netWrite.GroupID(group.ID);
		netWrite.Send(new SendInfo(connection));
		NetWrite netWrite2 = Network.Net.sv.StartWrite();
		netWrite2.PacketID(Message.Type.GroupDestroy);
		netWrite2.GroupID(group.ID);
		netWrite2.Send(new SendInfo(connection));
	}

	// Token: 0x06003402 RID: 13314 RVA: 0x00141A50 File Offset: 0x0013FC50
	public void SpawnMapEntities()
	{
		new PrefabPreProcess(false, true, false);
		global::BaseEntity[] array = UnityEngine.Object.FindObjectsOfType<global::BaseEntity>();
		global::BaseEntity[] array2 = array;
		for (int i = 0; i < array2.Length; i++)
		{
			array2[i].SpawnAsMapEntity();
		}
		DebugEx.Log(string.Format("Map Spawned {0} entities", array.Length), StackTraceLogType.None);
		foreach (global::BaseEntity baseEntity in array)
		{
			if (baseEntity != null)
			{
				baseEntity.PostMapEntitySpawn();
			}
		}
	}

	// Token: 0x06003403 RID: 13315 RVA: 0x00141AC0 File Offset: 0x0013FCC0
	public static global::BasePlayer.SpawnPoint FindSpawnPoint(global::BasePlayer forPlayer = null)
	{
		bool flag = false;
		BaseGameMode baseGameMode = ServerMgr.Gamemode();
		if (baseGameMode && baseGameMode.useCustomSpawns)
		{
			global::BasePlayer.SpawnPoint playerSpawn = baseGameMode.GetPlayerSpawn(forPlayer);
			if (playerSpawn != null)
			{
				return playerSpawn;
			}
		}
		if (SingletonComponent<SpawnHandler>.Instance != null & !flag)
		{
			global::BasePlayer.SpawnPoint spawnPoint = SpawnHandler.GetSpawnPoint();
			if (spawnPoint != null)
			{
				return spawnPoint;
			}
		}
		global::BasePlayer.SpawnPoint spawnPoint2 = new global::BasePlayer.SpawnPoint();
		GameObject[] array = GameObject.FindGameObjectsWithTag("spawnpoint");
		if (array.Length != 0)
		{
			GameObject gameObject = array[UnityEngine.Random.Range(0, array.Length)];
			spawnPoint2.pos = gameObject.transform.position;
			spawnPoint2.rot = gameObject.transform.rotation;
		}
		else
		{
			Debug.Log("Couldn't find an appropriate spawnpoint for the player - so spawning at camera");
			if (MainCamera.mainCamera != null)
			{
				spawnPoint2.pos = MainCamera.position;
				spawnPoint2.rot = MainCamera.rotation;
			}
		}
		RaycastHit raycastHit;
		if (UnityEngine.Physics.Raycast(new Ray(spawnPoint2.pos, Vector3.down), out raycastHit, 32f, 1537286401))
		{
			spawnPoint2.pos = raycastHit.point;
		}
		return spawnPoint2;
	}

	// Token: 0x06003404 RID: 13316 RVA: 0x00141BBC File Offset: 0x0013FDBC
	public void JoinGame(Network.Connection connection)
	{
		using (Approval approval = Facepunch.Pool.Get<Approval>())
		{
			uint num = (uint)ConVar.Server.encryption;
			if (num > 1U && connection.os == "editor" && DeveloperList.Contains(connection.ownerid))
			{
				num = 1U;
			}
			if (num > 1U && !ConVar.Server.secure)
			{
				num = 1U;
			}
			approval.level = UnityEngine.Application.loadedLevelName;
			approval.levelTransfer = global::World.Transfer;
			approval.levelUrl = global::World.Url;
			approval.levelSeed = global::World.Seed;
			approval.levelSize = global::World.Size;
			approval.checksum = global::World.Checksum;
			approval.hostname = ConVar.Server.hostname;
			approval.official = ConVar.Server.official;
			approval.encryption = num;
			approval.version = BuildInfo.Current.Scm.Branch + "#" + BuildInfo.Current.Scm.ChangeId;
			NetWrite netWrite = Network.Net.sv.StartWrite();
			netWrite.PacketID(Message.Type.Approved);
			approval.WriteToStream(netWrite);
			netWrite.Send(new SendInfo(connection));
			connection.encryptionLevel = num;
		}
		connection.connected = true;
	}

	// Token: 0x1700044B RID: 1099
	// (get) Token: 0x06003405 RID: 13317 RVA: 0x00141CE4 File Offset: 0x0013FEE4
	public bool Restarting
	{
		get
		{
			return this.restartCoroutine != null;
		}
	}

	// Token: 0x06003406 RID: 13318 RVA: 0x00141CF0 File Offset: 0x0013FEF0
	internal void Shutdown()
	{
		global::BasePlayer[] array = global::BasePlayer.activePlayerList.ToArray<global::BasePlayer>();
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Kick("Server Shutting Down");
		}
		ConsoleSystem.Run(ConsoleSystem.Option.Server, "server.save", Array.Empty<object>());
		ConsoleSystem.Run(ConsoleSystem.Option.Server, "server.writecfg", Array.Empty<object>());
	}

	// Token: 0x06003407 RID: 13319 RVA: 0x00141D4D File Offset: 0x0013FF4D
	private IEnumerator ServerRestartWarning(string info, int iSeconds)
	{
		if (iSeconds < 0)
		{
			yield break;
		}
		if (!string.IsNullOrEmpty(info))
		{
			ConsoleNetwork.BroadcastToAllClients("chat.add", new object[]
			{
				2,
				0,
				"<color=#fff>SERVER</color> Restarting: " + info
			});
		}
		int j;
		for (int i = iSeconds; i > 0; i = j - 1)
		{
			if (i == iSeconds || i % 60 == 0 || (i < 300 && i % 30 == 0) || (i < 60 && i % 10 == 0) || i < 10)
			{
				ConsoleNetwork.BroadcastToAllClients("chat.add", new object[]
				{
					2,
					0,
					string.Format("<color=#fff>SERVER</color> Restarting in {0} seconds ({1})!", i, info)
				});
				Debug.Log(string.Format("Restarting in {0} seconds", i));
			}
			yield return CoroutineEx.waitForSeconds(1f);
			j = i;
		}
		ConsoleNetwork.BroadcastToAllClients("chat.add", new object[]
		{
			2,
			0,
			"<color=#fff>SERVER</color> Restarting (" + info + ")"
		});
		yield return CoroutineEx.waitForSeconds(2f);
		global::BasePlayer[] array = global::BasePlayer.activePlayerList.ToArray<global::BasePlayer>();
		for (j = 0; j < array.Length; j++)
		{
			array[j].Kick("Server Restarting");
		}
		yield return CoroutineEx.waitForSeconds(1f);
		ConsoleSystem.Run(ConsoleSystem.Option.Server, "quit", Array.Empty<object>());
		yield break;
	}

	// Token: 0x06003408 RID: 13320 RVA: 0x00141D64 File Offset: 0x0013FF64
	public static void RestartServer(string strNotice, int iSeconds)
	{
		if (SingletonComponent<ServerMgr>.Instance == null)
		{
			return;
		}
		if (SingletonComponent<ServerMgr>.Instance.restartCoroutine != null)
		{
			ConsoleNetwork.BroadcastToAllClients("chat.add", new object[]
			{
				2,
				0,
				"<color=#fff>SERVER</color> Restart interrupted!"
			});
			SingletonComponent<ServerMgr>.Instance.StopCoroutine(SingletonComponent<ServerMgr>.Instance.restartCoroutine);
			SingletonComponent<ServerMgr>.Instance.restartCoroutine = null;
		}
		SingletonComponent<ServerMgr>.Instance.restartCoroutine = SingletonComponent<ServerMgr>.Instance.ServerRestartWarning(strNotice, iSeconds);
		SingletonComponent<ServerMgr>.Instance.StartCoroutine(SingletonComponent<ServerMgr>.Instance.restartCoroutine);
		SingletonComponent<ServerMgr>.Instance.UpdateServerInformation();
	}

	// Token: 0x06003409 RID: 13321 RVA: 0x00141E0C File Offset: 0x0014000C
	public static void SendReplicatedVars(string filter)
	{
		NetWrite netWrite = Network.Net.sv.StartWrite();
		List<Network.Connection> list = Facepunch.Pool.GetList<Network.Connection>();
		foreach (Network.Connection connection in Network.Net.sv.connections)
		{
			if (connection.connected)
			{
				list.Add(connection);
			}
		}
		List<ConsoleSystem.Command> list2 = Facepunch.Pool.GetList<ConsoleSystem.Command>();
		foreach (ConsoleSystem.Command command in ConsoleSystem.Index.Server.Replicated)
		{
			if (command.FullName.StartsWith(filter))
			{
				list2.Add(command);
			}
		}
		netWrite.PacketID(Message.Type.ConsoleReplicatedVars);
		netWrite.Int32(list2.Count);
		foreach (ConsoleSystem.Command command2 in list2)
		{
			netWrite.String(command2.FullName);
			netWrite.String(command2.String);
		}
		netWrite.Send(new SendInfo(list));
		Facepunch.Pool.FreeList<ConsoleSystem.Command>(ref list2);
		Facepunch.Pool.FreeList<Network.Connection>(ref list);
	}

	// Token: 0x0600340A RID: 13322 RVA: 0x00141F58 File Offset: 0x00140158
	public static void SendReplicatedVars(Network.Connection connection)
	{
		NetWrite netWrite = Network.Net.sv.StartWrite();
		List<ConsoleSystem.Command> replicated = ConsoleSystem.Index.Server.Replicated;
		netWrite.PacketID(Message.Type.ConsoleReplicatedVars);
		netWrite.Int32(replicated.Count);
		foreach (ConsoleSystem.Command command in replicated)
		{
			netWrite.String(command.FullName);
			netWrite.String(command.String);
		}
		netWrite.Send(new SendInfo(connection));
	}

	// Token: 0x0600340B RID: 13323 RVA: 0x00141FE8 File Offset: 0x001401E8
	private static void OnReplicatedVarChanged(string fullName, string value)
	{
		NetWrite netWrite = Network.Net.sv.StartWrite();
		List<Network.Connection> list = Facepunch.Pool.GetList<Network.Connection>();
		foreach (Network.Connection connection in Network.Net.sv.connections)
		{
			if (connection.connected)
			{
				list.Add(connection);
			}
		}
		netWrite.PacketID(Message.Type.ConsoleReplicatedVars);
		netWrite.Int32(1);
		netWrite.String(fullName);
		netWrite.String(value);
		netWrite.Send(new SendInfo(list));
		Facepunch.Pool.FreeList<Network.Connection>(ref list);
	}
}
