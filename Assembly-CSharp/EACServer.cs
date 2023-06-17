using System;
using System.Collections.Concurrent;
using ConVar;
using Epic.OnlineServices;
using Epic.OnlineServices.AntiCheatCommon;
using Epic.OnlineServices.AntiCheatServer;
using Epic.OnlineServices.Reports;
using Network;
using UnityEngine;

// Token: 0x02000741 RID: 1857
public static class EACServer
{
	// Token: 0x04002A13 RID: 10771
	private static AntiCheatServerInterface Interface = null;

	// Token: 0x04002A14 RID: 10772
	private static ReportsInterface Reports = null;

	// Token: 0x04002A15 RID: 10773
	private static ConcurrentDictionary<uint, Connection> client2connection = new ConcurrentDictionary<uint, Connection>();

	// Token: 0x04002A16 RID: 10774
	private static ConcurrentDictionary<Connection, uint> connection2client = new ConcurrentDictionary<Connection, uint>();

	// Token: 0x04002A17 RID: 10775
	private static ConcurrentDictionary<Connection, AntiCheatCommonClientAuthStatus> connection2status = new ConcurrentDictionary<Connection, AntiCheatCommonClientAuthStatus>();

	// Token: 0x04002A18 RID: 10776
	private static uint clientHandleCounter = 0U;

	// Token: 0x17000445 RID: 1093
	// (get) Token: 0x060033B7 RID: 13239 RVA: 0x0013DEB0 File Offset: 0x0013C0B0
	private static bool CanEnableGameplayData
	{
		get
		{
			return ConVar.Server.official && ConVar.Server.stats;
		}
	}

	// Token: 0x17000446 RID: 1094
	// (get) Token: 0x060033B8 RID: 13240 RVA: 0x0013DEC0 File Offset: 0x0013C0C0
	private static bool CanSendAnalytics
	{
		get
		{
			return EACServer.CanEnableGameplayData && EACServer.Interface != null;
		}
	}

	// Token: 0x17000447 RID: 1095
	// (get) Token: 0x060033B9 RID: 13241 RVA: 0x0013DED6 File Offset: 0x0013C0D6
	private static bool CanSendReports
	{
		get
		{
			return EACServer.Reports != null;
		}
	}

	// Token: 0x060033BA RID: 13242 RVA: 0x0013DEE3 File Offset: 0x0013C0E3
	private static IntPtr GenerateCompatibilityClient()
	{
		return (IntPtr)((long)((ulong)(EACServer.clientHandleCounter += 1U)));
	}

	// Token: 0x060033BB RID: 13243 RVA: 0x0013DEF8 File Offset: 0x0013C0F8
	public static void Encrypt(Connection connection, ArraySegment<byte> src, ref ArraySegment<byte> dst)
	{
		uint count = (uint)dst.Count;
		dst = new ArraySegment<byte>(dst.Array, dst.Offset, 0);
		if (EACServer.Interface != null)
		{
			IntPtr client = EACServer.GetClient(connection);
			if (client != IntPtr.Zero)
			{
				ProtectMessageOptions protectMessageOptions = new ProtectMessageOptions
				{
					ClientHandle = client,
					Data = src,
					OutBufferSizeBytes = count
				};
				uint count2;
				Result result = EACServer.Interface.ProtectMessage(ref protectMessageOptions, dst, out count2);
				if (result == Result.Success)
				{
					dst = new ArraySegment<byte>(dst.Array, dst.Offset, (int)count2);
					return;
				}
				Debug.LogWarning("[EAC] ProtectMessage failed: " + result);
			}
		}
	}

	// Token: 0x060033BC RID: 13244 RVA: 0x0013DFB0 File Offset: 0x0013C1B0
	public static void Decrypt(Connection connection, ArraySegment<byte> src, ref ArraySegment<byte> dst)
	{
		uint count = (uint)dst.Count;
		dst = new ArraySegment<byte>(dst.Array, dst.Offset, 0);
		if (EACServer.Interface != null)
		{
			IntPtr client = EACServer.GetClient(connection);
			if (client != IntPtr.Zero)
			{
				UnprotectMessageOptions unprotectMessageOptions = new UnprotectMessageOptions
				{
					ClientHandle = client,
					Data = src,
					OutBufferSizeBytes = count
				};
				uint count2;
				Result result = EACServer.Interface.UnprotectMessage(ref unprotectMessageOptions, dst, out count2);
				if (result == Result.Success)
				{
					dst = new ArraySegment<byte>(dst.Array, dst.Offset, (int)count2);
					return;
				}
				Debug.LogWarning("[EAC] UnprotectMessage failed: " + result);
			}
		}
	}

