using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Epic.OnlineServices.Logging;
using Epic.OnlineServices.Reports;
using Facepunch.Extend;
using Network;
using UnityEngine;

namespace ConVar
{
	// Token: 0x02000AD9 RID: 2777
	[ConsoleSystem.Factory("server")]
	public class Server : ConsoleSystem
	{
		// Token: 0x04003B99 RID: 15257
		[ServerVar]
		public static string ip = "";

		// Token: 0x04003B9A RID: 15258
		[ServerVar]
		public static int port = 28015;

		// Token: 0x04003B9B RID: 15259
		[ServerVar]
		public static int queryport = 0;

		// Token: 0x04003B9C RID: 15260
		[ServerVar(ShowInAdminUI = true)]
		public static int maxplayers = 500;

		// Token: 0x04003B9D RID: 15261
		[ServerVar(ShowInAdminUI = true)]
		public static string hostname = "My Untitled Rust Server";

		// Token: 0x04003B9E RID: 15262
		[ServerVar]
		public static string identity = "my_server_identity";

		// Token: 0x04003B9F RID: 15263
		[ServerVar]
		public static string level = "Procedural Map";

		// Token: 0x04003BA0 RID: 15264
		[ServerVar]
		public static string levelurl = "";

		// Token: 0x04003BA1 RID: 15265
		[ServerVar]
		public static bool leveltransfer = true;

		// Token: 0x04003BA2 RID: 15266
		[ServerVar]
		public static int seed = 1337;

		// Token: 0x04003BA3 RID: 15267
		[ServerVar]
		public static int salt = 1;

		// Token: 0x04003BA4 RID: 15268
		[ServerVar]
		public static int worldsize = 4500;

		// Token: 0x04003BA5 RID: 15269
		[ServerVar]
		public static int saveinterval = 600;

		// Token: 0x04003BA6 RID: 15270
		[ServerVar]
		public static bool secure = true;

		// Token: 0x04003BA7 RID: 15271
		[ServerVar]
		public static int encryption = 2;

		// Token: 0x04003BA8 RID: 15272
		[ServerVar]
		public static string anticheatid = "xyza7891h6UjNfd0eb2HQGtaul0WhfvS";

		// Token: 0x04003BA9 RID: 15273
		[ServerVar]
		public static string anticheatkey = "OWUDFZmi9VNL/7VhGVSSmCWALKTltKw8ISepa0VXs60";

		// Token: 0x04003BAA RID: 15274
		[ServerVar]
		public static int tickrate = 10;

		// Token: 0x04003BAB RID: 15275
		[ServerVar]
		public static int entityrate = 16;

		// Token: 0x04003BAC RID: 15276
		[ServerVar]
		public static float schematime = 1800f;

		// Token: 0x04003BAD RID: 15277
		[ServerVar]
		public static float cycletime = 500f;

		// Token: 0x04003BAE RID: 15278
		[ServerVar]
		public static bool official = false;

		// Token: 0x04003BAF RID: 15279
		[ServerVar]
		public static bool stats = false;

		// Token: 0x04003BB0 RID: 15280
		[ServerVar]
		public static bool stability = true;

		// Token: 0x04003BB1 RID: 15281
		[ServerVar(ShowInAdminUI = true)]
		public static bool radiation = true;

		// Token: 0x04003BB2 RID: 15282
		[ServerVar]
		public static float itemdespawn = 300f;

		// Token: 0x04003BB3 RID: 15283
		[ServerVar]
		public static float itemdespawn_container_scale = 2f;

		// Token: 0x04003BB4 RID: 15284
		[ServerVar]
		public static float itemdespawn_quick = 30f;

		// Token: 0x04003BB5 RID: 15285
		[ServerVar]
		public static float corpsedespawn = 300f;

		// Token: 0x04003BB6 RID: 15286
		[ServerVar]
		public static float debrisdespawn = 30f;

		// Token: 0x04003BB7 RID: 15287
		[ServerVar]
		public static bool pve = false;

		// Token: 0x04003BB8 RID: 15288
		[ServerVar]
		public static bool cinematic = false;

		// Token: 0x04003BB9 RID: 15289
		[ServerVar(ShowInAdminUI = true)]
		public static string description = "No server description has been provided.";

		// Token: 0x04003BBA RID: 15290
		[ServerVar(ShowInAdminUI = true)]
		public static string url = "";

		// Token: 0x04003BBB RID: 15291
		[ServerVar]
		public static string branch = "";

		// Token: 0x04003BBC RID: 15292
		[ServerVar]
		public static int queriesPerSecond = 2000;

		// Token: 0x04003BBD RID: 15293
		[ServerVar]
		public static int ipQueriesPerMin = 30;

		// Token: 0x04003BBE RID: 15294
		[ServerVar]
		public static bool statBackup = false;

		// Token: 0x04003BBF RID: 15295
		[ServerVar(Saved = true, ShowInAdminUI = true)]
		public static string headerimage = "";

		// Token: 0x04003BC0 RID: 15296
		[ServerVar(Saved = true, ShowInAdminUI = true)]
		public static string logoimage = "";

		// Token: 0x04003BC1 RID: 15297
		[ServerVar(Saved = true, ShowInAdminUI = true)]
		public static int saveBackupCount = 2;

		// Token: 0x04003BC2 RID: 15298
		[ReplicatedVar(Saved = true, ShowInAdminUI = true)]
		public static string motd = "";

		// Token: 0x04003BC3 RID: 15299
		[ServerVar(Saved = true)]
		public static float meleedamage = 1f;

		// Token: 0x04003BC4 RID: 15300
		[ServerVar(Saved = true)]
		public static float arrowdamage = 1f;

		// Token: 0x04003BC5 RID: 15301
		[ServerVar(Saved = true)]
		public static float bulletdamage = 1f;

		// Token: 0x04003BC6 RID: 15302
		[ServerVar(Saved = true)]
		public static float bleedingdamage = 1f;

