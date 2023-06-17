using System;
using UnityEngine.Scripting;

namespace UnityEngine.Rendering.PostProcessing
{
	// Token: 0x02000A70 RID: 2672
	[Preserve]
	[Serializable]
	public sealed class TemporalAntialiasing
	{
		// Token: 0x0400390E RID: 14606
		[Tooltip("The diameter (in texels) inside which jitter samples are spread. Smaller values result in crisper but more aliased output, while larger values result in more stable, but blurrier, output.")]
		[Range(0.1f, 1f)]
		public float jitterSpread = 0.75f;

		// Token: 0x0400390F RID: 14607
		[Tooltip("Controls the amount of sharpening applied to the color buffer. High values may introduce dark-border artifacts.")]
		[Range(0f, 3f)]
		public float sharpness = 0.25f;

		// Token: 0x04003910 RID: 14608
		[Tooltip("The blend coefficient for a stationary fragment. Controls the percentage of history sample blended into the final color.")]
		[Range(0f, 0.99f)]
		public float stationaryBlending = 0.95f;

		// Token: 0x04003911 RID: 14609
		[Tooltip("The blend coefficient for a fragment with significant motion. Controls the percentage of history sample blended into the final color.")]
		[Range(0f, 0.99f)]
		public float motionBlending = 0.85f;

		// Token: 0x04003912 RID: 14610
		public Func<Camera, Vector2, Matrix4x4> jitteredMatrixFunc;

		// Token: 0x04003915 RID: 14613
		private readonly RenderTargetIdentifier[] m_Mrt = new RenderTargetIdentifier[2];

		// Token: 0x04003916 RID: 14614
		private bool m_ResetHistory = true;

		// Token: 0x04003919 RID: 14617
		private const int k_NumEyes = 2;

		// Token: 0x0400391A RID: 14618
		private const int k_NumHistoryTextures = 2;

		// Token: 0x0400391B RID: 14619
		private readonly RenderTexture[][] m_HistoryTextures = new RenderTexture[2][];

		// Token: 0x0400391C RID: 14620
		private readonly int[] m_HistoryPingPong = new int[2];

		// Token: 0x17000573 RID: 1395
		// (get) Token: 0x06003FC5 RID: 16325 RVA: 0x00179564 File Offset: 0x00177764
		// (set) Token: 0x06003FC6 RID: 16326 RVA: 0x0017956C File Offset: 0x0017776C
		public Vector2 jitter { get; private set; }

		// Token: 0x17000574 RID: 1396
		// (get) Token: 0x06003FC7 RID: 16327 RVA: 0x00179575 File Offset: 0x00177775
		// (set) Token: 0x06003FC8 RID: 16328 RVA: 0x0017957D File Offset: 0x0017777D
		public Vector2 jitterRaw { get; private set; }

		// Token: 0x17000575 RID: 1397
		// (get) Token: 0x06003FC9 RID: 16329 RVA: 0x00179586 File Offset: 0x00177786
		// (set) Token: 0x06003FCA RID: 16330 RVA: 0x0017958E File Offset: 0x0017778E
		public int sampleIndex { get; private set; }

		// Token: 0x17000576 RID: 1398
		// (get) Token: 0x06003FCB RID: 16331 RVA: 0x00179597 File Offset: 0x00177797
		// (set) Token: 0x06003FCC RID: 16332 RVA: 0x0017959F File Offset: 0x0017779F
		public int sampleCount { get; set; }

		// Token: 0x06003FCD RID: 16333 RVA: 0x001795A8 File Offset: 0x001777A8
		public bool IsSupported()
		{
			return SystemInfo.supportedRenderTargetCount >= 2 && SystemInfo.supportsMotionVectors && SystemInfo.graphicsDeviceType != GraphicsDeviceType.OpenGLES2;
		}

		// Token: 0x06003FCE RID: 16334 RVA: 0x0002179A File Offset: 0x0001F99A
		internal DepthTextureMode GetCameraFlags()
		{
			return DepthTextureMode.Depth | DepthTextureMode.MotionVectors;
		}

