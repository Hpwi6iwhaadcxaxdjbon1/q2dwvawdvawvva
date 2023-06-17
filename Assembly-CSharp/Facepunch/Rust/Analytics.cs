using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using ConVar;
using Network;
using Newtonsoft.Json;
using Rust;
using UnityEngine;

namespace Facepunch.Rust
{
	// Token: 0x02000AFF RID: 2815
	public static class Analytics
	{
		// Token: 0x04003CDA RID: 15578
		public static HashSet<string> StatsBlacklist;

		// Token: 0x04003CDB RID: 15579
		private static HashSet<NetworkableId> trackedSpawnedIds = new HashSet<NetworkableId>();

		// Token: 0x17000630 RID: 1584
		// (get) Token: 0x060044B1 RID: 17585 RVA: 0x00193505 File Offset: 0x00191705
		// (set) Token: 0x060044B2 RID: 17586 RVA: 0x0019350C File Offset: 0x0019170C
		public static string ClientAnalyticsUrl { get; set; } = "https://rust-api.facepunch.com/api/public/analytics/rust/client";

		// Token: 0x17000631 RID: 1585
		// (get) Token: 0x060044B3 RID: 17587 RVA: 0x00193514 File Offset: 0x00191714
		// (set) Token: 0x060044B4 RID: 17588 RVA: 0x0019351B File Offset: 0x0019171B
		[ServerVar(Name = "server_analytics_url")]
		public static string ServerAnalyticsUrl { get; set; } = "https://rust-api.facepunch.com/api/public/analytics/rust/server";

		// Token: 0x17000632 RID: 1586
		// (get) Token: 0x060044B5 RID: 17589 RVA: 0x00193523 File Offset: 0x00191723
		// (set) Token: 0x060044B6 RID: 17590 RVA: 0x0019352A File Offset: 0x0019172A
		[ServerVar(Name = "analytics_header", Saved = true)]
		public static string AnalyticsHeader { get; set; } = "X-API-KEY";

		// Token: 0x17000633 RID: 1587
		// (get) Token: 0x060044B7 RID: 17591 RVA: 0x00193532 File Offset: 0x00191732
		// (set) Token: 0x060044B8 RID: 17592 RVA: 0x00193539 File Offset: 0x00191739
		[ServerVar(Name = "analytics_secret", Saved = true)]
		public static string AnalyticsSecret { get; set; } = "";

		// Token: 0x17000634 RID: 1588
		// (get) Token: 0x060044B9 RID: 17593 RVA: 0x00193541 File Offset: 0x00191741
		// (set) Token: 0x060044BA RID: 17594 RVA: 0x00193548 File Offset: 0x00191748
		public static string AnalyticsPublicKey { get; set; } = "pub878ABLezSB6onshSwBCRGYDCpEI";

		// Token: 0x17000635 RID: 1589
		// (get) Token: 0x060044BB RID: 17595 RVA: 0x00193550 File Offset: 0x00191750
		// (set) Token: 0x060044BC RID: 17596 RVA: 0x00193557 File Offset: 0x00191757
		[ServerVar(Name = "high_freq_stats", Saved = true)]
		public static bool HighFrequencyStats { get; set; } = true;

		// Token: 0x060044BD RID: 17597 RVA: 0x00193560 File Offset: 0x00191760
		[ClientVar(Name = "pending_analytics")]
		[ServerVar(Name = "pending_analytics")]
		public static void GetPendingAnalytics(ConsoleSystem.Arg arg)
		{
			int pendingCount = Analytics.AzureWebInterface.server.PendingCount;
			arg.ReplyWith(string.Format("Pending: {0}", pendingCount));
		}

