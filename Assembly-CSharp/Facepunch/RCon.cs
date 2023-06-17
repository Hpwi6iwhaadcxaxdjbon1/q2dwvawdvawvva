using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using ConVar;
using Facepunch.Rcon;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using UnityEngine;

namespace Facepunch
{
	// Token: 0x02000AED RID: 2797
	public class RCon
	{
		// Token: 0x04003C55 RID: 15445
		public static string Password = "";

		// Token: 0x04003C56 RID: 15446
		[ServerVar]
		public static int Port = 0;

		// Token: 0x04003C57 RID: 15447
		[ServerVar]
		public static string Ip = "";

		// Token: 0x04003C58 RID: 15448
		[ServerVar(Help = "If set to true, use websocket rcon. If set to false use legacy, source engine rcon.")]
		public static bool Web = true;

		// Token: 0x04003C59 RID: 15449
		[ServerVar(Help = "If true, rcon commands etc will be printed in the console")]
		public static bool Print = false;

		// Token: 0x04003C5A RID: 15450
		internal static RCon.RConListener listener = null;

		// Token: 0x04003C5B RID: 15451
		internal static Listener listenerNew = null;

		// Token: 0x04003C5C RID: 15452
		private static Queue<RCon.Command> Commands = new Queue<RCon.Command>();

		// Token: 0x04003C5D RID: 15453
		private static float lastRunTime = 0f;

		// Token: 0x04003C5E RID: 15454
		internal static List<RCon.BannedAddresses> bannedAddresses = new List<RCon.BannedAddresses>();

		// Token: 0x04003C5F RID: 15455
		private static int responseIdentifier;

		// Token: 0x04003C60 RID: 15456
		private static int responseConnection;

		// Token: 0x04003C61 RID: 15457
		private static bool isInput;

		// Token: 0x04003C62 RID: 15458
		internal static int SERVERDATA_AUTH = 3;

		// Token: 0x04003C63 RID: 15459
		internal static int SERVERDATA_EXECCOMMAND = 2;

		// Token: 0x04003C64 RID: 15460
		internal static int SERVERDATA_AUTH_RESPONSE = 2;

		// Token: 0x04003C65 RID: 15461
		internal static int SERVERDATA_RESPONSE_VALUE = 0;

		// Token: 0x04003C66 RID: 15462
		internal static int SERVERDATA_CONSOLE_LOG = 4;

		// Token: 0x04003C67 RID: 15463
		internal static int SERVERDATA_SWITCH_UTF8 = 5;

		// Token: 0x06004381 RID: 17281 RVA: 0x0018E7BC File Offset: 0x0018C9BC
		public static void Initialize()
		{
			if (RCon.Port == 0)
			{
				RCon.Port = Server.port;
			}
			RCon.Password = CommandLine.GetSwitch("-rcon.password", CommandLine.GetSwitch("+rcon.password", ""));
			if (RCon.Password == "password")
			{
				return;
			}
			if (RCon.Password == "")
			{
				return;
			}
			Output.OnMessage += RCon.OnMessage;
			if (RCon.Web)
			{
				RCon.listenerNew = new Listener();
				if (!string.IsNullOrEmpty(RCon.Ip))
				{
					RCon.listenerNew.Address = RCon.Ip;
				}
				RCon.listenerNew.Password = RCon.Password;
				RCon.listenerNew.Port = RCon.Port;
				RCon.listenerNew.SslCertificate = CommandLine.GetSwitch("-rcon.ssl", null);
				RCon.listenerNew.SslCertificatePassword = CommandLine.GetSwitch("-rcon.sslpwd", null);
				RCon.listenerNew.OnMessage = delegate(IPAddress ip, int id, string msg)
				{
					Queue<RCon.Command> commands = RCon.Commands;
					lock (commands)
					{
						RCon.Command item = JsonConvert.DeserializeObject<RCon.Command>(msg);
						item.Ip = ip;
						item.ConnectionId = id;
						RCon.Commands.Enqueue(item);
					}
				};
				RCon.listenerNew.Start();
				Debug.Log("WebSocket RCon Started on " + RCon.Port);
				return;
			}
			RCon.listener = new RCon.RConListener();
			Debug.Log("RCon Started on " + RCon.Port);
			Debug.Log("Source style TCP Rcon is deprecated. Please switch to Websocket Rcon before it goes away.");
		}

