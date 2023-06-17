using System;
using System.Collections.Generic;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using Rust;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x020000AD RID: 173
public class PlanterBox : StorageContainer, ISplashable
{
	// Token: 0x04000A3C RID: 2620
	public int soilSaturation;

	// Token: 0x04000A3D RID: 2621
	public int soilSaturationMax = 8000;

	// Token: 0x04000A3E RID: 2622
	public MeshRenderer soilRenderer;

	// Token: 0x04000A3F RID: 2623
	private static readonly float MinimumSaturationTriggerLevel = ConVar.Server.optimalPlanterQualitySaturation - 0.2f;

	// Token: 0x04000A40 RID: 2624
	private static readonly float MaximumSaturationTriggerLevel = ConVar.Server.optimalPlanterQualitySaturation + 0.1f;

	// Token: 0x04000A41 RID: 2625
	private TimeCachedValue<float> sunExposure;

	// Token: 0x04000A42 RID: 2626
	private TimeCachedValue<float> artificialLightExposure;

	// Token: 0x04000A43 RID: 2627
	private TimeCachedValue<float> plantTemperature;

	// Token: 0x04000A44 RID: 2628
	private TimeCachedValue<float> plantArtificalTemperature;

	// Token: 0x04000A45 RID: 2629
	private TimeSince lastSplashNetworkUpdate;

	// Token: 0x04000A46 RID: 2630
	private TimeSince lastRainCheck;

