using System;
using UnityEngine;

// Token: 0x0200098C RID: 2444
[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
[AddComponentMenu("KriptoFX/Explosion_Bloom")]
[ImageEffectAllowedInSceneView]
public class Explosion_Bloom : MonoBehaviour
{
	// Token: 0x04003472 RID: 13426
	[SerializeField]
	public Explosion_Bloom.Settings settings = Explosion_Bloom.Settings.defaultSettings;

	// Token: 0x04003473 RID: 13427
	[SerializeField]
	[HideInInspector]
	private Shader m_Shader;

	// Token: 0x04003474 RID: 13428
	private Material m_Material;

	// Token: 0x04003475 RID: 13429
	private const int kMaxIterations = 16;

	// Token: 0x04003476 RID: 13430
	private RenderTexture[] m_blurBuffer1 = new RenderTexture[16];

	// Token: 0x04003477 RID: 13431
	private RenderTexture[] m_blurBuffer2 = new RenderTexture[16];

	// Token: 0x04003478 RID: 13432
	private int m_Threshold;

	// Token: 0x04003479 RID: 13433
	private int m_Curve;

	// Token: 0x0400347A RID: 13434
	private int m_PrefilterOffs;

	// Token: 0x0400347B RID: 13435
	private int m_SampleScale;

	// Token: 0x0400347C RID: 13436
	private int m_Intensity;

	// Token: 0x0400347D RID: 13437
	private int m_BaseTex;

	// Token: 0x1700049D RID: 1181
	// (get) Token: 0x06003A30 RID: 14896 RVA: 0x001585BE File Offset: 0x001567BE
	public Shader shader
	{
		get
		{
			if (this.m_Shader == null)
			{
				this.m_Shader = Shader.Find("Hidden/KriptoFX/PostEffects/Explosion_Bloom");
			}
			return this.m_Shader;
		}
	}

	// Token: 0x1700049E RID: 1182
	// (get) Token: 0x06003A31 RID: 14897 RVA: 0x001585E4 File Offset: 0x001567E4
	public Material material
	{
		get
		{
			if (this.m_Material == null)
			{
				this.m_Material = Explosion_Bloom.CheckShaderAndCreateMaterial(this.shader);
			}
			return this.m_Material;
		}
	}

	// Token: 0x06003A32 RID: 14898 RVA: 0x0015860C File Offset: 0x0015680C
	public static bool IsSupported(Shader s, bool needDepth, bool needHdr, MonoBehaviour effect)
	{
		if (s == null || !s.isSupported)
		{
			Debug.LogWarningFormat("Missing shader for image effect {0}", new object[]
			{
				effect
			});
			return false;
		}
		if (!SystemInfo.supportsImageEffects)
		{
			Debug.LogWarningFormat("Image effects aren't supported on this device ({0})", new object[]
			{
				effect
			});
			return false;
		}
		if (needDepth && !SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.Depth))
		{
			Debug.LogWarningFormat("Depth textures aren't supported on this device ({0})", new object[]
			{
				effect
			});
			return false;
		}
		if (needHdr && !SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.ARGBHalf))
		{
			Debug.LogWarningFormat("Floating point textures aren't supported on this device ({0})", new object[]
			{
				effect
			});
			return false;
		}
		return true;
	}

	// Token: 0x06003A33 RID: 14899 RVA: 0x001586A0 File Offset: 0x001568A0
	public static Material CheckShaderAndCreateMaterial(Shader s)
	{
		if (s == null || !s.isSupported)
		{
			return null;
		}
		return new Material(s)
		{
			hideFlags = HideFlags.DontSave
		};
	}

	// Token: 0x1700049F RID: 1183
	// (get) Token: 0x06003A34 RID: 14900 RVA: 0x001586C3 File Offset: 0x001568C3
	public static bool supportsDX11
	{
		get
		{
			return SystemInfo.graphicsShaderLevel >= 50 && SystemInfo.supportsComputeShaders;
		}
	}

	// Token: 0x06003A35 RID: 14901 RVA: 0x001586D8 File Offset: 0x001568D8
	private void Awake()
	{
		this.m_Threshold = Shader.PropertyToID("_Threshold");
		this.m_Curve = Shader.PropertyToID("_Curve");
		this.m_PrefilterOffs = Shader.PropertyToID("_PrefilterOffs");
		this.m_SampleScale = Shader.PropertyToID("_SampleScale");
		this.m_Intensity = Shader.PropertyToID("_Intensity");
		this.m_BaseTex = Shader.PropertyToID("_BaseTex");
	}

	// Token: 0x06003A36 RID: 14902 RVA: 0x00158745 File Offset: 0x00156945
	private void OnEnable()
	{
		if (!Explosion_Bloom.IsSupported(this.shader, true, false, this))
		{
			base.enabled = false;
		}
	}

	// Token: 0x06003A37 RID: 14903 RVA: 0x0015875E File Offset: 0x0015695E
	private void OnDisable()
	{
		if (this.m_Material != null)
		{
			UnityEngine.Object.DestroyImmediate(this.m_Material);
		}
		this.m_Material = null;
	}

	// Token: 0x06003A38 RID: 14904 RVA: 0x00158780 File Offset: 0x00156980
	private void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		bool isMobilePlatform = Application.isMobilePlatform;
		int num = source.width;
		int num2 = source.height;
		if (!this.settings.highQuality)
		{
			num /= 2;
			num2 /= 2;
		}
		RenderTextureFormat format = isMobilePlatform ? RenderTextureFormat.Default : RenderTextureFormat.DefaultHDR;
		float num3 = Mathf.Log((float)num2, 2f) + this.settings.radius - 8f;
		int num4 = (int)num3;
		int num5 = Mathf.Clamp(num4, 1, 16);
		float thresholdLinear = this.settings.thresholdLinear;
		this.material.SetFloat(this.m_Threshold, thresholdLinear);
		float num6 = thresholdLinear * this.settings.softKnee + 1E-05f;
		Vector3 v = new Vector3(thresholdLinear - num6, num6 * 2f, 0.25f / num6);
		this.material.SetVector(this.m_Curve, v);
		bool flag = !this.settings.highQuality && this.settings.antiFlicker;
		this.material.SetFloat(this.m_PrefilterOffs, flag ? -0.5f : 0f);
		this.material.SetFloat(this.m_SampleScale, 0.5f + num3 - (float)num4);
		this.material.SetFloat(this.m_Intensity, Mathf.Max(0f, this.settings.intensity));
		RenderTexture temporary = RenderTexture.GetTemporary(num, num2, 0, format);
		Graphics.Blit(source, temporary, this.material, this.settings.antiFlicker ? 1 : 0);
		RenderTexture renderTexture = temporary;
		for (int i = 0; i < num5; i++)
		{
			this.m_blurBuffer1[i] = RenderTexture.GetTemporary(renderTexture.width / 2, renderTexture.height / 2, 0, format);
			Graphics.Blit(renderTexture, this.m_blurBuffer1[i], this.material, (i == 0) ? (this.settings.antiFlicker ? 3 : 2) : 4);
			renderTexture = this.m_blurBuffer1[i];
		}
		for (int j = num5 - 2; j >= 0; j--)
		{
			RenderTexture renderTexture2 = this.m_blurBuffer1[j];
			this.material.SetTexture(this.m_BaseTex, renderTexture2);
			this.m_blurBuffer2[j] = RenderTexture.GetTemporary(renderTexture2.width, renderTexture2.height, 0, format);
			Graphics.Blit(renderTexture, this.m_blurBuffer2[j], this.material, this.settings.highQuality ? 6 : 5);
			renderTexture = this.m_blurBuffer2[j];
		}
		int num7 = 7;
		num7 += (this.settings.highQuality ? 1 : 0);
		this.material.SetTexture(this.m_BaseTex, source);
		Graphics.Blit(renderTexture, destination, this.material, num7);
		for (int k = 0; k < 16; k++)
		{
			if (this.m_blurBuffer1[k] != null)
			{
				RenderTexture.ReleaseTemporary(this.m_blurBuffer1[k]);
			}
			if (this.m_blurBuffer2[k] != null)
			{
				RenderTexture.ReleaseTemporary(this.m_blurBuffer2[k]);
			}
			this.m_blurBuffer1[k] = null;
			this.m_blurBuffer2[k] = null;
		}
		RenderTexture.ReleaseTemporary(temporary);
	}

	// Token: 0x02000ED0 RID: 3792
	[Serializable]
	public struct Settings
	{
		// Token: 0x04004D14 RID: 19732
		[SerializeField]
		[Tooltip("Filters out pixels under this level of brightness.")]
		public float threshold;

		// Token: 0x04004D15 RID: 19733
		[SerializeField]
		[Range(0f, 1f)]
		[Tooltip("Makes transition between under/over-threshold gradual.")]
		public float softKnee;

		// Token: 0x04004D16 RID: 19734
		[SerializeField]
		[Range(1f, 7f)]
		[Tooltip("Changes extent of veiling effects in a screen resolution-independent fashion.")]
		public float radius;

		// Token: 0x04004D17 RID: 19735
		[SerializeField]
		[Tooltip("Blend factor of the result image.")]
		public float intensity;

		// Token: 0x04004D18 RID: 19736
		[SerializeField]
		[Tooltip("Controls filter quality and buffer resolution.")]
		public bool highQuality;

		// Token: 0x04004D19 RID: 19737
		[SerializeField]
		[Tooltip("Reduces flashing noise with an additional filter.")]
		public bool antiFlicker;

		// Token: 0x17000706 RID: 1798
		// (get) Token: 0x06005363 RID: 21347 RVA: 0x001B2574 File Offset: 0x001B0774
		// (set) Token: 0x06005362 RID: 21346 RVA: 0x001B256B File Offset: 0x001B076B
		public float thresholdGamma
		{
			get
			{
				return Mathf.Max(0f, this.threshold);
			}
			set
			{
				this.threshold = value;
			}
		}

		// Token: 0x17000707 RID: 1799
		// (get) Token: 0x06005365 RID: 21349 RVA: 0x001B2594 File Offset: 0x001B0794
		// (set) Token: 0x06005364 RID: 21348 RVA: 0x001B2586 File Offset: 0x001B0786
		public float thresholdLinear
		{
			get
			{
				return Mathf.GammaToLinearSpace(this.thresholdGamma);
			}
			set
			{
				this.threshold = Mathf.LinearToGammaSpace(value);
			}
		}

		// Token: 0x17000708 RID: 1800
		// (get) Token: 0x06005366 RID: 21350 RVA: 0x001B25A4 File Offset: 0x001B07A4
		public static Explosion_Bloom.Settings defaultSettings
		{
			get
			{
				return new Explosion_Bloom.Settings
				{
					threshold = 2f,
					softKnee = 0f,
					radius = 7f,
					intensity = 0.7f,
					highQuality = true,
					antiFlicker = true
				};
			}
		}
	}
}
