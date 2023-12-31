﻿using System;
using System.Diagnostics;
using System.Threading;
using UnityEngine;

// Token: 0x0200090E RID: 2318
public static class PerformanceMetrics
{
	// Token: 0x0400330C RID: 13068
	private static PerformanceSamplePoint current;

	// Token: 0x0400330D RID: 13069
	private static Action OnBeforeRender;

	// Token: 0x0400330E RID: 13070
	private static int _mainThreadId;

	// Token: 0x0600380B RID: 14347 RVA: 0x0014ED7C File Offset: 0x0014CF7C
	public static PerformanceSamplePoint GetCurrent(bool reset = false)
	{
		PerformanceSamplePoint result = PerformanceMetrics.current;
		if (reset)
		{
			PerformanceMetrics.current = default(PerformanceSamplePoint);
		}
		return result;
	}

	// Token: 0x0600380C RID: 14348 RVA: 0x0014ED94 File Offset: 0x0014CF94
	public static void Setup()
	{
		Application.onBeforeRender += delegate()
		{
			Action onBeforeRender = PerformanceMetrics.OnBeforeRender;
			if (onBeforeRender == null)
			{
				return;
			}
			onBeforeRender();
		};
		PerformanceMetrics._mainThreadId = Thread.CurrentThread.ManagedThreadId;
		PerformanceMetrics.AddStopwatch(PerformanceSample.PreCull, ref PerformanceMetrics.OnBeforeRender, ref CameraUpdateHook.RustCamera_PreRender);
		PerformanceMetrics.AddStopwatch(PerformanceSample.Update, ref PreUpdateHook.OnUpdate, ref PostUpdateHook.OnUpdate);
		PerformanceMetrics.AddStopwatch(PerformanceSample.LateUpdate, ref PreUpdateHook.OnLateUpdate, ref PostUpdateHook.OnLateUpdate);
		PerformanceMetrics.AddStopwatch(PerformanceSample.Render, ref CameraUpdateHook.PreRender, ref CameraUpdateHook.PostRender);
		PerformanceMetrics.AddStopwatch(PerformanceSample.FixedUpdate, ref PreUpdateHook.OnFixedUpdate, ref PostUpdateHook.OnFixedUpdate);
		PerformanceMetrics.AddCPUTimeStopwatch();
	}

	// Token: 0x0600380D RID: 14349 RVA: 0x0014EE2C File Offset: 0x0014D02C
	private static void AddCPUTimeStopwatch()
	{
		Stopwatch watch = new Stopwatch();
		int lastFrame = 0;
		TimeSpan lastTime;
		StartOfFrameHook.OnStartOfFrame = (Action)Delegate.Combine(StartOfFrameHook.OnStartOfFrame, new Action(delegate()
		{
			PerformanceMetrics.current.TotalCPU = PerformanceMetrics.current.TotalCPU + lastTime;
			PerformanceMetrics.current.CpuUpdateCount = PerformanceMetrics.current.CpuUpdateCount + 1;
			lastTime = default(TimeSpan);
			if (Time.frameCount != lastFrame)
			{
				lastFrame = Time.frameCount;
				watch.Restart();
			}
		}));
		CameraUpdateHook.PostRender = (Action)Delegate.Combine(CameraUpdateHook.PostRender, new Action(delegate()
		{
			lastTime = watch.Elapsed;
		}));
	}

	// Token: 0x0600380E RID: 14350 RVA: 0x0014EE94 File Offset: 0x0014D094
	private static void AddStopwatch(PerformanceSample sample, ref Action pre, ref Action post)
	{
		Stopwatch watch = new Stopwatch();
		bool active = false;
		pre = (Action)Delegate.Combine(pre, new Action(delegate()
		{
			if (active)
			{
				return;
			}
			active = true;
			watch.Restart();
		}));
		post = (Action)Delegate.Combine(post, new Action(delegate()
		{
			if (!active)
			{
				return;
			}
			active = false;
			watch.Stop();
			switch (sample)
			{
			case PerformanceSample.Update:
				PerformanceMetrics.current.UpdateCount = PerformanceMetrics.current.UpdateCount + 1;
				PerformanceMetrics.current.Update = PerformanceMetrics.current.Update + watch.Elapsed;
				return;
			case PerformanceSample.LateUpdate:
				PerformanceMetrics.current.LateUpdate = PerformanceMetrics.current.LateUpdate + watch.Elapsed;
				return;
			case PerformanceSample.PreCull:
				PerformanceMetrics.current.PreCull = PerformanceMetrics.current.PreCull + watch.Elapsed;
				return;
			case PerformanceSample.Render:
				PerformanceMetrics.current.Render = PerformanceMetrics.current.Render + watch.Elapsed;
				PerformanceMetrics.current.RenderCount = PerformanceMetrics.current.RenderCount + 1;
				return;
			case PerformanceSample.FixedUpdate:
				PerformanceMetrics.current.FixedUpdate = PerformanceMetrics.current.FixedUpdate + watch.Elapsed;
				PerformanceMetrics.current.FixedUpdateCount = PerformanceMetrics.current.FixedUpdateCount + 1;
				return;
			case PerformanceSample.NetworkMessage:
				break;
			case PerformanceSample.TotalCPU:
				PerformanceMetrics.current.TotalCPU = PerformanceMetrics.current.TotalCPU + watch.Elapsed;
				break;
			default:
				return;
			}
		}));
	}
}
