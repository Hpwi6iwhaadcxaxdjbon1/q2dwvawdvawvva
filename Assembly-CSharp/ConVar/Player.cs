using System;
using System.Collections.Generic;
using System.Text;
using Facepunch;
using ProtoBuf;
using UnityEngine;

namespace ConVar
{
	// Token: 0x02000AD3 RID: 2771
	[ConsoleSystem.Factory("player")]
	public class Player : ConsoleSystem
	{
		// Token: 0x04003B8F RID: 15247
		[ServerVar]
		public static int tickrate_cl = 20;

		// Token: 0x04003B90 RID: 15248
		[ServerVar]
		public static int tickrate_sv = 16;

		// Token: 0x04003B91 RID: 15249
		[ClientVar(ClientInfo = true)]
		public static bool InfiniteAmmo = false;

		// Token: 0x04003B92 RID: 15250
		[ServerVar(Saved = true, ShowInAdminUI = true, Help = "Whether the crawling state expires")]
		public static bool woundforever = false;

		// Token: 0x0600428E RID: 17038 RVA: 0x0018A144 File Offset: 0x00188344
		[ServerUserVar]
		[ClientVar(AllowRunFromServer = true)]
		public static void cinematic_play(ConsoleSystem.Arg arg)
		{
			if (!arg.HasArgs(1))
			{
				return;
			}
			if (arg.IsServerside)
			{
				global::BasePlayer basePlayer = arg.Player();
				if (basePlayer == null)
				{
					return;
				}
				string strCommand = string.Empty;
				if (basePlayer.IsAdmin || basePlayer.IsDeveloper)
				{
					strCommand = string.Concat(new string[]
					{
						arg.cmd.FullName,
						" ",
						arg.FullString,
						" ",
						basePlayer.UserIDString
					});
				}
				else if (Server.cinematic)
				{
					strCommand = string.Concat(new string[]
					{
						arg.cmd.FullName,
						" ",
						arg.GetString(0, ""),
						" ",
						basePlayer.UserIDString
					});
				}
				if (Server.cinematic)
				{
					ConsoleNetwork.BroadcastToAllClients(strCommand, Array.Empty<object>());
					return;
				}
				if (basePlayer.IsAdmin || basePlayer.IsDeveloper)
				{
					ConsoleNetwork.SendClientCommand(arg.Connection, strCommand, Array.Empty<object>());
				}
			}
		}

		// Token: 0x0600428F RID: 17039 RVA: 0x0018A248 File Offset: 0x00188448
		[ServerUserVar]
		[ClientVar(AllowRunFromServer = true)]
		public static void cinematic_stop(ConsoleSystem.Arg arg)
		{
			if (arg.IsServerside)
			{
				global::BasePlayer basePlayer = arg.Player();
				if (basePlayer == null)
				{
					return;
				}
				string strCommand = string.Empty;
				if (basePlayer.IsAdmin || basePlayer.IsDeveloper)
				{
					strCommand = string.Concat(new string[]
					{
						arg.cmd.FullName,
						" ",
						arg.FullString,
						" ",
						basePlayer.UserIDString
					});
				}
				else if (Server.cinematic)
				{
					strCommand = arg.cmd.FullName + " " + basePlayer.UserIDString;
				}
				if (Server.cinematic)
				{
					ConsoleNetwork.BroadcastToAllClients(strCommand, Array.Empty<object>());
					return;
				}
				if (basePlayer.IsAdmin || basePlayer.IsDeveloper)
				{
					ConsoleNetwork.SendClientCommand(arg.Connection, strCommand, Array.Empty<object>());
				}
			}
		}

		// Token: 0x06004290 RID: 17040 RVA: 0x0018A31C File Offset: 0x0018851C
		[ServerUserVar]
		public static void cinematic_gesture(ConsoleSystem.Arg arg)
		{
			if (!Server.cinematic)
			{
				return;
			}
			string @string = arg.GetString(0, "");
			global::BasePlayer basePlayer = arg.GetPlayer(1);
			if (basePlayer == null)
			{
				basePlayer = arg.Player();
			}
			basePlayer.UpdateActiveItem(default(ItemId));
			basePlayer.SignalBroadcast(global::BaseEntity.Signal.Gesture, @string, null);
		}

