using System;

namespace UnityEngine.Rendering.PostProcessing
{
	// Token: 0x02000A9A RID: 2714
	public sealed class PropertySheet
	{
		// Token: 0x17000587 RID: 1415
		// (get) Token: 0x060040A9 RID: 16553 RVA: 0x0017D529 File Offset: 0x0017B729
		// (set) Token: 0x060040AA RID: 16554 RVA: 0x0017D531 File Offset: 0x0017B731
		public MaterialPropertyBlock properties { get; private set; }

		// Token: 0x17000588 RID: 1416
		// (get) Token: 0x060040AB RID: 16555 RVA: 0x0017D53A File Offset: 0x0017B73A
		// (set) Token: 0x060040AC RID: 16556 RVA: 0x0017D542 File Offset: 0x0017B742
		internal Material material { get; private set; }

		// Token: 0x060040AD RID: 16557 RVA: 0x0017D54B File Offset: 0x0017B74B
		internal PropertySheet(Material material)
		{
			this.material = material;
			this.properties = new MaterialPropertyBlock();
		}

		// Token: 0x060040AE RID: 16558 RVA: 0x0017D565 File Offset: 0x0017B765
		public void ClearKeywords()
		{
			this.material.shaderKeywords = null;
		}

		// Token: 0x060040AF RID: 16559 RVA: 0x0017D573 File Offset: 0x0017B773
		public void EnableKeyword(string keyword)
		{
			this.material.EnableKeyword(keyword);
		}

		// Token: 0x060040B0 RID: 16560 RVA: 0x0017D581 File Offset: 0x0017B781
		public void DisableKeyword(string keyword)
		{
			this.material.DisableKeyword(keyword);
		}

		// Token: 0x060040B1 RID: 16561 RVA: 0x0017D58F File Offset: 0x0017B78F
		internal void Release()
		{
			RuntimeUtilities.Destroy(this.material);
			this.material = null;
		}
	}
}