		// Token: 0x06004382 RID: 17282 RVA: 0x0018E91E File Offset: 0x0018CB1E
		public static void Shutdown()
		{
			if (RCon.listenerNew != null)
			{
				RCon.listenerNew.Shutdown();
				RCon.listenerNew = null;
			}
			if (RCon.listener != null)
			{
				RCon.listener.Shutdown();
				RCon.listener = null;
			}
		}

		// Token: 0x06004383 RID: 17283 RVA: 0x0018E950 File Offset: 0x0018CB50
		public static void Broadcast(RCon.LogType type, object obj)
		{
			if (RCon.listenerNew == null)
			{
				return;
			}
			string message = JsonConvert.SerializeObject(obj, Formatting.Indented);
			RCon.Broadcast(type, message);
		}

		// Token: 0x06004384 RID: 17284 RVA: 0x0018E974 File Offset: 0x0018CB74
		public static void Broadcast(RCon.LogType type, string message)
		{
			if (RCon.listenerNew == null || string.IsNullOrWhiteSpace(message))
			{
				return;
			}
			RCon.Response response = default(RCon.Response);
			response.Identifier = -1;
			response.Message = message;
			response.Type = type;
			if (RCon.responseConnection < 0)
			{
				RCon.listenerNew.BroadcastMessage(JsonConvert.SerializeObject(response, Formatting.Indented));
				return;
			}
			RCon.listenerNew.SendMessage(RCon.responseConnection, JsonConvert.SerializeObject(response, Formatting.Indented));
		}

		// Token: 0x06004385 RID: 17285 RVA: 0x0018E9EC File Offset: 0x0018CBEC
		public static void Update()
		{
			Queue<RCon.Command> commands = RCon.Commands;
			lock (commands)
			{
				while (RCon.Commands.Count > 0)
				{
					RCon.OnCommand(RCon.Commands.Dequeue());
				}
			}
			if (RCon.listener == null)
			{
				return;
			}
			if (RCon.lastRunTime + 0.02f >= UnityEngine.Time.realtimeSinceStartup)
			{
				return;
			}
			RCon.lastRunTime = UnityEngine.Time.realtimeSinceStartup;
			try
			{
				RCon.bannedAddresses.RemoveAll((RCon.BannedAddresses x) => x.banTime < UnityEngine.Time.realtimeSinceStartup);
				RCon.listener.Cycle();
			}
			catch (Exception exception)
			{
				Debug.LogWarning("Rcon Exception");
				Debug.LogException(exception);
			}
		}

		// Token: 0x06004386 RID: 17286 RVA: 0x0018EABC File Offset: 0x0018CCBC
		public static void BanIP(IPAddress addr, float seconds)
		{
			RCon.bannedAddresses.RemoveAll((RCon.BannedAddresses x) => x.addr == addr);
			RCon.BannedAddresses bannedAddresses = default(RCon.BannedAddresses);
			bannedAddresses.addr = addr;
			bannedAddresses.banTime = UnityEngine.Time.realtimeSinceStartup + seconds;
		}

		// Token: 0x06004387 RID: 17287 RVA: 0x0018EB10 File Offset: 0x0018CD10
		public static bool IsBanned(IPAddress addr)
		{
			return RCon.bannedAddresses.Count((RCon.BannedAddresses x) => x.addr == addr && x.banTime > UnityEngine.Time.realtimeSinceStartup) > 0;
		}

		// Token: 0x06004388 RID: 17288 RVA: 0x0018EB44 File Offset: 0x0018CD44
		private static void OnCommand(RCon.Command cmd)
		{
			try
			{
				RCon.responseIdentifier = cmd.Identifier;
				RCon.responseConnection = cmd.ConnectionId;
				RCon.isInput = true;
				if (RCon.Print)
				{
					Debug.Log(string.Concat(new object[]
					{
						"[rcon] ",
						cmd.Ip,
						": ",
						cmd.Message
					}));
				}
				RCon.isInput = false;
				string text = ConsoleSystem.Run(ConsoleSystem.Option.Server.Quiet(), cmd.Message, Array.Empty<object>());
				if (text != null)
				{
					RCon.OnMessage(text, string.Empty, UnityEngine.LogType.Log);
				}
			}
			finally
			{
				RCon.responseIdentifier = 0;
				RCon.responseConnection = -1;
			}
		}

