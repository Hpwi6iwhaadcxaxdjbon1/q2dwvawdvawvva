using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Facepunch;
using Facepunch.Extend;
using Network;
using Network.Visibility;
using ProtoBuf;
using Rust;
using UnityEngine;
using UnityEngine.Profiling;

namespace ConVar
{
	// Token: 0x02000ABD RID: 2749
	[ConsoleSystem.Factory("global")]
	public class Global : ConsoleSystem
	{
		// Token: 0x04003B3F RID: 15167
		private static int _developer;

		// Token: 0x04003B40 RID: 15168
		[ServerVar]
		[ClientVar(Help = "WARNING: This causes random crashes!")]
		public static bool skipAssetWarmup_crashes = false;

		// Token: 0x04003B41 RID: 15169
		[ServerVar]
		[ClientVar]
		public static int maxthreads = 8;

		// Token: 0x04003B42 RID: 15170
		private const int DefaultWarmupConcurrency = 1;

		// Token: 0x04003B43 RID: 15171
		private const int DefaultPreloadConcurrency = 1;

		// Token: 0x04003B44 RID: 15172
		[ServerVar]
		[ClientVar]
		public static int warmupConcurrency = 1;

		// Token: 0x04003B45 RID: 15173
		[ServerVar]
		[ClientVar]
		public static int preloadConcurrency = 1;

		// Token: 0x04003B46 RID: 15174
		[ServerVar]
		[ClientVar]
		public static bool forceUnloadBundles = true;

		// Token: 0x04003B47 RID: 15175
		private const bool DefaultAsyncWarmupEnabled = false;

		// Token: 0x04003B48 RID: 15176
		[ServerVar]
		[ClientVar]
		public static bool asyncWarmup = false;

		// Token: 0x04003B49 RID: 15177
		[ClientVar(Saved = true, Help = "Experimental faster loading, requires game restart (0 = off, 1 = partial, 2 = full)")]
		public static int asyncLoadingPreset = 0;

		// Token: 0x04003B4A RID: 15178
		[ServerVar(Saved = true)]
		[ClientVar(Saved = true)]
		public static int perf = 0;

		// Token: 0x04003B4B RID: 15179
		[ClientVar(ClientInfo = true, Saved = true, Help = "If you're an admin this will enable god mode")]
		public static bool god = false;

		// Token: 0x04003B4C RID: 15180
		[ClientVar(ClientInfo = true, Saved = true, Help = "If enabled you will be networked when you're spectating. This means that you will hear audio chat, but also means that cheaters will potentially be able to detect you watching them.")]
		public static bool specnet = false;

		// Token: 0x04003B4D RID: 15181
		[ClientVar]
		[ServerVar(ClientAdmin = true, ServerAdmin = true, Help = "When enabled a player wearing a gingerbread suit will gib like the gingerbread NPC's")]
		public static bool cinematicGingerbreadCorpses = false;

		// Token: 0x04003B4E RID: 15182
		private static uint _gingerbreadMaterialID = 0U;

		// Token: 0x04003B4F RID: 15183
		[ServerVar(Saved = true, ShowInAdminUI = true, Help = "Multiplier applied to SprayDuration if a spray isn't in the sprayers auth (cannot go above 1f)")]
		public static float SprayOutOfAuthMultiplier = 0.5f;

		// Token: 0x04003B50 RID: 15184
		[ServerVar(Saved = true, ShowInAdminUI = true, Help = "Base time (in seconds) that sprays last")]
		public static float SprayDuration = 10800f;

		// Token: 0x04003B51 RID: 15185
		[ServerVar(Saved = true, ShowInAdminUI = true, Help = "If a player sprays more than this, the oldest spray will be destroyed. 0 will disable")]
		public static int MaxSpraysPerPlayer = 40;

		// Token: 0x04003B52 RID: 15186
		[ServerVar(Help = "Disables the backpacks that appear after a corpse times out")]
		public static bool disableBagDropping = false;

		// Token: 0x170005B0 RID: 1456
		// (get) Token: 0x060041EA RID: 16874 RVA: 0x00186C64 File Offset: 0x00184E64
		// (set) Token: 0x060041E9 RID: 16873 RVA: 0x00186C5C File Offset: 0x00184E5C
		[ServerVar]
		[ClientVar]
		public static int developer
		{
			get
			{
				return Global._developer;
			}
			set
			{
				Global._developer = value;
			}
		}

