using System;

// Token: 0x020003AB RID: 939
public static class FishStateExtensions
{
	// Token: 0x06002106 RID: 8454 RVA: 0x000D9276 File Offset: 0x000D7476
	public static bool Contains(this BaseFishingRod.FishState state, BaseFishingRod.FishState check)
	{
		return (state & check) == check;
	}

	// Token: 0x06002107 RID: 8455 RVA: 0x000D927E File Offset: 0x000D747E
	public static BaseFishingRod.FishState FlipHorizontal(this BaseFishingRod.FishState state)
	{
		if (state.Contains(BaseFishingRod.FishState.PullingLeft))
		{
			state |= BaseFishingRod.FishState.PullingRight;
			state &= ~BaseFishingRod.FishState.PullingLeft;
		}
		else if (state.Contains(BaseFishingRod.FishState.PullingRight))
		{
			state |= BaseFishingRod.FishState.PullingLeft;
			state &= ~BaseFishingRod.FishState.PullingRight;
		}
		return state;
	}
}
