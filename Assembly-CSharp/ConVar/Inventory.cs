using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Facepunch;
using Newtonsoft.Json;
using Steamworks;
using UnityEngine;

namespace ConVar
{
	// Token: 0x02000AC6 RID: 2758
	[ConsoleSystem.Factory("inventory")]
	public class Inventory : ConsoleSystem
	{
		// Token: 0x04003B7F RID: 15231
		private const string LoadoutDirectory = "loadouts";

		// Token: 0x04003B80 RID: 15232
		[ServerVar(Help = "Disables all attire limitations, so NPC clothing and invalid overlaps can be equipped")]
		public static bool disableAttireLimitations;

		// Token: 0x0600424F RID: 16975 RVA: 0x001889F8 File Offset: 0x00186BF8
		[ServerUserVar]
		public static void lighttoggle(ConsoleSystem.Arg arg)
		{
			BasePlayer basePlayer = arg.Player();
			if (!basePlayer)
			{
				return;
			}
			if (basePlayer.IsDead())
			{
				return;
			}
			if (basePlayer.IsSleeping())
			{
				return;
			}
			if (basePlayer.InGesture)
			{
				return;
			}
			basePlayer.LightToggle(true);
		}

		// Token: 0x06004250 RID: 16976 RVA: 0x00188A38 File Offset: 0x00186C38
		[ServerUserVar]
		public static void endloot(ConsoleSystem.Arg arg)
		{
			BasePlayer basePlayer = arg.Player();
			if (!basePlayer)
			{
				return;
			}
			if (basePlayer.IsDead())
			{
				return;
			}
			if (basePlayer.IsSleeping())
			{
				return;
			}
			basePlayer.inventory.loot.Clear();
		}

		// Token: 0x06004251 RID: 16977 RVA: 0x00188A78 File Offset: 0x00186C78
		[ServerVar]
		public static void give(ConsoleSystem.Arg arg)
		{
			BasePlayer basePlayer = arg.Player();
			if (!basePlayer)
			{
				return;
			}
			Item item = ItemManager.CreateByPartialName(arg.GetString(0, ""), 1, arg.GetULong(3, 0UL));
			if (item == null)
			{
				arg.ReplyWith("Invalid Item!");
				return;
			}
			int @int = arg.GetInt(1, 1);
			item.amount = @int;
			float @float = arg.GetFloat(2, 1f);
			item.conditionNormalized = @float;
			item.OnVirginSpawn();
			if (!basePlayer.inventory.GiveItem(item, null, false))
			{
				item.Remove(0f);
				arg.ReplyWith("Couldn't give item (inventory full?)");
				return;
			}
			basePlayer.Command("note.inv", new object[]
			{
				item.info.itemid,
				@int
			});
			Debug.Log(string.Concat(new object[]
			{
				"giving ",
				basePlayer.displayName,
				" ",
				@int,
				" x ",
				item.info.displayName.english
			}));
			if (basePlayer.IsDeveloper)
			{
				basePlayer.ChatMessage(string.Concat(new object[]
				{
					"you silently gave yourself ",
					@int,
					" x ",
					item.info.displayName.english
				}));
				return;
			}
			Chat.Broadcast(string.Concat(new object[]
			{
				basePlayer.displayName,
				" gave themselves ",
				@int,
				" x ",
				item.info.displayName.english
			}), "SERVER", "#eee", 0UL);
		}

		// Token: 0x06004252 RID: 16978 RVA: 0x00188C24 File Offset: 0x00186E24
		[ServerVar]
		public static void resetbp(ConsoleSystem.Arg arg)
		{
			BasePlayer basePlayer = arg.GetPlayer(0);
			if (basePlayer == null)
			{
				if (arg.HasArgs(1))
				{
					arg.ReplyWith("Can't find player");
					return;
				}
				basePlayer = arg.Player();
			}
			basePlayer.blueprints.Reset();
		}

