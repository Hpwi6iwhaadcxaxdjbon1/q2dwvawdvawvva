using System;

namespace UnityEngine.Rendering.PostProcessing
{
	// Token: 0x02000A88 RID: 2696
	[ExecuteAlways]
	[AddComponentMenu("Rendering/Post-process Debug", 1002)]
	public sealed class PostProcessDebug : MonoBehaviour
	{
		// Token: 0x04003951 RID: 14673
		public PostProcessLayer postProcessLayer;

		// Token: 0x04003952 RID: 14674
		private PostProcessLayer m_PreviousPostProcessLayer;

		// Token: 0x04003953 RID: 14675
		public bool lightMeter;

		// Token: 0x04003954 RID: 14676
		public bool histogram;

		// Token: 0x04003955 RID: 14677
		public bool waveform;

		// Token: 0x04003956 RID: 14678
		public bool vectorscope;

		// Token: 0x04003957 RID: 14679
		public DebugOverlay debugOverlay;

		// Token: 0x04003958 RID: 14680
		private Camera m_CurrentCamera;

		// Token: 0x04003959 RID: 14681
		private CommandBuffer m_CmdAfterEverything;

		// Token: 0x0600402E RID: 16430 RVA: 0x0017AF21 File Offset: 0x00179121
		private void OnEnable()
		{
			this.m_CmdAfterEverything = new CommandBuffer
			{
				name = "Post-processing Debug Overlay"
			};
		}

		// Token: 0x0600402F RID: 16431 RVA: 0x0017AF39 File Offset: 0x00179139
		private void OnDisable()
		{
			if (this.m_CurrentCamera != null)
			{
				this.m_CurrentCamera.RemoveCommandBuffer(CameraEvent.AfterImageEffects, this.m_CmdAfterEverything);
			}
			this.m_CurrentCamera = null;
			this.m_PreviousPostProcessLayer = null;
		}

		// Token: 0x06004030 RID: 16432 RVA: 0x0017AF6A File Offset: 0x0017916A
		private void Update()
		{
			this.UpdateStates();
		}

		// Token: 0x06004031 RID: 16433 RVA: 0x0017AF72 File Offset: 0x00179172
		private void Reset()
		{
			this.postProcessLayer = base.GetComponent<PostProcessLayer>();
		}

		// Token: 0x06004032 RID: 16434 RVA: 0x0017AF80 File Offset: 0x00179180
		private void UpdateStates()
		{
			if (this.m_PreviousPostProcessLayer != this.postProcessLayer)
			{
				if (this.m_CurrentCamera != null)
				{
					this.m_CurrentCamera.RemoveCommandBuffer(CameraEvent.AfterImageEffects, this.m_CmdAfterEverything);
					this.m_CurrentCamera = null;
				}
				this.m_PreviousPostProcessLayer = this.postProcessLayer;
				if (this.postProcessLayer != null)
				{
					this.m_CurrentCamera = this.postProcessLayer.GetComponent<Camera>();
					this.m_CurrentCamera.AddCommandBuffer(CameraEvent.AfterImageEffects, this.m_CmdAfterEverything);
				}
			}
			if (this.postProcessLayer == null || !this.postProcessLayer.enabled)
			{
				return;
			}
			if (this.lightMeter)
			{
				this.postProcessLayer.debugLayer.RequestMonitorPass(MonitorType.LightMeter);
			}
			if (this.histogram)
			{
				this.postProcessLayer.debugLayer.RequestMonitorPass(MonitorType.Histogram);
			}
			if (this.waveform)
			{
				this.postProcessLayer.debugLayer.RequestMonitorPass(MonitorType.Waveform);
			}
			if (this.vectorscope)
			{
				this.postProcessLayer.debugLayer.RequestMonitorPass(MonitorType.Vectorscope);
			}
			this.postProcessLayer.debugLayer.RequestDebugOverlay(this.debugOverlay);
		}

		// Token: 0x06004033 RID: 16435 RVA: 0x0017B09C File Offset: 0x0017929C
		private void OnPostRender()
		{
			this.m_CmdAfterEverything.Clear();
			if (this.postProcessLayer == null || !this.postProcessLayer.enabled || !this.postProcessLayer.debugLayer.debugOverlayActive)
			{
				return;
			}
			this.m_CmdAfterEverything.Blit(this.postProcessLayer.debugLayer.debugOverlayTarget, BuiltinRenderTextureType.CameraTarget);
		}

		// Token: 0x06004034 RID: 16436 RVA: 0x0017B104 File Offset: 0x00179304
		private void OnGUI()
		{
			if (this.postProcessLayer == null || !this.postProcessLayer.enabled)
			{
				return;
			}
			RenderTexture.active = null;
			Rect rect = new Rect(5f, 5f, 0f, 0f);
			PostProcessDebugLayer debugLayer = this.postProcessLayer.debugLayer;
			this.DrawMonitor(ref rect, debugLayer.lightMeter, this.lightMeter);
			this.DrawMonitor(ref rect, debugLayer.histogram, this.histogram);
			this.DrawMonitor(ref rect, debugLayer.waveform, this.waveform);
			this.DrawMonitor(ref rect, debugLayer.vectorscope, this.vectorscope);
		}

		// Token: 0x06004035 RID: 16437 RVA: 0x0017B1AC File Offset: 0x001793AC
		private void DrawMonitor(ref Rect rect, Monitor monitor, bool enabled)
		{
			if (!enabled || monitor.output == null)
			{
				return;
			}
			rect.width = (float)monitor.output.width;
			rect.height = (float)monitor.output.height;
			GUI.DrawTexture(rect, monitor.output);
			rect.x += (float)monitor.output.width + 5f;
		}
	}
}
