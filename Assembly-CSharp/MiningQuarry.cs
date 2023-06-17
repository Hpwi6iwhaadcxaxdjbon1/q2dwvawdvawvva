using System;
using Facepunch;
using Facepunch.Rust;
using ProtoBuf;
using UnityEngine;

// Token: 0x02000402 RID: 1026
public class MiningQuarry : BaseResourceExtractor
{
	// Token: 0x04001AF5 RID: 6901
	public Animator beltAnimator;

	// Token: 0x04001AF6 RID: 6902
	public Renderer beltScrollRenderer;

	// Token: 0x04001AF7 RID: 6903
	public int scrollMatIndex = 3;

	// Token: 0x04001AF8 RID: 6904
	public SoundPlayer[] onSounds;

	// Token: 0x04001AF9 RID: 6905
	public float processRate = 5f;

	// Token: 0x04001AFA RID: 6906
	public float workToAdd = 15f;

	// Token: 0x04001AFB RID: 6907
	public float workPerFuel = 1000f;

	// Token: 0x04001AFC RID: 6908
	public float pendingWork;

	// Token: 0x04001AFD RID: 6909
	public GameObjectRef bucketDropEffect;

	// Token: 0x04001AFE RID: 6910
	public GameObject bucketDropTransform;

	// Token: 0x04001AFF RID: 6911
	public global::MiningQuarry.ChildPrefab engineSwitchPrefab;

	// Token: 0x04001B00 RID: 6912
	public global::MiningQuarry.ChildPrefab hopperPrefab;

	// Token: 0x04001B01 RID: 6913
	public global::MiningQuarry.ChildPrefab fuelStoragePrefab;

	// Token: 0x04001B02 RID: 6914
	public global::MiningQuarry.QuarryType staticType;

	// Token: 0x04001B03 RID: 6915
	public bool isStatic;

	// Token: 0x04001B04 RID: 6916
	private ResourceDepositManager.ResourceDeposit _linkedDeposit;

	// Token: 0x060022EF RID: 8943 RVA: 0x0002A4EC File Offset: 0x000286EC
	public bool IsEngineOn()
	{
		return base.HasFlag(global::BaseEntity.Flags.On);
	}

	// Token: 0x060022F0 RID: 8944 RVA: 0x000DFB60 File Offset: 0x000DDD60
	private void SetOn(bool isOn)
	{
		base.SetFlag(global::BaseEntity.Flags.On, isOn, false, true);
		this.engineSwitchPrefab.instance.SetFlag(global::BaseEntity.Flags.On, isOn, false, true);
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
		this.engineSwitchPrefab.instance.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
		if (isOn)
		{
			base.InvokeRepeating(new Action(this.ProcessResources), this.processRate, this.processRate);
			return;
		}
		base.CancelInvoke(new Action(this.ProcessResources));
	}

	// Token: 0x060022F1 RID: 8945 RVA: 0x000DFBD7 File Offset: 0x000DDDD7
	public void EngineSwitch(bool isOn)
	{
		if (isOn && this.FuelCheck())
		{
			this.SetOn(true);
			return;
		}
		this.SetOn(false);
	}

	// Token: 0x060022F2 RID: 8946 RVA: 0x000DFBF4 File Offset: 0x000DDDF4
	public override void ServerInit()
	{
		base.ServerInit();
		if (this.isStatic)
		{
			this.UpdateStaticDeposit();
		}
		else
		{
			ResourceDepositManager.ResourceDeposit orCreate = ResourceDepositManager.GetOrCreate(base.transform.position);
			this._linkedDeposit = orCreate;
		}
		this.SpawnChildEntities();
		this.engineSwitchPrefab.instance.SetFlag(global::BaseEntity.Flags.On, base.HasFlag(global::BaseEntity.Flags.On), false, true);
		if (base.isServer)
		{
			global::ItemContainer inventory = this.fuelStoragePrefab.instance.GetComponent<StorageContainer>().inventory;
			inventory.canAcceptItem = (Func<global::Item, int, bool>)Delegate.Combine(inventory.canAcceptItem, new Func<global::Item, int, bool>(this.CanAcceptItem));
		}
	}

	// Token: 0x060022F3 RID: 8947 RVA: 0x000DFC8D File Offset: 0x000DDE8D
	public bool CanAcceptItem(global::Item item, int targetSlot)
	{
		return item.info.shortname == "diesel_barrel";
	}