		// Token: 0x060041EB RID: 16875 RVA: 0x00186C6C File Offset: 0x00184E6C
		public static void ApplyAsyncLoadingPreset()
		{
			if (Global.asyncLoadingPreset != 0)
			{
				UnityEngine.Debug.Log(string.Format("Applying async loading preset number {0}", Global.asyncLoadingPreset));
			}
			switch (Global.asyncLoadingPreset)
			{
			case 0:
				break;
			case 1:
				if (Global.warmupConcurrency <= 1)
				{
					Global.warmupConcurrency = 256;
				}
				if (Global.preloadConcurrency <= 1)
				{
					Global.preloadConcurrency = 256;
				}
				Global.asyncWarmup = false;
				return;
			case 2:
				if (Global.warmupConcurrency <= 1)
				{
					Global.warmupConcurrency = 256;
				}
				if (Global.preloadConcurrency <= 1)
				{
					Global.preloadConcurrency = 256;
				}
				Global.asyncWarmup = false;
				return;
			default:
				UnityEngine.Debug.LogWarning(string.Format("There is no asyncLoading preset number {0}", Global.asyncLoadingPreset));
				break;
			}
		}

		// Token: 0x060041EC RID: 16876 RVA: 0x00186D22 File Offset: 0x00184F22
		[ServerVar]
		public static void restart(ConsoleSystem.Arg args)
		{
			ServerMgr.RestartServer(args.GetString(1, string.Empty), args.GetInt(0, 300));
		}

		// Token: 0x060041ED RID: 16877 RVA: 0x00186D41 File Offset: 0x00184F41
		[ClientVar]
		[ServerVar]
		public static void quit(ConsoleSystem.Arg args)
		{
			SingletonComponent<ServerMgr>.Instance.Shutdown();
			Rust.Application.isQuitting = true;
			Net.sv.Stop("quit");
			Process.GetCurrentProcess().Kill();
			UnityEngine.Debug.Log("Quitting");
			Rust.Application.Quit();
		}

		// Token: 0x060041EE RID: 16878 RVA: 0x00186D7B File Offset: 0x00184F7B
		[ServerVar]
		public static void report(ConsoleSystem.Arg args)
		{
			ServerPerformance.DoReport();
		}

		// Token: 0x060041EF RID: 16879 RVA: 0x00186D84 File Offset: 0x00184F84
		[ServerVar]
		[ClientVar]
		public static void objects(ConsoleSystem.Arg args)
		{
			UnityEngine.Object[] array = UnityEngine.Object.FindObjectsOfType<UnityEngine.Object>();
			string text = "";
			Dictionary<Type, int> dictionary = new Dictionary<Type, int>();
			Dictionary<Type, long> dictionary2 = new Dictionary<Type, long>();
			foreach (UnityEngine.Object @object in array)
			{
				int runtimeMemorySize = Profiler.GetRuntimeMemorySize(@object);
				if (dictionary.ContainsKey(@object.GetType()))
				{
					Dictionary<Type, int> dictionary3 = dictionary;
					Type type = @object.GetType();
					int num = dictionary3[type];
					dictionary3[type] = num + 1;
				}
				else
				{
					dictionary.Add(@object.GetType(), 1);
				}
				if (dictionary2.ContainsKey(@object.GetType()))
				{
					Dictionary<Type, long> dictionary4 = dictionary2;
					Type type = @object.GetType();
					dictionary4[type] += (long)runtimeMemorySize;
				}
				else
				{
					dictionary2.Add(@object.GetType(), (long)runtimeMemorySize);
				}
			}
			foreach (KeyValuePair<Type, long> keyValuePair in dictionary2.OrderByDescending(delegate(KeyValuePair<Type, long> x)
			{
				KeyValuePair<Type, long> keyValuePair2 = x;
				return keyValuePair2.Value;
			}))
			{
				text = string.Concat(new object[]
				{
					text,
					dictionary[keyValuePair.Key].ToString().PadLeft(10),
					" ",
					keyValuePair.Value.FormatBytes(false).PadLeft(15),
					"\t",
					keyValuePair.Key,
					"\n"
				});
			}
			args.ReplyWith(text);
		}