	// Token: 0x060033BD RID: 13245 RVA: 0x0013E068 File Offset: 0x0013C268
	private static IntPtr GetClient(Connection connection)
	{
		uint num;
		EACServer.connection2client.TryGetValue(connection, out num);
		return (IntPtr)((long)((ulong)num));
	}

	// Token: 0x060033BE RID: 13246 RVA: 0x0013E08C File Offset: 0x0013C28C
	private static Connection GetConnection(IntPtr client)
	{
		Connection result;
		EACServer.client2connection.TryGetValue((uint)((int)client), out result);
		return result;
	}

	// Token: 0x060033BF RID: 13247 RVA: 0x0013E0B0 File Offset: 0x0013C2B0
	public static bool IsAuthenticated(Connection connection)
	{
		AntiCheatCommonClientAuthStatus antiCheatCommonClientAuthStatus;
		EACServer.connection2status.TryGetValue(connection, out antiCheatCommonClientAuthStatus);
		return antiCheatCommonClientAuthStatus == AntiCheatCommonClientAuthStatus.RemoteAuthComplete;
	}

	// Token: 0x060033C0 RID: 13248 RVA: 0x0013E0CF File Offset: 0x0013C2CF
	private static void OnAuthenticatedLocal(Connection connection)
	{
		if (connection.authStatus == string.Empty)
		{
			connection.authStatus = "ok";
		}
		EACServer.connection2status[connection] = AntiCheatCommonClientAuthStatus.LocalAuthComplete;
	}

	// Token: 0x060033C1 RID: 13249 RVA: 0x0013E0FA File Offset: 0x0013C2FA
	private static void OnAuthenticatedRemote(Connection connection)
	{
		EACServer.connection2status[connection] = AntiCheatCommonClientAuthStatus.RemoteAuthComplete;
	}

	// Token: 0x060033C2 RID: 13250 RVA: 0x0013E108 File Offset: 0x0013C308
	private static void OnClientAuthStatusChanged(ref OnClientAuthStatusChangedCallbackInfo data)
	{
		using (TimeWarning.New("AntiCheatKickPlayer", 10))
		{
			IntPtr clientHandle = data.ClientHandle;
			Connection connection = EACServer.GetConnection(clientHandle);
			if (connection == null)
			{
				Debug.LogError("[EAC] Status update for invalid client: " + clientHandle.ToString());
			}
			else if (data.ClientAuthStatus == AntiCheatCommonClientAuthStatus.LocalAuthComplete)
			{
				EACServer.OnAuthenticatedLocal(connection);
				SetClientNetworkStateOptions setClientNetworkStateOptions = new SetClientNetworkStateOptions
				{
					ClientHandle = clientHandle,
					IsNetworkActive = false
				};
				EACServer.Interface.SetClientNetworkState(ref setClientNetworkStateOptions);
			}
			else if (data.ClientAuthStatus == AntiCheatCommonClientAuthStatus.RemoteAuthComplete)
			{
				EACServer.OnAuthenticatedRemote(connection);
			}
		}
	}

