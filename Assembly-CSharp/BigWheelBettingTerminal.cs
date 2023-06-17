using System;
using Network;
using UnityEngine;

// Token: 0x0200004D RID: 77
public class BigWheelBettingTerminal : StorageContainer
{
	// Token: 0x04000592 RID: 1426
	public BigWheelGame bigWheel;

	// Token: 0x04000593 RID: 1427
	public Vector3 seatedPlayerOffset = Vector3.forward;

	// Token: 0x04000594 RID: 1428
	public float offsetCheckRadius = 0.4f;

	// Token: 0x04000595 RID: 1429
	public SoundDefinition winSound;

	// Token: 0x04000596 RID: 1430
	public SoundDefinition loseSound;

	// Token: 0x04000597 RID: 1431
	[NonSerialized]
	public BasePlayer lastPlayer;

	// Token: 0x06000871 RID: 2161 RVA: 0x000519C8 File Offset: 0x0004FBC8
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("BigWheelBettingTerminal.OnRpcMessage", 0))
		{
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000872 RID: 2162 RVA: 0x00051A08 File Offset: 0x0004FC08
	public new void OnDrawGizmos()
	{
		Gizmos.color = Color.yellow;
		Gizmos.DrawSphere(base.transform.TransformPoint(this.seatedPlayerOffset), this.offsetCheckRadius);
		base.OnDrawGizmos();
	}

	// Token: 0x06000873 RID: 2163 RVA: 0x00051A38 File Offset: 0x0004FC38
	public bool IsPlayerValid(BasePlayer player)
	{
		if (!player.isMounted || !(player.GetMounted() is BaseChair))
		{
			return false;
		}
		Vector3 b = base.transform.TransformPoint(this.seatedPlayerOffset);
		return Vector3Ex.Distance2D(player.transform.position, b) <= this.offsetCheckRadius;
	}

	// Token: 0x06000874 RID: 2164 RVA: 0x00051A8A File Offset: 0x0004FC8A
	public override bool PlayerOpenLoot(BasePlayer player, string panelToOpen = "", bool doPositionChecks = true)
	{
		if (!this.IsPlayerValid(player))
		{
			return false;
		}
		bool flag = base.PlayerOpenLoot(player, panelToOpen, true);
		if (flag)
		{
			this.lastPlayer = player;
		}
		return flag;
	}

	// Token: 0x06000875 RID: 2165 RVA: 0x00051AAC File Offset: 0x0004FCAC
	public bool TrySetBigWheel(BigWheelGame newWheel)
	{
		if (base.isClient)
		{
			return false;
		}
		if (this.bigWheel != null && this.bigWheel != newWheel)
		{
			float num = Vector3.SqrMagnitude(this.bigWheel.transform.position - base.transform.position);
			if (Vector3.SqrMagnitude(newWheel.transform.position - base.transform.position) >= num)
			{
				return false;
			}
			this.bigWheel.RemoveTerminal(this);
		}
		this.bigWheel = newWheel;
		return true;
	}
}