		// Token: 0x04003BC7 RID: 15303
		[ReplicatedVar(Saved = true)]
		public static float funWaterDamageThreshold = 0.8f;

		// Token: 0x04003BC8 RID: 15304
		[ReplicatedVar(Saved = true)]
		public static float funWaterWetnessGain = 0.05f;

		// Token: 0x04003BC9 RID: 15305
		[ServerVar(Saved = true)]
		public static float meleearmor = 1f;

		// Token: 0x04003BCA RID: 15306
		[ServerVar(Saved = true)]
		public static float arrowarmor = 1f;

		// Token: 0x04003BCB RID: 15307
		[ServerVar(Saved = true)]
		public static float bulletarmor = 1f;

		// Token: 0x04003BCC RID: 15308
		[ServerVar(Saved = true)]
		public static float bleedingarmor = 1f;

		// Token: 0x04003BCD RID: 15309
		[ServerVar]
		public static int updatebatch = 512;

		// Token: 0x04003BCE RID: 15310
		[ServerVar]
		public static int updatebatchspawn = 1024;

		// Token: 0x04003BCF RID: 15311
		[ServerVar]
		public static int entitybatchsize = 100;

		// Token: 0x04003BD0 RID: 15312
		[ServerVar]
		public static float entitybatchtime = 1f;

		// Token: 0x04003BD1 RID: 15313
		[ServerVar]
		public static float composterUpdateInterval = 300f;

		// Token: 0x04003BD2 RID: 15314
		[ReplicatedVar]
		public static float planttick = 60f;

		// Token: 0x04003BD3 RID: 15315
		[ServerVar]
		public static float planttickscale = 1f;

		// Token: 0x04003BD4 RID: 15316
		[ServerVar]
		public static bool useMinimumPlantCondition = true;

		// Token: 0x04003BD5 RID: 15317
		[ServerVar(Saved = true)]
		public static float nonPlanterDeathChancePerTick = 0.005f;

		// Token: 0x04003BD6 RID: 15318
		[ServerVar(Saved = true)]
		public static float ceilingLightGrowableRange = 3f;

		// Token: 0x04003BD7 RID: 15319
		[ServerVar(Saved = true)]
		public static float artificialTemperatureGrowableRange = 4f;

		// Token: 0x04003BD8 RID: 15320
		[ServerVar(Saved = true)]
		public static float ceilingLightHeightOffset = 3f;

		// Token: 0x04003BD9 RID: 15321
		[ServerVar(Saved = true)]
		public static float sprinklerRadius = 3f;

		// Token: 0x04003BDA RID: 15322
		[ServerVar(Saved = true)]
		public static float sprinklerEyeHeightOffset = 3f;

		// Token: 0x04003BDB RID: 15323
		[ServerVar(Saved = true)]
		public static float optimalPlanterQualitySaturation = 0.6f;

		// Token: 0x04003BDC RID: 15324
		[ServerVar]
		public static float metabolismtick = 1f;

		// Token: 0x04003BDD RID: 15325
		[ServerVar]
		public static float modifierTickRate = 1f;

		// Token: 0x04003BDE RID: 15326
		[ServerVar(Saved = true)]
		public static float rewounddelay = 60f;

		// Token: 0x04003BDF RID: 15327
		[ServerVar(Saved = true, Help = "Can players be wounded after recieving fatal damage")]
		public static bool woundingenabled = true;

		// Token: 0x04003BE0 RID: 15328
		[ServerVar(Saved = true, Help = "Do players go into the crawling wounded state")]
		public static bool crawlingenabled = true;

		// Token: 0x04003BE1 RID: 15329
		[ServerVar(Help = "Base chance of recovery after crawling wounded state", Saved = true)]
		public static float woundedrecoverchance = 0.2f;

		// Token: 0x04003BE2 RID: 15330
		[ServerVar(Help = "Base chance of recovery after incapacitated wounded state", Saved = true)]
		public static float incapacitatedrecoverchance = 0.1f;

		// Token: 0x04003BE3 RID: 15331
		[ServerVar(Help = "Maximum percent chance added to base wounded/incapacitated recovery chance, based on the player's food and water level", Saved = true)]
		public static float woundedmaxfoodandwaterbonus = 0.25f;

		// Token: 0x04003BE4 RID: 15332
		[ServerVar(Help = "Minimum initial health given when a player dies and moves to crawling wounded state", Saved = false)]
		public static int crawlingminimumhealth = 7;

		// Token: 0x04003BE5 RID: 15333
		[ServerVar(Help = "Maximum initial health given when a player dies and moves to crawling wounded state", Saved = false)]
		public static int crawlingmaximumhealth = 12;

		// Token: 0x04003BE6 RID: 15334
		[ServerVar(Saved = true)]
		public static bool playerserverfall = true;

		// Token: 0x04003BE7 RID: 15335
		[ServerVar]
		public static bool plantlightdetection = true;

		// Token: 0x04003BE8 RID: 15336
		[ServerVar]
		public static float respawnresetrange = 50f;

		// Token: 0x04003BE9 RID: 15337
		[ReplicatedVar]
		public static int max_sleeping_bags = 15;

		// Token: 0x04003BEA RID: 15338
		[ReplicatedVar]
		public static bool bag_quota_item_amount = true;

		// Token: 0x04003BEB RID: 15339
		[ServerVar]
		public static int maxunack = 4;

		// Token: 0x04003BEC RID: 15340
		[ServerVar]
		public static bool netcache = true;

		// Token: 0x04003BED RID: 15341
		[ServerVar]
		public static bool corpses = true;

		// Token: 0x04003BEE RID: 15342
		[ServerVar]
		public static bool events = true;

		// Token: 0x04003BEF RID: 15343
		[ServerVar]
		public static bool dropitems = true;

		// Token: 0x04003BF0 RID: 15344
		[ServerVar]
		public static int netcachesize = 0;