		// Token: 0x06004291 RID: 17041 RVA: 0x0018A370 File Offset: 0x00188570
		[ServerUserVar]
		public static void copyrotation(ConsoleSystem.Arg arg)
		{
			global::BasePlayer basePlayer = arg.Player();
			if (!basePlayer.IsAdmin && !basePlayer.IsDeveloper && !Server.cinematic)
			{
				return;
			}
			uint @uint = arg.GetUInt(0, 0U);
			global::BasePlayer basePlayer2 = global::BasePlayer.FindByID((ulong)@uint);
			if (basePlayer2 == null)
			{
				basePlayer2 = global::BasePlayer.FindBot((ulong)@uint);
			}
			if (basePlayer2 != null)
			{
				basePlayer2.CopyRotation(basePlayer);
				Debug.Log("Copied rotation of " + basePlayer2.UserIDString);
			}
		}

		// Token: 0x06004292 RID: 17042 RVA: 0x0018A3E4 File Offset: 0x001885E4
		[ServerUserVar]
		public static void abandonmission(ConsoleSystem.Arg arg)
		{
			global::BasePlayer basePlayer = arg.Player();
			if (basePlayer.HasActiveMission())
			{
				basePlayer.AbandonActiveMission();
			}
		}

		// Token: 0x06004293 RID: 17043 RVA: 0x0018A408 File Offset: 0x00188608
		[ServerUserVar]
		public static void mount(ConsoleSystem.Arg arg)
		{
			global::BasePlayer basePlayer = arg.Player();
			if (!basePlayer.IsAdmin && !basePlayer.IsDeveloper && !Server.cinematic)
			{
				return;
			}
			uint @uint = arg.GetUInt(0, 0U);
			global::BasePlayer basePlayer2 = global::BasePlayer.FindByID((ulong)@uint);
			if (basePlayer2 == null)
			{
				basePlayer2 = global::BasePlayer.FindBot((ulong)@uint);
			}
			if (!basePlayer2)
			{
				return;
			}
			RaycastHit hit;
			if (Physics.Raycast(basePlayer.eyes.position, basePlayer.eyes.HeadForward(), out hit, 5f, 10496, QueryTriggerInteraction.Ignore))
			{
				global::BaseEntity entity = hit.GetEntity();
				if (entity)
				{
					BaseMountable baseMountable = entity.GetComponent<BaseMountable>();
					if (!baseMountable)
					{
						global::BaseVehicle baseVehicle = entity.GetComponentInParent<global::BaseVehicle>();
						if (baseVehicle)
						{
							if (!baseVehicle.isServer)
							{
								baseVehicle = (global::BaseNetworkable.serverEntities.Find(baseVehicle.net.ID) as global::BaseVehicle);
							}
							baseVehicle.AttemptMount(basePlayer2, true);
							return;
						}
					}
					if (baseMountable && !baseMountable.isServer)
					{
						baseMountable = (global::BaseNetworkable.serverEntities.Find(baseMountable.net.ID) as BaseMountable);
					}
					if (baseMountable)
					{
						baseMountable.AttemptMount(basePlayer2, true);
					}
				}
			}
		}

		// Token: 0x06004294 RID: 17044 RVA: 0x0018A538 File Offset: 0x00188738
		[ServerVar]
		public static void gotosleep(ConsoleSystem.Arg arg)
		{
			global::BasePlayer basePlayer = arg.Player();
			if (!basePlayer.IsAdmin && !basePlayer.IsDeveloper && !Server.cinematic)
			{
				return;
			}
			uint @uint = arg.GetUInt(0, 0U);
			global::BasePlayer basePlayer2 = global::BasePlayer.FindSleeping(@uint.ToString());
			if (!basePlayer2)
			{
				basePlayer2 = global::BasePlayer.FindBotClosestMatch(@uint.ToString());
				if (basePlayer2.IsSleeping())
				{
					basePlayer2 = null;
				}
			}
			if (!basePlayer2)
			{
				return;
			}
			basePlayer2.StartSleeping();
		}

		// Token: 0x06004295 RID: 17045 RVA: 0x0018A5A8 File Offset: 0x001887A8
		[ServerVar]
		public static void dismount(ConsoleSystem.Arg arg)
		{
			global::BasePlayer basePlayer = arg.Player();
			if (!basePlayer.IsAdmin && !basePlayer.IsDeveloper && !Server.cinematic)
			{
				return;
			}
			uint @uint = arg.GetUInt(0, 0U);
			global::BasePlayer basePlayer2 = global::BasePlayer.FindByID((ulong)@uint);
			if (basePlayer2 == null)
			{
				basePlayer2 = global::BasePlayer.FindBot((ulong)@uint);
			}
			if (!basePlayer2)
			{
				return;
			}
			if (basePlayer2 && basePlayer2.isMounted)
			{
				basePlayer2.GetMounted().DismountPlayer(basePlayer2, false);
			}
		}

