using System;
using UnityEngine.Scripting;
using UnityEngine.Serialization;

namespace UnityEngine.Rendering.PostProcessing
{
	// Token: 0x02000A5F RID: 2655
	[Preserve]
	[Serializable]
	public sealed class FastApproximateAntialiasing
	{
		// Token: 0x040038D3 RID: 14547
		[FormerlySerializedAs("mobileOptimized")]
		[Tooltip("Boost performances by lowering the effect quality. This setting is meant to be used on mobile and other low-end platforms but can also provide a nice performance boost on desktops and consoles.")]
		public bool fastMode;

		// Token: 0x040038D4 RID: 14548
		[Tooltip("Keep alpha channel. This will slightly lower the effect quality but allows rendering against a transparent background.")]
		public bool keepAlpha;
	}
}
