using System;
using UnityEngine;

// Token: 0x02000589 RID: 1417
public class TriggerNoSpray : TriggerBase
{
	// Token: 0x04002325 RID: 8997
	public BoxCollider TriggerCollider;

	// Token: 0x04002326 RID: 8998
	private OBB cachedBounds;

	// Token: 0x04002327 RID: 8999
	private Transform cachedTransform;

	// Token: 0x06002B59 RID: 11097 RVA: 0x00107394 File Offset: 0x00105594
	private void OnEnable()
	{
		this.cachedTransform = base.transform;
		this.cachedBounds = new OBB(this.cachedTransform, new Bounds(this.TriggerCollider.center, this.TriggerCollider.size));
	}

	// Token: 0x06002B5A RID: 11098 RVA: 0x001073D0 File Offset: 0x001055D0
	internal override GameObject InterestedInObject(GameObject obj)
	{
		BaseEntity baseEntity = obj.ToBaseEntity();
		if (baseEntity == null)
		{
			return null;
		}
		if (baseEntity.ToPlayer() == null)
		{
			return null;
		}
		return baseEntity.gameObject;
	}

	// Token: 0x06002B5B RID: 11099 RVA: 0x00107405 File Offset: 0x00105605
	public bool IsPositionValid(Vector3 worldPosition)
	{
		return !this.cachedBounds.Contains(worldPosition);
	}
}
