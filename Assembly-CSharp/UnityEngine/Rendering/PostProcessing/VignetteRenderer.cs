using System;
using UnityEngine.Scripting;

namespace UnityEngine.Rendering.PostProcessing
{
	// Token: 0x02000A74 RID: 2676
	[Preserve]
	internal sealed class VignetteRenderer : PostProcessEffectRenderer<Vignette>
	{
		// Token: 0x06003FDC RID: 16348 RVA: 0x00179CEC File Offset: 0x00177EEC
		public override void Render(PostProcessRenderContext context)
		{
			PropertySheet uberSheet = context.uberSheet;
			uberSheet.EnableKeyword("VIGNETTE");
			uberSheet.properties.SetColor(ShaderIDs.Vignette_Color, base.settings.color.value);
			if (base.settings.mode == VignetteMode.Classic)
			{
				uberSheet.properties.SetFloat(ShaderIDs.Vignette_Mode, 0f);
				uberSheet.properties.SetVector(ShaderIDs.Vignette_Center, base.settings.center.value);
				float z = (1f - base.settings.roundness.value) * 6f + base.settings.roundness.value;
				uberSheet.properties.SetVector(ShaderIDs.Vignette_Settings, new Vector4(base.settings.intensity.value * 3f, base.settings.smoothness.value * 5f, z, base.settings.rounded.value ? 1f : 0f));
				return;
			}
			uberSheet.properties.SetFloat(ShaderIDs.Vignette_Mode, 1f);
			uberSheet.properties.SetTexture(ShaderIDs.Vignette_Mask, base.settings.mask.value);
			uberSheet.properties.SetFloat(ShaderIDs.Vignette_Opacity, Mathf.Clamp01(base.settings.opacity.value));
		}
	}
}
