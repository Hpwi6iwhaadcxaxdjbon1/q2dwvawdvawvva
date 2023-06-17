using System;
using System.Linq;
using Rust;
using UnityEngine;

// Token: 0x02000123 RID: 291
public class TeslaCoil : IOEntity
{
	// Token: 0x04000EB2 RID: 3762
	public TargetTrigger targetTrigger;

	// Token: 0x04000EB3 RID: 3763
	public TriggerMovement movementTrigger;

	// Token: 0x04000EB4 RID: 3764
	public float powerToDamageRatio = 2f;

	// Token: 0x04000EB5 RID: 3765
	public float dischargeTickRate = 0.25f;

	// Token: 0x04000EB6 RID: 3766
	public float maxDischargeSelfDamageSeconds = 120f;

	// Token: 0x04000EB7 RID: 3767
	public float maxDamageOutput = 35f;

	// Token: 0x04000EB8 RID: 3768
	public Transform damageEyes;

	// Token: 0x04000EB9 RID: 3769
	public const BaseEntity.Flags Flag_WeakShorting = BaseEntity.Flags.Reserved1;

	// Token: 0x04000EBA RID: 3770
	public const BaseEntity.Flags Flag_StrongShorting = BaseEntity.Flags.Reserved2;

	// Token: 0x04000EBB RID: 3771
	public int powerForHeavyShorting = 10;

	// Token: 0x04000EBC RID: 3772
	private float lastDischargeTime;

	// Token: 0x0600168B RID: 5771 RVA: 0x000AE32F File Offset: 0x000AC52F
	public override int ConsumptionAmount()
	{
		return Mathf.CeilToInt(this.maxDamageOutput / this.powerToDamageRatio);
	}

	// Token: 0x0600168C RID: 5772 RVA: 0x000AE343 File Offset: 0x000AC543
	public bool CanDischarge()
	{
		return base.healthFraction >= 0.25f;
	}

	// Token: 0x0600168D RID: 5773 RVA: 0x000AE358 File Offset: 0x000AC558
	public override void UpdateFromInput(int inputAmount, int inputSlot)
	{
		base.UpdateFromInput(inputAmount, inputSlot);
		if (inputAmount > 0 && this.CanDischarge())
		{
			float num = Time.time - this.lastDischargeTime;
			if (num < 0f)
			{
				num = 0f;
			}
			float time = Mathf.Min(this.dischargeTickRate - num, this.dischargeTickRate);
			base.InvokeRepeating(new Action(this.Discharge), time, this.dischargeTickRate);
			base.SetFlag(BaseEntity.Flags.Reserved1, inputAmount < this.powerForHeavyShorting, false, false);
			base.SetFlag(BaseEntity.Flags.Reserved2, inputAmount >= this.powerForHeavyShorting, false, false);
			base.SetFlag(BaseEntity.Flags.On, true, false, true);
			return;
		}
		base.CancelInvoke(new Action(this.Discharge));
		base.SetFlag(BaseEntity.Flags.Reserved1, false, false, false);
		base.SetFlag(BaseEntity.Flags.Reserved2, false, false, false);
		base.SetFlag(BaseEntity.Flags.On, false, false, true);
	}

	// Token: 0x0600168E RID: 5774 RVA: 0x000AE43C File Offset: 0x000AC63C
	public void Discharge()
	{
		float damageAmount = Mathf.Clamp((float)this.currentEnergy * this.powerToDamageRatio, 0f, this.maxDamageOutput) * this.dischargeTickRate;
		this.lastDischargeTime = Time.time;
		if (this.targetTrigger.entityContents != null)
		{
			BaseEntity[] array = this.targetTrigger.entityContents.ToArray<BaseEntity>();
			if (array != null)
			{
				BaseEntity[] array2 = array;
				for (int i = 0; i < array2.Length; i++)
				{
					BaseCombatEntity component = array2[i].GetComponent<BaseCombatEntity>();
					if (component && component.IsVisible(this.damageEyes.transform.position, component.CenterPoint(), float.PositiveInfinity))
					{
						component.OnAttacked(new HitInfo(this, component, DamageType.ElectricShock, damageAmount));
					}
				}
			}
		}
		float amount = this.dischargeTickRate / this.maxDischargeSelfDamageSeconds * this.MaxHealth();
		base.Hurt(amount, DamageType.ElectricShock, this, false);
		if (!this.CanDischarge())
		{
			this.MarkDirty();
		}
	}
}
