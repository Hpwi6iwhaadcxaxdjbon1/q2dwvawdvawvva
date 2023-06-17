using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ConVar;
using Facepunch;
using Facepunch.Rust;
using Network;
using Network.Visibility;
using ProtoBuf;
using Rust;
using Rust.Registry;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x020003B9 RID: 953
public abstract class BaseNetworkable : BaseMonoBehaviour, IPrefabPostProcess, IEntity, NetworkHandler
{
	// Token: 0x040019F3 RID: 6643
	public List<Component> postNetworkUpdateComponents = new List<Component>();

	// Token: 0x040019F4 RID: 6644
	private bool _limitedNetworking;

	// Token: 0x040019F5 RID: 6645
	[NonSerialized]
	public EntityRef parentEntity;

	// Token: 0x040019F6 RID: 6646
	[NonSerialized]
	public readonly List<global::BaseEntity> children = new List<global::BaseEntity>();

	// Token: 0x040019F7 RID: 6647
	[NonSerialized]
	public bool canTriggerParent = true;

	// Token: 0x040019F8 RID: 6648
	private int creationFrame;

	// Token: 0x040019F9 RID: 6649
	protected bool isSpawned;

	// Token: 0x040019FA RID: 6650
	private MemoryStream _NetworkCache;

	// Token: 0x040019FB RID: 6651
	public static Queue<MemoryStream> EntityMemoryStreamPool = new Queue<MemoryStream>();

	// Token: 0x040019FC RID: 6652
	private MemoryStream _SaveCache;

	// Token: 0x040019FD RID: 6653
	[Header("BaseNetworkable")]
	[ReadOnly]
	public uint prefabID;

	// Token: 0x040019FE RID: 6654
	[Tooltip("If enabled the entity will send to everyone on the server - regardless of position")]
	public bool globalBroadcast;

	// Token: 0x040019FF RID: 6655
	[NonSerialized]
	public Networkable net;

	// Token: 0x04001A01 RID: 6657
	private string _prefabName;

	// Token: 0x04001A02 RID: 6658
	private string _prefabNameWithoutExtension;

	// Token: 0x04001A03 RID: 6659
	public static global::BaseNetworkable.EntityRealm serverEntities = new global::BaseNetworkable.EntityRealmServer();

	// Token: 0x04001A04 RID: 6660
	private const bool isServersideEntity = true;

	// Token: 0x04001A05 RID: 6661
	private static List<Connection> connectionsInSphereList = new List<Connection>();

	// Token: 0x06002120 RID: 8480 RVA: 0x000D94AC File Offset: 0x000D76AC
	public void BroadcastOnPostNetworkUpdate(global::BaseEntity entity)
	{
		foreach (Component component in this.postNetworkUpdateComponents)
		{
			IOnPostNetworkUpdate onPostNetworkUpdate = component as IOnPostNetworkUpdate;
			if (onPostNetworkUpdate != null)
			{
				onPostNetworkUpdate.OnPostNetworkUpdate(entity);
			}
		}
		foreach (global::BaseEntity baseEntity in this.children)
		{
			baseEntity.BroadcastOnPostNetworkUpdate(entity);
		}
	}

	// Token: 0x06002121 RID: 8481 RVA: 0x000D9548 File Offset: 0x000D7748
	public virtual void PostProcess(IPrefabProcessor preProcess, GameObject rootObj, string name, bool serverside, bool clientside, bool bundling)
	{
		if (serverside)
		{
			return;
		}
		this.postNetworkUpdateComponents = base.GetComponentsInChildren<IOnPostNetworkUpdate>(true).Cast<Component>().ToList<Component>();
	}

	// Token: 0x170002C1 RID: 705
	// (get) Token: 0x06002122 RID: 8482 RVA: 0x000D9566 File Offset: 0x000D7766
	// (set) Token: 0x06002123 RID: 8483 RVA: 0x000D956E File Offset: 0x000D776E
	public bool limitNetworking
	{
		get
		{
			return this._limitedNetworking;
		}
		set
		{
			if (value == this._limitedNetworking)
			{
				return;
			}
			this._limitedNetworking = value;
			if (this._limitedNetworking)
			{
				this.OnNetworkLimitStart();
			}
			else
			{
				this.OnNetworkLimitEnd();
			}
			this.UpdateNetworkGroup();
		}
	}

	// Token: 0x06002124 RID: 8484 RVA: 0x000D95A0 File Offset: 0x000D77A0
	private void OnNetworkLimitStart()
	{
		base.LogEntry(BaseMonoBehaviour.LogEntryType.Network, 2, "OnNetworkLimitStart");
		List<Connection> list = this.GetSubscribers();
		if (list == null)
		{
			return;
		}
		list = list.ToList<Connection>();
		list.RemoveAll((Connection x) => this.ShouldNetworkTo(x.player as global::BasePlayer));
		this.OnNetworkSubscribersLeave(list);
		if (this.children != null)
		{
			foreach (global::BaseEntity baseEntity in this.children)
			{
				baseEntity.OnNetworkLimitStart();
			}
		}
	}

