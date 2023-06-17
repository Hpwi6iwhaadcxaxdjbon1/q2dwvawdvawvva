using System;
using System.Collections;
using System.Runtime.CompilerServices;
using ConVar;
using Facepunch.Extend;
using Facepunch.Math;
using Network;
using UnityEngine;
using UnityEngine.Networking;

// Token: 0x0200073C RID: 1852
public static class Auth_CentralizedBans
{
	// Token: 0x04002A0D RID: 10765
	private static readonly Auth_CentralizedBans.BanPayload payloadData = new Auth_CentralizedBans.BanPayload();

	// Token: 0x06003399 RID: 13209 RVA: 0x0013D5DD File Offset: 0x0013B7DD
	public static IEnumerator Run(Connection connection)
	{
		if (!connection.active)
		{
			yield break;
		}
		if (connection.rejected)
		{
			yield break;
		}
		if (string.IsNullOrWhiteSpace(ConVar.Server.bansServerEndpoint) || !ConVar.Server.bansServerEndpoint.StartsWith("http"))
		{
			yield break;
		}
		connection.authStatus = "";
		if (!ConVar.Server.bansServerEndpoint.EndsWith("/"))
		{
			ConVar.Server.bansServerEndpoint += "/";
		}
		if (connection.ownerid != 0UL && connection.ownerid != connection.userid)
		{
			string uri = ConVar.Server.bansServerEndpoint + connection.ownerid;
			UnityWebRequest ownerRequest = UnityWebRequest.Get(uri);
			ownerRequest.timeout = ConVar.Server.bansServerTimeout;
			yield return ownerRequest.SendWebRequest();
			if (Auth_CentralizedBans.CheckIfPlayerBanned(connection.ownerid, connection, ownerRequest))
			{
				yield break;
			}
			ownerRequest = null;
		}
		string uri2 = ConVar.Server.bansServerEndpoint + connection.userid;
		UnityWebRequest userRequest = UnityWebRequest.Get(uri2);
		userRequest.timeout = ConVar.Server.bansServerTimeout;
		yield return userRequest.SendWebRequest();
		if (Auth_CentralizedBans.CheckIfPlayerBanned(connection.userid, connection, userRequest))
		{
			yield break;
		}
		connection.authStatus = "ok";
		yield break;
	}

	// Token: 0x0600339A RID: 13210 RVA: 0x0013D5EC File Offset: 0x0013B7EC
	private static bool CheckIfPlayerBanned(ulong steamId, Connection connection, UnityWebRequest request)
	{
		Auth_CentralizedBans.<>c__DisplayClass2_0 CS$<>8__locals1;
		CS$<>8__locals1.connection = connection;
		if (request.isNetworkError)
		{
			Debug.LogError("Failed to check centralized bans due to a network error (" + request.error + ")");
			if (ConVar.Server.bansServerFailureMode == 1)
			{
				Auth_CentralizedBans.<CheckIfPlayerBanned>g__Reject|2_0("Centralized Ban Error: Network Error", ref CS$<>8__locals1);
				return true;
			}
			return false;
		}
		else
		{
			if (request.responseCode == 404L)
			{
				return false;
			}
			if (!request.isHttpError)
			{
				bool result;
				try
				{
					Auth_CentralizedBans.payloadData.steamId = 0UL;
					Auth_CentralizedBans.payloadData.reason = null;
					Auth_CentralizedBans.payloadData.expiryDate = 0L;
					JsonUtility.FromJsonOverwrite(request.downloadHandler.text, Auth_CentralizedBans.payloadData);
					if (Auth_CentralizedBans.payloadData.expiryDate > 0L && (long)Epoch.Current >= Auth_CentralizedBans.payloadData.expiryDate)
					{
						result = false;
					}
					else if (Auth_CentralizedBans.payloadData.steamId != steamId)
					{
						Debug.LogError(string.Format("Failed to check centralized bans due to SteamID mismatch (expected {0}, got {1})", steamId, Auth_CentralizedBans.payloadData.steamId));
						if (ConVar.Server.bansServerFailureMode == 1)
						{
							Auth_CentralizedBans.<CheckIfPlayerBanned>g__Reject|2_0("Centralized Ban Error: SteamID Mismatch", ref CS$<>8__locals1);
							result = true;
						}
						else
						{
							result = false;
						}
					}
					else
					{
						string text = Auth_CentralizedBans.payloadData.reason ?? "no reason given";
						string text2 = (Auth_CentralizedBans.payloadData.expiryDate > 0L) ? (" for " + (Auth_CentralizedBans.payloadData.expiryDate - (long)Epoch.Current).FormatSecondsLong()) : "";
						Auth_CentralizedBans.<CheckIfPlayerBanned>g__Reject|2_0(string.Concat(new string[]
						{
							"You are banned from this server",
							text2,
							" (",
							text,
							")"
						}), ref CS$<>8__locals1);
						result = true;
					}
				}
				catch (Exception exception)
				{
					Debug.LogError("Failed to check centralized bans due to a malformed response: " + request.downloadHandler.text);
					Debug.LogException(exception);
					if (ConVar.Server.bansServerFailureMode == 1)
					{
						Auth_CentralizedBans.<CheckIfPlayerBanned>g__Reject|2_0("Centralized Ban Error: Malformed Response", ref CS$<>8__locals1);
						result = true;
					}
					else
					{
						result = false;
					}
				}
				return result;
			}
			Debug.LogError(string.Format("Failed to check centralized bans due to a server error ({0}: {1})", request.responseCode, request.error));
			if (ConVar.Server.bansServerFailureMode == 1)
			{
				Auth_CentralizedBans.<CheckIfPlayerBanned>g__Reject|2_0("Centralized Ban Error: Server Error", ref CS$<>8__locals1);
				return true;
			}
			return false;
		}
	}

	// Token: 0x0600339C RID: 13212 RVA: 0x0013D824 File Offset: 0x0013BA24
	[CompilerGenerated]
	internal static void <CheckIfPlayerBanned>g__Reject|2_0(string reason, ref Auth_CentralizedBans.<>c__DisplayClass2_0 A_1)
	{
		ConnectionAuth.Reject(A_1.connection, reason, null);
		PlatformService.Instance.EndPlayerSession(A_1.connection.userid);
	}

	// Token: 0x02000E44 RID: 3652
	private class BanPayload
	{
		// Token: 0x04004AE2 RID: 19170
		public ulong steamId;

		// Token: 0x04004AE3 RID: 19171
		public string reason;

		// Token: 0x04004AE4 RID: 19172
		public long expiryDate;
	}
}
