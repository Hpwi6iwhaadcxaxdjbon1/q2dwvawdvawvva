using System;
using System.Collections.Generic;
using Facepunch;
using ProtoBuf;
using Rust.Modular;
using UnityEngine;

// Token: 0x02000492 RID: 1170
public abstract class BaseModularVehicle : GroundVehicle, global::PlayerInventory.ICanMoveFrom, IPrefabPreProcess
{
	// Token: 0x04001EBA RID: 7866
	internal bool inEditableLocation;

	// Token: 0x04001EBB RID: 7867
	private bool prevEditable;

	// Token: 0x04001EBC RID: 7868
	internal bool immuneToDecay;

	// Token: 0x04001EBE RID: 7870
	protected Vector3 realLocalCOM;

	// Token: 0x04001EBF RID: 7871
	public global::Item AssociatedItemInstance;

	// Token: 0x04001EC0 RID: 7872
	private bool disablePhysics;

	// Token: 0x04001EC1 RID: 7873
	[Header("Modular Vehicle")]
	[SerializeField]
	private List<ModularVehicleSocket> moduleSockets;

	// Token: 0x04001EC2 RID: 7874
	[SerializeField]
	private Transform centreOfMassTransform;

	// Token: 0x04001EC3 RID: 7875
	[SerializeField]
	protected Transform waterSample;

	// Token: 0x04001EC4 RID: 7876
	[SerializeField]
	private LODGroup lodGroup;

	// Token: 0x04001EC5 RID: 7877
	public GameObjectRef keyEnterDialog;

	// Token: 0x04001EC7 RID: 7879
	private float _mass = -1f;

	// Token: 0x04001ECA RID: 7882
	public const global::BaseEntity.Flags FLAG_KINEMATIC = global::BaseEntity.Flags.Reserved6;

	// Token: 0x04001ECB RID: 7883
	private Dictionary<BaseVehicleModule, Action> moduleAddActions = new Dictionary<BaseVehicleModule, Action>();

	// Token: 0x17000323 RID: 803
	// (get) Token: 0x06002657 RID: 9815 RVA: 0x000F1454 File Offset: 0x000EF654
	// (set) Token: 0x06002658 RID: 9816 RVA: 0x000F145C File Offset: 0x000EF65C
	public ModularVehicleInventory Inventory { get; private set; }

	// Token: 0x06002659 RID: 9817 RVA: 0x000F1468 File Offset: 0x000EF668
	public override void ServerInit()
	{
		base.ServerInit();
		if (!this.disablePhysics)
		{
			this.rigidBody.isKinematic = false;
		}
		this.prevEditable = this.IsEditableNow;
		if (this.Inventory == null)
		{
			this.Inventory = new ModularVehicleInventory(this, this.AssociatedItemDef, true);
		}
	}

	// Token: 0x0600265A RID: 9818 RVA: 0x000F14B6 File Offset: 0x000EF6B6
	public override void PreServerLoad()
	{
		base.PreServerLoad();
		if (this.Inventory == null)
		{
			this.Inventory = new ModularVehicleInventory(this, this.AssociatedItemDef, false);
		}
	}

	// Token: 0x0600265B RID: 9819 RVA: 0x000F14DC File Offset: 0x000EF6DC
	public override void PostServerLoad()
	{
		base.PostServerLoad();
		if (this.Inventory != null && !this.Inventory.UID.IsValid)
		{
			this.Inventory.GiveUIDs();
		}
		base.SetFlag(global::BaseEntity.Flags.Open, false, false, true);
	}

	// Token: 0x0600265C RID: 9820 RVA: 0x000F1521 File Offset: 0x000EF721
	internal override void DoServerDestroy()
	{
		base.DoServerDestroy();
		if (this.Inventory != null)
		{
			this.Inventory.Dispose();
		}
	}

	// Token: 0x0600265D RID: 9821 RVA: 0x00074975 File Offset: 0x00072B75
	public override float MaxVelocity()
	{
		return Mathf.Max(this.GetMaxForwardSpeed() * 1.3f, 30f);
	}