	// Token: 0x06002125 RID: 8485 RVA: 0x000D9634 File Offset: 0x000D7834
	private void OnNetworkLimitEnd()
	{
		base.LogEntry(BaseMonoBehaviour.LogEntryType.Network, 2, "OnNetworkLimitEnd");
		List<Connection> subscribers = this.GetSubscribers();
		if (subscribers == null)
		{
			return;
		}
		this.OnNetworkSubscribersEnter(subscribers);
		if (this.children != null)
		{
			foreach (global::BaseEntity baseEntity in this.children)
			{
				baseEntity.OnNetworkLimitEnd();
			}
		}
	}

	// Token: 0x06002126 RID: 8486 RVA: 0x000D96AC File Offset: 0x000D78AC
	public global::BaseEntity GetParentEntity()
	{
		return this.parentEntity.Get(this.isServer);
	}

	// Token: 0x06002127 RID: 8487 RVA: 0x000D96BF File Offset: 0x000D78BF
	public bool HasParent()
	{
		return this.parentEntity.IsValid(this.isServer);
	}

	// Token: 0x06002128 RID: 8488 RVA: 0x000D96D2 File Offset: 0x000D78D2
	public void AddChild(global::BaseEntity child)
	{
		if (this.children.Contains(child))
		{
			return;
		}
		this.children.Add(child);
		this.OnChildAdded(child);
	}

	// Token: 0x06002129 RID: 8489 RVA: 0x000063A5 File Offset: 0x000045A5
	protected virtual void OnChildAdded(global::BaseEntity child)
	{
	}

	// Token: 0x0600212A RID: 8490 RVA: 0x000D96F6 File Offset: 0x000D78F6
	public void RemoveChild(global::BaseEntity child)
	{
		this.children.Remove(child);
		this.OnChildRemoved(child);
	}

	// Token: 0x0600212B RID: 8491 RVA: 0x000063A5 File Offset: 0x000045A5
	protected virtual void OnChildRemoved(global::BaseEntity child)
	{
	}

	// Token: 0x170002C2 RID: 706
	// (get) Token: 0x0600212C RID: 8492 RVA: 0x000D970C File Offset: 0x000D790C
	public GameManager gameManager
	{
		get
		{
			if (this.isServer)
			{
				return GameManager.server;
			}
			throw new NotImplementedException("Missing gameManager path");
		}
	}

	// Token: 0x170002C3 RID: 707
	// (get) Token: 0x0600212D RID: 8493 RVA: 0x000D9726 File Offset: 0x000D7926
	public PrefabAttribute.Library prefabAttribute
	{
		get
		{
			if (this.isServer)
			{
				return PrefabAttribute.server;
			}
			throw new NotImplementedException("Missing prefabAttribute path");
		}
	}

	// Token: 0x170002C4 RID: 708
	// (get) Token: 0x0600212E RID: 8494 RVA: 0x000D9740 File Offset: 0x000D7940
	public static Group GlobalNetworkGroup
	{
		get
		{
			return Network.Net.sv.visibility.Get(0U);
		}
	}

	// Token: 0x170002C5 RID: 709
	// (get) Token: 0x0600212F RID: 8495 RVA: 0x000D9752 File Offset: 0x000D7952
	public static Group LimboNetworkGroup
	{
		get
		{
			return Network.Net.sv.visibility.Get(1U);
		}
	}

	// Token: 0x06002130 RID: 8496 RVA: 0x000CB9EB File Offset: 0x000C9BEB
	public virtual float GetNetworkTime()
	{
		return UnityEngine.Time.time;
	}

	// Token: 0x06002131 RID: 8497 RVA: 0x000D9764 File Offset: 0x000D7964
	public virtual void Spawn()
	{
		this.SpawnShared();
		if (this.net == null)
		{
			this.net = Network.Net.sv.CreateNetworkable();
		}
		this.creationFrame = UnityEngine.Time.frameCount;
		this.PreInitShared();
		this.InitShared();
		this.ServerInit();
		this.PostInitShared();
		this.UpdateNetworkGroup();
		this.isSpawned = true;
		this.SendNetworkUpdateImmediate(true);
		if (Rust.Application.isLoading && !Rust.Application.isLoadingSave)
		{
			base.gameObject.SendOnSendNetworkUpdate(this as global::BaseEntity);
		}
	}

	// Token: 0x06002132 RID: 8498 RVA: 0x000D97E5 File Offset: 0x000D79E5
	public bool IsFullySpawned()
	{
		return this.isSpawned;
	}

	// Token: 0x06002133 RID: 8499 RVA: 0x000D97ED File Offset: 0x000D79ED
	public virtual void ServerInit()
	{
		global::BaseNetworkable.serverEntities.RegisterID(this);
		if (this.net != null)
		{
			this.net.handler = this;
		}
	}

	// Token: 0x06002134 RID: 8500 RVA: 0x000D980E File Offset: 0x000D7A0E
	protected List<Connection> GetSubscribers()
	{
		if (this.net == null)
		{
			return null;
		}
		if (this.net.group == null)
		{
			return null;
		}
		return this.net.group.subscribers;
	}

	// Token: 0x06002135 RID: 8501 RVA: 0x00003384 File Offset: 0x00001584
	public void KillMessage()
	{
		this.Kill(global::BaseNetworkable.DestroyMode.None);
	}

