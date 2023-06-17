using System;
using System.Collections.Generic;
using System.Linq;
using ConVar;
using Facepunch;
using Facepunch.Rust;
using Network;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x02000051 RID: 81
public class BuildingPrivlidge : StorageContainer
{
	// Token: 0x04000613 RID: 1555
	private float cachedProtectedMinutes;

	// Token: 0x04000614 RID: 1556
	private float nextProtectedCalcTime;

	// Token: 0x04000615 RID: 1557
	private static BuildingPrivlidge.UpkeepBracket[] upkeepBrackets = new BuildingPrivlidge.UpkeepBracket[]
	{
		new BuildingPrivlidge.UpkeepBracket(ConVar.Decay.bracket_0_blockcount, ConVar.Decay.bracket_0_costfraction),
		new BuildingPrivlidge.UpkeepBracket(ConVar.Decay.bracket_1_blockcount, ConVar.Decay.bracket_1_costfraction),
		new BuildingPrivlidge.UpkeepBracket(ConVar.Decay.bracket_2_blockcount, ConVar.Decay.bracket_2_costfraction),
		new BuildingPrivlidge.UpkeepBracket(ConVar.Decay.bracket_3_blockcount, ConVar.Decay.bracket_3_costfraction)
	};

	// Token: 0x04000616 RID: 1558
	private List<ItemAmount> upkeepBuffer = new List<ItemAmount>();

	// Token: 0x04000617 RID: 1559
	public List<PlayerNameID> authorizedPlayers = new List<PlayerNameID>();

	// Token: 0x04000618 RID: 1560
	public const global::BaseEntity.Flags Flag_MaxAuths = global::BaseEntity.Flags.Reserved5;

	// Token: 0x04000619 RID: 1561
	public List<ItemDefinition> allowedConstructionItems = new List<ItemDefinition>();