		// Token: 0x060041F0 RID: 16880 RVA: 0x00186F20 File Offset: 0x00185120
		[ServerVar]
		[ClientVar]
		public static void textures(ConsoleSystem.Arg args)
		{
			Texture[] array = UnityEngine.Object.FindObjectsOfType<Texture>();
			string text = "";
			foreach (Texture texture in array)
			{
				string text2 = Profiler.GetRuntimeMemorySize(texture).FormatBytes(false);
				text = string.Concat(new string[]
				{
					text,
					texture.ToString().PadRight(30),
					texture.name.PadRight(30),
					text2,
					"\n"
				});
			}
			args.ReplyWith(text);
		}

		// Token: 0x060041F1 RID: 16881 RVA: 0x00186FA0 File Offset: 0x001851A0
		[ServerVar]
		[ClientVar]
		public static void colliders(ConsoleSystem.Arg args)
		{
			int num = (from x in UnityEngine.Object.FindObjectsOfType<Collider>()
			where x.enabled
			select x).Count<Collider>();
			int num2 = (from x in UnityEngine.Object.FindObjectsOfType<Collider>()
			where !x.enabled
			select x).Count<Collider>();
			string strValue = string.Concat(new object[]
			{
				num,
				" colliders enabled, ",
				num2,
				" disabled"
			});
			args.ReplyWith(strValue);
		}

		// Token: 0x060041F2 RID: 16882 RVA: 0x00187040 File Offset: 0x00185240
		[ServerVar]
		[ClientVar]
		public static void error(ConsoleSystem.Arg args)
		{
			((GameObject)null).transform.position = Vector3.zero;
		}

		// Token: 0x060041F3 RID: 16883 RVA: 0x00187054 File Offset: 0x00185254
		[ServerVar]
		[ClientVar]
		public static void queue(ConsoleSystem.Arg args)
		{
			string text = "";
			text = text + "stabilityCheckQueue:\t\t" + global::StabilityEntity.stabilityCheckQueue.Info() + "\n";
			text = text + "updateSurroundingsQueue:\t" + global::StabilityEntity.updateSurroundingsQueue.Info() + "\n";
			args.ReplyWith(text);
		}

		// Token: 0x060041F4 RID: 16884 RVA: 0x001870A4 File Offset: 0x001852A4
		[ServerUserVar]
		public static void setinfo(ConsoleSystem.Arg args)
		{
			global::BasePlayer basePlayer = args.Player();
			if (!basePlayer)
			{
				return;
			}
			string @string = args.GetString(0, null);
			string string2 = args.GetString(1, null);
			if (@string == null || string2 == null)
			{
				return;
			}
			basePlayer.SetInfo(@string, string2);
		}

		// Token: 0x060041F5 RID: 16885 RVA: 0x001870E4 File Offset: 0x001852E4
		[ServerVar]
		public static void sleep(ConsoleSystem.Arg args)
		{
			global::BasePlayer basePlayer = args.Player();
			if (!basePlayer)
			{
				return;
			}
			if (basePlayer.IsSleeping())
			{
				return;
			}
			if (basePlayer.IsSpectating())
			{
				return;
			}
			if (basePlayer.IsDead())
			{
				return;
			}
			basePlayer.StartSleeping();
		}

		// Token: 0x060041F6 RID: 16886 RVA: 0x00187124 File Offset: 0x00185324
		[ServerUserVar]
		public static void kill(ConsoleSystem.Arg args)
		{
			global::BasePlayer basePlayer = args.Player();
			if (!basePlayer)
			{
				return;
			}
			if (basePlayer.IsSpectating())
			{
				return;
			}
			if (basePlayer.IsDead())
			{
				return;
			}
			if (basePlayer.CanSuicide())
			{
				basePlayer.MarkSuicide();
				basePlayer.Hurt(1000f, DamageType.Suicide, basePlayer, false);
				return;
			}
			basePlayer.ConsoleMessage("You can't suicide again so quickly, wait a while");
		}

		// Token: 0x060041F7 RID: 16887 RVA: 0x0018717C File Offset: 0x0018537C
		[ServerUserVar]
		public static void respawn(ConsoleSystem.Arg args)
		{
			global::BasePlayer basePlayer = args.Player();
			if (!basePlayer)
			{
				return;
			}
			if (!basePlayer.IsDead() && !basePlayer.IsSpectating())
			{
				if (Global.developer > 0)
				{
					UnityEngine.Debug.LogWarning(basePlayer + " wanted to respawn but isn't dead or spectating");
				}
				basePlayer.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
				return;
			}
			if (basePlayer.CanRespawn())
			{
				basePlayer.MarkRespawn(5f);
				basePlayer.Respawn();
				return;
			}
			basePlayer.ConsoleMessage("You can't respawn again so quickly, wait a while");
		}