	// Token: 0x06002136 RID: 8502 RVA: 0x00029A3C File Offset: 0x00027C3C
	public virtual void AdminKill()
	{
		this.Kill(global::BaseNetworkable.DestroyMode.Gib);
	}

	// Token: 0x06002137 RID: 8503 RVA: 0x000D9839 File Offset: 0x000D7A39
	public void Kill(global::BaseNetworkable.DestroyMode mode = global::BaseNetworkable.DestroyMode.None)
	{
		if (this.IsDestroyed)
		{
			Debug.LogWarning("Calling kill - but already IsDestroyed!? " + this);
			return;
		}
		base.gameObject.BroadcastOnParentDestroying();
		this.DoEntityDestroy();
		this.TerminateOnClient(mode);
		this.TerminateOnServer();
		this.EntityDestroy();
	}

	// Token: 0x06002138 RID: 8504 RVA: 0x000D9878 File Offset: 0x000D7A78
	private void TerminateOnClient(global::BaseNetworkable.DestroyMode mode)
	{
		if (this.net == null)
		{
			return;
		}
		if (this.net.group == null)
		{
			return;
		}
		if (!Network.Net.sv.IsConnected())
		{
			return;
		}
		base.LogEntry(BaseMonoBehaviour.LogEntryType.Network, 2, "Term {0}", mode);
		NetWrite netWrite = Network.Net.sv.StartWrite();
		netWrite.PacketID(Message.Type.EntityDestroy);
		netWrite.EntityID(this.net.ID);
		netWrite.UInt8((byte)mode);
		netWrite.Send(new SendInfo(this.net.group.subscribers));
	}

	// Token: 0x06002139 RID: 8505 RVA: 0x000D98FF File Offset: 0x000D7AFF
	private void TerminateOnServer()
	{
		if (this.net == null)
		{
			return;
		}
		this.InvalidateNetworkCache();
		global::BaseNetworkable.serverEntities.UnregisterID(this);
		Network.Net.sv.DestroyNetworkable(ref this.net);
		base.StopAllCoroutines();
		base.gameObject.SetActive(false);
	}

	// Token: 0x0600213A RID: 8506 RVA: 0x000D993D File Offset: 0x000D7B3D
	internal virtual void DoServerDestroy()
	{
		this.isSpawned = false;
		Analytics.Azure.OnEntityDestroyed(this);
	}

	// Token: 0x0600213B RID: 8507 RVA: 0x000D994C File Offset: 0x000D7B4C
	public virtual bool ShouldNetworkTo(global::BasePlayer player)
	{
		return this.net.group == null || player.net.subscriber.IsSubscribed(this.net.group);
	}

	// Token: 0x0600213C RID: 8508 RVA: 0x000D9978 File Offset: 0x000D7B78
	protected void SendNetworkGroupChange()
	{
		if (!this.isSpawned)
		{
			return;
		}
		if (!Network.Net.sv.IsConnected())
		{
			return;
		}
		if (this.net.group == null)
		{
			Debug.LogWarning(this.ToString() + " changed its network group to null");
			return;
		}
		NetWrite netWrite = Network.Net.sv.StartWrite();
		netWrite.PacketID(Message.Type.GroupChange);
		netWrite.EntityID(this.net.ID);
		netWrite.GroupID(this.net.group.ID);
		netWrite.Send(new SendInfo(this.net.group.subscribers));
	}

	// Token: 0x0600213D RID: 8509 RVA: 0x000D9A10 File Offset: 0x000D7C10
	protected void SendAsSnapshot(Connection connection, bool justCreated = false)
	{
		NetWrite netWrite = Network.Net.sv.StartWrite();
		connection.validate.entityUpdates = connection.validate.entityUpdates + 1U;
		global::BaseNetworkable.SaveInfo saveInfo = new global::BaseNetworkable.SaveInfo
		{
			forConnection = connection,
			forDisk = false
		};
		netWrite.PacketID(Message.Type.Entities);
		netWrite.UInt32(connection.validate.entityUpdates);
		this.ToStreamForNetwork(netWrite, saveInfo);
		netWrite.Send(new SendInfo(connection));
	}

	// Token: 0x0600213E RID: 8510 RVA: 0x000D9A80 File Offset: 0x000D7C80
	public void SendNetworkUpdate(global::BasePlayer.NetworkQueue queue = global::BasePlayer.NetworkQueue.Update)
	{
		if (Rust.Application.isLoading)
		{
			return;
		}
		if (Rust.Application.isLoadingSave)
		{
			return;
		}
		if (this.IsDestroyed)
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
		using (TimeWarning.New("SendNetworkUpdate", 0))
		{
			base.LogEntry(BaseMonoBehaviour.LogEntryType.Network, 2, "SendNetworkUpdate");
			this.InvalidateNetworkCache();
			List<Connection> subscribers = this.GetSubscribers();
			if (subscribers != null && subscribers.Count > 0)
			{
				for (int i = 0; i < subscribers.Count; i++)
				{
					global::BasePlayer basePlayer = subscribers[i].player as global::BasePlayer;
					if (!(basePlayer == null) && this.ShouldNetworkTo(basePlayer))
					{
						basePlayer.QueueUpdate(queue, this);
					}
				}
			}
		}
		base.gameObject.SendOnSendNetworkUpdate(this as global::BaseEntity);
	}

