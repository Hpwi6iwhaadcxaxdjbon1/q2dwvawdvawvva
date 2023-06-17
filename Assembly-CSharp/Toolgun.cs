using System;
using Network;
using UnityEngine;

// Token: 0x020000E0 RID: 224
public class Toolgun : Hammer
{
	// Token: 0x04000C35 RID: 3125
	public GameObjectRef attackEffect;

	// Token: 0x04000C36 RID: 3126
	public GameObjectRef beamEffect;

	// Token: 0x04000C37 RID: 3127
	public GameObjectRef beamImpactEffect;

	// Token: 0x04000C38 RID: 3128
	public GameObjectRef errorEffect;

	// Token: 0x04000C39 RID: 3129
	public GameObjectRef beamEffectClassic;

	// Token: 0x04000C3A RID: 3130
	public GameObjectRef beamImpactEffectClassic;

	// Token: 0x04000C3B RID: 3131
	public Transform muzzlePoint;

	// Token: 0x060013A1 RID: 5025 RVA: 0x0009D860 File Offset: 0x0009BA60
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("Toolgun.OnRpcMessage", 0))
		{
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x060013A2 RID: 5026 RVA: 0x0009D8A0 File Offset: 0x0009BAA0
	public override void DoAttackShared(HitInfo info)
	{
		if (base.isServer)
		{
			base.ClientRPC<Vector3, Vector3>(null, "EffectSpawn", info.HitPositionWorld, info.HitNormalWorld);
		}
		base.DoAttackShared(info);
	}
}
