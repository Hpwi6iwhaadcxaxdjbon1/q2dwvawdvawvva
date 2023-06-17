using System;
using UnityEngine;

// Token: 0x02000149 RID: 329
public class BossFormController : ArcadeEntityController
{
	// Token: 0x04000F8C RID: 3980
	public float animationSpeed = 0.5f;

	// Token: 0x04000F8D RID: 3981
	public Sprite[] animationFrames;

	// Token: 0x04000F8E RID: 3982
	public Vector2 roamDistance;

	// Token: 0x04000F8F RID: 3983
	public Transform colliderParent;

	// Token: 0x04000F90 RID: 3984
	public BossFormController.BossDamagePoint[] damagePoints;

	// Token: 0x04000F91 RID: 3985
	public ArcadeEntityController flashController;

	// Token: 0x04000F92 RID: 3986
	public float health = 50f;

	// Token: 0x02000C27 RID: 3111
	[Serializable]
	public class BossDamagePoint
	{
		// Token: 0x0400424C RID: 16972
		public BoxCollider hitBox;

		// Token: 0x0400424D RID: 16973
		public float health;

		// Token: 0x0400424E RID: 16974
		public ArcadeEntityController damagePrefab;

		// Token: 0x0400424F RID: 16975
		public ArcadeEntityController damageInstance;

		// Token: 0x04004250 RID: 16976
		public bool destroyed;
	}
}
