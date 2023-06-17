using System;
using ConVar;
using UnityEngine;

// Token: 0x0200020C RID: 524
public class NPCDoorTriggerBox : MonoBehaviour
{
	// Token: 0x04001361 RID: 4961
	private Door door;

	// Token: 0x04001362 RID: 4962
	private static int playerServerLayer = -1;

	// Token: 0x06001B5D RID: 7005 RVA: 0x000C1E88 File Offset: 0x000C0088
	public void Setup(Door d)
	{
		this.door = d;
		base.transform.SetParent(this.door.transform, false);
		base.gameObject.layer = 18;
		BoxCollider boxCollider = base.gameObject.AddComponent<BoxCollider>();
		boxCollider.isTrigger = true;
		boxCollider.center = Vector3.zero;
		boxCollider.size = Vector3.one * AI.npc_door_trigger_size;
	}

	// Token: 0x06001B5E RID: 7006 RVA: 0x000C1EF4 File Offset: 0x000C00F4
	private void OnTriggerEnter(Collider other)
	{
		if (this.door == null || this.door.isClient || this.door.IsLocked())
		{
			return;
		}
		if (!this.door.isSecurityDoor && this.door.IsOpen())
		{
			return;
		}
		if (this.door.isSecurityDoor && !this.door.IsOpen())
		{
			return;
		}
		if (NPCDoorTriggerBox.playerServerLayer < 0)
		{
			NPCDoorTriggerBox.playerServerLayer = LayerMask.NameToLayer("Player (Server)");
		}
		if ((other.gameObject.layer & NPCDoorTriggerBox.playerServerLayer) > 0)
		{
			BasePlayer component = other.gameObject.GetComponent<BasePlayer>();
			if (component != null && component.IsNpc && !this.door.isSecurityDoor)
			{
				this.door.SetOpen(true, false);
			}
		}
	}
}
