using System;
using UnityEngine.Scripting;

namespace UnityEngine.Rendering.PostProcessing
{
	// Token: 0x02000A53 RID: 2643
	[Preserve]
	internal sealed class ChromaticAberrationRenderer : PostProcessEffectRenderer<ChromaticAberration>
	{
		// Token: 0x0400388C RID: 14476
		private Texture2D m_InternalSpectralLut;

		// Token: 0x06003F66 RID: 16230 RVA: 0x00174780 File Offset: 0x00172980
		public override void Render(PostProcessRenderContext context)
		{
			Texture texture = base.settings.spectralLut.value;
			if (texture == null)
			{
				if (this.m_InternalSpectralLut == null)
				{
					this.m_InternalSpectralLut = new Texture2D(3, 1, TextureFormat.RGB24, false)
					{
						name = "Chromatic Aberration Spectrum Lookup",
						filterMode = FilterMode.Bilinear,
						wrapMode = TextureWrapMode.Clamp,
						anisoLevel = 0,
						hideFlags = HideFlags.DontSave
					};
					this.m_InternalSpectralLut.SetPixels(new Color[]
					{
						new Color(1f, 0f, 0f),
						new Color(0f, 1f, 0f),
						new Color(0f, 0f, 1f)
					});
					this.m_InternalSpectralLut.Apply();
				}
				texture = this.m_InternalSpectralLut;
			}
			PropertySheet uberSheet = context.uberSheet;
			uberSheet.EnableKeyword((base.settings.fastMode || SystemInfo.graphicsDeviceType == GraphicsDeviceType.OpenGLES2) ? "CHROMATIC_ABERRATION_LOW" : "CHROMATIC_ABERRATION");
			uberSheet.properties.SetFloat(ShaderIDs.ChromaticAberration_Amount, base.settings.intensity * 0.05f);
			uberSheet.properties.SetTexture(ShaderIDs.ChromaticAberration_SpectralLut, texture);
		}

		// Token: 0x06003F67 RID: 16231 RVA: 0x001748D5 File Offset: 0x00172AD5
		public override void Release()
		{
			RuntimeUtilities.Destroy(this.m_InternalSpectralLut);
			this.m_InternalSpectralLut = null;
		}
	}
}