	// Token: 0x0600265E RID: 9822
	public abstract bool IsComplete();

	// Token: 0x0600265F RID: 9823 RVA: 0x000F153C File Offset: 0x000EF73C
	public bool CouldBeEdited()
	{
		return !this.AnyMounted() && !this.IsDead();
	}

	// Token: 0x06002660 RID: 9824 RVA: 0x000F1551 File Offset: 0x000EF751
	public void DisablePhysics()
	{
		this.disablePhysics = true;
		this.rigidBody.isKinematic = true;
	}

	// Token: 0x06002661 RID: 9825 RVA: 0x000F1566 File Offset: 0x000EF766
	public void EnablePhysics()
	{
		this.disablePhysics = false;
		this.rigidBody.isKinematic = false;
	}

	// Token: 0x06002662 RID: 9826 RVA: 0x000F157C File Offset: 0x000EF77C
	public override void VehicleFixedUpdate()
	{
		base.VehicleFixedUpdate();
		if (this.IsEditableNow != this.prevEditable)
		{
			base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
			this.prevEditable = this.IsEditableNow;
		}
		base.SetFlag(global::BaseEntity.Flags.Reserved6, this.rigidBody.isKinematic, false, true);
	}

	// Token: 0x06002663 RID: 9827 RVA: 0x000F15C8 File Offset: 0x000EF7C8
	public override bool MountEligable(global::BasePlayer player)
	{
		return base.MountEligable(player) && !this.IsDead() && (!base.HasDriver() || base.Velocity.magnitude < 2f);
	}

