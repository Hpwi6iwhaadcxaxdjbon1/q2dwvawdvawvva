using System;
using System.Collections.Generic;
using CompanionServer;
using Facepunch;
using ProtoBuf;
using UnityEngine;

// Token: 0x02000418 RID: 1048
public class MapMarker : global::BaseEntity
{
	// Token: 0x04001B91 RID: 7057
	public AppMarkerType appType;

	// Token: 0x04001B92 RID: 7058
	public GameObjectRef markerObj;

	// Token: 0x04001B93 RID: 7059
	public static readonly List<MapMarker> serverMapMarkers = new List<MapMarker>();

	// Token: 0x0600235F RID: 9055 RVA: 0x000E2433 File Offset: 0x000E0633
	public override void InitShared()
	{
		if (base.isServer && !MapMarker.serverMapMarkers.Contains(this))
		{
			MapMarker.serverMapMarkers.Add(this);
		}
		base.InitShared();
	}

	// Token: 0x06002360 RID: 9056 RVA: 0x000E245B File Offset: 0x000E065B
	public override void DestroyShared()
	{
		if (base.isServer)
		{
			MapMarker.serverMapMarkers.Remove(this);
		}
		base.DestroyShared();
	}

	// Token: 0x06002361 RID: 9057 RVA: 0x000E2478 File Offset: 0x000E0678
	public virtual AppMarker GetAppMarkerData()
	{
		AppMarker appMarker = Pool.Get<AppMarker>();
		Vector2 vector = CompanionServer.Util.WorldToMap(base.transform.position);
		appMarker.id = this.net.ID;
		appMarker.type = this.appType;
		appMarker.x = vector.x;
		appMarker.y = vector.y;
		return appMarker;
	}

	// Token: 0x02000CDA RID: 3290
	public enum ClusterType
	{
		// Token: 0x0400452B RID: 17707
		None,
		// Token: 0x0400452C RID: 17708
		Vending
	}
}
