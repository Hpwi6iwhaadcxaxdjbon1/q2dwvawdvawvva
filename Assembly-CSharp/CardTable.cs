using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200040C RID: 1036
public class CardTable : BaseCardGameEntity
{
	// Token: 0x04001B2F RID: 6959
	[Header("Card Table")]
	[SerializeField]
	private ViewModel viewModel;

	// Token: 0x04001B30 RID: 6960
	[SerializeField]
	private CardGameUI.PlayingCardImage[] tableCards;

	// Token: 0x04001B31 RID: 6961
	[SerializeField]
	private Renderer[] tableCardBackings;

	// Token: 0x04001B32 RID: 6962
	[SerializeField]
	private Canvas cardUICanvas;

	// Token: 0x04001B33 RID: 6963
	[SerializeField]
	private Image[] tableCardImages;

	// Token: 0x04001B34 RID: 6964
	[SerializeField]
	private Sprite blankCard;

	// Token: 0x04001B35 RID: 6965
	[SerializeField]
	private CardTable.ChipStack[] chipStacks;

	// Token: 0x04001B36 RID: 6966
	[SerializeField]
	private CardTable.ChipStack[] fillerStacks;

	// Token: 0x06002327 RID: 8999 RVA: 0x000E0A24 File Offset: 0x000DEC24
	public override void Load(BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
	}

	// Token: 0x170002F1 RID: 753
	// (get) Token: 0x06002328 RID: 9000 RVA: 0x00006CA5 File Offset: 0x00004EA5
	protected override float MaxStorageInteractionDist
	{
		get
		{
			return 1f;
		}
	}

	// Token: 0x02000CD7 RID: 3287
	[Serializable]
	public class ChipStack : IComparable<CardTable.ChipStack>
	{
		// Token: 0x0400451E RID: 17694
		public int chipValue;

		// Token: 0x0400451F RID: 17695
		public GameObject[] chips;

		// Token: 0x06004FC1 RID: 20417 RVA: 0x001A70B5 File Offset: 0x001A52B5
		public int CompareTo(CardTable.ChipStack other)
		{
			if (other == null)
			{
				return 1;
			}
			return this.chipValue.CompareTo(other.chipValue);
		}
	}
}