		// Token: 0x04003BF1 RID: 15345
		[ServerVar]
		public static int savecachesize = 0;

		// Token: 0x04003BF2 RID: 15346
		[ServerVar]
		public static int combatlogsize = 30;

		// Token: 0x04003BF3 RID: 15347
		[ServerVar]
		public static int combatlogdelay = 10;

		// Token: 0x04003BF4 RID: 15348
		[ServerVar]
		public static int authtimeout = 60;

		// Token: 0x04003BF5 RID: 15349
		[ServerVar]
		public static int playertimeout = 60;

		// Token: 0x04003BF6 RID: 15350
		[ServerVar(ShowInAdminUI = true)]
		public static int idlekick = 30;

		// Token: 0x04003BF7 RID: 15351
		[ServerVar]
		public static int idlekickmode = 1;

		// Token: 0x04003BF8 RID: 15352
		[ServerVar]
		public static int idlekickadmins = 0;

		// Token: 0x04003BF9 RID: 15353
		[ServerVar]
		public static string gamemode = "";

		// Token: 0x04003BFA RID: 15354
		[ServerVar(Help = "Comma-separated server browser tag values (see wiki)", Saved = true, ShowInAdminUI = true)]
		public static string tags = "";

		// Token: 0x04003BFB RID: 15355
		[ServerVar(Help = "Censors the Steam player list to make player tracking more difficult")]
		public static bool censorplayerlist = true;

		// Token: 0x04003BFC RID: 15356
		[ServerVar(Help = "HTTP API endpoint for centralized banning (see wiki)")]
		public static string bansServerEndpoint = "";

		// Token: 0x04003BFD RID: 15357
		[ServerVar(Help = "Failure mode for centralized banning, set to 1 to reject players from joining if it's down (see wiki)")]
		public static int bansServerFailureMode = 0;

		// Token: 0x04003BFE RID: 15358
		[ServerVar(Help = "Timeout (in seconds) for centralized banning web server requests")]
		public static int bansServerTimeout = 5;

		// Token: 0x04003BFF RID: 15359
		[ServerVar(Help = "HTTP API endpoint for receiving F7 reports", Saved = true)]
		public static string reportsServerEndpoint = "";

		// Token: 0x04003C00 RID: 15360
		[ServerVar(Help = "If set, this key will be included with any reports sent via reportsServerEndpoint (for validation)", Saved = true)]
		public static string reportsServerEndpointKey = "";

		// Token: 0x04003C01 RID: 15361
		[ServerVar(Help = "Should F7 reports from players be printed to console", Saved = true)]
		public static bool printReportsToConsole = false;

		// Token: 0x04003C02 RID: 15362
		[ServerVar(Help = "If a player presses the respawn button, respawn at their death location (for trailer filming)")]
		public static bool respawnAtDeathPosition = false;

		// Token: 0x04003C03 RID: 15363
		[ServerVar(Help = "When a player respawns give them the loadout assigned to client.RespawnLoadout (created with inventory.saveloadout)")]
		public static bool respawnWithLoadout = false;

		// Token: 0x04003C04 RID: 15364
		[ServerVar(Help = "When transferring water, should containers keep 1 water behind. Enabling this should help performance if water IO is causing performance loss", Saved = true)]
		public static bool waterContainersLeaveWaterBehind = false;

		// Token: 0x04003C05 RID: 15365
		[ServerVar(Help = "How often industrial conveyors attempt to move items (value is an interval measured in seconds). Setting to 0 will disable all movement", Saved = true, ShowInAdminUI = true)]
		public static float conveyorMoveFrequency = 5f;

		// Token: 0x04003C06 RID: 15366
		[ServerVar(Help = "How often industrial crafters attempt to craft items (value is an interval measured in seconds). Setting to 0 will disable all crafting", Saved = true, ShowInAdminUI = true)]
		public static float industrialCrafterFrequency = 5f;

		// Token: 0x04003C07 RID: 15367
		[ReplicatedVar(Help = "How much scrap is required to research default blueprints", Saved = true, ShowInAdminUI = true)]
		public static int defaultBlueprintResearchCost = 10;

		// Token: 0x04003C08 RID: 15368
		[ServerVar(Help = "Whether to check for illegal industrial pipes when changing building block states (roof bunkers)", Saved = true, ShowInAdminUI = true)]
		public static bool enforcePipeChecksOnBuildingBlockChanges = true;

		// Token: 0x04003C09 RID: 15369
		[ServerVar(Help = "How many stacks a single conveyor can move in a single tick", Saved = true, ShowInAdminUI = true)]
		public static int maxItemStacksMovedPerTickIndustrial = 12;

		// Token: 0x04003C0A RID: 15370
		[ServerVar(Help = "How long per frame to spend on industrial jobs", Saved = true, ShowInAdminUI = true)]
		public static float industrialFrameBudgetMs = 0.5f;

		// Token: 0x04003C0B RID: 15371
		[ReplicatedVar(Help = "How many markers each player can place", Saved = true, ShowInAdminUI = true)]
		public static int maximumMapMarkers = 5;

		// Token: 0x04003C0C RID: 15372
		[ServerVar(Help = "How many pings can be placed by each player", Saved = true, ShowInAdminUI = true)]
		public static int maximumPings = 5;

		// Token: 0x04003C0D RID: 15373
		[ServerVar(Help = "How long a ping should last", Saved = true, ShowInAdminUI = true)]
		public static float pingDuration = 10f;

		// Token: 0x04003C0E RID: 15374
		[ServerVar(Saved = true)]
		public static bool showHolsteredItems = true;

		// Token: 0x04003C0F RID: 15375
		[ServerVar]
		public static int maxpacketspersecond_world = 1;

		// Token: 0x04003C10 RID: 15376
		[ServerVar]
		public static int maxpacketspersecond_rpc = 200;

		// Token: 0x04003C11 RID: 15377
		[ServerVar]
		public static int maxpacketspersecond_rpc_signal = 50;