		// Token: 0x06004296 RID: 17046 RVA: 0x0018A61C File Offset: 0x0018881C
		[ServerVar]
		public static void swapseat(ConsoleSystem.Arg arg)
		{
			global::BasePlayer basePlayer = arg.Player();
			if (!basePlayer.IsAdmin && !basePlayer.IsDeveloper && !Server.cinematic)
			{
				return;
			}
			uint @uint = arg.GetUInt(0, 0U);
			global::BasePlayer basePlayer2 = global::BasePlayer.FindByID((ulong)@uint);
			if (basePlayer2 == null)
			{
				basePlayer2 = global::BasePlayer.FindBot((ulong)@uint);
			}
			if (!basePlayer2)
			{
				return;
			}
			int @int = arg.GetInt(1, 0);
			if (basePlayer2 && basePlayer2.isMounted && basePlayer2.GetMounted().VehicleParent())
			{
				basePlayer2.GetMounted().VehicleParent().SwapSeats(basePlayer2, @int);
			}
		}

		// Token: 0x06004297 RID: 17047 RVA: 0x0018A6B0 File Offset: 0x001888B0
		[ServerVar]
		public static void wakeup(ConsoleSystem.Arg arg)
		{
			global::BasePlayer basePlayer = arg.Player();
			if (!basePlayer.IsAdmin && !basePlayer.IsDeveloper && !Server.cinematic)
			{
				return;
			}
			global::BasePlayer basePlayer2 = global::BasePlayer.FindSleeping(arg.GetUInt(0, 0U).ToString());
			if (!basePlayer2)
			{
				return;
			}
			basePlayer2.EndSleeping();
		}

		// Token: 0x06004298 RID: 17048 RVA: 0x0018A704 File Offset: 0x00188904
		[ServerVar]
		public static void wakeupall(ConsoleSystem.Arg arg)
		{
			global::BasePlayer basePlayer = arg.Player();
			if (!basePlayer.IsAdmin && !basePlayer.IsDeveloper && !Server.cinematic)
			{
				return;
			}
			List<global::BasePlayer> list = Pool.GetList<global::BasePlayer>();
			foreach (global::BasePlayer item in global::BasePlayer.sleepingPlayerList)
			{
				list.Add(item);
			}
			foreach (global::BasePlayer basePlayer2 in list)
			{
				basePlayer2.EndSleeping();
			}
			Pool.FreeList<global::BasePlayer>(ref list);
		}

		// Token: 0x06004299 RID: 17049 RVA: 0x0018A7C0 File Offset: 0x001889C0
		[ServerVar]
		public static void printstats(ConsoleSystem.Arg arg)
		{
			global::BasePlayer basePlayer = arg.Player();
			if (!basePlayer)
			{
				return;
			}
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine(string.Format("{0:F1}s alive", basePlayer.lifeStory.secondsAlive));
			stringBuilder.AppendLine(string.Format("{0:F1}s sleeping", basePlayer.lifeStory.secondsSleeping));
			stringBuilder.AppendLine(string.Format("{0:F1}s swimming", basePlayer.lifeStory.secondsSwimming));
			stringBuilder.AppendLine(string.Format("{0:F1}s in base", basePlayer.lifeStory.secondsInBase));
			stringBuilder.AppendLine(string.Format("{0:F1}s in wilderness", basePlayer.lifeStory.secondsWilderness));
			stringBuilder.AppendLine(string.Format("{0:F1}s in monuments", basePlayer.lifeStory.secondsInMonument));
			stringBuilder.AppendLine(string.Format("{0:F1}s flying", basePlayer.lifeStory.secondsFlying));
			stringBuilder.AppendLine(string.Format("{0:F1}s boating", basePlayer.lifeStory.secondsBoating));
			stringBuilder.AppendLine(string.Format("{0:F1}s driving", basePlayer.lifeStory.secondsDriving));
			stringBuilder.AppendLine(string.Format("{0:F1}m run", basePlayer.lifeStory.metersRun));
			stringBuilder.AppendLine(string.Format("{0:F1}m walked", basePlayer.lifeStory.metersWalked));
			stringBuilder.AppendLine(string.Format("{0:F1} damage taken", basePlayer.lifeStory.totalDamageTaken));
			stringBuilder.AppendLine(string.Format("{0:F1} damage healed", basePlayer.lifeStory.totalHealing));
			stringBuilder.AppendLine("===");
			stringBuilder.AppendLine(string.Format("{0} other players killed", basePlayer.lifeStory.killedPlayers));
			stringBuilder.AppendLine(string.Format("{0} scientists killed", basePlayer.lifeStory.killedScientists));
			stringBuilder.AppendLine(string.Format("{0} animals killed", basePlayer.lifeStory.killedAnimals));
			stringBuilder.AppendLine("===");
			stringBuilder.AppendLine("Weapon stats:");
			if (basePlayer.lifeStory.weaponStats != null)
			{
				foreach (PlayerLifeStory.WeaponStats weaponStats in basePlayer.lifeStory.weaponStats)
				{
					float num = weaponStats.shotsHit / weaponStats.shotsFired;
					num *= 100f;
					stringBuilder.AppendLine(string.Format("{0} - shots fired: {1} shots hit: {2} accuracy: {3:F1}%", new object[]
					{
						weaponStats.weaponName,
						weaponStats.shotsFired,
						weaponStats.shotsHit,
						num
					}));
				}
			}
			stringBuilder.AppendLine("===");
			stringBuilder.AppendLine("Misc stats:");
			if (basePlayer.lifeStory.genericStats != null)
			{
				foreach (PlayerLifeStory.GenericStat genericStat in basePlayer.lifeStory.genericStats)
				{
					stringBuilder.AppendLine(string.Format("{0} = {1}", genericStat.key, genericStat.value));
				}
			}
			arg.ReplyWith(stringBuilder.ToString());
		}