		// Token: 0x060041F8 RID: 16888 RVA: 0x001871EE File Offset: 0x001853EE
		[ServerVar]
		public static void injure(ConsoleSystem.Arg args)
		{
			Global.InjurePlayer(args.Player());
		}

		// Token: 0x060041F9 RID: 16889 RVA: 0x001871FC File Offset: 0x001853FC
		public static void InjurePlayer(global::BasePlayer ply)
		{
			if (ply == null)
			{
				return;
			}
			if (ply.IsDead())
			{
				return;
			}
			if (!ConVar.Server.woundingenabled || ply.IsIncapacitated() || ply.IsSleeping() || ply.isMounted)
			{
				ply.ConsoleMessage("Can't go to wounded state right now.");
				return;
			}
			if (ply.IsCrawling())
			{
				ply.GoToIncapacitated(null);
				return;
			}
			ply.BecomeWounded(null);
		}

		// Token: 0x060041FA RID: 16890 RVA: 0x00187264 File Offset: 0x00185464
		[ServerVar]
		public static void recover(ConsoleSystem.Arg args)
		{
			Global.RecoverPlayer(args.Player());
		}

		// Token: 0x060041FB RID: 16891 RVA: 0x00187271 File Offset: 0x00185471
		public static void RecoverPlayer(global::BasePlayer ply)
		{
			if (ply == null)
			{
				return;
			}
			if (ply.IsDead())
			{
				return;
			}
			ply.StopWounded(null);
		}

		// Token: 0x060041FC RID: 16892 RVA: 0x00187290 File Offset: 0x00185490
		[ServerVar]
		public static void spectate(ConsoleSystem.Arg args)
		{
			global::BasePlayer basePlayer = args.Player();
			if (!basePlayer)
			{
				return;
			}
			if (!basePlayer.IsDead())
			{
				basePlayer.DieInstantly();
			}
			string @string = args.GetString(0, "");
			if (basePlayer.IsDead())
			{
				basePlayer.StartSpectating();
				basePlayer.UpdateSpectateTarget(@string);
			}
		}

		// Token: 0x060041FD RID: 16893 RVA: 0x001872E0 File Offset: 0x001854E0
		[ServerVar]
		public static void spectateid(ConsoleSystem.Arg args)
		{
			global::BasePlayer basePlayer = args.Player();
			if (!basePlayer)
			{
				return;
			}
			if (!basePlayer.IsDead())
			{
				basePlayer.DieInstantly();
			}
			ulong @ulong = args.GetULong(0, 0UL);
			if (basePlayer.IsDead())
			{
				basePlayer.StartSpectating();
				basePlayer.UpdateSpectateTarget(@ulong);
			}
		}

		// Token: 0x060041FE RID: 16894 RVA: 0x0018732C File Offset: 0x0018552C
		[ServerUserVar]
		public static void respawn_sleepingbag(ConsoleSystem.Arg args)
		{
			global::BasePlayer basePlayer = args.Player();
			if (!basePlayer)
			{
				return;
			}
			if (!basePlayer.IsDead())
			{
				return;
			}
			NetworkableId entityID = args.GetEntityID(0, default(NetworkableId));
			if (!entityID.IsValid)
			{
				args.ReplyWith("Missing sleeping bag ID");
				return;
			}
			if (!basePlayer.CanRespawn())
			{
				basePlayer.ConsoleMessage("You can't respawn again so quickly, wait a while");
				return;
			}
			if (global::SleepingBag.SpawnPlayer(basePlayer, entityID))
			{
				basePlayer.MarkRespawn(5f);
				return;
			}
			args.ReplyWith("Couldn't spawn in sleeping bag!");
		}

		// Token: 0x060041FF RID: 16895 RVA: 0x001873AC File Offset: 0x001855AC
		[ServerUserVar]
		public static void respawn_sleepingbag_remove(ConsoleSystem.Arg args)
		{
			global::BasePlayer basePlayer = args.Player();
			if (!basePlayer)
			{
				return;
			}
			NetworkableId entityID = args.GetEntityID(0, default(NetworkableId));
			if (!entityID.IsValid)
			{
				args.ReplyWith("Missing sleeping bag ID");
				return;
			}
			global::SleepingBag.DestroyBag(basePlayer, entityID);
		}