		// Token: 0x04003C12 RID: 15378
		[ServerVar]
		public static int maxpacketspersecond_command = 100;

		// Token: 0x04003C13 RID: 15379
		[ServerVar]
		public static int maxpacketsize_command = 100000;

		// Token: 0x04003C14 RID: 15380
		[ServerVar]
		public static int maxpacketspersecond_tick = 300;

		// Token: 0x04003C15 RID: 15381
		[ServerVar]
		public static int maxpacketspersecond_voice = 100;

		// Token: 0x04003C16 RID: 15382
		[ServerVar]
		public static bool packetlog_enabled = false;

		// Token: 0x04003C17 RID: 15383
		[ServerVar]
		public static bool rpclog_enabled = false;

		// Token: 0x170005C5 RID: 1477
		// (get) Token: 0x060042B8 RID: 17080 RVA: 0x0018B695 File Offset: 0x00189895
		// (set) Token: 0x060042B9 RID: 17081 RVA: 0x0018B69C File Offset: 0x0018989C
		[ServerVar]
		public static int anticheatlog
		{
			get
			{
				return (int)EOS.LogLevel;
			}
			set
			{
				EOS.LogLevel = (LogLevel)value;
			}
		}

		// Token: 0x060042BA RID: 17082 RVA: 0x0018B6A4 File Offset: 0x001898A4
		public static float TickDelta()
		{
			return 1f / (float)Server.tickrate;
		}

		// Token: 0x060042BB RID: 17083 RVA: 0x0018B6B2 File Offset: 0x001898B2
		public static float TickTime(uint tick)
		{
			return (float)((double)Server.TickDelta() * tick);
		}

		// Token: 0x060042BC RID: 17084 RVA: 0x0018B6C0 File Offset: 0x001898C0
		[ServerVar(Help = "Show holstered items on player bodies")]
		public static void setshowholstereditems(ConsoleSystem.Arg arg)
		{
			Server.showHolsteredItems = arg.GetBool(0, Server.showHolsteredItems);
			foreach (BasePlayer basePlayer in BasePlayer.activePlayerList)
			{
				basePlayer.inventory.UpdatedVisibleHolsteredItems();
			}
			foreach (BasePlayer basePlayer2 in BasePlayer.sleepingPlayerList)
			{
				basePlayer2.inventory.UpdatedVisibleHolsteredItems();
			}
		}

		// Token: 0x170005C6 RID: 1478
		// (get) Token: 0x060042BD RID: 17085 RVA: 0x0018B768 File Offset: 0x00189968
		// (set) Token: 0x060042BE RID: 17086 RVA: 0x0018B76F File Offset: 0x0018996F
		[ServerVar]
		public static int maxclientinfosize
		{
			get
			{
				return Connection.MaxClientInfoSize;
			}
			set
			{
				Connection.MaxClientInfoSize = Mathf.Max(value, 1);
			}
		}

		// Token: 0x170005C7 RID: 1479
		// (get) Token: 0x060042BF RID: 17087 RVA: 0x0018B77D File Offset: 0x0018997D
		// (set) Token: 0x060042C0 RID: 17088 RVA: 0x0018B784 File Offset: 0x00189984
		[ServerVar]
		public static int maxconnectionsperip
		{
			get
			{
				return Server.MaxConnectionsPerIP;
			}
			set
			{
				Server.MaxConnectionsPerIP = Mathf.Clamp(value, 1, 1000);
			}
		}

		// Token: 0x170005C8 RID: 1480
		// (get) Token: 0x060042C1 RID: 17089 RVA: 0x0018B797 File Offset: 0x00189997
		// (set) Token: 0x060042C2 RID: 17090 RVA: 0x0018B79E File Offset: 0x0018999E
		[ServerVar]
		public static int maxreceivetime
		{
			get
			{
				return Server.MaxReceiveTime;
			}
			set
			{
				Server.MaxReceiveTime = Mathf.Clamp(value, 10, 1000);
			}
		}

		// Token: 0x170005C9 RID: 1481
		// (get) Token: 0x060042C3 RID: 17091 RVA: 0x0018B7B2 File Offset: 0x001899B2
		// (set) Token: 0x060042C4 RID: 17092 RVA: 0x0018B7B9 File Offset: 0x001899B9
		[ServerVar]
		public static int maxmainthreadwait
		{
			get
			{
				return Server.MaxMainThreadWait;
			}
			set
			{
				Server.MaxMainThreadWait = Mathf.Clamp(value, 1, 1000);
			}
		}

		// Token: 0x170005CA RID: 1482
		// (get) Token: 0x060042C5 RID: 17093 RVA: 0x0018B7CC File Offset: 0x001899CC
		// (set) Token: 0x060042C6 RID: 17094 RVA: 0x0018B7D3 File Offset: 0x001899D3
		[ServerVar]
		public static int maxreadthreadwait
		{
			get
			{
				return Server.MaxReadThreadWait;
			}
			set
			{
				Server.MaxReadThreadWait = Mathf.Clamp(value, 1, 1000);
			}
		}

		// Token: 0x170005CB RID: 1483
		// (get) Token: 0x060042C7 RID: 17095 RVA: 0x0018B7E6 File Offset: 0x001899E6
		// (set) Token: 0x060042C8 RID: 17096 RVA: 0x0018B7ED File Offset: 0x001899ED
		[ServerVar]
		public static int maxwritethreadwait
		{
			get
			{
				return Server.MaxWriteThreadWait;
			}
			set
			{
				Server.MaxWriteThreadWait = Mathf.Clamp(value, 1, 1000);
			}
		}

		// Token: 0x170005CC RID: 1484
		// (get) Token: 0x060042C9 RID: 17097 RVA: 0x0018B800 File Offset: 0x00189A00
		// (set) Token: 0x060042CA RID: 17098 RVA: 0x0018B807 File Offset: 0x00189A07
		[ServerVar]
		public static int maxdecryptthreadwait
		{
			get
			{
				return Server.MaxDecryptThreadWait;
			}
			set
			{
				Server.MaxDecryptThreadWait = Mathf.Clamp(value, 1, 1000);
			}
		}

