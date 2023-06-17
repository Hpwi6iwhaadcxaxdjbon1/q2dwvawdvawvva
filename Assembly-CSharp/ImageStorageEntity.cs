using System;
using System.Collections.Generic;
using ConVar;
using Network;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x02000083 RID: 131
public class ImageStorageEntity : BaseEntity
{
	// Token: 0x040007FF RID: 2047
	private List<ImageStorageEntity.ImageRequest> _requests;

	// Token: 0x06000C5E RID: 3166 RVA: 0x0006B20C File Offset: 0x0006940C
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("ImageStorageEntity.OnRpcMessage", 0))
		{
			if (rpc == 652912521U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - ImageRequested ");
				}
				using (TimeWarning.New("ImageRequested", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.CallsPerSecond.Test(652912521U, "ImageRequested", this, player, 3UL))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							BaseEntity.RPCMessage msg2 = new BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.ImageRequested(msg2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in ImageRequested");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x17000126 RID: 294
	// (get) Token: 0x06000C5F RID: 3167 RVA: 0x0000441C File Offset: 0x0000261C
	protected virtual FileStorage.Type StorageType
	{
		get
		{
			return FileStorage.Type.jpg;
		}
	}

	// Token: 0x17000127 RID: 295
	// (get) Token: 0x06000C60 RID: 3168 RVA: 0x00007A3C File Offset: 0x00005C3C
	protected virtual uint CrcToLoad
	{
		get
		{
			return 0U;
		}
	}

	// Token: 0x06000C61 RID: 3169 RVA: 0x0006B374 File Offset: 0x00069574
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.CallsPerSecond(3UL)]
	private void ImageRequested(BaseEntity.RPCMessage msg)
	{
		if (msg.player == null)
		{
			return;
		}
		byte[] array = FileStorage.server.Get(this.CrcToLoad, this.StorageType, this.net.ID, 0U);
		if (array == null)
		{
			Debug.LogWarning("Image entity has no image!");
			return;
		}
		SendInfo sendInfo = new SendInfo(msg.connection)
		{
			method = SendMethod.Reliable,
			channel = 2
		};
		base.ClientRPCEx<uint, byte[]>(sendInfo, null, "ReceiveImage", (uint)array.Length, array);
	}

	// Token: 0x02000BD2 RID: 3026
	private struct ImageRequest
	{
		// Token: 0x040040F3 RID: 16627
		public IImageReceiver Receiver;

		// Token: 0x040040F4 RID: 16628
		public float Time;
	}
}
