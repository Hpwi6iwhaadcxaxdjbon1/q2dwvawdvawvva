using System;
using ConVar;
using Facepunch;
using Facepunch.Extend;
using Network;
using UnityEngine;
using Windows;

// Token: 0x02000311 RID: 785
public class ServerConsole : SingletonComponent<ServerConsole>
{
	// Token: 0x040017BD RID: 6077
	private ConsoleWindow console = new ConsoleWindow();

	// Token: 0x040017BE RID: 6078
	private ConsoleInput input = new ConsoleInput();

	// Token: 0x040017BF RID: 6079
	private float nextUpdate;

	// Token: 0x06001EAB RID: 7851 RVA: 0x000D0E68 File Offset: 0x000CF068
	public void OnEnable()
	{
		this.console.Initialize();
		this.input.OnInputText += this.OnInputText;
		Output.OnMessage += this.HandleLog;
		this.input.ClearLine(System.Console.WindowHeight);
		for (int i = 0; i < System.Console.WindowHeight; i++)
		{
			System.Console.WriteLine("");
		}
	}

	// Token: 0x06001EAC RID: 7852 RVA: 0x000D0ED2 File Offset: 0x000CF0D2
	private void OnDisable()
	{
		Output.OnMessage -= this.HandleLog;
		this.input.OnInputText -= this.OnInputText;
		this.console.Shutdown();
	}

	// Token: 0x06001EAD RID: 7853 RVA: 0x000D0F07 File Offset: 0x000CF107
	private void OnInputText(string obj)
	{
		ConsoleSystem.Run(ConsoleSystem.Option.Server, obj, Array.Empty<object>());
	}

	// Token: 0x06001EAE RID: 7854 RVA: 0x000D0F1C File Offset: 0x000CF11C
	public static void PrintColoured(params object[] objects)
	{
		if (SingletonComponent<ServerConsole>.Instance == null)
		{
			return;
		}
		SingletonComponent<ServerConsole>.Instance.input.ClearLine(SingletonComponent<ServerConsole>.Instance.input.statusText.Length);
		for (int i = 0; i < objects.Length; i++)
		{
			if (i % 2 == 0)
			{
				System.Console.ForegroundColor = (ConsoleColor)objects[i];
			}
			else
			{
				System.Console.Write((string)objects[i]);
			}
		}
		if (System.Console.CursorLeft != 0)
		{
			System.Console.CursorTop++;
		}
		SingletonComponent<ServerConsole>.Instance.input.RedrawInputLine();
	}

	// Token: 0x06001EAF RID: 7855 RVA: 0x000D0FA8 File Offset: 0x000CF1A8
	private void HandleLog(string message, string stackTrace, LogType type)
	{
		if (message.StartsWith("[CHAT]") || message.StartsWith("[TEAM CHAT]") || message.StartsWith("[CARDS CHAT]"))
		{
			return;
		}
		if (type == LogType.Warning)
		{
			if (message.StartsWith("HDR RenderTexture format is not") || message.StartsWith("The image effect") || message.StartsWith("Image Effects are not supported on this platform") || message.StartsWith("[AmplifyColor]") || message.StartsWith("Skipping profile frame.") || message.StartsWith("Kinematic body only supports Speculative Continuous collision detection"))
			{
				return;
			}
			System.Console.ForegroundColor = ConsoleColor.Yellow;
		}
		else if (type == LogType.Error)
		{
			System.Console.ForegroundColor = ConsoleColor.Red;
		}
		else if (type == LogType.Exception)
		{
			System.Console.ForegroundColor = ConsoleColor.Red;
		}
		else if (type == LogType.Assert)
		{
			System.Console.ForegroundColor = ConsoleColor.Red;
		}
		else
		{
			System.Console.ForegroundColor = ConsoleColor.Gray;
		}
		this.input.ClearLine(this.input.statusText.Length);
		System.Console.WriteLine(message);
		this.input.RedrawInputLine();
	}

