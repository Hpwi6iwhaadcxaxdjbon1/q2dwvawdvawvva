using System;
using System.Collections.Generic;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using Rust;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x020000E3 RID: 227
public class TrainCarUnloadable : TrainCar
{
	// Token: 0x04000C77 RID: 3191
	[Header("Train Car Unloadable")]
	[SerializeField]
	private GameObjectRef storagePrefab;

	// Token: 0x04000C78 RID: 3192
	[SerializeField]
	private BoxCollider[] unloadingAreas;

	// Token: 0x04000C79 RID: 3193
	[SerializeField]
	private TrainCarFuelHatches fuelHatches;

	// Token: 0x04000C7A RID: 3194
	[SerializeField]
	private Transform orePlaneVisuals;

	// Token: 0x04000C7B RID: 3195
	[SerializeField]
	private Transform orePlaneColliderDetailed;

	// Token: 0x04000C7C RID: 3196
	[SerializeField]
	private Transform orePlaneColliderWorld;

	// Token: 0x04000C7D RID: 3197
	[SerializeField]
	[Range(0f, 1f)]
	public float vacuumStretchPercent = 0.5f;

	// Token: 0x04000C7E RID: 3198
	[SerializeField]
	private ParticleSystemContainer unloadingFXContainer;

	// Token: 0x04000C7F RID: 3199
	[SerializeField]
	private ParticleSystem unloadingFX;

	// Token: 0x04000C80 RID: 3200
	public TrainCarUnloadable.WagonType wagonType;

	// Token: 0x04000C81 RID: 3201
	private int lootTypeIndex = -1;

	// Token: 0x04000C82 RID: 3202
	private List<EntityRef<LootContainer>> lootContainers = new List<EntityRef<LootContainer>>();

	// Token: 0x04000C83 RID: 3203
	private Vector3 _oreScale = Vector3.one;

	// Token: 0x04000C84 RID: 3204
	private float animPercent;

	// Token: 0x04000C85 RID: 3205
	private float prevAnimTime;

	// Token: 0x04000C86 RID: 3206
	[ServerVar(Help = "How long before an unloadable train car despawns afer being unloaded")]
	public static float decayminutesafterunload = 10f;

	// Token: 0x04000C87 RID: 3207
	private EntityRef<StorageContainer> storageInstance;