		// Token: 0x06004389 RID: 17289 RVA: 0x0018EBF8 File Offset: 0x0018CDF8
		private static void OnMessage(string message, string stacktrace, UnityEngine.LogType type)
		{
			if (RCon.isInput)
			{
				return;
			}
			if (RCon.listenerNew != null)
			{
				RCon.Response response = default(RCon.Response);
				response.Identifier = RCon.responseIdentifier;
				response.Message = message;
				response.Stacktrace = stacktrace;
				response.Type = RCon.LogType.Generic;
				if (type == UnityEngine.LogType.Error || type == UnityEngine.LogType.Exception)
				{
					response.Type = RCon.LogType.Error;
				}
				if (type == UnityEngine.LogType.Assert || type == UnityEngine.LogType.Warning)
				{
					response.Type = RCon.LogType.Warning;
				}
				if (RCon.responseConnection < 0)
				{
					RCon.listenerNew.BroadcastMessage(JsonConvert.SerializeObject(response, Formatting.Indented));
					return;
				}
				RCon.listenerNew.SendMessage(RCon.responseConnection, JsonConvert.SerializeObject(response, Formatting.Indented));
			}
		}

		// Token: 0x02000F69 RID: 3945
		public struct Command
		{
			// Token: 0x04004F94 RID: 20372
			public IPAddress Ip;

			// Token: 0x04004F95 RID: 20373
			public int ConnectionId;

			// Token: 0x04004F96 RID: 20374
			public string Name;

			// Token: 0x04004F97 RID: 20375
			public string Message;

			// Token: 0x04004F98 RID: 20376
			public int Identifier;
		}

		// Token: 0x02000F6A RID: 3946
		public enum LogType
		{
			// Token: 0x04004F9A RID: 20378
			Generic,
			// Token: 0x04004F9B RID: 20379
			Error,
			// Token: 0x04004F9C RID: 20380
			Warning,
			// Token: 0x04004F9D RID: 20381
			Chat,
			// Token: 0x04004F9E RID: 20382
			Report,
			// Token: 0x04004F9F RID: 20383
			ClientPerf
		}

		// Token: 0x02000F6B RID: 3947
		public struct Response
		{
			// Token: 0x04004FA0 RID: 20384
			public string Message;

			// Token: 0x04004FA1 RID: 20385
			public int Identifier;

			// Token: 0x04004FA2 RID: 20386
			[JsonConverter(typeof(StringEnumConverter))]
			public RCon.LogType Type;

			// Token: 0x04004FA3 RID: 20387
			public string Stacktrace;
		}

		// Token: 0x02000F6C RID: 3948
		internal struct BannedAddresses
		{
			// Token: 0x04004FA4 RID: 20388
			public IPAddress addr;

			// Token: 0x04004FA5 RID: 20389
			public float banTime;
		}

		// Token: 0x02000F6D RID: 3949
		internal class RConClient
		{
			// Token: 0x04004FA6 RID: 20390
			private Socket socket;

			// Token: 0x04004FA7 RID: 20391
			private bool isAuthorised;

			// Token: 0x04004FA8 RID: 20392
			private string connectionName;

			// Token: 0x04004FA9 RID: 20393
			private int lastMessageID = -1;

			// Token: 0x04004FAA RID: 20394
			private bool runningConsoleCommand;

			// Token: 0x04004FAB RID: 20395
			private bool utf8Mode;

			// Token: 0x060054B3 RID: 21683 RVA: 0x001B5C9A File Offset: 0x001B3E9A
			internal RConClient(Socket cl)
			{
				this.socket = cl;
				this.socket.NoDelay = true;
				this.connectionName = this.socket.RemoteEndPoint.ToString();
			}

			// Token: 0x060054B4 RID: 21684 RVA: 0x001B5CD2 File Offset: 0x001B3ED2
			internal bool IsValid()
			{
				return this.socket != null;
			}

