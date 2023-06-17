using System;
using UnityEngine;

// Token: 0x02000341 RID: 833
public class FirstPersonEffect : MonoBehaviour, IEffect
{
	// Token: 0x04001854 RID: 6228
	public bool isGunShot;

	// Token: 0x04001855 RID: 6229
	[HideInInspector]
	public EffectParentToWeaponBone parentToWeaponComponent;
}
