using System;
using UnityEngine;

// Token: 0x0200014A RID: 330
public class ChippyArcadeGame : BaseArcadeGame
{
	// Token: 0x04000F93 RID: 3987
	public ChippyMainCharacter mainChar;

	// Token: 0x04000F94 RID: 3988
	public SpriteArcadeEntity mainCharAim;

	// Token: 0x04000F95 RID: 3989
	public ChippyBoss currentBoss;

	// Token: 0x04000F96 RID: 3990
	public ChippyBoss[] bossPrefabs;

	// Token: 0x04000F97 RID: 3991
	public SpriteArcadeEntity mainMenuLogo;

	// Token: 0x04000F98 RID: 3992
	public Transform respawnPoint;

	// Token: 0x04000F99 RID: 3993
	public Vector2 mouseAim = new Vector2(0f, 1f);

	// Token: 0x04000F9A RID: 3994
	public TextArcadeEntity levelIndicator;

	// Token: 0x04000F9B RID: 3995
	public TextArcadeEntity gameOverIndicator;

	// Token: 0x04000F9C RID: 3996
	public TextArcadeEntity playGameButton;

	// Token: 0x04000F9D RID: 3997
	public TextArcadeEntity highScoresButton;

	// Token: 0x04000F9E RID: 3998
	public bool OnMainMenu;

	// Token: 0x04000F9F RID: 3999
	public bool GameActive;

	// Token: 0x04000FA0 RID: 4000
	public int level;

	// Token: 0x04000FA1 RID: 4001
	public TextArcadeEntity[] scoreDisplays;

	// Token: 0x04000FA2 RID: 4002
	public MenuButtonArcadeEntity[] mainMenuButtons;

	// Token: 0x04000FA3 RID: 4003
	public int selectedButtonIndex;

	// Token: 0x04000FA4 RID: 4004
	public bool OnHighScores;
}
