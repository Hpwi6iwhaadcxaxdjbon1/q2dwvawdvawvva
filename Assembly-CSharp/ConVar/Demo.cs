using System;
using System.IO;
using Network;
using ProtoBuf;
using UnityEngine;

namespace ConVar
{
	// Token: 0x02000AB4 RID: 2740
	[ConsoleSystem.Factory("demo")]
	public class Demo : ConsoleSystem
	{
		// Token: 0x04003B35 RID: 15157
		public static uint Version = 3U;

		// Token: 0x04003B36 RID: 15158
		[ServerVar]
		public static float splitseconds = 3600f;

		// Token: 0x04003B37 RID: 15159
		[ServerVar]
		public static float splitmegabytes = 200f;

		// Token: 0x04003B38 RID: 15160
		[ServerVar(Saved = true)]
		public static string recordlist = "";

		// Token: 0x04003B39 RID: 15161
		private static int _recordListModeValue = 0;

		// Token: 0x170005A1 RID: 1441
		// (get) Token: 0x060041A4 RID: 16804 RVA: 0x0018595A File Offset: 0x00183B5A
		// (set) Token: 0x060041A5 RID: 16805 RVA: 0x00185961 File Offset: 0x00183B61
		[ServerVar(Saved = true, Help = "Controls the behavior of recordlist, 0=whitelist, 1=blacklist")]
		public static int recordlistmode
		{
			get
			{
				return Demo._recordListModeValue;
			}
			set
			{
				Demo._recordListModeValue = Mathf.Clamp(value, 0, 1);
			}
		}

		// Token: 0x060041A6 RID: 16806 RVA: 0x00185970 File Offset: 0x00183B70
		[ServerVar]
		public static string record(ConsoleSystem.Arg arg)
		{
			global::BasePlayer playerOrSleeper = arg.GetPlayerOrSleeper(0);
			if (!playerOrSleeper || playerOrSleeper.net == null || playerOrSleeper.net.connection == null)
			{
				return "Player not found";
			}
			if (playerOrSleeper.net.connection.IsRecording)
			{
				return "Player already recording a demo";
			}
			playerOrSleeper.StartDemoRecording();
			return null;
		}

		// Token: 0x060041A7 RID: 16807 RVA: 0x001859C8 File Offset: 0x00183BC8
		[ServerVar]
		public static string stop(ConsoleSystem.Arg arg)
		{
			global::BasePlayer playerOrSleeper = arg.GetPlayerOrSleeper(0);
			if (!playerOrSleeper || playerOrSleeper.net == null || playerOrSleeper.net.connection == null)
			{
				return "Player not found";
			}
			if (!playerOrSleeper.net.connection.IsRecording)
			{
				return "Player not recording a demo";
			}
			playerOrSleeper.StopDemoRecording();
			return null;
		}

		// Token: 0x02000F51 RID: 3921
		public class Header : DemoHeader, IDemoHeader
		{
			// Token: 0x17000734 RID: 1844
			// (get) Token: 0x06005471 RID: 21617 RVA: 0x001B54AF File Offset: 0x001B36AF
			// (set) Token: 0x06005472 RID: 21618 RVA: 0x001B54B7 File Offset: 0x001B36B7
			long IDemoHeader.Length
			{
				get
				{
					return this.length;
				}
				set
				{
					this.length = value;
				}
			}

			// Token: 0x06005473 RID: 21619 RVA: 0x001B54C0 File Offset: 0x001B36C0
			public void Write(BinaryWriter writer)
			{
				byte[] array = base.ToProtoBytes();
				writer.Write("RUST DEMO FORMAT");
				writer.Write(array.Length);
				writer.Write(array);
				writer.Write('\0');
			}
		}
	}
}