		// Token: 0x06004253 RID: 16979 RVA: 0x00188C6C File Offset: 0x00186E6C
		[ServerVar]
		public static void unlockall(ConsoleSystem.Arg arg)
		{
			BasePlayer basePlayer = arg.GetPlayer(0);
			if (basePlayer == null)
			{
				if (arg.HasArgs(1))
				{
					arg.ReplyWith("Can't find player");
					return;
				}
				basePlayer = arg.Player();
			}
			basePlayer.blueprints.UnlockAll();
		}

		// Token: 0x06004254 RID: 16980 RVA: 0x00188CB4 File Offset: 0x00186EB4
		[ServerVar]
		public static void giveall(ConsoleSystem.Arg arg)
		{
			Item item = null;
			string text = "SERVER";
			if (arg.Player() != null)
			{
				text = arg.Player().displayName;
			}
			foreach (BasePlayer basePlayer in BasePlayer.activePlayerList)
			{
				item = ItemManager.CreateByPartialName(arg.GetString(0, ""), 1, 0UL);
				if (item == null)
				{
					arg.ReplyWith("Invalid Item!");
					return;
				}
				int @int = arg.GetInt(1, 1);
				item.amount = @int;
				item.OnVirginSpawn();
				if (!basePlayer.inventory.GiveItem(item, null, false))
				{
					item.Remove(0f);
					arg.ReplyWith("Couldn't give item (inventory full?)");
				}
				else
				{
					basePlayer.Command("note.inv", new object[]
					{
						item.info.itemid,
						@int
					});
					Debug.Log(string.Concat(new object[]
					{
						" [ServerVar] giving ",
						basePlayer.displayName,
						" ",
						item.amount,
						" x ",
						item.info.displayName.english
					}));
				}
			}
			if (item != null)
			{
				Chat.Broadcast(string.Concat(new object[]
				{
					text,
					" gave everyone ",
					item.amount,
					" x ",
					item.info.displayName.english
				}), "SERVER", "#eee", 0UL);
			}
		}

		// Token: 0x06004255 RID: 16981 RVA: 0x00188E74 File Offset: 0x00187074
		[ServerVar]
		public static void giveto(ConsoleSystem.Arg arg)
		{
			string text = "SERVER";
			if (arg.Player() != null)
			{
				text = arg.Player().displayName;
			}
			BasePlayer basePlayer = BasePlayer.Find(arg.GetString(0, ""));
			if (basePlayer == null)
			{
				arg.ReplyWith("Couldn't find player!");
				return;
			}
			Item item = ItemManager.CreateByPartialName(arg.GetString(1, ""), 1, arg.GetULong(3, 0UL));
			if (item == null)
			{
				arg.ReplyWith("Invalid Item!");
				return;
			}
			int @int = arg.GetInt(2, 1);
			item.amount = @int;
			item.OnVirginSpawn();
			if (!basePlayer.inventory.GiveItem(item, null, false))
			{
				item.Remove(0f);
				arg.ReplyWith("Couldn't give item (inventory full?)");
				return;
			}
			basePlayer.Command("note.inv", new object[]
			{
				item.info.itemid,
				@int
			});
			Debug.Log(string.Concat(new object[]
			{
				" [ServerVar] giving ",
				basePlayer.displayName,
				" ",
				@int,
				" x ",
				item.info.displayName.english
			}));
			Chat.Broadcast(string.Concat(new object[]
			{
				text,
				" gave ",
				basePlayer.displayName,
				" ",
				@int,
				" x ",
				item.info.displayName.english
			}), "SERVER", "#eee", 0UL);
		}