		// Token: 0x170005CD RID: 1485
		// (get) Token: 0x060042CB RID: 17099 RVA: 0x0018B81A File Offset: 0x00189A1A
		// (set) Token: 0x060042CC RID: 17100 RVA: 0x0018B821 File Offset: 0x00189A21
		[ServerVar]
		public static int maxreadqueuelength
		{
			get
			{
				return Server.MaxReadQueueLength;
			}
			set
			{
				Server.MaxReadQueueLength = Mathf.Max(value, 1);
			}
		}

		// Token: 0x170005CE RID: 1486
		// (get) Token: 0x060042CD RID: 17101 RVA: 0x0018B82F File Offset: 0x00189A2F
		// (set) Token: 0x060042CE RID: 17102 RVA: 0x0018B836 File Offset: 0x00189A36
		[ServerVar]
		public static int maxwritequeuelength
		{
			get
			{
				return Server.MaxWriteQueueLength;
			}
			set
			{
				Server.MaxWriteQueueLength = Mathf.Max(value, 1);
			}
		}

		// Token: 0x170005CF RID: 1487
		// (get) Token: 0x060042CF RID: 17103 RVA: 0x0018B844 File Offset: 0x00189A44
		// (set) Token: 0x060042D0 RID: 17104 RVA: 0x0018B84B File Offset: 0x00189A4B
		[ServerVar]
		public static int maxdecryptqueuelength
		{
			get
			{
				return Server.MaxDecryptQueueLength;
			}
			set
			{
				Server.MaxDecryptQueueLength = Mathf.Max(value, 1);
			}
		}

		// Token: 0x170005D0 RID: 1488
		// (get) Token: 0x060042D1 RID: 17105 RVA: 0x0018B859 File Offset: 0x00189A59
		// (set) Token: 0x060042D2 RID: 17106 RVA: 0x0018B860 File Offset: 0x00189A60
		[ServerVar]
		public static int maxreadqueuebytes
		{
			get
			{
				return Server.MaxReadQueueBytes;
			}
			set
			{
				Server.MaxReadQueueBytes = Mathf.Max(value, 1);
			}
		}

		// Token: 0x170005D1 RID: 1489
		// (get) Token: 0x060042D3 RID: 17107 RVA: 0x0018B86E File Offset: 0x00189A6E
		// (set) Token: 0x060042D4 RID: 17108 RVA: 0x0018B875 File Offset: 0x00189A75
		[ServerVar]
		public static int maxwritequeuebytes
		{
			get
			{
				return Server.MaxWriteQueueBytes;
			}
			set
			{
				Server.MaxWriteQueueBytes = Mathf.Max(value, 1);
			}
		}

		// Token: 0x170005D2 RID: 1490
		// (get) Token: 0x060042D5 RID: 17109 RVA: 0x0018B883 File Offset: 0x00189A83
		// (set) Token: 0x060042D6 RID: 17110 RVA: 0x0018B88A File Offset: 0x00189A8A
		[ServerVar]
		public static int maxdecryptqueuebytes
		{
			get
			{
				return Server.MaxDecryptQueueBytes;
			}
			set
			{
				Server.MaxDecryptQueueBytes = Mathf.Max(value, 1);
			}
		}

		// Token: 0x060042D7 RID: 17111 RVA: 0x0018B898 File Offset: 0x00189A98
		[ServerVar]
		public static string printreadqueue(ConsoleSystem.Arg arg)
		{
			return "Server read queue: " + Net.sv.ReadQueueLength.ToString() + " items / " + Net.sv.ReadQueueBytes.FormatBytes(false);
		}

		// Token: 0x060042D8 RID: 17112 RVA: 0x0018B8D8 File Offset: 0x00189AD8
		[ServerVar]
		public static string printwritequeue(ConsoleSystem.Arg arg)
		{
			return "Server write queue: " + Net.sv.WriteQueueLength.ToString() + " items / " + Net.sv.WriteQueueBytes.FormatBytes(false);
		}

		// Token: 0x060042D9 RID: 17113 RVA: 0x0018B918 File Offset: 0x00189B18
		[ServerVar]
		public static string printdecryptqueue(ConsoleSystem.Arg arg)
		{
			return "Server decrypt queue: " + Net.sv.DecryptQueueLength.ToString() + " items / " + Net.sv.DecryptQueueBytes.FormatBytes(false);
		}

		// Token: 0x170005D3 RID: 1491
		// (get) Token: 0x060042DA RID: 17114 RVA: 0x0018B956 File Offset: 0x00189B56
		// (set) Token: 0x060042DB RID: 17115 RVA: 0x0018B95E File Offset: 0x00189B5E
		[ServerVar]
		public static int maxpacketspersecond
		{
			get
			{
				return (int)Server.MaxPacketsPerSecond;
			}
			set
			{
				Server.MaxPacketsPerSecond = (ulong)((long)Mathf.Clamp(value, 1, 1000000));
			}
		}

		// Token: 0x060042DC RID: 17116 RVA: 0x0018B974 File Offset: 0x00189B74
		[ServerVar]
		public static string packetlog(ConsoleSystem.Arg arg)
		{
			if (!Server.packetlog_enabled)
			{
				return "Packet log is not enabled.";
			}
			List<Tuple<Message.Type, ulong>> list = new List<Tuple<Message.Type, ulong>>();
			foreach (KeyValuePair<Message.Type, TimeAverageValue> keyValuePair in SingletonComponent<ServerMgr>.Instance.packetHistory.dict)
			{
				list.Add(new Tuple<Message.Type, ulong>(keyValuePair.Key, keyValuePair.Value.Calculate()));
			}
			TextTable textTable = new TextTable();
			textTable.AddColumn("type");
			textTable.AddColumn("calls");
			foreach (Tuple<Message.Type, ulong> tuple in from entry in list
			orderby entry.Item2 descending
			select entry)
			{
				if (tuple.Item2 == 0UL)
				{
					break;
				}
				string text = tuple.Item1.ToString();
				string text2 = tuple.Item2.ToString();
				textTable.AddRow(new string[]
				{
					text,
					text2
				});
			}
			if (!arg.HasArg("--json"))
			{
				return textTable.ToString();
			}
			return textTable.ToJson();
		}

