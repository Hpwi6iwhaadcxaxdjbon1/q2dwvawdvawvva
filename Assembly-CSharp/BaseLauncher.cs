using System;
using ConVar;
using Facepunch.Rust;
using Network;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x0200003E RID: 62
public class BaseLauncher : BaseProjectile
{
	// Token: 0x060003EA RID: 1002 RVA: 0x000317E4 File Offset: 0x0002F9E4
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("BaseLauncher.OnRpcMessage", 0))
		{
			if (rpc == 853319324U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - SV_Launch ");
				}
				using (TimeWarning.New("SV_Launch", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.IsActiveItem.Test(853319324U, "SV_Launch", this, player))
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
							this.SV_Launch(msg2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in SV_Launch");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x060003EB RID: 1003 RVA: 0x0000441C File Offset: 0x0000261C
	public override bool ForceSendMagazine(BaseNetworkable.SaveInfo saveInfo)
	{
		return true;
	}

	// Token: 0x060003EC RID: 1004 RVA: 0x00031948 File Offset: 0x0002FB48
	public override void ServerUse()
	{
		this.ServerUse(1f, null);
	}

	// Token: 0x060003ED RID: 1005 RVA: 0x00031958 File Offset: 0x0002FB58
	public override void ServerUse(float damageModifier, Transform originOverride = null)
	{
		ItemModProjectile component = this.primaryMagazine.ammoType.GetComponent<ItemModProjectile>();
		if (!component)
		{
			return;
		}
		if (this.primaryMagazine.contents <= 0)
		{
			base.SignalBroadcast(BaseEntity.Signal.DryFire, null);
			base.StartAttackCooldown(1f);
			return;
		}
		if (!component.projectileObject.Get().GetComponent<ServerProjectile>())
		{
			base.ServerUse(damageModifier, originOverride);
			return;
		}
		this.primaryMagazine.contents--;
		if (this.primaryMagazine.contents < 0)
		{
			this.primaryMagazine.contents = 0;
		}
		Vector3 vector = this.MuzzlePoint.transform.forward;
		Vector3 position = this.MuzzlePoint.transform.position;
		float num = this.GetAimCone() + component.projectileSpread;
		if (num > 0f)
		{
			vector = AimConeUtil.GetModifiedAimConeDirection(num, vector, true);
		}
		float num2 = 1f;
		RaycastHit raycastHit;
		if (UnityEngine.Physics.Raycast(position, vector, out raycastHit, num2, 1236478737))
		{
			num2 = raycastHit.distance - 0.1f;
		}
		BaseEntity baseEntity = GameManager.server.CreateEntity(component.projectileObject.resourcePath, position + vector * num2, default(Quaternion), true);
		if (baseEntity == null)
		{
			return;
		}
		BasePlayer ownerPlayer = base.GetOwnerPlayer();
		bool flag = ownerPlayer != null && ownerPlayer.IsNpc;
		ServerProjectile component2 = baseEntity.GetComponent<ServerProjectile>();
		if (component2)
		{
			component2.InitializeVelocity(vector * component2.speed);
		}
		baseEntity.SendMessage("SetDamageScale", flag ? this.npcDamageScale : this.turretDamageScale);
		baseEntity.Spawn();
		base.StartAttackCooldown(base.ScaleRepeatDelay(this.repeatDelay));
		base.SignalBroadcast(BaseEntity.Signal.Attack, string.Empty, null);
		Item ownerItem = base.GetOwnerItem();
		if (ownerItem == null)
		{
			return;
		}
		ownerItem.LoseCondition(UnityEngine.Random.Range(1f, 2f));
	}

	// Token: 0x060003EE RID: 1006 RVA: 0x00031B44 File Offset: 0x0002FD44
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.IsActiveItem]
	private void SV_Launch(BaseEntity.RPCMessage msg)
	{
		BasePlayer player = msg.player;
		if (!this.VerifyClientAttack(player))
		{
			base.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
			return;
		}
		if (this.reloadFinished && base.HasReloadCooldown())
		{
			global::AntiHack.Log(player, AntiHackType.ProjectileHack, "Reloading (" + base.ShortPrefabName + ")");
			player.stats.combat.LogInvalid(player, this, "reload_cooldown");
			return;
		}
		this.reloadStarted = false;
		this.reloadFinished = false;
		if (!base.UsingInfiniteAmmoCheat)
		{
			if (this.primaryMagazine.contents <= 0)
			{
				global::AntiHack.Log(player, AntiHackType.ProjectileHack, "Magazine empty (" + base.ShortPrefabName + ")");
				player.stats.combat.LogInvalid(player, this, "magazine_empty");
				return;
			}
			this.primaryMagazine.contents--;
		}
		base.SignalBroadcast(BaseEntity.Signal.Attack, string.Empty, player.net.connection);
		Vector3 vector = msg.read.Vector3();
		Vector3 vector2 = msg.read.Vector3().normalized;
		bool flag = msg.read.Bit();
		BaseEntity baseEntity = player.GetParentEntity();
		if (baseEntity == null)
		{
			baseEntity = player.GetMounted();
		}
		if (flag)
		{
			if (baseEntity != null)
			{
				vector = baseEntity.transform.TransformPoint(vector);
				vector2 = baseEntity.transform.TransformDirection(vector2);
			}
			else
			{
				vector = player.eyes.position;
				vector2 = player.eyes.BodyForward();
			}
		}
		if (!base.ValidateEyePos(player, vector))
		{
			return;
		}
		ItemModProjectile component = this.primaryMagazine.ammoType.GetComponent<ItemModProjectile>();
		if (!component)
		{
			global::AntiHack.Log(player, AntiHackType.ProjectileHack, "Item mod not found (" + base.ShortPrefabName + ")");
			player.stats.combat.LogInvalid(player, this, "mod_missing");
			return;
		}
		float num = this.GetAimCone() + component.projectileSpread;
		if (num > 0f)
		{
			vector2 = AimConeUtil.GetModifiedAimConeDirection(num, vector2, true);
		}
		float num2 = 1f;
		RaycastHit raycastHit;
		if (UnityEngine.Physics.Raycast(vector, vector2, out raycastHit, num2, 1236478737))
		{
			num2 = raycastHit.distance - 0.1f;
		}
		BaseEntity baseEntity2 = GameManager.server.CreateEntity(component.projectileObject.resourcePath, vector + vector2 * num2, default(Quaternion), true);
		if (baseEntity2 == null)
		{
			return;
		}
		baseEntity2.creatorEntity = player;
		ServerProjectile component2 = baseEntity2.GetComponent<ServerProjectile>();
		if (component2)
		{
			component2.InitializeVelocity(this.GetInheritedVelocity(player, vector2) + vector2 * component2.speed);
		}
		baseEntity2.Spawn();
		Analytics.Azure.OnExplosiveLaunched(player, baseEntity2, this);
		base.StartAttackCooldown(base.ScaleRepeatDelay(this.repeatDelay));
		Item ownerItem = base.GetOwnerItem();
		if (ownerItem == null)
		{
			return;
		}
		if (!base.UsingInfiniteAmmoCheat)
		{
			ownerItem.LoseCondition(UnityEngine.Random.Range(1f, 2f));
		}
	}
}
