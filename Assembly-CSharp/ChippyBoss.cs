using System;
using UnityEngine;

// Token: 0x0200014B RID: 331
public class ChippyBoss : SpriteArcadeEntity
{
	// Token: 0x04000FA5 RID: 4005
	public Vector2 roamDistance;

	// Token: 0x04000FA6 RID: 4006
	public float animationSpeed = 0.5f;

	// Token: 0x04000FA7 RID: 4007
	public Sprite[] animationFrames;

	// Token: 0x04000FA8 RID: 4008
	public ArcadeEntity bulletTest;

	// Token: 0x04000FA9 RID: 4009
	public SpriteRenderer flashRenderer;

	// Token: 0x04000FAA RID: 4010
	public ChippyBoss.BossDamagePoint[] damagePoints;

	// Token: 0x02000C28 RID: 3112
	[Serializable]
	public class BossDamagePoint
	{
		// Token: 0x04004251 RID: 16977
		public BoxCollider hitBox;

		// Token: 0x04004252 RID: 16978
		public float health;

		// Token: 0x04004253 RID: 16979
		public ArcadeEntityController damagePrefab;

		// Token: 0x04004254 RID: 16980
		public ArcadeEntityController damageInstance;

		// Token: 0x04004255 RID: 16981
		public bool destroyed;
	}
}
