using System;
using UnityEngine;

// Token: 0x020004FD RID: 1277
public class DestroyOnGroundMissing : MonoBehaviour, IServerComponent
{
	// Token: 0x0600291B RID: 10523 RVA: 0x000FCB1C File Offset: 0x000FAD1C
	private void OnGroundMissing()
	{
		BaseEntity baseEntity = base.gameObject.ToBaseEntity();
		if (baseEntity != null)
		{
			BaseCombatEntity baseCombatEntity = baseEntity as BaseCombatEntity;
			if (baseCombatEntity != null)
			{
				baseCombatEntity.Die(null);
				return;
			}
			baseEntity.Kill(BaseNetworkable.DestroyMode.Gib);
		}
	}
}