		// Token: 0x060042DD RID: 17117 RVA: 0x0018BAD8 File Offset: 0x00189CD8
		[ServerVar]
		public static string rpclog(ConsoleSystem.Arg arg)
		{
			if (!Server.rpclog_enabled)
			{
				return "RPC log is not enabled.";
			}
			List<Tuple<uint, ulong>> list = new List<Tuple<uint, ulong>>();
			foreach (KeyValuePair<uint, TimeAverageValue> keyValuePair in SingletonComponent<ServerMgr>.Instance.rpcHistory.dict)
			{
				list.Add(new Tuple<uint, ulong>(keyValuePair.Key, keyValuePair.Value.Calculate()));
			}
			TextTable textTable = new TextTable();
			textTable.AddColumn("id");
			textTable.AddColumn("name");
			textTable.AddColumn("calls");
			foreach (Tuple<uint, ulong> tuple in from entry in list
			orderby entry.Item2 descending
			select entry)
			{
				if (tuple.Item2 == 0UL)
				{
					break;
				}
				string text = tuple.Item1.ToString();
				string text2 = StringPool.Get(tuple.Item1);
				string text3 = tuple.Item2.ToString();
				textTable.AddRow(new string[]
				{
					text,
					text2,
					text3
				});
			}
			return textTable.ToString();
		}

		// Token: 0x060042DE RID: 17118 RVA: 0x0018BC40 File Offset: 0x00189E40
		[ServerVar(Help = "Starts a server")]
		public static void start(ConsoleSystem.Arg arg)
		{
			if (Net.sv.IsConnected())
			{
				arg.ReplyWith("There is already a server running!");
				return;
			}
			string @string = arg.GetString(0, Server.level);
			if (!LevelManager.IsValid(@string))
			{
				arg.ReplyWith("Level '" + @string + "' isn't valid!");
				return;
			}
			if (UnityEngine.Object.FindObjectOfType<ServerMgr>())
			{
				arg.ReplyWith("There is already a server running!");
				return;
			}
			UnityEngine.Object.DontDestroyOnLoad(GameManager.server.CreatePrefab("assets/bundled/prefabs/system/server.prefab", true));
			LevelManager.LoadLevel(@string, true);
		}

		// Token: 0x060042DF RID: 17119 RVA: 0x0018BCC5 File Offset: 0x00189EC5
		[ServerVar(Help = "Stops a server")]
		public static void stop(ConsoleSystem.Arg arg)
		{
			if (!Net.sv.IsConnected())
			{
				arg.ReplyWith("There isn't a server running!");
				return;
			}
			Net.sv.Stop(arg.GetString(0, "Stopping Server"));
		}

		// Token: 0x170005D4 RID: 1492
		// (get) Token: 0x060042E0 RID: 17120 RVA: 0x0018BCF5 File Offset: 0x00189EF5
		public static string rootFolder
		{
			get
			{
				return "server/" + Server.identity;
			}
		}

		// Token: 0x170005D5 RID: 1493
		// (get) Token: 0x060042E1 RID: 17121 RVA: 0x0018BD06 File Offset: 0x00189F06
		public static string backupFolder
		{
			get
			{
				return "backup/0/" + Server.identity;
			}
		}

		// Token: 0x170005D6 RID: 1494
		// (get) Token: 0x060042E2 RID: 17122 RVA: 0x0018BD17 File Offset: 0x00189F17
		public static string backupFolder1
		{
			get
			{
				return "backup/1/" + Server.identity;
			}
		}

		// Token: 0x170005D7 RID: 1495
		// (get) Token: 0x060042E3 RID: 17123 RVA: 0x0018BD28 File Offset: 0x00189F28
		public static string backupFolder2
		{
			get
			{
				return "backup/2/" + Server.identity;
			}
		}

		// Token: 0x170005D8 RID: 1496
		// (get) Token: 0x060042E4 RID: 17124 RVA: 0x0018BD39 File Offset: 0x00189F39
		public static string backupFolder3
		{
			get
			{
				return "backup/3/" + Server.identity;
			}
		}

		// Token: 0x060042E5 RID: 17125 RVA: 0x0018BD4A File Offset: 0x00189F4A
		[ServerVar(Help = "Backup server folder")]
		public static void backup()
		{
			DirectoryEx.Backup(new string[]
			{
				Server.backupFolder,
				Server.backupFolder1,
				Server.backupFolder2,
				Server.backupFolder3
			});
			DirectoryEx.CopyAll(Server.rootFolder, Server.backupFolder);
		}

		// Token: 0x060042E6 RID: 17126 RVA: 0x0018BD88 File Offset: 0x00189F88
		public static string GetServerFolder(string folder)
		{
			string text = Server.rootFolder + "/" + folder;
			if (Directory.Exists(text))
			{
				return text;
			}
			Directory.CreateDirectory(text);
			return text;
		}

		// Token: 0x060042E7 RID: 17127 RVA: 0x0018BDB8 File Offset: 0x00189FB8
		[ServerVar(Help = "Writes config files")]
		public static void writecfg(ConsoleSystem.Arg arg)
		{
			string contents = ConsoleSystem.SaveToConfigString(true);
			File.WriteAllText(Server.GetServerFolder("cfg") + "/serverauto.cfg", contents);
			ServerUsers.Save();
			arg.ReplyWith("Config Saved");
		}