		// Token: 0x06004256 RID: 16982 RVA: 0x00189008 File Offset: 0x00187208
		[ServerVar]
		public static void giveid(ConsoleSystem.Arg arg)
		{
			BasePlayer basePlayer = arg.Player();
			if (!basePlayer)
			{
				return;
			}
			Item item = ItemManager.CreateByItemID(arg.GetInt(0, 0), 1, 0UL);
			if (item == null)
			{
				arg.ReplyWith("Invalid Item!");
				return;
			}
			int @int = arg.GetInt(1, 1);
			item.amount = @int;
			item.OnVirginSpawn();
			if (!basePlayer.inventory.GiveItem(item, null, false))
			{
				item.Remove(0f);
				arg.ReplyWith("Couldn't give item (inventory full?)");
				return;
			}
			basePlayer.Command("note.inv", new object[]
			{
				item.info.itemid,
				@int
			});
			Debug.Log(string.Concat(new object[]
			{
				" [ServerVar] giving ",
				basePlayer.displayName,
				" ",
				@int,
				" x ",
				item.info.displayName.english
			}));
			if (basePlayer.IsDeveloper)
			{
				basePlayer.ChatMessage(string.Concat(new object[]
				{
					"you silently gave yourself ",
					@int,
					" x ",
					item.info.displayName.english
				}));
				return;
			}
			Chat.Broadcast(string.Concat(new object[]
			{
				basePlayer.displayName,
				" gave themselves ",
				@int,
				" x ",
				item.info.displayName.english
			}), "SERVER", "#eee", 0UL);
		}

		// Token: 0x06004257 RID: 16983 RVA: 0x00189194 File Offset: 0x00187394
		[ServerVar]
		public static void givearm(ConsoleSystem.Arg arg)
		{
			BasePlayer basePlayer = arg.Player();
			if (!basePlayer)
			{
				return;
			}
			Item item = ItemManager.CreateByItemID(arg.GetInt(0, 0), 1, 0UL);
			if (item == null)
			{
				arg.ReplyWith("Invalid Item!");
				return;
			}
			int @int = arg.GetInt(1, 1);
			item.amount = @int;
			item.OnVirginSpawn();
			if (!basePlayer.inventory.GiveItem(item, basePlayer.inventory.containerBelt, false))
			{
				item.Remove(0f);
				arg.ReplyWith("Couldn't give item (inventory full?)");
				return;
			}
			basePlayer.Command("note.inv", new object[]
			{
				item.info.itemid,
				@int
			});
			Debug.Log(string.Concat(new object[]
			{
				" [ServerVar] giving ",
				basePlayer.displayName,
				" ",
				item.amount,
				" x ",
				item.info.displayName.english
			}));
			if (basePlayer.IsDeveloper)
			{
				basePlayer.ChatMessage(string.Concat(new object[]
				{
					"you silently gave yourself ",
					item.amount,
					" x ",
					item.info.displayName.english
				}));
				return;
			}
			Chat.Broadcast(string.Concat(new object[]
			{
				basePlayer.displayName,
				" gave themselves ",
				item.amount,
				" x ",
				item.info.displayName.english
			}), "SERVER", "#eee", 0UL);
		}

		// Token: 0x06004258 RID: 16984 RVA: 0x00189338 File Offset: 0x00187538
		[ServerVar(Help = "Copies the players inventory to the player in front of them")]
		public static void copyTo(ConsoleSystem.Arg arg)
		{
			BasePlayer basePlayer = arg.Player();
			if (!basePlayer.IsAdmin && !basePlayer.IsDeveloper && !Server.cinematic)
			{
				return;
			}
			if (basePlayer == null)
			{
				return;
			}
			BasePlayer basePlayer2 = null;
			if (arg.HasArgs(1) && arg.GetString(0, "").ToLower() != "true")
			{
				basePlayer2 = arg.GetPlayer(0);
				if (basePlayer2 == null)
				{
					uint @uint = arg.GetUInt(0, 0U);
					basePlayer2 = BasePlayer.FindByID((ulong)@uint);
					if (basePlayer2 == null)
					{
						basePlayer2 = BasePlayer.FindBot((ulong)@uint);
					}
				}
			}
			else
			{
				basePlayer2 = RelationshipManager.GetLookingAtPlayer(basePlayer);
			}
			if (basePlayer2 == null)
			{
				return;
			}
			basePlayer2.inventory.containerBelt.Clear();
			basePlayer2.inventory.containerWear.Clear();
			int num = 0;
			foreach (Item item in basePlayer.inventory.containerBelt.itemList)
			{
				basePlayer2.inventory.containerBelt.AddItem(item.info, item.amount, item.skin, ItemContainer.LimitStack.Existing);
				if (item.contents != null)
				{
					Item item2 = basePlayer2.inventory.containerBelt.itemList[num];
					foreach (Item item3 in item.contents.itemList)
					{
						item2.contents.AddItem(item3.info, item3.amount, item3.skin, ItemContainer.LimitStack.Existing);
					}
				}
				num++;
			}
			foreach (Item item4 in basePlayer.inventory.containerWear.itemList)
			{
				basePlayer2.inventory.containerWear.AddItem(item4.info, item4.amount, item4.skin, ItemContainer.LimitStack.Existing);
			}
			if (basePlayer.IsDeveloper)
			{
				basePlayer.ChatMessage("you silently copied items to " + basePlayer2.displayName);
				return;
			}
			Chat.Broadcast(basePlayer.displayName + " copied their inventory to " + basePlayer2.displayName, "SERVER", "#eee", 0UL);
		}

