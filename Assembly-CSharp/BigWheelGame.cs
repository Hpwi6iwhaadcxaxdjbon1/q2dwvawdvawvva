using System;
using System.Collections.Generic;
using System.Linq;
using Facepunch.Rust;
using UnityEngine;

// Token: 0x02000139 RID: 313
public class BigWheelGame : SpinnerWheel
{
	// Token: 0x04000F27 RID: 3879
	public HitNumber[] hitNumbers;

	// Token: 0x04000F28 RID: 3880
	public GameObject indicator;

	// Token: 0x04000F29 RID: 3881
	public GameObjectRef winEffect;

	// Token: 0x04000F2A RID: 3882
	[ServerVar]
	public static float spinFrequencySeconds = 45f;

	// Token: 0x04000F2B RID: 3883
	protected int spinNumber;

	// Token: 0x04000F2C RID: 3884
	protected int lastPaidSpinNumber = -1;

	// Token: 0x04000F2D RID: 3885
	protected List<BigWheelBettingTerminal> terminals = new List<BigWheelBettingTerminal>();

	// Token: 0x060016D5 RID: 5845 RVA: 0x00007A3C File Offset: 0x00005C3C
	public override bool AllowPlayerSpins()
	{
		return false;
	}

	// Token: 0x060016D6 RID: 5846 RVA: 0x00007A3C File Offset: 0x00005C3C
	public override bool CanUpdateSign(BasePlayer player)
	{
		return false;
	}

	// Token: 0x060016D7 RID: 5847 RVA: 0x000AF3A3 File Offset: 0x000AD5A3
	public override float GetMaxSpinSpeed()
	{
		return 180f;
	}

	// Token: 0x060016D8 RID: 5848 RVA: 0x000AF3AA File Offset: 0x000AD5AA
	public override void ServerInit()
	{
		base.ServerInit();
		base.Invoke(new Action(this.InitBettingTerminals), 3f);
		base.Invoke(new Action(this.DoSpin), 10f);
	}

	// Token: 0x060016D9 RID: 5849 RVA: 0x000AF3E0 File Offset: 0x000AD5E0
	public void DoSpin()
	{
		if (this.velocity > 0f)
		{
			return;
		}
		this.velocity += UnityEngine.Random.Range(7f, 16f);
		this.spinNumber++;
		this.SetTerminalsLocked(true);
	}

	// Token: 0x060016DA RID: 5850 RVA: 0x000AF42C File Offset: 0x000AD62C
	public void SetTerminalsLocked(bool isLocked)
	{
		foreach (BigWheelBettingTerminal bigWheelBettingTerminal in this.terminals)
		{
			bigWheelBettingTerminal.inventory.SetLocked(isLocked);
		}
	}

	// Token: 0x060016DB RID: 5851 RVA: 0x000AF484 File Offset: 0x000AD684
	public void RemoveTerminal(BigWheelBettingTerminal terminal)
	{
		this.terminals.Remove(terminal);
	}

	// Token: 0x060016DC RID: 5852 RVA: 0x000AF494 File Offset: 0x000AD694
	protected void InitBettingTerminals()
	{
		this.terminals.Clear();
		Vis.Entities<BigWheelBettingTerminal>(base.transform.position, 30f, this.terminals, 256, QueryTriggerInteraction.Collide);
		this.terminals = this.terminals.Distinct<BigWheelBettingTerminal>().ToList<BigWheelBettingTerminal>();
	}

	// Token: 0x060016DD RID: 5853 RVA: 0x000AF4E4 File Offset: 0x000AD6E4
	public override void Update_Server()
	{
		float velocity = this.velocity;
		base.Update_Server();
		float velocity2 = this.velocity;
		if (velocity > 0f && velocity2 == 0f && this.spinNumber > this.lastPaidSpinNumber)
		{
			this.Payout();
			this.lastPaidSpinNumber = this.spinNumber;
			this.QueueSpin();
		}
	}

	// Token: 0x060016DE RID: 5854 RVA: 0x000AF539 File Offset: 0x000AD739
	public float SpinSpacing()
	{
		return BigWheelGame.spinFrequencySeconds;
	}

	// Token: 0x060016DF RID: 5855 RVA: 0x000AF540 File Offset: 0x000AD740
	public void QueueSpin()
	{
		foreach (BigWheelBettingTerminal bigWheelBettingTerminal in this.terminals)
		{
			bigWheelBettingTerminal.ClientRPC<float>(null, "SetTimeUntilNextSpin", this.SpinSpacing());
		}
		base.Invoke(new Action(this.DoSpin), this.SpinSpacing());
	}

	// Token: 0x060016E0 RID: 5856 RVA: 0x000AF5B4 File Offset: 0x000AD7B4
	public void Payout()
	{
		HitNumber currentHitType = this.GetCurrentHitType();
		Guid value = Guid.NewGuid();
		foreach (BigWheelBettingTerminal bigWheelBettingTerminal in this.terminals)
		{
			if (!bigWheelBettingTerminal.isClient)
			{
				bool flag = false;
				bool flag2 = false;
				Item slot = bigWheelBettingTerminal.inventory.GetSlot((int)currentHitType.hitType);
				if (slot != null)
				{
					int num = currentHitType.ColorToMultiplier(currentHitType.hitType);
					int amount = slot.amount;
					slot.amount += slot.amount * num;
					slot.RemoveFromContainer();
					slot.MoveToContainer(bigWheelBettingTerminal.inventory, 5, true, false, null, true);
					flag = true;
					Analytics.Azure.OnGamblingResult(bigWheelBettingTerminal.lastPlayer, bigWheelBettingTerminal, amount, slot.amount, new Guid?(value));
				}
				for (int i = 0; i < 5; i++)
				{
					Item slot2 = bigWheelBettingTerminal.inventory.GetSlot(i);
					if (slot2 != null)
					{
						Analytics.Azure.OnGamblingResult(bigWheelBettingTerminal.lastPlayer, bigWheelBettingTerminal, slot2.amount, 0, new Guid?(value));
						slot2.Remove(0f);
						flag2 = true;
					}
				}
				if (flag || flag2)
				{
					bigWheelBettingTerminal.ClientRPC<bool>(null, "WinOrLoseSound", flag);
				}
			}
		}
		ItemManager.DoRemoves();
		this.SetTerminalsLocked(false);
	}

	// Token: 0x060016E1 RID: 5857 RVA: 0x000AF71C File Offset: 0x000AD91C
	public HitNumber GetCurrentHitType()
	{
		HitNumber result = null;
		float num = float.PositiveInfinity;
		foreach (HitNumber hitNumber in this.hitNumbers)
		{
			float num2 = Vector3.Distance(this.indicator.transform.position, hitNumber.transform.position);
			if (num2 < num)
			{
				result = hitNumber;
				num = num2;
			}
		}
		return result;
	}

	// Token: 0x060016E2 RID: 5858 RVA: 0x000AF77C File Offset: 0x000AD97C
	[ContextMenu("LoadHitNumbers")]
	private void LoadHitNumbers()
	{
		HitNumber[] componentsInChildren = base.GetComponentsInChildren<HitNumber>();
		this.hitNumbers = componentsInChildren;
	}
}
