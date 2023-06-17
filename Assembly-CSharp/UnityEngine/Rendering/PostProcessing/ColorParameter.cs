using System;

namespace UnityEngine.Rendering.PostProcessing
{
	// Token: 0x02000A80 RID: 2688
	[Serializable]
	public sealed class ColorParameter : ParameterOverride<Color>
	{
		// Token: 0x0600400F RID: 16399 RVA: 0x0017A990 File Offset: 0x00178B90
		public override void Interp(Color from, Color to, float t)
		{
			this.value.r = from.r + (to.r - from.r) * t;
			this.value.g = from.g + (to.g - from.g) * t;
			this.value.b = from.b + (to.b - from.b) * t;
			this.value.a = from.a + (to.a - from.a) * t;
		}

		// Token: 0x06004010 RID: 16400 RVA: 0x0017AA21 File Offset: 0x00178C21
		public static implicit operator Vector4(ColorParameter prop)
		{
			return prop.value;
		}
	}
}