		// Token: 0x06004259 RID: 16985 RVA: 0x001895B4 File Offset: 0x001877B4
		[ServerVar(Help = "Deploys a loadout to players in a radius eg. inventory.deployLoadoutInRange testloadout 30")]
		public static void deployLoadoutInRange(ConsoleSystem.Arg arg)
		{
			BasePlayer basePlayer = arg.Player();
			if (!basePlayer.IsAdmin && !basePlayer.IsDeveloper && !Server.cinematic)
			{
				return;
			}
			if (basePlayer == null)
			{
				return;
			}
			string @string = arg.GetString(0, "");
			Inventory.SavedLoadout savedLoadout;
			if (!Inventory.LoadLoadout(@string, out savedLoadout))
			{
				arg.ReplyWith("Can't find loadout: " + @string);
				return;
			}
			float @float = arg.GetFloat(1, 0f);
			List<BasePlayer> list = Pool.GetList<BasePlayer>();
			Vis.Entities<BasePlayer>(basePlayer.transform.position, @float, list, 131072, QueryTriggerInteraction.Collide);
			int num = 0;
			foreach (BasePlayer basePlayer2 in list)
			{
				if (!(basePlayer2 == basePlayer) && !basePlayer2.isClient)
				{
					savedLoadout.LoadItemsOnTo(basePlayer2);
					num++;
				}
			}
			arg.ReplyWith(string.Format("Applied loadout {0} to {1} players", @string, num));
			Pool.FreeList<BasePlayer>(ref list);
		}

		// Token: 0x0600425A RID: 16986 RVA: 0x001896C0 File Offset: 0x001878C0
		[ServerVar(Help = "Deploys the given loadout to a target player. eg. inventory.deployLoadout testloadout jim")]
		public static void deployLoadout(ConsoleSystem.Arg arg)
		{
			BasePlayer basePlayer = arg.Player();
			if (basePlayer == null)
			{
				return;
			}
			if (!basePlayer.IsAdmin && !basePlayer.IsDeveloper && !Server.cinematic)
			{
				return;
			}
			string @string = arg.GetString(0, "");
			BasePlayer basePlayer2 = string.IsNullOrEmpty(arg.GetString(1, "")) ? null : arg.GetPlayerOrSleeperOrBot(1);
			if (basePlayer2 == null)
			{
				basePlayer2 = basePlayer;
			}
			if (basePlayer2 == null)
			{
				arg.ReplyWith("Could not find player " + arg.GetString(1, "") + " and no local player available");
				return;
			}
			Inventory.SavedLoadout savedLoadout;
			if (Inventory.LoadLoadout(@string, out savedLoadout))
			{
				savedLoadout.LoadItemsOnTo(basePlayer2);
				arg.ReplyWith("Deployed loadout " + @string + " to " + basePlayer2.displayName);
				return;
			}
			arg.ReplyWith("Could not find loadout " + @string);
		}