	// Token: 0x060022F4 RID: 8948 RVA: 0x000DFCA4 File Offset: 0x000DDEA4
	public void UpdateStaticDeposit()
	{
		if (!this.isStatic)
		{
			return;
		}
		if (this._linkedDeposit == null)
		{
			this._linkedDeposit = new ResourceDepositManager.ResourceDeposit();
		}
		else
		{
			this._linkedDeposit._resources.Clear();
		}
		if (this.staticType == global::MiningQuarry.QuarryType.None)
		{
			this._linkedDeposit.Add(ItemManager.FindItemDefinition("stones"), 1f, 1000, 0.3f, ResourceDepositManager.ResourceDeposit.surveySpawnType.ITEM, false);
			this._linkedDeposit.Add(ItemManager.FindItemDefinition("metal.ore"), 1f, 1000, 5f, ResourceDepositManager.ResourceDeposit.surveySpawnType.ITEM, false);
			this._linkedDeposit.Add(ItemManager.FindItemDefinition("sulfur.ore"), 1f, 1000, 7.5f, ResourceDepositManager.ResourceDeposit.surveySpawnType.ITEM, false);
			this._linkedDeposit.Add(ItemManager.FindItemDefinition("hq.metal.ore"), 1f, 1000, 75f, ResourceDepositManager.ResourceDeposit.surveySpawnType.ITEM, false);
		}
		else if (this.staticType == global::MiningQuarry.QuarryType.Basic)
		{
			this._linkedDeposit.Add(ItemManager.FindItemDefinition("metal.ore"), 1f, 1000, 1f, ResourceDepositManager.ResourceDeposit.surveySpawnType.ITEM, false);
			this._linkedDeposit.Add(ItemManager.FindItemDefinition("stones"), 1f, 1000, 0.2f, ResourceDepositManager.ResourceDeposit.surveySpawnType.ITEM, false);
		}
		else if (this.staticType == global::MiningQuarry.QuarryType.Sulfur)
		{
			this._linkedDeposit.Add(ItemManager.FindItemDefinition("sulfur.ore"), 1f, 1000, 1f, ResourceDepositManager.ResourceDeposit.surveySpawnType.ITEM, false);
		}
		else if (this.staticType == global::MiningQuarry.QuarryType.HQM)
		{
			this._linkedDeposit.Add(ItemManager.FindItemDefinition("hq.metal.ore"), 1f, 1000, 20f, ResourceDepositManager.ResourceDeposit.surveySpawnType.ITEM, false);
		}
		this._linkedDeposit.Add(ItemManager.FindItemDefinition("crude.oil"), 1f, 1000, 16.666666f, ResourceDepositManager.ResourceDeposit.surveySpawnType.ITEM, true);
		this._linkedDeposit.Add(ItemManager.FindItemDefinition("lowgradefuel"), 1f, 1000, 5.882353f, ResourceDepositManager.ResourceDeposit.surveySpawnType.ITEM, true);
	}

	// Token: 0x060022F5 RID: 8949 RVA: 0x000DFE8A File Offset: 0x000DE08A
	public override void PostServerLoad()
	{
		base.PostServerLoad();
		this.EngineSwitch(base.HasFlag(global::BaseEntity.Flags.On));
		this.UpdateStaticDeposit();
	}

	// Token: 0x060022F6 RID: 8950 RVA: 0x000DFEA5 File Offset: 0x000DE0A5
	public void SpawnChildEntities()
	{
		this.engineSwitchPrefab.DoSpawn(this);
		this.hopperPrefab.DoSpawn(this);
		this.fuelStoragePrefab.DoSpawn(this);
	}

	// Token: 0x060022F7 RID: 8951 RVA: 0x000DFECC File Offset: 0x000DE0CC
	public void ProcessResources()
	{
		if (this._linkedDeposit == null)
		{
			return;
		}
		if (this.hopperPrefab.instance == null)
		{
			return;
		}
		if (!this.FuelCheck())
		{
			this.SetOn(false);
		}
		float num = Mathf.Min(this.workToAdd, this.pendingWork);
		this.pendingWork -= num;
		foreach (ResourceDepositManager.ResourceDeposit.ResourceDepositEntry resourceDepositEntry in this._linkedDeposit._resources)
		{
			if ((this.canExtractLiquid || !resourceDepositEntry.isLiquid) && (this.canExtractSolid || resourceDepositEntry.isLiquid))
			{
				float workNeeded = resourceDepositEntry.workNeeded;
				int num2 = Mathf.FloorToInt(resourceDepositEntry.workDone / workNeeded);
				resourceDepositEntry.workDone += num;
				int num3 = Mathf.FloorToInt(resourceDepositEntry.workDone / workNeeded);
				if (resourceDepositEntry.workDone > workNeeded)
				{
					resourceDepositEntry.workDone %= workNeeded;
				}
				if (num2 != num3)
				{
					int iAmount = num3 - num2;
					global::Item item = ItemManager.Create(resourceDepositEntry.type, iAmount, 0UL);
					Analytics.Azure.OnQuarryItem(Analytics.Azure.ResourceMode.Produced, item.info.shortname, item.amount, this);
					if (!item.MoveToContainer(this.hopperPrefab.instance.GetComponent<StorageContainer>().inventory, -1, true, false, null, true))
					{
						item.Remove(0f);
						this.SetOn(false);
					}
				}
			}
		}
	}