	// Token: 0x06001EB0 RID: 7856 RVA: 0x000D108E File Offset: 0x000CF28E
	private void Update()
	{
		this.UpdateStatus();
		this.input.Update();
	}

	// Token: 0x06001EB1 RID: 7857 RVA: 0x000D10A4 File Offset: 0x000CF2A4
	private void UpdateStatus()
	{
		if (this.nextUpdate > UnityEngine.Time.realtimeSinceStartup)
		{
			return;
		}
		if (Network.Net.sv == null || !Network.Net.sv.IsConnected())
		{
			return;
		}
		this.nextUpdate = UnityEngine.Time.realtimeSinceStartup + 0.33f;
		if (!this.input.valid)
		{
			return;
		}
		string text = ((long)UnityEngine.Time.realtimeSinceStartup).FormatSeconds();
		string text2 = this.currentGameTime.ToString("[H:mm]");
		string text3 = string.Concat(new object[]
		{
			" ",
			text2,
			" [",
			this.currentPlayerCount,
			"/",
			this.maxPlayerCount,
			"] ",
			ConVar.Server.hostname,
			" [",
			ConVar.Server.level,
			"]"
		});
		string text4 = string.Concat(new object[]
		{
			global::Performance.current.frameRate,
			"fps ",
			global::Performance.current.memoryCollections,
			"gc ",
			text
		}) ?? "";
		string text5 = Network.Net.sv.GetStat(null, BaseNetwork.StatTypeLong.BytesReceived_LastSecond).FormatBytes(true) + "/s in, " + Network.Net.sv.GetStat(null, BaseNetwork.StatTypeLong.BytesSent_LastSecond).FormatBytes(true) + "/s out";
		string text6 = text4.PadLeft(this.input.lineWidth - 1);
		text6 = text3 + ((text3.Length < text6.Length) ? text6.Substring(text3.Length) : "");
		string text7 = string.Concat(new string[]
		{
			" ",
			this.currentEntityCount.ToString("n0"),
			" ents, ",
			this.currentSleeperCount.ToString("n0"),
			" slprs"
		});
		string text8 = text5.PadLeft(this.input.lineWidth - 1);
		text8 = text7 + ((text7.Length < text8.Length) ? text8.Substring(text7.Length) : "");
		this.input.statusText[0] = "";
		this.input.statusText[1] = text6;
		this.input.statusText[2] = text8;
	}

	// Token: 0x1700027E RID: 638
	// (get) Token: 0x06001EB2 RID: 7858 RVA: 0x000D1307 File Offset: 0x000CF507
	private DateTime currentGameTime
	{
		get
		{
			if (!TOD_Sky.Instance)
			{
				return DateTime.Now;
			}
			return TOD_Sky.Instance.Cycle.DateTime;
		}
	}

	// Token: 0x1700027F RID: 639
	// (get) Token: 0x06001EB3 RID: 7859 RVA: 0x000D132A File Offset: 0x000CF52A
	private int currentPlayerCount
	{
		get
		{
			return BasePlayer.activePlayerList.Count;
		}
	}

	// Token: 0x17000280 RID: 640
	// (get) Token: 0x06001EB4 RID: 7860 RVA: 0x000D1336 File Offset: 0x000CF536
	private int maxPlayerCount
	{
		get
		{
			return ConVar.Server.maxplayers;
		}
	}

	// Token: 0x17000281 RID: 641
	// (get) Token: 0x06001EB5 RID: 7861 RVA: 0x000D133D File Offset: 0x000CF53D
	private int currentEntityCount
	{
		get
		{
			return BaseNetworkable.serverEntities.Count;
		}
	}

	// Token: 0x17000282 RID: 642
	// (get) Token: 0x06001EB6 RID: 7862 RVA: 0x000D1349 File Offset: 0x000CF549
	private int currentSleeperCount
	{
		get
		{
			return BasePlayer.sleepingPlayerList.Count;
		}
	}
}