		// Token: 0x0600425B RID: 16987 RVA: 0x00189798 File Offset: 0x00187998
		[ServerVar(Help = "Clears the inventory of a target player. eg. inventory.clearInventory jim")]
		public static void clearInventory(ConsoleSystem.Arg arg)
		{
			BasePlayer basePlayer = arg.Player();
			if (basePlayer == null)
			{
				return;
			}
			if (!basePlayer.IsAdmin && !basePlayer.IsDeveloper && !Server.cinematic)
			{
				return;
			}
			BasePlayer basePlayer2 = string.IsNullOrEmpty(arg.GetString(1, "")) ? null : arg.GetPlayerOrSleeperOrBot(1);
			if (basePlayer2 == null)
			{
				basePlayer2 = basePlayer;
			}
			if (basePlayer2 == null)
			{
				arg.ReplyWith("Could not find player " + arg.GetString(1, "") + " and no local player available");
				return;
			}
			basePlayer2.inventory.containerBelt.Clear();
			basePlayer2.inventory.containerWear.Clear();
			basePlayer2.inventory.containerMain.Clear();
		}

		// Token: 0x0600425C RID: 16988 RVA: 0x00189852 File Offset: 0x00187A52
		private static string GetLoadoutPath(string loadoutName)
		{
			return Server.GetServerFolder("loadouts") + "/" + loadoutName + ".ldt";
		}

		// Token: 0x0600425D RID: 16989 RVA: 0x00189870 File Offset: 0x00187A70
		[ServerVar(Help = "Saves the current equipped loadout of the calling player. eg. inventory.saveLoadout loaduoutname")]
		public static void saveloadout(ConsoleSystem.Arg arg)
		{
			BasePlayer basePlayer = arg.Player();
			if (basePlayer == null)
			{
				return;
			}
			if (!basePlayer.IsAdmin && !basePlayer.IsDeveloper && !Server.cinematic)
			{
				return;
			}
			string @string = arg.GetString(0, "");
			string contents = JsonConvert.SerializeObject(new Inventory.SavedLoadout(basePlayer), Formatting.Indented);
			string loadoutPath = Inventory.GetLoadoutPath(@string);
			File.WriteAllText(loadoutPath, contents);
			arg.ReplyWith("Saved loadout to " + loadoutPath);
		}

		// Token: 0x0600425E RID: 16990 RVA: 0x001898E0 File Offset: 0x00187AE0
		public static bool LoadLoadout(string name, out Inventory.SavedLoadout so)
		{
			PlayerInventoryProperties inventoryConfig = PlayerInventoryProperties.GetInventoryConfig(name);
			if (inventoryConfig != null)
			{
				Debug.Log("Found builtin config!");
				so = new Inventory.SavedLoadout(inventoryConfig);
				return true;
			}
			so = new Inventory.SavedLoadout();
			string loadoutPath = Inventory.GetLoadoutPath(name);
			if (!File.Exists(loadoutPath))
			{
				return false;
			}
			so = JsonConvert.DeserializeObject<Inventory.SavedLoadout>(File.ReadAllText(loadoutPath));
			return so != null;
		}

		// Token: 0x0600425F RID: 16991 RVA: 0x00189940 File Offset: 0x00187B40
		[ServerVar(Help = "Prints all saved inventory loadouts")]
		public static void listloadouts(ConsoleSystem.Arg arg)
		{
			BasePlayer basePlayer = arg.Player();
			if (basePlayer == null)
			{
				return;
			}
			if (!basePlayer.IsAdmin && !basePlayer.IsDeveloper && !Server.cinematic)
			{
				return;
			}
			string serverFolder = Server.GetServerFolder("loadouts");
			StringBuilder stringBuilder = new StringBuilder();
			foreach (string value in Directory.EnumerateFiles(serverFolder))
			{
				stringBuilder.AppendLine(value);
			}
			arg.ReplyWith(stringBuilder.ToString());
		}

		// Token: 0x06004260 RID: 16992 RVA: 0x001899D4 File Offset: 0x00187BD4
		[ClientVar]
		[ServerVar]
		public static void defs(ConsoleSystem.Arg arg)
		{
			if (Steamworks.SteamInventory.Definitions == null)
			{
				arg.ReplyWith("no definitions");
				return;
			}
			if (Steamworks.SteamInventory.Definitions.Length == 0)
			{
				arg.ReplyWith("0 definitions");
				return;
			}
			string[] obj = (from x in Steamworks.SteamInventory.Definitions
			select x.Name).ToArray<string>();
			arg.ReplyWith(obj);
		}

