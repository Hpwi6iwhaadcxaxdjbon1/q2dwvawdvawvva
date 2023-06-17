using System;
using Network;
using ProtoBuf;
using UnityEngine;

// Token: 0x02000098 RID: 152
public class MapMarkerGenericRadius : MapMarker
{
	// Token: 0x04000907 RID: 2311
	public float radius;

	// Token: 0x04000908 RID: 2312
	public Color color1;

	// Token: 0x04000909 RID: 2313
	public Color color2;

	// Token: 0x0400090A RID: 2314
	public float alpha;

	// Token: 0x06000DEC RID: 3564 RVA: 0x00075C84 File Offset: 0x00073E84
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("MapMarkerGenericRadius.OnRpcMessage", 0))
		{
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000DED RID: 3565 RVA: 0x00075CC4 File Offset: 0x00073EC4
	public void SendUpdate(bool fullUpdate = true)
	{
		float a = this.color1.a;
		Vector3 arg = new Vector3(this.color1.r, this.color1.g, this.color1.b);
		Vector3 arg2 = new Vector3(this.color2.r, this.color2.g, this.color2.b);
		base.ClientRPC<Vector3, float, Vector3, float, float>(null, "MarkerUpdate", arg, a, arg2, this.alpha, this.radius);
	}

	// Token: 0x06000DEE RID: 3566 RVA: 0x00075D48 File Offset: 0x00073F48
	public override AppMarker GetAppMarkerData()
	{
		AppMarker appMarkerData = base.GetAppMarkerData();
		appMarkerData.radius = this.radius;
		appMarkerData.color1 = this.color1;
		appMarkerData.color2 = this.color2;
		appMarkerData.alpha = this.alpha;
		return appMarkerData;
	}
}