	// Token: 0x060033C3 RID: 13251 RVA: 0x0013E1B0 File Offset: 0x0013C3B0
	private static void OnClientActionRequired(ref OnClientActionRequiredCallbackInfo data)
	{
		using (TimeWarning.New("OnClientActionRequired", 10))
		{
			IntPtr clientHandle = data.ClientHandle;
			Connection connection = EACServer.GetConnection(clientHandle);
			if (connection == null)
			{
				Debug.LogError("[EAC] Status update for invalid client: " + clientHandle.ToString());
			}
			else
			{
				AntiCheatCommonClientAction clientAction = data.ClientAction;
				if (clientAction == AntiCheatCommonClientAction.RemovePlayer)
				{
					Utf8String actionReasonDetailsString = data.ActionReasonDetailsString;
					Debug.Log(string.Format("[EAC] Kicking {0} / {1} ({2})", connection.userid, connection.username, actionReasonDetailsString));
					connection.authStatus = "eac";
					Network.Net.sv.Kick(connection, "EAC: " + actionReasonDetailsString, false);
					if (data.ActionReasonCode == AntiCheatCommonClientActionReason.PermanentBanned || data.ActionReasonCode == AntiCheatCommonClientActionReason.TemporaryBanned)
					{
						connection.authStatus = "eacbanned";
						ConsoleNetwork.BroadcastToAllClients("chat.add", new object[]
						{
							2,
							0,
							"<color=#fff>SERVER</color> Kicking " + connection.username + " (banned by anticheat)"
						});
						if (data.ActionReasonCode == AntiCheatCommonClientActionReason.PermanentBanned)
						{
							Entity.DeleteBy(connection.userid);
						}
					}
					UnregisterClientOptions unregisterClientOptions = new UnregisterClientOptions
					{
						ClientHandle = clientHandle
					};
					EACServer.Interface.UnregisterClient(ref unregisterClientOptions);
					Connection connection2;
					EACServer.client2connection.TryRemove((uint)((int)clientHandle), out connection2);
					uint num;
					EACServer.connection2client.TryRemove(connection, out num);
					AntiCheatCommonClientAuthStatus antiCheatCommonClientAuthStatus;
					EACServer.connection2status.TryRemove(connection, out antiCheatCommonClientAuthStatus);
				}
			}
		}
	}

	// Token: 0x060033C4 RID: 13252 RVA: 0x0013E344 File Offset: 0x0013C544
	private static void SendToClient(ref OnMessageToClientCallbackInfo data)
	{
		IntPtr clientHandle = data.ClientHandle;
		Connection connection = EACServer.GetConnection(clientHandle);
		if (connection == null)
		{
			Debug.LogError("[EAC] Network packet for invalid client: " + clientHandle.ToString());
			return;
		}
		NetWrite netWrite = Network.Net.sv.StartWrite();
		netWrite.PacketID(Message.Type.EAC);
		netWrite.UInt32((uint)data.MessageData.Count);
		netWrite.Write(data.MessageData.Array, data.MessageData.Offset, data.MessageData.Count);
		netWrite.Send(new SendInfo(connection));
	}

	// Token: 0x060033C5 RID: 13253 RVA: 0x0013E3DC File Offset: 0x0013C5DC
	public static void DoStartup()
	{
		if (ConVar.Server.secure && !Application.isEditor)
		{
			EACServer.client2connection.Clear();
			EACServer.connection2client.Clear();
			EACServer.connection2status.Clear();
			AddNotifyClientActionRequiredOptions addNotifyClientActionRequiredOptions = default(AddNotifyClientActionRequiredOptions);
			AddNotifyClientAuthStatusChangedOptions addNotifyClientAuthStatusChangedOptions = default(AddNotifyClientAuthStatusChangedOptions);
			AddNotifyMessageToClientOptions addNotifyMessageToClientOptions = default(AddNotifyMessageToClientOptions);
			BeginSessionOptions beginSessionOptions = new BeginSessionOptions
			{
				LocalUserId = null,
				EnableGameplayData = EACServer.CanEnableGameplayData,
				RegisterTimeoutSeconds = 20U,
				ServerName = ConVar.Server.hostname
			};
			LogGameRoundStartOptions logGameRoundStartOptions = new LogGameRoundStartOptions
			{
				LevelName = global::World.Name
			};
			EOS.Initialize(true, ConVar.Server.anticheatid, ConVar.Server.anticheatkey, ConVar.Server.rootFolder + "/Log.EAC.txt");
			EACServer.Interface = EOS.Interface.GetAntiCheatServerInterface();
			EACServer.Interface.AddNotifyClientActionRequired(ref addNotifyClientActionRequiredOptions, null, new OnClientActionRequiredCallback(EACServer.OnClientActionRequired));
			EACServer.Interface.AddNotifyClientAuthStatusChanged(ref addNotifyClientAuthStatusChangedOptions, null, new OnClientAuthStatusChangedCallback(EACServer.OnClientAuthStatusChanged));
			EACServer.Interface.AddNotifyMessageToClient(ref addNotifyMessageToClientOptions, null, new OnMessageToClientCallback(EACServer.SendToClient));
			EACServer.Interface.BeginSession(ref beginSessionOptions);
			EACServer.Interface.LogGameRoundStart(ref logGameRoundStartOptions);
			return;
		}
		EACServer.client2connection.Clear();
		EACServer.connection2client.Clear();
		EACServer.connection2status.Clear();
	}