	// Token: 0x0600213F RID: 8511 RVA: 0x000D9B54 File Offset: 0x000D7D54
	public void SendNetworkUpdateImmediate(bool justCreated = false)
	{
		if (Rust.Application.isLoading)
		{
			return;
		}
		if (Rust.Application.isLoadingSave)
		{
			return;
		}
		if (this.IsDestroyed)
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
		using (TimeWarning.New("SendNetworkUpdateImmediate", 0))
		{
			base.LogEntry(BaseMonoBehaviour.LogEntryType.Network, 2, "SendNetworkUpdateImmediate");
			this.InvalidateNetworkCache();
			List<Connection> subscribers = this.GetSubscribers();
			if (subscribers != null && subscribers.Count > 0)
			{
				for (int i = 0; i < subscribers.Count; i++)
				{
					Connection connection = subscribers[i];
					global::BasePlayer basePlayer = connection.player as global::BasePlayer;
					if (!(basePlayer == null) && this.ShouldNetworkTo(basePlayer))
					{
						this.SendAsSnapshot(connection, justCreated);
					}
				}
			}
		}
		base.gameObject.SendOnSendNetworkUpdate(this as global::BaseEntity);
	}

	// Token: 0x06002140 RID: 8512 RVA: 0x000D9C30 File Offset: 0x000D7E30
	protected void SendNetworkUpdate_Position()
	{
		if (Rust.Application.isLoading)
		{
			return;
		}
		if (Rust.Application.isLoadingSave)
		{
			return;
		}
		if (this.IsDestroyed)
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
		using (TimeWarning.New("SendNetworkUpdate_Position", 0))
		{
			base.LogEntry(BaseMonoBehaviour.LogEntryType.Network, 2, "SendNetworkUpdate_Position");
			List<Connection> subscribers = this.GetSubscribers();
			if (subscribers != null && subscribers.Count > 0)
			{
				NetWrite netWrite = Network.Net.sv.StartWrite();
				netWrite.PacketID(Message.Type.EntityPosition);
				netWrite.EntityID(this.net.ID);
				NetWrite netWrite2 = netWrite;
				Vector3 vector = this.GetNetworkPosition();
				netWrite2.Vector3(vector);
				NetWrite netWrite3 = netWrite;
				vector = this.GetNetworkRotation().eulerAngles;
				netWrite3.Vector3(vector);
				netWrite.Float(this.GetNetworkTime());
				NetworkableId uid = this.parentEntity.uid;
				if (uid.IsValid)
				{
					netWrite.EntityID(uid);
				}
				SendInfo info = new SendInfo(subscribers)
				{
					method = SendMethod.ReliableUnordered,
					priority = Priority.Immediate
				};
				netWrite.Send(info);
			}
		}
	}

	// Token: 0x06002141 RID: 8513 RVA: 0x000D9D50 File Offset: 0x000D7F50
	private void ToStream(Stream stream, global::BaseNetworkable.SaveInfo saveInfo)
	{
		using (saveInfo.msg = Facepunch.Pool.Get<ProtoBuf.Entity>())
		{
			this.Save(saveInfo);
			if (saveInfo.msg.baseEntity == null)
			{
				Debug.LogError(this + ": ToStream - no BaseEntity!?");
			}
			if (saveInfo.msg.baseNetworkable == null)
			{
				Debug.LogError(this + ": ToStream - no baseNetworkable!?");
			}
			saveInfo.msg.ToProto(stream);
			this.PostSave(saveInfo);
		}
	}

	// Token: 0x06002142 RID: 8514 RVA: 0x000D9DE0 File Offset: 0x000D7FE0
	public virtual bool CanUseNetworkCache(Connection connection)
	{
		return ConVar.Server.netcache;
	}

	// Token: 0x06002143 RID: 8515 RVA: 0x000D9DE8 File Offset: 0x000D7FE8
	public void ToStreamForNetwork(Stream stream, global::BaseNetworkable.SaveInfo saveInfo)
	{
		if (!this.CanUseNetworkCache(saveInfo.forConnection))
		{
			this.ToStream(stream, saveInfo);
			return;
		}
		if (this._NetworkCache == null)
		{
			this._NetworkCache = ((global::BaseNetworkable.EntityMemoryStreamPool.Count > 0) ? (this._NetworkCache = global::BaseNetworkable.EntityMemoryStreamPool.Dequeue()) : new MemoryStream(8));
			this.ToStream(this._NetworkCache, saveInfo);
			ConVar.Server.netcachesize += (int)this._NetworkCache.Length;
		}
		this._NetworkCache.WriteTo(stream);
	}

	// Token: 0x06002144 RID: 8516 RVA: 0x000D9E74 File Offset: 0x000D8074
	public void InvalidateNetworkCache()
	{
		using (TimeWarning.New("InvalidateNetworkCache", 0))
		{
			if (this._SaveCache != null)
			{
				ConVar.Server.savecachesize -= (int)this._SaveCache.Length;
				this._SaveCache.SetLength(0L);
				this._SaveCache.Position = 0L;
				global::BaseNetworkable.EntityMemoryStreamPool.Enqueue(this._SaveCache);
				this._SaveCache = null;
			}
			if (this._NetworkCache != null)
			{
				ConVar.Server.netcachesize -= (int)this._NetworkCache.Length;
				this._NetworkCache.SetLength(0L);
				this._NetworkCache.Position = 0L;
				global::BaseNetworkable.EntityMemoryStreamPool.Enqueue(this._NetworkCache);
				this._NetworkCache = null;
			}
			base.LogEntry(BaseMonoBehaviour.LogEntryType.Network, 3, "InvalidateNetworkCache");
		}
	}