			// Token: 0x060054B5 RID: 21685 RVA: 0x001B5CE0 File Offset: 0x001B3EE0
			internal void Update()
			{
				if (this.socket == null)
				{
					return;
				}
				if (!this.socket.Connected)
				{
					this.Close("Disconnected");
					return;
				}
				int available = this.socket.Available;
				if (available < 14)
				{
					return;
				}
				if (available > 4096)
				{
					this.Close("overflow");
					return;
				}
				byte[] buffer = new byte[available];
				this.socket.Receive(buffer);
				using (BinaryReader binaryReader = new BinaryReader(new MemoryStream(buffer, false), this.utf8Mode ? Encoding.UTF8 : Encoding.ASCII))
				{
					int num = binaryReader.ReadInt32();
					if (available < num)
					{
						this.Close("invalid packet");
					}
					else
					{
						this.lastMessageID = binaryReader.ReadInt32();
						int type = binaryReader.ReadInt32();
						string msg = this.ReadNullTerminatedString(binaryReader);
						this.ReadNullTerminatedString(binaryReader);
						if (!this.HandleMessage(type, msg))
						{
							this.Close("invalid packet");
						}
						else
						{
							this.lastMessageID = -1;
						}
					}
				}
			}

			// Token: 0x060054B6 RID: 21686 RVA: 0x001B5DE4 File Offset: 0x001B3FE4
			internal bool HandleMessage(int type, string msg)
			{
				if (!this.isAuthorised)
				{
					return this.HandleMessage_UnAuthed(type, msg);
				}
				if (type == RCon.SERVERDATA_SWITCH_UTF8)
				{
					this.utf8Mode = true;
					return true;
				}
				if (type == RCon.SERVERDATA_EXECCOMMAND)
				{
					Debug.Log("[RCON][" + this.connectionName + "] " + msg);
					this.runningConsoleCommand = true;
					ConsoleSystem.Run(ConsoleSystem.Option.Server, msg, Array.Empty<object>());
					this.runningConsoleCommand = false;
					this.Reply(-1, RCon.SERVERDATA_RESPONSE_VALUE, "");
					return true;
				}
				if (type == RCon.SERVERDATA_RESPONSE_VALUE)
				{
					this.Reply(this.lastMessageID, RCon.SERVERDATA_RESPONSE_VALUE, "");
					return true;
				}
				Debug.Log(string.Concat(new object[]
				{
					"[RCON][",
					this.connectionName,
					"] Unhandled: ",
					this.lastMessageID,
					" -> ",
					type,
					" -> ",
					msg
				}));
				return false;
			}

			// Token: 0x060054B7 RID: 21687 RVA: 0x001B5EE0 File Offset: 0x001B40E0
			internal bool HandleMessage_UnAuthed(int type, string msg)
			{
				if (type != RCon.SERVERDATA_AUTH)
				{
					RCon.BanIP((this.socket.RemoteEndPoint as IPEndPoint).Address, 60f);
					this.Close("Invalid Command - Not Authed");
					return false;
				}
				this.Reply(this.lastMessageID, RCon.SERVERDATA_RESPONSE_VALUE, "");
				this.isAuthorised = (RCon.Password == msg);
				if (!this.isAuthorised)
				{
					this.Reply(-1, RCon.SERVERDATA_AUTH_RESPONSE, "");
					RCon.BanIP((this.socket.RemoteEndPoint as IPEndPoint).Address, 60f);
					this.Close("Invalid Password");
					return true;
				}
				this.Reply(this.lastMessageID, RCon.SERVERDATA_AUTH_RESPONSE, "");
				Debug.Log("[RCON] Auth: " + this.connectionName);
				Output.OnMessage += this.Output_OnMessage;
				return true;
			}

			// Token: 0x060054B8 RID: 21688 RVA: 0x001B5FCC File Offset: 0x001B41CC
			private void Output_OnMessage(string message, string stacktrace, UnityEngine.LogType type)
			{
				if (!this.isAuthorised)
				{
					return;
				}
				if (!this.IsValid())
				{
					return;
				}
				if (this.lastMessageID != -1 && this.runningConsoleCommand)
				{
					this.Reply(this.lastMessageID, RCon.SERVERDATA_RESPONSE_VALUE, message);
				}
				this.Reply(0, RCon.SERVERDATA_CONSOLE_LOG, message);
			}