	// Token: 0x060033C6 RID: 13254 RVA: 0x0013E547 File Offset: 0x0013C747
	public static void DoUpdate()
	{
		if (ConVar.Server.secure && !Application.isEditor)
		{
			EOS.Tick();
		}
	}

	// Token: 0x060033C7 RID: 13255 RVA: 0x0013E55C File Offset: 0x0013C75C
	public static void DoShutdown()
	{
		if (ConVar.Server.secure && !Application.isEditor)
		{
			EACServer.client2connection.Clear();
			EACServer.connection2client.Clear();
			EACServer.connection2status.Clear();
			if (EACServer.Interface != null)
			{
				Debug.Log("EasyAntiCheat Server Shutting Down");
				EndSessionOptions endSessionOptions = default(EndSessionOptions);
				EACServer.Interface.EndSession(ref endSessionOptions);
				EACServer.Interface = null;
				EOS.Shutdown();
				return;
			}
		}
		else
		{
			EACServer.client2connection.Clear();
			EACServer.connection2client.Clear();
			EACServer.connection2status.Clear();
		}
	}

	// Token: 0x060033C8 RID: 13256 RVA: 0x0013E5F0 File Offset: 0x0013C7F0
	public static void OnLeaveGame(Connection connection)
	{
		if (ConVar.Server.secure && !Application.isEditor)
		{
			if (EACServer.Interface != null)
			{
				IntPtr client = EACServer.GetClient(connection);
				if (client != IntPtr.Zero)
				{
					UnregisterClientOptions unregisterClientOptions = new UnregisterClientOptions
					{
						ClientHandle = client
					};
					EACServer.Interface.UnregisterClient(ref unregisterClientOptions);
					Connection connection2;
					EACServer.client2connection.TryRemove((uint)((int)client), out connection2);
				}
				uint num;
				EACServer.connection2client.TryRemove(connection, out num);
				AntiCheatCommonClientAuthStatus antiCheatCommonClientAuthStatus;
				EACServer.connection2status.TryRemove(connection, out antiCheatCommonClientAuthStatus);
				return;
			}
		}
		else
		{
			AntiCheatCommonClientAuthStatus antiCheatCommonClientAuthStatus;
			EACServer.connection2status.TryRemove(connection, out antiCheatCommonClientAuthStatus);
		}
	}

	// Token: 0x060033C9 RID: 13257 RVA: 0x0013E68C File Offset: 0x0013C88C
	public static void OnJoinGame(Connection connection)
	{
		if (ConVar.Server.secure && !Application.isEditor)
		{
			if (EACServer.Interface != null)
			{
				IntPtr intPtr = EACServer.GenerateCompatibilityClient();
				if (intPtr == IntPtr.Zero)
				{
					Debug.LogError("[EAC] GenerateCompatibilityClient returned invalid client: " + intPtr.ToString());
					return;
				}
				RegisterClientOptions registerClientOptions = new RegisterClientOptions
				{
					ClientHandle = intPtr,
					AccountId = connection.userid.ToString(),
					IpAddress = connection.IPAddressWithoutPort(),
					ClientType = ((connection.authLevel >= 3U && connection.os == "editor") ? AntiCheatCommonClientType.UnprotectedClient : AntiCheatCommonClientType.ProtectedClient),
					ClientPlatform = ((connection.os == "windows") ? AntiCheatCommonClientPlatform.Windows : ((connection.os == "linux") ? AntiCheatCommonClientPlatform.Linux : ((connection.os == "mac") ? AntiCheatCommonClientPlatform.Mac : AntiCheatCommonClientPlatform.Unknown)))
				};
				SetClientDetailsOptions setClientDetailsOptions = new SetClientDetailsOptions
				{
					ClientHandle = intPtr,
					ClientFlags = ((connection.authLevel > 0U) ? AntiCheatCommonClientFlags.Admin : AntiCheatCommonClientFlags.None)
				};
				EACServer.Interface.RegisterClient(ref registerClientOptions);
				EACServer.Interface.SetClientDetails(ref setClientDetailsOptions);
				EACServer.client2connection.TryAdd((uint)((int)intPtr), connection);
				EACServer.connection2client.TryAdd(connection, (uint)((int)intPtr));
				EACServer.connection2status.TryAdd(connection, AntiCheatCommonClientAuthStatus.Invalid);
				return;
			}
		}
		else
		{
			EACServer.connection2status.TryAdd(connection, AntiCheatCommonClientAuthStatus.Invalid);
			EACServer.OnAuthenticatedLocal(connection);
			EACServer.OnAuthenticatedRemote(connection);
		}
	}

