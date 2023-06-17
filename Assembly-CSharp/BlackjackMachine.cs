using System;
using Facepunch.CardGames;
using UnityEngine;

// Token: 0x02000409 RID: 1033
public class BlackjackMachine : BaseCardGameEntity
{
	// Token: 0x04001B12 RID: 6930
	[Header("Blackjack Machine")]
	[SerializeField]
	private GameObjectRef mainScreenPrefab;

	// Token: 0x04001B13 RID: 6931
	[SerializeField]
	private GameObjectRef smallScreenPrefab;

	// Token: 0x04001B14 RID: 6932
	[SerializeField]
	private Transform mainScreenParent;

	// Token: 0x04001B15 RID: 6933
	[SerializeField]
	private Transform[] smallScreenParents;

	// Token: 0x04001B16 RID: 6934
	private static int _maxbet = 500;

	// Token: 0x04001B17 RID: 6935
	private BlackjackController controller;

	// Token: 0x04001B18 RID: 6936
	private BlackjackMainScreenUI mainScreenUI;

	// Token: 0x04001B19 RID: 6937
	private BlackjackSmallScreenUI[] smallScreenUIs = new BlackjackSmallScreenUI[3];

	// Token: 0x170002EF RID: 751
	// (get) Token: 0x06002316 RID: 8982 RVA: 0x000E09F0 File Offset: 0x000DEBF0
	// (set) Token: 0x06002317 RID: 8983 RVA: 0x000E09F7 File Offset: 0x000DEBF7
	[ServerVar(Help = "Maximum initial bet per round")]
	public static int maxbet
	{
		get
		{
			return BlackjackMachine._maxbet;
		}
		set
		{
			BlackjackMachine._maxbet = Mathf.Clamp(value, 25, 1000000);
		}
	}

	// Token: 0x170002F0 RID: 752
	// (get) Token: 0x06002318 RID: 8984 RVA: 0x00006CA5 File Offset: 0x00004EA5
	protected override float MaxStorageInteractionDist
	{
		get
		{
			return 1f;
		}
	}

	// Token: 0x06002319 RID: 8985 RVA: 0x000E0A0B File Offset: 0x000DEC0B
	public override void InitShared()
	{
		base.InitShared();
		this.controller = (BlackjackController)base.GameController;
	}

	// Token: 0x0600231A RID: 8986 RVA: 0x000E0A24 File Offset: 0x000DEC24
	public override void Load(BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
	}

	// Token: 0x0600231B RID: 8987 RVA: 0x000E0A2D File Offset: 0x000DEC2D
	public override void PlayerStorageChanged()
	{
		base.PlayerStorageChanged();
		base.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
	}
}
