using System;
using System.Collections.Generic;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x02000097 RID: 151
public class MapEntity : global::HeldEntity
{
	// Token: 0x04000905 RID: 2309
	[NonSerialized]
	public uint[] fogImages = new uint[1];

	// Token: 0x04000906 RID: 2310
	[NonSerialized]
	public uint[] paintImages = new uint[144];

	// Token: 0x06000DE7 RID: 3559 RVA: 0x000758B4 File Offset: 0x00073AB4
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("MapEntity.OnRpcMessage", 0))
		{
			if (rpc == 1443560440U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - ImageUpdate ");
				}
				using (TimeWarning.New("ImageUpdate", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.CallsPerSecond.Test(1443560440U, "ImageUpdate", this, player, 1UL))
						{
							return true;
						}
						if (!global::BaseEntity.RPC_Server.FromOwner.Test(1443560440U, "ImageUpdate", this, player))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage msg2 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.ImageUpdate(msg2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in ImageUpdate");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000DE8 RID: 3560 RVA: 0x00075A70 File Offset: 0x00073C70
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.mapEntity != null)
		{
			if (info.msg.mapEntity.fogImages.Count == this.fogImages.Length)
			{
				this.fogImages = info.msg.mapEntity.fogImages.ToArray();
			}
			if (info.msg.mapEntity.paintImages.Count == this.paintImages.Length)
			{
				this.paintImages = info.msg.mapEntity.paintImages.ToArray();
			}
		}
	}

	// Token: 0x06000DE9 RID: 3561 RVA: 0x00075B08 File Offset: 0x00073D08
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.mapEntity = Facepunch.Pool.Get<ProtoBuf.MapEntity>();
		info.msg.mapEntity.fogImages = Facepunch.Pool.Get<List<uint>>();
		info.msg.mapEntity.fogImages.AddRange(this.fogImages);
		info.msg.mapEntity.paintImages = Facepunch.Pool.Get<List<uint>>();
		info.msg.mapEntity.paintImages.AddRange(this.paintImages);
	}

	// Token: 0x06000DEA RID: 3562 RVA: 0x00075B8C File Offset: 0x00073D8C
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.CallsPerSecond(1UL)]
	[global::BaseEntity.RPC_Server.FromOwner]
	public void ImageUpdate(global::BaseEntity.RPCMessage msg)
	{
		if (msg.player == null)
		{
			return;
		}
		byte b = msg.read.UInt8();
		byte b2 = msg.read.UInt8();
		uint num = msg.read.UInt32();
		if (b == 0 && this.fogImages[(int)b2] == num)
		{
			return;
		}
		if (b == 1 && this.paintImages[(int)b2] == num)
		{
			return;
		}
		uint num2 = (uint)b * 1000U + (uint)b2;
		byte[] array = msg.read.BytesWithSize(10485760U);
		if (array == null)
		{
			return;
		}
		FileStorage.server.RemoveEntityNum(this.net.ID, num2);
		uint num3 = FileStorage.server.Store(array, FileStorage.Type.png, this.net.ID, num2);
		if (b == 0)
		{
			this.fogImages[(int)b2] = num3;
		}
		if (b == 1)
		{
			this.paintImages[(int)b2] = num3;
		}
		base.InvalidateNetworkCache();
	}
}