	// Token: 0x06002145 RID: 8517 RVA: 0x000D9F58 File Offset: 0x000D8158
	public MemoryStream GetSaveCache()
	{
		if (this._SaveCache == null)
		{
			if (global::BaseNetworkable.EntityMemoryStreamPool.Count > 0)
			{
				this._SaveCache = global::BaseNetworkable.EntityMemoryStreamPool.Dequeue();
			}
			else
			{
				this._SaveCache = new MemoryStream(8);
			}
			global::BaseNetworkable.SaveInfo saveInfo = new global::BaseNetworkable.SaveInfo
			{
				forDisk = true
			};
			this.ToStream(this._SaveCache, saveInfo);
			ConVar.Server.savecachesize += (int)this._SaveCache.Length;
		}
		return this._SaveCache;
	}

	// Token: 0x06002146 RID: 8518 RVA: 0x000D9FD4 File Offset: 0x000D81D4
	public virtual void UpdateNetworkGroup()
	{
		Assert.IsTrue(this.isServer, "UpdateNetworkGroup called on clientside entity!");
		if (this.net == null)
		{
			return;
		}
		using (TimeWarning.New("UpdateGroups", 0))
		{
			if (this.net.UpdateGroups(base.transform.position))
			{
				this.SendNetworkGroupChange();
			}
		}
	}

	// Token: 0x170002C6 RID: 710
	// (get) Token: 0x06002147 RID: 8519 RVA: 0x000DA040 File Offset: 0x000D8240
	// (set) Token: 0x06002148 RID: 8520 RVA: 0x000DA048 File Offset: 0x000D8248
	public bool IsDestroyed { get; private set; }

	// Token: 0x170002C7 RID: 711
	// (get) Token: 0x06002149 RID: 8521 RVA: 0x000DA051 File Offset: 0x000D8251
	public string PrefabName
	{
		get
		{
			if (this._prefabName == null)
			{
				this._prefabName = StringPool.Get(this.prefabID);
			}
			return this._prefabName;
		}
	}

	// Token: 0x170002C8 RID: 712
	// (get) Token: 0x0600214A RID: 8522 RVA: 0x000DA072 File Offset: 0x000D8272
	public string ShortPrefabName
	{
		get
		{
			if (this._prefabNameWithoutExtension == null)
			{
				this._prefabNameWithoutExtension = Path.GetFileNameWithoutExtension(this.PrefabName);
			}
			return this._prefabNameWithoutExtension;
		}
	}

	// Token: 0x0600214B RID: 8523 RVA: 0x0002C692 File Offset: 0x0002A892
	public virtual Vector3 GetNetworkPosition()
	{
		return base.transform.localPosition;
	}

	// Token: 0x0600214C RID: 8524 RVA: 0x0002C6CD File Offset: 0x0002A8CD
	public virtual Quaternion GetNetworkRotation()
	{
		return base.transform.localRotation;
	}

