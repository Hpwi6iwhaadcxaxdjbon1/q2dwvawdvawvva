using System;
using UnityEngine;

// Token: 0x0200049A RID: 1178
[Serializable]
public class ModularCarCodeLockVisuals : MonoBehaviour
{
	// Token: 0x04001EF1 RID: 7921
	[SerializeField]
	private GameObject lockedVisuals;

	// Token: 0x04001EF2 RID: 7922
	[SerializeField]
	private GameObject unlockedVisuals;

	// Token: 0x04001EF3 RID: 7923
	[SerializeField]
	private GameObject blockedVisuals;

	// Token: 0x04001EF4 RID: 7924
	[SerializeField]
	private GameObjectRef codelockEffectDenied;

	// Token: 0x04001EF5 RID: 7925
	[SerializeField]
	private GameObjectRef codelockEffectShock;

	// Token: 0x04001EF6 RID: 7926
	[SerializeField]
	private float xOffset = 0.91f;

	// Token: 0x04001EF7 RID: 7927
	[SerializeField]
	private ParticleSystemContainer keycodeDestroyableFX;
}