		// Token: 0x06004261 RID: 16993 RVA: 0x00189A3E File Offset: 0x00187C3E
		[ClientVar]
		[ServerVar]
		public static void reloaddefs(ConsoleSystem.Arg arg)
		{
			Steamworks.SteamInventory.LoadItemDefinitions();
		}

		// Token: 0x06004262 RID: 16994 RVA: 0x00189A48 File Offset: 0x00187C48
		[ServerVar]
		public static void equipslottarget(ConsoleSystem.Arg arg)
		{
			BasePlayer basePlayer = arg.Player();
			if (!basePlayer.IsAdmin && !basePlayer.IsDeveloper && !Server.cinematic)
			{
				return;
			}
			if (basePlayer == null)
			{
				return;
			}
			BasePlayer lookingAtPlayer = RelationshipManager.GetLookingAtPlayer(basePlayer);
			if (lookingAtPlayer == null)
			{
				return;
			}
			int @int = arg.GetInt(0, 0);
			Inventory.EquipItemInSlot(lookingAtPlayer, @int);
			arg.ReplyWith(string.Format("Equipped slot {0} on player {1}", @int, lookingAtPlayer.displayName));
		}

		// Token: 0x06004263 RID: 16995 RVA: 0x00189ABC File Offset: 0x00187CBC
		[ServerVar]
		public static void equipslot(ConsoleSystem.Arg arg)
		{
			BasePlayer basePlayer = arg.Player();
			if (!basePlayer.IsAdmin && !basePlayer.IsDeveloper && !Server.cinematic)
			{
				return;
			}
			if (basePlayer == null)
			{
				return;
			}
			BasePlayer basePlayer2 = null;
			if (arg.HasArgs(2))
			{
				basePlayer2 = arg.GetPlayer(1);
				if (basePlayer2 == null)
				{
					uint @uint = arg.GetUInt(1, 0U);
					basePlayer2 = BasePlayer.FindByID((ulong)@uint);
					if (basePlayer2 == null)
					{
						basePlayer2 = BasePlayer.FindBot((ulong)@uint);
					}
				}
			}
			if (basePlayer2 == null)
			{
				return;
			}
			int @int = arg.GetInt(0, 0);
			Inventory.EquipItemInSlot(basePlayer2, @int);
			Debug.Log(string.Format("Equipped slot {0} on player {1}", @int, basePlayer2.displayName));
		}

		// Token: 0x06004264 RID: 16996 RVA: 0x00189B68 File Offset: 0x00187D68
		private static void EquipItemInSlot(BasePlayer player, int slot)
		{
			ItemId itemID = default(ItemId);
			for (int i = 0; i < player.inventory.containerBelt.itemList.Count; i++)
			{
				if (player.inventory.containerBelt.itemList[i] != null && i == slot)
				{
					itemID = player.inventory.containerBelt.itemList[i].uid;
					break;
				}
			}
			player.UpdateActiveItem(itemID);
		}

		// Token: 0x06004265 RID: 16997 RVA: 0x00189BE0 File Offset: 0x00187DE0
		private static int GetSlotIndex(BasePlayer player)
		{
			if (player.GetActiveItem() == null)
			{
				return -1;
			}
			ItemId uid = player.GetActiveItem().uid;
			for (int i = 0; i < player.inventory.containerBelt.itemList.Count; i++)
			{
				if (player.inventory.containerBelt.itemList[i] != null && player.inventory.containerBelt.itemList[i].uid == uid)
				{
					return i;
				}
			}
			return -1;
		}

