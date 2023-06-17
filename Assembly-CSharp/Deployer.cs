using System;
using ConVar;
using Facepunch.Rust;
using Network;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x02000066 RID: 102
public class Deployer : HeldEntity
{
	// Token: 0x06000A4E RID: 2638 RVA: 0x0005ECB0 File Offset: 0x0005CEB0
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("Deployer.OnRpcMessage", 0))
		{
			if (rpc == 3001117906U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - DoDeploy ");
				}
				using (TimeWarning.New("DoDeploy", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.IsActiveItem.Test(3001117906U, "DoDeploy", this, player))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							BaseEntity.RPCMessage msg2 = new BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.DoDeploy(msg2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in DoDeploy");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000A4F RID: 2639 RVA: 0x0005EE14 File Offset: 0x0005D014
	public ItemModDeployable GetModDeployable()
	{
		ItemDefinition ownerItemDefinition = base.GetOwnerItemDefinition();
		if (ownerItemDefinition == null)
		{
			return null;
		}
		return ownerItemDefinition.GetComponent<ItemModDeployable>();
	}

	// Token: 0x06000A50 RID: 2640 RVA: 0x0005EE3C File Offset: 0x0005D03C
	public Deployable GetDeployable()
	{
		ItemModDeployable modDeployable = this.GetModDeployable();
		if (modDeployable == null)
		{
			return null;
		}
		return modDeployable.GetDeployable(this);
	}

	// Token: 0x06000A51 RID: 2641 RVA: 0x0005EE62 File Offset: 0x0005D062
	public Quaternion GetDeployedRotation(Vector3 normal, Vector3 placeDir)
	{
		return Quaternion.LookRotation(normal, placeDir) * Quaternion.Euler(90f, 0f, 0f);
	}

	// Token: 0x06000A52 RID: 2642 RVA: 0x0005EE84 File Offset: 0x0005D084
	public bool IsPlacementAngleAcceptable(Vector3 pos, Quaternion rot)
	{
		Vector3 lhs = rot * Vector3.up;
		return Mathf.Acos(Vector3.Dot(lhs, Vector3.up)) <= 0.61086524f;
	}

	// Token: 0x06000A53 RID: 2643 RVA: 0x0005EEB8 File Offset: 0x0005D0B8
	public bool CheckPlacement(Deployable deployable, Ray ray, float fDistance)
	{
		using (TimeWarning.New("Deploy.CheckPlacement", 0))
		{
			RaycastHit raycastHit;
			if (!UnityEngine.Physics.Raycast(ray, out raycastHit, fDistance, 1235288065))
			{
				return false;
			}
			DeployVolume[] volumes = PrefabAttribute.server.FindAll<DeployVolume>(deployable.prefabID);
			Vector3 point = raycastHit.point;
			Quaternion deployedRotation = this.GetDeployedRotation(raycastHit.normal, ray.direction);
			if (DeployVolume.Check(point, deployedRotation, volumes, -1))
			{
				return false;
			}
			if (!this.IsPlacementAngleAcceptable(raycastHit.point, deployedRotation))
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x06000A54 RID: 2644 RVA: 0x0005EF58 File Offset: 0x0005D158
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.IsActiveItem]
	private void DoDeploy(BaseEntity.RPCMessage msg)
	{
		if (!msg.player.CanInteract())
		{
			return;
		}
		Deployable deployable = this.GetDeployable();
		if (deployable == null)
		{
			return;
		}
		Ray ray = msg.read.Ray();
		NetworkableId entityID = msg.read.EntityID();
		if (deployable.toSlot)
		{
			this.DoDeploy_Slot(deployable, ray, entityID);
			return;
		}
		this.DoDeploy_Regular(deployable, ray);
	}

	// Token: 0x06000A55 RID: 2645 RVA: 0x0005EFB8 File Offset: 0x0005D1B8
	public void DoDeploy_Slot(Deployable deployable, Ray ray, NetworkableId entityID)
	{
		if (!base.HasItemAmount())
		{
			return;
		}
		BasePlayer ownerPlayer = base.GetOwnerPlayer();
		if (!ownerPlayer)
		{
			return;
		}
		if (!ownerPlayer.CanBuild())
		{
			ownerPlayer.ChatMessage("Building is blocked at player position!");
			return;
		}
		BaseEntity baseEntity = BaseNetworkable.serverEntities.Find(entityID) as BaseEntity;
		if (baseEntity == null)
		{
			return;
		}
		if (!baseEntity.HasSlot(deployable.slot))
		{
			return;
		}
		if (baseEntity.GetSlot(deployable.slot) != null)
		{
			return;
		}
		if (ownerPlayer.Distance(baseEntity) > 3f)
		{
			ownerPlayer.ChatMessage("Too far away!");
			return;
		}
		if (!ownerPlayer.CanBuild(baseEntity.WorldSpaceBounds()))
		{
			ownerPlayer.ChatMessage("Building is blocked at placement position!");
			return;
		}
		Item ownerItem = base.GetOwnerItem();
		ItemModDeployable modDeployable = this.GetModDeployable();
		BaseEntity baseEntity2 = GameManager.server.CreateEntity(modDeployable.entityPrefab.resourcePath, default(Vector3), default(Quaternion), true);
		if (baseEntity2 != null)
		{
			baseEntity2.skinID = ownerItem.skin;
			baseEntity2.SetParent(baseEntity, baseEntity.GetSlotAnchorName(deployable.slot), false, false);
			baseEntity2.OwnerID = ownerPlayer.userID;
			baseEntity2.OnDeployed(baseEntity, ownerPlayer, ownerItem);
			baseEntity2.Spawn();
			baseEntity.SetSlot(deployable.slot, baseEntity2);
			if (deployable.placeEffect.isValid)
			{
				Effect.server.Run(deployable.placeEffect.resourcePath, baseEntity.transform.position, Vector3.up, null, false);
			}
		}
		modDeployable.OnDeployed(baseEntity2, ownerPlayer);
		Analytics.Azure.OnEntityBuilt(baseEntity2, ownerPlayer);
		base.UseItemAmount(1);
	}

	// Token: 0x06000A56 RID: 2646 RVA: 0x0005F140 File Offset: 0x0005D340
	public void DoDeploy_Regular(Deployable deployable, Ray ray)
	{
		if (!base.HasItemAmount())
		{
			return;
		}
		BasePlayer ownerPlayer = base.GetOwnerPlayer();
		if (!ownerPlayer)
		{
			return;
		}
		if (!ownerPlayer.CanBuild())
		{
			ownerPlayer.ChatMessage("Building is blocked at player position!");
			return;
		}
		if (ConVar.AntiHack.objectplacement && ownerPlayer.TriggeredAntiHack(1f, float.PositiveInfinity))
		{
			ownerPlayer.ChatMessage("AntiHack!");
			return;
		}
		if (!this.CheckPlacement(deployable, ray, 8f))
		{
			return;
		}
		RaycastHit raycastHit;
		if (!UnityEngine.Physics.Raycast(ray, out raycastHit, 8f, 1235288065))
		{
			return;
		}
		Vector3 point = raycastHit.point;
		Quaternion deployedRotation = this.GetDeployedRotation(raycastHit.normal, ray.direction);
		Item ownerItem = base.GetOwnerItem();
		ItemModDeployable modDeployable = this.GetModDeployable();
		if (ownerPlayer.Distance(point) > 3f)
		{
			ownerPlayer.ChatMessage("Too far away!");
			return;
		}
		if (!ownerPlayer.CanBuild(point, deployedRotation, deployable.bounds))
		{
			ownerPlayer.ChatMessage("Building is blocked at placement position!");
			return;
		}
		BaseEntity baseEntity = GameManager.server.CreateEntity(modDeployable.entityPrefab.resourcePath, point, deployedRotation, true);
		if (!baseEntity)
		{
			Debug.LogWarning("Couldn't create prefab:" + modDeployable.entityPrefab.resourcePath);
			return;
		}
		baseEntity.skinID = ownerItem.skin;
		baseEntity.SendMessage("SetDeployedBy", ownerPlayer, SendMessageOptions.DontRequireReceiver);
		baseEntity.OwnerID = ownerPlayer.userID;
		baseEntity.Spawn();
		modDeployable.OnDeployed(baseEntity, ownerPlayer);
		Analytics.Azure.OnEntityBuilt(baseEntity, ownerPlayer);
		base.UseItemAmount(1);
	}
}
