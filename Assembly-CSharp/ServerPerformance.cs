using System;
using System.IO;
using System.Linq;
using Facepunch;
using Rust;
using UnityEngine;
using UnityEngine.Profiling;

// Token: 0x02000744 RID: 1860
public class ServerPerformance : BaseMonoBehaviour
{
	// Token: 0x04002A27 RID: 10791
	public static ulong deaths;

	// Token: 0x04002A28 RID: 10792
	public static ulong spawns;

	// Token: 0x04002A29 RID: 10793
	public static ulong position_changes;

	// Token: 0x04002A2A RID: 10794
	private string fileName;

	// Token: 0x04002A2B RID: 10795
	private int lastFrame;

	// Token: 0x0600340D RID: 13325 RVA: 0x001420BC File Offset: 0x001402BC
	private void Start()
	{
		if (!Profiler.supported)
		{
			return;
		}
		if (!CommandLine.HasSwitch("-perf"))
		{
			return;
		}
		this.fileName = "perfdata." + DateTime.Now.ToString() + ".txt";
		this.fileName = this.fileName.Replace('\\', '-');
		this.fileName = this.fileName.Replace('/', '-');
		this.fileName = this.fileName.Replace(' ', '_');
		this.fileName = this.fileName.Replace(':', '.');
		this.lastFrame = Time.frameCount;
		File.WriteAllText(this.fileName, "MemMono,MemUnity,Frame,PlayerCount,Sleepers,CollidersDisabled,BehavioursDisabled,GameObjects,Colliders,RigidBodies,BuildingBlocks,nwSend,nwRcv,cnInit,cnApp,cnRej,deaths,spawns,poschange\r\n");
		base.InvokeRepeating(new Action(this.WriteLine), 1f, 60f);
	}

	// Token: 0x0600340E RID: 13326 RVA: 0x0014218C File Offset: 0x0014038C
	private void WriteLine()
	{
		Rust.GC.Collect();
		uint monoUsedSize = Profiler.GetMonoUsedSize();
		uint usedHeapSize = Profiler.usedHeapSize;
		int count = BasePlayer.activePlayerList.Count;
		int count2 = BasePlayer.sleepingPlayerList.Count;
		int num = UnityEngine.Object.FindObjectsOfType<GameObject>().Length;
		int num2 = 0;
		int num3 = 0;
		int num4 = 0;
		int num5 = 0;
		int num6 = 0;
		int num7 = Time.frameCount - this.lastFrame;
		File.AppendAllText(this.fileName, string.Concat(new object[]
		{
			monoUsedSize,
			",",
			usedHeapSize,
			",",
			num7,
			",",
			count,
			",",
			count2,
			",",
			NetworkSleep.totalCollidersDisabled,
			",",
			NetworkSleep.totalBehavioursDisabled,
			",",
			num,
			",",
			UnityEngine.Object.FindObjectsOfType<Collider>().Length,
			",",
			UnityEngine.Object.FindObjectsOfType<Rigidbody>().Length,
			",",
			UnityEngine.Object.FindObjectsOfType<BuildingBlock>().Length,
			",",
			num2,
			",",
			num3,
			",",
			num4,
			",",
			num5,
			",",
			num6,
			",",
			ServerPerformance.deaths,
			",",
			ServerPerformance.spawns,
			",",
			ServerPerformance.position_changes,
			"\r\n"
		}));
		this.lastFrame = Time.frameCount;
		ServerPerformance.deaths = 0UL;
		ServerPerformance.spawns = 0UL;
		ServerPerformance.position_changes = 0UL;
	}

	// Token: 0x0600340F RID: 13327 RVA: 0x001423AC File Offset: 0x001405AC
	public static void DoReport()
	{
		string text = "report." + DateTime.Now.ToString() + ".txt";
		text = text.Replace('\\', '-');
		text = text.Replace('/', '-');
		text = text.Replace(' ', '_');
		text = text.Replace(':', '.');
		File.WriteAllText(text, "Report Generated " + DateTime.Now.ToString() + "\r\n");
		string filename = text;
		string title = "All Objects";
		UnityEngine.Object[] objects = UnityEngine.Object.FindObjectsOfType<Transform>();
		ServerPerformance.ComponentReport(filename, title, objects);
		string filename2 = text;
		string title2 = "Entities";
		objects = UnityEngine.Object.FindObjectsOfType<BaseEntity>();
		ServerPerformance.ComponentReport(filename2, title2, objects);
		string filename3 = text;
		string title3 = "Rigidbodies";
		objects = UnityEngine.Object.FindObjectsOfType<Rigidbody>();
		ServerPerformance.ComponentReport(filename3, title3, objects);
		string filename4 = text;
		string title4 = "Disabled Colliders";
		objects = (from x in UnityEngine.Object.FindObjectsOfType<Collider>()
		where !x.enabled
		select x).ToArray<Collider>();
		ServerPerformance.ComponentReport(filename4, title4, objects);
		string filename5 = text;
		string title5 = "Enabled Colliders";
		objects = (from x in UnityEngine.Object.FindObjectsOfType<Collider>()
		where x.enabled
		select x).ToArray<Collider>();
		ServerPerformance.ComponentReport(filename5, title5, objects);
		if (SingletonComponent<SpawnHandler>.Instance)
		{
			SingletonComponent<SpawnHandler>.Instance.DumpReport(text);
		}
	}

	// Token: 0x06003410 RID: 13328 RVA: 0x001424E8 File Offset: 0x001406E8
	public static string WorkoutPrefabName(GameObject obj)
	{
		if (obj == null)
		{
			return "null";
		}
		string str = obj.activeSelf ? "" : " (inactive)";
		BaseEntity baseEntity = obj.ToBaseEntity();
		if (baseEntity)
		{
			return baseEntity.PrefabName + str;
		}
		return obj.name + str;
	}

	// Token: 0x06003411 RID: 13329 RVA: 0x00142544 File Offset: 0x00140744
	public static void ComponentReport(string filename, string Title, UnityEngine.Object[] objects)
	{
		File.AppendAllText(filename, "\r\n\r\n" + Title + ":\r\n\r\n");
		foreach (IGrouping<string, UnityEngine.Object> source in from x in objects
		group x by ServerPerformance.WorkoutPrefabName((x as Component).gameObject) into x
		orderby x.Count<UnityEngine.Object>() descending
		select x)
		{
			File.AppendAllText(filename, string.Concat(new object[]
			{
				"\t",
				ServerPerformance.WorkoutPrefabName((source.ElementAt(0) as Component).gameObject),
				" - ",
				source.Count<UnityEngine.Object>(),
				"\r\n"
			}));
		}
		File.AppendAllText(filename, "\r\nTotal: " + objects.Count<UnityEngine.Object>() + "\r\n\r\n\r\n");
	}
}