	// Token: 0x060022F8 RID: 8952 RVA: 0x000E0050 File Offset: 0x000DE250
	public bool FuelCheck()
	{
		if (this.pendingWork > 0f)
		{
			return true;
		}
		global::Item item = this.fuelStoragePrefab.instance.GetComponent<StorageContainer>().inventory.FindItemsByItemName("diesel_barrel");
		if (item != null && item.amount >= 1)
		{
			this.pendingWork += this.workPerFuel;
			Analytics.Azure.OnQuarryItem(Analytics.Azure.ResourceMode.Consumed, item.info.shortname, 1, this);
			item.UseItem(1);
			return true;
		}
		return false;
	}

	// Token: 0x060022F9 RID: 8953 RVA: 0x000E00C8 File Offset: 0x000DE2C8
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		if (info.forDisk)
		{
			if (this.fuelStoragePrefab.instance == null || this.hopperPrefab.instance == null)
			{
				Debug.Log("Cannot save mining quary because children were null");
				return;
			}
			info.msg.miningQuarry = Pool.Get<ProtoBuf.MiningQuarry>();
			info.msg.miningQuarry.extractor = Pool.Get<ResourceExtractor>();
			info.msg.miningQuarry.extractor.fuelContents = this.fuelStoragePrefab.instance.GetComponent<StorageContainer>().inventory.Save();
			info.msg.miningQuarry.extractor.outputContents = this.hopperPrefab.instance.GetComponent<StorageContainer>().inventory.Save();
			info.msg.miningQuarry.staticType = (int)this.staticType;
		}
	}

	// Token: 0x060022FA RID: 8954 RVA: 0x000E01B4 File Offset: 0x000DE3B4
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.fromDisk && info.msg.miningQuarry != null)
		{
			if (this.fuelStoragePrefab.instance == null || this.hopperPrefab.instance == null)
			{
				Debug.Log("Cannot load mining quary because children were null");
				return;
			}
			this.fuelStoragePrefab.instance.GetComponent<StorageContainer>().inventory.Load(info.msg.miningQuarry.extractor.fuelContents);
			this.hopperPrefab.instance.GetComponent<StorageContainer>().inventory.Load(info.msg.miningQuarry.extractor.outputContents);
			this.staticType = (global::MiningQuarry.QuarryType)info.msg.miningQuarry.staticType;
		}
	}

	// Token: 0x060022FB RID: 8955 RVA: 0x000063A5 File Offset: 0x000045A5
	public void Update()
	{
	}

	// Token: 0x02000CD3 RID: 3283
	[Serializable]
	public enum QuarryType
	{
		// Token: 0x04004500 RID: 17664
		None,
		// Token: 0x04004501 RID: 17665
		Basic,
		// Token: 0x04004502 RID: 17666
		Sulfur,
		// Token: 0x04004503 RID: 17667
		HQM
	}

	// Token: 0x02000CD4 RID: 3284
	[Serializable]
	public class ChildPrefab
	{
		// Token: 0x04004504 RID: 17668
		public GameObjectRef prefabToSpawn;

		// Token: 0x04004505 RID: 17669
		public GameObject origin;

		// Token: 0x04004506 RID: 17670
		public global::BaseEntity instance;

		// Token: 0x06004FBD RID: 20413 RVA: 0x001A6FD0 File Offset: 0x001A51D0
		public void DoSpawn(global::MiningQuarry owner)
		{
			if (!this.prefabToSpawn.isValid)
			{
				return;
			}
			this.instance = GameManager.server.CreateEntity(this.prefabToSpawn.resourcePath, this.origin.transform.localPosition, this.origin.transform.localRotation, true);
			this.instance.SetParent(owner, false, false);
			this.instance.Spawn();
		}
	}
}
