using System;
using System.Collections.Generic;
using Facepunch;
using ProtoBuf;
using UnityEngine;

namespace CompanionServer
{
	// Token: 0x020009EE RID: 2542
	public static class AppPlayerExtensions
	{
		// Token: 0x06003CD3 RID: 15571 RVA: 0x00165760 File Offset: 0x00163960
		public static AppTeamInfo GetAppTeamInfo(this global::BasePlayer player, ulong steamId)
		{
			AppTeamInfo appTeamInfo = Pool.Get<AppTeamInfo>();
			appTeamInfo.members = Pool.GetList<AppTeamInfo.Member>();
			AppTeamInfo.Member member = Pool.Get<AppTeamInfo.Member>();
			if (player != null)
			{
				Vector2 vector = Util.WorldToMap(player.transform.position);
				member.steamId = player.userID;
				member.name = (player.displayName ?? "");
				member.x = vector.x;
				member.y = vector.y;
				member.isOnline = player.IsConnected;
				AppTeamInfo.Member member2 = member;
				PlayerLifeStory lifeStory = player.lifeStory;
				member2.spawnTime = ((lifeStory != null) ? lifeStory.timeBorn : 0U);
				member.isAlive = player.IsAlive();
				AppTeamInfo.Member member3 = member;
				PlayerLifeStory previousLifeStory = player.previousLifeStory;
				member3.deathTime = ((previousLifeStory != null) ? previousLifeStory.timeDied : 0U);
			}
			else
			{
				member.steamId = steamId;
				member.name = (SingletonComponent<ServerMgr>.Instance.persistance.GetPlayerName(steamId) ?? "");
				member.x = 0f;
				member.y = 0f;
				member.isOnline = false;
				member.spawnTime = 0U;
				member.isAlive = false;
				member.deathTime = 0U;
			}
			appTeamInfo.members.Add(member);
			appTeamInfo.leaderSteamId = 0UL;
			appTeamInfo.mapNotes = AppPlayerExtensions.GetMapNotes(member.steamId, true);
			appTeamInfo.leaderMapNotes = Pool.GetList<AppTeamInfo.Note>();
			return appTeamInfo;
		}

		// Token: 0x06003CD4 RID: 15572 RVA: 0x001658AC File Offset: 0x00163AAC
		public static AppTeamInfo GetAppTeamInfo(this global::RelationshipManager.PlayerTeam team, ulong requesterSteamId)
		{
			AppTeamInfo appTeamInfo = Pool.Get<AppTeamInfo>();
			appTeamInfo.members = Pool.GetList<AppTeamInfo.Member>();
			for (int i = 0; i < team.members.Count; i++)
			{
				ulong num = team.members[i];
				global::BasePlayer basePlayer = global::RelationshipManager.FindByID(num);
				if (!basePlayer)
				{
					basePlayer = null;
				}
				Vector2 vector = Util.WorldToMap((basePlayer != null) ? basePlayer.transform.position : Vector3.zero);
				AppTeamInfo.Member member = Pool.Get<AppTeamInfo.Member>();
				member.steamId = num;
				AppTeamInfo.Member member2 = member;
				string name;
				if ((name = ((basePlayer != null) ? basePlayer.displayName : null)) == null)
				{
					name = (SingletonComponent<ServerMgr>.Instance.persistance.GetPlayerName(num) ?? "");
				}
				member2.name = name;
				member.x = vector.x;
				member.y = vector.y;
				member.isOnline = (basePlayer != null && basePlayer.IsConnected);
				AppTeamInfo.Member member3 = member;
				uint? num2;
				if (basePlayer == null)
				{
					num2 = null;
				}
				else
				{
					PlayerLifeStory lifeStory = basePlayer.lifeStory;
					num2 = ((lifeStory != null) ? new uint?(lifeStory.timeBorn) : null);
				}
				member3.spawnTime = (num2 ?? 0U);
				member.isAlive = (basePlayer != null && basePlayer.IsAlive());
				AppTeamInfo.Member member4 = member;
				uint? num3;
				if (basePlayer == null)
				{
					num3 = null;
				}
				else
				{
					PlayerLifeStory previousLifeStory = basePlayer.previousLifeStory;
					num3 = ((previousLifeStory != null) ? new uint?(previousLifeStory.timeDied) : null);
				}
				member4.deathTime = (num3 ?? 0U);
				appTeamInfo.members.Add(member);
			}
			appTeamInfo.leaderSteamId = team.teamLeader;
			appTeamInfo.mapNotes = AppPlayerExtensions.GetMapNotes(requesterSteamId, true);
			if (requesterSteamId != team.teamLeader)
			{
				appTeamInfo.leaderMapNotes = AppPlayerExtensions.GetMapNotes(team.teamLeader, false);
			}
			else
			{
				appTeamInfo.leaderMapNotes = Pool.GetList<AppTeamInfo.Note>();
			}
			return appTeamInfo;
		}

		// Token: 0x06003CD5 RID: 15573 RVA: 0x00165A88 File Offset: 0x00163C88
		private static List<AppTeamInfo.Note> GetMapNotes(ulong playerId, bool personalNotes)
		{
			List<AppTeamInfo.Note> list = Pool.GetList<AppTeamInfo.Note>();
			PlayerState playerState = SingletonComponent<ServerMgr>.Instance.playerStateManager.Get(playerId);
			if (playerState != null)
			{
				if (personalNotes && playerState.deathMarker != null)
				{
					AppPlayerExtensions.AddMapNote(list, playerState.deathMarker, global::BasePlayer.MapNoteType.Death);
				}
				if (playerState.pointsOfInterest != null)
				{
					foreach (MapNote note in playerState.pointsOfInterest)
					{
						AppPlayerExtensions.AddMapNote(list, note, global::BasePlayer.MapNoteType.PointOfInterest);
					}
				}
			}
			return list;
		}

		// Token: 0x06003CD6 RID: 15574 RVA: 0x00165B18 File Offset: 0x00163D18
		private static void AddMapNote(List<AppTeamInfo.Note> result, MapNote note, global::BasePlayer.MapNoteType type)
		{
			Vector2 vector = Util.WorldToMap(note.worldPosition);
			AppTeamInfo.Note note2 = Pool.Get<AppTeamInfo.Note>();
			note2.type = (int)type;
			note2.x = vector.x;
			note2.y = vector.y;
			note2.icon = note.icon;
			note2.colourIndex = note.colourIndex;
			note2.label = note.label;
			result.Add(note2);
		}
	}
}
