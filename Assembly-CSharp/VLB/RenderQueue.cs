using System;

namespace VLB
{
	// Token: 0x020009B2 RID: 2482
	public enum RenderQueue
	{
		// Token: 0x040035DA RID: 13786
		Custom,
		// Token: 0x040035DB RID: 13787
		Background = 1000,
		// Token: 0x040035DC RID: 13788
		Geometry = 2000,
		// Token: 0x040035DD RID: 13789
		AlphaTest = 2450,
		// Token: 0x040035DE RID: 13790
		GeometryLast = 2500,
		// Token: 0x040035DF RID: 13791
		Transparent = 3000,
		// Token: 0x040035E0 RID: 13792
		Overlay = 4000
	}
}
