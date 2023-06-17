using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using CompanionServer;
using ConVar;
using Facepunch;
using Facepunch.Extend;
using Facepunch.Math;
using Facepunch.Models;
using Facepunch.Rust;
using Network;
using Network.Visibility;
using Newtonsoft.Json;
using ProtoBuf;
using Rust;
using SilentOrbit.ProtocolBuffers;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x02000045 RID: 69
public class BasePlayer : BaseCombatEntity, LootPanel.IHasLootPanel, IIdealSlotEntity
{
	// Token: 0x040003AB RID: 939
	[NonSerialized]
	public bool isInAir;

	// Token: 0x040003AC RID: 940
	[NonSerialized]
	public bool isOnPlayer;

	// Token: 0x040003AD RID: 941
	[NonSerialized]
	public float violationLevel;

	// Token: 0x040003AE RID: 942
	[NonSerialized]
	public float lastViolationTime;

	// Token: 0x040003AF RID: 943
	[NonSerialized]
	public float lastAdminCheatTime;

	// Token: 0x040003B0 RID: 944
	[NonSerialized]
	public AntiHackType lastViolationType;

	// Token: 0x040003B1 RID: 945
	[NonSerialized]
	public float vehiclePauseTime;

	// Token: 0x040003B2 RID: 946
	[NonSerialized]
	public float speedhackPauseTime;

	// Token: 0x040003B3 RID: 947
	[NonSerialized]
	public float speedhackDistance;

	// Token: 0x040003B4 RID: 948
	[NonSerialized]
	public float flyhackPauseTime;

	// Token: 0x040003B5 RID: 949
	[NonSerialized]
	public float flyhackDistanceVertical;

	// Token: 0x040003B6 RID: 950
	[NonSerialized]
	public float flyhackDistanceHorizontal;

	// Token: 0x040003B7 RID: 951
	[NonSerialized]
	public TimeAverageValueLookup<uint> rpcHistory = new TimeAverageValueLookup<uint>();

	// Token: 0x040003B8 RID: 952
	public ViewModel GestureViewModel;

	// Token: 0x040003B9 RID: 953
	private const float drinkRange = 1.5f;

	// Token: 0x040003BA RID: 954
	private const float drinkMovementSpeed = 0.1f;

	// Token: 0x040003BB RID: 955
	[NonSerialized]
	private global::BasePlayer.NetworkQueueList[] networkQueue = new global::BasePlayer.NetworkQueueList[]
	{
		new global::BasePlayer.NetworkQueueList(),
		new global::BasePlayer.NetworkQueueList()
	};

	// Token: 0x040003BC RID: 956
	[NonSerialized]
	private global::BasePlayer.NetworkQueueList SnapshotQueue = new global::BasePlayer.NetworkQueueList();

	// Token: 0x040003BD RID: 957
	public const string GestureCancelString = "cancel";

	// Token: 0x040003BE RID: 958
	public GestureCollection gestureList;

	// Token: 0x040003BF RID: 959
	private TimeUntil gestureFinishedTime;

	// Token: 0x040003C0 RID: 960
	private TimeSince blockHeldInputTimer;

	// Token: 0x040003C1 RID: 961
	private GestureConfig currentGesture;

	// Token: 0x040003C2 RID: 962
	private HashSet<NetworkableId> recentWaveTargets = new HashSet<NetworkableId>();

	// Token: 0x040003C3 RID: 963
	public const string WAVED_PLAYERS_STAT = "waved_at_players";

	// Token: 0x040003C4 RID: 964
	public ulong currentTeam;

	// Token: 0x040003C5 RID: 965
	public static readonly Translate.Phrase MaxTeamSizeToast = new Translate.Phrase("maxteamsizetip", "Your team is full. Remove a member to invite another player.");

	// Token: 0x040003C6 RID: 966
	private bool sentInstrumentTeamAchievement;

	// Token: 0x040003C7 RID: 967
	private bool sentSummerTeamAchievement;

	// Token: 0x040003C8 RID: 968
	private const int TEAMMATE_INSTRUMENT_COUNT_ACHIEVEMENT = 4;

	// Token: 0x040003C9 RID: 969
	private const int TEAMMATE_SUMMER_FLOATING_COUNT_ACHIEVEMENT = 4;

	// Token: 0x040003CA RID: 970
	private const string TEAMMATE_INSTRUMENT_ACHIEVEMENT = "TEAM_INSTRUMENTS";

	// Token: 0x040003CB RID: 971
	private const string TEAMMATE_SUMMER_ACHIEVEMENT = "SUMMER_INFLATABLE";

	// Token: 0x040003CC RID: 972
	public static Translate.Phrase MarkerLimitPhrase = new Translate.Phrase("map.marker.limited", "Cannot place more than {0} markers.");

	// Token: 0x040003CD RID: 973
	public const int MaxMapNoteLabelLength = 10;

	// Token: 0x040003CE RID: 974
	public List<BaseMission.MissionInstance> missions = new List<BaseMission.MissionInstance>();

	// Token: 0x040003CF RID: 975
	private float thinkEvery = 1f;

	// Token: 0x040003D0 RID: 976
	private float timeSinceMissionThink;

	// Token: 0x040003D1 RID: 977
	private int _activeMission = -1;

	// Token: 0x040003D2 RID: 978
	[NonSerialized]
	public ModelState modelState = new ModelState();

	// Token: 0x040003D4 RID: 980
	[NonSerialized]
	private bool wantsSendModelState;

	// Token: 0x040003D5 RID: 981
	[NonSerialized]
	private float nextModelStateUpdate;

	// Token: 0x040003D6 RID: 982
	[NonSerialized]
	private EntityRef mounted;

	// Token: 0x040003D7 RID: 983
	private float nextSeatSwapTime;

	// Token: 0x040003D8 RID: 984
	public global::BaseEntity PetEntity;

	// Token: 0x040003D9 RID: 985
	public IPet Pet;

	// Token: 0x040003DA RID: 986
	private float lastPetCommandIssuedTime;

	// Token: 0x040003DB RID: 987
	private static readonly Translate.Phrase HostileTitle = new Translate.Phrase("ping_hostile", "Hostile");

	// Token: 0x040003DC RID: 988
	private static readonly Translate.Phrase HostileDesc = new Translate.Phrase("ping_hostile_desc", "Danger in area");

	// Token: 0x040003DD RID: 989
	private static readonly global::BasePlayer.PingStyle HostileMarker = new global::BasePlayer.PingStyle(4, 3, global::BasePlayer.HostileTitle, global::BasePlayer.HostileDesc, global::BasePlayer.PingType.Hostile);

	// Token: 0x040003DE RID: 990
	private static readonly Translate.Phrase GoToTitle = new Translate.Phrase("ping_goto", "Go To");

	// Token: 0x040003DF RID: 991
	private static readonly Translate.Phrase GoToDesc = new Translate.Phrase("ping_goto_desc", "Look at this");

	// Token: 0x040003E0 RID: 992
	private static readonly global::BasePlayer.PingStyle GoToMarker = new global::BasePlayer.PingStyle(0, 2, global::BasePlayer.GoToTitle, global::BasePlayer.GoToDesc, global::BasePlayer.PingType.GoTo);

	// Token: 0x040003E1 RID: 993
	private static readonly Translate.Phrase DollarTitle = new Translate.Phrase("ping_dollar", "Value");

	// Token: 0x040003E2 RID: 994
	private static readonly Translate.Phrase DollarDesc = new Translate.Phrase("ping_dollar_desc", "Something valuable is here");

	// Token: 0x040003E3 RID: 995
	private static readonly global::BasePlayer.PingStyle DollarMarker = new global::BasePlayer.PingStyle(1, 1, global::BasePlayer.DollarTitle, global::BasePlayer.DollarDesc, global::BasePlayer.PingType.Dollar);

	// Token: 0x040003E4 RID: 996
	private static readonly Translate.Phrase LootTitle = new Translate.Phrase("ping_loot", "Loot");

	// Token: 0x040003E5 RID: 997
	private static readonly Translate.Phrase LootDesc = new Translate.Phrase("ping_loot_desc", "Loot is here");

	// Token: 0x040003E6 RID: 998
	private static readonly global::BasePlayer.PingStyle LootMarker = new global::BasePlayer.PingStyle(11, 0, global::BasePlayer.LootTitle, global::BasePlayer.LootDesc, global::BasePlayer.PingType.Loot);

	// Token: 0x040003E7 RID: 999
	private static readonly Translate.Phrase NodeTitle = new Translate.Phrase("ping_node", "Node");

	// Token: 0x040003E8 RID: 1000
	private static readonly Translate.Phrase NodeDesc = new Translate.Phrase("ping_node_desc", "An ore node is here");

	// Token: 0x040003E9 RID: 1001
	private static readonly global::BasePlayer.PingStyle NodeMarker = new global::BasePlayer.PingStyle(10, 4, global::BasePlayer.NodeTitle, global::BasePlayer.NodeDesc, global::BasePlayer.PingType.Node);

	// Token: 0x040003EA RID: 1002
	private static readonly Translate.Phrase GunTitle = new Translate.Phrase("ping_gun", "Weapon");

	// Token: 0x040003EB RID: 1003
	private static readonly Translate.Phrase GunDesc = new Translate.Phrase("ping_weapon_desc", "A dropped weapon is here");

	// Token: 0x040003EC RID: 1004
	private static readonly global::BasePlayer.PingStyle GunMarker = new global::BasePlayer.PingStyle(9, 5, global::BasePlayer.GunTitle, global::BasePlayer.GunDesc, global::BasePlayer.PingType.Gun);

	// Token: 0x040003ED RID: 1005
	private TimeSince lastTick;

	// Token: 0x040003EE RID: 1006
	private bool _playerStateDirty;

	// Token: 0x040003EF RID: 1007
	private string _wipeId;

	// Token: 0x040003F0 RID: 1008
	public Dictionary<int, global::BasePlayer.FiredProjectile> firedProjectiles = new Dictionary<int, global::BasePlayer.FiredProjectile>();

	// Token: 0x040003F1 RID: 1009
	private const int WILDERNESS = 1;

	// Token: 0x040003F2 RID: 1010
	private const int MONUMENT = 2;

	// Token: 0x040003F3 RID: 1011
	private const int BASE = 4;

	// Token: 0x040003F4 RID: 1012
	private const int FLYING = 8;

	// Token: 0x040003F5 RID: 1013
	private const int BOATING = 16;

	// Token: 0x040003F6 RID: 1014
	private const int SWIMMING = 32;

	// Token: 0x040003F7 RID: 1015
	private const int DRIVING = 64;

	// Token: 0x040003F8 RID: 1016
	[ServerVar]
	[Help("How many milliseconds to budget for processing life story updates per frame")]
	public static float lifeStoryFramebudgetms = 0.25f;

	// Token: 0x040003F9 RID: 1017
	[NonSerialized]
	public PlayerLifeStory lifeStory;

	// Token: 0x040003FA RID: 1018
	[NonSerialized]
	public PlayerLifeStory previousLifeStory;

	// Token: 0x040003FB RID: 1019
	private const float TimeCategoryUpdateFrequency = 7f;

	// Token: 0x040003FC RID: 1020
	private float nextTimeCategoryUpdate;

	// Token: 0x040003FE RID: 1022
	private bool hasSentPresenceState;

	// Token: 0x040003FF RID: 1023
	private bool LifeStoryInWilderness;

	// Token: 0x04000400 RID: 1024
	private bool LifeStoryInMonument;

	// Token: 0x04000401 RID: 1025
	private bool LifeStoryInBase;

	// Token: 0x04000402 RID: 1026
	private bool LifeStoryFlying;

	// Token: 0x04000403 RID: 1027
	private bool LifeStoryBoating;

	// Token: 0x04000404 RID: 1028
	private bool LifeStorySwimming;

	// Token: 0x04000405 RID: 1029
	private bool LifeStoryDriving;

	// Token: 0x04000406 RID: 1030
	private bool waitingForLifeStoryUpdate;

	// Token: 0x04000407 RID: 1031
	public static global::BasePlayer.LifeStoryWorkQueue lifeStoryQueue = new global::BasePlayer.LifeStoryWorkQueue();

	// Token: 0x04000408 RID: 1032
	[NonSerialized]
	public PlayerStatistics stats;

	// Token: 0x04000409 RID: 1033
	[NonSerialized]
	public ItemId svActiveItemID;

	// Token: 0x0400040A RID: 1034
	[NonSerialized]
	public float NextChatTime;

	// Token: 0x0400040B RID: 1035
	[NonSerialized]
	public float nextSuicideTime;

	// Token: 0x0400040C RID: 1036
	[NonSerialized]
	public float nextRespawnTime;

	// Token: 0x0400040D RID: 1037
	[NonSerialized]
	public string respawnId;

	// Token: 0x04000416 RID: 1046
	protected Vector3 viewAngles;

	// Token: 0x04000417 RID: 1047
	private float lastSubscriptionTick;

	// Token: 0x04000418 RID: 1048
	private float lastPlayerTick;

	// Token: 0x04000419 RID: 1049
	private float sleepStartTime = -1f;

	// Token: 0x0400041A RID: 1050
	private float fallTickRate = 0.1f;

	// Token: 0x0400041B RID: 1051
	private float lastFallTime;

	// Token: 0x0400041C RID: 1052
	private float fallVelocity;

	// Token: 0x0400041D RID: 1053
	private HitInfo cachedNonSuicideHitInfo;

	// Token: 0x0400041E RID: 1054
	public static ListHashSet<global::BasePlayer> activePlayerList = new ListHashSet<global::BasePlayer>(8);

	// Token: 0x0400041F RID: 1055
	public static ListHashSet<global::BasePlayer> sleepingPlayerList = new ListHashSet<global::BasePlayer>(8);

	// Token: 0x04000420 RID: 1056
	public static ListHashSet<global::BasePlayer> bots = new ListHashSet<global::BasePlayer>(8);

	// Token: 0x04000421 RID: 1057
	private float cachedCraftLevel;

	// Token: 0x04000422 RID: 1058
	private float nextCheckTime;

	// Token: 0x04000423 RID: 1059
	private Workbench _cachedWorkbench;

	// Token: 0x04000424 RID: 1060
	private PersistantPlayer cachedPersistantPlayer;

	// Token: 0x04000425 RID: 1061
	private int SpectateOffset = 1000000;

	// Token: 0x04000426 RID: 1062
	private string spectateFilter = "";

	// Token: 0x04000428 RID: 1064
	private float lastUpdateTime = float.NegativeInfinity;

	// Token: 0x04000429 RID: 1065
	private float cachedThreatLevel;

	// Token: 0x0400042A RID: 1066
	[NonSerialized]
	public float weaponDrawnDuration;

	// Token: 0x0400042B RID: 1067
	public const int serverTickRateDefault = 16;

	// Token: 0x0400042C RID: 1068
	public const int clientTickRateDefault = 20;

	// Token: 0x0400042D RID: 1069
	public int serverTickRate = 16;

	// Token: 0x0400042E RID: 1070
	public int clientTickRate = 20;

	// Token: 0x0400042F RID: 1071
	public float serverTickInterval = 0.0625f;

	// Token: 0x04000430 RID: 1072
	public float clientTickInterval = 0.05f;

	// Token: 0x04000432 RID: 1074
	[NonSerialized]
	private float lastTickTime;

	// Token: 0x04000433 RID: 1075
	[NonSerialized]
	private float lastStallTime;

	// Token: 0x04000434 RID: 1076
	[NonSerialized]
	private float lastInputTime;

	// Token: 0x04000435 RID: 1077
	private PlayerTick lastReceivedTick = new PlayerTick();

	// Token: 0x04000436 RID: 1078
	private float tickDeltaTime;

	// Token: 0x04000437 RID: 1079
	private bool tickNeedsFinalizing;

	// Token: 0x04000439 RID: 1081
	private TimeAverageValue ticksPerSecond = new TimeAverageValue();

	// Token: 0x0400043A RID: 1082
	private TickInterpolator tickInterpolator = new TickInterpolator();

	// Token: 0x0400043B RID: 1083
	public Deque<Vector3> eyeHistory = new Deque<Vector3>(8);

	// Token: 0x0400043C RID: 1084
	public TickHistory tickHistory = new TickHistory();

	// Token: 0x0400043D RID: 1085
	private float nextUnderwearValidationTime;

	// Token: 0x0400043E RID: 1086
	private uint lastValidUnderwearSkin;

	// Token: 0x0400043F RID: 1087
	private float woundedDuration;

	// Token: 0x04000440 RID: 1088
	private float lastWoundedStartTime = float.NegativeInfinity;

	// Token: 0x04000441 RID: 1089
	private float healingWhileCrawling;

	// Token: 0x04000442 RID: 1090
	private bool woundedByFallDamage;

	// Token: 0x04000443 RID: 1091
	private const float INCAPACITATED_HEALTH_MIN = 2f;

	// Token: 0x04000444 RID: 1092
	private const float INCAPACITATED_HEALTH_MAX = 6f;

	// Token: 0x04000445 RID: 1093
	public const int MaxBotIdRange = 10000000;

	// Token: 0x04000446 RID: 1094
	[Header("BasePlayer")]
	public GameObjectRef fallDamageEffect;

	// Token: 0x04000447 RID: 1095
	public GameObjectRef drownEffect;

	// Token: 0x04000448 RID: 1096
	[global::InspectorFlags]
	public global::BasePlayer.PlayerFlags playerFlags;

	// Token: 0x04000449 RID: 1097
	[NonSerialized]
	public PlayerEyes eyes;

	// Token: 0x0400044A RID: 1098
	[NonSerialized]
	public global::PlayerInventory inventory;

	// Token: 0x0400044B RID: 1099
	[NonSerialized]
	public PlayerBlueprints blueprints;

	// Token: 0x0400044C RID: 1100
	[NonSerialized]
	public global::PlayerMetabolism metabolism;

	// Token: 0x0400044D RID: 1101
	[NonSerialized]
	public global::PlayerModifiers modifiers;

	// Token: 0x0400044E RID: 1102
	private CapsuleCollider playerCollider;

	// Token: 0x0400044F RID: 1103
	public PlayerBelt Belt;

	// Token: 0x04000450 RID: 1104
	private Rigidbody playerRigidbody;

	// Token: 0x04000451 RID: 1105
	[NonSerialized]
	public ulong userID;

	// Token: 0x04000452 RID: 1106
	[NonSerialized]
	public string UserIDString;

	// Token: 0x04000453 RID: 1107
	[NonSerialized]
	public int gamemodeteam = -1;

	// Token: 0x04000454 RID: 1108
	[NonSerialized]
	public int reputation;

	// Token: 0x04000455 RID: 1109
	protected string _displayName;

	// Token: 0x04000456 RID: 1110
	private string _lastSetName;

	// Token: 0x04000457 RID: 1111
	public const float crouchSpeed = 1.7f;

	// Token: 0x04000458 RID: 1112
	public const float walkSpeed = 2.8f;

	// Token: 0x04000459 RID: 1113
	public const float runSpeed = 5.5f;

	// Token: 0x0400045A RID: 1114
	public const float crawlSpeed = 0.72f;

	// Token: 0x0400045B RID: 1115
	private global::BasePlayer.CapsuleColliderInfo playerColliderStanding;

	// Token: 0x0400045C RID: 1116
	private global::BasePlayer.CapsuleColliderInfo playerColliderDucked;

	// Token: 0x0400045D RID: 1117
	private global::BasePlayer.CapsuleColliderInfo playerColliderCrawling;

	// Token: 0x0400045E RID: 1118
	private global::BasePlayer.CapsuleColliderInfo playerColliderLyingDown;

	// Token: 0x0400045F RID: 1119
	private ProtectionProperties cachedProtection;

	// Token: 0x04000460 RID: 1120
	private float nextColliderRefreshTime = -1f;

	// Token: 0x04000461 RID: 1121
	public bool clothingBlocksAiming;

	// Token: 0x04000462 RID: 1122
	public float clothingMoveSpeedReduction;

	// Token: 0x04000463 RID: 1123
	public float clothingWaterSpeedBonus;

	// Token: 0x04000464 RID: 1124
	public float clothingAccuracyBonus;

	// Token: 0x04000465 RID: 1125
	public bool equippingBlocked;

	// Token: 0x04000466 RID: 1126
	public float eggVision;

	// Token: 0x04000467 RID: 1127
	private PhoneController activeTelephone;

	// Token: 0x04000468 RID: 1128
	public global::BaseEntity designingAIEntity;

	// Token: 0x0600052D RID: 1325 RVA: 0x00038488 File Offset: 0x00036688
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("BasePlayer.OnRpcMessage", 0))
		{
			if (rpc == 935768323U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - ClientKeepConnectionAlive ");
				}
				using (TimeWarning.New("ClientKeepConnectionAlive", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.FromOwner.Test(935768323U, "ClientKeepConnectionAlive", this, player))
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
							this.ClientKeepConnectionAlive(msg2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in ClientKeepConnectionAlive");
					}
				}
				return true;
			}
			if (rpc == 3782818894U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - ClientLoadingComplete ");
				}
				using (TimeWarning.New("ClientLoadingComplete", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.FromOwner.Test(3782818894U, "ClientLoadingComplete", this, player))
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
							this.ClientLoadingComplete(msg3);
						}
					}
					catch (Exception exception2)
					{
						Debug.LogException(exception2);
						player.Kick("RPC Error in ClientLoadingComplete");
					}
				}
				return true;
			}
			if (rpc == 1497207530U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - IssuePetCommand ");
				}
				using (TimeWarning.New("IssuePetCommand", 0))
				{
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
							this.IssuePetCommand(msg4);
						}
					}
					catch (Exception exception3)
					{
						Debug.LogException(exception3);
						player.Kick("RPC Error in IssuePetCommand");
					}
				}
				return true;
			}
			if (rpc == 2041023702U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - IssuePetCommandRaycast ");
				}
				using (TimeWarning.New("IssuePetCommandRaycast", 0))
				{
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
							this.IssuePetCommandRaycast(msg5);
						}
					}
					catch (Exception exception4)
					{
						Debug.LogException(exception4);
						player.Kick("RPC Error in IssuePetCommandRaycast");
					}
				}
				return true;
			}
			if (rpc == 3441821928U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - OnFeedbackReport ");
				}
				using (TimeWarning.New("OnFeedbackReport", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.CallsPerSecond.Test(3441821928U, "OnFeedbackReport", this, player, 1UL))
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
							this.OnFeedbackReport(msg6);
						}
					}
					catch (Exception exception5)
					{
						Debug.LogException(exception5);
						player.Kick("RPC Error in OnFeedbackReport");
					}
				}
				return true;
			}
			if (rpc == 1998170713U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - OnPlayerLanded ");
				}
				using (TimeWarning.New("OnPlayerLanded", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.FromOwner.Test(1998170713U, "OnPlayerLanded", this, player))
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
							this.OnPlayerLanded(msg7);
						}
					}
					catch (Exception exception6)
					{
						Debug.LogException(exception6);
						player.Kick("RPC Error in OnPlayerLanded");
					}
				}
				return true;
			}
			if (rpc == 2147041557U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - OnPlayerReported ");
				}
				using (TimeWarning.New("OnPlayerReported", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.CallsPerSecond.Test(2147041557U, "OnPlayerReported", this, player, 1UL))
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
							this.OnPlayerReported(msg8);
						}
					}
					catch (Exception exception7)
					{
						Debug.LogException(exception7);
						player.Kick("RPC Error in OnPlayerReported");
					}
				}
				return true;
			}
			if (rpc == 363681694U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - OnProjectileAttack ");
				}
				using (TimeWarning.New("OnProjectileAttack", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.FromOwner.Test(363681694U, "OnProjectileAttack", this, player))
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
							this.OnProjectileAttack(msg9);
						}
					}
					catch (Exception exception8)
					{
						Debug.LogException(exception8);
						player.Kick("RPC Error in OnProjectileAttack");
					}
				}
				return true;
			}
			if (rpc == 1500391289U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - OnProjectileRicochet ");
				}
				using (TimeWarning.New("OnProjectileRicochet", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.FromOwner.Test(1500391289U, "OnProjectileRicochet", this, player))
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
							this.OnProjectileRicochet(msg10);
						}
					}
					catch (Exception exception9)
					{
						Debug.LogException(exception9);
						player.Kick("RPC Error in OnProjectileRicochet");
					}
				}
				return true;
			}
			if (rpc == 2324190493U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - OnProjectileUpdate ");
				}
				using (TimeWarning.New("OnProjectileUpdate", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.FromOwner.Test(2324190493U, "OnProjectileUpdate", this, player))
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
							this.OnProjectileUpdate(msg11);
						}
					}
					catch (Exception exception10)
					{
						Debug.LogException(exception10);
						player.Kick("RPC Error in OnProjectileUpdate");
					}
				}
				return true;
			}
			if (rpc == 3167788018U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - PerformanceReport ");
				}
				using (TimeWarning.New("PerformanceReport", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.CallsPerSecond.Test(3167788018U, "PerformanceReport", this, player, 1UL))
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
							this.PerformanceReport(msg12);
						}
					}
					catch (Exception exception11)
					{
						Debug.LogException(exception11);
						player.Kick("RPC Error in PerformanceReport");
					}
				}
				return true;
			}
			if (rpc == 420048204U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - PerformanceReport_Frametime ");
				}
				using (TimeWarning.New("PerformanceReport_Frametime", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.CallsPerSecond.Test(420048204U, "PerformanceReport_Frametime", this, player, 1UL))
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
							this.PerformanceReport_Frametime(msg13);
						}
					}
					catch (Exception exception12)
					{
						Debug.LogException(exception12);
						player.Kick("RPC Error in PerformanceReport_Frametime");
					}
				}
				return true;
			}
			if (rpc == 52352806U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RequestRespawnInformation ");
				}
				using (TimeWarning.New("RequestRespawnInformation", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.CallsPerSecond.Test(52352806U, "RequestRespawnInformation", this, player, 1UL))
						{
							return true;
						}
						if (!global::BaseEntity.RPC_Server.FromOwner.Test(52352806U, "RequestRespawnInformation", this, player))
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
							this.RequestRespawnInformation(msg14);
						}
					}
					catch (Exception exception13)
					{
						Debug.LogException(exception13);
						player.Kick("RPC Error in RequestRespawnInformation");
					}
				}
				return true;
			}
			if (rpc == 970468557U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_Assist ");
				}
				using (TimeWarning.New("RPC_Assist", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.IsVisible.Test(970468557U, "RPC_Assist", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage msg15 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.RPC_Assist(msg15);
						}
					}
					catch (Exception exception14)
					{
						Debug.LogException(exception14);
						player.Kick("RPC Error in RPC_Assist");
					}
				}
				return true;
			}
			if (rpc == 3263238541U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_KeepAlive ");
				}
				using (TimeWarning.New("RPC_KeepAlive", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.IsVisible.Test(3263238541U, "RPC_KeepAlive", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage msg16 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.RPC_KeepAlive(msg16);
						}
					}
					catch (Exception exception15)
					{
						Debug.LogException(exception15);
						player.Kick("RPC Error in RPC_KeepAlive");
					}
				}
				return true;
			}
			if (rpc == 3692395068U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_LootPlayer ");
				}
				using (TimeWarning.New("RPC_LootPlayer", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.IsVisible.Test(3692395068U, "RPC_LootPlayer", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage msg17 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.RPC_LootPlayer(msg17);
						}
					}
					catch (Exception exception16)
					{
						Debug.LogException(exception16);
						player.Kick("RPC Error in RPC_LootPlayer");
					}
				}
				return true;
			}
			if (rpc == 1539133504U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_StartClimb ");
				}
				using (TimeWarning.New("RPC_StartClimb", 0))
				{
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage msg18 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.RPC_StartClimb(msg18);
						}
					}
					catch (Exception exception17)
					{
						Debug.LogException(exception17);
						player.Kick("RPC Error in RPC_StartClimb");
					}
				}
				return true;
			}
			if (rpc == 3047177092U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - Server_AddMarker ");
				}
				using (TimeWarning.New("Server_AddMarker", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.CallsPerSecond.Test(3047177092U, "Server_AddMarker", this, player, 8UL))
						{
							return true;
						}
						if (!global::BaseEntity.RPC_Server.FromOwner.Test(3047177092U, "Server_AddMarker", this, player))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage msg19 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.Server_AddMarker(msg19);
						}
					}
					catch (Exception exception18)
					{
						Debug.LogException(exception18);
						player.Kick("RPC Error in Server_AddMarker");
					}
				}
				return true;
			}
			if (rpc == 3618659425U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - Server_AddPing ");
				}
				using (TimeWarning.New("Server_AddPing", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.CallsPerSecond.Test(3618659425U, "Server_AddPing", this, player, 3UL))
						{
							return true;
						}
						if (!global::BaseEntity.RPC_Server.FromOwner.Test(3618659425U, "Server_AddPing", this, player))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage msg20 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.Server_AddPing(msg20);
						}
					}
					catch (Exception exception19)
					{
						Debug.LogException(exception19);
						player.Kick("RPC Error in Server_AddPing");
					}
				}
				return true;
			}
			if (rpc == 1005040107U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - Server_CancelGesture ");
				}
				using (TimeWarning.New("Server_CancelGesture", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.CallsPerSecond.Test(1005040107U, "Server_CancelGesture", this, player, 10UL))
						{
							return true;
						}
						if (!global::BaseEntity.RPC_Server.FromOwner.Test(1005040107U, "Server_CancelGesture", this, player))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							this.Server_CancelGesture();
						}
					}
					catch (Exception exception20)
					{
						Debug.LogException(exception20);
						player.Kick("RPC Error in Server_CancelGesture");
					}
				}
				return true;
			}
			if (rpc == 706157120U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - Server_ClearMapMarkers ");
				}
				using (TimeWarning.New("Server_ClearMapMarkers", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.CallsPerSecond.Test(706157120U, "Server_ClearMapMarkers", this, player, 1UL))
						{
							return true;
						}
						if (!global::BaseEntity.RPC_Server.FromOwner.Test(706157120U, "Server_ClearMapMarkers", this, player))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage msg21 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.Server_ClearMapMarkers(msg21);
						}
					}
					catch (Exception exception21)
					{
						Debug.LogException(exception21);
						player.Kick("RPC Error in Server_ClearMapMarkers");
					}
				}
				return true;
			}
			if (rpc == 1032755717U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - Server_RemovePing ");
				}
				using (TimeWarning.New("Server_RemovePing", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.CallsPerSecond.Test(1032755717U, "Server_RemovePing", this, player, 3UL))
						{
							return true;
						}
						if (!global::BaseEntity.RPC_Server.FromOwner.Test(1032755717U, "Server_RemovePing", this, player))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage msg22 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.Server_RemovePing(msg22);
						}
					}
					catch (Exception exception22)
					{
						Debug.LogException(exception22);
						player.Kick("RPC Error in Server_RemovePing");
					}
				}
				return true;
			}
			if (rpc == 31713840U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - Server_RemovePointOfInterest ");
				}
				using (TimeWarning.New("Server_RemovePointOfInterest", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.CallsPerSecond.Test(31713840U, "Server_RemovePointOfInterest", this, player, 10UL))
						{
							return true;
						}
						if (!global::BaseEntity.RPC_Server.FromOwner.Test(31713840U, "Server_RemovePointOfInterest", this, player))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage msg23 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.Server_RemovePointOfInterest(msg23);
						}
					}
					catch (Exception exception23)
					{
						Debug.LogException(exception23);
						player.Kick("RPC Error in Server_RemovePointOfInterest");
					}
				}
				return true;
			}
			if (rpc == 2567683804U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - Server_RequestMarkers ");
				}
				using (TimeWarning.New("Server_RequestMarkers", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.CallsPerSecond.Test(2567683804U, "Server_RequestMarkers", this, player, 1UL))
						{
							return true;
						}
						if (!global::BaseEntity.RPC_Server.FromOwner.Test(2567683804U, "Server_RequestMarkers", this, player))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage msg24 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.Server_RequestMarkers(msg24);
						}
					}
					catch (Exception exception24)
					{
						Debug.LogException(exception24);
						player.Kick("RPC Error in Server_RequestMarkers");
					}
				}
				return true;
			}
			if (rpc == 1572722245U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - Server_StartGesture ");
				}
				using (TimeWarning.New("Server_StartGesture", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.CallsPerSecond.Test(1572722245U, "Server_StartGesture", this, player, 1UL))
						{
							return true;
						}
						if (!global::BaseEntity.RPC_Server.FromOwner.Test(1572722245U, "Server_StartGesture", this, player))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage msg25 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.Server_StartGesture(msg25);
						}
					}
					catch (Exception exception25)
					{
						Debug.LogException(exception25);
						player.Kick("RPC Error in Server_StartGesture");
					}
				}
				return true;
			}
			if (rpc == 1180369886U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - Server_UpdateMarker ");
				}
				using (TimeWarning.New("Server_UpdateMarker", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.CallsPerSecond.Test(1180369886U, "Server_UpdateMarker", this, player, 1UL))
						{
							return true;
						}
						if (!global::BaseEntity.RPC_Server.FromOwner.Test(1180369886U, "Server_UpdateMarker", this, player))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage msg26 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.Server_UpdateMarker(msg26);
						}
					}
					catch (Exception exception26)
					{
						Debug.LogException(exception26);
						player.Kick("RPC Error in Server_UpdateMarker");
					}
				}
				return true;
			}
			if (rpc == 3635568749U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - ServerRPC_UnderwearChange ");
				}
				using (TimeWarning.New("ServerRPC_UnderwearChange", 0))
				{
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage msg27 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.ServerRPC_UnderwearChange(msg27);
						}
					}
					catch (Exception exception27)
					{
						Debug.LogException(exception27);
						player.Kick("RPC Error in ServerRPC_UnderwearChange");
					}
				}
				return true;
			}
			if (rpc == 970114602U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - SV_Drink ");
				}
				using (TimeWarning.New("SV_Drink", 0))
				{
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage msg28 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.SV_Drink(msg28);
						}
					}
					catch (Exception exception28)
					{
						Debug.LogException(exception28);
						player.Kick("RPC Error in SV_Drink");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x0600052E RID: 1326 RVA: 0x0003A9C4 File Offset: 0x00038BC4
	public bool TriggeredAntiHack(float seconds = 1f, float score = float.PositiveInfinity)
	{
		return UnityEngine.Time.realtimeSinceStartup - this.lastViolationTime < seconds || this.violationLevel > score;
	}

	// Token: 0x0600052F RID: 1327 RVA: 0x0003A9E0 File Offset: 0x00038BE0
	public bool UsedAdminCheat(float seconds = 2f)
	{
		return UnityEngine.Time.realtimeSinceStartup - this.lastAdminCheatTime < seconds;
	}

	// Token: 0x06000530 RID: 1328 RVA: 0x0003A9F1 File Offset: 0x00038BF1
	public void PauseVehicleNoClipDetection(float seconds = 1f)
	{
		this.vehiclePauseTime = Mathf.Max(this.vehiclePauseTime, seconds);
	}

	// Token: 0x06000531 RID: 1329 RVA: 0x0003AA05 File Offset: 0x00038C05
	public void PauseFlyHackDetection(float seconds = 1f)
	{
		this.flyhackPauseTime = Mathf.Max(this.flyhackPauseTime, seconds);
	}

	// Token: 0x06000532 RID: 1330 RVA: 0x0003AA19 File Offset: 0x00038C19
	public void PauseSpeedHackDetection(float seconds = 1f)
	{
		this.speedhackPauseTime = Mathf.Max(this.speedhackPauseTime, seconds);
	}

	// Token: 0x06000533 RID: 1331 RVA: 0x0003AA2D File Offset: 0x00038C2D
	public int GetAntiHackKicks()
	{
		return global::AntiHack.GetKickRecord(this);
	}

	// Token: 0x06000534 RID: 1332 RVA: 0x0003AA38 File Offset: 0x00038C38
	public void ResetAntiHack()
	{
		this.violationLevel = 0f;
		this.lastViolationTime = 0f;
		this.lastAdminCheatTime = 0f;
		this.speedhackPauseTime = 0f;
		this.speedhackDistance = 0f;
		this.flyhackPauseTime = 0f;
		this.flyhackDistanceVertical = 0f;
		this.flyhackDistanceHorizontal = 0f;
		this.rpcHistory.Clear();
	}

	// Token: 0x06000535 RID: 1333 RVA: 0x0003AAA8 File Offset: 0x00038CA8
	public override bool CanBeLooted(global::BasePlayer player)
	{
		return !(player == this) && (this.IsWounded() || this.IsSleeping());
	}

	// Token: 0x17000091 RID: 145
	// (get) Token: 0x06000536 RID: 1334 RVA: 0x0003AAC5 File Offset: 0x00038CC5
	public Translate.Phrase LootPanelTitle
	{
		get
		{
			return this.displayName;
		}
	}

	// Token: 0x06000537 RID: 1335 RVA: 0x0003AAD4 File Offset: 0x00038CD4
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsVisible(3f)]
	public void RPC_LootPlayer(global::BaseEntity.RPCMessage msg)
	{
		global::BasePlayer player = msg.player;
		if (!player || !player.CanInteract())
		{
			return;
		}
		if (!this.CanBeLooted(player))
		{
			return;
		}
		if (player.inventory.loot.StartLootingEntity(this, true))
		{
			player.inventory.loot.AddContainer(this.inventory.containerMain);
			player.inventory.loot.AddContainer(this.inventory.containerWear);
			player.inventory.loot.AddContainer(this.inventory.containerBelt);
			player.inventory.loot.SendImmediate();
			player.ClientRPCPlayer<string>(null, player, "RPC_OpenLootPanel", "player_corpse");
		}
	}

	// Token: 0x06000538 RID: 1336 RVA: 0x0003AB8C File Offset: 0x00038D8C
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsVisible(3f)]
	public void RPC_Assist(global::BaseEntity.RPCMessage msg)
	{
		if (!msg.player.CanInteract())
		{
			return;
		}
		if (msg.player == this)
		{
			return;
		}
		if (!this.IsWounded())
		{
			return;
		}
		this.StopWounded(msg.player);
		msg.player.stats.Add("wounded_assisted", 1, (global::Stats)5);
		this.stats.Add("wounded_healed", 1, global::Stats.Steam);
	}

	// Token: 0x06000539 RID: 1337 RVA: 0x0003ABF4 File Offset: 0x00038DF4
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsVisible(3f)]
	public void RPC_KeepAlive(global::BaseEntity.RPCMessage msg)
	{
		if (!msg.player.CanInteract())
		{
			return;
		}
		if (msg.player == this)
		{
			return;
		}
		if (!this.IsWounded())
		{
			return;
		}
		this.ProlongWounding(10f);
	}

	// Token: 0x0600053A RID: 1338 RVA: 0x0003AC28 File Offset: 0x00038E28
	[global::BaseEntity.RPC_Server]
	private void SV_Drink(global::BaseEntity.RPCMessage msg)
	{
		global::BasePlayer player = msg.player;
		Vector3 vector = msg.read.Vector3();
		if (vector.IsNaNOrInfinity())
		{
			return;
		}
		if (!player)
		{
			return;
		}
		if (!player.metabolism.CanConsume())
		{
			return;
		}
		if (Vector3.Distance(player.transform.position, vector) > 5f)
		{
			return;
		}
		if (!WaterLevel.Test(vector, true, this))
		{
			return;
		}
		if (this.isMounted && !this.GetMounted().canDrinkWhileMounted)
		{
			return;
		}
		ItemDefinition atPoint = WaterResource.GetAtPoint(vector);
		if (atPoint == null)
		{
			return;
		}
		ItemModConsumable component = atPoint.GetComponent<ItemModConsumable>();
		global::Item item = ItemManager.Create(atPoint, component.amountToConsume, 0UL);
		ItemModConsume component2 = item.info.GetComponent<ItemModConsume>();
		if (component2.CanDoAction(item, player))
		{
			component2.DoAction(item, player);
		}
		if (item != null)
		{
			item.Remove(0f);
		}
		player.metabolism.MarkConsumption();
	}

	// Token: 0x0600053B RID: 1339 RVA: 0x0003AD0C File Offset: 0x00038F0C
	[global::BaseEntity.RPC_Server]
	public void RPC_StartClimb(global::BaseEntity.RPCMessage msg)
	{
		global::BasePlayer player = msg.player;
		bool flag = msg.read.Bit();
		Vector3 vector = msg.read.Vector3();
		NetworkableId networkableId = msg.read.EntityID();
		global::BaseNetworkable baseNetworkable = global::BaseNetworkable.serverEntities.Find(networkableId);
		Vector3 vector2 = flag ? baseNetworkable.transform.TransformPoint(vector) : vector;
		if (!player.isMounted)
		{
			return;
		}
		if (player.Distance(vector2) > 5f)
		{
			return;
		}
		if (!GamePhysics.LineOfSight(player.eyes.position, vector2, 1218519041, null))
		{
			return;
		}
		if (!GamePhysics.LineOfSight(vector2, vector2 + player.eyes.offset, 1218519041, null))
		{
			return;
		}
		Vector3 end = vector2 - (vector2 - player.eyes.position).normalized * 0.25f;
		if (GamePhysics.CheckCapsule(player.eyes.position, end, 0.25f, 1218519041, QueryTriggerInteraction.UseGlobal))
		{
			return;
		}
		Collider collider;
		if (global::AntiHack.TestNoClipping(vector2 + player.NoClipOffset(), vector2 + player.NoClipOffset(), player.NoClipRadius(ConVar.AntiHack.noclip_margin), ConVar.AntiHack.noclip_backtracking, true, out collider, false, null))
		{
			return;
		}
		player.EnsureDismounted();
		player.transform.position = vector2;
		Collider component = player.GetComponent<Collider>();
		component.enabled = false;
		component.enabled = true;
		player.ForceUpdateTriggers(true, true, true);
		if (flag)
		{
			player.ClientRPCPlayer<Vector3, NetworkableId>(null, player, "ForcePositionToParentOffset", vector, networkableId);
			return;
		}
		player.ClientRPCPlayer<Vector3>(null, player, "ForcePositionTo", vector2);
	}

	// Token: 0x0600053C RID: 1340 RVA: 0x0003AE92 File Offset: 0x00039092
	public int GetQueuedUpdateCount(global::BasePlayer.NetworkQueue queue)
	{
		return this.networkQueue[(int)queue].Length;
	}

	// Token: 0x0600053D RID: 1341 RVA: 0x0003AEA4 File Offset: 0x000390A4
	public void SendSnapshots(ListHashSet<Networkable> ents)
	{
		using (TimeWarning.New("SendSnapshots", 0))
		{
			int count = ents.Values.Count;
			Networkable[] buffer = ents.Values.Buffer;
			for (int i = 0; i < count; i++)
			{
				this.SnapshotQueue.Add(buffer[i].handler as global::BaseNetworkable);
			}
		}
	}

	// Token: 0x0600053E RID: 1342 RVA: 0x0003AF18 File Offset: 0x00039118
	public void QueueUpdate(global::BasePlayer.NetworkQueue queue, global::BaseNetworkable ent)
	{
		if (!this.IsConnected)
		{
			return;
		}
		if (queue == global::BasePlayer.NetworkQueue.Update)
		{
			this.networkQueue[0].Add(ent);
			return;
		}
		if (queue != global::BasePlayer.NetworkQueue.UpdateDistance)
		{
			return;
		}
		if (this.IsReceivingSnapshot)
		{
			return;
		}
		if (this.networkQueue[1].Contains(ent))
		{
			return;
		}
		if (this.networkQueue[0].Contains(ent))
		{
			return;
		}
		global::BasePlayer.NetworkQueueList networkQueueList = this.networkQueue[1];
		if (base.Distance(ent as global::BaseEntity) < 20f)
		{
			this.QueueUpdate(global::BasePlayer.NetworkQueue.Update, ent);
			return;
		}
		networkQueueList.Add(ent);
	}

	// Token: 0x0600053F RID: 1343 RVA: 0x0003AF9C File Offset: 0x0003919C
	public void SendEntityUpdate()
	{
		using (TimeWarning.New("SendEntityUpdate", 0))
		{
			this.SendEntityUpdates(this.SnapshotQueue);
			this.SendEntityUpdates(this.networkQueue[0]);
			this.SendEntityUpdates(this.networkQueue[1]);
		}
	}

	// Token: 0x06000540 RID: 1344 RVA: 0x0003AFFC File Offset: 0x000391FC
	public void ClearEntityQueue(Group group = null)
	{
		this.SnapshotQueue.Clear(group);
		this.networkQueue[0].Clear(group);
		this.networkQueue[1].Clear(group);
	}

	// Token: 0x06000541 RID: 1345 RVA: 0x0003B028 File Offset: 0x00039228
	private void SendEntityUpdates(global::BasePlayer.NetworkQueueList queue)
	{
		if (queue.queueInternal.Count == 0)
		{
			return;
		}
		int num = this.IsReceivingSnapshot ? ConVar.Server.updatebatchspawn : ConVar.Server.updatebatch;
		List<global::BaseNetworkable> list = Facepunch.Pool.GetList<global::BaseNetworkable>();
		using (TimeWarning.New("SendEntityUpdates.SendEntityUpdates", 0))
		{
			int num2 = 0;
			foreach (global::BaseNetworkable baseNetworkable in queue.queueInternal)
			{
				this.SendEntitySnapshot(baseNetworkable);
				list.Add(baseNetworkable);
				num2++;
				if (num2 > num)
				{
					break;
				}
			}
		}
		if (num > queue.queueInternal.Count)
		{
			queue.queueInternal.Clear();
		}
		else
		{
			using (TimeWarning.New("SendEntityUpdates.Remove", 0))
			{
				for (int i = 0; i < list.Count; i++)
				{
					queue.queueInternal.Remove(list[i]);
				}
			}
		}
		if (queue.queueInternal.Count == 0 && queue.MaxLength > 2048)
		{
			queue.queueInternal.Clear();
			queue.queueInternal = new HashSet<global::BaseNetworkable>();
			queue.MaxLength = 0;
		}
		Facepunch.Pool.FreeList<global::BaseNetworkable>(ref list);
	}

	// Token: 0x06000542 RID: 1346 RVA: 0x0003B184 File Offset: 0x00039384
	private void SendEntitySnapshot(global::BaseNetworkable ent)
	{
		using (TimeWarning.New("SendEntitySnapshot", 0))
		{
			if (!(ent == null))
			{
				if (ent.net != null)
				{
					if (ent.ShouldNetworkTo(this))
					{
						NetWrite netWrite = Network.Net.sv.StartWrite();
						Network.Connection connection = this.net.connection;
						connection.validate.entityUpdates = connection.validate.entityUpdates + 1U;
						global::BaseNetworkable.SaveInfo saveInfo = new global::BaseNetworkable.SaveInfo
						{
							forConnection = this.net.connection,
							forDisk = false
						};
						netWrite.PacketID(Message.Type.Entities);
						netWrite.UInt32(this.net.connection.validate.entityUpdates);
						ent.ToStreamForNetwork(netWrite, saveInfo);
						netWrite.Send(new SendInfo(this.net.connection));
					}
				}
			}
		}
	}

	// Token: 0x06000543 RID: 1347 RVA: 0x0003B26C File Offset: 0x0003946C
	public bool HasPlayerFlag(global::BasePlayer.PlayerFlags f)
	{
		return (this.playerFlags & f) == f;
	}

	// Token: 0x17000092 RID: 146
	// (get) Token: 0x06000544 RID: 1348 RVA: 0x0003B279 File Offset: 0x00039479
	public bool IsReceivingSnapshot
	{
		get
		{
			return this.HasPlayerFlag(global::BasePlayer.PlayerFlags.ReceivingSnapshot);
		}
	}

	// Token: 0x17000093 RID: 147
	// (get) Token: 0x06000545 RID: 1349 RVA: 0x0003B282 File Offset: 0x00039482
	public bool IsAdmin
	{
		get
		{
			return this.HasPlayerFlag(global::BasePlayer.PlayerFlags.IsAdmin);
		}
	}

	// Token: 0x17000094 RID: 148
	// (get) Token: 0x06000546 RID: 1350 RVA: 0x0003B28B File Offset: 0x0003948B
	public bool IsDeveloper
	{
		get
		{
			return this.HasPlayerFlag(global::BasePlayer.PlayerFlags.IsDeveloper);
		}
	}

	// Token: 0x17000095 RID: 149
	// (get) Token: 0x06000547 RID: 1351 RVA: 0x0003B298 File Offset: 0x00039498
	public bool UnlockAllSkins
	{
		get
		{
			return this.IsDeveloper && base.isServer && this.net.connection.info.GetBool("client.unlock_all_skins", false);
		}
	}

	// Token: 0x17000096 RID: 150
	// (get) Token: 0x06000548 RID: 1352 RVA: 0x0003B2C9 File Offset: 0x000394C9
	public bool IsAiming
	{
		get
		{
			return this.HasPlayerFlag(global::BasePlayer.PlayerFlags.Aiming);
		}
	}

	// Token: 0x17000097 RID: 151
	// (get) Token: 0x06000549 RID: 1353 RVA: 0x0003B2D6 File Offset: 0x000394D6
	public bool IsFlying
	{
		get
		{
			return this.modelState != null && this.modelState.flying;
		}
	}

	// Token: 0x17000098 RID: 152
	// (get) Token: 0x0600054A RID: 1354 RVA: 0x0003B2ED File Offset: 0x000394ED
	public bool IsConnected
	{
		get
		{
			return base.isServer && Network.Net.sv != null && this.net != null && this.net.connection != null;
		}
	}

	// Token: 0x0600054B RID: 1355 RVA: 0x0003B31C File Offset: 0x0003951C
	public void SetPlayerFlag(global::BasePlayer.PlayerFlags f, bool b)
	{
		if (b)
		{
			if (this.HasPlayerFlag(f))
			{
				return;
			}
			this.playerFlags |= f;
		}
		else
		{
			if (!this.HasPlayerFlag(f))
			{
				return;
			}
			this.playerFlags &= ~f;
		}
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x0600054C RID: 1356 RVA: 0x0003B35C File Offset: 0x0003955C
	public void LightToggle(bool mask = true)
	{
		global::Item activeItem = this.GetActiveItem();
		if (activeItem != null)
		{
			global::BaseEntity heldEntity = activeItem.GetHeldEntity();
			if (heldEntity != null)
			{
				global::HeldEntity component = heldEntity.GetComponent<global::HeldEntity>();
				if (component)
				{
					component.SendMessage("SetLightsOn", mask && !component.LightsOn(), SendMessageOptions.DontRequireReceiver);
				}
			}
		}
		foreach (global::Item item in this.inventory.containerWear.itemList)
		{
			ItemModWearable component2 = item.info.GetComponent<ItemModWearable>();
			if (component2 && component2.emissive)
			{
				item.SetFlag(global::Item.Flag.IsOn, mask && !item.HasFlag(global::Item.Flag.IsOn));
				item.MarkDirty();
			}
		}
		if (this.isMounted)
		{
			this.GetMounted().LightToggle(this);
		}
	}

	// Token: 0x17000099 RID: 153
	// (get) Token: 0x0600054D RID: 1357 RVA: 0x0003B450 File Offset: 0x00039650
	public bool InGesture
	{
		get
		{
			return this.currentGesture != null && (this.gestureFinishedTime > 0f || this.currentGesture.animationType == GestureConfig.AnimationType.Loop);
		}
	}

	// Token: 0x1700009A RID: 154
	// (get) Token: 0x0600054E RID: 1358 RVA: 0x0003B484 File Offset: 0x00039684
	private bool CurrentGestureBlocksMovement
	{
		get
		{
			return this.InGesture && this.currentGesture.movementMode == GestureConfig.MovementCapabilities.NoMovement;
		}
	}

	// Token: 0x1700009B RID: 155
	// (get) Token: 0x0600054F RID: 1359 RVA: 0x0003B49E File Offset: 0x0003969E
	public bool CurrentGestureIsDance
	{
		get
		{
			return this.InGesture && this.currentGesture.actionType == GestureConfig.GestureActionType.DanceAchievement;
		}
	}

	// Token: 0x1700009C RID: 156
	// (get) Token: 0x06000550 RID: 1360 RVA: 0x0003B4B8 File Offset: 0x000396B8
	public bool CurrentGestureIsFullBody
	{
		get
		{
			return this.InGesture && this.currentGesture.playerModelLayer == GestureConfig.PlayerModelLayer.FullBody;
		}
	}

	// Token: 0x1700009D RID: 157
	// (get) Token: 0x06000551 RID: 1361 RVA: 0x0003B4D2 File Offset: 0x000396D2
	private bool InGestureCancelCooldown
	{
		get
		{
			return this.blockHeldInputTimer < 0.5f;
		}
	}

	// Token: 0x06000552 RID: 1362 RVA: 0x0003B4E8 File Offset: 0x000396E8
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.FromOwner]
	[global::BaseEntity.RPC_Server.CallsPerSecond(1UL)]
	private void Server_StartGesture(global::BaseEntity.RPCMessage msg)
	{
		if (this.IsGestureBlocked())
		{
			return;
		}
		uint id = msg.read.UInt32();
		if (this.gestureList == null)
		{
			return;
		}
		GestureConfig toPlay = this.gestureList.IdToGesture(id);
		this.Server_StartGesture(toPlay);
	}

	// Token: 0x06000553 RID: 1363 RVA: 0x0003B530 File Offset: 0x00039730
	public void Server_StartGesture(uint gestureId)
	{
		if (this.gestureList == null)
		{
			return;
		}
		GestureConfig toPlay = this.gestureList.IdToGesture(gestureId);
		this.Server_StartGesture(toPlay);
	}

	// Token: 0x06000554 RID: 1364 RVA: 0x0003B560 File Offset: 0x00039760
	public void Server_StartGesture(GestureConfig toPlay)
	{
		if (toPlay != null && toPlay.IsOwnedBy(this) && toPlay.CanBeUsedBy(this))
		{
			if (toPlay.animationType == GestureConfig.AnimationType.OneShot)
			{
				base.Invoke(new Action(this.TimeoutGestureServer), toPlay.duration);
			}
			else if (toPlay.animationType == GestureConfig.AnimationType.Loop)
			{
				base.InvokeRepeating(new Action(this.MonitorLoopingGesture), 0f, 0f);
			}
			base.ClientRPC<uint>(null, "Client_StartGesture", toPlay.gestureId);
			this.gestureFinishedTime = toPlay.duration;
			this.currentGesture = toPlay;
			if (toPlay.actionType == GestureConfig.GestureActionType.DanceAchievement)
			{
				TriggerDanceAchievement triggerDanceAchievement = base.FindTrigger<TriggerDanceAchievement>();
				if (triggerDanceAchievement != null)
				{
					triggerDanceAchievement.NotifyDanceStarted();
					return;
				}
			}
			else if (toPlay.actionType == GestureConfig.GestureActionType.ShowNameTag && GameInfo.HasAchievements)
			{
				int val = this.CountWaveTargets(base.transform.position, 4f, 0.6f, this.eyes.HeadForward(), this.recentWaveTargets, 5);
				this.stats.Add("waved_at_players", val, global::Stats.Steam);
				this.stats.Save(true);
			}
		}
	}

	// Token: 0x06000555 RID: 1365 RVA: 0x0003B67E File Offset: 0x0003987E
	private void TimeoutGestureServer()
	{
		this.currentGesture = null;
	}

	// Token: 0x06000556 RID: 1366 RVA: 0x0003B687 File Offset: 0x00039887
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.FromOwner]
	[global::BaseEntity.RPC_Server.CallsPerSecond(10UL)]
	public void Server_CancelGesture()
	{
		this.currentGesture = null;
		this.blockHeldInputTimer = 0f;
		base.ClientRPC(null, "Client_RemoteCancelledGesture");
		base.CancelInvoke(new Action(this.MonitorLoopingGesture));
	}

	// Token: 0x06000557 RID: 1367 RVA: 0x0003B6C0 File Offset: 0x000398C0
	private void MonitorLoopingGesture()
	{
		if (((!(this.currentGesture != null) || !this.currentGesture.canDuckDuringGesture) && this.modelState.ducked) || (this.modelState.sleeping || this.IsWounded() || this.IsSwimming() || this.IsDead() || (this.isMounted && this.GetMounted().allowedGestures == BaseMountable.MountGestureType.UpperBody && this.currentGesture.playerModelLayer == GestureConfig.PlayerModelLayer.FullBody)) || (this.isMounted && this.GetMounted().allowedGestures == BaseMountable.MountGestureType.None))
		{
			this.Server_CancelGesture();
		}
	}

	// Token: 0x06000558 RID: 1368 RVA: 0x0003B75C File Offset: 0x0003995C
	private void NotifyGesturesNewItemEquipped()
	{
		if (this.InGesture)
		{
			this.Server_CancelGesture();
		}
	}

	// Token: 0x06000559 RID: 1369 RVA: 0x0003B76C File Offset: 0x0003996C
	public int CountWaveTargets(Vector3 position, float distance, float minimumDot, Vector3 forward, HashSet<NetworkableId> workingList, int maxCount)
	{
		global::BasePlayer.<>c__DisplayClass87_0 CS$<>8__locals1;
		CS$<>8__locals1.<>4__this = this;
		CS$<>8__locals1.position = position;
		CS$<>8__locals1.forward = forward;
		CS$<>8__locals1.minimumDot = minimumDot;
		CS$<>8__locals1.workingList = workingList;
		CS$<>8__locals1.sqrDistance = distance * distance;
		Group group = this.net.group;
		if (group == null)
		{
			return 0;
		}
		List<Network.Connection> subscribers = group.subscribers;
		int num = 0;
		for (int i = 0; i < subscribers.Count; i++)
		{
			Network.Connection connection = subscribers[i];
			if (connection.active)
			{
				global::BasePlayer basePlayer = connection.player as global::BasePlayer;
				if (this.<CountWaveTargets>g__CheckPlayer|87_0(basePlayer, ref CS$<>8__locals1))
				{
					CS$<>8__locals1.workingList.Add(basePlayer.net.ID);
					num++;
					if (num >= maxCount)
					{
						break;
					}
				}
			}
		}
		return num;
	}

	// Token: 0x0600055A RID: 1370 RVA: 0x0003B82C File Offset: 0x00039A2C
	private bool IsGestureBlocked()
	{
		if (this.isMounted && this.GetMounted().allowedGestures == BaseMountable.MountGestureType.None)
		{
			return true;
		}
		if (this.GetHeldEntity() && this.GetHeldEntity().BlocksGestures())
		{
			return true;
		}
		bool flag = this.currentGesture != null;
		if (flag && this.currentGesture.gestureType == GestureConfig.GestureType.Cinematic)
		{
			flag = false;
		}
		return this.IsWounded() || flag || this.IsDead() || this.IsSleeping();
	}

	// Token: 0x1700009E RID: 158
	// (get) Token: 0x0600055B RID: 1371 RVA: 0x0003B8A6 File Offset: 0x00039AA6
	public global::RelationshipManager.PlayerTeam Team
	{
		get
		{
			if (global::RelationshipManager.ServerInstance == null)
			{
				return null;
			}
			return global::RelationshipManager.ServerInstance.FindTeam(this.currentTeam);
		}
	}

	// Token: 0x0600055C RID: 1372 RVA: 0x0003B8C7 File Offset: 0x00039AC7
	public void DelayedTeamUpdate()
	{
		this.UpdateTeam(this.currentTeam);
	}

	// Token: 0x0600055D RID: 1373 RVA: 0x0003B8D5 File Offset: 0x00039AD5
	public void TeamUpdate()
	{
		this.TeamUpdate(false);
	}

	// Token: 0x0600055E RID: 1374 RVA: 0x0003B8E0 File Offset: 0x00039AE0
	public void TeamUpdate(bool fullTeamUpdate)
	{
		if (!global::RelationshipManager.TeamsEnabled())
		{
			return;
		}
		if (this.IsConnected && this.currentTeam != 0UL)
		{
			global::RelationshipManager.PlayerTeam playerTeam = global::RelationshipManager.ServerInstance.FindTeam(this.currentTeam);
			if (playerTeam == null)
			{
				return;
			}
			int num = 0;
			int num2 = 0;
			using (PlayerTeam playerTeam2 = Facepunch.Pool.Get<PlayerTeam>())
			{
				playerTeam2.teamLeader = playerTeam.teamLeader;
				playerTeam2.teamID = playerTeam.teamID;
				playerTeam2.teamName = playerTeam.teamName;
				playerTeam2.members = Facepunch.Pool.GetList<PlayerTeam.TeamMember>();
				playerTeam2.teamLifetime = playerTeam.teamLifetime;
				playerTeam2.teamPings = Facepunch.Pool.GetList<MapNote>();
				foreach (ulong playerID in playerTeam.members)
				{
					global::BasePlayer basePlayer = global::RelationshipManager.FindByID(playerID);
					PlayerTeam.TeamMember teamMember = Facepunch.Pool.Get<PlayerTeam.TeamMember>();
					teamMember.displayName = ((basePlayer != null) ? basePlayer.displayName : (SingletonComponent<ServerMgr>.Instance.persistance.GetPlayerName(playerID) ?? "DEAD"));
					teamMember.healthFraction = ((basePlayer != null && basePlayer.IsAlive()) ? basePlayer.healthFraction : 0f);
					teamMember.position = ((basePlayer != null) ? basePlayer.transform.position : Vector3.zero);
					teamMember.online = (basePlayer != null && !basePlayer.IsSleeping());
					teamMember.wounded = (basePlayer != null && basePlayer.IsWounded());
					if ((!this.sentInstrumentTeamAchievement || !this.sentSummerTeamAchievement) && basePlayer != null)
					{
						if (basePlayer.GetHeldEntity() && basePlayer.GetHeldEntity().IsInstrument())
						{
							num++;
						}
						if (basePlayer.isMounted)
						{
							if (basePlayer.GetMounted().IsInstrument())
							{
								num++;
							}
							if (basePlayer.GetMounted().IsSummerDlcVehicle)
							{
								num2++;
							}
						}
						if (num >= 4 && !this.sentInstrumentTeamAchievement)
						{
							this.GiveAchievement("TEAM_INSTRUMENTS");
							this.sentInstrumentTeamAchievement = true;
						}
						if (num2 >= 4)
						{
							this.GiveAchievement("SUMMER_INFLATABLE");
							this.sentSummerTeamAchievement = true;
						}
					}
					teamMember.userID = playerID;
					playerTeam2.members.Add(teamMember);
					if (basePlayer != null)
					{
						if (basePlayer.State.pings != null && basePlayer.State.pings.Count > 0 && basePlayer != this)
						{
							playerTeam2.teamPings.AddRange(basePlayer.State.pings);
						}
						if (fullTeamUpdate && basePlayer != this)
						{
							basePlayer.TeamUpdate(false);
						}
					}
				}
				playerTeam2.leaderMapNotes = Facepunch.Pool.GetList<MapNote>();
				PlayerState playerState = SingletonComponent<ServerMgr>.Instance.playerStateManager.Get(playerTeam.teamLeader);
				if (((playerState != null) ? playerState.pointsOfInterest : null) != null)
				{
					foreach (MapNote item in playerState.pointsOfInterest)
					{
						playerTeam2.leaderMapNotes.Add(item);
					}
				}
				base.ClientRPCPlayerAndSpectators<PlayerTeam>(null, this, "CLIENT_ReceiveTeamInfo", playerTeam2);
				if (playerTeam2.leaderMapNotes != null)
				{
					playerTeam2.leaderMapNotes.Clear();
				}
				if (playerTeam2.teamPings != null)
				{
					playerTeam2.teamPings.Clear();
				}
				global::BasePlayer basePlayer2 = global::BasePlayer.FindByID(playerTeam.teamLeader);
				if (fullTeamUpdate && basePlayer2 != null && basePlayer2 != this)
				{
					basePlayer2.TeamUpdate(false);
				}
			}
		}
	}

	// Token: 0x0600055F RID: 1375 RVA: 0x0003BCB4 File Offset: 0x00039EB4
	public void UpdateTeam(ulong newTeam)
	{
		this.currentTeam = newTeam;
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
		if (global::RelationshipManager.ServerInstance.FindTeam(newTeam) == null)
		{
			this.ClearTeam();
			return;
		}
		this.TeamUpdate();
	}

	// Token: 0x06000560 RID: 1376 RVA: 0x0003BCDE File Offset: 0x00039EDE
	public void ClearTeam()
	{
		this.currentTeam = 0UL;
		base.ClientRPCPlayerAndSpectators(null, this, "CLIENT_ClearTeam");
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x06000561 RID: 1377 RVA: 0x0003BCFC File Offset: 0x00039EFC
	public void ClearPendingInvite()
	{
		base.ClientRPCPlayer<string, int>(null, this, "CLIENT_PendingInvite", "", 0);
	}

	// Token: 0x06000562 RID: 1378 RVA: 0x0003BD14 File Offset: 0x00039F14
	public global::HeldEntity GetHeldEntity()
	{
		if (!base.isServer)
		{
			return null;
		}
		global::Item activeItem = this.GetActiveItem();
		if (activeItem == null)
		{
			return null;
		}
		return activeItem.GetHeldEntity() as global::HeldEntity;
	}

	// Token: 0x06000563 RID: 1379 RVA: 0x0003BD44 File Offset: 0x00039F44
	public bool IsHoldingEntity<T>()
	{
		global::HeldEntity heldEntity = this.GetHeldEntity();
		return !(heldEntity == null) && heldEntity is T;
	}

	// Token: 0x06000564 RID: 1380 RVA: 0x0003BD6C File Offset: 0x00039F6C
	public bool IsHostileItem(global::Item item)
	{
		if (!item.info.isHoldable)
		{
			return false;
		}
		ItemModEntity component = item.info.GetComponent<ItemModEntity>();
		if (component == null)
		{
			return false;
		}
		GameObject gameObject = component.entityPrefab.Get();
		if (gameObject == null)
		{
			return false;
		}
		AttackEntity component2 = gameObject.GetComponent<AttackEntity>();
		return !(component2 == null) && component2.hostile;
	}

	// Token: 0x06000565 RID: 1381 RVA: 0x0003BDCE File Offset: 0x00039FCE
	public bool IsItemHoldRestricted(global::Item item)
	{
		return !this.IsNpc && (this.InSafeZone() && item != null && this.IsHostileItem(item));
	}

	// Token: 0x1700009F RID: 159
	// (get) Token: 0x06000566 RID: 1382 RVA: 0x0003BDF1 File Offset: 0x00039FF1
	// (set) Token: 0x06000567 RID: 1383 RVA: 0x0003BDFE File Offset: 0x00039FFE
	public MapNote ServerCurrentDeathNote
	{
		get
		{
			return this.State.deathMarker;
		}
		set
		{
			this.State.deathMarker = value;
		}
	}

	// Token: 0x06000568 RID: 1384 RVA: 0x0003BE0C File Offset: 0x0003A00C
	public void Server_LogDeathMarker(Vector3 position)
	{
		if (this.IsNpc)
		{
			return;
		}
		if (this.ServerCurrentDeathNote == null)
		{
			this.ServerCurrentDeathNote = Facepunch.Pool.Get<MapNote>();
			this.ServerCurrentDeathNote.noteType = 0;
		}
		this.ServerCurrentDeathNote.worldPosition = position;
		base.ClientRPCPlayer<MapNote>(null, this, "Client_AddNewDeathMarker", this.ServerCurrentDeathNote);
		this.DirtyPlayerState();
	}

	// Token: 0x06000569 RID: 1385 RVA: 0x0003BE68 File Offset: 0x0003A068
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.FromOwner]
	[global::BaseEntity.RPC_Server.CallsPerSecond(8UL)]
	public void Server_AddMarker(global::BaseEntity.RPCMessage msg)
	{
		if (this.State.pointsOfInterest == null)
		{
			this.State.pointsOfInterest = Facepunch.Pool.GetList<MapNote>();
		}
		if (this.State.pointsOfInterest.Count >= ConVar.Server.maximumMapMarkers)
		{
			msg.player.ShowToast(GameTip.Styles.Blue_Short, global::BasePlayer.MarkerLimitPhrase, new string[]
			{
				ConVar.Server.maximumMapMarkers.ToString()
			});
			return;
		}
		MapNote mapNote = MapNote.Deserialize(msg.read);
		this.ValidateMapNote(mapNote);
		mapNote.colourIndex = this.FindUnusedPointOfInterestColour();
		this.State.pointsOfInterest.Add(mapNote);
		this.DirtyPlayerState();
		this.SendMarkersToClient();
		this.TeamUpdate();
	}

	// Token: 0x0600056A RID: 1386 RVA: 0x0003BF10 File Offset: 0x0003A110
	private int FindUnusedPointOfInterestColour()
	{
		if (this.State.pointsOfInterest == null)
		{
			return 0;
		}
		int num = 0;
		for (int i = 0; i < 6; i++)
		{
			if (this.<FindUnusedPointOfInterestColour>g__HasColour|117_0(num))
			{
				num++;
			}
		}
		return num;
	}

	// Token: 0x0600056B RID: 1387 RVA: 0x0003BF48 File Offset: 0x0003A148
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.FromOwner]
	[global::BaseEntity.RPC_Server.CallsPerSecond(1UL)]
	public void Server_UpdateMarker(global::BaseEntity.RPCMessage msg)
	{
		if (this.State.pointsOfInterest == null)
		{
			this.State.pointsOfInterest = Facepunch.Pool.GetList<MapNote>();
		}
		int num = msg.read.Int32();
		if (this.State.pointsOfInterest.Count <= num)
		{
			return;
		}
		using (MapNote mapNote = MapNote.Deserialize(msg.read))
		{
			this.ValidateMapNote(mapNote);
			mapNote.CopyTo(this.State.pointsOfInterest[num]);
			this.DirtyPlayerState();
			this.SendMarkersToClient();
			this.TeamUpdate();
		}
	}

	// Token: 0x0600056C RID: 1388 RVA: 0x0003BFEC File Offset: 0x0003A1EC
	private void ValidateMapNote(MapNote n)
	{
		if (n.label != null)
		{
			n.label = n.label.Truncate(10, null).ToUpperInvariant();
		}
	}

	// Token: 0x0600056D RID: 1389 RVA: 0x0003C010 File Offset: 0x0003A210
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.FromOwner]
	[global::BaseEntity.RPC_Server.CallsPerSecond(10UL)]
	public void Server_RemovePointOfInterest(global::BaseEntity.RPCMessage msg)
	{
		int num = msg.read.Int32();
		if (this.State.pointsOfInterest != null && this.State.pointsOfInterest.Count > num && num >= 0)
		{
			this.State.pointsOfInterest[num].Dispose();
			this.State.pointsOfInterest.RemoveAt(num);
			this.DirtyPlayerState();
			this.SendMarkersToClient();
			this.TeamUpdate();
		}
	}

	// Token: 0x0600056E RID: 1390 RVA: 0x0003C086 File Offset: 0x0003A286
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.FromOwner]
	[global::BaseEntity.RPC_Server.CallsPerSecond(1UL)]
	public void Server_RequestMarkers(global::BaseEntity.RPCMessage msg)
	{
		this.SendMarkersToClient();
	}

	// Token: 0x0600056F RID: 1391 RVA: 0x0003C090 File Offset: 0x0003A290
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.FromOwner]
	[global::BaseEntity.RPC_Server.CallsPerSecond(1UL)]
	public void Server_ClearMapMarkers(global::BaseEntity.RPCMessage msg)
	{
		MapNote serverCurrentDeathNote = this.ServerCurrentDeathNote;
		if (serverCurrentDeathNote != null)
		{
			serverCurrentDeathNote.Dispose();
		}
		this.ServerCurrentDeathNote = null;
		if (this.State.pointsOfInterest != null)
		{
			foreach (MapNote mapNote in this.State.pointsOfInterest)
			{
				if (mapNote != null)
				{
					mapNote.Dispose();
				}
			}
			this.State.pointsOfInterest.Clear();
		}
		this.DirtyPlayerState();
		this.TeamUpdate();
	}

	// Token: 0x06000570 RID: 1392 RVA: 0x0003C12C File Offset: 0x0003A32C
	private void SendMarkersToClient()
	{
		using (MapNoteList mapNoteList = Facepunch.Pool.Get<MapNoteList>())
		{
			mapNoteList.notes = Facepunch.Pool.GetList<MapNote>();
			if (this.ServerCurrentDeathNote != null)
			{
				mapNoteList.notes.Add(this.ServerCurrentDeathNote);
			}
			if (this.State.pointsOfInterest != null)
			{
				mapNoteList.notes.AddRange(this.State.pointsOfInterest);
			}
			base.ClientRPCPlayer<MapNoteList>(null, this, "Client_ReceiveMarkers", mapNoteList);
			mapNoteList.notes.Clear();
		}
	}

	// Token: 0x06000571 RID: 1393 RVA: 0x0003C1BC File Offset: 0x0003A3BC
	public bool HasAttemptedMission(uint missionID)
	{
		using (List<BaseMission.MissionInstance>.Enumerator enumerator = this.missions.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.missionID == missionID)
				{
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x06000572 RID: 1394 RVA: 0x0003C218 File Offset: 0x0003A418
	public bool CanAcceptMission(uint missionID)
	{
		if (this.HasActiveMission())
		{
			return false;
		}
		if (!BaseMission.missionsenabled)
		{
			return false;
		}
		BaseMission fromID = MissionManifest.GetFromID(missionID);
		if (fromID == null)
		{
			Debug.LogError("MISSION NOT FOUND IN MANIFEST, ID :" + missionID);
			return false;
		}
		if (fromID.acceptDependancies != null && fromID.acceptDependancies.Length != 0)
		{
			foreach (BaseMission.MissionDependancy missionDependancy in fromID.acceptDependancies)
			{
				if (!missionDependancy.everAttempted)
				{
					bool flag = false;
					foreach (BaseMission.MissionInstance missionInstance in this.missions)
					{
						if (missionInstance.missionID == missionDependancy.targetMissionID && missionInstance.status == missionDependancy.targetMissionDesiredStatus)
						{
							flag = true;
						}
					}
					if (!flag)
					{
						return false;
					}
				}
			}
		}
		if (this.IsMissionActive(missionID))
		{
			return false;
		}
		if (fromID.isRepeatable)
		{
			bool flag2 = this.HasCompletedMission(missionID);
			bool flag3 = this.HasFailedMission(missionID);
			if (flag2 && fromID.repeatDelaySecondsSuccess == -1)
			{
				return false;
			}
			if (flag3 && fromID.repeatDelaySecondsFailed == -1)
			{
				return false;
			}
			foreach (BaseMission.MissionInstance missionInstance2 in this.missions)
			{
				if (missionInstance2.missionID == missionID)
				{
					float num = 0f;
					if (missionInstance2.status == BaseMission.MissionStatus.Completed)
					{
						num = (float)fromID.repeatDelaySecondsSuccess;
					}
					else if (missionInstance2.status == BaseMission.MissionStatus.Failed)
					{
						num = (float)fromID.repeatDelaySecondsFailed;
					}
					float endTime = missionInstance2.endTime;
					if (UnityEngine.Time.time - endTime < num)
					{
						return false;
					}
				}
			}
		}
		BaseMission.PositionGenerator[] positionGenerators = fromID.positionGenerators;
		for (int i = 0; i < positionGenerators.Length; i++)
		{
			if (!positionGenerators[i].Validate(this, fromID))
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x06000573 RID: 1395 RVA: 0x0003C3FC File Offset: 0x0003A5FC
	public bool IsMissionActive(uint missionID)
	{
		foreach (BaseMission.MissionInstance missionInstance in this.missions)
		{
			if (missionInstance.missionID == missionID && (missionInstance.status == BaseMission.MissionStatus.Active || missionInstance.status == BaseMission.MissionStatus.Accomplished))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06000574 RID: 1396 RVA: 0x0003C46C File Offset: 0x0003A66C
	public bool HasCompletedMission(uint missionID)
	{
		foreach (BaseMission.MissionInstance missionInstance in this.missions)
		{
			if (missionInstance.missionID == missionID && missionInstance.status == BaseMission.MissionStatus.Completed)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06000575 RID: 1397 RVA: 0x0003C4D4 File Offset: 0x0003A6D4
	public bool HasFailedMission(uint missionID)
	{
		foreach (BaseMission.MissionInstance missionInstance in this.missions)
		{
			if (missionInstance.missionID == missionID && missionInstance.status == BaseMission.MissionStatus.Failed)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06000576 RID: 1398 RVA: 0x0003C53C File Offset: 0x0003A73C
	private void WipeMissions()
	{
		if (this.missions.Count > 0)
		{
			for (int i = this.missions.Count - 1; i >= 0; i--)
			{
				BaseMission.MissionInstance missionInstance = this.missions[i];
				if (missionInstance != null)
				{
					missionInstance.GetMission().MissionFailed(missionInstance, this, BaseMission.MissionFailReason.ResetPlayerState);
					Facepunch.Pool.Free<BaseMission.MissionInstance>(ref missionInstance);
				}
			}
		}
		this.missions.Clear();
		this.SetActiveMission(-1);
		this.MissionDirty(true);
	}

	// Token: 0x06000577 RID: 1399 RVA: 0x0003C5B0 File Offset: 0x0003A7B0
	public void AbandonActiveMission()
	{
		if (!this.HasActiveMission())
		{
			return;
		}
		int activeMission = this.GetActiveMission();
		if (activeMission != -1 && activeMission < this.missions.Count)
		{
			BaseMission.MissionInstance missionInstance = this.missions[activeMission];
			missionInstance.GetMission().MissionFailed(missionInstance, this, BaseMission.MissionFailReason.Abandon);
		}
	}

	// Token: 0x06000578 RID: 1400 RVA: 0x0003C5FA File Offset: 0x0003A7FA
	public void AddMission(BaseMission.MissionInstance instance)
	{
		this.missions.Add(instance);
		this.MissionDirty(true);
	}

	// Token: 0x06000579 RID: 1401 RVA: 0x0003C610 File Offset: 0x0003A810
	public void ThinkMissions(float delta)
	{
		if (!BaseMission.missionsenabled)
		{
			return;
		}
		if (this.timeSinceMissionThink < this.thinkEvery)
		{
			this.timeSinceMissionThink += delta;
			return;
		}
		foreach (BaseMission.MissionInstance missionInstance in this.missions)
		{
			missionInstance.Think(this, this.timeSinceMissionThink);
		}
		this.timeSinceMissionThink = 0f;
	}

	// Token: 0x0600057A RID: 1402 RVA: 0x0003C698 File Offset: 0x0003A898
	public void ClearMissions()
	{
		this.missions.Clear();
		this.State.missions = this.SaveMissions();
		this.DirtyPlayerState();
	}

	// Token: 0x0600057B RID: 1403 RVA: 0x0003C6BC File Offset: 0x0003A8BC
	public void MissionDirty(bool shouldSendNetworkUpdate = true)
	{
		if (!BaseMission.missionsenabled)
		{
			return;
		}
		this.State.missions = this.SaveMissions();
		this.DirtyPlayerState();
		if (shouldSendNetworkUpdate)
		{
			base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
		}
	}

	// Token: 0x0600057C RID: 1404 RVA: 0x0003C6E8 File Offset: 0x0003A8E8
	public void ProcessMissionEvent(BaseMission.MissionEventType type, string identifier, float amount)
	{
		if (!BaseMission.missionsenabled)
		{
			return;
		}
		foreach (BaseMission.MissionInstance missionInstance in this.missions)
		{
			missionInstance.ProcessMissionEvent(this, type, identifier, amount);
		}
	}

	// Token: 0x0600057D RID: 1405 RVA: 0x0003C744 File Offset: 0x0003A944
	private Missions SaveMissions()
	{
		Missions missions = Facepunch.Pool.Get<Missions>();
		missions.missions = Facepunch.Pool.GetList<MissionInstance>();
		missions.activeMission = this.GetActiveMission();
		missions.protocol = 238;
		missions.seed = global::World.Seed;
		missions.saveCreatedTime = Epoch.FromDateTime(SaveRestore.SaveCreatedTime);
		foreach (BaseMission.MissionInstance missionInstance in this.missions)
		{
			MissionInstance missionInstance2 = Facepunch.Pool.Get<MissionInstance>();
			missionInstance2.providerID = missionInstance.providerID;
			missionInstance2.missionID = missionInstance.missionID;
			missionInstance2.missionStatus = (uint)missionInstance.status;
			missionInstance2.completionScale = missionInstance.completionScale;
			missionInstance2.startTime = UnityEngine.Time.realtimeSinceStartup - missionInstance.startTime;
			missionInstance2.endTime = missionInstance.endTime;
			missionInstance2.missionLocation = missionInstance.missionLocation;
			missionInstance2.missionPoints = Facepunch.Pool.GetList<ProtoBuf.MissionPoint>();
			foreach (KeyValuePair<string, Vector3> keyValuePair in missionInstance.missionPoints)
			{
				ProtoBuf.MissionPoint missionPoint = Facepunch.Pool.Get<ProtoBuf.MissionPoint>();
				missionPoint.identifier = keyValuePair.Key;
				missionPoint.location = keyValuePair.Value;
				missionInstance2.missionPoints.Add(missionPoint);
			}
			missionInstance2.objectiveStatuses = Facepunch.Pool.GetList<ObjectiveStatus>();
			foreach (BaseMission.MissionInstance.ObjectiveStatus objectiveStatus in missionInstance.objectiveStatuses)
			{
				ObjectiveStatus objectiveStatus2 = Facepunch.Pool.Get<ObjectiveStatus>();
				objectiveStatus2.completed = objectiveStatus.completed;
				objectiveStatus2.failed = objectiveStatus.failed;
				objectiveStatus2.started = objectiveStatus.started;
				objectiveStatus2.genericFloat1 = objectiveStatus.genericFloat1;
				objectiveStatus2.genericInt1 = objectiveStatus.genericInt1;
				missionInstance2.objectiveStatuses.Add(objectiveStatus2);
			}
			missionInstance2.createdEntities = Facepunch.Pool.GetList<NetworkableId>();
			if (missionInstance.createdEntities != null)
			{
				foreach (MissionEntity missionEntity in missionInstance.createdEntities)
				{
					if (!(missionEntity == null))
					{
						global::BaseEntity entity = missionEntity.GetEntity();
						if (entity)
						{
							missionInstance2.createdEntities.Add(entity.net.ID);
						}
					}
				}
			}
			if (missionInstance.rewards != null && missionInstance.rewards.Length != 0)
			{
				missionInstance2.rewards = Facepunch.Pool.GetList<MissionReward>();
				foreach (ItemAmount itemAmount in missionInstance.rewards)
				{
					MissionReward missionReward = Facepunch.Pool.Get<MissionReward>();
					missionReward.itemID = itemAmount.itemid;
					missionReward.itemAmount = Mathf.FloorToInt(itemAmount.amount);
					missionInstance2.rewards.Add(missionReward);
				}
			}
			missions.missions.Add(missionInstance2);
		}
		return missions;
	}

	// Token: 0x0600057E RID: 1406 RVA: 0x0003CA60 File Offset: 0x0003AC60
	public void SetActiveMission(int index)
	{
		this._activeMission = index;
	}

	// Token: 0x0600057F RID: 1407 RVA: 0x0003CA69 File Offset: 0x0003AC69
	public int GetActiveMission()
	{
		return this._activeMission;
	}

	// Token: 0x06000580 RID: 1408 RVA: 0x0003CA71 File Offset: 0x0003AC71
	public bool HasActiveMission()
	{
		return this.GetActiveMission() != -1;
	}

	// Token: 0x06000581 RID: 1409 RVA: 0x0003CA80 File Offset: 0x0003AC80
	private void LoadMissions(Missions loadedMissions)
	{
		if (this.missions.Count > 0)
		{
			for (int i = this.missions.Count - 1; i >= 0; i--)
			{
				BaseMission.MissionInstance missionInstance = this.missions[i];
				if (missionInstance != null)
				{
					Facepunch.Pool.Free<BaseMission.MissionInstance>(ref missionInstance);
				}
			}
		}
		this.missions.Clear();
		if (base.isServer && loadedMissions != null)
		{
			int protocol = loadedMissions.protocol;
			uint seed = loadedMissions.seed;
			int saveCreatedTime = loadedMissions.saveCreatedTime;
			int num = Epoch.FromDateTime(SaveRestore.SaveCreatedTime);
			if (238 != protocol || global::World.Seed != seed || num != saveCreatedTime)
			{
				Debug.Log("Missions were from old protocol or different seed, or not from a loaded save. Clearing");
				loadedMissions.activeMission = -1;
				this.SetActiveMission(-1);
				this.State.missions = this.SaveMissions();
				return;
			}
		}
		if (loadedMissions != null && loadedMissions.missions.Count > 0)
		{
			foreach (MissionInstance missionInstance2 in loadedMissions.missions)
			{
				BaseMission.MissionInstance missionInstance3 = Facepunch.Pool.Get<BaseMission.MissionInstance>();
				missionInstance3.providerID = missionInstance2.providerID;
				missionInstance3.missionID = missionInstance2.missionID;
				missionInstance3.status = (BaseMission.MissionStatus)missionInstance2.missionStatus;
				missionInstance3.completionScale = missionInstance2.completionScale;
				missionInstance3.startTime = UnityEngine.Time.realtimeSinceStartup - missionInstance2.startTime;
				missionInstance3.endTime = missionInstance2.endTime;
				missionInstance3.missionLocation = missionInstance2.missionLocation;
				if (missionInstance2.missionPoints != null)
				{
					foreach (ProtoBuf.MissionPoint missionPoint in missionInstance2.missionPoints)
					{
						missionInstance3.missionPoints.Add(missionPoint.identifier, missionPoint.location);
					}
				}
				missionInstance3.objectiveStatuses = new BaseMission.MissionInstance.ObjectiveStatus[missionInstance2.objectiveStatuses.Count];
				for (int j = 0; j < missionInstance2.objectiveStatuses.Count; j++)
				{
					ObjectiveStatus objectiveStatus = missionInstance2.objectiveStatuses[j];
					BaseMission.MissionInstance.ObjectiveStatus objectiveStatus2 = new BaseMission.MissionInstance.ObjectiveStatus();
					objectiveStatus2.completed = objectiveStatus.completed;
					objectiveStatus2.failed = objectiveStatus.failed;
					objectiveStatus2.started = objectiveStatus.started;
					objectiveStatus2.genericInt1 = objectiveStatus.genericInt1;
					objectiveStatus2.genericFloat1 = objectiveStatus.genericFloat1;
					missionInstance3.objectiveStatuses[j] = objectiveStatus2;
				}
				if (missionInstance2.createdEntities != null)
				{
					if (missionInstance3.createdEntities == null)
					{
						missionInstance3.createdEntities = Facepunch.Pool.GetList<MissionEntity>();
					}
					foreach (NetworkableId uid in missionInstance2.createdEntities)
					{
						global::BaseNetworkable baseNetworkable = null;
						if (base.isServer)
						{
							baseNetworkable = global::BaseNetworkable.serverEntities.Find(uid);
						}
						if (baseNetworkable != null)
						{
							MissionEntity component = baseNetworkable.GetComponent<MissionEntity>();
							if (component)
							{
								missionInstance3.createdEntities.Add(component);
							}
						}
					}
				}
				if (missionInstance2.rewards != null && missionInstance2.rewards.Count > 0)
				{
					missionInstance3.rewards = new ItemAmount[missionInstance2.rewards.Count];
					for (int k = 0; k < missionInstance2.rewards.Count; k++)
					{
						MissionReward missionReward = missionInstance2.rewards[k];
						ItemAmount itemAmount = new ItemAmount(null, 0f);
						ItemDefinition itemDefinition = ItemManager.FindItemDefinition(missionReward.itemID);
						if (itemDefinition == null)
						{
							Debug.LogError("MISSION LOAD UNABLE TO FIND REWARD ITEM, HUGE ERROR!");
						}
						itemAmount.itemDef = itemDefinition;
						itemAmount.amount = (float)missionReward.itemAmount;
						missionInstance3.rewards[k] = itemAmount;
					}
				}
				this.missions.Add(missionInstance3);
			}
			this.SetActiveMission(loadedMissions.activeMission);
			return;
		}
		this.SetActiveMission(-1);
	}

	// Token: 0x170000A0 RID: 160
	// (get) Token: 0x06000582 RID: 1410 RVA: 0x0003CEA8 File Offset: 0x0003B0A8
	// (set) Token: 0x06000583 RID: 1411 RVA: 0x0003CEB0 File Offset: 0x0003B0B0
	public ModelState modelStateTick { get; private set; }

	// Token: 0x06000584 RID: 1412 RVA: 0x0003CEB9 File Offset: 0x0003B0B9
	private void UpdateModelState()
	{
		if (this.IsDead())
		{
			return;
		}
		if (this.IsSpectating())
		{
			return;
		}
		this.wantsSendModelState = true;
	}

	// Token: 0x06000585 RID: 1413 RVA: 0x0003CED4 File Offset: 0x0003B0D4
	public void SendModelState(bool force = false)
	{
		if (!force)
		{
			if (!this.wantsSendModelState)
			{
				return;
			}
			if (this.nextModelStateUpdate > UnityEngine.Time.time)
			{
				return;
			}
		}
		this.wantsSendModelState = false;
		this.nextModelStateUpdate = UnityEngine.Time.time + 0.1f;
		if (this.IsDead())
		{
			return;
		}
		if (this.IsSpectating())
		{
			return;
		}
		this.modelState.sleeping = this.IsSleeping();
		this.modelState.mounted = this.isMounted;
		this.modelState.relaxed = this.IsRelaxed();
		this.modelState.onPhone = (this.HasActiveTelephone && !this.activeTelephone.IsMobile);
		this.modelState.crawling = this.IsCrawling();
		base.ClientRPC<ModelState>(null, "OnModelState", this.modelState);
	}

	// Token: 0x170000A1 RID: 161
	// (get) Token: 0x06000586 RID: 1414 RVA: 0x0003CF9F File Offset: 0x0003B19F
	public bool isMounted
	{
		get
		{
			return this.mounted.IsValid(base.isServer);
		}
	}

	// Token: 0x170000A2 RID: 162
	// (get) Token: 0x06000587 RID: 1415 RVA: 0x0003CFB2 File Offset: 0x0003B1B2
	public bool isMountingHidingWeapon
	{
		get
		{
			return this.isMounted && this.GetMounted().CanHoldItems();
		}
	}

	// Token: 0x06000588 RID: 1416 RVA: 0x0003CFC9 File Offset: 0x0003B1C9
	public BaseMountable GetMounted()
	{
		return this.mounted.Get(base.isServer) as BaseMountable;
	}

	// Token: 0x06000589 RID: 1417 RVA: 0x0003CFE4 File Offset: 0x0003B1E4
	public global::BaseVehicle GetMountedVehicle()
	{
		BaseMountable baseMountable = this.GetMounted();
		if (!baseMountable.IsValid())
		{
			return null;
		}
		return baseMountable.VehicleParent();
	}

	// Token: 0x0600058A RID: 1418 RVA: 0x0003D008 File Offset: 0x0003B208
	public void MarkSwapSeat()
	{
		this.nextSeatSwapTime = UnityEngine.Time.time + 0.75f;
	}

	// Token: 0x0600058B RID: 1419 RVA: 0x0003D01B File Offset: 0x0003B21B
	public bool SwapSeatCooldown()
	{
		return UnityEngine.Time.time < this.nextSeatSwapTime;
	}

	// Token: 0x0600058C RID: 1420 RVA: 0x0003D02A File Offset: 0x0003B22A
	public bool CanMountMountablesNow()
	{
		return !this.IsDead() && !this.IsWounded();
	}

	// Token: 0x0600058D RID: 1421 RVA: 0x0003D03F File Offset: 0x0003B23F
	public void MountObject(BaseMountable mount, int desiredSeat = 0)
	{
		this.mounted.Set(mount);
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x0600058E RID: 1422 RVA: 0x0003D054 File Offset: 0x0003B254
	public void EnsureDismounted()
	{
		if (this.isMounted)
		{
			this.GetMounted().DismountPlayer(this, false);
		}
	}

	// Token: 0x0600058F RID: 1423 RVA: 0x0003D06B File Offset: 0x0003B26B
	public virtual void DismountObject()
	{
		this.mounted.Set(null);
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
		this.PauseSpeedHackDetection(5f);
		this.PauseVehicleNoClipDetection(5f);
	}

	// Token: 0x06000590 RID: 1424 RVA: 0x0003D098 File Offset: 0x0003B298
	public void HandleMountedOnLoad()
	{
		if (this.mounted.IsValid(base.isServer))
		{
			BaseMountable baseMountable = this.mounted.Get(base.isServer) as BaseMountable;
			if (baseMountable != null)
			{
				baseMountable.MountPlayer(this);
				if (!baseMountable.allowSleeperMounting)
				{
					baseMountable.DismountPlayer(this, false);
					return;
				}
			}
			else
			{
				this.mounted.Set(null);
			}
		}
	}

	// Token: 0x06000591 RID: 1425 RVA: 0x0003D0FC File Offset: 0x0003B2FC
	public void ClearClientPetLink()
	{
		base.ClientRPCPlayer<int, int>(null, this, "CLIENT_SetPetPrefabID", 0, 0);
	}

	// Token: 0x06000592 RID: 1426 RVA: 0x0003D110 File Offset: 0x0003B310
	public void SendClientPetLink()
	{
		BasePet basePet;
		if (this.PetEntity == null && BasePet.ActivePetByOwnerID.TryGetValue(this.userID, out basePet) && basePet.Brain != null)
		{
			basePet.Brain.SetOwningPlayer(this);
		}
		base.ClientRPCPlayer<uint, NetworkableId>(null, this, "CLIENT_SetPetPrefabID", (this.PetEntity != null) ? this.PetEntity.prefabID : 0U, (this.PetEntity != null) ? this.PetEntity.net.ID : default(NetworkableId));
		if (this.PetEntity != null)
		{
			this.SendClientPetStateIndex();
		}
	}

	// Token: 0x06000593 RID: 1427 RVA: 0x0003D1C0 File Offset: 0x0003B3C0
	public void SendClientPetStateIndex()
	{
		BasePet basePet = this.PetEntity as BasePet;
		if (basePet == null)
		{
			return;
		}
		base.ClientRPCPlayer<int>(null, this, "CLIENT_SetPetPetLoadedStateIndex", basePet.Brain.LoadedDesignIndex());
	}

	// Token: 0x06000594 RID: 1428 RVA: 0x0003D1FB File Offset: 0x0003B3FB
	[global::BaseEntity.RPC_Server]
	private void IssuePetCommand(global::BaseEntity.RPCMessage msg)
	{
		this.ParsePetCommand(msg, false);
	}

	// Token: 0x06000595 RID: 1429 RVA: 0x0003D205 File Offset: 0x0003B405
	[global::BaseEntity.RPC_Server]
	private void IssuePetCommandRaycast(global::BaseEntity.RPCMessage msg)
	{
		this.ParsePetCommand(msg, true);
	}

	// Token: 0x06000596 RID: 1430 RVA: 0x0003D210 File Offset: 0x0003B410
	private void ParsePetCommand(global::BaseEntity.RPCMessage msg, bool raycast)
	{
		if (UnityEngine.Time.time - this.lastPetCommandIssuedTime <= 1f)
		{
			return;
		}
		this.lastPetCommandIssuedTime = UnityEngine.Time.time;
		if (msg.player == null)
		{
			return;
		}
		if (this.Pet == null)
		{
			return;
		}
		if (!this.Pet.IsOwnedBy(msg.player))
		{
			return;
		}
		int cmd = msg.read.Int32();
		int param = msg.read.Int32();
		if (raycast)
		{
			Ray value = msg.read.Ray();
			this.Pet.IssuePetCommand((PetCommandType)cmd, param, new Ray?(value));
			return;
		}
		this.Pet.IssuePetCommand((PetCommandType)cmd, param, null);
	}

	// Token: 0x06000597 RID: 1431 RVA: 0x0003D2BC File Offset: 0x0003B4BC
	public bool CanPing(bool disregardHeldEntity = false)
	{
		BaseGameMode activeGameMode = BaseGameMode.GetActiveGameMode(base.isServer);
		global::ComputerStation computerStation;
		return (!(activeGameMode != null) || activeGameMode.allowPings) && ((disregardHeldEntity || this.GetHeldEntity() is Binocular || (this.isMounted && (computerStation = (this.GetMounted() as global::ComputerStation)) != null && computerStation.AllowPings())) && this.IsAlive() && !this.IsWounded()) && !this.IsSpectating();
	}

	// Token: 0x06000598 RID: 1432 RVA: 0x0003D338 File Offset: 0x0003B538
	private global::BasePlayer.PingStyle GetPingStyle(global::BasePlayer.PingType t)
	{
		global::BasePlayer.PingStyle result = default(global::BasePlayer.PingStyle);
		switch (t)
		{
		case global::BasePlayer.PingType.Hostile:
			result = global::BasePlayer.HostileMarker;
			break;
		case global::BasePlayer.PingType.GoTo:
			result = global::BasePlayer.GoToMarker;
			break;
		case global::BasePlayer.PingType.Dollar:
			result = global::BasePlayer.DollarMarker;
			break;
		case global::BasePlayer.PingType.Loot:
			result = global::BasePlayer.LootMarker;
			break;
		case global::BasePlayer.PingType.Node:
			result = global::BasePlayer.NodeMarker;
			break;
		case global::BasePlayer.PingType.Gun:
			result = global::BasePlayer.GunMarker;
			break;
		}
		return result;
	}

	// Token: 0x06000599 RID: 1433 RVA: 0x0003D39C File Offset: 0x0003B59C
	private void ApplyPingStyle(MapNote note, global::BasePlayer.PingType type)
	{
		global::BasePlayer.PingStyle pingStyle = this.GetPingStyle(type);
		note.colourIndex = pingStyle.ColourIndex;
		note.icon = pingStyle.IconIndex;
	}

	// Token: 0x0600059A RID: 1434 RVA: 0x0003D3CC File Offset: 0x0003B5CC
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.FromOwner]
	[global::BaseEntity.RPC_Server.CallsPerSecond(3UL)]
	private void Server_AddPing(global::BaseEntity.RPCMessage msg)
	{
		if (this.State.pings == null)
		{
			this.State.pings = new List<MapNote>();
		}
		if (ConVar.Server.maximumPings == 0 || !this.CanPing(false))
		{
			return;
		}
		Vector3 vector = msg.read.Vector3();
		global::BasePlayer.PingType pingType = (global::BasePlayer.PingType)Mathf.Clamp(msg.read.Int32(), 0, 5);
		bool wasViaWheel = msg.read.Bit();
		global::BasePlayer.PingStyle pingStyle = this.GetPingStyle(pingType);
		foreach (MapNote mapNote in this.State.pings)
		{
			if (mapNote.icon == pingStyle.IconIndex && (mapNote.worldPosition - vector).sqrMagnitude < 0.75f)
			{
				return;
			}
		}
		if (this.State.pings.Count >= ConVar.Server.maximumPings)
		{
			this.State.pings.RemoveAt(0);
		}
		MapNote mapNote2 = Facepunch.Pool.Get<MapNote>();
		mapNote2.worldPosition = vector;
		mapNote2.isPing = true;
		mapNote2.timeRemaining = (mapNote2.totalDuration = ConVar.Server.pingDuration);
		this.ApplyPingStyle(mapNote2, pingType);
		this.State.pings.Add(mapNote2);
		this.DirtyPlayerState();
		this.SendPingsToClient();
		this.TeamUpdate(true);
		Analytics.Azure.OnPlayerPinged(this, pingType, wasViaWheel);
	}

	// Token: 0x0600059B RID: 1435 RVA: 0x0003D540 File Offset: 0x0003B740
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.FromOwner]
	[global::BaseEntity.RPC_Server.CallsPerSecond(3UL)]
	private void Server_RemovePing(global::BaseEntity.RPCMessage msg)
	{
		if (this.State.pings == null)
		{
			this.State.pings = new List<MapNote>();
		}
		int num = msg.read.Int32();
		if (num >= 0 && num < this.State.pings.Count)
		{
			this.State.pings.RemoveAt(num);
			this.DirtyPlayerState();
			this.SendPingsToClient();
			this.TeamUpdate(true);
		}
	}

	// Token: 0x0600059C RID: 1436 RVA: 0x0003D5B4 File Offset: 0x0003B7B4
	private void SendPingsToClient()
	{
		using (MapNoteList mapNoteList = Facepunch.Pool.Get<MapNoteList>())
		{
			mapNoteList.notes = Facepunch.Pool.GetList<MapNote>();
			mapNoteList.notes.AddRange(this.State.pings);
			base.ClientRPCPlayer<MapNoteList>(null, this, "Client_ReceivePings", mapNoteList);
			mapNoteList.notes.Clear();
		}
	}

	// Token: 0x0600059D RID: 1437 RVA: 0x0003D620 File Offset: 0x0003B820
	private void TickPings()
	{
		if (this.lastTick < 0.5f)
		{
			return;
		}
		TimeSince ts = this.lastTick;
		this.lastTick = 0f;
		if (this.State.pings == null)
		{
			return;
		}
		List<MapNote> list = Facepunch.Pool.GetList<MapNote>();
		foreach (MapNote mapNote in this.State.pings)
		{
			mapNote.timeRemaining -= ts;
			if (mapNote.timeRemaining <= 0f)
			{
				list.Add(mapNote);
			}
		}
		int count = list.Count;
		foreach (MapNote item in list)
		{
			if (this.State.pings.Contains(item))
			{
				this.State.pings.Remove(item);
			}
		}
		Facepunch.Pool.FreeList<MapNote>(ref list);
		if (count > 0)
		{
			this.DirtyPlayerState();
			this.SendPingsToClient();
			this.TeamUpdate(true);
		}
	}

	// Token: 0x170000A3 RID: 163
	// (get) Token: 0x0600059E RID: 1438 RVA: 0x0003D760 File Offset: 0x0003B960
	public PlayerState State
	{
		get
		{
			if (this.userID == 0UL)
			{
				throw new InvalidOperationException("Cannot get player state without a SteamID");
			}
			return SingletonComponent<ServerMgr>.Instance.playerStateManager.Get(this.userID);
		}
	}

	// Token: 0x170000A4 RID: 164
	// (get) Token: 0x0600059F RID: 1439 RVA: 0x0003D78A File Offset: 0x0003B98A
	public string WipeId
	{
		get
		{
			if (this._wipeId == null)
			{
				this._wipeId = SingletonComponent<ServerMgr>.Instance.persistance.GetUserWipeId(this.userID);
			}
			return this._wipeId;
		}
	}

	// Token: 0x060005A0 RID: 1440 RVA: 0x0003D7B5 File Offset: 0x0003B9B5
	public void DirtyPlayerState()
	{
		this._playerStateDirty = true;
	}

	// Token: 0x060005A1 RID: 1441 RVA: 0x0003D7BE File Offset: 0x0003B9BE
	public void SavePlayerState()
	{
		if (this._playerStateDirty)
		{
			this._playerStateDirty = false;
			SingletonComponent<ServerMgr>.Instance.playerStateManager.Save(this.userID);
		}
	}

	// Token: 0x060005A2 RID: 1442 RVA: 0x0003D7E4 File Offset: 0x0003B9E4
	public void ResetPlayerState()
	{
		SingletonComponent<ServerMgr>.Instance.playerStateManager.Reset(this.userID);
		base.ClientRPCPlayer<float>(null, this, "SetHostileLength", 0f);
		this.SendMarkersToClient();
		this.WipeMissions();
		this.MissionDirty(true);
	}

	// Token: 0x060005A3 RID: 1443 RVA: 0x0003D820 File Offset: 0x0003BA20
	public bool IsSleeping()
	{
		return this.HasPlayerFlag(global::BasePlayer.PlayerFlags.Sleeping);
	}

	// Token: 0x060005A4 RID: 1444 RVA: 0x0003D82A File Offset: 0x0003BA2A
	public bool IsSpectating()
	{
		return this.HasPlayerFlag(global::BasePlayer.PlayerFlags.Spectating);
	}

	// Token: 0x060005A5 RID: 1445 RVA: 0x0003D834 File Offset: 0x0003BA34
	public bool IsRelaxed()
	{
		return this.HasPlayerFlag(global::BasePlayer.PlayerFlags.Relaxed);
	}

	// Token: 0x060005A6 RID: 1446 RVA: 0x0003D841 File Offset: 0x0003BA41
	public bool IsServerFalling()
	{
		return this.HasPlayerFlag(global::BasePlayer.PlayerFlags.ServerFall);
	}

	// Token: 0x060005A7 RID: 1447 RVA: 0x0003D850 File Offset: 0x0003BA50
	public bool CanBuild()
	{
		BuildingPrivlidge buildingPrivilege = this.GetBuildingPrivilege();
		return buildingPrivilege == null || buildingPrivilege.IsAuthed(this);
	}

	// Token: 0x060005A8 RID: 1448 RVA: 0x0003D878 File Offset: 0x0003BA78
	public bool CanBuild(Vector3 position, Quaternion rotation, Bounds bounds)
	{
		BuildingPrivlidge buildingPrivilege = base.GetBuildingPrivilege(new OBB(position, rotation, bounds));
		return buildingPrivilege == null || buildingPrivilege.IsAuthed(this);
	}

	// Token: 0x060005A9 RID: 1449 RVA: 0x0003D8A8 File Offset: 0x0003BAA8
	public bool CanBuild(OBB obb)
	{
		BuildingPrivlidge buildingPrivilege = base.GetBuildingPrivilege(obb);
		return buildingPrivilege == null || buildingPrivilege.IsAuthed(this);
	}

	// Token: 0x060005AA RID: 1450 RVA: 0x0003D8D0 File Offset: 0x0003BAD0
	public bool IsBuildingBlocked()
	{
		BuildingPrivlidge buildingPrivilege = this.GetBuildingPrivilege();
		return !(buildingPrivilege == null) && !buildingPrivilege.IsAuthed(this);
	}

	// Token: 0x060005AB RID: 1451 RVA: 0x0003D8FC File Offset: 0x0003BAFC
	public bool IsBuildingBlocked(Vector3 position, Quaternion rotation, Bounds bounds)
	{
		BuildingPrivlidge buildingPrivilege = base.GetBuildingPrivilege(new OBB(position, rotation, bounds));
		return !(buildingPrivilege == null) && !buildingPrivilege.IsAuthed(this);
	}

	// Token: 0x060005AC RID: 1452 RVA: 0x0003D930 File Offset: 0x0003BB30
	public bool IsBuildingBlocked(OBB obb)
	{
		BuildingPrivlidge buildingPrivilege = base.GetBuildingPrivilege(obb);
		return !(buildingPrivilege == null) && !buildingPrivilege.IsAuthed(this);
	}

	// Token: 0x060005AD RID: 1453 RVA: 0x0003D95C File Offset: 0x0003BB5C
	public bool IsBuildingAuthed()
	{
		BuildingPrivlidge buildingPrivilege = this.GetBuildingPrivilege();
		return !(buildingPrivilege == null) && buildingPrivilege.IsAuthed(this);
	}

	// Token: 0x060005AE RID: 1454 RVA: 0x0003D984 File Offset: 0x0003BB84
	public bool IsBuildingAuthed(Vector3 position, Quaternion rotation, Bounds bounds)
	{
		BuildingPrivlidge buildingPrivilege = base.GetBuildingPrivilege(new OBB(position, rotation, bounds));
		return !(buildingPrivilege == null) && buildingPrivilege.IsAuthed(this);
	}

	// Token: 0x060005AF RID: 1455 RVA: 0x0003D9B4 File Offset: 0x0003BBB4
	public bool IsBuildingAuthed(OBB obb)
	{
		BuildingPrivlidge buildingPrivilege = base.GetBuildingPrivilege(obb);
		return !(buildingPrivilege == null) && buildingPrivilege.IsAuthed(this);
	}

	// Token: 0x060005B0 RID: 1456 RVA: 0x0003D9DB File Offset: 0x0003BBDB
	public bool CanPlaceBuildingPrivilege()
	{
		return this.GetBuildingPrivilege() == null;
	}

	// Token: 0x060005B1 RID: 1457 RVA: 0x0003D9E9 File Offset: 0x0003BBE9
	public bool CanPlaceBuildingPrivilege(Vector3 position, Quaternion rotation, Bounds bounds)
	{
		return base.GetBuildingPrivilege(new OBB(position, rotation, bounds)) == null;
	}

	// Token: 0x060005B2 RID: 1458 RVA: 0x0003D9FF File Offset: 0x0003BBFF
	public bool CanPlaceBuildingPrivilege(OBB obb)
	{
		return base.GetBuildingPrivilege(obb) == null;
	}

	// Token: 0x060005B3 RID: 1459 RVA: 0x0003DA10 File Offset: 0x0003BC10
	public bool IsNearEnemyBase()
	{
		BuildingPrivlidge buildingPrivilege = this.GetBuildingPrivilege();
		return !(buildingPrivilege == null) && !buildingPrivilege.IsAuthed(this) && buildingPrivilege.AnyAuthed();
	}

	// Token: 0x060005B4 RID: 1460 RVA: 0x0003DA40 File Offset: 0x0003BC40
	public bool IsNearEnemyBase(Vector3 position, Quaternion rotation, Bounds bounds)
	{
		BuildingPrivlidge buildingPrivilege = base.GetBuildingPrivilege(new OBB(position, rotation, bounds));
		return !(buildingPrivilege == null) && !buildingPrivilege.IsAuthed(this) && buildingPrivilege.AnyAuthed();
	}

	// Token: 0x060005B5 RID: 1461 RVA: 0x0003DA78 File Offset: 0x0003BC78
	public bool IsNearEnemyBase(OBB obb)
	{
		BuildingPrivlidge buildingPrivilege = base.GetBuildingPrivilege(obb);
		return !(buildingPrivilege == null) && !buildingPrivilege.IsAuthed(this) && buildingPrivilege.AnyAuthed();
	}

	// Token: 0x060005B6 RID: 1462 RVA: 0x0003DAAC File Offset: 0x0003BCAC
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.FromOwner]
	public void OnProjectileAttack(global::BaseEntity.RPCMessage msg)
	{
		PlayerProjectileAttack playerProjectileAttack = PlayerProjectileAttack.Deserialize(msg.read);
		if (playerProjectileAttack == null)
		{
			return;
		}
		PlayerAttack playerAttack = playerProjectileAttack.playerAttack;
		HitInfo hitInfo = new HitInfo();
		hitInfo.LoadFromAttack(playerAttack.attack, true);
		hitInfo.Initiator = this;
		hitInfo.ProjectileID = playerAttack.projectileID;
		hitInfo.ProjectileDistance = playerProjectileAttack.hitDistance;
		hitInfo.ProjectileVelocity = playerProjectileAttack.hitVelocity;
		hitInfo.Predicted = msg.connection;
		if (hitInfo.IsNaNOrInfinity() || float.IsNaN(playerProjectileAttack.travelTime) || float.IsInfinity(playerProjectileAttack.travelTime))
		{
			global::AntiHack.Log(this, AntiHackType.ProjectileHack, "Contains NaN (" + playerAttack.projectileID + ")");
			playerProjectileAttack.ResetToPool();
			this.stats.combat.LogInvalid(hitInfo, "projectile_nan");
			return;
		}
		global::BasePlayer.FiredProjectile firedProjectile;
		if (!this.firedProjectiles.TryGetValue(playerAttack.projectileID, out firedProjectile))
		{
			global::AntiHack.Log(this, AntiHackType.ProjectileHack, "Missing ID (" + playerAttack.projectileID + ")");
			playerProjectileAttack.ResetToPool();
			this.stats.combat.LogInvalid(hitInfo, "projectile_invalid");
			return;
		}
		hitInfo.ProjectileHits = firedProjectile.hits;
		hitInfo.ProjectileIntegrity = firedProjectile.integrity;
		hitInfo.ProjectileTravelTime = firedProjectile.travelTime;
		hitInfo.ProjectileTrajectoryMismatch = firedProjectile.trajectoryMismatch;
		if (firedProjectile.integrity <= 0f)
		{
			global::AntiHack.Log(this, AntiHackType.ProjectileHack, "Integrity is zero (" + playerAttack.projectileID + ")");
			Analytics.Azure.OnProjectileHackViolation(firedProjectile);
			playerProjectileAttack.ResetToPool();
			this.stats.combat.LogInvalid(hitInfo, "projectile_integrity");
			return;
		}
		if (firedProjectile.firedTime < UnityEngine.Time.realtimeSinceStartup - 8f)
		{
			global::AntiHack.Log(this, AntiHackType.ProjectileHack, "Lifetime is zero (" + playerAttack.projectileID + ")");
			Analytics.Azure.OnProjectileHackViolation(firedProjectile);
			playerProjectileAttack.ResetToPool();
			this.stats.combat.LogInvalid(hitInfo, "projectile_lifetime");
			return;
		}
		if (firedProjectile.ricochets > 0)
		{
			global::AntiHack.Log(this, AntiHackType.ProjectileHack, "Projectile is ricochet (" + playerAttack.projectileID + ")");
			Analytics.Azure.OnProjectileHackViolation(firedProjectile);
			playerProjectileAttack.ResetToPool();
			this.stats.combat.LogInvalid(hitInfo, "projectile_ricochet");
			return;
		}
		hitInfo.Weapon = firedProjectile.weaponSource;
		hitInfo.WeaponPrefab = firedProjectile.weaponPrefab;
		hitInfo.ProjectilePrefab = firedProjectile.projectilePrefab;
		hitInfo.damageProperties = firedProjectile.projectilePrefab.damageProperties;
		Vector3 position = firedProjectile.position;
		Vector3 velocity = firedProjectile.velocity;
		float partialTime = firedProjectile.partialTime;
		float travelTime = firedProjectile.travelTime;
		float num = Mathf.Clamp(playerProjectileAttack.travelTime, firedProjectile.travelTime, 8f);
		Vector3 gravity = UnityEngine.Physics.gravity * firedProjectile.projectilePrefab.gravityModifier;
		float drag = firedProjectile.projectilePrefab.drag;
		int layerMask = ConVar.AntiHack.projectile_terraincheck ? 10551296 : 2162688;
		global::BaseEntity hitEntity = hitInfo.HitEntity;
		global::BasePlayer basePlayer = hitEntity as global::BasePlayer;
		bool flag = basePlayer != null;
		bool flag2 = flag && basePlayer.IsSleeping();
		bool flag3 = flag && basePlayer.IsWounded();
		bool flag4 = flag && basePlayer.isMounted;
		bool flag5 = flag && basePlayer.HasParent();
		bool flag6 = hitEntity != null;
		bool flag7 = flag6 && hitEntity.IsNpc;
		bool flag8 = hitInfo.HitMaterial == Projectile.WaterMaterialID();
		if (firedProjectile.protection > 0)
		{
			bool flag9 = true;
			float num2 = 1f + ConVar.AntiHack.projectile_forgiveness;
			float num3 = 1f - ConVar.AntiHack.projectile_forgiveness;
			float projectile_clientframes = ConVar.AntiHack.projectile_clientframes;
			float projectile_serverframes = ConVar.AntiHack.projectile_serverframes;
			float num4 = Mathx.Decrement(firedProjectile.firedTime);
			float num5 = Mathf.Clamp(Mathx.Increment(UnityEngine.Time.realtimeSinceStartup) - num4, 0f, 8f);
			float num6 = num;
			float num7 = Mathf.Abs(num5 - num6);
			firedProjectile.desyncLifeTime = num7;
			float num8 = Mathf.Min(num5, num6);
			float num9 = projectile_clientframes / 60f;
			float num10 = projectile_serverframes * Mathx.Max(UnityEngine.Time.deltaTime, UnityEngine.Time.smoothDeltaTime, UnityEngine.Time.fixedDeltaTime);
			float num11 = (this.desyncTimeClamped + num8 + num9 + num10) * num2;
			float num12 = (firedProjectile.protection >= 6) ? ((this.desyncTimeClamped + num9 + num10) * num2) : num11;
			float num13 = (num5 - this.desyncTimeClamped - num9 - num10) * num3;
			float num14 = Vector3.Distance(firedProjectile.initialPosition, hitInfo.HitPositionWorld);
			if (flag && hitInfo.boneArea == (HitArea)(-1))
			{
				string name = hitInfo.ProjectilePrefab.name;
				string text = flag6 ? hitEntity.ShortPrefabName : "world";
				global::AntiHack.Log(this, AntiHackType.ProjectileHack, string.Concat(new object[]
				{
					"Bone is invalid (",
					name,
					" on ",
					text,
					" bone ",
					hitInfo.HitBone,
					")"
				}));
				this.stats.combat.LogInvalid(hitInfo, "projectile_bone");
				flag9 = false;
			}
			if (flag8)
			{
				if (flag6)
				{
					string name2 = hitInfo.ProjectilePrefab.name;
					string text2 = flag6 ? hitEntity.ShortPrefabName : "world";
					global::AntiHack.Log(this, AntiHackType.ProjectileHack, string.Concat(new string[]
					{
						"Projectile water hit on entity (",
						name2,
						" on ",
						text2,
						")"
					}));
					Analytics.Azure.OnProjectileHackViolation(firedProjectile);
					this.stats.combat.LogInvalid(hitInfo, "water_entity");
					flag9 = false;
				}
				if (!WaterLevel.Test(hitInfo.HitPositionWorld - 0.5f * Vector3.up, false, this))
				{
					string name3 = hitInfo.ProjectilePrefab.name;
					string text3 = flag6 ? hitEntity.ShortPrefabName : "world";
					global::AntiHack.Log(this, AntiHackType.ProjectileHack, string.Concat(new string[]
					{
						"Projectile water level (",
						name3,
						" on ",
						text3,
						")"
					}));
					Analytics.Azure.OnProjectileHackViolation(firedProjectile);
					this.stats.combat.LogInvalid(hitInfo, "water_level");
					flag9 = false;
				}
			}
			if (firedProjectile.protection >= 2)
			{
				if (flag6)
				{
					float num15 = hitEntity.MaxVelocity() + hitEntity.GetParentVelocity().magnitude;
					float num16 = hitEntity.BoundsPadding() + num12 * num15;
					float num17 = hitEntity.Distance(hitInfo.HitPositionWorld);
					if (num17 > num16)
					{
						string name4 = hitInfo.ProjectilePrefab.name;
						string shortPrefabName = hitEntity.ShortPrefabName;
						global::AntiHack.Log(this, AntiHackType.ProjectileHack, string.Concat(new object[]
						{
							"Entity too far away (",
							name4,
							" on ",
							shortPrefabName,
							" with ",
							num17,
							"m > ",
							num16,
							"m in ",
							num12,
							"s)"
						}));
						Analytics.Azure.OnProjectileHackViolation(firedProjectile);
						this.stats.combat.LogInvalid(hitInfo, "entity_distance");
						flag9 = false;
					}
				}
				if (firedProjectile.protection >= 6 && flag9 && flag && !flag7 && !flag2 && !flag3 && !flag4 && !flag5)
				{
					float magnitude = basePlayer.GetParentVelocity().magnitude;
					float num18 = basePlayer.BoundsPadding() + num12 * magnitude + ConVar.AntiHack.tickhistoryforgiveness;
					float num19 = basePlayer.tickHistory.Distance(basePlayer, hitInfo.HitPositionWorld);
					if (num19 > num18)
					{
						string name5 = hitInfo.ProjectilePrefab.name;
						string shortPrefabName2 = basePlayer.ShortPrefabName;
						global::AntiHack.Log(this, AntiHackType.ProjectileHack, string.Concat(new object[]
						{
							"Player too far away (",
							name5,
							" on ",
							shortPrefabName2,
							" with ",
							num19,
							"m > ",
							num18,
							"m in ",
							num12,
							"s)"
						}));
						Analytics.Azure.OnProjectileHackViolation(firedProjectile);
						this.stats.combat.LogInvalid(hitInfo, "player_distance");
						flag9 = false;
					}
				}
			}
			if (firedProjectile.protection >= 1)
			{
				float magnitude2 = firedProjectile.initialVelocity.magnitude;
				float num20 = hitInfo.ProjectilePrefab.initialDistance + num11 * magnitude2;
				float num21 = hitInfo.ProjectileDistance + 1f;
				if (num14 > num20)
				{
					string name6 = hitInfo.ProjectilePrefab.name;
					string text4 = flag6 ? hitEntity.ShortPrefabName : "world";
					global::AntiHack.Log(this, AntiHackType.ProjectileHack, string.Concat(new object[]
					{
						"Projectile too fast (",
						name6,
						" on ",
						text4,
						" with ",
						num14,
						"m > ",
						num20,
						"m in ",
						num11,
						"s)"
					}));
					Analytics.Azure.OnProjectileHackViolation(firedProjectile);
					this.stats.combat.LogInvalid(hitInfo, "projectile_maxspeed");
					flag9 = false;
				}
				if (num14 > num21)
				{
					string name7 = hitInfo.ProjectilePrefab.name;
					string text5 = flag6 ? hitEntity.ShortPrefabName : "world";
					global::AntiHack.Log(this, AntiHackType.ProjectileHack, string.Concat(new object[]
					{
						"Projectile too far away (",
						name7,
						" on ",
						text5,
						" with ",
						num14,
						"m > ",
						num21,
						"m in ",
						num11,
						"s)"
					}));
					Analytics.Azure.OnProjectileHackViolation(firedProjectile);
					this.stats.combat.LogInvalid(hitInfo, "projectile_distance");
					flag9 = false;
				}
				if (num7 > ConVar.AntiHack.projectile_desync)
				{
					string name8 = hitInfo.ProjectilePrefab.name;
					string text6 = flag6 ? hitEntity.ShortPrefabName : "world";
					global::AntiHack.Log(this, AntiHackType.ProjectileHack, string.Concat(new object[]
					{
						"Projectile desync (",
						name8,
						" on ",
						text6,
						" with ",
						num7,
						"s > ",
						ConVar.AntiHack.projectile_desync,
						"s)"
					}));
					Analytics.Azure.OnProjectileHackViolation(firedProjectile);
					this.stats.combat.LogInvalid(hitInfo, "projectile_desync");
					flag9 = false;
				}
			}
			if (firedProjectile.protection >= 4)
			{
				Vector3 a;
				Vector3 a2;
				this.SimulateProjectile(ref position, ref velocity, ref partialTime, num - travelTime, gravity, drag, out a, out a2);
				Vector3 b = a2 * 0.03125f;
				Line line = new Line(a - b, position + b);
				float num22 = line.Distance(hitInfo.PointStart);
				float num23 = line.Distance(hitInfo.HitPositionWorld);
				if (num22 > ConVar.AntiHack.projectile_trajectory)
				{
					string name9 = firedProjectile.projectilePrefab.name;
					string text7 = flag6 ? hitEntity.ShortPrefabName : "world";
					global::AntiHack.Log(this, AntiHackType.ProjectileHack, string.Concat(new object[]
					{
						"Start position trajectory (",
						name9,
						" on ",
						text7,
						" with ",
						num22,
						"m > ",
						ConVar.AntiHack.projectile_trajectory,
						"m)"
					}));
					Analytics.Azure.OnProjectileHackViolation(firedProjectile);
					this.stats.combat.LogInvalid(hitInfo, "trajectory_start");
					flag9 = false;
				}
				if (num23 > ConVar.AntiHack.projectile_trajectory)
				{
					string name10 = firedProjectile.projectilePrefab.name;
					string text8 = flag6 ? hitEntity.ShortPrefabName : "world";
					global::AntiHack.Log(this, AntiHackType.ProjectileHack, string.Concat(new object[]
					{
						"End position trajectory (",
						name10,
						" on ",
						text8,
						" with ",
						num23,
						"m > ",
						ConVar.AntiHack.projectile_trajectory,
						"m)"
					}));
					Analytics.Azure.OnProjectileHackViolation(firedProjectile);
					this.stats.combat.LogInvalid(hitInfo, "trajectory_end");
					flag9 = false;
				}
				hitInfo.ProjectileVelocity = velocity;
				if (playerProjectileAttack.hitVelocity != Vector3.zero && velocity != Vector3.zero)
				{
					float num24 = Vector3.Angle(playerProjectileAttack.hitVelocity, velocity);
					float num25 = playerProjectileAttack.hitVelocity.magnitude / velocity.magnitude;
					if (num24 > ConVar.AntiHack.projectile_anglechange)
					{
						string name11 = firedProjectile.projectilePrefab.name;
						string text9 = flag6 ? hitEntity.ShortPrefabName : "world";
						global::AntiHack.Log(this, AntiHackType.ProjectileHack, string.Concat(new object[]
						{
							"Trajectory angle change (",
							name11,
							" on ",
							text9,
							" with ",
							num24,
							"deg > ",
							ConVar.AntiHack.projectile_anglechange,
							"deg)"
						}));
						Analytics.Azure.OnProjectileHackViolation(firedProjectile);
						this.stats.combat.LogInvalid(hitInfo, "angle_change");
						flag9 = false;
					}
					if (num25 > ConVar.AntiHack.projectile_velocitychange)
					{
						string name12 = firedProjectile.projectilePrefab.name;
						string text10 = flag6 ? hitEntity.ShortPrefabName : "world";
						global::AntiHack.Log(this, AntiHackType.ProjectileHack, string.Concat(new object[]
						{
							"Trajectory velocity change (",
							name12,
							" on ",
							text10,
							" with ",
							num25,
							" > ",
							ConVar.AntiHack.projectile_velocitychange,
							")"
						}));
						Analytics.Azure.OnProjectileHackViolation(firedProjectile);
						this.stats.combat.LogInvalid(hitInfo, "velocity_change");
						flag9 = false;
					}
				}
				float magnitude3 = velocity.magnitude;
				float num26 = num13 * magnitude3;
				if (num14 < num26)
				{
					string name13 = hitInfo.ProjectilePrefab.name;
					string text11 = flag6 ? hitEntity.ShortPrefabName : "world";
					global::AntiHack.Log(this, AntiHackType.ProjectileHack, string.Concat(new object[]
					{
						"Projectile too slow (",
						name13,
						" on ",
						text11,
						" with ",
						num14,
						"m < ",
						num26,
						"m in ",
						num13,
						"s)"
					}));
					Analytics.Azure.OnProjectileHackViolation(firedProjectile);
					this.stats.combat.LogInvalid(hitInfo, "projectile_minspeed");
					flag9 = false;
				}
			}
			if (firedProjectile.protection >= 3)
			{
				Vector3 position2 = firedProjectile.position;
				Vector3 pointStart = hitInfo.PointStart;
				Vector3 vector = hitInfo.HitPositionWorld;
				if (!flag8)
				{
					vector -= hitInfo.ProjectileVelocity.normalized * 0.001f;
				}
				Vector3 vector2 = hitInfo.PositionOnRay(vector);
				Vector3 b2 = Vector3.zero;
				Vector3 b3 = Vector3.zero;
				if (ConVar.AntiHack.projectile_backtracking > 0f)
				{
					b2 = (pointStart - position2).normalized * ConVar.AntiHack.projectile_backtracking;
					b3 = (vector2 - pointStart).normalized * ConVar.AntiHack.projectile_backtracking;
				}
				bool flag10 = GamePhysics.LineOfSight(position2 - b2, pointStart + b2, layerMask, firedProjectile.lastEntityHit) && GamePhysics.LineOfSight(pointStart - b3, vector2, layerMask, firedProjectile.lastEntityHit) && GamePhysics.LineOfSight(vector2, vector, layerMask, firedProjectile.lastEntityHit);
				if (!flag10)
				{
					this.stats.Add("hit_" + (flag6 ? hitEntity.Categorize() : "world") + "_indirect_los", 1, global::Stats.Server);
				}
				else
				{
					this.stats.Add("hit_" + (flag6 ? hitEntity.Categorize() : "world") + "_direct_los", 1, global::Stats.Server);
				}
				if (!flag10)
				{
					string name14 = hitInfo.ProjectilePrefab.name;
					string text12 = flag6 ? hitEntity.ShortPrefabName : "world";
					global::AntiHack.Log(this, AntiHackType.ProjectileHack, string.Concat(new object[]
					{
						"Line of sight (",
						name14,
						" on ",
						text12,
						") ",
						position2,
						" ",
						pointStart,
						" ",
						vector2,
						" ",
						vector
					}));
					Analytics.Azure.OnProjectileHackViolation(firedProjectile);
					this.stats.combat.LogInvalid(hitInfo, "projectile_los");
					flag9 = false;
				}
				if (flag9 && flag && !flag7)
				{
					Vector3 hitPositionWorld = hitInfo.HitPositionWorld;
					Vector3 position3 = basePlayer.eyes.position;
					Vector3 vector3 = basePlayer.CenterPoint();
					float projectile_losforgiveness = ConVar.AntiHack.projectile_losforgiveness;
					bool flag11 = GamePhysics.LineOfSight(hitPositionWorld, position3, layerMask, 0f, projectile_losforgiveness, null) && GamePhysics.LineOfSight(position3, hitPositionWorld, layerMask, projectile_losforgiveness, 0f, null);
					if (!flag11)
					{
						flag11 = (GamePhysics.LineOfSight(hitPositionWorld, vector3, layerMask, 0f, projectile_losforgiveness, null) && GamePhysics.LineOfSight(vector3, hitPositionWorld, layerMask, projectile_losforgiveness, 0f, null));
					}
					if (!flag11)
					{
						string name15 = hitInfo.ProjectilePrefab.name;
						string text13 = flag6 ? hitEntity.ShortPrefabName : "world";
						global::AntiHack.Log(this, AntiHackType.ProjectileHack, string.Concat(new object[]
						{
							"Line of sight (",
							name15,
							" on ",
							text13,
							") ",
							hitPositionWorld,
							" ",
							position3,
							" or ",
							hitPositionWorld,
							" ",
							vector3
						}));
						Analytics.Azure.OnProjectileHackViolation(firedProjectile);
						this.stats.combat.LogInvalid(hitInfo, "projectile_los");
						flag9 = false;
					}
				}
			}
			if (!flag9)
			{
				global::AntiHack.AddViolation(this, AntiHackType.ProjectileHack, ConVar.AntiHack.projectile_penalty);
				playerProjectileAttack.ResetToPool();
				return;
			}
		}
		firedProjectile.position = hitInfo.HitPositionWorld;
		firedProjectile.velocity = playerProjectileAttack.hitVelocity;
		firedProjectile.travelTime = num;
		firedProjectile.partialTime = partialTime;
		firedProjectile.hits++;
		firedProjectile.lastEntityHit = hitEntity;
		hitInfo.ProjectilePrefab.CalculateDamage(hitInfo, firedProjectile.projectileModifier, firedProjectile.integrity);
		if (firedProjectile.integrity < 1f)
		{
			firedProjectile.integrity = 0f;
		}
		else if (flag8)
		{
			firedProjectile.integrity = Mathf.Clamp01(firedProjectile.integrity - 0.1f);
		}
		else if (hitInfo.ProjectilePrefab.penetrationPower <= 0f || !flag6)
		{
			firedProjectile.integrity = 0f;
		}
		else
		{
			float num27 = hitEntity.PenetrationResistance(hitInfo) / hitInfo.ProjectilePrefab.penetrationPower;
			firedProjectile.integrity = Mathf.Clamp01(firedProjectile.integrity - num27);
		}
		if (flag6)
		{
			this.stats.Add(firedProjectile.itemMod.category + "_hit_" + hitEntity.Categorize(), 1, global::Stats.Steam);
		}
		if (firedProjectile.integrity <= 0f)
		{
			if (firedProjectile.hits <= ConVar.AntiHack.projectile_impactspawndepth)
			{
				firedProjectile.itemMod.ServerProjectileHit(hitInfo);
			}
			if (hitInfo.ProjectilePrefab.remainInWorld)
			{
				this.CreateWorldProjectile(hitInfo, firedProjectile.itemDef, firedProjectile.itemMod, hitInfo.ProjectilePrefab, firedProjectile.pickupItem);
			}
		}
		this.firedProjectiles[playerAttack.projectileID] = firedProjectile;
		if (flag6)
		{
			if (firedProjectile.hits <= ConVar.AntiHack.projectile_damagedepth)
			{
				hitEntity.OnAttacked(hitInfo);
			}
			else
			{
				this.stats.combat.LogInvalid(hitInfo, "ricochet");
			}
		}
		hitInfo.DoHitEffects = hitInfo.ProjectilePrefab.doDefaultHitEffects;
		Effect.server.ImpactEffect(hitInfo);
		playerProjectileAttack.ResetToPool();
	}

	// Token: 0x060005B7 RID: 1463 RVA: 0x0003EE94 File Offset: 0x0003D094
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.FromOwner]
	public void OnProjectileRicochet(global::BaseEntity.RPCMessage msg)
	{
		PlayerProjectileRicochet playerProjectileRicochet = PlayerProjectileRicochet.Deserialize(msg.read);
		if (playerProjectileRicochet == null)
		{
			return;
		}
		if (playerProjectileRicochet.hitPosition.IsNaNOrInfinity() || playerProjectileRicochet.inVelocity.IsNaNOrInfinity() || playerProjectileRicochet.outVelocity.IsNaNOrInfinity() || playerProjectileRicochet.hitNormal.IsNaNOrInfinity() || float.IsNaN(playerProjectileRicochet.travelTime) || float.IsInfinity(playerProjectileRicochet.travelTime))
		{
			global::AntiHack.Log(this, AntiHackType.ProjectileHack, "Contains NaN (" + playerProjectileRicochet.projectileID + ")");
			playerProjectileRicochet.ResetToPool();
			return;
		}
		global::BasePlayer.FiredProjectile firedProjectile;
		if (!this.firedProjectiles.TryGetValue(playerProjectileRicochet.projectileID, out firedProjectile))
		{
			global::AntiHack.Log(this, AntiHackType.ProjectileHack, "Missing ID (" + playerProjectileRicochet.projectileID + ")");
			playerProjectileRicochet.ResetToPool();
			return;
		}
		if (firedProjectile.firedTime < UnityEngine.Time.realtimeSinceStartup - 8f)
		{
			global::AntiHack.Log(this, AntiHackType.ProjectileHack, "Lifetime is zero (" + playerProjectileRicochet.projectileID + ")");
			playerProjectileRicochet.ResetToPool();
			return;
		}
		firedProjectile.ricochets++;
		this.firedProjectiles[playerProjectileRicochet.projectileID] = firedProjectile;
		playerProjectileRicochet.ResetToPool();
	}

	// Token: 0x060005B8 RID: 1464 RVA: 0x0003EFD0 File Offset: 0x0003D1D0
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.FromOwner]
	public void OnProjectileUpdate(global::BaseEntity.RPCMessage msg)
	{
		PlayerProjectileUpdate playerProjectileUpdate = PlayerProjectileUpdate.Deserialize(msg.read);
		if (playerProjectileUpdate == null)
		{
			return;
		}
		if (playerProjectileUpdate.curPosition.IsNaNOrInfinity() || playerProjectileUpdate.curVelocity.IsNaNOrInfinity() || float.IsNaN(playerProjectileUpdate.travelTime) || float.IsInfinity(playerProjectileUpdate.travelTime))
		{
			global::AntiHack.Log(this, AntiHackType.ProjectileHack, "Contains NaN (" + playerProjectileUpdate.projectileID + ")");
			playerProjectileUpdate.ResetToPool();
			return;
		}
		global::BasePlayer.FiredProjectile firedProjectile;
		if (!this.firedProjectiles.TryGetValue(playerProjectileUpdate.projectileID, out firedProjectile))
		{
			global::AntiHack.Log(this, AntiHackType.ProjectileHack, "Missing ID (" + playerProjectileUpdate.projectileID + ")");
			playerProjectileUpdate.ResetToPool();
			return;
		}
		if (firedProjectile.firedTime < UnityEngine.Time.realtimeSinceStartup - 8f)
		{
			global::AntiHack.Log(this, AntiHackType.ProjectileHack, "Lifetime is zero (" + playerProjectileUpdate.projectileID + ")");
			Analytics.Azure.OnProjectileHackViolation(firedProjectile);
			playerProjectileUpdate.ResetToPool();
			return;
		}
		if (firedProjectile.ricochets > 0)
		{
			global::AntiHack.Log(this, AntiHackType.ProjectileHack, "Projectile is ricochet (" + playerProjectileUpdate.projectileID + ")");
			Analytics.Azure.OnProjectileHackViolation(firedProjectile);
			playerProjectileUpdate.ResetToPool();
			return;
		}
		Vector3 position = firedProjectile.position;
		Vector3 velocity = firedProjectile.velocity;
		float num = firedProjectile.trajectoryMismatch;
		float partialTime = firedProjectile.partialTime;
		float travelTime = firedProjectile.travelTime;
		float num2 = Mathf.Clamp(playerProjectileUpdate.travelTime, firedProjectile.travelTime, 8f);
		Vector3 gravity = UnityEngine.Physics.gravity * firedProjectile.projectilePrefab.gravityModifier;
		float drag = firedProjectile.projectilePrefab.drag;
		int layerMask = ConVar.AntiHack.projectile_terraincheck ? 10551296 : 2162688;
		if (firedProjectile.protection > 0)
		{
			float num3 = 1f + ConVar.AntiHack.projectile_forgiveness;
			float projectile_clientframes = ConVar.AntiHack.projectile_clientframes;
			float projectile_serverframes = ConVar.AntiHack.projectile_serverframes;
			float num4 = Mathx.Decrement(firedProjectile.firedTime);
			float num5 = Mathf.Clamp(Mathx.Increment(UnityEngine.Time.realtimeSinceStartup) - num4, 0f, 8f);
			float num6 = num2;
			float num7 = Mathf.Abs(num5 - num6);
			firedProjectile.desyncLifeTime = num7;
			float num8 = Mathf.Min(num5, num6);
			float num9 = projectile_clientframes / 60f;
			float num10 = projectile_serverframes * Mathx.Max(UnityEngine.Time.deltaTime, UnityEngine.Time.smoothDeltaTime, UnityEngine.Time.fixedDeltaTime);
			float num11 = (this.desyncTimeClamped + num8 + num9 + num10) * num3;
			if (firedProjectile.protection >= 1)
			{
				float magnitude = firedProjectile.initialVelocity.magnitude;
				float num12 = firedProjectile.projectilePrefab.initialDistance + num11 * magnitude;
				float num13 = Vector3.Distance(firedProjectile.initialPosition, playerProjectileUpdate.curPosition);
				if (num13 > num12)
				{
					string name = firedProjectile.projectilePrefab.name;
					global::AntiHack.Log(this, AntiHackType.ProjectileHack, string.Concat(new object[]
					{
						"Projectile too fast (",
						name,
						" with ",
						num13,
						"m > ",
						num12,
						"m in ",
						num11,
						"s)"
					}));
					Analytics.Azure.OnProjectileHackViolation(firedProjectile);
					playerProjectileUpdate.ResetToPool();
					return;
				}
				if (num7 > ConVar.AntiHack.projectile_desync)
				{
					string name2 = firedProjectile.projectilePrefab.name;
					global::AntiHack.Log(this, AntiHackType.ProjectileHack, string.Concat(new object[]
					{
						"Projectile desync (",
						name2,
						" with ",
						num7,
						"s > ",
						ConVar.AntiHack.projectile_desync,
						"s)"
					}));
					Analytics.Azure.OnProjectileHackViolation(firedProjectile);
					playerProjectileUpdate.ResetToPool();
					return;
				}
			}
			if (firedProjectile.protection >= 3)
			{
				Vector3 position2 = firedProjectile.position;
				Vector3 curPosition = playerProjectileUpdate.curPosition;
				Vector3 b = Vector3.zero;
				if (ConVar.AntiHack.projectile_backtracking > 0f)
				{
					b = (curPosition - position2).normalized * ConVar.AntiHack.projectile_backtracking;
				}
				if (!GamePhysics.LineOfSight(position2 - b, curPosition + b, layerMask, firedProjectile.lastEntityHit))
				{
					string name3 = firedProjectile.projectilePrefab.name;
					global::AntiHack.Log(this, AntiHackType.ProjectileHack, string.Concat(new object[]
					{
						"Line of sight (",
						name3,
						" on update) ",
						position2,
						" ",
						curPosition
					}));
					Analytics.Azure.OnProjectileHackViolation(firedProjectile);
					playerProjectileUpdate.ResetToPool();
					return;
				}
			}
			if (firedProjectile.protection >= 4)
			{
				Vector3 a;
				Vector3 a2;
				this.SimulateProjectile(ref position, ref velocity, ref partialTime, num2 - travelTime, gravity, drag, out a, out a2);
				Vector3 b2 = a2 * 0.03125f;
				Line line = new Line(a - b2, position + b2);
				num += line.Distance(playerProjectileUpdate.curPosition);
				if (num > ConVar.AntiHack.projectile_trajectory)
				{
					string name4 = firedProjectile.projectilePrefab.name;
					global::AntiHack.Log(this, AntiHackType.ProjectileHack, string.Concat(new object[]
					{
						"Update position trajectory (",
						name4,
						" on update with ",
						num,
						"m > ",
						ConVar.AntiHack.projectile_trajectory,
						"m)"
					}));
					Analytics.Azure.OnProjectileHackViolation(firedProjectile);
					playerProjectileUpdate.ResetToPool();
					return;
				}
			}
			if (firedProjectile.protection >= 5)
			{
				if (firedProjectile.inheritedVelocity != Vector3.zero)
				{
					Vector3 curVelocity = firedProjectile.inheritedVelocity + velocity;
					Vector3 curVelocity2 = playerProjectileUpdate.curVelocity;
					if (curVelocity2.magnitude > 2f * curVelocity.magnitude)
					{
						playerProjectileUpdate.curVelocity = curVelocity;
					}
					firedProjectile.inheritedVelocity = Vector3.zero;
				}
				else
				{
					playerProjectileUpdate.curVelocity = velocity;
				}
			}
		}
		firedProjectile.updates.Add(new global::BasePlayer.FiredProjectileUpdate
		{
			OldPosition = firedProjectile.position,
			NewPosition = playerProjectileUpdate.curPosition,
			OldVelocity = firedProjectile.velocity,
			NewVelocity = playerProjectileUpdate.curVelocity,
			Mismatch = num,
			PartialTime = partialTime
		});
		firedProjectile.position = playerProjectileUpdate.curPosition;
		firedProjectile.velocity = playerProjectileUpdate.curVelocity;
		firedProjectile.travelTime = playerProjectileUpdate.travelTime;
		firedProjectile.partialTime = partialTime;
		firedProjectile.trajectoryMismatch = num;
		this.firedProjectiles[playerProjectileUpdate.projectileID] = firedProjectile;
		playerProjectileUpdate.ResetToPool();
	}

	// Token: 0x060005B9 RID: 1465 RVA: 0x0003F5FC File Offset: 0x0003D7FC
	private void SimulateProjectile(ref Vector3 position, ref Vector3 velocity, ref float partialTime, float travelTime, Vector3 gravity, float drag, out Vector3 prevPosition, out Vector3 prevVelocity)
	{
		float num = 0.03125f;
		prevPosition = position;
		prevVelocity = velocity;
		if (partialTime > Mathf.Epsilon)
		{
			float num2 = num - partialTime;
			if (travelTime < num2)
			{
				prevPosition = position;
				prevVelocity = velocity;
				position += velocity * travelTime;
				partialTime += travelTime;
				return;
			}
			prevPosition = position;
			prevVelocity = velocity;
			position += velocity * num2;
			velocity += gravity * num;
			velocity -= velocity * drag * num;
			travelTime -= num2;
		}
		int num3 = Mathf.FloorToInt(travelTime / num);
		for (int i = 0; i < num3; i++)
		{
			prevPosition = position;
			prevVelocity = velocity;
			position += velocity * num;
			velocity += gravity * num;
			velocity -= velocity * drag * num;
		}
		partialTime = travelTime - num * (float)num3;
		if (partialTime > Mathf.Epsilon)
		{
			prevPosition = position;
			prevVelocity = velocity;
			position += velocity * partialTime;
		}
	}

	// Token: 0x060005BA RID: 1466 RVA: 0x0003F7D0 File Offset: 0x0003D9D0
	protected virtual void CreateWorldProjectile(HitInfo info, ItemDefinition itemDef, ItemModProjectile itemMod, Projectile projectilePrefab, global::Item recycleItem)
	{
		Vector3 projectileVelocity = info.ProjectileVelocity;
		global::Item item = (recycleItem != null) ? recycleItem : ItemManager.Create(itemDef, 1, 0UL);
		global::BaseEntity baseEntity;
		if (!info.DidHit)
		{
			baseEntity = item.CreateWorldObject(info.HitPositionWorld, Quaternion.LookRotation(projectileVelocity.normalized), null, 0U);
			baseEntity.Kill(global::BaseNetworkable.DestroyMode.Gib);
			return;
		}
		if (projectilePrefab.breakProbability > 0f && UnityEngine.Random.value <= projectilePrefab.breakProbability)
		{
			baseEntity = item.CreateWorldObject(info.HitPositionWorld, Quaternion.LookRotation(projectileVelocity.normalized), null, 0U);
			baseEntity.Kill(global::BaseNetworkable.DestroyMode.Gib);
			return;
		}
		if (projectilePrefab.conditionLoss > 0f)
		{
			item.LoseCondition(projectilePrefab.conditionLoss * 100f);
			if (item.isBroken)
			{
				baseEntity = item.CreateWorldObject(info.HitPositionWorld, Quaternion.LookRotation(projectileVelocity.normalized), null, 0U);
				baseEntity.Kill(global::BaseNetworkable.DestroyMode.Gib);
				return;
			}
		}
		if (projectilePrefab.stickProbability > 0f && UnityEngine.Random.value <= projectilePrefab.stickProbability)
		{
			if (info.HitEntity == null)
			{
				baseEntity = item.CreateWorldObject(info.HitPositionWorld, Quaternion.LookRotation(projectileVelocity.normalized), null, 0U);
			}
			else if (info.HitBone == 0U)
			{
				baseEntity = item.CreateWorldObject(info.HitPositionLocal, Quaternion.LookRotation(info.HitEntity.transform.InverseTransformDirection(projectileVelocity.normalized)), info.HitEntity, 0U);
			}
			else
			{
				baseEntity = item.CreateWorldObject(info.HitPositionLocal, Quaternion.LookRotation(info.HitNormalLocal * -1f), info.HitEntity, info.HitBone);
			}
			baseEntity.GetComponent<Rigidbody>().isKinematic = true;
			return;
		}
		baseEntity = item.CreateWorldObject(info.HitPositionWorld, Quaternion.LookRotation(projectileVelocity.normalized), null, 0U);
		Rigidbody component = baseEntity.GetComponent<Rigidbody>();
		component.AddForce(projectileVelocity.normalized * 200f);
		component.WakeUp();
	}

	// Token: 0x060005BB RID: 1467 RVA: 0x0003F9B0 File Offset: 0x0003DBB0
	public void CleanupExpiredProjectiles()
	{
		foreach (KeyValuePair<int, global::BasePlayer.FiredProjectile> keyValuePair in (from x in this.firedProjectiles
		where x.Value.firedTime < UnityEngine.Time.realtimeSinceStartup - 8f - 1f
		select x).ToList<KeyValuePair<int, global::BasePlayer.FiredProjectile>>())
		{
			Analytics.Azure.OnFiredProjectileRemoved(this, keyValuePair.Value);
			this.firedProjectiles.Remove(keyValuePair.Key);
			global::BasePlayer.FiredProjectile value = keyValuePair.Value;
			Facepunch.Pool.Free<global::BasePlayer.FiredProjectile>(ref value);
		}
	}

	// Token: 0x060005BC RID: 1468 RVA: 0x0003FA54 File Offset: 0x0003DC54
	public bool HasFiredProjectile(int id)
	{
		return this.firedProjectiles.ContainsKey(id);
	}

	// Token: 0x060005BD RID: 1469 RVA: 0x0003FA64 File Offset: 0x0003DC64
	public void NoteFiredProjectile(int projectileid, Vector3 startPos, Vector3 startVel, AttackEntity attackEnt, ItemDefinition firedItemDef, Guid projectileGroupId, global::Item pickupItem = null)
	{
		global::BaseProjectile baseProjectile = attackEnt as global::BaseProjectile;
		ItemModProjectile component = firedItemDef.GetComponent<ItemModProjectile>();
		Projectile component2 = component.projectileObject.Get().GetComponent<Projectile>();
		if (startPos.IsNaNOrInfinity() || startVel.IsNaNOrInfinity())
		{
			string name = component2.name;
			global::AntiHack.Log(this, AntiHackType.ProjectileHack, "Contains NaN (" + name + ")");
			this.stats.combat.LogInvalid(this, baseProjectile, "projectile_nan");
			return;
		}
		int projectile_protection = ConVar.AntiHack.projectile_protection;
		Vector3 inheritedVelocity = (attackEnt != null) ? attackEnt.GetInheritedVelocity(this, startVel.normalized) : Vector3.zero;
		if (projectile_protection >= 1)
		{
			float num = 1f - ConVar.AntiHack.projectile_forgiveness;
			float num2 = 1f + ConVar.AntiHack.projectile_forgiveness;
			float magnitude = startVel.magnitude;
			float num3 = component.GetMinVelocity();
			float num4 = component.GetMaxVelocity();
			global::BaseProjectile baseProjectile2 = attackEnt as global::BaseProjectile;
			if (baseProjectile2)
			{
				num3 *= baseProjectile2.GetProjectileVelocityScale(false);
				num4 *= baseProjectile2.GetProjectileVelocityScale(true);
			}
			num3 *= num;
			num4 *= num2;
			if (magnitude < num3)
			{
				string name2 = component2.name;
				global::AntiHack.Log(this, AntiHackType.ProjectileHack, string.Concat(new object[]
				{
					"Velocity (",
					name2,
					" with ",
					magnitude,
					" < ",
					num3,
					")"
				}));
				this.stats.combat.LogInvalid(this, baseProjectile, "projectile_minvelocity");
				return;
			}
			if (magnitude > num4)
			{
				string name3 = component2.name;
				global::AntiHack.Log(this, AntiHackType.ProjectileHack, string.Concat(new object[]
				{
					"Velocity (",
					name3,
					" with ",
					magnitude,
					" > ",
					num4,
					")"
				}));
				this.stats.combat.LogInvalid(this, baseProjectile, "projectile_maxvelocity");
				return;
			}
		}
		global::BasePlayer.FiredProjectile firedProjectile = Facepunch.Pool.Get<global::BasePlayer.FiredProjectile>();
		firedProjectile.itemDef = firedItemDef;
		firedProjectile.itemMod = component;
		firedProjectile.projectilePrefab = component2;
		firedProjectile.firedTime = UnityEngine.Time.realtimeSinceStartup;
		firedProjectile.travelTime = 0f;
		firedProjectile.weaponSource = attackEnt;
		firedProjectile.weaponPrefab = ((attackEnt == null) ? null : GameManager.server.FindPrefab(StringPool.Get(attackEnt.prefabID)).GetComponent<AttackEntity>());
		firedProjectile.projectileModifier = ((baseProjectile == null) ? Projectile.Modifier.Default : baseProjectile.GetProjectileModifier());
		firedProjectile.pickupItem = pickupItem;
		firedProjectile.integrity = 1f;
		firedProjectile.position = startPos;
		firedProjectile.velocity = startVel;
		firedProjectile.initialPosition = startPos;
		firedProjectile.initialVelocity = startVel;
		firedProjectile.inheritedVelocity = inheritedVelocity;
		firedProjectile.protection = projectile_protection;
		firedProjectile.ricochets = 0;
		firedProjectile.hits = 0;
		firedProjectile.id = projectileid;
		firedProjectile.attacker = this;
		this.firedProjectiles.Add(projectileid, firedProjectile);
		Analytics.Azure.OnFiredProjectile(this, firedProjectile, projectileGroupId);
	}

	// Token: 0x060005BE RID: 1470 RVA: 0x0003FD64 File Offset: 0x0003DF64
	public void ServerNoteFiredProjectile(int projectileid, Vector3 startPos, Vector3 startVel, AttackEntity attackEnt, ItemDefinition firedItemDef, global::Item pickupItem = null)
	{
		global::BaseProjectile baseProjectile = attackEnt as global::BaseProjectile;
		ItemModProjectile component = firedItemDef.GetComponent<ItemModProjectile>();
		Projectile component2 = component.projectileObject.Get().GetComponent<Projectile>();
		int protection = 0;
		Vector3 zero = Vector3.zero;
		if (startPos.IsNaNOrInfinity() || startVel.IsNaNOrInfinity())
		{
			return;
		}
		global::BasePlayer.FiredProjectile firedProjectile = Facepunch.Pool.Get<global::BasePlayer.FiredProjectile>();
		firedProjectile.itemDef = firedItemDef;
		firedProjectile.itemMod = component;
		firedProjectile.projectilePrefab = component2;
		firedProjectile.firedTime = UnityEngine.Time.realtimeSinceStartup;
		firedProjectile.travelTime = 0f;
		firedProjectile.weaponSource = attackEnt;
		firedProjectile.weaponPrefab = ((attackEnt == null) ? null : GameManager.server.FindPrefab(StringPool.Get(attackEnt.prefabID)).GetComponent<AttackEntity>());
		firedProjectile.projectileModifier = ((baseProjectile == null) ? Projectile.Modifier.Default : baseProjectile.GetProjectileModifier());
		firedProjectile.pickupItem = pickupItem;
		firedProjectile.integrity = 1f;
		firedProjectile.trajectoryMismatch = 0f;
		firedProjectile.position = startPos;
		firedProjectile.velocity = startVel;
		firedProjectile.initialPosition = startPos;
		firedProjectile.initialVelocity = startVel;
		firedProjectile.inheritedVelocity = zero;
		firedProjectile.protection = protection;
		firedProjectile.ricochets = 0;
		firedProjectile.hits = 0;
		firedProjectile.id = projectileid;
		firedProjectile.attacker = this;
		this.firedProjectiles.Add(projectileid, firedProjectile);
	}

	// Token: 0x060005BF RID: 1471 RVA: 0x0003FEB9 File Offset: 0x0003E0B9
	public override bool CanUseNetworkCache(Network.Connection connection)
	{
		return this.net == null || this.net.connection != connection;
	}

	// Token: 0x060005C0 RID: 1472 RVA: 0x0003FED6 File Offset: 0x0003E0D6
	public override void PostServerLoad()
	{
		base.PostServerLoad();
		this.HandleMountedOnLoad();
	}

	// Token: 0x060005C1 RID: 1473 RVA: 0x0003FEE4 File Offset: 0x0003E0E4
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		bool flag = this.net != null && this.net.connection == info.forConnection;
		info.msg.basePlayer = Facepunch.Pool.Get<ProtoBuf.BasePlayer>();
		info.msg.basePlayer.userid = this.userID;
		info.msg.basePlayer.name = this.displayName;
		info.msg.basePlayer.playerFlags = (int)this.playerFlags;
		info.msg.basePlayer.currentTeam = this.currentTeam;
		info.msg.basePlayer.heldEntity = this.svActiveItemID;
		info.msg.basePlayer.reputation = this.reputation;
		if (!info.forDisk && this.currentGesture != null && this.currentGesture.animationType == GestureConfig.AnimationType.Loop)
		{
			info.msg.basePlayer.loopingGesture = this.currentGesture.gestureId;
		}
		if (this.IsConnected && (this.IsAdmin || this.IsDeveloper))
		{
			info.msg.basePlayer.skinCol = this.net.connection.info.GetFloat("global.skincol", -1f);
			info.msg.basePlayer.skinTex = this.net.connection.info.GetFloat("global.skintex", -1f);
			info.msg.basePlayer.skinMesh = this.net.connection.info.GetFloat("global.skinmesh", -1f);
		}
		else
		{
			info.msg.basePlayer.skinCol = -1f;
			info.msg.basePlayer.skinTex = -1f;
			info.msg.basePlayer.skinMesh = -1f;
		}
		info.msg.basePlayer.underwear = this.GetUnderwearSkin();
		if (info.forDisk || flag)
		{
			info.msg.basePlayer.metabolism = this.metabolism.Save();
			info.msg.basePlayer.modifiers = null;
			if (this.modifiers != null)
			{
				info.msg.basePlayer.modifiers = this.modifiers.Save();
			}
		}
		if (!info.forDisk && !flag)
		{
			info.msg.basePlayer.playerFlags &= -5;
			info.msg.basePlayer.playerFlags &= -129;
		}
		info.msg.basePlayer.inventory = this.inventory.Save(info.forDisk || flag);
		this.modelState.sleeping = this.IsSleeping();
		this.modelState.relaxed = this.IsRelaxed();
		this.modelState.crawling = this.IsCrawling();
		info.msg.basePlayer.modelState = this.modelState.Copy();
		if (info.forDisk)
		{
			global::BaseEntity baseEntity = this.mounted.Get(base.isServer);
			if (baseEntity.IsValid())
			{
				if (baseEntity.enableSaving)
				{
					info.msg.basePlayer.mounted = this.mounted.uid;
				}
				else
				{
					global::BaseVehicle mountedVehicle = this.GetMountedVehicle();
					if (mountedVehicle.IsValid() && mountedVehicle.enableSaving)
					{
						info.msg.basePlayer.mounted = mountedVehicle.net.ID;
					}
				}
			}
			info.msg.basePlayer.respawnId = this.respawnId;
		}
		else
		{
			info.msg.basePlayer.mounted = this.mounted.uid;
		}
		if (flag)
		{
			info.msg.basePlayer.persistantData = this.PersistantPlayerInfo.Copy();
			if (!info.forDisk && this.State.missions != null)
			{
				info.msg.basePlayer.missions = this.State.missions.Copy();
			}
			info.msg.basePlayer.bagCount = global::SleepingBag.GetSleepingBagCount(this.userID);
		}
		if (info.forDisk)
		{
			info.msg.basePlayer.currentLife = this.lifeStory;
			info.msg.basePlayer.previousLife = this.previousLifeStory;
		}
		if (info.forDisk)
		{
			this.SavePlayerState();
		}
	}

	// Token: 0x060005C2 RID: 1474 RVA: 0x0004035C File Offset: 0x0003E55C
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.basePlayer != null)
		{
			ProtoBuf.BasePlayer basePlayer = info.msg.basePlayer;
			this.userID = basePlayer.userid;
			this.UserIDString = this.userID.ToString();
			if (basePlayer.name != null)
			{
				this.displayName = basePlayer.name;
			}
			this.playerFlags = (global::BasePlayer.PlayerFlags)basePlayer.playerFlags;
			this.currentTeam = basePlayer.currentTeam;
			this.reputation = basePlayer.reputation;
			if (basePlayer.metabolism != null)
			{
				this.metabolism.Load(basePlayer.metabolism);
			}
			if (basePlayer.modifiers != null && this.modifiers != null)
			{
				this.modifiers.Load(basePlayer.modifiers);
			}
			if (basePlayer.inventory != null)
			{
				this.inventory.Load(basePlayer.inventory);
			}
			if (basePlayer.modelState != null)
			{
				if (this.modelState != null)
				{
					this.modelState.ResetToPool();
					this.modelState = null;
				}
				this.modelState = basePlayer.modelState;
				basePlayer.modelState = null;
			}
		}
		if (info.fromDisk)
		{
			this.lifeStory = info.msg.basePlayer.currentLife;
			if (this.lifeStory != null)
			{
				this.lifeStory.ShouldPool = false;
			}
			this.previousLifeStory = info.msg.basePlayer.previousLife;
			if (this.previousLifeStory != null)
			{
				this.previousLifeStory.ShouldPool = false;
			}
			this.SetPlayerFlag(global::BasePlayer.PlayerFlags.Sleeping, false);
			this.StartSleeping();
			this.SetPlayerFlag(global::BasePlayer.PlayerFlags.Connected, false);
			if (this.lifeStory == null && this.IsAlive())
			{
				this.LifeStoryStart();
			}
			this.mounted.uid = info.msg.basePlayer.mounted;
			if (this.IsWounded())
			{
				this.Die(null);
			}
			this.respawnId = info.msg.basePlayer.respawnId;
		}
	}

	// Token: 0x170000A5 RID: 165
	// (get) Token: 0x060005C3 RID: 1475 RVA: 0x0004053F File Offset: 0x0003E73F
	public bool hasPreviousLife
	{
		get
		{
			return this.previousLifeStory != null;
		}
	}

	// Token: 0x170000A6 RID: 166
	// (get) Token: 0x060005C4 RID: 1476 RVA: 0x0004054A File Offset: 0x0003E74A
	// (set) Token: 0x060005C5 RID: 1477 RVA: 0x00040552 File Offset: 0x0003E752
	public int currentTimeCategory { get; private set; }

	// Token: 0x060005C6 RID: 1478 RVA: 0x0004055C File Offset: 0x0003E75C
	internal void LifeStoryStart()
	{
		if (this.lifeStory != null)
		{
			Debug.LogError("Stomping old lifeStory");
			this.lifeStory = null;
		}
		this.lifeStory = new PlayerLifeStory
		{
			ShouldPool = false
		};
		this.lifeStory.timeBorn = (uint)Epoch.Current;
		this.hasSentPresenceState = false;
	}

	// Token: 0x060005C7 RID: 1479 RVA: 0x000405AB File Offset: 0x0003E7AB
	internal void LifeStoryEnd()
	{
		SingletonComponent<ServerMgr>.Instance.persistance.AddLifeStory(this.userID, this.lifeStory);
		this.previousLifeStory = this.lifeStory;
		this.lifeStory = null;
	}

	// Token: 0x060005C8 RID: 1480 RVA: 0x000405DC File Offset: 0x0003E7DC
	internal void LifeStoryUpdate(float deltaTime, float moveSpeed)
	{
		if (this.lifeStory == null)
		{
			return;
		}
		this.lifeStory.secondsAlive += deltaTime;
		this.nextTimeCategoryUpdate -= deltaTime * ((moveSpeed > 0.1f) ? 1f : 0.25f);
		if (this.nextTimeCategoryUpdate <= 0f && !this.waitingForLifeStoryUpdate)
		{
			this.nextTimeCategoryUpdate = 7f + 7f * UnityEngine.Random.Range(0.2f, 1f);
			this.waitingForLifeStoryUpdate = true;
			global::BasePlayer.lifeStoryQueue.Add(this);
		}
		if (this.LifeStoryInWilderness)
		{
			this.lifeStory.secondsWilderness += deltaTime;
		}
		if (this.LifeStoryInMonument)
		{
			this.lifeStory.secondsInMonument += deltaTime;
		}
		if (this.LifeStoryInBase)
		{
			this.lifeStory.secondsInBase += deltaTime;
		}
		if (this.LifeStoryFlying)
		{
			this.lifeStory.secondsFlying += deltaTime;
		}
		if (this.LifeStoryBoating)
		{
			this.lifeStory.secondsBoating += deltaTime;
		}
		if (this.LifeStorySwimming)
		{
			this.lifeStory.secondsSwimming += deltaTime;
		}
		if (this.LifeStoryDriving)
		{
			this.lifeStory.secondsDriving += deltaTime;
		}
		if (this.IsSleeping())
		{
			this.lifeStory.secondsSleeping += deltaTime;
			return;
		}
		if (this.IsRunning())
		{
			this.lifeStory.metersRun += moveSpeed * deltaTime;
			return;
		}
		this.lifeStory.metersWalked += moveSpeed * deltaTime;
	}

	// Token: 0x060005C9 RID: 1481 RVA: 0x0004077C File Offset: 0x0003E97C
	public void UpdateTimeCategory()
	{
		using (TimeWarning.New("UpdateTimeCategory", 0))
		{
			this.waitingForLifeStoryUpdate = false;
			int currentTimeCategory = this.currentTimeCategory;
			this.currentTimeCategory = 1;
			if (this.IsBuildingAuthed())
			{
				this.currentTimeCategory = 4;
			}
			Vector3 position = base.transform.position;
			if (TerrainMeta.TopologyMap != null && (TerrainMeta.TopologyMap.GetTopology(position) & 1024) != 0)
			{
				foreach (MonumentInfo monumentInfo in TerrainMeta.Path.Monuments)
				{
					if (monumentInfo.shouldDisplayOnMap && monumentInfo.IsInBounds(position))
					{
						this.currentTimeCategory = 2;
						break;
					}
				}
			}
			if (this.IsSwimming())
			{
				this.currentTimeCategory |= 32;
			}
			BaseMountable baseMountable2;
			if (this.isMounted)
			{
				BaseMountable baseMountable = this.GetMounted();
				if (baseMountable.mountTimeStatType == BaseMountable.MountStatType.Boating)
				{
					this.currentTimeCategory |= 16;
				}
				else if (baseMountable.mountTimeStatType == BaseMountable.MountStatType.Flying)
				{
					this.currentTimeCategory |= 8;
				}
				else if (baseMountable.mountTimeStatType == BaseMountable.MountStatType.Driving)
				{
					this.currentTimeCategory |= 64;
				}
			}
			else if (base.HasParent() && (baseMountable2 = (base.GetParentEntity() as BaseMountable)) != null)
			{
				if (baseMountable2.mountTimeStatType == BaseMountable.MountStatType.Boating)
				{
					this.currentTimeCategory |= 16;
				}
				else if (baseMountable2.mountTimeStatType == BaseMountable.MountStatType.Flying)
				{
					this.currentTimeCategory |= 8;
				}
				else if (baseMountable2.mountTimeStatType == BaseMountable.MountStatType.Driving)
				{
					this.currentTimeCategory |= 64;
				}
			}
			if (currentTimeCategory != this.currentTimeCategory || !this.hasSentPresenceState)
			{
				this.LifeStoryInWilderness = ((1 & this.currentTimeCategory) != 0);
				this.LifeStoryInMonument = ((2 & this.currentTimeCategory) != 0);
				this.LifeStoryInBase = ((4 & this.currentTimeCategory) != 0);
				this.LifeStoryFlying = ((8 & this.currentTimeCategory) != 0);
				this.LifeStoryBoating = ((16 & this.currentTimeCategory) != 0);
				this.LifeStorySwimming = ((32 & this.currentTimeCategory) != 0);
				this.LifeStoryDriving = ((64 & this.currentTimeCategory) != 0);
				base.ClientRPCPlayer<int>(null, this, "UpdateRichPresenceState", this.currentTimeCategory);
				this.hasSentPresenceState = true;
			}
		}
	}

	// Token: 0x060005CA RID: 1482 RVA: 0x00040A00 File Offset: 0x0003EC00
	public void LifeStoryShotFired(global::BaseEntity withWeapon)
	{
		if (this.lifeStory == null)
		{
			return;
		}
		if (this.lifeStory.weaponStats == null)
		{
			this.lifeStory.weaponStats = Facepunch.Pool.GetList<PlayerLifeStory.WeaponStats>();
		}
		foreach (PlayerLifeStory.WeaponStats weaponStats in this.lifeStory.weaponStats)
		{
			if (weaponStats.weaponName == withWeapon.ShortPrefabName)
			{
				weaponStats.shotsFired += 1UL;
				return;
			}
		}
		PlayerLifeStory.WeaponStats weaponStats2 = Facepunch.Pool.Get<PlayerLifeStory.WeaponStats>();
		weaponStats2.weaponName = withWeapon.ShortPrefabName;
		weaponStats2.shotsFired += 1UL;
		this.lifeStory.weaponStats.Add(weaponStats2);
	}

	// Token: 0x060005CB RID: 1483 RVA: 0x00040AD0 File Offset: 0x0003ECD0
	public void LifeStoryShotHit(global::BaseEntity withWeapon)
	{
		if (this.lifeStory == null || withWeapon == null)
		{
			return;
		}
		if (this.lifeStory.weaponStats == null)
		{
			this.lifeStory.weaponStats = Facepunch.Pool.GetList<PlayerLifeStory.WeaponStats>();
		}
		foreach (PlayerLifeStory.WeaponStats weaponStats in this.lifeStory.weaponStats)
		{
			if (weaponStats.weaponName == withWeapon.ShortPrefabName)
			{
				weaponStats.shotsHit += 1UL;
				return;
			}
		}
		PlayerLifeStory.WeaponStats weaponStats2 = Facepunch.Pool.Get<PlayerLifeStory.WeaponStats>();
		weaponStats2.weaponName = withWeapon.ShortPrefabName;
		weaponStats2.shotsHit += 1UL;
		this.lifeStory.weaponStats.Add(weaponStats2);
	}

	// Token: 0x060005CC RID: 1484 RVA: 0x00040BA8 File Offset: 0x0003EDA8
	public void LifeStoryKill(BaseCombatEntity killed)
	{
		if (this.lifeStory == null)
		{
			return;
		}
		if (killed is ScientistNPC)
		{
			this.lifeStory.killedScientists++;
			return;
		}
		if (killed is global::BasePlayer)
		{
			this.lifeStory.killedPlayers++;
			return;
		}
		if (killed is BaseAnimalNPC)
		{
			this.lifeStory.killedAnimals++;
		}
	}

	// Token: 0x060005CD RID: 1485 RVA: 0x00040C14 File Offset: 0x0003EE14
	public void LifeStoryGenericStat(string key, int value)
	{
		if (this.lifeStory == null)
		{
			return;
		}
		if (this.lifeStory.genericStats == null)
		{
			this.lifeStory.genericStats = Facepunch.Pool.GetList<PlayerLifeStory.GenericStat>();
		}
		foreach (PlayerLifeStory.GenericStat genericStat in this.lifeStory.genericStats)
		{
			if (genericStat.key == key)
			{
				genericStat.value += value;
				return;
			}
		}
		PlayerLifeStory.GenericStat genericStat2 = Facepunch.Pool.Get<PlayerLifeStory.GenericStat>();
		genericStat2.key = key;
		genericStat2.value = value;
		this.lifeStory.genericStats.Add(genericStat2);
	}

	// Token: 0x060005CE RID: 1486 RVA: 0x00040CD0 File Offset: 0x0003EED0
	public void LifeStoryHurt(float amount)
	{
		if (this.lifeStory == null)
		{
			return;
		}
		this.lifeStory.totalDamageTaken += amount;
	}

	// Token: 0x060005CF RID: 1487 RVA: 0x00040CEE File Offset: 0x0003EEEE
	public void LifeStoryHeal(float amount)
	{
		if (this.lifeStory == null)
		{
			return;
		}
		this.lifeStory.totalHealing += amount;
	}

	// Token: 0x060005D0 RID: 1488 RVA: 0x00040D0C File Offset: 0x0003EF0C
	internal void LifeStoryLogDeath(HitInfo deathBlow, DamageType lastDamage)
	{
		if (this.lifeStory == null)
		{
			return;
		}
		this.lifeStory.timeDied = (uint)Epoch.Current;
		PlayerLifeStory.DeathInfo deathInfo = Facepunch.Pool.Get<PlayerLifeStory.DeathInfo>();
		deathInfo.lastDamageType = (int)lastDamage;
		if (deathBlow != null)
		{
			if (deathBlow.Initiator != null)
			{
				deathBlow.Initiator.AttackerInfo(deathInfo);
				deathInfo.attackerDistance = base.Distance(deathBlow.Initiator);
			}
			if (deathBlow.WeaponPrefab != null)
			{
				deathInfo.inflictorName = deathBlow.WeaponPrefab.ShortPrefabName;
			}
			if (deathBlow.HitBone > 0U)
			{
				deathInfo.hitBone = StringPool.Get(deathBlow.HitBone);
			}
			else
			{
				deathInfo.hitBone = "";
			}
		}
		else if (base.SecondsSinceAttacked <= 60f && this.lastAttacker != null)
		{
			this.lastAttacker.AttackerInfo(deathInfo);
		}
		this.lifeStory.deathInfo = deathInfo;
	}

	// Token: 0x170000A7 RID: 167
	// (get) Token: 0x060005D1 RID: 1489 RVA: 0x00040DE9 File Offset: 0x0003EFE9
	public virtual BaseNpc.AiStatistics.FamilyEnum Family
	{
		get
		{
			return BaseNpc.AiStatistics.FamilyEnum.Player;
		}
	}

	// Token: 0x170000A8 RID: 168
	// (get) Token: 0x060005D2 RID: 1490 RVA: 0x00040DED File Offset: 0x0003EFED
	protected override float PositionTickRate
	{
		get
		{
			return -1f;
		}
	}

	// Token: 0x170000A9 RID: 169
	// (get) Token: 0x060005D3 RID: 1491 RVA: 0x00040DF4 File Offset: 0x0003EFF4
	// (set) Token: 0x060005D4 RID: 1492 RVA: 0x00040DFC File Offset: 0x0003EFFC
	public int DebugMapMarkerIndex { get; set; }

	// Token: 0x170000AA RID: 170
	// (get) Token: 0x060005D5 RID: 1493 RVA: 0x00040E05 File Offset: 0x0003F005
	// (set) Token: 0x060005D6 RID: 1494 RVA: 0x00040E0D File Offset: 0x0003F00D
	public uint LastBlockColourChangeId { get; set; }

	// Token: 0x060005D7 RID: 1495 RVA: 0x00040E16 File Offset: 0x0003F016
	internal override void OnParentRemoved()
	{
		if (this.IsNpc)
		{
			base.OnParentRemoved();
			return;
		}
		base.SetParent(null, true, true);
	}

	// Token: 0x060005D8 RID: 1496 RVA: 0x00040E30 File Offset: 0x0003F030
	public override void OnParentChanging(global::BaseEntity oldParent, global::BaseEntity newParent)
	{
		if (oldParent != null)
		{
			this.TransformState(oldParent.transform.localToWorldMatrix);
		}
		if (newParent != null)
		{
			this.TransformState(newParent.transform.worldToLocalMatrix);
		}
	}

	// Token: 0x060005D9 RID: 1497 RVA: 0x00040E68 File Offset: 0x0003F068
	private void TransformState(Matrix4x4 matrix)
	{
		this.tickInterpolator.TransformEntries(matrix);
		this.tickHistory.TransformEntries(matrix);
		Vector3 euler = new Vector3(0f, matrix.rotation.eulerAngles.y, 0f);
		this.eyes.bodyRotation = Quaternion.Euler(euler) * this.eyes.bodyRotation;
	}

	// Token: 0x060005DA RID: 1498 RVA: 0x00040ED3 File Offset: 0x0003F0D3
	public bool CanSuicide()
	{
		return this.IsAdmin || this.IsDeveloper || UnityEngine.Time.realtimeSinceStartup > this.nextSuicideTime;
	}

	// Token: 0x060005DB RID: 1499 RVA: 0x00040EF4 File Offset: 0x0003F0F4
	public void MarkSuicide()
	{
		this.nextSuicideTime = UnityEngine.Time.realtimeSinceStartup + 60f;
	}

	// Token: 0x060005DC RID: 1500 RVA: 0x00040F07 File Offset: 0x0003F107
	public bool CanRespawn()
	{
		return UnityEngine.Time.realtimeSinceStartup > this.nextRespawnTime;
	}

	// Token: 0x060005DD RID: 1501 RVA: 0x00040F16 File Offset: 0x0003F116
	public void MarkRespawn(float nextSpawnDelay = 5f)
	{
		this.nextRespawnTime = UnityEngine.Time.realtimeSinceStartup + nextSpawnDelay;
	}

	// Token: 0x060005DE RID: 1502 RVA: 0x00040F28 File Offset: 0x0003F128
	public global::Item GetActiveItem()
	{
		if (!this.svActiveItemID.IsValid)
		{
			return null;
		}
		if (this.IsDead())
		{
			return null;
		}
		if (this.inventory == null || this.inventory.containerBelt == null)
		{
			return null;
		}
		return this.inventory.containerBelt.FindItemByUID(this.svActiveItemID);
	}

	// Token: 0x060005DF RID: 1503 RVA: 0x00040F84 File Offset: 0x0003F184
	public void MovePosition(Vector3 newPos)
	{
		base.transform.position = newPos;
		if (this.parentEntity.Get(base.isServer) != null)
		{
			this.tickInterpolator.Reset(this.parentEntity.Get(base.isServer).transform.InverseTransformPoint(newPos));
		}
		else
		{
			this.tickInterpolator.Reset(newPos);
		}
		this.ticksPerSecond.Increment();
		this.tickHistory.AddPoint(newPos, this.tickHistoryCapacity);
		base.NetworkPositionTick();
	}

	// Token: 0x170000AB RID: 171
	// (get) Token: 0x060005E0 RID: 1504 RVA: 0x0004100E File Offset: 0x0003F20E
	// (set) Token: 0x060005E1 RID: 1505 RVA: 0x00041016 File Offset: 0x0003F216
	public Vector3 estimatedVelocity { get; private set; }

	// Token: 0x170000AC RID: 172
	// (get) Token: 0x060005E2 RID: 1506 RVA: 0x0004101F File Offset: 0x0003F21F
	// (set) Token: 0x060005E3 RID: 1507 RVA: 0x00041027 File Offset: 0x0003F227
	public float estimatedSpeed { get; private set; }

	// Token: 0x170000AD RID: 173
	// (get) Token: 0x060005E4 RID: 1508 RVA: 0x00041030 File Offset: 0x0003F230
	// (set) Token: 0x060005E5 RID: 1509 RVA: 0x00041038 File Offset: 0x0003F238
	public float estimatedSpeed2D { get; private set; }

	// Token: 0x170000AE RID: 174
	// (get) Token: 0x060005E6 RID: 1510 RVA: 0x00041041 File Offset: 0x0003F241
	// (set) Token: 0x060005E7 RID: 1511 RVA: 0x00041049 File Offset: 0x0003F249
	public int secondsConnected { get; private set; }

	// Token: 0x170000AF RID: 175
	// (get) Token: 0x060005E8 RID: 1512 RVA: 0x00041052 File Offset: 0x0003F252
	// (set) Token: 0x060005E9 RID: 1513 RVA: 0x0004105A File Offset: 0x0003F25A
	public float desyncTimeRaw { get; private set; }

	// Token: 0x170000B0 RID: 176
	// (get) Token: 0x060005EA RID: 1514 RVA: 0x00041063 File Offset: 0x0003F263
	// (set) Token: 0x060005EB RID: 1515 RVA: 0x0004106B File Offset: 0x0003F26B
	public float desyncTimeClamped { get; private set; }

	// Token: 0x060005EC RID: 1516 RVA: 0x00041074 File Offset: 0x0003F274
	public void OverrideViewAngles(Vector3 newAng)
	{
		this.viewAngles = newAng;
	}

	// Token: 0x060005ED RID: 1517 RVA: 0x00041080 File Offset: 0x0003F280
	public override void ServerInit()
	{
		this.stats = new PlayerStatistics(this);
		if (this.userID == 0UL)
		{
			this.userID = (ulong)((long)UnityEngine.Random.Range(0, 10000000));
			this.UserIDString = this.userID.ToString();
			this.displayName = this.UserIDString;
			global::BasePlayer.bots.Add(this);
		}
		this.EnablePlayerCollider();
		this.SetPlayerRigidbodyState(!this.IsSleeping());
		base.ServerInit();
		global::BaseEntity.Query.Server.AddPlayer(this);
		this.inventory.ServerInit(this);
		this.metabolism.ServerInit(this);
		if (this.modifiers != null)
		{
			this.modifiers.ServerInit(this);
		}
		if (this.recentWaveTargets != null)
		{
			this.recentWaveTargets.Clear();
		}
	}

	// Token: 0x060005EE RID: 1518 RVA: 0x00041148 File Offset: 0x0003F348
	internal override void DoServerDestroy()
	{
		base.DoServerDestroy();
		global::BaseEntity.Query.Server.RemovePlayer(this);
		if (this.inventory)
		{
			this.inventory.DoDestroy();
		}
		global::BasePlayer.sleepingPlayerList.Remove(this);
		this.SavePlayerState();
		if (this.cachedPersistantPlayer != null)
		{
			Facepunch.Pool.Free<PersistantPlayer>(ref this.cachedPersistantPlayer);
		}
	}

	// Token: 0x060005EF RID: 1519 RVA: 0x000411A4 File Offset: 0x0003F3A4
	protected void ServerUpdate(float deltaTime)
	{
		if (!Network.Net.sv.IsConnected())
		{
			return;
		}
		this.LifeStoryUpdate(deltaTime, this.IsOnGround() ? this.estimatedSpeed : 0f);
		this.FinalizeTick(deltaTime);
		this.ThinkMissions(deltaTime);
		this.desyncTimeRaw = Mathf.Max(this.timeSinceLastTick - deltaTime, 0f);
		this.desyncTimeClamped = Mathf.Min(this.desyncTimeRaw, ConVar.AntiHack.maxdesync);
		if (this.clientTickRate != Player.tickrate_cl)
		{
			this.clientTickRate = Player.tickrate_cl;
			this.clientTickInterval = 1f / (float)this.clientTickRate;
			base.ClientRPCPlayer<int>(null, this, "UpdateClientTickRate", this.clientTickRate);
		}
		if (this.serverTickRate != Player.tickrate_sv)
		{
			this.serverTickRate = Player.tickrate_sv;
			this.serverTickInterval = 1f / (float)this.serverTickRate;
		}
		if (ConVar.AntiHack.terrain_protection > 0 && (long)(UnityEngine.Time.frameCount % ConVar.AntiHack.terrain_timeslice) == (long)((ulong)((uint)this.net.ID.Value) % (ulong)((long)ConVar.AntiHack.terrain_timeslice)) && !global::AntiHack.ShouldIgnore(this))
		{
			bool flag = false;
			if (global::AntiHack.IsInsideTerrain(this))
			{
				flag = true;
				global::AntiHack.AddViolation(this, AntiHackType.InsideTerrain, ConVar.AntiHack.terrain_penalty);
			}
			else if (ConVar.AntiHack.terrain_check_geometry && global::AntiHack.IsInsideMesh(this.eyes.position))
			{
				flag = true;
				global::AntiHack.AddViolation(this, AntiHackType.InsideGeometry, ConVar.AntiHack.terrain_penalty);
				global::AntiHack.Log(this, AntiHackType.InsideGeometry, "Seems to be clipped inside " + global::AntiHack.isInsideRayHit.collider.name);
			}
			if (flag && ConVar.AntiHack.terrain_kill)
			{
				Analytics.Azure.OnTerrainHackViolation(this);
				base.Hurt(1000f, DamageType.Suicide, this, false);
				return;
			}
		}
		if (UnityEngine.Time.realtimeSinceStartup < this.lastPlayerTick + this.serverTickInterval)
		{
			return;
		}
		if (this.lastPlayerTick < UnityEngine.Time.realtimeSinceStartup - this.serverTickInterval * 100f)
		{
			this.lastPlayerTick = UnityEngine.Time.realtimeSinceStartup - UnityEngine.Random.Range(0f, this.serverTickInterval);
		}
		while (this.lastPlayerTick < UnityEngine.Time.realtimeSinceStartup)
		{
			this.lastPlayerTick += this.serverTickInterval;
		}
		if (this.IsConnected)
		{
			this.ConnectedPlayerUpdate(this.serverTickInterval);
		}
		if (!this.IsNpc)
		{
			this.TickPings();
		}
	}

	// Token: 0x060005F0 RID: 1520 RVA: 0x000413D1 File Offset: 0x0003F5D1
	private void ServerUpdateBots(float deltaTime)
	{
		this.RefreshColliderSize(false);
	}

	// Token: 0x060005F1 RID: 1521 RVA: 0x000413DC File Offset: 0x0003F5DC
	private void ConnectedPlayerUpdate(float deltaTime)
	{
		if (this.IsReceivingSnapshot)
		{
			this.net.UpdateSubscriptions(int.MaxValue, int.MaxValue);
		}
		else if (UnityEngine.Time.realtimeSinceStartup > this.lastSubscriptionTick + ConVar.Server.entitybatchtime && this.net.UpdateSubscriptions(ConVar.Server.entitybatchsize * 2, ConVar.Server.entitybatchsize))
		{
			this.lastSubscriptionTick = UnityEngine.Time.realtimeSinceStartup;
		}
		this.SendEntityUpdate();
		if (this.IsReceivingSnapshot)
		{
			if (this.SnapshotQueue.Length == 0 && EACServer.IsAuthenticated(this.net.connection))
			{
				this.EnterGame();
			}
			return;
		}
		if (this.IsAlive())
		{
			this.metabolism.ServerUpdate(this, deltaTime);
			if (this.isMounted)
			{
				this.PauseVehicleNoClipDetection(1f);
			}
			if (this.modifiers != null && !this.IsReceivingSnapshot)
			{
				this.modifiers.ServerUpdate(this);
			}
			if (this.InSafeZone())
			{
				float num = 0f;
				global::HeldEntity heldEntity = this.GetHeldEntity();
				if (heldEntity && heldEntity.hostile)
				{
					num = deltaTime;
				}
				if (num == 0f)
				{
					this.MarkWeaponDrawnDuration(0f);
				}
				else
				{
					this.AddWeaponDrawnDuration(num);
				}
				if (this.weaponDrawnDuration >= 8f)
				{
					this.MarkHostileFor(30f);
				}
			}
			else
			{
				this.MarkWeaponDrawnDuration(0f);
			}
			if (this.timeSinceLastTick > (float)ConVar.Server.playertimeout)
			{
				this.lastTickTime = 0f;
				this.Kick("Unresponsive");
				return;
			}
		}
		int num2 = (int)this.net.connection.GetSecondsConnected();
		int num3 = num2 - this.secondsConnected;
		if (num3 > 0)
		{
			this.stats.Add("time", num3, global::Stats.Server);
			this.secondsConnected = num2;
		}
		this.RefreshColliderSize(false);
		this.SendModelState(false);
	}

	// Token: 0x060005F2 RID: 1522 RVA: 0x00041594 File Offset: 0x0003F794
	private void EnterGame()
	{
		this.SetPlayerFlag(global::BasePlayer.PlayerFlags.ReceivingSnapshot, false);
		base.ClientRPCPlayer(null, this, "FinishLoading");
		base.Invoke(new Action(this.DelayedTeamUpdate), 1f);
		this.LoadMissions(this.State.missions);
		this.MissionDirty(true);
		double num = this.State.unHostileTimestamp - TimeEx.currentTimestamp;
		if (num > 0.0)
		{
			base.ClientRPCPlayer<float>(null, this, "SetHostileLength", (float)num);
		}
		if (this.modifiers != null)
		{
			this.modifiers.ResetTicking();
		}
		if (this.net != null)
		{
			EACServer.OnFinishLoading(this.net.connection);
		}
		Debug.Log(string.Format("{0} has spawned", this));
		if ((Demo.recordlistmode == 0) ? Demo.recordlist.Contains(this.UserIDString) : (!Demo.recordlist.Contains(this.UserIDString)))
		{
			this.StartDemoRecording();
		}
		this.SendClientPetLink();
	}

	// Token: 0x060005F3 RID: 1523 RVA: 0x0004168D File Offset: 0x0003F88D
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.FromOwner]
	private void ClientKeepConnectionAlive(global::BaseEntity.RPCMessage msg)
	{
		this.lastTickTime = UnityEngine.Time.time;
	}

	// Token: 0x060005F4 RID: 1524 RVA: 0x000063A5 File Offset: 0x000045A5
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.FromOwner]
	private void ClientLoadingComplete(global::BaseEntity.RPCMessage msg)
	{
	}

	// Token: 0x060005F5 RID: 1525 RVA: 0x0004169C File Offset: 0x0003F89C
	public void PlayerInit(Network.Connection c)
	{
		using (TimeWarning.New("PlayerInit", 10))
		{
			base.CancelInvoke(new Action(base.KillMessage));
			this.SetPlayerFlag(global::BasePlayer.PlayerFlags.Connected, true);
			global::BasePlayer.activePlayerList.Add(this);
			global::BasePlayer.bots.Remove(this);
			this.userID = c.userid;
			this.UserIDString = this.userID.ToString();
			this.displayName = c.username;
			c.player = this;
			this.secondsConnected = 0;
			global::RelationshipManager.PlayerTeam playerTeam = global::RelationshipManager.ServerInstance.FindPlayersTeam(this.userID);
			this.currentTeam = ((playerTeam != null) ? playerTeam.teamID : 0UL);
			SingletonComponent<ServerMgr>.Instance.persistance.SetPlayerName(this.userID, this.displayName);
			this.tickInterpolator.Reset(base.transform.position);
			this.tickHistory.Reset(base.transform.position);
			this.eyeHistory.Clear();
			this.lastTickTime = 0f;
			this.lastInputTime = 0f;
			this.SetPlayerFlag(global::BasePlayer.PlayerFlags.ReceivingSnapshot, true);
			this.stats.Init();
			base.InvokeRandomized(new Action(this.StatSave), UnityEngine.Random.Range(5f, 10f), 30f, UnityEngine.Random.Range(0f, 6f));
			this.previousLifeStory = SingletonComponent<ServerMgr>.Instance.persistance.GetLastLifeStory(this.userID);
			this.SetPlayerFlag(global::BasePlayer.PlayerFlags.IsAdmin, c.authLevel > 0U);
			this.SetPlayerFlag(global::BasePlayer.PlayerFlags.IsDeveloper, DeveloperList.IsDeveloper(this));
			if (this.IsDead() && this.net.SwitchGroup(global::BaseNetworkable.LimboNetworkGroup))
			{
				base.SendNetworkGroupChange();
			}
			this.net.OnConnected(c);
			this.net.StartSubscriber();
			base.SendAsSnapshot(this.net.connection, false);
			base.ClientRPCPlayer(null, this, "StartLoading");
			if (BaseGameMode.GetActiveGameMode(true))
			{
				BaseGameMode.GetActiveGameMode(true).OnPlayerConnected(this);
			}
			if (this.net != null)
			{
				EACServer.OnStartLoading(this.net.connection);
			}
			if (this.IsAdmin)
			{
				if (ConVar.AntiHack.noclip_protection <= 0)
				{
					this.ChatMessage("antihack.noclip_protection is disabled!");
				}
				if (ConVar.AntiHack.speedhack_protection <= 0)
				{
					this.ChatMessage("antihack.speedhack_protection is disabled!");
				}
				if (ConVar.AntiHack.flyhack_protection <= 0)
				{
					this.ChatMessage("antihack.flyhack_protection is disabled!");
				}
				if (ConVar.AntiHack.projectile_protection <= 0)
				{
					this.ChatMessage("antihack.projectile_protection is disabled!");
				}
				if (ConVar.AntiHack.melee_protection <= 0)
				{
					this.ChatMessage("antihack.melee_protection is disabled!");
				}
				if (ConVar.AntiHack.eye_protection <= 0)
				{
					this.ChatMessage("antihack.eye_protection is disabled!");
				}
			}
		}
	}

	// Token: 0x060005F6 RID: 1526 RVA: 0x0004195C File Offset: 0x0003FB5C
	public void StatSave()
	{
		if (this.stats != null)
		{
			this.stats.Save(false);
		}
	}

	// Token: 0x060005F7 RID: 1527 RVA: 0x00041972 File Offset: 0x0003FB72
	public void SendDeathInformation()
	{
		base.ClientRPCPlayer(null, this, "OnDied");
	}

	// Token: 0x060005F8 RID: 1528 RVA: 0x00041984 File Offset: 0x0003FB84
	public void SendRespawnOptions()
	{
		using (RespawnInformation respawnInformation = Facepunch.Pool.Get<RespawnInformation>())
		{
			respawnInformation.spawnOptions = Facepunch.Pool.Get<List<RespawnInformation.SpawnOptions>>();
			foreach (global::SleepingBag sleepingBag in global::SleepingBag.FindForPlayer(this.userID, true))
			{
				RespawnInformation.SpawnOptions spawnOptions = Facepunch.Pool.Get<RespawnInformation.SpawnOptions>();
				spawnOptions.id = sleepingBag.net.ID;
				spawnOptions.name = sleepingBag.niceName;
				spawnOptions.worldPosition = sleepingBag.transform.position;
				spawnOptions.type = sleepingBag.RespawnType;
				spawnOptions.unlockSeconds = sleepingBag.GetUnlockSeconds(this.userID);
				spawnOptions.occupied = sleepingBag.IsOccupied();
				respawnInformation.spawnOptions.Add(spawnOptions);
			}
			if (this.IsDead())
			{
				respawnInformation.previousLife = this.previousLifeStory;
				respawnInformation.fadeIn = (this.previousLifeStory != null && (ulong)this.previousLifeStory.timeDied > (ulong)((long)(Epoch.Current - 5)));
			}
			base.ClientRPCPlayer<RespawnInformation>(null, this, "OnRespawnInformation", respawnInformation);
		}
	}

	// Token: 0x060005F9 RID: 1529 RVA: 0x00041A98 File Offset: 0x0003FC98
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.FromOwner]
	[global::BaseEntity.RPC_Server.CallsPerSecond(1UL)]
	private void RequestRespawnInformation(global::BaseEntity.RPCMessage msg)
	{
		this.SendRespawnOptions();
	}

	// Token: 0x170000B1 RID: 177
	// (get) Token: 0x060005FA RID: 1530 RVA: 0x00041AA0 File Offset: 0x0003FCA0
	public float secondsSleeping
	{
		get
		{
			if (this.sleepStartTime == -1f || !this.IsSleeping())
			{
				return 0f;
			}
			return UnityEngine.Time.time - this.sleepStartTime;
		}
	}

	// Token: 0x060005FB RID: 1531 RVA: 0x00003384 File Offset: 0x00001584
	public void ScheduledDeath()
	{
		base.Kill(global::BaseNetworkable.DestroyMode.None);
	}

	// Token: 0x060005FC RID: 1532 RVA: 0x00041ACC File Offset: 0x0003FCCC
	public virtual void StartSleeping()
	{
		if (this.IsSleeping())
		{
			return;
		}
		if (this.InSafeZone() && !base.IsInvoking(new Action(this.ScheduledDeath)))
		{
			base.Invoke(new Action(this.ScheduledDeath), NPCAutoTurret.sleeperhostiledelay);
		}
		BaseMountable baseMountable = this.GetMounted();
		if (baseMountable != null && !baseMountable.allowSleeperMounting)
		{
			this.EnsureDismounted();
		}
		this.SetPlayerFlag(global::BasePlayer.PlayerFlags.Sleeping, true);
		this.sleepStartTime = UnityEngine.Time.time;
		global::BasePlayer.sleepingPlayerList.Add(this);
		global::BasePlayer.bots.Remove(this);
		base.CancelInvoke(new Action(this.InventoryUpdate));
		base.CancelInvoke(new Action(this.TeamUpdate));
		this.inventory.loot.Clear();
		this.inventory.crafting.CancelAll(true);
		this.inventory.containerMain.OnChanged();
		this.inventory.containerBelt.OnChanged();
		this.inventory.containerWear.OnChanged();
		this.TurnOffAllLights();
		this.EnablePlayerCollider();
		this.RemovePlayerRigidbody();
		this.SetServerFall(true);
	}

	// Token: 0x060005FD RID: 1533 RVA: 0x00041BEC File Offset: 0x0003FDEC
	private void TurnOffAllLights()
	{
		this.LightToggle(false);
		global::HeldEntity heldEntity = this.GetHeldEntity();
		if (heldEntity != null)
		{
			TorchWeapon component = heldEntity.GetComponent<TorchWeapon>();
			if (component != null)
			{
				component.SetIsOn(false);
			}
		}
	}

	// Token: 0x060005FE RID: 1534 RVA: 0x00041C27 File Offset: 0x0003FE27
	private void OnPhysicsNeighbourChanged()
	{
		if (this.IsSleeping() || this.IsIncapacitated())
		{
			base.Invoke(new Action(this.DelayedServerFall), 0.05f);
		}
	}

	// Token: 0x060005FF RID: 1535 RVA: 0x00041C50 File Offset: 0x0003FE50
	private void DelayedServerFall()
	{
		this.SetServerFall(true);
	}

	// Token: 0x06000600 RID: 1536 RVA: 0x00041C5C File Offset: 0x0003FE5C
	public void SetServerFall(bool wantsOn)
	{
		if (wantsOn && ConVar.Server.playerserverfall)
		{
			if (!base.IsInvoking(new Action(this.ServerFall)))
			{
				this.SetPlayerFlag(global::BasePlayer.PlayerFlags.ServerFall, true);
				this.lastFallTime = UnityEngine.Time.time - this.fallTickRate;
				base.InvokeRandomized(new Action(this.ServerFall), 0f, this.fallTickRate, this.fallTickRate * 0.1f);
				this.fallVelocity = this.estimatedVelocity.y;
				return;
			}
		}
		else
		{
			base.CancelInvoke(new Action(this.ServerFall));
			this.SetPlayerFlag(global::BasePlayer.PlayerFlags.ServerFall, false);
		}
	}

	// Token: 0x06000601 RID: 1537 RVA: 0x00041D00 File Offset: 0x0003FF00
	public void ServerFall()
	{
		if (this.IsDead() || base.HasParent() || (!this.IsIncapacitated() && !this.IsSleeping()))
		{
			this.SetServerFall(false);
			return;
		}
		float num = UnityEngine.Time.time - this.lastFallTime;
		this.lastFallTime = UnityEngine.Time.time;
		float radius = this.GetRadius();
		float num2 = this.GetHeight(true) * 0.5f;
		float num3 = 2.5f;
		float num4 = 0.5f;
		this.fallVelocity += UnityEngine.Physics.gravity.y * num3 * num4 * num;
		float num5 = Mathf.Abs(this.fallVelocity * num);
		Vector3 origin = base.transform.position + Vector3.up * (radius + num2);
		Vector3 position = base.transform.position;
		Vector3 vector = base.transform.position;
		RaycastHit raycastHit;
		if (UnityEngine.Physics.SphereCast(origin, radius, Vector3.down, out raycastHit, num5 + num2, 1537286401, QueryTriggerInteraction.Ignore))
		{
			this.SetServerFall(false);
			if (raycastHit.distance > num2)
			{
				vector += Vector3.down * (raycastHit.distance - num2);
			}
			this.ApplyFallDamageFromVelocity(this.fallVelocity);
			this.UpdateEstimatedVelocity(vector, vector, num);
			this.fallVelocity = 0f;
		}
		else if (UnityEngine.Physics.Raycast(origin, Vector3.down, out raycastHit, num5 + radius + num2, 1537286401, QueryTriggerInteraction.Ignore))
		{
			this.SetServerFall(false);
			if (raycastHit.distance > num2 - radius)
			{
				vector += Vector3.down * (raycastHit.distance - num2 - radius);
			}
			this.ApplyFallDamageFromVelocity(this.fallVelocity);
			this.UpdateEstimatedVelocity(vector, vector, num);
			this.fallVelocity = 0f;
		}
		else
		{
			vector += Vector3.down * num5;
			this.UpdateEstimatedVelocity(position, vector, num);
			if (WaterLevel.Test(vector, true, this) || global::AntiHack.TestInsideTerrain(vector))
			{
				this.SetServerFall(false);
			}
		}
		this.MovePosition(vector);
	}

	// Token: 0x06000602 RID: 1538 RVA: 0x00041EF8 File Offset: 0x000400F8
	public void DelayedRigidbodyDisable()
	{
		this.RemovePlayerRigidbody();
	}

	// Token: 0x06000603 RID: 1539 RVA: 0x00041F00 File Offset: 0x00040100
	public virtual void EndSleeping()
	{
		if (!this.IsSleeping())
		{
			return;
		}
		this.SetPlayerFlag(global::BasePlayer.PlayerFlags.Sleeping, false);
		this.sleepStartTime = -1f;
		global::BasePlayer.sleepingPlayerList.Remove(this);
		if (this.userID < 10000000UL && !global::BasePlayer.bots.Contains(this))
		{
			global::BasePlayer.bots.Add(this);
		}
		base.CancelInvoke(new Action(this.ScheduledDeath));
		base.InvokeRepeating(new Action(this.InventoryUpdate), 1f, 0.1f * UnityEngine.Random.Range(0.99f, 1.01f));
		if (global::RelationshipManager.TeamsEnabled())
		{
			base.InvokeRandomized(new Action(this.TeamUpdate), 1f, 4f, 1f);
		}
		this.EnablePlayerCollider();
		this.AddPlayerRigidbody();
		this.SetServerFall(false);
		if (base.HasParent())
		{
			base.SetParent(null, true, false);
			base.ForceUpdateTriggers(true, true, true);
		}
		this.inventory.containerMain.OnChanged();
		this.inventory.containerBelt.OnChanged();
		this.inventory.containerWear.OnChanged();
		EACServer.LogPlayerSpawn(this);
	}

	// Token: 0x06000604 RID: 1540 RVA: 0x00042025 File Offset: 0x00040225
	public virtual void EndLooting()
	{
		if (this.inventory.loot)
		{
			this.inventory.loot.Clear();
		}
	}

	// Token: 0x06000605 RID: 1541 RVA: 0x0004204C File Offset: 0x0004024C
	public virtual void OnDisconnected()
	{
		this.stats.Save(true);
		this.EndLooting();
		this.ClearDesigningAIEntity();
		if (this.IsAlive() || this.IsSleeping())
		{
			this.StartSleeping();
		}
		else
		{
			base.Invoke(new Action(base.KillMessage), 0f);
		}
		global::BasePlayer.activePlayerList.Remove(this);
		this.SetPlayerFlag(global::BasePlayer.PlayerFlags.Connected, false);
		this.StopDemoRecording();
		if (this.net != null)
		{
			this.net.OnDisconnected();
		}
		this.ResetAntiHack();
		this.RefreshColliderSize(true);
		this.clientTickRate = 20;
		this.clientTickInterval = 0.05f;
		if (BaseGameMode.GetActiveGameMode(true))
		{
			BaseGameMode.GetActiveGameMode(true).OnPlayerDisconnected(this);
		}
		BaseMission.PlayerDisconnected(this);
	}

	// Token: 0x06000606 RID: 1542 RVA: 0x00042110 File Offset: 0x00040310
	private void InventoryUpdate()
	{
		if (this.IsConnected && !this.IsDead())
		{
			this.inventory.ServerUpdate(0.1f);
		}
	}

	// Token: 0x06000607 RID: 1543 RVA: 0x00042134 File Offset: 0x00040334
	public void ApplyFallDamageFromVelocity(float velocity)
	{
		float num = Mathf.InverseLerp(-15f, -100f, velocity);
		if (num == 0f)
		{
			return;
		}
		this.metabolism.bleeding.Add(num * 0.5f);
		float num2 = num * 500f;
		Analytics.Azure.OnFallDamage(this, velocity, num2);
		base.Hurt(num2, DamageType.Fall, null, true);
		if (num2 > 20f && this.fallDamageEffect.isValid)
		{
			Effect.server.Run(this.fallDamageEffect.resourcePath, base.transform.position, Vector3.zero, null, false);
		}
	}

	// Token: 0x06000608 RID: 1544 RVA: 0x000421C4 File Offset: 0x000403C4
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.FromOwner]
	private void OnPlayerLanded(global::BaseEntity.RPCMessage msg)
	{
		float num = msg.read.Float();
		if (float.IsNaN(num) || float.IsInfinity(num))
		{
			return;
		}
		this.ApplyFallDamageFromVelocity(num);
		this.fallVelocity = 0f;
	}

	// Token: 0x06000609 RID: 1545 RVA: 0x00042200 File Offset: 0x00040400
	public void SendGlobalSnapshot()
	{
		using (TimeWarning.New("SendGlobalSnapshot", 10))
		{
			this.EnterVisibility(Network.Net.sv.visibility.Get(0U));
		}
	}

	// Token: 0x0600060A RID: 1546 RVA: 0x0004224C File Offset: 0x0004044C
	public void SendFullSnapshot()
	{
		using (TimeWarning.New("SendFullSnapshot", 0))
		{
			foreach (Group group in this.net.subscriber.subscribed)
			{
				if (group.ID != 0U)
				{
					this.EnterVisibility(group);
				}
			}
		}
	}

	// Token: 0x0600060B RID: 1547 RVA: 0x000422D4 File Offset: 0x000404D4
	public override void OnNetworkGroupLeave(Group group)
	{
		base.OnNetworkGroupLeave(group);
		this.LeaveVisibility(group);
	}

	// Token: 0x0600060C RID: 1548 RVA: 0x000422E4 File Offset: 0x000404E4
	private void LeaveVisibility(Group group)
	{
		ServerMgr.OnLeaveVisibility(this.net.connection, group);
		this.ClearEntityQueue(group);
	}

	// Token: 0x0600060D RID: 1549 RVA: 0x000422FE File Offset: 0x000404FE
	public override void OnNetworkGroupEnter(Group group)
	{
		base.OnNetworkGroupEnter(group);
		this.EnterVisibility(group);
	}

	// Token: 0x0600060E RID: 1550 RVA: 0x0004230E File Offset: 0x0004050E
	private void EnterVisibility(Group group)
	{
		ServerMgr.OnEnterVisibility(this.net.connection, group);
		this.SendSnapshots(group.networkables);
	}

	// Token: 0x0600060F RID: 1551 RVA: 0x0004232D File Offset: 0x0004052D
	public void CheckDeathCondition(HitInfo info = null)
	{
		Assert.IsTrue(base.isServer, "CheckDeathCondition called on client!");
		if (this.IsSpectating())
		{
			return;
		}
		if (this.IsDead())
		{
			return;
		}
		if (this.metabolism.ShouldDie())
		{
			this.Die(info);
		}
	}

	// Token: 0x06000610 RID: 1552 RVA: 0x00042368 File Offset: 0x00040568
	public virtual BaseCorpse CreateCorpse()
	{
		using (TimeWarning.New("Create corpse", 0))
		{
			string strCorpsePrefab = "assets/prefabs/player/player_corpse.prefab";
			bool flag = false;
			if (ConVar.Global.cinematicGingerbreadCorpses)
			{
				foreach (global::Item item in this.inventory.containerWear.itemList)
				{
					ItemCorpseOverride itemCorpseOverride;
					if (item != null && item.info.TryGetComponent<ItemCorpseOverride>(out itemCorpseOverride))
					{
						strCorpsePrefab = ((global::BasePlayer.<CreateCorpse>g__GetFloatBasedOnUserID|387_0(this.userID, 4332UL) > 0.5f) ? itemCorpseOverride.FemaleCorpse.resourcePath : itemCorpseOverride.MaleCorpse.resourcePath);
						flag = itemCorpseOverride.BlockWearableCopy;
						break;
					}
				}
			}
			PlayerCorpse playerCorpse = base.DropCorpse(strCorpsePrefab) as PlayerCorpse;
			if (playerCorpse)
			{
				playerCorpse.SetFlag(global::BaseEntity.Flags.Reserved5, this.HasPlayerFlag(global::BasePlayer.PlayerFlags.DisplaySash), false, true);
				if (!flag)
				{
					playerCorpse.TakeFrom(new global::ItemContainer[]
					{
						this.inventory.containerMain,
						this.inventory.containerWear,
						this.inventory.containerBelt
					});
				}
				playerCorpse.playerName = this.displayName;
				playerCorpse.streamerName = RandomUsernames.Get(this.userID);
				playerCorpse.playerSteamID = this.userID;
				playerCorpse.underwearSkin = this.GetUnderwearSkin();
				playerCorpse.Spawn();
				playerCorpse.TakeChildren(this);
				ResourceDispenser component = playerCorpse.GetComponent<ResourceDispenser>();
				int num = 2;
				if (this.lifeStory != null)
				{
					num += Mathf.Clamp(Mathf.FloorToInt(this.lifeStory.secondsAlive / 180f), 0, 20);
				}
				component.containedItems.Add(new ItemAmount(ItemManager.FindItemDefinition("fat.animal"), (float)num));
				return playerCorpse;
			}
		}
		return null;
	}

	// Token: 0x06000611 RID: 1553 RVA: 0x00042564 File Offset: 0x00040764
	public override void OnKilled(HitInfo info)
	{
		this.SetPlayerFlag(global::BasePlayer.PlayerFlags.Unused2, false);
		this.SetPlayerFlag(global::BasePlayer.PlayerFlags.Unused1, false);
		this.EnsureDismounted();
		this.EndSleeping();
		this.EndLooting();
		this.stats.Add("deaths", 1, global::Stats.All);
		if (info != null && info.InitiatorPlayer != null && !info.InitiatorPlayer.IsNpc && !this.IsNpc)
		{
			global::RelationshipManager.ServerInstance.SetSeen(info.InitiatorPlayer, this);
			global::RelationshipManager.ServerInstance.SetSeen(this, info.InitiatorPlayer);
			global::RelationshipManager.ServerInstance.SetRelationship(this, info.InitiatorPlayer, global::RelationshipManager.RelationshipType.Enemy, 1, false);
		}
		if (BaseGameMode.GetActiveGameMode(true))
		{
			global::BasePlayer instigator = (info == null) ? null : info.InitiatorPlayer;
			BaseGameMode.GetActiveGameMode(true).OnPlayerDeath(instigator, this, info);
		}
		BaseMission.PlayerKilled(this);
		this.DisablePlayerCollider();
		this.RemovePlayerRigidbody();
		List<global::BasePlayer> list = Facepunch.Pool.GetList<global::BasePlayer>();
		if (this.IsIncapacitated())
		{
			foreach (global::BasePlayer basePlayer in global::BasePlayer.activePlayerList)
			{
				if (basePlayer != null && basePlayer.inventory != null && basePlayer.inventory.loot != null && basePlayer.inventory.loot.entitySource == this)
				{
					list.Add(basePlayer);
				}
			}
		}
		bool flag = this.IsWounded();
		this.StopWounded(null);
		if (this.inventory.crafting != null)
		{
			this.inventory.crafting.CancelAll(true);
		}
		EACServer.LogPlayerDespawn(this);
		BaseCorpse baseCorpse = this.CreateCorpse();
		if (baseCorpse != null)
		{
			if (info != null)
			{
				Rigidbody component = baseCorpse.GetComponent<Rigidbody>();
				if (component != null)
				{
					component.AddForce((info.attackNormal + Vector3.up * 0.5f).normalized * 1f, ForceMode.VelocityChange);
				}
			}
			PlayerCorpse playerCorpse;
			if ((playerCorpse = (baseCorpse as PlayerCorpse)) != null && playerCorpse.containers != null)
			{
				foreach (global::BasePlayer basePlayer2 in list)
				{
					if (!(basePlayer2 == null))
					{
						basePlayer2.inventory.loot.StartLootingEntity(playerCorpse, true);
						foreach (global::ItemContainer itemContainer in playerCorpse.containers)
						{
							if (itemContainer != null)
							{
								basePlayer2.inventory.loot.AddContainer(itemContainer);
							}
						}
						basePlayer2.inventory.loot.SendImmediate();
					}
				}
			}
		}
		Facepunch.Pool.FreeList<global::BasePlayer>(ref list);
		this.inventory.Strip();
		if (flag && this.lastDamage == DamageType.Suicide && this.cachedNonSuicideHitInfo != null)
		{
			info = this.cachedNonSuicideHitInfo;
			this.lastDamage = info.damageTypes.GetMajorityDamageType();
		}
		this.cachedNonSuicideHitInfo = null;
		if (this.lastDamage == DamageType.Fall)
		{
			this.stats.Add("death_fall", 1, global::Stats.Steam);
		}
		string message;
		string msg;
		if (info != null)
		{
			if (info.Initiator)
			{
				if (info.Initiator == this)
				{
					message = string.Concat(new object[]
					{
						this.ToString(),
						" was killed by ",
						this.lastDamage,
						" at ",
						base.transform.position
					});
					msg = "You died: killed by " + this.lastDamage;
					if (this.lastDamage == DamageType.Suicide)
					{
						Analytics.Server.Death("suicide", this.ServerPosition, Analytics.Server.DeathType.Player);
						this.stats.Add("death_suicide", 1, global::Stats.All);
					}
					else
					{
						Analytics.Server.Death("selfinflicted", this.ServerPosition, Analytics.Server.DeathType.Player);
						this.stats.Add("death_selfinflicted", 1, global::Stats.Steam);
					}
				}
				else
				{
					if (info.Initiator is global::BasePlayer)
					{
						global::BasePlayer basePlayer3 = info.Initiator.ToPlayer();
						message = string.Concat(new object[]
						{
							this.ToString(),
							" was killed by ",
							basePlayer3.ToString(),
							" at ",
							base.transform.position
						});
						msg = string.Concat(new object[]
						{
							"You died: killed by ",
							basePlayer3.displayName,
							" (",
							basePlayer3.userID,
							")"
						});
						basePlayer3.stats.Add("kill_player", 1, global::Stats.All);
						basePlayer3.LifeStoryKill(this);
						this.OnKilledByPlayer(basePlayer3);
						if (this.lastDamage == DamageType.Fun_Water)
						{
							basePlayer3.GiveAchievement("SUMMER_LIQUIDATOR");
							LiquidWeapon liquidWeapon = basePlayer3.GetHeldEntity() as LiquidWeapon;
							if (liquidWeapon != null && liquidWeapon.RequiresPumping && liquidWeapon.PressureFraction <= liquidWeapon.MinimumPressureFraction)
							{
								basePlayer3.GiveAchievement("SUMMER_NO_PRESSURE");
							}
						}
						else if (GameInfo.HasAchievements && this.lastDamage == DamageType.Explosion && info.WeaponPrefab != null && info.WeaponPrefab.ShortPrefabName.Contains("mlrs") && basePlayer3 != null)
						{
							basePlayer3.stats.Add("mlrs_kills", 1, global::Stats.All);
							basePlayer3.stats.Save(true);
						}
					}
					else
					{
						message = string.Concat(new object[]
						{
							this.ToString(),
							" was killed by ",
							info.Initiator.ShortPrefabName,
							" (",
							info.Initiator.Categorize(),
							") at ",
							base.transform.position
						});
						msg = "You died: killed by " + info.Initiator.Categorize();
						this.stats.Add("death_" + info.Initiator.Categorize(), 1, global::Stats.Steam);
					}
					if (!this.IsNpc)
					{
						Analytics.Server.Death(info.Initiator, info.WeaponPrefab, this.ServerPosition);
					}
				}
			}
			else if (this.lastDamage == DamageType.Fall)
			{
				message = this.ToString() + " was killed by fall at " + base.transform.position;
				msg = "You died: killed by fall";
				Analytics.Server.Death("fall", this.ServerPosition, Analytics.Server.DeathType.Player);
			}
			else
			{
				message = string.Concat(new object[]
				{
					this.ToString(),
					" was killed by ",
					info.damageTypes.GetMajorityDamageType().ToString(),
					" at ",
					base.transform.position
				});
				msg = "You died: " + info.damageTypes.GetMajorityDamageType().ToString();
			}
		}
		else
		{
			message = string.Concat(new object[]
			{
				this.ToString(),
				" died (",
				this.lastDamage,
				")"
			});
			msg = "You died: " + this.lastDamage.ToString();
		}
		using (TimeWarning.New("LogMessage", 0))
		{
			DebugEx.Log(message, StackTraceLogType.None);
			this.ConsoleMessage(msg);
		}
		if (this.net.connection == null && ((info != null) ? info.Initiator : null) != null && info.Initiator != this)
		{
			CompanionServer.Util.SendDeathNotification(this, info.Initiator);
		}
		base.SendNetworkUpdateImmediate(false);
		this.LifeStoryLogDeath(info, this.lastDamage);
		this.Server_LogDeathMarker(base.transform.position);
		this.LifeStoryEnd();
		this.LastBlockColourChangeId = 0U;
		if (this.net.connection == null)
		{
			base.Invoke(new Action(base.KillMessage), 0f);
			return;
		}
		this.SendRespawnOptions();
		this.SendDeathInformation();
		this.stats.Save(false);
	}

	// Token: 0x06000612 RID: 1554 RVA: 0x00042DBC File Offset: 0x00040FBC
	public void RespawnAt(Vector3 position, Quaternion rotation, global::BaseEntity spawnPointEntity = null)
	{
		BaseGameMode activeGameMode = BaseGameMode.GetActiveGameMode(true);
		if (activeGameMode && !activeGameMode.CanPlayerRespawn(this))
		{
			return;
		}
		this.SetPlayerFlag(global::BasePlayer.PlayerFlags.Wounded, false);
		this.SetPlayerFlag(global::BasePlayer.PlayerFlags.Unused2, false);
		this.SetPlayerFlag(global::BasePlayer.PlayerFlags.Unused1, false);
		this.SetPlayerFlag(global::BasePlayer.PlayerFlags.ReceivingSnapshot, true);
		this.SetPlayerFlag(global::BasePlayer.PlayerFlags.DisplaySash, false);
		this.respawnId = Guid.NewGuid().ToString("N");
		ServerPerformance.spawns += 1UL;
		base.SetParent(null, true, false);
		base.transform.SetPositionAndRotation(position, rotation);
		this.tickInterpolator.Reset(position);
		this.tickHistory.Reset(position);
		this.eyeHistory.Clear();
		this.estimatedVelocity = Vector3.zero;
		this.estimatedSpeed = 0f;
		this.estimatedSpeed2D = 0f;
		this.lastTickTime = 0f;
		this.StopWounded(null);
		this.ResetWoundingVars();
		this.StopSpectating();
		this.UpdateNetworkGroup();
		this.EnablePlayerCollider();
		this.RemovePlayerRigidbody();
		this.StartSleeping();
		this.LifeStoryStart();
		this.metabolism.Reset();
		if (this.modifiers != null)
		{
			this.modifiers.RemoveAll();
		}
		this.InitializeHealth(this.StartHealth(), this.StartMaxHealth());
		bool flag = false;
		if (ConVar.Server.respawnWithLoadout)
		{
			string infoString = this.GetInfoString("client.respawnloadout", string.Empty);
			Inventory.SavedLoadout savedLoadout;
			if (!string.IsNullOrEmpty(infoString) && Inventory.LoadLoadout(infoString, out savedLoadout))
			{
				savedLoadout.LoadItemsOnTo(this);
				flag = true;
			}
		}
		if (!flag)
		{
			this.inventory.GiveDefaultItems();
		}
		base.SendNetworkUpdateImmediate(false);
		base.ClientRPCPlayer(null, this, "StartLoading");
		Analytics.Azure.OnPlayerRespawned(this, spawnPointEntity);
		if (activeGameMode)
		{
			BaseGameMode.GetActiveGameMode(true).OnPlayerRespawn(this);
		}
		if (this.net != null)
		{
			EACServer.OnStartLoading(this.net.connection);
		}
	}

	// Token: 0x06000613 RID: 1555 RVA: 0x00042F8C File Offset: 0x0004118C
	public void Respawn()
	{
		global::BasePlayer.SpawnPoint spawnPoint = ServerMgr.FindSpawnPoint(this);
		if (ConVar.Server.respawnAtDeathPosition && this.ServerCurrentDeathNote != null)
		{
			spawnPoint.pos = this.ServerCurrentDeathNote.worldPosition;
		}
		this.RespawnAt(spawnPoint.pos, spawnPoint.rot, null);
	}

	// Token: 0x06000614 RID: 1556 RVA: 0x00042FD4 File Offset: 0x000411D4
	public bool IsImmortalTo(HitInfo info)
	{
		if (this.IsGod())
		{
			return true;
		}
		if (this.WoundingCausingImmortality(info))
		{
			return true;
		}
		global::BaseVehicle mountedVehicle = this.GetMountedVehicle();
		if (mountedVehicle != null && mountedVehicle.ignoreDamageFromOutside)
		{
			global::BasePlayer initiatorPlayer = info.InitiatorPlayer;
			if (initiatorPlayer != null && initiatorPlayer.GetMountedVehicle() != mountedVehicle)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06000615 RID: 1557 RVA: 0x0004302F File Offset: 0x0004122F
	public float TimeAlive()
	{
		return this.lifeStory.secondsAlive;
	}

	// Token: 0x06000616 RID: 1558 RVA: 0x0004303C File Offset: 0x0004123C
	public override void Hurt(HitInfo info)
	{
		if (this.IsDead())
		{
			return;
		}
		if (this.IsImmortalTo(info) && info.damageTypes.Total() >= 0f)
		{
			return;
		}
		if (ConVar.Server.pve && info.Initiator && info.Initiator is global::BasePlayer && info.Initiator != this)
		{
			(info.Initiator as global::BasePlayer).Hurt(info.damageTypes.Total(), DamageType.Generic, null, true);
			return;
		}
		if (info.damageTypes.Has(DamageType.Fun_Water))
		{
			bool flag = true;
			global::Item activeItem = this.GetActiveItem();
			if (activeItem != null && (activeItem.info.shortname == "gun.water" || activeItem.info.shortname == "pistol.water"))
			{
				float value = this.metabolism.wetness.value;
				this.metabolism.wetness.Add(ConVar.Server.funWaterWetnessGain);
				bool flag2 = this.metabolism.wetness.value >= ConVar.Server.funWaterDamageThreshold;
				flag = !flag2;
				if (info.InitiatorPlayer != null)
				{
					if (flag2 && value < ConVar.Server.funWaterDamageThreshold)
					{
						info.InitiatorPlayer.GiveAchievement("SUMMER_SOAKED");
					}
					if (this.metabolism.radiation_level.Fraction() > 0.2f && !string.IsNullOrEmpty("SUMMER_RADICAL"))
					{
						info.InitiatorPlayer.GiveAchievement("SUMMER_RADICAL");
					}
				}
			}
			if (flag)
			{
				info.damageTypes.Scale(DamageType.Fun_Water, 0f);
			}
		}
		if (info.damageTypes.Get(DamageType.Drowned) > 5f && this.drownEffect.isValid)
		{
			Effect.server.Run(this.drownEffect.resourcePath, this, StringPool.Get("head"), Vector3.zero, Vector3.zero, null, false);
		}
		if (this.modifiers != null)
		{
			if (info.damageTypes.Has(DamageType.Radiation))
			{
				info.damageTypes.Scale(DamageType.Radiation, 1f - Mathf.Clamp01(this.modifiers.GetValue(global::Modifier.ModifierType.Radiation_Resistance, 0f)));
			}
			if (info.damageTypes.Has(DamageType.RadiationExposure))
			{
				info.damageTypes.Scale(DamageType.RadiationExposure, 1f - Mathf.Clamp01(this.modifiers.GetValue(global::Modifier.ModifierType.Radiation_Exposure_Resistance, 0f)));
			}
		}
		this.metabolism.pending_health.Subtract(info.damageTypes.Total() * 10f);
		global::BasePlayer initiatorPlayer = info.InitiatorPlayer;
		if (initiatorPlayer && initiatorPlayer != this)
		{
			if (initiatorPlayer.InSafeZone() || this.InSafeZone())
			{
				initiatorPlayer.MarkHostileFor(300f);
			}
			if (initiatorPlayer.InSafeZone() && !initiatorPlayer.IsNpc)
			{
				info.damageTypes.ScaleAll(0f);
				return;
			}
			if (initiatorPlayer.IsNpc && initiatorPlayer.Family == BaseNpc.AiStatistics.FamilyEnum.Murderer && info.damageTypes.Get(DamageType.Explosion) > 0f)
			{
				info.damageTypes.ScaleAll(Halloween.scarecrow_beancan_vs_player_dmg_modifier);
			}
		}
		base.Hurt(info);
		if (BaseGameMode.GetActiveGameMode(true))
		{
			global::BasePlayer instigator = (info == null) ? null : info.InitiatorPlayer;
			BaseGameMode.GetActiveGameMode(true).OnPlayerHurt(instigator, this, info);
		}
		EACServer.LogPlayerTakeDamage(this, info);
		this.metabolism.SendChangesToClient();
		if (info.PointStart != Vector3.zero && (info.damageTypes.Total() >= 0f || this.IsGod()))
		{
			int arg = (int)info.damageTypes.GetMajorityDamageType();
			if (info.Weapon != null && info.damageTypes.Has(DamageType.Bullet))
			{
				global::BaseProjectile component = info.Weapon.GetComponent<global::BaseProjectile>();
				if (component != null && component.IsSilenced())
				{
					arg = 12;
				}
			}
			base.ClientRPCPlayerAndSpectators<Vector3, int, int>(null, this, "DirectionalDamage", info.PointStart, arg, Mathf.CeilToInt(info.damageTypes.Total()));
		}
		this.cachedNonSuicideHitInfo = info;
	}

	// Token: 0x06000617 RID: 1559 RVA: 0x00043424 File Offset: 0x00041624
	public override void Heal(float amount)
	{
		if (this.IsCrawling())
		{
			float health = base.health;
			base.Heal(amount);
			this.healingWhileCrawling += base.health - health;
			return;
		}
		base.Heal(amount);
	}

	// Token: 0x170000B2 RID: 178
	// (get) Token: 0x06000618 RID: 1560 RVA: 0x00043464 File Offset: 0x00041664
	public static IEnumerable<global::BasePlayer> allPlayerList
	{
		get
		{
			foreach (global::BasePlayer basePlayer in global::BasePlayer.sleepingPlayerList)
			{
				yield return basePlayer;
			}
			ListHashSet<global::BasePlayer>.Enumerator enumerator = default(ListHashSet<global::BasePlayer>.Enumerator);
			foreach (global::BasePlayer basePlayer2 in global::BasePlayer.activePlayerList)
			{
				yield return basePlayer2;
			}
			enumerator = default(ListHashSet<global::BasePlayer>.Enumerator);
			yield break;
			yield break;
		}
	}

	// Token: 0x06000619 RID: 1561 RVA: 0x00043470 File Offset: 0x00041670
	public static global::BasePlayer FindBot(ulong userId)
	{
		foreach (global::BasePlayer basePlayer in global::BasePlayer.bots)
		{
			if (basePlayer.userID == userId)
			{
				return basePlayer;
			}
		}
		return global::BasePlayer.FindBotClosestMatch(userId.ToString());
	}

	// Token: 0x0600061A RID: 1562 RVA: 0x000434D8 File Offset: 0x000416D8
	public static global::BasePlayer FindBotClosestMatch(string name)
	{
		if (string.IsNullOrEmpty(name))
		{
			return null;
		}
		foreach (global::BasePlayer basePlayer in global::BasePlayer.bots)
		{
			if (basePlayer.displayName.Contains(name))
			{
				return basePlayer;
			}
		}
		return null;
	}

	// Token: 0x0600061B RID: 1563 RVA: 0x00043544 File Offset: 0x00041744
	public static global::BasePlayer FindByID(ulong userID)
	{
		global::BasePlayer result;
		using (TimeWarning.New("BasePlayer.FindByID", 0))
		{
			foreach (global::BasePlayer basePlayer in global::BasePlayer.activePlayerList)
			{
				if (basePlayer.userID == userID)
				{
					return basePlayer;
				}
			}
			result = null;
		}
		return result;
	}

	// Token: 0x0600061C RID: 1564 RVA: 0x000435C4 File Offset: 0x000417C4
	public static bool TryFindByID(ulong userID, out global::BasePlayer basePlayer)
	{
		basePlayer = global::BasePlayer.FindByID(userID);
		return basePlayer != null;
	}

	// Token: 0x0600061D RID: 1565 RVA: 0x000435D8 File Offset: 0x000417D8
	public static global::BasePlayer FindSleeping(ulong userID)
	{
		global::BasePlayer result;
		using (TimeWarning.New("BasePlayer.FindSleeping", 0))
		{
			foreach (global::BasePlayer basePlayer in global::BasePlayer.sleepingPlayerList)
			{
				if (basePlayer.userID == userID)
				{
					return basePlayer;
				}
			}
			result = null;
		}
		return result;
	}

	// Token: 0x0600061E RID: 1566 RVA: 0x00043658 File Offset: 0x00041858
	public void Command(string strCommand, params object[] arguments)
	{
		if (this.net.connection == null)
		{
			return;
		}
		ConsoleNetwork.SendClientCommand(this.net.connection, strCommand, arguments);
	}

	// Token: 0x0600061F RID: 1567 RVA: 0x0004367A File Offset: 0x0004187A
	public override void OnInvalidPosition()
	{
		if (this.IsDead())
		{
			return;
		}
		this.Die(null);
	}

	// Token: 0x06000620 RID: 1568 RVA: 0x0004368C File Offset: 0x0004188C
	private static global::BasePlayer Find(string strNameOrIDOrIP, IEnumerable<global::BasePlayer> list)
	{
		global::BasePlayer basePlayer = list.FirstOrDefault((global::BasePlayer x) => x.UserIDString == strNameOrIDOrIP);
		if (basePlayer)
		{
			return basePlayer;
		}
		global::BasePlayer basePlayer2 = list.FirstOrDefault((global::BasePlayer x) => x.displayName.StartsWith(strNameOrIDOrIP, StringComparison.CurrentCultureIgnoreCase));
		if (basePlayer2)
		{
			return basePlayer2;
		}
		global::BasePlayer basePlayer3 = list.FirstOrDefault((global::BasePlayer x) => x.net != null && x.net.connection != null && x.net.connection.ipaddress == strNameOrIDOrIP);
		if (basePlayer3)
		{
			return basePlayer3;
		}
		return null;
	}

	// Token: 0x06000621 RID: 1569 RVA: 0x000436FE File Offset: 0x000418FE
	public static global::BasePlayer Find(string strNameOrIDOrIP)
	{
		return global::BasePlayer.Find(strNameOrIDOrIP, global::BasePlayer.activePlayerList);
	}

	// Token: 0x06000622 RID: 1570 RVA: 0x0004370B File Offset: 0x0004190B
	public static global::BasePlayer FindSleeping(string strNameOrIDOrIP)
	{
		return global::BasePlayer.Find(strNameOrIDOrIP, global::BasePlayer.sleepingPlayerList);
	}

	// Token: 0x06000623 RID: 1571 RVA: 0x00043718 File Offset: 0x00041918
	public static global::BasePlayer FindAwakeOrSleeping(string strNameOrIDOrIP)
	{
		return global::BasePlayer.Find(strNameOrIDOrIP, global::BasePlayer.allPlayerList);
	}

	// Token: 0x06000624 RID: 1572 RVA: 0x00043725 File Offset: 0x00041925
	public void SendConsoleCommand(string command, params object[] obj)
	{
		ConsoleNetwork.SendClientCommand(this.net.connection, command, obj);
	}

	// Token: 0x06000625 RID: 1573 RVA: 0x00043739 File Offset: 0x00041939
	public void UpdateRadiation(float fAmount)
	{
		this.metabolism.radiation_level.Increase(fAmount);
	}

	// Token: 0x06000626 RID: 1574 RVA: 0x0004374C File Offset: 0x0004194C
	public override float RadiationExposureFraction()
	{
		float num = Mathf.Clamp(this.baseProtection.amounts[17], 0f, 1f);
		return 1f - num;
	}

	// Token: 0x06000627 RID: 1575 RVA: 0x0004377E File Offset: 0x0004197E
	public override float RadiationProtection()
	{
		return this.baseProtection.amounts[17] * 100f;
	}

	// Token: 0x06000628 RID: 1576 RVA: 0x00043794 File Offset: 0x00041994
	public override void OnHealthChanged(float oldvalue, float newvalue)
	{
		base.OnHealthChanged(oldvalue, newvalue);
		if (!base.isServer)
		{
			return;
		}
		if (oldvalue > newvalue)
		{
			this.LifeStoryHurt(oldvalue - newvalue);
		}
		else
		{
			this.LifeStoryHeal(newvalue - oldvalue);
		}
		this.metabolism.isDirty = true;
	}

	// Token: 0x06000629 RID: 1577 RVA: 0x000437CB File Offset: 0x000419CB
	public void SV_ClothingChanged()
	{
		this.UpdateProtectionFromClothing();
		this.UpdateMoveSpeedFromClothing();
	}

	// Token: 0x0600062A RID: 1578 RVA: 0x000437D9 File Offset: 0x000419D9
	public bool IsNoob()
	{
		return !this.HasPlayerFlag(global::BasePlayer.PlayerFlags.DisplaySash);
	}

	// Token: 0x0600062B RID: 1579 RVA: 0x000437EC File Offset: 0x000419EC
	public bool HasHostileItem()
	{
		bool result;
		using (TimeWarning.New("BasePlayer.HasHostileItem", 0))
		{
			foreach (global::Item item in this.inventory.containerBelt.itemList)
			{
				if (this.IsHostileItem(item))
				{
					return true;
				}
			}
			foreach (global::Item item2 in this.inventory.containerMain.itemList)
			{
				if (this.IsHostileItem(item2))
				{
					return true;
				}
			}
			result = false;
		}
		return result;
	}

	// Token: 0x0600062C RID: 1580 RVA: 0x000438CC File Offset: 0x00041ACC
	public override void GiveItem(global::Item item, global::BaseEntity.GiveItemReason reason = global::BaseEntity.GiveItemReason.Generic)
	{
		if (reason == global::BaseEntity.GiveItemReason.ResourceHarvested)
		{
			this.stats.Add(string.Format("harvest.{0}", item.info.shortname), item.amount, (global::Stats)6);
		}
		if (reason == global::BaseEntity.GiveItemReason.ResourceHarvested || reason == global::BaseEntity.GiveItemReason.Crafted)
		{
			this.ProcessMissionEvent(BaseMission.MissionEventType.HARVEST, item.info.shortname, (float)item.amount);
		}
		int amount = item.amount;
		if (!this.inventory.GiveItem(item, null, false))
		{
			item.Drop(this.inventory.containerMain.dropPosition, this.inventory.containerMain.dropVelocity, default(Quaternion));
			return;
		}
		bool infoBool = this.GetInfoBool("global.streamermode", false);
		string name = item.GetName(new bool?(infoBool));
		if (!string.IsNullOrEmpty(name))
		{
			this.Command("note.inv", new object[]
			{
				item.info.itemid,
				amount,
				name,
				(int)reason
			});
			return;
		}
		this.Command("note.inv", new object[]
		{
			item.info.itemid,
			amount,
			string.Empty,
			(int)reason
		});
	}

	// Token: 0x0600062D RID: 1581 RVA: 0x00043A0B File Offset: 0x00041C0B
	public override void AttackerInfo(PlayerLifeStory.DeathInfo info)
	{
		info.attackerName = this.displayName;
		info.attackerSteamID = this.userID;
	}

	// Token: 0x0600062E RID: 1582 RVA: 0x00043A25 File Offset: 0x00041C25
	public Workbench GetCachedCraftLevelWorkbench()
	{
		return this._cachedWorkbench;
	}

	// Token: 0x170000B3 RID: 179
	// (get) Token: 0x0600062F RID: 1583 RVA: 0x00043A30 File Offset: 0x00041C30
	public float currentCraftLevel
	{
		get
		{
			if (this.triggers == null)
			{
				this._cachedWorkbench = null;
				return 0f;
			}
			if (this.nextCheckTime > UnityEngine.Time.realtimeSinceStartup)
			{
				return this.cachedCraftLevel;
			}
			this._cachedWorkbench = null;
			this.nextCheckTime = UnityEngine.Time.realtimeSinceStartup + UnityEngine.Random.Range(0.4f, 0.5f);
			float num = 0f;
			for (int i = 0; i < this.triggers.Count; i++)
			{
				TriggerWorkbench triggerWorkbench = this.triggers[i] as TriggerWorkbench;
				if (!(triggerWorkbench == null) && !(triggerWorkbench.parentBench == null) && triggerWorkbench.parentBench.IsVisible(this.eyes.position, float.PositiveInfinity))
				{
					this._cachedWorkbench = triggerWorkbench.parentBench;
					float num2 = triggerWorkbench.WorkbenchLevel();
					if (num2 > num)
					{
						num = num2;
					}
				}
			}
			this.cachedCraftLevel = num;
			return num;
		}
	}

	// Token: 0x170000B4 RID: 180
	// (get) Token: 0x06000630 RID: 1584 RVA: 0x00043B0C File Offset: 0x00041D0C
	public float currentComfort
	{
		get
		{
			float num = 0f;
			if (this.isMounted)
			{
				num = this.GetMounted().GetComfort();
			}
			if (this.triggers == null)
			{
				return num;
			}
			for (int i = 0; i < this.triggers.Count; i++)
			{
				TriggerComfort triggerComfort = this.triggers[i] as TriggerComfort;
				if (!(triggerComfort == null))
				{
					float num2 = triggerComfort.CalculateComfort(base.transform.position, this);
					if (num2 > num)
					{
						num = num2;
					}
				}
			}
			return num;
		}
	}

	// Token: 0x170000B5 RID: 181
	// (get) Token: 0x06000631 RID: 1585 RVA: 0x00043B88 File Offset: 0x00041D88
	public float currentSafeLevel
	{
		get
		{
			float num = 0f;
			if (this.triggers == null)
			{
				return num;
			}
			for (int i = 0; i < this.triggers.Count; i++)
			{
				TriggerSafeZone triggerSafeZone = this.triggers[i] as TriggerSafeZone;
				if (!(triggerSafeZone == null))
				{
					float safeLevel = triggerSafeZone.GetSafeLevel(base.transform.position);
					if (safeLevel > num)
					{
						num = safeLevel;
					}
				}
			}
			return num;
		}
	}

	// Token: 0x06000632 RID: 1586 RVA: 0x0000441C File Offset: 0x0000261C
	public virtual bool ShouldDropActiveItem()
	{
		return true;
	}

	// Token: 0x06000633 RID: 1587 RVA: 0x00043BF0 File Offset: 0x00041DF0
	public override void Die(HitInfo info = null)
	{
		using (TimeWarning.New("Player.Die", 0))
		{
			if (!this.IsDead())
			{
				if (this.Belt != null && this.ShouldDropActiveItem())
				{
					Vector3 vector = new Vector3(UnityEngine.Random.Range(-2f, 2f), 0.2f, UnityEngine.Random.Range(-2f, 2f));
					this.Belt.DropActive(this.GetDropPosition(), this.GetInheritedDropVelocity() + vector.normalized * 3f);
				}
				if (!this.WoundInsteadOfDying(info))
				{
					global::SleepingBag.OnPlayerDeath(this);
					base.Die(info);
				}
			}
		}
	}

	// Token: 0x06000634 RID: 1588 RVA: 0x00043CB4 File Offset: 0x00041EB4
	public void Kick(string reason)
	{
		if (!this.IsConnected)
		{
			return;
		}
		Network.Net.sv.Kick(this.net.connection, reason, false);
	}

	// Token: 0x06000635 RID: 1589 RVA: 0x00043CD6 File Offset: 0x00041ED6
	public override Vector3 GetDropPosition()
	{
		return this.eyes.position;
	}

	// Token: 0x06000636 RID: 1590 RVA: 0x00043CE3 File Offset: 0x00041EE3
	public override Vector3 GetDropVelocity()
	{
		return this.GetInheritedDropVelocity() + this.eyes.BodyForward() * 4f + Vector3Ex.Range(-0.5f, 0.5f);
	}

	// Token: 0x06000637 RID: 1591 RVA: 0x00043D1C File Offset: 0x00041F1C
	public override void ApplyInheritedVelocity(Vector3 velocity)
	{
		global::BaseEntity parentEntity = base.GetParentEntity();
		if (parentEntity != null)
		{
			base.ClientRPCPlayer<Vector3, NetworkableId>(null, this, "SetInheritedVelocity", parentEntity.transform.InverseTransformDirection(velocity), parentEntity.net.ID);
		}
		else
		{
			base.ClientRPCPlayer<Vector3>(null, this, "SetInheritedVelocity", velocity);
		}
		this.PauseSpeedHackDetection(1f);
	}

	// Token: 0x06000638 RID: 1592 RVA: 0x00043D78 File Offset: 0x00041F78
	public virtual void SetInfo(string key, string val)
	{
		if (!this.IsConnected)
		{
			return;
		}
		this.net.connection.info.Set(key, val);
	}

	// Token: 0x06000639 RID: 1593 RVA: 0x00043D9A File Offset: 0x00041F9A
	public virtual int GetInfoInt(string key, int defaultVal)
	{
		if (!this.IsConnected)
		{
			return defaultVal;
		}
		return this.net.connection.info.GetInt(key, defaultVal);
	}

	// Token: 0x0600063A RID: 1594 RVA: 0x00043DBD File Offset: 0x00041FBD
	public virtual bool GetInfoBool(string key, bool defaultVal)
	{
		if (!this.IsConnected)
		{
			return defaultVal;
		}
		return this.net.connection.info.GetBool(key, defaultVal);
	}

	// Token: 0x0600063B RID: 1595 RVA: 0x00043DE0 File Offset: 0x00041FE0
	public virtual string GetInfoString(string key, string defaultVal)
	{
		if (!this.IsConnected)
		{
			return defaultVal;
		}
		return this.net.connection.info.GetString(key, defaultVal);
	}

	// Token: 0x0600063C RID: 1596 RVA: 0x00043E04 File Offset: 0x00042004
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.CallsPerSecond(1UL)]
	public void PerformanceReport(global::BaseEntity.RPCMessage msg)
	{
		string text = msg.read.String(256);
		string text2 = msg.read.StringRaw(8388608U);
		ClientPerformanceReport clientPerformanceReport = JsonConvert.DeserializeObject<ClientPerformanceReport>(text2);
		if (clientPerformanceReport.user_id != this.UserIDString)
		{
			DebugEx.Log(string.Format("Client performance report from {0} has incorrect user_id ({1})", this, this.UserIDString), StackTraceLogType.None);
			return;
		}
		if (text == "json")
		{
			DebugEx.Log(text2, StackTraceLogType.None);
			return;
		}
		if (text == "legacy")
		{
			string text3 = (clientPerformanceReport.memory_managed_heap + "MB").PadRight(9);
			string text4 = (clientPerformanceReport.memory_system + "MB").PadRight(9);
			string text5 = (clientPerformanceReport.fps.ToString("0") + "FPS").PadRight(8);
			string text6 = ((long)clientPerformanceReport.fps).FormatSeconds().PadRight(9);
			string text7 = this.UserIDString.PadRight(20);
			string text8 = clientPerformanceReport.streamer_mode.ToString().PadRight(7);
			DebugEx.Log(string.Concat(new string[]
			{
				text3,
				text4,
				text5,
				text6,
				text8,
				text7,
				this.displayName
			}), StackTraceLogType.None);
			return;
		}
		if (!(text == "none"))
		{
			if (text == "rcon")
			{
				RCon.Broadcast(RCon.LogType.ClientPerf, text2);
				return;
			}
			Debug.LogError("Unknown PerformanceReport format '" + text + "'");
		}
	}

	// Token: 0x0600063D RID: 1597 RVA: 0x00043F90 File Offset: 0x00042190
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.CallsPerSecond(1UL)]
	public void PerformanceReport_Frametime(global::BaseEntity.RPCMessage msg)
	{
		int request_id = msg.read.Int32();
		int start_frame = msg.read.Int32();
		int num = msg.read.Int32();
		List<int> list = Facepunch.Pool.GetList<int>();
		for (int i = 0; i < num; i++)
		{
			list.Add(ProtocolParser.ReadInt32(msg.read));
		}
		ClientFrametimeReport clientFrametimeReport = new ClientFrametimeReport();
		clientFrametimeReport.frame_times = list;
		clientFrametimeReport.request_id = request_id;
		clientFrametimeReport.start_frame = start_frame;
		DebugEx.Log(JsonConvert.SerializeObject(clientFrametimeReport), StackTraceLogType.None);
		Facepunch.Pool.FreeList<int>(ref clientFrametimeReport.frame_times);
	}

	// Token: 0x0600063E RID: 1598 RVA: 0x00044018 File Offset: 0x00042218
	public override bool ShouldNetworkTo(global::BasePlayer player)
	{
		return (!this.IsSpectating() || !(player != this) || player.net.connection.info.GetBool("global.specnet", false)) && base.ShouldNetworkTo(player);
	}

	// Token: 0x0600063F RID: 1599 RVA: 0x00044051 File Offset: 0x00042251
	internal void GiveAchievement(string name)
	{
		if (GameInfo.HasAchievements)
		{
			base.ClientRPCPlayer<string>(null, this, "RecieveAchievement", name);
		}
	}

	// Token: 0x06000640 RID: 1600 RVA: 0x00044068 File Offset: 0x00042268
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.CallsPerSecond(1UL)]
	public void OnPlayerReported(global::BaseEntity.RPCMessage msg)
	{
		string text = msg.read.String(256);
		string message = msg.read.StringMultiLine(2048);
		string type = msg.read.String(256);
		string text2 = msg.read.String(256);
		string text3 = msg.read.String(256);
		DebugEx.Log(string.Format("[PlayerReport] {0} reported {1}[{2}] - \"{3}\"", new object[]
		{
			this,
			text3,
			text2,
			text
		}), StackTraceLogType.None);
		RCon.Broadcast(RCon.LogType.Report, new
		{
			PlayerId = this.UserIDString,
			PlayerName = this.displayName,
			TargetId = text2,
			TargetName = text3,
			Subject = text,
			Message = message,
			Type = type
		});
	}

	// Token: 0x06000641 RID: 1601 RVA: 0x00044110 File Offset: 0x00042310
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.CallsPerSecond(1UL)]
	public void OnFeedbackReport(global::BaseEntity.RPCMessage msg)
	{
		if (!ConVar.Server.printReportsToConsole && string.IsNullOrEmpty(ConVar.Server.reportsServerEndpoint))
		{
			return;
		}
		string text = msg.read.String(256);
		string text2 = msg.read.StringMultiLine(2048);
		ReportType reportType = (ReportType)Mathf.Clamp(msg.read.Int32(), 0, 5);
		if (ConVar.Server.printReportsToConsole)
		{
			DebugEx.Log(string.Format("[FeedbackReport] {0} reported {1} - \"{2}\" \"{3}\"", new object[]
			{
				this,
				reportType,
				text,
				text2
			}), StackTraceLogType.None);
			RCon.Broadcast(RCon.LogType.Report, new
			{
				PlayerId = this.UserIDString,
				PlayerName = this.displayName,
				Subject = text,
				Message = text2,
				Type = reportType
			});
		}
		if (!string.IsNullOrEmpty(ConVar.Server.reportsServerEndpoint))
		{
			string image = msg.read.StringMultiLine(60000);
			Facepunch.Models.Feedback feedback = new Facepunch.Models.Feedback
			{
				Type = reportType,
				Message = text2,
				Subject = text
			};
			feedback.AppInfo.Image = image;
			Facepunch.Feedback.ServerReport(ConVar.Server.reportsServerEndpoint, this.userID, ConVar.Server.reportsServerEndpointKey, feedback);
		}
	}

	// Token: 0x06000642 RID: 1602 RVA: 0x00044218 File Offset: 0x00042418
	public void StartDemoRecording()
	{
		if (this.net == null || this.net.connection == null)
		{
			return;
		}
		if (this.net.connection.IsRecording)
		{
			return;
		}
		string text = string.Format("demos/{0}/{1:yyyy-MM-dd-hhmmss}.dem", this.UserIDString, DateTime.Now);
		Debug.Log(this.ToString() + " recording started: " + text);
		this.net.connection.StartRecording(text, new Demo.Header
		{
			version = Demo.Version,
			level = UnityEngine.Application.loadedLevelName,
			levelSeed = global::World.Seed,
			levelSize = global::World.Size,
			checksum = global::World.Checksum,
			localclient = this.userID,
			position = this.eyes.position,
			rotation = this.eyes.HeadForward(),
			levelUrl = global::World.Url,
			recordedTime = DateTime.Now.ToBinary()
		});
		base.SendNetworkUpdateImmediate(false);
		this.SendGlobalSnapshot();
		this.SendFullSnapshot();
		this.SendEntityUpdate();
		TreeManager.SendSnapshot(this);
		ServerMgr.SendReplicatedVars(this.net.connection);
		base.InvokeRepeating(new Action(this.MonitorDemoRecording), 10f, 10f);
	}

	// Token: 0x06000643 RID: 1603 RVA: 0x00044364 File Offset: 0x00042564
	public void StopDemoRecording()
	{
		if (this.net == null || this.net.connection == null)
		{
			return;
		}
		if (!this.net.connection.IsRecording)
		{
			return;
		}
		Debug.Log(this.ToString() + " recording stopped: " + this.net.connection.RecordFilename);
		this.net.connection.StopRecording();
		base.CancelInvoke(new Action(this.MonitorDemoRecording));
	}

	// Token: 0x06000644 RID: 1604 RVA: 0x000443E4 File Offset: 0x000425E4
	public void MonitorDemoRecording()
	{
		if (this.net == null || this.net.connection == null)
		{
			return;
		}
		if (!this.net.connection.IsRecording)
		{
			return;
		}
		if (this.net.connection.RecordTimeElapsed.TotalSeconds >= (double)Demo.splitseconds || (float)this.net.connection.RecordFilesize >= Demo.splitmegabytes * 1024f * 1024f)
		{
			this.StopDemoRecording();
			this.StartDemoRecording();
		}
	}

	// Token: 0x06000645 RID: 1605 RVA: 0x0004446A File Offset: 0x0004266A
	public void InvalidateCachedPeristantPlayer()
	{
		this.cachedPersistantPlayer = null;
	}

	// Token: 0x170000B6 RID: 182
	// (get) Token: 0x06000646 RID: 1606 RVA: 0x00044473 File Offset: 0x00042673
	// (set) Token: 0x06000647 RID: 1607 RVA: 0x0004449E File Offset: 0x0004269E
	public PersistantPlayer PersistantPlayerInfo
	{
		get
		{
			if (this.cachedPersistantPlayer == null)
			{
				this.cachedPersistantPlayer = SingletonComponent<ServerMgr>.Instance.persistance.GetPlayerInfo(this.userID);
			}
			return this.cachedPersistantPlayer;
		}
		set
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			this.cachedPersistantPlayer = value;
			SingletonComponent<ServerMgr>.Instance.persistance.SetPlayerInfo(this.userID, value);
		}
	}

	// Token: 0x06000648 RID: 1608 RVA: 0x000444CC File Offset: 0x000426CC
	public bool IsPlayerVisibleToUs(global::BasePlayer otherPlayer, int layerMask)
	{
		if (otherPlayer == null)
		{
			return false;
		}
		Vector3 vector;
		if (this.isMounted)
		{
			vector = this.eyes.worldMountedPosition;
		}
		else if (this.IsDucked())
		{
			vector = this.eyes.worldCrouchedPosition;
		}
		else if (this.IsCrawling())
		{
			vector = this.eyes.worldCrawlingPosition;
		}
		else
		{
			vector = this.eyes.worldStandingPosition;
		}
		return (otherPlayer.IsVisibleSpecificLayers(vector, otherPlayer.CenterPoint(), layerMask, float.PositiveInfinity) || otherPlayer.IsVisibleSpecificLayers(vector, otherPlayer.transform.position, layerMask, float.PositiveInfinity) || otherPlayer.IsVisibleSpecificLayers(vector, otherPlayer.eyes.position, layerMask, float.PositiveInfinity)) && (base.IsVisibleSpecificLayers(otherPlayer.CenterPoint(), vector, layerMask, float.PositiveInfinity) || base.IsVisibleSpecificLayers(otherPlayer.transform.position, vector, layerMask, float.PositiveInfinity) || base.IsVisibleSpecificLayers(otherPlayer.eyes.position, vector, layerMask, float.PositiveInfinity));
	}

	// Token: 0x06000649 RID: 1609 RVA: 0x000063A5 File Offset: 0x000045A5
	protected virtual void OnKilledByPlayer(global::BasePlayer p)
	{
	}

	// Token: 0x0600064A RID: 1610 RVA: 0x000445C9 File Offset: 0x000427C9
	public int GetIdealSlot(global::BasePlayer player, global::Item item)
	{
		return -1;
	}

	// Token: 0x0600064B RID: 1611 RVA: 0x000445CC File Offset: 0x000427CC
	public ItemContainerId GetIdealContainer(global::BasePlayer looter, global::Item item, bool altMove)
	{
		bool flag = !altMove && looter.inventory.loot.containers.Count > 0;
		global::ItemContainer parent = item.parent;
		global::Item activeItem = looter.GetActiveItem();
		if (activeItem != null && !flag && activeItem.contents != null && activeItem.contents != item.parent && activeItem.contents.capacity > 0 && activeItem.contents.CanAcceptItem(item, -1) == global::ItemContainer.CanAcceptResult.CanAccept)
		{
			return activeItem.contents.uid;
		}
		if (item.info.isWearable && item.info.ItemModWearable.equipOnRightClick && (item.parent == this.inventory.containerBelt || item.parent == this.inventory.containerMain) && !flag)
		{
			return this.inventory.containerWear.uid;
		}
		if (parent == this.inventory.containerMain)
		{
			if (flag)
			{
				return default(ItemContainerId);
			}
			return this.inventory.containerBelt.uid;
		}
		else
		{
			if (parent == this.inventory.containerWear)
			{
				return this.inventory.containerMain.uid;
			}
			if (parent == this.inventory.containerBelt)
			{
				return this.inventory.containerMain.uid;
			}
			return default(ItemContainerId);
		}
	}

	// Token: 0x170000B7 RID: 183
	// (get) Token: 0x0600064C RID: 1612 RVA: 0x00044717 File Offset: 0x00042917
	// (set) Token: 0x0600064D RID: 1613 RVA: 0x0004471F File Offset: 0x0004291F
	public bool IsBeingSpectated { get; private set; }

	// Token: 0x0600064E RID: 1614 RVA: 0x00044728 File Offset: 0x00042928
	private void Tick_Spectator()
	{
		int num = 0;
		if (this.serverInput.WasJustPressed(BUTTON.JUMP))
		{
			num++;
		}
		if (this.serverInput.WasJustPressed(BUTTON.DUCK))
		{
			num--;
		}
		if (num != 0)
		{
			this.SpectateOffset += num;
			using (TimeWarning.New("UpdateSpectateTarget", 0))
			{
				this.UpdateSpectateTarget(this.spectateFilter);
			}
		}
	}

	// Token: 0x0600064F RID: 1615 RVA: 0x000447A4 File Offset: 0x000429A4
	public void UpdateSpectateTarget(string strName)
	{
		this.spectateFilter = strName;
		IEnumerable<global::BaseEntity> source;
		if (this.spectateFilter.StartsWith("@"))
		{
			string filter = this.spectateFilter.Substring(1);
			source = (from x in global::BaseNetworkable.serverEntities
			where x.name.Contains(filter, CompareOptions.IgnoreCase)
			where x != this
			select x).Cast<global::BaseEntity>();
		}
		else
		{
			IEnumerable<global::BasePlayer> source2 = from x in global::BasePlayer.activePlayerList
			where !x.IsSpectating() && !x.IsDead() && !x.IsSleeping()
			select x;
			if (strName.Length > 0)
			{
				source2 = from x in source2
				where x.displayName.Contains(this.spectateFilter, CompareOptions.IgnoreCase) || x.UserIDString.Contains(this.spectateFilter)
				where x != this
				select x;
			}
			source2 = from x in source2
			orderby x.displayName
			select x;
			source = source2.Cast<global::BaseEntity>();
		}
		global::BaseEntity[] array = source.ToArray<global::BaseEntity>();
		if (array.Length == 0)
		{
			this.ChatMessage("No valid spectate targets!");
			return;
		}
		global::BaseEntity baseEntity = array[this.SpectateOffset % array.Length];
		if (baseEntity != null)
		{
			this.SpectatePlayer(baseEntity);
		}
	}

	// Token: 0x06000650 RID: 1616 RVA: 0x000448D4 File Offset: 0x00042AD4
	public void UpdateSpectateTarget(ulong id)
	{
		foreach (global::BasePlayer basePlayer in global::BasePlayer.activePlayerList)
		{
			if (basePlayer != null && basePlayer.userID == id)
			{
				this.spectateFilter = string.Empty;
				this.SpectatePlayer(basePlayer);
				break;
			}
		}
	}

	// Token: 0x06000651 RID: 1617 RVA: 0x00044948 File Offset: 0x00042B48
	private void SpectatePlayer(global::BaseEntity target)
	{
		if (target is global::BasePlayer)
		{
			this.ChatMessage("Spectating: " + (target as global::BasePlayer).displayName);
		}
		else
		{
			this.ChatMessage("Spectating: " + target.ToString());
		}
		using (TimeWarning.New("SendEntitySnapshot", 0))
		{
			this.SendEntitySnapshot(target);
		}
		base.gameObject.Identity();
		using (TimeWarning.New("SetParent", 0))
		{
			base.SetParent(target, false, false);
		}
	}

	// Token: 0x06000652 RID: 1618 RVA: 0x000449F8 File Offset: 0x00042BF8
	public void StartSpectating()
	{
		if (this.IsSpectating())
		{
			return;
		}
		this.SetPlayerFlag(global::BasePlayer.PlayerFlags.Spectating, true);
		base.gameObject.SetLayerRecursive(10);
		base.CancelInvoke(new Action(this.InventoryUpdate));
		this.ChatMessage("Becoming Spectator");
		this.UpdateSpectateTarget(this.spectateFilter);
	}

	// Token: 0x06000653 RID: 1619 RVA: 0x00044A4D File Offset: 0x00042C4D
	public void StopSpectating()
	{
		if (!this.IsSpectating())
		{
			return;
		}
		base.SetParent(null, false, false);
		this.SetPlayerFlag(global::BasePlayer.PlayerFlags.Spectating, false);
		base.gameObject.SetLayerRecursive(17);
	}

	// Token: 0x06000654 RID: 1620 RVA: 0x00044A77 File Offset: 0x00042C77
	public void Teleport(global::BasePlayer player)
	{
		this.Teleport(player.transform.position);
	}

	// Token: 0x06000655 RID: 1621 RVA: 0x00044A8C File Offset: 0x00042C8C
	public void Teleport(string strName, bool playersOnly)
	{
		global::BaseEntity[] array = global::BaseEntity.Util.FindTargets(strName, playersOnly);
		if (array == null || array.Length == 0)
		{
			return;
		}
		global::BaseEntity baseEntity = array[UnityEngine.Random.Range(0, array.Length)];
		this.Teleport(baseEntity.transform.position);
	}

	// Token: 0x06000656 RID: 1622 RVA: 0x00044AC6 File Offset: 0x00042CC6
	public void Teleport(Vector3 position)
	{
		this.MovePosition(position);
		base.ClientRPCPlayer<Vector3>(null, this, "ForcePositionTo", position);
	}

	// Token: 0x06000657 RID: 1623 RVA: 0x00044ADD File Offset: 0x00042CDD
	public void CopyRotation(global::BasePlayer player)
	{
		this.viewAngles = player.viewAngles;
		base.SendNetworkUpdate_Position();
	}

	// Token: 0x06000658 RID: 1624 RVA: 0x00044AF1 File Offset: 0x00042CF1
	protected override void OnChildAdded(global::BaseEntity child)
	{
		base.OnChildAdded(child);
		if (child is global::BasePlayer)
		{
			this.IsBeingSpectated = true;
		}
	}

	// Token: 0x06000659 RID: 1625 RVA: 0x00044B0C File Offset: 0x00042D0C
	protected override void OnChildRemoved(global::BaseEntity child)
	{
		base.OnChildRemoved(child);
		if (child is global::BasePlayer)
		{
			this.IsBeingSpectated = false;
			using (List<global::BaseEntity>.Enumerator enumerator = this.children.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current is global::BasePlayer)
					{
						this.IsBeingSpectated = true;
					}
				}
			}
		}
	}

	// Token: 0x0600065A RID: 1626 RVA: 0x00044B7C File Offset: 0x00042D7C
	public override float GetThreatLevel()
	{
		this.EnsureUpdated();
		return this.cachedThreatLevel;
	}

	// Token: 0x0600065B RID: 1627 RVA: 0x00044B8C File Offset: 0x00042D8C
	public void EnsureUpdated()
	{
		if (UnityEngine.Time.realtimeSinceStartup - this.lastUpdateTime < 30f)
		{
			return;
		}
		this.lastUpdateTime = UnityEngine.Time.realtimeSinceStartup;
		this.cachedThreatLevel = 0f;
		if (!this.IsSleeping())
		{
			if (this.inventory.containerWear.itemList.Count > 2)
			{
				this.cachedThreatLevel += 1f;
			}
			foreach (global::Item item in this.inventory.containerBelt.itemList)
			{
				global::BaseEntity heldEntity = item.GetHeldEntity();
				if (heldEntity && heldEntity is global::BaseProjectile && !(heldEntity is BowWeapon))
				{
					this.cachedThreatLevel += 2f;
					break;
				}
			}
		}
	}

	// Token: 0x0600065C RID: 1628 RVA: 0x00044C74 File Offset: 0x00042E74
	public override bool IsHostile()
	{
		return this.State.unHostileTimestamp > TimeEx.currentTimestamp;
	}

	// Token: 0x0600065D RID: 1629 RVA: 0x00044C88 File Offset: 0x00042E88
	public virtual float GetHostileDuration()
	{
		return Mathf.Clamp((float)(this.State.unHostileTimestamp - TimeEx.currentTimestamp), 0f, float.PositiveInfinity);
	}

	// Token: 0x0600065E RID: 1630 RVA: 0x00044CAC File Offset: 0x00042EAC
	public override void MarkHostileFor(float duration = 60f)
	{
		double currentTimestamp = TimeEx.currentTimestamp;
		double val = currentTimestamp + (double)duration;
		this.State.unHostileTimestamp = Math.Max(this.State.unHostileTimestamp, val);
		this.DirtyPlayerState();
		double num = Math.Max(this.State.unHostileTimestamp - currentTimestamp, 0.0);
		base.ClientRPCPlayer<float>(null, this, "SetHostileLength", (float)num);
	}

	// Token: 0x0600065F RID: 1631 RVA: 0x00044D14 File Offset: 0x00042F14
	public void MarkWeaponDrawnDuration(float newDuration)
	{
		float num = this.weaponDrawnDuration;
		this.weaponDrawnDuration = newDuration;
		if ((float)Mathf.FloorToInt(newDuration) != num)
		{
			base.ClientRPCPlayer<float>(null, this, "SetWeaponDrawnDuration", this.weaponDrawnDuration);
		}
	}

	// Token: 0x06000660 RID: 1632 RVA: 0x00044D4C File Offset: 0x00042F4C
	public void AddWeaponDrawnDuration(float duration)
	{
		this.MarkWeaponDrawnDuration(this.weaponDrawnDuration + duration);
	}

	// Token: 0x170000B8 RID: 184
	// (get) Token: 0x06000661 RID: 1633 RVA: 0x00044D5C File Offset: 0x00042F5C
	// (set) Token: 0x06000662 RID: 1634 RVA: 0x00044D64 File Offset: 0x00042F64
	public InputState serverInput { get; private set; } = new InputState();

	// Token: 0x170000B9 RID: 185
	// (get) Token: 0x06000663 RID: 1635 RVA: 0x00044D6D File Offset: 0x00042F6D
	public float timeSinceLastTick
	{
		get
		{
			if (this.lastTickTime == 0f)
			{
				return 0f;
			}
			return UnityEngine.Time.time - this.lastTickTime;
		}
	}

	// Token: 0x170000BA RID: 186
	// (get) Token: 0x06000664 RID: 1636 RVA: 0x00044D8E File Offset: 0x00042F8E
	public float IdleTime
	{
		get
		{
			if (this.lastInputTime == 0f)
			{
				return 0f;
			}
			return UnityEngine.Time.time - this.lastInputTime;
		}
	}

	// Token: 0x170000BB RID: 187
	// (get) Token: 0x06000665 RID: 1637 RVA: 0x00044DAF File Offset: 0x00042FAF
	public bool isStalled
	{
		get
		{
			return !this.IsDead() && !this.IsSleeping() && this.timeSinceLastTick > 1f;
		}
	}

	// Token: 0x170000BC RID: 188
	// (get) Token: 0x06000666 RID: 1638 RVA: 0x00044DD2 File Offset: 0x00042FD2
	public bool wasStalled
	{
		get
		{
			if (this.isStalled)
			{
				this.lastStallTime = UnityEngine.Time.time;
			}
			return UnityEngine.Time.time - this.lastStallTime < 1f;
		}
	}

	// Token: 0x06000667 RID: 1639 RVA: 0x00044DFC File Offset: 0x00042FFC
	public void OnReceivedTick(Stream stream)
	{
		using (TimeWarning.New("OnReceiveTickFromStream", 0))
		{
			PlayerTick playerTick = null;
			using (TimeWarning.New("PlayerTick.Deserialize", 0))
			{
				playerTick = PlayerTick.Deserialize(stream, this.lastReceivedTick, true);
			}
			using (TimeWarning.New("RecordPacket", 0))
			{
				this.net.connection.RecordPacket(15, playerTick);
			}
			using (TimeWarning.New("PlayerTick.Copy", 0))
			{
				this.lastReceivedTick = playerTick.Copy();
			}
			using (TimeWarning.New("OnReceiveTick", 0))
			{
				this.OnReceiveTick(playerTick, this.wasStalled);
			}
			this.lastTickTime = UnityEngine.Time.time;
			playerTick.Dispose();
		}
	}

	// Token: 0x06000668 RID: 1640 RVA: 0x00044F10 File Offset: 0x00043110
	public void OnReceivedVoice(byte[] data)
	{
		NetWrite netWrite = Network.Net.sv.StartWrite();
		netWrite.PacketID(Message.Type.VoiceData);
		netWrite.EntityID(this.net.ID);
		netWrite.BytesWithSize(data);
		float num = 0f;
		if (this.HasPlayerFlag(global::BasePlayer.PlayerFlags.VoiceRangeBoost))
		{
			num = Voice.voiceRangeBoostAmount;
		}
		netWrite.Send(new SendInfo(global::BaseNetworkable.GetConnectionsWithin(base.transform.position, 100f + num))
		{
			priority = Priority.Immediate
		});
		if (this.activeTelephone != null)
		{
			this.activeTelephone.OnReceivedVoiceFromUser(data);
		}
	}

	// Token: 0x06000669 RID: 1641 RVA: 0x00044FA6 File Offset: 0x000431A6
	public void ResetInputIdleTime()
	{
		this.lastInputTime = UnityEngine.Time.time;
	}

	// Token: 0x0600066A RID: 1642 RVA: 0x00044FB3 File Offset: 0x000431B3
	private void EACStateUpdate()
	{
		if (this.IsReceivingSnapshot)
		{
			return;
		}
		EACServer.LogPlayerTick(this);
	}

	// Token: 0x0600066B RID: 1643 RVA: 0x00044FC4 File Offset: 0x000431C4
	private void OnReceiveTick(PlayerTick msg, bool wasPlayerStalled)
	{
		if (msg.inputState != null)
		{
			this.serverInput.Flip(msg.inputState);
		}
		if (this.serverInput.current.buttons != this.serverInput.previous.buttons)
		{
			this.ResetInputIdleTime();
		}
		if (this.IsReceivingSnapshot)
		{
			return;
		}
		if (this.IsSpectating())
		{
			using (TimeWarning.New("Tick_Spectator", 0))
			{
				this.Tick_Spectator();
			}
			return;
		}
		if (this.IsDead())
		{
			return;
		}
		if (this.IsSleeping())
		{
			if (this.serverInput.WasJustPressed(BUTTON.FIRE_PRIMARY) || this.serverInput.WasJustPressed(BUTTON.FIRE_SECONDARY) || this.serverInput.WasJustPressed(BUTTON.JUMP) || this.serverInput.WasJustPressed(BUTTON.DUCK))
			{
				this.EndSleeping();
				base.SendNetworkUpdateImmediate(false);
			}
			this.UpdateActiveItem(default(ItemId));
			return;
		}
		this.UpdateActiveItem(msg.activeItem);
		this.UpdateModelStateFromTick(msg);
		if (this.IsIncapacitated())
		{
			return;
		}
		if (this.isMounted)
		{
			this.GetMounted().PlayerServerInput(this.serverInput, this);
		}
		this.UpdatePositionFromTick(msg, wasPlayerStalled);
		this.UpdateRotationFromTick(msg);
	}

	// Token: 0x0600066C RID: 1644 RVA: 0x00045108 File Offset: 0x00043308
	public void UpdateActiveItem(ItemId itemID)
	{
		Assert.IsTrue(base.isServer, "Realm should be server!");
		if (this.svActiveItemID == itemID)
		{
			return;
		}
		if (this.equippingBlocked)
		{
			itemID = default(ItemId);
		}
		global::Item item = this.inventory.containerBelt.FindItemByUID(itemID);
		if (this.IsItemHoldRestricted(item))
		{
			itemID = default(ItemId);
		}
		global::Item activeItem = this.GetActiveItem();
		this.svActiveItemID = default(ItemId);
		if (activeItem != null)
		{
			global::HeldEntity heldEntity = activeItem.GetHeldEntity() as global::HeldEntity;
			if (heldEntity != null)
			{
				heldEntity.SetHeld(false);
			}
		}
		this.svActiveItemID = itemID;
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
		global::Item activeItem2 = this.GetActiveItem();
		if (activeItem2 != null)
		{
			global::HeldEntity heldEntity2 = activeItem2.GetHeldEntity() as global::HeldEntity;
			if (heldEntity2 != null)
			{
				heldEntity2.SetHeld(true);
			}
			this.NotifyGesturesNewItemEquipped();
		}
		this.inventory.UpdatedVisibleHolsteredItems();
	}

	// Token: 0x0600066D RID: 1645 RVA: 0x000451E4 File Offset: 0x000433E4
	internal void UpdateModelStateFromTick(PlayerTick tick)
	{
		if (tick.modelState == null)
		{
			return;
		}
		if (ModelState.Equal(this.modelStateTick, tick.modelState))
		{
			return;
		}
		if (this.modelStateTick != null)
		{
			this.modelStateTick.ResetToPool();
		}
		this.modelStateTick = tick.modelState;
		tick.modelState = null;
		this.tickNeedsFinalizing = true;
	}

	// Token: 0x0600066E RID: 1646 RVA: 0x0004523C File Offset: 0x0004343C
	internal void UpdatePositionFromTick(PlayerTick tick, bool wasPlayerStalled)
	{
		if (tick.position.IsNaNOrInfinity() || tick.eyePos.IsNaNOrInfinity())
		{
			this.Kick("Kicked: Invalid Position");
			return;
		}
		if (tick.parentID != this.parentEntity.uid)
		{
			return;
		}
		if (this.isMounted || (this.modelState != null && this.modelState.mounted) || (this.modelStateTick != null && this.modelStateTick.mounted))
		{
			return;
		}
		if (wasPlayerStalled)
		{
			float num = Vector3.Distance(tick.position, this.tickInterpolator.EndPoint);
			if (num > 0.01f)
			{
				global::AntiHack.ResetTimer(this);
			}
			if (num > 0.5f)
			{
				base.ClientRPCPlayer<Vector3, NetworkableId>(null, this, "ForcePositionToParentOffset", this.tickInterpolator.EndPoint, this.parentEntity.uid);
			}
			return;
		}
		if ((this.modelState == null || !this.modelState.flying || (!this.IsAdmin && !this.IsDeveloper)) && Vector3.Distance(tick.position, this.tickInterpolator.EndPoint) > 5f)
		{
			global::AntiHack.ResetTimer(this);
			base.ClientRPCPlayer<Vector3, NetworkableId>(null, this, "ForcePositionToParentOffset", this.tickInterpolator.EndPoint, this.parentEntity.uid);
			return;
		}
		this.tickInterpolator.AddPoint(tick.position);
		this.tickNeedsFinalizing = true;
	}

	// Token: 0x0600066F RID: 1647 RVA: 0x00045394 File Offset: 0x00043594
	internal void UpdateRotationFromTick(PlayerTick tick)
	{
		if (tick.inputState == null)
		{
			return;
		}
		if (tick.inputState.aimAngles.IsNaNOrInfinity())
		{
			this.Kick("Kicked: Invalid Rotation");
			return;
		}
		this.tickViewAngles = tick.inputState.aimAngles;
		this.tickNeedsFinalizing = true;
	}

	// Token: 0x06000670 RID: 1648 RVA: 0x000453E0 File Offset: 0x000435E0
	public void UpdateEstimatedVelocity(Vector3 lastPos, Vector3 currentPos, float deltaTime)
	{
		this.estimatedVelocity = (currentPos - lastPos) / deltaTime;
		this.estimatedSpeed = this.estimatedVelocity.magnitude;
		this.estimatedSpeed2D = this.estimatedVelocity.Magnitude2D();
		if (this.estimatedSpeed < 0.01f)
		{
			this.estimatedSpeed = 0f;
		}
		if (this.estimatedSpeed2D < 0.01f)
		{
			this.estimatedSpeed2D = 0f;
		}
	}

	// Token: 0x170000BD RID: 189
	// (get) Token: 0x06000671 RID: 1649 RVA: 0x00045455 File Offset: 0x00043655
	// (set) Token: 0x06000672 RID: 1650 RVA: 0x0004545D File Offset: 0x0004365D
	public Vector3 tickViewAngles { get; private set; }

	// Token: 0x170000BE RID: 190
	// (get) Token: 0x06000673 RID: 1651 RVA: 0x00045466 File Offset: 0x00043666
	public int tickHistoryCapacity
	{
		get
		{
			return Mathf.Max(1, Mathf.CeilToInt(this.ticksPerSecond.Calculate() * ConVar.AntiHack.tickhistorytime));
		}
	}

	// Token: 0x170000BF RID: 191
	// (get) Token: 0x06000674 RID: 1652 RVA: 0x00045486 File Offset: 0x00043686
	public Matrix4x4 tickHistoryMatrix
	{
		get
		{
			if (!base.transform.parent)
			{
				return Matrix4x4.identity;
			}
			return base.transform.parent.localToWorldMatrix;
		}
	}

	// Token: 0x06000675 RID: 1653 RVA: 0x000454B0 File Offset: 0x000436B0
	private void FinalizeTick(float deltaTime)
	{
		this.tickDeltaTime += deltaTime;
		if (this.IsReceivingSnapshot)
		{
			return;
		}
		if (!this.tickNeedsFinalizing)
		{
			return;
		}
		this.tickNeedsFinalizing = false;
		using (TimeWarning.New("ModelState", 0))
		{
			if (this.modelStateTick != null)
			{
				if (this.modelStateTick.flying && !this.IsAdmin && !this.IsDeveloper)
				{
					global::AntiHack.NoteAdminHack(this);
				}
				if (this.modelStateTick.inheritedVelocity != Vector3.zero && base.FindTrigger<TriggerForce>() == null)
				{
					this.modelStateTick.inheritedVelocity = Vector3.zero;
				}
				if (this.modelState != null)
				{
					if (ConVar.AntiHack.modelstate && this.TriggeredAntiHack(1f, float.PositiveInfinity))
					{
						this.modelStateTick.ducked = this.modelState.ducked;
					}
					this.modelState.ResetToPool();
					this.modelState = null;
				}
				this.modelState = this.modelStateTick;
				this.modelStateTick = null;
				this.UpdateModelState();
			}
		}
		using (TimeWarning.New("Transform", 0))
		{
			this.UpdateEstimatedVelocity(this.tickInterpolator.StartPoint, this.tickInterpolator.EndPoint, this.tickDeltaTime);
			bool flag = this.tickInterpolator.StartPoint != this.tickInterpolator.EndPoint;
			bool flag2 = this.tickViewAngles != this.viewAngles;
			if (flag)
			{
				if (global::AntiHack.ValidateMove(this, this.tickInterpolator, this.tickDeltaTime))
				{
					base.transform.localPosition = this.tickInterpolator.EndPoint;
					this.ticksPerSecond.Increment();
					this.tickHistory.AddPoint(this.tickInterpolator.EndPoint, this.tickHistoryCapacity);
					global::AntiHack.FadeViolations(this, this.tickDeltaTime);
				}
				else
				{
					flag = false;
					if (ConVar.AntiHack.forceposition)
					{
						base.ClientRPCPlayer<Vector3, NetworkableId>(null, this, "ForcePositionToParentOffset", base.transform.localPosition, this.parentEntity.uid);
					}
				}
			}
			this.tickInterpolator.Reset(base.transform.localPosition);
			if (flag2)
			{
				this.viewAngles = this.tickViewAngles;
				base.transform.rotation = Quaternion.identity;
				base.transform.hasChanged = true;
			}
			if (flag || flag2)
			{
				this.eyes.NetworkUpdate(Quaternion.Euler(this.viewAngles));
				base.NetworkPositionTick();
			}
			global::AntiHack.ValidateEyeHistory(this);
		}
		using (TimeWarning.New("ModelState", 0))
		{
			if (this.modelState != null)
			{
				this.modelState.waterLevel = this.WaterFactor();
			}
		}
		using (TimeWarning.New("EACStateUpdate", 0))
		{
			this.EACStateUpdate();
		}
		using (TimeWarning.New("AntiHack.EnforceViolations", 0))
		{
			global::AntiHack.EnforceViolations(this);
		}
		this.tickDeltaTime = 0f;
	}

	// Token: 0x06000676 RID: 1654 RVA: 0x00045818 File Offset: 0x00043A18
	public uint GetUnderwearSkin()
	{
		uint infoInt = (uint)this.GetInfoInt("client.underwearskin", 0);
		if (infoInt != this.lastValidUnderwearSkin && UnityEngine.Time.time > this.nextUnderwearValidationTime)
		{
			UnderwearManifest underwearManifest = UnderwearManifest.Get();
			this.nextUnderwearValidationTime = UnityEngine.Time.time + 0.2f;
			Underwear underwear = underwearManifest.GetUnderwear(infoInt);
			if (underwear == null)
			{
				this.lastValidUnderwearSkin = 0U;
			}
			else if (Underwear.Validate(underwear, this))
			{
				this.lastValidUnderwearSkin = infoInt;
			}
		}
		return this.lastValidUnderwearSkin;
	}

	// Token: 0x06000677 RID: 1655 RVA: 0x00045890 File Offset: 0x00043A90
	[global::BaseEntity.RPC_Server]
	public void ServerRPC_UnderwearChange(global::BaseEntity.RPCMessage msg)
	{
		if (msg.player != this)
		{
			return;
		}
		uint num = this.lastValidUnderwearSkin;
		uint underwearSkin = this.GetUnderwearSkin();
		if (num != underwearSkin)
		{
			base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
		}
	}

	// Token: 0x06000678 RID: 1656 RVA: 0x000458C3 File Offset: 0x00043AC3
	public bool IsWounded()
	{
		return this.HasPlayerFlag(global::BasePlayer.PlayerFlags.Wounded);
	}

	// Token: 0x06000679 RID: 1657 RVA: 0x000458CD File Offset: 0x00043ACD
	public bool IsCrawling()
	{
		return this.HasPlayerFlag(global::BasePlayer.PlayerFlags.Wounded) && !this.HasPlayerFlag(global::BasePlayer.PlayerFlags.Incapacitated);
	}

	// Token: 0x0600067A RID: 1658 RVA: 0x000458E9 File Offset: 0x00043AE9
	public bool IsIncapacitated()
	{
		return this.HasPlayerFlag(global::BasePlayer.PlayerFlags.Incapacitated);
	}

	// Token: 0x170000C0 RID: 192
	// (get) Token: 0x0600067B RID: 1659 RVA: 0x000458F6 File Offset: 0x00043AF6
	public float TimeSinceWoundedStarted
	{
		get
		{
			return UnityEngine.Time.realtimeSinceStartup - this.lastWoundedStartTime;
		}
	}

	// Token: 0x0600067C RID: 1660 RVA: 0x00045904 File Offset: 0x00043B04
	private bool WoundInsteadOfDying(HitInfo info)
	{
		if (!this.EligibleForWounding(info))
		{
			return false;
		}
		this.BecomeWounded(info);
		return true;
	}

	// Token: 0x0600067D RID: 1661 RVA: 0x00045919 File Offset: 0x00043B19
	private void ResetWoundingVars()
	{
		base.CancelInvoke(new Action(this.WoundingTick));
		this.woundedDuration = 0f;
		this.lastWoundedStartTime = float.NegativeInfinity;
		this.healingWhileCrawling = 0f;
		this.woundedByFallDamage = false;
	}

	// Token: 0x0600067E RID: 1662 RVA: 0x00045958 File Offset: 0x00043B58
	public virtual bool EligibleForWounding(HitInfo info)
	{
		if (!ConVar.Server.woundingenabled)
		{
			return false;
		}
		if (this.IsWounded())
		{
			return false;
		}
		if (this.IsSleeping())
		{
			return false;
		}
		if (this.isMounted)
		{
			return false;
		}
		if (info == null)
		{
			return false;
		}
		if (!this.IsWounded() && UnityEngine.Time.realtimeSinceStartup - this.lastWoundedStartTime < ConVar.Server.rewounddelay)
		{
			return false;
		}
		BaseGameMode activeGameMode = BaseGameMode.GetActiveGameMode(true);
		if (activeGameMode && !activeGameMode.allowWounding)
		{
			return false;
		}
		if (this.triggers != null)
		{
			for (int i = 0; i < this.triggers.Count; i++)
			{
				if (this.triggers[i] is IHurtTrigger)
				{
					return false;
				}
			}
		}
		if (info.WeaponPrefab is BaseMelee)
		{
			return true;
		}
		if (info.WeaponPrefab is global::BaseProjectile)
		{
			return !info.isHeadshot;
		}
		DamageType majorityDamageType = info.damageTypes.GetMajorityDamageType();
		return majorityDamageType != DamageType.Suicide && (majorityDamageType == DamageType.Fall || majorityDamageType == DamageType.Bite || majorityDamageType == DamageType.Bleeding || majorityDamageType == DamageType.Hunger || majorityDamageType == DamageType.Thirst || majorityDamageType == DamageType.Poison);
	}

	// Token: 0x0600067F RID: 1663 RVA: 0x00045A5C File Offset: 0x00043C5C
	public void BecomeWounded(HitInfo info = null)
	{
		if (this.IsWounded())
		{
			return;
		}
		bool flag = info != null && info.damageTypes.GetMajorityDamageType() == DamageType.Fall;
		if (this.IsCrawling())
		{
			this.woundedByFallDamage = (this.woundedByFallDamage || flag);
			this.GoToIncapacitated(info);
			return;
		}
		this.woundedByFallDamage = flag;
		if (flag || !ConVar.Server.crawlingenabled)
		{
			this.GoToIncapacitated(info);
			return;
		}
		this.GoToCrawling(info);
	}

	// Token: 0x06000680 RID: 1664 RVA: 0x00045AC6 File Offset: 0x00043CC6
	public void StopWounded(global::BasePlayer source = null)
	{
		if (!this.IsWounded())
		{
			return;
		}
		this.RecoverFromWounded();
		base.CancelInvoke(new Action(this.WoundingTick));
		EACServer.LogPlayerRevive(source, this);
	}

	// Token: 0x06000681 RID: 1665 RVA: 0x00045AF0 File Offset: 0x00043CF0
	public void ProlongWounding(float delay)
	{
		this.woundedDuration = Mathf.Max(this.woundedDuration, Mathf.Min(this.TimeSinceWoundedStarted + delay, this.woundedDuration + delay));
	}

	// Token: 0x06000682 RID: 1666 RVA: 0x00045B18 File Offset: 0x00043D18
	private void WoundingTick()
	{
		using (TimeWarning.New("WoundingTick", 0))
		{
			if (!this.IsDead())
			{
				if (!Player.woundforever && this.TimeSinceWoundedStarted >= this.woundedDuration)
				{
					float num = this.IsIncapacitated() ? ConVar.Server.incapacitatedrecoverchance : ConVar.Server.woundedrecoverchance;
					float t = (this.metabolism.hydration.Fraction() + this.metabolism.calories.Fraction()) / 2f;
					float num2 = Mathf.Lerp(0f, ConVar.Server.woundedmaxfoodandwaterbonus, t);
					float num3 = Mathf.Clamp01(num + num2);
					if (UnityEngine.Random.value < num3)
					{
						this.RecoverFromWounded();
					}
					else if (this.woundedByFallDamage)
					{
						this.Die(null);
					}
					else
					{
						ItemDefinition itemDefinition = ItemManager.FindItemDefinition("largemedkit");
						global::Item item = this.inventory.containerBelt.FindItemByItemID(itemDefinition.itemid);
						if (item != null)
						{
							item.UseItem(1);
							this.RecoverFromWounded();
						}
						else
						{
							this.Die(null);
						}
					}
				}
				else
				{
					if (this.IsSwimming() && this.IsCrawling())
					{
						this.GoToIncapacitated(null);
					}
					base.Invoke(new Action(this.WoundingTick), 1f);
				}
			}
		}
	}

	// Token: 0x06000683 RID: 1667 RVA: 0x00045C6C File Offset: 0x00043E6C
	private void GoToCrawling(HitInfo info)
	{
		base.health = (float)UnityEngine.Random.Range(ConVar.Server.crawlingminimumhealth, ConVar.Server.crawlingmaximumhealth);
		this.metabolism.bleeding.value = 0f;
		this.healingWhileCrawling = 0f;
		this.WoundedStartSharedCode(info);
		this.StartWoundedTick(40, 50);
		base.SendNetworkUpdateImmediate(false);
	}

	// Token: 0x06000684 RID: 1668 RVA: 0x00045CC8 File Offset: 0x00043EC8
	public void GoToIncapacitated(HitInfo info)
	{
		if (!this.IsWounded())
		{
			this.WoundedStartSharedCode(info);
		}
		base.health = UnityEngine.Random.Range(2f, 6f);
		this.metabolism.bleeding.value = 0f;
		this.healingWhileCrawling = 0f;
		this.SetPlayerFlag(global::BasePlayer.PlayerFlags.Incapacitated, true);
		this.SetServerFall(true);
		this.StartWoundedTick(10, 25);
		base.SendNetworkUpdateImmediate(false);
	}

	// Token: 0x06000685 RID: 1669 RVA: 0x00045D40 File Offset: 0x00043F40
	private void WoundedStartSharedCode(HitInfo info)
	{
		this.stats.Add("wounded", 1, (global::Stats)5);
		this.SetPlayerFlag(global::BasePlayer.PlayerFlags.Wounded, true);
		if (BaseGameMode.GetActiveGameMode(base.isServer))
		{
			BaseGameMode.GetActiveGameMode(base.isServer).OnPlayerWounded(info.InitiatorPlayer, this, info);
		}
	}

	// Token: 0x06000686 RID: 1670 RVA: 0x00045D92 File Offset: 0x00043F92
	private void StartWoundedTick(int minTime, int maxTime)
	{
		this.woundedDuration = (float)UnityEngine.Random.Range(minTime, maxTime + 1);
		this.lastWoundedStartTime = UnityEngine.Time.realtimeSinceStartup;
		base.Invoke(new Action(this.WoundingTick), 1f);
	}

	// Token: 0x06000687 RID: 1671 RVA: 0x00045DC8 File Offset: 0x00043FC8
	private void RecoverFromWounded()
	{
		if (this.IsCrawling())
		{
			base.health = UnityEngine.Random.Range(2f, 6f) + this.healingWhileCrawling;
		}
		this.healingWhileCrawling = 0f;
		this.SetPlayerFlag(global::BasePlayer.PlayerFlags.Wounded, false);
		this.SetPlayerFlag(global::BasePlayer.PlayerFlags.Incapacitated, false);
		if (BaseGameMode.GetActiveGameMode(base.isServer))
		{
			BaseGameMode.GetActiveGameMode(base.isServer).OnPlayerRevived(null, this);
		}
	}

	// Token: 0x06000688 RID: 1672 RVA: 0x00045E3D File Offset: 0x0004403D
	private bool WoundingCausingImmortality(HitInfo info)
	{
		return this.IsWounded() && this.TimeSinceWoundedStarted <= 0.25f && (info == null || info.damageTypes.GetMajorityDamageType() != DamageType.Fall);
	}

	// Token: 0x06000689 RID: 1673 RVA: 0x000037E7 File Offset: 0x000019E7
	public override global::BasePlayer ToPlayer()
	{
		return this;
	}

	// Token: 0x170000C1 RID: 193
	// (get) Token: 0x0600068A RID: 1674 RVA: 0x00045E6D File Offset: 0x0004406D
	public Network.Connection Connection
	{
		get
		{
			if (this.net != null)
			{
				return this.net.connection;
			}
			return null;
		}
	}

	// Token: 0x170000C2 RID: 194
	// (get) Token: 0x0600068B RID: 1675 RVA: 0x00045E84 File Offset: 0x00044084
	public bool IsBot
	{
		get
		{
			return this.userID < 10000000UL;
		}
	}

	// Token: 0x170000C3 RID: 195
	// (get) Token: 0x0600068C RID: 1676 RVA: 0x00045E94 File Offset: 0x00044094
	// (set) Token: 0x0600068D RID: 1677 RVA: 0x00045EAD File Offset: 0x000440AD
	public string displayName
	{
		get
		{
			return NameHelper.Get(this.userID, this._displayName, base.isClient);
		}
		set
		{
			if (this._lastSetName == value)
			{
				return;
			}
			this._lastSetName = value;
			this._displayName = global::BasePlayer.SanitizePlayerNameString(value, this.userID);
		}
	}

	// Token: 0x0600068E RID: 1678 RVA: 0x00045ED7 File Offset: 0x000440D7
	public static string SanitizePlayerNameString(string playerName, ulong userId)
	{
		playerName = playerName.ToPrintable(32).EscapeRichText().Trim();
		if (string.IsNullOrWhiteSpace(playerName))
		{
			playerName = userId.ToString();
		}
		return playerName;
	}

	// Token: 0x0600068F RID: 1679 RVA: 0x00045F00 File Offset: 0x00044100
	public bool IsGod()
	{
		return base.isServer && (this.IsAdmin || this.IsDeveloper) && this.IsConnected && this.net.connection != null && this.net.connection.info.GetBool("global.god", false);
	}

	// Token: 0x06000690 RID: 1680 RVA: 0x00045F5A File Offset: 0x0004415A
	public override Quaternion GetNetworkRotation()
	{
		if (base.isServer)
		{
			return Quaternion.Euler(this.viewAngles);
		}
		return Quaternion.identity;
	}

	// Token: 0x06000691 RID: 1681 RVA: 0x00045F75 File Offset: 0x00044175
	public bool CanInteract()
	{
		return this.CanInteract(false);
	}

	// Token: 0x06000692 RID: 1682 RVA: 0x00045F7E File Offset: 0x0004417E
	public bool CanInteract(bool usableWhileCrawling)
	{
		return !this.IsDead() && !this.IsSleeping() && !this.IsSpectating() && (usableWhileCrawling ? (!this.IsIncapacitated()) : (!this.IsWounded())) && !this.HasActiveTelephone;
	}

	// Token: 0x06000693 RID: 1683 RVA: 0x00045FBC File Offset: 0x000441BC
	public override float StartHealth()
	{
		return UnityEngine.Random.Range(50f, 60f);
	}

	// Token: 0x06000694 RID: 1684 RVA: 0x0002A2EC File Offset: 0x000284EC
	public override float StartMaxHealth()
	{
		return 100f;
	}

	// Token: 0x06000695 RID: 1685 RVA: 0x00045FCD File Offset: 0x000441CD
	public override float MaxHealth()
	{
		return 100f * (1f + ((this.modifiers != null) ? this.modifiers.GetValue(global::Modifier.ModifierType.Max_Health, 0f) : 0f));
	}

	// Token: 0x06000696 RID: 1686 RVA: 0x00046001 File Offset: 0x00044201
	public override float MaxVelocity()
	{
		if (this.IsSleeping())
		{
			return 0f;
		}
		if (this.isMounted)
		{
			return this.GetMounted().MaxVelocity();
		}
		return this.GetMaxSpeed();
	}

	// Token: 0x06000697 RID: 1687 RVA: 0x0004602C File Offset: 0x0004422C
	public override OBB WorldSpaceBounds()
	{
		if (this.IsSleeping())
		{
			Vector3 center = this.bounds.center;
			Vector3 size = this.bounds.size;
			center.y /= 2f;
			size.y /= 2f;
			return new OBB(base.transform.position, base.transform.lossyScale, base.transform.rotation, new Bounds(center, size));
		}
		return base.WorldSpaceBounds();
	}

	// Token: 0x06000698 RID: 1688 RVA: 0x000460B0 File Offset: 0x000442B0
	public Vector3 GetMountVelocity()
	{
		BaseMountable baseMountable = this.GetMounted();
		if (!(baseMountable != null))
		{
			return Vector3.zero;
		}
		return baseMountable.GetWorldVelocity();
	}

	// Token: 0x06000699 RID: 1689 RVA: 0x000460DC File Offset: 0x000442DC
	public override Vector3 GetInheritedProjectileVelocity(Vector3 direction)
	{
		BaseMountable baseMountable = this.GetMounted();
		if (!baseMountable)
		{
			return base.GetInheritedProjectileVelocity(direction);
		}
		return baseMountable.GetInheritedProjectileVelocity(direction);
	}

	// Token: 0x0600069A RID: 1690 RVA: 0x00046108 File Offset: 0x00044308
	public override Vector3 GetInheritedThrowVelocity(Vector3 direction)
	{
		BaseMountable baseMountable = this.GetMounted();
		if (!baseMountable)
		{
			return base.GetInheritedThrowVelocity(direction);
		}
		return baseMountable.GetInheritedThrowVelocity(direction);
	}

	// Token: 0x0600069B RID: 1691 RVA: 0x00046134 File Offset: 0x00044334
	public override Vector3 GetInheritedDropVelocity()
	{
		BaseMountable baseMountable = this.GetMounted();
		if (!baseMountable)
		{
			return base.GetInheritedDropVelocity();
		}
		return baseMountable.GetInheritedDropVelocity();
	}

	// Token: 0x0600069C RID: 1692 RVA: 0x00046160 File Offset: 0x00044360
	public override void PreInitShared()
	{
		base.PreInitShared();
		this.cachedProtection = ScriptableObject.CreateInstance<ProtectionProperties>();
		this.baseProtection = ScriptableObject.CreateInstance<ProtectionProperties>();
		this.inventory = base.GetComponent<global::PlayerInventory>();
		this.blueprints = base.GetComponent<PlayerBlueprints>();
		this.metabolism = base.GetComponent<global::PlayerMetabolism>();
		this.modifiers = base.GetComponent<global::PlayerModifiers>();
		this.playerCollider = base.GetComponent<CapsuleCollider>();
		this.eyes = base.GetComponent<PlayerEyes>();
		this.playerColliderStanding = new global::BasePlayer.CapsuleColliderInfo(this.playerCollider.height, this.playerCollider.radius, this.playerCollider.center);
		this.playerColliderDucked = new global::BasePlayer.CapsuleColliderInfo(1.5f, this.playerCollider.radius, Vector3.up * 0.75f);
		this.playerColliderCrawling = new global::BasePlayer.CapsuleColliderInfo(this.playerCollider.radius, this.playerCollider.radius, Vector3.up * this.playerCollider.radius);
		this.playerColliderLyingDown = new global::BasePlayer.CapsuleColliderInfo(0.4f, this.playerCollider.radius, Vector3.up * 0.2f);
		this.Belt = new PlayerBelt(this);
	}

	// Token: 0x0600069D RID: 1693 RVA: 0x00046293 File Offset: 0x00044493
	public override void DestroyShared()
	{
		UnityEngine.Object.Destroy(this.cachedProtection);
		UnityEngine.Object.Destroy(this.baseProtection);
		base.DestroyShared();
	}

	// Token: 0x0600069E RID: 1694 RVA: 0x000462B4 File Offset: 0x000444B4
	public static void ServerCycle(float deltaTime)
	{
		for (int i = 0; i < global::BasePlayer.activePlayerList.Values.Count; i++)
		{
			if (global::BasePlayer.activePlayerList.Values[i] == null)
			{
				global::BasePlayer.activePlayerList.RemoveAt(i--);
			}
		}
		List<global::BasePlayer> list = Facepunch.Pool.GetList<global::BasePlayer>();
		for (int j = 0; j < global::BasePlayer.activePlayerList.Count; j++)
		{
			list.Add(global::BasePlayer.activePlayerList[j]);
		}
		for (int k = 0; k < list.Count; k++)
		{
			if (!(list[k] == null))
			{
				list[k].ServerUpdate(deltaTime);
			}
		}
		for (int l = 0; l < global::BasePlayer.bots.Count; l++)
		{
			if (!(global::BasePlayer.bots[l] == null))
			{
				global::BasePlayer.bots[l].ServerUpdateBots(deltaTime);
			}
		}
		if (ConVar.Server.idlekick > 0 && ((ServerMgr.AvailableSlots <= 0 && ConVar.Server.idlekickmode == 1) || ConVar.Server.idlekickmode == 2))
		{
			for (int m = 0; m < list.Count; m++)
			{
				if (list[m].IdleTime >= (float)(ConVar.Server.idlekick * 60) && (!list[m].IsAdmin || ConVar.Server.idlekickadmins != 0) && (!list[m].IsDeveloper || ConVar.Server.idlekickadmins != 0))
				{
					list[m].Kick("Idle for " + ConVar.Server.idlekick + " minutes");
				}
			}
		}
		Facepunch.Pool.FreeList<global::BasePlayer>(ref list);
	}

	// Token: 0x0600069F RID: 1695 RVA: 0x00046444 File Offset: 0x00044644
	public bool InSafeZone()
	{
		BaseGameMode activeGameMode = BaseGameMode.GetActiveGameMode(base.isServer);
		return (!(activeGameMode != null) || activeGameMode.safeZone) && base.isServer && this.currentSafeLevel > 0f;
	}

	// Token: 0x060006A0 RID: 1696 RVA: 0x00046488 File Offset: 0x00044688
	public override bool OnStartBeingLooted(global::BasePlayer baseEntity)
	{
		if (baseEntity.InSafeZone() && baseEntity.userID != this.userID)
		{
			return false;
		}
		if (global::RelationshipManager.ServerInstance != null)
		{
			if ((this.IsSleeping() || this.IsIncapacitated()) && !global::RelationshipManager.ServerInstance.HasRelations(baseEntity.userID, this.userID))
			{
				global::RelationshipManager.ServerInstance.SetRelationship(baseEntity, this, global::RelationshipManager.RelationshipType.Acquaintance, 1, false);
			}
			global::RelationshipManager.ServerInstance.SetSeen(baseEntity, this);
		}
		if (this.IsCrawling())
		{
			this.GoToIncapacitated(null);
		}
		return base.OnStartBeingLooted(baseEntity);
	}

	// Token: 0x060006A1 RID: 1697 RVA: 0x00046516 File Offset: 0x00044716
	public Bounds GetBounds(bool ducked)
	{
		return new Bounds(base.transform.position + this.GetOffset(ducked), this.GetSize(ducked));
	}

	// Token: 0x060006A2 RID: 1698 RVA: 0x0004653B File Offset: 0x0004473B
	public Bounds GetBounds()
	{
		return this.GetBounds(this.modelState.ducked);
	}

	// Token: 0x060006A3 RID: 1699 RVA: 0x0004654E File Offset: 0x0004474E
	public Vector3 GetCenter(bool ducked)
	{
		return base.transform.position + this.GetOffset(ducked);
	}

	// Token: 0x060006A4 RID: 1700 RVA: 0x00046567 File Offset: 0x00044767
	public Vector3 GetCenter()
	{
		return this.GetCenter(this.modelState.ducked);
	}

	// Token: 0x060006A5 RID: 1701 RVA: 0x0004657A File Offset: 0x0004477A
	public Vector3 GetOffset(bool ducked)
	{
		if (ducked)
		{
			return new Vector3(0f, 0.55f, 0f);
		}
		return new Vector3(0f, 0.9f, 0f);
	}

	// Token: 0x060006A6 RID: 1702 RVA: 0x000465A8 File Offset: 0x000447A8
	public Vector3 GetOffset()
	{
		return this.GetOffset(this.modelState.ducked);
	}

	// Token: 0x060006A7 RID: 1703 RVA: 0x000465BB File Offset: 0x000447BB
	public Vector3 GetSize(bool ducked)
	{
		if (ducked)
		{
			return new Vector3(1f, 1.1f, 1f);
		}
		return new Vector3(1f, 1.8f, 1f);
	}

	// Token: 0x060006A8 RID: 1704 RVA: 0x000465E9 File Offset: 0x000447E9
	public Vector3 GetSize()
	{
		return this.GetSize(this.modelState.ducked);
	}

	// Token: 0x060006A9 RID: 1705 RVA: 0x000465FC File Offset: 0x000447FC
	public float GetHeight(bool ducked)
	{
		if (ducked)
		{
			return 1.1f;
		}
		return 1.8f;
	}

	// Token: 0x060006AA RID: 1706 RVA: 0x0004660C File Offset: 0x0004480C
	public float GetHeight()
	{
		return this.GetHeight(this.modelState.ducked);
	}

	// Token: 0x060006AB RID: 1707 RVA: 0x00028E70 File Offset: 0x00027070
	public float GetRadius()
	{
		return 0.5f;
	}

	// Token: 0x060006AC RID: 1708 RVA: 0x0004661F File Offset: 0x0004481F
	public float GetJumpHeight()
	{
		return 1.5f;
	}

	// Token: 0x060006AD RID: 1709 RVA: 0x00046626 File Offset: 0x00044826
	public override Vector3 TriggerPoint()
	{
		return base.transform.position + this.NoClipOffset();
	}

	// Token: 0x060006AE RID: 1710 RVA: 0x0004663E File Offset: 0x0004483E
	public Vector3 NoClipOffset()
	{
		return new Vector3(0f, this.GetHeight(true) - this.GetRadius(), 0f);
	}

	// Token: 0x060006AF RID: 1711 RVA: 0x0004665D File Offset: 0x0004485D
	public float NoClipRadius(float margin)
	{
		return this.GetRadius() - margin;
	}

	// Token: 0x060006B0 RID: 1712 RVA: 0x00046667 File Offset: 0x00044867
	public float MaxDeployDistance(global::Item item)
	{
		return 8f;
	}

	// Token: 0x060006B1 RID: 1713 RVA: 0x0004666E File Offset: 0x0004486E
	public float GetMinSpeed()
	{
		return this.GetSpeed(0f, 0f, 1f);
	}

	// Token: 0x060006B2 RID: 1714 RVA: 0x00046685 File Offset: 0x00044885
	public float GetMaxSpeed()
	{
		return this.GetSpeed(1f, 0f, 0f);
	}

	// Token: 0x060006B3 RID: 1715 RVA: 0x0004669C File Offset: 0x0004489C
	public float GetSpeed(float running, float ducking, float crawling)
	{
		float num = 1f;
		num -= this.clothingMoveSpeedReduction;
		if (this.IsSwimming())
		{
			num += this.clothingWaterSpeedBonus;
		}
		if (crawling > 0f)
		{
			return Mathf.Lerp(2.8f, 0.72f, crawling) * num;
		}
		return Mathf.Lerp(Mathf.Lerp(2.8f, 5.5f, running), 1.7f, ducking) * num;
	}

	// Token: 0x060006B4 RID: 1716 RVA: 0x00046704 File Offset: 0x00044904
	public override void OnAttacked(HitInfo info)
	{
		float health = base.health;
		if (this.InSafeZone() && !this.IsHostile() && info.Initiator != null && info.Initiator != this)
		{
			info.damageTypes.ScaleAll(0f);
		}
		if (base.isServer)
		{
			HitArea boneArea = info.boneArea;
			if (boneArea != (HitArea)(-1))
			{
				List<global::Item> list = Facepunch.Pool.GetList<global::Item>();
				list.AddRange(this.inventory.containerWear.itemList);
				for (int i = 0; i < list.Count; i++)
				{
					global::Item item = list[i];
					if (item != null)
					{
						ItemModWearable component = item.info.GetComponent<ItemModWearable>();
						if (!(component == null) && component.ProtectsArea(boneArea))
						{
							item.OnAttacked(info);
						}
					}
				}
				Facepunch.Pool.FreeList<global::Item>(ref list);
				this.inventory.ServerUpdate(0f);
			}
		}
		base.OnAttacked(info);
		if (base.isServer && base.isServer && info.hasDamage)
		{
			if (!info.damageTypes.Has(DamageType.Bleeding) && info.damageTypes.IsBleedCausing() && !this.IsWounded() && !this.IsImmortalTo(info))
			{
				this.metabolism.bleeding.Add(info.damageTypes.Total() * 0.2f);
			}
			if (this.isMounted)
			{
				this.GetMounted().MounteeTookDamage(this, info);
			}
			this.CheckDeathCondition(info);
			if (this.net != null && this.net.connection != null)
			{
				base.ClientRPCPlayer(null, this, "TakeDamageHit");
			}
			string a = StringPool.Get(info.HitBone);
			bool flag = Vector3.Dot((info.PointEnd - info.PointStart).normalized, this.eyes.BodyForward()) > 0.4f;
			global::BasePlayer initiatorPlayer = info.InitiatorPlayer;
			if (initiatorPlayer && !info.damageTypes.IsMeleeType())
			{
				initiatorPlayer.LifeStoryShotHit(info.Weapon);
			}
			if (info.isHeadshot)
			{
				if (flag)
				{
					base.SignalBroadcast(global::BaseEntity.Signal.Flinch_RearHead, string.Empty, null);
				}
				else
				{
					base.SignalBroadcast(global::BaseEntity.Signal.Flinch_Head, string.Empty, null);
				}
				Effect.server.Run("assets/bundled/prefabs/fx/headshot.prefab", this, 0U, new Vector3(0f, 2f, 0f), Vector3.zero, (initiatorPlayer != null) ? initiatorPlayer.net.connection : null, false);
				if (!initiatorPlayer)
				{
					goto IL_320;
				}
				initiatorPlayer.stats.Add("headshot", 1, (global::Stats)5);
				if (!initiatorPlayer.IsBeingSpectated)
				{
					goto IL_320;
				}
				using (List<global::BaseEntity>.Enumerator enumerator = initiatorPlayer.children.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						global::BasePlayer basePlayer;
						if ((basePlayer = (enumerator.Current as global::BasePlayer)) != null)
						{
							basePlayer.ClientRPCPlayer(null, basePlayer, "SpectatedPlayerHeadshot");
						}
					}
					goto IL_320;
				}
			}
			if (flag)
			{
				base.SignalBroadcast(global::BaseEntity.Signal.Flinch_RearTorso, string.Empty, null);
			}
			else if (a == "spine" || a == "spine2")
			{
				base.SignalBroadcast(global::BaseEntity.Signal.Flinch_Stomach, string.Empty, null);
			}
			else
			{
				base.SignalBroadcast(global::BaseEntity.Signal.Flinch_Chest, string.Empty, null);
			}
		}
		IL_320:
		if (this.stats != null)
		{
			if (this.IsWounded())
			{
				this.stats.combat.LogAttack(info, "wounded", health);
			}
			else if (this.IsDead())
			{
				this.stats.combat.LogAttack(info, "killed", health);
			}
			else
			{
				this.stats.combat.LogAttack(info, "", health);
			}
		}
		if (ConVar.Global.cinematicGingerbreadCorpses)
		{
			info.HitMaterial = ConVar.Global.GingerbreadMaterialID();
		}
	}

	// Token: 0x060006B5 RID: 1717 RVA: 0x00046AB4 File Offset: 0x00044CB4
	private void EnablePlayerCollider()
	{
		if (this.playerCollider.enabled)
		{
			return;
		}
		this.RefreshColliderSize(true);
		this.playerCollider.enabled = true;
	}

	// Token: 0x060006B6 RID: 1718 RVA: 0x00046AD7 File Offset: 0x00044CD7
	private void DisablePlayerCollider()
	{
		if (!this.playerCollider.enabled)
		{
			return;
		}
		base.RemoveFromTriggers();
		this.playerCollider.enabled = false;
	}

	// Token: 0x060006B7 RID: 1719 RVA: 0x00046AFC File Offset: 0x00044CFC
	private void RefreshColliderSize(bool forced)
	{
		if (!forced)
		{
			if (!this.playerCollider.enabled)
			{
				return;
			}
			if (UnityEngine.Time.time < this.nextColliderRefreshTime)
			{
				return;
			}
		}
		this.nextColliderRefreshTime = UnityEngine.Time.time + 0.25f + UnityEngine.Random.Range(-0.05f, 0.05f);
		BaseMountable baseMountable = this.GetMounted();
		global::BasePlayer.CapsuleColliderInfo customPlayerCollider;
		if (baseMountable != null && baseMountable.IsValid())
		{
			if (baseMountable.modifiesPlayerCollider)
			{
				customPlayerCollider = baseMountable.customPlayerCollider;
			}
			else
			{
				customPlayerCollider = this.playerColliderStanding;
			}
		}
		else if (this.IsIncapacitated() || this.IsSleeping())
		{
			customPlayerCollider = this.playerColliderLyingDown;
		}
		else if (this.IsCrawling())
		{
			customPlayerCollider = this.playerColliderCrawling;
		}
		else if (this.modelState.ducked)
		{
			customPlayerCollider = this.playerColliderDucked;
		}
		else
		{
			customPlayerCollider = this.playerColliderStanding;
		}
		if (this.playerCollider.height != customPlayerCollider.height || this.playerCollider.radius != customPlayerCollider.radius || this.playerCollider.center != customPlayerCollider.center)
		{
			this.playerCollider.height = customPlayerCollider.height;
			this.playerCollider.radius = customPlayerCollider.radius;
			this.playerCollider.center = customPlayerCollider.center;
		}
	}

	// Token: 0x060006B8 RID: 1720 RVA: 0x00046C33 File Offset: 0x00044E33
	private void SetPlayerRigidbodyState(bool isEnabled)
	{
		if (isEnabled)
		{
			this.AddPlayerRigidbody();
			return;
		}
		this.RemovePlayerRigidbody();
	}

	// Token: 0x060006B9 RID: 1721 RVA: 0x00046C48 File Offset: 0x00044E48
	private void AddPlayerRigidbody()
	{
		if (this.playerRigidbody == null)
		{
			this.playerRigidbody = base.gameObject.GetComponent<Rigidbody>();
		}
		if (this.playerRigidbody == null)
		{
			this.playerRigidbody = base.gameObject.AddComponent<Rigidbody>();
			this.playerRigidbody.useGravity = false;
			this.playerRigidbody.isKinematic = true;
			this.playerRigidbody.mass = 1f;
			this.playerRigidbody.interpolation = RigidbodyInterpolation.None;
			this.playerRigidbody.collisionDetectionMode = CollisionDetectionMode.Discrete;
		}
	}

	// Token: 0x060006BA RID: 1722 RVA: 0x00046CD4 File Offset: 0x00044ED4
	private void RemovePlayerRigidbody()
	{
		if (this.playerRigidbody == null)
		{
			this.playerRigidbody = base.gameObject.GetComponent<Rigidbody>();
		}
		if (this.playerRigidbody != null)
		{
			base.RemoveFromTriggers();
			UnityEngine.Object.DestroyImmediate(this.playerRigidbody);
			this.playerRigidbody = null;
		}
	}

	// Token: 0x060006BB RID: 1723 RVA: 0x00046D28 File Offset: 0x00044F28
	public bool IsEnsnared()
	{
		if (this.triggers == null)
		{
			return false;
		}
		for (int i = 0; i < this.triggers.Count; i++)
		{
			if (this.triggers[i] is TriggerEnsnare)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x060006BC RID: 1724 RVA: 0x00046D6C File Offset: 0x00044F6C
	public bool IsAttacking()
	{
		global::HeldEntity heldEntity = this.GetHeldEntity();
		if (heldEntity == null)
		{
			return false;
		}
		AttackEntity attackEntity = heldEntity as AttackEntity;
		return !(attackEntity == null) && attackEntity.NextAttackTime - UnityEngine.Time.time > attackEntity.repeatDelay - 1f;
	}

	// Token: 0x060006BD RID: 1725 RVA: 0x00046DB8 File Offset: 0x00044FB8
	public bool CanAttack()
	{
		global::HeldEntity heldEntity = this.GetHeldEntity();
		if (heldEntity == null)
		{
			return false;
		}
		bool flag = this.IsSwimming();
		bool flag2 = heldEntity.CanBeUsedInWater();
		return !this.modelState.onLadder && (flag || this.modelState.onground) && (!flag || flag2) && !this.IsEnsnared();
	}

	// Token: 0x060006BE RID: 1726 RVA: 0x00046E19 File Offset: 0x00045019
	public bool OnLadder()
	{
		return this.modelState.onLadder && !this.IsWounded() && base.FindTrigger<TriggerLadder>();
	}

	// Token: 0x060006BF RID: 1727 RVA: 0x00046E3D File Offset: 0x0004503D
	public bool IsSwimming()
	{
		return this.WaterFactor() >= 0.65f;
	}

	// Token: 0x060006C0 RID: 1728 RVA: 0x00046E4F File Offset: 0x0004504F
	public bool IsHeadUnderwater()
	{
		return this.WaterFactor() > 0.75f;
	}

	// Token: 0x060006C1 RID: 1729 RVA: 0x00046E5E File Offset: 0x0004505E
	public virtual bool IsOnGround()
	{
		return this.modelState.onground;
	}

	// Token: 0x060006C2 RID: 1730 RVA: 0x00046E6B File Offset: 0x0004506B
	public bool IsRunning()
	{
		return this.modelState != null && this.modelState.sprinting;
	}

	// Token: 0x060006C3 RID: 1731 RVA: 0x00046E82 File Offset: 0x00045082
	public bool IsDucked()
	{
		return this.modelState != null && this.modelState.ducked;
	}

	// Token: 0x060006C4 RID: 1732 RVA: 0x00046E99 File Offset: 0x00045099
	public void ShowToast(GameTip.Styles style, Translate.Phrase phrase, params string[] arguments)
	{
		if (base.isServer)
		{
			this.SendConsoleCommand("gametip.showtoast_translated", new object[]
			{
				(int)style,
				phrase.token,
				phrase.english,
				arguments
			});
			return;
		}
	}

	// Token: 0x060006C5 RID: 1733 RVA: 0x00046ED4 File Offset: 0x000450D4
	public void ChatMessage(string msg)
	{
		if (base.isServer)
		{
			this.SendConsoleCommand("chat.add", new object[]
			{
				2,
				0,
				msg
			});
			return;
		}
	}

	// Token: 0x060006C6 RID: 1734 RVA: 0x00046F06 File Offset: 0x00045106
	public void ConsoleMessage(string msg)
	{
		if (base.isServer)
		{
			this.SendConsoleCommand("echo " + msg, Array.Empty<object>());
			return;
		}
	}

	// Token: 0x060006C7 RID: 1735 RVA: 0x0002A2EC File Offset: 0x000284EC
	public override float PenetrationResistance(HitInfo info)
	{
		return 100f;
	}

	// Token: 0x060006C8 RID: 1736 RVA: 0x00046F28 File Offset: 0x00045128
	public override void ScaleDamage(HitInfo info)
	{
		if (this.isMounted)
		{
			this.GetMounted().ScaleDamageForPlayer(this, info);
		}
		if (info.UseProtection)
		{
			HitArea boneArea = info.boneArea;
			if (boneArea != (HitArea)(-1))
			{
				this.cachedProtection.Clear();
				this.cachedProtection.Add(this.inventory.containerWear.itemList, boneArea);
				this.cachedProtection.Multiply(DamageType.Arrow, ConVar.Server.arrowarmor);
				this.cachedProtection.Multiply(DamageType.Bullet, ConVar.Server.bulletarmor);
				this.cachedProtection.Multiply(DamageType.Slash, ConVar.Server.meleearmor);
				this.cachedProtection.Multiply(DamageType.Blunt, ConVar.Server.meleearmor);
				this.cachedProtection.Multiply(DamageType.Stab, ConVar.Server.meleearmor);
				this.cachedProtection.Multiply(DamageType.Bleeding, ConVar.Server.bleedingarmor);
				this.cachedProtection.Scale(info.damageTypes, 1f);
			}
			else
			{
				this.baseProtection.Scale(info.damageTypes, 1f);
			}
		}
		if (info.damageProperties)
		{
			info.damageProperties.ScaleDamage(info);
		}
	}

	// Token: 0x060006C9 RID: 1737 RVA: 0x0004703C File Offset: 0x0004523C
	private void UpdateMoveSpeedFromClothing()
	{
		float num = 0f;
		float num2 = 0f;
		float num3 = 0f;
		bool flag = false;
		bool flag2 = false;
		float num4 = 0f;
		this.eggVision = 0f;
		base.Weight = 0f;
		foreach (global::Item item in this.inventory.containerWear.itemList)
		{
			ItemModWearable component = item.info.GetComponent<ItemModWearable>();
			if (component)
			{
				if (component.blocksAiming)
				{
					flag = true;
				}
				if (component.blocksEquipping)
				{
					flag2 = true;
				}
				num4 += component.accuracyBonus;
				this.eggVision += component.eggVision;
				base.Weight += component.weight;
				if (component.movementProperties != null)
				{
					num2 += component.movementProperties.speedReduction;
					num = Mathf.Max(num, component.movementProperties.minSpeedReduction);
					num3 += component.movementProperties.waterSpeedBonus;
				}
			}
		}
		this.clothingAccuracyBonus = num4;
		this.clothingMoveSpeedReduction = Mathf.Max(num2, num);
		this.clothingBlocksAiming = flag;
		this.clothingWaterSpeedBonus = num3;
		this.equippingBlocked = flag2;
		if (base.isServer && this.equippingBlocked)
		{
			this.UpdateActiveItem(default(ItemId));
		}
	}

	// Token: 0x060006CA RID: 1738 RVA: 0x000471BC File Offset: 0x000453BC
	public virtual void UpdateProtectionFromClothing()
	{
		this.baseProtection.Clear();
		this.baseProtection.Add(this.inventory.containerWear.itemList, (HitArea)(-1));
		float num = 0.16666667f;
		for (int i = 0; i < this.baseProtection.amounts.Length; i++)
		{
			if (i != 17)
			{
				if (i == 22)
				{
					this.baseProtection.amounts[i] = 1f;
				}
				else
				{
					this.baseProtection.amounts[i] *= num;
				}
			}
		}
	}

	// Token: 0x060006CB RID: 1739 RVA: 0x00047242 File Offset: 0x00045442
	public override string Categorize()
	{
		return "player";
	}

	// Token: 0x060006CC RID: 1740 RVA: 0x0004724C File Offset: 0x0004544C
	public override string ToString()
	{
		if (this._name == null)
		{
			if (base.isServer)
			{
				this._name = string.Format("{0}[{1}]", this.displayName, this.userID);
			}
			else
			{
				this._name = base.ShortPrefabName;
			}
		}
		return this._name;
	}

	// Token: 0x060006CD RID: 1741 RVA: 0x000472A0 File Offset: 0x000454A0
	public string GetDebugStatus()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendFormat("Entity: {0}\n", this.ToString());
		stringBuilder.AppendFormat("Name: {0}\n", this.displayName);
		stringBuilder.AppendFormat("SteamID: {0}\n", this.userID);
		foreach (object obj in Enum.GetValues(typeof(global::BasePlayer.PlayerFlags)))
		{
			global::BasePlayer.PlayerFlags playerFlags = (global::BasePlayer.PlayerFlags)obj;
			stringBuilder.AppendFormat("{1}: {0}\n", this.HasPlayerFlag(playerFlags), playerFlags);
		}
		return stringBuilder.ToString();
	}

	// Token: 0x060006CE RID: 1742 RVA: 0x00047360 File Offset: 0x00045560
	public override global::Item GetItem(ItemId itemId)
	{
		if (this.inventory == null)
		{
			return null;
		}
		return this.inventory.FindItemUID(itemId);
	}

	// Token: 0x170000C4 RID: 196
	// (get) Token: 0x060006CF RID: 1743 RVA: 0x0004737E File Offset: 0x0004557E
	public override global::BaseEntity.TraitFlag Traits
	{
		get
		{
			return base.Traits | global::BaseEntity.TraitFlag.Human | global::BaseEntity.TraitFlag.Food | global::BaseEntity.TraitFlag.Meat | global::BaseEntity.TraitFlag.Alive;
		}
	}

	// Token: 0x060006D0 RID: 1744 RVA: 0x00047390 File Offset: 0x00045590
	public override float WaterFactor()
	{
		if (this.GetMounted().IsValid())
		{
			return this.GetMounted().WaterFactorForPlayer(this);
		}
		if (base.GetParentEntity() != null && base.GetParentEntity().BlocksWaterFor(this))
		{
			return 0f;
		}
		float radius = this.playerCollider.radius;
		float num = this.playerCollider.height * 0.5f;
		Vector3 start = this.playerCollider.transform.position + this.playerCollider.transform.rotation * (this.playerCollider.center - Vector3.up * (num - radius));
		Vector3 end = this.playerCollider.transform.position + this.playerCollider.transform.rotation * (this.playerCollider.center + Vector3.up * (num - radius));
		return WaterLevel.Factor(start, end, radius, this);
	}

	// Token: 0x060006D1 RID: 1745 RVA: 0x00047490 File Offset: 0x00045690
	public override float AirFactor()
	{
		float num = (this.WaterFactor() > 0.85f) ? 0f : 1f;
		BaseMountable baseMountable = this.GetMounted();
		if (baseMountable.IsValid() && baseMountable.BlocksWaterFor(this))
		{
			float num2 = baseMountable.AirFactor();
			if (num2 < num)
			{
				num = num2;
			}
		}
		return num;
	}

	// Token: 0x060006D2 RID: 1746 RVA: 0x000474E0 File Offset: 0x000456E0
	public float GetOxygenTime(out ItemModGiveOxygen.AirSupplyType airSupplyType)
	{
		global::BaseVehicle mountedVehicle = this.GetMountedVehicle();
		IAirSupply airSupply;
		if (mountedVehicle.IsValid() && (airSupply = (mountedVehicle as IAirSupply)) != null)
		{
			float airTimeRemaining = airSupply.GetAirTimeRemaining();
			if (airTimeRemaining > 0f)
			{
				airSupplyType = airSupply.AirType;
				return airTimeRemaining;
			}
		}
		foreach (global::Item item in this.inventory.containerWear.itemList)
		{
			IAirSupply componentInChildren = item.info.GetComponentInChildren<IAirSupply>();
			if (componentInChildren != null)
			{
				float airTimeRemaining2 = componentInChildren.GetAirTimeRemaining();
				if (airTimeRemaining2 > 0f)
				{
					airSupplyType = componentInChildren.AirType;
					return airTimeRemaining2;
				}
			}
		}
		airSupplyType = ItemModGiveOxygen.AirSupplyType.Lungs;
		if (this.metabolism.oxygen.value > 0.5f)
		{
			float num = 5f;
			float num2 = Mathf.InverseLerp(0.5f, 1f, this.metabolism.oxygen.value);
			return num * num2;
		}
		return 0f;
	}

	// Token: 0x060006D3 RID: 1747 RVA: 0x000475E4 File Offset: 0x000457E4
	public override bool ShouldInheritNetworkGroup()
	{
		return this.IsSpectating();
	}

	// Token: 0x060006D4 RID: 1748 RVA: 0x000475EC File Offset: 0x000457EC
	public static bool AnyPlayersVisibleToEntity(Vector3 pos, float radius, global::BaseEntity source, Vector3 entityEyePos, bool ignorePlayersWithPriv = false)
	{
		List<RaycastHit> list = Facepunch.Pool.GetList<RaycastHit>();
		List<global::BasePlayer> list2 = Facepunch.Pool.GetList<global::BasePlayer>();
		global::Vis.Entities<global::BasePlayer>(pos, radius, list2, 131072, QueryTriggerInteraction.Collide);
		bool flag = false;
		foreach (global::BasePlayer basePlayer in list2)
		{
			if (!basePlayer.IsSleeping() && basePlayer.IsAlive() && (!basePlayer.IsBuildingAuthed() || !ignorePlayersWithPriv))
			{
				list.Clear();
				GamePhysics.TraceAll(new Ray(basePlayer.eyes.position, (entityEyePos - basePlayer.eyes.position).normalized), 0f, list, 9f, 1218519297, QueryTriggerInteraction.UseGlobal, null);
				for (int i = 0; i < list.Count; i++)
				{
					global::BaseEntity entity = list[i].GetEntity();
					if (entity != null && (entity == source || entity.EqualNetID(source)))
					{
						flag = true;
						break;
					}
					if (!(entity != null) || entity.ShouldBlockProjectiles())
					{
						break;
					}
				}
				if (flag)
				{
					break;
				}
			}
		}
		Facepunch.Pool.FreeList<RaycastHit>(ref list);
		Facepunch.Pool.FreeList<global::BasePlayer>(ref list2);
		return flag;
	}

	// Token: 0x060006D5 RID: 1749 RVA: 0x00047734 File Offset: 0x00045934
	public bool IsStandingOnEntity(global::BaseEntity standingOn, int layerMask)
	{
		if (!this.IsOnGround())
		{
			return false;
		}
		RaycastHit hit;
		if (UnityEngine.Physics.SphereCast(base.transform.position + Vector3.up * (0.25f + this.GetRadius()), this.GetRadius() * 0.95f, Vector3.down, out hit, 4f, layerMask))
		{
			global::BaseEntity entity = hit.GetEntity();
			if (entity != null)
			{
				if (entity.EqualNetID(standingOn))
				{
					return true;
				}
				global::BaseEntity parentEntity = entity.GetParentEntity();
				if (parentEntity != null && parentEntity.EqualNetID(standingOn))
				{
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x060006D6 RID: 1750 RVA: 0x000477C8 File Offset: 0x000459C8
	public void SetActiveTelephone(PhoneController t)
	{
		this.activeTelephone = t;
	}

	// Token: 0x170000C5 RID: 197
	// (get) Token: 0x060006D7 RID: 1751 RVA: 0x000477D1 File Offset: 0x000459D1
	public bool HasActiveTelephone
	{
		get
		{
			return this.activeTelephone != null;
		}
	}

	// Token: 0x170000C6 RID: 198
	// (get) Token: 0x060006D8 RID: 1752 RVA: 0x000477DF File Offset: 0x000459DF
	public bool IsDesigningAI
	{
		get
		{
			return this.designingAIEntity != null;
		}
	}

	// Token: 0x060006D9 RID: 1753 RVA: 0x000477F0 File Offset: 0x000459F0
	public void ClearDesigningAIEntity()
	{
		if (this.IsDesigningAI)
		{
			IAIDesign component = this.designingAIEntity.GetComponent<IAIDesign>();
			if (component != null)
			{
				component.StopDesigning();
			}
		}
		this.designingAIEntity = null;
	}

	// Token: 0x060006DC RID: 1756 RVA: 0x00047B4C File Offset: 0x00045D4C
	[CompilerGenerated]
	private bool <CountWaveTargets>g__CheckPlayer|87_0(global::BasePlayer player, ref global::BasePlayer.<>c__DisplayClass87_0 A_2)
	{
		return !(player == null) && !(player == this) && player.SqrDistance(A_2.position) <= A_2.sqrDistance && Vector3.Dot((player.transform.position - A_2.position).normalized, A_2.forward) >= A_2.minimumDot && !A_2.workingList.Contains(player.net.ID);
	}

	// Token: 0x060006DD RID: 1757 RVA: 0x00047BD4 File Offset: 0x00045DD4
	[CompilerGenerated]
	private bool <FindUnusedPointOfInterestColour>g__HasColour|117_0(int index)
	{
		using (List<MapNote>.Enumerator enumerator = this.State.pointsOfInterest.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.colourIndex == index)
				{
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x060006DE RID: 1758 RVA: 0x00047C34 File Offset: 0x00045E34
	[CompilerGenerated]
	internal static float <CreateCorpse>g__GetFloatBasedOnUserID|387_0(ulong steamid, ulong seed)
	{
		UnityEngine.Random.State state = UnityEngine.Random.state;
		UnityEngine.Random.InitState((int)(seed + steamid));
		float result = UnityEngine.Random.Range(0f, 1f);
		UnityEngine.Random.state = state;
		return result;
	}

	// Token: 0x02000B9E RID: 2974
	public enum CameraMode
	{
		// Token: 0x0400400D RID: 16397
		FirstPerson,
		// Token: 0x0400400E RID: 16398
		ThirdPerson,
		// Token: 0x0400400F RID: 16399
		Eyes,
		// Token: 0x04004010 RID: 16400
		FirstPersonWithArms,
		// Token: 0x04004011 RID: 16401
		DeathCamClassic,
		// Token: 0x04004012 RID: 16402
		Last = 3
	}

	// Token: 0x02000B9F RID: 2975
	public enum NetworkQueue
	{
		// Token: 0x04004014 RID: 16404
		Update,
		// Token: 0x04004015 RID: 16405
		UpdateDistance,
		// Token: 0x04004016 RID: 16406
		Count
	}

	// Token: 0x02000BA0 RID: 2976
	private class NetworkQueueList
	{
		// Token: 0x04004017 RID: 16407
		public HashSet<global::BaseNetworkable> queueInternal = new HashSet<global::BaseNetworkable>();

		// Token: 0x04004018 RID: 16408
		public int MaxLength;

		// Token: 0x06004D1B RID: 19739 RVA: 0x001A013E File Offset: 0x0019E33E
		public bool Contains(global::BaseNetworkable ent)
		{
			return this.queueInternal.Contains(ent);
		}

		// Token: 0x06004D1C RID: 19740 RVA: 0x001A014C File Offset: 0x0019E34C
		public void Add(global::BaseNetworkable ent)
		{
			if (!this.Contains(ent))
			{
				this.queueInternal.Add(ent);
			}
			this.MaxLength = Mathf.Max(this.MaxLength, this.queueInternal.Count);
		}

		// Token: 0x06004D1D RID: 19741 RVA: 0x001A0180 File Offset: 0x0019E380
		public void Add(global::BaseNetworkable[] ent)
		{
			foreach (global::BaseNetworkable ent2 in ent)
			{
				this.Add(ent2);
			}
		}

		// Token: 0x1700066B RID: 1643
		// (get) Token: 0x06004D1E RID: 19742 RVA: 0x001A01A8 File Offset: 0x0019E3A8
		public int Length
		{
			get
			{
				return this.queueInternal.Count;
			}
		}

		// Token: 0x06004D1F RID: 19743 RVA: 0x001A01B8 File Offset: 0x0019E3B8
		public void Clear(Group group)
		{
			using (TimeWarning.New("NetworkQueueList.Clear", 0))
			{
				if (group != null)
				{
					if (!group.isGlobal)
					{
						List<global::BaseNetworkable> list = Facepunch.Pool.GetList<global::BaseNetworkable>();
						foreach (global::BaseNetworkable baseNetworkable in this.queueInternal)
						{
							if (!(baseNetworkable == null))
							{
								Networkable net = baseNetworkable.net;
								if (((net != null) ? net.group : null) != null && baseNetworkable.net.group != group)
								{
									continue;
								}
							}
							list.Add(baseNetworkable);
						}
						foreach (global::BaseNetworkable item in list)
						{
							this.queueInternal.Remove(item);
						}
						Facepunch.Pool.FreeList<global::BaseNetworkable>(ref list);
					}
				}
				else
				{
					this.queueInternal.RemoveWhere(delegate(global::BaseNetworkable x)
					{
						if (!(x == null))
						{
							Networkable net2 = x.net;
							if (((net2 != null) ? net2.group : null) != null)
							{
								return !x.net.group.isGlobal;
							}
						}
						return true;
					});
				}
			}
		}
	}

	// Token: 0x02000BA1 RID: 2977
	[Flags]
	public enum PlayerFlags
	{
		// Token: 0x0400401A RID: 16410
		Unused1 = 1,
		// Token: 0x0400401B RID: 16411
		Unused2 = 2,
		// Token: 0x0400401C RID: 16412
		IsAdmin = 4,
		// Token: 0x0400401D RID: 16413
		ReceivingSnapshot = 8,
		// Token: 0x0400401E RID: 16414
		Sleeping = 16,
		// Token: 0x0400401F RID: 16415
		Spectating = 32,
		// Token: 0x04004020 RID: 16416
		Wounded = 64,
		// Token: 0x04004021 RID: 16417
		IsDeveloper = 128,
		// Token: 0x04004022 RID: 16418
		Connected = 256,
		// Token: 0x04004023 RID: 16419
		ThirdPersonViewmode = 1024,
		// Token: 0x04004024 RID: 16420
		EyesViewmode = 2048,
		// Token: 0x04004025 RID: 16421
		ChatMute = 4096,
		// Token: 0x04004026 RID: 16422
		NoSprint = 8192,
		// Token: 0x04004027 RID: 16423
		Aiming = 16384,
		// Token: 0x04004028 RID: 16424
		DisplaySash = 32768,
		// Token: 0x04004029 RID: 16425
		Relaxed = 65536,
		// Token: 0x0400402A RID: 16426
		SafeZone = 131072,
		// Token: 0x0400402B RID: 16427
		ServerFall = 262144,
		// Token: 0x0400402C RID: 16428
		Incapacitated = 524288,
		// Token: 0x0400402D RID: 16429
		Workbench1 = 1048576,
		// Token: 0x0400402E RID: 16430
		Workbench2 = 2097152,
		// Token: 0x0400402F RID: 16431
		Workbench3 = 4194304,
		// Token: 0x04004030 RID: 16432
		VoiceRangeBoost = 8388608
	}

	// Token: 0x02000BA2 RID: 2978
	public static class GestureIds
	{
		// Token: 0x04004031 RID: 16433
		public const uint FlashBlindId = 235662700U;
	}

	// Token: 0x02000BA3 RID: 2979
	public enum MapNoteType
	{
		// Token: 0x04004033 RID: 16435
		Death,
		// Token: 0x04004034 RID: 16436
		PointOfInterest
	}

	// Token: 0x02000BA4 RID: 2980
	public enum PingType
	{
		// Token: 0x04004036 RID: 16438
		Hostile,
		// Token: 0x04004037 RID: 16439
		GoTo,
		// Token: 0x04004038 RID: 16440
		Dollar,
		// Token: 0x04004039 RID: 16441
		Loot,
		// Token: 0x0400403A RID: 16442
		Node,
		// Token: 0x0400403B RID: 16443
		Gun,
		// Token: 0x0400403C RID: 16444
		LAST = 5
	}

	// Token: 0x02000BA5 RID: 2981
	private struct PingStyle
	{
		// Token: 0x0400403D RID: 16445
		public int IconIndex;

		// Token: 0x0400403E RID: 16446
		public int ColourIndex;

		// Token: 0x0400403F RID: 16447
		public Translate.Phrase PingTitle;

		// Token: 0x04004040 RID: 16448
		public Translate.Phrase PingDescription;

		// Token: 0x04004041 RID: 16449
		public global::BasePlayer.PingType Type;

		// Token: 0x06004D21 RID: 19745 RVA: 0x001A0303 File Offset: 0x0019E503
		public PingStyle(int icon, int colour, Translate.Phrase title, Translate.Phrase desc, global::BasePlayer.PingType pType)
		{
			this.IconIndex = icon;
			this.ColourIndex = colour;
			this.PingTitle = title;
			this.PingDescription = desc;
			this.Type = pType;
		}
	}

	// Token: 0x02000BA6 RID: 2982
	public struct FiredProjectileUpdate
	{
		// Token: 0x04004042 RID: 16450
		public Vector3 OldPosition;

		// Token: 0x04004043 RID: 16451
		public Vector3 NewPosition;

		// Token: 0x04004044 RID: 16452
		public Vector3 OldVelocity;

		// Token: 0x04004045 RID: 16453
		public Vector3 NewVelocity;

		// Token: 0x04004046 RID: 16454
		public float Mismatch;

		// Token: 0x04004047 RID: 16455
		public float PartialTime;
	}

	// Token: 0x02000BA7 RID: 2983
	public class FiredProjectile : Facepunch.Pool.IPooled
	{
		// Token: 0x04004048 RID: 16456
		public ItemDefinition itemDef;

		// Token: 0x04004049 RID: 16457
		public ItemModProjectile itemMod;

		// Token: 0x0400404A RID: 16458
		public Projectile projectilePrefab;

		// Token: 0x0400404B RID: 16459
		public float firedTime;

		// Token: 0x0400404C RID: 16460
		public float travelTime;

		// Token: 0x0400404D RID: 16461
		public float partialTime;

		// Token: 0x0400404E RID: 16462
		public AttackEntity weaponSource;

		// Token: 0x0400404F RID: 16463
		public AttackEntity weaponPrefab;

		// Token: 0x04004050 RID: 16464
		public Projectile.Modifier projectileModifier;

		// Token: 0x04004051 RID: 16465
		public global::Item pickupItem;

		// Token: 0x04004052 RID: 16466
		public float integrity;

		// Token: 0x04004053 RID: 16467
		public float trajectoryMismatch;

		// Token: 0x04004054 RID: 16468
		public Vector3 position;

		// Token: 0x04004055 RID: 16469
		public Vector3 velocity;

		// Token: 0x04004056 RID: 16470
		public Vector3 initialPosition;

		// Token: 0x04004057 RID: 16471
		public Vector3 initialVelocity;

		// Token: 0x04004058 RID: 16472
		public Vector3 inheritedVelocity;

		// Token: 0x04004059 RID: 16473
		public int protection;

		// Token: 0x0400405A RID: 16474
		public int ricochets;

		// Token: 0x0400405B RID: 16475
		public int hits;

		// Token: 0x0400405C RID: 16476
		public global::BaseEntity lastEntityHit;

		// Token: 0x0400405D RID: 16477
		public float desyncLifeTime;

		// Token: 0x0400405E RID: 16478
		public int id;

		// Token: 0x0400405F RID: 16479
		public List<global::BasePlayer.FiredProjectileUpdate> updates = new List<global::BasePlayer.FiredProjectileUpdate>();

		// Token: 0x04004060 RID: 16480
		public global::BasePlayer attacker;

		// Token: 0x06004D22 RID: 19746 RVA: 0x001A032C File Offset: 0x0019E52C
		public void EnterPool()
		{
			this.itemDef = null;
			this.itemMod = null;
			this.projectilePrefab = null;
			this.firedTime = 0f;
			this.travelTime = 0f;
			this.partialTime = 0f;
			this.weaponSource = null;
			this.weaponPrefab = null;
			this.projectileModifier = default(Projectile.Modifier);
			this.pickupItem = null;
			this.integrity = 0f;
			this.trajectoryMismatch = 0f;
			this.position = default(Vector3);
			this.velocity = default(Vector3);
			this.initialPosition = default(Vector3);
			this.initialVelocity = default(Vector3);
			this.inheritedVelocity = default(Vector3);
			this.protection = 0;
			this.ricochets = 0;
			this.hits = 0;
			this.lastEntityHit = null;
			this.desyncLifeTime = 0f;
			this.id = 0;
			this.updates.Clear();
			this.attacker = null;
		}

		// Token: 0x06004D23 RID: 19747 RVA: 0x000063A5 File Offset: 0x000045A5
		public void LeavePool()
		{
		}
	}

	// Token: 0x02000BA8 RID: 2984
	public enum TimeCategory
	{
		// Token: 0x04004062 RID: 16482
		Wilderness = 1,
		// Token: 0x04004063 RID: 16483
		Monument,
		// Token: 0x04004064 RID: 16484
		Base = 4,
		// Token: 0x04004065 RID: 16485
		Flying = 8,
		// Token: 0x04004066 RID: 16486
		Boating = 16,
		// Token: 0x04004067 RID: 16487
		Swimming = 32,
		// Token: 0x04004068 RID: 16488
		Driving = 64
	}

	// Token: 0x02000BA9 RID: 2985
	public class LifeStoryWorkQueue : ObjectWorkQueue<global::BasePlayer>
	{
		// Token: 0x06004D25 RID: 19749 RVA: 0x001A0435 File Offset: 0x0019E635
		protected override void RunJob(global::BasePlayer entity)
		{
			entity.UpdateTimeCategory();
		}

		// Token: 0x06004D26 RID: 19750 RVA: 0x001A043D File Offset: 0x0019E63D
		protected override bool ShouldAdd(global::BasePlayer entity)
		{
			return base.ShouldAdd(entity) && entity.IsValid();
		}
	}

	// Token: 0x02000BAA RID: 2986
	public class SpawnPoint
	{
		// Token: 0x04004069 RID: 16489
		public Vector3 pos;

		// Token: 0x0400406A RID: 16490
		public Quaternion rot;
	}

	// Token: 0x02000BAB RID: 2987
	[Serializable]
	public struct CapsuleColliderInfo
	{
		// Token: 0x0400406B RID: 16491
		public float height;

		// Token: 0x0400406C RID: 16492
		public float radius;

		// Token: 0x0400406D RID: 16493
		public Vector3 center;

		// Token: 0x06004D29 RID: 19753 RVA: 0x001A0458 File Offset: 0x0019E658
		public CapsuleColliderInfo(float height, float radius, Vector3 center)
		{
			this.height = height;
			this.radius = radius;
			this.center = center;
		}
	}
}
