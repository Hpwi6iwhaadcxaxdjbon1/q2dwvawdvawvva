using System;
using ConVar;
using Network;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x0200005E RID: 94
public class CompoundBowWeapon : BowWeapon
{
	// Token: 0x04000688 RID: 1672
	public float stringHoldDurationMax = 3f;

	// Token: 0x04000689 RID: 1673
	public float stringBonusDamage = 1f;

	// Token: 0x0400068A RID: 1674
	public float stringBonusDistance = 0.5f;

	// Token: 0x0400068B RID: 1675
	public float stringBonusVelocity = 1f;

	// Token: 0x0400068C RID: 1676
	public float movementPenaltyRampUpTime = 0.5f;

	// Token: 0x0400068D RID: 1677
	public float conditionLossPerSecondHeld = 1f;

	// Token: 0x0400068E RID: 1678
	public float conditionLossHeldDelay = 3f;

	// Token: 0x0400068F RID: 1679
	public SoundDefinition chargeUpSoundDef;

	// Token: 0x04000690 RID: 1680
	public SoundDefinition stringHeldSoundDef;

	// Token: 0x04000691 RID: 1681
	public SoundDefinition drawFinishSoundDef;

	// Token: 0x04000692 RID: 1682
	private Sound chargeUpSound;

	// Token: 0x04000693 RID: 1683
	private Sound stringHeldSound;

	// Token: 0x04000694 RID: 1684
	protected float movementPenalty;

	// Token: 0x04000695 RID: 1685
	internal float stringHoldTimeStart;

	// Token: 0x04000696 RID: 1686
	protected float conditionLossCheckTickRate = 0.5f;

	// Token: 0x04000697 RID: 1687
	protected float serverMovementCheckTickRate = 0.1f;

	// Token: 0x060009CF RID: 2511 RVA: 0x0005C410 File Offset: 0x0005A610
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("CompoundBowWeapon.OnRpcMessage", 0))
		{
			if (rpc == 618693016U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_StringHoldStatus ");
				}
				using (TimeWarning.New("RPC_StringHoldStatus", 0))
				{
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
							this.RPC_StringHoldStatus(msg2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in RPC_StringHoldStatus");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x060009D0 RID: 2512 RVA: 0x0005C534 File Offset: 0x0005A734
	public void UpdateMovementPenalty(float delta)
	{
		BasePlayer ownerPlayer = base.GetOwnerPlayer();
		bool flag = false;
		if (base.isServer)
		{
			if (ownerPlayer == null)
			{
				return;
			}
			flag = (ownerPlayer.estimatedSpeed > 0.1f);
		}
		if (flag)
		{
			this.movementPenalty += delta * (1f / this.movementPenaltyRampUpTime);
		}
		else
		{
			this.movementPenalty -= delta * (1f / this.stringHoldDurationMax);
		}
		this.movementPenalty = Mathf.Clamp01(this.movementPenalty);
	}

	// Token: 0x060009D1 RID: 2513 RVA: 0x0005C5B8 File Offset: 0x0005A7B8
	public void UpdateConditionLoss()
	{
		if (this.stringHoldTimeStart != 0f && UnityEngine.Time.time - this.stringHoldTimeStart > this.conditionLossHeldDelay && this.GetStringBonusScale() > 0f)
		{
			Item ownerItem = base.GetOwnerItem();
			if (ownerItem == null)
			{
				return;
			}
			ownerItem.LoseCondition(this.conditionLossCheckTickRate * this.conditionLossPerSecondHeld);
		}
	}

	// Token: 0x060009D2 RID: 2514 RVA: 0x0005C616 File Offset: 0x0005A816
	public void ServerMovementCheck()
	{
		this.UpdateMovementPenalty(this.serverMovementCheckTickRate);
	}

	// Token: 0x060009D3 RID: 2515 RVA: 0x0005C624 File Offset: 0x0005A824
	public override void OnHeldChanged()
	{
		base.OnHeldChanged();
		if (base.IsDisabled())
		{
			base.CancelInvoke(new Action(this.ServerMovementCheck));
			base.CancelInvoke(new Action(this.UpdateConditionLoss));
			return;
		}
		base.InvokeRepeating(new Action(this.ServerMovementCheck), 0f, this.serverMovementCheckTickRate);
		base.InvokeRepeating(new Action(this.UpdateConditionLoss), 0f, this.conditionLossCheckTickRate);
	}

	// Token: 0x060009D4 RID: 2516 RVA: 0x0005C69E File Offset: 0x0005A89E
	[BaseEntity.RPC_Server]
	public void RPC_StringHoldStatus(BaseEntity.RPCMessage msg)
	{
		if (msg.read.Bit())
		{
			this.stringHoldTimeStart = UnityEngine.Time.time;
			return;
		}
		this.stringHoldTimeStart = 0f;
	}

	// Token: 0x060009D5 RID: 2517 RVA: 0x0005C6C4 File Offset: 0x0005A8C4
	public override void DidAttackServerside()
	{
		base.DidAttackServerside();
		this.stringHoldTimeStart = 0f;
	}

	// Token: 0x060009D6 RID: 2518 RVA: 0x0005C6D7 File Offset: 0x0005A8D7
	public float GetLastPlayerMovementTime()
	{
		bool isServer = base.isServer;
		return 0f;
	}

	// Token: 0x060009D7 RID: 2519 RVA: 0x0005C6E5 File Offset: 0x0005A8E5
	public float GetStringBonusScale()
	{
		if (this.stringHoldTimeStart == 0f)
		{
			return 0f;
		}
		return Mathf.Clamp01(Mathf.Clamp01((UnityEngine.Time.time - this.stringHoldTimeStart) / this.stringHoldDurationMax) - this.movementPenalty);
	}

	// Token: 0x060009D8 RID: 2520 RVA: 0x0005C720 File Offset: 0x0005A920
	public override float GetDamageScale(bool getMax = false)
	{
		float num = getMax ? 1f : this.GetStringBonusScale();
		return this.damageScale + this.stringBonusDamage * num;
	}

	// Token: 0x060009D9 RID: 2521 RVA: 0x0005C750 File Offset: 0x0005A950
	public override float GetDistanceScale(bool getMax = false)
	{
		float num = getMax ? 1f : this.GetStringBonusScale();
		return this.distanceScale + this.stringBonusDistance * num;
	}

	// Token: 0x060009DA RID: 2522 RVA: 0x0005C780 File Offset: 0x0005A980
	public override float GetProjectileVelocityScale(bool getMax = false)
	{
		float num = getMax ? 1f : this.GetStringBonusScale();
		return this.projectileVelocityScale + this.stringBonusVelocity * num;
	}
}