	// Token: 0x0600214D RID: 8525 RVA: 0x000DA094 File Offset: 0x000D8294
	public string InvokeString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		List<InvokeAction> list = Facepunch.Pool.GetList<InvokeAction>();
		InvokeHandler.FindInvokes(this, list);
		foreach (InvokeAction invokeAction in list)
		{
			if (stringBuilder.Length > 0)
			{
				stringBuilder.Append(", ");
			}
			stringBuilder.Append(invokeAction.action.Method.Name);
		}
		Facepunch.Pool.FreeList<InvokeAction>(ref list);
		return stringBuilder.ToString();
	}

	// Token: 0x0600214E RID: 8526 RVA: 0x000DA128 File Offset: 0x000D8328
	public global::BaseEntity LookupPrefab()
	{
		return this.gameManager.FindPrefab(this.PrefabName).ToBaseEntity();
	}

	// Token: 0x0600214F RID: 8527 RVA: 0x000DA140 File Offset: 0x000D8340
	public bool EqualNetID(global::BaseNetworkable other)
	{
		return !other.IsRealNull() && other.net != null && this.net != null && other.net.ID == this.net.ID;
	}

	// Token: 0x06002150 RID: 8528 RVA: 0x000DA177 File Offset: 0x000D8377
	public bool EqualNetID(NetworkableId otherID)
	{
		return this.net != null && otherID == this.net.ID;
	}

	// Token: 0x06002151 RID: 8529 RVA: 0x000DA194 File Offset: 0x000D8394
	public virtual void ResetState()
	{
		if (this.children.Count > 0)
		{
			this.children.Clear();
		}
		ILootableEntity lootableEntity;
		if ((lootableEntity = (this as ILootableEntity)) != null)
		{
			lootableEntity.LastLootedBy = 0UL;
		}
	}

	// Token: 0x06002152 RID: 8530 RVA: 0x000063A5 File Offset: 0x000045A5
	public virtual void InitShared()
	{
	}

	// Token: 0x06002153 RID: 8531 RVA: 0x000063A5 File Offset: 0x000045A5
	public virtual void PreInitShared()
	{
	}

	// Token: 0x06002154 RID: 8532 RVA: 0x000063A5 File Offset: 0x000045A5
	public virtual void PostInitShared()
	{
	}

	// Token: 0x06002155 RID: 8533 RVA: 0x000063A5 File Offset: 0x000045A5
	public virtual void DestroyShared()
	{
	}

	// Token: 0x06002156 RID: 8534 RVA: 0x000063A5 File Offset: 0x000045A5
	public virtual void OnNetworkGroupEnter(Group group)
	{
	}

	// Token: 0x06002157 RID: 8535 RVA: 0x000063A5 File Offset: 0x000045A5
	public virtual void OnNetworkGroupLeave(Group group)
	{
	}

	// Token: 0x06002158 RID: 8536 RVA: 0x000DA1CC File Offset: 0x000D83CC
	public void OnNetworkGroupChange()
	{
		if (this.children != null)
		{
			foreach (global::BaseEntity baseEntity in this.children)
			{
				if (baseEntity.ShouldInheritNetworkGroup())
				{
					baseEntity.net.SwitchGroup(this.net.group);
				}
				else if (this.isServer)
				{
					baseEntity.UpdateNetworkGroup();
				}
			}
		}
	}

	// Token: 0x06002159 RID: 8537 RVA: 0x000DA250 File Offset: 0x000D8450
	public void OnNetworkSubscribersEnter(List<Connection> connections)
	{
		if (!Network.Net.sv.IsConnected())
		{
			return;
		}
		foreach (Connection connection in connections)
		{
			global::BasePlayer basePlayer = connection.player as global::BasePlayer;
			if (!(basePlayer == null))
			{
				basePlayer.QueueUpdate(global::BasePlayer.NetworkQueue.Update, this as global::BaseEntity);
			}
		}
	}

	// Token: 0x0600215A RID: 8538 RVA: 0x000DA2C4 File Offset: 0x000D84C4
	public void OnNetworkSubscribersLeave(List<Connection> connections)
	{
		if (!Network.Net.sv.IsConnected())
		{
			return;
		}
		base.LogEntry(BaseMonoBehaviour.LogEntryType.Network, 2, "LeaveVisibility");
		NetWrite netWrite = Network.Net.sv.StartWrite();
		netWrite.PacketID(Message.Type.EntityDestroy);
		netWrite.EntityID(this.net.ID);
		netWrite.UInt8(0);
		netWrite.Send(new SendInfo(connections));
	}

	// Token: 0x0600215B RID: 8539 RVA: 0x000DA31F File Offset: 0x000D851F
	private void EntityDestroy()
	{
		if (base.gameObject)
		{
			this.ResetState();
			this.gameManager.Retire(base.gameObject);
		}
	}

	// Token: 0x0600215C RID: 8540 RVA: 0x000DA348 File Offset: 0x000D8548
	private void DoEntityDestroy()
	{
		if (this.IsDestroyed)
		{
			return;
		}
		this.IsDestroyed = true;
		if (Rust.Application.isQuitting)
		{
			return;
		}
		this.DestroyShared();
		if (this.isServer)
		{
			this.DoServerDestroy();
		}
		using (TimeWarning.New("Registry.Entity.Unregister", 0))
		{
			Rust.Registry.Entity.Unregister(base.gameObject);
		}
	}

	// Token: 0x0600215D RID: 8541 RVA: 0x000DA3B4 File Offset: 0x000D85B4
	private void SpawnShared()
	{
		this.IsDestroyed = false;
		using (TimeWarning.New("Registry.Entity.Register", 0))
		{
			Rust.Registry.Entity.Register(base.gameObject, this);
		}
	}

	// Token: 0x0600215E RID: 8542 RVA: 0x000DA3FC File Offset: 0x000D85FC
	public virtual void Save(global::BaseNetworkable.SaveInfo info)
	{
		if (this.prefabID == 0U)
		{
			Debug.LogError("PrefabID is 0! " + base.transform.GetRecursiveName(""), base.gameObject);
		}
		info.msg.baseNetworkable = Facepunch.Pool.Get<ProtoBuf.BaseNetworkable>();
		info.msg.baseNetworkable.uid = this.net.ID;
		info.msg.baseNetworkable.prefabID = this.prefabID;
		if (this.net.group != null)
		{
			info.msg.baseNetworkable.group = this.net.group.ID;
		}
		if (!info.forDisk)
		{
			info.msg.createdThisFrame = (this.creationFrame == UnityEngine.Time.frameCount);
		}
	}

	// Token: 0x0600215F RID: 8543 RVA: 0x000063A5 File Offset: 0x000045A5
	public virtual void PostSave(global::BaseNetworkable.SaveInfo info)
	{
	}

	// Token: 0x06002160 RID: 8544 RVA: 0x000DA4C4 File Offset: 0x000D86C4
	public void InitLoad(NetworkableId entityID)
	{
		this.net = Network.Net.sv.CreateNetworkable(entityID);
		global::BaseNetworkable.serverEntities.RegisterID(this);
		this.PreServerLoad();
	}

	// Token: 0x06002161 RID: 8545 RVA: 0x000063A5 File Offset: 0x000045A5
	public virtual void PreServerLoad()
	{
	}

	// Token: 0x06002162 RID: 8546 RVA: 0x000DA4E8 File Offset: 0x000D86E8
	public virtual void Load(global::BaseNetworkable.LoadInfo info)
	{
		if (info.msg.baseNetworkable == null)
		{
			return;
		}
		ProtoBuf.BaseNetworkable baseNetworkable = info.msg.baseNetworkable;
		if (this.prefabID != baseNetworkable.prefabID)
		{
			Debug.LogError(string.Concat(new object[]
			{
				"Prefab IDs don't match! ",
				this.prefabID,
				"/",
				baseNetworkable.prefabID,
				" -> ",
				base.gameObject
			}), base.gameObject);
		}
	}

	// Token: 0x06002163 RID: 8547 RVA: 0x000DA570 File Offset: 0x000D8770
	public virtual void PostServerLoad()
	{
		base.gameObject.SendOnSendNetworkUpdate(this as global::BaseEntity);
	}

	// Token: 0x170002C9 RID: 713
	// (get) Token: 0x06002164 RID: 8548 RVA: 0x0000441C File Offset: 0x0000261C
	public bool isServer
	{
		get
		{
			return true;
		}
	}

	// Token: 0x170002CA RID: 714
	// (get) Token: 0x06002165 RID: 8549 RVA: 0x00007A3C File Offset: 0x00005C3C
	public bool isClient
	{
		get
		{
			return false;
		}
	}

	// Token: 0x06002166 RID: 8550 RVA: 0x000DA584 File Offset: 0x000D8784
	public T ToServer<T>() where T : global::BaseNetworkable
	{
		if (this.isServer)
		{
			return this as T;
		}
		return default(T);
	}

	// Token: 0x06002167 RID: 8551 RVA: 0x00007A3C File Offset: 0x00005C3C
	public virtual bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		return false;
	}

	// Token: 0x06002168 RID: 8552 RVA: 0x000DA5B0 File Offset: 0x000D87B0
	public static List<Connection> GetConnectionsWithin(Vector3 position, float distance)
	{
		global::BaseNetworkable.connectionsInSphereList.Clear();
		float num = distance * distance;
		List<Connection> subscribers = global::BaseNetworkable.GlobalNetworkGroup.subscribers;
		for (int i = 0; i < subscribers.Count; i++)
		{
			Connection connection = subscribers[i];
			if (connection.active)
			{
				global::BasePlayer basePlayer = connection.player as global::BasePlayer;
				if (!(basePlayer == null) && basePlayer.SqrDistance(position) <= num)
				{
					global::BaseNetworkable.connectionsInSphereList.Add(connection);
				}
			}
		}
		return global::BaseNetworkable.connectionsInSphereList;
	}

	// Token: 0x06002169 RID: 8553 RVA: 0x000DA62C File Offset: 0x000D882C
	public static void GetCloseConnections(Vector3 position, float distance, List<global::BasePlayer> players)
	{
		if (Network.Net.sv == null)
		{
			return;
		}
		if (Network.Net.sv.visibility == null)
		{
			return;
		}
		float num = distance * distance;
		Group group = Network.Net.sv.visibility.GetGroup(position);
		if (group == null)
		{
			return;
		}
		List<Connection> subscribers = group.subscribers;
		for (int i = 0; i < subscribers.Count; i++)
		{
			Connection connection = subscribers[i];
			if (connection.active)
			{
				global::BasePlayer basePlayer = connection.player as global::BasePlayer;
				if (!(basePlayer == null) && basePlayer.SqrDistance(position) <= num)
				{
					players.Add(basePlayer);
				}
			}
		}
	}

	// Token: 0x0600216A RID: 8554 RVA: 0x000DA6BC File Offset: 0x000D88BC
	public static bool HasCloseConnections(Vector3 position, float distance)
	{
		if (Network.Net.sv == null)
		{
			return false;
		}
		if (Network.Net.sv.visibility == null)
		{
			return false;
		}
		float num = distance * distance;
		Group group = Network.Net.sv.visibility.GetGroup(position);
		if (group == null)
		{
			return false;
		}
		List<Connection> subscribers = group.subscribers;
		for (int i = 0; i < subscribers.Count; i++)
		{
			Connection connection = subscribers[i];
			if (connection.active)
			{
				global::BasePlayer basePlayer = connection.player as global::BasePlayer;
				if (!(basePlayer == null) && basePlayer.SqrDistance(position) <= num)
				{
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x02000CB7 RID: 3255
	public struct SaveInfo
	{
		// Token: 0x04004496 RID: 17558
		public ProtoBuf.Entity msg;

		// Token: 0x04004497 RID: 17559
		public bool forDisk;

		// Token: 0x04004498 RID: 17560
		public Connection forConnection;

		// Token: 0x06004F98 RID: 20376 RVA: 0x001A6B7F File Offset: 0x001A4D7F
		internal bool SendingTo(Connection ownerConnection)
		{
			return ownerConnection != null && this.forConnection != null && this.forConnection == ownerConnection;
		}
	}

	// Token: 0x02000CB8 RID: 3256
	public struct LoadInfo
	{
		// Token: 0x04004499 RID: 17561
		public ProtoBuf.Entity msg;

		// Token: 0x0400449A RID: 17562
		public bool fromDisk;
	}

	// Token: 0x02000CB9 RID: 3257
	public class EntityRealmServer : global::BaseNetworkable.EntityRealm
	{
		// Token: 0x170006A1 RID: 1697
		// (get) Token: 0x06004F99 RID: 20377 RVA: 0x001A6B99 File Offset: 0x001A4D99
		protected override Manager visibilityManager
		{
			get
			{
				if (Network.Net.sv == null)
				{
					return null;
				}
				return Network.Net.sv.visibility;
			}
		}
	}

	// Token: 0x02000CBA RID: 3258
	public abstract class EntityRealm : IEnumerable<global::BaseNetworkable>, IEnumerable
	{
		// Token: 0x0400449B RID: 17563
		private ListDictionary<NetworkableId, global::BaseNetworkable> entityList = new ListDictionary<NetworkableId, global::BaseNetworkable>();

		// Token: 0x170006A2 RID: 1698
		// (get) Token: 0x06004F9B RID: 20379 RVA: 0x001A6BB6 File Offset: 0x001A4DB6
		public int Count
		{
			get
			{
				return this.entityList.Count;
			}
		}

		// Token: 0x170006A3 RID: 1699
		// (get) Token: 0x06004F9C RID: 20380
		protected abstract Manager visibilityManager { get; }

		// Token: 0x06004F9D RID: 20381 RVA: 0x001A6BC3 File Offset: 0x001A4DC3
		public bool Contains(NetworkableId uid)
		{
			return this.entityList.Contains(uid);
		}

		// Token: 0x06004F9E RID: 20382 RVA: 0x001A6BD4 File Offset: 0x001A4DD4
		public global::BaseNetworkable Find(NetworkableId uid)
		{
			global::BaseNetworkable result = null;
			if (!this.entityList.TryGetValue(uid, out result))
			{
				return null;
			}
			return result;
		}

		// Token: 0x06004F9F RID: 20383 RVA: 0x001A6BF8 File Offset: 0x001A4DF8
		public void RegisterID(global::BaseNetworkable ent)
		{
			if (ent.net != null)
			{
				if (this.entityList.Contains(ent.net.ID))
				{
					this.entityList[ent.net.ID] = ent;
					return;
				}
				this.entityList.Add(ent.net.ID, ent);
			}
		}

		// Token: 0x06004FA0 RID: 20384 RVA: 0x001A6C54 File Offset: 0x001A4E54
		public void UnregisterID(global::BaseNetworkable ent)
		{
			if (ent.net != null)
			{
				this.entityList.Remove(ent.net.ID);
			}
		}

		// Token: 0x06004FA1 RID: 20385 RVA: 0x001A6C78 File Offset: 0x001A4E78
		public Group FindGroup(uint uid)
		{
			Manager visibilityManager = this.visibilityManager;
			if (visibilityManager == null)
			{
				return null;
			}
			return visibilityManager.Get(uid);
		}

		// Token: 0x06004FA2 RID: 20386 RVA: 0x001A6C98 File Offset: 0x001A4E98
		public Group TryFindGroup(uint uid)
		{
			Manager visibilityManager = this.visibilityManager;
			if (visibilityManager == null)
			{
				return null;
			}
			return visibilityManager.TryGet(uid);
		}

		// Token: 0x06004FA3 RID: 20387 RVA: 0x001A6CB8 File Offset: 0x001A4EB8
		public void FindInGroup(uint uid, List<global::BaseNetworkable> list)
		{
			Group group = this.TryFindGroup(uid);
			if (group == null)
			{
				return;
			}
			int count = group.networkables.Values.Count;
			Networkable[] buffer = group.networkables.Values.Buffer;
			for (int i = 0; i < count; i++)
			{
				Networkable networkable = buffer[i];
				global::BaseNetworkable baseNetworkable = this.Find(networkable.ID);
				if (!(baseNetworkable == null) && baseNetworkable.net != null && baseNetworkable.net.group != null)
				{
					if (baseNetworkable.net.group.ID != uid)
					{
						Debug.LogWarning("Group ID mismatch: " + baseNetworkable.ToString());
					}
					else
					{
						list.Add(baseNetworkable);
					}
				}
			}
		}

		// Token: 0x06004FA4 RID: 20388 RVA: 0x001A6D68 File Offset: 0x001A4F68
		public IEnumerator<global::BaseNetworkable> GetEnumerator()
		{
			return this.entityList.Values.GetEnumerator();
		}

		// Token: 0x06004FA5 RID: 20389 RVA: 0x001A6D7F File Offset: 0x001A4F7F
		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		// Token: 0x06004FA6 RID: 20390 RVA: 0x001A6D87 File Offset: 0x001A4F87
		public void Clear()
		{
			this.entityList.Clear();
		}
	}

	// Token: 0x02000CBB RID: 3259
	public enum DestroyMode : byte
	{
		// Token: 0x0400449D RID: 17565
		None,
		// Token: 0x0400449E RID: 17566
		Gib
	}
}
