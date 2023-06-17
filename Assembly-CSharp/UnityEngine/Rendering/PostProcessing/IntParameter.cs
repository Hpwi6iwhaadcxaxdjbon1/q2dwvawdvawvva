using System;

namespace UnityEngine.Rendering.PostProcessing
{
	// Token: 0x02000A7E RID: 2686
	[Serializable]
	public sealed class IntParameter : ParameterOverride<int>
	{
		// Token: 0x0600400C RID: 16396 RVA: 0x0017A973 File Offset: 0x00178B73
		public override void Interp(int from, int to, float t)
		{
			this.value = (int)((float)from + (float)(to - from) * t);
		}
	}
}
