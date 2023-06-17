using System;
using System.Collections.Generic;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x020000AB RID: 171
public class PhotoFrame : StorageContainer, ILOD, IImageReceiver, ISignage, IUGCBrowserEntity
{
	// Token: 0x04000A35 RID: 2613
	public GameObjectRef SignEditorDialog;

	// Token: 0x04000A36 RID: 2614
	public OverlayMeshPaintableSource PaintableSource;

	// Token: 0x04000A37 RID: 2615
	private const float TextureRequestDistance = 100f;

	// Token: 0x04000A38 RID: 2616
	private EntityRef _photoEntity;

	// Token: 0x04000A39 RID: 2617
	private uint _overlayTextureCrc;

	// Token: 0x04000A3A RID: 2618
	private List<ulong> editHistory = new List<ulong>();

	// Token: 0x06000F91 RID: 3985 RVA: 0x000823B0 File Offset: 0x000805B0
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("PhotoFrame.OnRpcMessage", 0))
		{
			if (rpc == 1455609404U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - LockSign ");
				}
				using (TimeWarning.New("LockSign", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.MaxDistance.Test(1455609404U, "LockSign", this, player, 3f))
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
							this.LockSign(msg2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in LockSign");
					}
				}
				return true;
			}
			if (rpc == 4149904254U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - UnLockSign ");
				}
				using (TimeWarning.New("UnLockSign", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.MaxDistance.Test(4149904254U, "UnLockSign", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage msg3 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.UnLockSign(msg3);
						}
					}
					catch (Exception exception2)
					{
						Debug.LogException(exception2);
						player.Kick("RPC Error in UnLockSign");
					}
				}
				return true;
			}
			if (rpc == 1255380462U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - UpdateSign ");
				}
				using (TimeWarning.New("UpdateSign", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.CallsPerSecond.Test(1255380462U, "UpdateSign", this, player, 3UL))
						{
							return true;
						}
						if (!global::BaseEntity.RPC_Server.MaxDistance.Test(1255380462U, "UpdateSign", this, player, 5f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage msg4 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.UpdateSign(msg4);
						}
					}
					catch (Exception exception3)
					{
						Debug.LogException(exception3);
						player.Kick("RPC Error in UpdateSign");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x17000173 RID: 371
	// (get) Token: 0x06000F92 RID: 3986 RVA: 0x00082828 File Offset: 0x00080A28
	public Vector2i TextureSize
	{
		get
		{
			return new Vector2i(this.PaintableSource.texWidth, this.PaintableSource.texHeight);
		}
	}

	// Token: 0x17000174 RID: 372
	// (get) Token: 0x06000F93 RID: 3987 RVA: 0x0000441C File Offset: 0x0000261C
	public int TextureCount
	{
		get
		{
			return 1;
		}
	}

	// Token: 0x06000F94 RID: 3988 RVA: 0x000588A7 File Offset: 0x00056AA7
	public bool CanUpdateSign(global::BasePlayer player)
	{
		return player.IsAdmin || player.IsDeveloper || (player.CanBuild() && (!base.IsLocked() || player.userID == base.OwnerID));
	}

	// Token: 0x06000F95 RID: 3989 RVA: 0x00082845 File Offset: 0x00080A45
	public bool CanUnlockSign(global::BasePlayer player)
	{
		return base.IsLocked() && this.CanUpdateSign(player);
	}

	// Token: 0x06000F96 RID: 3990 RVA: 0x00082858 File Offset: 0x00080A58
	public bool CanLockSign(global::BasePlayer player)
	{
		return !base.IsLocked() && this.CanUpdateSign(player);
	}

	// Token: 0x06000F97 RID: 3991 RVA: 0x0008286C File Offset: 0x00080A6C
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.MaxDistance(5f)]
	[global::BaseEntity.RPC_Server.CallsPerSecond(3UL)]
	public void UpdateSign(global::BaseEntity.RPCMessage msg)
	{
		if (msg.player == null)
		{
			return;
		}
		if (!this.CanUpdateSign(msg.player))
		{
			return;
		}
		byte[] array = msg.read.BytesWithSize(10485760U);
		if (array == null)
		{
			return;
		}
		if (!ImageProcessing.IsValidPNG(array, 1024, 1024))
		{
			return;
		}
		FileStorage.server.RemoveAllByEntity(this.net.ID);
		this._overlayTextureCrc = FileStorage.server.Store(array, FileStorage.Type.png, this.net.ID, 0U);
		this.LogEdit(msg.player);
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x06000F98 RID: 3992 RVA: 0x00082908 File Offset: 0x00080B08
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.MaxDistance(3f)]
	public void LockSign(global::BaseEntity.RPCMessage msg)
	{
		if (!msg.player.CanInteract())
		{
			return;
		}
		if (!this.CanUpdateSign(msg.player))
		{
			return;
		}
		base.SetFlag(global::BaseEntity.Flags.Locked, true, false, true);
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
		base.OwnerID = msg.player.userID;
	}

	// Token: 0x06000F99 RID: 3993 RVA: 0x00082955 File Offset: 0x00080B55
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.MaxDistance(3f)]
	public void UnLockSign(global::BaseEntity.RPCMessage msg)
	{
		if (!msg.player.CanInteract())
		{
			return;
		}
		if (!this.CanUnlockSign(msg.player))
		{
			return;
		}
		base.SetFlag(global::BaseEntity.Flags.Locked, false, false, true);
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x06000F9A RID: 3994 RVA: 0x00082986 File Offset: 0x00080B86
	public override void OnKilled(HitInfo info)
	{
		if (this.net != null)
		{
			FileStorage.server.RemoveAllByEntity(this.net.ID);
		}
		this._overlayTextureCrc = 0U;
		base.OnKilled(info);
	}

	// Token: 0x06000F9B RID: 3995 RVA: 0x0000441C File Offset: 0x0000261C
	public override bool ShouldNetworkOwnerInfo()
	{
		return true;
	}

	// Token: 0x06000F9C RID: 3996 RVA: 0x000829B3 File Offset: 0x00080BB3
	public override string Categorize()
	{
		return "sign";
	}

	// Token: 0x06000F9D RID: 3997 RVA: 0x000829BC File Offset: 0x00080BBC
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.photoFrame != null)
		{
			this._photoEntity.uid = info.msg.photoFrame.photoEntityId;
			this._overlayTextureCrc = info.msg.photoFrame.overlayImageCrc;
		}
		if (base.isServer && info.msg.photoFrame != null)
		{
			if (info.msg.photoFrame.editHistory != null)
			{
				if (this.editHistory == null)
				{
					this.editHistory = Facepunch.Pool.GetList<ulong>();
				}
				this.editHistory.Clear();
				using (List<ulong>.Enumerator enumerator = info.msg.photoFrame.editHistory.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						ulong item = enumerator.Current;
						this.editHistory.Add(item);
					}
					return;
				}
			}
			if (this.editHistory != null)
			{
				Facepunch.Pool.FreeList<ulong>(ref this.editHistory);
			}
		}
	}

	// Token: 0x06000F9E RID: 3998 RVA: 0x00082AC4 File Offset: 0x00080CC4
	public uint[] GetTextureCRCs()
	{
		return new uint[]
		{
			this._overlayTextureCrc
		};
	}

	// Token: 0x17000175 RID: 373
	// (get) Token: 0x06000F9F RID: 3999 RVA: 0x00050EF0 File Offset: 0x0004F0F0
	public NetworkableId NetworkID
	{
		get
		{
			return this.net.ID;
		}
	}

	// Token: 0x17000176 RID: 374
	// (get) Token: 0x06000FA0 RID: 4000 RVA: 0x00007A3C File Offset: 0x00005C3C
	public FileStorage.Type FileType
	{
		get
		{
			return FileStorage.Type.png;
		}
	}

	// Token: 0x17000177 RID: 375
	// (get) Token: 0x06000FA1 RID: 4001 RVA: 0x0000441C File Offset: 0x0000261C
	public UGCType ContentType
	{
		get
		{
			return UGCType.ImagePng;
		}
	}

	// Token: 0x06000FA2 RID: 4002 RVA: 0x00082AD8 File Offset: 0x00080CD8
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.photoFrame = Facepunch.Pool.Get<ProtoBuf.PhotoFrame>();
		info.msg.photoFrame.photoEntityId = this._photoEntity.uid;
		info.msg.photoFrame.overlayImageCrc = this._overlayTextureCrc;
		if (this.editHistory.Count > 0)
		{
			info.msg.photoFrame.editHistory = Facepunch.Pool.GetList<ulong>();
			foreach (ulong item in this.editHistory)
			{
				info.msg.photoFrame.editHistory.Add(item);
			}
		}
	}

	// Token: 0x06000FA3 RID: 4003 RVA: 0x00082BA8 File Offset: 0x00080DA8
	public override void OnItemAddedOrRemoved(global::Item item, bool added)
	{
		base.OnItemAddedOrRemoved(item, added);
		global::Item item2 = (base.inventory.itemList.Count > 0) ? base.inventory.itemList[0] : null;
		NetworkableId networkableId = (item2 != null && item2.IsValid()) ? item2.instanceData.subEntity : default(NetworkableId);
		if (networkableId != this._photoEntity.uid)
		{
			this._photoEntity.uid = networkableId;
			base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
		}
	}

	// Token: 0x06000FA4 RID: 4004 RVA: 0x00082C30 File Offset: 0x00080E30
	public override void OnPickedUpPreItemMove(global::Item createdItem, global::BasePlayer player)
	{
		base.OnPickedUpPreItemMove(createdItem, player);
		ItemModSign itemModSign;
		if (this._overlayTextureCrc > 0U && createdItem.info.TryGetComponent<ItemModSign>(out itemModSign))
		{
			itemModSign.OnSignPickedUp(this, this, createdItem);
		}
	}

	// Token: 0x06000FA5 RID: 4005 RVA: 0x00082C68 File Offset: 0x00080E68
	public override void OnDeployed(global::BaseEntity parent, global::BasePlayer deployedBy, global::Item fromItem)
	{
		base.OnDeployed(parent, deployedBy, fromItem);
		ItemModSign itemModSign;
		if (fromItem.info.TryGetComponent<ItemModSign>(out itemModSign))
		{
			SignContent associatedEntity = ItemModAssociatedEntity<SignContent>.GetAssociatedEntity(fromItem, true);
			if (associatedEntity != null)
			{
				associatedEntity.CopyInfoToSign(this, this);
			}
		}
	}

	// Token: 0x06000FA6 RID: 4006 RVA: 0x00082CA6 File Offset: 0x00080EA6
	public void SetTextureCRCs(uint[] crcs)
	{
		if (crcs.Length != 0)
		{
			this._overlayTextureCrc = crcs[0];
			base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
		}
	}

	// Token: 0x17000178 RID: 376
	// (get) Token: 0x06000FA7 RID: 4007 RVA: 0x00082CBC File Offset: 0x00080EBC
	public List<ulong> EditingHistory
	{
		get
		{
			return this.editHistory;
		}
	}

	// Token: 0x06000FA8 RID: 4008 RVA: 0x00082CC4 File Offset: 0x00080EC4
	private void LogEdit(global::BasePlayer byPlayer)
	{
		if (this.editHistory.Contains(byPlayer.userID))
		{
			return;
		}
		this.editHistory.Insert(0, byPlayer.userID);
		int num = 0;
		while (this.editHistory.Count > 5 && num < 10)
		{
			this.editHistory.RemoveAt(5);
			num++;
		}
	}

	// Token: 0x17000179 RID: 377
	// (get) Token: 0x06000FA9 RID: 4009 RVA: 0x00082AC4 File Offset: 0x00080CC4
	public uint[] GetContentCRCs
	{
		get
		{
			return new uint[]
			{
				this._overlayTextureCrc
			};
		}
	}

	// Token: 0x06000FAA RID: 4010 RVA: 0x00082D1E File Offset: 0x00080F1E
	public void ClearContent()
	{
		this._overlayTextureCrc = 0U;
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x1700017A RID: 378
	// (get) Token: 0x06000FAB RID: 4011 RVA: 0x000037E7 File Offset: 0x000019E7
	public global::BaseNetworkable UgcEntity
	{
		get
		{
			return this;
		}
	}

	// Token: 0x06000FAC RID: 4012 RVA: 0x00082D30 File Offset: 0x00080F30
	public override bool CanPickup(global::BasePlayer player)
	{
		return base.CanPickup(player) && !this._photoEntity.uid.IsValid;
	}
}