		// Token: 0x06004200 RID: 16896 RVA: 0x001873F8 File Offset: 0x001855F8
		[ServerUserVar]
		public static void status_sv(ConsoleSystem.Arg args)
		{
			global::BasePlayer basePlayer = args.Player();
			if (!basePlayer)
			{
				return;
			}
			args.ReplyWith(basePlayer.GetDebugStatus());
		}

		// Token: 0x06004201 RID: 16897 RVA: 0x000063A5 File Offset: 0x000045A5
		[ClientVar]
		public static void status_cl(ConsoleSystem.Arg args)
		{
		}

		// Token: 0x06004202 RID: 16898 RVA: 0x00187424 File Offset: 0x00185624
		[ServerVar]
		public static void teleport(ConsoleSystem.Arg args)
		{
			if (args.HasArgs(2))
			{
				global::BasePlayer playerOrSleeperOrBot = args.GetPlayerOrSleeperOrBot(0);
				if (!playerOrSleeperOrBot)
				{
					return;
				}
				if (!playerOrSleeperOrBot.IsAlive())
				{
					return;
				}
				global::BasePlayer playerOrSleeperOrBot2 = args.GetPlayerOrSleeperOrBot(1);
				if (!playerOrSleeperOrBot2)
				{
					return;
				}
				if (!playerOrSleeperOrBot2.IsAlive())
				{
					return;
				}
				playerOrSleeperOrBot.Teleport(playerOrSleeperOrBot2);
				return;
			}
			else
			{
				global::BasePlayer basePlayer = args.Player();
				if (!basePlayer)
				{
					return;
				}
				if (!basePlayer.IsAlive())
				{
					return;
				}
				global::BasePlayer playerOrSleeperOrBot3 = args.GetPlayerOrSleeperOrBot(0);
				if (!playerOrSleeperOrBot3)
				{
					return;
				}
				if (!playerOrSleeperOrBot3.IsAlive())
				{
					return;
				}
				basePlayer.Teleport(playerOrSleeperOrBot3);
				return;
			}
		}

		// Token: 0x06004203 RID: 16899 RVA: 0x001874B0 File Offset: 0x001856B0
		[ServerVar]
		public static void teleport2me(ConsoleSystem.Arg args)
		{
			global::BasePlayer playerOrSleeperOrBot = args.GetPlayerOrSleeperOrBot(0);
			if (playerOrSleeperOrBot == null)
			{
				args.ReplyWith("Player or bot not found");
				return;
			}
			if (!playerOrSleeperOrBot.IsAlive())
			{
				args.ReplyWith("Target is not alive");
				return;
			}
			global::BasePlayer basePlayer = args.Player();
			if (!basePlayer)
			{
				return;
			}
			if (!basePlayer.IsAlive())
			{
				return;
			}
			playerOrSleeperOrBot.Teleport(basePlayer);
		}

		// Token: 0x06004204 RID: 16900 RVA: 0x00187510 File Offset: 0x00185710
		[ServerVar]
		public static void teleportany(ConsoleSystem.Arg args)
		{
			global::BasePlayer basePlayer = args.Player();
			if (!basePlayer)
			{
				return;
			}
			if (!basePlayer.IsAlive())
			{
				return;
			}
			basePlayer.Teleport(args.GetString(0, ""), false);
		}

		// Token: 0x06004205 RID: 16901 RVA: 0x0018754C File Offset: 0x0018574C
		[ServerVar]
		public static void teleportpos(ConsoleSystem.Arg args)
		{
			global::BasePlayer basePlayer = args.Player();
			if (!basePlayer)
			{
				return;
			}
			if (!basePlayer.IsAlive())
			{
				return;
			}
			basePlayer.Teleport(args.GetVector3(0, Vector3.zero));
		}

		// Token: 0x06004206 RID: 16902 RVA: 0x00187584 File Offset: 0x00185784
		[ServerVar]
		public static void teleportlos(ConsoleSystem.Arg args)
		{
			global::BasePlayer basePlayer = args.Player();
			if (!basePlayer)
			{
				return;
			}
			if (!basePlayer.IsAlive())
			{
				return;
			}
			Ray ray = basePlayer.eyes.HeadRay();
			int @int = args.GetInt(0, 1000);
			RaycastHit raycastHit;
			if (Physics.Raycast(ray, out raycastHit, (float)@int, 1218652417))
			{
				basePlayer.Teleport(raycastHit.point);
				return;
			}
			basePlayer.Teleport(ray.origin + ray.direction * (float)@int);
		}

