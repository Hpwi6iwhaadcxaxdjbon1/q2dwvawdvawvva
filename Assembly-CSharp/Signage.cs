using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x020000CA RID: 202
public class Signage : global::IOEntity, ILOD, ISignage, IUGCBrowserEntity
{
	// Token: 0x04000B48 RID: 2888
	private const float TextureRequestTimeout = 15f;

	// Token: 0x04000B49 RID: 2889
	public GameObjectRef changeTextDialog;

	// Token: 0x04000B4A RID: 2890
	public MeshPaintableSource[] paintableSources;

	// Token: 0x04000B4B RID: 2891
	[NonSerialized]
	public uint[] textureIDs;

	// Token: 0x04000B4C RID: 2892
	public ItemDefinition RequiredHeldEntity;

	// Token: 0x04000B4D RID: 2893
	private List<ulong> editHistory = new List<ulong>();

	// Token: 0x0600120B RID: 4619 RVA: 0x000920E8 File Offset: 0x000902E8
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("Signage.OnRpcMessage", 0))
		{
			if (rpc == 1455609404U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					UnityEngine.Debug.Log("SV_RPCMessage: " + player + " - LockSign ");
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
						UnityEngine.Debug.LogException(exception);
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
					UnityEngine.Debug.Log("SV_RPCMessage: " + player + " - UnLockSign ");
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
						UnityEngine.Debug.LogException(exception2);
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
					UnityEngine.Debug.Log("SV_RPCMessage: " + player + " - UpdateSign ");
				}
				using (TimeWarning.New("UpdateSign", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.CallsPerSecond.Test(1255380462U, "UpdateSign", this, player, 5UL))
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
						UnityEngine.Debug.LogException(exception3);
						player.Kick("RPC Error in UpdateSign");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x1700019E RID: 414
	// (get) Token: 0x0600120C RID: 4620 RVA: 0x00092560 File Offset: 0x00090760
	public Vector2i TextureSize
	{
		get
		{
			if (this.paintableSources == null || this.paintableSources.Length == 0)
			{
				return Vector2i.zero;
			}
			MeshPaintableSource meshPaintableSource = this.paintableSources[0];
			return new Vector2i(meshPaintableSource.texWidth, meshPaintableSource.texHeight);
		}
	}

	// Token: 0x1700019F RID: 415
	// (get) Token: 0x0600120D RID: 4621 RVA: 0x0009259E File Offset: 0x0009079E
	public int TextureCount
	{
		get
		{
			MeshPaintableSource[] array = this.paintableSources;
			if (array == null)
			{
				return 0;
			}
			return array.Length;
		}
	}

	// Token: 0x0600120E RID: 4622 RVA: 0x000925B0 File Offset: 0x000907B0
	public override void PreProcess(IPrefabProcessor preProcess, GameObject rootObj, string name, bool serverside, bool clientside, bool bundling)
	{
		base.PreProcess(preProcess, rootObj, name, serverside, clientside, bundling);
		if (this.paintableSources != null && this.paintableSources.Length > 1)
		{
			MeshPaintableSource meshPaintableSource = this.paintableSources[0];
			for (int i = 1; i < this.paintableSources.Length; i++)
			{
				MeshPaintableSource meshPaintableSource2 = this.paintableSources[i];
				meshPaintableSource2.texWidth = meshPaintableSource.texWidth;
				meshPaintableSource2.texHeight = meshPaintableSource.texHeight;
			}
		}
	}

	// Token: 0x0600120F RID: 4623 RVA: 0x0009261C File Offset: 0x0009081C
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.CallsPerSecond(5UL)]
	[global::BaseEntity.RPC_Server.MaxDistance(5f)]
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
		int num = msg.read.Int32();
		if (num < 0 || num >= this.paintableSources.Length)
		{
			return;
		}
		byte[] array = msg.read.BytesWithSize(10485760U);
		if (msg.read.Unread > 0 && msg.read.Bit() && !msg.player.IsAdmin)
		{
			UnityEngine.Debug.LogWarning(string.Format("{0} tried to upload a sign from a file but they aren't admin, ignoring", msg.player));
			return;
		}
		this.EnsureInitialized();
		if (array == null)
		{
			if (this.textureIDs[num] != 0U)
			{
				FileStorage.server.RemoveExact(this.textureIDs[num], FileStorage.Type.png, this.net.ID, (uint)num);
			}
			this.textureIDs[num] = 0U;
		}
		else
		{
			if (!ImageProcessing.IsValidPNG(array, 1024, 1024))
			{
				return;
			}
			if (this.textureIDs[num] != 0U)
			{
				FileStorage.server.RemoveExact(this.textureIDs[num], FileStorage.Type.png, this.net.ID, (uint)num);
			}
			this.textureIDs[num] = FileStorage.server.Store(array, FileStorage.Type.png, this.net.ID, (uint)num);
		}
		this.LogEdit(msg.player);
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x06001210 RID: 4624 RVA: 0x00092764 File Offset: 0x00090964
	private void EnsureInitialized()
	{
		int num = Mathf.Max(this.paintableSources.Length, 1);
		if (this.textureIDs == null || this.textureIDs.Length != num)
		{
			Array.Resize<uint>(ref this.textureIDs, num);
		}
	}

	// Token: 0x06001211 RID: 4625 RVA: 0x00058897 File Offset: 0x00056A97
	[Conditional("SIGN_DEBUG")]
	private static void SignDebugLog(string str)
	{
		UnityEngine.Debug.Log(str);
	}

	// Token: 0x06001212 RID: 4626 RVA: 0x000927A0 File Offset: 0x000909A0
	public virtual bool CanUpdateSign(global::BasePlayer player)
	{
		if (player.IsAdmin || player.IsDeveloper)
		{
			return true;
		}
		if (!player.CanBuild())
		{
			return false;
		}
		if (base.IsLocked())
		{
			return player.userID == base.OwnerID;
		}
		return this.HeldEntityCheck(player);
	}

	// Token: 0x06001213 RID: 4627 RVA: 0x000927EC File Offset: 0x000909EC
	public bool CanUnlockSign(global::BasePlayer player)
	{
		return base.IsLocked() && this.HeldEntityCheck(player) && this.CanUpdateSign(player);
	}

	// Token: 0x06001214 RID: 4628 RVA: 0x0009280A File Offset: 0x00090A0A
	public bool CanLockSign(global::BasePlayer player)
	{
		return !base.IsLocked() && this.HeldEntityCheck(player) && this.CanUpdateSign(player);
	}

	// Token: 0x06001215 RID: 4629 RVA: 0x00092828 File Offset: 0x00090A28
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		this.EnsureInitialized();
		bool flag = false;
		if (info.msg.sign != null)
		{
			uint num = this.textureIDs[0];
			if (info.msg.sign.imageIds != null && info.msg.sign.imageIds.Count > 0)
			{
				int num2 = Mathf.Min(info.msg.sign.imageIds.Count, this.textureIDs.Length);
				for (int i = 0; i < num2; i++)
				{
					uint num3 = info.msg.sign.imageIds[i];
					bool flag2 = num3 != this.textureIDs[i];
					flag = (flag || flag2);
					this.textureIDs[i] = num3;
				}
			}
			else
			{
				flag = (num != info.msg.sign.imageid);
				this.textureIDs[0] = info.msg.sign.imageid;
			}
		}
		if (base.isServer)
		{
			bool flag3 = false;
			for (int j = 0; j < this.paintableSources.Length; j++)
			{
				uint num4 = this.textureIDs[j];
				if (num4 != 0U)
				{
					byte[] array = FileStorage.server.Get(num4, FileStorage.Type.png, this.net.ID, (uint)j);
					if (array == null)
					{
						base.Log(string.Format("Frame {0} (id={1}) doesn't exist, clearing", j, num4));
						this.textureIDs[j] = 0U;
					}
					flag3 = (flag3 || array != null);
				}
			}
			if (!flag3)
			{
				base.SetFlag(global::BaseEntity.Flags.Locked, false, false, true);
			}
			if (info.msg.sign != null)
			{
				if (info.msg.sign.editHistory != null)
				{
					if (this.editHistory == null)
					{
						this.editHistory = Facepunch.Pool.GetList<ulong>();
					}
					this.editHistory.Clear();
					using (List<ulong>.Enumerator enumerator = info.msg.sign.editHistory.GetEnumerator())
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
	}

	// Token: 0x06001216 RID: 4630 RVA: 0x00092A68 File Offset: 0x00090C68
	private bool HeldEntityCheck(global::BasePlayer player)
	{
		return !(this.RequiredHeldEntity != null) || (player.GetHeldEntity() && !(player.GetHeldEntity().GetItem().info != this.RequiredHeldEntity));
	}

	// Token: 0x06001217 RID: 4631 RVA: 0x00092AA5 File Offset: 0x00090CA5
	public uint[] GetTextureCRCs()
	{
		return this.textureIDs;
	}

	// Token: 0x170001A0 RID: 416
	// (get) Token: 0x06001218 RID: 4632 RVA: 0x00050EF0 File Offset: 0x0004F0F0
	public NetworkableId NetworkID
	{
		get
		{
			return this.net.ID;
		}
	}

	// Token: 0x170001A1 RID: 417
	// (get) Token: 0x06001219 RID: 4633 RVA: 0x00007A3C File Offset: 0x00005C3C
	public FileStorage.Type FileType
	{
		get
		{
			return FileStorage.Type.png;
		}
	}

	// Token: 0x170001A2 RID: 418
	// (get) Token: 0x0600121A RID: 4634 RVA: 0x0000441C File Offset: 0x0000261C
	public UGCType ContentType
	{
		get
		{
			return UGCType.ImagePng;
		}
	}

	// Token: 0x0600121B RID: 4635 RVA: 0x00092AB0 File Offset: 0x00090CB0
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

	// Token: 0x0600121C RID: 4636 RVA: 0x00092AFD File Offset: 0x00090CFD
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

	// Token: 0x0600121D RID: 4637 RVA: 0x00092B30 File Offset: 0x00090D30
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		this.EnsureInitialized();
		List<uint> list = Facepunch.Pool.GetList<uint>();
		foreach (uint item in this.textureIDs)
		{
			list.Add(item);
		}
		info.msg.sign = Facepunch.Pool.Get<Sign>();
		info.msg.sign.imageid = 0U;
		info.msg.sign.imageIds = list;
		if (this.editHistory != null && this.editHistory.Count > 0)
		{
			info.msg.sign.editHistory = Facepunch.Pool.GetList<ulong>();
			foreach (ulong item2 in this.editHistory)
			{
				info.msg.sign.editHistory.Add(item2);
			}
		}
	}

	// Token: 0x0600121E RID: 4638 RVA: 0x00092C28 File Offset: 0x00090E28
	public override void OnKilled(HitInfo info)
	{
		if (this.net != null)
		{
			FileStorage.server.RemoveAllByEntity(this.net.ID);
		}
		if (this.textureIDs != null)
		{
			Array.Clear(this.textureIDs, 0, this.textureIDs.Length);
		}
		base.OnKilled(info);
	}

	// Token: 0x0600121F RID: 4639 RVA: 0x00092C78 File Offset: 0x00090E78
	public override void OnPickedUpPreItemMove(global::Item createdItem, global::BasePlayer player)
	{
		base.OnPickedUpPreItemMove(createdItem, player);
		bool flag = false;
		uint[] array = this.textureIDs;
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i] != 0U)
			{
				flag = true;
				break;
			}
		}
		ItemModSign itemModSign;
		if (flag && createdItem.info.TryGetComponent<ItemModSign>(out itemModSign))
		{
			itemModSign.OnSignPickedUp(this, this, createdItem);
		}
	}

	// Token: 0x06001220 RID: 4640 RVA: 0x00092CC8 File Offset: 0x00090EC8
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

	// Token: 0x06001221 RID: 4641 RVA: 0x0000441C File Offset: 0x0000261C
	public override bool ShouldNetworkOwnerInfo()
	{
		return true;
	}

	// Token: 0x06001222 RID: 4642 RVA: 0x00092D06 File Offset: 0x00090F06
	public void SetTextureCRCs(uint[] crcs)
	{
		this.textureIDs = new uint[crcs.Length];
		crcs.CopyTo(this.textureIDs, 0);
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x170001A3 RID: 419
	// (get) Token: 0x06001223 RID: 4643 RVA: 0x00092D2A File Offset: 0x00090F2A
	public List<ulong> EditingHistory
	{
		get
		{
			return this.editHistory;
		}
	}

	// Token: 0x170001A4 RID: 420
	// (get) Token: 0x06001224 RID: 4644 RVA: 0x000037E7 File Offset: 0x000019E7
	public global::BaseNetworkable UgcEntity
	{
		get
		{
			return this;
		}
	}

	// Token: 0x06001225 RID: 4645 RVA: 0x00092D34 File Offset: 0x00090F34
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

	// Token: 0x170001A5 RID: 421
	// (get) Token: 0x06001226 RID: 4646 RVA: 0x00092D8E File Offset: 0x00090F8E
	public uint[] GetContentCRCs
	{
		get
		{
			return this.GetTextureCRCs();
		}
	}

	// Token: 0x06001227 RID: 4647 RVA: 0x00092D96 File Offset: 0x00090F96
	public void ClearContent()
	{
		this.SetTextureCRCs(Array.Empty<uint>());
	}

	// Token: 0x06001228 RID: 4648 RVA: 0x00092DA4 File Offset: 0x00090FA4
	public override string Admin_Who()
	{
		if (this.editHistory == null || this.editHistory.Count == 0)
		{
			return base.Admin_Who();
		}
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine(base.Admin_Who());
		for (int i = 0; i < this.editHistory.Count; i++)
		{
			stringBuilder.AppendLine(string.Format("Edit {0}: {1}", i, this.editHistory[i]));
		}
		return stringBuilder.ToString();
	}

	// Token: 0x06001229 RID: 4649 RVA: 0x00007A3C File Offset: 0x00005C3C
	public override int ConsumptionAmount()
	{
		return 0;
	}

	// Token: 0x0600122A RID: 4650 RVA: 0x000829B3 File Offset: 0x00080BB3
	public override string Categorize()
	{
		return "sign";
	}
}