		// Token: 0x06004266 RID: 16998 RVA: 0x00189C64 File Offset: 0x00187E64
		[ServerVar]
		public static void giveBp(ConsoleSystem.Arg arg)
		{
			BasePlayer basePlayer = arg.Player();
			if (!basePlayer)
			{
				return;
			}
			ItemDefinition itemDefinition = ItemManager.FindDefinitionByPartialName(arg.GetString(0, ""), 1, 0UL);
			if (itemDefinition == null)
			{
				arg.ReplyWith("Could not find item: " + arg.GetString(0, ""));
				return;
			}
			if (itemDefinition.Blueprint == null)
			{
				arg.ReplyWith(itemDefinition.shortname + " has no blueprint!");
				return;
			}
			Item item = ItemManager.Create(ItemManager.blueprintBaseDef, 1, 0UL);
			item.blueprintTarget = itemDefinition.itemid;
			item.OnVirginSpawn();
			if (!basePlayer.inventory.GiveItem(item, null, false))
			{
				item.Remove(0f);
				arg.ReplyWith("Couldn't give item (inventory full?)");
				return;
			}
			basePlayer.Command("note.inv", new object[]
			{
				item.info.itemid,
				1
			});
			Debug.Log(string.Concat(new string[]
			{
				"giving ",
				basePlayer.displayName,
				" 1 x ",
				item.blueprintTargetDef.shortname,
				" blueprint"
			}));
			if (basePlayer.IsDeveloper)
			{
				basePlayer.ChatMessage("you silently gave yourself 1 x " + item.blueprintTargetDef.shortname + " blueprint");
				return;
			}
			Chat.Broadcast(basePlayer.displayName + " gave themselves 1 x " + item.blueprintTargetDef.shortname + " blueprint", "SERVER", "#eee", 0UL);
		}

		// Token: 0x02000F62 RID: 3938
		public class SavedLoadout
		{
			// Token: 0x04004F82 RID: 20354
			public Inventory.SavedLoadout.SavedItem[] belt;

			// Token: 0x04004F83 RID: 20355
			public Inventory.SavedLoadout.SavedItem[] wear;

			// Token: 0x04004F84 RID: 20356
			public Inventory.SavedLoadout.SavedItem[] main;

			// Token: 0x04004F85 RID: 20357
			public int heldItemIndex;

			// Token: 0x0600549C RID: 21660 RVA: 0x00008747 File Offset: 0x00006947
			public SavedLoadout()
			{
			}

			// Token: 0x0600549D RID: 21661 RVA: 0x001B5818 File Offset: 0x001B3A18
			public SavedLoadout(BasePlayer player)
			{
				this.belt = Inventory.SavedLoadout.SaveItems(player.inventory.containerBelt);
				this.wear = Inventory.SavedLoadout.SaveItems(player.inventory.containerWear);
				this.main = Inventory.SavedLoadout.SaveItems(player.inventory.containerMain);
				this.heldItemIndex = Inventory.GetSlotIndex(player);
			}

			// Token: 0x0600549E RID: 21662 RVA: 0x001B587C File Offset: 0x001B3A7C
			public SavedLoadout(PlayerInventoryProperties properties)
			{
				this.belt = Inventory.SavedLoadout.SaveItems(properties.belt);
				this.wear = Inventory.SavedLoadout.SaveItems(properties.wear);
				this.main = Inventory.SavedLoadout.SaveItems(properties.main);
				this.heldItemIndex = 0;
			}

			// Token: 0x0600549F RID: 21663 RVA: 0x001B58CC File Offset: 0x001B3ACC
			private static Inventory.SavedLoadout.SavedItem[] SaveItems(ItemContainer itemContainer)
			{
				List<Inventory.SavedLoadout.SavedItem> list = new List<Inventory.SavedLoadout.SavedItem>();
				for (int i = 0; i < itemContainer.capacity; i++)
				{
					Item slot = itemContainer.GetSlot(i);
					if (slot != null)
					{
						Inventory.SavedLoadout.SavedItem item = new Inventory.SavedLoadout.SavedItem
						{
							id = slot.info.itemid,
							amount = slot.amount,
							skin = slot.skin,
							blueprintTarget = slot.blueprintTarget
						};
						if (slot.contents != null && slot.contents.itemList != null)
						{
							List<int> list2 = new List<int>();
							foreach (Item item2 in slot.contents.itemList)
							{
								list2.Add(item2.info.itemid);
							}
							item.containedItems = list2.ToArray();
						}
						list.Add(item);
					}
				}
				return list.ToArray();
			}

