using System;

namespace UnityEngine.Rendering.PostProcessing
{
	// Token: 0x02000A86 RID: 2694
	[Serializable]
	public sealed class TextureParameter : ParameterOverride<Texture>
	{
		// Token: 0x0400394D RID: 14669
		public TextureParameterDefault defaultState = TextureParameterDefault.Black;

		// Token: 0x06004022 RID: 16418 RVA: 0x0017ACA0 File Offset: 0x00178EA0
		public override void Interp(Texture from, Texture to, float t)
		{
			if (from == null && to == null)
			{
				this.value = null;
				return;
			}
			if (from != null && to != null)
			{
				this.value = TextureLerper.instance.Lerp(from, to, t);
				return;
			}
			if (this.defaultState == TextureParameterDefault.Lut2D)
			{
				Texture lutStrip = RuntimeUtilities.GetLutStrip((from != null) ? from.height : to.height);
				if (from == null)
				{
					from = lutStrip;
				}
				if (to == null)
				{
					to = lutStrip;
				}
			}
			Color to2;
			switch (this.defaultState)
			{
			case TextureParameterDefault.Black:
				to2 = Color.black;
				break;
			case TextureParameterDefault.White:
				to2 = Color.white;
				break;
			case TextureParameterDefault.Transparent:
				to2 = Color.clear;
				break;
			case TextureParameterDefault.Lut2D:
			{
				Texture lutStrip2 = RuntimeUtilities.GetLutStrip((from != null) ? from.height : to.height);
				if (from == null)
				{
					from = lutStrip2;
				}
				if (to == null)
				{
					to = lutStrip2;
				}
				if (from.width != to.width || from.height != to.height)
				{
					this.value = null;
					return;
				}
				this.value = TextureLerper.instance.Lerp(from, to, t);
				return;
			}
			default:
				base.Interp(from, to, t);
				return;
			}
			if (from == null)
			{
				this.value = TextureLerper.instance.Lerp(to, to2, 1f - t);
				return;
			}
			this.value = TextureLerper.instance.Lerp(from, to2, t);
		}
	}
}