		// Token: 0x0600429A RID: 17050 RVA: 0x0018AB68 File Offset: 0x00188D68
		[ServerVar]
		public static void printpresence(ConsoleSystem.Arg arg)
		{
			global::BasePlayer basePlayer = arg.Player();
			bool flag = (basePlayer.currentTimeCategory & 1) != 0;
			bool flag2 = (basePlayer.currentTimeCategory & 4) != 0;
			bool flag3 = (basePlayer.currentTimeCategory & 2) != 0;
			bool flag4 = (basePlayer.currentTimeCategory & 32) != 0;
			bool flag5 = (basePlayer.currentTimeCategory & 16) != 0;
			bool flag6 = (basePlayer.currentTimeCategory & 8) != 0;
			arg.ReplyWith(string.Format("Wilderness:{0} Base:{1} Monument:{2} Swimming: {3} Boating: {4} Flying: {5}", new object[]
			{
				flag,
				flag2,
				flag3,
				flag4,
				flag5,
				flag6
			}));
		}

		// Token: 0x0600429B RID: 17051 RVA: 0x0018AC14 File Offset: 0x00188E14
		[ServerVar(Help = "Resets the PlayerState of the given player")]
		public static void resetstate(ConsoleSystem.Arg args)
		{
			global::BasePlayer playerOrSleeper = args.GetPlayerOrSleeper(0);
			if (playerOrSleeper == null)
			{
				args.ReplyWith("Player not found");
				return;
			}
			playerOrSleeper.ResetPlayerState();
			args.ReplyWith("Player state reset");
		}

		// Token: 0x0600429C RID: 17052 RVA: 0x0018AC50 File Offset: 0x00188E50
		[ServerVar(ServerAdmin = true)]
		public static void fillwater(ConsoleSystem.Arg arg)
		{
			bool flag = arg.GetString(0, "").ToLower() == "salt";
			global::BasePlayer basePlayer = arg.Player();
			ItemDefinition liquidType = ItemManager.FindItemDefinition(flag ? "water.salt" : "water");
			for (int i = 0; i < PlayerBelt.MaxBeltSlots; i++)
			{
				global::Item itemInSlot = basePlayer.Belt.GetItemInSlot(i);
				BaseLiquidVessel baseLiquidVessel;
				if (itemInSlot != null && (baseLiquidVessel = (itemInSlot.GetHeldEntity() as BaseLiquidVessel)) != null && baseLiquidVessel.hasLid)
				{
					int amount = 999;
					ItemModContainer itemModContainer;
					if (baseLiquidVessel.GetItem().info.TryGetComponent<ItemModContainer>(out itemModContainer))
					{
						amount = itemModContainer.maxStackSize;
					}
					baseLiquidVessel.AddLiquid(liquidType, amount);
				}
			}
		}

