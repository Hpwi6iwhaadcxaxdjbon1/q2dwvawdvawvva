using System;
using System.Collections.Generic;

namespace UnityEngine.Rendering.PostProcessing
{
	// Token: 0x02000A8B RID: 2699
	[Serializable]
	public sealed class PostProcessDebugLayer
	{
		// Token: 0x0400396A RID: 14698
		public LightMeterMonitor lightMeter;

		// Token: 0x0400396B RID: 14699
		public HistogramMonitor histogram;

		// Token: 0x0400396C RID: 14700
		public WaveformMonitor waveform;

		// Token: 0x0400396D RID: 14701
		public VectorscopeMonitor vectorscope;

		// Token: 0x0400396E RID: 14702
		private Dictionary<MonitorType, Monitor> m_Monitors;

		// Token: 0x0400396F RID: 14703
		private int frameWidth;

		// Token: 0x04003970 RID: 14704
		private int frameHeight;

		// Token: 0x04003974 RID: 14708
		public PostProcessDebugLayer.OverlaySettings overlaySettings;

		// Token: 0x1700057B RID: 1403
		// (get) Token: 0x06004037 RID: 16439 RVA: 0x0017B21F File Offset: 0x0017941F
		// (set) Token: 0x06004038 RID: 16440 RVA: 0x0017B227 File Offset: 0x00179427
		public RenderTexture debugOverlayTarget { get; private set; }

		// Token: 0x1700057C RID: 1404
		// (get) Token: 0x06004039 RID: 16441 RVA: 0x0017B230 File Offset: 0x00179430
		// (set) Token: 0x0600403A RID: 16442 RVA: 0x0017B238 File Offset: 0x00179438
		public bool debugOverlayActive { get; private set; }

		// Token: 0x1700057D RID: 1405
		// (get) Token: 0x0600403B RID: 16443 RVA: 0x0017B241 File Offset: 0x00179441
		// (set) Token: 0x0600403C RID: 16444 RVA: 0x0017B249 File Offset: 0x00179449
		public DebugOverlay debugOverlay { get; private set; }

		// Token: 0x0600403D RID: 16445 RVA: 0x0017B254 File Offset: 0x00179454
		internal void OnEnable()
		{
			RuntimeUtilities.CreateIfNull<LightMeterMonitor>(ref this.lightMeter);
			RuntimeUtilities.CreateIfNull<HistogramMonitor>(ref this.histogram);
			RuntimeUtilities.CreateIfNull<WaveformMonitor>(ref this.waveform);
			RuntimeUtilities.CreateIfNull<VectorscopeMonitor>(ref this.vectorscope);
			RuntimeUtilities.CreateIfNull<PostProcessDebugLayer.OverlaySettings>(ref this.overlaySettings);
			this.m_Monitors = new Dictionary<MonitorType, Monitor>
			{
				{
					MonitorType.LightMeter,
					this.lightMeter
				},
				{
					MonitorType.Histogram,
					this.histogram
				},
				{
					MonitorType.Waveform,
					this.waveform
				},
				{
					MonitorType.Vectorscope,
					this.vectorscope
				}
			};
			foreach (KeyValuePair<MonitorType, Monitor> keyValuePair in this.m_Monitors)
			{
				keyValuePair.Value.OnEnable();
			}
		}

		// Token: 0x0600403E RID: 16446 RVA: 0x0017B324 File Offset: 0x00179524
		internal void OnDisable()
		{
			foreach (KeyValuePair<MonitorType, Monitor> keyValuePair in this.m_Monitors)
			{
				keyValuePair.Value.OnDisable();
			}
			this.DestroyDebugOverlayTarget();
		}

		// Token: 0x0600403F RID: 16447 RVA: 0x0017B384 File Offset: 0x00179584
		private void DestroyDebugOverlayTarget()
		{
			RuntimeUtilities.Destroy(this.debugOverlayTarget);
			this.debugOverlayTarget = null;
		}

		// Token: 0x06004040 RID: 16448 RVA: 0x0017B398 File Offset: 0x00179598
		public void RequestMonitorPass(MonitorType monitor)
		{
			this.m_Monitors[monitor].requested = true;
		}

		// Token: 0x06004041 RID: 16449 RVA: 0x0017B3AC File Offset: 0x001795AC
		public void RequestDebugOverlay(DebugOverlay mode)
		{
			this.debugOverlay = mode;
		}

		// Token: 0x06004042 RID: 16450 RVA: 0x0017B3B5 File Offset: 0x001795B5
		internal void SetFrameSize(int width, int height)
		{
			this.frameWidth = width;
			this.frameHeight = height;
			this.debugOverlayActive = false;
		}

		// Token: 0x06004043 RID: 16451 RVA: 0x0017B3CC File Offset: 0x001795CC
		public void PushDebugOverlay(CommandBuffer cmd, RenderTargetIdentifier source, PropertySheet sheet, int pass)
		{
			if (this.debugOverlayTarget == null || !this.debugOverlayTarget.IsCreated() || this.debugOverlayTarget.width != this.frameWidth || this.debugOverlayTarget.height != this.frameHeight)
			{
				RuntimeUtilities.Destroy(this.debugOverlayTarget);
				this.debugOverlayTarget = new RenderTexture(this.frameWidth, this.frameHeight, 0, RenderTextureFormat.ARGB32)
				{
					name = "Debug Overlay Target",
					anisoLevel = 1,
					filterMode = FilterMode.Bilinear,
					wrapMode = TextureWrapMode.Clamp,
					hideFlags = HideFlags.HideAndDontSave
				};
				this.debugOverlayTarget.Create();
			}
			cmd.BlitFullscreenTriangle(source, this.debugOverlayTarget, sheet, pass, false, null);
			this.debugOverlayActive = true;
		}

