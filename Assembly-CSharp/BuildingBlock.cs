using System;
using System.Collections.Generic;
using ConVar;
using Facepunch;
using Facepunch.Rust;
using Network;
using ProtoBuf;
using Rust;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x02000050 RID: 80
public class BuildingBlock : global::StabilityEntity
{
	// Token: 0x04000601 RID: 1537
	private bool forceSkinRefresh;

	// Token: 0x04000602 RID: 1538
	private ulong lastSkinID;

	// Token: 0x04000603 RID: 1539
	private int modelState;

	// Token: 0x04000604 RID: 1540
	private int lastModelState;

	// Token: 0x04000606 RID: 1542
	private uint lastCustomColour;

	// Token: 0x04000607 RID: 1543
	private uint playerCustomColourToApply;

	// Token: 0x04000608 RID: 1544
	public BuildingGrade.Enum grade;

	// Token: 0x04000609 RID: 1545
	private BuildingGrade.Enum lastGrade = BuildingGrade.Enum.None;

	// Token: 0x0400060A RID: 1546
	private ConstructionSkin currentSkin;

	// Token: 0x0400060B RID: 1547
	private DeferredAction skinChange;

	// Token: 0x0400060C RID: 1548
	private MeshRenderer placeholderRenderer;

	// Token: 0x0400060D RID: 1549
	private MeshCollider placeholderCollider;

	// Token: 0x0400060E RID: 1550
	public static global::BuildingBlock.UpdateSkinWorkQueue updateSkinQueueServer = new global::BuildingBlock.UpdateSkinWorkQueue();

	// Token: 0x0400060F RID: 1551
	public bool CullBushes;

	// Token: 0x04000610 RID: 1552
	public bool CheckForPipesOnModelChange;

	// Token: 0x04000611 RID: 1553
	[NonSerialized]
	public Construction blockDefinition;

	// Token: 0x04000612 RID: 1554
	private static Vector3[] outsideLookupOffsets = new Vector3[]
	{
		new Vector3(0f, 1f, 0f).normalized,
		new Vector3(1f, 1f, 0f).normalized,
		new Vector3(-1f, 1f, 0f).normalized,
		new Vector3(0f, 1f, 1f).normalized,
		new Vector3(0f, 1f, -1f).normalized
	};

