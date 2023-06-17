using System;
using Facepunch;
using ProtoBuf;
using UnityEngine;

// Token: 0x02000411 RID: 1041
public class EnvSync : PointEntity
{
	// Token: 0x04001B3C RID: 6972
	private const float syncInterval = 5f;

	// Token: 0x04001B3D RID: 6973
	private const float syncIntervalInv = 0.2f;

	// Token: 0x0600232F RID: 9007 RVA: 0x000E0E2E File Offset: 0x000DF02E
	public override void ServerInit()
	{
		base.ServerInit();
		base.InvokeRepeating(new Action(this.UpdateNetwork), 5f, 5f);
	}

	// Token: 0x06002330 RID: 9008 RVA: 0x00007D00 File Offset: 0x00005F00
	private void UpdateNetwork()
	{
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x06002331 RID: 9009 RVA: 0x000E0E54 File Offset: 0x000DF054
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.environment = Pool.Get<ProtoBuf.Environment>();
		if (TOD_Sky.Instance)
		{
			info.msg.environment.dateTime = TOD_Sky.Instance.Cycle.DateTime.ToBinary();
		}
		info.msg.environment.engineTime = Time.realtimeSinceStartup;
	}

	// Token: 0x06002332 RID: 9010 RVA: 0x000E0EC0 File Offset: 0x000DF0C0
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.environment == null)
		{
			return;
		}
		if (!TOD_Sky.Instance)
		{
			return;
		}
		if (base.isServer)
		{
			TOD_Sky.Instance.Cycle.DateTime = DateTime.FromBinary(info.msg.environment.dateTime);
		}
	}
}
