using System;

namespace UnityEngine.Rendering.PostProcessing
{
	// Token: 0x02000A7D RID: 2685
	[Serializable]
	public sealed class FloatParameter : ParameterOverride<float>
	{
		// Token: 0x0600400A RID: 16394 RVA: 0x0017A95C File Offset: 0x00178B5C
		public override void Interp(float from, float to, float t)
		{
			this.value = from + (to - from) * t;
		}
	}
}
