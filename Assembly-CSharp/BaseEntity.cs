using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using ConVar;
using Facepunch;
using Facepunch.Extend;
using Network;
using ProtoBuf;
using Rust;
using Rust.Ai;
using Rust.Workshop;
using Spatial;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

// Token: 0x0200003A RID: 58
public class BaseEntity : global::BaseNetworkable, IOnParentSpawning, IPrefabPreProcess
{
	// Token: 0x0400024B RID: 587
	private static Queue<global::BaseEntity> globalBroadcastQueue = new Queue<global::BaseEntity>();

	// Token: 0x0400024C RID: 588
	private static uint globalBroadcastProtocol = 0U;

	// Token: 0x0400024D RID: 589
	private uint broadcastProtocol;

	// Token: 0x0400024E RID: 590
	private List<EntityLink> links = new List<EntityLink>();

	// Token: 0x0400024F RID: 591
	private bool linkedToNeighbours;

	// Token: 0x04000250 RID: 592
	[NonSerialized]
	public global::BaseEntity creatorEntity;

	// Token: 0x04000251 RID: 593
	private int ticksSinceStopped;

	// Token: 0x04000252 RID: 594
	private int doneMovingWithoutARigidBodyCheck = 1;

	// Token: 0x04000253 RID: 595
	private bool isCallingUpdateNetworkGroup;

	// Token: 0x04000254 RID: 596
	private EntityRef[] entitySlots = new EntityRef[8];

	// Token: 0x04000255 RID: 597
	protected List<TriggerBase> triggers;

	// Token: 0x04000256 RID: 598
	protected bool isVisible = true;

	// Token: 0x04000257 RID: 599
	protected bool isAnimatorVisible = true;

	// Token: 0x04000258 RID: 600
	protected bool isShadowVisible = true;

	// Token: 0x04000259 RID: 601
	protected OccludeeSphere localOccludee = new OccludeeSphere(-1);

	// Token: 0x0400025B RID: 603
	[Header("BaseEntity")]
	public Bounds bounds;

	// Token: 0x0400025C RID: 604
	public GameObjectRef impactEffect;

	// Token: 0x0400025D RID: 605
	public bool enableSaving = true;

	// Token: 0x0400025E RID: 606
	public bool syncPosition;

	// Token: 0x0400025F RID: 607
	public Model model;

	// Token: 0x04000260 RID: 608
	[global::InspectorFlags]
	public global::BaseEntity.Flags flags;

	// Token: 0x04000261 RID: 609
	[NonSerialized]
	public uint parentBone;

	// Token: 0x04000262 RID: 610
	[NonSerialized]
	public ulong skinID;

	// Token: 0x04000263 RID: 611
	private EntityComponentBase[] _components;

	// Token: 0x04000264 RID: 612
	[HideInInspector]
	public bool HasBrain;

	// Token: 0x04000265 RID: 613
	[NonSerialized]
	protected string _name;

	// Token: 0x04000267 RID: 615
	private global::Spawnable _spawnable;

	// Token: 0x04000268 RID: 616
	public static HashSet<global::BaseEntity> saveList = new HashSet<global::BaseEntity>();

