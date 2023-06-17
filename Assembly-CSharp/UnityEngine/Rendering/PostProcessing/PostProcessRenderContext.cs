using System;
using System.Collections.Generic;

namespace UnityEngine.Rendering.PostProcessing
{
	// Token: 0x02000A3E RID: 2622
	public class PostProcessRenderContext
	{
		// Token: 0x04003830 RID: 14384
		public bool dlssEnabled;

		// Token: 0x04003831 RID: 14385
		private Camera m_Camera;

		// Token: 0x04003846 RID: 14406
		internal PropertySheet uberSheet;

		// Token: 0x04003847 RID: 14407
		internal Texture autoExposureTexture;

		// Token: 0x04003848 RID: 14408
		internal LogHistogram logHistogram;

		// Token: 0x04003849 RID: 14409
		internal Texture logLut;

		// Token: 0x0400384A RID: 14410
		internal AutoExposure autoExposure;

		// Token: 0x0400384B RID: 14411
		internal int bloomBufferNameID;

		// Token: 0x0400384C RID: 14412
		internal bool physicalCamera;

		// Token: 0x0400384D RID: 14413
		private RenderTextureDescriptor m_sourceDescriptor;

		// Token: 0x06003F0C RID: 16140 RVA: 0x0017313C File Offset: 0x0017133C
		public void Resize(int width, int height, bool dlssEnabled)
		{
			this.screenWidth = width;
			this.width = width;
			this.screenHeight = height;
			this.height = height;
			this.dlssEnabled = dlssEnabled;
			this.m_sourceDescriptor.width = width;
			this.m_sourceDescriptor.height = height;
		}

		// Token: 0x1700055E RID: 1374
		// (get) Token: 0x06003F0D RID: 16141 RVA: 0x00173188 File Offset: 0x00171388
		// (set) Token: 0x06003F0E RID: 16142 RVA: 0x00173190 File Offset: 0x00171390
		public Camera camera
		{
			get
			{
				return this.m_Camera;
			}
			set
			{
				this.m_Camera = value;
				if (!this.m_Camera.stereoEnabled)
				{
					this.width = this.m_Camera.pixelWidth;
					this.height = this.m_Camera.pixelHeight;
					this.m_sourceDescriptor.width = this.width;
					this.m_sourceDescriptor.height = this.height;
					this.screenWidth = this.width;
					this.screenHeight = this.height;
					this.stereoActive = false;
					this.numberOfEyes = 1;
				}
			}
		}

		// Token: 0x1700055F RID: 1375
		// (get) Token: 0x06003F0F RID: 16143 RVA: 0x0017321B File Offset: 0x0017141B
		// (set) Token: 0x06003F10 RID: 16144 RVA: 0x00173223 File Offset: 0x00171423
		public CommandBuffer command { get; set; }

		// Token: 0x17000560 RID: 1376
		// (get) Token: 0x06003F11 RID: 16145 RVA: 0x0017322C File Offset: 0x0017142C
		// (set) Token: 0x06003F12 RID: 16146 RVA: 0x00173234 File Offset: 0x00171434
		public RenderTargetIdentifier source { get; set; }

		// Token: 0x17000561 RID: 1377
		// (get) Token: 0x06003F13 RID: 16147 RVA: 0x0017323D File Offset: 0x0017143D
		// (set) Token: 0x06003F14 RID: 16148 RVA: 0x00173245 File Offset: 0x00171445
		public RenderTargetIdentifier destination { get; set; }

		// Token: 0x17000562 RID: 1378
		// (get) Token: 0x06003F15 RID: 16149 RVA: 0x0017324E File Offset: 0x0017144E
		// (set) Token: 0x06003F16 RID: 16150 RVA: 0x00173256 File Offset: 0x00171456
		public RenderTextureFormat sourceFormat { get; set; }

		// Token: 0x17000563 RID: 1379
		// (get) Token: 0x06003F17 RID: 16151 RVA: 0x0017325F File Offset: 0x0017145F
		// (set) Token: 0x06003F18 RID: 16152 RVA: 0x00173267 File Offset: 0x00171467
		public bool flip { get; set; }

		// Token: 0x17000564 RID: 1380
		// (get) Token: 0x06003F19 RID: 16153 RVA: 0x00173270 File Offset: 0x00171470
		// (set) Token: 0x06003F1A RID: 16154 RVA: 0x00173278 File Offset: 0x00171478
		public PostProcessResources resources { get; internal set; }

		// Token: 0x17000565 RID: 1381
		// (get) Token: 0x06003F1B RID: 16155 RVA: 0x00173281 File Offset: 0x00171481
		// (set) Token: 0x06003F1C RID: 16156 RVA: 0x00173289 File Offset: 0x00171489
		public PropertySheetFactory propertySheets { get; internal set; }

		// Token: 0x17000566 RID: 1382
		// (get) Token: 0x06003F1D RID: 16157 RVA: 0x00173292 File Offset: 0x00171492
		// (set) Token: 0x06003F1E RID: 16158 RVA: 0x0017329A File Offset: 0x0017149A
		public Dictionary<string, object> userData { get; private set; }

