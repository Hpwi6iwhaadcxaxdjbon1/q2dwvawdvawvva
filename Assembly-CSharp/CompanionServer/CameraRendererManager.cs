using System;
using System.Collections.Generic;
using System.Diagnostics;
using CompanionServer.Cameras;
using Facepunch;
using Facepunch.Extend;

namespace CompanionServer
{
	// Token: 0x020009DF RID: 2527
	public class CameraRendererManager : SingletonComponent<CameraRendererManager>
	{
		// Token: 0x04003688 RID: 13960
		private readonly Stack<CameraRenderTask> _taskPool = new Stack<CameraRenderTask>();

		// Token: 0x04003689 RID: 13961
		private int _tasksTaken;

		// Token: 0x0400368A RID: 13962
		private int _tasksReturned;

		// Token: 0x0400368B RID: 13963
		private int _tasksCreated;

		// Token: 0x0400368C RID: 13964
		private readonly Stopwatch _stopwatch = new Stopwatch();

		// Token: 0x0400368D RID: 13965
		private readonly List<CameraRenderer> _renderers = new List<CameraRenderer>();

		// Token: 0x0400368E RID: 13966
		private int _renderIndex;

		// Token: 0x0400368F RID: 13967
		private int _completeIndex;

		// Token: 0x06003C5D RID: 15453 RVA: 0x00163444 File Offset: 0x00161644
		protected override void OnDestroy()
		{
			base.OnDestroy();
			foreach (CameraRenderer cameraRenderer in this._renderers)
			{
				cameraRenderer.Reset();
			}
			this._renderers.Clear();
			CameraRenderTask.FreeCachedSamplePositions();
			while (this._taskPool.Count > 0)
			{
				this._taskPool.Pop().Dispose();
			}
		}

		// Token: 0x06003C5E RID: 15454 RVA: 0x001634CC File Offset: 0x001616CC
		public void StartRendering(IRemoteControllable rc)
		{
			if (rc == null || rc.IsUnityNull<IRemoteControllable>())
			{
				throw new ArgumentNullException("rc");
			}
			if (this._renderers.FindWith((CameraRenderer r) => r.rc, rc, null) == null)
			{
				CameraRenderer cameraRenderer = Pool.Get<CameraRenderer>();
				this._renderers.Add(cameraRenderer);
				cameraRenderer.Init(rc);
			}
		}

		// Token: 0x06003C5F RID: 15455 RVA: 0x00163536 File Offset: 0x00161736
		public void Tick()
		{
			if (!CameraRenderer.enabled)
			{
				return;
			}
			this.DispatchRenderers();
			this.CompleteRenderers();
			this.CleanupRenderers();
		}

		// Token: 0x06003C60 RID: 15456 RVA: 0x00163552 File Offset: 0x00161752
		public CameraRenderTask BorrowTask()
		{
			if (this._taskPool.Count > 0)
			{
				this._tasksTaken++;
				return this._taskPool.Pop();
			}
			this._tasksCreated++;
			return new CameraRenderTask();
		}

		// Token: 0x06003C61 RID: 15457 RVA: 0x0016358F File Offset: 0x0016178F
		public void ReturnTask(ref CameraRenderTask task)
		{
			if (task == null)
			{
				return;
			}
			task.Reset();
			this._tasksReturned++;
			this._taskPool.Push(task);
			task = null;
		}

		// Token: 0x06003C62 RID: 15458 RVA: 0x001635BC File Offset: 0x001617BC
		[ServerVar]
		public static void pool_stats(ConsoleSystem.Arg arg)
		{
			CameraRendererManager instance = SingletonComponent<CameraRendererManager>.Instance;
			if (instance == null)
			{
				arg.ReplyWith("Camera renderer manager is null!");
				return;
			}
			arg.ReplyWith(string.Format("Active renderers: {0}\nTasks in pool: {1}\nTasks taken: {2}\nTasks returned: {3}\nTasks created: {4}", new object[]
			{
				instance._renderers.Count,
				instance._taskPool.Count,
				instance._tasksTaken,
				instance._tasksReturned,
				instance._tasksCreated
			}));
		}

		// Token: 0x06003C63 RID: 15459 RVA: 0x0016364C File Offset: 0x0016184C
		private void DispatchRenderers()
		{
			List<CameraRenderer> list = Pool.GetList<CameraRenderer>();
			int count = this._renderers.Count;
			for (int i = 0; i < count; i++)
			{
				if (this._renderIndex >= count)
				{
					this._renderIndex = 0;
				}
				List<CameraRenderer> renderers = this._renderers;
				int renderIndex = this._renderIndex;
				this._renderIndex = renderIndex + 1;
				CameraRenderer cameraRenderer = renderers[renderIndex];
				if (cameraRenderer.CanRender())
				{
					list.Add(cameraRenderer);
					if (list.Count >= CameraRenderer.maxRendersPerFrame)
					{
						break;
					}
				}
			}
			if (list.Count > 0)
			{
				int maxSampleCount = CameraRenderer.maxRaysPerFrame / list.Count;
				foreach (CameraRenderer cameraRenderer2 in list)
				{
					cameraRenderer2.Render(maxSampleCount);
				}
			}
			Pool.FreeList<CameraRenderer>(ref list);
		}

		// Token: 0x06003C64 RID: 15460 RVA: 0x00163724 File Offset: 0x00161924
		private void CompleteRenderers()
		{
			this._stopwatch.Restart();
			int count = this._renderers.Count;
			for (int i = 0; i < count; i++)
			{
				if (this._completeIndex >= count)
				{
					this._completeIndex = 0;
				}
				List<CameraRenderer> renderers = this._renderers;
				int completeIndex = this._completeIndex;
				this._completeIndex = completeIndex + 1;
				CameraRenderer cameraRenderer = renderers[completeIndex];
				if (cameraRenderer.state == CameraRendererState.Rendering)
				{
					cameraRenderer.CompleteRender();
					if (this._stopwatch.Elapsed.TotalMilliseconds >= (double)CameraRenderer.completionFrameBudgetMs)
					{
						break;
					}
				}
			}
		}

		// Token: 0x06003C65 RID: 15461 RVA: 0x001637AC File Offset: 0x001619AC
		private void CleanupRenderers()
		{
			List<CameraRenderer> list = Pool.GetList<CameraRenderer>();
			foreach (CameraRenderer cameraRenderer in this._renderers)
			{
				if (cameraRenderer.state == CameraRendererState.Invalid)
				{
					list.Add(cameraRenderer);
				}
			}
			this._renderers.RemoveAll((CameraRenderer r) => r.state == CameraRendererState.Invalid);
			foreach (CameraRenderer cameraRenderer2 in list)
			{
				Pool.Free<CameraRenderer>(ref cameraRenderer2);
			}
			Pool.FreeList<CameraRenderer>(ref list);
		}
	}
}