	// Token: 0x06000299 RID: 665 RVA: 0x0002A040 File Offset: 0x00028240
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("BaseEntity.OnRpcMessage", 0))
		{
			if (rpc == 1552640099U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - BroadcastSignalFromClient ");
				}
				using (TimeWarning.New("BroadcastSignalFromClient", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.FromOwner.Test(1552640099U, "BroadcastSignalFromClient", this, player))
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
							this.BroadcastSignalFromClient(msg2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in BroadcastSignalFromClient");
					}
				}
				return true;
			}
			if (rpc == 3645147041U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - SV_RequestFile ");
				}
				using (TimeWarning.New("SV_RequestFile", 0))
				{
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
							this.SV_RequestFile(msg3);
						}
					}
					catch (Exception exception2)
					{
						Debug.LogException(exception2);
						player.Kick("RPC Error in SV_RequestFile");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x17000052 RID: 82
	// (get) Token: 0x0600029A RID: 666 RVA: 0x0002A2EC File Offset: 0x000284EC
	public virtual float RealisticMass
	{
		get
		{
			return 100f;
		}
	}

	// Token: 0x0600029B RID: 667 RVA: 0x0002A2F3 File Offset: 0x000284F3
	public virtual void OnCollision(Collision collision, global::BaseEntity hitEntity)
	{
		throw new NotImplementedException();
	}

	// Token: 0x0600029C RID: 668 RVA: 0x0002A2FA File Offset: 0x000284FA
	protected void ReceiveCollisionMessages(bool b)
	{
		if (b)
		{
			base.gameObject.transform.GetOrAddComponent<EntityCollisionMessage>();
			return;
		}
		base.gameObject.transform.RemoveComponent<EntityCollisionMessage>();
	}

	// Token: 0x0600029D RID: 669 RVA: 0x0002A324 File Offset: 0x00028524
	public virtual void DebugServer(int rep, float time)
	{
		Vector3 pos = base.transform.position + Vector3.up * 1f;
		string format = "{0}: {1}\n{2}";
		Networkable net = this.net;
		this.DebugText(pos, string.Format(format, (net != null) ? net.ID.Value : 0UL, base.name, this.DebugText()), Color.white, time);
	}

	// Token: 0x0600029E RID: 670 RVA: 0x0002A38F File Offset: 0x0002858F
	public virtual string DebugText()
	{
		return "";
	}

	// Token: 0x0600029F RID: 671 RVA: 0x0002A398 File Offset: 0x00028598
	public void OnDebugStart()
	{
		EntityDebug entityDebug = base.gameObject.GetComponent<EntityDebug>();
		if (entityDebug == null)
		{
			entityDebug = base.gameObject.AddComponent<EntityDebug>();
		}
		entityDebug.enabled = true;
	}

	// Token: 0x060002A0 RID: 672 RVA: 0x0002A3CD File Offset: 0x000285CD
	protected void DebugText(Vector3 pos, string str, Color color, float time)
	{
		if (base.isServer)
		{
			ConsoleNetwork.BroadcastToAllClients("ddraw.text", new object[]
			{
				time,
				color,
				pos,
				str
			});
		}
	}

	// Token: 0x060002A1 RID: 673 RVA: 0x0002A407 File Offset: 0x00028607
	public bool HasFlag(global::BaseEntity.Flags f)
	{
		return (this.flags & f) == f;
	}

	// Token: 0x060002A2 RID: 674 RVA: 0x0002A414 File Offset: 0x00028614
	public bool HasAny(global::BaseEntity.Flags f)
	{
		return (this.flags & f) > (global::BaseEntity.Flags)0;
	}

	// Token: 0x060002A3 RID: 675 RVA: 0x0002A424 File Offset: 0x00028624
	public bool ParentHasFlag(global::BaseEntity.Flags f)
	{
		global::BaseEntity parentEntity = base.GetParentEntity();
		return !(parentEntity == null) && parentEntity.HasFlag(f);
	}

	// Token: 0x060002A4 RID: 676 RVA: 0x0002A44C File Offset: 0x0002864C
	public void SetFlag(global::BaseEntity.Flags f, bool b, bool recursive = false, bool networkupdate = true)
	{
		global::BaseEntity.Flags old = this.flags;
		if (b)
		{
			if (this.HasFlag(f))
			{
				return;
			}
			this.flags |= f;
		}
		else
		{
			if (!this.HasFlag(f))
			{
				return;
			}
			this.flags &= ~f;
		}
		this.OnFlagsChanged(old, this.flags);
		if (networkupdate)
		{
			base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
		}
		else
		{
			base.InvalidateNetworkCache();
		}
		if (recursive && this.children != null)
		{
			for (int i = 0; i < this.children.Count; i++)
			{
				this.children[i].SetFlag(f, b, true, true);
			}
		}
	}

	// Token: 0x060002A5 RID: 677 RVA: 0x0002A4EC File Offset: 0x000286EC
	public bool IsOn()
	{
		return this.HasFlag(global::BaseEntity.Flags.On);
	}

	// Token: 0x060002A6 RID: 678 RVA: 0x0002A4F5 File Offset: 0x000286F5
	public bool IsOpen()
	{
		return this.HasFlag(global::BaseEntity.Flags.Open);
	}

	// Token: 0x060002A7 RID: 679 RVA: 0x0000326F File Offset: 0x0000146F
	public bool IsOnFire()
	{
		return this.HasFlag(global::BaseEntity.Flags.OnFire);
	}

	// Token: 0x060002A8 RID: 680 RVA: 0x0002A4FE File Offset: 0x000286FE
	public bool IsLocked()
	{
		return this.HasFlag(global::BaseEntity.Flags.Locked);
	}

	// Token: 0x060002A9 RID: 681 RVA: 0x0002A508 File Offset: 0x00028708
	public override bool IsDebugging()
	{
		return this.HasFlag(global::BaseEntity.Flags.Debugging);
	}

	// Token: 0x060002AA RID: 682 RVA: 0x0002A512 File Offset: 0x00028712
	public bool IsDisabled()
	{
		return this.HasFlag(global::BaseEntity.Flags.Disabled) || this.ParentHasFlag(global::BaseEntity.Flags.Disabled);
	}

	// Token: 0x060002AB RID: 683 RVA: 0x0002A528 File Offset: 0x00028728
	public bool IsBroken()
	{
		return this.HasFlag(global::BaseEntity.Flags.Broken);
	}

	// Token: 0x060002AC RID: 684 RVA: 0x0002A535 File Offset: 0x00028735
	public bool IsBusy()
	{
		return this.HasFlag(global::BaseEntity.Flags.Busy);
	}

	// Token: 0x060002AD RID: 685 RVA: 0x0002A542 File Offset: 0x00028742
	public override string GetLogColor()
	{
		if (base.isServer)
		{
			return "cyan";
		}
		return "yellow";
	}

	// Token: 0x060002AE RID: 686 RVA: 0x0002A557 File Offset: 0x00028757
	public virtual void OnFlagsChanged(global::BaseEntity.Flags old, global::BaseEntity.Flags next)
	{
		if (this.IsDebugging() && (old & global::BaseEntity.Flags.Debugging) != (next & global::BaseEntity.Flags.Debugging))
		{
			this.OnDebugStart();
		}
	}

	// Token: 0x060002AF RID: 687 RVA: 0x0002A574 File Offset: 0x00028774
	protected void SendNetworkUpdate_Flags()
	{
		if (Rust.Application.isLoading)
		{
			return;
		}
		if (Rust.Application.isLoadingSave)
		{
			return;
		}
		if (base.IsDestroyed)
		{
			return;
		}
		if (this.net == null)
		{
			return;
		}
		if (!this.isSpawned)
		{
			return;
		}
		using (TimeWarning.New("SendNetworkUpdate_Flags", 0))
		{
			base.LogEntry(BaseMonoBehaviour.LogEntryType.Network, 2, "SendNetworkUpdate_Flags");
			List<Connection> subscribers = base.GetSubscribers();
			if (subscribers != null && subscribers.Count > 0)
			{
				NetWrite netWrite = Network.Net.sv.StartWrite();
				netWrite.PacketID(Message.Type.EntityFlags);
				netWrite.EntityID(this.net.ID);
				netWrite.Int32((int)this.flags);
				SendInfo info = new SendInfo(subscribers);
				netWrite.Send(info);
			}
			base.gameObject.SendOnSendNetworkUpdate(this);
		}
	}

	// Token: 0x060002B0 RID: 688 RVA: 0x0002A640 File Offset: 0x00028840
	public bool IsOccupied(Socket_Base socket)
	{
		EntityLink entityLink = this.FindLink(socket);
		return entityLink != null && entityLink.IsOccupied();
	}

	// Token: 0x060002B1 RID: 689 RVA: 0x0002A660 File Offset: 0x00028860
	public bool IsOccupied(string socketName)
	{
		EntityLink entityLink = this.FindLink(socketName);
		return entityLink != null && entityLink.IsOccupied();
	}

	// Token: 0x060002B2 RID: 690 RVA: 0x0002A680 File Offset: 0x00028880
	public EntityLink FindLink(Socket_Base socket)
	{
		List<EntityLink> entityLinks = this.GetEntityLinks(true);
		for (int i = 0; i < entityLinks.Count; i++)
		{
			if (entityLinks[i].socket == socket)
			{
				return entityLinks[i];
			}
		}
		return null;
	}

	// Token: 0x060002B3 RID: 691 RVA: 0x0002A6C4 File Offset: 0x000288C4
	public EntityLink FindLink(string socketName)
	{
		List<EntityLink> entityLinks = this.GetEntityLinks(true);
		for (int i = 0; i < entityLinks.Count; i++)
		{
			if (entityLinks[i].socket.socketName == socketName)
			{
				return entityLinks[i];
			}
		}
		return null;
	}

	// Token: 0x060002B4 RID: 692 RVA: 0x0002A70C File Offset: 0x0002890C
	public EntityLink FindLink(string[] socketNames)
	{
		List<EntityLink> entityLinks = this.GetEntityLinks(true);
		for (int i = 0; i < entityLinks.Count; i++)
		{
			for (int j = 0; j < socketNames.Length; j++)
			{
				if (entityLinks[i].socket.socketName == socketNames[j])
				{
					return entityLinks[i];
				}
			}
		}
		return null;
	}

	// Token: 0x060002B5 RID: 693 RVA: 0x0002A764 File Offset: 0x00028964
	public T FindLinkedEntity<T>() where T : global::BaseEntity
	{
		List<EntityLink> entityLinks = this.GetEntityLinks(true);
		for (int i = 0; i < entityLinks.Count; i++)
		{
			EntityLink entityLink = entityLinks[i];
			for (int j = 0; j < entityLink.connections.Count; j++)
			{
				EntityLink entityLink2 = entityLink.connections[j];
				if (entityLink2.owner is T)
				{
					return entityLink2.owner as T;
				}
			}
		}
		return default(T);
	}

	// Token: 0x060002B6 RID: 694 RVA: 0x0002A7E0 File Offset: 0x000289E0
	public void EntityLinkMessage<T>(Action<T> action) where T : global::BaseEntity
	{
		List<EntityLink> entityLinks = this.GetEntityLinks(true);
		for (int i = 0; i < entityLinks.Count; i++)
		{
			EntityLink entityLink = entityLinks[i];
			for (int j = 0; j < entityLink.connections.Count; j++)
			{
				EntityLink entityLink2 = entityLink.connections[j];
				if (entityLink2.owner is T)
				{
					action(entityLink2.owner as T);
				}
			}
		}
	}

	// Token: 0x060002B7 RID: 695 RVA: 0x0002A858 File Offset: 0x00028A58
	public void EntityLinkBroadcast<T, S>(Action<T> action, Func<S, bool> canTraverseSocket) where T : global::BaseEntity where S : Socket_Base
	{
		global::BaseEntity.globalBroadcastProtocol += 1U;
		global::BaseEntity.globalBroadcastQueue.Clear();
		this.broadcastProtocol = global::BaseEntity.globalBroadcastProtocol;
		global::BaseEntity.globalBroadcastQueue.Enqueue(this);
		if (this is T)
		{
			action(this as T);
		}
		while (global::BaseEntity.globalBroadcastQueue.Count > 0)
		{
			List<EntityLink> entityLinks = global::BaseEntity.globalBroadcastQueue.Dequeue().GetEntityLinks(true);
			for (int i = 0; i < entityLinks.Count; i++)
			{
				EntityLink entityLink = entityLinks[i];
				if (entityLink.socket is S && canTraverseSocket(entityLink.socket as S))
				{
					for (int j = 0; j < entityLink.connections.Count; j++)
					{
						global::BaseEntity owner = entityLink.connections[j].owner;
						if (owner.broadcastProtocol != global::BaseEntity.globalBroadcastProtocol)
						{
							owner.broadcastProtocol = global::BaseEntity.globalBroadcastProtocol;
							global::BaseEntity.globalBroadcastQueue.Enqueue(owner);
							if (owner is T)
							{
								action(owner as T);
							}
						}
					}
				}
			}
		}
	}

	// Token: 0x060002B8 RID: 696 RVA: 0x0002A984 File Offset: 0x00028B84
	public void EntityLinkBroadcast<T>(Action<T> action) where T : global::BaseEntity
	{
		global::BaseEntity.globalBroadcastProtocol += 1U;
		global::BaseEntity.globalBroadcastQueue.Clear();
		this.broadcastProtocol = global::BaseEntity.globalBroadcastProtocol;
		global::BaseEntity.globalBroadcastQueue.Enqueue(this);
		if (this is T)
		{
			action(this as T);
		}
		while (global::BaseEntity.globalBroadcastQueue.Count > 0)
		{
			List<EntityLink> entityLinks = global::BaseEntity.globalBroadcastQueue.Dequeue().GetEntityLinks(true);
			for (int i = 0; i < entityLinks.Count; i++)
			{
				EntityLink entityLink = entityLinks[i];
				for (int j = 0; j < entityLink.connections.Count; j++)
				{
					global::BaseEntity owner = entityLink.connections[j].owner;
					if (owner.broadcastProtocol != global::BaseEntity.globalBroadcastProtocol)
					{
						owner.broadcastProtocol = global::BaseEntity.globalBroadcastProtocol;
						global::BaseEntity.globalBroadcastQueue.Enqueue(owner);
						if (owner is T)
						{
							action(owner as T);
						}
					}
				}
			}
		}
	}

	// Token: 0x060002B9 RID: 697 RVA: 0x0002AA84 File Offset: 0x00028C84
	public void EntityLinkBroadcast()
	{
		global::BaseEntity.globalBroadcastProtocol += 1U;
		global::BaseEntity.globalBroadcastQueue.Clear();
		this.broadcastProtocol = global::BaseEntity.globalBroadcastProtocol;
		global::BaseEntity.globalBroadcastQueue.Enqueue(this);
		while (global::BaseEntity.globalBroadcastQueue.Count > 0)
		{
			List<EntityLink> entityLinks = global::BaseEntity.globalBroadcastQueue.Dequeue().GetEntityLinks(true);
			for (int i = 0; i < entityLinks.Count; i++)
			{
				EntityLink entityLink = entityLinks[i];
				for (int j = 0; j < entityLink.connections.Count; j++)
				{
					global::BaseEntity owner = entityLink.connections[j].owner;
					if (owner.broadcastProtocol != global::BaseEntity.globalBroadcastProtocol)
					{
						owner.broadcastProtocol = global::BaseEntity.globalBroadcastProtocol;
						global::BaseEntity.globalBroadcastQueue.Enqueue(owner);
					}
				}
			}
		}
	}

	// Token: 0x060002BA RID: 698 RVA: 0x0002AB48 File Offset: 0x00028D48
	public bool ReceivedEntityLinkBroadcast()
	{
		return this.broadcastProtocol == global::BaseEntity.globalBroadcastProtocol;
	}

	// Token: 0x060002BB RID: 699 RVA: 0x0002AB57 File Offset: 0x00028D57
	public List<EntityLink> GetEntityLinks(bool linkToNeighbours = true)
	{
		if (Rust.Application.isLoadingSave)
		{
			return this.links;
		}
		if (!this.linkedToNeighbours && linkToNeighbours)
		{
			this.LinkToNeighbours();
		}
		return this.links;
	}

	// Token: 0x060002BC RID: 700 RVA: 0x0002AB80 File Offset: 0x00028D80
	private void LinkToEntity(global::BaseEntity other)
	{
		if (this == other)
		{
			return;
		}
		if (this.links.Count == 0 || other.links.Count == 0)
		{
			return;
		}
		using (TimeWarning.New("LinkToEntity", 0))
		{
			for (int i = 0; i < this.links.Count; i++)
			{
				EntityLink entityLink = this.links[i];
				for (int j = 0; j < other.links.Count; j++)
				{
					EntityLink entityLink2 = other.links[j];
					if (entityLink.CanConnect(entityLink2))
					{
						if (!entityLink.Contains(entityLink2))
						{
							entityLink.Add(entityLink2);
						}
						if (!entityLink2.Contains(entityLink))
						{
							entityLink2.Add(entityLink);
						}
					}
				}
			}
		}
	}

	// Token: 0x060002BD RID: 701 RVA: 0x0002AC50 File Offset: 0x00028E50
	private void LinkToNeighbours()
	{
		if (this.links.Count == 0)
		{
			return;
		}
		this.linkedToNeighbours = true;
		using (TimeWarning.New("LinkToNeighbours", 0))
		{
			List<global::BaseEntity> list = Facepunch.Pool.GetList<global::BaseEntity>();
			OBB obb = this.WorldSpaceBounds();
			global::Vis.Entities<global::BaseEntity>(obb.position, obb.extents.magnitude + 1f, list, -1, QueryTriggerInteraction.Collide);
			for (int i = 0; i < list.Count; i++)
			{
				global::BaseEntity baseEntity = list[i];
				if (baseEntity.isServer == base.isServer)
				{
					this.LinkToEntity(baseEntity);
				}
			}
			Facepunch.Pool.FreeList<global::BaseEntity>(ref list);
		}
	}

	// Token: 0x060002BE RID: 702 RVA: 0x0002AD00 File Offset: 0x00028F00
	private void InitEntityLinks()
	{
		using (TimeWarning.New("InitEntityLinks", 0))
		{
			if (base.isServer)
			{
				this.links.AddLinks(this, PrefabAttribute.server.FindAll<Socket_Base>(this.prefabID));
			}
		}
	}

	// Token: 0x060002BF RID: 703 RVA: 0x0002AD5C File Offset: 0x00028F5C
	private void FreeEntityLinks()
	{
		using (TimeWarning.New("FreeEntityLinks", 0))
		{
			this.links.FreeLinks();
			this.linkedToNeighbours = false;
		}
	}

	// Token: 0x060002C0 RID: 704 RVA: 0x0002ADA4 File Offset: 0x00028FA4
	public void RefreshEntityLinks()
	{
		using (TimeWarning.New("RefreshEntityLinks", 0))
		{
			this.links.ClearLinks();
			this.LinkToNeighbours();
		}
	}

	// Token: 0x060002C1 RID: 705 RVA: 0x0002ADEC File Offset: 0x00028FEC
	[global::BaseEntity.RPC_Server]
	public void SV_RequestFile(global::BaseEntity.RPCMessage msg)
	{
		uint num = msg.read.UInt32();
		FileStorage.Type type = (FileStorage.Type)msg.read.UInt8();
		string funcName = StringPool.Get(msg.read.UInt32());
		uint num2 = (msg.read.Unread > 0) ? msg.read.UInt32() : 0U;
		bool flag = msg.read.Unread > 0 && msg.read.Bit();
		byte[] array = FileStorage.server.Get(num, type, this.net.ID, num2);
		if (array == null)
		{
			if (!flag)
			{
				return;
			}
			array = Array.Empty<byte>();
		}
		SendInfo sendInfo = new SendInfo(msg.connection)
		{
			channel = 2,
			method = SendMethod.Reliable
		};
		this.ClientRPCEx<uint, uint, byte[], uint, byte>(sendInfo, null, funcName, num, (uint)array.Length, array, num2, (byte)type);
	}

	// Token: 0x060002C2 RID: 706 RVA: 0x0002AEBC File Offset: 0x000290BC
	public void SetParent(global::BaseEntity entity, bool worldPositionStays = false, bool sendImmediate = false)
	{
		this.SetParent(entity, 0U, worldPositionStays, sendImmediate);
	}

	// Token: 0x060002C3 RID: 707 RVA: 0x0002AEC8 File Offset: 0x000290C8
	public void SetParent(global::BaseEntity entity, string strBone, bool worldPositionStays = false, bool sendImmediate = false)
	{
		this.SetParent(entity, string.IsNullOrEmpty(strBone) ? 0U : StringPool.Get(strBone), worldPositionStays, sendImmediate);
	}

	// Token: 0x060002C4 RID: 708 RVA: 0x0002AEE8 File Offset: 0x000290E8
	public bool HasChild(global::BaseEntity c)
	{
		if (c == this)
		{
			return true;
		}
		global::BaseEntity parentEntity = c.GetParentEntity();
		return parentEntity != null && this.HasChild(parentEntity);
	}

	// Token: 0x060002C5 RID: 709 RVA: 0x0002AF1C File Offset: 0x0002911C
	public void SetParent(global::BaseEntity entity, uint boneID, bool worldPositionStays = false, bool sendImmediate = false)
	{
		if (entity != null)
		{
			if (entity == this)
			{
				Debug.LogError("Trying to parent to self " + this, base.gameObject);
				return;
			}
			if (this.HasChild(entity))
			{
				Debug.LogError("Trying to parent to child " + this, base.gameObject);
				return;
			}
		}
		base.LogEntry(BaseMonoBehaviour.LogEntryType.Hierarchy, 2, "SetParent {0} {1}", entity, boneID);
		global::BaseEntity parentEntity = base.GetParentEntity();
		if (parentEntity)
		{
			parentEntity.RemoveChild(this);
		}
		if (base.limitNetworking && parentEntity != null && parentEntity != entity)
		{
			global::BasePlayer basePlayer = parentEntity as global::BasePlayer;
			if (basePlayer.IsValid())
			{
				this.DestroyOnClient(basePlayer.net.connection);
			}
		}
		if (entity == null)
		{
			this.OnParentChanging(parentEntity, null);
			this.parentEntity.Set(null);
			base.transform.SetParent(null, worldPositionStays);
			this.parentBone = 0U;
			this.UpdateNetworkGroup();
			if (sendImmediate)
			{
				base.SendNetworkUpdateImmediate(false);
				this.SendChildrenNetworkUpdateImmediate();
				return;
			}
			base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
			this.SendChildrenNetworkUpdate();
			return;
		}
		else
		{
			Debug.Assert(entity.isServer, "SetParent - child should be a SERVER entity");
			Debug.Assert(entity.net != null, "Setting parent to entity that hasn't spawned yet! (net is null)");
			Debug.Assert(entity.net.ID.IsValid, "Setting parent to entity that hasn't spawned yet! (id = 0)");
			entity.AddChild(this);
			this.OnParentChanging(parentEntity, entity);
			this.parentEntity.Set(entity);
			if (boneID != 0U && boneID != StringPool.closest)
			{
				base.transform.SetParent(entity.FindBone(StringPool.Get(boneID)), worldPositionStays);
			}
			else
			{
				base.transform.SetParent(entity.transform, worldPositionStays);
			}
			this.parentBone = boneID;
			this.UpdateNetworkGroup();
			if (sendImmediate)
			{
				base.SendNetworkUpdateImmediate(false);
				this.SendChildrenNetworkUpdateImmediate();
				return;
			}
			base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
			this.SendChildrenNetworkUpdate();
			return;
		}
	}

	// Token: 0x060002C6 RID: 710 RVA: 0x0002B0E8 File Offset: 0x000292E8
	private void DestroyOnClient(Connection connection)
	{
		if (this.children != null)
		{
			foreach (global::BaseEntity baseEntity in this.children)
			{
				baseEntity.DestroyOnClient(connection);
			}
		}
		if (Network.Net.sv.IsConnected())
		{
			NetWrite netWrite = Network.Net.sv.StartWrite();
			netWrite.PacketID(Message.Type.EntityDestroy);
			netWrite.EntityID(this.net.ID);
			netWrite.UInt8(0);
			netWrite.Send(new SendInfo(connection));
			base.LogEntry(BaseMonoBehaviour.LogEntryType.Network, 2, "EntityDestroy");
		}
	}

	// Token: 0x060002C7 RID: 711 RVA: 0x0002B190 File Offset: 0x00029390
	private void SendChildrenNetworkUpdate()
	{
		if (this.children == null)
		{
			return;
		}
		foreach (global::BaseEntity baseEntity in this.children)
		{
			baseEntity.UpdateNetworkGroup();
			baseEntity.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
		}
	}

	// Token: 0x060002C8 RID: 712 RVA: 0x0002B1F0 File Offset: 0x000293F0
	private void SendChildrenNetworkUpdateImmediate()
	{
		if (this.children == null)
		{
			return;
		}
		foreach (global::BaseEntity baseEntity in this.children)
		{
			baseEntity.UpdateNetworkGroup();
			baseEntity.SendNetworkUpdateImmediate(false);
		}
	}

	// Token: 0x060002C9 RID: 713 RVA: 0x0002B250 File Offset: 0x00029450
	public virtual void SwitchParent(global::BaseEntity ent)
	{
		this.Log("SwitchParent Missed " + ent);
	}

	// Token: 0x060002CA RID: 714 RVA: 0x0002B264 File Offset: 0x00029464
	public virtual void OnParentChanging(global::BaseEntity oldParent, global::BaseEntity newParent)
	{
		Rigidbody component = base.GetComponent<Rigidbody>();
		if (component)
		{
			if (oldParent != null && oldParent.GetComponent<Rigidbody>() == null)
			{
				component.velocity += oldParent.GetWorldVelocity();
			}
			if (newParent != null && newParent.GetComponent<Rigidbody>() == null)
			{
				component.velocity -= newParent.GetWorldVelocity();
			}
		}
	}

	// Token: 0x060002CB RID: 715 RVA: 0x0002B2DC File Offset: 0x000294DC
	public virtual BuildingPrivlidge GetBuildingPrivilege()
	{
		return this.GetBuildingPrivilege(this.WorldSpaceBounds());
	}

	// Token: 0x060002CC RID: 716 RVA: 0x0002B2EC File Offset: 0x000294EC
	public BuildingPrivlidge GetBuildingPrivilege(OBB obb)
	{
		global::BuildingBlock other = null;
		BuildingPrivlidge result = null;
		List<global::BuildingBlock> list = Facepunch.Pool.GetList<global::BuildingBlock>();
		global::Vis.Entities<global::BuildingBlock>(obb.position, 16f + obb.extents.magnitude, list, 2097152, QueryTriggerInteraction.Collide);
		for (int i = 0; i < list.Count; i++)
		{
			global::BuildingBlock buildingBlock = list[i];
			if (buildingBlock.isServer == base.isServer && buildingBlock.IsOlderThan(other) && obb.Distance(buildingBlock.WorldSpaceBounds()) <= 16f)
			{
				BuildingManager.Building building = buildingBlock.GetBuilding();
				if (building != null)
				{
					BuildingPrivlidge dominatingBuildingPrivilege = building.GetDominatingBuildingPrivilege();
					if (!(dominatingBuildingPrivilege == null))
					{
						other = buildingBlock;
						result = dominatingBuildingPrivilege;
					}
				}
			}
		}
		Facepunch.Pool.FreeList<global::BuildingBlock>(ref list);
		return result;
	}

	// Token: 0x060002CD RID: 717 RVA: 0x0002B3A0 File Offset: 0x000295A0
	public void SV_RPCMessage(uint nameID, Message message)
	{
		Assert.IsTrue(base.isServer, "Should be server!");
		global::BasePlayer basePlayer = message.Player();
		if (!basePlayer.IsValid())
		{
			if (ConVar.Global.developer > 0)
			{
				Debug.Log("SV_RPCMessage: From invalid player " + basePlayer);
			}
			return;
		}
		if (basePlayer.isStalled)
		{
			if (ConVar.Global.developer > 0)
			{
				Debug.Log("SV_RPCMessage: player is stalled " + basePlayer);
			}
			return;
		}
		if (this.OnRpcMessage(basePlayer, nameID, message))
		{
			return;
		}
		for (int i = 0; i < this.Components.Length; i++)
		{
			if (this.Components[i].OnRpcMessage(basePlayer, nameID, message))
			{
				return;
			}
		}
	}

	// Token: 0x060002CE RID: 718 RVA: 0x0002B438 File Offset: 0x00029638
	public void ClientRPCPlayer<T1, T2, T3, T4, T5>(Connection sourceConnection, global::BasePlayer player, string funcName, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
	{
		if (!Network.Net.sv.IsConnected())
		{
			return;
		}
		if (this.net == null)
		{
			return;
		}
		if (player.net.connection == null)
		{
			return;
		}
		this.ClientRPCEx<T1, T2, T3, T4, T5>(new SendInfo(player.net.connection), sourceConnection, funcName, arg1, arg2, arg3, arg4, arg5);
	}

	// Token: 0x060002CF RID: 719 RVA: 0x0002B48C File Offset: 0x0002968C
	public void ClientRPCPlayer<T1, T2, T3, T4>(Connection sourceConnection, global::BasePlayer player, string funcName, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
	{
		if (!Network.Net.sv.IsConnected())
		{
			return;
		}
		if (this.net == null)
		{
			return;
		}
		if (player.net.connection == null)
		{
			return;
		}
		this.ClientRPCEx<T1, T2, T3, T4>(new SendInfo(player.net.connection), sourceConnection, funcName, arg1, arg2, arg3, arg4);
	}

	// Token: 0x060002D0 RID: 720 RVA: 0x0002B4E0 File Offset: 0x000296E0
	public void ClientRPCPlayer<T1, T2, T3>(Connection sourceConnection, global::BasePlayer player, string funcName, T1 arg1, T2 arg2, T3 arg3)
	{
		if (!Network.Net.sv.IsConnected())
		{
			return;
		}
		if (this.net == null)
		{
			return;
		}
		if (player.net.connection == null)
		{
			return;
		}
		this.ClientRPCEx<T1, T2, T3>(new SendInfo(player.net.connection), sourceConnection, funcName, arg1, arg2, arg3);
	}

	// Token: 0x060002D1 RID: 721 RVA: 0x0002B530 File Offset: 0x00029730
	public void ClientRPCPlayer<T1, T2>(Connection sourceConnection, global::BasePlayer player, string funcName, T1 arg1, T2 arg2)
	{
		if (!Network.Net.sv.IsConnected())
		{
			return;
		}
		if (this.net == null)
		{
			return;
		}
		if (player.net.connection == null)
		{
			return;
		}
		this.ClientRPCEx<T1, T2>(new SendInfo(player.net.connection), sourceConnection, funcName, arg1, arg2);
	}

	// Token: 0x060002D2 RID: 722 RVA: 0x0002B57D File Offset: 0x0002977D
	public void ClientRPCPlayer<T1>(Connection sourceConnection, global::BasePlayer player, string funcName, T1 arg1)
	{
		if (!Network.Net.sv.IsConnected())
		{
			return;
		}
		if (this.net == null)
		{
			return;
		}
		if (player.net.connection == null)
		{
			return;
		}
		this.ClientRPCEx<T1>(new SendInfo(player.net.connection), sourceConnection, funcName, arg1);
	}

	// Token: 0x060002D3 RID: 723 RVA: 0x0002B5BD File Offset: 0x000297BD
	public void ClientRPCPlayer(Connection sourceConnection, global::BasePlayer player, string funcName)
	{
		if (!Network.Net.sv.IsConnected())
		{
			return;
		}
		if (this.net == null)
		{
			return;
		}
		if (player.net.connection == null)
		{
			return;
		}
		this.ClientRPCEx(new SendInfo(player.net.connection), sourceConnection, funcName);
	}

	// Token: 0x060002D4 RID: 724 RVA: 0x0002B5FC File Offset: 0x000297FC
	public void ClientRPC<T1, T2, T3, T4, T5>(Connection sourceConnection, string funcName, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
	{
		if (!Network.Net.sv.IsConnected())
		{
			return;
		}
		if (this.net == null)
		{
			return;
		}
		if (this.net.group == null)
		{
			return;
		}
		this.ClientRPCEx<T1, T2, T3, T4, T5>(new SendInfo(this.net.group.subscribers), sourceConnection, funcName, arg1, arg2, arg3, arg4, arg5);
	}

	// Token: 0x060002D5 RID: 725 RVA: 0x0002B654 File Offset: 0x00029854
	public void ClientRPC<T1, T2, T3, T4>(Connection sourceConnection, string funcName, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
	{
		if (!Network.Net.sv.IsConnected())
		{
			return;
		}
		if (this.net == null)
		{
			return;
		}
		if (this.net.group == null)
		{
			return;
		}
		this.ClientRPCEx<T1, T2, T3, T4>(new SendInfo(this.net.group.subscribers), sourceConnection, funcName, arg1, arg2, arg3, arg4);
	}

	// Token: 0x060002D6 RID: 726 RVA: 0x0002B6AC File Offset: 0x000298AC
	public void ClientRPC<T1, T2, T3>(Connection sourceConnection, string funcName, T1 arg1, T2 arg2, T3 arg3)
	{
		if (!Network.Net.sv.IsConnected())
		{
			return;
		}
		if (this.net == null)
		{
			return;
		}
		if (this.net.group == null)
		{
			return;
		}
		this.ClientRPCEx<T1, T2, T3>(new SendInfo(this.net.group.subscribers), sourceConnection, funcName, arg1, arg2, arg3);
	}

	// Token: 0x060002D7 RID: 727 RVA: 0x0002B700 File Offset: 0x00029900
	public void ClientRPC<T1, T2>(Connection sourceConnection, string funcName, T1 arg1, T2 arg2)
	{
		if (!Network.Net.sv.IsConnected())
		{
			return;
		}
		if (this.net == null)
		{
			return;
		}
		if (this.net.group == null)
		{
			return;
		}
		this.ClientRPCEx<T1, T2>(new SendInfo(this.net.group.subscribers), sourceConnection, funcName, arg1, arg2);
	}

	// Token: 0x060002D8 RID: 728 RVA: 0x0002B754 File Offset: 0x00029954
	public void ClientRPC<T1>(Connection sourceConnection, string funcName, T1 arg1)
	{
		if (!Network.Net.sv.IsConnected())
		{
			return;
		}
		if (this.net == null)
		{
			return;
		}
		if (this.net.group == null)
		{
			return;
		}
		this.ClientRPCEx<T1>(new SendInfo(this.net.group.subscribers), sourceConnection, funcName, arg1);
	}

	// Token: 0x060002D9 RID: 729 RVA: 0x0002B7A4 File Offset: 0x000299A4
	public void ClientRPC(Connection sourceConnection, string funcName)
	{
		if (!Network.Net.sv.IsConnected())
		{
			return;
		}
		if (this.net == null)
		{
			return;
		}
		if (this.net.group == null)
		{
			return;
		}
		this.ClientRPCEx(new SendInfo(this.net.group.subscribers), sourceConnection, funcName);
	}

	// Token: 0x060002DA RID: 730 RVA: 0x0002B7F4 File Offset: 0x000299F4
	public void ClientRPCEx<T1, T2, T3, T4, T5>(SendInfo sendInfo, Connection sourceConnection, string funcName, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
	{
		if (!Network.Net.sv.IsConnected())
		{
			return;
		}
		if (this.net == null)
		{
			return;
		}
		NetWrite write = this.ClientRPCStart(sourceConnection, funcName);
		this.ClientRPCWrite<T1>(write, arg1);
		this.ClientRPCWrite<T2>(write, arg2);
		this.ClientRPCWrite<T3>(write, arg3);
		this.ClientRPCWrite<T4>(write, arg4);
		this.ClientRPCWrite<T5>(write, arg5);
		this.ClientRPCSend(write, sendInfo);
	}

	// Token: 0x060002DB RID: 731 RVA: 0x0002B858 File Offset: 0x00029A58
	public void ClientRPCEx<T1, T2, T3, T4>(SendInfo sendInfo, Connection sourceConnection, string funcName, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
	{
		if (!Network.Net.sv.IsConnected())
		{
			return;
		}
		if (this.net == null)
		{
			return;
		}
		NetWrite write = this.ClientRPCStart(sourceConnection, funcName);
		this.ClientRPCWrite<T1>(write, arg1);
		this.ClientRPCWrite<T2>(write, arg2);
		this.ClientRPCWrite<T3>(write, arg3);
		this.ClientRPCWrite<T4>(write, arg4);
		this.ClientRPCSend(write, sendInfo);
	}

	// Token: 0x060002DC RID: 732 RVA: 0x0002B8B0 File Offset: 0x00029AB0
	public void ClientRPCEx<T1, T2, T3>(SendInfo sendInfo, Connection sourceConnection, string funcName, T1 arg1, T2 arg2, T3 arg3)
	{
		if (!Network.Net.sv.IsConnected())
		{
			return;
		}
		if (this.net == null)
		{
			return;
		}
		NetWrite write = this.ClientRPCStart(sourceConnection, funcName);
		this.ClientRPCWrite<T1>(write, arg1);
		this.ClientRPCWrite<T2>(write, arg2);
		this.ClientRPCWrite<T3>(write, arg3);
		this.ClientRPCSend(write, sendInfo);
	}

	// Token: 0x060002DD RID: 733 RVA: 0x0002B900 File Offset: 0x00029B00
	public void ClientRPCEx<T1, T2>(SendInfo sendInfo, Connection sourceConnection, string funcName, T1 arg1, T2 arg2)
	{
		if (!Network.Net.sv.IsConnected())
		{
			return;
		}
		if (this.net == null)
		{
			return;
		}
		NetWrite write = this.ClientRPCStart(sourceConnection, funcName);
		this.ClientRPCWrite<T1>(write, arg1);
		this.ClientRPCWrite<T2>(write, arg2);
		this.ClientRPCSend(write, sendInfo);
	}

	// Token: 0x060002DE RID: 734 RVA: 0x0002B948 File Offset: 0x00029B48
	public void ClientRPCEx<T1>(SendInfo sendInfo, Connection sourceConnection, string funcName, T1 arg1)
	{
		if (!Network.Net.sv.IsConnected())
		{
			return;
		}
		if (this.net == null)
		{
			return;
		}
		NetWrite write = this.ClientRPCStart(sourceConnection, funcName);
		this.ClientRPCWrite<T1>(write, arg1);
		this.ClientRPCSend(write, sendInfo);
	}

	// Token: 0x060002DF RID: 735 RVA: 0x0002B988 File Offset: 0x00029B88
	public void ClientRPCEx(SendInfo sendInfo, Connection sourceConnection, string funcName)
	{
		if (!Network.Net.sv.IsConnected())
		{
			return;
		}
		if (this.net == null)
		{
			return;
		}
		NetWrite write = this.ClientRPCStart(sourceConnection, funcName);
		this.ClientRPCSend(write, sendInfo);
	}

	// Token: 0x060002E0 RID: 736 RVA: 0x0002B9BC File Offset: 0x00029BBC
	public void ClientRPCPlayerAndSpectators(Connection sourceConnection, global::BasePlayer player, string funcName)
	{
		if (!Network.Net.sv.IsConnected())
		{
			return;
		}
		if (player.net == null)
		{
			return;
		}
		if (player.net.connection == null)
		{
			return;
		}
		this.ClientRPCEx(new SendInfo(player.net.connection), sourceConnection, funcName);
		if (player.IsBeingSpectated && player.children != null)
		{
			using (List<global::BaseEntity>.Enumerator enumerator = player.children.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					global::BasePlayer player2;
					if ((player2 = (enumerator.Current as global::BasePlayer)) != null)
					{
						this.ClientRPCPlayer(sourceConnection, player2, funcName);
					}
				}
			}
		}
	}

	// Token: 0x060002E1 RID: 737 RVA: 0x0002BA68 File Offset: 0x00029C68
	public void ClientRPCPlayerAndSpectators<T1>(Connection sourceConnection, global::BasePlayer player, string funcName, T1 arg1)
	{
		if (!Network.Net.sv.IsConnected())
		{
			return;
		}
		if (player.net == null)
		{
			return;
		}
		if (player.net.connection == null)
		{
			return;
		}
		this.ClientRPCEx<T1>(new SendInfo(player.net.connection), sourceConnection, funcName, arg1);
		if (player.IsBeingSpectated && player.children != null)
		{
			using (List<global::BaseEntity>.Enumerator enumerator = player.children.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					global::BasePlayer player2;
					if ((player2 = (enumerator.Current as global::BasePlayer)) != null)
					{
						this.ClientRPCPlayer<T1>(sourceConnection, player2, funcName, arg1);
					}
				}
			}
		}
	}

	// Token: 0x060002E2 RID: 738 RVA: 0x0002BB18 File Offset: 0x00029D18
	public void ClientRPCPlayerAndSpectators<T1, T2>(Connection sourceConnection, global::BasePlayer player, string funcName, T1 arg1, T2 arg2)
	{
		if (!Network.Net.sv.IsConnected())
		{
			return;
		}
		if (player.net == null)
		{
			return;
		}
		if (player.net.connection == null)
		{
			return;
		}
		this.ClientRPCPlayer<T1, T2>(sourceConnection, player, funcName, arg1, arg2);
		if (player.IsBeingSpectated && player.children != null)
		{
			using (List<global::BaseEntity>.Enumerator enumerator = this.children.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					global::BasePlayer player2;
					if ((player2 = (enumerator.Current as global::BasePlayer)) != null)
					{
						this.ClientRPCPlayer<T1, T2>(sourceConnection, player2, funcName, arg1, arg2);
					}
				}
			}
		}
	}

	// Token: 0x060002E3 RID: 739 RVA: 0x0002BBBC File Offset: 0x00029DBC
	public void ClientRPCPlayerAndSpectators<T1, T2, T3>(Connection sourceConnection, global::BasePlayer player, string funcName, T1 arg1, T2 arg2, T3 arg3)
	{
		if (!Network.Net.sv.IsConnected())
		{
			return;
		}
		if (player.net == null)
		{
			return;
		}
		if (player.net.connection == null)
		{
			return;
		}
		this.ClientRPCPlayer<T1, T2, T3>(sourceConnection, player, funcName, arg1, arg2, arg3);
		if (player.IsBeingSpectated && player.children != null)
		{
			using (List<global::BaseEntity>.Enumerator enumerator = player.children.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					global::BasePlayer player2;
					if ((player2 = (enumerator.Current as global::BasePlayer)) != null)
					{
						this.ClientRPCPlayer<T1, T2, T3>(sourceConnection, player2, funcName, arg1, arg2, arg3);
					}
				}
			}
		}
	}

	// Token: 0x060002E4 RID: 740 RVA: 0x0002BC64 File Offset: 0x00029E64
	private NetWrite ClientRPCStart(Connection sourceConnection, string funcName)
	{
		NetWrite netWrite = Network.Net.sv.StartWrite();
		netWrite.PacketID(Message.Type.RPCMessage);
		netWrite.EntityID(this.net.ID);
		netWrite.UInt32(StringPool.Get(funcName));
		netWrite.UInt64((sourceConnection == null) ? 0UL : sourceConnection.userid);
		return netWrite;
	}

	// Token: 0x060002E5 RID: 741 RVA: 0x0002BCB3 File Offset: 0x00029EB3
	private void ClientRPCWrite<T>(NetWrite write, T arg)
	{
		write.WriteObject(arg);
	}

	// Token: 0x060002E6 RID: 742 RVA: 0x0002BCBC File Offset: 0x00029EBC
	private void ClientRPCSend(NetWrite write, SendInfo sendInfo)
	{
		write.Send(sendInfo);
	}

	// Token: 0x17000053 RID: 83
	// (get) Token: 0x060002E7 RID: 743 RVA: 0x0002BCC8 File Offset: 0x00029EC8
	public float radiationLevel
	{
		get
		{
			if (this.triggers == null)
			{
				return 0f;
			}
			float num = 0f;
			for (int i = 0; i < this.triggers.Count; i++)
			{
				TriggerRadiation triggerRadiation = this.triggers[i] as TriggerRadiation;
				if (!(triggerRadiation == null))
				{
					Vector3 position = this.GetNetworkPosition();
					global::BaseEntity parentEntity = base.GetParentEntity();
					if (parentEntity != null)
					{
						position = parentEntity.transform.TransformPoint(position);
					}
					num = Mathf.Max(num, triggerRadiation.GetRadiation(position, this.RadiationProtection()));
				}
			}
			return num;
		}
	}

	// Token: 0x060002E8 RID: 744 RVA: 0x00029CA8 File Offset: 0x00027EA8
	public virtual float RadiationProtection()
	{
		return 0f;
	}

	// Token: 0x060002E9 RID: 745 RVA: 0x00006CA5 File Offset: 0x00004EA5
	public virtual float RadiationExposureFraction()
	{
		return 1f;
	}

	// Token: 0x17000054 RID: 84
	// (get) Token: 0x060002EA RID: 746 RVA: 0x0002BD58 File Offset: 0x00029F58
	public float currentTemperature
	{
		get
		{
			float num = Climate.GetTemperature(base.transform.position);
			if (this.triggers == null)
			{
				return num;
			}
			for (int i = 0; i < this.triggers.Count; i++)
			{
				TriggerTemperature triggerTemperature = this.triggers[i] as TriggerTemperature;
				if (!(triggerTemperature == null))
				{
					num = triggerTemperature.WorkoutTemperature(this.GetNetworkPosition(), num);
				}
			}
			return num;
		}
	}

	// Token: 0x17000055 RID: 85
	// (get) Token: 0x060002EB RID: 747 RVA: 0x0002BDC0 File Offset: 0x00029FC0
	public float currentEnvironmentalWetness
	{
		get
		{
			if (this.triggers == null)
			{
				return 0f;
			}
			float num = 0f;
			Vector3 networkPosition = this.GetNetworkPosition();
			using (List<TriggerBase>.Enumerator enumerator = this.triggers.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					TriggerWetness triggerWetness;
					if ((triggerWetness = (enumerator.Current as TriggerWetness)) != null)
					{
						num += triggerWetness.WorkoutWetness(networkPosition);
					}
				}
			}
			return Mathf.Clamp01(num);
		}
	}

	// Token: 0x060002EC RID: 748 RVA: 0x0002BE40 File Offset: 0x0002A040
	public virtual void SetCreatorEntity(global::BaseEntity newCreatorEntity)
	{
		this.creatorEntity = newCreatorEntity;
	}

	// Token: 0x060002ED RID: 749 RVA: 0x0002BE49 File Offset: 0x0002A049
	public virtual Vector3 GetLocalVelocityServer()
	{
		return Vector3.zero;
	}

	// Token: 0x060002EE RID: 750 RVA: 0x0002BE50 File Offset: 0x0002A050
	public virtual Quaternion GetAngularVelocityServer()
	{
		return Quaternion.identity;
	}

	// Token: 0x060002EF RID: 751 RVA: 0x0002BE57 File Offset: 0x0002A057
	public void EnableGlobalBroadcast(bool wants)
	{
		if (this.globalBroadcast == wants)
		{
			return;
		}
		this.globalBroadcast = wants;
		this.UpdateNetworkGroup();
	}

	// Token: 0x060002F0 RID: 752 RVA: 0x0002BE70 File Offset: 0x0002A070
	public void EnableSaving(bool wants)
	{
		if (this.enableSaving == wants)
		{
			return;
		}
		this.enableSaving = wants;
		if (this.enableSaving)
		{
			if (!global::BaseEntity.saveList.Contains(this))
			{
				global::BaseEntity.saveList.Add(this);
				return;
			}
		}
		else
		{
			global::BaseEntity.saveList.Remove(this);
		}
	}

	// Token: 0x060002F1 RID: 753 RVA: 0x0002BEBC File Offset: 0x0002A0BC
	public override void ServerInit()
	{
		this._spawnable = base.GetComponent<global::Spawnable>();
		base.ServerInit();
		if (this.enableSaving && !global::BaseEntity.saveList.Contains(this))
		{
			global::BaseEntity.saveList.Add(this);
		}
		if (this.flags != (global::BaseEntity.Flags)0)
		{
			this.OnFlagsChanged((global::BaseEntity.Flags)0, this.flags);
		}
		if (this.syncPosition && this.PositionTickRate >= 0f)
		{
			if (this.PositionTickFixedTime)
			{
				base.InvokeRepeatingFixedTime(new Action(this.NetworkPositionTick));
			}
			else
			{
				base.InvokeRandomized(new Action(this.NetworkPositionTick), this.PositionTickRate, this.PositionTickRate - this.PositionTickRate * 0.05f, this.PositionTickRate * 0.05f);
			}
		}
		global::BaseEntity.Query.Server.Add(this);
	}

	// Token: 0x060002F2 RID: 754 RVA: 0x000063A5 File Offset: 0x000045A5
	public virtual void OnSensation(Sensation sensation)
	{
	}

	// Token: 0x17000056 RID: 86
	// (get) Token: 0x060002F3 RID: 755 RVA: 0x0002BF84 File Offset: 0x0002A184
	protected virtual float PositionTickRate
	{
		get
		{
			return 0.1f;
		}
	}

	// Token: 0x17000057 RID: 87
	// (get) Token: 0x060002F4 RID: 756 RVA: 0x00007A3C File Offset: 0x00005C3C
	protected virtual bool PositionTickFixedTime
	{
		get
		{
			return false;
		}
	}

	// Token: 0x060002F5 RID: 757 RVA: 0x0002BF8C File Offset: 0x0002A18C
	protected void NetworkPositionTick()
	{
		if (!base.transform.hasChanged)
		{
			if (this.ticksSinceStopped >= 6)
			{
				return;
			}
			this.ticksSinceStopped++;
		}
		else
		{
			this.ticksSinceStopped = 0;
		}
		this.TransformChanged();
		base.transform.hasChanged = false;
	}

	// Token: 0x060002F6 RID: 758 RVA: 0x0002BFDC File Offset: 0x0002A1DC
	private void TransformChanged()
	{
		if (global::BaseEntity.Query.Server != null)
		{
			global::BaseEntity.Query.Server.Move(this);
		}
		if (this.net == null)
		{
			return;
		}
		base.InvalidateNetworkCache();
		if (!this.globalBroadcast && !ValidBounds.Test(base.transform.position))
		{
			this.OnInvalidPosition();
			return;
		}
		if (this.syncPosition)
		{
			if (!this.isCallingUpdateNetworkGroup)
			{
				base.Invoke(new Action(this.UpdateNetworkGroup), 5f);
				this.isCallingUpdateNetworkGroup = true;
			}
			base.SendNetworkUpdate_Position();
			this.OnPositionalNetworkUpdate();
		}
	}

	// Token: 0x060002F7 RID: 759 RVA: 0x000063A5 File Offset: 0x000045A5
	public virtual void OnPositionalNetworkUpdate()
	{
	}

	// Token: 0x060002F8 RID: 760 RVA: 0x0002C068 File Offset: 0x0002A268
	public void DoMovingWithoutARigidBodyCheck()
	{
		if (this.doneMovingWithoutARigidBodyCheck > 10)
		{
			return;
		}
		this.doneMovingWithoutARigidBodyCheck++;
		if (this.doneMovingWithoutARigidBodyCheck < 10)
		{
			return;
		}
		if (base.GetComponent<Collider>() == null)
		{
			return;
		}
		if (base.GetComponent<Rigidbody>() == null)
		{
			Debug.LogWarning("Entity moving without a rigid body! (" + base.gameObject + ")", this);
		}
	}

	// Token: 0x060002F9 RID: 761 RVA: 0x0002C0D1 File Offset: 0x0002A2D1
	public override void Spawn()
	{
		base.Spawn();
		if (base.isServer)
		{
			base.gameObject.BroadcastOnParentSpawning();
		}
	}

	// Token: 0x060002FA RID: 762 RVA: 0x0002C0EC File Offset: 0x0002A2EC
	public void OnParentSpawning()
	{
		if (this.net != null)
		{
			return;
		}
		if (base.IsDestroyed)
		{
			return;
		}
		if (Rust.Application.isLoadingSave)
		{
			UnityEngine.Object.Destroy(base.gameObject);
			return;
		}
		if (GameManager.server.preProcessed.NeedsProcessing(base.gameObject))
		{
			GameManager.server.preProcessed.ProcessObject(null, base.gameObject, false);
		}
		global::BaseEntity baseEntity = (base.transform.parent != null) ? base.transform.parent.GetComponentInParent<global::BaseEntity>() : null;
		this.Spawn();
		if (baseEntity != null)
		{
			this.SetParent(baseEntity, true, false);
		}
	}

	// Token: 0x060002FB RID: 763 RVA: 0x0002C18C File Offset: 0x0002A38C
	public void SpawnAsMapEntity()
	{
		if (this.net != null)
		{
			return;
		}
		if (base.IsDestroyed)
		{
			return;
		}
		if (((base.transform.parent != null) ? base.transform.parent.GetComponentInParent<global::BaseEntity>() : null) == null)
		{
			if (GameManager.server.preProcessed.NeedsProcessing(base.gameObject))
			{
				GameManager.server.preProcessed.ProcessObject(null, base.gameObject, false);
			}
			base.transform.parent = null;
			SceneManager.MoveGameObjectToScene(base.gameObject, Rust.Server.EntityScene);
			base.gameObject.SetActive(true);
			this.Spawn();
		}
	}

	// Token: 0x060002FC RID: 764 RVA: 0x000063A5 File Offset: 0x000045A5
	public virtual void PostMapEntitySpawn()
	{
	}

	// Token: 0x060002FD RID: 765 RVA: 0x0002C238 File Offset: 0x0002A438
	internal override void DoServerDestroy()
	{
		base.CancelInvoke(new Action(this.NetworkPositionTick));
		global::BaseEntity.saveList.Remove(this);
		this.RemoveFromTriggers();
		if (this.children != null)
		{
			global::BaseEntity[] array = this.children.ToArray();
			for (int i = 0; i < array.Length; i++)
			{
				array[i].OnParentRemoved();
			}
		}
		this.SetParent(null, true, false);
		global::BaseEntity.Query.Server.Remove(this, false);
		base.DoServerDestroy();
	}

	// Token: 0x060002FE RID: 766 RVA: 0x00003384 File Offset: 0x00001584
	internal virtual void OnParentRemoved()
	{
		base.Kill(global::BaseNetworkable.DestroyMode.None);
	}

	// Token: 0x060002FF RID: 767 RVA: 0x0002C2B0 File Offset: 0x0002A4B0
	public virtual void OnInvalidPosition()
	{
		Debug.Log(string.Concat(new object[]
		{
			"Invalid Position: ",
			this,
			" ",
			base.transform.position,
			" (destroying)"
		}));
		base.Kill(global::BaseNetworkable.DestroyMode.None);
	}

	// Token: 0x06000300 RID: 768 RVA: 0x0002C304 File Offset: 0x0002A504
	public BaseCorpse DropCorpse(string strCorpsePrefab)
	{
		Assert.IsTrue(base.isServer, "DropCorpse called on client!");
		if (!ConVar.Server.corpses)
		{
			return null;
		}
		if (string.IsNullOrEmpty(strCorpsePrefab))
		{
			return null;
		}
		BaseCorpse baseCorpse = GameManager.server.CreateEntity(strCorpsePrefab, default(Vector3), default(Quaternion), true) as BaseCorpse;
		if (baseCorpse == null)
		{
			Debug.LogWarning(string.Concat(new object[]
			{
				"Error creating corpse: ",
				base.gameObject,
				" - ",
				strCorpsePrefab
			}));
			return null;
		}
		baseCorpse.InitCorpse(this);
		return baseCorpse;
	}

	// Token: 0x06000301 RID: 769 RVA: 0x0002C398 File Offset: 0x0002A598
	public override void UpdateNetworkGroup()
	{
		Assert.IsTrue(base.isServer, "UpdateNetworkGroup called on clientside entity!");
		this.isCallingUpdateNetworkGroup = false;
		if (this.net == null)
		{
			return;
		}
		if (Network.Net.sv == null)
		{
			return;
		}
		if (Network.Net.sv.visibility == null)
		{
			return;
		}
		using (TimeWarning.New("UpdateNetworkGroup", 0))
		{
			if (this.globalBroadcast)
			{
				if (this.net.SwitchGroup(global::BaseNetworkable.GlobalNetworkGroup))
				{
					base.SendNetworkGroupChange();
				}
			}
			else if (this.ShouldInheritNetworkGroup() && this.parentEntity.IsSet())
			{
				global::BaseEntity parentEntity = base.GetParentEntity();
				if (!parentEntity.IsValid())
				{
					if (!Rust.Application.isLoadingSave)
					{
						Debug.LogWarning("UpdateNetworkGroup: Missing parent entity " + this.parentEntity.uid);
						base.Invoke(new Action(this.UpdateNetworkGroup), 2f);
						this.isCallingUpdateNetworkGroup = true;
					}
				}
				else if (parentEntity != null)
				{
					if (this.net.SwitchGroup(parentEntity.net.group))
					{
						base.SendNetworkGroupChange();
					}
				}
				else
				{
					Debug.LogWarning(base.gameObject + ": has parent id - but couldn't find parent! " + this.parentEntity);
				}
			}
			else if (base.limitNetworking)
			{
				if (this.net.SwitchGroup(global::BaseNetworkable.LimboNetworkGroup))
				{
					base.SendNetworkGroupChange();
				}
			}
			else
			{
				base.UpdateNetworkGroup();
			}
		}
	}

	// Token: 0x06000302 RID: 770 RVA: 0x0002C51C File Offset: 0x0002A71C
	public virtual void Eat(BaseNpc baseNpc, float timeSpent)
	{
		baseNpc.AddCalories(100f);
	}

	// Token: 0x06000303 RID: 771 RVA: 0x000063A5 File Offset: 0x000045A5
	public virtual void OnDeployed(global::BaseEntity parent, global::BasePlayer deployedBy, global::Item fromItem)
	{
	}

	// Token: 0x06000304 RID: 772 RVA: 0x0002C52C File Offset: 0x0002A72C
	public override bool ShouldNetworkTo(global::BasePlayer player)
	{
		if (player == this)
		{
			return true;
		}
		global::BaseEntity parentEntity = base.GetParentEntity();
		if (base.limitNetworking)
		{
			if (parentEntity == null)
			{
				return false;
			}
			if (parentEntity != player)
			{
				return false;
			}
		}
		if (parentEntity != null)
		{
			return parentEntity.ShouldNetworkTo(player);
		}
		return base.ShouldNetworkTo(player);
	}

	// Token: 0x06000305 RID: 773 RVA: 0x0002C581 File Offset: 0x0002A781
	public virtual void AttackerInfo(PlayerLifeStory.DeathInfo info)
	{
		info.attackerName = base.ShortPrefabName;
		info.attackerSteamID = 0UL;
		info.inflictorName = "";
	}

	// Token: 0x06000306 RID: 774 RVA: 0x0002C5A2 File Offset: 0x0002A7A2
	public virtual void Push(Vector3 velocity)
	{
		this.SetVelocity(velocity);
	}

	// Token: 0x06000307 RID: 775 RVA: 0x0002C5AC File Offset: 0x0002A7AC
	public virtual void ApplyInheritedVelocity(Vector3 velocity)
	{
		Rigidbody component = base.GetComponent<Rigidbody>();
		if (component)
		{
			component.velocity = Vector3.Lerp(component.velocity, velocity, 10f * UnityEngine.Time.fixedDeltaTime);
			component.angularVelocity *= Mathf.Clamp01(1f - 10f * UnityEngine.Time.fixedDeltaTime);
			component.AddForce(-UnityEngine.Physics.gravity * Mathf.Clamp01(0.9f), ForceMode.Acceleration);
		}
	}

	// Token: 0x06000308 RID: 776 RVA: 0x0002C62C File Offset: 0x0002A82C
	public virtual void SetVelocity(Vector3 velocity)
	{
		Rigidbody component = base.GetComponent<Rigidbody>();
		if (component)
		{
			component.velocity = velocity;
		}
	}

	// Token: 0x06000309 RID: 777 RVA: 0x0002C650 File Offset: 0x0002A850
	public virtual void SetAngularVelocity(Vector3 velocity)
	{
		Rigidbody component = base.GetComponent<Rigidbody>();
		if (component)
		{
			component.angularVelocity = velocity;
		}
	}

	// Token: 0x0600030A RID: 778 RVA: 0x0002C673 File Offset: 0x0002A873
	public virtual Vector3 GetDropPosition()
	{
		return base.transform.position;
	}

	// Token: 0x0600030B RID: 779 RVA: 0x0002C680 File Offset: 0x0002A880
	public virtual Vector3 GetDropVelocity()
	{
		return this.GetInheritedDropVelocity() + Vector3.up;
	}

	// Token: 0x0600030C RID: 780 RVA: 0x0000441C File Offset: 0x0000261C
	public virtual bool OnStartBeingLooted(global::BasePlayer baseEntity)
	{
		return true;
	}

	// Token: 0x17000058 RID: 88
	// (get) Token: 0x0600030D RID: 781 RVA: 0x0002C692 File Offset: 0x0002A892
	// (set) Token: 0x0600030E RID: 782 RVA: 0x0002C69F File Offset: 0x0002A89F
	public virtual Vector3 ServerPosition
	{
		get
		{
			return base.transform.localPosition;
		}
		set
		{
			if (base.transform.localPosition == value)
			{
				return;
			}
			base.transform.localPosition = value;
			base.transform.hasChanged = true;
		}
	}

	// Token: 0x17000059 RID: 89
	// (get) Token: 0x0600030F RID: 783 RVA: 0x0002C6CD File Offset: 0x0002A8CD
	// (set) Token: 0x06000310 RID: 784 RVA: 0x0002C6DA File Offset: 0x0002A8DA
	public virtual Quaternion ServerRotation
	{
		get
		{
			return base.transform.localRotation;
		}
		set
		{
			if (base.transform.localRotation == value)
			{
				return;
			}
			base.transform.localRotation = value;
			base.transform.hasChanged = true;
		}
	}

	// Token: 0x06000311 RID: 785 RVA: 0x0002C708 File Offset: 0x0002A908
	public virtual string Admin_Who()
	{
		return string.Format("Owner ID: {0}", this.OwnerID);
	}

	// Token: 0x06000312 RID: 786 RVA: 0x0002C720 File Offset: 0x0002A920
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.FromOwner]
	private void BroadcastSignalFromClient(global::BaseEntity.RPCMessage msg)
	{
		uint num = StringPool.Get("BroadcastSignalFromClient");
		if (num == 0U)
		{
			return;
		}
		global::BasePlayer player = msg.player;
		if (player == null)
		{
			return;
		}
		if (!player.rpcHistory.TryIncrement(num, (ulong)((long)ConVar.Server.maxpacketspersecond_rpc_signal)))
		{
			return;
		}
		global::BaseEntity.Signal signal = (global::BaseEntity.Signal)msg.read.Int32();
		string arg = msg.read.String(256);
		this.SignalBroadcast(signal, arg, msg.connection);
	}

	// Token: 0x06000313 RID: 787 RVA: 0x0002C790 File Offset: 0x0002A990
	public void SignalBroadcast(global::BaseEntity.Signal signal, string arg, Connection sourceConnection = null)
	{
		if (this.net == null)
		{
			return;
		}
		if (this.net.group == null)
		{
			return;
		}
		this.ClientRPCEx<int, string>(new SendInfo(this.net.group.subscribers)
		{
			method = SendMethod.Unreliable,
			priority = Priority.Immediate
		}, sourceConnection, "SignalFromServerEx", (int)signal, arg);
	}

	// Token: 0x06000314 RID: 788 RVA: 0x0002C7EC File Offset: 0x0002A9EC
	public void SignalBroadcast(global::BaseEntity.Signal signal, Connection sourceConnection = null)
	{
		if (this.net == null)
		{
			return;
		}
		if (this.net.group == null)
		{
			return;
		}
		this.ClientRPCEx<int>(new SendInfo(this.net.group.subscribers)
		{
			method = SendMethod.Unreliable,
			priority = Priority.Immediate
		}, sourceConnection, "SignalFromServer", (int)signal);
	}

	// Token: 0x06000315 RID: 789 RVA: 0x0002C845 File Offset: 0x0002AA45
	protected virtual void OnSkinChanged(ulong oldSkinID, ulong newSkinID)
	{
		if (oldSkinID == newSkinID)
		{
			return;
		}
		this.skinID = newSkinID;
	}

	// Token: 0x06000316 RID: 790 RVA: 0x0002C853 File Offset: 0x0002AA53
	protected virtual void OnSkinPreProcess(IPrefabProcessor preProcess, GameObject rootObj, string name, bool serverside, bool clientside, bool bundling)
	{
		if (clientside && Skinnable.All != null && Skinnable.FindForEntity(name) != null)
		{
			Rust.Workshop.WorkshopSkin.Prepare(rootObj);
			MaterialReplacement.Prepare(rootObj);
		}
	}

	// Token: 0x06000317 RID: 791 RVA: 0x0002C87A File Offset: 0x0002AA7A
	public virtual void PreProcess(IPrefabProcessor preProcess, GameObject rootObj, string name, bool serverside, bool clientside, bool bundling)
	{
		this.OnSkinPreProcess(preProcess, rootObj, name, serverside, clientside, bundling);
	}

	// Token: 0x06000318 RID: 792 RVA: 0x0002C88C File Offset: 0x0002AA8C
	public bool HasAnySlot()
	{
		for (int i = 0; i < this.entitySlots.Length; i++)
		{
			if (this.entitySlots[i].IsValid(base.isServer))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06000319 RID: 793 RVA: 0x0002C8C8 File Offset: 0x0002AAC8
	public global::BaseEntity GetSlot(global::BaseEntity.Slot slot)
	{
		return this.entitySlots[(int)slot].Get(base.isServer);
	}

	// Token: 0x0600031A RID: 794 RVA: 0x0002C8E1 File Offset: 0x0002AAE1
	public string GetSlotAnchorName(global::BaseEntity.Slot slot)
	{
		return slot.ToString().ToLower();
	}

	// Token: 0x0600031B RID: 795 RVA: 0x0002C8F5 File Offset: 0x0002AAF5
	public void SetSlot(global::BaseEntity.Slot slot, global::BaseEntity ent)
	{
		this.entitySlots[(int)slot].Set(ent);
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x0600031C RID: 796 RVA: 0x0002C910 File Offset: 0x0002AB10
	public EntityRef[] GetSlots()
	{
		return this.entitySlots;
	}

	// Token: 0x0600031D RID: 797 RVA: 0x0002C918 File Offset: 0x0002AB18
	public void SetSlots(EntityRef[] newSlots)
	{
		this.entitySlots = newSlots;
	}

	// Token: 0x0600031E RID: 798 RVA: 0x00007A3C File Offset: 0x00005C3C
	public virtual bool HasSlot(global::BaseEntity.Slot slot)
	{
		return false;
	}

	// Token: 0x1700005A RID: 90
	// (get) Token: 0x0600031F RID: 799 RVA: 0x00007A3C File Offset: 0x00005C3C
	public virtual global::BaseEntity.TraitFlag Traits
	{
		get
		{
			return global::BaseEntity.TraitFlag.None;
		}
	}

	// Token: 0x06000320 RID: 800 RVA: 0x0002C921 File Offset: 0x0002AB21
	public bool HasTrait(global::BaseEntity.TraitFlag f)
	{
		return (this.Traits & f) == f;
	}

	// Token: 0x06000321 RID: 801 RVA: 0x0002C92E File Offset: 0x0002AB2E
	public bool HasAnyTrait(global::BaseEntity.TraitFlag f)
	{
		return (this.Traits & f) > global::BaseEntity.TraitFlag.None;
	}

	// Token: 0x06000322 RID: 802 RVA: 0x0002C93B File Offset: 0x0002AB3B
	public virtual bool EnterTrigger(TriggerBase trigger)
	{
		if (this.triggers == null)
		{
			this.triggers = Facepunch.Pool.Get<List<TriggerBase>>();
		}
		this.triggers.Add(trigger);
		return true;
	}

	// Token: 0x06000323 RID: 803 RVA: 0x0002C95D File Offset: 0x0002AB5D
	public virtual void LeaveTrigger(TriggerBase trigger)
	{
		if (this.triggers == null)
		{
			return;
		}
		this.triggers.Remove(trigger);
		if (this.triggers.Count == 0)
		{
			Facepunch.Pool.FreeList<TriggerBase>(ref this.triggers);
		}
	}

	// Token: 0x06000324 RID: 804 RVA: 0x0002C990 File Offset: 0x0002AB90
	public void RemoveFromTriggers()
	{
		if (this.triggers == null)
		{
			return;
		}
		using (TimeWarning.New("RemoveFromTriggers", 0))
		{
			foreach (TriggerBase triggerBase in this.triggers.ToArray())
			{
				if (triggerBase)
				{
					triggerBase.RemoveEntity(this);
				}
			}
			if (this.triggers != null && this.triggers.Count == 0)
			{
				Facepunch.Pool.FreeList<TriggerBase>(ref this.triggers);
			}
		}
	}

	// Token: 0x06000325 RID: 805 RVA: 0x0002CA1C File Offset: 0x0002AC1C
	public T FindTrigger<T>() where T : TriggerBase
	{
		if (this.triggers == null)
		{
			return default(T);
		}
		foreach (TriggerBase triggerBase in this.triggers)
		{
			if (!(triggerBase as T == null))
			{
				return triggerBase as T;
			}
		}
		return default(T);
	}

	// Token: 0x06000326 RID: 806 RVA: 0x0002CAAC File Offset: 0x0002ACAC
	public bool FindTrigger<T>(out T result) where T : TriggerBase
	{
		result = this.FindTrigger<T>();
		return result != null;
	}

	// Token: 0x06000327 RID: 807 RVA: 0x0002CACB File Offset: 0x0002ACCB
	private void ForceUpdateTriggersAction()
	{
		if (!base.IsDestroyed)
		{
			this.ForceUpdateTriggers(false, true, false);
		}
	}

	// Token: 0x06000328 RID: 808 RVA: 0x0002CAE0 File Offset: 0x0002ACE0
	public void ForceUpdateTriggers(bool enter = true, bool exit = true, bool invoke = true)
	{
		List<TriggerBase> list = Facepunch.Pool.GetList<TriggerBase>();
		List<TriggerBase> list2 = Facepunch.Pool.GetList<TriggerBase>();
		if (this.triggers != null)
		{
			list.AddRange(this.triggers);
		}
		Collider componentInChildren = base.GetComponentInChildren<Collider>();
		if (componentInChildren is CapsuleCollider)
		{
			CapsuleCollider capsuleCollider = componentInChildren as CapsuleCollider;
			Vector3 point = base.transform.position + new Vector3(0f, capsuleCollider.radius, 0f);
			Vector3 point2 = base.transform.position + new Vector3(0f, capsuleCollider.height - capsuleCollider.radius, 0f);
			GamePhysics.OverlapCapsule<TriggerBase>(point, point2, capsuleCollider.radius, list2, 262144, QueryTriggerInteraction.Collide);
		}
		else if (componentInChildren is BoxCollider)
		{
			BoxCollider boxCollider = componentInChildren as BoxCollider;
			GamePhysics.OverlapOBB<TriggerBase>(new OBB(base.transform.position, base.transform.lossyScale, base.transform.rotation, new Bounds(boxCollider.center, boxCollider.size)), list2, 262144, QueryTriggerInteraction.Collide);
		}
		else if (componentInChildren is SphereCollider)
		{
			SphereCollider sphereCollider = componentInChildren as SphereCollider;
			GamePhysics.OverlapSphere<TriggerBase>(base.transform.TransformPoint(sphereCollider.center), sphereCollider.radius, list2, 262144, QueryTriggerInteraction.Collide);
		}
		else
		{
			list2.AddRange(list);
		}
		if (exit)
		{
			foreach (TriggerBase triggerBase in list)
			{
				if (!list2.Contains(triggerBase))
				{
					triggerBase.OnTriggerExit(componentInChildren);
				}
			}
		}
		if (enter)
		{
			foreach (TriggerBase triggerBase2 in list2)
			{
				if (!list.Contains(triggerBase2))
				{
					triggerBase2.OnTriggerEnter(componentInChildren);
				}
			}
		}
		Facepunch.Pool.FreeList<TriggerBase>(ref list);
		Facepunch.Pool.FreeList<TriggerBase>(ref list2);
		if (invoke)
		{
			base.Invoke(new Action(this.ForceUpdateTriggersAction), UnityEngine.Time.time - UnityEngine.Time.fixedTime + UnityEngine.Time.fixedDeltaTime * 1.5f);
		}
	}

	// Token: 0x06000329 RID: 809 RVA: 0x0002CCFC File Offset: 0x0002AEFC
	public TriggerParent FindSuitableParent()
	{
		if (this.triggers == null)
		{
			return null;
		}
		using (List<TriggerBase>.Enumerator enumerator = this.triggers.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				TriggerParent triggerParent;
				if ((triggerParent = (enumerator.Current as TriggerParent)) != null && triggerParent.ShouldParent(this, true))
				{
					return triggerParent;
				}
			}
		}
		return null;
	}

	// Token: 0x1700005B RID: 91
	// (get) Token: 0x0600032A RID: 810 RVA: 0x0002CD6C File Offset: 0x0002AF6C
	// (set) Token: 0x0600032B RID: 811 RVA: 0x0002CD74 File Offset: 0x0002AF74
	public float Weight { get; protected set; }

	// Token: 0x1700005C RID: 92
	// (get) Token: 0x0600032C RID: 812 RVA: 0x0002CD80 File Offset: 0x0002AF80
	public EntityComponentBase[] Components
	{
		get
		{
			EntityComponentBase[] result;
			if ((result = this._components) == null)
			{
				result = (this._components = base.GetComponentsInChildren<EntityComponentBase>(true));
			}
			return result;
		}
	}

	// Token: 0x0600032D RID: 813 RVA: 0x0002CDA7 File Offset: 0x0002AFA7
	public virtual global::BasePlayer ToPlayer()
	{
		return null;
	}

	// Token: 0x1700005D RID: 93
	// (get) Token: 0x0600032E RID: 814 RVA: 0x00007A3C File Offset: 0x00005C3C
	public virtual bool IsNpc
	{
		get
		{
			return false;
		}
	}

	// Token: 0x0600032F RID: 815 RVA: 0x0002CDAA File Offset: 0x0002AFAA
	public override void InitShared()
	{
		base.InitShared();
		this.InitEntityLinks();
	}

	// Token: 0x06000330 RID: 816 RVA: 0x0002CDB8 File Offset: 0x0002AFB8
	public override void DestroyShared()
	{
		base.DestroyShared();
		this.FreeEntityLinks();
	}

	// Token: 0x06000331 RID: 817 RVA: 0x0002CDC6 File Offset: 0x0002AFC6
	public override void ResetState()
	{
		base.ResetState();
		this.parentBone = 0U;
		this.OwnerID = 0UL;
		this.flags = (global::BaseEntity.Flags)0;
		this.parentEntity = default(EntityRef);
		if (base.isServer)
		{
			this._spawnable = null;
		}
	}

	// Token: 0x06000332 RID: 818 RVA: 0x00029CA8 File Offset: 0x00027EA8
	public virtual float InheritedVelocityScale()
	{
		return 0f;
	}

	// Token: 0x06000333 RID: 819 RVA: 0x0000441C File Offset: 0x0000261C
	public virtual bool InheritedVelocityDirection()
	{
		return true;
	}

	// Token: 0x06000334 RID: 820 RVA: 0x0002CE00 File Offset: 0x0002B000
	public virtual Vector3 GetInheritedProjectileVelocity(Vector3 direction)
	{
		global::BaseEntity baseEntity = this.parentEntity.Get(base.isServer);
		if (baseEntity == null)
		{
			return Vector3.zero;
		}
		if (baseEntity.InheritedVelocityDirection())
		{
			return this.GetParentVelocity() * baseEntity.InheritedVelocityScale();
		}
		return Mathf.Max(Vector3.Dot(this.GetParentVelocity() * baseEntity.InheritedVelocityScale(), direction), 0f) * direction;
	}

	// Token: 0x06000335 RID: 821 RVA: 0x0002CE6F File Offset: 0x0002B06F
	public virtual Vector3 GetInheritedThrowVelocity(Vector3 direction)
	{
		return this.GetParentVelocity();
	}

	// Token: 0x06000336 RID: 822 RVA: 0x0002CE78 File Offset: 0x0002B078
	public virtual Vector3 GetInheritedDropVelocity()
	{
		global::BaseEntity baseEntity = this.parentEntity.Get(base.isServer);
		if (!(baseEntity != null))
		{
			return Vector3.zero;
		}
		return baseEntity.GetWorldVelocity();
	}

	// Token: 0x06000337 RID: 823 RVA: 0x0002CEAC File Offset: 0x0002B0AC
	public Vector3 GetParentVelocity()
	{
		global::BaseEntity baseEntity = this.parentEntity.Get(base.isServer);
		if (!(baseEntity != null))
		{
			return Vector3.zero;
		}
		return baseEntity.GetWorldVelocity() + (baseEntity.GetAngularVelocity() * base.transform.localPosition - base.transform.localPosition);
	}

	// Token: 0x06000338 RID: 824 RVA: 0x0002CF0C File Offset: 0x0002B10C
	public Vector3 GetWorldVelocity()
	{
		global::BaseEntity baseEntity = this.parentEntity.Get(base.isServer);
		if (!(baseEntity != null))
		{
			return this.GetLocalVelocity();
		}
		return baseEntity.GetWorldVelocity() + (baseEntity.GetAngularVelocity() * base.transform.localPosition - base.transform.localPosition) + baseEntity.transform.TransformDirection(this.GetLocalVelocity());
	}

	// Token: 0x06000339 RID: 825 RVA: 0x0002CF82 File Offset: 0x0002B182
	public Vector3 GetLocalVelocity()
	{
		if (base.isServer)
		{
			return this.GetLocalVelocityServer();
		}
		return Vector3.zero;
	}

	// Token: 0x0600033A RID: 826 RVA: 0x0002CF98 File Offset: 0x0002B198
	public Quaternion GetAngularVelocity()
	{
		if (base.isServer)
		{
			return this.GetAngularVelocityServer();
		}
		return Quaternion.identity;
	}

	// Token: 0x0600033B RID: 827 RVA: 0x0002CFAE File Offset: 0x0002B1AE
	public virtual OBB WorldSpaceBounds()
	{
		return new OBB(base.transform.position, base.transform.lossyScale, base.transform.rotation, this.bounds);
	}

	// Token: 0x0600033C RID: 828 RVA: 0x0002C673 File Offset: 0x0002A873
	public Vector3 PivotPoint()
	{
		return base.transform.position;
	}

	// Token: 0x0600033D RID: 829 RVA: 0x0002CFDC File Offset: 0x0002B1DC
	public Vector3 CenterPoint()
	{
		return this.WorldSpaceBounds().position;
	}

	// Token: 0x0600033E RID: 830 RVA: 0x0002CFEC File Offset: 0x0002B1EC
	public Vector3 ClosestPoint(Vector3 position)
	{
		return this.WorldSpaceBounds().ClosestPoint(position);
	}

	// Token: 0x0600033F RID: 831 RVA: 0x0002D008 File Offset: 0x0002B208
	public virtual Vector3 TriggerPoint()
	{
		return this.CenterPoint();
	}

	// Token: 0x06000340 RID: 832 RVA: 0x0002D010 File Offset: 0x0002B210
	public float Distance(Vector3 position)
	{
		return (this.ClosestPoint(position) - position).magnitude;
	}

	// Token: 0x06000341 RID: 833 RVA: 0x0002D034 File Offset: 0x0002B234
	public float SqrDistance(Vector3 position)
	{
		return (this.ClosestPoint(position) - position).sqrMagnitude;
	}

	// Token: 0x06000342 RID: 834 RVA: 0x0002D056 File Offset: 0x0002B256
	public float Distance(global::BaseEntity other)
	{
		return this.Distance(other.transform.position);
	}

	// Token: 0x06000343 RID: 835 RVA: 0x0002D069 File Offset: 0x0002B269
	public float SqrDistance(global::BaseEntity other)
	{
		return this.SqrDistance(other.transform.position);
	}

	// Token: 0x06000344 RID: 836 RVA: 0x0002D07C File Offset: 0x0002B27C
	public float Distance2D(Vector3 position)
	{
		return (this.ClosestPoint(position) - position).Magnitude2D();
	}

	// Token: 0x06000345 RID: 837 RVA: 0x0002D090 File Offset: 0x0002B290
	public float SqrDistance2D(Vector3 position)
	{
		return (this.ClosestPoint(position) - position).SqrMagnitude2D();
	}

	// Token: 0x06000346 RID: 838 RVA: 0x0002D056 File Offset: 0x0002B256
	public float Distance2D(global::BaseEntity other)
	{
		return this.Distance(other.transform.position);
	}

	// Token: 0x06000347 RID: 839 RVA: 0x0002D069 File Offset: 0x0002B269
	public float SqrDistance2D(global::BaseEntity other)
	{
		return this.SqrDistance(other.transform.position);
	}

	// Token: 0x06000348 RID: 840 RVA: 0x0002D0A4 File Offset: 0x0002B2A4
	public bool IsVisible(Ray ray, int layerMask, float maxDistance)
	{
		if (ray.origin.IsNaNOrInfinity())
		{
			return false;
		}
		if (ray.direction.IsNaNOrInfinity())
		{
			return false;
		}
		if (ray.direction == Vector3.zero)
		{
			return false;
		}
		RaycastHit raycastHit;
		if (!this.WorldSpaceBounds().Trace(ray, out raycastHit, maxDistance))
		{
			return false;
		}
		RaycastHit hit;
		if (GamePhysics.Trace(ray, 0f, out hit, maxDistance, layerMask, QueryTriggerInteraction.UseGlobal, null))
		{
			global::BaseEntity entity = hit.GetEntity();
			if (entity == this)
			{
				return true;
			}
			if (entity != null && base.GetParentEntity() && base.GetParentEntity().EqualNetID(entity) && hit.IsOnLayer(Rust.Layer.Vehicle_Detailed))
			{
				return true;
			}
			if (hit.distance <= raycastHit.distance)
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x06000349 RID: 841 RVA: 0x0002D164 File Offset: 0x0002B364
	public bool IsVisibleSpecificLayers(Vector3 position, Vector3 target, int layerMask, float maxDistance = float.PositiveInfinity)
	{
		Vector3 a = target - position;
		float magnitude = a.magnitude;
		if (magnitude < Mathf.Epsilon)
		{
			return true;
		}
		Vector3 vector = a / magnitude;
		Vector3 b = vector * Mathf.Min(magnitude, 0.01f);
		return this.IsVisible(new Ray(position + b, vector), layerMask, maxDistance);
	}

	// Token: 0x0600034A RID: 842 RVA: 0x0002D1BC File Offset: 0x0002B3BC
	public bool IsVisible(Vector3 position, Vector3 target, float maxDistance = float.PositiveInfinity)
	{
		Vector3 a = target - position;
		float magnitude = a.magnitude;
		if (magnitude < Mathf.Epsilon)
		{
			return true;
		}
		Vector3 vector = a / magnitude;
		Vector3 b = vector * Mathf.Min(magnitude, 0.01f);
		return this.IsVisible(new Ray(position + b, vector), 1218519041, maxDistance);
	}

	// Token: 0x0600034B RID: 843 RVA: 0x0002D218 File Offset: 0x0002B418
	public bool IsVisible(Vector3 position, float maxDistance = float.PositiveInfinity)
	{
		Vector3 target = this.CenterPoint();
		if (this.IsVisible(position, target, maxDistance))
		{
			return true;
		}
		Vector3 target2 = this.ClosestPoint(position);
		return this.IsVisible(position, target2, maxDistance);
	}

	// Token: 0x0600034C RID: 844 RVA: 0x0002D250 File Offset: 0x0002B450
	public bool IsVisibleAndCanSee(Vector3 position, float maxDistance = float.PositiveInfinity)
	{
		Vector3 vector = this.CenterPoint();
		if (this.IsVisible(position, vector, maxDistance) && this.IsVisible(vector, position, maxDistance))
		{
			return true;
		}
		Vector3 vector2 = this.ClosestPoint(position);
		return this.IsVisible(position, vector2, maxDistance) && this.IsVisible(vector2, position, maxDistance);
	}

	// Token: 0x0600034D RID: 845 RVA: 0x0002D2A0 File Offset: 0x0002B4A0
	public bool IsOlderThan(global::BaseEntity other)
	{
		if (other == null)
		{
			return true;
		}
		Networkable net = this.net;
		ref NetworkableId ptr = (net != null) ? net.ID : default(NetworkableId);
		Networkable net2 = other.net;
		NetworkableId networkableId = (net2 != null) ? net2.ID : default(NetworkableId);
		return ptr.Value < networkableId.Value;
	}

	// Token: 0x0600034E RID: 846 RVA: 0x0002D2FC File Offset: 0x0002B4FC
	public virtual bool IsOutside()
	{
		OBB obb = this.WorldSpaceBounds();
		return this.IsOutside(obb.position);
	}

	// Token: 0x0600034F RID: 847 RVA: 0x0002D31C File Offset: 0x0002B51C
	public bool IsOutside(Vector3 position)
	{
		bool result = true;
		bool flag;
		do
		{
			flag = false;
			RaycastHit raycastHit;
			if (UnityEngine.Physics.Raycast(position, Vector3.up, out raycastHit, 100f, 161546513))
			{
				global::BaseEntity baseEntity = raycastHit.collider.ToBaseEntity();
				if (baseEntity != null && baseEntity.HasEntityInParents(this))
				{
					if (raycastHit.point.y > position.y + 0.2f)
					{
						position = raycastHit.point + Vector3.up * 0.05f;
					}
					else
					{
						position.y += 0.2f;
					}
					flag = true;
				}
				else
				{
					result = false;
				}
			}
		}
		while (flag);
		return result;
	}

	// Token: 0x06000350 RID: 848 RVA: 0x0002D3BC File Offset: 0x0002B5BC
	public virtual float WaterFactor()
	{
		return WaterLevel.Factor(this.WorldSpaceBounds().ToBounds(), this);
	}

	// Token: 0x06000351 RID: 849 RVA: 0x0002D3DD File Offset: 0x0002B5DD
	public virtual float AirFactor()
	{
		if (this.WaterFactor() <= 0.85f)
		{
			return 1f;
		}
		return 0f;
	}

	// Token: 0x06000352 RID: 850 RVA: 0x0002D3F8 File Offset: 0x0002B5F8
	public bool WaterTestFromVolumes(Vector3 pos, out WaterLevel.WaterInfo info)
	{
		if (this.triggers == null)
		{
			info = default(WaterLevel.WaterInfo);
			return false;
		}
		for (int i = 0; i < this.triggers.Count; i++)
		{
			WaterVolume waterVolume;
			if ((waterVolume = (this.triggers[i] as WaterVolume)) != null && waterVolume.Test(pos, out info))
			{
				return true;
			}
		}
		info = default(WaterLevel.WaterInfo);
		return false;
	}

	// Token: 0x06000353 RID: 851 RVA: 0x0002D458 File Offset: 0x0002B658
	public bool IsInWaterVolume(Vector3 pos)
	{
		if (this.triggers == null)
		{
			return false;
		}
		WaterLevel.WaterInfo waterInfo = default(WaterLevel.WaterInfo);
		for (int i = 0; i < this.triggers.Count; i++)
		{
			WaterVolume waterVolume;
			if ((waterVolume = (this.triggers[i] as WaterVolume)) != null && waterVolume.Test(pos, out waterInfo))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06000354 RID: 852 RVA: 0x0002D4B0 File Offset: 0x0002B6B0
	public bool WaterTestFromVolumes(Bounds bounds, out WaterLevel.WaterInfo info)
	{
		if (this.triggers == null)
		{
			info = default(WaterLevel.WaterInfo);
			return false;
		}
		for (int i = 0; i < this.triggers.Count; i++)
		{
			WaterVolume waterVolume;
			if ((waterVolume = (this.triggers[i] as WaterVolume)) != null && waterVolume.Test(bounds, out info))
			{
				return true;
			}
		}
		info = default(WaterLevel.WaterInfo);
		return false;
	}

	// Token: 0x06000355 RID: 853 RVA: 0x0002D510 File Offset: 0x0002B710
	public bool WaterTestFromVolumes(Vector3 start, Vector3 end, float radius, out WaterLevel.WaterInfo info)
	{
		if (this.triggers == null)
		{
			info = default(WaterLevel.WaterInfo);
			return false;
		}
		for (int i = 0; i < this.triggers.Count; i++)
		{
			WaterVolume waterVolume;
			if ((waterVolume = (this.triggers[i] as WaterVolume)) != null && waterVolume.Test(start, end, radius, out info))
			{
				return true;
			}
		}
		info = default(WaterLevel.WaterInfo);
		return false;
	}

	// Token: 0x06000356 RID: 854 RVA: 0x00007A3C File Offset: 0x00005C3C
	public virtual bool BlocksWaterFor(global::BasePlayer player)
	{
		return false;
	}

	// Token: 0x06000357 RID: 855 RVA: 0x00029CA8 File Offset: 0x00027EA8
	public virtual float Health()
	{
		return 0f;
	}

	// Token: 0x06000358 RID: 856 RVA: 0x00029CA8 File Offset: 0x00027EA8
	public virtual float MaxHealth()
	{
		return 0f;
	}

	// Token: 0x06000359 RID: 857 RVA: 0x00029CA8 File Offset: 0x00027EA8
	public virtual float MaxVelocity()
	{
		return 0f;
	}

	// Token: 0x0600035A RID: 858 RVA: 0x0002BF84 File Offset: 0x0002A184
	public virtual float BoundsPadding()
	{
		return 0.1f;
	}

	// Token: 0x0600035B RID: 859 RVA: 0x0002A2EC File Offset: 0x000284EC
	public virtual float PenetrationResistance(HitInfo info)
	{
		return 100f;
	}

	// Token: 0x0600035C RID: 860 RVA: 0x0002D572 File Offset: 0x0002B772
	public virtual GameObjectRef GetImpactEffect(HitInfo info)
	{
		return this.impactEffect;
	}

	// Token: 0x0600035D RID: 861 RVA: 0x000063A5 File Offset: 0x000045A5
	public virtual void OnAttacked(HitInfo info)
	{
	}

	// Token: 0x0600035E RID: 862 RVA: 0x0002CDA7 File Offset: 0x0002AFA7
	public virtual global::Item GetItem()
	{
		return null;
	}

	// Token: 0x0600035F RID: 863 RVA: 0x0002CDA7 File Offset: 0x0002AFA7
	public virtual global::Item GetItem(ItemId itemId)
	{
		return null;
	}

	// Token: 0x06000360 RID: 864 RVA: 0x0002D57A File Offset: 0x0002B77A
	public virtual void GiveItem(global::Item item, global::BaseEntity.GiveItemReason reason = global::BaseEntity.GiveItemReason.Generic)
	{
		item.Remove(0f);
	}

	// Token: 0x06000361 RID: 865 RVA: 0x0000441C File Offset: 0x0000261C
	public virtual bool CanBeLooted(global::BasePlayer player)
	{
		return true;
	}

	// Token: 0x06000362 RID: 866 RVA: 0x000037E7 File Offset: 0x000019E7
	public virtual global::BaseEntity GetEntity()
	{
		return this;
	}

	// Token: 0x06000363 RID: 867 RVA: 0x0002D588 File Offset: 0x0002B788
	public override string ToString()
	{
		if (this._name == null)
		{
			if (base.isServer)
			{
				string format = "{1}[{0}]";
				Networkable net = this.net;
				this._name = string.Format(format, (net != null) ? net.ID : default(NetworkableId), base.ShortPrefabName);
			}
			else
			{
				this._name = base.ShortPrefabName;
			}
		}
		return this._name;
	}

	// Token: 0x06000364 RID: 868 RVA: 0x0002D5EE File Offset: 0x0002B7EE
	public virtual string Categorize()
	{
		return "entity";
	}

	// Token: 0x06000365 RID: 869 RVA: 0x0002D5F8 File Offset: 0x0002B7F8
	public void Log(string str)
	{
		if (base.isClient)
		{
			Debug.Log(string.Concat(new string[]
			{
				"<color=#ffa>[",
				this.ToString(),
				"] ",
				str,
				"</color>"
			}), base.gameObject);
			return;
		}
		Debug.Log(string.Concat(new string[]
		{
			"<color=#aff>[",
			this.ToString(),
			"] ",
			str,
			"</color>"
		}), base.gameObject);
	}

	// Token: 0x06000366 RID: 870 RVA: 0x0002D684 File Offset: 0x0002B884
	public void SetModel(Model mdl)
	{
		if (this.model == mdl)
		{
			return;
		}
		this.model = mdl;
	}

	// Token: 0x06000367 RID: 871 RVA: 0x0002D69C File Offset: 0x0002B89C
	public Model GetModel()
	{
		return this.model;
	}

	// Token: 0x06000368 RID: 872 RVA: 0x0002D6A4 File Offset: 0x0002B8A4
	public virtual Transform[] GetBones()
	{
		if (this.model)
		{
			return this.model.GetBones();
		}
		return null;
	}

	// Token: 0x06000369 RID: 873 RVA: 0x0002D6C0 File Offset: 0x0002B8C0
	public virtual Transform FindBone(string strName)
	{
		if (this.model)
		{
			return this.model.FindBone(strName);
		}
		return base.transform;
	}

	// Token: 0x0600036A RID: 874 RVA: 0x0002D6E2 File Offset: 0x0002B8E2
	public virtual uint FindBoneID(Transform boneTransform)
	{
		if (this.model)
		{
			return this.model.FindBoneID(boneTransform);
		}
		return StringPool.closest;
	}

	// Token: 0x0600036B RID: 875 RVA: 0x0002D703 File Offset: 0x0002B903
	public virtual Transform FindClosestBone(Vector3 worldPos)
	{
		if (this.model)
		{
			return this.model.FindClosestBone(worldPos);
		}
		return base.transform;
	}

	// Token: 0x1700005E RID: 94
	// (get) Token: 0x0600036C RID: 876 RVA: 0x0002D725 File Offset: 0x0002B925
	// (set) Token: 0x0600036D RID: 877 RVA: 0x0002D72D File Offset: 0x0002B92D
	public ulong OwnerID { get; set; }

	// Token: 0x0600036E RID: 878 RVA: 0x0000441C File Offset: 0x0000261C
	public virtual bool ShouldBlockProjectiles()
	{
		return true;
	}

	// Token: 0x0600036F RID: 879 RVA: 0x0000441C File Offset: 0x0000261C
	public virtual bool ShouldInheritNetworkGroup()
	{
		return true;
	}

	// Token: 0x06000370 RID: 880 RVA: 0x00007A3C File Offset: 0x00005C3C
	public virtual bool SupportsChildDeployables()
	{
		return false;
	}

	// Token: 0x06000371 RID: 881 RVA: 0x0002D738 File Offset: 0x0002B938
	public void BroadcastEntityMessage(string msg, float radius = 20f, int layerMask = 1218652417)
	{
		if (base.isClient)
		{
			return;
		}
		List<global::BaseEntity> list = Facepunch.Pool.GetList<global::BaseEntity>();
		global::Vis.Entities<global::BaseEntity>(base.transform.position, radius, list, layerMask, QueryTriggerInteraction.Collide);
		foreach (global::BaseEntity baseEntity in list)
		{
			if (baseEntity.isServer)
			{
				baseEntity.OnEntityMessage(this, msg);
			}
		}
		Facepunch.Pool.FreeList<global::BaseEntity>(ref list);
	}

	// Token: 0x06000372 RID: 882 RVA: 0x000063A5 File Offset: 0x000045A5
	public virtual void OnEntityMessage(global::BaseEntity from, string msg)
	{
	}

	// Token: 0x06000373 RID: 883 RVA: 0x0002D7BC File Offset: 0x0002B9BC
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		global::BaseEntity baseEntity = this.parentEntity.Get(base.isServer);
		info.msg.baseEntity = Facepunch.Pool.Get<ProtoBuf.BaseEntity>();
		if (info.forDisk)
		{
			if (this is global::BasePlayer)
			{
				if (baseEntity == null || baseEntity.enableSaving)
				{
					info.msg.baseEntity.pos = base.transform.localPosition;
					info.msg.baseEntity.rot = base.transform.localRotation.eulerAngles;
				}
				else
				{
					info.msg.baseEntity.pos = base.transform.position;
					info.msg.baseEntity.rot = base.transform.rotation.eulerAngles;
				}
			}
			else
			{
				info.msg.baseEntity.pos = base.transform.localPosition;
				info.msg.baseEntity.rot = base.transform.localRotation.eulerAngles;
			}
		}
		else
		{
			info.msg.baseEntity.pos = this.GetNetworkPosition();
			info.msg.baseEntity.rot = this.GetNetworkRotation().eulerAngles;
			info.msg.baseEntity.time = this.GetNetworkTime();
		}
		info.msg.baseEntity.flags = (int)this.flags;
		info.msg.baseEntity.skinid = this.skinID;
		if (info.forDisk && this is global::BasePlayer)
		{
			if (baseEntity != null && baseEntity.enableSaving)
			{
				info.msg.parent = Facepunch.Pool.Get<ParentInfo>();
				info.msg.parent.uid = this.parentEntity.uid;
				info.msg.parent.bone = this.parentBone;
			}
		}
		else if (baseEntity != null)
		{
			info.msg.parent = Facepunch.Pool.Get<ParentInfo>();
			info.msg.parent.uid = this.parentEntity.uid;
			info.msg.parent.bone = this.parentBone;
		}
		if (this.HasAnySlot())
		{
			info.msg.entitySlots = Facepunch.Pool.Get<EntitySlots>();
			info.msg.entitySlots.slotLock = this.entitySlots[0].uid;
			info.msg.entitySlots.slotFireMod = this.entitySlots[1].uid;
			info.msg.entitySlots.slotUpperModification = this.entitySlots[2].uid;
			info.msg.entitySlots.centerDecoration = this.entitySlots[5].uid;
			info.msg.entitySlots.lowerCenterDecoration = this.entitySlots[6].uid;
			info.msg.entitySlots.storageMonitor = this.entitySlots[7].uid;
		}
		if (info.forDisk && this._spawnable)
		{
			this._spawnable.Save(info);
		}
		if (this.OwnerID != 0UL && (info.forDisk || this.ShouldNetworkOwnerInfo()))
		{
			info.msg.ownerInfo = Facepunch.Pool.Get<OwnerInfo>();
			info.msg.ownerInfo.steamid = this.OwnerID;
		}
		if (this.Components != null)
		{
			for (int i = 0; i < this.Components.Length; i++)
			{
				if (!(this.Components[i] == null))
				{
					this.Components[i].SaveComponent(info);
				}
			}
		}
	}

	// Token: 0x06000374 RID: 884 RVA: 0x00007A3C File Offset: 0x00005C3C
	public virtual bool ShouldNetworkOwnerInfo()
	{
		return false;
	}

	// Token: 0x06000375 RID: 885 RVA: 0x0002DB84 File Offset: 0x0002BD84
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.baseEntity != null)
		{
			ProtoBuf.BaseEntity baseEntity = info.msg.baseEntity;
			global::BaseEntity.Flags old = this.flags;
			this.flags = (global::BaseEntity.Flags)baseEntity.flags;
			this.OnFlagsChanged(old, this.flags);
			this.OnSkinChanged(this.skinID, info.msg.baseEntity.skinid);
			if (info.fromDisk)
			{
				if (baseEntity.pos.IsNaNOrInfinity())
				{
					Debug.LogWarning(this.ToString() + " has broken position - " + baseEntity.pos);
					baseEntity.pos = Vector3.zero;
				}
				base.transform.localPosition = baseEntity.pos;
				base.transform.localRotation = Quaternion.Euler(baseEntity.rot);
			}
		}
		if (info.msg.entitySlots != null)
		{
			this.entitySlots[0].uid = info.msg.entitySlots.slotLock;
			this.entitySlots[1].uid = info.msg.entitySlots.slotFireMod;
			this.entitySlots[2].uid = info.msg.entitySlots.slotUpperModification;
			this.entitySlots[5].uid = info.msg.entitySlots.centerDecoration;
			this.entitySlots[6].uid = info.msg.entitySlots.lowerCenterDecoration;
			this.entitySlots[7].uid = info.msg.entitySlots.storageMonitor;
		}
		if (info.msg.parent != null)
		{
			if (base.isServer)
			{
				global::BaseEntity entity = global::BaseNetworkable.serverEntities.Find(info.msg.parent.uid) as global::BaseEntity;
				this.SetParent(entity, info.msg.parent.bone, false, false);
			}
			this.parentEntity.uid = info.msg.parent.uid;
			this.parentBone = info.msg.parent.bone;
		}
		else
		{
			this.parentEntity.uid = default(NetworkableId);
			this.parentBone = 0U;
		}
		if (info.msg.ownerInfo != null)
		{
			this.OwnerID = info.msg.ownerInfo.steamid;
		}
		if (this._spawnable)
		{
			this._spawnable.Load(info);
		}
		if (this.Components != null)
		{
			for (int i = 0; i < this.Components.Length; i++)
			{
				if (!(this.Components[i] == null))
				{
					this.Components[i].LoadComponent(info);
				}
			}
		}
	}

	// Token: 0x02000B76 RID: 2934
	public class Menu : Attribute
	{
		// Token: 0x04003F03 RID: 16131
		public string TitleToken;

		// Token: 0x04003F04 RID: 16132
		public string TitleEnglish;

		// Token: 0x04003F05 RID: 16133
		public string UseVariable;

		// Token: 0x04003F06 RID: 16134
		public int Order;

		// Token: 0x04003F07 RID: 16135
		public string ProxyFunction;

		// Token: 0x04003F08 RID: 16136
		public float Time;

		// Token: 0x04003F09 RID: 16137
		public string OnStart;

		// Token: 0x04003F0A RID: 16138
		public string OnProgress;

		// Token: 0x04003F0B RID: 16139
		public bool LongUseOnly;

		// Token: 0x04003F0C RID: 16140
		public bool PrioritizeIfNotWhitelisted;

		// Token: 0x04003F0D RID: 16141
		public bool PrioritizeIfUnlocked;

		// Token: 0x06004CF2 RID: 19698 RVA: 0x000031B9 File Offset: 0x000013B9
		public Menu()
		{
		}

		// Token: 0x06004CF3 RID: 19699 RVA: 0x0019FA20 File Offset: 0x0019DC20
		public Menu(string menuTitleToken, string menuTitleEnglish)
		{
			this.TitleToken = menuTitleToken;
			this.TitleEnglish = menuTitleEnglish;
		}

		// Token: 0x02000FB1 RID: 4017
		[Serializable]
		public struct Option
		{
			// Token: 0x04005094 RID: 20628
			public Translate.Phrase name;

			// Token: 0x04005095 RID: 20629
			public Translate.Phrase description;

			// Token: 0x04005096 RID: 20630
			public Sprite icon;

			// Token: 0x04005097 RID: 20631
			public int order;

			// Token: 0x04005098 RID: 20632
			public bool usableWhileWounded;
		}

		// Token: 0x02000FB2 RID: 4018
		public class Description : Attribute
		{
			// Token: 0x04005099 RID: 20633
			public string token;

			// Token: 0x0400509A RID: 20634
			public string english;

			// Token: 0x0600556E RID: 21870 RVA: 0x001BA1C4 File Offset: 0x001B83C4
			public Description(string t, string e)
			{
				this.token = t;
				this.english = e;
			}
		}

		// Token: 0x02000FB3 RID: 4019
		public class Icon : Attribute
		{
			// Token: 0x0400509B RID: 20635
			public string icon;

			// Token: 0x0600556F RID: 21871 RVA: 0x001BA1DA File Offset: 0x001B83DA
			public Icon(string i)
			{
				this.icon = i;
			}
		}

		// Token: 0x02000FB4 RID: 4020
		public class ShowIf : Attribute
		{
			// Token: 0x0400509C RID: 20636
			public string functionName;

			// Token: 0x06005570 RID: 21872 RVA: 0x001BA1E9 File Offset: 0x001B83E9
			public ShowIf(string testFunc)
			{
				this.functionName = testFunc;
			}
		}

		// Token: 0x02000FB5 RID: 4021
		public class Priority : Attribute
		{
			// Token: 0x0400509D RID: 20637
			public string functionName;

			// Token: 0x06005571 RID: 21873 RVA: 0x001BA1F8 File Offset: 0x001B83F8
			public Priority(string priorityFunc)
			{
				this.functionName = priorityFunc;
			}
		}

		// Token: 0x02000FB6 RID: 4022
		public class UsableWhileWounded : Attribute
		{
		}
	}

	// Token: 0x02000B77 RID: 2935
	[Serializable]
	public struct MovementModify
	{
		// Token: 0x04003F0E RID: 16142
		public float drag;
	}

	// Token: 0x02000B78 RID: 2936
	[Flags]
	public enum Flags
	{
		// Token: 0x04003F10 RID: 16144
		Placeholder = 1,
		// Token: 0x04003F11 RID: 16145
		On = 2,
		// Token: 0x04003F12 RID: 16146
		OnFire = 4,
		// Token: 0x04003F13 RID: 16147
		Open = 8,
		// Token: 0x04003F14 RID: 16148
		Locked = 16,
		// Token: 0x04003F15 RID: 16149
		Debugging = 32,
		// Token: 0x04003F16 RID: 16150
		Disabled = 64,
		// Token: 0x04003F17 RID: 16151
		Reserved1 = 128,
		// Token: 0x04003F18 RID: 16152
		Reserved2 = 256,
		// Token: 0x04003F19 RID: 16153
		Reserved3 = 512,
		// Token: 0x04003F1A RID: 16154
		Reserved4 = 1024,
		// Token: 0x04003F1B RID: 16155
		Reserved5 = 2048,
		// Token: 0x04003F1C RID: 16156
		Broken = 4096,
		// Token: 0x04003F1D RID: 16157
		Busy = 8192,
		// Token: 0x04003F1E RID: 16158
		Reserved6 = 16384,
		// Token: 0x04003F1F RID: 16159
		Reserved7 = 32768,
		// Token: 0x04003F20 RID: 16160
		Reserved8 = 65536,
		// Token: 0x04003F21 RID: 16161
		Reserved9 = 131072,
		// Token: 0x04003F22 RID: 16162
		Reserved10 = 262144,
		// Token: 0x04003F23 RID: 16163
		Reserved11 = 524288,
		// Token: 0x04003F24 RID: 16164
		InUse = 1048576
	}

	// Token: 0x02000B79 RID: 2937
	private readonly struct QueuedFileRequest : IEquatable<global::BaseEntity.QueuedFileRequest>
	{
		// Token: 0x04003F25 RID: 16165
		public readonly global::BaseEntity Entity;

		// Token: 0x04003F26 RID: 16166
		public readonly FileStorage.Type Type;

		// Token: 0x04003F27 RID: 16167
		public readonly uint Part;

		// Token: 0x04003F28 RID: 16168
		public readonly uint Crc;

		// Token: 0x04003F29 RID: 16169
		public readonly uint ResponseFunction;

		// Token: 0x04003F2A RID: 16170
		public readonly bool? RespondIfNotFound;

		// Token: 0x06004CF4 RID: 19700 RVA: 0x0019FA36 File Offset: 0x0019DC36
		public QueuedFileRequest(global::BaseEntity entity, FileStorage.Type type, uint part, uint crc, uint responseFunction, bool? respondIfNotFound)
		{
			this.Entity = entity;
			this.Type = type;
			this.Part = part;
			this.Crc = crc;
			this.ResponseFunction = responseFunction;
			this.RespondIfNotFound = respondIfNotFound;
		}

		// Token: 0x06004CF5 RID: 19701 RVA: 0x0019FA68 File Offset: 0x0019DC68
		public bool Equals(global::BaseEntity.QueuedFileRequest other)
		{
			if (object.Equals(this.Entity, other.Entity) && this.Type == other.Type && this.Part == other.Part && this.Crc == other.Crc && this.ResponseFunction == other.ResponseFunction)
			{
				bool? respondIfNotFound = this.RespondIfNotFound;
				bool? respondIfNotFound2 = other.RespondIfNotFound;
				return respondIfNotFound.GetValueOrDefault() == respondIfNotFound2.GetValueOrDefault() & respondIfNotFound != null == (respondIfNotFound2 != null);
			}
			return false;
		}

		// Token: 0x06004CF6 RID: 19702 RVA: 0x0019FAF4 File Offset: 0x0019DCF4
		public override bool Equals(object obj)
		{
			if (obj is global::BaseEntity.QueuedFileRequest)
			{
				global::BaseEntity.QueuedFileRequest other = (global::BaseEntity.QueuedFileRequest)obj;
				return this.Equals(other);
			}
			return false;
		}

		// Token: 0x06004CF7 RID: 19703 RVA: 0x0019FB1C File Offset: 0x0019DD1C
		public override int GetHashCode()
		{
			return ((((((this.Entity != null) ? this.Entity.GetHashCode() : 0) * 397 ^ (int)this.Type) * 397 ^ (int)this.Part) * 397 ^ (int)this.Crc) * 397 ^ (int)this.ResponseFunction) * 397 ^ this.RespondIfNotFound.GetHashCode();
		}
	}

	// Token: 0x02000B7A RID: 2938
	private readonly struct PendingFileRequest : IEquatable<global::BaseEntity.PendingFileRequest>
	{
		// Token: 0x04003F2B RID: 16171
		public readonly FileStorage.Type Type;

		// Token: 0x04003F2C RID: 16172
		public readonly uint NumId;

		// Token: 0x04003F2D RID: 16173
		public readonly uint Crc;

		// Token: 0x04003F2E RID: 16174
		public readonly IServerFileReceiver Receiver;

		// Token: 0x04003F2F RID: 16175
		public readonly float Time;

		// Token: 0x06004CF8 RID: 19704 RVA: 0x0019FB94 File Offset: 0x0019DD94
		public PendingFileRequest(FileStorage.Type type, uint numId, uint crc, IServerFileReceiver receiver)
		{
			this.Type = type;
			this.NumId = numId;
			this.Crc = crc;
			this.Receiver = receiver;
			this.Time = UnityEngine.Time.realtimeSinceStartup;
		}

		// Token: 0x06004CF9 RID: 19705 RVA: 0x0019FBBE File Offset: 0x0019DDBE
		public bool Equals(global::BaseEntity.PendingFileRequest other)
		{
			return this.Type == other.Type && this.NumId == other.NumId && this.Crc == other.Crc && object.Equals(this.Receiver, other.Receiver);
		}

		// Token: 0x06004CFA RID: 19706 RVA: 0x0019FC00 File Offset: 0x0019DE00
		public override bool Equals(object obj)
		{
			if (obj is global::BaseEntity.PendingFileRequest)
			{
				global::BaseEntity.PendingFileRequest other = (global::BaseEntity.PendingFileRequest)obj;
				return this.Equals(other);
			}
			return false;
		}

		// Token: 0x06004CFB RID: 19707 RVA: 0x0019FC27 File Offset: 0x0019DE27
		public override int GetHashCode()
		{
			return (int)(((this.Type * (FileStorage.Type)397 ^ (FileStorage.Type)this.NumId) * (FileStorage.Type)397 ^ (FileStorage.Type)this.Crc) * (FileStorage.Type)397 ^ (FileStorage.Type)((this.Receiver != null) ? this.Receiver.GetHashCode() : 0));
		}
	}

	// Token: 0x02000B7B RID: 2939
	public static class Query
	{
		// Token: 0x04003F30 RID: 16176
		public static global::BaseEntity.Query.EntityTree Server;

		// Token: 0x02000FB7 RID: 4023
		public class EntityTree
		{
			// Token: 0x0400509E RID: 20638
			private Grid<global::BaseEntity> Grid;

			// Token: 0x0400509F RID: 20639
			private Grid<global::BasePlayer> PlayerGrid;

			// Token: 0x040050A0 RID: 20640
			private Grid<global::BaseEntity> BrainGrid;

			// Token: 0x06005573 RID: 21875 RVA: 0x001BA207 File Offset: 0x001B8407
			public EntityTree(float worldSize)
			{
				this.Grid = new Grid<global::BaseEntity>(32, worldSize);
				this.PlayerGrid = new Grid<global::BasePlayer>(32, worldSize);
				this.BrainGrid = new Grid<global::BaseEntity>(32, worldSize);
			}

			// Token: 0x06005574 RID: 21876 RVA: 0x001BA23C File Offset: 0x001B843C
			public void Add(global::BaseEntity ent)
			{
				Vector3 position = ent.transform.position;
				this.Grid.Add(ent, position.x, position.z);
			}

			// Token: 0x06005575 RID: 21877 RVA: 0x001BA270 File Offset: 0x001B8470
			public void AddPlayer(global::BasePlayer player)
			{
				Vector3 position = player.transform.position;
				this.PlayerGrid.Add(player, position.x, position.z);
			}

			// Token: 0x06005576 RID: 21878 RVA: 0x001BA2A4 File Offset: 0x001B84A4
			public void AddBrain(global::BaseEntity entity)
			{
				Vector3 position = entity.transform.position;
				this.BrainGrid.Add(entity, position.x, position.z);
			}

			// Token: 0x06005577 RID: 21879 RVA: 0x001BA2D8 File Offset: 0x001B84D8
			public void Remove(global::BaseEntity ent, bool isPlayer = false)
			{
				this.Grid.Remove(ent);
				if (isPlayer)
				{
					global::BasePlayer basePlayer = ent as global::BasePlayer;
					if (basePlayer != null)
					{
						this.PlayerGrid.Remove(basePlayer);
					}
				}
			}

			// Token: 0x06005578 RID: 21880 RVA: 0x001BA312 File Offset: 0x001B8512
			public void RemovePlayer(global::BasePlayer player)
			{
				this.PlayerGrid.Remove(player);
			}

			// Token: 0x06005579 RID: 21881 RVA: 0x001BA321 File Offset: 0x001B8521
			public void RemoveBrain(global::BaseEntity entity)
			{
				if (entity == null)
				{
					return;
				}
				this.BrainGrid.Remove(entity);
			}

			// Token: 0x0600557A RID: 21882 RVA: 0x001BA33C File Offset: 0x001B853C
			public void Move(global::BaseEntity ent)
			{
				Vector3 position = ent.transform.position;
				this.Grid.Move(ent, position.x, position.z);
				global::BasePlayer basePlayer = ent as global::BasePlayer;
				if (basePlayer != null)
				{
					this.MovePlayer(basePlayer);
				}
				if (ent.HasBrain)
				{
					this.MoveBrain(ent);
				}
			}

			// Token: 0x0600557B RID: 21883 RVA: 0x001BA394 File Offset: 0x001B8594
			public void MovePlayer(global::BasePlayer player)
			{
				Vector3 position = player.transform.position;
				this.PlayerGrid.Move(player, position.x, position.z);
			}

			// Token: 0x0600557C RID: 21884 RVA: 0x001BA3C8 File Offset: 0x001B85C8
			public void MoveBrain(global::BaseEntity entity)
			{
				Vector3 position = entity.transform.position;
				this.BrainGrid.Move(entity, position.x, position.z);
			}

			// Token: 0x0600557D RID: 21885 RVA: 0x001BA3F9 File Offset: 0x001B85F9
			public int GetInSphere(Vector3 position, float distance, global::BaseEntity[] results, Func<global::BaseEntity, bool> filter = null)
			{
				return this.Grid.Query(position.x, position.z, distance, results, filter);
			}

			// Token: 0x0600557E RID: 21886 RVA: 0x001BA416 File Offset: 0x001B8616
			public int GetPlayersInSphere(Vector3 position, float distance, global::BasePlayer[] results, Func<global::BasePlayer, bool> filter = null)
			{
				return this.PlayerGrid.Query(position.x, position.z, distance, results, filter);
			}

			// Token: 0x0600557F RID: 21887 RVA: 0x001BA433 File Offset: 0x001B8633
			public int GetBrainsInSphere(Vector3 position, float distance, global::BaseEntity[] results, Func<global::BaseEntity, bool> filter = null)
			{
				return this.BrainGrid.Query(position.x, position.z, distance, results, filter);
			}
		}
	}

	// Token: 0x02000B7C RID: 2940
	public class RPC_Shared : Attribute
	{
	}

	// Token: 0x02000B7D RID: 2941
	public struct RPCMessage
	{
		// Token: 0x04003F31 RID: 16177
		public Connection connection;

		// Token: 0x04003F32 RID: 16178
		public global::BasePlayer player;

		// Token: 0x04003F33 RID: 16179
		public NetRead read;
	}

	// Token: 0x02000B7E RID: 2942
	public class RPC_Server : global::BaseEntity.RPC_Shared
	{
		// Token: 0x02000FB8 RID: 4024
		public abstract class Conditional : Attribute
		{
			// Token: 0x06005580 RID: 21888 RVA: 0x0002CDA7 File Offset: 0x0002AFA7
			public virtual string GetArgs()
			{
				return null;
			}
		}

		// Token: 0x02000FB9 RID: 4025
		public class MaxDistance : global::BaseEntity.RPC_Server.Conditional
		{
			// Token: 0x040050A1 RID: 20641
			private float maximumDistance;

			// Token: 0x06005582 RID: 21890 RVA: 0x001BA450 File Offset: 0x001B8650
			public MaxDistance(float maxDist)
			{
				this.maximumDistance = maxDist;
			}

			// Token: 0x06005583 RID: 21891 RVA: 0x001BA45F File Offset: 0x001B865F
			public override string GetArgs()
			{
				return this.maximumDistance.ToString("0.00f");
			}

			// Token: 0x06005584 RID: 21892 RVA: 0x001BA471 File Offset: 0x001B8671
			public static bool Test(uint id, string debugName, global::BaseEntity ent, global::BasePlayer player, float maximumDistance)
			{
				return !(ent == null) && !(player == null) && ent.Distance(player.eyes.position) <= maximumDistance;
			}
		}

		// Token: 0x02000FBA RID: 4026
		public class IsVisible : global::BaseEntity.RPC_Server.Conditional
		{
			// Token: 0x040050A2 RID: 20642
			private float maximumDistance;

			// Token: 0x06005585 RID: 21893 RVA: 0x001BA49F File Offset: 0x001B869F
			public IsVisible(float maxDist)
			{
				this.maximumDistance = maxDist;
			}

			// Token: 0x06005586 RID: 21894 RVA: 0x001BA4AE File Offset: 0x001B86AE
			public override string GetArgs()
			{
				return this.maximumDistance.ToString("0.00f");
			}

			// Token: 0x06005587 RID: 21895 RVA: 0x001BA4C0 File Offset: 0x001B86C0
			public static bool Test(uint id, string debugName, global::BaseEntity ent, global::BasePlayer player, float maximumDistance)
			{
				return !(ent == null) && !(player == null) && GamePhysics.LineOfSight(player.eyes.center, player.eyes.position, 2162688, null) && (ent.IsVisible(player.eyes.HeadRay(), 1218519041, maximumDistance) || ent.IsVisible(player.eyes.position, maximumDistance));
			}
		}

		// Token: 0x02000FBB RID: 4027
		public class FromOwner : global::BaseEntity.RPC_Server.Conditional
		{
			// Token: 0x06005588 RID: 21896 RVA: 0x001BA538 File Offset: 0x001B8738
			public static bool Test(uint id, string debugName, global::BaseEntity ent, global::BasePlayer player)
			{
				return !(ent == null) && !(player == null) && ent.net != null && player.net != null && (ent.net.ID == player.net.ID || !(ent.parentEntity.uid != player.net.ID));
			}
		}

		// Token: 0x02000FBC RID: 4028
		public class IsActiveItem : global::BaseEntity.RPC_Server.Conditional
		{
			// Token: 0x0600558A RID: 21898 RVA: 0x001BA5B4 File Offset: 0x001B87B4
			public static bool Test(uint id, string debugName, global::BaseEntity ent, global::BasePlayer player)
			{
				if (ent == null || player == null)
				{
					return false;
				}
				if (ent.net == null || player.net == null)
				{
					return false;
				}
				if (ent.net.ID == player.net.ID)
				{
					return true;
				}
				if (ent.parentEntity.uid != player.net.ID)
				{
					return false;
				}
				global::Item activeItem = player.GetActiveItem();
				return activeItem != null && !(activeItem.GetHeldEntity() != ent);
			}
		}

		// Token: 0x02000FBD RID: 4029
		public class CallsPerSecond : global::BaseEntity.RPC_Server.Conditional
		{
			// Token: 0x040050A3 RID: 20643
			private ulong callsPerSecond;

			// Token: 0x0600558C RID: 21900 RVA: 0x001BA642 File Offset: 0x001B8842
			public CallsPerSecond(ulong limit)
			{
				this.callsPerSecond = limit;
			}

			// Token: 0x0600558D RID: 21901 RVA: 0x001BA651 File Offset: 0x001B8851
			public override string GetArgs()
			{
				return this.callsPerSecond.ToString();
			}

			// Token: 0x0600558E RID: 21902 RVA: 0x001BA65E File Offset: 0x001B885E
			public static bool Test(uint id, string debugName, global::BaseEntity ent, global::BasePlayer player, ulong callsPerSecond)
			{
				return !(ent == null) && !(player == null) && player.rpcHistory.TryIncrement(id, callsPerSecond);
			}
		}
	}

	// Token: 0x02000B7F RID: 2943
	public enum Signal
	{
		// Token: 0x04003F35 RID: 16181
		Attack,
		// Token: 0x04003F36 RID: 16182
		Alt_Attack,
		// Token: 0x04003F37 RID: 16183
		DryFire,
		// Token: 0x04003F38 RID: 16184
		Reload,
		// Token: 0x04003F39 RID: 16185
		Deploy,
		// Token: 0x04003F3A RID: 16186
		Flinch_Head,
		// Token: 0x04003F3B RID: 16187
		Flinch_Chest,
		// Token: 0x04003F3C RID: 16188
		Flinch_Stomach,
		// Token: 0x04003F3D RID: 16189
		Flinch_RearHead,
		// Token: 0x04003F3E RID: 16190
		Flinch_RearTorso,
		// Token: 0x04003F3F RID: 16191
		Throw,
		// Token: 0x04003F40 RID: 16192
		Relax,
		// Token: 0x04003F41 RID: 16193
		Gesture,
		// Token: 0x04003F42 RID: 16194
		PhysImpact,
		// Token: 0x04003F43 RID: 16195
		Eat,
		// Token: 0x04003F44 RID: 16196
		Startled,
		// Token: 0x04003F45 RID: 16197
		Admire
	}

	// Token: 0x02000B80 RID: 2944
	public enum Slot
	{
		// Token: 0x04003F47 RID: 16199
		Lock,
		// Token: 0x04003F48 RID: 16200
		FireMod,
		// Token: 0x04003F49 RID: 16201
		UpperModifier,
		// Token: 0x04003F4A RID: 16202
		MiddleModifier,
		// Token: 0x04003F4B RID: 16203
		LowerModifier,
		// Token: 0x04003F4C RID: 16204
		CenterDecoration,
		// Token: 0x04003F4D RID: 16205
		LowerCenterDecoration,
		// Token: 0x04003F4E RID: 16206
		StorageMonitor,
		// Token: 0x04003F4F RID: 16207
		Count
	}

	// Token: 0x02000B81 RID: 2945
	[Flags]
	public enum TraitFlag
	{
		// Token: 0x04003F51 RID: 16209
		None = 0,
		// Token: 0x04003F52 RID: 16210
		Alive = 1,
		// Token: 0x04003F53 RID: 16211
		Animal = 2,
		// Token: 0x04003F54 RID: 16212
		Human = 4,
		// Token: 0x04003F55 RID: 16213
		Interesting = 8,
		// Token: 0x04003F56 RID: 16214
		Food = 16,
		// Token: 0x04003F57 RID: 16215
		Meat = 32,
		// Token: 0x04003F58 RID: 16216
		Water = 32
	}

	// Token: 0x02000B82 RID: 2946
	public static class Util
	{
		// Token: 0x06004CFE RID: 19710 RVA: 0x0019FC70 File Offset: 0x0019DE70
		public static global::BaseEntity[] FindTargets(string strFilter, bool onlyPlayers)
		{
			return (from x in global::BaseNetworkable.serverEntities.Where(delegate(global::BaseNetworkable x)
			{
				if (x is global::BasePlayer)
				{
					global::BasePlayer basePlayer = x as global::BasePlayer;
					return string.IsNullOrEmpty(strFilter) || (strFilter == "!alive" && basePlayer.IsAlive()) || (strFilter == "!sleeping" && basePlayer.IsSleeping()) || strFilter[0] == '!' || basePlayer.displayName.Contains(strFilter, CompareOptions.IgnoreCase) || basePlayer.UserIDString.Contains(strFilter);
				}
				return !onlyPlayers && !string.IsNullOrEmpty(strFilter) && x.ShortPrefabName.Contains(strFilter);
			})
			select x as global::BaseEntity).ToArray<global::BaseEntity>();
		}

		// Token: 0x06004CFF RID: 19711 RVA: 0x0019FCD0 File Offset: 0x0019DED0
		public static global::BaseEntity[] FindTargetsOwnedBy(ulong ownedBy, string strFilter)
		{
			bool hasFilter = !string.IsNullOrEmpty(strFilter);
			return (from x in global::BaseNetworkable.serverEntities.Where(delegate(global::BaseNetworkable x)
			{
				global::BaseEntity baseEntity;
				if ((baseEntity = (x as global::BaseEntity)) != null)
				{
					if (baseEntity.OwnerID != ownedBy)
					{
						return false;
					}
					if (!hasFilter || baseEntity.ShortPrefabName.Contains(strFilter))
					{
						return true;
					}
				}
				return false;
			})
			select x as global::BaseEntity).ToArray<global::BaseEntity>();
		}

		// Token: 0x06004D00 RID: 19712 RVA: 0x0019FD44 File Offset: 0x0019DF44
		public static global::BaseEntity[] FindTargetsAuthedTo(ulong authId, string strFilter)
		{
			bool hasFilter = !string.IsNullOrEmpty(strFilter);
			return (from x in global::BaseNetworkable.serverEntities.Where(delegate(global::BaseNetworkable x)
			{
				BuildingPrivlidge buildingPrivlidge;
				global::AutoTurret autoTurret;
				global::CodeLock codeLock;
				if ((buildingPrivlidge = (x as BuildingPrivlidge)) != null)
				{
					if (!buildingPrivlidge.IsAuthed(authId))
					{
						return false;
					}
					if (!hasFilter || x.ShortPrefabName.Contains(strFilter))
					{
						return true;
					}
				}
				else if ((autoTurret = (x as global::AutoTurret)) != null)
				{
					if (!autoTurret.IsAuthed(authId))
					{
						return false;
					}
					if (!hasFilter || x.ShortPrefabName.Contains(strFilter))
					{
						return true;
					}
				}
				else if ((codeLock = (x as global::CodeLock)) != null)
				{
					if (!codeLock.whitelistPlayers.Contains(authId))
					{
						return false;
					}
					if (!hasFilter || x.ShortPrefabName.Contains(strFilter))
					{
						return true;
					}
				}
				return false;
			})
			select x as global::BaseEntity).ToArray<global::BaseEntity>();
		}

		// Token: 0x06004D01 RID: 19713 RVA: 0x0019FDB8 File Offset: 0x0019DFB8
		public static T[] FindAll<T>() where T : global::BaseEntity
		{
			return global::BaseNetworkable.serverEntities.OfType<T>().ToArray<T>();
		}
	}

	// Token: 0x02000B83 RID: 2947
	public enum GiveItemReason
	{
		// Token: 0x04003F5A RID: 16218
		Generic,
		// Token: 0x04003F5B RID: 16219
		ResourceHarvested,
		// Token: 0x04003F5C RID: 16220
		PickedUp,
		// Token: 0x04003F5D RID: 16221
		Crafted
	}
}