		// Token: 0x06004044 RID: 16452 RVA: 0x0017B498 File Offset: 0x00179698
		internal DepthTextureMode GetCameraFlags()
		{
			if (this.debugOverlay == DebugOverlay.Depth)
			{
				return DepthTextureMode.Depth;
			}
			if (this.debugOverlay == DebugOverlay.Normals)
			{
				return DepthTextureMode.DepthNormals;
			}
			if (this.debugOverlay == DebugOverlay.MotionVectors)
			{
				return DepthTextureMode.Depth | DepthTextureMode.MotionVectors;
			}
			return DepthTextureMode.None;
		}

		// Token: 0x06004045 RID: 16453 RVA: 0x0017B4BC File Offset: 0x001796BC
		internal void RenderMonitors(PostProcessRenderContext context)
		{
			bool flag = false;
			bool flag2 = false;
			foreach (KeyValuePair<MonitorType, Monitor> keyValuePair in this.m_Monitors)
			{
				bool flag3 = keyValuePair.Value.IsRequestedAndSupported(context);
				flag = (flag || flag3);
				flag2 |= (flag3 && keyValuePair.Value.NeedsHalfRes());
			}
			if (!flag)
			{
				return;
			}
			CommandBuffer command = context.command;
			command.BeginSample("Monitors");
			if (flag2)
			{
				command.GetTemporaryRT(ShaderIDs.HalfResFinalCopy, context.width / 2, context.height / 2, 0, FilterMode.Bilinear, context.sourceFormat);
				command.Blit(context.destination, ShaderIDs.HalfResFinalCopy);
			}
			foreach (KeyValuePair<MonitorType, Monitor> keyValuePair2 in this.m_Monitors)
			{
				Monitor value = keyValuePair2.Value;
				if (value.requested)
				{
					value.Render(context);
				}
			}
			if (flag2)
			{
				command.ReleaseTemporaryRT(ShaderIDs.HalfResFinalCopy);
			}
			command.EndSample("Monitors");
		}

		// Token: 0x06004046 RID: 16454 RVA: 0x0017B5F8 File Offset: 0x001797F8
		internal void RenderSpecialOverlays(PostProcessRenderContext context)
		{
			if (this.debugOverlay == DebugOverlay.Depth)
			{
				PropertySheet propertySheet = context.propertySheets.Get(context.resources.shaders.debugOverlays);
				propertySheet.properties.SetVector(ShaderIDs.Params, new Vector4(this.overlaySettings.linearDepth ? 1f : 0f, 0f, 0f, 0f));
				this.PushDebugOverlay(context.command, BuiltinRenderTextureType.None, propertySheet, 0);
				return;
			}
			if (this.debugOverlay == DebugOverlay.Normals)
			{
				PropertySheet propertySheet2 = context.propertySheets.Get(context.resources.shaders.debugOverlays);
				propertySheet2.ClearKeywords();
				if (context.camera.actualRenderingPath == RenderingPath.DeferredLighting)
				{
					propertySheet2.EnableKeyword("SOURCE_GBUFFER");
				}
				this.PushDebugOverlay(context.command, BuiltinRenderTextureType.None, propertySheet2, 1);
				return;
			}
			if (this.debugOverlay == DebugOverlay.MotionVectors)
			{
				PropertySheet propertySheet3 = context.propertySheets.Get(context.resources.shaders.debugOverlays);
				propertySheet3.properties.SetVector(ShaderIDs.Params, new Vector4(this.overlaySettings.motionColorIntensity, (float)this.overlaySettings.motionGridSize, 0f, 0f));
				this.PushDebugOverlay(context.command, context.source, propertySheet3, 2);
				return;
			}
			if (this.debugOverlay == DebugOverlay.NANTracker)
			{
				PropertySheet sheet = context.propertySheets.Get(context.resources.shaders.debugOverlays);
				this.PushDebugOverlay(context.command, context.source, sheet, 3);
				return;
			}
			if (this.debugOverlay == DebugOverlay.ColorBlindnessSimulation)
			{
				PropertySheet propertySheet4 = context.propertySheets.Get(context.resources.shaders.debugOverlays);
				propertySheet4.properties.SetVector(ShaderIDs.Params, new Vector4(this.overlaySettings.colorBlindnessStrength, 0f, 0f, 0f));
				this.PushDebugOverlay(context.command, context.source, propertySheet4, (int)(4 + this.overlaySettings.colorBlindnessType));
			}
		}

		// Token: 0x06004047 RID: 16455 RVA: 0x0017B7F8 File Offset: 0x001799F8
		internal void EndFrame()
		{
			foreach (KeyValuePair<MonitorType, Monitor> keyValuePair in this.m_Monitors)
			{
				keyValuePair.Value.requested = false;
			}
			if (!this.debugOverlayActive)
			{
				this.DestroyDebugOverlayTarget();
			}
			this.debugOverlay = DebugOverlay.None;
		}

		// Token: 0x02000F2D RID: 3885
		[Serializable]
		public class OverlaySettings
		{
			// Token: 0x04004EBA RID: 20154
			public bool linearDepth;

			// Token: 0x04004EBB RID: 20155
			[Range(0f, 16f)]
			public float motionColorIntensity = 4f;

			// Token: 0x04004EBC RID: 20156
			[Range(4f, 128f)]
			public int motionGridSize = 64;

			// Token: 0x04004EBD RID: 20157
			public ColorBlindnessType colorBlindnessType;

			// Token: 0x04004EBE RID: 20158
			[Range(0f, 1f)]
			public float colorBlindnessStrength = 1f;
		}
	}
}
