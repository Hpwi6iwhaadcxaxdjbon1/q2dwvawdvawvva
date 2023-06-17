using System;
using UnityEngine;

// Token: 0x0200013E RID: 318
[CreateAssetMenu(menuName = "Rust/Slot Machine Payouts")]
public class SlotMachinePayoutSettings : ScriptableObject
{
	// Token: 0x04000F36 RID: 3894
	public ItemAmount SpinCost;

	// Token: 0x04000F37 RID: 3895
	public SlotMachinePayoutSettings.PayoutInfo[] Payouts;

	// Token: 0x04000F38 RID: 3896
	public int[] VirtualFaces = new int[16];

	// Token: 0x04000F39 RID: 3897
	public SlotMachinePayoutSettings.IndividualPayouts[] FacePayouts = new SlotMachinePayoutSettings.IndividualPayouts[0];

	// Token: 0x04000F3A RID: 3898
	public int TotalStops;

	// Token: 0x04000F3B RID: 3899
	public GameObjectRef DefaultWinEffect;

	// Token: 0x02000C25 RID: 3109
	[Serializable]
	public struct PayoutInfo
	{
		// Token: 0x04004245 RID: 16965
		public ItemAmount Item;

		// Token: 0x04004246 RID: 16966
		[Range(0f, 15f)]
		public int Result1;

		// Token: 0x04004247 RID: 16967
		[Range(0f, 15f)]
		public int Result2;

		// Token: 0x04004248 RID: 16968
		[Range(0f, 15f)]
		public int Result3;

		// Token: 0x04004249 RID: 16969
		public GameObjectRef OverrideWinEffect;
	}

	// Token: 0x02000C26 RID: 3110
	[Serializable]
	public struct IndividualPayouts
	{
		// Token: 0x0400424A RID: 16970
		public ItemAmount Item;

		// Token: 0x0400424B RID: 16971
		[Range(0f, 15f)]
		public int Result;
	}
}
