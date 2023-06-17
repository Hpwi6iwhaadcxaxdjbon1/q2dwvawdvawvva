using System;
using Network;
using UnityEngine;

// Token: 0x0200006B RID: 107
public class DoorKnocker : BaseCombatEntity
{
	// Token: 0x040006EE RID: 1774
	public Animator knocker1;

	// Token: 0x040006EF RID: 1775
	public Animator knocker2;

	// Token: 0x06000A9D RID: 2717 RVA: 0x00061288 File Offset: 0x0005F488
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("DoorKnocker.OnRpcMessage", 0))
		{
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000A9E RID: 2718 RVA: 0x000612C8 File Offset: 0x0005F4C8
	public void Knock(BasePlayer player)
	{
		base.ClientRPC<Vector3>(null, "ClientKnock", player.transform.position);
	}
}