			// Token: 0x060054B9 RID: 21689 RVA: 0x001B601C File Offset: 0x001B421C
			internal void Reply(int id, int type, string msg)
			{
				MemoryStream memoryStream = new MemoryStream(1024);
				using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
				{
					if (this.utf8Mode)
					{
						byte[] bytes = Encoding.UTF8.GetBytes(msg);
						int value = 10 + bytes.Length;
						binaryWriter.Write(value);
						binaryWriter.Write(id);
						binaryWriter.Write(type);
						binaryWriter.Write(bytes);
					}
					else
					{
						int value2 = 10 + msg.Length;
						binaryWriter.Write(value2);
						binaryWriter.Write(id);
						binaryWriter.Write(type);
						foreach (char c in msg)
						{
							binaryWriter.Write((sbyte)c);
						}
					}
					binaryWriter.Write(0);
					binaryWriter.Write(0);
					binaryWriter.Flush();
					try
					{
						this.socket.Send(memoryStream.GetBuffer(), (int)memoryStream.Position, SocketFlags.None);
					}
					catch (Exception arg)
					{
						Debug.LogWarning("Error sending rcon reply: " + arg);
						this.Close("Exception");
					}
				}
			}

			// Token: 0x060054BA RID: 21690 RVA: 0x001B6138 File Offset: 0x001B4338
			internal void Close(string strReasn)
			{
				Output.OnMessage -= this.Output_OnMessage;
				if (this.socket == null)
				{
					return;
				}
				Debug.Log("[RCON][" + this.connectionName + "] Disconnected: " + strReasn);
				this.socket.Close();
				this.socket = null;
			}

			// Token: 0x060054BB RID: 21691 RVA: 0x001B618C File Offset: 0x001B438C
			internal string ReadNullTerminatedString(BinaryReader read)
			{
				string text = "";
				while (read.BaseStream.Position != read.BaseStream.Length)
				{
					char c = read.ReadChar();
					if (c == '\0')
					{
						return text;
					}
					text += c.ToString();
					if (text.Length > 8192)
					{
						return string.Empty;
					}
				}
				return "";
			}
		}

		// Token: 0x02000F6E RID: 3950
		internal class RConListener
		{
			// Token: 0x04004FAC RID: 20396
			private TcpListener server;

			// Token: 0x04004FAD RID: 20397
			private List<RCon.RConClient> clients = new List<RCon.RConClient>();

			// Token: 0x060054BC RID: 21692 RVA: 0x001B61EC File Offset: 0x001B43EC
			internal RConListener()
			{
				IPAddress any = IPAddress.Any;
				if (!IPAddress.TryParse(RCon.Ip, out any))
				{
					any = IPAddress.Any;
				}
				this.server = new TcpListener(any, RCon.Port);
				try
				{
					this.server.Start();
				}
				catch (Exception ex)
				{
					Debug.LogWarning("Couldn't start RCON Listener: " + ex.Message);
					this.server = null;
				}
			}

			// Token: 0x060054BD RID: 21693 RVA: 0x001B6274 File Offset: 0x001B4474
			internal void Shutdown()
			{
				if (this.server != null)
				{
					this.server.Stop();
					this.server = null;
				}
			}

			// Token: 0x060054BE RID: 21694 RVA: 0x001B6290 File Offset: 0x001B4490
			internal void Cycle()
			{
				if (this.server == null)
				{
					return;
				}
				this.ProcessConnections();
				this.RemoveDeadClients();
				this.UpdateClients();
			}

			// Token: 0x060054BF RID: 21695 RVA: 0x001B62B0 File Offset: 0x001B44B0
			private void ProcessConnections()
			{
				if (!this.server.Pending())
				{
					return;
				}
				Socket socket = this.server.AcceptSocket();
				if (socket == null)
				{
					return;
				}
				IPEndPoint ipendPoint = socket.RemoteEndPoint as IPEndPoint;
				if (RCon.IsBanned(ipendPoint.Address))
				{
					Debug.Log("[RCON] Ignoring connection - banned. " + ipendPoint.Address.ToString());
					socket.Close();
					return;
				}
				this.clients.Add(new RCon.RConClient(socket));
			}

			// Token: 0x060054C0 RID: 21696 RVA: 0x001B6328 File Offset: 0x001B4528
			private void UpdateClients()
			{
				foreach (RCon.RConClient rconClient in this.clients)
				{
					rconClient.Update();
				}
			}

			// Token: 0x060054C1 RID: 21697 RVA: 0x001B6378 File Offset: 0x001B4578
			private void RemoveDeadClients()
			{
				this.clients.RemoveAll((RCon.RConClient x) => !x.IsValid());
			}
		}
	}
}