		// Token: 0x17000567 RID: 1383
		// (get) Token: 0x06003F1F RID: 16159 RVA: 0x001732A3 File Offset: 0x001714A3
		// (set) Token: 0x06003F20 RID: 16160 RVA: 0x001732AB File Offset: 0x001714AB
		public PostProcessDebugLayer debugLayer { get; internal set; }

		// Token: 0x17000568 RID: 1384
		// (get) Token: 0x06003F21 RID: 16161 RVA: 0x001732B4 File Offset: 0x001714B4
		// (set) Token: 0x06003F22 RID: 16162 RVA: 0x001732BC File Offset: 0x001714BC
		public int width { get; set; }

		// Token: 0x17000569 RID: 1385
		// (get) Token: 0x06003F23 RID: 16163 RVA: 0x001732C5 File Offset: 0x001714C5
		// (set) Token: 0x06003F24 RID: 16164 RVA: 0x001732CD File Offset: 0x001714CD
		public int height { get; set; }

		// Token: 0x1700056A RID: 1386
		// (get) Token: 0x06003F25 RID: 16165 RVA: 0x001732D6 File Offset: 0x001714D6
		// (set) Token: 0x06003F26 RID: 16166 RVA: 0x001732DE File Offset: 0x001714DE
		public bool stereoActive { get; private set; }

		// Token: 0x1700056B RID: 1387
		// (get) Token: 0x06003F27 RID: 16167 RVA: 0x001732E7 File Offset: 0x001714E7
		// (set) Token: 0x06003F28 RID: 16168 RVA: 0x001732EF File Offset: 0x001714EF
		public int xrActiveEye { get; private set; }

		// Token: 0x1700056C RID: 1388
		// (get) Token: 0x06003F29 RID: 16169 RVA: 0x001732F8 File Offset: 0x001714F8
		// (set) Token: 0x06003F2A RID: 16170 RVA: 0x00173300 File Offset: 0x00171500
		public int numberOfEyes { get; private set; }

		// Token: 0x1700056D RID: 1389
		// (get) Token: 0x06003F2B RID: 16171 RVA: 0x00173309 File Offset: 0x00171509
		// (set) Token: 0x06003F2C RID: 16172 RVA: 0x00173311 File Offset: 0x00171511
		public PostProcessRenderContext.StereoRenderingMode stereoRenderingMode { get; private set; }

		// Token: 0x1700056E RID: 1390
		// (get) Token: 0x06003F2D RID: 16173 RVA: 0x0017331A File Offset: 0x0017151A
		// (set) Token: 0x06003F2E RID: 16174 RVA: 0x00173322 File Offset: 0x00171522
		public int screenWidth { get; set; }

		// Token: 0x1700056F RID: 1391
		// (get) Token: 0x06003F2F RID: 16175 RVA: 0x0017332B File Offset: 0x0017152B
		// (set) Token: 0x06003F30 RID: 16176 RVA: 0x00173333 File Offset: 0x00171533
		public int screenHeight { get; set; }

		// Token: 0x17000570 RID: 1392
		// (get) Token: 0x06003F31 RID: 16177 RVA: 0x0017333C File Offset: 0x0017153C
		// (set) Token: 0x06003F32 RID: 16178 RVA: 0x00173344 File Offset: 0x00171544
		public bool isSceneView { get; internal set; }

		// Token: 0x17000571 RID: 1393
		// (get) Token: 0x06003F33 RID: 16179 RVA: 0x0017334D File Offset: 0x0017154D
		// (set) Token: 0x06003F34 RID: 16180 RVA: 0x00173355 File Offset: 0x00171555
		public PostProcessLayer.Antialiasing antialiasing { get; internal set; }

		// Token: 0x17000572 RID: 1394
		// (get) Token: 0x06003F35 RID: 16181 RVA: 0x0017335E File Offset: 0x0017155E
		// (set) Token: 0x06003F36 RID: 16182 RVA: 0x00173366 File Offset: 0x00171566
		public TemporalAntialiasing temporalAntialiasing { get; internal set; }

		// Token: 0x06003F37 RID: 16183 RVA: 0x00173370 File Offset: 0x00171570
		public void Reset()
		{
			this.m_Camera = null;
			this.width = 0;
			this.height = 0;
			this.dlssEnabled = false;
			this.m_sourceDescriptor = new RenderTextureDescriptor(0, 0);
			this.physicalCamera = false;
			this.stereoActive = false;
			this.xrActiveEye = 0;
			this.screenWidth = 0;
			this.screenHeight = 0;
			this.command = null;
			this.source = 0;
			this.destination = 0;
			this.sourceFormat = RenderTextureFormat.ARGB32;
			this.flip = false;
			this.resources = null;
			this.propertySheets = null;
			this.debugLayer = null;
			this.isSceneView = false;
			this.antialiasing = PostProcessLayer.Antialiasing.None;
			this.temporalAntialiasing = null;
			this.uberSheet = null;
			this.autoExposureTexture = null;
			this.logLut = null;
			this.autoExposure = null;
			this.bloomBufferNameID = -1;
			if (this.userData == null)
			{
				this.userData = new Dictionary<string, object>();
			}
			this.userData.Clear();
		}

