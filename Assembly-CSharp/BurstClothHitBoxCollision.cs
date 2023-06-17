using System;
using System.Collections.Generic;
using Facepunch.BurstCloth;
using UnityEngine;

// Token: 0x020000FA RID: 250
public class BurstClothHitBoxCollision : BurstCloth, IClientComponent, IPrefabPreProcess
{
	// Token: 0x04000DC0 RID: 3520
	[Header("Rust Wearable BurstCloth")]
	public bool UseLocalGravity = true;

	// Token: 0x04000DC1 RID: 3521
	public float GravityStrength = 0.8f;

	// Token: 0x04000DC2 RID: 3522
	public float DefaultLength = 1f;

	// Token: 0x04000DC3 RID: 3523
	public float MountedLengthMultiplier;

	// Token: 0x04000DC4 RID: 3524
	public float DuckedLengthMultiplier = 0.5f;

	// Token: 0x04000DC5 RID: 3525
	public float CorpseLengthMultiplier = 0.2f;

	// Token: 0x04000DC6 RID: 3526
	public Transform UpAxis;

	// Token: 0x04000DC7 RID: 3527
	[Header("Collision")]
	public Transform ColliderRoot;

	// Token: 0x04000DC8 RID: 3528
	[Tooltip("Keywords in bone names which should be ignored for collision")]
	public string[] IgnoreKeywords;

	// Token: 0x06001576 RID: 5494 RVA: 0x000063A5 File Offset: 0x000045A5
	protected override void GatherColliders(List<CapsuleParams> colliders)
	{
	}

	// Token: 0x06001577 RID: 5495 RVA: 0x000063A5 File Offset: 0x000045A5
	public void PreProcess(IPrefabProcessor preProcess, GameObject rootObj, string name, bool serverside, bool clientside, bool bundling)
	{
	}
}