	// Token: 0x060008BA RID: 2234 RVA: 0x00054B28 File Offset: 0x00052D28
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("BuildingBlock.OnRpcMessage", 0))
		{
			if (rpc == 2858062413U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - DoDemolish ");
				}
				using (TimeWarning.New("DoDemolish", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.MaxDistance.Test(2858062413U, "DoDemolish", this, player, 3f))
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
							this.DoDemolish(msg2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in DoDemolish");
					}
				}
				return true;
			}
			if (rpc == 216608990U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - DoImmediateDemolish ");
				}
				using (TimeWarning.New("DoImmediateDemolish", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.MaxDistance.Test(216608990U, "DoImmediateDemolish", this, player, 3f))
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
							this.DoImmediateDemolish(msg3);
						}
					}
					catch (Exception exception2)
					{
						Debug.LogException(exception2);
						player.Kick("RPC Error in DoImmediateDemolish");
					}
				}
				return true;
			}
			if (rpc == 1956645865U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - DoRotation ");
				}
				using (TimeWarning.New("DoRotation", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.MaxDistance.Test(1956645865U, "DoRotation", this, player, 3f))
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
							this.DoRotation(msg4);
						}
					}
					catch (Exception exception3)
					{
						Debug.LogException(exception3);
						player.Kick("RPC Error in DoRotation");
					}
				}
				return true;
			}
			if (rpc == 3746288057U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - DoUpgradeToGrade ");
				}
				using (TimeWarning.New("DoUpgradeToGrade", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.MaxDistance.Test(3746288057U, "DoUpgradeToGrade", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage msg5 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.DoUpgradeToGrade(msg5);
						}
					}
					catch (Exception exception4)
					{
						Debug.LogException(exception4);
						player.Kick("RPC Error in DoUpgradeToGrade");
					}
				}
				return true;
			}
			if (rpc == 4081052216U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - DoUpgradeToGrade_Delayed ");
				}
				using (TimeWarning.New("DoUpgradeToGrade_Delayed", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.MaxDistance.Test(4081052216U, "DoUpgradeToGrade_Delayed", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage msg6 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.DoUpgradeToGrade_Delayed(msg6);
						}
					}
					catch (Exception exception5)
					{
						Debug.LogException(exception5);
						player.Kick("RPC Error in DoUpgradeToGrade_Delayed");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x060008BB RID: 2235 RVA: 0x00055238 File Offset: 0x00053438
	private bool CanDemolish(global::BasePlayer player)
	{
		return this.IsDemolishable() && this.HasDemolishPrivilege(player);
	}

	// Token: 0x060008BC RID: 2236 RVA: 0x0005524B File Offset: 0x0005344B
	private bool IsDemolishable()
	{
		return ConVar.Server.pve || base.HasFlag(global::BaseEntity.Flags.Reserved2);
	}

	// Token: 0x060008BD RID: 2237 RVA: 0x00055264 File Offset: 0x00053464
	private bool HasDemolishPrivilege(global::BasePlayer player)
	{
		return player.IsBuildingAuthed(base.transform.position, base.transform.rotation, this.bounds);
	}

	// Token: 0x060008BE RID: 2238 RVA: 0x00055288 File Offset: 0x00053488
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.MaxDistance(3f)]
	private void DoDemolish(global::BaseEntity.RPCMessage msg)
	{
		if (!msg.player.CanInteract())
		{
			return;
		}
		if (!this.CanDemolish(msg.player))
		{
			return;
		}
		Analytics.Azure.OnBuildingBlockDemolished(msg.player, this);
		base.Kill(global::BaseNetworkable.DestroyMode.Gib);
	}

	// Token: 0x060008BF RID: 2239 RVA: 0x000552BA File Offset: 0x000534BA
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.MaxDistance(3f)]
	private void DoImmediateDemolish(global::BaseEntity.RPCMessage msg)
	{
		if (!msg.player.CanInteract())
		{
			return;
		}
		if (!msg.player.IsAdmin)
		{
			return;
		}
		Analytics.Azure.OnBuildingBlockDemolished(msg.player, this);
		base.Kill(global::BaseNetworkable.DestroyMode.Gib);
	}

	// Token: 0x060008C0 RID: 2240 RVA: 0x000552EB File Offset: 0x000534EB
	private void StopBeingDemolishable()
	{
		base.SetFlag(global::BaseEntity.Flags.Reserved2, false, false, true);
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x060008C1 RID: 2241 RVA: 0x00055302 File Offset: 0x00053502
	private void StartBeingDemolishable()
	{
		base.SetFlag(global::BaseEntity.Flags.Reserved2, true, false, true);
		base.Invoke(new Action(this.StopBeingDemolishable), 600f);
	}

	// Token: 0x170000EC RID: 236
	// (get) Token: 0x060008C2 RID: 2242 RVA: 0x00055329 File Offset: 0x00053529
	// (set) Token: 0x060008C3 RID: 2243 RVA: 0x00055331 File Offset: 0x00053531
	public uint customColour { get; private set; }

	// Token: 0x060008C4 RID: 2244 RVA: 0x0005533A File Offset: 0x0005353A
	public void SetConditionalModel(int state)
	{
		this.modelState = state;
	}

	// Token: 0x060008C5 RID: 2245 RVA: 0x00055343 File Offset: 0x00053543
	public bool GetConditionalModel(int index)
	{
		return (this.modelState & 1 << index) != 0;
	}

	// Token: 0x170000ED RID: 237
	// (get) Token: 0x060008C6 RID: 2246 RVA: 0x00055355 File Offset: 0x00053555
	public ConstructionGrade currentGrade
	{
		get
		{
			return this.blockDefinition.GetGrade(this.grade, this.skinID);
		}
	}

	// Token: 0x060008C7 RID: 2247 RVA: 0x0005536E File Offset: 0x0005356E
	private bool CanChangeToGrade(BuildingGrade.Enum iGrade, ulong iSkin, global::BasePlayer player)
	{
		return this.HasUpgradePrivilege(iGrade, iSkin, player) && !this.IsUpgradeBlocked();
	}

	// Token: 0x060008C8 RID: 2248 RVA: 0x00055388 File Offset: 0x00053588
	private bool HasUpgradePrivilege(BuildingGrade.Enum iGrade, ulong iSkin, global::BasePlayer player)
	{
		return iGrade >= this.grade && (iGrade != this.grade || iSkin != this.skinID) && iGrade > BuildingGrade.Enum.None && iGrade < BuildingGrade.Enum.Count && !player.IsBuildingBlocked(base.transform.position, base.transform.rotation, this.bounds);
	}

	// Token: 0x060008C9 RID: 2249 RVA: 0x000553E8 File Offset: 0x000535E8
	private bool IsUpgradeBlocked()
	{
		if (!this.blockDefinition.checkVolumeOnUpgrade)
		{
			return false;
		}
		DeployVolume[] volumes = PrefabAttribute.server.FindAll<DeployVolume>(this.prefabID);
		return DeployVolume.Check(base.transform.position, base.transform.rotation, volumes, ~(1 << base.gameObject.layer));
	}

	// Token: 0x060008CA RID: 2250 RVA: 0x00055444 File Offset: 0x00053644
	private bool CanAffordUpgrade(BuildingGrade.Enum iGrade, ulong iSkin, global::BasePlayer player)
	{
		foreach (ItemAmount itemAmount in this.blockDefinition.GetGrade(iGrade, iSkin).CostToBuild(this.grade))
		{
			if ((float)player.inventory.GetAmount(itemAmount.itemid) < itemAmount.amount)
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x060008CB RID: 2251 RVA: 0x000554C4 File Offset: 0x000536C4
	public void SetGrade(BuildingGrade.Enum iGrade)
	{
		if (this.blockDefinition.grades == null || iGrade <= BuildingGrade.Enum.None || iGrade >= BuildingGrade.Enum.Count)
		{
			Debug.LogError("Tried to set to undefined grade! " + this.blockDefinition.fullName, base.gameObject);
			return;
		}
		this.grade = iGrade;
		this.grade = this.currentGrade.gradeBase.type;
		this.UpdateGrade();
	}

	// Token: 0x060008CC RID: 2252 RVA: 0x0005552A File Offset: 0x0005372A
	private void UpdateGrade()
	{
		this.baseProtection = this.currentGrade.gradeBase.damageProtecton;
	}

	// Token: 0x060008CD RID: 2253 RVA: 0x0002C845 File Offset: 0x0002AA45
	protected override void OnSkinChanged(ulong oldSkinID, ulong newSkinID)
	{
		if (oldSkinID == newSkinID)
		{
			return;
		}
		this.skinID = newSkinID;
	}

	// Token: 0x060008CE RID: 2254 RVA: 0x000063A5 File Offset: 0x000045A5
	protected override void OnSkinPreProcess(IPrefabProcessor preProcess, GameObject rootObj, string name, bool serverside, bool clientside, bool bundling)
	{
	}

	// Token: 0x060008CF RID: 2255 RVA: 0x00055542 File Offset: 0x00053742
	public void SetHealthToMax()
	{
		base.health = this.MaxHealth();
	}

	// Token: 0x060008D0 RID: 2256 RVA: 0x00055550 File Offset: 0x00053750
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.MaxDistance(3f)]
	private void DoUpgradeToGrade_Delayed(global::BaseEntity.RPCMessage msg)
	{
		if (!msg.player.CanInteract())
		{
			return;
		}
		BuildingGrade.Enum @enum = (BuildingGrade.Enum)msg.read.Int32();
		ulong num = msg.read.UInt64();
		ConstructionGrade constructionGrade = this.blockDefinition.GetGrade(@enum, num);
		if (constructionGrade == null)
		{
			return;
		}
		if (!this.CanChangeToGrade(@enum, num, msg.player))
		{
			return;
		}
		if (!this.CanAffordUpgrade(@enum, num, msg.player))
		{
			return;
		}
		if (base.SecondsSinceAttacked < 30f)
		{
			return;
		}
		if (num != 0UL && !msg.player.blueprints.steamInventory.HasItem((int)num))
		{
			return;
		}
		this.PayForUpgrade(constructionGrade, msg.player);
		Analytics.Azure.OnBuildingBlockUpgraded(msg.player, this, @enum);
		if (msg.player != null)
		{
			this.playerCustomColourToApply = msg.player.LastBlockColourChangeId;
		}
		base.ClientRPC<int, ulong>(null, "DoUpgradeEffect", (int)@enum, num);
		this.OnSkinChanged(this.skinID, num);
		this.ChangeGrade(@enum, true, true);
	}

	// Token: 0x060008D1 RID: 2257 RVA: 0x00055648 File Offset: 0x00053848
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.MaxDistance(3f)]
	private void DoUpgradeToGrade(global::BaseEntity.RPCMessage msg)
	{
		if (!msg.player.CanInteract())
		{
			return;
		}
		BuildingGrade.Enum @enum = (BuildingGrade.Enum)msg.read.Int32();
		ulong num = msg.read.UInt64();
		ConstructionGrade constructionGrade = this.blockDefinition.GetGrade(@enum, num);
		if (constructionGrade == null)
		{
			return;
		}
		if (!this.CanChangeToGrade(@enum, num, msg.player))
		{
			return;
		}
		if (!this.CanAffordUpgrade(@enum, num, msg.player))
		{
			return;
		}
		if (base.SecondsSinceAttacked < 30f)
		{
			return;
		}
		if (num != 0UL && !msg.player.blueprints.steamInventory.HasItem((int)num))
		{
			return;
		}
		this.PayForUpgrade(constructionGrade, msg.player);
		Analytics.Azure.OnBuildingBlockUpgraded(msg.player, this, @enum);
		if (msg.player != null)
		{
			this.playerCustomColourToApply = msg.player.LastBlockColourChangeId;
		}
		base.ClientRPC<int, ulong>(null, "DoUpgradeEffect", (int)@enum, num);
		this.OnSkinChanged(this.skinID, num);
		this.ChangeGrade(@enum, true, true);
	}

	// Token: 0x060008D2 RID: 2258 RVA: 0x0005573D File Offset: 0x0005393D
	public void ChangeGradeAndSkin(BuildingGrade.Enum targetGrade, ulong skin, bool playEffect = false, bool updateSkin = true)
	{
		this.OnSkinChanged(this.skinID, skin);
		this.ChangeGrade(targetGrade, playEffect, updateSkin);
	}

	// Token: 0x060008D3 RID: 2259 RVA: 0x00055758 File Offset: 0x00053958
	public void ChangeGrade(BuildingGrade.Enum targetGrade, bool playEffect = false, bool updateSkin = true)
	{
		this.SetGrade(targetGrade);
		if (this.grade != this.lastGrade)
		{
			this.SetHealthToMax();
			this.StartBeingRotatable();
		}
		if (updateSkin)
		{
			this.UpdateSkin(false);
		}
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
		base.ResetUpkeepTime();
		base.UpdateSurroundingEntities();
		BuildingManager.Building building = BuildingManager.server.GetBuilding(this.buildingID);
		if (building != null)
		{
			building.Dirty();
		}
	}

	// Token: 0x060008D4 RID: 2260 RVA: 0x000557C0 File Offset: 0x000539C0
	private void PayForUpgrade(ConstructionGrade g, global::BasePlayer player)
	{
		List<global::Item> list = new List<global::Item>();
		foreach (ItemAmount itemAmount in g.CostToBuild(this.grade))
		{
			player.inventory.Take(list, itemAmount.itemid, (int)itemAmount.amount);
			ItemDefinition itemDefinition = ItemManager.FindItemDefinition(itemAmount.itemid);
			Analytics.Azure.LogResource(Analytics.Azure.ResourceMode.Consumed, "upgrade_block", itemDefinition.shortname, (int)itemAmount.amount, this, null, false, null, player.userID, null, null, null);
			player.Command(string.Concat(new object[]
			{
				"note.inv ",
				itemAmount.itemid,
				" ",
				itemAmount.amount * -1f
			}), Array.Empty<object>());
		}
		foreach (global::Item item in list)
		{
			item.Remove(0f);
		}
	}

	// Token: 0x060008D5 RID: 2261 RVA: 0x000558F4 File Offset: 0x00053AF4
	public void SetCustomColour(uint newColour)
	{
		if (newColour == this.customColour)
		{
			return;
		}
		this.customColour = newColour;
		base.SendNetworkUpdateImmediate(false);
		base.ClientRPC(null, "RefreshSkin");
	}

	// Token: 0x060008D6 RID: 2262 RVA: 0x0005591C File Offset: 0x00053B1C
	private bool NeedsSkinChange()
	{
		return this.currentSkin == null || this.forceSkinRefresh || this.lastGrade != this.grade || this.lastModelState != this.modelState || this.lastSkinID != this.skinID;
	}

	// Token: 0x060008D7 RID: 2263 RVA: 0x00055970 File Offset: 0x00053B70
	public void UpdateSkin(bool force = false)
	{
		if (force)
		{
			this.forceSkinRefresh = true;
		}
		if (!this.NeedsSkinChange())
		{
			return;
		}
		if (this.cachedStability <= 0f || base.isServer)
		{
			this.ChangeSkin();
			return;
		}
		if (!this.skinChange)
		{
			this.skinChange = new DeferredAction(this, new Action(this.ChangeSkin), ActionPriority.Medium);
		}
		if (!this.skinChange.Idle)
		{
			return;
		}
		this.skinChange.Invoke();
	}

	// Token: 0x060008D8 RID: 2264 RVA: 0x000559EB File Offset: 0x00053BEB
	private void DestroySkin()
	{
		if (this.currentSkin != null)
		{
			this.currentSkin.Destroy(this);
			this.currentSkin = null;
		}
	}

	// Token: 0x060008D9 RID: 2265 RVA: 0x00055A10 File Offset: 0x00053C10
	private void RefreshNeighbours(bool linkToNeighbours)
	{
		List<EntityLink> entityLinks = base.GetEntityLinks(linkToNeighbours);
		for (int i = 0; i < entityLinks.Count; i++)
		{
			EntityLink entityLink = entityLinks[i];
			for (int j = 0; j < entityLink.connections.Count; j++)
			{
				global::BuildingBlock buildingBlock = entityLink.connections[j].owner as global::BuildingBlock;
				if (!(buildingBlock == null))
				{
					if (Rust.Application.isLoading)
					{
						buildingBlock.UpdateSkin(true);
					}
					else
					{
						global::BuildingBlock.updateSkinQueueServer.Add(buildingBlock);
					}
				}
			}
		}
	}

	// Token: 0x060008DA RID: 2266 RVA: 0x00055A93 File Offset: 0x00053C93
	private void UpdatePlaceholder(bool state)
	{
		if (this.placeholderRenderer)
		{
			this.placeholderRenderer.enabled = state;
		}
		if (this.placeholderCollider)
		{
			this.placeholderCollider.enabled = state;
		}
	}

	// Token: 0x060008DB RID: 2267 RVA: 0x00055AC8 File Offset: 0x00053CC8
	private void ChangeSkin()
	{
		if (base.IsDestroyed)
		{
			return;
		}
		ConstructionGrade currentGrade = this.currentGrade;
		if (currentGrade.skinObject.isValid)
		{
			this.ChangeSkin(currentGrade.skinObject);
			return;
		}
		ConstructionGrade defaultGrade = this.blockDefinition.defaultGrade;
		if (defaultGrade.skinObject.isValid)
		{
			this.ChangeSkin(defaultGrade.skinObject);
			return;
		}
		Debug.LogWarning("No skins found for " + base.gameObject);
	}

	// Token: 0x060008DC RID: 2268 RVA: 0x00055B3C File Offset: 0x00053D3C
	private void ChangeSkin(GameObjectRef prefab)
	{
		bool flag = this.lastGrade != this.grade || this.lastSkinID != this.skinID;
		this.lastGrade = this.grade;
		this.lastSkinID = this.skinID;
		if (flag)
		{
			if (this.currentSkin == null)
			{
				this.UpdatePlaceholder(false);
			}
			else
			{
				this.DestroySkin();
			}
			GameObject gameObject = base.gameManager.CreatePrefab(prefab.resourcePath, base.transform, true);
			this.currentSkin = gameObject.GetComponent<ConstructionSkin>();
			if (this.currentSkin != null && base.isServer && !Rust.Application.isLoading)
			{
				this.customColour = this.currentSkin.GetStartingDetailColour(this.playerCustomColourToApply);
			}
			Model component = this.currentSkin.GetComponent<Model>();
			base.SetModel(component);
			Assert.IsTrue(this.model == component, "Didn't manage to set model successfully!");
		}
		if (base.isServer)
		{
			this.modelState = this.currentSkin.DetermineConditionalModelState(this);
		}
		bool flag2 = this.lastModelState != this.modelState;
		this.lastModelState = this.modelState;
		bool flag3 = this.lastCustomColour != this.customColour;
		this.lastCustomColour = this.customColour;
		if (flag || flag2 || this.forceSkinRefresh || flag3)
		{
			this.currentSkin.Refresh(this);
			if (base.isServer && flag2)
			{
				this.CheckForPipes();
			}
			this.forceSkinRefresh = false;
		}
		if (base.isServer)
		{
			if (flag)
			{
				this.RefreshNeighbours(true);
			}
			if (flag2)
			{
				base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
			}
		}
	}

	// Token: 0x060008DD RID: 2269 RVA: 0x00055CD3 File Offset: 0x00053ED3
	public override bool ShouldBlockProjectiles()
	{
		return this.grade > BuildingGrade.Enum.Twigs;
	}

	// Token: 0x060008DE RID: 2270 RVA: 0x00055CE0 File Offset: 0x00053EE0
	public void CheckForPipes()
	{
		if (!this.CheckForPipesOnModelChange || !ConVar.Server.enforcePipeChecksOnBuildingBlockChanges || Rust.Application.isLoading)
		{
			return;
		}
		List<ColliderInfo_Pipe> list = Facepunch.Pool.GetList<ColliderInfo_Pipe>();
		global::Vis.Components<ColliderInfo_Pipe>(new OBB(base.transform, this.bounds), list, 536870912, QueryTriggerInteraction.Collide);
		foreach (ColliderInfo_Pipe colliderInfo_Pipe in list)
		{
			if (!(colliderInfo_Pipe == null) && colliderInfo_Pipe.gameObject.activeInHierarchy && colliderInfo_Pipe.HasFlag(ColliderInfo.Flags.OnlyBlockBuildingBlock) && colliderInfo_Pipe.ParentEntity != null && colliderInfo_Pipe.ParentEntity.isServer)
			{
				WireTool.AttemptClearSlot(colliderInfo_Pipe.ParentEntity, null, colliderInfo_Pipe.OutputSlotIndex, false);
			}
		}
		Facepunch.Pool.FreeList<ColliderInfo_Pipe>(ref list);
	}

	// Token: 0x060008DF RID: 2271 RVA: 0x000063A5 File Offset: 0x000045A5
	private void OnHammered()
	{
	}

	// Token: 0x060008E0 RID: 2272 RVA: 0x00055DB8 File Offset: 0x00053FB8
	public override float MaxHealth()
	{
		return this.currentGrade.maxHealth;
	}

	// Token: 0x060008E1 RID: 2273 RVA: 0x00055DC5 File Offset: 0x00053FC5
	public override List<ItemAmount> BuildCost()
	{
		return this.currentGrade.CostToBuild(BuildingGrade.Enum.None);
	}

	// Token: 0x060008E2 RID: 2274 RVA: 0x00055DD3 File Offset: 0x00053FD3
	public override void OnHealthChanged(float oldvalue, float newvalue)
	{
		base.OnHealthChanged(oldvalue, newvalue);
		if (!base.isServer)
		{
			return;
		}
		if (Mathf.RoundToInt(oldvalue) == Mathf.RoundToInt(newvalue))
		{
			return;
		}
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.UpdateDistance);
	}

	// Token: 0x060008E3 RID: 2275 RVA: 0x00006CA5 File Offset: 0x00004EA5
	public override float RepairCostFraction()
	{
		return 1f;
	}

	// Token: 0x060008E4 RID: 2276 RVA: 0x00055DFC File Offset: 0x00053FFC
	private bool CanRotate(global::BasePlayer player)
	{
		return this.IsRotatable() && this.HasRotationPrivilege(player) && !this.IsRotationBlocked();
	}

	// Token: 0x060008E5 RID: 2277 RVA: 0x00055E1A File Offset: 0x0005401A
	private bool IsRotatable()
	{
		return this.blockDefinition.grades != null && this.blockDefinition.canRotateAfterPlacement && base.HasFlag(global::BaseEntity.Flags.Reserved1);
	}

	// Token: 0x060008E6 RID: 2278 RVA: 0x00055E4C File Offset: 0x0005404C
	private bool IsRotationBlocked()
	{
		if (!this.blockDefinition.checkVolumeOnRotate)
		{
			return false;
		}
		DeployVolume[] volumes = PrefabAttribute.server.FindAll<DeployVolume>(this.prefabID);
		return DeployVolume.Check(base.transform.position, base.transform.rotation, volumes, ~(1 << base.gameObject.layer));
	}

	// Token: 0x060008E7 RID: 2279 RVA: 0x00055EA6 File Offset: 0x000540A6
	private bool HasRotationPrivilege(global::BasePlayer player)
	{
		return !player.IsBuildingBlocked(base.transform.position, base.transform.rotation, this.bounds);
	}

	// Token: 0x060008E8 RID: 2280 RVA: 0x00055ED0 File Offset: 0x000540D0
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.MaxDistance(3f)]
	private void DoRotation(global::BaseEntity.RPCMessage msg)
	{
		if (!msg.player.CanInteract())
		{
			return;
		}
		if (!this.CanRotate(msg.player))
		{
			return;
		}
		if (!this.blockDefinition.canRotateAfterPlacement)
		{
			return;
		}
		base.transform.localRotation *= Quaternion.Euler(this.blockDefinition.rotationAmount);
		base.RefreshEntityLinks();
		base.UpdateSurroundingEntities();
		this.UpdateSkin(true);
		this.RefreshNeighbours(false);
		base.SendNetworkUpdateImmediate(false);
		base.ClientRPC(null, "RefreshSkin");
	}

	// Token: 0x060008E9 RID: 2281 RVA: 0x00055F5B File Offset: 0x0005415B
	private void StopBeingRotatable()
	{
		base.SetFlag(global::BaseEntity.Flags.Reserved1, false, false, true);
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x060008EA RID: 2282 RVA: 0x00055F74 File Offset: 0x00054174
	private void StartBeingRotatable()
	{
		if (this.blockDefinition.grades == null)
		{
			return;
		}
		if (!this.blockDefinition.canRotateAfterPlacement)
		{
			return;
		}
		base.SetFlag(global::BaseEntity.Flags.Reserved1, true, false, true);
		base.Invoke(new Action(this.StopBeingRotatable), 600f);
	}

	// Token: 0x060008EB RID: 2283 RVA: 0x00055FC4 File Offset: 0x000541C4
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.buildingBlock = Facepunch.Pool.Get<ProtoBuf.BuildingBlock>();
		info.msg.buildingBlock.model = this.modelState;
		info.msg.buildingBlock.grade = (int)this.grade;
		if (this.customColour > 0U)
		{
			info.msg.simpleUint = Facepunch.Pool.Get<SimpleUInt>();
			info.msg.simpleUint.value = this.customColour;
		}
	}

	// Token: 0x060008EC RID: 2284 RVA: 0x00056044 File Offset: 0x00054244
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		this.customColour = 0U;
		if (info.msg.simpleUint != null)
		{
			this.customColour = info.msg.simpleUint.value;
		}
		if (info.msg.buildingBlock != null)
		{
			this.SetConditionalModel(info.msg.buildingBlock.model);
			this.SetGrade((BuildingGrade.Enum)info.msg.buildingBlock.grade);
		}
		if (info.fromDisk)
		{
			base.SetFlag(global::BaseEntity.Flags.Reserved2, false, false, true);
			base.SetFlag(global::BaseEntity.Flags.Reserved1, false, false, true);
			this.UpdateSkin(false);
		}
	}

	// Token: 0x060008ED RID: 2285 RVA: 0x000560E6 File Offset: 0x000542E6
	public override void AttachToBuilding(global::DecayEntity other)
	{
		if (other != null && other is global::BuildingBlock)
		{
			base.AttachToBuilding(other.buildingID);
			BuildingManager.server.CheckMerge(this);
			return;
		}
		base.AttachToBuilding(BuildingManager.server.NewBuildingID());
	}

	// Token: 0x060008EE RID: 2286 RVA: 0x00056124 File Offset: 0x00054324
	public override void ServerInit()
	{
		this.blockDefinition = PrefabAttribute.server.Find<Construction>(this.prefabID);
		if (this.blockDefinition == null)
		{
			Debug.LogError("Couldn't find Construction for prefab " + this.prefabID);
		}
		base.ServerInit();
		this.UpdateSkin(false);
		if (base.HasFlag(global::BaseEntity.Flags.Reserved1) || !Rust.Application.isLoadingSave)
		{
			this.StartBeingRotatable();
		}
		if (base.HasFlag(global::BaseEntity.Flags.Reserved2) || !Rust.Application.isLoadingSave)
		{
			this.StartBeingDemolishable();
		}
		if (this.CullBushes && !Rust.Application.isLoadingSave)
		{
			List<BushEntity> list = Facepunch.Pool.GetList<BushEntity>();
			global::Vis.Entities<BushEntity>(this.WorldSpaceBounds(), list, 67108864, QueryTriggerInteraction.Collide);
			foreach (BushEntity bushEntity in list)
			{
				if (bushEntity.isServer)
				{
					bushEntity.Kill(global::BaseNetworkable.DestroyMode.None);
				}
			}
			Facepunch.Pool.FreeList<BushEntity>(ref list);
		}
	}

	// Token: 0x060008EF RID: 2287 RVA: 0x00056228 File Offset: 0x00054428
	public override void Hurt(HitInfo info)
	{
		if (ConVar.Server.pve && info.Initiator && info.Initiator is global::BasePlayer)
		{
			(info.Initiator as global::BasePlayer).Hurt(info.damageTypes.Total(), DamageType.Generic, null, true);
			return;
		}
		base.Hurt(info);
	}

	// Token: 0x060008F0 RID: 2288 RVA: 0x0005627C File Offset: 0x0005447C
	public override void ResetState()
	{
		base.ResetState();
		this.blockDefinition = null;
		this.forceSkinRefresh = false;
		this.modelState = 0;
		this.lastModelState = 0;
		this.grade = BuildingGrade.Enum.Twigs;
		this.lastGrade = BuildingGrade.Enum.None;
		this.DestroySkin();
		this.UpdatePlaceholder(true);
	}

	// Token: 0x060008F1 RID: 2289 RVA: 0x000562BB File Offset: 0x000544BB
	public override void InitShared()
	{
		base.InitShared();
		this.placeholderRenderer = base.GetComponent<MeshRenderer>();
		this.placeholderCollider = base.GetComponent<MeshCollider>();
	}

	// Token: 0x060008F2 RID: 2290 RVA: 0x000562DB File Offset: 0x000544DB
	public override void PostInitShared()
	{
		this.baseProtection = this.currentGrade.gradeBase.damageProtecton;
		this.grade = this.currentGrade.gradeBase.type;
		base.PostInitShared();
	}

	// Token: 0x060008F3 RID: 2291 RVA: 0x0005630F File Offset: 0x0005450F
	public override void DestroyShared()
	{
		if (base.isServer)
		{
			this.RefreshNeighbours(false);
		}
		base.DestroyShared();
	}

	// Token: 0x060008F4 RID: 2292 RVA: 0x00056326 File Offset: 0x00054526
	public override string Categorize()
	{
		return "building";
	}

	// Token: 0x060008F5 RID: 2293 RVA: 0x00006CA5 File Offset: 0x00004EA5
	public override float BoundsPadding()
	{
		return 1f;
	}

	// Token: 0x060008F6 RID: 2294 RVA: 0x00056330 File Offset: 0x00054530
	public override bool IsOutside()
	{
		float outside_test_range = ConVar.Decay.outside_test_range;
		Vector3 a = base.PivotPoint();
		for (int i = 0; i < global::BuildingBlock.outsideLookupOffsets.Length; i++)
		{
			Vector3 a2 = global::BuildingBlock.outsideLookupOffsets[i];
			Vector3 origin = a + a2 * outside_test_range;
			if (!UnityEngine.Physics.Raycast(new Ray(origin, -a2), outside_test_range - 0.5f, 2097152))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x060008F7 RID: 2295 RVA: 0x0000441C File Offset: 0x0000261C
	public override bool SupportsChildDeployables()
	{
		return true;
	}

	// Token: 0x02000BC1 RID: 3009
	public static class BlockFlags
	{
		// Token: 0x040040CB RID: 16587
		public const global::BaseEntity.Flags CanRotate = global::BaseEntity.Flags.Reserved1;

		// Token: 0x040040CC RID: 16588
		public const global::BaseEntity.Flags CanDemolish = global::BaseEntity.Flags.Reserved2;
	}

	// Token: 0x02000BC2 RID: 3010
	public class UpdateSkinWorkQueue : ObjectWorkQueue<global::BuildingBlock>
	{
		// Token: 0x06004D7C RID: 19836 RVA: 0x001A0EB7 File Offset: 0x0019F0B7
		protected override void RunJob(global::BuildingBlock entity)
		{
			if (!this.ShouldAdd(entity))
			{
				return;
			}
			entity.UpdateSkin(true);
		}

		// Token: 0x06004D7D RID: 19837 RVA: 0x001A0ECA File Offset: 0x0019F0CA
		protected override bool ShouldAdd(global::BuildingBlock entity)
		{
			return entity.IsValid();
		}
	}
}
