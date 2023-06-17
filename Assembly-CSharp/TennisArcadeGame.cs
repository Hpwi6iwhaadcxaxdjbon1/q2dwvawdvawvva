using System;
using UnityEngine;

// Token: 0x02000153 RID: 339
public class TennisArcadeGame : BaseArcadeGame
{
	// Token: 0x04000FBF RID: 4031
	public ArcadeEntity paddle1;

	// Token: 0x04000FC0 RID: 4032
	public ArcadeEntity paddle2;

	// Token: 0x04000FC1 RID: 4033
	public ArcadeEntity ball;

	// Token: 0x04000FC2 RID: 4034
	public Transform paddle1Origin;

	// Token: 0x04000FC3 RID: 4035
	public Transform paddle2Origin;

	// Token: 0x04000FC4 RID: 4036
	public Transform paddle1Goal;

	// Token: 0x04000FC5 RID: 4037
	public Transform paddle2Goal;

	// Token: 0x04000FC6 RID: 4038
	public Transform ballSpawn;

	// Token: 0x04000FC7 RID: 4039
	public float maxScore = 5f;

	// Token: 0x04000FC8 RID: 4040
	public ArcadeEntity[] paddle1ScoreNodes;

	// Token: 0x04000FC9 RID: 4041
	public ArcadeEntity[] paddle2ScoreNodes;

	// Token: 0x04000FCA RID: 4042
	public int paddle1Score;

	// Token: 0x04000FCB RID: 4043
	public int paddle2Score;

	// Token: 0x04000FCC RID: 4044
	public float sensitivity = 1f;

	// Token: 0x04000FCD RID: 4045
	public ArcadeEntity logo;

	// Token: 0x04000FCE RID: 4046
	public bool OnMainMenu;

	// Token: 0x04000FCF RID: 4047
	public bool GameActive;
}
