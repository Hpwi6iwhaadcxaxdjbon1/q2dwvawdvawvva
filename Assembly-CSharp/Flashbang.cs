using System;
using Network;
using UnityEngine;

// Token: 0x02000077 RID: 119
public class Flashbang : TimedExplosive
{
	// Token: 0x04000768 RID: 1896
	public SoundDefinition deafLoopDef;

	// Token: 0x04000769 RID: 1897
	public float flashReductionPerSecond = 1f;

	// Token: 0x0400076A RID: 1898
	public float flashToAdd = 3f;

	// Token: 0x0400076B RID: 1899
	public float flashMinRange = 5f;

	// Token: 0x0400076C RID: 1900
	public float flashMaxRange = 10f;

	// Token: 0x06000B3E RID: 2878 RVA: 0x0006509C File Offset: 0x0006329C
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("Flashbang.OnRpcMessage", 0))
		{
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000B3F RID: 2879 RVA: 0x000650DC File Offset: 0x000632DC
	public override void Explode()
	{
		base.ClientRPC<Vector3>(null, "Client_DoFlash", base.transform.position);
		base.Explode();
	}

	// Token: 0x06000B40 RID: 2880 RVA: 0x00029A3C File Offset: 0x00027C3C
	public void DelayedDestroy()
	{
		base.Kill(BaseNetworkable.DestroyMode.Gib);
	}
}