	// Token: 0x060013FB RID: 5115 RVA: 0x0009F0AC File Offset: 0x0009D2AC
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("TrainCarUnloadable.OnRpcMessage", 0))
		{
			if (rpc == 4254195175U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_Open ");
				}
				using (TimeWarning.New("RPC_Open", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.MaxDistance.Test(4254195175U, "RPC_Open", this, player, 3f))
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
							this.RPC_Open(msg2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in RPC_Open");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x060013FC RID: 5116 RVA: 0x0009F214 File Offset: 0x0009D414
	public override void OnFlagsChanged(global::BaseEntity.Flags old, global::BaseEntity.Flags next)
	{
		base.OnFlagsChanged(old, next);
		if (old.HasFlag(global::BaseEntity.Flags.Reserved4) != next.HasFlag(global::BaseEntity.Flags.Reserved4) && this.fuelHatches != null)
		{
			this.fuelHatches.LinedUpStateChanged(base.LinedUpToUnload);
		}
	}

	// Token: 0x060013FD RID: 5117 RVA: 0x0009F274 File Offset: 0x0009D474
	protected override void OnChildAdded(global::BaseEntity child)
	{
		base.OnChildAdded(child);
		if (this.IsDead() || base.IsDestroyed)
		{
			return;
		}
		LootContainer lootContainer;
		if (child.TryGetComponent<LootContainer>(out lootContainer))
		{
			if (base.isServer)
			{
				lootContainer.inventory.SetLocked(!this.IsEmpty());
			}
			this.lootContainers.Add(new EntityRef<LootContainer>(lootContainer.net.ID));
		}
		if (base.isServer && child.prefabID == this.storagePrefab.GetEntity().prefabID)
		{
			StorageContainer storageContainer = (StorageContainer)child;
			this.storageInstance.Set(storageContainer);
			if (!Rust.Application.isLoadingSave)
			{
				this.FillWithLoot(storageContainer);
			}
		}
	}

	// Token: 0x060013FE RID: 5118 RVA: 0x0009F31C File Offset: 0x0009D51C
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.baseTrain != null)
		{
			this.lootTypeIndex = info.msg.baseTrain.lootTypeIndex;
			if (base.isServer)
			{
				this.SetVisualOreLevel(info.msg.baseTrain.lootPercent);
			}
		}
	}

	// Token: 0x060013FF RID: 5119 RVA: 0x0009F371 File Offset: 0x0009D571
	public bool IsEmpty()
	{
		return this.GetOrePercent() == 0f;
	}

	// Token: 0x06001400 RID: 5120 RVA: 0x0009F380 File Offset: 0x0009D580
	public bool TryGetLootType(out TrainWagonLootData.LootOption lootOption)
	{
		return TrainWagonLootData.instance.TryGetLootFromIndex(this.lootTypeIndex, out lootOption);
	}

	// Token: 0x06001401 RID: 5121 RVA: 0x0009F393 File Offset: 0x0009D593
	public override bool CanBeLooted(global::BasePlayer player)
	{
		return base.CanBeLooted(player) && !this.IsEmpty();
	}

	// Token: 0x06001402 RID: 5122 RVA: 0x0009F3AC File Offset: 0x0009D5AC
	public int GetFilledLootAmount()
	{
		TrainWagonLootData.LootOption lootOption;
		if (this.TryGetLootType(out lootOption))
		{
			return lootOption.maxLootAmount;
		}
		Debug.LogWarning(base.GetType().Name + ": Called GetFilledLootAmount without a lootTypeIndex set.");
		return 0;
	}

	// Token: 0x06001403 RID: 5123 RVA: 0x0009F3E8 File Offset: 0x0009D5E8
	public void SetVisualOreLevel(float percent)
	{
		if (this.orePlaneColliderDetailed == null)
		{
			return;
		}
		this._oreScale.y = Mathf.Clamp01(percent);
		this.orePlaneColliderDetailed.localScale = this._oreScale;
		if (base.isClient)
		{
			this.orePlaneVisuals.localScale = this._oreScale;
			this.orePlaneVisuals.gameObject.SetActive(percent > 0f);
		}
		if (base.isServer)
		{
			this.orePlaneColliderWorld.localScale = this._oreScale;
		}
	}

	// Token: 0x06001404 RID: 5124 RVA: 0x0009F470 File Offset: 0x0009D670
	private void AnimateUnload(float startPercent)
	{
		this.prevAnimTime = UnityEngine.Time.time;
		this.animPercent = startPercent;
		if (base.isClient && this.unloadingFXContainer != null)
		{
			this.unloadingFXContainer.Play();
		}
		base.InvokeRepeating(new Action(this.UnloadAnimTick), 0f, 0f);
	}

	// Token: 0x06001405 RID: 5125 RVA: 0x0009F4CC File Offset: 0x0009D6CC
	private void UnloadAnimTick()
	{
		this.animPercent -= (UnityEngine.Time.time - this.prevAnimTime) / 40f;
		this.SetVisualOreLevel(this.animPercent);
		this.prevAnimTime = UnityEngine.Time.time;
		if (this.animPercent <= 0f)
		{
			this.EndUnloadAnim();
		}
	}

	// Token: 0x06001406 RID: 5126 RVA: 0x0009F522 File Offset: 0x0009D722
	private void EndUnloadAnim()
	{
		if (base.isClient && this.unloadingFXContainer != null)
		{
			this.unloadingFXContainer.Stop();
		}
		base.CancelInvoke(new Action(this.UnloadAnimTick));
	}

	// Token: 0x06001407 RID: 5127 RVA: 0x0009F557 File Offset: 0x0009D757
	public float GetOrePercent()
	{
		if (base.isServer)
		{
			return TrainWagonLootData.GetOrePercent(this.lootTypeIndex, this.GetStorageContainer());
		}
		return 0f;
	}

	// Token: 0x06001408 RID: 5128 RVA: 0x0009F578 File Offset: 0x0009D778
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.baseTrain = Facepunch.Pool.Get<BaseTrain>();
		info.msg.baseTrain.lootTypeIndex = this.lootTypeIndex;
		info.msg.baseTrain.lootPercent = this.GetOrePercent();
	}

	// Token: 0x06001409 RID: 5129 RVA: 0x0009F5C8 File Offset: 0x0009D7C8
	internal override void DoServerDestroy()
	{
		if (vehicle.vehiclesdroploot)
		{
			foreach (EntityRef<LootContainer> entityRef in this.lootContainers)
			{
				LootContainer lootContainer = entityRef.Get(base.isServer);
				if (lootContainer != null && lootContainer.inventory != null && !lootContainer.inventory.IsLocked())
				{
					lootContainer.DropItems(null);
				}
			}
		}
		base.DoServerDestroy();
	}

	// Token: 0x0600140A RID: 5130 RVA: 0x0009F654 File Offset: 0x0009D854
	public bool IsLinedUpToUnload(BoxCollider unloaderBounds)
	{
		foreach (BoxCollider boxCollider in this.unloadingAreas)
		{
			if (unloaderBounds.bounds.Intersects(boxCollider.bounds))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x0600140B RID: 5131 RVA: 0x0009F694 File Offset: 0x0009D894
	public void FillWithLoot(StorageContainer sc)
	{
		sc.inventory.Clear();
		ItemManager.DoRemoves();
		TrainWagonLootData.LootOption lootOption = TrainWagonLootData.instance.GetLootOption(this.wagonType, out this.lootTypeIndex);
		int amount = UnityEngine.Random.Range(lootOption.minLootAmount, lootOption.maxLootAmount);
		ItemDefinition itemToCreate = ItemManager.FindItemDefinition(lootOption.lootItem.itemid);
		sc.inventory.AddItem(itemToCreate, amount, 0UL, global::ItemContainer.LimitStack.All);
		sc.inventory.SetLocked(true);
		this.SetVisualOreLevel(this.GetOrePercent());
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x0600140C RID: 5132 RVA: 0x0009F71A File Offset: 0x0009D91A
	public void EmptyOutLoot(StorageContainer sc)
	{
		sc.inventory.Clear();
		ItemManager.DoRemoves();
		this.SetVisualOreLevel(this.GetOrePercent());
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x0600140D RID: 5133 RVA: 0x0009F740 File Offset: 0x0009D940
	public void BeginUnloadAnimation()
	{
		float orePercent = this.GetOrePercent();
		this.AnimateUnload(orePercent);
		base.ClientRPC<float>(null, "RPC_AnimateUnload", orePercent);
	}

	// Token: 0x0600140E RID: 5134 RVA: 0x0009F768 File Offset: 0x0009D968
	public void EndEmptyProcess()
	{
		float orePercent = this.GetOrePercent();
		if (orePercent <= 0f)
		{
			this.lootTypeIndex = -1;
			foreach (EntityRef<LootContainer> entityRef in this.lootContainers)
			{
				LootContainer lootContainer = entityRef.Get(base.isServer);
				if (lootContainer != null && lootContainer.inventory != null)
				{
					lootContainer.inventory.SetLocked(false);
				}
			}
		}
		this.SetVisualOreLevel(orePercent);
		base.ClientRPC<float>(null, "RPC_StopAnimateUnload", orePercent);
		this.decayingFor = 0f;
	}

	// Token: 0x0600140F RID: 5135 RVA: 0x0009F818 File Offset: 0x0009DA18
	public StorageContainer GetStorageContainer()
	{
		StorageContainer storageContainer = this.storageInstance.Get(base.isServer);
		if (storageContainer.IsValid())
		{
			return storageContainer;
		}
		return null;
	}

	// Token: 0x06001410 RID: 5136 RVA: 0x0009F842 File Offset: 0x0009DA42
	protected override float GetDecayMinutes(bool hasPassengers)
	{
		if ((this.wagonType == TrainCarUnloadable.WagonType.Ore || this.wagonType == TrainCarUnloadable.WagonType.Fuel) && !hasPassengers && this.IsEmpty())
		{
			return TrainCarUnloadable.decayminutesafterunload;
		}
		return base.GetDecayMinutes(hasPassengers);
	}

	// Token: 0x06001411 RID: 5137 RVA: 0x0009F86D File Offset: 0x0009DA6D
	protected override bool CanDieFromDecayNow()
	{
		return this.IsEmpty() || base.CanDieFromDecayNow();
	}

	// Token: 0x06001412 RID: 5138 RVA: 0x0009F880 File Offset: 0x0009DA80
	public override bool AdminFixUp(int tier)
	{
		if (!base.AdminFixUp(tier))
		{
			return false;
		}
		StorageContainer storageContainer = this.GetStorageContainer();
		if (storageContainer.IsValid())
		{
			if (tier > 1)
			{
				this.FillWithLoot(storageContainer);
			}
			else
			{
				this.EmptyOutLoot(storageContainer);
			}
		}
		return true;
	}

	// Token: 0x06001413 RID: 5139 RVA: 0x0009F8BC File Offset: 0x0009DABC
	public float MinDistToUnloadingArea(Vector3 point)
	{
		float num = float.MaxValue;
		point.y = 0f;
		foreach (BoxCollider boxCollider in this.unloadingAreas)
		{
			Vector3 b = boxCollider.transform.position + boxCollider.transform.rotation * boxCollider.center;
			b.y = 0f;
			float num2 = Vector3.Distance(point, b);
			if (num2 < num)
			{
				num = num2;
			}
		}
		return num;
	}

	// Token: 0x06001414 RID: 5140 RVA: 0x0009F93C File Offset: 0x0009DB3C
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.MaxDistance(3f)]
	public void RPC_Open(global::BaseEntity.RPCMessage msg)
	{
		global::BasePlayer player = msg.player;
		if (player == null)
		{
			return;
		}
		if (!this.CanBeLooted(player))
		{
			return;
		}
		StorageContainer storageContainer = this.GetStorageContainer();
		if (storageContainer.IsValid())
		{
			storageContainer.PlayerOpenLoot(player, "", true);
			return;
		}
		Debug.LogError(base.GetType().Name + ": No container component found.");
	}

	// Token: 0x02000C0E RID: 3086
	public enum WagonType
	{
		// Token: 0x040041CE RID: 16846
		Ore,
		// Token: 0x040041CF RID: 16847
		Lootboxes,
		// Token: 0x040041D0 RID: 16848
		Fuel
	}
}
