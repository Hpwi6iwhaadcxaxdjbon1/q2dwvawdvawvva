using System;
using System.Collections.Generic;
using System.Diagnostics;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x02000055 RID: 85
public class CarvablePumpkin : global::BaseOven, ILOD, ISignage, IUGCBrowserEntity
{
	// Token: 0x04000626 RID: 1574
	private const float TextureRequestTimeout = 15f;

	// Token: 0x04000627 RID: 1575
	public GameObjectRef changeTextDialog;

	// Token: 0x04000628 RID: 1576
	public MeshPaintableSource[] paintableSources;

	// Token: 0x04000629 RID: 1577
	[NonSerialized]
	public uint[] textureIDs;

	// Token: 0x0400062A RID: 1578
	private List<ulong> editHistory = new List<ulong>();

	// Token: 0x0600093D RID: 2365 RVA: 0x000581E0 File Offset: 0x000563E0
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("CarvablePumpkin.OnRpcMessage", 0))
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

	// Token: 0x170000EE RID: 238
	// (get) Token: 0x0600093E RID: 2366 RVA: 0x00058658 File Offset: 0x00056858
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

	// Token: 0x170000EF RID: 239
	// (get) Token: 0x0600093F RID: 2367 RVA: 0x00058696 File Offset: 0x00056896
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

	// Token: 0x06000940 RID: 2368 RVA: 0x000586A8 File Offset: 0x000568A8
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

	// Token: 0x06000941 RID: 2369 RVA: 0x00058714 File Offset: 0x00056914
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

	// Token: 0x06000942 RID: 2370 RVA: 0x0005885C File Offset: 0x00056A5C
	private void EnsureInitialized()
	{
		int num = Mathf.Max(this.paintableSources.Length, 1);
		if (this.textureIDs == null || this.textureIDs.Length != num)
		{
			Array.Resize<uint>(ref this.textureIDs, num);
		}
	}

	// Token: 0x06000943 RID: 2371 RVA: 0x00058897 File Offset: 0x00056A97
	[Conditional("SIGN_DEBUG")]
	private static void SignDebugLog(string str)
	{
		UnityEngine.Debug.Log(str);
	}

	// Token: 0x170000F0 RID: 240
	// (get) Token: 0x06000944 RID: 2372 RVA: 0x00007A3C File Offset: 0x00005C3C
	public FileStorage.Type FileType
	{
		get
		{
			return FileStorage.Type.png;
		}
	}

	// Token: 0x06000945 RID: 2373 RVA: 0x0005889F File Offset: 0x00056A9F
	public uint[] GetTextureCRCs()
	{
		return this.textureIDs;
	}

	// Token: 0x170000F1 RID: 241
	// (get) Token: 0x06000946 RID: 2374 RVA: 0x00050EF0 File Offset: 0x0004F0F0
	public NetworkableId NetworkID
	{
		get
		{
			return this.net.ID;
		}
	}

	// Token: 0x06000947 RID: 2375 RVA: 0x000588A7 File Offset: 0x00056AA7
	public virtual bool CanUpdateSign(global::BasePlayer player)
	{
		return player.IsAdmin || player.IsDeveloper || (player.CanBuild() && (!base.IsLocked() || player.userID == base.OwnerID));
	}

	// Token: 0x06000948 RID: 2376 RVA: 0x000588DD File Offset: 0x00056ADD
	public bool CanUnlockSign(global::BasePlayer player)
	{
		return base.IsLocked() && this.CanUpdateSign(player);
	}

	// Token: 0x06000949 RID: 2377 RVA: 0x000588F0 File Offset: 0x00056AF0
	public bool CanLockSign(global::BasePlayer player)
	{
		return !base.IsLocked() && this.CanUpdateSign(player);
	}

	// Token: 0x0600094A RID: 2378 RVA: 0x00058904 File Offset: 0x00056B04
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

	// Token: 0x170000F2 RID: 242
	// (get) Token: 0x0600094B RID: 2379 RVA: 0x0000441C File Offset: 0x0000261C
	public UGCType ContentType
	{
		get
		{
			return UGCType.ImagePng;
		}
	}

	// Token: 0x0600094C RID: 2380 RVA: 0x00058B44 File Offset: 0x00056D44
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

	// Token: 0x0600094D RID: 2381 RVA: 0x00058B91 File Offset: 0x00056D91
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

	// Token: 0x0600094E RID: 2382 RVA: 0x00058BC4 File Offset: 0x00056DC4
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
		if (this.editHistory.Count > 0)
		{
			info.msg.sign.editHistory = Facepunch.Pool.GetList<ulong>();
			foreach (ulong item2 in this.editHistory)
			{
				info.msg.sign.editHistory.Add(item2);
			}
		}
	}

	// Token: 0x0600094F RID: 2383 RVA: 0x00058CB4 File Offset: 0x00056EB4
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

	// Token: 0x06000950 RID: 2384 RVA: 0x00058D04 File Offset: 0x00056F04
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

	// Token: 0x06000951 RID: 2385 RVA: 0x00058D54 File Offset: 0x00056F54
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

	// Token: 0x06000952 RID: 2386 RVA: 0x0000441C File Offset: 0x0000261C
	public override bool ShouldNetworkOwnerInfo()
	{
		return true;
	}

	// Token: 0x06000953 RID: 2387 RVA: 0x00058D92 File Offset: 0x00056F92
	public void SetTextureCRCs(uint[] crcs)
	{
		this.textureIDs = new uint[crcs.Length];
		crcs.CopyTo(this.textureIDs, 0);
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x170000F3 RID: 243
	// (get) Token: 0x06000954 RID: 2388 RVA: 0x00058DB6 File Offset: 0x00056FB6
	public List<ulong> EditingHistory
	{
		get
		{
			return this.editHistory;
		}
	}

	// Token: 0x06000955 RID: 2389 RVA: 0x00058DC0 File Offset: 0x00056FC0
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

	// Token: 0x170000F4 RID: 244
	// (get) Token: 0x06000956 RID: 2390 RVA: 0x0005889F File Offset: 0x00056A9F
	public uint[] GetContentCRCs
	{
		get
		{
			return this.textureIDs;
		}
	}

	// Token: 0x06000957 RID: 2391 RVA: 0x00058E1A File Offset: 0x0005701A
	public void ClearContent()
	{
		this.SetTextureCRCs(Array.Empty<uint>());
	}

	// Token: 0x170000F5 RID: 245
	// (get) Token: 0x06000958 RID: 2392 RVA: 0x000037E7 File Offset: 0x000019E7
	public global::BaseNetworkable UgcEntity
	{
		get
		{
			return this;
		}
	}

	// Token: 0x06000959 RID: 2393 RVA: 0x00058E27 File Offset: 0x00057027
	public override string Categorize()
	{
		return "sign";
	}
}