		// Token: 0x06004207 RID: 16903 RVA: 0x00187604 File Offset: 0x00185804
		[ServerVar]
		public static void teleport2owneditem(ConsoleSystem.Arg arg)
		{
			global::BasePlayer basePlayer = arg.Player();
			global::BasePlayer playerOrSleeper = arg.GetPlayerOrSleeper(0);
			ulong userID;
			if (playerOrSleeper != null)
			{
				userID = playerOrSleeper.userID;
			}
			else if (!ulong.TryParse(arg.GetString(0, ""), out userID))
			{
				arg.ReplyWith("No player with that id found");
				return;
			}
			string @string = arg.GetString(1, "");
			global::BaseEntity[] array = global::BaseEntity.Util.FindTargetsOwnedBy(userID, @string);
			if (array.Length == 0)
			{
				arg.ReplyWith("No targets found");
				return;
			}
			int num = UnityEngine.Random.Range(0, array.Length);
			arg.ReplyWith(string.Format("Teleporting to {0} at {1}", array[num].ShortPrefabName, array[num].transform.position));
			basePlayer.Teleport(array[num].transform.position);
		}

		// Token: 0x06004208 RID: 16904 RVA: 0x001876CC File Offset: 0x001858CC
		[ServerVar]
		public static void teleport2autheditem(ConsoleSystem.Arg arg)
		{
			global::BasePlayer basePlayer = arg.Player();
			global::BasePlayer playerOrSleeper = arg.GetPlayerOrSleeper(0);
			ulong userID;
			if (playerOrSleeper != null)
			{
				userID = playerOrSleeper.userID;
			}
			else if (!ulong.TryParse(arg.GetString(0, ""), out userID))
			{
				arg.ReplyWith("No player with that id found");
				return;
			}
			string @string = arg.GetString(1, "");
			global::BaseEntity[] array = global::BaseEntity.Util.FindTargetsAuthedTo(userID, @string);
			if (array.Length == 0)
			{
				arg.ReplyWith("No targets found");
				return;
			}
			int num = UnityEngine.Random.Range(0, array.Length);
			arg.ReplyWith(string.Format("Teleporting to {0} at {1}", array[num].ShortPrefabName, array[num].transform.position));
			basePlayer.Teleport(array[num].transform.position);
		}

		// Token: 0x06004209 RID: 16905 RVA: 0x00187794 File Offset: 0x00185994
		[ServerVar]
		public static void teleport2marker(ConsoleSystem.Arg arg)
		{
			global::BasePlayer basePlayer = arg.Player();
			if (basePlayer.State.pointsOfInterest == null || basePlayer.State.pointsOfInterest.Count == 0)
			{
				arg.ReplyWith("You don't have a marker set");
				return;
			}
			string @string = arg.GetString(0, "");
			if (!string.IsNullOrEmpty(@string))
			{
				foreach (MapNote mapNote in basePlayer.State.pointsOfInterest)
				{
					if (!string.IsNullOrEmpty(mapNote.label) && string.Equals(mapNote.label, @string, StringComparison.InvariantCultureIgnoreCase))
					{
						Global.TeleportToMarker(mapNote, basePlayer);
						return;
					}
				}
			}
			if (arg.HasArgs(1))
			{
				int @int = arg.GetInt(0, 0);
				if (@int >= 0 && @int < basePlayer.State.pointsOfInterest.Count)
				{
					Global.TeleportToMarker(basePlayer.State.pointsOfInterest[@int], basePlayer);
					return;
				}
			}
			int num = basePlayer.DebugMapMarkerIndex;
			num++;
			if (num >= basePlayer.State.pointsOfInterest.Count)
			{
				num = 0;
			}
			Global.TeleportToMarker(basePlayer.State.pointsOfInterest[num], basePlayer);
			basePlayer.DebugMapMarkerIndex = num;
		}

		// Token: 0x0600420A RID: 16906 RVA: 0x001878DC File Offset: 0x00185ADC
		private static void TeleportToMarker(MapNote marker, global::BasePlayer player)
		{
			Vector3 worldPosition = marker.worldPosition;
			float height = TerrainMeta.HeightMap.GetHeight(worldPosition);
			float height2 = TerrainMeta.WaterMap.GetHeight(worldPosition);
			worldPosition.y = Mathf.Max(height, height2);
			player.Teleport(worldPosition);
		}

