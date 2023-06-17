using System;
using UnityEngine;

// Token: 0x02000400 RID: 1024
[Serializable]
public class MetabolismAttribute
{
	// Token: 0x04001AEB RID: 6891
	public float startMin;

	// Token: 0x04001AEC RID: 6892
	public float startMax;

	// Token: 0x04001AED RID: 6893
	public float min;

	// Token: 0x04001AEE RID: 6894
	public float max;

	// Token: 0x04001AEF RID: 6895
	public float value;

	// Token: 0x04001AF0 RID: 6896
	internal float lastValue;

	// Token: 0x04001AF1 RID: 6897
	internal float lastGreatFraction;

	// Token: 0x04001AF2 RID: 6898
	private const float greatInterval = 0.1f;

	// Token: 0x170002EE RID: 750
	// (get) Token: 0x060022E1 RID: 8929 RVA: 0x000DF971 File Offset: 0x000DDB71
	public float greatFraction
	{
		get
		{
			return Mathf.Floor(this.Fraction() / 0.1f) / 10f;
		}
	}

	// Token: 0x060022E2 RID: 8930 RVA: 0x000DF98A File Offset: 0x000DDB8A
	public void Reset()
	{
		this.value = Mathf.Clamp(UnityEngine.Random.Range(this.startMin, this.startMax), this.min, this.max);
	}

	// Token: 0x060022E3 RID: 8931 RVA: 0x000DF9B4 File Offset: 0x000DDBB4
	public float Fraction()
	{
		return Mathf.InverseLerp(this.min, this.max, this.value);
	}

	// Token: 0x060022E4 RID: 8932 RVA: 0x000DF9CD File Offset: 0x000DDBCD
	public float InverseFraction()
	{
		return 1f - this.Fraction();
	}

	// Token: 0x060022E5 RID: 8933 RVA: 0x000DF9DB File Offset: 0x000DDBDB
	public void Add(float val)
	{
		this.value = Mathf.Clamp(this.value + val, this.min, this.max);
	}

	// Token: 0x060022E6 RID: 8934 RVA: 0x000DF9FC File Offset: 0x000DDBFC
	public void Subtract(float val)
	{
		this.value = Mathf.Clamp(this.value - val, this.min, this.max);
	}

	// Token: 0x060022E7 RID: 8935 RVA: 0x000DFA1D File Offset: 0x000DDC1D
	public void Increase(float fTarget)
	{
		fTarget = Mathf.Clamp(fTarget, this.min, this.max);
		if (fTarget <= this.value)
		{
			return;
		}
		this.value = fTarget;
	}

	// Token: 0x060022E8 RID: 8936 RVA: 0x000DFA44 File Offset: 0x000DDC44
	public void MoveTowards(float fTarget, float fRate)
	{
		if (fRate == 0f)
		{
			return;
		}
		this.value = Mathf.Clamp(Mathf.MoveTowards(this.value, fTarget, fRate), this.min, this.max);
	}

	// Token: 0x060022E9 RID: 8937 RVA: 0x000DFA73 File Offset: 0x000DDC73
	public bool HasChanged()
	{
		bool result = this.lastValue != this.value;
		this.lastValue = this.value;
		return result;
	}

	// Token: 0x060022EA RID: 8938 RVA: 0x000DFA94 File Offset: 0x000DDC94
	public bool HasGreatlyChanged()
	{
		float greatFraction = this.greatFraction;
		bool result = this.lastGreatFraction != greatFraction;
		this.lastGreatFraction = greatFraction;
		return result;
	}

	// Token: 0x060022EB RID: 8939 RVA: 0x000DFABB File Offset: 0x000DDCBB
	public void SetValue(float newValue)
	{
		this.value = newValue;
	}

	// Token: 0x02000CD2 RID: 3282
	public enum Type
	{
		// Token: 0x040044F7 RID: 17655
		Calories,
		// Token: 0x040044F8 RID: 17656
		Hydration,
		// Token: 0x040044F9 RID: 17657
		Heartrate,
		// Token: 0x040044FA RID: 17658
		Poison,
		// Token: 0x040044FB RID: 17659
		Radiation,
		// Token: 0x040044FC RID: 17660
		Bleeding,
		// Token: 0x040044FD RID: 17661
		Health,
		// Token: 0x040044FE RID: 17662
		HealthOverTime
	}
}
