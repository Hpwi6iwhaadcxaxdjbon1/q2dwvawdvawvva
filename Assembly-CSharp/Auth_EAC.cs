using System;
using System.Collections;
using Network;

// Token: 0x0200073D RID: 1853
public static class Auth_EAC
{
	// Token: 0x0600339D RID: 13213 RVA: 0x0013D848 File Offset: 0x0013BA48
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
		connection.authStatus = string.Empty;
		EACServer.OnJoinGame(connection);
		while (connection.active && !connection.rejected && connection.authStatus == string.Empty)
		{
			yield return null;
		}
		yield break;
	}
}