		// Token: 0x06003FCF RID: 16335 RVA: 0x001795C6 File Offset: 0x001777C6
		internal void ResetHistory()
		{
			this.m_ResetHistory = true;
		}

		// Token: 0x06003FD0 RID: 16336 RVA: 0x001795D0 File Offset: 0x001777D0
		private Vector2 GenerateRandomOffset()
		{
			Vector2 result = new Vector2(HaltonSeq.Get((this.sampleIndex & 1023) + 1, 2) - 0.5f, HaltonSeq.Get((this.sampleIndex & 1023) + 1, 3) - 0.5f);
			int num = this.sampleIndex + 1;
			this.sampleIndex = num;
			if (num >= this.sampleCount)
			{
				this.sampleIndex = 0;
			}
			return result;
		}

		// Token: 0x06003FD1 RID: 16337 RVA: 0x00179638 File Offset: 0x00177838
		public Matrix4x4 GetJitteredProjectionMatrix(Camera camera)
		{
			this.jitter = this.GenerateRandomOffset();
			this.jitter *= this.jitterSpread;
			Matrix4x4 result;
			if (this.jitteredMatrixFunc != null)
			{
				result = this.jitteredMatrixFunc(camera, this.jitter);
			}
			else
			{
				result = (camera.orthographic ? RuntimeUtilities.GetJitteredOrthographicProjectionMatrix(camera, this.jitter) : RuntimeUtilities.GetJitteredPerspectiveProjectionMatrix(camera, this.jitter));
			}
			this.jitterRaw = this.jitter;
			this.jitter = new Vector2(this.jitter.x / (float)camera.pixelWidth, this.jitter.y / (float)camera.pixelHeight);
			return result;
		}

		// Token: 0x06003FD2 RID: 16338 RVA: 0x001796E8 File Offset: 0x001778E8
		public void ConfigureJitteredProjectionMatrix(PostProcessRenderContext context)
		{
			Camera camera = context.camera;
			camera.nonJitteredProjectionMatrix = camera.projectionMatrix;
			camera.projectionMatrix = this.GetJitteredProjectionMatrix(camera);
			camera.useJitteredProjectionMatrixForTransparentRendering = true;
		}

		// Token: 0x06003FD3 RID: 16339 RVA: 0x0017971C File Offset: 0x0017791C
		public void ConfigureStereoJitteredProjectionMatrices(PostProcessRenderContext context)
		{
			Camera camera = context.camera;
			this.jitter = this.GenerateRandomOffset();
			this.jitter *= this.jitterSpread;
			for (Camera.StereoscopicEye stereoscopicEye = Camera.StereoscopicEye.Left; stereoscopicEye <= Camera.StereoscopicEye.Right; stereoscopicEye++)
			{
				context.camera.CopyStereoDeviceProjectionMatrixToNonJittered(stereoscopicEye);
				Matrix4x4 stereoNonJitteredProjectionMatrix = context.camera.GetStereoNonJitteredProjectionMatrix(stereoscopicEye);
				Matrix4x4 matrix = RuntimeUtilities.GenerateJitteredProjectionMatrixFromOriginal(context, stereoNonJitteredProjectionMatrix, this.jitter);
				context.camera.SetStereoProjectionMatrix(stereoscopicEye, matrix);
			}
			this.jitter = new Vector2(this.jitter.x / (float)context.screenWidth, this.jitter.y / (float)context.screenHeight);
			camera.useJitteredProjectionMatrixForTransparentRendering = true;
		}

		// Token: 0x06003FD4 RID: 16340 RVA: 0x001797CC File Offset: 0x001779CC
		private void GenerateHistoryName(RenderTexture rt, int id, PostProcessRenderContext context)
		{
			rt.name = "Temporal Anti-aliasing History id #" + id;
			if (context.stereoActive)
			{
				rt.name = rt.name + " for eye " + context.xrActiveEye;
			}
		}