			// Token: 0x060054A0 RID: 21664 RVA: 0x001B59D8 File Offset: 0x001B3BD8
			private static Inventory.SavedLoadout.SavedItem[] SaveItems(List<PlayerInventoryProperties.ItemAmountSkinned> items)
			{
				List<Inventory.SavedLoadout.SavedItem> list = new List<Inventory.SavedLoadout.SavedItem>();
				foreach (PlayerInventoryProperties.ItemAmountSkinned itemAmountSkinned in items)
				{
					Inventory.SavedLoadout.SavedItem savedItem = new Inventory.SavedLoadout.SavedItem
					{
						id = itemAmountSkinned.itemid,
						amount = (int)itemAmountSkinned.amount,
						skin = itemAmountSkinned.skinOverride
					};
					if (itemAmountSkinned.blueprint)
					{
						savedItem.blueprintTarget = savedItem.id;
						savedItem.id = ItemManager.blueprintBaseDef.itemid;
					}
					list.Add(savedItem);
				}
				return list.ToArray();
			}

			// Token: 0x060054A1 RID: 21665 RVA: 0x001B5A8C File Offset: 0x001B3C8C
			public void LoadItemsOnTo(BasePlayer player)
			{
				Inventory.SavedLoadout.<>c__DisplayClass10_0 CS$<>8__locals1;
				CS$<>8__locals1.player = player;
				CS$<>8__locals1.<>4__this = this;
				CS$<>8__locals1.player.inventory.containerMain.Clear();
				CS$<>8__locals1.player.inventory.containerBelt.Clear();
				CS$<>8__locals1.player.inventory.containerWear.Clear();
				ItemManager.DoRemoves();
				this.<LoadItemsOnTo>g__LoadItems|10_0(this.belt, CS$<>8__locals1.player.inventory.containerBelt, ref CS$<>8__locals1);
				this.<LoadItemsOnTo>g__LoadItems|10_0(this.wear, CS$<>8__locals1.player.inventory.containerWear, ref CS$<>8__locals1);
				this.<LoadItemsOnTo>g__LoadItems|10_0(this.main, CS$<>8__locals1.player.inventory.containerMain, ref CS$<>8__locals1);
				Inventory.EquipItemInSlot(CS$<>8__locals1.player, this.heldItemIndex);
				CS$<>8__locals1.player.inventory.SendSnapshot();
			}

			// Token: 0x060054A2 RID: 21666 RVA: 0x001B5B68 File Offset: 0x001B3D68
			private Item LoadItem(Inventory.SavedLoadout.SavedItem item)
			{
				Item item2 = ItemManager.CreateByItemID(item.id, item.amount, item.skin);
				if (item.blueprintTarget != 0)
				{
					item2.blueprintTarget = item.blueprintTarget;
				}
				if (item.containedItems != null && item.containedItems.Length != 0)
				{
					foreach (int itemID in item.containedItems)
					{
						item2.contents.AddItem(ItemManager.FindItemDefinition(itemID), 1, 0UL, ItemContainer.LimitStack.Existing);
					}
				}
				return item2;
			}

			// Token: 0x060054A3 RID: 21667 RVA: 0x001B5BE4 File Offset: 0x001B3DE4
			[CompilerGenerated]
			private void <LoadItemsOnTo>g__LoadItems|10_0(Inventory.SavedLoadout.SavedItem[] items, ItemContainer container, ref Inventory.SavedLoadout.<>c__DisplayClass10_0 A_3)
			{
				foreach (Inventory.SavedLoadout.SavedItem item in items)
				{
					A_3.player.inventory.GiveItem(this.LoadItem(item), container, false);
				}
			}

			// Token: 0x02000FDB RID: 4059
			public struct SavedItem
			{
				// Token: 0x04005119 RID: 20761
				public int id;

				// Token: 0x0400511A RID: 20762
				public int amount;

				// Token: 0x0400511B RID: 20763
				public ulong skin;

				// Token: 0x0400511C RID: 20764
				public int[] containedItems;

				// Token: 0x0400511D RID: 20765
				public int blueprintTarget;
			}
		}
	}
}
