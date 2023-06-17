using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using ConVar;
using Epic.OnlineServices;
using Epic.OnlineServices.Version;
using Network;
using UnityEngine;

namespace Facepunch.Rust
{
	// Token: 0x02000B02 RID: 2818
	public class PerformanceLogging
	{
		// Token: 0x04003CE8 RID: 15592
		public static PerformanceLogging server = new PerformanceLogging(false);

		// Token: 0x04003CE9 RID: 15593
		public static PerformanceLogging client = new PerformanceLogging(true);

		// Token: 0x04003CEA RID: 15594
		private readonly TimeSpan ClientInterval = TimeSpan.FromMinutes(10.0);

		// Token: 0x04003CEB RID: 15595
		private readonly TimeSpan ServerInterval = TimeSpan.FromMinutes(1.0);

		// Token: 0x04003CEC RID: 15596
		private readonly TimeSpan PublicServerInterval = TimeSpan.FromHours(1.0);

		// Token: 0x04003CED RID: 15597
		private readonly TimeSpan PingInterval = TimeSpan.FromSeconds(5.0);

		// Token: 0x04003CEE RID: 15598
		private List<TimeSpan> Frametimes = new List<TimeSpan>();

		// Token: 0x04003CEF RID: 15599
		private List<int> PingHistory = new List<int>();

		// Token: 0x04003CF0 RID: 15600
		private List<PerformanceLogging.LagSpike> lagSpikes = new List<PerformanceLogging.LagSpike>();

		// Token: 0x04003CF1 RID: 15601
		private List<PerformanceLogging.GarbageCollect> garbageCollections = new List<PerformanceLogging.GarbageCollect>();

		// Token: 0x04003CF2 RID: 15602
		private bool isClient;

		// Token: 0x04003CF3 RID: 15603
		private Stopwatch frameWatch = new Stopwatch();

		// Token: 0x04003CF4 RID: 15604
		private DateTime nextPingTime;

		// Token: 0x04003CF5 RID: 15605
		private DateTime nextFlushTime;

		// Token: 0x04003CF6 RID: 15606
		private DateTime connectedTime;

		// Token: 0x04003CF7 RID: 15607
		private int serverIndex;

		// Token: 0x04003CF8 RID: 15608
		private Guid totalSessionId = Guid.NewGuid();

		// Token: 0x04003CF9 RID: 15609
		private Guid sessionId;

		// Token: 0x04003CFA RID: 15610
		private int lastFrameGC;

		// Token: 0x04003CFB RID: 15611
		private ConcurrentQueue<PerformanceLogging.PerformancePool> pool = new ConcurrentQueue<PerformanceLogging.PerformancePool>();

		// Token: 0x04003CFC RID: 15612
		private Type oxideType;

		// Token: 0x04003CFD RID: 15613
		private bool hasOxideType;

		// Token: 0x04003CFE RID: 15614
		private List<TimeSpan> sortedList = new List<TimeSpan>();

		// Token: 0x060044D9 RID: 17625 RVA: 0x00193DA0 File Offset: 0x00191FA0
		public PerformanceLogging(bool client)
		{
			this.isClient = client;
		}

		// Token: 0x060044DA RID: 17626 RVA: 0x00193E62 File Offset: 0x00192062
		private TimeSpan GetLagSpikeThreshold()
		{
			if (!this.isClient)
			{
				return TimeSpan.FromMilliseconds(200.0);
			}
			return TimeSpan.FromMilliseconds(100.0);
		}

		// Token: 0x060044DB RID: 17627 RVA: 0x00193E8C File Offset: 0x0019208C
		public void OnFrame()
		{
			TimeSpan elapsed = this.frameWatch.Elapsed;
			this.Frametimes.Add(elapsed);
			this.frameWatch.Restart();
			DateTime utcNow = DateTime.UtcNow;
			int num = System.GC.CollectionCount(0);
			bool flag = this.lastFrameGC != num;
			this.lastFrameGC = num;
			if (flag)
			{
				this.garbageCollections.Add(new PerformanceLogging.GarbageCollect
				{
					FrameIndex = this.Frametimes.Count - 1,
					Time = elapsed
				});
			}
			if (elapsed > this.GetLagSpikeThreshold())
			{
				this.lagSpikes.Add(new PerformanceLogging.LagSpike
				{
					FrameIndex = this.Frametimes.Count - 1,
					Time = elapsed,
					WasGC = flag
				});
			}
			if (utcNow > this.nextFlushTime)
			{
				if (this.nextFlushTime == default(DateTime))
				{
					this.nextFlushTime = DateTime.UtcNow.Add(this.GetFlushInterval());
					return;
				}
				this.Flush();
			}
		}

