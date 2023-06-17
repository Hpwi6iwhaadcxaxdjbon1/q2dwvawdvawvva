using System;
using UnityEngine;

// Token: 0x020002F0 RID: 752
public class UnparentOnDestroy : MonoBehaviour, IOnParentDestroying
{
	// Token: 0x04001769 RID: 5993
	public float destroyAfterSeconds = 1f;

	// Token: 0x06001E0B RID: 7691 RVA: 0x000CD20A File Offset: 0x000CB40A
	public void OnParentDestroying()
	{
		base.transform.parent = null;
		GameManager.Destroy(base.gameObject, (this.destroyAfterSeconds <= 0f) ? 1f : this.destroyAfterSeconds);
	}

	// Token: 0x06001E0C RID: 7692 RVA: 0x000CD23D File Offset: 0x000CB43D
	protected void OnValidate()
	{
		if (this.destroyAfterSeconds <= 0f)
		{
			this.destroyAfterSeconds = 1f;
		}
	}
}
