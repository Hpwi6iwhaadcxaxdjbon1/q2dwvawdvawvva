using System;
using System.Collections.Generic;
using ConVar;
using Network;
using UnityEngine;

// Token: 0x02000359 RID: 857
public static class ConsoleNetwork
{
	// Token: 0x06001F46 RID: 8006 RVA: 0x000063A5 File Offset: 0x000045A5
	internal static void Init()
	{
	}

	// Token: 0x06001F47 RID: 8007 RVA: 0x000D3BA0 File Offset: 0x000D1DA0
	internal static void OnClientCommand(Message packet)
	{
		if (packet.read.Unread > ConVar.Server.maxpacketsize_command)
		{
			Debug.LogWarning("Dropping client command due to size");
			return;
		}
		string text = packet.read.StringRaw(8388608U);
		if (packet.connection == null || !packet.connection.connected)
		{
			Debug.LogWarning("Client without connection tried to run command: " + text);
			return;
		}
		string text2 = ConsoleSystem.Run(ConsoleSystem.Option.Server.FromConnection(packet.connection).Quiet(), text, Array.Empty<object>());
		if (!string.IsNullOrEmpty(text2))
		{
			ConsoleNetwork.SendClientReply(packet.connection, text2);
		}
	}

	// Token: 0x06001F48 RID: 8008 RVA: 0x000D3C3C File Offset: 0x000D1E3C
	internal static void SendClientReply(Connection cn, string strCommand)
	{
		if (!Network.Net.sv.IsConnected())
		{
			return;
		}
		NetWrite netWrite = Network.Net.sv.StartWrite();
		netWrite.PacketID(Message.Type.ConsoleMessage);
		netWrite.String(strCommand);
		netWrite.Send(new SendInfo(cn));
	}

	// Token: 0x06001F49 RID: 8009 RVA: 0x000D3C70 File Offset: 0x000D1E70
	public static void SendClientCommand(Connection cn, string strCommand, params object[] args)
	{
		if (!Network.Net.sv.IsConnected())
		{
			return;
		}
		NetWrite netWrite = Network.Net.sv.StartWrite();
		netWrite.PacketID(Message.Type.ConsoleCommand);
		string val = ConsoleSystem.BuildCommand(strCommand, args);
		netWrite.String(val);
		netWrite.Send(new SendInfo(cn));
	}

	// Token: 0x06001F4A RID: 8010 RVA: 0x000D3CB6 File Offset: 0x000D1EB6
	public static void SendClientCommand(List<Connection> cn, string strCommand, params object[] args)
	{
		if (!Network.Net.sv.IsConnected())
		{
			return;
		}
		NetWrite netWrite = Network.Net.sv.StartWrite();
		netWrite.PacketID(Message.Type.ConsoleCommand);
		netWrite.String(ConsoleSystem.BuildCommand(strCommand, args));
		netWrite.Send(new SendInfo(cn));
	}

	// Token: 0x06001F4B RID: 8011 RVA: 0x000D3CF0 File Offset: 0x000D1EF0
	public static void BroadcastToAllClients(string strCommand, params object[] args)
	{
		if (!Network.Net.sv.IsConnected())
		{
			return;
		}
		NetWrite netWrite = Network.Net.sv.StartWrite();
		netWrite.PacketID(Message.Type.ConsoleCommand);
		netWrite.String(ConsoleSystem.BuildCommand(strCommand, args));
		netWrite.Send(new SendInfo(Network.Net.sv.connections));
	}
}