		// Token: 0x0600420B RID: 16907 RVA: 0x00187920 File Offset: 0x00185B20
		[ServerVar]
		public static void teleport2death(ConsoleSystem.Arg arg)
		{
			global::BasePlayer basePlayer = arg.Player();
			if (basePlayer.ServerCurrentDeathNote == null)
			{
				arg.ReplyWith("You don't have a current death note!");
			}
			Vector3 worldPosition = basePlayer.ServerCurrentDeathNote.worldPosition;
			basePlayer.Teleport(worldPosition);
		}

		// Token: 0x0600420C RID: 16908 RVA: 0x00187958 File Offset: 0x00185B58
		[ServerVar]
		[ClientVar]
		public static void free(ConsoleSystem.Arg args)
		{
			Pool.clear_prefabs(args);
			Pool.clear_assets(args);
			Pool.clear_memory(args);
			ConVar.GC.collect();
			ConVar.GC.unload();
		}

		// Token: 0x0600420D RID: 16909 RVA: 0x00187978 File Offset: 0x00185B78
		[ServerVar(ServerUser = true)]
		[ClientVar]
		public static void version(ConsoleSystem.Arg arg)
		{
			arg.ReplyWith(string.Format("Protocol: {0}\nBuild Date: {1}\nUnity Version: {2}\nChangeset: {3}\nBranch: {4}", new object[]
			{
				Protocol.printable,
				BuildInfo.Current.BuildDate,
				UnityEngine.Application.unityVersion,
				BuildInfo.Current.Scm.ChangeId,
				BuildInfo.Current.Scm.Branch
			}));
		}

		// Token: 0x0600420E RID: 16910 RVA: 0x001879E1 File Offset: 0x00185BE1
		[ServerVar]
		[ClientVar]
		public static void sysinfo(ConsoleSystem.Arg arg)
		{
			arg.ReplyWith(SystemInfoGeneralText.currentInfo);
		}

		// Token: 0x0600420F RID: 16911 RVA: 0x001879EE File Offset: 0x00185BEE
		[ServerVar]
		[ClientVar]
		public static void sysuid(ConsoleSystem.Arg arg)
		{
			arg.ReplyWith(SystemInfo.deviceUniqueIdentifier);
		}

		// Token: 0x06004210 RID: 16912 RVA: 0x001879FC File Offset: 0x00185BFC
		[ServerVar]
		public static void breakitem(ConsoleSystem.Arg args)
		{
			global::BasePlayer basePlayer = args.Player();
			if (!basePlayer)
			{
				return;
			}
			global::Item activeItem = basePlayer.GetActiveItem();
			if (activeItem != null)
			{
				activeItem.LoseCondition(activeItem.condition);
			}
		}

		// Token: 0x06004211 RID: 16913 RVA: 0x00187A30 File Offset: 0x00185C30
		[ServerVar]
		public static void breakclothing(ConsoleSystem.Arg args)
		{
			global::BasePlayer basePlayer = args.Player();
			if (!basePlayer)
			{
				return;
			}
			foreach (global::Item item in basePlayer.inventory.containerWear.itemList)
			{
				if (item != null)
				{
					item.LoseCondition(item.condition);
				}
			}
		}

		// Token: 0x06004212 RID: 16914 RVA: 0x00187AA8 File Offset: 0x00185CA8
		[ServerVar]
		[ClientVar]
		public static void subscriptions(ConsoleSystem.Arg arg)
		{
			TextTable textTable = new TextTable();
			textTable.AddColumn("realm");
			textTable.AddColumn("group");
			global::BasePlayer basePlayer = arg.Player();
			if (basePlayer)
			{
				foreach (Group group in basePlayer.net.subscriber.subscribed)
				{
					textTable.AddRow(new string[]
					{
						"sv",
						group.ID.ToString()
					});
				}
			}
			arg.ReplyWith(arg.HasArg("--json") ? textTable.ToJson() : textTable.ToString());
		}

		// Token: 0x06004213 RID: 16915 RVA: 0x00187B6C File Offset: 0x00185D6C
		public static uint GingerbreadMaterialID()
		{
			if (Global._gingerbreadMaterialID == 0U)
			{
				Global._gingerbreadMaterialID = StringPool.Get("Gingerbread");
			}
			return Global._gingerbreadMaterialID;
		}