		// Token: 0x060042E8 RID: 17128 RVA: 0x0018BDF6 File Offset: 0x00189FF6
		[ServerVar]
		public static void fps(ConsoleSystem.Arg arg)
		{
			arg.ReplyWith(Performance.report.frameRate.ToString() + " FPS");
		}

		// Token: 0x060042E9 RID: 17129 RVA: 0x0018BE18 File Offset: 0x0018A018
		[ServerVar(Help = "Force save the current game")]
		public static void save(ConsoleSystem.Arg arg)
		{
			Stopwatch stopwatch = Stopwatch.StartNew();
			foreach (BaseEntity baseEntity in BaseEntity.saveList)
			{
				baseEntity.InvalidateNetworkCache();
			}
			UnityEngine.Debug.Log("Invalidate Network Cache took " + stopwatch.Elapsed.TotalSeconds.ToString("0.00") + " seconds");
			SaveRestore.Save(true);
		}

		// Token: 0x060042EA RID: 17130 RVA: 0x0018BEA4 File Offset: 0x0018A0A4
		[ServerVar]
		public static string readcfg(ConsoleSystem.Arg arg)
		{
			string serverFolder = Server.GetServerFolder("cfg");
			if (File.Exists(serverFolder + "/serverauto.cfg"))
			{
				string strFile = File.ReadAllText(serverFolder + "/serverauto.cfg");
				ConsoleSystem.RunFile(ConsoleSystem.Option.Server.Quiet(), strFile);
			}
			if (File.Exists(serverFolder + "/server.cfg"))
			{
				string strFile2 = File.ReadAllText(serverFolder + "/server.cfg");
				ConsoleSystem.RunFile(ConsoleSystem.Option.Server.Quiet(), strFile2);
			}
			return "Server Config Loaded";
		}

		// Token: 0x170005D9 RID: 1497
		// (get) Token: 0x060042EB RID: 17131 RVA: 0x0018BF2D File Offset: 0x0018A12D
		// (set) Token: 0x060042EC RID: 17132 RVA: 0x0018BF42 File Offset: 0x0018A142
		[ServerVar]
		public static bool compression
		{
			get
			{
				return Net.sv != null && Net.sv.compressionEnabled;
			}
			set
			{
				Net.sv.compressionEnabled = value;
			}
		}

		// Token: 0x170005DA RID: 1498
		// (get) Token: 0x060042ED RID: 17133 RVA: 0x0018BF4F File Offset: 0x0018A14F
		// (set) Token: 0x060042EE RID: 17134 RVA: 0x0018BF64 File Offset: 0x0018A164
		[ServerVar]
		public static bool netlog
		{
			get
			{
				return Net.sv != null && Net.sv.logging;
			}
			set
			{
				Net.sv.logging = value;
			}
		}

		// Token: 0x060042EF RID: 17135 RVA: 0x0018BF71 File Offset: 0x0018A171
		[ServerVar]
		public static string netprotocol(ConsoleSystem.Arg arg)
		{
			if (Net.sv == null)
			{
				return string.Empty;
			}
			return Net.sv.ProtocolId;
		}

		// Token: 0x060042F0 RID: 17136 RVA: 0x0018BF8C File Offset: 0x0018A18C
		[ServerUserVar]
		public static void cheatreport(ConsoleSystem.Arg arg)
		{
			BasePlayer basePlayer = arg.Player();
			if (basePlayer == null)
			{
				return;
			}
			string text = arg.GetUInt64(0, 0UL).ToString();
			string @string = arg.GetString(1, "");
			UnityEngine.Debug.LogWarning(string.Concat(new object[]
			{
				basePlayer,
				" reported ",
				text,
				": ",
				@string.ToPrintable(140)
			}));
			EACServer.SendPlayerBehaviorReport(basePlayer, PlayerReportsCategory.Cheating, text, @string);
		}

		// Token: 0x060042F1 RID: 17137 RVA: 0x0018C008 File Offset: 0x0018A208
		[ServerAllVar(Help = "Get the player combat log")]
		public static string combatlog(ConsoleSystem.Arg arg)
		{
			BasePlayer basePlayer = arg.Player();
			if (arg.HasArgs(1) && arg.IsAdmin)
			{
				basePlayer = arg.GetPlayerOrSleeper(0);
			}
			if (basePlayer == null || basePlayer.net == null)
			{
				return "invalid player";
			}
			CombatLog combat = basePlayer.stats.combat;
			int count = Server.combatlogsize;
			NetworkableId filterByAttacker = default(NetworkableId);
			bool json = arg.HasArg("--json");
			bool isAdmin = arg.IsAdmin;
			Connection connection = arg.Connection;
			return combat.Get(count, filterByAttacker, json, isAdmin, (connection != null) ? connection.userid : 0UL);
		}

		// Token: 0x060042F2 RID: 17138 RVA: 0x0018C090 File Offset: 0x0018A290
		[ServerAllVar(Help = "Get the player combat log, only showing outgoing damage")]
		public static string combatlog_outgoing(ConsoleSystem.Arg arg)
		{
			BasePlayer basePlayer = arg.Player();
			if (arg.HasArgs(1) && arg.IsAdmin)
			{
				basePlayer = arg.GetPlayerOrSleeper(0);
			}
			if (basePlayer == null)
			{
				return "invalid player";
			}
			CombatLog combat = basePlayer.stats.combat;
			int count = Server.combatlogsize;
			NetworkableId id = basePlayer.net.ID;
			bool json = arg.HasArg("--json");
			bool isAdmin = arg.IsAdmin;
			Connection connection = arg.Connection;
			return combat.Get(count, id, json, isAdmin, (connection != null) ? connection.userid : 0UL);
		}

		// Token: 0x060042F3 RID: 17139 RVA: 0x0018C110 File Offset: 0x0018A310
		[ServerVar(Help = "Print the current player position.")]
		public static string printpos(ConsoleSystem.Arg arg)
		{
			BasePlayer basePlayer = arg.Player();
			if (arg.HasArgs(1))
			{
				basePlayer = arg.GetPlayerOrSleeper(0);
			}
			if (!(basePlayer == null))
			{
				return basePlayer.transform.position.ToString();
			}
			return "invalid player";
		}