		// Token: 0x0600429D RID: 17053 RVA: 0x0018ACFC File Offset: 0x00188EFC
		[ServerVar(ServerAdmin = true)]
		public static void reloadweapons(ConsoleSystem.Arg arg)
		{
			global::BasePlayer basePlayer = arg.Player();
			for (int i = 0; i < PlayerBelt.MaxBeltSlots; i++)
			{
				global::Item itemInSlot = basePlayer.Belt.GetItemInSlot(i);
				if (itemInSlot != null)
				{
					global::BaseProjectile baseProjectile;
					FlameThrower flameThrower;
					LiquidWeapon liquidWeapon;
					if ((baseProjectile = (itemInSlot.GetHeldEntity() as global::BaseProjectile)) != null)
					{
						if (baseProjectile.primaryMagazine != null)
						{
							baseProjectile.primaryMagazine.contents = baseProjectile.primaryMagazine.capacity;
							baseProjectile.SendNetworkUpdateImmediate(false);
						}
					}
					else if ((flameThrower = (itemInSlot.GetHeldEntity() as FlameThrower)) != null)
					{
						flameThrower.ammo = flameThrower.maxAmmo;
						flameThrower.SendNetworkUpdateImmediate(false);
					}
					else if ((liquidWeapon = (itemInSlot.GetHeldEntity() as LiquidWeapon)) != null)
					{
						liquidWeapon.AddLiquid(ItemManager.FindItemDefinition("water"), 999);
					}
				}
			}
		}

		// Token: 0x0600429E RID: 17054 RVA: 0x0018ADC0 File Offset: 0x00188FC0
		[ServerVar]
		public static void createskull(ConsoleSystem.Arg arg)
		{
			string text = arg.GetString(0, "");
			global::BasePlayer basePlayer = arg.Player();
			if (string.IsNullOrEmpty(text))
			{
				text = RandomUsernames.Get(UnityEngine.Random.Range(0, 1000));
			}
			global::Item item = ItemManager.Create(ItemManager.FindItemDefinition("skull.human"), 1, 0UL);
			item.name = HumanBodyResourceDispenser.CreateSkullName(text);
			item.streamerName = item.name;
			basePlayer.inventory.GiveItem(item, null, false);
		}

		// Token: 0x0600429F RID: 17055 RVA: 0x0018AE34 File Offset: 0x00189034
		[ServerVar]
		public static void gesture_radius(ConsoleSystem.Arg arg)
		{
			global::BasePlayer basePlayer = arg.Player();
			if (basePlayer == null || !basePlayer.IsAdmin)
			{
				return;
			}
			float @float = arg.GetFloat(0, 0f);
			List<string> list = Pool.GetList<string>();
			for (int i = 0; i < 5; i++)
			{
				if (!string.IsNullOrEmpty(arg.GetString(i + 1, "")))
				{
					list.Add(arg.GetString(i + 1, ""));
				}
			}
			if (list.Count == 0)
			{
				arg.ReplyWith("No gestures provided. eg. player.gesture_radius 10f cabbagepatch raiseroof");
				return;
			}
			List<global::BasePlayer> list2 = Pool.GetList<global::BasePlayer>();
			Vis.Entities<global::BasePlayer>(basePlayer.transform.position, @float, list2, 131072, QueryTriggerInteraction.Collide);
			foreach (global::BasePlayer basePlayer2 in list2)
			{
				GestureConfig toPlay = basePlayer.gestureList.StringToGesture(list[UnityEngine.Random.Range(0, list.Count)]);
				basePlayer2.Server_StartGesture(toPlay);
			}
			Pool.FreeList<global::BasePlayer>(ref list2);
		}

		// Token: 0x060042A0 RID: 17056 RVA: 0x0018AF44 File Offset: 0x00189144
		[ServerVar]
		public static void stopgesture_radius(ConsoleSystem.Arg arg)
		{
			global::BasePlayer basePlayer = arg.Player();
			if (basePlayer == null || !basePlayer.IsAdmin)
			{
				return;
			}
			float @float = arg.GetFloat(0, 0f);
			List<global::BasePlayer> list = Pool.GetList<global::BasePlayer>();
			Vis.Entities<global::BasePlayer>(basePlayer.transform.position, @float, list, 131072, QueryTriggerInteraction.Collide);
			foreach (global::BasePlayer basePlayer2 in list)
			{
				basePlayer2.Server_CancelGesture();
			}
			Pool.FreeList<global::BasePlayer>(ref list);
		}

		// Token: 0x060042A1 RID: 17057 RVA: 0x0018AFDC File Offset: 0x001891DC
		[ServerVar]
		public static void markhostile(ConsoleSystem.Arg arg)
		{
			global::BasePlayer basePlayer = arg.Player();
			if (basePlayer != null)
			{
				basePlayer.MarkHostileFor(60f);
			}
		}
	}
}