	// Token: 0x060008FA RID: 2298 RVA: 0x00056480 File Offset: 0x00054680
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("BuildingPrivlidge.OnRpcMessage", 0))
		{
			if (rpc == 1092560690U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - AddSelfAuthorize ");
				}
				using (TimeWarning.New("AddSelfAuthorize", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.IsVisible.Test(1092560690U, "AddSelfAuthorize", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage rpc2 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.AddSelfAuthorize(rpc2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in AddSelfAuthorize");
					}
				}
				return true;
			}
			if (rpc == 253307592U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - ClearList ");
				}
				using (TimeWarning.New("ClearList", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.IsVisible.Test(253307592U, "ClearList", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage rpc3 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.ClearList(rpc3);
						}
					}
					catch (Exception exception2)
					{
						Debug.LogException(exception2);
						player.Kick("RPC Error in ClearList");
					}
				}
				return true;
			}
			if (rpc == 3617985969U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RemoveSelfAuthorize ");
				}
				using (TimeWarning.New("RemoveSelfAuthorize", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.IsVisible.Test(3617985969U, "RemoveSelfAuthorize", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage rpc4 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.RemoveSelfAuthorize(rpc4);
						}
					}
					catch (Exception exception3)
					{
						Debug.LogException(exception3);
						player.Kick("RPC Error in RemoveSelfAuthorize");
					}
				}
				return true;
			}
			if (rpc == 2051750736U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_Rotate ");
				}
				using (TimeWarning.New("RPC_Rotate", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.IsVisible.Test(2051750736U, "RPC_Rotate", this, player, 3f))
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
							this.RPC_Rotate(msg2);
						}
					}
					catch (Exception exception4)
					{
						Debug.LogException(exception4);
						player.Kick("RPC Error in RPC_Rotate");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x060008FB RID: 2299 RVA: 0x00056A38 File Offset: 0x00054C38
	public float CalculateUpkeepPeriodMinutes()
	{
		if (base.isServer)
		{
			return ConVar.Decay.upkeep_period_minutes;
		}
		return 0f;
	}

	// Token: 0x060008FC RID: 2300 RVA: 0x00056A4D File Offset: 0x00054C4D
	public float CalculateUpkeepCostFraction()
	{
		if (base.isServer)
		{
			return this.CalculateBuildingTaxRate();
		}
		return 0f;
	}

	// Token: 0x060008FD RID: 2301 RVA: 0x00056A64 File Offset: 0x00054C64
	public void CalculateUpkeepCostAmounts(List<ItemAmount> itemAmounts)
	{
		BuildingManager.Building building = base.GetBuilding();
		if (building == null)
		{
			return;
		}
		if (!building.HasDecayEntities())
		{
			return;
		}
		float multiplier = this.CalculateUpkeepCostFraction();
		foreach (global::DecayEntity decayEntity in building.decayEntities)
		{
			decayEntity.CalculateUpkeepCostAmounts(itemAmounts, multiplier);
		}
	}

	// Token: 0x060008FE RID: 2302 RVA: 0x00056AD4 File Offset: 0x00054CD4
	public float GetProtectedMinutes(bool force = false)
	{
		if (!base.isServer)
		{
			return 0f;
		}
		if (!force && UnityEngine.Time.realtimeSinceStartup < this.nextProtectedCalcTime)
		{
			return this.cachedProtectedMinutes;
		}
		this.nextProtectedCalcTime = UnityEngine.Time.realtimeSinceStartup + 60f;
		List<ItemAmount> list = Facepunch.Pool.GetList<ItemAmount>();
		this.CalculateUpkeepCostAmounts(list);
		float num = this.CalculateUpkeepPeriodMinutes();
		float num2 = -1f;
		if (base.inventory != null)
		{
			foreach (ItemAmount itemAmount in list)
			{
				int num3 = base.inventory.FindItemsByItemID(itemAmount.itemid).Sum((global::Item x) => x.amount);
				if (num3 > 0 && itemAmount.amount > 0f)
				{
					float num4 = (float)num3 / itemAmount.amount * num;
					if (num2 == -1f || num4 < num2)
					{
						num2 = num4;
					}
				}
				else
				{
					num2 = 0f;
				}
			}
			if (num2 == -1f)
			{
				num2 = 0f;
			}
		}
		Facepunch.Pool.FreeList<ItemAmount>(ref list);
		this.cachedProtectedMinutes = num2;
		return this.cachedProtectedMinutes;
	}

	// Token: 0x060008FF RID: 2303 RVA: 0x00056C14 File Offset: 0x00054E14
	public override void OnKilled(HitInfo info)
	{
		if (ConVar.Decay.upkeep_grief_protection > 0f)
		{
			this.PurchaseUpkeepTime(ConVar.Decay.upkeep_grief_protection * 60f);
		}
		base.OnKilled(info);
	}

	// Token: 0x06000900 RID: 2304 RVA: 0x00056C3A File Offset: 0x00054E3A
	public override void DecayTick()
	{
		if (this.EnsurePrimary())
		{
			base.DecayTick();
		}
	}

	// Token: 0x06000901 RID: 2305 RVA: 0x00056C4C File Offset: 0x00054E4C
	private bool EnsurePrimary()
	{
		BuildingManager.Building building = base.GetBuilding();
		if (building != null)
		{
			BuildingPrivlidge dominatingBuildingPrivilege = building.GetDominatingBuildingPrivilege();
			if (dominatingBuildingPrivilege != null && dominatingBuildingPrivilege != this)
			{
				base.Kill(global::BaseNetworkable.DestroyMode.Gib);
				return false;
			}
		}
		return true;
	}

	// Token: 0x06000902 RID: 2306 RVA: 0x00056C86 File Offset: 0x00054E86
	public void MarkProtectedMinutesDirty(float delay = 0f)
	{
		this.nextProtectedCalcTime = UnityEngine.Time.realtimeSinceStartup + delay;
	}

	// Token: 0x06000903 RID: 2307 RVA: 0x00056C98 File Offset: 0x00054E98
	private float CalculateBuildingTaxRate()
	{
		BuildingManager.Building building = base.GetBuilding();
		if (building == null)
		{
			return ConVar.Decay.bracket_0_costfraction;
		}
		if (!building.HasBuildingBlocks())
		{
			return ConVar.Decay.bracket_0_costfraction;
		}
		int count = building.buildingBlocks.Count;
		int num = count;
		for (int i = 0; i < BuildingPrivlidge.upkeepBrackets.Length; i++)
		{
			BuildingPrivlidge.UpkeepBracket upkeepBracket = BuildingPrivlidge.upkeepBrackets[i];
			upkeepBracket.blocksTaxPaid = 0f;
			if (num > 0)
			{
				int num2;
				if (i == BuildingPrivlidge.upkeepBrackets.Length - 1)
				{
					num2 = num;
				}
				else
				{
					num2 = Mathf.Min(num, BuildingPrivlidge.upkeepBrackets[i].objectsUpTo);
				}
				num -= num2;
				upkeepBracket.blocksTaxPaid = (float)num2 * upkeepBracket.fraction;
			}
		}
		float num3 = 0f;
		for (int j = 0; j < BuildingPrivlidge.upkeepBrackets.Length; j++)
		{
			BuildingPrivlidge.UpkeepBracket upkeepBracket2 = BuildingPrivlidge.upkeepBrackets[j];
			if (upkeepBracket2.blocksTaxPaid <= 0f)
			{
				break;
			}
			num3 += upkeepBracket2.blocksTaxPaid;
		}
		return num3 / (float)count;
	}

	// Token: 0x06000904 RID: 2308 RVA: 0x00056D88 File Offset: 0x00054F88
	private void ApplyUpkeepPayment()
	{
		List<global::Item> list = Facepunch.Pool.GetList<global::Item>();
		for (int i = 0; i < this.upkeepBuffer.Count; i++)
		{
			ItemAmount itemAmount = this.upkeepBuffer[i];
			int num = (int)itemAmount.amount;
			if (num >= 1)
			{
				base.inventory.Take(list, itemAmount.itemid, num);
				Analytics.Azure.AddPendingItems(this, itemAmount.itemDef.shortname, num, "upkeep", true, true);
				foreach (global::Item item in list)
				{
					if (this.IsDebugging())
					{
						Debug.Log(string.Concat(new object[]
						{
							this.ToString(),
							": Using ",
							item.amount,
							" of ",
							item.info.shortname
						}));
					}
					item.UseItem(item.amount);
				}
				list.Clear();
				itemAmount.amount -= (float)num;
				this.upkeepBuffer[i] = itemAmount;
			}
		}
		Facepunch.Pool.FreeList<global::Item>(ref list);
	}

	// Token: 0x06000905 RID: 2309 RVA: 0x00056EC4 File Offset: 0x000550C4
	private void QueueUpkeepPayment(List<ItemAmount> itemAmounts)
	{
		for (int i = 0; i < itemAmounts.Count; i++)
		{
			ItemAmount itemAmount = itemAmounts[i];
			bool flag = false;
			foreach (ItemAmount itemAmount2 in this.upkeepBuffer)
			{
				if (itemAmount2.itemDef == itemAmount.itemDef)
				{
					itemAmount2.amount += itemAmount.amount;
					if (this.IsDebugging())
					{
						Debug.Log(string.Concat(new object[]
						{
							this.ToString(),
							": Adding ",
							itemAmount.amount,
							" of ",
							itemAmount.itemDef.shortname,
							" to ",
							itemAmount2.amount
						}));
					}
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				if (this.IsDebugging())
				{
					Debug.Log(string.Concat(new object[]
					{
						this.ToString(),
						": Adding ",
						itemAmount.amount,
						" of ",
						itemAmount.itemDef.shortname
					}));
				}
				this.upkeepBuffer.Add(new ItemAmount(itemAmount.itemDef, itemAmount.amount));
			}
		}
	}

	// Token: 0x06000906 RID: 2310 RVA: 0x00057034 File Offset: 0x00055234
	private bool CanAffordUpkeepPayment(List<ItemAmount> itemAmounts)
	{
		for (int i = 0; i < itemAmounts.Count; i++)
		{
			ItemAmount itemAmount = itemAmounts[i];
			if ((float)base.inventory.GetAmount(itemAmount.itemid, true) < itemAmount.amount)
			{
				if (this.IsDebugging())
				{
					Debug.Log(string.Concat(new object[]
					{
						this.ToString(),
						": Can't afford ",
						itemAmount.amount,
						" of ",
						itemAmount.itemDef.shortname
					}));
				}
				return false;
			}
		}
		return true;
	}

	// Token: 0x06000907 RID: 2311 RVA: 0x000570C8 File Offset: 0x000552C8
	public float PurchaseUpkeepTime(global::DecayEntity entity, float deltaTime)
	{
		float num = this.CalculateUpkeepCostFraction();
		float num2 = this.CalculateUpkeepPeriodMinutes() * 60f;
		float multiplier = num * deltaTime / num2;
		List<ItemAmount> list = Facepunch.Pool.GetList<ItemAmount>();
		entity.CalculateUpkeepCostAmounts(list, multiplier);
		bool flag = this.CanAffordUpkeepPayment(list);
		this.QueueUpkeepPayment(list);
		Facepunch.Pool.FreeList<ItemAmount>(ref list);
		this.ApplyUpkeepPayment();
		if (!flag)
		{
			return 0f;
		}
		return deltaTime;
	}

	// Token: 0x06000908 RID: 2312 RVA: 0x00057120 File Offset: 0x00055320
	public void PurchaseUpkeepTime(float deltaTime)
	{
		BuildingManager.Building building = base.GetBuilding();
		if (building != null && building.HasDecayEntities())
		{
			float num = Mathf.Min(this.GetProtectedMinutes(true) * 60f, deltaTime);
			if (num > 0f)
			{
				foreach (global::DecayEntity decayEntity in building.decayEntities)
				{
					float protectedSeconds = decayEntity.GetProtectedSeconds();
					if (num > protectedSeconds)
					{
						float num2 = this.PurchaseUpkeepTime(decayEntity, num - protectedSeconds);
						decayEntity.AddUpkeepTime(num2);
						if (this.IsDebugging())
						{
							Debug.Log(string.Concat(new object[]
							{
								this.ToString(),
								" purchased upkeep time for ",
								decayEntity.ToString(),
								": ",
								protectedSeconds,
								" + ",
								num2,
								" = ",
								decayEntity.GetProtectedSeconds()
							}));
						}
					}
				}
			}
		}
	}

	// Token: 0x06000909 RID: 2313 RVA: 0x0005723C File Offset: 0x0005543C
	public override void ResetState()
	{
		base.ResetState();
		this.authorizedPlayers.Clear();
	}

	// Token: 0x0600090A RID: 2314 RVA: 0x00057250 File Offset: 0x00055450
	public bool IsAuthed(global::BasePlayer player)
	{
		return this.authorizedPlayers.Any((PlayerNameID x) => x.userid == player.userID);
	}

	// Token: 0x0600090B RID: 2315 RVA: 0x00057284 File Offset: 0x00055484
	public bool IsAuthed(ulong userID)
	{
		return this.authorizedPlayers.Any((PlayerNameID x) => x.userid == userID);
	}

	// Token: 0x0600090C RID: 2316 RVA: 0x000572B5 File Offset: 0x000554B5
	public bool AnyAuthed()
	{
		return this.authorizedPlayers.Count > 0;
	}

	// Token: 0x0600090D RID: 2317 RVA: 0x000572C8 File Offset: 0x000554C8
	public override bool ItemFilter(global::Item item, int targetSlot)
	{
		bool flag = this.allowedConstructionItems.Contains(item.info);
		if (!flag && targetSlot == -1)
		{
			int num = 0;
			foreach (global::Item item2 in base.inventory.itemList)
			{
				if (!this.allowedConstructionItems.Contains(item2.info) && (item2.info != item.info || item2.amount == item2.MaxStackable()))
				{
					num++;
				}
			}
			if (num >= 24)
			{
				return false;
			}
		}
		if (targetSlot >= 24 && targetSlot <= 27)
		{
			return flag;
		}
		return base.ItemFilter(item, targetSlot);
	}

	// Token: 0x0600090E RID: 2318 RVA: 0x00057388 File Offset: 0x00055588
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.buildingPrivilege = Facepunch.Pool.Get<BuildingPrivilege>();
		info.msg.buildingPrivilege.users = this.authorizedPlayers;
		if (!info.forDisk)
		{
			info.msg.buildingPrivilege.upkeepPeriodMinutes = this.CalculateUpkeepPeriodMinutes();
			info.msg.buildingPrivilege.costFraction = this.CalculateUpkeepCostFraction();
			info.msg.buildingPrivilege.protectedMinutes = this.GetProtectedMinutes(false);
		}
	}

	// Token: 0x0600090F RID: 2319 RVA: 0x0005740D File Offset: 0x0005560D
	public override void PostSave(global::BaseNetworkable.SaveInfo info)
	{
		info.msg.buildingPrivilege.users = null;
	}

	// Token: 0x06000910 RID: 2320 RVA: 0x00057420 File Offset: 0x00055620
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		this.authorizedPlayers.Clear();
		if (info.msg.buildingPrivilege != null && info.msg.buildingPrivilege.users != null)
		{
			this.authorizedPlayers = info.msg.buildingPrivilege.users;
			if (!info.fromDisk)
			{
				this.cachedProtectedMinutes = info.msg.buildingPrivilege.protectedMinutes;
			}
			info.msg.buildingPrivilege.users = null;
		}
	}

	// Token: 0x06000911 RID: 2321 RVA: 0x000574A3 File Offset: 0x000556A3
	public void BuildingDirty()
	{
		if (base.isServer)
		{
			this.AddDelayedUpdate();
		}
	}

	// Token: 0x06000912 RID: 2322 RVA: 0x00003FA8 File Offset: 0x000021A8
	public bool AtMaxAuthCapacity()
	{
		return base.HasFlag(global::BaseEntity.Flags.Reserved5);
	}

	// Token: 0x06000913 RID: 2323 RVA: 0x000574B4 File Offset: 0x000556B4
	public void UpdateMaxAuthCapacity()
	{
		BaseGameMode activeGameMode = BaseGameMode.GetActiveGameMode(true);
		if (activeGameMode && activeGameMode.limitTeamAuths)
		{
			base.SetFlag(global::BaseEntity.Flags.Reserved5, this.authorizedPlayers.Count >= activeGameMode.GetMaxRelationshipTeamSize(), false, true);
		}
	}

	// Token: 0x06000914 RID: 2324 RVA: 0x000574FB File Offset: 0x000556FB
	protected override void OnInventoryDirty()
	{
		base.OnInventoryDirty();
		this.AddDelayedUpdate();
	}

	// Token: 0x06000915 RID: 2325 RVA: 0x00057509 File Offset: 0x00055709
	public override void OnItemAddedOrRemoved(global::Item item, bool bAdded)
	{
		base.OnItemAddedOrRemoved(item, bAdded);
		this.AddDelayedUpdate();
	}

	// Token: 0x06000916 RID: 2326 RVA: 0x00057519 File Offset: 0x00055719
	public void AddDelayedUpdate()
	{
		if (base.IsInvoking(new Action(this.DelayedUpdate)))
		{
			base.CancelInvoke(new Action(this.DelayedUpdate));
		}
		base.Invoke(new Action(this.DelayedUpdate), 1f);
	}

	// Token: 0x06000917 RID: 2327 RVA: 0x00057558 File Offset: 0x00055758
	public void DelayedUpdate()
	{
		this.MarkProtectedMinutesDirty(0f);
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x06000918 RID: 2328 RVA: 0x0005756C File Offset: 0x0005576C
	public bool CanAdministrate(global::BasePlayer player)
	{
		BaseLock baseLock = base.GetSlot(global::BaseEntity.Slot.Lock) as BaseLock;
		return baseLock == null || baseLock.OnTryToOpen(player);
	}

	// Token: 0x06000919 RID: 2329 RVA: 0x00057598 File Offset: 0x00055798
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsVisible(3f)]
	private void AddSelfAuthorize(global::BaseEntity.RPCMessage rpc)
	{
		if (!rpc.player.CanInteract())
		{
			return;
		}
		if (!this.CanAdministrate(rpc.player))
		{
			return;
		}
		this.AddPlayer(rpc.player);
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x0600091A RID: 2330 RVA: 0x000575CC File Offset: 0x000557CC
	public void AddPlayer(global::BasePlayer player)
	{
		if (this.AtMaxAuthCapacity())
		{
			return;
		}
		this.authorizedPlayers.RemoveAll((PlayerNameID x) => x.userid == player.userID);
		PlayerNameID playerNameID = new PlayerNameID();
		playerNameID.userid = player.userID;
		playerNameID.username = player.displayName;
		this.authorizedPlayers.Add(playerNameID);
		Analytics.Azure.OnEntityAuthChanged(this, player, from x in this.authorizedPlayers
		select x.userid, "added", player.userID);
		this.UpdateMaxAuthCapacity();
	}

	// Token: 0x0600091B RID: 2331 RVA: 0x00057688 File Offset: 0x00055888
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsVisible(3f)]
	private void RemoveSelfAuthorize(global::BaseEntity.RPCMessage rpc)
	{
		if (!rpc.player.CanInteract())
		{
			return;
		}
		if (!this.CanAdministrate(rpc.player))
		{
			return;
		}
		this.authorizedPlayers.RemoveAll((PlayerNameID x) => x.userid == rpc.player.userID);
		Analytics.Azure.OnEntityAuthChanged(this, rpc.player, from x in this.authorizedPlayers
		select x.userid, "removed", rpc.player.userID);
		this.UpdateMaxAuthCapacity();
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x0600091C RID: 2332 RVA: 0x0005773E File Offset: 0x0005593E
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsVisible(3f)]
	private void ClearList(global::BaseEntity.RPCMessage rpc)
	{
		if (!rpc.player.CanInteract())
		{
			return;
		}
		if (!this.CanAdministrate(rpc.player))
		{
			return;
		}
		this.authorizedPlayers.Clear();
		this.UpdateMaxAuthCapacity();
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x0600091D RID: 2333 RVA: 0x00057778 File Offset: 0x00055978
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsVisible(3f)]
	public void RPC_Rotate(global::BaseEntity.RPCMessage msg)
	{
		global::BasePlayer player = msg.player;
		if (player.CanBuild() && player.GetHeldEntity() && player.GetHeldEntity().GetComponent<Hammer>() != null && (base.GetSlot(global::BaseEntity.Slot.Lock) == null || !base.GetSlot(global::BaseEntity.Slot.Lock).IsLocked()) && !base.HasAttachedStorageAdaptor())
		{
			base.transform.rotation = Quaternion.LookRotation(-base.transform.forward, base.transform.up);
			base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
			Deployable component = base.GetComponent<Deployable>();
			if (component != null && component.placeEffect.isValid)
			{
				Effect.server.Run(component.placeEffect.resourcePath, base.transform.position, Vector3.up, null, false);
			}
		}
		global::BaseEntity slot = base.GetSlot(global::BaseEntity.Slot.Lock);
		if (slot != null)
		{
			slot.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
		}
	}

	// Token: 0x0600091E RID: 2334 RVA: 0x0005786C File Offset: 0x00055A6C
	public override int GetIdealSlot(global::BasePlayer player, global::Item item)
	{
		if (item != null && item.info != null && this.allowedConstructionItems.Contains(item.info))
		{
			for (int i = 24; i <= 27; i++)
			{
				if (base.inventory.GetSlot(i) == null)
				{
					return i;
				}
			}
		}
		return base.GetIdealSlot(player, item);
	}

	// Token: 0x0600091F RID: 2335 RVA: 0x000578C3 File Offset: 0x00055AC3
	public override bool HasSlot(global::BaseEntity.Slot slot)
	{
		return slot == global::BaseEntity.Slot.Lock || base.HasSlot(slot);
	}

	// Token: 0x06000920 RID: 2336 RVA: 0x0000441C File Offset: 0x0000261C
	public override bool SupportsChildDeployables()
	{
		return true;
	}

	// Token: 0x02000BC3 RID: 3011
	public class UpkeepBracket
	{
		// Token: 0x040040CD RID: 16589
		public int objectsUpTo;

		// Token: 0x040040CE RID: 16590
		public float fraction;

		// Token: 0x040040CF RID: 16591
		public float blocksTaxPaid;

		// Token: 0x06004D7F RID: 19839 RVA: 0x001A0EDA File Offset: 0x0019F0DA
		public UpkeepBracket(int numObjs, float frac)
		{
			this.objectsUpTo = numObjs;
			this.fraction = frac;
			this.blocksTaxPaid = 0f;
		}
	}
}
