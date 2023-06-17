using System;
using UnityEngine;

// Token: 0x020004D9 RID: 1241
public class ProgressDoor : IOEntity
{
	// Token: 0x0400206E RID: 8302
	public float storedEnergy;

	// Token: 0x0400206F RID: 8303
	public float energyForOpen = 1f;

	// Token: 0x04002070 RID: 8304
	public float secondsToClose = 1f;

	// Token: 0x04002071 RID: 8305
	public float openProgress;

	// Token: 0x06002835 RID: 10293 RVA: 0x000F8B8E File Offset: 0x000F6D8E
	public override void ResetIOState()
	{
		this.storedEnergy = 0f;
		this.UpdateProgress();
	}

	// Token: 0x06002836 RID: 10294 RVA: 0x000F8BA1 File Offset: 0x000F6DA1
	public override float IOInput(IOEntity from, IOEntity.IOType inputType, float inputAmount, int slot = 0)
	{
		if (inputAmount <= 0f)
		{
			this.NoEnergy();
			return inputAmount;
		}
		this.AddEnergy(inputAmount);
		if (this.storedEnergy == this.energyForOpen)
		{
			return inputAmount;
		}
		return 0f;
	}

	// Token: 0x06002837 RID: 10295 RVA: 0x000063A5 File Offset: 0x000045A5
	public virtual void NoEnergy()
	{
	}

	// Token: 0x06002838 RID: 10296 RVA: 0x000F8BCF File Offset: 0x000F6DCF
	public virtual void AddEnergy(float amount)
	{
		if (amount <= 0f)
		{
			return;
		}
		this.storedEnergy += amount;
		this.storedEnergy = Mathf.Clamp(this.storedEnergy, 0f, this.energyForOpen);
	}

	// Token: 0x06002839 RID: 10297 RVA: 0x00007D00 File Offset: 0x00005F00
	public virtual void UpdateProgress()
	{
		base.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
	}
}
