using System;
using ConVar;
using UnityEngine;

// Token: 0x0200020B RID: 523
public class NPCBarricadeTriggerBox : MonoBehaviour
{
	// Token: 0x0400135F RID: 4959
	private Barricade target;

	// Token: 0x04001360 RID: 4960
	private static int playerServerLayer = -1;

	// Token: 0x06001B59 RID: 7001 RVA: 0x000C1D68 File Offset: 0x000BFF68
	public void Setup(Barricade t)
	{
		this.target = t;
		base.transform.SetParent(this.target.transform, false);
		base.gameObject.layer = 18;
		BoxCollider boxCollider = base.gameObject.AddComponent<BoxCollider>();
		boxCollider.isTrigger = true;
		boxCollider.center = Vector3.zero;
		boxCollider.size = Vector3.one * AI.npc_door_trigger_size + Vector3.right * this.target.bounds.size.x;
	}

	// Token: 0x06001B5A RID: 7002 RVA: 0x000C1DF8 File Offset: 0x000BFFF8
	private void OnTriggerEnter(Collider other)
	{
		if (this.target == null || this.target.isClient)
		{
			return;
		}
		if (NPCBarricadeTriggerBox.playerServerLayer < 0)
		{
			NPCBarricadeTriggerBox.playerServerLayer = LayerMask.NameToLayer("Player (Server)");
		}
		if ((other.gameObject.layer & NPCBarricadeTriggerBox.playerServerLayer) > 0)
		{
			BasePlayer component = other.gameObject.GetComponent<BasePlayer>();
			if (component != null && component.IsNpc && !(component is BasePet))
			{
				this.target.Kill(BaseNetworkable.DestroyMode.Gib);
			}
		}
	}
}