	// Token: 0x060033CA RID: 13258 RVA: 0x0013E81C File Offset: 0x0013CA1C
	public static void OnStartLoading(Connection connection)
	{
		if (EACServer.Interface != null)
		{
			IntPtr client = EACServer.GetClient(connection);
			if (client != IntPtr.Zero)
			{
				SetClientNetworkStateOptions setClientNetworkStateOptions = new SetClientNetworkStateOptions
				{
					ClientHandle = client,
					IsNetworkActive = false
				};
				EACServer.Interface.SetClientNetworkState(ref setClientNetworkStateOptions);
			}
		}
	}

	// Token: 0x060033CB RID: 13259 RVA: 0x0013E874 File Offset: 0x0013CA74
	public static void OnFinishLoading(Connection connection)
	{
		if (EACServer.Interface != null)
		{
			IntPtr client = EACServer.GetClient(connection);
			if (client != IntPtr.Zero)
			{
				SetClientNetworkStateOptions setClientNetworkStateOptions = new SetClientNetworkStateOptions
				{
					ClientHandle = client,
					IsNetworkActive = true
				};
				EACServer.Interface.SetClientNetworkState(ref setClientNetworkStateOptions);
			}
		}
	}

	// Token: 0x060033CC RID: 13260 RVA: 0x0013E8CC File Offset: 0x0013CACC
	public static void OnMessageReceived(Message message)
	{
		IntPtr client = EACServer.GetClient(message.connection);
		if (client == IntPtr.Zero)
		{
			Debug.LogError("EAC network packet from invalid connection: " + message.connection.userid);
			return;
		}
		byte[] array;
		int count;
		if (!message.read.TemporaryBytesWithSize(out array, out count))
		{
			return;
		}
		ReceiveMessageFromClientOptions receiveMessageFromClientOptions = new ReceiveMessageFromClientOptions
		{
			ClientHandle = client,
			Data = new ArraySegment<byte>(array, 0, count)
		};
		EACServer.Interface.ReceiveMessageFromClient(ref receiveMessageFromClientOptions);
	}

	// Token: 0x060033CD RID: 13261 RVA: 0x0013E954 File Offset: 0x0013CB54
	public static void LogPlayerUseWeapon(BasePlayer player, BaseProjectile weapon)
	{
		if (EACServer.CanSendAnalytics && player.net.connection != null)
		{
			using (TimeWarning.New("EAC.LogPlayerShooting", 0))
			{
				Vector3 networkPosition = player.GetNetworkPosition();
				Quaternion networkRotation = player.GetNetworkRotation();
				Item item = weapon.GetItem();
				string str = (item != null) ? item.info.shortname : "unknown";
				LogPlayerUseWeaponOptions logPlayerUseWeaponOptions = default(LogPlayerUseWeaponOptions);
				logPlayerUseWeaponOptions.UseWeaponData = new LogPlayerUseWeaponData?(new LogPlayerUseWeaponData
				{
					PlayerHandle = EACServer.GetClient(player.net.connection),
					PlayerPosition = new Vec3f?(new Vec3f
					{
						x = networkPosition.x,
						y = networkPosition.y,
						z = networkPosition.z
					}),
					PlayerViewRotation = new Quat?(new Quat
					{
						w = networkRotation.w,
						x = networkRotation.x,
						y = networkRotation.y,
						z = networkRotation.z
					}),
					WeaponName = str
				});
				EACServer.Interface.LogPlayerUseWeapon(ref logPlayerUseWeaponOptions);
			}
		}
	}

	// Token: 0x060033CE RID: 13262 RVA: 0x0013EAB4 File Offset: 0x0013CCB4
	public static void LogPlayerSpawn(BasePlayer player)
	{
		if (EACServer.CanSendAnalytics && player.net.connection != null)
		{
			using (TimeWarning.New("EAC.LogPlayerSpawn", 0))
			{
				LogPlayerSpawnOptions logPlayerSpawnOptions = default(LogPlayerSpawnOptions);
				logPlayerSpawnOptions.SpawnedPlayerHandle = EACServer.GetClient(player.net.connection);
				EACServer.Interface.LogPlayerSpawn(ref logPlayerSpawnOptions);
			}
		}
	}

