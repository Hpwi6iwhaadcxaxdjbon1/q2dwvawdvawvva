using System;
using UnityEngine;

// Token: 0x020003CA RID: 970
public class EntityTimedDestroy : EntityComponent<BaseEntity>
{
	// Token: 0x04001A24 RID: 6692
	public float secondsTillDestroy = 1f;

	// Token: 0x0600219A RID: 8602 RVA: 0x000DAE27 File Offset: 0x000D9027
	private void OnEnable()
	{
		base.Invoke(new Action(this.TimedDestroy), this.secondsTillDestroy);
	}

	// Token: 0x0600219B RID: 8603 RVA: 0x000DAE41 File Offset: 0x000D9041
	private void TimedDestroy()
	{
		if (base.baseEntity != null)
		{
			base.baseEntity.Kill(BaseNetworkable.DestroyMode.None);
			return;
		}
		Debug.LogWarning("EntityTimedDestroy failed, baseEntity was already null!");
	}
}
