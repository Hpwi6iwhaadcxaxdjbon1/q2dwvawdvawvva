using System;
using ConVar;
using Facepunch.Rust;
using Network;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x0200009A RID: 154
public class MedicalTool : AttackEntity
{
	// Token: 0x0400091F RID: 2335
	public float healDurationSelf = 4f;

	// Token: 0x04000920 RID: 2336
	public float healDurationOther = 4f;

	// Token: 0x04000921 RID: 2337
	public float healDurationOtherWounded = 7f;

	// Token: 0x04000922 RID: 2338
	public float maxDistanceOther = 2f;

	// Token: 0x04000923 RID: 2339
	public bool canUseOnOther = true;

	// Token: 0x04000924 RID: 2340
	public bool canRevive = true;

	// Token: 0x06000E0A RID: 3594 RVA: 0x00076E24 File Offset: 0x00075024
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("MedicalTool.OnRpcMessage", 0))
		{
			if (rpc == 789049461U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - UseOther ");
				}
				using (TimeWarning.New("UseOther", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.IsActiveItem.Test(789049461U, "UseOther", this, player))
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
							this.UseOther(msg2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in UseOther");
					}
				}
				return true;
			}
			if (rpc == 2918424470U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - UseSelf ");
				}
				using (TimeWarning.New("UseSelf", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.IsActiveItem.Test(2918424470U, "UseSelf", this, player))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							BaseEntity.RPCMessage msg3 = new BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.UseSelf(msg3);
						}
					}
					catch (Exception exception2)
					{
						Debug.LogException(exception2);
						player.Kick("RPC Error in UseSelf");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000E0B RID: 3595 RVA: 0x0007711C File Offset: 0x0007531C
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.IsActiveItem]
	private void UseOther(BaseEntity.RPCMessage msg)
	{
		BasePlayer player = msg.player;
		if (!this.VerifyClientAttack(player))
		{
			base.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
			return;
		}
		if (!player.CanInteract())
		{
			return;
		}
		if (!base.HasItemAmount() || !this.canUseOnOther)
		{
			return;
		}
		BasePlayer basePlayer = BaseNetworkable.serverEntities.Find(msg.read.EntityID()) as BasePlayer;
		if (basePlayer != null && Vector3.Distance(basePlayer.transform.position, player.transform.position) < 4f)
		{
			base.ClientRPCPlayer(null, player, "Reset");
			this.GiveEffectsTo(basePlayer);
			base.UseItemAmount(1);
			base.StartAttackCooldown(this.repeatDelay);
		}
	}

	// Token: 0x06000E0C RID: 3596 RVA: 0x000771CC File Offset: 0x000753CC
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.IsActiveItem]
	private void UseSelf(BaseEntity.RPCMessage msg)
	{
		BasePlayer player = msg.player;
		if (!this.VerifyClientAttack(player))
		{
			base.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
			return;
		}
		if (!player.CanInteract())
		{
			return;
		}
		if (!base.HasItemAmount())
		{
			return;
		}
		base.ClientRPCPlayer(null, player, "Reset");
		this.GiveEffectsTo(player);
		base.UseItemAmount(1);
		base.StartAttackCooldown(this.repeatDelay);
	}

	// Token: 0x06000E0D RID: 3597 RVA: 0x0007722C File Offset: 0x0007542C
	public override void ServerUse()
	{
		if (base.isClient)
		{
			return;
		}
		BasePlayer ownerPlayer = base.GetOwnerPlayer();
		if (ownerPlayer == null)
		{
			return;
		}
		if (!ownerPlayer.CanInteract())
		{
			return;
		}
		if (!base.HasItemAmount())
		{
			return;
		}
		this.GiveEffectsTo(ownerPlayer);
		base.UseItemAmount(1);
		base.StartAttackCooldown(this.repeatDelay);
		base.SignalBroadcast(BaseEntity.Signal.Attack, string.Empty, null);
		if (ownerPlayer.IsNpc)
		{
			ownerPlayer.SignalBroadcast(BaseEntity.Signal.Attack, null);
		}
	}

	// Token: 0x06000E0E RID: 3598 RVA: 0x000772A0 File Offset: 0x000754A0
	private void GiveEffectsTo(BasePlayer player)
	{
		if (!player)
		{
			return;
		}
		ItemDefinition ownerItemDefinition = base.GetOwnerItemDefinition();
		ItemModConsumable component = ownerItemDefinition.GetComponent<ItemModConsumable>();
		if (!component)
		{
			Debug.LogWarning("No consumable for medicaltool :" + base.name);
			return;
		}
		BasePlayer ownerPlayer = base.GetOwnerPlayer();
		Analytics.Azure.OnMedUsed(ownerItemDefinition.shortname, ownerPlayer, player);
		if (player != ownerPlayer && player.IsWounded() && this.canRevive)
		{
			player.StopWounded(ownerPlayer);
		}
		foreach (ItemModConsumable.ConsumableEffect consumableEffect in component.effects)
		{
			if (consumableEffect.type == MetabolismAttribute.Type.Health)
			{
				player.health += consumableEffect.amount;
			}
			else
			{
				player.metabolism.ApplyChange(consumableEffect.type, consumableEffect.amount, consumableEffect.time);
			}
		}
		if (player is BasePet)
		{
			player.SendNetworkUpdateImmediate(false);
		}
	}
}