		// Token: 0x060042F4 RID: 17140 RVA: 0x0018C160 File Offset: 0x0018A360
		[ServerVar(Help = "Print the current player rotation.")]
		public static string printrot(ConsoleSystem.Arg arg)
		{
			BasePlayer basePlayer = arg.Player();
			if (arg.HasArgs(1))
			{
				basePlayer = arg.GetPlayerOrSleeper(0);
			}
			if (!(basePlayer == null))
			{
				return basePlayer.transform.rotation.eulerAngles.ToString();
			}
			return "invalid player";
		}

		// Token: 0x060042F5 RID: 17141 RVA: 0x0018C1B8 File Offset: 0x0018A3B8
		[ServerVar(Help = "Print the current player eyes.")]
		public static string printeyes(ConsoleSystem.Arg arg)
		{
			BasePlayer basePlayer = arg.Player();
			if (arg.HasArgs(1))
			{
				basePlayer = arg.GetPlayerOrSleeper(0);
			}
			if (!(basePlayer == null))
			{
				return basePlayer.eyes.rotation.eulerAngles.ToString();
			}
			return "invalid player";
		}

		// Token: 0x060042F6 RID: 17142 RVA: 0x0018C210 File Offset: 0x0018A410
		[ServerVar(ServerAdmin = true, Help = "This sends a snapshot of all the entities in the client's pvs. This is mostly redundant, but we request this when the client starts recording a demo.. so they get all the information.")]
		public static void snapshot(ConsoleSystem.Arg arg)
		{
			BasePlayer basePlayer = arg.Player();
			if (basePlayer == null)
			{
				return;
			}
			UnityEngine.Debug.Log("Sending full snapshot to " + basePlayer);
			basePlayer.SendNetworkUpdateImmediate(false);
			basePlayer.SendGlobalSnapshot();
			basePlayer.SendFullSnapshot();
			basePlayer.SendEntityUpdate();
			TreeManager.SendSnapshot(basePlayer);
			ServerMgr.SendReplicatedVars(basePlayer.net.connection);
		}

		// Token: 0x060042F7 RID: 17143 RVA: 0x0018C270 File Offset: 0x0018A470
		[ServerVar(Help = "Send network update for all players")]
		public static void sendnetworkupdate(ConsoleSystem.Arg arg)
		{
			foreach (BasePlayer basePlayer in BasePlayer.activePlayerList)
			{
				basePlayer.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
			}
		}

		// Token: 0x060042F8 RID: 17144 RVA: 0x0018C2C0 File Offset: 0x0018A4C0
		[ServerVar(Help = "Prints the position of all players on the server")]
		public static void playerlistpos(ConsoleSystem.Arg arg)
		{
			TextTable textTable = new TextTable();
			textTable.AddColumns(new string[]
			{
				"SteamID",
				"DisplayName",
				"POS",
				"ROT"
			});
			foreach (BasePlayer basePlayer in BasePlayer.activePlayerList)
			{
				textTable.AddRow(new string[]
				{
					basePlayer.userID.ToString(),
					basePlayer.displayName,
					basePlayer.transform.position.ToString(),
					basePlayer.eyes.BodyForward().ToString()
				});
			}
			arg.ReplyWith(arg.HasArg("--json") ? textTable.ToJson() : textTable.ToString());
		}

		// Token: 0x060042F9 RID: 17145 RVA: 0x0018C3BC File Offset: 0x0018A5BC
		[ServerVar(Help = "Prints all the vending machines on the server")]
		public static void listvendingmachines(ConsoleSystem.Arg arg)
		{
			TextTable textTable = new TextTable();
			textTable.AddColumns(new string[]
			{
				"EntityId",
				"Position",
				"Name"
			});
			foreach (VendingMachine vendingMachine in BaseNetworkable.serverEntities.OfType<VendingMachine>())
			{
				textTable.AddRow(new string[]
				{
					vendingMachine.net.ID.ToString(),
					vendingMachine.transform.position.ToString(),
					vendingMachine.shopName.QuoteSafe()
				});
			}
			arg.ReplyWith(arg.HasArg("--json") ? textTable.ToJson() : textTable.ToString());
		}

		// Token: 0x060042FA RID: 17146 RVA: 0x0018C4A0 File Offset: 0x0018A6A0
		[ServerVar(Help = "Prints all the Tool Cupboards on the server")]
		public static void listtoolcupboards(ConsoleSystem.Arg arg)
		{
			TextTable textTable = new TextTable();
			textTable.AddColumns(new string[]
			{
				"EntityId",
				"Position",
				"Authed"
			});
			foreach (BuildingPrivlidge buildingPrivlidge in BaseNetworkable.serverEntities.OfType<BuildingPrivlidge>())
			{
				textTable.AddRow(new string[]
				{
					buildingPrivlidge.net.ID.ToString(),
					buildingPrivlidge.transform.position.ToString(),
					buildingPrivlidge.authorizedPlayers.Count.ToString()
				});
			}
			arg.ReplyWith(arg.HasArg("--json") ? textTable.ToJson() : textTable.ToString());
		}

		// Token: 0x060042FB RID: 17147 RVA: 0x0018C590 File Offset: 0x0018A790
		[ServerVar]
		public static void BroadcastPlayVideo(ConsoleSystem.Arg arg)
		{
			string @string = arg.GetString(0, "");
			if (string.IsNullOrWhiteSpace(@string))
			{
				arg.ReplyWith("Missing video URL");
				return;
			}
			foreach (BasePlayer basePlayer in BasePlayer.activePlayerList)
			{
				basePlayer.Command("client.playvideo", new object[]
				{
					@string
				});
			}
			arg.ReplyWith(string.Format("Sent video to {0} players", BasePlayer.activePlayerList.Count));
		}
	}
}
