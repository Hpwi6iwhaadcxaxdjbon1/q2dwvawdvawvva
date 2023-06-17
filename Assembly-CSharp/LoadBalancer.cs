using System;
using System.Collections.Generic;
using System.Diagnostics;
using Rust;
using UnityEngine;

// Token: 0x02000900 RID: 2304
public class LoadBalancer : SingletonComponent<LoadBalancer>
{
	// Token: 0x040032DA RID: 13018
	public static bool Paused;

	// Token: 0x040032DB RID: 13019
	private const float MinMilliseconds = 1f;

	// Token: 0x040032DC RID: 13020
	private const float MaxMilliseconds = 100f;

	// Token: 0x040032DD RID: 13021
	private const int MinBacklog = 1000;

	// Token: 0x040032DE RID: 13022
	private const int MaxBacklog = 100000;

	// Token: 0x040032DF RID: 13023
	private Queue<DeferredAction>[] queues = new Queue<DeferredAction>[]
	{
		new Queue<DeferredAction>(),
		new Queue<DeferredAction>(),
		new Queue<DeferredAction>(),
		new Queue<DeferredAction>(),
		new Queue<DeferredAction>()
	};

	// Token: 0x040032E0 RID: 13024
	private Stopwatch watch = Stopwatch.StartNew();

	// Token: 0x060037E2 RID: 14306 RVA: 0x0014E794 File Offset: 0x0014C994
	protected void LateUpdate()
	{
		if (Rust.Application.isReceiving)
		{
			return;
		}
		if (Rust.Application.isLoading)
		{
			return;
		}
		if (LoadBalancer.Paused)
		{
			return;
		}
		int num = LoadBalancer.Count();
		float t = Mathf.InverseLerp(1000f, 100000f, (float)num);
		float num2 = Mathf.SmoothStep(1f, 100f, t);
		this.watch.Reset();
		this.watch.Start();
		for (int i = 0; i < this.queues.Length; i++)
		{
			Queue<DeferredAction> queue = this.queues[i];
			while (queue.Count > 0)
			{
				queue.Dequeue().Action();
				if (this.watch.Elapsed.TotalMilliseconds > (double)num2)
				{
					return;
				}
			}
		}
	}

	// Token: 0x060037E3 RID: 14307 RVA: 0x0014E848 File Offset: 0x0014CA48
	public static int Count()
	{
		if (!SingletonComponent<LoadBalancer>.Instance)
		{
			return 0;
		}
		Queue<DeferredAction>[] array = SingletonComponent<LoadBalancer>.Instance.queues;
		int num = 0;
		for (int i = 0; i < array.Length; i++)
		{
			num += array[i].Count;
		}
		return num;
	}

	// Token: 0x060037E4 RID: 14308 RVA: 0x0014E88C File Offset: 0x0014CA8C
	public static void ProcessAll()
	{
		if (!SingletonComponent<LoadBalancer>.Instance)
		{
			LoadBalancer.CreateInstance();
		}
		foreach (Queue<DeferredAction> queue in SingletonComponent<LoadBalancer>.Instance.queues)
		{
			while (queue.Count > 0)
			{
				queue.Dequeue().Action();
			}
		}
	}

	// Token: 0x060037E5 RID: 14309 RVA: 0x0014E8DD File Offset: 0x0014CADD
	public static void Enqueue(DeferredAction action)
	{
		if (!SingletonComponent<LoadBalancer>.Instance)
		{
			LoadBalancer.CreateInstance();
		}
		SingletonComponent<LoadBalancer>.Instance.queues[action.Index].Enqueue(action);
	}

	// Token: 0x060037E6 RID: 14310 RVA: 0x0014E907 File Offset: 0x0014CB07
	private static void CreateInstance()
	{
		GameObject gameObject = new GameObject();
		gameObject.name = "LoadBalancer";
		gameObject.AddComponent<LoadBalancer>();
		UnityEngine.Object.DontDestroyOnLoad(gameObject);
	}
}
