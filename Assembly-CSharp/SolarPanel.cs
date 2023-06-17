using System;
using UnityEngine;

// Token: 0x0200011E RID: 286
public class SolarPanel : IOEntity
{
	// Token: 0x04000E96 RID: 3734
	public Transform sunSampler;

	// Token: 0x04000E97 RID: 3735
	private const int tickrateSeconds = 60;

	// Token: 0x04000E98 RID: 3736
	public int maximalPowerOutput = 10;

	// Token: 0x04000E99 RID: 3737
	public float dot_minimum = 0.1f;

	// Token: 0x04000E9A RID: 3738
	public float dot_maximum = 0.6f;

	// Token: 0x06001671 RID: 5745 RVA: 0x0000441C File Offset: 0x0000261C
	public override bool IsRootEntity()
	{
		return true;
	}

	// Token: 0x06001672 RID: 5746 RVA: 0x000ADC4B File Offset: 0x000ABE4B
	public override int MaximalPowerOutput()
	{
		return this.maximalPowerOutput;
	}

	// Token: 0x06001673 RID: 5747 RVA: 0x00007A3C File Offset: 0x00005C3C
	public override int ConsumptionAmount()
	{
		return 0;
	}

	// Token: 0x06001674 RID: 5748 RVA: 0x000ADC53 File Offset: 0x000ABE53
	public override void ServerInit()
	{
		base.ServerInit();
		base.InvokeRandomized(new Action(this.SunUpdate), 1f, 5f, 2f);
	}

	// Token: 0x06001675 RID: 5749 RVA: 0x000ADC7C File Offset: 0x000ABE7C
	public void SunUpdate()
	{
		int num = this.currentEnergy;
		if (TOD_Sky.Instance.IsNight)
		{
			num = 0;
		}
		else
		{
			Vector3 sunDirection = TOD_Sky.Instance.SunDirection;
			float value = Vector3.Dot(this.sunSampler.transform.forward, sunDirection);
			float num2 = Mathf.InverseLerp(this.dot_minimum, this.dot_maximum, value);
			if (num2 > 0f && !base.IsVisible(this.sunSampler.transform.position + sunDirection * 100f, 101f))
			{
				num2 = 0f;
			}
			num = Mathf.FloorToInt((float)this.maximalPowerOutput * num2 * base.healthFraction);
		}
		bool flag = this.currentEnergy != num;
		this.currentEnergy = num;
		if (flag)
		{
			this.MarkDirty();
		}
	}

	// Token: 0x06001676 RID: 5750 RVA: 0x0006647B File Offset: 0x0006467B
	public override int GetPassthroughAmount(int outputSlot = 0)
	{
		if (outputSlot != 0)
		{
			return 0;
		}
		return this.currentEnergy;
	}
}