		// Token: 0x06003F38 RID: 16184 RVA: 0x00173461 File Offset: 0x00171661
		public bool IsTemporalAntialiasingActive()
		{
			return this.antialiasing == PostProcessLayer.Antialiasing.TemporalAntialiasing && !this.isSceneView && this.temporalAntialiasing.IsSupported();
		}

		// Token: 0x06003F39 RID: 16185 RVA: 0x00173481 File Offset: 0x00171681
		public bool IsDebugOverlayEnabled(DebugOverlay overlay)
		{
			return this.debugLayer.debugOverlay == overlay;
		}

		// Token: 0x06003F3A RID: 16186 RVA: 0x00173491 File Offset: 0x00171691
		public void PushDebugOverlay(CommandBuffer cmd, RenderTargetIdentifier source, PropertySheet sheet, int pass)
		{
			this.debugLayer.PushDebugOverlay(cmd, source, sheet, pass);
		}

		// Token: 0x06003F3B RID: 16187 RVA: 0x001734A4 File Offset: 0x001716A4
		private RenderTextureDescriptor GetDescriptor(int depthBufferBits = 0, RenderTextureFormat colorFormat = RenderTextureFormat.Default, RenderTextureReadWrite readWrite = RenderTextureReadWrite.Default)
		{
			RenderTextureDescriptor result = new RenderTextureDescriptor(this.m_sourceDescriptor.width, this.m_sourceDescriptor.height, this.m_sourceDescriptor.colorFormat, depthBufferBits);
			result.dimension = this.m_sourceDescriptor.dimension;
			result.volumeDepth = this.m_sourceDescriptor.volumeDepth;
			result.vrUsage = this.m_sourceDescriptor.vrUsage;
			result.msaaSamples = this.m_sourceDescriptor.msaaSamples;
			result.memoryless = this.m_sourceDescriptor.memoryless;
			result.useMipMap = this.m_sourceDescriptor.useMipMap;
			result.autoGenerateMips = this.m_sourceDescriptor.autoGenerateMips;
			result.enableRandomWrite = this.m_sourceDescriptor.enableRandomWrite;
			result.shadowSamplingMode = this.m_sourceDescriptor.shadowSamplingMode;
			if (colorFormat != RenderTextureFormat.Default)
			{
				result.colorFormat = colorFormat;
			}
			if (readWrite == RenderTextureReadWrite.sRGB)
			{
				result.sRGB = true;
			}
			else if (readWrite == RenderTextureReadWrite.Linear)
			{
				result.sRGB = false;
			}
			else if (readWrite == RenderTextureReadWrite.Default)
			{
				result.sRGB = (QualitySettings.activeColorSpace > ColorSpace.Gamma);
			}
			return result;
		}

		// Token: 0x06003F3C RID: 16188 RVA: 0x001735B8 File Offset: 0x001717B8
		public void GetScreenSpaceTemporaryRT(CommandBuffer cmd, int nameID, int depthBufferBits = 0, RenderTextureFormat colorFormat = RenderTextureFormat.Default, RenderTextureReadWrite readWrite = RenderTextureReadWrite.Default, FilterMode filter = FilterMode.Bilinear, int widthOverride = 0, int heightOverride = 0)
		{
			RenderTextureDescriptor descriptor = this.GetDescriptor(depthBufferBits, colorFormat, readWrite);
			if (widthOverride > 0)
			{
				descriptor.width = widthOverride;
			}
			if (heightOverride > 0)
			{
				descriptor.height = heightOverride;
			}
			if (this.stereoActive && descriptor.dimension == TextureDimension.Tex2DArray)
			{
				descriptor.dimension = TextureDimension.Tex2D;
			}
			cmd.GetTemporaryRT(nameID, descriptor, filter);
		}

		// Token: 0x06003F3D RID: 16189 RVA: 0x00173614 File Offset: 0x00171814
		public RenderTexture GetScreenSpaceTemporaryRT(int depthBufferBits = 0, RenderTextureFormat colorFormat = RenderTextureFormat.Default, RenderTextureReadWrite readWrite = RenderTextureReadWrite.Default, int widthOverride = 0, int heightOverride = 0)
		{
			RenderTextureDescriptor descriptor = this.GetDescriptor(depthBufferBits, colorFormat, readWrite);
			if (widthOverride > 0)
			{
				descriptor.width = widthOverride;
			}
			if (heightOverride > 0)
			{
				descriptor.height = heightOverride;
			}
			return RenderTexture.GetTemporary(descriptor);
		}

		// Token: 0x02000F1D RID: 3869
		public enum StereoRenderingMode
		{
			// Token: 0x04004E61 RID: 20065
			MultiPass,
			// Token: 0x04004E62 RID: 20066
			SinglePass,
			// Token: 0x04004E63 RID: 20067
			SinglePassInstanced,
			// Token: 0x04004E64 RID: 20068
			SinglePassMultiview
		}
	}
}
