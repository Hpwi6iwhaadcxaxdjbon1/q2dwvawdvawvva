using System;
using UnityEngine;

// Token: 0x0200014D RID: 333
public class ChippyMainCharacter : SpriteArcadeEntity
{
	// Token: 0x04000FB0 RID: 4016
	public float speed;

	// Token: 0x04000FB1 RID: 4017
	public float maxSpeed = 0.25f;

	// Token: 0x04000FB2 RID: 4018
	public ChippyBulletEntity bulletPrefab;

	// Token: 0x04000FB3 RID: 4019
	public float fireRate = 0.1f;

	// Token: 0x04000FB4 RID: 4020
	public Vector3 aimDir = Vector3.up;
}
