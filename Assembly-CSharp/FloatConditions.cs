using System;

// Token: 0x02000932 RID: 2354
[Serializable]
public class FloatConditions
{
	// Token: 0x04003342 RID: 13122
	public FloatConditions.Condition[] conditions;

	// Token: 0x06003885 RID: 14469 RVA: 0x0015135C File Offset: 0x0014F55C
	public bool AllTrue(float val)
	{
		foreach (FloatConditions.Condition condition in this.conditions)
		{
			if (!condition.Test(val))
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x02000EBB RID: 3771
	[Serializable]
	public struct Condition
	{
		// Token: 0x04004CC0 RID: 19648
		public FloatConditions.Condition.Types type;

		// Token: 0x04004CC1 RID: 19649
		public float value;

		// Token: 0x06005331 RID: 21297 RVA: 0x001B1D84 File Offset: 0x001AFF84
		public bool Test(float val)
		{
			switch (this.type)
			{
			case FloatConditions.Condition.Types.Equal:
				return val == this.value;
			case FloatConditions.Condition.Types.NotEqual:
				return val != this.value;
			case FloatConditions.Condition.Types.Higher:
				return val > this.value;
			case FloatConditions.Condition.Types.Lower:
				return val < this.value;
			default:
				return false;
			}
		}

		// Token: 0x02000FD6 RID: 4054
		public enum Types
		{
			// Token: 0x04005104 RID: 20740
			Equal,
			// Token: 0x04005105 RID: 20741
			NotEqual,
			// Token: 0x04005106 RID: 20742
			Higher,
			// Token: 0x04005107 RID: 20743
			Lower
		}
	}
}
