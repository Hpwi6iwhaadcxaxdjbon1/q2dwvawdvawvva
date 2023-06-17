using System;
using UnityEngine;

// Token: 0x0200040B RID: 1035
public class CardGameSounds : PrefabAttribute
{
	// Token: 0x04001B1B RID: 6939
	public SoundDefinition ChipsSfx;

	// Token: 0x04001B1C RID: 6940
	public SoundDefinition DrawSfx;

	// Token: 0x04001B1D RID: 6941
	public SoundDefinition PlaySfx;

	// Token: 0x04001B1E RID: 6942
	public SoundDefinition ShuffleSfx;

	// Token: 0x04001B1F RID: 6943
	public SoundDefinition WinSfx;

	// Token: 0x04001B20 RID: 6944
	public SoundDefinition LoseSfx;

	// Token: 0x04001B21 RID: 6945
	public SoundDefinition YourTurnSfx;

	// Token: 0x04001B22 RID: 6946
	public SoundDefinition CheckSfx;

	// Token: 0x04001B23 RID: 6947
	public SoundDefinition HitSfx;

	// Token: 0x04001B24 RID: 6948
	public SoundDefinition StandSfx;

	// Token: 0x04001B25 RID: 6949
	public SoundDefinition BetSfx;

	// Token: 0x04001B26 RID: 6950
	public SoundDefinition IncreaseBetSfx;

	// Token: 0x04001B27 RID: 6951
	public SoundDefinition DecreaseBetSfx;

	// Token: 0x04001B28 RID: 6952
	public SoundDefinition AllInSfx;

	// Token: 0x04001B29 RID: 6953
	public SoundDefinition UIInteractSfx;

	// Token: 0x04001B2A RID: 6954
	[Header("Dealer Reactions")]
	public SoundDefinition DealerCoolSfx;

	// Token: 0x04001B2B RID: 6955
	public SoundDefinition DealerHappySfx;

	// Token: 0x04001B2C RID: 6956
	public SoundDefinition DealerLoveSfx;

	// Token: 0x04001B2D RID: 6957
	public SoundDefinition DealerSadSfx;

	// Token: 0x04001B2E RID: 6958
	public SoundDefinition DealerShockedSfx;

	// Token: 0x06002324 RID: 8996 RVA: 0x000E0B33 File Offset: 0x000DED33
	protected override Type GetIndexedType()
	{
		return typeof(CardGameSounds);
	}

	// Token: 0x06002325 RID: 8997 RVA: 0x000E0B40 File Offset: 0x000DED40
	public void PlaySound(CardGameSounds.SoundType sound, GameObject forGameObject)
	{
		switch (sound)
		{
		case CardGameSounds.SoundType.Chips:
			this.ChipsSfx.Play(forGameObject);
			return;
		case CardGameSounds.SoundType.Draw:
			this.DrawSfx.Play(forGameObject);
			return;
		case CardGameSounds.SoundType.Play:
			this.PlaySfx.Play(forGameObject);
			return;
		case CardGameSounds.SoundType.Shuffle:
			this.ShuffleSfx.Play(forGameObject);
			return;
		case CardGameSounds.SoundType.Win:
			this.WinSfx.Play(forGameObject);
			return;
		case CardGameSounds.SoundType.YourTurn:
			this.YourTurnSfx.Play(forGameObject);
			return;
		case CardGameSounds.SoundType.Check:
			this.CheckSfx.Play(forGameObject);
			return;
		case CardGameSounds.SoundType.Hit:
			this.HitSfx.Play(forGameObject);
			return;
		case CardGameSounds.SoundType.Stand:
			this.StandSfx.Play(forGameObject);
			return;
		case CardGameSounds.SoundType.Bet:
			this.BetSfx.Play(forGameObject);
			return;
		case CardGameSounds.SoundType.IncreaseBet:
			this.IncreaseBetSfx.Play(forGameObject);
			return;
		case CardGameSounds.SoundType.DecreaseBet:
			this.DecreaseBetSfx.Play(forGameObject);
			return;
		case CardGameSounds.SoundType.AllIn:
			this.AllInSfx.Play(forGameObject);
			return;
		case CardGameSounds.SoundType.UIInteract:
			this.UIInteractSfx.Play(forGameObject);
			return;
		case CardGameSounds.SoundType.DealerCool:
			this.DealerCoolSfx.Play(forGameObject);
			return;
		case CardGameSounds.SoundType.DealerHappy:
			this.DealerHappySfx.Play(forGameObject);
			return;
		case CardGameSounds.SoundType.DealerLove:
			this.DealerLoveSfx.Play(forGameObject);
			return;
		case CardGameSounds.SoundType.DealerSad:
			this.DealerSadSfx.Play(forGameObject);
			return;
		case CardGameSounds.SoundType.DealerShocked:
			this.DealerShockedSfx.Play(forGameObject);
			return;
		default:
			throw new ArgumentOutOfRangeException("sound", sound, null);
		}
	}

	// Token: 0x02000CD6 RID: 3286
	public enum SoundType
	{
		// Token: 0x0400450B RID: 17675
		Chips,
		// Token: 0x0400450C RID: 17676
		Draw,
		// Token: 0x0400450D RID: 17677
		Play,
		// Token: 0x0400450E RID: 17678
		Shuffle,
		// Token: 0x0400450F RID: 17679
		Win,
		// Token: 0x04004510 RID: 17680
		YourTurn,
		// Token: 0x04004511 RID: 17681
		Check,
		// Token: 0x04004512 RID: 17682
		Hit,
		// Token: 0x04004513 RID: 17683
		Stand,
		// Token: 0x04004514 RID: 17684
		Bet,
		// Token: 0x04004515 RID: 17685
		IncreaseBet,
		// Token: 0x04004516 RID: 17686
		DecreaseBet,
		// Token: 0x04004517 RID: 17687
		AllIn,
		// Token: 0x04004518 RID: 17688
		UIInteract,
		// Token: 0x04004519 RID: 17689
		DealerCool,
		// Token: 0x0400451A RID: 17690
		DealerHappy,
		// Token: 0x0400451B RID: 17691
		DealerLove,
		// Token: 0x0400451C RID: 17692
		DealerSad,
		// Token: 0x0400451D RID: 17693
		DealerShocked
	}
}