	// Token: 0x06002664 RID: 9828 RVA: 0x000F160A File Offset: 0x000EF80A
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.modularVehicle = Pool.Get<ModularVehicle>();
		info.msg.modularVehicle.editable = this.IsEditableNow;
	}

	// Token: 0x06002665 RID: 9829 RVA: 0x000F163C File Offset: 0x000EF83C
	public bool CanMoveFrom(global::BasePlayer player, global::Item item)
	{
		BaseVehicleModule moduleForItem = this.GetModuleForItem(item);
		return !(moduleForItem != null) || moduleForItem.CanBeMovedNow();
	}

	// Token: 0x06002666 RID: 9830
	protected abstract Vector3 GetCOMMultiplier();

	// Token: 0x06002667 RID: 9831
	public abstract void ModuleHurt(BaseVehicleModule hurtModule, HitInfo info);

	// Token: 0x06002668 RID: 9832
	public abstract void ModuleReachedZeroHealth();

	// Token: 0x06002669 RID: 9833 RVA: 0x000F1664 File Offset: 0x000EF864
	public bool TryAddModule(global::Item moduleItem, int socketIndex)
	{
		string str;
		if (!this.ModuleCanBeAdded(moduleItem, socketIndex, out str))
		{
			Debug.LogError(base.GetType().Name + ": Can't add module: " + str);
			return false;
		}
		bool flag = this.Inventory.TryAddModuleItem(moduleItem, socketIndex);
		if (!flag)
		{
			Debug.LogError(base.GetType().Name + ": Couldn't add new item!");
		}
		return flag;
	}

	// Token: 0x0600266A RID: 9834 RVA: 0x000F16C4 File Offset: 0x000EF8C4
	public bool TryAddModule(global::Item moduleItem)
	{
		ItemModVehicleModule component = moduleItem.info.GetComponent<ItemModVehicleModule>();
		if (component == null)
		{
			return false;
		}
		int socketsTaken = component.socketsTaken;
		int num = this.Inventory.TryGetFreeSocket(socketsTaken);
		return num >= 0 && this.TryAddModule(moduleItem, num);
	}

	// Token: 0x0600266B RID: 9835 RVA: 0x000F170C File Offset: 0x000EF90C
	public bool ModuleCanBeAdded(global::Item moduleItem, int socketIndex, out string failureReason)
	{
		if (!base.isServer)
		{
			failureReason = "Can only add modules on server";
			return false;
		}
		if (moduleItem == null)
		{
			failureReason = "Module item is null";
			return false;
		}
		if (moduleItem.info.category != ItemCategory.Component)
		{
			failureReason = "Not a component type item";
			return false;
		}
		ItemModVehicleModule component = moduleItem.info.GetComponent<ItemModVehicleModule>();
		if (component == null)
		{
			failureReason = "Not the right item module type";
			return false;
		}
		int socketsTaken = component.socketsTaken;
		if (socketIndex < 0)
		{
			socketIndex = this.Inventory.TryGetFreeSocket(socketsTaken);
		}
		if (!this.Inventory.SocketsAreFree(socketIndex, socketsTaken, moduleItem))
		{
			failureReason = "One or more desired sockets already in use";
			return false;
		}
		failureReason = string.Empty;
		return true;
	}

	// Token: 0x0600266C RID: 9836 RVA: 0x000F17A8 File Offset: 0x000EF9A8
	public BaseVehicleModule CreatePhysicalModuleEntity(global::Item moduleItem, ItemModVehicleModule itemModModule, int socketIndex)
	{
		Vector3 worldPosition = this.moduleSockets[socketIndex].WorldPosition;
		Quaternion worldRotation = this.moduleSockets[socketIndex].WorldRotation;
		BaseVehicleModule baseVehicleModule = itemModModule.CreateModuleEntity(this, worldPosition, worldRotation);
		baseVehicleModule.AssociatedItemInstance = moduleItem;
		this.SetUpModule(baseVehicleModule, moduleItem);
		return baseVehicleModule;
	}

	// Token: 0x0600266D RID: 9837 RVA: 0x000F17F3 File Offset: 0x000EF9F3
	public void SetUpModule(BaseVehicleModule moduleEntity, global::Item moduleItem)
	{
		moduleEntity.InitializeHealth(moduleItem.condition, moduleItem.maxCondition);
		if (moduleItem.condition < moduleItem.maxCondition)
		{
			moduleEntity.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
		}
	}

	// Token: 0x0600266E RID: 9838 RVA: 0x000F181C File Offset: 0x000EFA1C
	public global::Item GetVehicleItem(ItemId itemUID)
	{
		global::Item item = this.Inventory.ChassisContainer.FindItemByUID(itemUID);
		if (item == null)
		{
			item = this.Inventory.ModuleContainer.FindItemByUID(itemUID);
		}
		return item;
	}

	// Token: 0x0600266F RID: 9839 RVA: 0x000F1854 File Offset: 0x000EFA54
	public BaseVehicleModule GetModuleForItem(global::Item item)
	{
		if (item == null)
		{
			return null;
		}
		foreach (BaseVehicleModule baseVehicleModule in this.AttachedModuleEntities)
		{
			if (baseVehicleModule.AssociatedItemInstance == item)
			{
				return baseVehicleModule;
			}
		}
		return null;
	}

	// Token: 0x06002670 RID: 9840 RVA: 0x000F18B8 File Offset: 0x000EFAB8
	private void SetMass(float mass)
	{
		this.TotalMass = mass;
		this.rigidBody.mass = this.TotalMass;
	}

	// Token: 0x06002671 RID: 9841 RVA: 0x000F18D2 File Offset: 0x000EFAD2
	private void SetCOM(Vector3 com)
	{
		this.realLocalCOM = com;
		this.rigidBody.centerOfMass = Vector3.Scale(this.realLocalCOM, this.GetCOMMultiplier());
	}

	// Token: 0x17000324 RID: 804
	// (get) Token: 0x06002672 RID: 9842 RVA: 0x000F18F7 File Offset: 0x000EFAF7
	public Vector3 CentreOfMass
	{
		get
		{
			return this.centreOfMassTransform.localPosition;
		}
	}

	// Token: 0x17000325 RID: 805
	// (get) Token: 0x06002673 RID: 9843 RVA: 0x000F1904 File Offset: 0x000EFB04
	public int NumAttachedModules
	{
		get
		{
			return this.AttachedModuleEntities.Count;
		}
	}

	// Token: 0x17000326 RID: 806
	// (get) Token: 0x06002674 RID: 9844 RVA: 0x000F1911 File Offset: 0x000EFB11
	public bool HasAnyModules
	{
		get
		{
			return this.AttachedModuleEntities.Count > 0;
		}
	}

	// Token: 0x17000327 RID: 807
	// (get) Token: 0x06002675 RID: 9845 RVA: 0x000F1921 File Offset: 0x000EFB21
	public List<BaseVehicleModule> AttachedModuleEntities { get; } = new List<BaseVehicleModule>();

	// Token: 0x17000328 RID: 808
	// (get) Token: 0x06002676 RID: 9846 RVA: 0x000F1929 File Offset: 0x000EFB29
	public int TotalSockets
	{
		get
		{
			return this.moduleSockets.Count;
		}
	}

	// Token: 0x17000329 RID: 809
	// (get) Token: 0x06002677 RID: 9847 RVA: 0x000F1938 File Offset: 0x000EFB38
	public int NumFreeSockets
	{
		get
		{
			int num = 0;
			for (int i = 0; i < this.NumAttachedModules; i++)
			{
				num += this.AttachedModuleEntities[i].GetNumSocketsTaken();
			}
			return this.TotalSockets - num;
		}
	}

	// Token: 0x1700032A RID: 810
	// (get) Token: 0x06002678 RID: 9848 RVA: 0x000F1974 File Offset: 0x000EFB74
	private float Mass
	{
		get
		{
			if (base.isServer)
			{
				return this.rigidBody.mass;
			}
			return this._mass;
		}
	}

	// Token: 0x1700032B RID: 811
	// (get) Token: 0x06002679 RID: 9849 RVA: 0x000F1990 File Offset: 0x000EFB90
	// (set) Token: 0x0600267A RID: 9850 RVA: 0x000F1998 File Offset: 0x000EFB98
	public float TotalMass { get; private set; }

	// Token: 0x1700032C RID: 812
	// (get) Token: 0x0600267B RID: 9851 RVA: 0x00003F9B File Offset: 0x0000219B
	public bool IsKinematic
	{
		get
		{
			return base.HasFlag(global::BaseEntity.Flags.Reserved6);
		}
	}

	// Token: 0x1700032D RID: 813
	// (get) Token: 0x0600267C RID: 9852 RVA: 0x00007A3C File Offset: 0x00005C3C
	public virtual bool IsLockable
	{
		get
		{
			return false;
		}
	}

	// Token: 0x1700032E RID: 814
	// (get) Token: 0x0600267D RID: 9853 RVA: 0x000F19A1 File Offset: 0x000EFBA1
	// (set) Token: 0x0600267E RID: 9854 RVA: 0x000F19A9 File Offset: 0x000EFBA9
	public bool HasInited { get; private set; }

	// Token: 0x1700032F RID: 815
	// (get) Token: 0x0600267F RID: 9855 RVA: 0x00050F0B File Offset: 0x0004F10B
	private ItemDefinition AssociatedItemDef
	{
		get
		{
			return this.repair.itemTarget;
		}
	}

	// Token: 0x17000330 RID: 816
	// (get) Token: 0x06002680 RID: 9856 RVA: 0x000F19B2 File Offset: 0x000EFBB2
	public bool IsEditableNow
	{
		get
		{
			return base.isServer && this.inEditableLocation && this.CouldBeEdited();
		}
	}

	// Token: 0x06002681 RID: 9857 RVA: 0x000F19D0 File Offset: 0x000EFBD0
	public override void InitShared()
	{
		base.InitShared();
		this.AddMass(this.Mass, this.CentreOfMass, base.transform.position);
		this.HasInited = true;
		foreach (BaseVehicleModule baseVehicleModule in this.AttachedModuleEntities)
		{
			baseVehicleModule.RefreshConditionals(false);
		}
	}

	// Token: 0x06002682 RID: 9858 RVA: 0x0000441C File Offset: 0x0000261C
	public virtual bool PlayerCanUseThis(global::BasePlayer player, ModularCarCodeLock.LockType lockType)
	{
		return true;
	}

	// Token: 0x06002683 RID: 9859 RVA: 0x000F1A4C File Offset: 0x000EFC4C
	public bool TryDeduceSocketIndex(BaseVehicleModule addedModule, out int index)
	{
		if (addedModule.FirstSocketIndex >= 0)
		{
			index = addedModule.FirstSocketIndex;
			return index >= 0;
		}
		index = -1;
		for (int i = 0; i < this.moduleSockets.Count; i++)
		{
			if (Vector3.SqrMagnitude(this.moduleSockets[i].WorldPosition - addedModule.transform.position) < 0.1f)
			{
				index = i;
				return true;
			}
		}
		return false;
	}

	// Token: 0x06002684 RID: 9860 RVA: 0x000F1AC0 File Offset: 0x000EFCC0
	public void AddMass(float moduleMass, Vector3 moduleCOM, Vector3 moduleWorldPos)
	{
		if (base.isServer)
		{
			Vector3 vector = base.transform.InverseTransformPoint(moduleWorldPos) + moduleCOM;
			if (this.TotalMass == 0f)
			{
				this.SetMass(moduleMass);
				this.SetCOM(vector);
				return;
			}
			float num = this.TotalMass + moduleMass;
			Vector3 com = this.realLocalCOM * (this.TotalMass / num) + vector * (moduleMass / num);
			this.SetMass(num);
			this.SetCOM(com);
		}
	}

	// Token: 0x06002685 RID: 9861 RVA: 0x000F1B40 File Offset: 0x000EFD40
	public void RemoveMass(float moduleMass, Vector3 moduleCOM, Vector3 moduleWorldPos)
	{
		if (base.isServer)
		{
			float num = this.TotalMass - moduleMass;
			Vector3 a = base.transform.InverseTransformPoint(moduleWorldPos) + moduleCOM;
			Vector3 com = (this.realLocalCOM - a * (moduleMass / this.TotalMass)) / (num / this.TotalMass);
			this.SetMass(num);
			this.SetCOM(com);
		}
	}

	// Token: 0x06002686 RID: 9862 RVA: 0x000F1BA8 File Offset: 0x000EFDA8
	public bool TryGetModuleAt(int socketIndex, out BaseVehicleModule result)
	{
		if (socketIndex < 0 || socketIndex >= this.moduleSockets.Count)
		{
			result = null;
			return false;
		}
		foreach (BaseVehicleModule baseVehicleModule in this.AttachedModuleEntities)
		{
			int firstSocketIndex = baseVehicleModule.FirstSocketIndex;
			int num = firstSocketIndex + baseVehicleModule.GetNumSocketsTaken() - 1;
			if (firstSocketIndex <= socketIndex && num >= socketIndex)
			{
				result = baseVehicleModule;
				return true;
			}
		}
		result = null;
		return false;
	}

	// Token: 0x06002687 RID: 9863 RVA: 0x000F1C30 File Offset: 0x000EFE30
	public ModularVehicleSocket GetSocket(int index)
	{
		if (index < 0 || index >= this.moduleSockets.Count)
		{
			return null;
		}
		return this.moduleSockets[index];
	}

	// Token: 0x06002688 RID: 9864 RVA: 0x000F1C52 File Offset: 0x000EFE52
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		ModularVehicle modularVehicle = info.msg.modularVehicle;
	}

	// Token: 0x06002689 RID: 9865 RVA: 0x000F1C67 File Offset: 0x000EFE67
	protected override bool CanPushNow(global::BasePlayer pusher)
	{
		return base.CanPushNow(pusher) && !this.IsKinematic && !this.IsEditableNow;
	}

	// Token: 0x0600268A RID: 9866 RVA: 0x000F1C88 File Offset: 0x000EFE88
	protected override void OnChildAdded(global::BaseEntity childEntity)
	{
		base.OnChildAdded(childEntity);
		BaseVehicleModule module;
		if ((module = (childEntity as BaseVehicleModule)) != null)
		{
			Action action = delegate()
			{
				this.ModuleEntityAdded(module);
			};
			this.moduleAddActions[module] = action;
			module.Invoke(action, 0f);
		}
	}

	// Token: 0x0600268B RID: 9867 RVA: 0x000F1CEC File Offset: 0x000EFEEC
	protected override void OnChildRemoved(global::BaseEntity childEntity)
	{
		base.OnChildRemoved(childEntity);
		BaseVehicleModule removedModule;
		if ((removedModule = (childEntity as BaseVehicleModule)) != null)
		{
			this.ModuleEntityRemoved(removedModule);
		}
	}

	// Token: 0x0600268C RID: 9868 RVA: 0x000F1D14 File Offset: 0x000EFF14
	protected virtual void ModuleEntityAdded(BaseVehicleModule addedModule)
	{
		if (this.AttachedModuleEntities.Contains(addedModule))
		{
			return;
		}
		if (base.isServer && (this == null || this.IsDead() || base.IsDestroyed))
		{
			if (addedModule != null && !addedModule.IsDestroyed)
			{
				addedModule.Kill(global::BaseNetworkable.DestroyMode.None);
			}
			return;
		}
		int num = -1;
		if (base.isServer && addedModule.AssociatedItemInstance != null)
		{
			num = addedModule.AssociatedItemInstance.position;
		}
		if (num == -1 && !this.TryDeduceSocketIndex(addedModule, out num))
		{
			string text = string.Format("{0}: Couldn't get socket index from position ({1}).", base.GetType().Name, addedModule.transform.position);
			for (int i = 0; i < this.moduleSockets.Count; i++)
			{
				text += string.Format(" Sqr dist to socket {0} at {1} is {2}.", i, this.moduleSockets[i].WorldPosition, Vector3.SqrMagnitude(this.moduleSockets[i].WorldPosition - addedModule.transform.position));
			}
			Debug.LogError(text, addedModule.gameObject);
			return;
		}
		if (this.moduleAddActions.ContainsKey(addedModule))
		{
			this.moduleAddActions.Remove(addedModule);
		}
		this.AttachedModuleEntities.Add(addedModule);
		addedModule.ModuleAdded(this, num);
		this.AddMass(addedModule.Mass, addedModule.CentreOfMass, addedModule.transform.position);
		if (base.isServer && !this.Inventory.TrySyncModuleInventory(addedModule, num))
		{
			Debug.LogError(string.Format("{0}: Unable to add module {1} to socket ({2}). Destroying it.", base.GetType().Name, addedModule.name, num), base.gameObject);
			addedModule.Kill(global::BaseNetworkable.DestroyMode.None);
			this.AttachedModuleEntities.Remove(addedModule);
			return;
		}
		this.RefreshModulesExcept(addedModule);
		if (base.isServer)
		{
			this.UpdateMountFlags();
		}
	}

	// Token: 0x0600268D RID: 9869 RVA: 0x000F1EF8 File Offset: 0x000F00F8
	protected virtual void ModuleEntityRemoved(BaseVehicleModule removedModule)
	{
		if (base.IsDestroyed)
		{
			return;
		}
		if (this.moduleAddActions.ContainsKey(removedModule))
		{
			removedModule.CancelInvoke(this.moduleAddActions[removedModule]);
			this.moduleAddActions.Remove(removedModule);
		}
		if (!this.AttachedModuleEntities.Contains(removedModule))
		{
			return;
		}
		this.RemoveMass(removedModule.Mass, removedModule.CentreOfMass, removedModule.transform.position);
		this.AttachedModuleEntities.Remove(removedModule);
		removedModule.ModuleRemoved();
		this.RefreshModulesExcept(removedModule);
		if (base.isServer)
		{
			this.UpdateMountFlags();
		}
	}

	// Token: 0x0600268E RID: 9870 RVA: 0x000F1F90 File Offset: 0x000F0190
	private void RefreshModulesExcept(BaseVehicleModule ignoredModule)
	{
		foreach (BaseVehicleModule baseVehicleModule in this.AttachedModuleEntities)
		{
			if (baseVehicleModule != ignoredModule)
			{
				baseVehicleModule.OtherVehicleModulesChanged();
			}
		}
	}
}