		// Token: 0x06004214 RID: 16916 RVA: 0x00187B8C File Offset: 0x00185D8C
		[ServerVar]
		public static void ClearAllSprays()
		{
			List<SprayCanSpray> list = Pool.GetList<SprayCanSpray>();
			foreach (SprayCanSpray item in SprayCanSpray.AllSprays)
			{
				list.Add(item);
			}
			foreach (SprayCanSpray sprayCanSpray in list)
			{
				sprayCanSpray.Kill(global::BaseNetworkable.DestroyMode.None);
			}
			Pool.FreeList<SprayCanSpray>(ref list);
		}

		// Token: 0x06004215 RID: 16917 RVA: 0x00187C28 File Offset: 0x00185E28
		[ServerVar]
		public static void ClearAllSpraysByPlayer(ConsoleSystem.Arg arg)
		{
			if (!arg.HasArgs(1))
			{
				return;
			}
			ulong @ulong = arg.GetULong(0, 0UL);
			List<SprayCanSpray> list = Pool.GetList<SprayCanSpray>();
			foreach (SprayCanSpray sprayCanSpray in SprayCanSpray.AllSprays)
			{
				if (sprayCanSpray.sprayedByPlayer == @ulong)
				{
					list.Add(sprayCanSpray);
				}
			}
			foreach (SprayCanSpray sprayCanSpray2 in list)
			{
				sprayCanSpray2.Kill(global::BaseNetworkable.DestroyMode.None);
			}
			int count = list.Count;
			Pool.FreeList<SprayCanSpray>(ref list);
			arg.ReplyWith(string.Format("Deleted {0} sprays by {1}", count, @ulong));
		}

		// Token: 0x06004216 RID: 16918 RVA: 0x00187D08 File Offset: 0x00185F08
		[ServerVar]
		public static void ClearSpraysInRadius(ConsoleSystem.Arg arg)
		{
			global::BasePlayer basePlayer = arg.Player();
			if (basePlayer == null)
			{
				return;
			}
			float @float = arg.GetFloat(0, 16f);
			int num = Global.ClearSpraysInRadius(basePlayer.transform.position, @float);
			arg.ReplyWith(string.Format("Deleted {0} sprays within {1} of {2}", num, @float, basePlayer.displayName));
		}

		// Token: 0x06004217 RID: 16919 RVA: 0x00187D68 File Offset: 0x00185F68
		private static int ClearSpraysInRadius(Vector3 position, float radius)
		{
			List<SprayCanSpray> list = Pool.GetList<SprayCanSpray>();
			foreach (SprayCanSpray sprayCanSpray in SprayCanSpray.AllSprays)
			{
				if (sprayCanSpray.Distance(position) <= radius)
				{
					list.Add(sprayCanSpray);
				}
			}
			foreach (SprayCanSpray sprayCanSpray2 in list)
			{
				sprayCanSpray2.Kill(global::BaseNetworkable.DestroyMode.None);
			}
			int count = list.Count;
			Pool.FreeList<SprayCanSpray>(ref list);
			return count;
		}

		// Token: 0x06004218 RID: 16920 RVA: 0x00187E14 File Offset: 0x00186014
		[ServerVar]
		public static void ClearSpraysAtPositionInRadius(ConsoleSystem.Arg arg)
		{
			Vector3 vector = arg.GetVector3(0, default(Vector3));
			float @float = arg.GetFloat(1, 0f);
			if (@float == 0f)
			{
				return;
			}
			int num = Global.ClearSpraysInRadius(vector, @float);
			arg.ReplyWith(string.Format("Deleted {0} sprays within {1} of {2}", num, @float, vector));
		}

		// Token: 0x06004219 RID: 16921 RVA: 0x00187E74 File Offset: 0x00186074
		[ServerVar]
		public static void ClearDroppedItems()
		{
			List<DroppedItem> list = Pool.GetList<DroppedItem>();
			using (IEnumerator<global::BaseNetworkable> enumerator = global::BaseNetworkable.serverEntities.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					DroppedItem item;
					if ((item = (enumerator.Current as DroppedItem)) != null)
					{
						list.Add(item);
					}
				}
			}
			foreach (DroppedItem droppedItem in list)
			{
				droppedItem.Kill(global::BaseNetworkable.DestroyMode.None);
			}
			Pool.FreeList<DroppedItem>(ref list);
		}
	}
}
