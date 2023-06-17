using System;
using UnityEngine;

// Token: 0x02000352 RID: 850
public class ScaleTransform : ScaleRenderer
{
	// Token: 0x0400189A RID: 6298
	private Vector3 initialScale;

	// Token: 0x06001F2E RID: 7982 RVA: 0x000D35AE File Offset: 0x000D17AE
	public override void SetScale_Internal(float scale)
	{
		base.SetScale_Internal(scale);
		this.myRenderer.transform.localScale = this.initialScale * scale;
	}

	// Token: 0x06001F2F RID: 7983 RVA: 0x000D35D3 File Offset: 0x000D17D3
	public override void GatherInitialValues()
	{
		this.initialScale = this.myRenderer.transform.localScale;
		base.GatherInitialValues();
	}
}