	// Token: 0x060033CF RID: 13263 RVA: 0x0013EB2C File Offset: 0x0013CD2C
	public static void LogPlayerDespawn(BasePlayer player)
	{
		if (EACServer.CanSendAnalytics && player.net.connection != null)
		{
			using (TimeWarning.New("EAC.LogPlayerDespawn", 0))
			{
				LogPlayerDespawnOptions logPlayerDespawnOptions = default(LogPlayerDespawnOptions);
				logPlayerDespawnOptions.DespawnedPlayerHandle = EACServer.GetClient(player.net.connection);
				EACServer.Interface.LogPlayerDespawn(ref logPlayerDespawnOptions);
			}
		}
	}

	// Token: 0x060033D0 RID: 13264 RVA: 0x0013EBA4 File Offset: 0x0013CDA4
	public static void LogPlayerTakeDamage(BasePlayer player, HitInfo info)
	{
		if (EACServer.CanSendAnalytics && info.Initiator != null && info.Initiator is BasePlayer)
		{
			BasePlayer basePlayer = info.Initiator.ToPlayer();
			if (player.net.connection != null && basePlayer.net.connection != null)
			{
				using (TimeWarning.New("EAC.LogPlayerTakeDamage", 0))
				{
					LogPlayerTakeDamageOptions logPlayerTakeDamageOptions = default(LogPlayerTakeDamageOptions);
					LogPlayerUseWeaponData value = default(LogPlayerUseWeaponData);
					logPlayerTakeDamageOptions.AttackerPlayerHandle = EACServer.GetClient(basePlayer.net.connection);
					logPlayerTakeDamageOptions.VictimPlayerHandle = EACServer.GetClient(player.net.connection);
					logPlayerTakeDamageOptions.DamageTaken = info.damageTypes.Total();
					logPlayerTakeDamageOptions.DamagePosition = new Vec3f?(new Vec3f
					{
						x = info.HitPositionWorld.x,
						y = info.HitPositionWorld.y,
						z = info.HitPositionWorld.z
					});
					logPlayerTakeDamageOptions.IsCriticalHit = info.isHeadshot;
					if (player.IsDead())
					{
						logPlayerTakeDamageOptions.DamageResult = AntiCheatCommonPlayerTakeDamageResult.Eliminated;
					}
					else if (player.IsWounded())
					{
						logPlayerTakeDamageOptions.DamageResult = AntiCheatCommonPlayerTakeDamageResult.Downed;
					}
					if (info.Weapon != null)
					{
						Item item = info.Weapon.GetItem();
						if (item != null)
						{
							value.WeaponName = item.info.shortname;
						}
						else
						{
							value.WeaponName = "unknown";
						}
					}
					else
					{
						value.WeaponName = "unknown";
					}
					Vector3 position = basePlayer.eyes.position;
					Quaternion rotation = basePlayer.eyes.rotation;
					Vector3 position2 = player.eyes.position;
					Quaternion rotation2 = player.eyes.rotation;
					logPlayerTakeDamageOptions.AttackerPlayerPosition = new Vec3f?(new Vec3f
					{
						x = position.x,
						y = position.y,
						z = position.z
					});
					logPlayerTakeDamageOptions.AttackerPlayerViewRotation = new Quat?(new Quat
					{
						w = rotation.w,
						x = rotation.x,
						y = rotation.y,
						z = rotation.z
					});
					logPlayerTakeDamageOptions.VictimPlayerPosition = new Vec3f?(new Vec3f
					{
						x = position2.x,
						y = position2.y,
						z = position2.z
					});
					logPlayerTakeDamageOptions.VictimPlayerViewRotation = new Quat?(new Quat
					{
						w = rotation2.w,
						x = rotation2.x,
						y = rotation2.y,
						z = rotation2.z
					});
					logPlayerTakeDamageOptions.PlayerUseWeaponData = new LogPlayerUseWeaponData?(value);
					EACServer.Interface.LogPlayerTakeDamage(ref logPlayerTakeDamageOptions);
				}
			}
		}
	}