		// Token: 0x06003FD5 RID: 16341 RVA: 0x00179818 File Offset: 0x00177A18
		private RenderTexture CheckHistory(int id, PostProcessRenderContext context)
		{
			int xrActiveEye = context.xrActiveEye;
			if (this.m_HistoryTextures[xrActiveEye] == null)
			{
				this.m_HistoryTextures[xrActiveEye] = new RenderTexture[2];
			}
			RenderTexture renderTexture = this.m_HistoryTextures[xrActiveEye][id];
			if (this.m_ResetHistory || renderTexture == null || !renderTexture.IsCreated())
			{
				RenderTexture.ReleaseTemporary(renderTexture);
				renderTexture = context.GetScreenSpaceTemporaryRT(0, context.sourceFormat, RenderTextureReadWrite.Default, 0, 0);
				this.GenerateHistoryName(renderTexture, id, context);
				renderTexture.filterMode = FilterMode.Bilinear;
				this.m_HistoryTextures[xrActiveEye][id] = renderTexture;
				context.command.BlitFullscreenTriangle(context.source, renderTexture, false, null);
			}
			else if (renderTexture.width != context.width || renderTexture.height != context.height)
			{
				RenderTexture screenSpaceTemporaryRT = context.GetScreenSpaceTemporaryRT(0, context.sourceFormat, RenderTextureReadWrite.Default, 0, 0);
				this.GenerateHistoryName(screenSpaceTemporaryRT, id, context);
				screenSpaceTemporaryRT.filterMode = FilterMode.Bilinear;
				this.m_HistoryTextures[xrActiveEye][id] = screenSpaceTemporaryRT;
				context.command.BlitFullscreenTriangle(renderTexture, screenSpaceTemporaryRT, false, null);
				RenderTexture.ReleaseTemporary(renderTexture);
			}
			return this.m_HistoryTextures[xrActiveEye][id];
		}

		// Token: 0x06003FD6 RID: 16342 RVA: 0x00179938 File Offset: 0x00177B38
		internal void Render(PostProcessRenderContext context)
		{
			PropertySheet propertySheet = context.propertySheets.Get(context.resources.shaders.temporalAntialiasing);
			CommandBuffer command = context.command;
			command.BeginSample("TemporalAntialiasing");
			int num = this.m_HistoryPingPong[context.xrActiveEye];
			RenderTexture value = this.CheckHistory(++num % 2, context);
			RenderTexture tex = this.CheckHistory(++num % 2, context);
			this.m_HistoryPingPong[context.xrActiveEye] = (num + 1) % 2;
			propertySheet.properties.SetVector(ShaderIDs.Jitter, this.jitter);
			propertySheet.properties.SetFloat(ShaderIDs.Sharpness, this.sharpness);
			propertySheet.properties.SetVector(ShaderIDs.FinalBlendParameters, new Vector4(this.stationaryBlending, this.motionBlending, 6000f, 0f));
			propertySheet.properties.SetTexture(ShaderIDs.HistoryTex, value);
			int pass = context.camera.orthographic ? 1 : 0;
			this.m_Mrt[0] = context.destination;
			this.m_Mrt[1] = tex;
			command.BlitFullscreenTriangle(context.source, this.m_Mrt, context.source, propertySheet, pass, false, null);
			command.EndSample("TemporalAntialiasing");
			this.m_ResetHistory = false;
		}

		// Token: 0x06003FD7 RID: 16343 RVA: 0x00179A90 File Offset: 0x00177C90
		internal void Release()
		{
			if (this.m_HistoryTextures != null)
			{
				for (int i = 0; i < this.m_HistoryTextures.Length; i++)
				{
					if (this.m_HistoryTextures[i] != null)
					{
						for (int j = 0; j < this.m_HistoryTextures[i].Length; j++)
						{
							RenderTexture.ReleaseTemporary(this.m_HistoryTextures[i][j]);
							this.m_HistoryTextures[i][j] = null;
						}
						this.m_HistoryTextures[i] = null;
					}
				}
			}
			this.sampleIndex = 0;
			this.m_HistoryPingPong[0] = 0;
			this.m_HistoryPingPong[1] = 0;
			this.ResetHistory();
		}

		// Token: 0x02000F2B RID: 3883
		private enum Pass
		{
			// Token: 0x04004EB3 RID: 20147
			SolverDilate,
			// Token: 0x04004EB4 RID: 20148
			SolverNoDilate
		}
	}
}