		// Token: 0x060044DC RID: 17628 RVA: 0x00193F9C File Offset: 0x0019219C
		public void Flush()
		{
			PerformanceLogging.<>c__DisplayClass31_0 CS$<>8__locals1 = new PerformanceLogging.<>c__DisplayClass31_0();
			CS$<>8__locals1.<>4__this = this;
			this.nextFlushTime = DateTime.UtcNow.Add(this.GetFlushInterval());
			if (!this.isClient && BasePlayer.activePlayerList.Count == 0 && !Analytics.Azure.Stats)
			{
				this.ResetMeasurements();
				return;
			}
			Stopwatch stopwatch = Stopwatch.StartNew();
			CS$<>8__locals1.record = EventRecord.New(this.isClient ? "client_performance" : "server_performance", !this.isClient);
			CS$<>8__locals1.record.AddField("lag_spike_count", this.lagSpikes.Count).AddField("lag_spike_threshold", this.GetLagSpikeThreshold()).AddField("gc_count", this.garbageCollections.Count).AddField("ram_managed", System.GC.GetTotalMemory(false)).AddField("ram_total", SystemInfoEx.systemMemoryUsed).AddField("total_session_id", this.totalSessionId.ToString("N")).AddField("uptime", (int)UnityEngine.Time.realtimeSinceStartup).AddField("map_url", global::World.Url).AddField("world_size", global::World.Size).AddField("world_seed", global::World.Seed).AddField("active_scene", LevelManager.CurrentLevelName);
			if (!this.isClient && !this.isClient)
			{
				int value = (Network.Net.sv == null) ? 0 : ((int)Network.Net.sv.GetStat(null, BaseNetwork.StatTypeLong.BytesReceived_LastSecond));
				int value2 = (Network.Net.sv == null) ? 0 : ((int)Network.Net.sv.GetStat(null, BaseNetwork.StatTypeLong.BytesSent_LastSecond));
				CS$<>8__locals1.record.AddField("is_official", ConVar.Server.official && ConVar.Server.stats).AddField("bot_count", BasePlayer.bots.Count).AddField("player_count", BasePlayer.activePlayerList.Count).AddField("max_players", ConVar.Server.maxplayers).AddField("ent_count", BaseNetworkable.serverEntities.Count).AddField("hostname", ConVar.Server.hostname).AddField("net_in", value).AddField("net_out", value2);
			}
			if (!this.isClient)
			{
				try
				{
					if (!this.hasOxideType)
					{
						this.oxideType = Type.GetType("Oxide.Core.Interface,Oxide.Core");
						this.hasOxideType = true;
					}
					if (this.oxideType != null)
					{
						CS$<>8__locals1.record.AddField("is_oxide", true);
						PropertyInfo property = this.oxideType.GetProperty("Oxide", BindingFlags.Static | BindingFlags.Public);
						object obj = (property != null) ? property.GetValue(null) : null;
						if (obj != null)
						{
							PropertyInfo property2 = obj.GetType().GetProperty("RootPluginManager", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
							object obj2 = (property2 != null) ? property2.GetValue(obj) : null;
							if (obj2 != null)
							{
								List<PerformanceLogging.PluginInfo> list = new List<PerformanceLogging.PluginInfo>();
								MethodInfo method = obj2.GetType().GetMethod("GetPlugins");
								foreach (object obj3 in (((method != null) ? method.Invoke(obj2, null) : null) as IEnumerable))
								{
									if (obj3 != null)
									{
										PropertyInfo property3 = obj3.GetType().GetProperty("Name");
										string name = ((property3 != null) ? property3.GetValue(obj3) : null) as string;
										PropertyInfo property4 = obj3.GetType().GetProperty("Author");
										string author = ((property4 != null) ? property4.GetValue(obj3) : null) as string;
										PropertyInfo property5 = obj3.GetType().GetProperty("Version");
										string text;
										if (property5 == null)
										{
											text = null;
										}
										else
										{
											object value3 = property5.GetValue(obj3);
											text = ((value3 != null) ? value3.ToString() : null);
										}
										string version = text;
										list.Add(new PerformanceLogging.PluginInfo
										{
											Name = name,
											Author = author,
											Version = version
										});
									}
								}
								CS$<>8__locals1.record.AddObject("oxide_plugins", list);
								CS$<>8__locals1.record.AddField("oxide_plugin_count", list.Count);
							}
						}
					}
				}
				catch (Exception arg)
				{
					UnityEngine.Debug.LogError(string.Format("Failed to get oxide when flushing server performance: {0}", arg));
				}
				try
				{
					List<PerformanceLogging.ProcessInfo> list2 = new List<PerformanceLogging.ProcessInfo>();
					Process[] processes = Process.GetProcesses();
					Process currentProcess = Process.GetCurrentProcess();
					foreach (Process process in processes)
					{
						try
						{
							if (currentProcess.Id != process.Id)
							{
								if (process.ProcessName.Contains("RustDedicated"))
								{
									list2.Add(new PerformanceLogging.ProcessInfo
									{
										Name = process.ProcessName,
										WorkingSet = process.WorkingSet64
									});
								}
							}
						}
						catch (Exception ex)
						{
							if (!(ex is InvalidOperationException))
							{
								UnityEngine.Debug.LogWarning(string.Format("Failed to get memory from process when flushing performance info: {0}", ex));
								list2.Add(new PerformanceLogging.ProcessInfo
								{
									Name = process.ProcessName,
									WorkingSet = -1L
								});
							}
						}
					}
					CS$<>8__locals1.record.AddObject("other_servers", list2);
					CS$<>8__locals1.record.AddField("other_server_count", list2.Count);
				}
				catch (Exception arg2)
				{
					UnityEngine.Debug.LogError(string.Format("Failed to log processes when flushing performance info: {0}", arg2));
				}
			}
			if (!this.isClient)
			{
				IEnumerable<HarmonyModInfo> harmonyMods = HarmonyLoader.GetHarmonyMods();
				CS$<>8__locals1.record.AddObject("harmony_mods", harmonyMods);
				CS$<>8__locals1.record.AddField("harmony_mod_count", harmonyMods.Count<HarmonyModInfo>());
			}
			string value4;
			using (SHA256 sha = SHA256.Create())
			{
				value4 = Convert.ToBase64String(sha.ComputeHash(Encoding.UTF8.GetBytes(SystemInfo.deviceUniqueIdentifier)));
			}
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary["device_name"] = SystemInfo.deviceName;
			dictionary["device_hash"] = value4;
			dictionary["gpu_name"] = SystemInfo.graphicsDeviceName;
			string key = "gpu_ram";
			int i = SystemInfo.graphicsMemorySize;
			dictionary[key] = i.ToString();
			dictionary["gpu_vendor"] = SystemInfo.graphicsDeviceVendor;
			dictionary["gpu_version"] = SystemInfo.graphicsDeviceVersion;
			string key2 = "cpu_cores";
			i = SystemInfo.processorCount;
			dictionary[key2] = i.ToString();
			string key3 = "cpu_frequency";
			i = SystemInfo.processorFrequency;
			dictionary[key3] = i.ToString();
			dictionary["cpu_name"] = SystemInfo.processorType.Trim();
			string key4 = "system_memory";
			i = SystemInfo.systemMemorySize;
			dictionary[key4] = i.ToString();
			dictionary["os"] = SystemInfo.operatingSystem;
			Dictionary<string, string> data = dictionary;
			Dictionary<string, string> dictionary2 = new Dictionary<string, string>();
			dictionary2["unity"] = (Application.unityVersion ?? "editor");
			string key5 = "changeset";
			BuildInfo buildInfo = BuildInfo.Current;
			dictionary2[key5] = (((buildInfo != null) ? buildInfo.Scm.ChangeId : null) ?? "editor");
			string key6 = "branch";
			BuildInfo buildInfo2 = BuildInfo.Current;
			dictionary2[key6] = (((buildInfo2 != null) ? buildInfo2.Scm.Branch : null) ?? "editor");
			string key7 = "network_version";
			i = 2392;
			dictionary2[key7] = i.ToString();
			Dictionary<string, string> dictionary3 = dictionary2;
			Utf8String version2 = VersionInterface.GetVersion();
			dictionary3["eos_sdk"] = (((version2 != null) ? version2.ToString() : null) ?? "disabled");
			CS$<>8__locals1.record.AddObject("hardware", data).AddObject("application", dictionary3);
			stopwatch.Stop();
			CS$<>8__locals1.record.AddField("flush_ms", stopwatch.ElapsedMilliseconds);
			CS$<>8__locals1.frametimes = this.Frametimes;
			CS$<>8__locals1.ping = this.PingHistory;
			Task.Run(delegate()
			{
				PerformanceLogging.<>c__DisplayClass31_0.<<Flush>b__0>d <<Flush>b__0>d;
				<<Flush>b__0>d.<>4__this = CS$<>8__locals1;
				<<Flush>b__0>d.<>t__builder = AsyncTaskMethodBuilder.Create();
				<<Flush>b__0>d.<>1__state = -1;
				AsyncTaskMethodBuilder <>t__builder = <<Flush>b__0>d.<>t__builder;
				<>t__builder.Start<PerformanceLogging.<>c__DisplayClass31_0.<<Flush>b__0>d>(ref <<Flush>b__0>d);
				return <<Flush>b__0>d.<>t__builder.Task;
			});
			this.ResetMeasurements();
		}

		// Token: 0x060044DD RID: 17629 RVA: 0x001947AC File Offset: 0x001929AC
		private TimeSpan GetFlushInterval()
		{
			if (this.isClient)
			{
				return TimeSpan.FromHours(1.0);
			}
			if (Analytics.Azure.Stats)
			{
				return this.ServerInterval;
			}
			return this.PublicServerInterval;
		}

		// Token: 0x060044DE RID: 17630 RVA: 0x001947DC File Offset: 0x001929DC
		private void ResetMeasurements()
		{
			this.nextFlushTime = DateTime.UtcNow.Add(this.GetFlushInterval());
			if (this.Frametimes.Count == 0)
			{
				return;
			}
			PerformanceLogging.PerformancePool performancePool;
			while (this.pool.TryDequeue(out performancePool))
			{
				Pool.FreeList<TimeSpan>(ref performancePool.Frametimes);
				Pool.FreeList<int>(ref performancePool.Ping);
			}
			this.Frametimes = Pool.GetList<TimeSpan>();
			this.PingHistory = Pool.GetList<int>();
			this.garbageCollections.Clear();
		}

		// Token: 0x060044DF RID: 17631 RVA: 0x00194858 File Offset: 0x00192A58
		private Task ProcessPerformanceData(EventRecord record, List<TimeSpan> frametimes, List<int> ping)
		{
			if (frametimes.Count <= 1)
			{
				return Task.CompletedTask;
			}
			this.sortedList.Clear();
			this.sortedList.AddRange(frametimes);
			this.sortedList.Sort();
			int count = frametimes.Count;
			Mathf.Max(1, frametimes.Count / 100);
			Mathf.Max(1, frametimes.Count / 1000);
			TimeSpan timeSpan = default(TimeSpan);
			for (int i = 0; i < count; i++)
			{
				TimeSpan t = this.sortedList[i];
				timeSpan += t;
			}
			double frametime_average = timeSpan.TotalMilliseconds / (double)count;
			double value = Math.Sqrt(this.sortedList.Sum((TimeSpan x) => Math.Pow(x.TotalMilliseconds - frametime_average, 2.0)) / (double)this.sortedList.Count - 1.0);
			record.AddField("total_time", timeSpan).AddField("frames", count).AddField("frametime_average", timeSpan.TotalSeconds / (double)count).AddField("frametime_99_9", this.sortedList[Mathf.Clamp(count - count / 1000, 0, count - 1)]).AddField("frametime_99", this.sortedList[Mathf.Clamp(count - count / 100, 0, count - 1)]).AddField("frametime_90", this.sortedList[Mathf.Clamp(count - count / 10, 0, count - 1)]).AddField("frametime_75", this.sortedList[Mathf.Clamp(count - count / 4, 0, count - 1)]).AddField("frametime_50", this.sortedList[count / 2]).AddField("frametime_25", this.sortedList[count / 4]).AddField("frametime_10", this.sortedList[count / 10]).AddField("frametime_1", this.sortedList[count / 100]).AddField("frametime_0_1", this.sortedList[count / 1000]).AddField("frametime_std_dev", value).AddField("gc_generations", System.GC.MaxGeneration).AddField("gc_total", System.GC.CollectionCount(System.GC.MaxGeneration));
			if (this.isClient)
			{
				record.AddField("ping_average", (ping.Count == 0) ? 0 : ((int)ping.Average())).AddField("ping_count", ping.Count);
			}
			record.Submit();
			frametimes.Clear();
			ping.Clear();
			this.pool.Enqueue(new PerformanceLogging.PerformancePool
			{
				Frametimes = frametimes,
				Ping = ping
			});
			return Task.CompletedTask;
		}

		// Token: 0x02000F87 RID: 3975
		private struct LagSpike
		{
			// Token: 0x04005020 RID: 20512
			public int FrameIndex;

			// Token: 0x04005021 RID: 20513
			public TimeSpan Time;

			// Token: 0x04005022 RID: 20514
			public bool WasGC;
		}

		// Token: 0x02000F88 RID: 3976
		private struct GarbageCollect
		{
			// Token: 0x04005023 RID: 20515
			public int FrameIndex;

			// Token: 0x04005024 RID: 20516
			public TimeSpan Time;
		}

		// Token: 0x02000F89 RID: 3977
		private class PerformancePool
		{
			// Token: 0x04005025 RID: 20517
			public List<TimeSpan> Frametimes;

			// Token: 0x04005026 RID: 20518
			public List<int> Ping;
		}

		// Token: 0x02000F8A RID: 3978
		private struct PluginInfo
		{
			// Token: 0x04005027 RID: 20519
			public string Name;

			// Token: 0x04005028 RID: 20520
			public string Author;

			// Token: 0x04005029 RID: 20521
			public string Version;
		}

		// Token: 0x02000F8B RID: 3979
		private struct ProcessInfo
		{
			// Token: 0x0400502A RID: 20522
			public string Name;

			// Token: 0x0400502B RID: 20523
			public long WorkingSet;
		}
	}
}
