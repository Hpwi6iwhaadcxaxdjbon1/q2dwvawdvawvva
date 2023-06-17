using System;
using System.Collections.Generic;
using Facepunch;
using ProtoBuf;
using UnityEngine;

// Token: 0x020000F4 RID: 244
public class ZiplineArrivalPoint : global::BaseEntity
{
	// Token: 0x04000D8C RID: 3468
	public LineRenderer Line;

	// Token: 0x04000D8D RID: 3469
	private Vector3[] linePositions;

	// Token: 0x0600155B RID: 5467 RVA: 0x000A9540 File Offset: 0x000A7740
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		if (info.msg.ZiplineArrival == null)
		{
			info.msg.ZiplineArrival = Pool.Get<ProtoBuf.ZiplineArrivalPoint>();
		}
		info.msg.ZiplineArrival.linePoints = Pool.GetList<VectorData>();
		foreach (Vector3 v in this.linePositions)
		{
			info.msg.ZiplineArrival.linePoints.Add(v);
		}
	}

	// Token: 0x0600155C RID: 5468 RVA: 0x000A95C0 File Offset: 0x000A77C0
	public void SetPositions(List<Vector3> points)
	{
		this.linePositions = new Vector3[points.Count];
		for (int i = 0; i < points.Count; i++)
		{
			this.linePositions[i] = points[i];
		}
	}

	// Token: 0x0600155D RID: 5469 RVA: 0x000A9604 File Offset: 0x000A7804
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.ZiplineArrival != null && this.linePositions == null)
		{
			this.linePositions = new Vector3[info.msg.ZiplineArrival.linePoints.Count];
			for (int i = 0; i < info.msg.ZiplineArrival.linePoints.Count; i++)
			{
				this.linePositions[i] = info.msg.ZiplineArrival.linePoints[i];
			}
		}
	}

	// Token: 0x0600155E RID: 5470 RVA: 0x000A9694 File Offset: 0x000A7894
	public override void ResetState()
	{
		base.ResetState();
		this.linePositions = null;
	}
}