	// Token: 0x060033D1 RID: 13265 RVA: 0x0013EED0 File Offset: 0x0013D0D0
	public static void LogPlayerTick(BasePlayer player)
	{
		if (EACServer.CanSendAnalytics && player.net != null && player.net.connection != null)
		{
			using (TimeWarning.New("EAC.LogPlayerTick", 0))
			{
				Vector3 position = player.eyes.position;
				Quaternion rotation = player.eyes.rotation;
				LogPlayerTickOptions logPlayerTickOptions = default(LogPlayerTickOptions);
				logPlayerTickOptions.PlayerHandle = EACServer.GetClient(player.net.connection);
				logPlayerTickOptions.PlayerPosition = new Vec3f?(new Vec3f
				{
					x = position.x,
					y = position.y,
					z = position.z
				});
				logPlayerTickOptions.PlayerViewRotation = new Quat?(new Quat
				{
					w = rotation.w,
					x = rotation.x,
					y = rotation.y,
					z = rotation.z
				});
				logPlayerTickOptions.PlayerHealth = player.Health();
				if (player.IsDucked())
				{
					logPlayerTickOptions.PlayerMovementState |= AntiCheatCommonPlayerMovementState.Crouching;
				}
				if (player.isMounted)
				{
					logPlayerTickOptions.PlayerMovementState |= AntiCheatCommonPlayerMovementState.Mounted;
				}
				if (player.IsCrawling())
				{
					logPlayerTickOptions.PlayerMovementState |= AntiCheatCommonPlayerMovementState.Prone;
				}
				if (player.IsSwimming())
				{
					logPlayerTickOptions.PlayerMovementState |= AntiCheatCommonPlayerMovementState.Swimming;
				}
				if (!player.IsOnGround())
				{
					logPlayerTickOptions.PlayerMovementState |= AntiCheatCommonPlayerMovementState.Falling;
				}
				if (player.OnLadder())
				{
					logPlayerTickOptions.PlayerMovementState |= AntiCheatCommonPlayerMovementState.OnLadder;
				}
				if (player.IsFlying)
				{
					logPlayerTickOptions.PlayerMovementState |= AntiCheatCommonPlayerMovementState.Flying;
				}
				EACServer.Interface.LogPlayerTick(ref logPlayerTickOptions);
			}
		}
	}

	// Token: 0x060033D2 RID: 13266 RVA: 0x0013F0B0 File Offset: 0x0013D2B0
	public static void LogPlayerRevive(BasePlayer source, BasePlayer target)
	{
		if (EACServer.CanSendAnalytics && target.net.connection != null && source != null && source.net.connection != null)
		{
			using (TimeWarning.New("EAC.LogPlayerRevive", 0))
			{
				LogPlayerReviveOptions logPlayerReviveOptions = default(LogPlayerReviveOptions);
				logPlayerReviveOptions.RevivedPlayerHandle = EACServer.GetClient(target.net.connection);
				logPlayerReviveOptions.ReviverPlayerHandle = EACServer.GetClient(source.net.connection);
				EACServer.Interface.LogPlayerRevive(ref logPlayerReviveOptions);
			}
		}
	}

	// Token: 0x060033D3 RID: 13267 RVA: 0x0013F154 File Offset: 0x0013D354
	public static void SendPlayerBehaviorReport(BasePlayer reporter, PlayerReportsCategory reportCategory, string reportedID, string reportText)
	{
		if (EACServer.CanSendReports)
		{
			SendPlayerBehaviorReportOptions sendPlayerBehaviorReportOptions = new SendPlayerBehaviorReportOptions
			{
				ReportedUserId = ProductUserId.FromString(reportedID),
				ReporterUserId = ProductUserId.FromString(reporter.UserIDString),
				Category = reportCategory,
				Message = reportText
			};
			EACServer.Reports.SendPlayerBehaviorReport(ref sendPlayerBehaviorReportOptions, null, null);
		}
	}

	// Token: 0x060033D4 RID: 13268 RVA: 0x0013F1C0 File Offset: 0x0013D3C0
	public static void SendPlayerBehaviorReport(PlayerReportsCategory reportCategory, string reportedID, string reportText)
	{
		if (EACServer.CanSendReports)
		{
			SendPlayerBehaviorReportOptions sendPlayerBehaviorReportOptions = new SendPlayerBehaviorReportOptions
			{
				ReportedUserId = ProductUserId.FromString(reportedID),
				Category = reportCategory,
				Message = reportText
			};
			EACServer.Reports.SendPlayerBehaviorReport(ref sendPlayerBehaviorReportOptions, null, null);
		}
	}
}
