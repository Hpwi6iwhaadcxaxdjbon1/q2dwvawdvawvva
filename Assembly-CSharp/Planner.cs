using System;
using System.Collections.Generic;
using System.Linq;
using ConVar;
using Facepunch.Rust;
using Network;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x020000AC RID: 172
public class Planner : global::HeldEntity
{
	// Token: 0x04000A3B RID: 2619
	public global::BaseEntity[] buildableList;

	// Token: 0x06000FAE RID: 4014 RVA: 0x00082D74 File Offset: 0x00080F74
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("Planner.OnRpcMessage", 0))
		{
			if (rpc == 1872774636U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - DoPlace ");
				}
				using (TimeWarning.New("DoPlace", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.IsActiveItem.Test(1872774636U, "DoPlace", this, player))
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
							this.DoPlace(msg2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in DoPlace");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000FAF RID: 4015 RVA: 0x00082ED8 File Offset: 0x000810D8
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsActiveItem]
	private void DoPlace(global::BaseEntity.RPCMessage msg)
	{
		if (!msg.player.CanInteract())
		{
			return;
		}
		using (CreateBuilding createBuilding = CreateBuilding.Deserialize(msg.read))
		{
			this.DoBuild(createBuilding);
		}
	}

	// Token: 0x06000FB0 RID: 4016 RVA: 0x00082F24 File Offset: 0x00081124
	public Socket_Base FindSocket(string name, uint prefabIDToFind)
	{
		return PrefabAttribute.server.FindAll<Socket_Base>(prefabIDToFind).FirstOrDefault((Socket_Base s) => s.socketName == name);
	}

	// Token: 0x06000FB1 RID: 4017 RVA: 0x00082F5C File Offset: 0x0008115C
	public void DoBuild(CreateBuilding msg)
	{
		global::BasePlayer ownerPlayer = base.GetOwnerPlayer();
		if (!ownerPlayer)
		{
			return;
		}
		if (ConVar.AntiHack.objectplacement && ownerPlayer.TriggeredAntiHack(1f, float.PositiveInfinity))
		{
			ownerPlayer.ChatMessage("AntiHack!");
			return;
		}
		Construction construction = PrefabAttribute.server.Find<Construction>(msg.blockID);
		if (construction == null)
		{
			ownerPlayer.ChatMessage("Couldn't find Construction " + msg.blockID);
			return;
		}
		if (!this.CanAffordToPlace(construction))
		{
			ownerPlayer.ChatMessage("Can't afford to place!");
			return;
		}
		if (!ownerPlayer.CanBuild() && !construction.canBypassBuildingPermission)
		{
			ownerPlayer.ChatMessage("Building is blocked!");
			return;
		}
		Deployable deployable = this.GetDeployable();
		if (construction.deployable != deployable)
		{
			ownerPlayer.ChatMessage("Deployable mismatch!");
			global::AntiHack.NoteAdminHack(ownerPlayer);
			return;
		}
		if (ConVar.Server.max_sleeping_bags > 0)
		{
			global::SleepingBag.CanBuildResult? canBuildResult = global::SleepingBag.CanBuildBed(ownerPlayer, construction);
			if (canBuildResult != null)
			{
				if (canBuildResult.Value.Phrase != null)
				{
					ownerPlayer.ShowToast(canBuildResult.Value.Result ? GameTip.Styles.Blue_Long : GameTip.Styles.Red_Normal, canBuildResult.Value.Phrase, canBuildResult.Value.Arguments);
				}
				if (!canBuildResult.Value.Result)
				{
					return;
				}
			}
		}
		Construction.Target target = default(Construction.Target);
		if (msg.entity.IsValid)
		{
			target.entity = (global::BaseNetworkable.serverEntities.Find(msg.entity) as global::BaseEntity);
			if (target.entity == null)
			{
				ownerPlayer.ChatMessage("Couldn't find entity " + msg.entity);
				return;
			}
			msg.position = target.entity.transform.TransformPoint(msg.position);
			msg.normal = target.entity.transform.TransformDirection(msg.normal);
			msg.rotation = target.entity.transform.rotation * msg.rotation;
			if (msg.socket > 0U)
			{
				string text = StringPool.Get(msg.socket);
				if (text != "")
				{
					target.socket = this.FindSocket(text, target.entity.prefabID);
				}
				if (target.socket == null)
				{
					ownerPlayer.ChatMessage("Couldn't find socket " + msg.socket);
					return;
				}
			}
			else if (target.entity is Door)
			{
				ownerPlayer.ChatMessage("Can't deploy on door");
				return;
			}
		}
		target.ray = msg.ray;
		target.onTerrain = msg.onterrain;
		target.position = msg.position;
		target.normal = msg.normal;
		target.rotation = msg.rotation;
		target.player = ownerPlayer;
		target.valid = true;
		if (target.entity != null && deployable != null && deployable.setSocketParent)
		{
			Vector3 position = (target.socket != null) ? target.GetWorldPosition() : target.position;
			float num = target.entity.Distance(position);
			if (num > 1f)
			{
				ownerPlayer.ChatMessage("Parent too far away: " + num);
				return;
			}
		}
		this.DoBuild(target, construction);
	}

	// Token: 0x06000FB2 RID: 4018 RVA: 0x00083294 File Offset: 0x00081494
	public void DoBuild(Construction.Target target, Construction component)
	{
		global::BasePlayer ownerPlayer = base.GetOwnerPlayer();
		if (!ownerPlayer)
		{
			return;
		}
		if (target.ray.IsNaNOrInfinity())
		{
			return;
		}
		if (target.position.IsNaNOrInfinity())
		{
			return;
		}
		if (target.normal.IsNaNOrInfinity())
		{
			return;
		}
		if (target.socket != null)
		{
			if (!target.socket.female)
			{
				ownerPlayer.ChatMessage("Target socket is not female. (" + target.socket.socketName + ")");
				return;
			}
			if (target.entity != null && target.entity.IsOccupied(target.socket))
			{
				ownerPlayer.ChatMessage("Target socket is occupied. (" + target.socket.socketName + ")");
				return;
			}
			if (target.onTerrain)
			{
				ownerPlayer.ChatMessage("Target on terrain is not allowed when attaching to socket. (" + target.socket.socketName + ")");
				return;
			}
		}
		Vector3 vector = (target.entity != null && target.socket != null) ? target.GetWorldPosition() : target.position;
		if (global::AntiHack.TestIsBuildingInsideSomething(target, vector))
		{
			ownerPlayer.ChatMessage("Can't deploy inside objects");
			return;
		}
		if (ConVar.AntiHack.eye_protection >= 2)
		{
			Vector3 center = ownerPlayer.eyes.center;
			Vector3 position = ownerPlayer.eyes.position;
			Vector3 origin = target.ray.origin;
			Vector3 vector2 = vector;
			int num = 2097152;
			int num2 = ConVar.AntiHack.build_terraincheck ? 10551296 : 2162688;
			float num3 = ConVar.AntiHack.build_losradius;
			float padding = ConVar.AntiHack.build_losradius + 0.01f;
			int layerMask = num2;
			if (target.socket != null)
			{
				num3 = 0f;
				padding = 0.5f;
				layerMask = num;
			}
			if (component.isSleepingBag)
			{
				num3 = ConVar.AntiHack.build_losradius_sleepingbag;
				padding = ConVar.AntiHack.build_losradius_sleepingbag + 0.01f;
				layerMask = num2;
			}
			if (num3 > 0f)
			{
				vector2 += target.normal.normalized * num3;
			}
			if (target.entity != null)
			{
				DeployShell deployShell = PrefabAttribute.server.Find<DeployShell>(target.entity.prefabID);
				if (deployShell != null)
				{
					vector2 += target.normal.normalized * deployShell.LineOfSightPadding();
				}
			}
			if (!GamePhysics.LineOfSightRadius(center, position, layerMask, num3, null) || !GamePhysics.LineOfSightRadius(position, origin, layerMask, num3, null) || !GamePhysics.LineOfSightRadius(origin, vector2, layerMask, num3, 0f, padding, null))
			{
				ownerPlayer.ChatMessage("Line of sight blocked.");
				return;
			}
		}
		Construction.lastPlacementError = "No Error";
		GameObject gameObject = this.DoPlacement(target, component);
		if (gameObject == null)
		{
			ownerPlayer.ChatMessage("Can't place: " + Construction.lastPlacementError);
		}
		if (gameObject != null)
		{
			Deployable deployable = this.GetDeployable();
			global::BaseEntity baseEntity = gameObject.ToBaseEntity();
			if (deployable != null)
			{
				if (deployable.setSocketParent && target.entity != null && target.entity.SupportsChildDeployables() && baseEntity)
				{
					baseEntity.SetParent(target.entity, true, false);
				}
				if (deployable.wantsInstanceData && base.GetOwnerItem().instanceData != null)
				{
					(baseEntity as IInstanceDataReceiver).ReceiveInstanceData(base.GetOwnerItem().instanceData);
				}
				if (deployable.copyInventoryFromItem)
				{
					StorageContainer component2 = baseEntity.GetComponent<StorageContainer>();
					if (component2)
					{
						component2.ReceiveInventoryFromItem(base.GetOwnerItem());
					}
				}
				ItemModDeployable modDeployable = this.GetModDeployable();
				if (modDeployable != null)
				{
					modDeployable.OnDeployed(baseEntity, ownerPlayer);
				}
				baseEntity.OnDeployed(baseEntity.GetParentEntity(), ownerPlayer, base.GetOwnerItem());
				if (deployable.placeEffect.isValid)
				{
					if (target.entity && target.socket != null)
					{
						Effect.server.Run(deployable.placeEffect.resourcePath, target.entity.transform.TransformPoint(target.socket.worldPosition), target.entity.transform.up, null, false);
					}
					else
					{
						Effect.server.Run(deployable.placeEffect.resourcePath, target.position, target.normal, null, false);
					}
				}
			}
			if (baseEntity != null)
			{
				Analytics.Azure.OnEntityBuilt(baseEntity, ownerPlayer);
			}
			this.PayForPlacement(ownerPlayer, component);
		}
	}

	// Token: 0x06000FB3 RID: 4019 RVA: 0x000836E4 File Offset: 0x000818E4
	public GameObject DoPlacement(Construction.Target placement, Construction component)
	{
		global::BasePlayer ownerPlayer = base.GetOwnerPlayer();
		if (!ownerPlayer)
		{
			return null;
		}
		global::BaseEntity baseEntity = component.CreateConstruction(placement, true);
		if (!baseEntity)
		{
			return null;
		}
		float num = 1f;
		global::Item ownerItem = base.GetOwnerItem();
		if (ownerItem != null)
		{
			baseEntity.skinID = ownerItem.skin;
			if (ownerItem.hasCondition)
			{
				num = ownerItem.conditionNormalized;
			}
		}
		baseEntity.gameObject.AwakeFromInstantiate();
		global::BuildingBlock buildingBlock = baseEntity as global::BuildingBlock;
		if (buildingBlock)
		{
			buildingBlock.blockDefinition = PrefabAttribute.server.Find<Construction>(buildingBlock.prefabID);
			if (!buildingBlock.blockDefinition)
			{
				Debug.LogError("Placing a building block that has no block definition!");
				return null;
			}
			buildingBlock.SetGrade(buildingBlock.blockDefinition.defaultGrade.gradeBase.type);
			float num2 = buildingBlock.currentGrade.maxHealth;
		}
		BaseCombatEntity baseCombatEntity = baseEntity as BaseCombatEntity;
		if (baseCombatEntity)
		{
			float num2 = (buildingBlock != null) ? buildingBlock.currentGrade.maxHealth : baseCombatEntity.startHealth;
			baseCombatEntity.ResetLifeStateOnSpawn = false;
			baseCombatEntity.InitializeHealth(num2 * num, num2);
		}
		baseEntity.gameObject.SendMessage("SetDeployedBy", ownerPlayer, SendMessageOptions.DontRequireReceiver);
		baseEntity.OwnerID = ownerPlayer.userID;
		baseEntity.Spawn();
		if (buildingBlock)
		{
			Effect.server.Run("assets/bundled/prefabs/fx/build/frame_place.prefab", baseEntity, 0U, Vector3.zero, Vector3.zero, null, false);
		}
		global::StabilityEntity stabilityEntity = baseEntity as global::StabilityEntity;
		if (stabilityEntity)
		{
			stabilityEntity.UpdateSurroundingEntities();
		}
		return baseEntity.gameObject;
	}

	// Token: 0x06000FB4 RID: 4020 RVA: 0x0008386C File Offset: 0x00081A6C
	public void PayForPlacement(global::BasePlayer player, Construction component)
	{
		if (this.isTypeDeployable)
		{
			this.GetItem().UseItem(1);
			return;
		}
		List<global::Item> list = new List<global::Item>();
		foreach (ItemAmount itemAmount in component.defaultGrade.CostToBuild(BuildingGrade.Enum.None))
		{
			player.inventory.Take(list, itemAmount.itemDef.itemid, (int)itemAmount.amount);
			player.Command("note.inv", new object[]
			{
				itemAmount.itemDef.itemid,
				itemAmount.amount * -1f
			});
		}
		foreach (global::Item item in list)
		{
			item.Remove(0f);
		}
	}

	// Token: 0x06000FB5 RID: 4021 RVA: 0x00083970 File Offset: 0x00081B70
	public bool CanAffordToPlace(Construction component)
	{
		if (this.isTypeDeployable)
		{
			return true;
		}
		global::BasePlayer ownerPlayer = base.GetOwnerPlayer();
		if (!ownerPlayer)
		{
			return false;
		}
		foreach (ItemAmount itemAmount in component.defaultGrade.CostToBuild(BuildingGrade.Enum.None))
		{
			if ((float)ownerPlayer.inventory.GetAmount(itemAmount.itemDef.itemid) < itemAmount.amount)
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x06000FB6 RID: 4022 RVA: 0x00083A04 File Offset: 0x00081C04
	public ItemModDeployable GetModDeployable()
	{
		ItemDefinition ownerItemDefinition = base.GetOwnerItemDefinition();
		if (ownerItemDefinition == null)
		{
			return null;
		}
		return ownerItemDefinition.GetComponent<ItemModDeployable>();
	}

	// Token: 0x06000FB7 RID: 4023 RVA: 0x00083A2C File Offset: 0x00081C2C
	public Deployable GetDeployable()
	{
		ItemModDeployable modDeployable = this.GetModDeployable();
		if (modDeployable == null)
		{
			return null;
		}
		return modDeployable.GetDeployable(this);
	}

	// Token: 0x1700017B RID: 379
	// (get) Token: 0x06000FB8 RID: 4024 RVA: 0x00083A52 File Offset: 0x00081C52
	public bool isTypeDeployable
	{
		get
		{
			return this.GetModDeployable() != null;
		}
	}
}
