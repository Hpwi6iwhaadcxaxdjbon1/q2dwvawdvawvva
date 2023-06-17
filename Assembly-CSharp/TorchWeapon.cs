using System;
using ConVar;
using Network;
using Rust;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x020000E1 RID: 225
public class TorchWeapon : BaseMelee
{
	// Token: 0x04000C3C RID: 3132
	[NonSerialized]
	public float fuelTickAmount = 0.083333336f;

	// Token: 0x04000C3D RID: 3133
	[Header("TorchWeapon")]
	public AnimatorOverrideController LitHoldAnimationOverride;

	// Token: 0x04000C3E RID: 3134
	public GameObjectRef litStrikeFX;

	// Token: 0x060013A4 RID: 5028 RVA: 0x0009D8D4 File Offset: 0x0009BAD4
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("TorchWeapon.OnRpcMessage", 0))
		{
			if (rpc == 2235491565U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - Extinguish ");
				}
				using (TimeWarning.New("Extinguish", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.IsActiveItem.Test(2235491565U, "Extinguish", this, player))
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
							this.Extinguish(msg2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in Extinguish");
					}
				}
				return true;
			}
			if (rpc == 3010584743U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - Ignite ");
				}
				using (TimeWarning.New("Ignite", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.IsActiveItem.Test(3010584743U, "Ignite", this, player))
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
							this.Ignite(msg3);
						}
					}
					catch (Exception exception2)
					{
						Debug.LogException(exception2);
						player.Kick("RPC Error in Ignite");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x060013A5 RID: 5029 RVA: 0x0009DBCC File Offset: 0x0009BDCC
	public override void GetAttackStats(HitInfo info)
	{
		base.GetAttackStats(info);
		if (base.HasFlag(BaseEntity.Flags.On))
		{
			info.damageTypes.Add(DamageType.Heat, 1f);
		}
	}

	// Token: 0x060013A6 RID: 5030 RVA: 0x0009DBEF File Offset: 0x0009BDEF
	public override float GetConditionLoss()
	{
		return base.GetConditionLoss() + (base.HasFlag(BaseEntity.Flags.On) ? 6f : 0f);
	}

	// Token: 0x060013A7 RID: 5031 RVA: 0x0009DC10 File Offset: 0x0009BE10
	public void SetIsOn(bool isOn)
	{
		if (isOn)
		{
			base.SetFlag(BaseEntity.Flags.On, true, false, true);
			base.InvokeRepeating(new Action(this.UseFuel), 1f, 1f);
			return;
		}
		base.SetFlag(BaseEntity.Flags.On, false, false, true);
		base.CancelInvoke(new Action(this.UseFuel));
	}

	// Token: 0x060013A8 RID: 5032 RVA: 0x0009DC63 File Offset: 0x0009BE63
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.IsActiveItem]
	private void Ignite(BaseEntity.RPCMessage msg)
	{
		if (msg.player.CanInteract())
		{
			this.SetIsOn(true);
		}
	}

	// Token: 0x060013A9 RID: 5033 RVA: 0x0009DC79 File Offset: 0x0009BE79
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.IsActiveItem]
	private void Extinguish(BaseEntity.RPCMessage msg)
	{
		if (msg.player.CanInteract())
		{
			this.SetIsOn(false);
		}
	}

	// Token: 0x060013AA RID: 5034 RVA: 0x0009DC90 File Offset: 0x0009BE90
	public void UseFuel()
	{
		Item ownerItem = base.GetOwnerItem();
		if (ownerItem == null)
		{
			return;
		}
		ownerItem.LoseCondition(this.fuelTickAmount);
	}

	// Token: 0x060013AB RID: 5035 RVA: 0x0009DCB4 File Offset: 0x0009BEB4
	public override void OnHeldChanged()
	{
		if (base.IsDisabled())
		{
			base.SetFlag(BaseEntity.Flags.On, false, false, true);
			base.CancelInvoke(new Action(this.UseFuel));
		}
	}

	// Token: 0x060013AC RID: 5036 RVA: 0x0009DCDC File Offset: 0x0009BEDC
	public override string GetStrikeEffectPath(string materialName)
	{
		for (int i = 0; i < this.materialStrikeFX.Count; i++)
		{
			if (this.materialStrikeFX[i].materialName == materialName && this.materialStrikeFX[i].fx.isValid)
			{
				return this.materialStrikeFX[i].fx.resourcePath;
			}
		}
		if (base.HasFlag(BaseEntity.Flags.On) && this.litStrikeFX.isValid)
		{
			return this.litStrikeFX.resourcePath;
		}
		return this.strikeFX.resourcePath;
	}
}