	// Token: 0x06000FBA RID: 4026 RVA: 0x00083A60 File Offset: 0x00081C60
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("PlanterBox.OnRpcMessage", 0))
		{
			if (rpc == 2965786167U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_RequestSaturationUpdate ");
				}
				using (TimeWarning.New("RPC_RequestSaturationUpdate", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.MaxDistance.Test(2965786167U, "RPC_RequestSaturationUpdate", this, player, 3f))
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
							this.RPC_RequestSaturationUpdate(msg2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in RPC_RequestSaturationUpdate");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000FBB RID: 4027 RVA: 0x00083BC8 File Offset: 0x00081DC8
	public override void ServerInit()
	{
		base.ServerInit();
		base.inventory.onItemAddedRemoved = new Action<global::Item, bool>(this.OnItemAddedOrRemoved);
		base.inventory.SetOnlyAllowedItem(this.allowedItem);
		global::ItemContainer inventory = base.inventory;
		inventory.canAcceptItem = (Func<global::Item, int, bool>)Delegate.Combine(inventory.canAcceptItem, new Func<global::Item, int, bool>(this.InventoryItemFilter));
		this.sunExposure = new TimeCachedValue<float>
		{
			refreshCooldown = 30f,
			refreshRandomRange = 5f,
			updateValue = new Func<float>(this.CalculateSunExposure)
		};
		this.artificialLightExposure = new TimeCachedValue<float>
		{
			refreshCooldown = 60f,
			refreshRandomRange = 5f,
			updateValue = new Func<float>(this.CalculateArtificialLightExposure)
		};
		this.plantTemperature = new TimeCachedValue<float>
		{
			refreshCooldown = 20f,
			refreshRandomRange = 5f,
			updateValue = new Func<float>(this.CalculatePlantTemperature)
		};
		this.plantArtificalTemperature = new TimeCachedValue<float>
		{
			refreshCooldown = 60f,
			refreshRandomRange = 5f,
			updateValue = new Func<float>(this.CalculateArtificialTemperature)
		};
		this.lastRainCheck = 0f;
		base.InvokeRandomized(new Action(this.CalculateRainFactor), 20f, 30f, 15f);
	}

	// Token: 0x06000FBC RID: 4028 RVA: 0x00083D28 File Offset: 0x00081F28
	public override void OnItemAddedOrRemoved(global::Item item, bool added)
	{
		base.OnItemAddedOrRemoved(item, added);
		if (added && this.ItemIsFertilizer(item))
		{
			this.FertilizeGrowables();
		}
	}

	// Token: 0x06000FBD RID: 4029 RVA: 0x00083D44 File Offset: 0x00081F44
	public bool InventoryItemFilter(global::Item item, int targetSlot)
	{
		return item != null && this.ItemIsFertilizer(item);
	}

	// Token: 0x06000FBE RID: 4030 RVA: 0x00083D57 File Offset: 0x00081F57
	private bool ItemIsFertilizer(global::Item item)
	{
		return item.info.shortname == "fertilizer";
	}

	// Token: 0x06000FBF RID: 4031 RVA: 0x00083D6E File Offset: 0x00081F6E
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.resource = Facepunch.Pool.Get<BaseResource>();
		info.msg.resource.stage = this.soilSaturation;
	}

	// Token: 0x06000FC0 RID: 4032 RVA: 0x00083D9D File Offset: 0x00081F9D
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.resource != null)
		{
			this.soilSaturation = info.msg.resource.stage;
		}
	}

	// Token: 0x1700017C RID: 380
	// (get) Token: 0x06000FC1 RID: 4033 RVA: 0x00083DC9 File Offset: 0x00081FC9
	public float soilSaturationFraction
	{
		get
		{
			return (float)this.soilSaturation / (float)this.soilSaturationMax;
		}
	}

	// Token: 0x1700017D RID: 381
	// (get) Token: 0x06000FC2 RID: 4034 RVA: 0x00083DDA File Offset: 0x00081FDA
	public int availableIdealWaterCapacity
	{
		get
		{
			return Mathf.Max(this.availableIdealWaterCapacity, Mathf.Max(this.idealSaturation - this.soilSaturation, 0));
		}
	}

	// Token: 0x1700017E RID: 382
	// (get) Token: 0x06000FC3 RID: 4035 RVA: 0x00083DFA File Offset: 0x00081FFA
	public int availableWaterCapacity
	{
		get
		{
			return this.soilSaturationMax - this.soilSaturation;
		}
	}

	// Token: 0x1700017F RID: 383
	// (get) Token: 0x06000FC4 RID: 4036 RVA: 0x00083E09 File Offset: 0x00082009
	public int idealSaturation
	{
		get
		{
			return Mathf.FloorToInt((float)this.soilSaturationMax * ConVar.Server.optimalPlanterQualitySaturation);
		}
	}

	// Token: 0x17000180 RID: 384
	// (get) Token: 0x06000FC5 RID: 4037 RVA: 0x00083E1D File Offset: 0x0008201D
	public bool BelowMinimumSaturationTriggerLevel
	{
		get
		{
			return this.soilSaturationFraction < PlanterBox.MinimumSaturationTriggerLevel;
		}
	}

	// Token: 0x17000181 RID: 385
	// (get) Token: 0x06000FC6 RID: 4038 RVA: 0x00083E2C File Offset: 0x0008202C
	public bool AboveMaximumSaturationTriggerLevel
	{
		get
		{
			return this.soilSaturationFraction > PlanterBox.MaximumSaturationTriggerLevel;
		}
	}

	// Token: 0x06000FC7 RID: 4039 RVA: 0x00083E3C File Offset: 0x0008203C
	public void FertilizeGrowables()
	{
		int num = this.GetFertilizerCount();
		if (num <= 0)
		{
			return;
		}
		foreach (global::BaseEntity baseEntity in this.children)
		{
			if (!(baseEntity == null))
			{
				global::GrowableEntity growableEntity = baseEntity as global::GrowableEntity;
				if (!(growableEntity == null) && !growableEntity.Fertilized && this.ConsumeFertilizer())
				{
					growableEntity.Fertilize();
					num--;
					if (num == 0)
					{
						break;
					}
				}
			}
		}
	}

	// Token: 0x06000FC8 RID: 4040 RVA: 0x00083ECC File Offset: 0x000820CC
	public int GetFertilizerCount()
	{
		int num = 0;
		for (int i = 0; i < base.inventory.capacity; i++)
		{
			global::Item slot = base.inventory.GetSlot(i);
			if (slot != null && this.ItemIsFertilizer(slot))
			{
				num += slot.amount;
			}
		}
		return num;
	}

	// Token: 0x06000FC9 RID: 4041 RVA: 0x00083F14 File Offset: 0x00082114
	public bool ConsumeFertilizer()
	{
		for (int i = 0; i < base.inventory.capacity; i++)
		{
			global::Item slot = base.inventory.GetSlot(i);
			if (slot != null && this.ItemIsFertilizer(slot))
			{
				int num = Mathf.Min(1, slot.amount);
				if (num > 0)
				{
					slot.UseItem(num);
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x06000FCA RID: 4042 RVA: 0x00083F6C File Offset: 0x0008216C
	public int ConsumeWater(int amount, global::GrowableEntity ignoreEntity = null)
	{
		int num = Mathf.Min(amount, this.soilSaturation);
		this.soilSaturation -= num;
		this.RefreshGrowables(ignoreEntity);
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
		return num;
	}

	// Token: 0x06000FCB RID: 4043 RVA: 0x00083FA4 File Offset: 0x000821A4
	public bool WantsSplash(ItemDefinition splashType, int amount)
	{
		return !base.IsDestroyed && !(splashType == null) && splashType.shortname != null && (splashType.shortname == "water.salt" || this.soilSaturation < this.soilSaturationMax);
	}

	// Token: 0x06000FCC RID: 4044 RVA: 0x00083FF0 File Offset: 0x000821F0
	public int DoSplash(ItemDefinition splashType, int amount)
	{
		if (splashType.shortname == "water.salt")
		{
			this.soilSaturation = 0;
			this.RefreshGrowables(null);
			if (this.lastSplashNetworkUpdate > 60f)
			{
				base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
				this.lastSplashNetworkUpdate = 0f;
			}
			return amount;
		}
		int num = Mathf.Min(this.availableWaterCapacity, amount);
		this.soilSaturation += num;
		this.RefreshGrowables(null);
		if (this.lastSplashNetworkUpdate > 60f)
		{
			base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
			this.lastSplashNetworkUpdate = 0f;
		}
		return num;
	}

	// Token: 0x06000FCD RID: 4045 RVA: 0x00084094 File Offset: 0x00082294
	private void RefreshGrowables(global::GrowableEntity ignoreEntity = null)
	{
		if (this.children == null)
		{
			return;
		}
		foreach (global::BaseEntity baseEntity in this.children)
		{
			global::GrowableEntity growableEntity;
			if (!(baseEntity == null) && !(baseEntity == ignoreEntity) && (growableEntity = (baseEntity as global::GrowableEntity)) != null)
			{
				growableEntity.QueueForQualityUpdate();
			}
		}
	}

	// Token: 0x06000FCE RID: 4046 RVA: 0x0008410C File Offset: 0x0008230C
	public void ForceLightUpdate()
	{
		TimeCachedValue<float> timeCachedValue = this.sunExposure;
		if (timeCachedValue != null)
		{
			timeCachedValue.ForceNextRun();
		}
		TimeCachedValue<float> timeCachedValue2 = this.artificialLightExposure;
		if (timeCachedValue2 == null)
		{
			return;
		}
		timeCachedValue2.ForceNextRun();
	}

	// Token: 0x06000FCF RID: 4047 RVA: 0x0008412F File Offset: 0x0008232F
	public void ForceTemperatureUpdate()
	{
		TimeCachedValue<float> timeCachedValue = this.plantArtificalTemperature;
		if (timeCachedValue == null)
		{
			return;
		}
		timeCachedValue.ForceNextRun();
	}

	// Token: 0x06000FD0 RID: 4048 RVA: 0x00084141 File Offset: 0x00082341
	public float GetSunExposure()
	{
		TimeCachedValue<float> timeCachedValue = this.sunExposure;
		if (timeCachedValue == null)
		{
			return 0f;
		}
		return timeCachedValue.Get(false);
	}

	// Token: 0x06000FD1 RID: 4049 RVA: 0x000673D3 File Offset: 0x000655D3
	private float CalculateSunExposure()
	{
		return global::GrowableEntity.SunRaycast(base.transform.position + new Vector3(0f, 1f, 0f));
	}

	// Token: 0x06000FD2 RID: 4050 RVA: 0x00084159 File Offset: 0x00082359
	public float GetArtificialLightExposure()
	{
		TimeCachedValue<float> timeCachedValue = this.artificialLightExposure;
		if (timeCachedValue == null)
		{
			return 0f;
		}
		return timeCachedValue.Get(false);
	}

	// Token: 0x06000FD3 RID: 4051 RVA: 0x00067430 File Offset: 0x00065630
	private float CalculateArtificialLightExposure()
	{
		return global::GrowableEntity.CalculateArtificialLightExposure(base.transform);
	}

	// Token: 0x06000FD4 RID: 4052 RVA: 0x00084171 File Offset: 0x00082371
	public float GetPlantTemperature()
	{
		TimeCachedValue<float> timeCachedValue = this.plantTemperature;
		float num = (timeCachedValue != null) ? timeCachedValue.Get(false) : 0f;
		TimeCachedValue<float> timeCachedValue2 = this.plantArtificalTemperature;
		return num + ((timeCachedValue2 != null) ? timeCachedValue2.Get(false) : 0f);
	}

	// Token: 0x06000FD5 RID: 4053 RVA: 0x000841A2 File Offset: 0x000823A2
	private float CalculatePlantTemperature()
	{
		return Mathf.Max(Climate.GetTemperature(base.transform.position), 15f);
	}

	// Token: 0x06000FD6 RID: 4054 RVA: 0x000841C0 File Offset: 0x000823C0
	private void CalculateRainFactor()
	{
		if (this.sunExposure.Get(false) > 0f)
		{
			float rain = Climate.GetRain(base.transform.position);
			if (rain > 0f)
			{
				this.soilSaturation = Mathf.Clamp(this.soilSaturation + Mathf.RoundToInt(4f * rain * this.lastRainCheck), 0, this.soilSaturationMax);
				this.RefreshGrowables(null);
				base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
			}
		}
		this.lastRainCheck = 0f;
	}

	// Token: 0x06000FD7 RID: 4055 RVA: 0x00067913 File Offset: 0x00065B13
	private float CalculateArtificialTemperature()
	{
		return global::GrowableEntity.CalculateArtificialTemperature(base.transform);
	}

	// Token: 0x06000FD8 RID: 4056 RVA: 0x00084248 File Offset: 0x00082448
	public void OnPlantInserted(global::GrowableEntity entity, global::BasePlayer byPlayer)
	{
		if (!GameInfo.HasAchievements)
		{
			return;
		}
		List<uint> list = Facepunch.Pool.GetList<uint>();
		using (List<global::BaseEntity>.Enumerator enumerator = this.children.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				global::GrowableEntity growableEntity;
				if ((growableEntity = (enumerator.Current as global::GrowableEntity)) != null && !list.Contains(growableEntity.prefabID))
				{
					list.Add(growableEntity.prefabID);
				}
			}
		}
		if (list.Count == 9)
		{
			byPlayer.GiveAchievement("HONEST_WORK");
		}
		Facepunch.Pool.FreeList<uint>(ref list);
	}

	// Token: 0x06000FD9 RID: 4057 RVA: 0x000842E0 File Offset: 0x000824E0
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.MaxDistance(3f)]
	private void RPC_RequestSaturationUpdate(global::BaseEntity.RPCMessage msg)
	{
		if (msg.player != null)
		{
			base.ClientRPCPlayer<int>(null, msg.player, "RPC_ReceiveSaturationUpdate", this.soilSaturation);
		}
	}

	// Token: 0x06000FDA RID: 4058 RVA: 0x0000441C File Offset: 0x0000261C
	public override bool SupportsChildDeployables()
	{
		return true;
	}
}
