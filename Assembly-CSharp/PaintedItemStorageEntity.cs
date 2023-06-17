using System;
using System.Collections.Generic;
using System.Diagnostics;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using Rust;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x020000A9 RID: 169
public class PaintedItemStorageEntity : global::BaseEntity, IServerFileReceiver, IUGCBrowserEntity
{
	// Token: 0x04000A2C RID: 2604
	private uint _currentImageCrc;

	// Token: 0x04000A2D RID: 2605
	private ulong lastEditedBy;

	// Token: 0x06000F7C RID: 3964 RVA: 0x00081D2C File Offset: 0x0007FF2C
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("PaintedItemStorageEntity.OnRpcMessage", 0))
		{
			if (rpc == 2439017595U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					UnityEngine.Debug.Log("SV_RPCMessage: " + player + " - Server_UpdateImage ");
				}
				using (TimeWarning.New("Server_UpdateImage", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.CallsPerSecond.Test(2439017595U, "Server_UpdateImage", this, player, 3UL))
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
							this.Server_UpdateImage(msg2);
						}
					}
					catch (Exception exception)
					{
						UnityEngine.Debug.LogException(exception);
						player.Kick("RPC Error in Server_UpdateImage");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000F7D RID: 3965 RVA: 0x00081E94 File Offset: 0x00080094
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.paintedItem != null)
		{
			this._currentImageCrc = info.msg.paintedItem.imageCrc;
			if (base.isServer)
			{
				this.lastEditedBy = info.msg.paintedItem.editedBy;
			}
		}
	}

	// Token: 0x06000F7E RID: 3966 RVA: 0x00081EEC File Offset: 0x000800EC
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.paintedItem = Facepunch.Pool.Get<PaintedItem>();
		info.msg.paintedItem.imageCrc = this._currentImageCrc;
		info.msg.paintedItem.editedBy = this.lastEditedBy;
	}

	// Token: 0x06000F7F RID: 3967 RVA: 0x00081F3C File Offset: 0x0008013C
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.CallsPerSecond(3UL)]
	private void Server_UpdateImage(global::BaseEntity.RPCMessage msg)
	{
		if (msg.player == null || msg.player.userID != base.OwnerID)
		{
			return;
		}
		foreach (global::Item item in msg.player.inventory.containerWear.itemList)
		{
			if (item.instanceData != null && item.instanceData.subEntity == this.net.ID)
			{
				return;
			}
		}
		global::Item item2 = msg.player.inventory.FindBySubEntityID(this.net.ID);
		if (item2 == null || item2.isBroken)
		{
			return;
		}
		byte[] array = msg.read.BytesWithSize(10485760U);
		if (array == null)
		{
			if (this._currentImageCrc != 0U)
			{
				FileStorage.server.RemoveExact(this._currentImageCrc, FileStorage.Type.png, this.net.ID, 0U);
			}
			this._currentImageCrc = 0U;
		}
		else
		{
			if (!ImageProcessing.IsValidPNG(array, 512, 512))
			{
				return;
			}
			uint currentImageCrc = this._currentImageCrc;
			if (this._currentImageCrc != 0U)
			{
				FileStorage.server.RemoveExact(this._currentImageCrc, FileStorage.Type.png, this.net.ID, 0U);
			}
			this._currentImageCrc = FileStorage.server.Store(array, FileStorage.Type.png, this.net.ID, 0U);
			if (this._currentImageCrc != currentImageCrc)
			{
				item2.LoseCondition(0.25f);
			}
			this.lastEditedBy = msg.player.userID;
		}
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x06000F80 RID: 3968 RVA: 0x000820DC File Offset: 0x000802DC
	internal override void DoServerDestroy()
	{
		base.DoServerDestroy();
		if (!Rust.Application.isQuitting && this.net != null)
		{
			FileStorage.server.RemoveAllByEntity(this.net.ID);
		}
	}

	// Token: 0x1700016F RID: 367
	// (get) Token: 0x06000F81 RID: 3969 RVA: 0x00082108 File Offset: 0x00080308
	public uint[] GetContentCRCs
	{
		get
		{
			if (this._currentImageCrc <= 0U)
			{
				return Array.Empty<uint>();
			}
			return new uint[]
			{
				this._currentImageCrc
			};
		}
	}

	// Token: 0x06000F82 RID: 3970 RVA: 0x00082128 File Offset: 0x00080328
	public void ClearContent()
	{
		this._currentImageCrc = 0U;
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x17000170 RID: 368
	// (get) Token: 0x06000F83 RID: 3971 RVA: 0x00007A3C File Offset: 0x00005C3C
	public UGCType ContentType
	{
		get
		{
			return UGCType.ImageJpg;
		}
	}

	// Token: 0x17000171 RID: 369
	// (get) Token: 0x06000F84 RID: 3972 RVA: 0x00082138 File Offset: 0x00080338
	public List<ulong> EditingHistory
	{
		get
		{
			if (this.lastEditedBy <= 0UL)
			{
				return new List<ulong>();
			}
			return new List<ulong>
			{
				this.lastEditedBy
			};
		}
	}

	// Token: 0x17000172 RID: 370
	// (get) Token: 0x06000F85 RID: 3973 RVA: 0x000037E7 File Offset: 0x000019E7
	public global::BaseNetworkable UgcEntity
	{
		get
		{
			return this;
		}
	}

	// Token: 0x06000F86 RID: 3974 RVA: 0x0008215B File Offset: 0x0008035B
	[Conditional("PAINTED_ITEM_DEBUG")]
	private void DebugOnlyLog(string str)
	{
		UnityEngine.Debug.Log(str, this);
	}
}
