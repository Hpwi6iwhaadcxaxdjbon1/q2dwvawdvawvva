using System;

namespace UnityEngine
{
	// Token: 0x02000A28 RID: 2600
	public static class SkinnedMeshRendererEx
	{
		// Token: 0x06003DAA RID: 15786 RVA: 0x00169D38 File Offset: 0x00167F38
		public static Transform FindRig(this SkinnedMeshRenderer renderer)
		{
			Transform parent = renderer.transform.parent;
			Transform transform = renderer.rootBone;
			while (transform != null && transform.parent != null && transform.parent != parent)
			{
				transform = transform.parent;
			}
			return transform;
		}
	}
}