		// Token: 0x17000636 RID: 1590
		// (get) Token: 0x060044BE RID: 17598 RVA: 0x0019358E File Offset: 0x0019178E
		// (set) Token: 0x060044BF RID: 17599 RVA: 0x001935AC File Offset: 0x001917AC
		[ServerVar(Name = "stats_blacklist", Saved = true)]
		public static string stats_blacklist
		{
			get
			{
				if (Analytics.StatsBlacklist != null)
				{
					return string.Join(",", Analytics.StatsBlacklist);
				}
				return "";
			}
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					Analytics.StatsBlacklist = null;
					return;
				}
				Analytics.StatsBlacklist = new HashSet<string>(value.Split(new char[]
				{
					','
				}));
			}
		}

		// Token: 0x02000F84 RID: 3972
		public static class Azure
		{
			// Token: 0x0400500D RID: 20493
			private static Dictionary<int, string> geneCache = new Dictionary<int, string>();

			// Token: 0x0400500E RID: 20494
			public static int MaxMSPerFrame = 5;

			// Token: 0x0400500F RID: 20495
			private static Dictionary<Analytics.Azure.PendingItemsKey, Analytics.Azure.PendingItemsData> pendingItems = new Dictionary<Analytics.Azure.PendingItemsKey, Analytics.Azure.PendingItemsData>();

			// Token: 0x04005010 RID: 20496
			private static Dictionary<Analytics.Azure.FiredProjectileKey, Analytics.Azure.PendingFiredProjectile> firedProjectiles = new Dictionary<Analytics.Azure.FiredProjectileKey, Analytics.Azure.PendingFiredProjectile>();

			// Token: 0x060054DF RID: 21727 RVA: 0x001B67A8 File Offset: 0x001B49A8
			private static string GetGenesAsString(GrowableEntity plant)
			{
				int key = GrowableGeneEncoding.EncodeGenesToInt(plant.Genes);
				string result;
				if (!Analytics.Azure.geneCache.TryGetValue(key, out result))
				{
					result = string.Join("", from x in plant.Genes.Genes
					group x by x.GetDisplayCharacter() into x
					orderby x.Key
					select x.Count<GrowableGene>().ToString() + x.Key);
				}
				return result;
			}

			// Token: 0x060054E0 RID: 21728 RVA: 0x001B6858 File Offset: 0x001B4A58
			private static string GetMonument(BaseEntity entity)
			{
				if (entity == null)
				{
					return null;
				}
				SpawnGroup spawnGroup = null;
				BaseCorpse baseCorpse;
				if ((baseCorpse = (entity as BaseCorpse)) != null)
				{
					spawnGroup = baseCorpse.spawnGroup;
				}
				if (spawnGroup == null)
				{
					SpawnPointInstance component = entity.GetComponent<SpawnPointInstance>();
					if (component != null)
					{
						spawnGroup = (component.parentSpawnPointUser as SpawnGroup);
					}
				}
				if (spawnGroup != null)
				{
					if (!string.IsNullOrEmpty(spawnGroup.category))
					{
						return spawnGroup.category;
					}
					if (spawnGroup.Monument != null)
					{
						return spawnGroup.Monument.name;
					}
				}
				MonumentInfo monumentInfo = TerrainMeta.Path.FindMonumentWithBoundsOverlap(entity.transform.position);
				if (monumentInfo != null)
				{
					return monumentInfo.name;
				}
				return null;
			}

			// Token: 0x060054E1 RID: 21729 RVA: 0x001B6908 File Offset: 0x001B4B08
			private static string GetBiome(Vector3 position)
			{
				string result = null;
				TerrainBiome.Enum biomeMaxType = (TerrainBiome.Enum)TerrainMeta.BiomeMap.GetBiomeMaxType(position, -1);
				switch (biomeMaxType)
				{
				case TerrainBiome.Enum.Arid:
					result = "arid";
					break;
				case TerrainBiome.Enum.Temperate:
					result = "grass";
					break;
				case (TerrainBiome.Enum)3:
					break;
				case TerrainBiome.Enum.Tundra:
					result = "tundra";
					break;
				default:
					if (biomeMaxType == TerrainBiome.Enum.Arctic)
					{
						result = "arctic";
					}
					break;
				}
				return result;
			}

			// Token: 0x060054E2 RID: 21730 RVA: 0x001B6961 File Offset: 0x001B4B61
			private static bool IsOcean(Vector3 position)
			{
				return TerrainMeta.TopologyMap.GetTopology(position) == 128;
			}

			// Token: 0x060054E3 RID: 21731 RVA: 0x001B6975 File Offset: 0x001B4B75
			private static IEnumerator AggregateLoop()
			{
				int loop = 0;
				while (!Rust.Application.isQuitting)
				{
					yield return CoroutineEx.waitForSecondsRealtime(60f);
					if (Analytics.Azure.Stats)
					{
						yield return Analytics.Azure.TryCatch(Analytics.Azure.AggregatePlayers(false, true));
						if (loop % 60 == 0)
						{
							Analytics.Azure.PushServerInfo();
							yield return Analytics.Azure.TryCatch(Analytics.Azure.AggregateEntitiesAndItems());
							yield return Analytics.Azure.TryCatch(Analytics.Azure.AggregatePlayers(true, false));
							yield return Analytics.Azure.TryCatch(Analytics.Azure.AggregateTeams());
							Dictionary<Analytics.Azure.PendingItemsKey, Analytics.Azure.PendingItemsData> dict = Analytics.Azure.pendingItems;
							Analytics.Azure.pendingItems = new Dictionary<Analytics.Azure.PendingItemsKey, Analytics.Azure.PendingItemsData>();
							yield return Analytics.Azure.PushPendingItemsLoopAsync(dict);
						}
						int num = loop;
						loop = num + 1;
					}
				}
				yield break;
			}

			// Token: 0x060054E4 RID: 21732 RVA: 0x001B697D File Offset: 0x001B4B7D
			private static IEnumerator TryCatch(IEnumerator coroutine)
			{
				for (;;)
				{
					try
					{
						if (!coroutine.MoveNext())
						{
							yield break;
						}
					}
					catch (Exception exception)
					{
						UnityEngine.Debug.LogException(exception);
						yield break;
					}
					yield return coroutine.Current;
				}
				yield break;
			}

			// Token: 0x060054E5 RID: 21733 RVA: 0x001B698C File Offset: 0x001B4B8C
			private static IEnumerator AggregateEntitiesAndItems()
			{
				List<BaseNetworkable> entityQueue = new List<BaseNetworkable>();
				entityQueue.Clear();
				int totalCount = BaseNetworkable.serverEntities.Count;
				entityQueue.AddRange(BaseNetworkable.serverEntities);
				Dictionary<string, int> itemDict = new Dictionary<string, int>();
				Dictionary<Analytics.Azure.EntityKey, int> entityDict = new Dictionary<Analytics.Azure.EntityKey, int>();
				yield return null;
				UnityEngine.Debug.Log("Starting to aggregate entities & items...");
				DateTime startTime = DateTime.UtcNow;
				Stopwatch watch = Stopwatch.StartNew();
				foreach (BaseNetworkable entity in entityQueue)
				{
					if (watch.ElapsedMilliseconds > (long)Analytics.Azure.MaxMSPerFrame)
					{
						yield return null;
						watch.Restart();
					}
					if (!(entity == null) && !entity.IsDestroyed)
					{
						Analytics.Azure.EntityKey key = new Analytics.Azure.EntityKey
						{
							PrefabId = entity.prefabID
						};
						BuildingBlock buildingBlock;
						if ((buildingBlock = (entity as BuildingBlock)) != null)
						{
							key.Grade = (int)(buildingBlock.grade + 1);
						}
						int num;
						entityDict.TryGetValue(key, out num);
						entityDict[key] = num + 1;
						BasePlayer basePlayer;
						if (!(entity is LootContainer) && ((basePlayer = (entity as BasePlayer)) == null || !basePlayer.IsNpc) && !(entity is NPCPlayer))
						{
							BasePlayer basePlayer2;
							IItemContainerEntity itemContainerEntity;
							DroppedItemContainer droppedItemContainer;
							if ((basePlayer2 = (entity as BasePlayer)) != null)
							{
								Analytics.Azure.AddItemsToDict(basePlayer2.inventory.containerMain, itemDict);
								Analytics.Azure.AddItemsToDict(basePlayer2.inventory.containerBelt, itemDict);
								Analytics.Azure.AddItemsToDict(basePlayer2.inventory.containerWear, itemDict);
							}
							else if ((itemContainerEntity = (entity as IItemContainerEntity)) != null)
							{
								Analytics.Azure.AddItemsToDict(itemContainerEntity.inventory, itemDict);
							}
							else if ((droppedItemContainer = (entity as DroppedItemContainer)) != null && droppedItemContainer.inventory != null)
							{
								Analytics.Azure.AddItemsToDict(droppedItemContainer.inventory, itemDict);
							}
							entity = null;
						}
					}
				}
				List<BaseNetworkable>.Enumerator enumerator = default(List<BaseNetworkable>.Enumerator);
				UnityEngine.Debug.Log(string.Format("Took {0}s to aggregate {1} entities & items...", Math.Round(DateTime.UtcNow.Subtract(startTime).TotalSeconds, 1), totalCount));
				DateTime utcNow = DateTime.UtcNow;
				EventRecord.New("entity_sum", true).AddObject("counts", from x in entityDict
				select new Analytics.Azure.EntitySumItem
				{
					PrefabId = x.Key.PrefabId,
					Grade = x.Key.Grade,
					Count = x.Value
				}).Submit();
				yield return null;
				EventRecord.New("item_sum", true).AddObject("counts", itemDict).Submit();
				yield return null;
				yield break;
				yield break;
			}

			// Token: 0x060054E6 RID: 21734 RVA: 0x001B6994 File Offset: 0x001B4B94
			private static void AddItemsToDict(ItemContainer container, Dictionary<string, int> dict)
			{
				if (container == null || container.itemList == null)
				{
					return;
				}
				foreach (Item item in container.itemList)
				{
					string shortname = item.info.shortname;
					int num;
					dict.TryGetValue(shortname, out num);
					dict[shortname] = num + item.amount;
					if (item.contents != null)
					{
						Analytics.Azure.AddItemsToDict(item.contents, dict);
					}
				}
			}

			// Token: 0x060054E7 RID: 21735 RVA: 0x001B6A28 File Offset: 0x001B4C28
			private static IEnumerator PushPendingItemsLoopAsync(Dictionary<Analytics.Azure.PendingItemsKey, Analytics.Azure.PendingItemsData> dict)
			{
				Stopwatch watch = Stopwatch.StartNew();
				foreach (Analytics.Azure.PendingItemsData pendingItemsData in dict.Values)
				{
					try
					{
						Analytics.Azure.LogResource(pendingItemsData.Key.Consumed ? Analytics.Azure.ResourceMode.Consumed : Analytics.Azure.ResourceMode.Produced, pendingItemsData.category, pendingItemsData.Key.Item, pendingItemsData.amount, null, null, false, null, 0UL, pendingItemsData.Key.Entity, null, null);
					}
					catch (Exception exception)
					{
						UnityEngine.Debug.LogException(exception);
					}
					Analytics.Azure.PendingItemsData pendingItemsData2 = pendingItemsData;
					Pool.Free<Analytics.Azure.PendingItemsData>(ref pendingItemsData2);
					if (watch.ElapsedMilliseconds > (long)Analytics.Azure.MaxMSPerFrame)
					{
						yield return null;
						watch.Restart();
					}
				}
				Dictionary<Analytics.Azure.PendingItemsKey, Analytics.Azure.PendingItemsData>.ValueCollection.Enumerator enumerator = default(Dictionary<Analytics.Azure.PendingItemsKey, Analytics.Azure.PendingItemsData>.ValueCollection.Enumerator);
				dict.Clear();
				yield break;
				yield break;
			}

			// Token: 0x060054E8 RID: 21736 RVA: 0x001B6A38 File Offset: 0x001B4C38
			public static void AddPendingItems(BaseEntity entity, string itemName, int amount, string category, bool consumed = true, bool perEntity = false)
			{
				Analytics.Azure.PendingItemsKey key = new Analytics.Azure.PendingItemsKey
				{
					Entity = entity.ShortPrefabName,
					Category = category,
					Item = itemName,
					Consumed = consumed,
					EntityId = (perEntity ? entity.net.ID : default(NetworkableId))
				};
				Analytics.Azure.PendingItemsData pendingItemsData;
				if (!Analytics.Azure.pendingItems.TryGetValue(key, out pendingItemsData))
				{
					pendingItemsData = Pool.Get<Analytics.Azure.PendingItemsData>();
					pendingItemsData.Key = key;
					pendingItemsData.category = category;
					Analytics.Azure.pendingItems[key] = pendingItemsData;
				}
				pendingItemsData.amount += amount;
			}

			// Token: 0x060054E9 RID: 21737 RVA: 0x001B6AD3 File Offset: 0x001B4CD3
			private static IEnumerator AggregatePlayers(bool blueprints = false, bool positions = false)
			{
				Stopwatch watch = Stopwatch.StartNew();
				List<BasePlayer> list = Pool.GetList<BasePlayer>();
				list.AddRange(BasePlayer.activePlayerList);
				Dictionary<int, int> playerBps = blueprints ? new Dictionary<int, int>() : null;
				List<Analytics.Azure.PlayerPos> playerPositions = positions ? new List<Analytics.Azure.PlayerPos>() : null;
				foreach (BasePlayer basePlayer in list)
				{
					if (!(basePlayer == null) && !basePlayer.IsDestroyed)
					{
						if (blueprints)
						{
							foreach (int key in basePlayer.PersistantPlayerInfo.unlockedItems)
							{
								int num;
								playerBps.TryGetValue(key, out num);
								playerBps[key] = num + 1;
							}
						}
						if (positions)
						{
							playerPositions.Add(new Analytics.Azure.PlayerPos
							{
								UserId = basePlayer.WipeId,
								Position = basePlayer.transform.position,
								Direction = basePlayer.eyes.bodyRotation.eulerAngles
							});
						}
						if (watch.ElapsedMilliseconds > (long)Analytics.Azure.MaxMSPerFrame)
						{
							yield return null;
							watch.Restart();
						}
					}
				}
				List<BasePlayer>.Enumerator enumerator = default(List<BasePlayer>.Enumerator);
				if (blueprints)
				{
					EventRecord.New("blueprint_aggregate_online", true).AddObject("blueprints", from x in playerBps
					select new
					{
						Key = ItemManager.FindItemDefinition(x.Key).shortname,
						value = x.Value
					}).Submit();
				}
				if (positions)
				{
					EventRecord.New("player_positions", true).AddObject("positions", playerPositions).AddObject("player_count", playerPositions.Count).Submit();
				}
				yield break;
				yield break;
			}

			// Token: 0x060054EA RID: 21738 RVA: 0x001B6AE9 File Offset: 0x001B4CE9
			private static IEnumerator AggregateTeams()
			{
				yield return null;
				HashSet<ulong> teamIds = new HashSet<ulong>();
				int inTeam = 0;
				int notInTeam = 0;
				foreach (BasePlayer basePlayer in BasePlayer.activePlayerList)
				{
					if (basePlayer != null && !basePlayer.IsDestroyed && basePlayer.currentTeam != 0UL)
					{
						teamIds.Add(basePlayer.currentTeam);
						int num = inTeam;
						inTeam = num + 1;
					}
					else
					{
						int num = notInTeam;
						notInTeam = num + 1;
					}
				}
				yield return null;
				Stopwatch watch = Stopwatch.StartNew();
				List<Analytics.Azure.TeamInfo> teams = Pool.GetList<Analytics.Azure.TeamInfo>();
				foreach (ulong teamID in teamIds)
				{
					RelationshipManager.PlayerTeam playerTeam = RelationshipManager.ServerInstance.FindTeam(teamID);
					if (playerTeam != null && (playerTeam.members != null & playerTeam.members.Count > 0))
					{
						Analytics.Azure.TeamInfo teamInfo = Pool.Get<Analytics.Azure.TeamInfo>();
						teams.Add(teamInfo);
						foreach (ulong num2 in playerTeam.members)
						{
							BasePlayer basePlayer2 = RelationshipManager.FindByID(num2);
							if (basePlayer2 != null && !basePlayer2.IsDestroyed && basePlayer2.IsConnected && !basePlayer2.IsSleeping())
							{
								teamInfo.online.Add(SingletonComponent<ServerMgr>.Instance.persistance.GetUserWipeId(num2));
							}
							else
							{
								teamInfo.offline.Add(SingletonComponent<ServerMgr>.Instance.persistance.GetUserWipeId(num2));
							}
						}
						teamInfo.member_count = teamInfo.online.Count + teamInfo.offline.Count;
						if (watch.ElapsedMilliseconds > (long)Analytics.Azure.MaxMSPerFrame)
						{
							yield return null;
							watch.Restart();
						}
					}
				}
				HashSet<ulong>.Enumerator enumerator2 = default(HashSet<ulong>.Enumerator);
				EventRecord.New("online_teams", true).AddObject("teams", teams).AddField("users_in_team", inTeam).AddField("users_not_in_team", notInTeam).Submit();
				foreach (Analytics.Azure.TeamInfo teamInfo2 in teams)
				{
					Pool.Free<Analytics.Azure.TeamInfo>(ref teamInfo2);
				}
				Pool.FreeList<Analytics.Azure.TeamInfo>(ref teams);
				yield break;
				yield break;
			}

			// Token: 0x17000738 RID: 1848
			// (get) Token: 0x060054EB RID: 21739 RVA: 0x001B6AF1 File Offset: 0x001B4CF1
			public static bool Stats
			{
				get
				{
					return (!string.IsNullOrEmpty(Analytics.AnalyticsSecret) || ConVar.Server.official) && ConVar.Server.stats;
				}
			}

			// Token: 0x060054EC RID: 21740 RVA: 0x001B6B0D File Offset: 0x001B4D0D
			public static void Initialize()
			{
				Analytics.Azure.PushItemDefinitions();
				Analytics.Azure.PushEntityManifest();
				SingletonComponent<ServerMgr>.Instance.StartCoroutine(Analytics.Azure.AggregateLoop());
			}

			// Token: 0x060054ED RID: 21741 RVA: 0x001B6B2C File Offset: 0x001B4D2C
			private static void PushServerInfo()
			{
				if (!Analytics.Azure.Stats)
				{
					return;
				}
				try
				{
					EventRecord eventRecord = EventRecord.New("server_info", true).AddField("seed", global::World.Seed).AddField("size", global::World.Size).AddField("url", global::World.Url).AddField("wipe_id", SaveRestore.WipeId).AddField("ip_convar", Network.Net.sv.ip).AddField("port_convar", Network.Net.sv.port).AddField("net_protocol", Network.Net.sv.ProtocolId).AddField("protocol_network", 2392).AddField("protocol_save", 238);
					string key = "changeset";
					BuildInfo buildInfo = BuildInfo.Current;
					EventRecord eventRecord2 = eventRecord.AddField(key, ((buildInfo != null) ? buildInfo.Scm.ChangeId : null) ?? "0").AddField("unity_version", UnityEngine.Application.unityVersion);
					string key2 = "branch";
					BuildInfo buildInfo2 = BuildInfo.Current;
					eventRecord2.AddField(key2, ((buildInfo2 != null) ? buildInfo2.Scm.Branch : null) ?? "empty").AddField("server_tags", ConVar.Server.tags).AddField("device_id", SystemInfo.deviceUniqueIdentifier).AddField("network_id", Network.Net.sv.GetLastUIDGiven()).Submit();
				}
				catch (Exception exception)
				{
					UnityEngine.Debug.LogException(exception);
				}
			}

			// Token: 0x060054EE RID: 21742 RVA: 0x001B6CA0 File Offset: 0x001B4EA0
			private static void PushItemDefinitions()
			{
				if (!Analytics.Azure.Stats)
				{
					return;
				}
				try
				{
					if (!(GameManifest.Current == null))
					{
						BuildInfo buildInfo = BuildInfo.Current;
						bool flag;
						if (buildInfo == null)
						{
							flag = (null != null);
						}
						else
						{
							BuildInfo.ScmInfo scm = buildInfo.Scm;
							flag = (((scm != null) ? scm.ChangeId : null) != null);
						}
						if (flag)
						{
							EventRecord.New("item_definitions", true).AddObject("items", (from x in ItemManager.itemDictionary
							select x.Value).Select(delegate(ItemDefinition x)
							{
								int itemid = x.itemid;
								string shortname = x.shortname;
								ItemBlueprint blueprint = x.Blueprint;
								float craft_time = (blueprint != null) ? blueprint.time : 0f;
								ItemBlueprint blueprint2 = x.Blueprint;
								int workbench = (blueprint2 != null) ? blueprint2.workbenchLevelRequired : 0;
								string category = x.category.ToString();
								string english = x.displayName.english;
								Rarity despawnRarity = x.despawnRarity;
								ItemBlueprint blueprint3 = x.Blueprint;
								var ingredients;
								if (blueprint3 == null)
								{
									ingredients = null;
								}
								else
								{
									ingredients = from y in blueprint3.ingredients
									select new
									{
										shortname = y.itemDef.shortname,
										amount = (int)y.amount
									};
								}
								return new
								{
									item_id = itemid,
									shortname = shortname,
									craft_time = craft_time,
									workbench = workbench,
									category = category,
									display_name = english,
									despawn_rarity = despawnRarity,
									ingredients = ingredients
								};
							})).AddField("changeset", BuildInfo.Current.Scm.ChangeId).Submit();
						}
					}
				}
				catch (Exception exception)
				{
					UnityEngine.Debug.LogException(exception);
				}
			}

			// Token: 0x060054EF RID: 21743 RVA: 0x001B6D80 File Offset: 0x001B4F80
			private static void PushEntityManifest()
			{
				if (!Analytics.Azure.Stats)
				{
					return;
				}
				try
				{
					if (!(GameManifest.Current == null))
					{
						BuildInfo buildInfo = BuildInfo.Current;
						bool flag;
						if (buildInfo == null)
						{
							flag = (null != null);
						}
						else
						{
							BuildInfo.ScmInfo scm = buildInfo.Scm;
							flag = (((scm != null) ? scm.ChangeId : null) != null);
						}
						if (flag)
						{
							EventRecord eventRecord = EventRecord.New("entity_manifest", true).AddObject("entities", from x in GameManifest.Current.entities
							select new
							{
								shortname = Path.GetFileNameWithoutExtension(x),
								prefab_id = StringPool.Get(x.ToLower())
							});
							string key = "changeset";
							BuildInfo buildInfo2 = BuildInfo.Current;
							eventRecord.AddField(key, ((buildInfo2 != null) ? buildInfo2.Scm.ChangeId : null) ?? "editor").Submit();
						}
					}
				}
				catch (Exception exception)
				{
					UnityEngine.Debug.LogException(exception);
				}
			}

			// Token: 0x060054F0 RID: 21744 RVA: 0x001B6E50 File Offset: 0x001B5050
			public static void OnFiredProjectile(BasePlayer player, BasePlayer.FiredProjectile projectile, Guid projectileGroupId)
			{
				if (!Analytics.Azure.Stats || !Analytics.HighFrequencyStats)
				{
					return;
				}
				try
				{
					EventRecord eventRecord = EventRecord.New("entity_damage", true).AddField("start_pos", projectile.position).AddField("start_vel", projectile.initialVelocity).AddField("velocity_inherit", projectile.inheritedVelocity);
					string key = "ammo_item";
					ItemDefinition itemDef = projectile.itemDef;
					EventRecord record = eventRecord.AddField(key, (itemDef != null) ? itemDef.shortname : null).AddField("weapon", projectile.weaponSource).AddField("projectile_group", projectileGroupId).AddField("projectile_id", projectile.id).AddField("attacker", player).AddField("look_dir", player.tickViewAngles).AddField("model_state", (player.modelStateTick ?? player.modelState).flags);
					Analytics.Azure.PendingFiredProjectile pendingFiredProjectile = Pool.Get<Analytics.Azure.PendingFiredProjectile>();
					pendingFiredProjectile.Record = record;
					pendingFiredProjectile.FiredProjectile = projectile;
					Analytics.Azure.firedProjectiles[new Analytics.Azure.FiredProjectileKey(player.userID, projectile.id)] = pendingFiredProjectile;
				}
				catch (Exception exception)
				{
					UnityEngine.Debug.LogException(exception);
				}
			}

			// Token: 0x060054F1 RID: 21745 RVA: 0x001B6F74 File Offset: 0x001B5174
			public static void OnFiredProjectileRemoved(BasePlayer player, BasePlayer.FiredProjectile projectile)
			{
				if (!Analytics.Azure.Stats)
				{
					return;
				}
				try
				{
					Analytics.Azure.FiredProjectileKey key = new Analytics.Azure.FiredProjectileKey(player.userID, projectile.id);
					Analytics.Azure.PendingFiredProjectile pendingFiredProjectile;
					if (!Analytics.Azure.firedProjectiles.TryGetValue(key, out pendingFiredProjectile))
					{
						UnityEngine.Debug.LogWarning(string.Format("Can't find projectile for player '{0}' with id {1}", player, projectile.id));
					}
					else
					{
						if (!pendingFiredProjectile.Hit)
						{
							EventRecord record = pendingFiredProjectile.Record;
							if (projectile.updates.Count > 0)
							{
								record.AddObject("projectile_updates", projectile.updates);
							}
							record.Submit();
						}
						Pool.Free<Analytics.Azure.PendingFiredProjectile>(ref pendingFiredProjectile);
						Analytics.Azure.firedProjectiles.Remove(key);
					}
				}
				catch (Exception exception)
				{
					UnityEngine.Debug.LogException(exception);
				}
			}

			// Token: 0x060054F2 RID: 21746 RVA: 0x001B702C File Offset: 0x001B522C
			public static void OnQuarryItem(Analytics.Azure.ResourceMode mode, string item, int amount, MiningQuarry sourceEntity)
			{
				if (!Analytics.Azure.Stats)
				{
					return;
				}
				try
				{
					Analytics.Azure.AddPendingItems(sourceEntity, item, amount, "quarry", mode == Analytics.Azure.ResourceMode.Consumed, false);
				}
				catch (Exception exception)
				{
					UnityEngine.Debug.LogException(exception);
				}
			}

			// Token: 0x060054F3 RID: 21747 RVA: 0x001B7070 File Offset: 0x001B5270
			public static void OnExcavatorProduceItem(Item item, BaseEntity sourceEntity)
			{
				if (!Analytics.Azure.Stats)
				{
					return;
				}
				try
				{
					Analytics.Azure.AddPendingItems(sourceEntity, item.info.shortname, item.amount, "excavator", false, false);
				}
				catch (Exception exception)
				{
					UnityEngine.Debug.LogException(exception);
				}
			}

			// Token: 0x060054F4 RID: 21748 RVA: 0x001B70C0 File Offset: 0x001B52C0
			public static void OnExcavatorConsumeFuel(Item item, int amount, BaseEntity dieselEngine)
			{
				if (!Analytics.Azure.Stats)
				{
					return;
				}
				try
				{
					Analytics.Azure.LogResource(Analytics.Azure.ResourceMode.Consumed, "excavator", item.info.shortname, amount, dieselEngine, null, false, null, 0UL, null, null, null);
				}
				catch (Exception exception)
				{
					UnityEngine.Debug.LogException(exception);
				}
			}

			// Token: 0x060054F5 RID: 21749 RVA: 0x001B7110 File Offset: 0x001B5310
			public static void OnCraftItem(string item, int amount, BasePlayer player, BaseEntity workbench, bool inSafezone)
			{
				if (!Analytics.Azure.Stats)
				{
					return;
				}
				try
				{
					Analytics.Azure.LogResource(Analytics.Azure.ResourceMode.Produced, "craft", item, amount, null, null, inSafezone, workbench, (player != null) ? player.userID : 0UL, null, null, null);
				}
				catch (Exception exception)
				{
					UnityEngine.Debug.LogException(exception);
				}
			}

			// Token: 0x060054F6 RID: 21750 RVA: 0x001B7164 File Offset: 0x001B5364
			public static void OnCraftMaterialConsumed(string item, int amount, BasePlayer player, BaseEntity workbench, bool inSafezone, string targetItem)
			{
				if (!Analytics.Azure.Stats)
				{
					return;
				}
				try
				{
					Analytics.Azure.LogResource(Analytics.Azure.ResourceMode.Consumed, "craft", item, amount, null, null, inSafezone, workbench, (player != null) ? player.userID : 0UL, null, null, targetItem);
				}
				catch (Exception exception)
				{
					UnityEngine.Debug.LogException(exception);
				}
			}

			// Token: 0x060054F7 RID: 21751 RVA: 0x001B71BC File Offset: 0x001B53BC
			public static void OnConsumableUsed(BasePlayer player, Item item)
			{
				if (!Analytics.Azure.Stats)
				{
					return;
				}
				try
				{
					EventRecord.New("consumeable_used", true).AddField("player", player).AddField("item", item).Submit();
				}
				catch (Exception exception)
				{
					UnityEngine.Debug.LogException(exception);
				}
			}

			// Token: 0x060054F8 RID: 21752 RVA: 0x001B7214 File Offset: 0x001B5414
			public static void OnEntitySpawned(BaseEntity entity)
			{
				if (!Analytics.Azure.Stats)
				{
					return;
				}
				try
				{
					Analytics.trackedSpawnedIds.Add(entity.net.ID);
					EventRecord.New("entity_spawned", true).AddField("entity", entity).Submit();
				}
				catch (Exception exception)
				{
					UnityEngine.Debug.LogException(exception);
				}
			}

			// Token: 0x060054F9 RID: 21753 RVA: 0x001B7274 File Offset: 0x001B5474
			private static void TryLogEntityKilled(BaseNetworkable entity)
			{
				if (!Analytics.Azure.Stats)
				{
					return;
				}
				try
				{
					if (Analytics.trackedSpawnedIds.Contains(entity.net.ID))
					{
						EventRecord.New("entity_killed", true).AddField("entity", entity).Submit();
					}
				}
				catch (Exception exception)
				{
					UnityEngine.Debug.LogException(exception);
				}
			}

			// Token: 0x060054FA RID: 21754 RVA: 0x001B72DC File Offset: 0x001B54DC
			public static void OnMedUsed(string itemName, BasePlayer player, BasePlayer target)
			{
				if (!Analytics.Azure.Stats)
				{
					return;
				}
				try
				{
					EventRecord.New("med_used", true).AddField("player", player).AddField("target", target).AddField("item_name", itemName).Submit();
				}
				catch (Exception exception)
				{
					UnityEngine.Debug.LogException(exception);
				}
			}

			// Token: 0x060054FB RID: 21755 RVA: 0x001B733C File Offset: 0x001B553C
			public static void OnCodelockChanged(BasePlayer player, CodeLock codeLock, string oldCode, string newCode, bool isGuest)
			{
				if (!Analytics.Azure.Stats)
				{
					return;
				}
				try
				{
					EventRecord.New("code_change", true).AddField("player", player).AddField("codelock", codeLock).AddField("old_code", oldCode).AddField("new_code", newCode).AddField("is_guest", isGuest).Submit();
				}
				catch (Exception exception)
				{
					UnityEngine.Debug.LogException(exception);
				}
			}

			// Token: 0x060054FC RID: 21756 RVA: 0x001B73B4 File Offset: 0x001B55B4
			public static void OnCodeLockEntered(BasePlayer player, CodeLock codeLock, bool isGuest)
			{
				if (!Analytics.Azure.Stats)
				{
					return;
				}
				try
				{
					EventRecord.New("code_enter", true).AddField("player", player).AddField("codelock", codeLock).AddField("is_guest", isGuest).Submit();
				}
				catch (Exception exception)
				{
					UnityEngine.Debug.LogException(exception);
				}
			}

			// Token: 0x060054FD RID: 21757 RVA: 0x001B7414 File Offset: 0x001B5614
			public static void OnTeamChanged(string change, ulong teamId, ulong teamLeader, ulong user, List<ulong> members)
			{
				if (!Analytics.Azure.Stats)
				{
					return;
				}
				List<string> list = Pool.GetList<string>();
				try
				{
					if (members != null)
					{
						foreach (ulong playerID in members)
						{
							list.Add(SingletonComponent<ServerMgr>.Instance.persistance.GetUserWipeId(playerID));
						}
					}
					EventRecord.New("team_change", true).AddField("team_leader", SingletonComponent<ServerMgr>.Instance.persistance.GetUserWipeId(teamLeader)).AddField("team", teamId).AddField("target_user", SingletonComponent<ServerMgr>.Instance.persistance.GetUserWipeId(user)).AddField("change", change).AddObject("users", list).AddField("member_count", members.Count).Submit();
				}
				catch (Exception exception)
				{
					UnityEngine.Debug.LogException(exception);
				}
				Pool.FreeList<string>(ref list);
			}

			// Token: 0x060054FE RID: 21758 RVA: 0x001B7518 File Offset: 0x001B5718
			public static void OnEntityAuthChanged(BaseEntity entity, BasePlayer player, IEnumerable<ulong> authedList, string change, ulong targetUser)
			{
				if (!Analytics.Azure.Stats)
				{
					return;
				}
				try
				{
					string userWipeId = SingletonComponent<ServerMgr>.Instance.persistance.GetUserWipeId(targetUser);
					EventRecord.New("auth_change", true).AddField("entity", entity).AddField("player", player).AddField("target", userWipeId).AddObject("auth_list", from x in authedList
					select SingletonComponent<ServerMgr>.Instance.persistance.GetUserWipeId(x)).AddField("change", change).Submit();
				}
				catch (Exception exception)
				{
					UnityEngine.Debug.LogException(exception);
				}
			}

			// Token: 0x060054FF RID: 21759 RVA: 0x001B75C4 File Offset: 0x001B57C4
			public static void OnSleepingBagAssigned(BasePlayer player, SleepingBag bag, ulong targetUser)
			{
				if (!Analytics.Azure.Stats)
				{
					return;
				}
				try
				{
					string value = (targetUser != 0UL) ? SingletonComponent<ServerMgr>.Instance.persistance.GetUserWipeId(targetUser) : "";
					EventRecord.New("sleeping_bag_assign", true).AddField("entity", bag).AddField("player", player).AddField("target", value).Submit();
				}
				catch (Exception exception)
				{
					UnityEngine.Debug.LogException(exception);
				}
			}

			// Token: 0x06005500 RID: 21760 RVA: 0x001B7640 File Offset: 0x001B5840
			public static void OnFallDamage(BasePlayer player, float velocity, float damage)
			{
				if (!Analytics.Azure.Stats)
				{
					return;
				}
				try
				{
					EventRecord.New("fall_damage", true).AddField("player", player).AddField("velocity", velocity).AddField("damage", damage).Submit();
				}
				catch (Exception exception)
				{
					UnityEngine.Debug.LogException(exception);
				}
			}

			// Token: 0x06005501 RID: 21761 RVA: 0x001B76A0 File Offset: 0x001B58A0
			public static void OnResearchStarted(BasePlayer player, BaseEntity entity, Item item, int scrapCost)
			{
				if (!Analytics.Azure.Stats)
				{
					return;
				}
				try
				{
					EventRecord.New("research_start", true).AddField("player", player).AddField("item", item.info.shortname).AddField("scrap", scrapCost).AddField("entity", entity).Submit();
				}
				catch (Exception exception)
				{
					UnityEngine.Debug.LogException(exception);
				}
			}

			// Token: 0x06005502 RID: 21762 RVA: 0x001B7718 File Offset: 0x001B5918
			public static void OnBlueprintLearned(BasePlayer player, ItemDefinition item, string reason, BaseEntity entity = null)
			{
				if (!Analytics.Azure.Stats)
				{
					return;
				}
				try
				{
					EventRecord.New("blueprint_learned", true).AddField("player", player).AddField("item", item.shortname).AddField("reason", reason).AddField("entity", entity).Submit();
				}
				catch (Exception exception)
				{
					UnityEngine.Debug.LogException(exception);
				}
			}

			// Token: 0x06005503 RID: 21763 RVA: 0x001B7788 File Offset: 0x001B5988
			public static void OnItemRecycled(string item, int amount, Recycler recycler)
			{
				if (!Analytics.Azure.Stats)
				{
					return;
				}
				try
				{
					Analytics.Azure.LogResource(Analytics.Azure.ResourceMode.Consumed, "recycler", item, amount, recycler, null, false, null, recycler.LastLootedBy, null, null, null);
				}
				catch (Exception exception)
				{
					UnityEngine.Debug.LogException(exception);
				}
			}

			// Token: 0x06005504 RID: 21764 RVA: 0x001B77D4 File Offset: 0x001B59D4
			public static void OnRecyclerItemProduced(string item, int amount, Recycler recycler, Item sourceItem)
			{
				if (!Analytics.Azure.Stats)
				{
					return;
				}
				try
				{
					Analytics.Azure.LogResource(Analytics.Azure.ResourceMode.Produced, "recycler", item, amount, recycler, null, false, null, recycler.LastLootedBy, null, sourceItem, null);
				}
				catch (Exception exception)
				{
					UnityEngine.Debug.LogException(exception);
				}
			}

			// Token: 0x06005505 RID: 21765 RVA: 0x001B7820 File Offset: 0x001B5A20
			public static void OnGatherItem(string item, int amount, BaseEntity sourceEntity, BasePlayer player, AttackEntity weapon = null)
			{
				if (!Analytics.Azure.Stats)
				{
					return;
				}
				try
				{
					Analytics.Azure.LogResource(Analytics.Azure.ResourceMode.Produced, "gather", item, amount, sourceEntity, weapon, false, null, player.userID, null, null, null);
				}
				catch (Exception exception)
				{
					UnityEngine.Debug.LogException(exception);
				}
			}

			// Token: 0x06005506 RID: 21766 RVA: 0x001B786C File Offset: 0x001B5A6C
			public static void OnFirstLooted(BaseEntity entity, BasePlayer player)
			{
				if (!Analytics.Azure.Stats)
				{
					return;
				}
				try
				{
					LootContainer lootContainer;
					LootableCorpse lootableCorpse;
					if ((lootContainer = (entity as LootContainer)) != null)
					{
						Analytics.Azure.LogItemsLooted(player, entity, lootContainer.inventory, null);
						EventRecord.New("loot_entity", true).AddField("entity", entity).AddField("player", player).AddField("monument", Analytics.Azure.GetMonument(entity)).AddField("biome", Analytics.Azure.GetBiome(entity.transform.position)).Submit();
					}
					else if ((lootableCorpse = (entity as LootableCorpse)) != null)
					{
						foreach (ItemContainer container in lootableCorpse.containers)
						{
							Analytics.Azure.LogItemsLooted(player, entity, container, null);
						}
					}
				}
				catch (Exception exception)
				{
					UnityEngine.Debug.LogException(exception);
				}
			}

			// Token: 0x06005507 RID: 21767 RVA: 0x001B7934 File Offset: 0x001B5B34
			public static void OnLootContainerDestroyed(LootContainer entity, BasePlayer player, AttackEntity weapon)
			{
				if (!Analytics.Azure.Stats)
				{
					return;
				}
				try
				{
					if (entity.DropsLoot && player != null && Vector3.Distance(entity.transform.position, player.transform.position) < 50f)
					{
						ItemContainer inventory = entity.inventory;
						if (((inventory != null) ? inventory.itemList : null) != null && entity.inventory.itemList.Count > 0)
						{
							Analytics.Azure.LogItemsLooted(player, entity, entity.inventory, weapon);
						}
					}
				}
				catch (Exception exception)
				{
					UnityEngine.Debug.LogException(exception);
				}
			}

			// Token: 0x06005508 RID: 21768 RVA: 0x001B79CC File Offset: 0x001B5BCC
			public static void OnEntityDestroyed(BaseNetworkable entity)
			{
				Analytics.Azure.TryLogEntityKilled(entity);
				if (!Analytics.Azure.Stats)
				{
					return;
				}
				try
				{
					LootContainer lootContainer;
					if ((lootContainer = (entity as LootContainer)) != null && lootContainer.FirstLooted)
					{
						foreach (Item item in lootContainer.inventory.itemList)
						{
							Analytics.Azure.OnItemDespawn(lootContainer, item, 3, lootContainer.LastLootedBy);
						}
					}
				}
				catch (Exception exception)
				{
					UnityEngine.Debug.LogException(exception);
				}
			}

			// Token: 0x06005509 RID: 21769 RVA: 0x001B7A64 File Offset: 0x001B5C64
			public static void OnEntityBuilt(BaseEntity entity, BasePlayer player)
			{
				if (!Analytics.Azure.Stats)
				{
					return;
				}
				try
				{
					EventRecord eventRecord = EventRecord.New("entity_built", true).AddField("player", player).AddField("entity", entity);
					if (entity is SleepingBag)
					{
						int sleepingBagCount = SleepingBag.GetSleepingBagCount(player.userID);
						eventRecord.AddField("bags_active", sleepingBagCount);
						eventRecord.AddField("max_sleeping_bags", ConVar.Server.max_sleeping_bags);
					}
					eventRecord.Submit();
				}
				catch (Exception exception)
				{
					UnityEngine.Debug.LogException(exception);
				}
			}

			// Token: 0x0600550A RID: 21770 RVA: 0x001B7AF0 File Offset: 0x001B5CF0
			public static void OnKeycardSwiped(BasePlayer player, CardReader cardReader)
			{
				if (!Analytics.Azure.Stats)
				{
					return;
				}
				try
				{
					EventRecord.New("keycard_swiped", true).AddField("player", player).AddField("card_level", cardReader.accessLevel).AddField("entity", cardReader).Submit();
				}
				catch (Exception exception)
				{
					UnityEngine.Debug.LogException(exception);
				}
			}

			// Token: 0x0600550B RID: 21771 RVA: 0x001B7B58 File Offset: 0x001B5D58
			public static void OnLockedCrateStarted(BasePlayer player, HackableLockedCrate crate)
			{
				if (!Analytics.Azure.Stats)
				{
					return;
				}
				try
				{
					EventRecord.New("hackable_crate_started", true).AddField("player", player).AddField("entity", crate).Submit();
				}
				catch (Exception exception)
				{
					UnityEngine.Debug.LogException(exception);
				}
			}

			// Token: 0x0600550C RID: 21772 RVA: 0x001B7BB0 File Offset: 0x001B5DB0
			public static void OnLockedCrateFinished(ulong player, HackableLockedCrate crate)
			{
				if (!Analytics.Azure.Stats)
				{
					return;
				}
				try
				{
					string userWipeId = SingletonComponent<ServerMgr>.Instance.persistance.GetUserWipeId(player);
					EventRecord.New("hackable_crate_ended", true).AddField("player_userid", userWipeId).AddField("entity", crate).Submit();
				}
				catch (Exception exception)
				{
					UnityEngine.Debug.LogException(exception);
				}
			}

			// Token: 0x0600550D RID: 21773 RVA: 0x001B7C18 File Offset: 0x001B5E18
			public static void OnStashHidden(BasePlayer player, StashContainer entity)
			{
				if (!Analytics.Azure.Stats)
				{
					return;
				}
				try
				{
					EventRecord.New("stash_hidden", true).AddField("player", player).AddField("entity", entity).AddField("owner", SingletonComponent<ServerMgr>.Instance.persistance.GetUserWipeId(entity.OwnerID)).Submit();
				}
				catch (Exception exception)
				{
					UnityEngine.Debug.LogException(exception);
				}
			}

			// Token: 0x0600550E RID: 21774 RVA: 0x001B7C8C File Offset: 0x001B5E8C
			public static void OnStashRevealed(BasePlayer player, StashContainer entity)
			{
				if (!Analytics.Azure.Stats)
				{
					return;
				}
				try
				{
					EventRecord.New("stash_reveal", true).AddField("player", player).AddField("entity", entity).AddField("owner", SingletonComponent<ServerMgr>.Instance.persistance.GetUserWipeId(entity.OwnerID)).Submit();
				}
				catch (Exception exception)
				{
					UnityEngine.Debug.LogException(exception);
				}
			}

			// Token: 0x0600550F RID: 21775 RVA: 0x001B7D00 File Offset: 0x001B5F00
			public static void OnAntihackViolation(BasePlayer player, int type, string message)
			{
				if (!Analytics.Azure.Stats)
				{
					return;
				}
				try
				{
					EventRecord.New("antihack_violation", true).AddField("player", player).AddField("violation_type", type).AddField("message", message).Submit();
				}
				catch (Exception exception)
				{
					UnityEngine.Debug.LogException(exception);
				}
			}

			// Token: 0x06005510 RID: 21776 RVA: 0x001B7D60 File Offset: 0x001B5F60
			public static void OnEyehackViolation(BasePlayer player, Vector3 eyePos)
			{
				if (!Analytics.Azure.Stats)
				{
					return;
				}
				try
				{
					EventRecord.New("antihack_violation_detailed", true).AddField("player", player).AddField("violation_type", 6).AddField("eye_pos", eyePos).Submit();
				}
				catch (Exception exception)
				{
					UnityEngine.Debug.LogException(exception);
				}
			}

			// Token: 0x06005511 RID: 21777 RVA: 0x001B7DC0 File Offset: 0x001B5FC0
			public static void OnNoclipViolation(BasePlayer player, Vector3 startPos, Vector3 endPos, int tickCount, Collider collider)
			{
				if (!Analytics.Azure.Stats || !Analytics.HighFrequencyStats)
				{
					return;
				}
				try
				{
					EventRecord.New("antihack_violation_detailed", true).AddField("player", player).AddField("violation_type", 1).AddField("start_pos", startPos).AddField("end_pos", endPos).AddField("tick_count", tickCount).AddField("collider_name", collider.name).Submit();
				}
				catch (Exception exception)
				{
					UnityEngine.Debug.LogException(exception);
				}
			}

			// Token: 0x06005512 RID: 21778 RVA: 0x001B7E50 File Offset: 0x001B6050
			public static void OnFlyhackViolation(BasePlayer player, Vector3 startPos, Vector3 endPos, int tickCount)
			{
				if (!Analytics.Azure.Stats)
				{
					return;
				}
				try
				{
					EventRecord.New("antihack_violation_detailed", true).AddField("player", player).AddField("violation_type", 3).AddField("start_pos", startPos).AddField("end_pos", endPos).AddField("tick_count", tickCount).Submit();
				}
				catch (Exception exception)
				{
					UnityEngine.Debug.LogException(exception);
				}
			}

			// Token: 0x06005513 RID: 21779 RVA: 0x001B7EC8 File Offset: 0x001B60C8
			public static void OnProjectileHackViolation(BasePlayer.FiredProjectile projectile)
			{
				if (!Analytics.Azure.Stats)
				{
					return;
				}
				try
				{
					Analytics.Azure.FiredProjectileKey key = new Analytics.Azure.FiredProjectileKey(projectile.attacker.userID, projectile.id);
					Analytics.Azure.PendingFiredProjectile pendingFiredProjectile;
					if (!Analytics.Azure.firedProjectiles.TryGetValue(key, out pendingFiredProjectile))
					{
						UnityEngine.Debug.LogWarning(string.Format("Can't find projectile for player '{0}' with id {1}", projectile.attacker, projectile.id));
					}
					else
					{
						pendingFiredProjectile.Record.AddField("projectile_invalid", true).AddObject("updates", projectile.updates);
					}
				}
				catch (Exception exception)
				{
					UnityEngine.Debug.LogException(exception);
				}
			}

			// Token: 0x06005514 RID: 21780 RVA: 0x001B7F64 File Offset: 0x001B6164
			public static void OnSpeedhackViolation(BasePlayer player, Vector3 startPos, Vector3 endPos, int tickCount)
			{
				if (!Analytics.Azure.Stats)
				{
					return;
				}
				try
				{
					EventRecord.New("antihack_violation_detailed", true).AddField("player", player).AddField("violation_type", 2).AddField("start_pos", startPos).AddField("end_pos", endPos).AddField("tick_count", tickCount).AddField("distance", Vector3.Distance(startPos, endPos)).AddField("distance_2d", Vector3Ex.Distance2D(startPos, endPos)).Submit();
				}
				catch (Exception exception)
				{
					UnityEngine.Debug.LogException(exception);
				}
			}

			// Token: 0x06005515 RID: 21781 RVA: 0x001B7FFC File Offset: 0x001B61FC
			public static void OnTerrainHackViolation(BasePlayer player)
			{
				if (!Analytics.Azure.Stats)
				{
					return;
				}
				try
				{
					EventRecord.New("antihack_violation_detailed", true).AddField("player", player).AddField("violation_type", 10).AddField("seed", global::World.Seed).AddField("size", global::World.Size).AddField("map_url", global::World.Url).AddField("map_checksum", global::World.Checksum).Submit();
				}
				catch (Exception exception)
				{
					UnityEngine.Debug.LogException(exception);
				}
			}

			// Token: 0x06005516 RID: 21782 RVA: 0x001B8090 File Offset: 0x001B6290
			public static void OnEntityTakeDamage(HitInfo info, bool isDeath)
			{
				if (!Analytics.Azure.Stats || !Analytics.HighFrequencyStats)
				{
					return;
				}
				try
				{
					BasePlayer initiatorPlayer = info.InitiatorPlayer;
					BasePlayer basePlayer = info.HitEntity as BasePlayer;
					if (!(info.Initiator == null) || isDeath)
					{
						if ((!(initiatorPlayer == null) && !initiatorPlayer.IsNpc && !initiatorPlayer.IsBot) || (!(basePlayer == null) && !basePlayer.IsNpc && !basePlayer.IsBot))
						{
							EventRecord eventRecord = null;
							float value = -1f;
							float value2 = -1f;
							if (initiatorPlayer != null)
							{
								if (info.IsProjectile())
								{
									Analytics.Azure.FiredProjectileKey key = new Analytics.Azure.FiredProjectileKey(initiatorPlayer.userID, info.ProjectileID);
									Analytics.Azure.PendingFiredProjectile pendingFiredProjectile;
									if (Analytics.Azure.firedProjectiles.TryGetValue(key, out pendingFiredProjectile))
									{
										eventRecord = pendingFiredProjectile.Record;
										value = Vector3.Distance(info.HitNormalWorld, pendingFiredProjectile.FiredProjectile.initialPosition);
										value = Vector3Ex.Distance2D(info.HitNormalWorld, pendingFiredProjectile.FiredProjectile.initialPosition);
										pendingFiredProjectile.Hit = info.DidHit;
									}
								}
								else
								{
									value = Vector3.Distance(info.HitNormalWorld, initiatorPlayer.eyes.position);
									value = Vector3Ex.Distance2D(info.HitNormalWorld, initiatorPlayer.eyes.position);
								}
							}
							if (eventRecord == null)
							{
								eventRecord = EventRecord.New("entity_damage", true);
							}
							eventRecord.AddField("is_hit", true).AddField("is_headshot", info.isHeadshot).AddField("victim", info.HitEntity).AddField("damage", info.damageTypes.Total()).AddField("damage_type", info.damageTypes.GetMajorityDamageType().ToString()).AddField("pos_world", info.HitPositionWorld).AddField("pos_local", info.HitPositionLocal).AddField("point_start", info.PointStart).AddField("point_end", info.PointEnd).AddField("normal_world", info.HitNormalWorld).AddField("normal_local", info.HitNormalLocal).AddField("distance_cl", info.ProjectileDistance).AddField("distance", value).AddField("distance_2d", value2);
							if (!info.IsProjectile())
							{
								eventRecord.AddField("weapon", info.Weapon);
								eventRecord.AddField("attacker", info.Initiator);
							}
							if (info.HitBone != 0U)
							{
								eventRecord.AddField("bone", info.HitBone).AddField("bone_name", info.boneName).AddField("hit_area", (int)info.boneArea);
							}
							if (info.ProjectileID != 0)
							{
								eventRecord.AddField("projectile_id", info.ProjectileID).AddField("projectile_integrity", info.ProjectileIntegrity).AddField("projectile_hits", info.ProjectileHits).AddField("trajectory_mismatch", info.ProjectileTrajectoryMismatch).AddField("travel_time", info.ProjectileTravelTime).AddField("projectile_velocity", info.ProjectileVelocity).AddField("projectile_prefab", info.ProjectilePrefab.name);
							}
							if (initiatorPlayer != null && !info.IsProjectile())
							{
								eventRecord.AddField("attacker_eye_pos", initiatorPlayer.eyes.position);
								eventRecord.AddField("attacker_eye_dir", initiatorPlayer.eyes.BodyForward());
								if (initiatorPlayer.GetType() == typeof(BasePlayer))
								{
									eventRecord.AddField("attacker_life", initiatorPlayer.respawnId);
								}
								if (isDeath)
								{
									eventRecord.AddObject("attacker_worn", from x in initiatorPlayer.inventory.containerWear.itemList
									select new Analytics.Azure.SimpleItemAmount(x));
									eventRecord.AddObject("attacker_hotbar", from x in initiatorPlayer.inventory.containerBelt.itemList
									select new Analytics.Azure.SimpleItemAmount(x));
								}
							}
							if (basePlayer != null)
							{
								eventRecord.AddField("victim_life", basePlayer.respawnId);
								eventRecord.AddObject("victim_worn", from x in basePlayer.inventory.containerWear.itemList
								select new Analytics.Azure.SimpleItemAmount(x));
								eventRecord.AddObject("victim_hotbar", from x in basePlayer.inventory.containerBelt.itemList
								select new Analytics.Azure.SimpleItemAmount(x));
							}
							eventRecord.Submit();
						}
					}
				}
				catch (Exception exception)
				{
					UnityEngine.Debug.LogException(exception);
				}
			}

			// Token: 0x06005517 RID: 21783 RVA: 0x001B8568 File Offset: 0x001B6768
			public static void OnPlayerRespawned(BasePlayer player, BaseEntity targetEntity)
			{
				if (!Analytics.Azure.Stats)
				{
					return;
				}
				try
				{
					EventRecord.New("player_respawn", true).AddField("player", player).AddField("bag", targetEntity).AddField("life_id", player.respawnId).Submit();
				}
				catch (Exception exception)
				{
					UnityEngine.Debug.LogException(exception);
				}
			}

			// Token: 0x06005518 RID: 21784 RVA: 0x001B85D0 File Offset: 0x001B67D0
			public static void OnExplosiveLaunched(BasePlayer player, BaseEntity explosive, BaseEntity launcher = null)
			{
				if (!Analytics.Azure.Stats)
				{
					return;
				}
				try
				{
					EventRecord eventRecord = EventRecord.New("explosive_launch", true).AddField("player", player).AddField("explosive", explosive).AddField("explosive_velocity", explosive.GetWorldVelocity()).AddField("explosive_direction", explosive.GetWorldVelocity().normalized);
					if (launcher != null)
					{
						eventRecord.AddField("launcher", launcher);
					}
					eventRecord.Submit();
				}
				catch (Exception exception)
				{
					UnityEngine.Debug.LogException(exception);
				}
			}

			// Token: 0x06005519 RID: 21785 RVA: 0x001B8668 File Offset: 0x001B6868
			public static void OnExplosion(TimedExplosive explosive)
			{
				if (!Analytics.Azure.Stats)
				{
					return;
				}
				try
				{
					EventRecord.New("explosion", true).AddField("entity", explosive).Submit();
				}
				catch (Exception exception)
				{
					UnityEngine.Debug.LogException(exception);
				}
			}

			// Token: 0x0600551A RID: 21786 RVA: 0x001B86B4 File Offset: 0x001B68B4
			public static void OnItemDespawn(BaseEntity itemContainer, Item item, int dropReason, ulong userId)
			{
				if (!Analytics.Azure.Stats)
				{
					return;
				}
				try
				{
					EventRecord eventRecord = EventRecord.New("item_despawn", true).AddField("entity", itemContainer).AddField("item", item).AddField("drop_reason", dropReason);
					if (userId != 0UL)
					{
						eventRecord.AddField("player_userid", userId);
					}
					eventRecord.Submit();
				}
				catch (Exception exception)
				{
					UnityEngine.Debug.LogException(exception);
				}
			}

			// Token: 0x0600551B RID: 21787 RVA: 0x001B8728 File Offset: 0x001B6928
			public static void OnItemDropped(BasePlayer player, WorldItem entity, DroppedItem.DropReasonEnum dropReason)
			{
				if (!Analytics.Azure.Stats)
				{
					return;
				}
				try
				{
					EventRecord.New("item_drop", true).AddField("player", player).AddField("entity", entity).AddField("item", entity.GetItem()).AddField("drop_reason", (int)dropReason).Submit();
				}
				catch (Exception exception)
				{
					UnityEngine.Debug.LogException(exception);
				}
			}

			// Token: 0x0600551C RID: 21788 RVA: 0x001B8798 File Offset: 0x001B6998
			public static void OnItemPickup(BasePlayer player, WorldItem entity)
			{
				if (!Analytics.Azure.Stats)
				{
					return;
				}
				try
				{
					EventRecord.New("item_pickup", true).AddField("player", player).AddField("entity", entity).AddField("item", entity.GetItem()).Submit();
				}
				catch (Exception exception)
				{
					UnityEngine.Debug.LogException(exception);
				}
			}

			// Token: 0x0600551D RID: 21789 RVA: 0x001B8800 File Offset: 0x001B6A00
			public static void OnPlayerConnected(Connection connection)
			{
				if (!Analytics.Azure.Stats)
				{
					return;
				}
				try
				{
					string userWipeId = SingletonComponent<ServerMgr>.Instance.persistance.GetUserWipeId(connection.userid);
					EventRecord.New("player_connect", true).AddField("player_userid", userWipeId).AddField("username", connection.username).Submit();
				}
				catch (Exception exception)
				{
					UnityEngine.Debug.LogException(exception);
				}
			}

			// Token: 0x0600551E RID: 21790 RVA: 0x001B8870 File Offset: 0x001B6A70
			public static void OnPlayerDisconnected(Connection connection, string reason)
			{
				if (!Analytics.Azure.Stats)
				{
					return;
				}
				try
				{
					string userWipeId = SingletonComponent<ServerMgr>.Instance.persistance.GetUserWipeId(connection.userid);
					EventRecord.New("player_disconnect", true).AddField("player_userid", userWipeId).AddField("username", connection.username).AddField("reason", reason).Submit();
				}
				catch (Exception exception)
				{
					UnityEngine.Debug.LogException(exception);
				}
			}

			// Token: 0x0600551F RID: 21791 RVA: 0x001B88EC File Offset: 0x001B6AEC
			public static void OnEntityPickedUp(BasePlayer player, BaseEntity entity)
			{
				if (!Analytics.Azure.Stats)
				{
					return;
				}
				try
				{
					EventRecord.New("entity_pickup", true).AddField("player", player).AddField("entity", entity).Submit();
				}
				catch (Exception exception)
				{
					UnityEngine.Debug.LogException(exception);
				}
			}

			// Token: 0x06005520 RID: 21792 RVA: 0x001B8944 File Offset: 0x001B6B44
			public static void OnChatMessage(BasePlayer player, string message, int channel)
			{
				if (!Analytics.Azure.Stats)
				{
					return;
				}
				try
				{
					EventRecord.New("chat", true).AddField("player", player).AddField("message", message).AddField("channel", channel).Submit();
				}
				catch (Exception exception)
				{
					UnityEngine.Debug.LogException(exception);
				}
			}

			// Token: 0x06005521 RID: 21793 RVA: 0x001B89A4 File Offset: 0x001B6BA4
			public static void OnVendingMachineOrderChanged(BasePlayer player, VendingMachine vendingMachine, int sellItemId, int sellAmount, bool sellingBp, int buyItemId, int buyAmount, bool buyingBp, bool added)
			{
				if (!Analytics.Azure.Stats)
				{
					return;
				}
				try
				{
					ItemDefinition itemDefinition = ItemManager.FindItemDefinition(sellItemId);
					ItemDefinition itemDefinition2 = ItemManager.FindItemDefinition(buyItemId);
					EventRecord.New("vending_changed", true).AddField("player", player).AddField("entity", vendingMachine).AddField("sell_item", itemDefinition.shortname).AddField("sell_amount", sellAmount).AddField("buy_item", itemDefinition2.shortname).AddField("buy_amount", buyAmount).AddField("is_selling_bp", sellingBp).AddField("is_buying_bp", buyingBp).AddField("change", added ? "added" : "removed").Submit();
				}
				catch (Exception exception)
				{
					UnityEngine.Debug.LogException(exception);
				}
			}

			// Token: 0x06005522 RID: 21794 RVA: 0x001B8A74 File Offset: 0x001B6C74
			public static void OnBuyFromVendingMachine(BasePlayer player, VendingMachine vendingMachine, int sellItemId, int sellAmount, bool sellingBp, int buyItemId, int buyAmount, bool buyingBp, int numberOfTransactions, BaseEntity drone = null)
			{
				if (!Analytics.Azure.Stats)
				{
					return;
				}
				try
				{
					ItemDefinition itemDefinition = ItemManager.FindItemDefinition(sellItemId);
					ItemDefinition itemDefinition2 = ItemManager.FindItemDefinition(buyItemId);
					EventRecord.New("vending_sale", true).AddField("player", player).AddField("entity", vendingMachine).AddField("sell_item", itemDefinition.shortname).AddField("sell_amount", sellAmount).AddField("buy_item", itemDefinition2.shortname).AddField("buy_amount", buyAmount).AddField("transactions", numberOfTransactions).AddField("is_selling_bp", sellingBp).AddField("is_buying_bp", buyingBp).AddField("drone_terminal", drone).Submit();
				}
				catch (Exception exception)
				{
					UnityEngine.Debug.LogException(exception);
				}
			}

			// Token: 0x06005523 RID: 21795 RVA: 0x001B8B40 File Offset: 0x001B6D40
			public static void OnNPCVendor(BasePlayer player, NPCTalking vendor, int scrapCost, string action)
			{
				if (!Analytics.Azure.Stats)
				{
					return;
				}
				try
				{
					EventRecord.New("npc_vendor", true).AddField("player", player).AddField("vendor", vendor).AddField("scrap_amount", scrapCost).AddField("action", action).Submit();
				}
				catch (Exception exception)
				{
					UnityEngine.Debug.LogException(exception);
				}
			}

			// Token: 0x06005524 RID: 21796 RVA: 0x001B8BAC File Offset: 0x001B6DAC
			private static void LogItemsLooted(BasePlayer looter, BaseEntity entity, ItemContainer container, AttackEntity tool = null)
			{
				if (!Analytics.Azure.Stats)
				{
					return;
				}
				try
				{
					if (!(entity == null) && container != null)
					{
						foreach (Item item in container.itemList)
						{
							if (item != null)
							{
								Analytics.Azure.ResourceMode mode = Analytics.Azure.ResourceMode.Produced;
								string category = "loot";
								string shortname = item.info.shortname;
								int amount = item.amount;
								ulong steamId = (looter != null) ? looter.userID : 0UL;
								Analytics.Azure.LogResource(mode, category, shortname, amount, entity, tool, false, null, steamId, null, null, null);
							}
						}
					}
				}
				catch (Exception exception)
				{
					UnityEngine.Debug.LogException(exception);
				}
			}

			// Token: 0x06005525 RID: 21797 RVA: 0x001B8C5C File Offset: 0x001B6E5C
			public static void LogResource(Analytics.Azure.ResourceMode mode, string category, string itemName, int amount, BaseEntity sourceEntity = null, AttackEntity tool = null, bool safezone = false, BaseEntity workbench = null, ulong steamId = 0UL, string sourceEntityPrefab = null, Item sourceItem = null, string targetItem = null)
			{
				if (!Analytics.Azure.Stats || !Analytics.HighFrequencyStats)
				{
					return;
				}
				try
				{
					EventRecord eventRecord = EventRecord.New("item_event", true).AddField("item_mode", mode.ToString()).AddField("category", category).AddField("item_name", itemName).AddField("amount", amount);
					if (sourceEntity != null)
					{
						eventRecord.AddField("entity", sourceEntity);
						string biome = Analytics.Azure.GetBiome(sourceEntity.transform.position);
						if (biome != null)
						{
							eventRecord.AddField("biome", biome);
						}
						if (Analytics.Azure.IsOcean(sourceEntity.transform.position))
						{
							eventRecord.AddField("ocean", true);
						}
						string monument = Analytics.Azure.GetMonument(sourceEntity);
						if (monument != null)
						{
							eventRecord.AddField("monument", monument);
						}
					}
					if (sourceEntityPrefab != null)
					{
						eventRecord.AddField("entity_prefab", sourceEntityPrefab);
					}
					if (tool != null)
					{
						eventRecord.AddField("tool", tool);
					}
					if (safezone)
					{
						eventRecord.AddField("safezone", true);
					}
					if (workbench != null)
					{
						eventRecord.AddField("workbench", workbench);
					}
					GrowableEntity plant;
					if ((plant = (sourceEntity as GrowableEntity)) != null)
					{
						eventRecord.AddField("genes", Analytics.Azure.GetGenesAsString(plant));
					}
					if (sourceItem != null)
					{
						eventRecord.AddField("source_item", sourceItem.info.shortname);
					}
					if (targetItem != null)
					{
						eventRecord.AddField("target_item", targetItem);
					}
					if (steamId != 0UL)
					{
						string userWipeId = SingletonComponent<ServerMgr>.Instance.persistance.GetUserWipeId(steamId);
						eventRecord.AddField("player_userid", userWipeId);
					}
					eventRecord.Submit();
				}
				catch (Exception exception)
				{
					UnityEngine.Debug.LogException(exception);
				}
			}

			// Token: 0x06005526 RID: 21798 RVA: 0x001B8E20 File Offset: 0x001B7020
			public static void OnSkinChanged(BasePlayer player, RepairBench repairBench, Item item, ulong workshopId)
			{
				if (!Analytics.Azure.Stats)
				{
					return;
				}
				try
				{
					EventRecord.New("item_skinned", true).AddField("player", player).AddField("entity", repairBench).AddField("item", item).AddField("new_skin", workshopId).Submit();
				}
				catch (Exception exception)
				{
					UnityEngine.Debug.LogException(exception);
				}
			}

			// Token: 0x06005527 RID: 21799 RVA: 0x001B8E8C File Offset: 0x001B708C
			public static void OnItemRepaired(BasePlayer player, BaseEntity repairBench, Item itemToRepair, float conditionBefore, float maxConditionBefore)
			{
				if (!Analytics.Azure.Stats)
				{
					return;
				}
				try
				{
					EventRecord.New("item_repair", true).AddField("player", player).AddField("entity", repairBench).AddField("item", itemToRepair).AddField("old_condition", conditionBefore).AddField("old_max_condition", maxConditionBefore).AddField("max_condition", itemToRepair.maxConditionNormalized).Submit();
				}
				catch (Exception exception)
				{
					UnityEngine.Debug.LogException(exception);
				}
			}

			// Token: 0x06005528 RID: 21800 RVA: 0x001B8F14 File Offset: 0x001B7114
			public static void OnEntityRepaired(BasePlayer player, BaseEntity entity, float healthBefore, float healthAfter)
			{
				if (!Analytics.Azure.Stats)
				{
					return;
				}
				try
				{
					EventRecord.New("entity_repair", true).AddField("player", player).AddField("entity", entity).AddField("healing", healthAfter - healthBefore).AddField("health_before", healthBefore).AddField("health_after", healthAfter).Submit();
				}
				catch (Exception exception)
				{
					UnityEngine.Debug.LogException(exception);
				}
			}

			// Token: 0x06005529 RID: 21801 RVA: 0x001B8F8C File Offset: 0x001B718C
			public static void OnBuildingBlockUpgraded(BasePlayer player, BuildingBlock buildingBlock, BuildingGrade.Enum targetGrade)
			{
				if (!Analytics.Azure.Stats)
				{
					return;
				}
				try
				{
					EventRecord.New("block_upgrade", true).AddField("player", player).AddField("entity", buildingBlock).AddField("old_grade", (int)buildingBlock.grade).AddField("new_grade", (int)targetGrade).Submit();
				}
				catch (Exception exception)
				{
					UnityEngine.Debug.LogException(exception);
				}
			}

			// Token: 0x0600552A RID: 21802 RVA: 0x001B8FFC File Offset: 0x001B71FC
			public static void OnBuildingBlockDemolished(BasePlayer player, BuildingBlock buildingBlock)
			{
				if (!Analytics.Azure.Stats)
				{
					return;
				}
				try
				{
					EventRecord.New("block_demolish", true).AddField("player", player).AddField("entity", buildingBlock).Submit();
				}
				catch (Exception exception)
				{
					UnityEngine.Debug.LogException(exception);
				}
			}

			// Token: 0x0600552B RID: 21803 RVA: 0x001B9054 File Offset: 0x001B7254
			public static void OnPlayerInitializedWipeId(ulong userId, string wipeId)
			{
				if (!Analytics.Azure.Stats)
				{
					return;
				}
				try
				{
					EventRecord.New("player_wipe_id_set", true).AddField("user_id", userId).AddField("player_wipe_id", wipeId).Submit();
				}
				catch (Exception exception)
				{
					UnityEngine.Debug.LogException(exception);
				}
			}

			// Token: 0x0600552C RID: 21804 RVA: 0x001B90AC File Offset: 0x001B72AC
			public static void OnFreeUnderwaterCrate(BasePlayer player, FreeableLootContainer freeableLootContainer)
			{
				if (!Analytics.Azure.Stats)
				{
					return;
				}
				try
				{
					EventRecord.New("crate_untied", true).AddField("player", player).AddField("entity", freeableLootContainer).Submit();
				}
				catch (Exception exception)
				{
					UnityEngine.Debug.LogException(exception);
				}
			}

			// Token: 0x0600552D RID: 21805 RVA: 0x001B9104 File Offset: 0x001B7304
			public static void OnVehiclePurchased(BasePlayer player, BaseEntity vehicle)
			{
				if (!Analytics.Azure.Stats)
				{
					return;
				}
				try
				{
					EventRecord.New("vehicle_purchase", true).AddField("player", player).AddField("entity", vehicle).AddField("price", vehicle).Submit();
				}
				catch (Exception exception)
				{
					UnityEngine.Debug.LogException(exception);
				}
			}

			// Token: 0x0600552E RID: 21806 RVA: 0x001B9164 File Offset: 0x001B7364
			public static void OnMissionComplete(BasePlayer player, BaseMission mission, BaseMission.MissionFailReason? failReason = null)
			{
				if (!Analytics.Azure.Stats)
				{
					return;
				}
				try
				{
					EventRecord eventRecord = EventRecord.New("mission_complete", true).AddField("player", player).AddField("mission", mission.shortname).AddField("mission_succeed", true);
					if (failReason != null)
					{
						eventRecord.AddField("mission_succeed", false).AddField("fail_reason", failReason.Value.ToString());
					}
					eventRecord.Submit();
				}
				catch (Exception exception)
				{
					UnityEngine.Debug.LogException(exception);
				}
			}

			// Token: 0x0600552F RID: 21807 RVA: 0x001B9200 File Offset: 0x001B7400
			public static void OnGamblingResult(BasePlayer player, BaseEntity entity, int scrapPaid, int scrapRecieved, Guid? gambleGroupId = null)
			{
				if (!Analytics.Azure.Stats)
				{
					return;
				}
				try
				{
					EventRecord eventRecord = EventRecord.New("gambing", true).AddField("player", player).AddField("entity", entity).AddField("scrap_input", scrapPaid).AddField("scrap_output", scrapRecieved);
					if (gambleGroupId != null)
					{
						eventRecord.AddField("gamble_grouping", gambleGroupId.Value);
					}
					eventRecord.Submit();
				}
				catch (Exception exception)
				{
					UnityEngine.Debug.LogException(exception);
				}
			}

			// Token: 0x06005530 RID: 21808 RVA: 0x001B928C File Offset: 0x001B748C
			public static void OnPlayerPinged(BasePlayer player, BasePlayer.PingType type, bool wasViaWheel)
			{
				if (!Analytics.Azure.Stats)
				{
					return;
				}
				try
				{
					EventRecord.New("player_pinged", true).AddField("player", player).AddField("pingType", (int)type).AddField("viaWheel", wasViaWheel).Submit();
				}
				catch (Exception exception)
				{
					UnityEngine.Debug.LogException(exception);
				}
			}

			// Token: 0x06005531 RID: 21809 RVA: 0x001B92EC File Offset: 0x001B74EC
			public static void OnBagUnclaimed(BasePlayer player, SleepingBag bag)
			{
				if (!Analytics.Azure.Stats)
				{
					return;
				}
				try
				{
					EventRecord.New("bag_unclaim", true).AddField("player", player).AddField("entity", bag).Submit();
				}
				catch (Exception exception)
				{
					UnityEngine.Debug.LogException(exception);
				}
			}

			// Token: 0x02000FDE RID: 4062
			private struct EntitySumItem
			{
				// Token: 0x04005122 RID: 20770
				public uint PrefabId;

				// Token: 0x04005123 RID: 20771
				public int Count;

				// Token: 0x04005124 RID: 20772
				public int Grade;
			}

			// Token: 0x02000FDF RID: 4063
			private struct EntityKey : IEquatable<Analytics.Azure.EntityKey>
			{
				// Token: 0x04005125 RID: 20773
				public uint PrefabId;

				// Token: 0x04005126 RID: 20774
				public int Grade;

				// Token: 0x060055AD RID: 21933 RVA: 0x001BAA27 File Offset: 0x001B8C27
				public bool Equals(Analytics.Azure.EntityKey other)
				{
					return this.PrefabId == other.PrefabId && this.Grade == other.Grade;
				}

				// Token: 0x060055AE RID: 21934 RVA: 0x001BAA47 File Offset: 0x001B8C47
				public override int GetHashCode()
				{
					return (17 * 23 + this.PrefabId.GetHashCode()) * 31 + this.Grade.GetHashCode();
				}
			}

			// Token: 0x02000FE0 RID: 4064
			private class PendingItemsData : Pool.IPooled
			{
				// Token: 0x04005127 RID: 20775
				public Analytics.Azure.PendingItemsKey Key;

				// Token: 0x04005128 RID: 20776
				public int amount;

				// Token: 0x04005129 RID: 20777
				public string category;

				// Token: 0x060055AF RID: 21935 RVA: 0x001BAA69 File Offset: 0x001B8C69
				public void EnterPool()
				{
					this.Key = default(Analytics.Azure.PendingItemsKey);
					this.amount = 0;
					this.category = null;
				}

				// Token: 0x060055B0 RID: 21936 RVA: 0x000063A5 File Offset: 0x000045A5
				public void LeavePool()
				{
				}
			}

			// Token: 0x02000FE1 RID: 4065
			private struct PendingItemsKey : IEquatable<Analytics.Azure.PendingItemsKey>
			{
				// Token: 0x0400512A RID: 20778
				public string Item;

				// Token: 0x0400512B RID: 20779
				public bool Consumed;

				// Token: 0x0400512C RID: 20780
				public string Entity;

				// Token: 0x0400512D RID: 20781
				public string Category;

				// Token: 0x0400512E RID: 20782
				public NetworkableId EntityId;

				// Token: 0x060055B2 RID: 21938 RVA: 0x001BAA88 File Offset: 0x001B8C88
				public bool Equals(Analytics.Azure.PendingItemsKey other)
				{
					return this.Item == other.Item && this.Entity == other.Entity && this.EntityId == other.EntityId && this.Consumed == other.Consumed && this.Category == other.Category;
				}

				// Token: 0x060055B3 RID: 21939 RVA: 0x001BAAF0 File Offset: 0x001B8CF0
				public override int GetHashCode()
				{
					return ((((17 * 23 + this.Item.GetHashCode()) * 31 + this.Consumed.GetHashCode()) * 37 + this.Entity.GetHashCode()) * 47 + this.Category.GetHashCode()) * 53 + this.EntityId.GetHashCode();
				}
			}

			// Token: 0x02000FE2 RID: 4066
			private struct PlayerPos
			{
				// Token: 0x0400512F RID: 20783
				public string UserId;

				// Token: 0x04005130 RID: 20784
				public Vector3 Position;

				// Token: 0x04005131 RID: 20785
				public Vector3 Direction;
			}

			// Token: 0x02000FE3 RID: 4067
			private class TeamInfo : Pool.IPooled
			{
				// Token: 0x04005132 RID: 20786
				public List<string> online = new List<string>();

				// Token: 0x04005133 RID: 20787
				public List<string> offline = new List<string>();

				// Token: 0x04005134 RID: 20788
				public int member_count;

				// Token: 0x060055B4 RID: 21940 RVA: 0x001BAB50 File Offset: 0x001B8D50
				public void EnterPool()
				{
					this.online.Clear();
					this.offline.Clear();
					this.member_count = 0;
				}

				// Token: 0x060055B5 RID: 21941 RVA: 0x000063A5 File Offset: 0x000045A5
				public void LeavePool()
				{
				}
			}

			// Token: 0x02000FE4 RID: 4068
			public enum ResourceMode
			{
				// Token: 0x04005136 RID: 20790
				Produced,
				// Token: 0x04005137 RID: 20791
				Consumed
			}

			// Token: 0x02000FE5 RID: 4069
			private static class EventIds
			{
				// Token: 0x04005138 RID: 20792
				public const string EntityBuilt = "entity_built";

				// Token: 0x04005139 RID: 20793
				public const string EntityPickup = "entity_pickup";

				// Token: 0x0400513A RID: 20794
				public const string EntityDamage = "entity_damage";

				// Token: 0x0400513B RID: 20795
				public const string PlayerRespawn = "player_respawn";

				// Token: 0x0400513C RID: 20796
				public const string ExplosiveLaunched = "explosive_launch";

				// Token: 0x0400513D RID: 20797
				public const string Explosion = "explosion";

				// Token: 0x0400513E RID: 20798
				public const string ItemEvent = "item_event";

				// Token: 0x0400513F RID: 20799
				public const string EntitySum = "entity_sum";

				// Token: 0x04005140 RID: 20800
				public const string ItemSum = "item_sum";

				// Token: 0x04005141 RID: 20801
				public const string ItemDespawn = "item_despawn";

				// Token: 0x04005142 RID: 20802
				public const string ItemDropped = "item_drop";

				// Token: 0x04005143 RID: 20803
				public const string ItemPickup = "item_pickup";

				// Token: 0x04005144 RID: 20804
				public const string AntihackViolation = "antihack_violation";

				// Token: 0x04005145 RID: 20805
				public const string AntihackViolationDetailed = "antihack_violation_detailed";

				// Token: 0x04005146 RID: 20806
				public const string PlayerConnect = "player_connect";

				// Token: 0x04005147 RID: 20807
				public const string PlayerDisconnect = "player_disconnect";

				// Token: 0x04005148 RID: 20808
				public const string ConsumableUsed = "consumeable_used";

				// Token: 0x04005149 RID: 20809
				public const string MedUsed = "med_used";

				// Token: 0x0400514A RID: 20810
				public const string ResearchStarted = "research_start";

				// Token: 0x0400514B RID: 20811
				public const string BlueprintLearned = "blueprint_learned";

				// Token: 0x0400514C RID: 20812
				public const string TeamChanged = "team_change";

				// Token: 0x0400514D RID: 20813
				public const string EntityAuthChange = "auth_change";

				// Token: 0x0400514E RID: 20814
				public const string VendingOrderChanged = "vending_changed";

				// Token: 0x0400514F RID: 20815
				public const string VendingSale = "vending_sale";

				// Token: 0x04005150 RID: 20816
				public const string ChatMessage = "chat";

				// Token: 0x04005151 RID: 20817
				public const string BlockUpgrade = "block_upgrade";

				// Token: 0x04005152 RID: 20818
				public const string BlockDemolish = "block_demolish";

				// Token: 0x04005153 RID: 20819
				public const string ItemRepair = "item_repair";

				// Token: 0x04005154 RID: 20820
				public const string EntityRepair = "entity_repair";

				// Token: 0x04005155 RID: 20821
				public const string ItemSkinned = "item_skinned";

				// Token: 0x04005156 RID: 20822
				public const string ItemAggregate = "item_aggregate";

				// Token: 0x04005157 RID: 20823
				public const string CodelockChanged = "code_change";

				// Token: 0x04005158 RID: 20824
				public const string CodelockEntered = "code_enter";

				// Token: 0x04005159 RID: 20825
				public const string SleepingBagAssign = "sleeping_bag_assign";

				// Token: 0x0400515A RID: 20826
				public const string FallDamage = "fall_damage";

				// Token: 0x0400515B RID: 20827
				public const string PlayerWipeIdSet = "player_wipe_id_set";

				// Token: 0x0400515C RID: 20828
				public const string ServerInfo = "server_info";

				// Token: 0x0400515D RID: 20829
				public const string UnderwaterCrateUntied = "crate_untied";

				// Token: 0x0400515E RID: 20830
				public const string VehiclePurchased = "vehicle_purchase";

				// Token: 0x0400515F RID: 20831
				public const string NPCVendor = "npc_vendor";

				// Token: 0x04005160 RID: 20832
				public const string BlueprintsOnline = "blueprint_aggregate_online";

				// Token: 0x04005161 RID: 20833
				public const string PlayerPositions = "player_positions";

				// Token: 0x04005162 RID: 20834
				public const string ProjectileInvalid = "projectile_invalid";

				// Token: 0x04005163 RID: 20835
				public const string ItemDefinitions = "item_definitions";

				// Token: 0x04005164 RID: 20836
				public const string KeycardSwiped = "keycard_swiped";

				// Token: 0x04005165 RID: 20837
				public const string EntitySpawned = "entity_spawned";

				// Token: 0x04005166 RID: 20838
				public const string EntityKilled = "entity_killed";

				// Token: 0x04005167 RID: 20839
				public const string HackableCrateStarted = "hackable_crate_started";

				// Token: 0x04005168 RID: 20840
				public const string HackableCrateEnded = "hackable_crate_ended";

				// Token: 0x04005169 RID: 20841
				public const string StashHidden = "stash_hidden";

				// Token: 0x0400516A RID: 20842
				public const string StashRevealed = "stash_reveal";

				// Token: 0x0400516B RID: 20843
				public const string EntityManifest = "entity_manifest";

				// Token: 0x0400516C RID: 20844
				public const string LootEntity = "loot_entity";

				// Token: 0x0400516D RID: 20845
				public const string OnlineTeams = "online_teams";

				// Token: 0x0400516E RID: 20846
				public const string Gambling = "gambing";

				// Token: 0x0400516F RID: 20847
				public const string MissionComplete = "mission_complete";

				// Token: 0x04005170 RID: 20848
				public const string PlayerPinged = "player_pinged";

				// Token: 0x04005171 RID: 20849
				public const string BagUnclaim = "bag_unclaim";
			}

			// Token: 0x02000FE6 RID: 4070
			private struct SimpleItemAmount
			{
				// Token: 0x04005172 RID: 20850
				public string ItemName;

				// Token: 0x04005173 RID: 20851
				public int Amount;

				// Token: 0x04005174 RID: 20852
				public ulong Skin;

				// Token: 0x04005175 RID: 20853
				public float Condition;

				// Token: 0x060055B7 RID: 21943 RVA: 0x001BAB8D File Offset: 0x001B8D8D
				public SimpleItemAmount(Item item)
				{
					this.ItemName = item.info.shortname;
					this.Amount = item.amount;
					this.Skin = item.skin;
					this.Condition = item.conditionNormalized;
				}
			}

			// Token: 0x02000FE7 RID: 4071
			private struct FiredProjectileKey : IEquatable<Analytics.Azure.FiredProjectileKey>
			{
				// Token: 0x04005176 RID: 20854
				public ulong UserId;

				// Token: 0x04005177 RID: 20855
				public int ProjectileId;

				// Token: 0x060055B8 RID: 21944 RVA: 0x001BABC4 File Offset: 0x001B8DC4
				public FiredProjectileKey(ulong userId, int projectileId)
				{
					this.UserId = userId;
					this.ProjectileId = projectileId;
				}

				// Token: 0x060055B9 RID: 21945 RVA: 0x001BABD4 File Offset: 0x001B8DD4
				public bool Equals(Analytics.Azure.FiredProjectileKey other)
				{
					return other.UserId == this.UserId && other.ProjectileId == this.ProjectileId;
				}
			}

			// Token: 0x02000FE8 RID: 4072
			private class PendingFiredProjectile : Pool.IPooled
			{
				// Token: 0x04005178 RID: 20856
				public EventRecord Record;

				// Token: 0x04005179 RID: 20857
				public BasePlayer.FiredProjectile FiredProjectile;

				// Token: 0x0400517A RID: 20858
				public bool Hit;

				// Token: 0x060055BA RID: 21946 RVA: 0x001BABF4 File Offset: 0x001B8DF4
				public void EnterPool()
				{
					this.Hit = false;
					this.Record = null;
					this.FiredProjectile = null;
				}

				// Token: 0x060055BB RID: 21947 RVA: 0x000063A5 File Offset: 0x000045A5
				public void LeavePool()
				{
				}
			}
		}

		// Token: 0x02000F85 RID: 3973
		public class AzureWebInterface
		{
			// Token: 0x04005011 RID: 20497
			public static readonly Analytics.AzureWebInterface client = new Analytics.AzureWebInterface(true);

			// Token: 0x04005012 RID: 20498
			public static readonly Analytics.AzureWebInterface server = new Analytics.AzureWebInterface(false);

			// Token: 0x04005013 RID: 20499
			public bool IsClient;

			// Token: 0x04005014 RID: 20500
			public int MaxRetries = 1;

			// Token: 0x04005015 RID: 20501
			public int FlushSize = 1000;

			// Token: 0x04005016 RID: 20502
			public TimeSpan FlushDelay = TimeSpan.FromSeconds(30.0);

			// Token: 0x04005017 RID: 20503
			private DateTime nextFlush;

			// Token: 0x04005018 RID: 20504
			private List<EventRecord> pending = new List<EventRecord>();

			// Token: 0x04005019 RID: 20505
			private HttpClient HttpClient = new HttpClient();

			// Token: 0x0400501A RID: 20506
			private static readonly MediaTypeHeaderValue JsonContentType = new MediaTypeHeaderValue("application/json")
			{
				CharSet = Encoding.UTF8.WebName
			};

			// Token: 0x17000739 RID: 1849
			// (get) Token: 0x06005533 RID: 21811 RVA: 0x001B936A File Offset: 0x001B756A
			public int PendingCount
			{
				get
				{
					return this.pending.Count;
				}
			}

			// Token: 0x06005534 RID: 21812 RVA: 0x001B9378 File Offset: 0x001B7578
			public AzureWebInterface(bool isClient)
			{
				this.IsClient = isClient;
			}

			// Token: 0x06005535 RID: 21813 RVA: 0x001B93D0 File Offset: 0x001B75D0
			public void EnqueueEvent(EventRecord point)
			{
				DateTime utcNow = DateTime.UtcNow;
				this.pending.Add(point);
				if (this.pending.Count > this.FlushSize || utcNow > this.nextFlush)
				{
					Analytics.AzureWebInterface.<>c__DisplayClass13_0 CS$<>8__locals1 = new Analytics.AzureWebInterface.<>c__DisplayClass13_0();
					CS$<>8__locals1.<>4__this = this;
					this.nextFlush = utcNow.Add(this.FlushDelay);
					CS$<>8__locals1.toUpload = this.pending;
					Task.Run(delegate()
					{
						Analytics.AzureWebInterface.<>c__DisplayClass13_0.<<EnqueueEvent>b__0>d <<EnqueueEvent>b__0>d;
						<<EnqueueEvent>b__0>d.<>4__this = CS$<>8__locals1;
						<<EnqueueEvent>b__0>d.<>t__builder = AsyncTaskMethodBuilder.Create();
						<<EnqueueEvent>b__0>d.<>1__state = -1;
						AsyncTaskMethodBuilder <>t__builder = <<EnqueueEvent>b__0>d.<>t__builder;
						<>t__builder.Start<Analytics.AzureWebInterface.<>c__DisplayClass13_0.<<EnqueueEvent>b__0>d>(ref <<EnqueueEvent>b__0>d);
						return <<EnqueueEvent>b__0>d.<>t__builder.Task;
					});
					this.pending = Pool.GetList<EventRecord>();
				}
			}

			// Token: 0x06005536 RID: 21814 RVA: 0x001B9458 File Offset: 0x001B7658
			private void SerializeEvents(List<EventRecord> records, MemoryStream stream)
			{
				int num = 0;
				using (StreamWriter streamWriter = new StreamWriter(stream, Encoding.UTF8, 1024, true))
				{
					streamWriter.Write("[");
					foreach (EventRecord record in records)
					{
						this.SerializeEvent(record, streamWriter, num);
						num++;
					}
					streamWriter.Write("]");
					streamWriter.Flush();
				}
			}

			// Token: 0x06005537 RID: 21815 RVA: 0x001B94F4 File Offset: 0x001B76F4
			private void SerializeEvent(EventRecord record, StreamWriter writer, int index)
			{
				if (index > 0)
				{
					writer.Write(',');
				}
				writer.Write("{\"Timestamp\":\"");
				writer.Write(record.Timestamp.ToString("o"));
				writer.Write("\",\"Data\":{");
				bool flag = true;
				foreach (EventRecordField eventRecordField in record.Data)
				{
					if (flag)
					{
						flag = false;
					}
					else
					{
						writer.Write(',');
					}
					writer.Write("\"");
					writer.Write(eventRecordField.Key1);
					if (eventRecordField.Key2 != null)
					{
						writer.Write(eventRecordField.Key2);
					}
					writer.Write("\":");
					if (!eventRecordField.IsObject)
					{
						writer.Write('"');
					}
					if (eventRecordField.String != null)
					{
						if (eventRecordField.IsObject)
						{
							writer.Write(eventRecordField.String);
						}
						else
						{
							string @string = eventRecordField.String;
							int length = eventRecordField.String.Length;
							for (int i = 0; i < length; i++)
							{
								char c = @string[i];
								if (c == '\\' || c == '"')
								{
									writer.Write('\\');
									writer.Write(c);
								}
								else if (c == '\n')
								{
									writer.Write("\\n");
								}
								else if (c == '\r')
								{
									writer.Write("\\r");
								}
								else if (c == '\t')
								{
									writer.Write("\\t");
								}
								else
								{
									writer.Write(c);
								}
							}
						}
					}
					else if (eventRecordField.Float != null)
					{
						writer.Write(eventRecordField.Float.Value);
					}
					else if (eventRecordField.Number != null)
					{
						writer.Write(eventRecordField.Number.Value);
					}
					else if (eventRecordField.Guid != null)
					{
						writer.Write(eventRecordField.Guid.Value.ToString("N"));
					}
					else if (eventRecordField.Vector != null)
					{
						writer.Write('(');
						Vector3 value = eventRecordField.Vector.Value;
						writer.Write(value.x);
						writer.Write(',');
						writer.Write(value.y);
						writer.Write(',');
						writer.Write(value.z);
						writer.Write(')');
					}
					if (!eventRecordField.IsObject)
					{
						writer.Write("\"");
					}
				}
				writer.Write('}');
				writer.Write('}');
			}

			// Token: 0x06005538 RID: 21816 RVA: 0x001B9798 File Offset: 0x001B7998
			private async Task UploadAsync(List<EventRecord> records)
			{
				MemoryStream stream = Pool.Get<MemoryStream>();
				stream.Position = 0L;
				stream.SetLength(0L);
				try
				{
					this.SerializeEvents(records, stream);
					for (int attempt = 0; attempt < this.MaxRetries; attempt++)
					{
						try
						{
							using (ByteArrayContent content = new ByteArrayContent(stream.GetBuffer(), 0, (int)stream.Length))
							{
								content.Headers.ContentType = Analytics.AzureWebInterface.JsonContentType;
								if (!string.IsNullOrEmpty(Analytics.AnalyticsSecret))
								{
									content.Headers.Add(Analytics.AnalyticsHeader, Analytics.AnalyticsSecret);
								}
								else
								{
									content.Headers.Add(Analytics.AnalyticsHeader, Analytics.AnalyticsPublicKey);
								}
								if (!this.IsClient)
								{
									content.Headers.Add("X-SERVER-IP", Network.Net.sv.ip);
									content.Headers.Add("X-SERVER-PORT", Network.Net.sv.port.ToString());
								}
								(await this.HttpClient.PostAsync(this.IsClient ? Analytics.ClientAnalyticsUrl : Analytics.ServerAnalyticsUrl, content)).EnsureSuccessStatusCode();
								break;
							}
						}
						catch (Exception ex)
						{
							if (!(ex is HttpRequestException))
							{
								UnityEngine.Debug.LogException(ex);
							}
						}
					}
				}
				catch (Exception ex2)
				{
					if (this.IsClient)
					{
						UnityEngine.Debug.LogWarning(ex2.ToString());
					}
					else
					{
						UnityEngine.Debug.LogException(ex2);
					}
				}
				finally
				{
					List<EventRecord>.Enumerator enumerator = records.GetEnumerator();
					try
					{
						while (enumerator.MoveNext())
						{
							EventRecord eventRecord = enumerator.Current;
							Pool.Free<EventRecord>(ref eventRecord);
						}
					}
					finally
					{
						int num;
						if (num < 0)
						{
							((IDisposable)enumerator).Dispose();
						}
					}
					Pool.FreeList<EventRecord>(ref records);
					Pool.FreeMemoryStream(ref stream);
				}
			}
		}

		// Token: 0x02000F86 RID: 3974
		public static class Server
		{
			// Token: 0x0400501B RID: 20507
			public static bool Enabled;

			// Token: 0x0400501C RID: 20508
			private static Dictionary<string, float> bufferData;

			// Token: 0x0400501D RID: 20509
			private static TimeSince lastHeldItemEvent;

			// Token: 0x0400501E RID: 20510
			private static TimeSince lastAnalyticsSave;

			// Token: 0x0400501F RID: 20511
			private static DateTime backupDate;

			// Token: 0x1700073A RID: 1850
			// (get) Token: 0x0600553A RID: 21818 RVA: 0x001B981C File Offset: 0x001B7A1C
			private static bool WriteToFile
			{
				get
				{
					return ConVar.Server.statBackup;
				}
			}

			// Token: 0x1700073B RID: 1851
			// (get) Token: 0x0600553B RID: 21819 RVA: 0x001B9823 File Offset: 0x001B7A23
			private static bool CanSendAnalytics
			{
				get
				{
					return ConVar.Server.official && ConVar.Server.stats && Analytics.Server.Enabled;
				}
			}

			// Token: 0x0600553C RID: 21820 RVA: 0x001B983C File Offset: 0x001B7A3C
			internal static void Death(BaseEntity initiator, BaseEntity weaponPrefab, Vector3 worldPosition)
			{
				if (!Analytics.Server.CanSendAnalytics)
				{
					return;
				}
				if (initiator != null)
				{
					if (initiator is BasePlayer)
					{
						if (weaponPrefab != null)
						{
							Analytics.Server.Death(weaponPrefab.ShortPrefabName, worldPosition, initiator.IsNpc ? Analytics.Server.DeathType.NPC : Analytics.Server.DeathType.Player);
							return;
						}
						Analytics.Server.Death("player", worldPosition, Analytics.Server.DeathType.Player);
						return;
					}
					else if (initiator is AutoTurret)
					{
						if (weaponPrefab != null)
						{
							Analytics.Server.Death(weaponPrefab.ShortPrefabName, worldPosition, Analytics.Server.DeathType.AutoTurret);
							return;
						}
					}
					else
					{
						Analytics.Server.Death(initiator.Categorize(), worldPosition, initiator.IsNpc ? Analytics.Server.DeathType.NPC : Analytics.Server.DeathType.Player);
					}
				}
			}

			// Token: 0x0600553D RID: 21821 RVA: 0x001B98C8 File Offset: 0x001B7AC8
			internal static void Death(string v, Vector3 worldPosition, Analytics.Server.DeathType deathType = Analytics.Server.DeathType.Player)
			{
				if (!Analytics.Server.CanSendAnalytics)
				{
					return;
				}
				string monumentStringFromPosition = Analytics.Server.GetMonumentStringFromPosition(worldPosition);
				if (!string.IsNullOrEmpty(monumentStringFromPosition))
				{
					switch (deathType)
					{
					case Analytics.Server.DeathType.Player:
						Analytics.Server.DesignEvent("player:" + monumentStringFromPosition + "death:" + v, false);
						return;
					case Analytics.Server.DeathType.NPC:
						Analytics.Server.DesignEvent("player:" + monumentStringFromPosition + "death:npc:" + v, false);
						return;
					case Analytics.Server.DeathType.AutoTurret:
						Analytics.Server.DesignEvent("player:" + monumentStringFromPosition + "death:autoturret:" + v, false);
						return;
					default:
						return;
					}
				}
				else
				{
					switch (deathType)
					{
					case Analytics.Server.DeathType.Player:
						Analytics.Server.DesignEvent("player:death:" + v, false);
						return;
					case Analytics.Server.DeathType.NPC:
						Analytics.Server.DesignEvent("player:death:npc:" + v, false);
						return;
					case Analytics.Server.DeathType.AutoTurret:
						Analytics.Server.DesignEvent("player:death:autoturret:" + v, false);
						return;
					default:
						return;
					}
				}
			}

			// Token: 0x0600553E RID: 21822 RVA: 0x001B9990 File Offset: 0x001B7B90
			private static string GetMonumentStringFromPosition(Vector3 worldPosition)
			{
				MonumentInfo monumentInfo = TerrainMeta.Path.FindMonumentWithBoundsOverlap(worldPosition);
				if (monumentInfo != null && !string.IsNullOrEmpty(monumentInfo.displayPhrase.token))
				{
					return monumentInfo.displayPhrase.token;
				}
				if (SingletonComponent<EnvironmentManager>.Instance != null && (EnvironmentManager.Get(worldPosition) & EnvironmentType.TrainTunnels) == EnvironmentType.TrainTunnels)
				{
					return "train_tunnel_display_name";
				}
				return string.Empty;
			}

			// Token: 0x0600553F RID: 21823 RVA: 0x001B99F5 File Offset: 0x001B7BF5
			public static void Crafting(string targetItemShortname, int skinId)
			{
				if (!Analytics.Server.CanSendAnalytics)
				{
					return;
				}
				Analytics.Server.DesignEvent("player:craft:" + targetItemShortname, false);
				Analytics.Server.SkinUsed(targetItemShortname, skinId);
			}

			// Token: 0x06005540 RID: 21824 RVA: 0x001B9A17 File Offset: 0x001B7C17
			public static void SkinUsed(string itemShortName, int skinId)
			{
				if (!Analytics.Server.CanSendAnalytics)
				{
					return;
				}
				if (skinId == 0)
				{
					return;
				}
				Analytics.Server.DesignEvent(string.Format("skinUsed:{0}:{1}", itemShortName, skinId), false);
			}

			// Token: 0x06005541 RID: 21825 RVA: 0x001B9A3C File Offset: 0x001B7C3C
			public static void ExcavatorStarted()
			{
				if (!Analytics.Server.CanSendAnalytics)
				{
					return;
				}
				Analytics.Server.DesignEvent("monuments:excavatorstarted", false);
			}

			// Token: 0x06005542 RID: 21826 RVA: 0x001B9A51 File Offset: 0x001B7C51
			public static void ExcavatorStopped(float activeDuration)
			{
				if (!Analytics.Server.CanSendAnalytics)
				{
					return;
				}
				Analytics.Server.DesignEvent("monuments:excavatorstopped", activeDuration, false);
			}

			// Token: 0x06005543 RID: 21827 RVA: 0x001B9A67 File Offset: 0x001B7C67
			public static void SlotMachineTransaction(int scrapSpent, int scrapReceived)
			{
				if (!Analytics.Server.CanSendAnalytics)
				{
					return;
				}
				Analytics.Server.DesignEvent("slots:scrapSpent", scrapSpent, false);
				Analytics.Server.DesignEvent("slots:scrapReceived", scrapReceived, false);
			}

			// Token: 0x06005544 RID: 21828 RVA: 0x001B9A89 File Offset: 0x001B7C89
			public static void VehiclePurchased(string vehicleType)
			{
				if (!Analytics.Server.CanSendAnalytics)
				{
					return;
				}
				Analytics.Server.DesignEvent("vehiclePurchased:" + vehicleType, false);
			}

			// Token: 0x06005545 RID: 21829 RVA: 0x001B9AA4 File Offset: 0x001B7CA4
			public static void FishCaught(ItemDefinition fish)
			{
				if (!Analytics.Server.CanSendAnalytics)
				{
					return;
				}
				if (fish == null)
				{
					return;
				}
				Analytics.Server.DesignEvent("fishCaught:" + fish.shortname, false);
			}

			// Token: 0x06005546 RID: 21830 RVA: 0x001B9AD0 File Offset: 0x001B7CD0
			public static void VendingMachineTransaction(NPCVendingOrder npcVendingOrder, ItemDefinition purchased, int amount)
			{
				if (!Analytics.Server.CanSendAnalytics)
				{
					return;
				}
				if (purchased == null)
				{
					return;
				}
				if (npcVendingOrder == null)
				{
					Analytics.Server.DesignEvent("vendingPurchase:player:" + purchased.shortname, amount, false);
					return;
				}
				Analytics.Server.DesignEvent("vendingPurchase:static:" + purchased.shortname, amount, false);
			}

			// Token: 0x06005547 RID: 21831 RVA: 0x001B9B27 File Offset: 0x001B7D27
			public static void Consume(string consumedItem)
			{
				if (!Analytics.Server.CanSendAnalytics)
				{
					return;
				}
				if (string.IsNullOrEmpty(consumedItem))
				{
					return;
				}
				Analytics.Server.DesignEvent("player:consume:" + consumedItem, false);
			}

			// Token: 0x06005548 RID: 21832 RVA: 0x001B9B4B File Offset: 0x001B7D4B
			public static void TreeKilled(BaseEntity withWeapon)
			{
				if (!Analytics.Server.CanSendAnalytics)
				{
					return;
				}
				if (withWeapon != null)
				{
					Analytics.Server.DesignEvent("treekilled:" + withWeapon.ShortPrefabName, false);
					return;
				}
				Analytics.Server.DesignEvent("treekilled", false);
			}

			// Token: 0x06005549 RID: 21833 RVA: 0x001B9B80 File Offset: 0x001B7D80
			public static void OreKilled(OreResourceEntity entity, HitInfo info)
			{
				if (!Analytics.Server.CanSendAnalytics)
				{
					return;
				}
				ResourceDispenser resourceDispenser;
				if (entity.TryGetComponent<ResourceDispenser>(out resourceDispenser) && resourceDispenser.containedItems.Count > 0 && resourceDispenser.containedItems[0].itemDef != null)
				{
					if (info.WeaponPrefab != null)
					{
						Analytics.Server.DesignEvent("orekilled:" + resourceDispenser.containedItems[0].itemDef.shortname + ":" + info.WeaponPrefab.ShortPrefabName, false);
						return;
					}
					Analytics.Server.DesignEvent(string.Format("orekilled:{0}", resourceDispenser.containedItems[0]), false);
				}
			}

			// Token: 0x0600554A RID: 21834 RVA: 0x001B9C2A File Offset: 0x001B7E2A
			public static void MissionComplete(BaseMission mission)
			{
				if (!Analytics.Server.CanSendAnalytics)
				{
					return;
				}
				Analytics.Server.DesignEvent("missionComplete:" + mission.shortname, true);
			}

			// Token: 0x0600554B RID: 21835 RVA: 0x001B9C4A File Offset: 0x001B7E4A
			public static void MissionFailed(BaseMission mission, BaseMission.MissionFailReason reason)
			{
				if (!Analytics.Server.CanSendAnalytics)
				{
					return;
				}
				Analytics.Server.DesignEvent(string.Format("missionFailed:{0}:{1}", mission.shortname, reason), true);
			}

			// Token: 0x0600554C RID: 21836 RVA: 0x001B9C70 File Offset: 0x001B7E70
			public static void FreeUnderwaterCrate()
			{
				if (!Analytics.Server.CanSendAnalytics)
				{
					return;
				}
				Analytics.Server.DesignEvent("loot:freeUnderWaterCrate", false);
			}

			// Token: 0x0600554D RID: 21837 RVA: 0x001B9C85 File Offset: 0x001B7E85
			public static void HeldItemDeployed(ItemDefinition def)
			{
				if (!Analytics.Server.CanSendAnalytics || Analytics.Server.lastHeldItemEvent < 0.1f)
				{
					return;
				}
				Analytics.Server.lastHeldItemEvent = 0f;
				Analytics.Server.DesignEvent("heldItemDeployed:" + def.shortname, false);
			}

			// Token: 0x0600554E RID: 21838 RVA: 0x001B9CC5 File Offset: 0x001B7EC5
			public static void UsedZipline()
			{
				if (!Analytics.Server.CanSendAnalytics)
				{
					return;
				}
				Analytics.Server.DesignEvent("usedZipline", false);
			}

			// Token: 0x0600554F RID: 21839 RVA: 0x001B9CDA File Offset: 0x001B7EDA
			public static void ReportCandiesCollectedByPlayer(int count)
			{
				if (!Analytics.Server.Enabled)
				{
					return;
				}
				Analytics.Server.DesignEvent("halloween:candiesCollected", count, false);
			}

			// Token: 0x06005550 RID: 21840 RVA: 0x001B9CF0 File Offset: 0x001B7EF0
			public static void ReportPlayersParticipatedInHalloweenEvent(int count)
			{
				if (!Analytics.Server.Enabled)
				{
					return;
				}
				Analytics.Server.DesignEvent("halloween:playersParticipated", count, false);
			}

			// Token: 0x06005551 RID: 21841 RVA: 0x001B9D06 File Offset: 0x001B7F06
			public static void Trigger(string message)
			{
				if (!Analytics.Server.CanSendAnalytics || string.IsNullOrEmpty(message))
				{
					return;
				}
				Analytics.Server.DesignEvent(message, false);
			}

			// Token: 0x06005552 RID: 21842 RVA: 0x001B9D1F File Offset: 0x001B7F1F
			private static void DesignEvent(string message, bool canBackup = false)
			{
				if (!Analytics.Server.CanSendAnalytics || string.IsNullOrEmpty(message))
				{
					return;
				}
				GA.DesignEvent(message);
				if (canBackup)
				{
					Analytics.Server.LocalBackup(message, 1f);
				}
			}

			// Token: 0x06005553 RID: 21843 RVA: 0x001B9D45 File Offset: 0x001B7F45
			private static void DesignEvent(string message, float value, bool canBackup = false)
			{
				if (!Analytics.Server.CanSendAnalytics || string.IsNullOrEmpty(message))
				{
					return;
				}
				GA.DesignEvent(message, value);
				if (canBackup)
				{
					Analytics.Server.LocalBackup(message, value);
				}
			}

			// Token: 0x06005554 RID: 21844 RVA: 0x001B9D68 File Offset: 0x001B7F68
			private static void DesignEvent(string message, int value, bool canBackup = false)
			{
				if (!Analytics.Server.CanSendAnalytics || string.IsNullOrEmpty(message))
				{
					return;
				}
				GA.DesignEvent(message, (float)value);
				if (canBackup)
				{
					Analytics.Server.LocalBackup(message, (float)value);
				}
			}

			// Token: 0x06005555 RID: 21845 RVA: 0x001B9D90 File Offset: 0x001B7F90
			private static string GetBackupPath(DateTime date)
			{
				return string.Format("{0}/{1}_{2}_{3}_analytics_backup.txt", new object[]
				{
					ConVar.Server.GetServerFolder("analytics"),
					date.Day,
					date.Month,
					date.Year
				});
			}

			// Token: 0x1700073C RID: 1852
			// (get) Token: 0x06005556 RID: 21846 RVA: 0x001B9DE7 File Offset: 0x001B7FE7
			private static DateTime currentDate
			{
				get
				{
					return DateTime.Now;
				}
			}

			// Token: 0x06005557 RID: 21847 RVA: 0x001B9DF0 File Offset: 0x001B7FF0
			private static void LocalBackup(string message, float value)
			{
				if (!Analytics.Server.WriteToFile)
				{
					return;
				}
				if (Analytics.Server.bufferData != null && Analytics.Server.backupDate.Date != Analytics.Server.currentDate.Date)
				{
					Analytics.Server.<LocalBackup>g__SaveBufferIntoDateFile|38_1(Analytics.Server.backupDate);
					Analytics.Server.bufferData.Clear();
					Analytics.Server.backupDate = Analytics.Server.currentDate;
				}
				if (Analytics.Server.bufferData == null)
				{
					if (Analytics.Server.bufferData == null)
					{
						Analytics.Server.bufferData = new Dictionary<string, float>();
					}
					Analytics.Server.lastAnalyticsSave = 0f;
					Analytics.Server.backupDate = Analytics.Server.currentDate;
				}
				if (Analytics.Server.bufferData.ContainsKey(message))
				{
					Dictionary<string, float> dictionary = Analytics.Server.bufferData;
					dictionary[message] += value;
				}
				else
				{
					Analytics.Server.bufferData.Add(message, value);
				}
				if (Analytics.Server.lastAnalyticsSave > 120f)
				{
					Analytics.Server.lastAnalyticsSave = 0f;
					Analytics.Server.<LocalBackup>g__SaveBufferIntoDateFile|38_1(Analytics.Server.currentDate);
					Analytics.Server.bufferData.Clear();
				}
			}

			// Token: 0x06005559 RID: 21849 RVA: 0x001B9EE0 File Offset: 0x001B80E0
			[CompilerGenerated]
			internal static void <LocalBackup>g__MergeBuffers|38_0(Dictionary<string, float> target, Dictionary<string, float> destination)
			{
				foreach (KeyValuePair<string, float> keyValuePair in target)
				{
					if (destination.ContainsKey(keyValuePair.Key))
					{
						string key = keyValuePair.Key;
						destination[key] += keyValuePair.Value;
					}
					else
					{
						destination.Add(keyValuePair.Key, keyValuePair.Value);
					}
				}
			}

			// Token: 0x0600555A RID: 21850 RVA: 0x001B9F6C File Offset: 0x001B816C
			[CompilerGenerated]
			internal static void <LocalBackup>g__SaveBufferIntoDateFile|38_1(DateTime date)
			{
				string backupPath = Analytics.Server.GetBackupPath(date);
				if (File.Exists(backupPath))
				{
					Dictionary<string, float> dictionary = (Dictionary<string, float>)JsonConvert.DeserializeObject(File.ReadAllText(backupPath), typeof(Dictionary<string, float>));
					if (dictionary != null)
					{
						Analytics.Server.<LocalBackup>g__MergeBuffers|38_0(dictionary, Analytics.Server.bufferData);
					}
				}
				string contents = JsonConvert.SerializeObject(Analytics.Server.bufferData);
				File.WriteAllText(Analytics.Server.GetBackupPath(date), contents);
			}

			// Token: 0x02000FF2 RID: 4082
			public enum DeathType
			{
				// Token: 0x040051BA RID: 20922
				Player,
				// Token: 0x040051BB RID: 20923
				NPC,
				// Token: 0x040051BC RID: 20924
				AutoTurret
			}
		}
	}
}
