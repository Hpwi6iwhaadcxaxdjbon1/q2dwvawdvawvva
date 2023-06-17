using System;
using UnityEngine;

// Token: 0x02000693 RID: 1683
public class TerrainCollisionTrigger : EnvironmentVolumeTrigger
{
	// Token: 0x06002FE1 RID: 12257 RVA: 0x0011FB9F File Offset: 0x0011DD9F
	protected void OnTriggerEnter(Collider other)
	{
		if (!TerrainMeta.Collision || other.isTrigger)
		{
			return;
		}
		this.UpdateCollider(other, true);
	}

	// Token: 0x06002FE2 RID: 12258 RVA: 0x0011FBBE File Offset: 0x0011DDBE
	protected void OnTriggerExit(Collider other)
	{
		if (!TerrainMeta.Collision || other.isTrigger)
		{
			return;
		}
		this.UpdateCollider(other, false);
	}

	// Token: 0x06002FE3 RID: 12259 RVA: 0x0011FBE0 File Offset: 0x0011DDE0
	private void UpdateCollider(Collider other, bool state)
	{
		TerrainMeta.Collision.SetIgnore(other, base.volume.trigger, state);
		TerrainCollisionProxy component = other.GetComponent<TerrainCollisionProxy>();
		if (component)
		{
			for (int i = 0; i < component.colliders.Length; i++)
			{
				TerrainMeta.Collision.SetIgnore(component.colliders[i], base.volume.trigger, state);
			}
		}
	}
}
