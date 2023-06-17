using System;
using UnityEngine;

// Token: 0x020001B8 RID: 440
public class MapMarkerExplosion : MapMarker
{
	// Token: 0x0400119E RID: 4510
	private float duration = 10f;

	// Token: 0x060018F2 RID: 6386 RVA: 0x000B875C File Offset: 0x000B695C
	public void SetDuration(float newDuration)
	{
		this.duration = newDuration;
		if (base.IsInvoking(new Action(this.DelayedDestroy)))
		{
			base.CancelInvoke(new Action(this.DelayedDestroy));
		}
		base.Invoke(new Action(this.DelayedDestroy), this.duration * 60f);
	}

	// Token: 0x060018F3 RID: 6387 RVA: 0x000B87B4 File Offset: 0x000B69B4
	public override void Load(BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.fromDisk)
		{
			Debug.LogWarning("Loaded explosion marker from disk, cleaning up");
			base.Invoke(new Action(this.DelayedDestroy), 3f);
		}
	}

	// Token: 0x060018F4 RID: 6388 RVA: 0x00003384 File Offset: 0x00001584
	public void DelayedDestroy()
	{
		base.Kill(BaseNetworkable.DestroyMode.None);
	}
}
