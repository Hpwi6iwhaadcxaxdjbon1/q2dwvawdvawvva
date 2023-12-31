﻿using System;
using UnityEngine;

namespace ConVar
{
	// Token: 0x02000ABB RID: 2747
	[ConsoleSystem.Factory("gamemode")]
	public class gamemode : ConsoleSystem
	{
		// Token: 0x060041D9 RID: 16857 RVA: 0x0018693C File Offset: 0x00184B3C
		[ServerUserVar]
		public static void setteam(ConsoleSystem.Arg arg)
		{
			BasePlayer basePlayer = arg.Player();
			if (basePlayer == null)
			{
				return;
			}
			BaseGameMode activeGameMode = BaseGameMode.GetActiveGameMode(true);
			if (!activeGameMode)
			{
				return;
			}
			int @int = arg.GetInt(0, 0);
			if (@int < 0 || @int >= activeGameMode.GetNumTeams())
			{
				return;
			}
			activeGameMode.ResetPlayerScores(basePlayer);
			activeGameMode.SetPlayerTeam(basePlayer, @int);
			basePlayer.Respawn();
		}

		// Token: 0x060041DA RID: 16858 RVA: 0x00186998 File Offset: 0x00184B98
		[ServerVar]
		public static void set(ConsoleSystem.Arg arg)
		{
			string @string = arg.GetString(0, "");
			if (string.IsNullOrEmpty(@string))
			{
				Debug.Log("Invalid gamemode");
			}
			BaseGameMode baseGameMode = null;
			GameObjectRef gameObjectRef = null;
			GameModeManifest gameModeManifest = GameModeManifest.Get();
			Debug.Log("total gamemodes : " + gameModeManifest.gameModePrefabs.Count);
			foreach (GameObjectRef gameObjectRef2 in gameModeManifest.gameModePrefabs)
			{
				BaseGameMode component = gameObjectRef2.Get().GetComponent<BaseGameMode>();
				if (component.shortname == @string)
				{
					baseGameMode = component;
					gameObjectRef = gameObjectRef2;
					Debug.Log(string.Concat(new string[]
					{
						"Found :",
						component.shortname,
						" prefab name is :",
						component.PrefabName,
						": rpath is ",
						gameObjectRef2.resourcePath,
						":"
					}));
					break;
				}
				Debug.Log("search name " + @string + "searched against : " + component.shortname);
			}
			if (baseGameMode == null)
			{
				Debug.Log("Unknown gamemode : " + @string);
				return;
			}
			BaseGameMode activeGameMode = BaseGameMode.GetActiveGameMode(true);
			if (activeGameMode)
			{
				if (baseGameMode.shortname == activeGameMode.shortname)
				{
					Debug.Log("Same gamemode, resetting");
				}
				if (activeGameMode.permanent)
				{
					Debug.LogError("This game mode is permanent, you must reset the server to switch game modes.");
					return;
				}
				activeGameMode.ShutdownGame();
				activeGameMode.Kill(BaseNetworkable.DestroyMode.None);
				BaseGameMode.SetActiveGameMode(null, true);
			}
			BaseEntity baseEntity = GameManager.server.CreateEntity(gameObjectRef.resourcePath, Vector3.zero, Quaternion.identity, true);
			if (baseEntity)
			{
				Debug.Log("Spawning new game mode : " + baseGameMode.shortname);
				baseEntity.Spawn();
				return;
			}
			Debug.Log("Failed to create new game mode :" + baseGameMode.PrefabName);
		}
	}
}
