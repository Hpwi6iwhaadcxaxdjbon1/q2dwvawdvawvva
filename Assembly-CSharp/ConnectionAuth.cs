﻿using System;
using System.Collections;
using System.Collections.Generic;
using Facepunch.Extend;
using Facepunch.Math;
using Network;
using UnityEngine;

// Token: 0x0200073F RID: 1855
public class ConnectionAuth : MonoBehaviour
{
	// Token: 0x04002A0F RID: 10767
	[NonSerialized]
	public static List<Connection> m_AuthConnection = new List<Connection>();

	// Token: 0x060033A1 RID: 13217 RVA: 0x0013D92D File Offset: 0x0013BB2D
	public bool IsAuthed(ulong iSteamID)
	{
		return BasePlayer.FindByID(iSteamID) || SingletonComponent<ServerMgr>.Instance.connectionQueue.IsJoining(iSteamID) || SingletonComponent<ServerMgr>.Instance.connectionQueue.IsQueued(iSteamID);
	}

	// Token: 0x060033A2 RID: 13218 RVA: 0x0013D967 File Offset: 0x0013BB67
	public static void Reject(Connection connection, string strReason, string strReasonPrivate = null)
	{
		DebugEx.Log(connection.ToString() + " Rejecting connection - " + (string.IsNullOrEmpty(strReasonPrivate) ? strReason : strReasonPrivate), StackTraceLogType.None);
		Net.sv.Kick(connection, strReason, false);
		ConnectionAuth.m_AuthConnection.Remove(connection);
	}

	// Token: 0x060033A3 RID: 13219 RVA: 0x0013D9A4 File Offset: 0x0013BBA4
	public static void OnDisconnect(Connection connection)
	{
		ConnectionAuth.m_AuthConnection.Remove(connection);
	}

	// Token: 0x060033A4 RID: 13220 RVA: 0x0013D9B2 File Offset: 0x0013BBB2
	public void Approve(Connection connection)
	{
		ConnectionAuth.m_AuthConnection.Remove(connection);
		SingletonComponent<ServerMgr>.Instance.connectionQueue.Join(connection);
	}

	// Token: 0x060033A5 RID: 13221 RVA: 0x0013D9D0 File Offset: 0x0013BBD0
	public void OnNewConnection(Connection connection)
	{
		connection.connected = false;
		if (connection.token == null || connection.token.Length < 32)
		{
			ConnectionAuth.Reject(connection, "Invalid Token", null);
			return;
		}
		if (connection.userid == 0UL)
		{
			ConnectionAuth.Reject(connection, "Invalid SteamID", null);
			return;
		}
		if (connection.protocol != 2392U)
		{
			if (!DeveloperList.Contains(connection.userid))
			{
				ConnectionAuth.Reject(connection, "Incompatible Version", null);
				return;
			}
			DebugEx.Log("Not kicking " + connection.userid + " for incompatible protocol (is a developer)", StackTraceLogType.None);
		}
		if (ServerUsers.Is(connection.userid, ServerUsers.UserGroup.Banned))
		{
			ServerUsers.User user = ServerUsers.Get(connection.userid);
			string text = ((user != null) ? user.notes : null) ?? "no reason given";
			string text2 = (user != null && user.expiry > 0L) ? (" for " + (user.expiry - (long)Epoch.Current).FormatSecondsLong()) : "";
			ConnectionAuth.Reject(connection, string.Concat(new string[]
			{
				"You are banned from this server",
				text2,
				" (",
				text,
				")"
			}), null);
			return;
		}
		if (ServerUsers.Is(connection.userid, ServerUsers.UserGroup.Moderator))
		{
			DebugEx.Log(connection.ToString() + " has auth level 1", StackTraceLogType.None);
			connection.authLevel = 1U;
		}
		if (ServerUsers.Is(connection.userid, ServerUsers.UserGroup.Owner))
		{
			DebugEx.Log(connection.ToString() + " has auth level 2", StackTraceLogType.None);
			connection.authLevel = 2U;
		}
		if (DeveloperList.Contains(connection.userid))
		{
			DebugEx.Log(connection.ToString() + " is a developer", StackTraceLogType.None);
			connection.authLevel = 3U;
		}
		ConnectionAuth.m_AuthConnection.Add(connection);
		base.StartCoroutine(this.AuthorisationRoutine(connection));
	}

	// Token: 0x060033A6 RID: 13222 RVA: 0x0013DB93 File Offset: 0x0013BD93
	public IEnumerator AuthorisationRoutine(Connection connection)
	{
		yield return base.StartCoroutine(Auth_Steam.Run(connection));
		yield return base.StartCoroutine(Auth_EAC.Run(connection));
		yield return base.StartCoroutine(Auth_CentralizedBans.Run(connection));
		if (connection.rejected || !connection.active)
		{
			yield break;
		}
		if (this.IsAuthed(connection.userid))
		{
			ConnectionAuth.Reject(connection, "You are already connected as a player!", null);
			yield break;
		}
		this.Approve(connection);
		yield break;
	}
}
